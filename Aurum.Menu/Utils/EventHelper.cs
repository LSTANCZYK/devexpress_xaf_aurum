using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu.Utils
{
    /// <summary>
    /// EventHelper by Hedley Muscroft
    /// </summary>
    /// <see cref="http://www.codeproject.com/Articles/103542/Removing-Event-Handlers-using-Reflection"/>
    static public class EventHelper
    {
        static Dictionary<Type, Dictionary<string, FieldInfo>> dicEventFieldInfos = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        static BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }

        //--------------------------------------------------------------------------------
        static FieldInfo GetTypeEventFields(Type t, string eventName)
        {
            Dictionary<string, FieldInfo> typeEvents;
            if (dicEventFieldInfos.ContainsKey(t))
                typeEvents = dicEventFieldInfos[t];
            else
            {
                typeEvents = new Dictionary<string, FieldInfo>();
                dicEventFieldInfos.Add(t, typeEvents);
            }

            if (typeEvents.ContainsKey(eventName))
                return typeEvents[eventName];

            FieldInfo e = BuildEventFields(t, eventName);
            typeEvents.Add(eventName, e);
            return e;
        }

        //--------------------------------------------------------------------------------
        static FieldInfo BuildEventFields(Type t, string eventName)
        {
            // Type.GetEvent(s) gets all Events for the type AND it's ancestors
            // Type.GetField(s) gets only Fields for the exact type.
            //  (BindingFlags.FlattenHierarchy only works on PROTECTED & PUBLIC
            //   doesn't work because Fieds are PRIVATE)

            // NEW version of this routine uses .GetEvents and then uses .DeclaringType
            // to get the correct ancestor type so that we can get the FieldInfo.
            foreach (EventInfo ei in t.GetEvents(AllBindings))
            {
                if (ei.Name == eventName)
                {
                    Type dt = ei.DeclaringType;
                    FieldInfo fi = dt.GetField(ei.Name, AllBindings);
                    return fi;
                }
            }
            return null;

            // OLD version of the code - called itself recursively to get all fields
            // for 't' and ancestors and then tested each one to see if it's an EVENT
            // Much less efficient than the new code
            /*
                  foreach (FieldInfo fi in t.GetFields(AllBindings))
                  {
                    EventInfo ei = t.GetEvent(fi.Name, AllBindings);
                    if (ei != null)
                    {
                      lst.Add(fi);
                      Console.WriteLine(ei.Name);
                    }
                  }
                  if (t.BaseType != null)
                    BuildEventFields(t.BaseType, lst);*/
        }

        //--------------------------------------------------------------------------------
        static EventHandlerList GetStaticEventHandlerList(Type t, object obj)
        {
            MethodInfo mi = t.GetMethod("get_Events", AllBindings);
            return (EventHandlerList)mi.Invoke(obj, new object[] { });
        }

        //--------------------------------------------------------------------------------
        public static void RemoveEventHandler(object obj, string eventName, Func<Delegate, bool> pred = null)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();
            FieldInfo fi = GetTypeEventFields(t, eventName);
            EventHandlerList static_event_handlers = null;

            if (fi != null)
            {
                // After hours and hours of research and trial and error, it turns out that
                // STATIC Events have to be treated differently from INSTANCE Events...
                if (fi.IsStatic)
                {
                    // STATIC EVENT
                    if (static_event_handlers == null)
                        static_event_handlers = GetStaticEventHandlerList(t, obj);

                    object idx = fi.GetValue(obj);
                    Delegate eh = static_event_handlers[idx];
                    if (eh == null)
                        return;

                    Delegate[] dels = eh.GetInvocationList();
                    if (dels == null)
                        return;

                    EventInfo ei = t.GetEvent(fi.Name, AllBindings);
                    foreach (Delegate del in dels.Where(pred))
                        ei.RemoveEventHandler(obj, del);
                }
                else
                {
                    // INSTANCE EVENT
                    EventInfo ei = t.GetEvent(fi.Name, AllBindings);
                    if (ei != null)
                    {
                        object val = fi.GetValue(obj);
                        Delegate mdel = (val as Delegate);
                        if (mdel != null)
                        {
                            foreach (Delegate del in mdel.GetInvocationList().Where(pred))
                                ei.RemoveEventHandler(obj, del);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Can't find event '" + eventName + "' in '" + Convert.ToString(obj) + "'");
            }
        }

        //--------------------------------------------------------------------------------
    }
}
