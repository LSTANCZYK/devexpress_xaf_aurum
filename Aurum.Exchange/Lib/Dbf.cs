using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Aurum.Exchange.Lib
{
    /// <summary>
    /// ����� ����������� ������ � Dbf ������.
    /// ���������������� ������
    /// </summary>
    public class DbfWriter
    {
        // �������� ����� dbf
        private Stream fsDBF;
        // �������� ����� dbt
        private Stream fsDBT;
        // ���� ��������
        private bool isOpened;
        // ���������
        private DbfHeader header;
        // �������
        private List<DbfColumn> columns;
        // ����������
        private Encoder encoder;

        /// <summary>
        /// �����������
        /// </summary>
        public DbfWriter()
        {
            fsDBF = null;
            fsDBT = null;
            header = null;
            columns = null;
            isOpened = false;
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        public void Open(Stream streamDBF, Stream streamDBT, Encoding encoding, params DbfColumn[] cols)
        {
            if (isOpened)
                return;
            if (streamDBF == null)
                throw new ArgumentNullException("streamDBF");
            if (encoding == null || (encoding.CodePage != 866 && encoding.CodePage != 1251))
                throw new ArgumentException("Encoding");
            if (cols == null && cols.Length == 0)
                throw new ArgumentNullException("Columns");

            fsDBF = streamDBF;

            if (encoding.CodePage == 1251)
            {
                header = new DbfHeader(0x57);   //1251
            }
            else
            {
                header = new DbfHeader(0x26);   //866
            }
            encoder = encoding.GetEncoder();

            columns = new List<DbfColumn>();
            bool hasMemo = false;
            foreach (DbfColumn col in cols)
            {
                col.SetEncodedName(encoder);
                if (col is DbfColumnMemo)
                {
                    hasMemo = true;
                }
                columns.Add(col);
            }
            header.HasMemo = hasMemo;

            if (hasMemo)
            {
                fsDBT = streamDBT;
                foreach (DbfColumn col in cols)
                {
                    if (col is DbfColumnMemo)
                    {
                        ((DbfColumnMemo)col).SetStream(fsDBT);
                    }
                }
            }

            // ������ ��������� (32 - ��������� DbfHeader, 32 - ��������� DbfFieldDescriptor, 1 - ����������� ����)
            int headerSize = 32 + 32 * columns.Count + 1;
            if (headerSize > Int16.MaxValue)
                throw new Exception("Too many columns");
            header.HeaderSize = (Int16)headerSize;

            // ������ ������ (1 - ���� ����� ��������, ��������� - ���� ������)
            int recordSize = 1;
            foreach (DbfColumn col in columns)
            {
                recordSize += col.FieldDescriptor.fieldLen;
            }
            if (recordSize > Int16.MaxValue)
                throw new Exception("Too long record size");
            header.RecordSize = (Int16)recordSize;
            isOpened = true;

            WriteHeader();
            WriteColumns();

            if (hasMemo)
            {
                DbfColumnMemo.count = 0;
                WriteDbtHeader();
                DbfColumnMemo.count = 1;
            }
        }

        // ������ ���������
        private void WriteHeader()
        {
            if (!isOpened)
                throw new Exception("File is not opened!");
            fsDBF.Seek(0, SeekOrigin.Begin);

            byte[] h = new byte[32];

            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(header.Header));
            Marshal.StructureToPtr(header.Header, pnt, false);
            Marshal.Copy(pnt, h, 0, 32);
            Marshal.FreeHGlobal(pnt);

            fsDBF.Write(h, 0, 32);
        }

        // ������ �������� �������
        private void WriteColumns()
        {
            if (!isOpened)
                throw new Exception("File is not opened!");
            fsDBF.Seek(32, SeekOrigin.Begin);

            byte[] c = new byte[32];
            foreach (DbfColumn col in columns)
            {
                IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(col.FieldDescriptor));
                Marshal.StructureToPtr(col.FieldDescriptor, pnt, false);
                Marshal.Copy(pnt, c, 0, 32);
                Marshal.FreeHGlobal(pnt);

                fsDBF.Write(c, 0, 32);
            }
            fsDBF.WriteByte(0x0D); // ����� ����������
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="record">������</param>
        public int WriteRecord(List<object> record)
        {
            if (!isOpened)
                throw new Exception("File is not opened!");
            if (record == null || record.Count == 0)
                return 0;

            if (record.Count % columns.Count != 0)
                throw new InvalidOperationException("���-�� ����� ������ �� ��������� � ���-��� ������� � dbf �����");

            for (int j = 0; j < record.Count / columns.Count; j++)
            {
                fsDBF.WriteByte(0x20); // ���� �������� �����������
                int l = 1;
                for (int i = 0; i < columns.Count; i++)
                {
                    byte[] b = columns[i].ObjectToBytes(record[j * columns.Count + i], encoder);
                    if (b == null || b.Length == 0 || b.Length != columns[i].FieldDescriptor.fieldLen)
                        throw new Exception("Dbf: Internal error 1");
                    l += b.Length;
                    fsDBF.Write(b, 0, b.Length);
                }
                if (l != header.RecordSize)
                    throw new Exception("Dbf: Internal error 2");
            }

            var written = record.Count / columns.Count;
            header.RecordsCount += written;
            return written;
        }

        // ������ ��������� dbt
        private void WriteDbtHeader()
        {
            if (!isOpened)
                throw new Exception("File is not opened!");
            fsDBT.Seek(0, SeekOrigin.Begin);

            byte[] h = new byte[512];

            // � ����, ������������ � ������� ������� �����,
            // ������� ����� �����, ��������������� ������ ���������
            // ������� � �����.
            // ���� ����� ����� �������� � ������ ����
            // ������ � �������� �������
            if (DbfColumnMemo.count > 0 && DbfColumnMemo.count < 65536)
            {
                UInt16 cnt = (UInt16)DbfColumnMemo.count;
                var b = BitConverter.GetBytes(cnt);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(b);
                }
                h[0] = b[0];
                h[1] = b[1];
            }

            fsDBT.Write(h, 0, 512);
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        public void Close()
        {
            if (!isOpened)
                return;
            try
            {
                if (fsDBF != null)
                {
                    fsDBF.WriteByte(0x1A); // EOF
                    WriteHeader();
                    fsDBF.Flush();
                }
                if (fsDBT != null)
                {
                    WriteDbtHeader();
                    fsDBT.Flush();
                }
            }
            catch
            {
            }
            finally
            {
                fsDBF = null;
                fsDBT = null;
                header = null;
                columns = null;
                encoder = null;
                isOpened = false;
            }
        }
    }

    /// <summary>
    /// ����� ����������� ������ � Dbf ������
    /// </summary>
    public class DbfReader
    {
        // �������� �����
        private BinaryReader br;
        // ���� ��������
        private bool isOpened;

        private DbfHeaderStruct? header;

        private Encoding encoding;

        private List<DbfFieldDescriptor> fields;

        // ����� ������� ������
        private int i;

        // ������ ����������� �����
        private static NumberFormatInfo numberFormat = null;

        /// <summary>
        /// ������ ����������� ������� �����
        /// </summary>
        static NumberFormatInfo DbfNumberFormat
        {
            get
            {
                if (numberFormat == null)
                {
                    numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = ".";
                }
                return numberFormat;
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        public DbfReader()
        {
            br = null;
            isOpened = false;
        }

        /// <summary>
        /// ���������
        /// </summary>
        public DbfHeaderStruct Header
        {
            get { return header.Value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public List<DbfFieldDescriptor> Fields
        {
            get { return fields; }
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        public List<string> Columns
        {
            get
            {
                List<string> columns = new List<string>();
                foreach (DbfFieldDescriptor field in fields)
                {
                    string fieldName = decodeString(field.fieldName);
                    columns.Add(fieldName);
                }
                return columns;
            }
        }

        /// <summary>
        /// ����� ������� ������
        /// </summary>
        public int I
        {
            get { return i; }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        public long FileSize
        {
            get
            {
                if (!isOpened)
                    throw new InvalidOperationException("File is not opened!");
                return br.BaseStream.Length;
            }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        public uint RowsCount
        {
            get
            {
                return header.HasValue ? (uint)header.Value.numRecords : 0;
            }
        }

        /// <summary>
        /// ��������� ����
        /// </summary>
        /// <returns></returns>
        public long BytesRead()
        {
            if (!isOpened)
                throw new InvalidOperationException("File is not opened!");
            return br.BaseStream.Position;
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        /// <param name="file">���� � �����</param>
        /// <param name="en">��������� (���� �� �������, �� ������� �� ���������</param>
        /// <returns></returns>
        public bool Open(string file, Encoding en)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");
            string filePath = Path.GetFullPath(file);
            return Open(File.OpenRead(filePath), en);
        }

        /// <summary>
        /// �������� �����
        /// </summary>
        /// <param name="stream">������� �����</param>
        /// <param name="en">��������� (���� �� �������, �� ������� �� ���������</param>
        /// <returns></returns>
        public bool Open(Stream stream, Encoding en)
        {
            if (isOpened)
                return false;            
            try
            {
                // ������ ��������� � �����
                br = new BinaryReader(stream);
                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DbfHeaderStruct)));

                // ��������� ��������� � DBFHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                header = (DbfHeaderStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DbfHeaderStruct));
                handle.Free();

                if (en != null)
                    encoding = en;
                else
                {
                    // ��������� �� ���������
                    if (header.Value.language == 0x57 || header.Value.language == 0xc9)    // 1251
                    {
                        encoding = Encoding.GetEncoding(1251);
                    }
                    else // 0x26 - 866, 0x65 - 866
                    {
                        encoding = Encoding.GetEncoding(866);
                    }
                }

                // ������ ���� �����. �� ������������ ���� 0x0D (13) �������� ��������������
                fields = new List<DbfFieldDescriptor>();
                while ((13 != br.PeekChar()))
                {
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof(DbfFieldDescriptor)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    fields.Add((DbfFieldDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DbfFieldDescriptor)));
                    handle.Free();
                }

                // ������� ����������
                ((FileStream)br.BaseStream).Seek(header.Value.headerLen, SeekOrigin.Begin);

                i = -1;
                isOpened = true;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        public void Close()
        {
            if (!isOpened)
                return;
            try
            {
                if (br != null)
                {
                    br.Close();
                }
            }
            catch
            {
            }
            finally
            {
                br = null;
                header = null;
                encoding = null;
                fields = null;
                i = -1;
                isOpened = false;
            }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ReadRecord()
        {
            if (!isOpened)
                throw new Exception("Dbf is not opened");
            while (true)
            {
                if (i + 1 >= header.Value.numRecords)
                    return null;

                Dictionary<string, object> result = new Dictionary<string, object>();

                // ������ ��������� ��� ������ � �����, � ����� ������ ������ ���� �� ������
                byte[] buffer = br.ReadBytes(header.Value.recordLen);
                BinaryReader recReader = new BinaryReader(new MemoryStream(buffer));
                byte[] deleted = recReader.ReadBytes(1);
                if (deleted[0] != 0x2A)	// ������ �� �������
                {
                    // ���� �� ���� ����� ������
                    foreach (DbfFieldDescriptor field in fields)
                    {
                        string fieldName = decodeString(field.fieldName);
                        switch (field.fieldType)
                        {
                            case 'N':  // ����� (Number)
                                {
                                    byte[] b = recReader.ReadBytes(field.fieldLen);
                                    string number = decodeString(b);
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(number))
                                            result[fieldName] = decimal.Parse(number, System.Globalization.NumberStyles.Number, DbfNumberFormat);
                                        else
                                            result[fieldName] = null;
                                    }
                                    catch
                                    {
                                        result[fieldName] = null;
                                    }
                                    break;
                                }
                            case 'C': // ������
                                {
                                    byte[] b = recReader.ReadBytes(field.fieldLen);
                                    string value = decodeString(b);
                                    result[fieldName] = value;
                                    break;
                                }
                            case 'D': // ���� (Date) (YYYYMMDD)
                                {
                                    string year = decodeString(recReader.ReadBytes(4));
                                    string month = decodeString(recReader.ReadBytes(2));
                                    string day = decodeString(recReader.ReadBytes(2));
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(day))
                                            result[fieldName] = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                                        else
                                            result[fieldName] = null;
                                    }
                                    catch
                                    {
                                        result[fieldName] = null;
                                    }
                                    break;
                                }
                            case 'L': // Boolean (Y/N)
                                {
                                    byte b = recReader.ReadByte();
                                    if (b == 'Y' || b == 'y' || b == 'T' || b == 't')
                                    {
                                        result[fieldName] = true;
                                    }
                                    else if (b == 'N' || b == 'n' || b == 'F' || b == 'f')
                                    {
                                        result[fieldName] = false;
                                    }
                                    else
                                    {
                                        result[fieldName] = null;
                                    }
                                    break;
                                }
                            case 'F':
                                {
                                    // Float	ASCII �������(-.0123456789)
                                    // ���������� ������� ��������� �����
                                    // n = 1..20
#if DEBUG
                                    throw new NotImplementedException();
#else
                            break;
#endif
                                }
                            case 'M':
                                {
                                    // Memo	10 ����, ������������ ������ ����� ������ � .dbt-�����
                                    // ��� 10 ��������, ���� ���� ���� �����
#if DEBUG
                                    throw new NotImplementedException();
#else
                            break;
#endif
                                }
                        }
                    }
                    recReader.Close();
                    i++;
                    return result;
                }
                else
                {
                    recReader.Close();
                    i++;
                }
            }
        }

        // ��������� ������ �� ������� ����
        private string decodeString(byte[] b)
        {
            string s = encoding.GetString(b);
            if (s.IndexOf('\0') >= 0)
                s = s.Substring(0, s.IndexOf('\0'));
            s = s.Trim();
            return s;
        }
    }

    /// <summary>
    /// ���������
    /// </summary>
    internal class DbfHeader
    {
        private DbfHeaderStruct header;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="lang">���������</param>
        public DbfHeader(byte lang)
        {
            header.version = 0x03;  // 0x03 FoxBASE+/dBASE III PLUS, no memo;  0x83 FoxBASE+/dBASE III PLUS, with memo
            header.updateYear = 0x6E;
            header.updateMonth = 0x1;
            header.updateDay = 0x1;
            header.language = lang; //0x26 - 866, 0x57 - 1251, 0x00 - ignore
        }

        /// <summary>
        /// ���������
        /// </summary>
        public DbfHeaderStruct Header
        {
            get { return header; }
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        public int RecordsCount
        {
            get { return header.numRecords; }
            set { header.numRecords = value; }
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        public Int16 HeaderSize
        {
            get { return header.headerLen; }
            set { header.headerLen = value; }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        public Int16 RecordSize
        {
            get { return header.recordLen; }
            set { header.recordLen = value; }
        }

        /// <summary>
        /// Memo ����
        /// </summary>
        public bool HasMemo
        {
            get { return header.version == 0x83; }
            set
            {
                header.version = (byte)(value ? 0x83 : 0x03);
                header.flags = (byte)(value ? 0x02 : 0x00);
            }
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    public abstract class DbfColumn
    {
        private DbfFieldDescriptor desc;
        private string name;
        private bool nullable;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="type">���</param>
        /// <param name="nullable">����������� ��������� null</param>
        /// <param name="size">������</param>
        /// <param name="count">���-�� ������ ����� �������</param>
        public DbfColumn(string name, char type, bool nullable, byte size, byte count)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Empty dbf column name");
            if (name.Length > 10)
                throw new ArgumentException("dbf column name more than 10 symbols: " + name);
            desc = new DbfFieldDescriptor();
            this.name = name;
            desc.fieldType = type;
            desc.fieldLen = size;
            desc.count = count;
            this.nullable = nullable;
        }

        /// <summary>
        /// FieldDescriptor
        /// </summary>
        internal DbfFieldDescriptor FieldDescriptor
        {
            get { return desc; }
        }

        /// <summary>
        /// ����������� ��������� ������� ��������
        /// </summary>
        internal bool Nullable
        {
            get { return nullable; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        /// <param name="encoder">����������</param>
        public void SetEncodedName(Encoder encoder)
        {
            char[] charArray = Name.ToCharArray();
            desc.fieldName = new byte[11];
            encoder.GetBytes(charArray, 0, charArray.Length, desc.fieldName, 0, true);
        }

        /// <summary>
        /// ������� ������� � ����� ������ �������� ������������ dbf
        /// </summary>
        /// <param name="o">������</param>
        /// <param name="encoder">���������� �������� Unicode � �����</param>
        /// <returns>������ ������</returns>
        internal abstract byte[] ObjectToBytes(object o, Encoder encoder);
    }

    /// <summary>
    /// ����� (������������� ������� �����)
    /// int, short, long
    /// uint, ushort, ulong
    /// float, double
    /// byte, sbyte
    /// decimal
    /// </summary>
    public class DbfColumnNumber : DbfColumn
    {
        // ������ ������
        private byte[] empty;
        private int sz;
        private int cnt;
        private string formater;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="size">������������ ���������� ��������</param>
        /// <param name="count">����� ������ ����� �������</param>
        public DbfColumnNumber(string name, int size, int count)
            : this(name, size, count, true)
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="size">������������ ���������� ��������</param>
        /// <param name="count">����� ������ ����� �������</param>
        /// <param name="nullable">����������� ��������� ������� ��������</param>
        public DbfColumnNumber(string name, int size, int count, bool nullable)
            : base(name, 'N', nullable, (byte)size, (byte)count)
        {
            if (size > 20 || size < 1)
                throw new ArgumentException("Dbf ������� �����. ���������� �������� ������ ���� � ��������� [1..20]");
            if (size >= 3)
            {
                if (count > size - 2 || count < 0)
                    throw new ArgumentException("Dbf ������� �����. ����� ������ ����� ������� ������ ���� � ��������� [0..size-2]");
            }
            else
            {
                // size = 1 or size = 2
                if (count != 0)
                    throw new ArgumentException("Dbf ������� �����. ��� ���������� �������� 1 ��� 2, ����� ������ ����� ������� ������ ���� ����� 0");
            }
            sz = size;
            cnt = count;
            empty = new byte[size];
            for (int i = 0; i < size; i++)
                empty[i] = 0x20;
            formater = "{0," + size + ":F" + count + "}";
        }

        internal override byte[] ObjectToBytes(object o, Encoder encoder)
        {
            byte[] res = new byte[empty.Length];
            Array.Copy(empty, res, empty.Length);
            if (o == null)
            {
                if (!Nullable)
                    throw new Exception("Null value");
                return res;
            }
            string str = string.Empty;
            // ������������� �� ������
            if (o is SByte || o is Int16 || o is Int32 || o is Int64)
            {
                Int64 n = Convert.ToInt64(o);
                str = string.Format(formater, n);
            }
            // ������������� �����������
            else if (o is Byte || o is UInt16 || o is UInt32 || o is UInt64)
            {
                UInt64 n = Convert.ToUInt64(o);
                str = string.Format(formater, n);
            }
            // � ��������� ������
            else if (o is Single || o is Double)
            {
                Double n = Convert.ToDouble(o);
                str = string.Format(formater, n);
            }
            // decimal
            else if (o is Decimal)
            {
                Decimal n = Convert.ToDecimal(o);
                str = string.Format(formater, n);
            }
            else
                throw new Exception("Wrong type: number expected");
            str = str.Replace(",", ".");
            char[] charArray = str.ToCharArray();
            if (charArray.Length > sz)
                throw new Exception("Number too long. " + res.Length + " symbols expexted, " + charArray.Length + " symbols received: " + str);
            encoder.GetBytes(charArray, 0, Math.Min(charArray.Length, res.Length), res, 0, true);
            return res;
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    public class DbfColumnString : DbfColumn
    {
        // ������ ������
        private byte[] empty;
        // �������� ������� ������
        private bool truncate;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="size">������������ ���������� ��������</param>
        public DbfColumnString(string name, int size)
            : this(name, size, false, true)
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="size">������������ ���������� ��������</param>
        /// <param name="truncate">�������� ������� ������� ������</param>
        /// <param name="nullable">����������� ��������� ������� ��������</param>
        public DbfColumnString(string name, int size, bool truncate, bool nullable)
            : base(name, 'C', nullable, (byte)size, 0)
        {
            if (size > 254 || size < 1)
                throw new ArgumentException("Wrong size. Size must be in (1..254)");
            empty = new byte[size];
            for (int i = 0; i < size; i++)
                empty[i] = 0x20;
            this.truncate = truncate;
        }

        internal override byte[] ObjectToBytes(object o, Encoder encoder)
        {
            byte[] res = new byte[empty.Length];
            Array.Copy(empty, res, empty.Length);
            if (o == null)
            {
                if (!Nullable)
                    throw new Exception("Null value");
                return res;
            }
            if (!(o is string))
                throw new Exception("Wrong type: string expected");
            string str = (string)o;
            char[] charArray = str.ToCharArray();
            if (!truncate && charArray.Length > res.Length)
                throw new Exception("String too long. " + res.Length + " symbols expexted, " + charArray.Length + " symbols received: " + str);
            encoder.GetBytes(charArray, 0, Math.Min(charArray.Length, res.Length), res, 0, true);
            return res;
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public class DbfColumnDate : DbfColumn
    {
        private static byte[] empty = new byte[8] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        public DbfColumnDate(string name)
            : this(name, true)
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="nullable">����������� ��������� ������� ��������</param>
        public DbfColumnDate(string name, bool nullable)
            : base(name, 'D', nullable, 8, 0)
        {
        }

        internal override byte[] ObjectToBytes(object o, Encoder encoder)
        {
            if (o == null)
            {
                if (!Nullable)
                    throw new Exception("Null value");
                return empty;
            }
            if (!(o is DateTime))
                throw new Exception("Wrong type: date expected");
            DateTime dt = (DateTime)o;
            // YYYYMMDD
            byte[] res = new byte[8];
            string str = dt.ToString("yyyyMMdd");
            encoder.GetBytes(str.ToCharArray(), 0, 8, res, 0, true);
            return res;
        }
    }

    /// <summary>
    /// Bool
    /// </summary>
    public class DbfColumnBoolean : DbfColumn
    {
        private static readonly char[] Null = new char[1] { ' ' };
        private static readonly char[] Y = new char[1] { 'Y' };
        private static readonly char[] N = new char[1] { 'N' };

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        public DbfColumnBoolean(string name)
            : this(name, true)
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="nullable">����������� ��������� ������� ��������</param>
        public DbfColumnBoolean(string name, bool nullable)
            : base(name, 'L', nullable, 1, 0)
        {
        }

        internal override byte[] ObjectToBytes(object o, Encoder encoder)
        {
            byte[] res = new byte[1];
            if (o == null)
            {
                if (!Nullable)
                    throw new Exception("Null value");
                encoder.GetBytes(Null, 0, 1, res, 0, true);
                return res;
            }
            if (!(o is bool))
                throw new Exception("Wrong type: bool expected");
            bool b = (bool)o;
            encoder.GetBytes(b ? Y : N, 0, 1, res, 0, true);
            return res;
        }
    }

    /// <summary>
    /// Memo
    /// </summary>
    public class DbfColumnMemo : DbfColumn
    {
        // ������ ������
        private byte[] empty;
        private string formater;
        private const int size = 10;

        // �����
        private Stream stream;

        /// <summary>
        /// �������
        /// </summary>
        internal static int count = 0;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        public DbfColumnMemo(string name)
            : this(name, true)
        {
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="nullable">����������� ��������� ������� ��������</param>
        public DbfColumnMemo(string name, bool nullable)
            : base(name, 'M', nullable, size, 0)
        {
            empty = new byte[size];
            for (int i = 0; i < size; i++)
                empty[i] = 0x20;
            formater = "{0," + size + ":F" + 0 + "}";
        }

        internal override byte[] ObjectToBytes(object o, Encoder encoder)
        {
            byte[] res = new byte[empty.Length];
            Array.Copy(empty, res, empty.Length);
            if (o == null)
            {
                if (!Nullable)
                    throw new Exception("Null value");
                return res;
            }

            string text = (string)o;

            // 10 digits (bytes) representing a .DBT block number. The number is stored as a string, right justified and padded with blanks.
            long n = count;

            {
                // ������ � dbt
                List<string> ss = new List<string>();
                if (text.Length == 0)
                {
                    ss.Add(string.Empty);
                }
                else
                {
                    int k = text.Length / 512;
                    int r = text.Length % 512;
                    for (int i = 0; i < k; i++)
                    {
                        ss.Add(text.Substring(512 * i, 512));
                    }
                    if (r > 0)
                    {
                        ss.Add(text.Substring(512 * k, r));
                    }
                }
                for (int i = 0; i < ss.Count; i++)
                {
                    char[] chars = ss[i].ToCharArray();
                    byte[] res2 = new byte[512];
                    encoder.GetBytes(chars, 0, Math.Min(chars.Length, res2.Length), res2, 0, true);
                    if (i == ss.Count - 1 && chars.Length < 512)
                    {
                        if (chars.Length <= 510)
                        {
                            res2[chars.Length] = (byte)0x1A;
                            res2[chars.Length + 1] = (byte)0x1A;
                        }
                        else
                        {
                            res2[chars.Length] = (byte)0x1A;
                        }
                    }
                    stream.Write(res2, 0, 512);
                }
                count += ss.Count;
            }

            string str = string.Empty;
            str = string.Format(formater, n);
            str = str.Replace(",", ".");
            char[] charArray = str.ToCharArray();
            if (charArray.Length > size)
                throw new Exception("Memo address too long. " + res.Length + " symbols expexted, " + charArray.Length + " symbols received: " + str);
            encoder.GetBytes(charArray, 0, Math.Min(charArray.Length, res.Length), res, 0, true);
            return res;
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="fsDBT"></param>
        internal void SetStream(Stream fsDBT)
        {
            stream = fsDBT;
        }
    }

    // ���������
    // ����������� layout ��� ������� ������ �/�� ���������/�
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct DbfHeaderStruct
    {
        public byte version;        // ������
        public byte updateYear;     // ���, �����, ���� ���������� ���������� �������
        public byte updateMonth;
        public byte updateDay;
        public Int32 numRecords;    // ���-�� �������
        public Int16 headerLen;     // ������ ��������� � ������
        public Int16 recordLen;     // ������ ������ � ������
        public Int16 reserved1;     // ���������������
        public byte incompleteTrans;    // ����������
        public byte encryptionFlag; // ����������
        public Int32 reserved2;     // ��������������������� ���������
        public Int64 reserved3;
        
        //Table flags:
        //0x01   file has a structural .cdx
        //0x02   file has a Memo field
        //0x04   file is a database (.dbc)
        //This byte can contain the sum of any of the above values. For example, the value 0x03 indicates the table has a structural .cdx and a Memo field.
        public byte flags;            // ����� �������        
        
        public byte language;       // ���������
        public Int16 reserved4;     // ���������������
    }

    // �������� ����
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DbfFieldDescriptor
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public byte[] fieldName;    // ��� ����
        public char fieldType;      // ��� ����
        public Int32 address;       // ����� ���� � ������
        public byte fieldLen;       // ������ ����
        public byte count;          // ���-�� ������ ����� �������
        public Int16 reserved1;     // ���������������
        public byte workArea;       // ������������� ������� �������
        public Int16 reserved2;     // ��������������������� dBase
        public byte flag;           // ������������� ����
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] reserved3;    // ���������������
        public byte indexFlag;      // ���� �������� � .mdx ������
    }
}
