using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ASY
{
    /// <summary>
    /// Реализует клиента сервера
    /// </summary>
    public class TcpAsyClient
    {
        private Socket socket;                          // сокет по которому подключен клиент к серверу

        private Mutex mutex;                            // реализует синхронный доступ к данным класса
        private Mutex outerMutex;                       // реализует синхронный доступ к разделяемым ресурсам между клиентами сервера

        private Timer timer;                            // осуществляет запуск процедуры извлекающей пакеты            
        private Translater translater;                  // осуществляет парсинг данных

        private StringBuilder input;                    // входной буфер
        private StringBuilder output;                   // выходной буфер

        private Regex regex;                            // осуществляет выделение пакетов из потока данных

        /// <summary>
        /// Возникает когда был выделен пакет
        /// </summary>
        public event PacketEventHandler OnPacket;

        /// <summary>
        /// Определяет сокет подключенного к серверу клиента
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }

        /// <summary>
        /// Определяет мьютекс, синхронизирующий доступ к общим ресурсам
        /// </summary>
        public Mutex Share
        {
            get { return outerMutex; }
            set { outerMutex = value; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public TcpAsyClient()
        {
            socket = null;
            
            outerMutex = null;
            mutex = new Mutex(false);

            input = new StringBuilder();
            output = new StringBuilder();

            translater = new Translater(TranslaterFunction);
            regex = new Regex(":[0-9a-fA-F]*[\0]", RegexOptions.IgnoreCase);

            timer = new Timer(new TimerCallback(CallBack), null, 0, 100);
            timer.Change(0, 50);
        }

        /// <summary>
        /// Процедура таймера
        /// </summary>
        /// <param name="state">Не используется</param>
        private void CallBack(object state)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    blocked = true;
                    lock (input)
                    {
                        if (input.Length > 0) output.Append(input.ToString());
                        input.Remove(0, input.Length);
                    }

                    translater.BeginInvoke(null, null);
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Попытаться выделить пакет из пришедших данных
        /// </summary>
        private void TranslaterFunction()
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(5000, false))
                {
                    blocked = true;
                    if (regex.IsMatch(output.ToString()))
                    {
                        string bufferString = output.ToString();
                        MatchCollection colection = regex.Matches(bufferString);

                        foreach (Match match in colection)
                        {
                            int pos = bufferString.IndexOf(match.Value);
                            bufferString = bufferString.Remove(0, match.Value.Length + pos);

                            //bufferString = bufferString.Remove(pos, match.Value.Length);

                            Signalize(match.Value);
                        }

                        output.Remove(0, output.Length);
                        output.Append(bufferString);
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Оповестить о том, что был выделен пакет
        /// </summary>
        /// <param name="Packet">Выделеннй пакет</param>
        private void Signalize(string Packet)
        {
            bool blocked = false;
            try
            {
                if (outerMutex.WaitOne(3000, false))
                {
                    blocked = true;
                    if (OnPacket != null)
                    {
                        OnPacket(Packet);
                    }
                }
            }
            finally
            {
                if (blocked) outerMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Добавить данные
        /// </summary>
        /// <param name="data">Добавляемы данные</param>
        public void Insert(string data)
        {
            //data = data.Replace("\0", string.Empty);

            lock (input)
            {
                input.Append(data);
            }

            if (mutex.WaitOne(0, false))
            {
                if (input.Length > 0) output.Append(input.ToString());
                input.Remove(0, input.Length);
                
                translater.BeginInvoke(null, null);
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Определяет интерфейс функции осуществляющей выделение пакета из данных
        /// </summary>
        private delegate void Translater();

        /// <summary>
        /// Определяет интерфейс функции, осуществляющей обработку события OnPacket
        /// </summary>
        /// <param name="packet">Передаваемый в событии пакет</param>
        public delegate void PacketEventHandler(string packet);
    }
}