using System;
using System.Threading;
using System.Collections.Generic;

using WCF;

namespace WCF.WCF_Client
{
    /// <summary>
    /// Хранит текущие настройки пользователя DeviceManager
    /// </summary>
    public class DevManClientContext
    {
        private Handle handle = null;                       // дескриптор пользователя

        private Role role = Role.Common;                    // роль выполняемая системой
        private UserMode mode = UserMode.Passive;           // режим работы с сервером

        private ReaderWriterLock s_locker = null;           // синхронизатор
        
        /// <summary>
        /// Возникает когда данные необходимо синхронизовать с DeviceManager
        /// </summary>
        public event EventHandler onUpdate;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public DevManClientContext()
        {
            s_locker = new ReaderWriterLock();  
        }

        /// <summary>
        /// Определяет идентификатор пользователя
        /// </summary>
        public Handle Handle
        {
            get
            {
                try
                {
                    s_locker.AcquireReaderLock(100);
                    try
                    {
                        return handle;
                    }
                    finally
                    {
                        s_locker.ReleaseReaderLock();
                    }
                }
                catch { }
                return null;
            }

            set
            {
                try
                {
                    s_locker.AcquireWriterLock(100);
                    try
                    {
                        handle = value;
                    }
                    finally
                    {
                        s_locker.ReleaseWriterLock();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Определяет роль которую выполняет пользователь системы
        /// </summary>
        public Role Role
        {
            get
            {
                try
                {
                    s_locker.AcquireReaderLock(100);
                    try
                    {
                        return role;
                    }
                    finally
                    {
                        s_locker.ReleaseReaderLock();
                    }
                }
                catch { }
                return WCF.Role.Default;
            }

            set
            {
                try
                {
                    s_locker.AcquireWriterLock(100);
                    try
                    {
                        role = value;
                    }
                    finally
                    {
                        s_locker.ReleaseWriterLock();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// определяет режим в котором работает пользователь системы
        /// </summary>
        public UserMode Mode
        {
            get
            {
                try
                {
                    s_locker.AcquireReaderLock(100);
                    try
                    {
                        return mode;
                    }
                    finally
                    {
                        s_locker.ReleaseReaderLock();
                    }
                }
                catch { }
                return UserMode.Default;
            }

            set
            {
                try
                {
                    s_locker.AcquireWriterLock(100);
                    try
                    {
                        mode = value;
                    }
                    finally
                    {
                        s_locker.ReleaseWriterLock();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Синхронизовать данные с DeviceManager
        /// </summary>
        internal void Update()
        {
            if (onUpdate != null)
            {
                onUpdate(this, EventArgs.Empty);
            }
        }
    }
}