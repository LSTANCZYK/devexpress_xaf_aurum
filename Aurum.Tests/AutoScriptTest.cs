using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using Aurum.Xpo;

namespace Aurum.Tests
{
    /// <summary>
    /// Тестирование автоскрипта обновления структуры БД
    /// </summary>
    [TestClass]
    public class AutoScriptTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [ClassInitialize]
        public static void TestsInitialize(TestContext testContext) 
        {
            Connection.Initialize(Properties.Settings.Default.ConnectionAdmin);
            Connection.ExecuteFile(@"..\..\..\Aurum.Tests\testxpo.sql");
        }

        [ClassCleanup]
        public static void TestsCleanup()
        {
            Connection.Cleanup();
        }

        /// <summary>
        /// Тест автоматического скрипта
        /// </summary>
        [TestMethod]
        public void AutoScript()
        {
            // Создание скрипта
            const string path = "autoscript.txt";
            StreamWriter script = new StreamWriter(path);
            ReflectionDictionary dictionary = new ReflectionDictionary();
            XPDictionaryInformer.Register(dictionary);
            SimpleDataLayer dataLayer = new SimpleDataLayer(dictionary, new OracleConnectionProviderEx(
                ODPConnectionProvider.CreateConnection(Properties.Settings.Default.ConnectionAdmin), AutoCreateOption.SchemaOnly, script, UpdateSchemaOptions.Default));
            dataLayer.UpdateSchema(false, new Type[] { 
                typeof(AutoIdClass), typeof(StringIdClass), typeof(ComplexIdClass), typeof(NoIdClass), 
                typeof(NotNullClass), typeof(ReadOnlyClass), 
                typeof(ConsistentPeriodClass), typeof(ConsistentPeriodClass2), typeof(ContinuousPeriodClass), typeof(HierarchyClass), 
                typeof(SimpleConstraintClass), typeof(ConstraintClass), typeof(CategoryClass), typeof(SecurityClass), 
                typeof(UniqueBaseClass), typeof(UniqueClass),
                typeof(DevExpress.Xpo.XPObjectType)
            }.Select(c => dictionary.GetClassInfo(c)).ToArray());
            script.Close();

            // Проверка непустого содержания файла
            FileStream file = new FileStream(path, FileMode.Open);
            Assert.IsFalse(file.Length == 0, "File of autoscript is empty.");
            file.Close();

            // Проверка валидности скрипта
            Connection.ExecuteFile(path);
            dataLayer.Dispose();
        }
    }
}
