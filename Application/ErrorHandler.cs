using System;
using System.Diagnostics;
using Microsoft.VisualBasic;

using Log;

namespace ASY
{
    /// <summary>
    /// Реализует обработку ошибок в приложении
    /// </summary>
    public class ErrorHandler
    {
        private static Journal journal = null;            // журнал событий ОС

        /// <summary>
        /// Возникает когда необходимо завершить работу приложения
        /// </summary>
        public static event EventHandler OnExit;

        /// <summary>
        /// инициализировать обработчик ошибок
        /// </summary>
        public static void InitializeErrorHandler()
        {
            if (journal == null)
            {
                journal = Journal.CreateInstance();
            }
        }

        /// <summary>
        /// Выполнить запись ошибки в жкрнал событий
        /// </summary>
        /// <param name="sender">Источник сообщения</param>
        /// <param name="args">Параметры сообщения</param>
        public static void WriteToLog(object sender, ErrorArgs args)
        {
            try
            {
                if (journal != null)
                {
                    switch (args.ErrorType)
                    {
                        case ErrorType.Information:

                            journal.Write(args.Message, EventLogEntryType.Information);
                            break;

                        case ErrorType.Warning:

                            journal.Write(args.Message, EventLogEntryType.Warning);
                            break;

                        case ErrorType.NotFatal:

                            journal.Write(args.Message, EventLogEntryType.Error);
                            break;

                        case ErrorType.Fatal:

                            journal.Write(args.Message, EventLogEntryType.Error);
                            if (OnExit != null)
                            {
                                OnExit(sender, new EventArgs());
                            }
                            break;

                        case ErrorType.Unknown:

                            journal.Write(args.Message, EventLogEntryType.Error);
                            break;

                        case ErrorType.Default:

                            journal.Write(args.Message, EventLogEntryType.Information);
                            break;
                    }
                }
                else
                {
                    journal = Journal.CreateInstance();
                    if (journal != null)
                    {
                        string message = string.Format("{0}{1}{2}", "Не был создан экземпляр класса Journal",
                            Constants.vbCrLf, "Сообщение приложения будет сохранено как Error!");

                        journal.Write(message, EventLogEntryType.Error);
                        journal.Write(args.Message, EventLogEntryType.Error);
                    }
                }
            }
            catch
            {
                // ...
            }
        }
    }
    
    /// <summary>
    /// определяет тип ошибки
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Информация о текущем состянии
        /// </summary>
        Information,

        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning,

        /// <summary>
        /// Ошибка не фатальная, приложение может продолжать работу
        /// </summary>
        NotFatal,

        /// <summary>
        /// Ошибка фатальная, приложение должно завершить свою работу
        /// </summary>
        Fatal,

        /// <summary>
        /// Не известная ошибка, приложение может продолжать работу
        /// </summary>
        Unknown,

        /// <summary>
        /// Тип ошибки не определен
        /// </summary>
        Default
    }

    /// <summary>
    /// Реализует класс в котором передаются параметры события об ошибке
    /// </summary>
    public class ErrorArgs : EventArgs
    {
        private String message = string.Empty;              // сообщение
        private ErrorType type = ErrorType.Default;         // тип ошибки

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public ErrorArgs()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="Notice">Сообщение</param>
        public ErrorArgs(string Notice)
        {
            message = Notice;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="Notice">Сообщение</param>
        /// <param name="TypeError">Тип ошибки</param>
        public ErrorArgs(string Notice, ErrorType TypeError)
        {
            message = Notice;
            type = TypeError;
        }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Тип ошибки
        /// </summary>
        public ErrorType ErrorType
        {
            get { return type; }
        }
    }

    /// <summary>
    /// Определяет интерфейс метода, осуществляющего обработку ошибок
    /// </summary>
    /// <param name="sender">Источник события</param>
    /// <param name="args">Параметры события</param>
    public delegate void ApplicationErrorHandler(object sender, ErrorArgs args);
}