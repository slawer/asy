using System;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;

using WCF;

namespace WCF.WCF_Client
{
    /// <summary>
    /// Реализует клиента devMan
    /// </summary>
    public class DevManClient : ICallBack
    {
        private static InstanceContext context = new InstanceContext(new DevManClient());   // контекст клиента

        private static IService devMan = null;                                  // интерфейс взаимодействия с devMan
        private static Uri uri = new Uri("net.tcp://localhost:57000");     // объектное представление универсального кода ресурсов (URI)

        private static ConnectionState connectionState;                         // текущее состояние соединения
        private static DuplexChannelFactory<IService> proxy = null;             // прокси на devMan

        private static DevManClientContext devContext;                          // настройки пользователя системы

        /// <summary>
        /// Возникает когда установлено соединение с DeviceManager
        /// </summary>
        public static event EventHandler onConnected;

        /// <summary>
        /// Возникает когда разорвано соединение с DeviceManager
        /// </summary>
        public static event EventHandler onDisconnected;

        /// <summary>
        /// Возникает когда получен срез данных
        /// </summary>
        public static event ReceivedEventHandler onReceive;

        /// <summary>
        /// Настройка соединения с devMan
        /// </summary>
        public static Uri DevManUri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        private DevManClient()
        {
            devContext = new DevManClientContext();
            devContext.onUpdate += new EventHandler(devContext_onUpdate);

            connectionState = ConnectionState.Default;
        }

        /// <summary>
        /// Синхронизовать данные с DeviceManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void devContext_onUpdate(object sender, EventArgs e)
        {
            try
            {
                devMan.SetMode(devContext.Handle, devContext.Mode);
            }
            catch { }
        }

        /// <summary>
        /// Установить соединение с DeviceManager
        /// </summary>
        public static void Connect()
        {
            try
            {
                if (connectionState == ConnectionState.Disconnected || connectionState == ConnectionState.Default)
                {
                    NetTcpBinding binder = new NetTcpBinding(SecurityMode.None);
                    proxy = new DuplexChannelFactory<IService>(context, binder, new EndpointAddress(uri));

                    devMan = proxy.CreateChannel();
                    IChannel channel = devMan as IChannel;

                    channel.Faulted += new EventHandler(channel_Faulted);

                    channel.Closed += new EventHandler(channel_Closed);
                    channel.Closing += new EventHandler(channel_Closing);

                    channel.Opening += new EventHandler(channel_Opening);
                    channel.Opened += new EventHandler(channel_Opened);
                    
                    channel.BeginOpen(null, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Разорвать соединение с DeviceManager
        /// </summary>
        public static void Disconnect()
        {
            try
            {
                if (connectionState == ConnectionState.Connected)
                {
                    ((IChannel)devMan).Close();
                    proxy.Close();

                    connectionState = ConnectionState.Disconnected;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Запросить срез данных от DeviceManager
        /// </summary>
        public static void RequestSlice()
        {
            try
            {
                devMan.RequestDataSlice(devContext.Handle);
            }
            catch { }
        }

        /// <summary>
        /// Получить срез данных
        /// </summary>
        /// <returns>Срез данных</returns>
        public static Single[] GetSlice()
        {
            try
            {
                return devMan.GetDataSlice(devContext.Handle);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Канал обмена данными открывается
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        static void channel_Opening(object sender, EventArgs e)
        {            
        }

        /// <summary>
        /// Канал обмена данными открыт
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private static void channel_Opened(object sender, EventArgs e)
        {
            try
            {
                devContext.Handle = devMan.Register(devContext.Role);
                if (devContext.Handle != null)
                {
                    devContext.Update();
                    connectionState = ConnectionState.Connected;

                    if (onConnected != null)
                    {
                        onConnected(null, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Канал обмена данными закрывается
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private static void channel_Closing(object sender, EventArgs e)
        {            
        }

        /// <summary>
        /// Канал обмена данными закрыт
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private static void channel_Closed(object sender, EventArgs e)
        {
            try
            {
                connectionState = ConnectionState.Disconnected;
                if (onDisconnected != null)
                {
                    onDisconnected(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Канал обмена данными поврежден
        /// </summary>
        /// <param name="sender">Источник собыия</param>
        /// <param name="e">Параметры события</param>
        private static void channel_Faulted(object sender, EventArgs e)
        {
            try
            {
                switch (connectionState)
                {
                    case ConnectionState.Connected:

                        connectionState = ConnectionState.Disconnected;
                        if (onDisconnected != null)
                        {
                            onDisconnected(null, EventArgs.Empty);
                        }

                        break;

                    case ConnectionState.Disconnected:

                        break;

                    case ConnectionState.Default:

                        break;

                    default:

                        break;
                }

                Connect();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Передать пользователю весь срез данных
        /// </summary>
        /// <param name="Time">Время получения среза данных</param>
        /// <param name="Slice">Срез данных</param>
        public void SendAll(DateTime Time, Single[] Slice)
        {
            if (onReceive != null)
            {
                onReceive(null, new ReceivedEventArgs(Time, Slice));
            }
        }

        /// <summary>
        /// обновить значение параметра
        /// </summary>
        /// <param name="Number">Номер параметра в списке</param>
        /// <param name="Value">Значение которое присвоить обновляемому параметру</param>
        public static void UpdateParameter(int Number, float Value)
        {
            if (connectionState == ConnectionState.Connected)
            {
                devMan.SetParameterValue(devContext.Handle, Number, Value);
            }
        }

        /// <summary>
        /// Возвращяет текущие настройки пользователя системы
        /// </summary>
        public static DevManClientContext Context
        {
            get { return devContext; }
        }

        /// <summary>
        /// Возвращяет список параметров DeviceManager
        /// </summary>
        public static PDescription[] Parameters
        {
            get
            {
                try
                {
                    return devMan.GetParametersDescription();
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
                return null;
            }

        }

        /// <summary>
        /// Определяет свойства подключения к devMan
        /// </summary>
        public static Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }
    }

    /// <summary>
    /// Определяет интерфейс функции, осуществляющую обработку событий при взамодейтсвии с devMan
    /// </summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Данные события</param>
    public delegate void ReceivedEventHandler(Object sender, ReceivedEventArgs e);

    /// <summary>
    /// Реализует данные события
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        protected DateTime _time;               // время. смысл зависит от типа события
        protected Single[] slice;               // спосок переданных параметров

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public ReceivedEventArgs()
        {
            _time = DateTime.Now;
            slice = null;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="_Time">Время когда был сделан срез данных</param>
        /// <param name="_Slice">Срез данных</param>
        public ReceivedEventArgs(DateTime _Time, Single[] _Slice)
        {
            _time = _Time;
            slice = _Slice;
        }

        /// <summary>
        /// Возвращяет время
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
        }

        /// <summary>
        /// Возвращяет срез данных
        /// </summary>
        public Single[] Slice
        {
            get { return slice; }
        }
    }

    /// <summary>
    /// Перечисляет возможные состяния клиента DeviceManager
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Подключен к DeviceManager
        /// </summary>
        Connected,

        /// <summary>
        /// Не подключен к DeviceManager
        /// </summary>
        Disconnected,

        /// <summary>
        /// По умолчанию.
        /// </summary>
        Default
    }
}