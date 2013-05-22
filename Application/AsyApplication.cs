using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

using SoftwareDevelopmentKit.Servers.Tcp;

namespace ASY
{
    /// <summary>
    /// Реализует основное приложение
    /// </summary>
    public partial class AsyApplication
    {
        protected static AsyApplication _app = null;    // приложение

        // --------------------------------------------------------------------------

        protected TcpAsyManager server = null;          // Tcp сервер
        protected Commutator commutator = null;         // коммутатор
        
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        protected AsyApplication()
        {
            commutator = new Commutator();

            server = new TcpAsyManager();
            server.OnPacket += new TcpAsyManager.PacketEventHandler(server_OnPacket);
        }

        /// <summary>
        /// Возвращяет коммутатор
        /// </summary>
        public Commutator Commutator
        {
            get
            {
                return commutator;
            }
        }

        /// <summary>
        /// Возвращяет Tcp сервер приложения
        /// </summary>
        public TcpAsyManager Server
        {
            get
            {
                return server;
            }
        }

        /// <summary>
        /// Получить основное приложение
        /// </summary>
        /// <returns></returns>
        public static AsyApplication CreateInstance()
        {
            if (_app == null)
            {
                _app = new AsyApplication();
                
                _app.Initialize();
                ErrorHandler.InitializeErrorHandler();
            }

            return _app;
        }

        /// <summary>
        /// Выполнить инициализацию приложения
        /// </summary>
        protected void Initialize()
        {
            try
            {
            }
            catch { }
        }

        /// <summary>
        /// Подключиться к серверу данных
        /// </summary>
        public void Connect()
        {
            try
            {
                commutator.ConnectToServer();
                server.Start();
            }
            catch { }
        }

        /// <summary>
        /// Выделить число с плавающей точкой из строки
        /// </summary>
        /// <param name="single">Строка содержащая число</param>
        /// <returns>Значение или Nan если не удалось выполнить преобразование</returns>
        public static float ParseSingle(string single)
        {
            try
            {
                string ds = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                string value = single;

                value = value.Replace(".", ds);
                value = value.Replace(",", ds);

                return float.Parse(value);
            }
            catch
            { }

            return float.NaN;
        }

        /// <summary>
        /// Выделить число с плавающей точкой из строки
        /// </summary>
        /// <param name="single">Строка содержащая число</param>
        /// <returns>Значение или Nan если не удалось выполнить преобразование</returns>
        public static double ParseDouble(string single)
        {
            try
            {
                string ds = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                string value = single;

                value = value.Replace(".", ds);
                value = value.Replace(",", ds);

                return double.Parse(value);
            }
            catch
            { }

            return double.NaN;
        }

        /// <summary>
        /// Получить параметр, по его индексу в срезе данных
        /// </summary>
        /// <param name="IndexOnSlice">Индекс параметра в срезе данных</param>
        /// <returns>Параметр, который соответствует индексу в срезе данных</returns>
        public Parameter GetParameter(int IndexOnSlice)
        {
            try
            {
                foreach (Parameter parameter in commutator.Parameters)
                {
                    if (parameter != null)
                    {
                        if (parameter.Channel != null)
                        {
                            if (IndexOnSlice == parameter.Channel.Number)
                            {
                                return parameter;
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        // ------------------------------ сохранение/загрузка ------------------------------

        /// <summary>
        /// имя конфигурационного файла
        /// </summary>
        protected const string sgt_cfg_file_name = "asy.xml";

        /// <summary>
        /// Корневой узел конфигурации
        /// </summary>
        protected const string RootName = "asy_configuration";

        /// <summary>
        /// Сохранить конфигурацию приложения
        /// </summary>
        public void Save()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                XmlElement root = doc.CreateElement(RootName);

                doc.AppendChild(root);

                commutator.Save(doc, root);
                doc.Save(string.Format("{0}\\{1}", Environment.CurrentDirectory, sgt_cfg_file_name));
            }
            catch { }
        }

        /// <summary>
        /// Загрузить конфигурацию приложения
        /// </summary>
        public void Load()
        {
            XmlDocument document = null;
            try
            {
                string path = Environment.CurrentDirectory;
                string totalPathCfg = string.Format("{0}\\{1}", path, sgt_cfg_file_name);

                if (System.IO.File.Exists(totalPathCfg))
                {
                    document = new XmlDocument();

                    document.Load(totalPathCfg);
                    XmlNode root = document.FirstChild;

                    if (root != null)
                    {
                        if (root.Name == RootName)
                        {
                            if (root.HasChildNodes)
                            {
                                foreach (XmlNode child in root.ChildNodes)
                                {
                                    switch (child.Name)
                                    {
                                        case Commutator.RootName:

                                            commutator.Load(child);
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // ---------------------------------------------------------------------------------
    }
}