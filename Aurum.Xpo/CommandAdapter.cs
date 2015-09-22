using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;

namespace Aurum.Xpo
{
    /// <summary>
    /// Адаптер команды базы данных 
    /// </summary>
    /// <remarks>Адаптер перехватывает обработку оригинальной команды базы данных и позволяет ее переопределить.
    /// Все методы и свойства интерфейса <see cref="IDbCommand"/>, реализованные в <b>CommandAdapter</b> виртуальные и дублируют поведение
    /// оригинальной команды, доступном в свойстве <see cref="Command"/></remarks>
    public class CommandAdapter : IDbCommand
    {
        private readonly IDbCommand command;

        /// <summary>
        /// Оригинальная команда базы данных
        /// </summary>
        protected IDbCommand Command
        {
            get { return command; }
        }

        /// <summary>
        /// Конструктор адаптера на основе оригинальной команды базы данных
        /// </summary>
        /// <param name="command">Оригинальная команда базы данных</param>
        /// <exception cref="ArgumentNullException">Вызывается, если не указана оригинальная команда базы данных</exception>
        public CommandAdapter(IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException();

            this.command = command;
        }

        /// <contentfrom cref="IDbCommand.Cancel" />
        public virtual void Cancel()
        {
            command.Cancel();
        }

        /// <contentfrom cref="IDbCommand.CommandText" />
        public virtual string CommandText
        {
            get
            {
                return command.CommandText;
            }
            set
            {
                command.CommandText = value;
            }
        }

        /// <contentfrom cref="IDbCommand.CommandTimeout" />
        public virtual int CommandTimeout
        {
            get
            {
                return command.CommandTimeout;
            }
            set
            {
                command.CommandTimeout = value;
            }
        }

        /// <contentfrom cref="IDbCommand.CommandType" />
        public virtual CommandType CommandType
        {
            get
            {
                return command.CommandType;
            }
            set
            {
                command.CommandType = value;
            }
        }

        /// <contentfrom cref="IDbCommand.Connection" />
        public virtual IDbConnection Connection
        {
            get
            {
                return command.Connection;
            }
            set
            {
                command.Connection = value;
            }
        }

        /// <contentfrom cref="IDbCommand.CreateParameter" />
        public virtual IDbDataParameter CreateParameter()
        {
            return command.CreateParameter();
        }

        /// <contentfrom cref="IDbCommand.ExecuteNonQuery" />
        public virtual int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        /// <contentfrom cref="IDbCommand.ExecuteReader(CommandBehavior)" />
        public virtual IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return command.ExecuteReader(behavior);
        }

        /// <contentfrom cref="IDbCommand.ExecuteReader()" />
        public virtual IDataReader ExecuteReader()
        {
            return command.ExecuteReader();
        }

        /// <contentfrom cref="IDbCommand.ExecuteScalar" />
        public virtual object ExecuteScalar()
        {
            return command.ExecuteScalar();
        }

        /// <contentfrom cref="IDbCommand.Parameters" />
        public virtual IDataParameterCollection Parameters
        {
            get { return command.Parameters; }
        }

        /// <contentfrom cref="IDbCommand.Prepare" />
        public virtual void Prepare()
        {
            command.Prepare();
        }

        /// <contentfrom cref="IDbCommand.Transaction" />
        public virtual IDbTransaction Transaction
        {
            get
            {
                return command.Transaction;
            }
            set
            {
                command.Transaction = value;
            }
        }

        /// <contentfrom cref="IDbCommand.UpdatedRowSource" />
        public virtual UpdateRowSource UpdatedRowSource
        {
            get
            {
                return command.UpdatedRowSource;
            }
            set
            {
                command.UpdatedRowSource = value;
            }
        }

        /// <contentfrom cref="IDisposable.Dispose" />
        public virtual void Dispose()
        {
            command.Dispose();
        }
    }

    /// <summary>
    /// Адаптер команды базы данных, позволяющий только чтение данных
    /// </summary>
    /// <remarks>Адаптер команды <c>CommandReadOnly</c> исключает все операции модификации данных и изменения структуры
    /// с помощью переопределения метода <c>ExecuteNonQuery</c></remarks>
    public class CommandReadOnly : CommandAdapter
    {
        /// <summary>
        /// Конструктор адаптера на основе оригинальной команды базы данных
        /// </summary>
        /// <param name="command">Оригинальная команда базы данных</param>
        public CommandReadOnly(IDbCommand command)
            : base(command)
        {
        }

        /// <inheritdoc/>
        public override int ExecuteNonQuery()
        {
            foreach (IDataParameter p in Parameters)
                if (((p.Direction & ParameterDirection.Output) == ParameterDirection.Output) && !p.IsNullable)
                    p.Value = GetParameterDefaultValue(p);
            return 0;
        }

        /// <summary>
        /// Устанавливает значение по умолчанию для параметра команды с обязательным значением.
        /// <seealso cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="parameter">Параметр команды базы данных, для которого устанавливается значение</param>
        /// <returns>Возвращаемое значение по умолчанию:
        /// <list type="bullet">
        /// <item><description><c>string.Empty</c> для строковых типов</description></item>
        /// <item><description><c>DateTime.MinValue</c> для типов с датой и/или временем</description></item>
        /// <item><description><c>false</c> для <c>DbType.Boolean</c></description></item>
        /// <item><description>0 для числовых типов</description></item>
        /// </list></returns>
        public virtual object GetParameterDefaultValue(IDataParameter parameter)
        {
            if (parameter.IsNullable)
                return null;
            switch (parameter.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return string.Empty;
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return DateTime.MinValue;
                case DbType.Boolean:
                    return false;
                case DbType.Binary:
                case DbType.Object:
                case DbType.Xml:
                    return null;
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Адаптер команды, записывающий скрипт вместо выполнения модификации данных или изменения структуры
    /// </summary>
    public class CommandScript : CommandReadOnly
    {
        private StreamWriter writer;
        private bool writeQueries = false;

        /// <summary>
        /// Поток для записи скрипта
        /// </summary>
        public StreamWriter Writer
        {
            get { return writer; }
            set { writer = value; }
        }

        /// <summary>
        /// Обработчик события начала записи команды
        /// </summary>
        public EventHandler BeforeWrite { get; set; }

        /// <summary>
        /// Обработчик события завершения записи команды
        /// </summary>
        public EventHandler AfterWrite { get; set; }

        /// <summary>
        /// Флаг записи операций запроса данных
        /// </summary>
        public bool WriteQueries
        {
            get { return writeQueries; }
            set { writeQueries = value; }
        }

        /// <summary>
        /// Конструктор адаптера на основе оригинальной команды базы данных с указанием потока для записи скрипта
        /// </summary>
        /// <param name="command">Оригинальная команда базы данных</param>
        /// <param name="writer">Поток для записи скрипта</param>
        public CommandScript(IDbCommand command, StreamWriter writer)
            : base(command)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Переопределяет выполнение команды базы данных по модификации данных или изменению структуры.
        /// Вместо модификации данных или изменения структуры выполняется запись скрипта команды (<see cref="WriteScript"/>).
        /// </summary>
        /// <returns>Возвращает 0</returns>
        public override int ExecuteNonQuery()
        {
            WriteScript();
            return base.ExecuteNonQuery();
        }

        /// <summary>
        /// Переопределяет выполнение команды базы данных по чтению данных, дополнительно записывая команду в скрипт (<see cref="WriteScript"/>).
        /// </summary>
        public override IDataReader ExecuteReader()
        {
            if (writeQueries) WriteScript();
            return base.ExecuteReader();
        }

        /// <summary>
        /// Переопределяет выполнение команды базы данных по чтению данных, дополнительно записывая команду в скрипт (<see cref="WriteScript"/>).
        /// </summary>
        public override IDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (writeQueries) WriteScript();
            return base.ExecuteReader(behavior);
        }

        /// <summary>
        /// Переопределяет выполнение команды базы данных по чтению данных, дополнительно записывая команду в скрипт (<see cref="WriteScript"/>).
        /// </summary>
        public override object ExecuteScalar()
        {
            if (writeQueries) WriteScript();
            return base.ExecuteScalar();
        }

        /// <summary>
        /// Получить символ завершения команды
        /// </summary>
        public virtual string GetCommandTerminator(string commandText)
        {
            return string.Empty;
        }

        /// <summary>
        /// Запись скрипта команды в поток записи, определенном в свойстве <c>Writer</c>
        /// </summary>
        public virtual void WriteScript()
        {
            if (writer == null) return;
            if (BeforeWrite != null) BeforeWrite(this, EventArgs.Empty);
            writer.Write(CommandText);
            writer.Write(GetCommandTerminator(CommandText));
            writer.WriteLine();
            if (AfterWrite != null) AfterWrite(this, EventArgs.Empty);
        }
    }
}
