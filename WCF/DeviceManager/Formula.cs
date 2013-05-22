using System;
using System.Xml;
using System.Threading;

namespace DeviceManager
{
    /// <summary>
    /// перечисляет типы формул
    /// </summary>
    [Serializable]
    public enum FormulaType
    {
        /// <summary>
        /// Константа
        /// </summary>
        Constant,

        /// <summary>
        /// Значение не определено
        /// </summary>
        Undefined,

        /// <summary>
        /// Присваивание
        /// </summary>
        Assignment,

        /// <summary>
        /// Сумма двух чисел
        /// </summary>
        Summa,

        /// <summary>
        /// Разность
        /// </summary>
        Difference,

        /// <summary>
        /// Умножение
        /// </summary>
        Multiplication,

        /// <summary>
        /// Деление
        /// </summary>
        Division,

        /// <summary>
        /// Степень 10
        /// </summary>
        PowerOf10,

        /// <summary>
        /// Суммирование (сумма всех значений параметра)
        /// </summary>
        Accumulation,

        /// <summary>
        /// Приращение параметра
        /// </summary>
        Increment,

        /// <summary>
        /// Минимальное значение параметра
        /// </summary>
        Minimum,

        /// <summary>
        /// Максимальное значение параметра
        /// </summary>
        Maximum,

        /// <summary>
        /// Скользящее среднее параметра (фильтр первого порядка)
        /// </summary>
        Media,

        /// <summary>
        /// Кусочно-линейное преобразование
        /// </summary>
        Tranformation,

        /// <summary>
        /// Захват канала
        /// </summary>
        Capture,

        /// <summary>
        /// Газы (добавлена для СГТ)
        /// </summary>
        Gases,

        /// <summary>
        /// Реализует сценарий
        /// </summary>
        Script,

        /// <summary>
        /// Формула не определена
        /// </summary>
        Default
    }

    /// <summary>
    /// Перечисляет источник данных для вычисления
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// Значение из списка сигнала
        /// </summary>
        Signals,

        /// <summary>
        /// Значение из результирующего списка
        /// </summary>
        Results,

        /// <summary>
        /// Источник не определен
        /// </summary>
        Default
    }

    /// <summary>
    /// Реализует аргумент формулы
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// имя узла в который сериализуется аргумент
        /// </summary>
        protected const string rootName = "argument";

        /// <summary>
        /// узел в который сохраняется индекс аргумента
        /// </summary>
        protected const string indexName = "index";

        /// <summary>
        /// узел в который сохраняется источник значения аргумента
        /// </summary>
        protected const string sourceName = "source";

        protected long index = -1;                                  // расположение в списке значения
        protected DataSource source = DataSource.Default;           // источник данных

        protected ReaderWriterLock locker = null;                   // синхронизирует доступ к классу
        protected long actual = 1;                                  // актуален параметр или нет

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public Argument()
        {
            locker = new ReaderWriterLock();
        }

        /// <summary>
        /// Определяет расположение в списке значения
        /// </summary>
        public int Index
        {
            get { return (int)Interlocked.Read(ref index); }
            set { Interlocked.Exchange(ref index, (long)value); }
        }

        /// <summary>
        /// Определяет источник данных
        /// </summary>
        public DataSource Source
        {
            get
            {
                try
                {
                    locker.AcquireReaderLock(100);
                    try
                    {
                        return source;
                    }
                    finally
                    {
                        locker.ReleaseReaderLock();
                    }
                }
                catch { }
                return DataSource.Default;
            }

            set
            {
                try
                {
                    locker.AcquireWriterLock(100);
                    try
                    {
                        source = value;
                    }
                    finally
                    {
                        locker.ReleaseWriterLock();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Определяет агрумент актуален или нет
        /// </summary>
        public bool IsActual
        {
            get { return (Interlocked.Read(ref actual) == 1); }
            set
            {
                if (value)
                {
                    Interlocked.Exchange(ref actual, 1);
                }
                else
                    Interlocked.Exchange(ref actual, 0);
            }
        }

        /// <summary>
        /// Создать узел описывающий аргумент
        /// </summary>
        /// <param name="document">Документ с помощью которого создается узел</param>
        /// <returns>Созданный узел, в противном случае null</returns>
        public XmlNode CreateNode(XmlDocument document)
        {
            try
            {
                XmlNode root = document.CreateElement(rootName);

                XmlNode indexNode = document.CreateElement(indexName);
                XmlNode sourceNode = document.CreateElement(sourceName);

                indexNode.InnerText = Index.ToString();
                sourceNode.InnerText = Source.ToString();

                root.AppendChild(indexNode);
                root.AppendChild(sourceNode);

                return root;

            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Инициализировать аргумен из узла
        /// </summary>
        /// <param name="node">Узел в котором находятся данные для инициализации</param>
        public void InstanceFromXml(XmlNode node)
        {
            try
            {
                if (node.Name == rootName)
                {
                    XmlNodeList childs = node.ChildNodes;
                    if (childs != null)
                    {
                        foreach (XmlNode child in childs)
                        {
                            switch (child.Name)
                            {
                                case indexName:

                                    try
                                    {
                                        index = long.Parse(child.InnerText);
                                    }
                                    catch
                                    {
                                    }
                                    break;

                                case sourceName:

                                    try
                                    {
                                        source = (DataSource)Enum.Parse(typeof(DataSource), child.InnerText);
                                    }
                                    catch
                                    {
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}