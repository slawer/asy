using System;
using System.Threading;

using Buffering;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    /// <summary>
    /// Реализует коммутатор, который отвечает за 
    /// соединение, прием и передачу данных от devMan
    /// </summary>
    public partial class Commutator
    {
        /// <summary>
        /// Возникает когда получены параметры от сервера данных и готовы для обработки
        /// </summary>
        public event CommutatorEventHandler onUpdated;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public Commutator()
        {
            parameters = new Parameter[256];

            p_slim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            wcf_slim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = new Parameter();
            }            

            speed_1 = new Parameter();
            speed_1.Name = "скорость двигателя насоса 1";
            
            speed_2 = new Parameter();
            speed_2.Name = "скорость двигателя насоса 2";

            speed_rotor = new Parameter();
            speed_rotor.Name = "скорость двигателя ротора";
            
            torque_rotor = new Parameter();
            torque_rotor.Name = "крутящий момент ротора";

            wedges_state = new Parameter();
            wedges_state.Name = "клинья подняты/опущены";
        
            diameter_1 = new Parameter();
            diameter_1.Name = "диаметер поршня 1";
            
            diameter_2 = new Parameter();
            diameter_2.Name = "диаметер поршня 2";

            force = new Parameter();
            force.Name = "Усилие в гидрораскрепителе";

            code_button = new Parameter();
            code_button.Name = "Код нажатой кнопки";

            InitializeWcf();
        }

        /// <summary>
        /// Возвращяет параметры получаемые от сервера данных
        /// </summary>
        public Parameter[] Parameters
        {
            get
            {
                return parameters;
            }
        }

        /// <summary>
        /// Возвращяет текущее состояние подключения к серверу данных
        /// </summary>
        public WcfConnectionState ConnectionState
        {
            get
            {
                if (wcf_slim.TryEnterReadLock(300))
                {
                    try
                    {
                        return wcf_state;
                    }
                    finally
                    {
                        wcf_slim.ExitReadLock();
                    }
                }

                return WcfConnectionState.Default;
            }

            protected set
            {
                if (wcf_slim.TryEnterWriteLock(500))
                {
                    try
                    {
                        wcf_state = value;
                    }
                    finally
                    {
                        wcf_slim.ExitWriteLock();
                    }
                }
            }
        }

        /// <summary>
        /// Подключиться к серверу данных
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                DevManClient.Connect();
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteToLog(this, new ErrorArgs(ex.Message, ErrorType.NotFatal));
            }
        }

        /// <summary>
        /// Адрес сервера данных
        /// </summary>
        public Uri DevManUri
        {
            get
            {
                return DevManClient.Uri;
            }

            set
            {
                DevManClient.Uri = value;
            }
        }
    }

    public delegate void SaverTechnologyData(float[] slice);

    /// <summary>
    /// Возможные состояния подключения к серверу данных
    /// </summary>
    public enum WcfConnectionState
    {
        /// <summary>
        /// Подключен к серверу данных
        /// </summary>
        Conected,

        /// <summary>
        /// Не подключен к серверу данных
        /// </summary>
        Disconnected,

        /// <summary>
        /// Стартовое состояние, не осуществлялась работа с сервером данных.(по умолчаню)
        /// </summary>
        Default
    }

    /// <summary>
    /// Интерфейс функции обрабатывающей событие получение среза данных коммутатором
    /// </summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Параметры события</param>
    public delegate void CommutatorEventHandler(Object sender, CommutatorEventArgs e);

    /// <summary>
    /// Класс посредством которого передаются данные
    /// </summary>
    public class CommutatorEventArgs : EventArgs
    {
        protected float[] _slice = null;             // передаваемый срез данных

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public CommutatorEventArgs()
            :base()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="slice">Передаваемый срез данных</param>
        public CommutatorEventArgs(float[] slice)
            : base()
        {
            _slice = slice;
        }

        /// <summary>
        /// Возвращяет срез данных
        /// </summary>
        public Single[] Slice
        {
            get
            {
                return _slice;
            }
        }
    }
}