using System;
using System.Net.Sockets;

namespace SoftwareDevelopmentKit.Servers.Tcp
{
    /// <summary>
    /// Класс выполняющий хранение данных конкретного подключения
    /// </summary>
    internal class Client
    {
        private Socket m_socket;                // хранит сокет подключенный к удаленному узлу
        private Object m_tag;                   // объек используемый для идентификации конкретного экземпляра класса. Определяется пользователем

        // ------- Свойства класса --------

        /// <summary>
        /// Получает объект идентифицирующий данный экземпляр класса
        /// </summary>
        public Object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        /// <summary>
        /// Получает сокет данного объекта
        /// </summary>
        public Socket Socket
        {
            get { return m_socket; }
            set { m_socket = value; }
        }

        /// <summary>
        /// Инициализирует экземпляр класса
        /// </summary>
        public Client()
            : this(null)
        {
        }

        /// <summary>
        /// Инициализирует экземпляр класса
        /// </summary>
        /// <param name="socket">Сокет ассоциированный с данным объектом</param>
        public Client(Socket socket)
        {
            m_socket = socket;
            m_tag = new object();            
        }
    }
}