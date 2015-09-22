using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Aurum.Xpo
{
    /// <summary>
    /// Секция конфигурационного файла с определениями схем базы данных
    /// </summary>
    public sealed class PersistentSchemasSection : ConfigurationSection
    {
        /// <summary>
        /// Коллекция определений схем базы данных
        /// </summary>
        [ConfigurationProperty("schemas", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(PersistentSchemaCollection), AddItemName="definition")]
        public PersistentSchemaCollection Schemas
        {
            get
            {
                PersistentSchemaCollection schemas = (PersistentSchemaCollection)base["schemas"];
                return schemas;
            }
        }
    }

    /// <summary>
    /// Коллекция конфигурационного файла с определениями схем базы данных
    /// </summary>
    public sealed class PersistentSchemaCollection : ConfigurationElementCollection
    {
        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PersistentSchemaElement();
        }

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PersistentSchemaElement)element).Namespace;
        }

        /// <summary>
        /// Возвращает или устанавливает элемент определения схемы базы данных по указанному индексу
        /// </summary>
        /// <param name="index">Индекс коллекции</param>
        /// <returns>Элемент определения схемы базы данных</returns>
        public PersistentSchemaElement this[int index]
        {
            get
            {
                return (PersistentSchemaElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null) BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
    }

    /// <summary>
    /// Элемент конфигурационного файла с определением схемы базы данных
    /// </summary>
    public sealed class PersistentSchemaElement : ConfigurationElement
    {
        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public PersistentSchemaElement() { }

        /// <summary>
        /// Конструктор с указанием схемы базы данных и пространства имен классов
        /// </summary>
        /// <param name="schema">Схема базы данных</param>
        /// <param name="nameSpace">Пространство имен классов</param>
        public PersistentSchemaElement(string schema, string nameSpace)
        {
            this.Schema = schema;
            this.Namespace = nameSpace;
        }

        /// <summary>
        /// Пространство имен классов
        /// </summary>
        [ConfigurationProperty("namespace", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Namespace
        {
            get { return (string)this["namespace"]; }
            set { this["namespace"] = value; }
        }

        /// <summary>
        /// Схема базы данных
        /// </summary>
        [ConfigurationProperty("schema", IsRequired = true)]
        public string Schema
        {
            get { return (string)this["schema"]; }
            set { this["schema"] = value; }
        }
    }
}
