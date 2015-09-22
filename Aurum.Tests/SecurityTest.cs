using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;
using Aurum.Xpo;

namespace Aurum.Tests
{
    /// <summary>
    /// Тестирование операций администрирования безопасности базы данных
    /// </summary>
    /// <remarks>Работает с уже созданной структурой данных <see cref="AutoScriptTest"/></remarks>
    [TestClass]
    public class SecurityTest
    {
        [ClassInitialize]
        public static void TestsInitialize(TestContext testContext)
        {
            ReflectionDictionary dictionary = new ReflectionDictionary();
            XPDictionaryInformer.Register(dictionary);
            OracleConnectionProviderEx provider = new OracleConnectionProviderEx(
                ODPConnectionProvider.CreateConnection(Properties.Settings.Default.Connection), AutoCreateOption.SchemaAlreadyExists);
            XpoDefault.DataLayer = new SimpleDataLayerEx(dictionary, provider);
        }

        public static void AdminSecurity(params SecurityStatement[] statements)
        {
            ((IDataLayerSecurity)XpoDefault.DataLayer).AdminSecurity(statements);
        }

        /// <summary>
        /// Создание и удаление роли
        /// </summary>
        [TestMethod]
        public void TestSimpleRole()
        {
            SecurityObject role2876 = new SecurityObject(SecurityObjectTypes.Role, "ROLE_2876");
            AdminSecurity(new SecurityStatement(AdminSecurityOperations.Create, role2876));
            string role = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select role from dba_roles where role = '{0}'", role2876.ObjectName));
            Assert.AreEqual(role2876.ObjectName, role, "Role is not created");
            
            AdminSecurity(new SecurityStatement(AdminSecurityOperations.Drop, role2876));
            role = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select role from dba_roles where role = '{0}'", role2876.ObjectName));
            Assert.AreEqual(null, role, "Role is not droped");
        }

        /// <summary>
        /// Создание и удаление пользователя
        /// </summary>
        [TestMethod]
        public void TestSimpleUser()
        {
            SecurityObject user2876 = new SecurityObject(SecurityObjectTypes.User, "USER_2876");
            AdminSecurity(new SecurityStatement(AdminSecurityOperations.Create, user2876));
            string user = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select username from dba_users where username = '{0}'", user2876.ObjectName));
            Assert.AreEqual(user2876.ObjectName, user, "User is not created");

            AdminSecurity(new SecurityStatement(AdminSecurityOperations.Drop, user2876));
            user = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select username from dba_users where username = '{0}'", user2876.ObjectName));
            Assert.AreEqual(null, user, "User is not droped");
        }

        /// <summary>
        /// Назначение и отзыв прав на объекты
        /// </summary>
        [TestMethod]
        public void TestGrant()
        {
            SecurityObject role2877 = new SecurityObject(SecurityObjectTypes.Role, "ROLE_2877");
            SecurityObject user2877 = new SecurityObject(SecurityObjectTypes.User, "USER_2877");
            SecurityObject tableAutoId = new SecurityObject("TESTXPO.AUTOIDCLASS", SecurityTableRights.Select | SecurityTableRights.Update);
            AdminSecurity(
                new SecurityStatement(AdminSecurityOperations.Create, role2877),
                new SecurityStatement(AdminSecurityOperations.Create, user2877),
                new SecurityStatement(AdminSecurityOperations.GrantTo, role2877, user2877),
                new SecurityStatement(AdminSecurityOperations.GrantTo, tableAutoId, role2877));
            string userGrant = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select granted_role from dba_role_privs where grantee = '{0}'", user2877.ObjectName));
            int roleGrants = Convert.ToInt32(XpoDefault.Session.ExecuteScalar(
                string.Format("select count(*) from dba_tab_privs where grantee = '{0}'", role2877.ObjectName)));
            Assert.AreEqual(role2877.ObjectName, userGrant, "Role is not granted");
            Assert.AreEqual(2, roleGrants, "Table privileges are not granted completely");

            AdminSecurity(
                new SecurityStatement(AdminSecurityOperations.RevokeFrom, role2877, user2877),
                new SecurityStatement(AdminSecurityOperations.RevokeFrom, tableAutoId, role2877));
            userGrant = (string)XpoDefault.Session.ExecuteScalar(
                string.Format("select granted_role from dba_role_privs where grantee = '{0}'", user2877.ObjectName));
            roleGrants = Convert.ToInt32(XpoDefault.Session.ExecuteScalar(
                string.Format("select count(*) from dba_tab_privs where grantee = '{0}'", role2877.ObjectName)));
            Assert.AreEqual(null, userGrant, "Role is not revoked");
            Assert.AreEqual(0, roleGrants, "Table privileges are not revoked completely");

            AdminSecurity(
                new SecurityStatement(AdminSecurityOperations.Drop, role2877),
                new SecurityStatement(AdminSecurityOperations.Drop, user2877));
        }
    }
}
