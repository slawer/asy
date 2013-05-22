using System;

namespace WCF
{
    /// <summary>
    /// Дескриптор пользователя системы
    /// </summary>
    [Serializable]
    public class Handle
    {
        protected Guid identifier;      // идентификатор пользователя

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public Handle()
        {
            identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Возвращяет идентификатор пользователя
        /// </summary>
        public Guid Identifier
        {
            get { return identifier; }
        }
    }
}