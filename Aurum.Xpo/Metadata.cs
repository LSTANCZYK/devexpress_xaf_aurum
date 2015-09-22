using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace Aurum.Xpo
{
    /// <summary>
    /// Поставщик дополнительной информации (информатор) о типах зарегистрированных поставщиков метаданных
    /// </summary>
    /// <remarks>Поставщик дополнительной информации собирает ее на основе типов тех поставщиков метаданных,
    /// которые зарегистрированы с помощью метода <see cref="M:Register"/>. Для исключения поставщика метаданных
    /// из списка информатора следует использовать метод <see cref="Unregister"/>.</remarks>
    public class XPDictionaryInformer 
    {
        private static XPDictionaryInformer instance = new XPDictionaryInformer();
        private XPDictionaryInformer() { }

        // Внутренний справочник метаданных
        private ReflectionDictionaryEx dictionary = new ReflectionDictionaryEx();

        // Трансляция названий таблиц
        private Dictionary<string, string> tableNames = new Dictionary<string, string>();
        private HashSet<string> tableConflicts = new HashSet<string>();
        private Dictionary<XPClassInfo, string> originals = new Dictionary<XPClassInfo, string>();

        // Соответствия типов объектов и названий таблиц (транслированных)
        private Dictionary<Type, List<string>> typeTables = new Dictionary<Type, List<string>>();
        private Dictionary<string, List<Type>> tableTypes = new Dictionary<string, List<Type>>();

        // Добавление или удаление класса в поставщике метаданных, зарегистрированном в информаторе
        private void OnClassInfoChanged(object sender, ClassInfoEventArgs e)
        {
            AddInfo(e.ClassInfo);
        }

        // Добавление информации о классе в информатор
        private void AddInfo(XPClassInfo info)
        {
            if (!dictionary.Contains(info))
            {
                ReflectionClassInfoEx internalInfo = dictionary.GetClassInfoEx(info); 
                string tablePrefix = dictionary.ResolveTablePrefix(info.ClassType.Namespace);

                // Регистрация таблиц
                if (!string.IsNullOrEmpty(info.TableName) && info.TableMapType != MapInheritanceType.ParentTable && info.IsPersistent)
                {
                    dictionary.Schema.Add(internalInfo.TableName, new Lazy<DBTableEx>(() => internalInfo.TableEx));
                    FullRegisterTable(tablePrefix + info.TableName, internalInfo.TableName, false, info, info);
                    if (!string.IsNullOrEmpty(tablePrefix) && info.ClassType.Namespace.StartsWith("DevExpress"))
                        FullRegisterTable(info.TableName, internalInfo.TableName, false, info, info);
                }

                // Регистрация таблиц для ассоциаций many-to-many
                foreach (ReflectionPropertyInfo prop in info.AssociationListProperties)
                    if (prop.IsManyToMany && prop.Owner == info &&
                        prop.IntermediateClass != null && !string.IsNullOrEmpty(prop.IntermediateClass.TableName))
                    {
                        string tableName = prop.IntermediateClass.TableName;
                        if (tableName.IndexOf('.') < 0 && !string.IsNullOrEmpty(internalInfo.SchemaName))
                            tableName = string.Concat(internalInfo.SchemaName, ".", tableName);
                        ReflectionPropertyInfo intermediateProperty = prop; // переменная для лямбда-выражения
                        dictionary.Schema.Add(tableName, new Lazy<DBTableEx>(() => 
                            internalInfo.GetIntermediateTableEx(intermediateProperty, tableName)));

                        // Трансляция названия
                        if (string.IsNullOrEmpty(tablePrefix))
                            FullRegisterTable(prop.IntermediateClass.TableName, tableName, true, info, prop.IntermediateClass);
                        else
                        {
                            string part1 = null, part2 = null;
                            foreach (XPMemberInfo property in prop.IntermediateClass.ObjectProperties)
                            {
                                if (part1 == null) part1 = property.ReferenceType.TableName + property.Name;
                                else if (part2 == null) part2 = property.ReferenceType.TableName + property.Name;
                            }
                            string assocName = prop.IntermediateClass.TableName;
                            if (string.Concat(part1, "_", part2) == assocName) assocName = string.Concat(tablePrefix, part1, "_", tablePrefix, part2);
                            else if (string.Concat(part2, "_", part1) == assocName) assocName = string.Concat(tablePrefix, part2, "_", tablePrefix, part1);
                            else throw new XPDictionaryInformerException("Error in constructing association table name: " + prop.IntermediateClass.FullName);
                            FullRegisterTable(assocName, tableName, true, info, prop.IntermediateClass);
                        }
                    }

                // Регистрация соответствий типов и таблиц вышестоящих классов
                if (info.IsPersistent)
                {
                    XPClassInfo parent = info.BaseClass;
                    while (parent != null)
                    {
                        foreach (string parentTable in GetReferencedTables(parent.ClassType))
                            RegisterTableType(parentTable, info.ClassType);
                        parent = parent.BaseClass;
                    }
                }
            }
        }

        // Регистрирует таблицу в справочнике трансляции и справочнике типов
        private void FullRegisterTable(string tableName, string translatedName, bool isInternal, XPClassInfo info, XPClassInfo tableInfo)
        {
            // Регистрация трансляции
            if (!isInternal)
                AddTableName(tableName, translatedName);
            else
                tableNames[tableName] = translatedName;
            if (tableInfo != null && !originals.ContainsKey(tableInfo))
                originals.Add(tableInfo, translatedName);

            // Регистрация типов
            if (info != null)
                RegisterTableType(translatedName, info.ClassType);
        }

        // Регистрирует таблицу и тип в справочнике типов
        private void RegisterTableType(string table, Type type)
        {
            List<string> tables; List<Type> types;
            if (!tableTypes.TryGetValue(table, out types)) tableTypes[table] = types = new List<Type>();
            if (!typeTables.TryGetValue(type, out tables)) typeTables[type] = tables = new List<string>();
            if (!tables.Contains(table)) tables.Add(table);
            if (!types.Contains(type)) types.Add(type);
        }

        /// Добавляет трансляцию названия таблицы для использования в базе данных
        private void AddTableName(string tableName, string translatedName)
        {
            if (tableNames.ContainsKey(tableName))
                tableConflicts.Add(tableName);
            else
                tableNames[tableName] = translatedName;
        }

        /// <summary>
        /// Добавляет трансляцию названия таблицы для использования в базе данных
        /// </summary>
        /// <param name="tableName">Исходное название таблицы</param>
        /// <param name="translatedName">Транслированное название, используемое в базе данных</param>
        internal static void AddTranslation(string tableName, string translatedName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName", "Add table translation invalid parameter");
            if (string.IsNullOrEmpty(translatedName))
                throw new ArgumentNullException("translatedName", "Add table translation invalid parameter");

            instance.AddTableName(tableName, translatedName);
        }

        /// <summary>
        /// Регистрация таблицы в схеме и в трансляции имен
        /// </summary>
        /// <param name="tableName">Исходное название таблицы</param>
        /// <param name="table">Регистрируемая таблица</param>
        internal static void RegisterTable(string tableName, DBTableEx table)
        {
            if (string.IsNullOrEmpty(tableName)) 
                throw new ArgumentNullException("tableName", "Register table invalid parameter");
            if (table == null)
                throw new ArgumentNullException("table", "Register table invalid parameter");

            AddTranslation(tableName, table.Name);
            Schema.Add(table.Name, new Lazy<DBTableEx>(() => table));
        }

        /// <summary>Схема объектов базы данных</summary>
        public static DBSchema Schema { get { return instance.dictionary.Schema; } }

        /// <summary>
        /// Возвращает оригинальную таблицу указанного типа
        /// </summary>
        /// <param name="type">Тип, для которого необходимо вернуть таблицу</param>
        /// <returns>Оригинальная таблица, соответствующая типу <paramref name="type"/>, 
        /// или null, если тип не найден или не является персистентным</returns>
        public static DBTable FindOriginalTable(Type type)
        {
            XPClassInfo result = instance.originals.Keys.FirstOrDefault(ci => ci.ClassType == type);
            return result != null ? result.Table : null;
        }

        /// <summary>
        /// Возвращает перечисление названий таблиц, связанных с указанным типом
        /// </summary>
        /// <param name="type">Тип, для которого необходимо вернуть названия связанных таблиц</param>
        /// <returns>Перечисление транслированных названий таблиц, связанных с типом <c>type</c></returns>
        public static IEnumerable<string> GetReferencedTables(Type type)
        {
            List<string> result;
            if (instance.typeTables.TryGetValue(type, out result)) return result;
            return new string[0];
        }

        /// <summary>
        /// Возвращает перечисление типов, связанных с таблицей с указанным названием
        /// </summary>
        /// <param name="tableName">Транслированное название таблицы, для которой необходимо вернуть связанные типы</param>
        /// <returns>Перечисление типов, связанных с таблицей <c>tableName</c></returns>
        public static IEnumerable<Type> GetReferencedTypes(string tableName)
        {
            List<Type> result;
            if (instance.tableTypes.TryGetValue(tableName, out result))
                return result;
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Транслирует название таблицы в название для использования в базе данных
        /// </summary>
        /// <param name="tableName">Исходное название таблицы</param>
        /// <returns>Название, используемое в базе данных</returns>
        internal static string TranslateTableName(string tableName)
        {
            string result;
            if (instance.tableConflicts.Contains(tableName))
                throw new XPDictionaryInformerException("Table name conflict: " + tableName + ". Set explicitly PersistentAttribute");
            if (instance.tableNames.TryGetValue(tableName, out result))
                return result;
            else
            {
                // Поиск в оригинальных названиях
                foreach (KeyValuePair<XPClassInfo, string> original in instance.originals)
                    if (original.Key.Table != null && original.Key.Table.Name == tableName)
                    {
                        instance.AddTableName(tableName, original.Value);
                        return original.Value;
                    }

                // Проверить наличие таких ситуаций
                //if (instance.dictionary.Schema.Contains(tableName)) return tableName; // возможно, это переведенное название таблицы
                throw new XPDictionaryInformerException("Table name is not registrated: " + tableName + ". Check XPDictionaryInformer registration");
            }
        }

        /// <summary>
        /// Транслирует название таблицы и возвращает ее из схемы
        /// </summary>
        /// <param name="tableName">Название таблицы (без трансляции)</param>
        /// <returns>Таблица базы данных</returns>
        internal static DBTableEx TranslateAndGet(string tableName)
        {
            return Schema.GetTable(TranslateTableName(tableName));
        }

        /// <summary>
        /// Регистрация поставщика метаданных в список информатора
        /// </summary>
        /// <param name="dictionary">Поставщик метаданных, типы которого требуют дополнительной информации</param>
        /// <param name="systemAssembly">Системная сборка, содержащая атрибуты схем для базовых типов DevExpress</param>
        public static void Register(XPDictionary dictionary, Assembly systemAssembly)
        {
            // Регистрация системной сборки, включающей атрибуты схем для DevExpress
            if (systemAssembly != null)
                instance.dictionary.ResolveSchemaName(systemAssembly, null);

            // Добавление уже существующих классов
            foreach (XPClassInfo info in dictionary.Classes) instance.AddInfo(info);
            dictionary.ClassInfoChanged += new ClassInfoEventHandler(instance.OnClassInfoChanged);
        }

        /// <summary>
        /// Регистрация поставщика метаданных в список информатора
        /// </summary>
        /// <param name="dictionary">Поставщик метаданных, типы которого требуют дополнительной информации</param>
        public static void Register(XPDictionary dictionary)
        {
            Register(dictionary, null);
        }

        /// <summary>
        /// Исключение поставщика метаданных из списка ифнорматора
        /// </summary>
        /// <param name="dictionary">Поставщик метаданных, типы которого больше не требуют дополнительной информации</param>
        public static void Unregister(XPDictionary dictionary)
        {
            dictionary.ClassInfoChanged -= new ClassInfoEventHandler(instance.OnClassInfoChanged);
        }

        /// <summary>
        /// Установка внешних определений схем базы данных
        /// </summary>
        /// <param name="tablePrefixes">Префиксы таблиц, заданные в свойстве TablePrefixes приложения Xaf</param>
        /// <param name="schemas">Внешние определения схем, заданные перечислением</param>
        /// <param name="overrideMode">Приоритет внешних определений схем: true - высокий (default), false - низкий</param>
        /// <remarks>Определения схем базы данных считываются из раздела конфигурации <b>persistentSchemas</b> и берутся из параметра <b>schemas</b>. 
        /// Если приоритет внешних определений схем базы данных низкий (overrideMode = false), 
        /// то сначала схемы определяются по атрибутам сборки <see cref="PersistentSchemaAttribute"/>, заданным в коде,
        /// а затем по определениям, заданным снаружи кода, т.е. в файле конфигурации и через параметр <b>schemas</b>.
        /// Если приоритет высокий (overrideMode = true, значение по умолчанию), то внешние определения учитываются первыми.
        /// <para>
        /// Основные классы DevExpress, необходимые во время обновления структуры базы данных, это 
        /// <c>DevExpress.ExpressApp.Xpo.ModuleInfo</c> и <c>DevExpress.Xpo.XPObjectType</c>. 
        /// Если в базе данных несколько приложений, то класс <c>DevExpress.ExpressApp.Xpo.ModuleInfo</c> 
        /// необходимо определить в отдельную схему для каждого приложения. (Также можно использовать префиксы таблиц для всего приложения
        /// (раздел конфигурации appSettings.TablePrefixes), но не рекомендуется.)
        /// Если вы используете утилиту обновления DBUpdater, то необходимо включить определения схем в конфигурационный файл утилиты.
        /// </para>
        /// </remarks>
        /// <example>Пример из конфигурационного файла с разделом определений схем базы данных:<code>
        /// <![CDATA[<configSections>]]>
        /// <![CDATA[  <section name="persistentSchemas" type="Aurum.Xpo.PersistentSchemasSection, Aurum.Xpo"/>]]>
        /// <![CDATA[</configSections>]]>
        /// ...
        /// <![CDATA[<persistentSchemas>]]>
        /// <![CDATA[  <schemas>]]>
        /// <![CDATA[    <definition schema="devexpress" namespace="DevExpress"/>]]>
        /// <![CDATA[    <definition schema="my_app" namespace=""/>]]>
        /// <![CDATA[  </schemas>]]>
        /// <![CDATA[</persistentSchemas>]]>
        /// </code></example>
        public static void SetupSchemaAttributes(string tablePrefixes, IEnumerable<PersistentSchemaElement> schemas = null, bool overrideMode = true)
        {
            List<PersistentSchemaAttribute> attributes = new List<PersistentSchemaAttribute>();
            PersistentSchemasSection section = (PersistentSchemasSection)ConfigurationManager.GetSection("persistentSchemas");
            if (section != null)
            {
                foreach (PersistentSchemaElement e in section.Schemas)
                    attributes.Add(new PersistentSchemaAttribute(e.Schema, e.Namespace));
            }
            if (schemas != null)
            {
                foreach (PersistentSchemaElement e in schemas)
                    attributes.Add(new PersistentSchemaAttribute(e.Schema, e.Namespace));
            }
            if (attributes.Count > 0)
                instance.dictionary.SetupExternalSchemaAttributes(attributes.ToArray(), tablePrefixes, overrideMode);
        }
    }

    /// <summary>
    /// Исключение, связанное с регистрацией классов метаданных в поставщике дополнительной информации
    /// </summary>
    public class XPDictionaryInformerException : Exception
    {
        /// <summary>Конструктор</summary>
        public XPDictionaryInformerException() { }

        /// <summary>Конструктор с сообщением</summary>
        /// <param name="message">Сообщение, описывающее исключение</param>
        public XPDictionaryInformerException(string message) : base(message) { }
    }

    /// <summary>
    /// Справочник расширенных метаданных
    /// </summary>
    class ReflectionDictionaryEx 
    {
        private Dictionary<Type, ReflectionClassInfoEx> classesByType = new Dictionary<Type, ReflectionClassInfoEx>();
        private DBSchema schema = new DBSchema();

        /// <summary>Справочник таблиц с дополнительной информацией о констрейнтах</summary>
        public DBSchema Schema { get { return schema; } }
        
        /// <summary>Глобальный справочник соответствий пространств имен и схем в базе данных</summary>
        private ResolveSchema schemasByNamespace = new ResolveSchema();
        
        /// <summary>Справочник соответствий сборок с пространствами имен и схем в базе данных</summary>
        private Dictionary<Assembly, ResolveSchema> schemasByAssembly = new Dictionary<Assembly, ResolveSchema>();
        
        /// <summary>Внешний справочник соответствий пространств имен и схем в базе данных</summary>
        private ResolveSchema schemasExternal = new ResolveSchema();

        /// <summary>Префиксы таблиц, заданные в свойстве TablePrefixes приложения Xaf</summary>
        private ResolveSchema tablePrefixes = new ResolveSchema();

        /// <summary>Режим приоритета справочников атрибутов схем</summary>
        /// <value>True - приоритет имеет внешний справочник атрибутов схем, 
        /// false - приоритет имеют атрибуты, указанные в сборках (default)</value>
        private bool schemasExternalPriority = false;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReflectionDictionaryEx() { }

        /// <summary>
        /// Возвращает расширенные метаданные класса
        /// </summary>
        /// <param name="classInfo">Исходные метаданные класса</param>
        /// <returns>Расширенные метаданные класса</returns>
        public ReflectionClassInfoEx GetClassInfoEx(XPClassInfo classInfo) 
        {
            ReflectionClassInfoEx result;
            if (!classesByType.TryGetValue(classInfo.ClassType, out result))
            {
                result = new ReflectionClassInfoEx(classInfo, this);
                classesByType.Add(classInfo.ClassType, result);
            }
            return result;
        }

        /// <summary>
        /// Определяет, содержит ли справочник метаданных информацию об указанном классе
        /// </summary>
        /// <param name="info">Информация о классе</param>
        /// <returns>True, если справочник уже включает в себя указанный класс, иначе false</returns>
        public bool Contains(XPClassInfo info)
        {
            return classesByType.ContainsKey(info.ClassType);
        }

        /// <summary>
        /// Определить название схемы по умолчанию для указанных сборки и пространства имен
        /// </summary>
        /// <param name="assembly">Cборка класса</param>
        /// <param name="nameSpace">Пространство имен класса</param>
        /// <returns>Название схемы по умолчанию или null, если нет ни одного названия для указанной сборки</returns>
        public string ResolveSchemaName(Assembly assembly, string nameSpace)
        {
            ResolveSchema rs;
            if (!schemasByAssembly.TryGetValue(assembly, out rs))
                lock (schemasByAssembly)
                {
                    rs = schemasByAssembly[assembly] = new ResolveSchema();
                    PersistentSchemaAttribute[] attributes = (PersistentSchemaAttribute[])assembly.GetCustomAttributes(typeof(PersistentSchemaAttribute), true);
                    rs.AddAttributes(attributes, false);
                    schemasByNamespace.AddAttributes(attributes, true);
                }
            return schemasExternalPriority ?
                (schemasExternal.Resolve(nameSpace) ?? rs.Resolve(nameSpace) ?? schemasByNamespace.Resolve(nameSpace)) :
                (rs.Resolve(nameSpace) ?? schemasByNamespace.Resolve(nameSpace) ?? schemasExternal.Resolve(nameSpace));
        }

        /// <summary>
        /// Определить префикс таблицы для указанного пространства имен
        /// </summary>
        /// <param name="nameSpace">Пространство имен класса</param>
        /// <returns>Префикс таблицы, заданный в свойстве TablePrefixes приложения Xaf</returns>
        public string ResolveTablePrefix(string nameSpace)
        {
            return tablePrefixes.Resolve(nameSpace);
        }

        /// <summary>
        /// Устанавливает внешние атрибуты схем базы данных
        /// </summary>
        /// <param name="attributes">Атрибуты схем базы данных, указанные из произвольного источника</param>
        /// <param name="tablePrefixes">Префиксы таблиц, заданные в свойстве TablePrefixes приложения Xaf</param>
        /// <param name="overrideMode">Приоритет использования внешних атрибутов: true - высокий, false - низкий</param>
        public void SetupExternalSchemaAttributes(PersistentSchemaAttribute[] attributes, string tablePrefixes, bool overrideMode)
        {
            // Очистка установленных значений
            schemasExternal.Clear();
            this.tablePrefixes.Clear();

            // Схемы базы данных
            schemasExternal.AddAttributes(attributes, false);
            schemasExternal.AddAttributes(attributes, true);
            this.schemasExternalPriority = overrideMode;

            // Префиксы таблиц
            if (!string.IsNullOrEmpty(tablePrefixes))
            {
                List<PersistentSchemaAttribute> list = new List<PersistentSchemaAttribute>();
                foreach (string pair in tablePrefixes.Split(';'))
                {
                    string value = pair != null ? pair.Trim() : pair;
                    if (string.IsNullOrEmpty(value)) continue;
                    int pos = value.IndexOf('=');
                    if (pos <= 0 || pos == value.Length - 1) continue;
                    string nameSpace = value.Substring(0, pos).Trim();
                    string prefix = value.Substring(pos + 1, value.Length - pos - 1).Trim();
                    list.Add(new PersistentSchemaAttribute(prefix, nameSpace));
                }
                this.tablePrefixes.AddAttributes(list.ToArray(), false);
            }
        }

        /// <summary>
        /// Служебный класс для определения схемы базы данных по умолчанию по атрибутам сборки
        /// </summary>
        protected class ResolveSchema
        {
            private string defaultSchema;
            private List<PersistentSchemaAttribute> namespaceSchemas;

            /// <summary>
            /// Добавляет указанные атрибуты для определения схемы базы данных
            /// </summary>
            /// <param name="attributes">Атрибуты сборки для определения схемы базы данных</param>
            /// <param name="isGlobal">True - только глобальные, false - только локальные</param>
            public void AddAttributes(PersistentSchemaAttribute[] attributes, bool isGlobal)
            {
                foreach (PersistentSchemaAttribute attribute in attributes)
                    if (attribute.IsGlobal == isGlobal)
                    {
                        if (string.IsNullOrEmpty(attribute.Namespace))
                            defaultSchema = attribute.Schema;
                        else
                        {
                            if (namespaceSchemas == null)
                                namespaceSchemas = new List<PersistentSchemaAttribute>();
                            namespaceSchemas.Add(attribute);
                        }
                    }
                if (namespaceSchemas != null)
                    namespaceSchemas.Sort((a, b) => -string.Compare(a.Namespace, b.Namespace));
            }

            /// <summary>
            /// Определяет схему по умолчанию для указанного пространства имен
            /// </summary>
            /// <param name="nameSpace">Пространство имен</param>
            /// <returns>Схема по умолчанию</returns>
            public string Resolve(string nameSpace)
            {
                if (!string.IsNullOrEmpty(nameSpace) && namespaceSchemas != null)
                    foreach (PersistentSchemaAttribute attribute in namespaceSchemas)
                        if (nameSpace.StartsWith(attribute.Namespace)) return attribute.Schema;
                return defaultSchema;
            }

            /// <summary>
            /// Очищает определения схемы
            /// </summary>
            public void Clear()
            {
                defaultSchema = null;
                if (namespaceSchemas != null)
                    namespaceSchemas.Clear();
            }
        }
    }

    /// <summary>
    /// Обеспечивает доступ к расширенным метаданным, получаемым через рефлексию
    /// </summary>
    /// <remarks>Метаданные, получаемые дополнительно к метаданным класса <c>ReflectionClassInfo</c>:
    /// <list type="bullet">
    /// <item><description>Информация о схемах сборки, указанных в атрибутах <c>PersistentSchemaAttribute</c></description></item>
    /// <item><description>Таблица с дополнительной информацией о констрейнтах <see cref="TableEx"/></description></item>
    /// </list>
    /// </remarks>
    class ReflectionClassInfoEx 
    {
        private Attribute[] allAttributes;
        private DBTableEx table;

        /// <summary>
        /// Все атрибуты класса, включая наследованные и однотипные
        /// </summary>
        /// <remarks>Свойство <see cref="DevExpress.Xpo.Metadata.XPTypeInfo.Attributes"/> содержит только по одному атрибуту каждого типа (кроме CustomAttribute) 
        /// и не включает наследованные атрибуты от базового класса. Для получения всех атрибутов класса используйте это свойство.</remarks>
        public Attribute[] AllAttributes
        {
            get { return allAttributes; }
        }

        /// <summary>
        /// Таблица с дополнительными данными
        /// </summary>
        /// <remarks>Таблица <b>TableEx</b> создается на основе свойства <see cref="DevExpress.Xpo.Metadata.XPClassInfo.Table"/>, но является отдельным объектом, 
        /// так как свойство не виртуальное и его нельзя переопределить. Для соответствия названий колонок в констрейнтах с колонками таблицы 
        /// используется <see cref="DBTableHelperEx"/>, дублирующий поведение класса <see cref="DBTableHelper"/> при обходе свойств.</remarks>
        public DBTableEx TableEx
        {
            get
            {
                if (table == null)
                {
                    if (TableName == null) return null;
                    lock (this) { if (table == null) CreateTable(); }
                }
                return table;
            }
        }

        // Создать таблицу с дополнительными данными о констрейнтах
        private void CreateTable()
        {
            table = new DBTableEx(TableName, sourceInfo.Table);

            // Констрейнты (констрейнты базовой таблицы не наследуются)
            DBTableHelperEx.ProcessClassInfo(table, this);

            // Настраиваемая модификация данных
            if (IsCustomPersistent)
            {
                ICustomPersistent customPersistent = GetCustomPersistent(null);
                if (customPersistent == null)
                    throw new InvalidOperationException(string.Format(
                        "Custom persistent controller of class {0} is failed", FullName));
                table.CustomPersistent = customPersistent;
            }
        }

        /// <summary>
        /// Возвращает таблицу с дополнительными данными для ассоциации many-to-many
        /// </summary>
        /// <param name="property">Свойство, устанавливающее ассоциацию</param>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Таблица для ассоциации many-to-many</returns>
        public DBTableEx GetIntermediateTableEx(ReflectionPropertyInfo property, string tableName)
        {
            DBTableEx table = new DBTableEx(tableName, property.IntermediateClass.Table);
            if (IsCustomPersistent)
            {
                string associationName = ((AssociationAttribute)property.Attributes.First(a => a is AssociationAttribute)).Name;
                table.CustomPersistent = GetCustomPersistent(associationName);
            }
            return table;
        }

        /// <summary>
        /// Название схемы таблицы
        /// </summary>
        public string SchemaName
        {
            get
            {
                if (TableName == null) return null;
                int dot = TableName.IndexOf('.');
                return dot < 0 ? null : TableName.Substring(0, dot);
            }
        }

        /// <summary>
        /// Название таблицы
        /// </summary>
        public string TableName
        {
            get
            {
                if (!tableNameInit)
                {
                    tableName = GetTableName();
                    tableNameInit = true;
                }
                return tableName;
            }
        }

        /// <summary>
        /// Получить название схемы по умолчанию
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDefaultSchemaName()
        {
            return dictionary.ResolveSchemaName(ClassType.Assembly, ClassType.Namespace);
        }

        /// <summary>
        /// Получить имя таблицы для класса объектов
        /// </summary>
        /// <returns>Имя таблицы вместе с названием схемы</returns>
        protected virtual string GetTableName()
        {
            string result = null;
            MapInheritanceAttribute mapInheritance = (MapInheritanceAttribute)sourceInfo.FindAttributeInfo(typeof(MapInheritanceAttribute));
            if (sourceInfo.IsPersistent && (mapInheritance == null || mapInheritance.MapType == MapInheritanceType.OwnTable))
            {
                PersistentAttribute persistent = (PersistentAttribute)sourceInfo.FindAttributeInfo(typeof(PersistentAttribute));
                string schema = GetDefaultSchemaName();
                string table = persistent != null && !string.IsNullOrEmpty(persistent.MapTo) ? persistent.MapTo : sourceInfo.ClassType.Name;
                result = (table.IndexOf('.') >= 0 || string.IsNullOrEmpty(schema)) ? table : string.Concat(schema, ".", table);
            }
            else if (baseClassEx != null) 
                result = baseClassEx.TableName;

            // Проверка наследования от класса с атрибутом CustomPersistent
            if (result != null)
            {
                ReflectionClassInfoEx parent = baseClassEx;
                while (parent != null)
                    if (parent.TableName == result && parent.IsCustomPersistent)
                        throw new Exception(string.Format("{0} is inherited from {1} with CustomPersistent and has the same table", FullName, parent.FullName));
                    else parent = parent.baseClassEx;
            }
            
            return result;
        }

        /// <summary>
        /// Указывает, что класс представляет объект базы данных с настраиваемым управлением данными
        /// </summary>
        public bool IsCustomPersistent
        {
            get { return TableName != null && allAttributes.FirstOrDefault(a => a is CustomPersistentAttribute) != null; }
        }

        /// <summary>
        /// Возвращает объект, осуществляющий настраиваемое управление данными
        /// </summary>
        /// <param name="associationName">Название ассоциации, если осуществляется управление данными ассоциации many-to-many</param>
        public ICustomPersistent GetCustomPersistent(string associationName)
        {
            object result = null;
            CustomPersistentAttribute attr = (CustomPersistentAttribute)allAttributes.FirstOrDefault(
                a => a is CustomPersistentAttribute && ((CustomPersistentAttribute)a).AssociationName == associationName);
            if (attr != null && attr.ControllerType != null)
                result = attr.ControllerType.GetConstructor(Type.EmptyTypes).Invoke(null);
            return result as ICustomPersistent;
        }

        private XPClassInfo sourceInfo;
        private ReflectionClassInfoEx baseClassEx;
        private ReflectionDictionaryEx dictionary;
        private string tableName;
        private bool tableNameInit = false;

        /// <summary>Расширенные метаданные базового класса</summary>
        public ReflectionClassInfoEx Base { get { return baseClassEx; } }

        /// <summary>Исходные метаданные класса</summary>
        public XPClassInfo SourceInfo { get { return sourceInfo; } }

        /// <summary>Тип класса</summary>
        public Type ClassType { get { return sourceInfo.ClassType; } }

        /// <summary>Полное название класса</summary>
        public string FullName { get { return sourceInfo.FullName; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="classInfo">Исходные метаданные класса</param>
        /// <param name="dictionary">Справочник расширенных метаданных</param>
        public ReflectionClassInfoEx(XPClassInfo classInfo, ReflectionDictionaryEx dictionary) 
        {
            this.sourceInfo = classInfo;
            this.dictionary = dictionary;
            this.baseClassEx = classInfo.BaseClass != null ? dictionary.GetClassInfoEx(classInfo.BaseClass) : null;

            // Все атрибуты класса, включая наследованные и однотипные
            object[] attributes = ClassType.GetCustomAttributes(true);
            allAttributes = new Attribute[attributes != null ? attributes.Length : 0];
            if (attributes != null && attributes.Length > 0)
                Array.Copy(attributes, allAttributes, attributes.Length);
        }
    }
}
