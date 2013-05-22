using System;
using System.Threading;

using SoftwareDevelopmentKit.Servers.Tcp;

namespace ASY
{
    public partial class TcpAsyManager
    {
        /// <summary>
        /// Клиент установил соединение с сервером
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        void OnConnect(object sender, ServerEventArgs e)
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
        /// Клиент разорвал соединение с серверов 
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        void OnDisconnect(object sender, ServerEventArgs e)
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
        /// Поступили данные серверу
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        void OnReceive(object sender, ServerReceiveEventArgs e)
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
        /// Клиент выделил пакет из TCP
        /// </summary>
        /// <param name="packet"></param>
        void client_OnPacket(string packet)
        {
            if (OnPacket != null)
            {
                Interlocked.Increment(ref countPacketsReceive);
                OnPacket(packet);
            }
        }

        /// <summary>
        /// Отправить пакет по TCP
        /// </summary>
        /// <param name="packet">Пакет который отправить</param>
        /// <returns>Количество отправленных байт</returns>
        public int Send(string packet)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(100, false))
                {
                    blocked = true;
                    int sendBytes = 0;

                    byte[] tcp_packet = System.Text.Encoding.ASCII.GetBytes(packet);
                    foreach (TcpAsyClient client in clients)
                    {
                        try
                        {
                            sendBytes = client.Socket.Send(tcp_packet);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                client.Socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            }
                            catch
                            {
                                // ...
                            }

                            client.Socket.Close();
                            Interlocked.Increment(ref countBadClients);
                        }
                    }

                    if (sendBytes > 0)
                    {
                        Interlocked.Increment(ref countPacketsSend);
                        Interlocked.Add(ref sendingBytes, packet.Length);
                    }

                    return sendBytes;
                }
                else
                    throw new TimeoutException();
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }
    }
}