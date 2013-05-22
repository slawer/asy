using System;
using System.Threading;

namespace Buffering
{
    /// <summary>
    /// Реализует хранение среза данных
    /// </summary>
    [Serializable]
    public struct Slice
    {
        public DateTime _date;        // время поступдения среза данных
        public float[] slice;         // срез данных
                
        private int sliceSize;        // размер среза данных
        public int index;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="Lenght">Размер среза данных</param>
        public Slice(int Lenght)
        {
            index = -1;
            sliceSize = 1024;
            _date = DateTime.MinValue;            

            if (Lenght > 0)
            {
                slice = new float[Lenght];
                for (int i = 0; i < slice.Length; i++)
                {
                    slice[i] = float.NaN;
                }

                sliceSize = Lenght;
            }
            else
            {
                slice = new float[sliceSize];
                for (int i = 0; i < slice.Length; i++)
                {
                    slice[i] = float.NaN;
                }
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="Date">Время когда был сделан срез данных</param>
        /// <param name="Data">Срез данных</param>
        public Slice(DateTime Date, float[] Data)
        {
            index = -1;
            sliceSize = 1024;
            _date = DateTime.MinValue;

            if (Data != null)
            {
                _date = Date;
                slice = new float[Data.Length];

                Data.CopyTo(slice, 0);
            }
            else
            {
                _date = DateTime.MinValue;
                slice = new float[sliceSize];

                for (int i = 0; i < slice.Length; i++)
                {
                    slice[i] = float.NaN;
                }
            }
        }

        /// <summary>
        /// Возвращяет количество элементов в срезе данных
        /// </summary>
        public int Length
        {
            get
            {
                return slice.Length;
            }
        }

        /// <summary>
        /// Возвращяет значение парметра из среза данных
        /// </summary>
        /// <param name="index">Номер параметра в срезе данных</param>
        /// <returns>Значение параметра в срезе данных или float.NaN если индекс не корректен</returns>
        public float this[int index]
        {
            get
            {
                if (index > -1 && index < slice.Length)
                {
                    return slice[index];
                }
                else
                    return float.NaN;
            }
        }
    }
}