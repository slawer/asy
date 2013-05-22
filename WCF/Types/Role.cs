using System;

namespace WCF
{
    /// <summary>
    /// Определяет роль выполняемую зарегистрированным клиентом в системе
    /// </summary>
    [Serializable]
    public enum Role
    {
        /// <summary>
        /// Базовая. Разрешено управлять процессом
        /// </summary>
        Basic,

        /// <summary>
        /// Общая. Разрешено только наблюдать за процессом
        /// </summary>
        Common,

        /// <summary>
        /// Роль не определена
        /// </summary>
        Default
    }
}