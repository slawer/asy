using System;
using DeviceManager;

namespace WCF
{
    /// <summary>
    /// Реализует хранение описания параметра
    /// </summary>
    [Serializable]
    public class PDescription
    {
        private int index = -1;             // номер параметра в списке
        private string description;         // текстовое описание параметра

        private FormulaType _type;          // тип формулы

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="type">Тип формулы</param>
        public PDescription(FormulaType type)
        {
            _type = type;
            description = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="number">Номер параметра в списке</param>
        /// <param name="desc">Описание параметра с указанным номером</param>
        /// <param name="type">Тип формулы</param>
        public PDescription(int number, string desc, FormulaType type)
        {
            _type = type;

            index = number;
            description = desc;
        }

        /// <summary>
        /// Определяет номер параметра в списке параметров
        /// </summary>
        public int Number
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Определяет строку, описывающую параметр
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Возвращяет тип формулы
        /// </summary>
        public FormulaType Type
        {
            get { return _type; }
        }
    }
}