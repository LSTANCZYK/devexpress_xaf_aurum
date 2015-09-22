using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using DevExpress.Data.Filtering;

namespace Aurum.Xpo
{
    /// <summary>
    /// Базовый класс атрибутов целостности данных
    /// </summary>
    public abstract class ConstraintBaseAttribute : Attribute
    {
    }

    /// <summary>
    /// Атрибут, указывающий, что поле или свойство не должно иметь значение null
    /// </summary>
    /// <remarks>По умолчанию только ключ не может иметь пустое значение. Обычные свойства и поля могут иметь значение null в базе данных. 
    /// Если указан атрибут <b>NotNullAttribute</b>, то в базе данных добавляется констрейнт not null для колонок свойства или поля.</remarks>
    /// <todo>Проверка существующих констрейнтов not null</todo>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NotNullAttribute : ConstraintBaseAttribute
    {
        /// <summary>Конструктор</summary>
        public NotNullAttribute() { }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public NotNullAttribute(XmlNode attributeNode) { }
    }

    /// <summary>
    /// Атрибут, указывающий, что поле или свойство должно быть доступно только для чтения
    /// </summary>
    /// <remarks>По умолчанию в базе данных редактируются все поля и свойства кроме первичного ключа. Если у поля/свойства указан
    /// атрибут <b>ReadOnlyAttribute</b> или поле/свойство доступно только для чтения в самом классе (см. <see cref="P:XPMemberInfo.IsReadOnly"/>),
    /// то в базу данных добавляется соответствующее ограничение (см. поддержку атрибутов в базе данных).</remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ReadOnlyAttribute : ConstraintBaseAttribute
    {
        /// <summary>Конструктор</summary>
        public ReadOnlyAttribute() { }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public ReadOnlyAttribute(XmlNode attributeNode) { }
    }

    /// <summary>
    /// Атрибут, определяющий непересекающуюся последовательность периодов для заданного набора полей 
    /// </summary>
    /// <remarks>Атрибут определяет целостность периодов в пределах одного класса. Для определения целостности требуется ключ <see cref="PeriodKey"/>,
    /// по каждому уникальному значению которого проверяются периоды, и поля/свойства <see cref="DateIn"/> и <see cref="DateOut"/>, определяющие отдельный период.
    /// Последовательность периодов может быть непрерывной или прерываться промежутками, что определяется свойством <see cref="Continuous"/>.
    /// По умолчанию период определяется как полуинтервал [DateIn,DateOut), но можно переопределить включение концов промежутка с помощью свойств <see cref="IncludeIn"/> и <see cref="IncludeOut"/>. 
    /// <para>Поля/свойства ключа и периода могут храниться как в таблице текущего класса, так и в таблицах одного из родительских классов. 
    /// Поля/свойства дат должны быть типа <c>System.DateTime</c> или производного от него, т.е. конвертироваться в колонку таблицы соответствующего типа. 
    /// Учитывается колонка логического удаления <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/> таблицы текущего или родительского класса.
    /// Атрибут <see cref="DevExpress.Xpo.PersistentAliasAttribute"/> не поддерживается.</para></remarks>
    /// <example><code title="Пример класса с проверкой пересечения периодов" description="" lang="CS"><![CDATA[
    /// [ConsistentPeriod("Client", "DateIn", "DateOut")]
    /// public class ClientHistory : XPBaseObject
    /// {
    ///     public Client Client;
    ///     public DateTime DateIn;
    ///     public DateTime DateOut;
    ///     public SomeData SomeData;
    ///     
    ///     public ClientHistory(Client client, DateTime dateIn, DateTime dateOut, SomeData someData)
    ///     {
    ///         this.Client = client;
    ///         this.DateIn = dateIn;
    ///         this.DateOut = dateOut;
    ///         this.SomeData = someData;
    ///     }
    /// 
    ///     public static void TestConsistentPeriod(Client client, SomeData data1)
    ///     {
    ///         // Первая запись для данного клиента
    ///         new ClientHistory(client, new DateTime(2010,1,10), new DateTime(2010,1,20), data1).Save(); 
    ///         
    ///         // Вызовет исключение о пересечении периодов
    ///         // new ClientHistory(client, new DateTime(2010,1,15), new DateTime(2010,1,25), data1).Save(); 
    ///         
    ///         // Вторая запись для данного клиента, с первой пересечения не имеет.
    ///         // Но если установить в атрибуте класса Continuous = True, то вызовет исключение непрерывности периодов
    ///         new ClientHistory(client, new DateTime(2010,1,21), new DateTime(2010,1,25), data1).Save();
    ///     }
    /// }
    /// ]]></code></example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ConsistentPeriodAttribute : ConstraintBaseAttribute
    {
        private StringCollection periodKey = new StringCollection();
        private string dateIn;
        private string dateOut;
        private bool includeIn = true;
        private bool includeOut = false;
        private bool continuous = false;

        /// <summary>Поля или свойства, определяющие ключ периода</summary>
        /// <remarks>Ключ периода определяет список полей и свойств, по уникальным значениям которых 
        /// рассматривается каждая последовательность периодов</remarks>
        public StringCollection PeriodKey { get { return periodKey; } }

        /// <summary>Поле или свойство, определяющее начало периода</summary>
        public string DateIn { get { return dateIn; } }

        /// <summary>Поле или свойство, определяющее конец периода</summary>
        public string DateOut { get { return dateOut; } }

        /// <summary>Указывает, включено ли начало в период</summary>
        /// <value>True - начало включено в период (<b>default</b>), false - начало исключено из периода</value>
        public bool IncludeIn { get { return includeIn; } set { includeIn = value; } }

        /// <summary>Указывает, включен ли конец в период</summary>
        /// <value>True - конец включен в период, false - конец исключен из периода (<b>default</b>)</value>
        public bool IncludeOut { get { return includeOut; } set { includeOut = value; } }

        /// <summary>Непрерывность периодов</summary>
        /// <value>True - последовательность периодов непрерывная, false - последовательность периодов может иметь промежутки (<b>default</b>)</value>
        /// <remarks>Непрерывность последовательности периодов означает, могут ли быть между периодами промежутки или нет, если их расположить друг за другом.</remarks>
        public bool Continuous { get { return continuous; } set { continuous = value; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="periodKey">Поля или свойства через &quot;;&quot;, определяющие ключ периода <see cref="PeriodKey"/></param>
        /// <param name="dateIn">Поле или свойство, определяющее начало периода</param>
        /// <param name="dateOut">Поле или свойство, определяющее конец периода</param>
        public ConsistentPeriodAttribute(string periodKey, string dateIn, string dateOut)
        {
            this.periodKey.AddRange(periodKey.Split(';'));
            this.dateIn = dateIn;
            this.dateOut = dateOut;
        }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public ConsistentPeriodAttribute(XmlNode attributeNode) 
        {
            periodKey.AddRange(attributeNode.Attributes["PeriodKey"].Value.Split(';'));
            dateIn = attributeNode.Attributes["DateIn"].Value;
            dateOut = attributeNode.Attributes["DateOut"].Value;
            if (attributeNode.Attributes["IncludeIn"] != null)
                includeIn = bool.Parse(attributeNode.Attributes["IncludeIn"].Value);
            if (attributeNode.Attributes["IncludeOut"] != null)
                includeOut = bool.Parse(attributeNode.Attributes["IncludeOut"].Value);
            if (attributeNode.Attributes["Continious"] != null)
                continuous = bool.Parse(attributeNode.Attributes["Continious"].Value);
        }
    }

    /// <summary>
    /// Атрибут, определяющий иерархию между указанными полями
    /// </summary>
    /// <remarks>Атрибут <see cref="HierarchyAttribute"/> определяет иерархию объектов по указанному полю/свойству. 
    /// Класс ссылки на родительскую запись <see cref="P:Parent"/> должен совпадать с классом объектов, для которых определена иерархия.
    /// Свойство <see cref="P:NoCycle"/> определяет возможность циклов внутри иерархии.<br/>
    /// Учитывается колонка логического удаления <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/> таблицы текущего класса.
    /// <see cref="DevExpress.Xpo.PersistentAliasAttribute"/> не поддерживается.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class HierarchyAttribute : ConstraintBaseAttribute
    {
        private string parent;
        private bool noCycle = true;

        /// <summary>Поле или свойство, определяющее ссылку на родительскую запись</summary>
        public string Parent { get { return parent; } }

        /// <summary>Отсутствие циклов в иерархии</summary>
        /// <value>True - в иерархии отсутствуют записи, имеющие в качестве родительской одну из своих дочерних записей (<b>default</b>),
        /// false - в иерархии могут присутствовать цепочки записей, составляющих замкнутый цикл</value>
        public bool NoCycle { get { return noCycle; } set { noCycle = value; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parent">Поле или свойство, определяющее ссылку на родительскую запись</param>
        public HierarchyAttribute(string parent)
        {
            this.parent = parent;
        }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public HierarchyAttribute(XmlNode attributeNode) 
        {
            parent = attributeNode.Attributes["Parent"].Value;
        }
    }

    /// <summary>
    /// Атрибут, определяющий уникальность записей по заданным полям
    /// </summary>
    /// <remarks>Атрибут определяет уникальность записей по указанным полям или свойствам. 
    /// Поля/свойства могут храниться как в таблице текущего класса, так и в таблицах одного из родительских классов.
    /// Если требуется задать уникальность полей/свойств в пределах таблицы текущего класса, 
    /// то рекомендуется использовать уникальный индекс <see cref="DevExpress.Xpo.IndexedAttribute"/><c>(Unique=true)</c>.<br/>
    /// Учитывается колонка логического удаления <see cref="DevExpress.Xpo.DeferredDeletionAttribute"/> таблицы текущего или родительского класса.
    /// Атрибут <see cref="DevExpress.Xpo.PersistentAliasAttribute"/> не поддерживается.</remarks>
    /// <example><code title="Пример класса с уникальным набором полей"><![CDATA[
    /// public class Good : XPObjectBase
    /// {
    ///    public Good() { }
    ///    public Good(Session session) : base(session) { }
    ///    
    ///    public string Name { get; set; }
    ///    public decimal Price { get; set; }
    /// }
    /// 
    /// [Unique("Year;Name")]
    /// public class Video : Good
    /// {
    ///    public Video() { }
    ///    public Video(Session session) : base(session) { }
    ///    
    ///    public int Year { get; set; }
    /// }
    /// ]]></code></example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UniqueAttribute : ConstraintBaseAttribute
    {
        private StringCollection fields = new StringCollection();

        /// <summary>Поля или свойства, определяющие уникальность записей</summary>
        public StringCollection Fields { get { return fields; } }

        /// <summary>Конструктор</summary>
        public UniqueAttribute() { }

        /// <summary>Конструктор</summary>
        /// <param name="fields">Поля или свойства через &quot;;&quot;, определяющие уникальность записей</param>
        public UniqueAttribute(string fields)
        {
            this.fields.AddRange(fields.Split(';'));
        }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public UniqueAttribute(XmlNode attributeNode)
        {
            fields.AddRange(attributeNode.Attributes["Fields"].Value.Split(';'));
        }
    }

    /// <summary>
    /// Атрибут, определяющий целостность данных указанным выражением
    /// </summary>
    /// <remarks>Атрибут определяет целостность данных согласно указанного выражения. Целостность данных может быть реализована
    /// как статическим констрейнтом таблицы БД, так и процедурами проверки в операциях изменения данных, в зависимости от сложности выражения.
    /// Выражение сложное, т.е. не может быть преобразовано в статический констрейнт БД, когда оно содержит ссылки на другие таблицы или 
    /// функции, вычисляемые в зависимости от контекста, например, функция текущей даты. Процедуры проверки вызываются только при изменении данных
    /// того класса, к которому применен атрибут. Целостность данных тех классов, на которые в выражении есть ссылки, нужно проверять отдельно.
    /// <para>Отличие атрибута целостности <see cref="ConstraintAttribute"/> от атрибута безопасности <seealso cref="SecurityAttribute"/> в том, 
    /// что он проверяется только при добавлении и изменении данных и может быть конвертирован в простой констрейнт таблицы</para></remarks>
    /// <example>В данном примере на класс накладывается ограничение, не позволяющее сохранять объекты, у которых свойство DateIn больше или равно свойству DateOut.
    /// <code title="Пример применения атрибута для проверки корректности дат" lang="CS">
    /// [Constraint("DateIn" &lt; "DateOut" or DateOut is null)]
    /// public class ClientHistory: XPBaseObject
    /// {
    ///     public DateTime DateIn;
    ///     public DateTime? DateOut;
    ///     ...
    /// }
    /// </code>
    /// <code title="Пример применения атрибута со ссылкой на свойство другого класса" lang="CS">
    /// public class Category: XPObjectBase
    /// {
    ///     public string Code;
    ///     public string Name;
    /// }
    /// 
    /// public class Good: XPObjectBase
    /// {
    ///     public Category Category;
    ///     public string Name;
    /// }
    /// 
    /// [Constraint("Category.Code in ('Toy', 'Game', 'Souvenir')")]
    /// public class Gift: Good
    /// {
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ConstraintAttribute : ConstraintBaseAttribute
    {
        private string expression;
        private CriteriaOperator criteria;

        /// <summary>Выражение, определяющее целостность данных</summary>
        public string Expression { get { return expression; } }

        /// <summary>Критерий, определяющий целостность данных</summary>
        internal CriteriaOperator Criteria
        {
            get { return criteria ?? (criteria = CriteriaOperator.Parse(expression)); }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="expression">Выражение, определяющее целостность данных</param>
        public ConstraintAttribute(string expression)
        {
            this.expression = expression;
        }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public ConstraintAttribute(XmlNode attributeNode) 
        {
            expression = attributeNode.Attributes["Expression"].Value;
        }
    }

    /// <summary>
    /// Атрибут, определяющий безопасность данных
    /// </summary>
    /// <remarks>Атрибут определяет безопасность для операций <see cref="P:Operations"/> в базе данных. 
    /// Только для данных, которые удовлетворяют указанному выражению <see cref="P:Expression"/>, эти операции доступны.
    /// <para>Атрибут безопасности <see cref="SecurityAttribute"/> отличается от атрибута целостности <seealso cref="ConstraintAttribute"/> тем, 
    /// что в нем указываются операции, он вызывается до изменения или удаления записи и он не может быть конвертирован в простой констрейнт таблицы</para></remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SecurityAttribute : Attribute
    {
        private string expression;
        private CriteriaOperator criteria;
        private DBOperations operations;

        /// <summary>Выражение, определяющее безопасность данных</summary>
        public string Expression { get { return expression; } }

        /// <summary>Критерий, определяющий безопасность данных</summary>
        internal CriteriaOperator Criteria
        {
            get { return criteria ?? (criteria = CriteriaOperator.Parse(expression)); }
        }

        /// <summary>Операции, для которых определяется безопасность</summary>
        /// <value>Значение по умолчанию <code>DBOperations.Add | DBOperations.Edit</code></value>
        public DBOperations Operations
        {
            get { return operations; }
            set { operations = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="expression">Выражение, определяющее безопасность данных</param>
        public SecurityAttribute(string expression)
        {
            this.operations = DBOperations.AddEdit;
            this.expression = expression;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="operations">Операции, для которых определяется безопасность</param>
        /// <param name="expression">Выражение, определяющее безопасность данных</param>
        public SecurityAttribute(DBOperations operations, string expression)
        {
            this.operations = operations;
            this.expression = expression;
        }

        /// <summary>Конструктор для загрузки из xml</summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public SecurityAttribute(XmlNode attributeNode) 
        {
            operations = (DBOperations)Convert.ToInt32(attributeNode.Attributes["Operations"].Value);
            expression = attributeNode.Attributes["Expression"].Value;
        }
    }

    /// <summary>
    /// Определяет схему в базе данных по умолчанию для классов хранимых объектов
    /// </summary>
    /// <remarks>В атрибуте может быть указано пространство имен, уточняющее для каких классов определена сборка.
    /// Если пространство имен не указано, то схема относится ко всем классам хранимых объектов.
    /// Если для сборки указано несколько схем для разных пространств имен, то схема для конкретного класса выбирается 
    /// по наиболее точному пространству имен. 
    /// Если установлен параметр <c>IsGlobal</c> в True, то атрибут считается глобальным. Это означает, что схема определена 
    /// для классов любой сборки при соответствующем совпадении пространства имен. При определении схемы глобальные атрибуты 
    /// проверяются после локальных атрибутов сборки. При использовании глобальных атрибутов рекомендуется инициализировать
    /// сбор метаданных в справочнике метаданных, явно указав сборки с глобальными атрибутами.</remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class PersistentSchemaAttribute : Attribute
    {
        private string schema;
        private string ns;
        private bool isGlobal;

        /// <summary>
        /// Схема в базе данных
        /// </summary>
        public string Schema
        {
            get { return schema; }
        }

        /// <summary>
        /// Пространство имен классов хранимых объектов, для которых определена схема в базе данных
        /// </summary>
        /// <value>Если пространство имен не указано или пустое, то схема относится ко всем классам сборки</value>
        public string Namespace
        {
            get { return ns; }
            set { ns = value; }
        }

        /// <summary>
        /// Устанавливает, определена ли схема для классов любой сборки или только для классов текущей сборки
        /// </summary>
        /// <value>True - схема определена для классов любой сборки с данным пространством имен, 
        /// false - схема определена только для классов текущей сборки</value>
        public bool IsGlobal
        {
            get { return isGlobal; }
            set { isGlobal = value; }
        }

        /// <summary>
        /// Конструктор атрибута с указанием схемы базы данных
        /// </summary>
        /// <param name="schema">Схема базы данных</param>
        public PersistentSchemaAttribute(string schema)
        {
            this.schema = schema;
        }

        /// <summary>
        /// Конструктор атрибута с указанием схемы базы данных и пространства имен классов
        /// </summary>
        /// <param name="schema">Схема базы данных</param>
        /// <param name="nameSpace">Пространство имен классов</param>
        public PersistentSchemaAttribute(string schema, string nameSpace)
        {
            this.schema = schema;
            this.ns = nameSpace;
        }

        /// <summary>
        /// Конструктор для загрузки из xml
        /// </summary>
        /// <param name="attributeNode">Xml-элемент с описанием атрибута</param>
        public PersistentSchemaAttribute(XmlNode attributeNode)
        {
            schema = attributeNode.Attributes["Schema"].Value;
            if (attributeNode.Attributes["Namespace"] != null)
                ns = attributeNode.Attributes["Namespace"].Value;
        }
    }

    /// <summary>
    /// Определяет класс объектов базы данных с настраиваемым управлением данными
    /// </summary>
    /// <remarks>Атрибут имеет смысл только при наличии <see cref="DevExpress.Xpo.PersistentAttribute"/>.
    /// Для классов, отмеченных этим атрибутом не создаются таблицы и ссылки на них. 
    /// Выборка, создание, изменение и удаление определяются в указанном в атрибуте контроллере <see cref="ControllerType"/> через интерфейс 
    /// <see cref="ICustomPersistent"/>. Контроллер управления данными создается конструктором без параметров.
    /// <para>
    /// Прямые наследники класса не могут хранить свои поля в таблице такого класса, 
    /// т.е. иметь атрибут <see cref="DevExpress.Xpo.MapInheritanceAttribute"/> 
    /// со значением <see cref="DevExpress.Xpo.MapInheritanceType.ParentTable"/>.
    /// Объекты с настраиваемым управлением данными не поддерживаются в констрейнтах.
    /// </para></remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomPersistentAttribute : Attribute
    {
        private Type controllerType;

        /// <summary>
        /// Тип объекта, обеспечивающего настройку управления данными
        /// </summary>
        public Type ControllerType { get { return controllerType; } }

        /// <summary>
        /// Название ассоциации many-to-many, данными которой управляет контроллер
        /// </summary>
        public string AssociationName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controllerType">Тип объекта, обеспечивающего настройку управления данными</param>
        public CustomPersistentAttribute(Type controllerType) { this.controllerType = controllerType; }
    }
}
