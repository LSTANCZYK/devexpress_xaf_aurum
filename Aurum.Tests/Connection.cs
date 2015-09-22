using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DevExpress.Xpo.DB;

namespace Aurum.Tests
{
    /// <summary>
    /// Соединение для тестов
    /// </summary>
    static class Connection
    {
        private static IDbConnection connection;

        /// <summary>
        /// Инициализация
        /// </summary>
        public static void Initialize(string connectionString)
        {
            if (connection == null)
            {
                connection = ODPConnectionProvider.CreateConnection(connectionString);
                connection.Open();
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public static void Cleanup()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Выполнить блок
        /// </summary>
        /// <param name="text">Текс команды</param>
        public static void ExecuteBlock(string text)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = text;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /// <summary>
        /// Выполнить скрипт
        /// </summary>
        /// <param name="text">Текст скрипта</param>
        public static void ExecuteScript(string text)
        {
            foreach (string block in text.Split('/'))
            {
                List<string> commands = new List<string>();
                string lower = block.ToLower();
                int posPackage = lower.IndexOf("package");
                int posProcedure = lower.IndexOf("procedure");
                int posDeclare = lower.IndexOf("declare");
                int posBegin = lower.IndexOf("begin");
                int posStart = posDeclare >= 0 ? posDeclare : posBegin;
                if (posPackage >= 0 || posProcedure >= 0)
                    posStart = lower.LastIndexOf("create", posPackage >= 0 ? posPackage : posProcedure);
                if (posStart >= 0)
                {
                    if (posStart > 0)
                        commands.AddRange(block.Substring(0, posStart - 1).Split(';'));
                    commands.Add(block.Substring(posStart));
                }
                else
                    commands.AddRange(block.Split(';'));
                foreach (string command in commands)
                {
                    string commandText = command.Trim();
                    if (string.IsNullOrEmpty(commandText)) continue;
                    ExecuteBlock(commandText);
                }
            }
        }

        /// <summary>
        /// Выполнить скрипт, указанный в файле
        /// </summary>
        /// <param name="path">Путь к файлу скрипта</param>
        public static void ExecuteFile(string path)
        {
            StreamReader file = new StreamReader(path);
            string text = file.ReadToEnd();
            file.Close();
            ExecuteScript(text);
        }
    }
}
