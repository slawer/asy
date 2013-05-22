using System;
using System.Threading;

using WCF;
using WCF.WCF_Client;

using Tcp;
using Buffering;

namespace ASY
{
    /// <summary>
    /// Реализует коммутатор, который отвечает за 
    /// соединение, прием и передачу данных от devMan
    /// </summary>
    public partial class Commutator
    {
        protected ReaderWriterLockSlim wcf_slim;                                // синхронизатор доступа к состоянию соединения
        protected WcfConnectionState wcf_state = WcfConnectionState.Default;    // состояние соединения к devMan

        /// <summary>
        /// Выполнить инициализацию подсистему WCF
        /// </summary>
        protected void InitializeWcf()
        {
            DevManClient.onConnected += new EventHandler(DevManClient_onConnected);
            DevManClient.onDisconnected += new EventHandler(DevManClient_onDisconnected);

            DevManClient.onReceive += new ReceivedEventHandler(DevManClient_onReceive);
        }

        /// <summary>
        /// Подключились к серверу данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DevManClient_onConnected(object sender, EventArgs e)
        {
            try
            {
                ConnectionState = WcfConnectionState.Conected;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteToLog(this, new ErrorArgs(ex.Message, ErrorType.NotFatal));
            }
        }

        /// <summary>
        /// Разорванно соединение с сервером данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DevManClient_onDisconnected(object sender, EventArgs e)
        {
            try
            {
                ConnectionState = WcfConnectionState.Disconnected;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteToLog(this, new ErrorArgs(ex.Message, ErrorType.NotFatal));
            }
        }

        private DateTime lastTime = DateTime.MinValue;                  // для вычисления интервала тишины
        private TimeSpan tInterval = new TimeSpan(0, 0, 0, 0, 300);     // время тишины для приема данных от сервера данных

        /// <summary>
        /// Получили данные от devMan
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        protected void DevManClient_onReceive(object sender, ReceivedEventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (now > lastTime)
                {
                    TimeSpan interval = now - lastTime;
                    if (interval.Ticks > tInterval.Ticks)
                    {
                        lastTime = now;
                        foreach (Parameter parameter in parameters)
                        {
                            PDescription channel = parameter.Channel;
                            if (channel != null)
                            {
                                if (channel.Number >= 0 && channel.Number < e.Slice.Length)
                                {
                                    CommutatorParameter.setCurrent(parameter, e.Slice[channel.Number]);
                                }
                            }
                        }

                        UpdateTechnologyParameters();
                        if (onUpdated != null)
                        {
                            onUpdated(this, new CommutatorEventArgs(e.Slice));
                        }
                    }
                }
                else
                    lastTime = now;
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteToLog(sender, new ErrorArgs(ex.Message, ErrorType.NotFatal));
            }
        }

        /// <summary>
        /// Передать технологически параметры серверу данных
        /// </summary>
        protected void UpdateTechnologyParameters()
        {
            try
            {
            }
            catch { }
        }

        /// <summary>
        /// Найти номер параметра на сервере данных, который необходимо обновить
        /// </summary>
        /// <param name="t_number">Номер параметра</param>
        /// <returns>Номер параметра который нужно обновить на сервере данных</returns>
        protected int NumberTechOnDev(int t_number)
        {
            if (t_number != -1)
            {
                foreach (Parameter parameter in parameters)
                {
                    PDescription channel = parameter.Channel;
                    if (channel != null && channel.Number == t_number)
                    {
                        //if (channel.Type == DeviceManager.FormulaType.Capture)
                        {
                            return channel.Number;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Вспомогательный класс для коммутатора.
        /// Неоходим для присваивания нового значения параметру.
        /// </summary>
        protected class CommutatorParameter : Parameter
        {
            public static void setCurrent(Parameter parameter, Single value)
            {
                Parameter.SetCurrentValue(parameter, value);
            }
        }
    }
}