using System;
using System.Threading;
using System.Collections.Generic;

using SoftwareDevelopmentKit.Servers.Tcp;

namespace ASY
{
    /// <summary>
    /// Реализует обмен пакетами по Tcp соединению 
    /// в старом режиме, по порту протоколу Dsn и порту 56000
    /// </summary>
    public partial class TcpAsyManager
    {
        // ----- Данные класса -------

        private TcpServer server;                   // основной TCP сервер
        private List<TcpAsyClient> clients;         // клиенты подключеннык к Tcp серверу

        private int m_port = 58000;                 // порт обмена
        private int m_numConnections = 10;          // количество одновременно подключенных клиентов
        private int m_receiveBuferSize = 512;       // размер буфера приема данных

        private Mutex mutex = null;                 // синхронизирует доступ к списку подключенных клиентов 
        private Mutex shareClientMutex;             // синхронизирует доступ к разделяемым данным для клиентов сервера

        /// <summary>
        /// Возникает при получении пакета
        /// </summary>
        public event PacketEventHandler OnPacket;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public TcpAsyManager()
        {
            server = new TcpServer();
            clients = new List<TcpAsyClient>();

            mutex = new Mutex(false);
            shareClientMutex = new Mutex(false);
        }

        /// <summary>
        /// Определяет количество одновременно подключенных клиентов
        /// </summary>
        public int CountConnections
        {
            get { return m_numConnections; }
            set { m_numConnections = value; }
        }

        /// <summary>
        /// Определяет размер буфера приема данных
        /// </summary>
        public int ReceiverBufferSize
        {
            get { return m_receiveBuferSize; }
            set { m_receiveBuferSize = value; }
        }

        /// <summary>
        /// Определяет порт обмена
        /// </summary>
        public int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        /// <summary>
        /// Количество подключенных клиентов в данный момент
        /// </summary>
        public int CountConnected
        {
            get 
            { 
                return server.NumberOfConnectedClients; 
            }
        }

        /// <summary>
        /// Tcp сервер, осуществляющий работу с сокетами
        /// </summary>
        public TcpServer Server
        {
            get { return server; }
        }

        /// <summary>
        /// Запустить Tcp сервер
        /// </summary>
        public void Start()
        {
            server = new TcpServer(m_numConnections, m_receiveBuferSize);

            server.OnConnect += new ServerEventHandler(OnConnect);
            server.OnDisconnect += new ServerEventHandler(OnDisconnect);
            
            server.OnReceive += new ServerReceiveEventHandler(OnReceive);

            server.Port = Port;
            server.Start();
        }

        /// <summary>
        /// Подключился к серверу клиент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_OnConnect(object sender, ServerEventArgs e)
        {
            TcpAsyClient client = new TcpAsyClient();

            client.Socket = e.Socket;
            client.Socket.SendTimeout = 1000;

            client.Share = shareClientMutex;
            client.OnPacket += new TcpAsyClient.PacketEventHandler(client_OnPacket);

            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    clients.Add(client);
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Разорвал с серверос соединение клиент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_OnDisconnect(object sender, ServerEventArgs e)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    foreach (TcpAsyClient client in clients)
                    {
                        if (client.Socket.Handle == e.Socket.Handle)
                        {
                            clients.Remove(client);
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Полученны данные от клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void server_OnReceive(object sender, ServerReceiveEventArgs e)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    foreach (TcpAsyClient client in clients)
                    {
                        if (client.Socket.Handle == e.Socket.Handle)
                        {
                            client.Insert(e.DataString);
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Остановить работу сервера
        /// </summary>
        public void Stop()
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;                 
                    server.Stop();

                    foreach (TcpAsyClient client in clients)
                    {
                        client.Socket.Close();
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Определяет интерфейс функции обрабатывающей событие OnPacket
        /// </summary>
        /// <param name="packet">Пакет</param>
        public delegate void PacketEventHandler(string packet);
    }
}