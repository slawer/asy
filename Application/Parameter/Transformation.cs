using System;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using DeviceManager;

namespace ASY
{
    /// <summary>
    /// Реализует кусочно-линейное преобразование
    /// </summary>
    public class Transformation
    {
        /// <summary>
        /// имя корневого узла настроек макроса
        /// </summary>
        protected const string rootName = "macros";

        /// <summary>
        /// имя узла в котором хранится позиция канала из которого брать значение
        /// </summary>
        protected const string indexName = "index";

        /// <summary>
        /// имя узла в котором сохраняется описание макроса
        /// </summary>
        protected const string descriptionName = "desc";

        /// <summary>
        /// имя узла в который сериализуется аргумент
        /// </summary>
        protected const string argumentName = "argument";

        /// <summary>
        /// имя узла в который сериализуется строка калибровочная таблицы
        /// </summary>
        protected const string tableRecordName = "tableRecord";

        /// <summary>
        /// Значение сигнала в строке калибровочной таблицы
        /// </summary>
        protected const string tableSignalName = "signal";

        /// <summary>
        /// Значение, получаемое из сигнала в строке калибровочной таблицы после применения КЛП
        /// </summary>
        protected const string tableValueName = "value";

        /// <summary>
        /// Параметр линейного афинного преобразования: сдвиг
        /// </summary>
        protected const string tableShiftName = "shift";

        /// <summary>
        /// Параметр линейного афинного преобразования: множитель
        /// </summary>
        protected const string tableMultyName = "multy";

        protected Mutex mutex = null;                   // синхронизатор

        protected TCondition template;                  // для поска калибровочной точки
        protected List<TCondition> table;               // калибровочная таблица

        protected float arg;                            // аргумент функции калибровки

        /// <summary>
        /// Возникает когда добавленна калибровочная точка
        /// </summary>
        public event TConditionEventHandle OnInsert;

        /// <summary>
        /// Возникает когда удалена калибровочная точка
        /// </summary>
        public event TConditionEventHandle OnRemove;

        /// <summary>
        /// Возникает когда очищена таблица калибровки
        /// </summary>
        public event EventHandler OnClear;

        /// <summary>
        /// Возникает, когда метод Insert вставляет точку, уже имеющуюся в таблице калибровки
        /// </summary>
        public event TConditionEventHandle OnExist;

        /// <summary>
        /// Возникает в случае возникновения ошибки
        /// </summary>
        public event ErrorEventHandle OnError;

        /// <summary>
        /// Возникает когда изменена калибровочная точка
        /// </summary>
        public event TConditionEventHandle OnEdit;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public Transformation()
        {
            mutex = new Mutex();
            table = new List<TCondition>();
        }

        /// <summary>
        /// Возвращяет тип формулы
        /// </summary>
        public FormulaType Type { get { return FormulaType.Tranformation; } }

        /// <summary>
        /// Определяет калибруемое значение
        /// </summary>
        public float Arg 
        { 
            get { return arg; }
            set { arg = value; }
        }
        /// <summary>
        /// Определяет значение макроса
        /// </summary>
        public float Value 
        {
            get { return float.NaN; }
            set { } 
        }

        /// <summary>
        /// Возвращает таблицу калибровки
        /// </summary>
        public List<TCondition> Table
        {
            get { return this.table; }
        }

        /// <summary>
        /// Вычислить параметр 
        /// </summary>
        /// <param name="_val">Калибруемое значение</param>
        /// <returns>Вычисленное значение</returns>
        public float Calculate()
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    if (this.table.Count < 2) // таблица калибровки пуста
                    {
                        return float.NaN;
                    }

                    float x = arg;
                    if (float.IsNaN(x))
                    {
                        return float.NaN;
                    }

                    // Поиск интервала
                    double s = x;
                    int ind=0, LastIndex = this.table.Count - 1;
                    if (this.table[0].Signal > s)
                    {
                        ind = 1;
                    }
                    else
                        if (this.table[LastIndex].Signal < s)
                        {
                            ind = LastIndex;
                        }
                        else
                        {
                            for (int j = 1; j <= LastIndex; j++)
                            {
                                if ((this.table[j-1].Signal <= s) && (s <= this.table[j].Signal))
                                {
                                    ind = j;
                                    break;
                                }
                            }
                        }

                    // афинное преобразование сигнала в значение
                    double val = s * this.table[ind].Multy + this.table[ind].Shift;
                    return (float)val;
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }

            return float.NaN;
        }

        /// <summary>
        /// Сбросить состояние макроcа в начальное состояние (по умолчанию)
        /// </summary>
        public void Reset() {}

        /// <summary>
        /// Текстовое описание формулы (сложение, вычитание и т.п..)
        /// </summary>
        public string Name { get { return "Кусочно-линейное преобразование"; } }

        /// <summary>
        /// Получить XmlNode макроса для сохранения настроек конвертора
        /// </summary>
        /// <param name="document">Документ в который осуществляется сохранение настроек</param>
        /// <returns>XmlNode макроса</returns>
        public XmlNode CreateXmlNode(XmlDocument document)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(500, false))
                {
                    blocked = true;
                    XmlNode root = document.CreateElement(rootName);
                    //XmlNode arg_1 = arguments[0].CreateNode(document);

                    XmlNode descNode = document.CreateElement(descriptionName);
                    //descNode.InnerText = description;

                    //root.AppendChild(arg_1);

                    root.AppendChild(descNode);

                    // ---- сохранение таблицы калибровочных точек ----

                    int rcdCount = this.table.Count;
                    if (rcdCount > 0)
                    {
                        for (int j = 0; j < rcdCount; j++)
                        {
                            XmlNode tRcdName = document.CreateElement(tableRecordName);
                            tRcdName.InnerText = tableRecordName;

                            XmlNode signal = document.CreateElement(tableSignalName);
                            signal.InnerText = this.table[j].Signal.ToString();

                            XmlNode value = document.CreateElement(tableValueName);
                            value.InnerText = this.table[j].Result.ToString();

                            XmlNode shift = document.CreateElement(tableShiftName);
                            shift.InnerText = this.table[j].Shift.ToString();

                            XmlNode multy = document.CreateElement(tableMultyName);
                            multy.InnerText = this.table[j].Multy.ToString();

                            tRcdName.AppendChild(signal);
                            tRcdName.AppendChild(value);

                            tRcdName.AppendChild(shift);
                            tRcdName.AppendChild(multy);

                            root.AppendChild(tRcdName);
                        }
                    }

                    // ---------------------------------------------------------

                    return root;
                }
            }
            catch
            {
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
            return null; 
        }

        /// <summary>
        /// Инициализировать формулу из сохраненного раннее узла Xml
        /// </summary>
        /// <param name="node">Узел на основе которого выполнить инициализацию макроса</param>
        public void InstanceMacrosFromXmlNode(XmlNode node)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(500, false))
                {
                    // ---- загрузить таблицу калибровочных точек ----

                    blocked = true;

                    bool xmlError = false;
                    this.table.Clear();

                    XmlNodeList childs = node.ChildNodes;

                    if (childs != null)
                    {
                        foreach (XmlNode child in childs)
                        {
                            if (child.Name != tableRecordName) continue;

                            XmlNodeList parametrs = child.ChildNodes;
                            Boolean bFirst = true;

                            foreach (XmlNode childParametrs in parametrs)
                            {
                                int j;
                                switch (childParametrs.Name)
                                {
                                    case tableSignalName:

                                        try
                                        {
                                            if (bFirst)
                                            {
                                                TCondition newRecord = new TCondition();
                                                this.table.Add(newRecord);
                                                bFirst = false;
                                            }
                                            j = this.table.Count - 1;
                                            this.table[j].Signal = AsyApplication.ParseDouble(childParametrs.InnerText);
                                        }
                                        catch
                                        {
                                            xmlError = true;
                                        }
                                        break;

                                    case tableValueName:

                                        try
                                        {
                                            if (bFirst)
                                            {
                                                TCondition newRecord = new TCondition();
                                                this.table.Add(newRecord);
                                                bFirst = false;
                                            }
                                            j = this.table.Count - 1;
                                            this.table[j].Result = AsyApplication.ParseDouble(childParametrs.InnerText);
                                        }
                                        catch
                                        {
                                            xmlError = true;
                                        }
                                        break;

                                    case tableShiftName:

                                        try
                                        {
                                            if (bFirst)
                                            {
                                                TCondition newRecord = new TCondition();
                                                this.table.Add(newRecord);
                                                bFirst = false;
                                            }
                                            j = this.table.Count - 1;
                                            this.table[j].Shift = AsyApplication.ParseDouble(childParametrs.InnerText);
                                        }
                                        catch
                                        {
                                            xmlError = true;
                                        }
                                        break;

                                    case tableMultyName:

                                        try
                                        {
                                            if (bFirst)
                                            {
                                                TCondition newRecord = new TCondition();
                                                this.table.Add(newRecord);

                                                bFirst = false;
                                            }
                                            j = this.table.Count - 1;
                                            this.table[j].Multy = AsyApplication.ParseDouble(childParametrs.InnerText);
                                        }
                                        catch
                                        {
                                            xmlError = true;
                                        }
                                        break;
                                }
                            }
                        }
                        if (this.table.Count > 0) this.table.Sort(Comparison);
                    }

                    if (xmlError) this.table.Clear();

                    // -----------------------------------------------
                }
            }
            catch
            {
                //throw new Exception();
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Найти аргумент функции
        /// </summary>
        /// <param name="node">Узел в котором искать аргумент</param>
        /// <returns>Узел, содержащий настройки аргумента</returns>
        protected XmlNode[] FindArguments(XmlNode node)
        {
            try
            {
                if (node != null)
                {
                    if (node.HasChildNodes)
                    {
                        int index = 0;
                        XmlNode[] args = new XmlNode[2];

                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == argumentName)
                            {
                                args[index++] = child;
                                return args;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Редактировать поле Signal в строке таблицы
        /// </summary>
        /// <param name="SignalNew">Новое значение сигнала</param>
        /// <param name="selectedRow">Номер строки в таблице</param>
        public bool EditSignal(double SignalNew, int selectedRow)
        {
            if ((selectedRow < 0) || (selectedRow >= this.Table.Count)) return false;
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;

                    // проверяем дубликат сигнала
                    for (int j = 0; j < selectedRow; j++)
                    {
                        if(this.Table[j].Signal == SignalNew) return false;
                    }
                    for (int j = selectedRow + 1; j <this.Table.Count ; j++)
                    {
                        if (this.Table[j].Signal == SignalNew) return false;
                    }

                    this.Table[selectedRow].Signal = SignalNew;
                    TCondition Condition = this.Table[selectedRow];

                    // Вычисление парметров афинного преобразования
                    try
                    {
                        table.Sort(Comparison);
                        this.CalcTransformTable();
                    }
                    catch
                    {
                        if (OnError != null)
                        {
                            OnError(Condition, TypeError.TransformationError);
                        }
                        return false;
                    }

                    if (OnEdit != null)
                    {
                        OnEdit(Condition);
                    }
                    return true;
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
            return false;
        }

        /// <summary>
        /// Редактировать поле Result в строке таблицы
        /// </summary>
        /// <param name="ResultNew">Новое значение результата</param>
        /// <param name="selectedRow">Номер строки в таблице</param>
        public void EditResult(double ResultNew, int selectedRow)
        {
            if ((selectedRow < 0) || (selectedRow >= this.Table.Count)) return;
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;

                    this.Table[selectedRow].Result = ResultNew;

                    // Вычисление парметров афинного преобразования
                    try
                    {
                        this.CalcTransformTable();
                    }
                    catch
                    {
                        if (OnError != null)
                        {
                            OnError(Table[selectedRow], TypeError.TransformationError);
                        }
                        return;
                    }

                    if (OnEdit != null)
                    {
                        OnEdit(Table[selectedRow]);
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Добавить калибровочную точку
        /// </summary>
        /// <param name="condition">Калибровочная точка</param>
        public void Insert(TCondition condition)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;

                    template = condition;
                    if (table.Exists(Predicate))
                    { 
                        // уже существует - игнорировать

                        if (OnExist != null)
                        {
                            OnExist(condition);
                        }
                        return;
                    };


                    table.Add(condition);
                    table.Sort(Comparison);

                    // Вычисление парметров афинного преобразования
                    try
                    {
                        this.CalcTransformTable();
                    }
                    catch
                    {
                        if (OnError != null)
                        {
                            OnError(condition, TypeError.TransformationError);
                        }
                        return;
                    }

                    if (OnInsert != null)
                    {
                        OnInsert(condition);
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Расчитать значения коэффициентов афинного преобразования
        /// Коэффициенты интервала [j-1, j] хранятся в table[j]. ...
        /// </summary>
        private void CalcTransformTable()
        {
            if (table.Count >= 2)
            {
                int count = table.Count;
                for (int j = 1; j < count; j++)
                {
                    table[j].Multy = (table[j].Result - table[j - 1].Result) / (table[j].Signal - table[j - 1].Signal);
                    table[j].Shift = table[j].Result - table[j].Multy * table[j].Signal;
                }
            }
        }

        /// <summary>
        /// Удалить калибровочную точку
        /// </summary>
        /// <param name="condition">Калибровочная точка</param>
        public void Remove(TCondition condition)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    table.Remove(condition);

                    // Вычисление парметров афинного преобразования
                    
                    try
                    {
                        this.CalcTransformTable();
                    }
                    catch
                    {
                        if (OnError != null)
                        {
                            OnError(condition, TypeError.TransformationError);
                        }
                        return;
                    }

                    if (OnRemove != null)
                    {
                        OnRemove(condition);
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Очистить таблицу калибровки
        /// </summary>
        public void Clear()
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    table.Clear();

                    if (OnClear != null)
                    {
                        OnClear(this, EventArgs.Empty);
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Реализует сравнение калибровочных точек по исходному сигналу
        /// </summary>
        /// <param name="x">Первая точка</param>
        /// <param name="y">Вторая точка</param>
        /// <returns></returns>
        protected int Comparison(TCondition x, TCondition y)
        {
            double z = x.Signal - y.Signal;
            if (z == 0) 
                return 0;
            else 
            if (z < 0) 
                return -1;
            else 
                return 1;
        }

        /// <summary>
        /// Реализует поиск калибровочной точки по исходному сигналу
        /// </summary>
        /// <param name="obj">Калибровочная точка</param>
        /// <returns></returns>
        protected bool Predicate(TCondition obj)
        {
            if (template != null)
            {
                if (template.Signal == obj.Signal)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Интерфейс функции обрабатывающей события таблицы калибровки
        /// </summary>
        /// <param name="Condition">Объект, связанный с событием. В случае, если нет объекта, возвращается null</param>
        public delegate void TConditionEventHandle(TCondition Condition);

        /// <summary>
        /// Интерфейс функции обрабатывающей события "Возникновение ошибки номер №"
        /// </summary>
        /// <param name="Condition">Обьект, вызвавший ошибку. В случае, если нет объекта, возвращается null</param> 
        /// <param name="Error">Номер ошибка</param>
        public delegate void ErrorEventHandle(TCondition Condition, TypeError Error);

        /// <summary>
        /// Реализует строку калибровочной таблицы
        /// </summary>
        public class TCondition
        {
            protected double signal_value;            // значени сигнала
            protected double result_value;            // значение после обработки
            
            protected double afin_shift;              // сдвиг в афинном преобразовании
            protected double afin_multy;              // множитель в афинном преобразовании

            /// <summary>
            /// Инициализирует новый экземпляр класса
            /// </summary>
            public TCondition()
            {
                signal_value = double.NaN;
                result_value = double.NaN;
                
                afin_shift   = double.NaN;
                afin_multy   = double.NaN;
            }

            /// <summary>
            /// Значение исходного сигнала
            /// </summary>
            public double Signal
            {
                get { return signal_value; }
                set { signal_value = value; }
            }

            /// <summary>
            /// Значение после обработки
            /// </summary>
            public double Result
            {
                get { return result_value; }
                set { result_value = value; }
            }

            /// <summary>
            /// Значение сдвига (параметр афинного преобразования)
            /// </summary>
            public double Shift
            {
                get { return afin_shift; }
                set { afin_shift = value; }
            }

            /// <summary>
            /// Значение множителя (параметр афинного преобразования)
            /// </summary>
            public double Multy
            {
                get { return afin_multy; }
                set { afin_multy = value; }
            }
        }

        /// <summary>
        /// Типы ошибок
        /// </summary>
        public enum TypeError
        {
            /// <summary>
            /// В процессе вычисления параметров афинного преобразования возникла ошибка
            /// </summary>
            TransformationError,

            /// <summary>
            /// Тип ошибки неопределен
            /// </summary>
            Default
        }
    }
}