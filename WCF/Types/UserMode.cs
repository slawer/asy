using System;

namespace WCF
{
    /// <summary>
    /// Перечисляет возможные режими работы пользователя
    /// </summary>
    [Serializable]
    public enum UserMode
    {
        /// <summary>
        /// Активный режим. Пользователь сам инициирует передачу ему данных.
        /// </summary>
        Active,
        
        /// <summary>
        /// Пассивный режим. Пользователь получает от сервиса данные, когда сервис сам определит передавать данные
        /// </summary>
        Passive,

        /// <summary>
        /// Режим по умолчанию. Не установлен.
        /// </summary>
        Default
    }
}