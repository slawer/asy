using System;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace Tcp
{
    /// <summary>
    /// Реализует подключение к devMan по старому Tcp соединению
    /// </summary>
    public class devTcpManager
    {
        protected ReaderWriterLockSlim slim_input;      // синхронизатор входного буфера

        protected StringBuilder input_buffer;           // входной буфер
        protected StringBuilder output_buffer;          // выходной буфер

        protected Regex regex;                          // осуществляет выделение пакетов из потока данных

        private Mutex mutex;                            // синхронизирует работу таймера
        private Timer timer;                            // осуществляет запуск процедуры извлекающей пакеты

        protected TcpClient client = null;              // клиент 

        /// <summary>
        /// Возникает когда был выделен пакет
        /// </summary>
        public event PacketEventHandler OnPacket;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public devTcpManager()
        {
            mutex = new Mutex();
            slim_input = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            input_buffer = new StringBuilder();
            output_buffer = new StringBuilder();

            regex = new Regex("@%[0-9A-Fa-f]*[\\$]", RegexOptions.IgnoreCase);

            timer = new Timer(new TimerCallback(CallBack), null, 0, 100);
            timer.Change(0, 50);

            // ------------------------- 

            client = new TcpClient();
            client.OnReceive += new ReceiveEventHandler(client_OnReceive);
        }

        /// <summary>
        /// Возвращяет клиента devMan
        /// </summary>
        public TcpClient Client
        {
            get
            {
                return client;
            }
        }

        /// <summary>
        /// Получили данные от devMan
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="data">Полученные данные</param>
        protected void client_OnReceive(object sender, byte[] data)
        {
            if (slim_input.TryEnterWriteLock(100))
            {
                try
                {
                    if (data != null && data.Length > 0)
                    {
                        input_buffer.Append(Encoding.ASCII.GetString(data));
                    }
                }
                finally
                {
                    slim_input.ExitWriteLock();
                }
            }
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
                if (mutex.WaitOne(0))
                {
                    blocked = true;
                    if (slim_input.TryEnterWriteLock(300))
                    {
                        try
                        {
                            if (input_buffer.Length > 0) output_buffer.Append(input_buffer.ToString());
                            input_buffer.Remove(0, input_buffer.Length);
                        }
                        finally
                        {
                            slim_input.ExitWriteLock();
                        }
                    }

                    // ------ пытаемся выделить пакет ------

                    if (regex.IsMatch(output_buffer.ToString()))
                    {
                        string bufferString = output_buffer.ToString();
                        MatchCollection colection = regex.Matches(bufferString);

                        foreach (Match match in colection)
                        {
                            int pos = bufferString.IndexOf(match.Value);
                            bufferString = bufferString.Remove(pos, match.Value.Length);

                            if (OnPacket != null)
                            {
                                OnPacket(match.Value);
                            }
                        }

                        output_buffer.Remove(0, output_buffer.Length);
                        output_buffer.Append(bufferString);
                    }
                }
            }
            catch { }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }
        
    }

    /// <summary>
    /// Определяет интерфейс функции, осуществляющей обработку события OnPacket
    /// </summary>
    /// <param name="packet">Передаваемый в событии пакет</param>
    public delegate void PacketEventHandler(string packet);
}