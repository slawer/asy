using System.Net.Sockets;
using System.Collections.Generic;

namespace SoftwareDevelopmentKit.Servers.Tcp
{
    /// <summary>
    /// Реализует хранение данных в виде "кучи байтов"
    /// </summary>
    class Heap
    {
        private int m_totalSize;
        private int m_localSize;

        private int m_currentIndex;
        private byte[] m_buffer;

        private Stack<int> m_freeIndexPool;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="totalBytes">Общий размер кучи в байтах</param>
        /// <param name="localBytes">Размер одного сегмента данных в куче</param>
        public Heap(int totalBytes, int localBytes)
        {
            m_totalSize = totalBytes;
            m_localSize = localBytes;

            m_currentIndex = 0;
            m_freeIndexPool = new Stack<int>();

            m_buffer = new byte[m_totalSize];
        }

        /// <summary>
        /// Выделить байты
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool AllocBytes(SocketAsyncEventArgs e)
        {
            if (m_freeIndexPool.Count > 0)
            {
                e.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_localSize);
            }
            else
            {
                if ((m_totalSize - m_localSize) < m_currentIndex)
                {
                    return false;
                }
                e.SetBuffer(m_buffer, m_currentIndex, m_localSize);
                m_currentIndex += m_localSize;
            }
            return true;
        }

        /// <summary>
        /// Освободить байты
        /// </summary>
        /// <param name="e"></param>
        public void FreeBytes(SocketAsyncEventArgs e)
        {
            m_freeIndexPool.Push(e.Offset);
            e.SetBuffer(null, 0, 0);
        }
    }
}