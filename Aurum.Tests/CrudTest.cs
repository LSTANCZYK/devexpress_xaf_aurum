using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
    /// Тестирование операций выборки и модификации данных
    /// </summary>
    /// <remarks>Работает с уже созданной структурой данных <see cref="AutoScriptTest"/></remarks>
    [TestClass]
    public class CrudTest
    {
        [ClassInitialize]
        public static void TestsInitialize(TestContext testContext)
        {
            ReflectionDictionary dictionary = new ReflectionDictionary();
            XPDictionaryInformer.Register(dictionary);
            OracleConnectionProviderEx.Register();
            IDataStore provider = XpoDefault.GetConnectionProvider(Properties.Settings.Default.Connection, AutoCreateOption.SchemaAlreadyExists);
            XpoDefault.DataLayer = new SimpleDataLayer(dictionary, provider);
            
            // Очистка данных таблиц
            XpoDefault.Session.ExecuteNonQuery("alter table testxpo.consistentperiodclass2 disable constraint FK_CONSISTENTPERI_B734BF4F");
            XpoDefault.Session.ExecuteNonQuery("alter table testxpo.uniqueclass disable constraint FK_UNIQUECLASSID");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.autoidclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.stringidclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.complexidclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.noidclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.consistentperiodclass2");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.consistentperiodclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.continuousperiodclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.uniquebaseclass");
            XpoDefault.Session.ExecuteNonQuery("truncate table testxpo.uniqueclass");
            XpoDefault.Session.ExecuteNonQuery("alter table testxpo.consistentperiodclass2 enable constraint FK_CONSISTENTPERI_B734BF4F");
            XpoDefault.Session.ExecuteNonQuery("alter table testxpo.uniqueclass enable constraint FK_UNIQUECLASSID");
        }

        /// <summary>
        /// Тест AutoIdClass
        /// </summary>
        [TestMethod]
        public void TestAutoIdClass()
        {
            TestCrud<AutoIdClass>(i => new AutoIdClass());
        }

        /// <summary>
        /// Тест StringIdClass
        /// </summary>
        [TestMethod]
        public void TestStringIdClass()
        {
            TestCrud<StringIdClass>(i => new StringIdClass(i.ToString()));
        }

        /// <summary>
        /// Тест ComplexIdClass
        /// </summary>
        [TestMethod]
        public void TestComplexIdClass()
        {
            TestCrud<ComplexIdClass>(i => 
            {
                ComplexIdentifier id = new ComplexIdentifier();
                id.Part1 = i.ToString();
                id.Part2 = i;
                return new ComplexIdClass(id);
            });
        }

        /// <summary>
        /// Тест NoIdClass
        /// </summary>
        /// <remarks>Crud для класса без идентификатора в настоящее время не поддерживается</remarks>
        //[TestMethod]
        public void TestNoIdClass()
        {
            throw new NotSupportedException();
            //NoIdClass obj = new NoIdClass(Session.DefaultSession);
            //Session.DefaultSession.Save(obj);
            //TestCrud<NoIdClass>(i => new NoIdClass());
        }

        /// <summary>
        /// Базовое тестирование crud-функционала
        /// </summary>
        /// <typeparam name="T">Тип объектов</typeparam>
        /// <param name="creator">Делегатор создания</param>
        private void TestCrud<T>(CreateObjectDelegate creator) where T : XPBaseObject, ITestCrud
        {
            // Создание
            List<T> objects = new List<T>();
            for (int i = 1; i <= 5; i++)
            {
                T obj = (T)creator(i);
                obj.Field1 = i <= 3 ? "aaa" : "b" + i.ToString();
                objects.Add(obj);
                obj.Save();
            }
            T o = objects[1];
            o.Reload();
            Assert.AreEqual("aaa", o.Field1, "Field1 after insert");

            // Изменение
            o.Field1 = "b#";
            o.Save();
            o.Reload();
            Assert.AreEqual("b#", o.Field1, "Field1 after update");

            // Удаление
            objects[3].Delete();
            objects[4].Delete();

            // Выборка с помощью Linq
            XPQuery<T> query = Session.DefaultSession.Query<T>();
            var list = from c in query select c;
            Assert.AreEqual(3, list.Count(), "Records after delete");
        }

        delegate object CreateObjectDelegate(int i);

        /// <summary>
        /// Тест атрибута NotNull (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestNotNull()
        {
            NotNullClass o = new NotNullClass();
            o.Field1 = null;
            o.Field2NotNull = "not null";
            o.Save();
        }

        /// <summary>
        /// Тест атрибута NotNull (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(1400)]
        public void TestNotNullException()
        {
            NotNullClass o = new NotNullClass();
            o.Field1 = null;
            o.Field2NotNull = null;
            o.Save();
        }

        /// <summary>
        /// Тест атрибута ReadOnly (ожидаемое исключение)
        /// </summary>
        [TestMethod]
        [ExpectedOracleConnectionProviderException]
        public void TestReadOnly()
        {
            ReadOnlyClass o = new ReadOnlyClass();
            o.Field1 = "123";
            o.Field2ReadOnly = "456";
            o.Save();
            o.Reload();

            Assert.AreEqual("123", o.Field1, "Field1 after insert");
            Assert.AreEqual("456", o.Field2ReadOnly, "Field2ReadOnly after insert");

            o.Field1 = "abc";
            o.Field2ReadOnly = "def"; 
            o.Save(); // expected exception
            o.Reload();

            Assert.AreEqual("abc", o.Field1, "Field1 after update");
            Assert.AreEqual("456", o.Field2ReadOnly, "Field2ReadOnly after update");
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestConsistentPeriod()
        {
            // [01.01.2001-01.12.2001)  [08.12.2001-01.01.2002)[01.01.2002-01.02.2002) 
            ConsistentPeriodClass o1 = new ConsistentPeriodClass(1, 1, new DateTime(2001, 1, 1), new DateTime(2001, 12, 1));
            ConsistentPeriodClass o2 = new ConsistentPeriodClass(1, 1, new DateTime(2002, 1, 1), new DateTime(2002, 2, 1));
            ConsistentPeriodClass o3 = new ConsistentPeriodClass(1, 1, new DateTime(2001, 12, 8), new DateTime(2002, 1, 1));
            o1.Save(); o2.Save(); o3.Save();

            o3.PeriodKey1 = 2;
            o3.DateOut = null;
            o3.Save();

            o1.DateOut = new DateTime(2002, 1, 1);
            o1.Save();
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestConsistentPeriodException()
        {
            // [01.01.2001-01.12.2001)                       [01.01.2002-01.02.2002) 
            //                          [08.12.2001-02.01.2002)
            ConsistentPeriodClass o1 = new ConsistentPeriodClass(1, 2, new DateTime(2001, 1, 1), new DateTime(2001, 12, 1));
            ConsistentPeriodClass o2 = new ConsistentPeriodClass(1, 2, new DateTime(2002, 1, 1), new DateTime(2002, 2, 1));
            ConsistentPeriodClass o3 = new ConsistentPeriodClass(2, 2, new DateTime(2001, 12, 8), new DateTime(2002, 1, 2));
            o1.Save(); o2.Save(); o3.Save();

            o3.PeriodKey1 = 1;
            o3.Save();
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod для наследованного класса (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestConsistentPeriod2()
        {
            // [01.01.2001-01.12.2001)  [08.12.2001-01.01.2002)[01.01.2002-01.02.2002) 
            ConsistentPeriodClass2 o1 = new ConsistentPeriodClass2(1, 10, 3, new DateTime(2001, 1, 1), new DateTime(2001, 12, 1));
            ConsistentPeriodClass2 o2 = new ConsistentPeriodClass2(2, 10, 3, new DateTime(2002, 1, 1), new DateTime(2002, 2, 1));
            ConsistentPeriodClass2 o3 = new ConsistentPeriodClass2(3, 10, 3, new DateTime(2001, 12, 8), new DateTime(2002, 1, 1));
            o1.Save(); o2.Save(); o3.Save();

            o3.PeriodKey3 = 5;
            o3.DateOut = null;
            o3.Save();

            o1.DateOut = new DateTime(2002, 1, 1);
            o1.Save();
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod для наследованного класса (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestConsistentPeriod2Exception()
        {
            // [01.01.2001-01.12.2001)                       [01.01.2002-01.02.2002) 
            //                          [08.12.2001-02.01.2002)
            ConsistentPeriodClass2 o1 = new ConsistentPeriodClass2(1, 20, 8, new DateTime(2001, 1, 1), new DateTime(2001, 12, 1));
            ConsistentPeriodClass2 o2 = new ConsistentPeriodClass2(1, 20, 8, new DateTime(2002, 1, 1), new DateTime(2002, 2, 1));
            ConsistentPeriodClass2 o3 = new ConsistentPeriodClass2(2, 20, 9, new DateTime(2001, 12, 8), new DateTime(2002, 1, 2));
            o1.Save(); o2.Save(); o3.Save();

            o3.PeriodKey3 = 8;
            o3.Save();
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod с непрерывностью периодов (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestContinuousPeriod()
        {
            // [01.08.2000-01.01.2001)[01.01.2001-01.01.2002)[01.01.2002-15.01.2002)
            ContinuousPeriodClass o1 = new ContinuousPeriodClass("a", new DateTime(2000, 8, 1), new DateTime(2001, 1, 1));
            ContinuousPeriodClass o2 = new ContinuousPeriodClass("a", new DateTime(2001, 1, 1), new DateTime(2002, 1, 1));
            ContinuousPeriodClass o3 = new ContinuousPeriodClass("a", new DateTime(2002, 1, 1), new DateTime(2002, 1, 15));
            o2.Save(); o1.Save(); o3.Save();

            o1.DateIn = new DateTime(2000, 7, 20);
            o3.DateOut = new DateTime(2002, 1, 25);
            o1.Save(); o3.Save();

            o3.Delete();
            o2.Delete();
        }

        /// <summary>
        /// Тест атрибута ConsistentPeriod с непрерывностью периодов (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestContinuousPeriodException()
        {
            // [01.08.2000-01.01.2001)[01.01.2001-01.01.2002)[01.01.2002-15.01.2002)
            ContinuousPeriodClass o1 = new ContinuousPeriodClass("b", new DateTime(2000, 8, 1), new DateTime(2001, 1, 1));
            ContinuousPeriodClass o2 = new ContinuousPeriodClass("b", new DateTime(2001, 1, 1), new DateTime(2002, 1, 1));
            ContinuousPeriodClass o3 = new ContinuousPeriodClass("b", new DateTime(2002, 1, 1), new DateTime(2002, 1, 15));
            o2.Save(); o1.Save(); o3.Save();

            o2.PeriodKey = "c";
            o2.Save();
        }

        /// <summary>
        /// Тест атрибута Hierarchy (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestHierarchy()
        {
            HierarchyClass o1 = new HierarchyClass(null, "1");      // 1-------
            HierarchyClass o2 = new HierarchyClass(o1, "2");        //   |2   |3---
            HierarchyClass o3 = new HierarchyClass(o1, "3");        //            |4
            HierarchyClass o4 = new HierarchyClass(o3, "4");
            o1.Save(); o2.Save(); o3.Save(); o4.Save();

            o3.Parent = o2;
            o3.Save();
        }

        /// <summary>
        /// Тест атрибута Hierarchy (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestHierarchyException()
        {
            HierarchyClass o1 = new HierarchyClass(null, "1");      // 1--
            HierarchyClass o2 = new HierarchyClass(o1, "2");        //   |2--
            HierarchyClass o3 = new HierarchyClass(o2, "3");        //      |3--
            HierarchyClass o4 = new HierarchyClass(o3, "4");        //         |4
            o1.Save(); o2.Save(); o3.Save(); o4.Save();

            o2.Parent = o4;
            o2.Save();
        }

        /// <summary>
        /// Тест атрибута Unique (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestUnique()
        {
            UniqueClass o1 = new UniqueClass(1, 2);
            UniqueClass o2 = new UniqueClass(1, null);
            UniqueClass o3 = new UniqueClass(2, null);
            UniqueClass o4 = new UniqueClass(2, 2);
            o1.Save(); o2.Save(); o3.Save(); o4.Save();

            o3.Int = 1;
            o3.Int2 = 1;
            o3.Save();
        }

        /// <summary>
        ///  Тест атрибута Unique (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestUniqueException()
        {
            UniqueClass o1 = new UniqueClass(5, 2);
            UniqueClass o2 = new UniqueClass(5, null);
            UniqueClass o3 = new UniqueClass(5, null);
            o1.Save(); o2.Save(); o3.Save(); 
        }

        /// <summary>
        /// Тест атрибута Constraint с простыми выражениями (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestSimpleConstraint()
        {
            SimpleConstraintClass o = new SimpleConstraintClass();
            o.Value = 5; o.State = "State1";
            o.Save();

            o.Value = 0; o.State = "Default"; o.Date1 = new DateTime(2011, 11, 1);
            o.Save();

            o.State = "State2";
            o.Save();
        }

        /// <summary>
        /// Тест атрибута Constraint с простыми выражениями (ожидаемое исключение 1)
        /// </summary>
        [TestMethod, ExpectedOracleException(2290)]
        public void TestSimpleConstraintException1()
        {
            SimpleConstraintClass o = new SimpleConstraintClass();
            o.Value = 5; o.State = "Default";
            o.Save();
        }
        
        /// <summary>
        /// Тест атрибута Constraint с простыми выражениями (ожидаемое исключение 2)
        /// </summary>
        [TestMethod, ExpectedOracleException(2290)]
        public void TestSimpleConstraintException2()
        {
            SimpleConstraintClass o = new SimpleConstraintClass();
            o.Value = 5; o.State = "State3";
            o.Save();
        }

        /// <summary>
        /// Тест атрибута Constraint с простыми выражениями (ожидаемое исключение 3)
        /// </summary>
        [TestMethod, ExpectedOracleException(2290)]
        public void TestSimpleConstraintException3()
        {
            SimpleConstraintClass o = new SimpleConstraintClass();
            o.Date1 = new DateTime(2011, 11, 11);
            o.Save();
        }

        /// <summary>
        /// Тест атрибута Constraint со сложными выражениями (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestConstraint()
        {
            CategoryClass c = new CategoryClass("1");
            ConstraintClass o = new ConstraintClass();
            o.Date1 = DateTime.Today.AddDays(1);
            o.Category = c;
            c.Save(); o.Save();
        }

        /// <summary>
        /// Тест атрибута Constraint со сложными выражениями (ожидаемое исключение 1)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestConstraintException1()
        {
            ConstraintClass o = new ConstraintClass();
            o.Date1 = DateTime.Today;
            o.Save();
        }

        /// <summary>
        /// Тест атрибута Constraint со сложными выражениями (ожидаемое исключение 2)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestConstraintException2()
        {
            CategoryClass c2 = new CategoryClass("2");
            CategoryClass c5 = new CategoryClass("5");
            ConstraintClass o = new ConstraintClass();
            o.Category = c2;
            try
            {
                c2.Save(); c5.Save(); o.Save();
            }
            catch (Exception e) { throw new Exception("Not expected: " + e.Message); };

            o.Category = c5;
            o.Save();
        }

        /// <summary>
        /// Тест атрибута Security (нормальное выполнение)
        /// </summary>
        [TestMethod]
        public void TestSecurity()
        {
            CategoryClass c1 = new CategoryClass("1"), c2 = new CategoryClass("2"), c3 = new CategoryClass("3");
            c1.Save(); c2.Save(); c3.Save();

            SecurityClass s1 = new SecurityClass(c1), s2 = new SecurityClass(c2), s3 = new SecurityClass(c3);
            s1.Save(); s2.Save(); s3.Save();

            XPCollection<SecurityClass> coll = new XPCollection<SecurityClass>(
                CriteriaOperator.Parse("Category in (?, ?, ?)", c1, c2, c3));
            coll.Load();
            Assert.AreEqual(2, coll.Count());

            s3.Delete();
            coll.Reload();
            Assert.AreEqual(1, coll.Count());
        }

        /// <summary>
        /// Тест атрибута Security (ожидаемое исключение)
        /// </summary>
        [TestMethod, ExpectedOracleException(20001)]
        public void TestSecurityException()
        {
            CategoryClass c2 = new CategoryClass("2");
            SecurityClass s2 = new SecurityClass(c2);
            c2.Save(); s2.Save();

            s2.Delete();
        }
    }

    /// <summary>
    /// Атрибут ожидаемого исключения Oracle
    /// </summary>
    public class ExpectedOracleExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        private int expectedNumber;

        public ExpectedOracleExceptionAttribute(int expectedNumber) { this.expectedNumber = expectedNumber; }

        protected override void Verify(Exception exception)
        {
            Assert.IsNotNull(exception);
            base.RethrowIfAssertException(exception);
            Assert.IsInstanceOfType(exception, typeof(SqlExecutionErrorException), "Not SqlExecutionError was thrown ({0})", exception);
            exception = ((SqlExecutionErrorException)exception).InnerException;
            Assert.IsNotNull(exception, "There is not inner exception");
            Assert.AreEqual(exception.GetType().Name, "OracleException", "Not Oracle exception was thrown ({0})", exception);
            int number = (int)exception.GetType().GetProperty("Number").GetValue(exception, new object[0]);
            Assert.AreEqual(expectedNumber, number, "Not expected Oracle exception code");
        }
    }

    /// <summary>
    /// Атрибут ожидаемого исключения OracleConnectionProvider
    /// </summary>
    public class ExpectedOracleConnectionProviderExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        protected override void Verify(Exception exception)
        {
            Assert.IsNotNull(exception);
            base.RethrowIfAssertException(exception);
            Assert.IsInstanceOfType(exception, typeof(OracleConnectionProviderException), "Not OracleConnectionProviderException was thrown ({0})", exception);
        }
    }
}
