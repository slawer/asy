using System;
using System.Xml;
using System.Threading;

using WCF;

namespace ASY
{
    /// <summary>
    /// Реализует базовый параметр, которым оперирует приложение
    /// </summary>
    public partial class Parameter
    {
        protected ReaderWriterLockSlim slim;            // синхронизатор

        protected string name;                          // имя параметра
        protected string description;                   // описание параметра (краткое имя параметра)

        protected string units;                         // единицы измерения параметра

        protected float alarmValue;                     // аварийное значение
        protected float blockingValue;                  // блокировочное значение

        protected bool controlAlarm;                    // контролировать или нет аварийное значение параметра
        protected bool controlBlocking;                 // контролировать или нет блокировочное значение параметра

        protected ParameterRange range;                 // интервал значений параметра

        protected bool saveToDB;                        // сохранять параметр в БД или нет
        protected int intervalToSave;                   // интервал записи параметра в БД

        protected DateTime db_time;                     // время последнего сохранения параметра в БД
        protected int thresholdToBD;                    // пороговое значение

        // ------------------------------------

        protected float currentValue;                   // текущее значение параметра полученное от devMan
        protected float calculatedValue;                // вычисленное значение(откалиброванное ...)

        protected int decimalPoints = 2;                // количество точек после запятой
        protected string format = "{0:F2}";             // строка определяющая формат параметра

        protected Transformation transformation;        // осуществляет калибровку параметра
        protected ReaderWriterLockSlim p_slim;          // синхронизатор значения параметра

        // ------------------------------------

        protected PDescription channel;                 // канал от devMan

        private int devManindex = -1;                   // номер параметра в списке от devMan
        private string devManDescription;               // текстовое описание параметра от devMan        

        protected ReaderWriterLockSlim c_slim;          // синхронизатор для номера канала

        protected Guid identifier;                      // идентификатор параметра

        // ------------------------------------

        /// <summary>
        /// инициализирует новый экземпляр класса
        /// </summary>
        public Parameter()
        {
            transformation = new Transformation();
            slim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            p_slim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            c_slim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            range = new ParameterRange();

            Transformation.TCondition t1 = new Transformation.TCondition();
            Transformation.TCondition t2 = new Transformation.TCondition();

            t1.Result = 0;
            t1.Signal = 0;

            t2.Result = 65535;
            t2.Signal = 65535;

            transformation.Insert(t1);
            transformation.Insert(t2);

            name = string.Empty;
            description = string.Empty;

            identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Определяет канал от devMan
        /// </summary>
        public PDescription Channel
        {
            get
            {
                if (c_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        if (channel == null)
                        {
                            if (devManindex > -1)
                            {
                                channel = new PDescription(devManindex, devManDescription, DeviceManager.FormulaType.Default);
                            }
                        }

                        return channel;
                    }
                    finally
                    {
                        c_slim.ExitReadLock();
                    }
                }

                return null;
            }

            set
            {
                if (c_slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        channel = value;
                        if (channel != null)
                        {
                            devManindex = channel.Number;
                            devManDescription = channel.Description;
                        }
                    }
                    finally
                    {
                        c_slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет идентификатор параметра
        /// </summary>
        public Guid Identifier
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return identifier;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return Guid.Empty;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        identifier = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        // ------------- работа с значением параметра ------------------

        /// <summary>
        /// Возвращяет текущее значение параметра такое какое пришло от devMan.
        /// Если не удалось получить текущее значение параметра, возвращается float.PositiveInfinity
        /// </summary>
        public float CurrentValue
        {
            get
            {
                if (p_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return currentValue;
                    }
                    finally
                    {
                        p_slim.ExitReadLock();
                    }
                }

                return float.PositiveInfinity;
            }
        }

        /// <summary>
        /// Возвращяет вычисленное значение параметра.
        /// Если не удалось получить текущее значение параметра, возвращается float.PositiveInfinity
        /// </summary>
        public float CalculatedValue
        {
            get
            {
                if (p_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return calculatedValue;
                    }
                    finally
                    {
                        p_slim.ExitReadLock();
                    }
                }

                return float.PositiveInfinity;
            }
        }

        /// <summary>
        /// Определяет количество точек после запятой
        /// </summary>
        public int NumberOfDecimalPoints
        {
            get
            {
                if (p_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return decimalPoints;
                    }
                    finally
                    {
                        p_slim.ExitReadLock();
                    }
                }

                return 2;
            }

            set
            {
                if (p_slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        if (value > -1)
                        {
                            decimalPoints = value;
                            format = "{0:F" + decimalPoints.ToString() + "}";
                        }
                    }
                    catch { }
                    finally
                    {
                        p_slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Возвращяет отформатированное представление текущего значения параметра.
        /// </summary>
        public string FormattedCurrentValue
        {
            get
            {
                if (p_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return string.Format(format, currentValue);
                    }
                    finally
                    {
                        p_slim.ExitReadLock();
                    }
                }

                return string.Format("{0:F2}", currentValue);
            }
        }

        /// <summary>
        /// Возвращяет отформатированное представление вычисленного значения параметра.
        /// </summary>
        public string FormattedCaclulatedValue
        {
            get
            {
                if (p_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return string.Format(format, calculatedValue);
                    }
                    finally
                    {
                        p_slim.ExitReadLock();
                    }
                }

                return string.Format("{0:F2}", calculatedValue);
            }
        }

        /// <summary>
        /// Возвращяет формулу калибровки параметра
        /// </summary>
        public Transformation Transformation
        {
            get { return transformation; }
        }

        /// <summary>
        /// Присвоить значение параметру
        /// </summary>
        /// <param name="parameter">Параметр, которому присвоить значение</param>
        /// <param name="value">Присваиваемое значение параметру</param>
        public static void SetCurrentValue(Parameter parameter, float value)
        {
            if (parameter != null)
            {
                if (parameter.p_slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        parameter.currentValue = value;
                        if (parameter.transformation != null)
                        {
                            parameter.transformation.Arg = value;
                            parameter.calculatedValue = parameter.transformation.Calculate();
                        }
                        else
                            parameter.calculatedValue = value;
                    }
                    finally
                    {
                        parameter.p_slim.ExitWriteLock();
                    }
                }
            }
        }

        // -------------------------------------------------------------

        /// <summary>
        /// Определяет текстовое имя параметра
        /// </summary>
        public string Name
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        if (name == string.Empty)
                        {
                            if (channel == null)
                            {
                                return "Параметр не определен";
                            }

                            else
                                return channel.Description;
                        }

                        return name;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return name;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        name = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет текстовое описание параметра(краткое).
        /// </summary>
        public string Description
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return description;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return description;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        description = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет текстовое описание единиц в которых измеряется параметр
        /// </summary>
        public string Units
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return units;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return units;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        units = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет аварийное значение параметра
        /// </summary>
        public float Alarm
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return alarmValue;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return alarmValue;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        alarmValue = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет блокировочное значение параметра
        /// </summary>
        public float Blocking
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return blockingValue;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return blockingValue;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        blockingValue = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет контролировать аварийное значение или нет
        /// </summary>
        public bool IsControlAlarm
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return controlAlarm;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return controlAlarm;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        controlAlarm = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет контролировать блокировочное значение или нет
        /// </summary>
        public bool IsControlBlocking
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return controlBlocking;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return controlBlocking;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        controlBlocking = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет диапазон возможных значений параметра
        /// </summary>
        public ParameterRange Range
        {
            get { return range; }
        }

        /// <summary>
        /// Определяет сохранять параметр в Бд или нет
        /// </summary>
        public bool SaveToDB
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return saveToDB;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return saveToDB;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        saveToDB = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет интервал сохранения параметра в БД
        /// </summary>
        public int IntervalToSaveToDB
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return intervalToSave;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return intervalToSave;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        intervalToSave = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Время последней записи параметра в БД
        /// </summary>
        public DateTime DB_Time
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return db_time;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return db_time;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        db_time = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет пороговое значение
        /// </summary>
        public int ThresholdToBD
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return thresholdToBD;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                return thresholdToBD;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        thresholdToBD = value;
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Реализует хранение диапазона допустимых значений для параметра
    /// </summary>
    public class ParameterRange
    {
        protected ReaderWriterLockSlim slim;     // синхронизатор

        protected float _min;                   // минимальное значение диапазона
        protected float _max;                   // максимальное значение диапазона

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public ParameterRange()
        {
            slim = new ReaderWriterLockSlim();

            _min = 0;
            _max = 65535;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="MinValue">Минимальное значение диапазона</param>
        /// <param name="MaxValue">Максимальное значение диапазона</param>
        public ParameterRange(float MinValue, float MaxValue)
        {
            slim = new ReaderWriterLockSlim();

            if (MinValue <= MaxValue)
            {
                _min = MinValue;
                _max = MaxValue;
            }
            else
            {
                _min = MaxValue;
                _max = MinValue;
            }
        }

        /// <summary>
        /// Определяет минимальное значение диапазона
        /// </summary>
        public float Min
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return _min;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }
                return float.NaN;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        if (value <= _max)
                        {
                            _min = value;
                        }
                        else
                        {
                            _min = _max;
                            _max = value;
                        }
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Определяет максимальное значение диапазона
        /// </summary>
        public float Max
        {
            get
            {
                if (slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return _max;
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }
                return float.NaN;
            }

            set
            {
                if (slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        if (value >= _min)
                        {
                            _max = value;
                        }
                        else
                        {
                            _max = _min;
                            _min = value;
                        }
                    }
                    finally
                    {
                        slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Сохранить текущее состояние объекта в XML узел
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public XmlNode Save(XmlDocument doc)
        {
            try
            {
                if (doc != null)
                {
                    XmlNode root = doc.CreateElement("ParameterRange");

                    XmlNode minNode = doc.CreateElement("min");
                    XmlNode maxNode = doc.CreateElement("max");

                    minNode.InnerText = Min.ToString();
                    maxNode.InnerText = Max.ToString();

                    root.AppendChild(minNode);
                    root.AppendChild(maxNode);

                    return root;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Десериализовать параметр
        /// </summary>
        /// <param name="node">Узел в котором сохранен параметр</param>
        public void Load(XmlNode node)
        {
            try
            {
                if (node != null)
                {
                    if (node.Name == "ParameterRange")
                    {
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                switch (child.Name)
                                {
                                    case "min":

                                        try
                                        {
                                            Min = float.Parse(child.InnerText);
                                        }
                                        catch { }
                                        break;

                                    case "max":

                                        try
                                        {
                                            Max = float.Parse(child.InnerText);
                                        }
                                        catch { }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}