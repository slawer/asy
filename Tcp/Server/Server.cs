using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SoftwareDevelopmentKit.Servers.Tcp
{
    /// <summary>
    /// Реализует TCP сервер
    /// </summary>
    public class TcpServer
    {
        private Socket m_listenSocket;
        private Semaphore m_maxNumberAcceptedClients;

        private Heap m_memory;
        private SocketAsyncEventArgsPool m_asyncEventsPool;

        private int m_numConnections;
        private int m_numConnectedSockets;
                
        private Int64 m_totalBytesRead;
        private int m_receiveBufferSize;

        private int m_listenPort = 50000;
        private int optToPreAlloc = 0x02;

        private List<Socket> m_openSockets;

        // -------- События класса ---------

        /// <summary>
        /// Возникает когда к серверу подключается клиент
        /// </summary>
        public event ServerEventHandler OnConnect;

        /// <summary>
        /// Возникает во время разрыва связи клиента с сервером
        /// </summary>
        public event ServerEventHandler OnDisconnect;

        /// <summary>
        /// Возникает когда от клиента пришли данные
        /// </summary>
        public event ServerReceiveEventHandler OnReceive;

        /// <summary>
        /// Возникает когда сервер начал работу
        /// </summary>
        public event EventHandler OnServerStart;

        /// <summary>
        /// Возникает когда сервер остановил свою работу
        /// </summary>
        public event EventHandler OnServerStop;

        // ------------ defaults -----------

        private const int backlog = 0x64;
        private const int default_numConnections = 0x0A;
        private const int default_receiveBufferSize = 256;

        /// <summary>
        /// Порт по которому работать
        /// </summary>
        public int Port
        {
            get { return m_listenPort; }
            set { m_listenPort = value; }
        }

        /// <summary>
        /// Количество подключенных клиентов
        /// </summary>
        public int NumberOfConnectedClients
        {
            get { return m_numConnectedSockets; }
        }

        /// <summary>
        /// Получить сокет клиента по его индексу
        /// </summary>
        /// <param name="index">Индекс сокета</param>
        /// <returns>В случае успеха сокет с указанным индексов, в противном случае null</returns>
        public Socket this[int index]
        {
            get
            {
                if (index >= 0 && index < m_openSockets.Count)
                {
                    lock (m_openSockets)
                    {
                        return m_openSockets[index];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Общее количество полученных байтов от всех клиентов
        /// </summary>
        public Int64 TotalBytesRead
        {
            get { return Interlocked.Read(ref m_totalBytesRead); }
            set
            {
                Interlocked.Exchange(ref m_totalBytesRead, value); 
            }
        }
        
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public TcpServer()
            : this(default_numConnections, default_receiveBufferSize)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="numConnections">Максимально возможное количество подключенных клиентов</param>
        /// <param name="receiveBufferSize">Размер буфера выделяемый для каждого подключенного клиента для приема данных от данного клиента</param>
        public TcpServer(int numConnections, int receiveBufferSize)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;

            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;

            m_memory = new Heap(m_numConnections * m_receiveBufferSize * optToPreAlloc,
                m_receiveBufferSize);
            
            m_asyncEventsPool = new SocketAsyncEventArgsPool(m_numConnections);
            m_maxNumberAcceptedClients = new Semaphore(m_numConnections, m_numConnections);

            m_openSockets = new List<Socket>();
            InitServer();
        }

        /// <summary>
        /// Инициализировать сервер. Данный метод должен быть вызван перед использование сервера.
        /// </summary>
        private void InitServer()
        {
            SocketAsyncEventArgs asyncEvent = null;

            for (int i = 0; i < m_numConnections; i++)
            {
                asyncEvent = new SocketAsyncEventArgs();
                asyncEvent.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                asyncEvent.UserToken = new Client();
                
                m_memory.AllocBytes(asyncEvent);
                m_asyncEventsPool.Push(asyncEvent);
            }
        }

        /// <summary>
        /// Запуск сервера
        /// </summary>
        public void Start()
        {
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, m_listenPort);

            m_listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_listenSocket.Bind(EndPoint);
            m_listenSocket.Listen(backlog);

            if (OnServerStart != null)
            {
                OnServerStart(this, null);
            }

            Accept(null);
        }

        /// <summary>
        /// роверка возможности продключения к серверу клиентап
        /// </summary>
        /// <param name="e"> Представляет асинхронную операцию сокета.</param>
        private void Accept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(StartAcceptProcess);
            }
            else
            {
                e.AcceptSocket = null;
            }

            if (m_listenSocket != null)
            {
                m_maxNumberAcceptedClients.WaitOne();
                if (m_listenSocket != null)
                {
                    bool willRaiseEvent = m_listenSocket.AcceptAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessAccept(e);
                    }
                }
            }
        }

        /// <summary>
        /// Запуск процедуры регистрации клиента
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e"> Представляет асинхронную операцию сокета.</param>
        void StartAcceptProcess(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// Обработать поступивший запрос от клиента
        /// </summary>
        /// <param name="e"> Представляет асинхронную операцию сокета.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);

            lock (m_openSockets)
            {
                m_openSockets.Add(e.AcceptSocket);
            }

            SocketAsyncEventArgs async = m_asyncEventsPool.Pop();
            (async.UserToken as Client).Socket = e.AcceptSocket;            

            // ---- Сообщаем наружу ---------

            if (OnConnect != null)
            {
                OnConnect(this, new ServerEventArgs(e.AcceptSocket));
            }

            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(async);
            if (!willRaiseEvent)
            {
                ProcessReceive(async);
            }
            Accept(e);
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="e">Представляет асинхронную операцию сокета.</param>
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
                            OnReceive(this, new ServerReceiveEventArgs((e.UserToken as Client).Socket, data));
                        }
                        if ((e.UserToken as Client).Socket.Connected)
                            (e.UserToken as Client).Socket.ReceiveAsync(e);
                    }
                    else
                    {
                        CloseSocket(e);
                    }
                    break;

                default:

                    CloseSocket(e);
                    break;
            }
        }

        /// <summary>
        /// Завершение операции ввода/вывода
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Представляет асинхронную операцию сокета.</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:

                    ProcessReceive(e);
                    break;

                default:

                    break;
            }            
        }

        /// <summary>
        /// Закрытие соединения с клиентом
        /// </summary>
        /// <param name="e">Представляет асинхронную операцию сокета.</param>
        private void CloseSocket(SocketAsyncEventArgs e)
        {
            Client client = e.UserToken as Client;
            try
            {
                client.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            client.Socket.Close();

            lock (m_openSockets)
            {
                m_openSockets.Remove(client.Socket);
            }
            m_asyncEventsPool.Push(e);

            // ----- Сообщаем наружу ----------

            if (OnDisconnect != null)
            {
                OnDisconnect(this, new ServerEventArgs(client.Socket));
            }

            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClients.Release();
        }

        /// <summary>
        /// Остановка работы сервера
        /// </summary>
        public void Stop()
        {
            m_listenSocket.Close();
            m_listenSocket = null;

            if (OnServerStop != null)
            {
                OnServerStop(this, null);
            }
        }
    }

    /// <summary>
    /// Определяет интерфейс функции вызываемой при возникновении события Tcp сервера
    /// </summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Параметры события</param>
    public delegate void ServerEventHandler(object sender, ServerEventArgs e);

    /// <summary>
    /// Событие генерируемое сервером TCP
    /// </summary>
    public class ServerEventArgs : EventArgs
    {
        private Socket m_socket;

        /// <summary>
        /// Сокет клиента
        /// </summary>
        public Socket Socket
        {
            get { return m_socket; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="socket">Сокет для которого создается класс</param>
        public ServerEventArgs(Socket socket)
            : base()
        {
            m_socket = socket;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ServerReceiveEventHandler(object sender, ServerReceiveEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class ServerReceiveEventArgs : EventArgs
    {
        private Socket m_socket;
        private byte[] m_data;

        public Socket Socket
        {
            get { return m_socket; }
        }

        public byte[] DataBytes
        {
            get { return m_data; }
        }

        public string DataString
        {
            get
            {
                return System.Text.Encoding.ASCII.GetString(m_data);
            }
        }

        public ServerReceiveEventArgs(Socket socket, byte[] data)
            : base()
        {
            m_socket = socket;
            m_data = data;
        }
    }
}