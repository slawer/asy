using System;
using System.Threading;

namespace ASY
{
    /// <summary>
    /// Реализует обмен пакетами по Tcp соединению 
    /// в старом режиме, используя протоколу Dsn и порту 56000
    /// </summary>
    public partial class TcpAsyManager
    {
        private Int64 sendingBytes = 0;             // количество отправленных байт
        private Int64 countPacketsSend = 0;         // количество отправленных пакетов
        private Int64 countPacketsReceive = 0;      // количество полученных пакетов
        private Int64 countBadClients = 0;          // количество аварийных отключений клиентов сервера

        /// <summary>
        /// 
        /// </summary>
        public Int64 SendingBytes
        {
            get { return Interlocked.Read(ref sendingBytes); }
            set { Interlocked.Exchange(ref sendingBytes, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 TotalBytesRead
        {
            get { return server.TotalBytesRead; }
            set { server.TotalBytesRead = value; }
        }

        public Int64 PacketsSend
        {
            get { return Interlocked.Read(ref countPacketsSend); }
            set
            {
                Interlocked.Exchange(ref countPacketsSend, value);
            }
        }

        public Int64 PacketsReceive
        {
            get { return Interlocked.Read(ref countPacketsReceive); }
            set
            {
                Interlocked.Exchange(ref countPacketsReceive, value);
            }
        }

        public Int64 CountBadCliens
        {
            get { return Interlocked.Read(ref countBadClients); }
            set { Interlocked.Exchange(ref countBadClients, value); }
        }
    }
}