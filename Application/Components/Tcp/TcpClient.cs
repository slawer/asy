using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Tcp
{
    /// <summary>
    /// Реализует Tcp клиента
    /// </summary>
    public class TcpClient
    {
        // ------ Данные класса --------

        private Socket socket = null;
        private SocketAsyncEventArgs async = null;

        private int _port;
        private string _host;

        byte[] buffer;
        private Int64 m_totalBytesRead = 0;

        // ------ свойства ---------

        /// <summary>
        /// Определяет порт подключения
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Определяет адрес удаленного хоста
        /// </summary>
        public string Host
        {
            get { return _host; }
            set
            {
                if (value == "localhost")
                {
                    _host = "127.0.0.1";
                }
                else
                    _host = value; 
            }
        }

        /// <summary>
        /// Показывает подключен ли в данный момент клиент к серверу
        /// </summary>
        public bool Connected
        {
            get 
            {
                if (socket != null)
                {
                    return socket.Connected;
                }
                return false;
            }
        }

        /// <summary>
        /// Определяет таймаут для отправки данных удаленному хосту
        /// </summary>
        public int SendTimeout { get { return 3000; } }

        // -------- События ---------------

        /// <summary>
        /// Возникает при успешном соединении с удаленным хостом
        /// </summary>
        public event EventHandler OnConnect;

        /// <summary>
        /// Возникает при разрыве соединения с удаленным хостом
        /// </summary>
        public event EventHandler OnDisconnect;

        /// <summary>
        /// Возникает когда были полученны данные от удаленного хоста
        /// </summary>
        public event ReceiveEventHandler OnReceive; 

        // -------- Конструктор -------

        /// <summary>
        /// инициализирует новый экземпляр класса
        /// </summary>
        public TcpClient()
        {
            async = null;
            socket = null;

            _port = 56000;
            _host = "127.0.0.1";

            buffer = new byte[10240];
        }

        // -------- подключиться к серверу --------

        /// <summary>
        /// Установить соединение с сервером
        /// </summary>
        public void Connect()
        {
            try
            {
                IPEndPoint ePoint = new IPEndPoint(IPAddress.Parse(_host), _port);

                async = new SocketAsyncEventArgs();
                async.RemoteEndPoint = ePoint;

                async.Completed += new EventHandler<SocketAsyncEventArgs>(async_Completed);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ConnectAsync(async);
            }
            catch (Exception ex)
            {
                socket = null;
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Завершенно асинхронное событие
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Представляет асинхронную операцию сокета</param>
        private void async_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:

                    ProcessReceive(e);
                    break;

                case SocketAsyncOperation.Connect:

                    if (socket.Connected)
                    {
                        e.SetBuffer(buffer, 0, buffer.Length);
                        if (OnConnect != null) OnConnect(this, null);

                        socket.SendTimeout = SendTimeout;
                        socket.ReceiveAsync(e);
                    }
                    else
                        socket.ConnectAsync(e);

                    break;

                default:

                    break;
            }
        }

        /// <summary>
        /// Извлекаем данные
        /// </summary>
        /// <param name="e">Представляет асинхронную операцию сокета</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            switch (e.SocketError)
            {
                case SocketError.Success:

                    if (e.BytesTransferred > 0)
                    {
                        Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);

                        // ------ сообщаем наружу --------

                        if (OnReceive != null)
                        {
                            byte[] data = new byte[e.BytesTransferred];
                            Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);

                            OnReceive(this, data);                            
                        }
                        if (socket.Connected) socket.ReceiveAsync(e);
                        else CloseSocket();
                    }
                    else
                    {
                        CloseSocket();
                    }
                    break;

                case SocketError.ConnectionReset:

                    Connect();
                    break;

                case SocketError.OperationAborted:

                    break;

                default:

                    CloseSocket();
                    break;
            }
        }

        /// <summary>
        /// Закрытие соединения с клиентом
        /// </summary>
        private void CloseSocket()
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }

                socket.Close();
                socket = null;

                // ----- Сообщаем наружу ----------

                if (OnDisconnect != null) OnDisconnect(this, null);
            }
        }  
      
        /// <summary>
        /// Закрыть соединение
        /// </summary>
        public void Disconnect()
        {
            socket.Disconnect(false);
        }

        /// <summary>
        /// Отправляет массив байтов серверу
        /// </summary>
        /// <param name="data">Данные которые необходимо отправить</param>
        /// <returns>Количество переданных байт</returns>
        public int Send(byte[] data)
        {
            int sended = -1;
            try
            {
                if (socket != null && socket.Connected)
                {
                    sended = socket.Send(data);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }
                socket.Close();
                throw new Exception(ex.Message, ex.InnerException);
            }
            return sended;
        }
    }

    public delegate void ReceiveEventHandler(object sender, byte[] data);
}