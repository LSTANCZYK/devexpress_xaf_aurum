using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Aurum.Xpo
{
    /// <summary>
    /// Справочник запросов
    /// </summary>
    /// <remarks>Справочник запросов используется для преобразования оригинального текста запросов для их оптимизации</remarks>
    public class SqlDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Справочник по умолчанию
        /// </summary>
        public static SqlDictionary Default { get; set; }

        /// <summary>
        /// Возвращает преобразованный запрос
        /// </summary>
        /// <param name="sql">Текст оригинального запроса</param>
        /// <returns>Текст преобразованного запроса из справочника запросов</returns>
        public string TransformSql(string sql)
        {
            string result;
            return TryGetValue(sql, out result) ? result : sql;
        }

        /// <summary>
        /// Загрузка из указанного справочника
        /// </summary>
        /// <param name="dictionary">Справочник запросов</param>
        /// <exception cref="ArgumentNullException">Не указан справочник</exception>
        /// <remarks>Ключ справочника должен содержать оригинальный запрос, а значение - преобразованный запрос</remarks>
        public void LoadFrom(Dictionary<string, string> dictionary)
        {
            if (dictionary == null) 
                throw new ArgumentNullException("dictionary");
            foreach (var kv in dictionary)
                if (!ContainsKey(kv.Key)) Add(kv.Key, kv.Value);
        }

        /// <summary>
        /// Загрузка из текстового потока
        /// </summary>
        /// <param name="reader">Текстовый поток с запросами</param>
        /// <param name="delimiter">Разделитель запросов</param>
        /// <exception cref="ArgumentNullException">Не указан текстовый поток</exception>
        /// <remarks>Оригинальный и преобразованный запросы должны идти друг за другом и разделяться разделителем <b>delimiter</b>:
        /// <![CDATA[
        /// Оригинальный запрос 1
        /// delimiter
        /// Преобразованный запрос 1
        /// delimiter
        /// Оригинальный запрос 2
        /// delimiter
        /// Преобразованный запрос 2
        /// ...
        /// ]]></remarks>
        public void LoadFrom(StreamReader reader, string delimiter)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            Clear();
            StringBuilder sb1 = new StringBuilder(), sb2 = new StringBuilder(); bool first = true;
            while (!reader.EndOfStream)
            {
                string s = reader.ReadLine();
                if ((s != null && s.StartsWith(delimiter)) || reader.EndOfStream)
                {
                    if (!first && sb1.Length > 0 && sb2.Length > 0)
                    {
                        string s1 = sb1.ToString().Trim();
                        string s2 = sb2.ToString().Trim();
                        if (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2) && !ContainsKey(s1))
                            Add(s1, sb2.ToString());
                        sb1.Length = 0; sb2.Length = 0;
                    }
                    first = !first;
                }
                else if (first) sb1.Append(s); else sb2.Append(s);
            }
        }

        /// <summary>
        /// Загрузка из xml-файла
        /// </summary>
        /// <param name="reader">Xml-файл с запросами</param>
        /// <exception cref="ArgumentNullException">Не указан xml-файл</exception>
        /// <remarks>Оригинальный и преобразованный запросы должны идти друг за другом в элементах &lt;sql&gt; и &lt;replace&gt;:
        /// <![CDATA[
        /// <sql>Оригинальный запрос 1</sql>
        /// <replace>Преобразованный запрос 1</replace>
        /// <sql>Оригинальный запрос 2</sql>
        /// <replace>Преобразованный запрос 2</replace>
        /// ...
        /// ]]></remarks>
        public void LoadFrom(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            Clear();
            string sql = null, replace = null;
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string name = reader.Name != null ? reader.Name.ToLower() : string.Empty;
                    switch (name)
                    {
                        case "sql": 
                            sql = GetText(reader); 
                            break;
                        case "replace":
                            replace = GetText(reader);
                            if (!string.IsNullOrEmpty(sql) && !string.IsNullOrEmpty(replace) && !ContainsKey(sql))
                                Add(sql, replace);
                            sql = null; replace = null;
                            break;
                    }
                }
        }

        private string GetText(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement)
            {
                StringBuilder sb = new StringBuilder();
                while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement))
                    if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA) sb.Append(reader.Value);
                return sb.ToString();
            }
            return null;
        }
    }
}
