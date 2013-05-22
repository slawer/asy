using System;
using System.Diagnostics;

namespace Log
{
    /// <summary>
    /// Реализует работу с журналом Windows
    /// </summary>
    public class Journal
    {
        // ---- константы класса ----

        private const string sourceName = "skc";         // Имя источника, регистрируемого в журнале и используемого при записи в журнал событий  

        // ---- данные класса ----
        
        private EventLog _log;                              // Предоставляет возможности взаимодействия с журналами событий Windows.
        private static Journal journal = null;              // Реализуем синглетон

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        protected Journal()
        {
            bool find = EventLog.SourceExists(sourceName);
            if (!find)
            {
                EventLog.CreateEventSource(sourceName, "SKCII");
            }

            _log = new EventLog();
            _log.Source = sourceName;
        }

        /// <summary>
        /// Получить класс реализующего работу с журналом событий Windows
        /// </summary>
        /// <returns>Класс реализующего работу с журналом событий Windows</returns>
        public static Journal CreateInstance()
        {
            try
            {
                if (journal == null)
                {
                    journal = new Journal();
                }
                return journal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Выполняет запись данных в журнал событий Windows
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="type">Тип сообщения</param>
        public void Write(string message, EventLogEntryType type)
        {
            _log.WriteEntry(message, type);
        }
    }    
}