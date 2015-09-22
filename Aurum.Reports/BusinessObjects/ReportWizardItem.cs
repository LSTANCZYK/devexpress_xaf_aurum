using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Aurum.Reports
{
    /// <summary>
    /// Элемент мастера отчета
    /// </summary>
    [DomainComponent]
    [XmlInclude(typeof(ReportWizard))]
    [XmlInclude(typeof(ReportWizardBlock))]
    [XmlInclude(typeof(ReportWizardParameter))]
    [XmlInclude(typeof(ReportWizardQuery))]
    [XmlInclude(typeof(ReportWizardColumn))]
    [XmlInclude(typeof(ReportWizardCondition))]
    [XmlInclude(typeof(ReportWizardGroup))]
    [XmlInclude(typeof(ReportWizardSort))]
    public abstract class ReportWizardItem : INotifyPropertyChanged
    {
        /// <summary>Алиас редактора свойства выражения</summary>
        public const string ExpressionPropertyEditorAlias = "ReportBlockExpressionPropertyEditor";

        private string name;
        private ReportWizardBlock parent;
        private IList list;
        private TypeToStringConverter converter = new TypeToStringConverter();
        private bool parentChanging = false;
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        /// Название элемента
        /// </summary>
        public string Name
        {
            get { return string.IsNullOrEmpty(name) ? DefaultName : name; }
            set { if (value != DefaultName) { name = value; OnChanged("Name"); } }
        }

        /// <summary>
        /// Название элемента по умолчанию
        /// </summary>
        protected virtual string DefaultName 
        {
            get { return CaptionHelper.GetClassCaption(GetType().FullName); } 
        }

        /// <summary>
        /// Список, в который входит текущий элемент
        /// </summary>
        [Browsable(false), XmlIgnore]
        public IList List { get { return list; } }

        /// <summary>
        /// Родительский блок мастера отчетов
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ReportWizardBlock Parent
        {
            get { return parent; }
            set 
            {
                if (!parentChanging && value != parent)
                {
                    parentChanging = true;
                    try
                    {
                        if (list != null && list.Contains(this)) list.Remove(this);
                        parent = value;
                        list = parent != null ? parent.GetList(this) : null;
                        if (list != null && !list.Contains(this)) list.Add(this);
                    }
                    finally
                    {
                        parentChanging = false;
                    }
                    OnChanged("Parent");
                    OnChanged("IndexOf");
                }
            }
        }

        /// <summary>
        /// Тип данных, на котором основаны выражения элемента
        /// </summary>
        [Browsable(false), XmlIgnore]
        public virtual Type TargetObjectType
        {
            get { return Parent != null ? Parent.TargetObjectType : null; }
        }

        /// <summary>
        /// Описание типа данных, на котором основаны выражения элемента
        /// </summary>
        [Browsable(false)]
        public string TargetObjectCaption
        {
            get { return TargetObjectType != null ? CaptionHelper.GetClassCaption(TargetObjectType.FullName) : string.Empty; }
        }

        /// <summary>
        /// Индекс текущего элемента мастера отчетов среди дочерних элементов родителя
        /// </summary>
        [VisibleInDetailView(false), VisibleInListView(false)]
        public int IndexOf 
        { 
            get { return list != null ? list.IndexOf(this) : -1; } 
        }

        /// <summary>
        /// Данные произвольного типа
        /// </summary>
        [Browsable(false), XmlIgnore]
        public object Tag { get; set; }

        /// <summary>Конвертирует тип в строку</summary>
        /// <param name="type">Тип</param>
        /// <returns>Строка, определяющая тип <b>type</b></returns>
        protected string TypeToString(Type type) { return (string)converter.ConvertToStorageType(type); }

        /// <summary>Конвретирует строку в тип</summary>
        /// <param name="type">Строка с определением типа</param>
        /// <returns>Тип, определенный в строке <b>type</b></returns>
        protected Type StringToType(string type) { return (Type)converter.ConvertFromStorageType(type); }

        /// <summary>
        /// Заполняет таблицу параметров для редактора выражений
        /// </summary>
        /// <param name="table">Таблица параметров редактора выражений</param>
        public virtual void FillParameters(Dictionary<string, string> table)
        {
            if (Parent != null) Parent.FillParameters(table);
        }

        /// <summary>
        /// Возвращает мастер отчетов, которому принадлежит текущий элемент
        /// </summary>
        /// <returns>Мастер отчетов, которому принадлежит текущий элемент или null, если элемент не входит в состав мастера отчетов</returns>
        protected ReportWizard FindWizard()
        {
            if (this is ReportWizard) return (ReportWizard)this;
            ReportWizardBlock block = parent;
            while (block != null && !(block is ReportWizard)) block = block.parent;
            return (ReportWizard)block;
        }

        /// <summary>
        /// Обрабатывает изменение указанного свойства
        /// </summary>
        /// <param name="propertyName">Название свойства, которое было изменено</param>
        /// <remarks>По умолчанию вызывает события изменения соответствующего свойства и мастера отчетов в целом</remarks>
        /// <seealso cref="RaisePropertyChanged"/>
        protected internal virtual void OnChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
            RaiseWizardChanged();
        }

        /// <summary>
        /// Вызывает событие изменения указанного свойства
        /// </summary>
        /// <param name="propertyName">Название свойства, которое было изменено</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Вызов события изменения мастера отчетов
        /// </summary>
        protected virtual void RaiseWizardChanged()
        {
            ReportWizard wizard = FindWizard();
            if (wizard != null) wizard.RaiseWizardChanged();
        }

        /// <summary>
        /// Устанавливает новое значение свойства
        /// </summary>
        /// <typeparam name="T">Тип свойства</typeparam>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="propertyValueHolder">Холдер значения свойства</param>
        /// <param name="newValue">Новое значение</param>
        /// <remarks>Новое значение устанавливается, если оно не равно старому. 
        /// После установки вызывается обработка изменения указанного свойства <see cref="OnChanged"/>.</remarks>
        protected void SetPropertyValue<T>(string propertyName, ref T propertyValueHolder, T newValue)
        {
            if (!object.Equals(propertyValueHolder, newValue))
            {
                propertyValueHolder = newValue;
                OnChanged(propertyName);
            }
        }

        /// <summary>
        /// Валидация элемента мастера отчетов
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке элемента, если валидация не успешная</param>
        /// <returns>True - если элемент валидный, иначе false</returns>
        public virtual bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;
            return true;
        }

        #region INotifyPropertyChanged

        /// <contentfrom cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        #endregion
    }

    /// <summary>
    /// Блок мастера отчетов
    /// </summary>
    [DomainComponent]
    public abstract class ReportWizardBlock : ReportWizardItem
    {
        private ReportWizardItemList<ReportWizardBlock> children;
        private Type dataType;

        /// <summary>Конструктор</summary>
        public ReportWizardBlock()
        {
            children = new ReportWizardItemList<ReportWizardBlock>(this, "Children");
        }

        /// <summary>Тип данных</summary>
        [TypeConverter(typeof(ReportDataTypeConverter))] // IsVisibleInReports
        [ValueConverter(typeof(TypeToStringConverter))]
        [XmlIgnore]
        public Type DataType 
        {
            get { return dataType; }
            set { SetPropertyValue("DataType", ref dataType, value); } 
        }

        /// <summary>Сериализация типа данных</summary>
        [XmlElement("DataType"), Browsable(false)]
        public string DataTypeSerializable
        {
            get { return TypeToString(DataType); }
            set { DataType = StringToType(value); }
        }

        /// <summary>Тип данных, на котором основаны выражения блока</summary>
        [Browsable(false), XmlIgnore]
        public override Type TargetObjectType
        {
            get { return DataType ?? base.TargetObjectType; }
        }

        /// <summary>
        /// Дочерние блоки мастера отчетов
        /// </summary>
        [VisibleInDetailView(false)]
        public BindingList<ReportWizardBlock> Children
        {
            get { return children; }
        }

        /// <summary>Уровень текущего блока в иерархии</summary>
        [VisibleInDetailView(false)]
        public int LevelOf { get { return Parent != null ? Parent.LevelOf + 1 : 0; } }

        /// <summary>Порядок текущего блока в иерархии</summary>
        [VisibleInDetailView(false)]
        public string HierarchyOrder
        {
            get
            {
                string parentOrder = Parent != null ? Parent.HierarchyOrder : null;
                string index = Parent != null ? IndexOf.ToString().PadLeft(3, '0') : null;
                return string.IsNullOrEmpty(parentOrder) ? index : string.Concat(parentOrder, "-", index);
            }
        }

        /// <summary>Название блока с отступом, равным уровню блока в иерархии минус 1 (верхний уровень не считается)</summary>
        [VisibleInDetailView(false)]
        public string LevelName { get { return (LevelOf > 0 ? new string('\t', LevelOf - 1) : string.Empty) + Name; } }

        /// <summary>
        /// Возвращает список элементов мастера отчетов, соответствующий указанному
        /// </summary>
        /// <param name="item">Элемент мастера отчетов</param>
        /// <returns>Список элементов мастера отчетов, сответствующий элементу <b>item</b> или null, если нет подходящего списка</returns>
        protected internal virtual IList GetList(ReportWizardItem item)
        {
            if (item is ReportWizardBlock) return Children;
            return null;
        }

        /// <inheritdoc/>
        protected internal override void OnChanged(string propertyName)
        {
            base.OnChanged(propertyName);
            if (propertyName == "Parent" || propertyName == "IndexOf")
                RaisePropertyChanged("HierarchyOrder");
        }

        /// <inheritdoc/>
        public override bool Validate(out string errorMessage)
        {
            if (!children.Validate(out errorMessage)) return false;
            if (TargetObjectType == null) { errorMessage = "Не определен тип данных"; return false; }
            return base.Validate(out errorMessage);
        }
    }

    /// <summary>Связанный список элементов мастера отчетов с синхронизацией родительского блока</summary>
    class ReportWizardItemList<T> : BindingList<T>
        where T : ReportWizardItem
    {
        private ReportWizardBlock parent;
        private string propertyName;

        public ReportWizardItemList(ReportWizardBlock parent, string propertyName) 
        { 
            this.parent = parent; 
            this.propertyName = propertyName; 
        }

        public ReportWizardItemList(ReportWizardBlock parent, IList<T> list, string propertyName) 
            : base(list) 
        { 
            this.parent = parent;
            this.propertyName = propertyName; 
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            T removed = index >= 0 && index < Count ? this[index] : null;
            base.RemoveItem(index);
            if (removed != null) removed.Parent = null;
        }

        /// <inheritdoc/>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            if (e.ListChangedType == ListChangedType.ItemAdded) this[e.NewIndex].Parent = parent;
            //if (parent != null) parent.OnChanged(propertyName); // какая-то рекурсия возникает, интерфейс сильно зависает
        }

        /// <summary>
        /// Валидация списка элементов мастера отчетов
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке элемента, если валидация не успешная</param>
        /// <returns>True - если элемент валидный, иначе false</returns>
        public virtual bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;
            foreach (T item in this) if (!item.Validate(out errorMessage)) return false;
            return true;
        }
    }
}
