using Aurum.Exchange;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Audit
{
    /// <summary>
    /// Аудит обмена данными
    /// </summary>
    [OptimisticLocking(false)]
    [DeferredDeletion(false)]
    public class ExchangeTrail : XPObject
    {
        private DateTime time;
        private string exchangeId;
        private string parameters;
        private string clientname;
        private string username;
        private string osUser;

        public ExchangeTrail() : base() { }
        public ExchangeTrail(Session session) : base(session) { }

        /// <summary>
        /// Время
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { SetPropertyValue("Time", ref time, value); }
        }

        /// <summary>
        /// Идентификатор обмена данными
        /// </summary>
        [Size(100)]
        public string ExchangeId
        {
            get { return exchangeId; }
            set { SetPropertyValue("ExchangeId", ref exchangeId, value); }
        }

        /// <summary>
        /// Параметры
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        public string Parameters
        {
            get { return parameters; }
            set { SetPropertyValue("Parameters", ref parameters, value); }
        }

        /// <summary>
        /// Название клиентской машины
        /// </summary>
        [Size(100)]
        public string Clientname
        {
            get { return clientname; }
            set { SetPropertyValue("Clientname", ref clientname, value); }
        }

        /// <summary>
        /// Имя пользователя (логин)
        /// </summary>
        [Size(100)]
        public string Username
        {
            get { return username; }
            set { SetPropertyValue("Username", ref username, value); }
        }

        /// <summary>
        /// Пользователь ОС
        /// </summary>
        [Size(100)]
        public string OsUser
        {
            get { return osUser; }
            set { SetPropertyValue("OsUser", ref osUser, value); }
        }

        public static void LogOperation(IObjectSpace objectSpace, ExchangeOperation export)
        {
            if (export == null) return;
            var trail = objectSpace.CreateObject<Audit.ExchangeTrail>();
            trail.Time = DateTime.Now;
            trail.ExchangeId = export.GetType().FullName;
            trail.Clientname = GetComputerName();
            trail.Username = SecuritySystem.CurrentUserName;
            trail.OsUser = GetLoginName();
            trail.Parameters = ConvertParamsToString((XPObjectSpace)objectSpace, export.ParametersObject);
            objectSpace.CommitChanges();
        }

        private static string ConvertParamsToString(XPObjectSpace objectSpace, ExchangeParameters p)
        {
            if (p == null) return string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                var props = p.GetType().GetProperties();
                if (props != null)
                {
                    foreach (var prop in props)
                    {
                        if (prop.Name == "ObjectSpace") continue;
                        if (!prop.CanWrite) continue;
                        if (PropertyAttribute<System.ComponentModel.BrowsableAttribute>(prop, a => !a.Browsable)) continue;
                        if (PropertyAttribute<DevExpress.Persistent.Base.VisibleInDetailViewAttribute>(prop, a => !(bool)a.Value)) continue;

                        var propValue = prop.GetValue(p);
                        string s = GetParameterString(objectSpace, propValue);
                        sb.Append(prop.Name + "=" + PrepareForCsvExport(s));
                        sb.Append(";");
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static bool PropertyAttribute<T>(PropertyInfo prop, Predicate<T> predicate) where T : System.Attribute
        {
            var attr = prop.GetCustomAttribute<T>();
            return attr != null ? predicate(attr) : false;
        }

        private static string GetParameterString(XPObjectSpace objectSpace, object o)
        {
            if (o == null) return "null";

            if (o is string) return (string)o;
            if (o is int ||
                o is int? ||
                o is bool ||
                o is bool? ||
                o is decimal ||
                o is decimal? ||
                o is DateTime ||
                o is DateTime? ||
                o is short ||
                o is short? ||
                o is long ||
                o is long? ||
                o is float ||
                o is float? ||
                o is double ||
                o is double? ||
                o is char ||
                o is char?
                ) return Convert.ToString(o);

            string s = Convert.ToString(o);
            try
            {
                if (o is IEnumerable)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    bool first = true;
                    XPClassInfo elemClassInfo = null;
                    foreach (var e in (IEnumerable)o)
                    {
                        if (first)
                        {
                            try
                            {
                                elemClassInfo = objectSpace.Session.GetClassInfo(e);
                            }
                            catch { }
                            first = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        if (elemClassInfo != null)
                        {
                            sb.Append(Convert.ToString(elemClassInfo.KeyProperty.GetValue(e)));
                        }
                        else
                        {
                            sb.Append(Convert.ToString(e));
                        }
                    }
                    sb.Append("}");
                    return sb.ToString();
                }
                else
                {
                    XPClassInfo classInfo = objectSpace.Session.GetClassInfo(o);
                    string id = Convert.ToString(classInfo.KeyProperty.GetValue(o));
                    return s + " (id=" + id + ")";
                }
            }
            catch
            {
            }
            return s;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static string PrepareForCsvExport(string s)
        {
            if (!string.IsNullOrEmpty(s)
                        && (s.Contains(";") || s.Contains(",") || s.Contains(" ") || s.Contains("\n")))
            {
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            }
            else
            {
                return s;
            }
        }

        private static string GetComputerName()
        {
            return WTSApi.GetWTSClientName() ?? WTSApi.GetComputerName();
        }

        private static string GetLoginName()
        {
            return Environment.UserName;
        }

        #region WTSApi

        /// <summary>
        /// WTS Api
        /// </summary>
        internal class WTSApi
        {
            enum WTSInfoClass
            {
                WTSInitialProgram,
                WTSApplicationName,
                WTSWorkingDirectory,
                WTSOEMId,
                WTSSessionId,
                WTSUserName,
                WTSWinStationName,
                WTSDomainName,
                WTSConnectState,
                WTSClientBuildNumber,
                WTSClientName,
                WTSClientDirectory,
                WTSClientProductId,
                WTSClientHardwareId,
                WTSClientAddress,
                WTSClientDisplay,
                WTSClientProtocolType,
                WTSIdleTime,
                WTSLogonTime,
                WTSIncomingBytes,
                WTSOutgoingBytes,
                WTSIncomingFrames,
                WTSOutgoingFrames,
                WTSClientInfo,
                WTSSessionInfo
            }
            const int WTS_CURRENT_SESSION = -1;

            [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
            static extern void WTSFreeMemory(IntPtr memory);

            [DllImport("wtsapi32.dll", EntryPoint = "WTSQuerySessionInformation", SetLastError = true)]
            static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass,
                out IntPtr ppBuffer, out uint pBytesReturned);

            [DllImport("kernel32.dll", EntryPoint = "GetComputerName", SetLastError = true)]
            static extern bool GetComputerName([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpBuffer, ref int nSize);

            /// <summary>
            /// Получить клиентское имя сессии
            /// </summary>
            /// <returns>Клиентское имя сессии</returns>
            public static string GetWTSClientName()
            {
                IntPtr buffer = IntPtr.Zero;
                uint len;
                try
                {
                    if (WTSQuerySessionInformation(IntPtr.Zero, WTS_CURRENT_SESSION,
                        WTSInfoClass.WTSClientName, out buffer, out len))
                    {
                        string res = Marshal.PtrToStringAnsi(buffer, (int)len - 1).Trim();
                        return string.IsNullOrEmpty(res) ? null : res;
                    }
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    WTSFreeMemory(buffer);
                    buffer = IntPtr.Zero;
                }
            }


            /// <summary>
            /// Получить локальное имя компьютера
            /// </summary>
            /// <returns>Локальное имя компьютера</returns>
            public static string GetComputerName()
            {
                string StringBuffer = new string(new char(), 512);
                int StringBuffer_Size = StringBuffer.Length;
                if (GetComputerName(ref StringBuffer, ref StringBuffer_Size))
                    return StringBuffer.Substring(0, StringBuffer_Size);

                return string.Empty;
            }
        }

        #endregion WTSApi
    }
}
