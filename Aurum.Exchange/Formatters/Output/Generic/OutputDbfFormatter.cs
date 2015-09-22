using Aurum.Exchange.Lib;
using Aurum.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Exchange
{
    /// <summary>
    /// Форматирование в виде dbf файла
    /// </summary>
    public abstract class OutputDbfFormatter<TDataClass> : OutputFormatter<TDataClass>
    {
        private static readonly Encoding Windows1251 = Encoding.GetEncoding(1251);
        private static readonly Encoding CP866 = Encoding.GetEncoding(866);

        private string fileDBF;
        private string fileDBT;
        private Stream streamDBF;
        private Stream streamDBT;
        private FileMode fileMode;
        private DbfWriter writer;
        private List<DbfColumn> dbfColumns = new List<DbfColumn>();

        /// <summary>
        /// Использовать кодировку Windows-1251 (по-умочанию используется кодировка CP866)
        /// </summary>
        public bool UseEncoding1251
        {
            get;
            set;
        }

        /// <summary>
        /// DbfWriter
        /// </summary>
        protected DbfWriter Writer
        {
            get { return writer; }
        }

        /// <summary>
        /// Имеется ли MEMO-поле
        /// </summary>
        /// <remarks>MEMO-поля хранятся в файле DBT</remarks>
        /// <returns></returns>
        protected bool HasMemo()
        {
            return dbfColumns.Where(c => c is DbfColumnMemo).Count() > 0;
        }

        /// <summary>
        /// Добавление колонки dbf
        /// </summary>
        /// <param name="column">Колонка</param>
        public void AddDbfColumn(DbfColumn column)
        {
            if (column == null)
                throw new ArgumentNullException("Column");
            dbfColumns.Add(column);
        }

        private void OpenDbfWriter()
        {
            if (string.IsNullOrEmpty(fileDBF))
                throw new InvalidOperationException("Не указан выходной файл");
            if (!fileDBF.ToLower().EndsWith(".dbf"))
                throw new InvalidOperationException("Указано неверное расширение для выходного файла");
            
            streamDBF = new FileStream(fileDBF, fileMode, FileAccess.ReadWrite, FileShare.None);
            if (HasMemo())
            {
                this.fileDBT = fileDBF.Substring(0, fileDBF.ToLower().LastIndexOf(".dbf")) + ".dbt";
                streamDBT = new FileStream(fileDBT, fileMode, FileAccess.ReadWrite, FileShare.None);
            }

            writer = new DbfWriter();
            writer.Open(streamDBF, streamDBT, UseEncoding1251 ? Windows1251 : CP866, dbfColumns.ToArray());
        }

        private void CloseDbfWriter()
        {
            writer.Close();
        }

        protected internal override void OnBeforeFormatting()
        {
            OpenDbfWriter();
            base.OnBeforeFormatting();
        }

        protected internal override void OnAfterFormatting()
        {
            base.OnAfterFormatting();
            CloseDbfWriter();
        }

        /// <summary>
        /// Обработка данных
        /// </summary>
        /// <param name="objects">Данные</param>
        /// <param name="interop">Посредник между операцией и менеджером операции</param>
        protected override void CustomFormat(IEnumerable<TDataClass> objects, OperationInterop interop)
        {
            if (objects == null) return;
            foreach (var record in objects)
            {
                writer.WriteRecord(FormatRecord(record, interop));
            }
        }

        /// <summary>
        /// Обработка записи данных
        /// </summary>
        /// <param name="record">Запись данных</param>
        /// <param name="interop">Посредник между операцией и менеджером операции</param>
        /// <returns></returns>
        protected abstract List<object> FormatRecord(TDataClass record, OperationInterop interop);

        /// <summary>
        /// Установить файловый вывод
        /// </summary>
        /// <param name="file">Путь к файлу</param>
        /// <param name="fileMode">Режим открытия файла</param>
        public override void AddFile(string file, FileMode fileMode = FileMode.Create)
        {
            this.fileDBF = file;
            this.fileMode = fileMode;
        }

        public override void Dispose()
        {
            if (streamDBF != null)
            {
                streamDBF.Dispose();
                streamDBF = null;
            }
            if (streamDBT != null)
            {
                streamDBT.Dispose();
                streamDBT = null;
            }
            base.Dispose();
        }
    }
}
