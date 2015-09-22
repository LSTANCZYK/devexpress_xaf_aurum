namespace Aurum.Exchange
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Объект представления пути к сущности
    /// </summary>
    public abstract class EntityPath
    {
        /// <summary>
        /// Путь
        /// </summary>
        public String Path { get; private set; }

        /// <summary>
        /// Существует ли указанный путь
        /// </summary>
        /// <returns></returns>
        public abstract bool Exists();

        /// <summary>
        /// Строковое представление пути
        /// </summary>
        public override string ToString()
        {
            return Path;
        }

        /// <summary>
        /// Оператор неявного преобразования к строке
        /// </summary>
        /// <param name="entityPath">Наследованный от EntityPath объект</param>
        /// <returns>Строковый путь объекта, либо пустая строка (String.Empty), если объект пустой</returns>
        public static implicit operator String(EntityPath entityPath)
        {
            if (entityPath == null)
            {
                return String.Empty;
            }
            return entityPath.Path;
        }

        /// <summary>
        /// Создать новый объект представления пути к сущности
        /// </summary>
        /// <param name="path"></param>
        public EntityPath(string path)
        {
            Path = path;
        }
    }

    /// <summary>
    /// Представление пути к директории
    /// </summary>
    public class DirectoryPath : EntityPath
    {
        /// <summary>
        /// Существует ли указанный путь к директории
        /// </summary>
        public override bool Exists()
        {
            return System.IO.Directory.Exists(Path);
        }

        /// <summary>
        /// Создать новый объект представления пути к директории
        /// </summary>
        /// <param name="path">Путь</param>
        public DirectoryPath(string path)
            : base(path)
        {
        }
    }

    /// <summary>
    /// Представление пути к файлу
    /// </summary>
    public class FilePath : EntityPath
    {
        /// <summary>
        /// Существует ли указанный путь к файлу
        /// </summary>
        public override bool Exists()
        {
            return System.IO.File.Exists(Path);
        }

        /// <summary>
        /// Создать новый объект представления пути к файлу
        /// </summary>
        /// <param name="path"></param>
        public FilePath(string path)
            : base(path)
        {
        }
    }

    /// <summary>
    /// Представление нескольких путей к сущностям типа T
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public abstract class MultipleEntityPath<T>
        where T : EntityPath
    {
        /// <summary>
        /// Неизменяемая коллекция путей к сущностям типа T
        /// </summary>
        public IEnumerable<T> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// Существуют ли пути всех сущностей в коллекции
        /// </summary>
        public bool AllExist()
        {
            return Items.All(i => i.Exists());
        }

        /// <summary>
        /// Оператор неявного преобразования к строке. Каждый элемент отделен запятой
        /// </summary>
        /// <param name="mEntityPath">Объект</param>
        public static implicit operator String(MultipleEntityPath<T> mEntityPath)
        {
            return String.Join(",", mEntityPath.Items.Select(i => i.Path));
        }

        /// <summary>
        /// Создать новый обхект представления нескольких путей к сущностям
        /// </summary>
        /// <param name="collection"></param>
        public MultipleEntityPath(IEnumerable<T> collection)
        {
            Items = collection;
        }
    }

    /// <summary>
    /// Представление нескольких путей к файлам
    /// </summary>
    public class MultipleFilePath : MultipleEntityPath<FilePath>
    {
        /// <summary>
        /// Создать объект представления нескольких путей к файлам
        /// </summary>
        /// <param name="collection">Коллекция объектов путей</param>
        public MultipleFilePath(IEnumerable<FilePath> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Создать объект представления нескольких путей к файлам из строки, в который элементы разделены запятыми
        /// </summary>
        /// <param name="filesPath">Строка. Каждый элемент отделяется запятой</param>
        public MultipleFilePath(string filesPath)
            : base(filesPath.Split(',').Select(t => new FilePath(t)))
        {
        }
    }

    /// <summary>
    /// Атрибут установки режима выбора пути
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FilePathModeAttribute : Attribute
    {
        /// <summary>
        /// Режим
        /// </summary>
        public FilePathMode Mode
        {
            get;
            private set;
        }

        /// <summary>
        /// Установить режим открытия или сохранения файла (файлов)
        /// </summary>
        public FilePathModeAttribute(FilePathMode mode)
            : base()
        {
            Mode = mode;
        }
    }

    /// <summary>
    /// Атрибут фильтрации файлов
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FilePathFilterAttribute : Attribute
    {
        /// <summary>
        /// Маска
        /// </summary>
        public string Filter
        {
            get;
            private set;
        }

        /// <summary>
        /// Установить строку фильтра для указанного параметра пути файла.
        /// Пример: "Text files|*.txt;*.text|All files|*.*".
        /// </summary>
        /// <param name="filter">Строка фильтра</param>
        public FilePathFilterAttribute(string filter)
            : base()
        {
            Filter = filter;
        }
    }

    /// <summary>
    /// Режим выбора пути к файлу или файлам
    /// </summary>
    public enum FilePathMode
    {
        /// <summary>
        /// Открытие
        /// </summary>
        Open,

        /// <summary>
        /// Сохранение
        /// </summary>
        Save
    }
}
