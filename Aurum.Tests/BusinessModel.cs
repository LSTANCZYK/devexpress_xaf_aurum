using System;
using DevExpress.Xpo;
using Aurum.Xpo;

[assembly: PersistentSchema("testxpo", Namespace = "DevExpress", IsGlobal = true)]
[assembly: PersistentSchema("testxpo")]

namespace Aurum.Tests
{
    /// <summary>
    /// Описывает стандартные поля для тестирования crud-функционала
    /// </summary>
    public interface ITestCrud
    {
        string Field1 { get; set; }
    }

    /// <summary>
    /// Класс с идентификатором с автогенерацией
    /// </summary>
    public class AutoIdClass : XPObjectBase, ITestCrud
    {
        public AutoIdClass() { }
        public AutoIdClass(Session session) : base(session) { }

        private string field1;
        public string Field1
        {
            get { return field1; }
            set { SetPropertyValue("Field1", ref field1, value); }
        }
    }

    /// <summary>
    /// Класс с простым строчным идентификатором
    /// </summary>
    public class StringIdClass : XPObjectBase<string>, ITestCrud
    {
        public StringIdClass(string id) : base(id) { }
        public StringIdClass(Session session) : base(session) { }

        private string field1;
        public string Field1
        {
            get { return field1; }
            set { SetPropertyValue("Field1", ref field1, value); }
        }
    }

    /// <summary>
    /// Сложный идентификатор с двумя полями
    /// </summary>
    public struct ComplexIdentifier
    {
        [Size(30)]
        public string Part1;

        public int Part2;
    }

    /// <summary>
    /// Класс со сложным идентификатором
    /// </summary>
    public class ComplexIdClass : XPObjectBase<ComplexIdentifier>, ITestCrud
    {
        public ComplexIdClass(ComplexIdentifier id) : base(id) { }
        public ComplexIdClass(Session session) : base(session) { }

        private string field1;
        public string Field1
        {
            get { return field1; }
            set { SetPropertyValue("Field1", ref field1, value); }
        }
    }

    /// <summary>
    /// Класс без идентификатора
    /// </summary>
    /// <remarks>Crud для класса без идентификатора в настоящее время не поддерживается</remarks>
    [OptimisticLocking(false)]
    public class NoIdClass : XPBaseObject, ITestCrud
    {
        public NoIdClass() { }
        public NoIdClass(Session session) : base(session) { }

        private string field1;
        public string Field1
        {
            get { return field1; }
            set { SetPropertyValue("Field1", ref field1, value); }
        }
    }

    /// <summary>
    /// Класс с атрибутом свойства NotNull
    /// </summary>
    public class NotNullClass : XPObjectBase
    {
        public NotNullClass() { }
        public NotNullClass(Session session) : base(session) { }

        [Size(50)]
        public string Field1;

        [Size(50), NotNull]
        public string Field2NotNull;
    }

    /// <summary>
    /// Класс с атрибутом свойства ReadOnly
    /// </summary>
    public class ReadOnlyClass : XPObjectBase
    {
        public ReadOnlyClass() { }
        public ReadOnlyClass(Session session) : base(session) { }

        [Size(50)]
        public string Field1;

        [Size(50), ReadOnly]
        public string Field2ReadOnly;
    }

    /// <summary>
    /// Класс с атрибутом ConsistentPeriod со сложным ключом периода
    /// </summary>
    [ConsistentPeriod("PeriodKey1;PeriodKey2", "DateIn", "DateOut")]
    public class ConsistentPeriodClass : XPObjectBase
    {
        public ConsistentPeriodClass(int pk1, int pk2, DateTime din, DateTime? dout) 
        {
            PeriodKey1 = pk1;
            PeriodKey2 = pk2;
            DateIn = din;
            DateOut = dout;
        }
        public ConsistentPeriodClass(Session session) : base(session) { }

        public int PeriodKey1;
        public int PeriodKey2;
        public DateTime DateIn;
        public DateTime? DateOut;
    }

    /// <summary>
    /// Класс с атрибутом ConsistentPeriod с непрерывностью периодов
    /// </summary>
    [ConsistentPeriod("PeriodKey", "DateIn", "DateOut", Continuous = true)]
    public class ContinuousPeriodClass : XPObjectBase
    {
        public ContinuousPeriodClass(string pk, DateTime din, DateTime? dout) 
        {
            PeriodKey = pk;
            DateIn = din;
            DateOut = dout;
        }
        public ContinuousPeriodClass(Session session) : base(session) { }

        public string PeriodKey;
        public DateTime DateIn;
        public DateTime? DateOut;
    }

    /// <summary>
    /// Класс с атрибутом ConsistentPeriod с полями в базовом классе
    /// </summary>
    [ConsistentPeriod("PeriodKey2;PeriodKey3", "DateIn", "DateOut")]
    public class ConsistentPeriodClass2 : ConsistentPeriodClass
    {
        public ConsistentPeriodClass2(int pk1, int pk2, int pk3, DateTime din, DateTime? dout)
            : base(pk1, pk2, din, dout)
        {
            PeriodKey3 = pk3;
        }
        public ConsistentPeriodClass2(Session session) : base(session) { }

        public int PeriodKey3;
    }

    /// <summary>
    /// Класс с атрибутом Hierarchy
    /// </summary>
    [Hierarchy("Parent")]
    public class HierarchyClass : XPObjectBase
    {
        public HierarchyClass(HierarchyClass parent, string someData)
        {
            this.Parent = parent;
            this.SomeData = someData;
        }
        public HierarchyClass(Session session) : base(session) { }

        public HierarchyClass Parent;
        public string SomeData;
    }

    /// <summary>
    /// Класс с атрибутом Constraint с простыми выражениями (в пределах таблицы и без контекстных функций)
    /// </summary>
    [Constraint("Value = 0 or (not State is null and State <> 'Default')")]
    [Constraint("State is null or State in ('Default', 'State1', 'State2')")]
    [Constraint("GetDay(Date1) = 1 and GetDate(Date1) = Date1")]
    public class SimpleConstraintClass : XPObjectBase
    {
        public SimpleConstraintClass() { }
        public SimpleConstraintClass(Session session) : base(session) { }

        public int Value;
        public string State;
        public DateTime Date1;
    }

    /// <summary>
    /// Класс с атрибутом Constraint со сложными выражениями (разные таблицы или контекстные функции)
    /// </summary>
    [Constraint("Date1 is null or Date1 > Today()")]
    [Constraint("Category is null or Category.Name in ('1', '2', '3')")]
    public class ConstraintClass : XPObjectBase
    {
        public ConstraintClass() { }
        public ConstraintClass(Session session) : base(session) { }

        public DateTime Date1;
        public CategoryClass Category;
    }

    /// <summary>
    /// Категория со строковым названием
    /// </summary>
    public class CategoryClass : XPObjectBase
    {
        public CategoryClass(string name) { this.Name = name; }
        public CategoryClass(Session session) : base(session) { }

        public string Name;
    }

    /// <summary>
    /// Класс с атрибутами Security
    /// </summary>
    [Security(DBOperations.Read, "Category.Name in ('1', '2', '3')")]
    [Security(DBOperations.Read, "Category.Name in ('2', '3')")]
    [Security(DBOperations.Delete, "Category.Name = '3'")]
    public class SecurityClass : XPObjectBase
    {
        public SecurityClass(CategoryClass category) { this.Category = category; }
        public CategoryClass Category;
    }

    /// <summary>
    /// Базовый класс UniqueClass
    /// </summary>
    public class UniqueBaseClass : XPObjectBase
    {
        public UniqueBaseClass(int value) { this.Int = value; }
        public UniqueBaseClass(Session session) : base(session) { }
        
        public int Int;
    }

    /// <summary>
    /// Наследованный класс с атрибутом уникального индекса
    /// </summary>
    [Unique("Int;Int2")]
    public class UniqueClass : UniqueBaseClass
    {
        public UniqueClass(int value, int? value2) : base(value) { this.Int2 = value2; }
        public UniqueClass(Session session) : base(session) { }

        public int? Int2;
    }
}
