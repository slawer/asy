using System;
using System.Xml;

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
        // ------------------------------ сохранение/загрузка ------------------------------

        /// <summary>
        /// Корневой узел коммутатора
        /// </summary>
        public const string RootName = "commutator";

        /// <summary>
        /// Корневой узел списка параметров
        /// </summary>
        protected const string ParametersName = "parameters";

        /// <summary>
        /// узел в котором сохраняется информация об uri
        /// </summary>
        protected const string UriName = "uri";

        /// <summary>
        /// Узел в котором сохраняется скорость двигателя насоса 1
        /// </summary>
        protected const string speed_1Name = "speed_1";
        
        /// <summary>
        /// Узел в котором сохраняется скорость двигателя насоса 2
        /// </summary>
        protected const string speed_2Name = "speed_2";

        /// <summary>
        /// Узел в котором сохраняется скорость двигателя ротора
        /// </summary>
        protected const string speed_rotorName = "speed_rotor";

        /// <summary>
        /// Узел в котором сохраняется крутящий момент ротора
        /// </summary>
        protected const string torque_rotorName = "torque_rotor";

        /// <summary>
        /// Узел в котором сохраняется клинья подняты/опущены
        /// </summary>
        protected const string wedges_stateName = "wedges_state";

        /// <summary>
        /// Узел в котором сохраняется диаметер поршня 1
        /// </summary>
        protected const string diameter_1Name = "diameter_1";

        /// <summary>
        /// Узел в котором сохраняется диаметер поршня 2
        /// </summary>
        protected const string diameter_2Name = "diameter_2";

        /// <summary>
        /// Узел в котором сохраняется усилие в гидрораскрепителе
        /// </summary>
        protected const string forceName = "force";

        /// <summary>
        /// Узел в котором сохраняется код кнопки
        /// </summary>
        protected const string codeButtonName = "code_button";

        /// <summary>
        /// Сохранить конфигурацию приложения
        /// </summary>
        /// <param name="doc">Документ в который осуществляется сохранение конфигурации</param>
        /// <param name="Root">Корневой узел в который необходимо сохранить конфигурацию коммутатора</param>
        public void Save(XmlDocument doc, XmlNode Root)
        {
            try
            {
                if (Root != null)
                {
                    XmlNode commutatorNode = doc.CreateElement(RootName);
                    XmlNode devManUriNode = doc.CreateElement(UriName);

                    devManUriNode.InnerText = DevManClient.DevManUri.OriginalString;

                    commutatorNode.AppendChild(devManUriNode);
                    
                    SaveParameters(doc, commutatorNode);                  

                    XmlNode speed_1Node = doc.CreateElement(speed_1Name);
                    XmlNode speed_2Node = doc.CreateElement(speed_2Name);

                    XmlNode speed_rotorNode = doc.CreateElement(speed_rotorName);
                    XmlNode torque_rotorNode = doc.CreateElement(torque_rotorName);

                    XmlNode wedges_stateNode = doc.CreateElement(wedges_stateName);
        
                    XmlNode diameter_1Node = doc.CreateElement(diameter_1Name);
                    XmlNode diameter_2Node = doc.CreateElement(diameter_2Name);

                    XmlNode forceNode = doc.CreateElement(forceName);
                    XmlNode codeButtonNode = doc.CreateElement(codeButtonName);

                    speed_1Node.AppendChild(speed_1.Save(doc));
                    speed_2Node.AppendChild(speed_2.Save(doc));

                    speed_rotorNode.AppendChild(speed_rotor.Save(doc));
                    torque_rotorNode.AppendChild(torque_rotor.Save(doc));

                    wedges_stateNode.AppendChild(wedges_state.Save(doc));

                    diameter_1Node.AppendChild(diameter_1.Save(doc));
                    diameter_2Node.AppendChild(diameter_2.Save(doc));

                    forceNode.AppendChild(force.Save(doc));
                    codeButtonNode.AppendChild(code_button.Save(doc));

                    commutatorNode.AppendChild(speed_1Node);
                    commutatorNode.AppendChild(speed_2Node);
                    commutatorNode.AppendChild(speed_rotorNode);
                    commutatorNode.AppendChild(torque_rotorNode);
                    commutatorNode.AppendChild(wedges_stateNode);
                    commutatorNode.AppendChild(diameter_1Node);
                    commutatorNode.AppendChild(diameter_2Node);
                    commutatorNode.AppendChild(forceNode);
                    commutatorNode.AppendChild(codeButtonNode);

                    Root.AppendChild(commutatorNode);
                }
            }
            catch { }
        }

        /// <summary>
        /// Сохранить параметры
        /// </summary>
        /// <param name="doc">Документ в который осуществляется сохранение</param>
        /// <param name="root">Корневой узел, в который сохранять параметры</param>
        protected void SaveParameters(XmlDocument doc, XmlNode root)
        {
            if (p_slim.TryEnterWriteLock(500))
            {
                try
                {
                    if (doc != null && root != null)
                    {
                        XmlNode p_root = doc.CreateElement(ParametersName);
                        if (p_root != null)
                        {
                            foreach (Parameter parameter in parameters)
                            {
                                if (parameter != null)
                                {
                                    XmlNode p_node = parameter.Save(doc);
                                    if (p_node != null)
                                    {
                                        p_root.AppendChild(p_node);
                                    }
                                }
                            }

                            root.AppendChild(p_root);
                        }
                    }
                }
                catch { }
                finally
                {
                    p_slim.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Загрузить конфигурацию приложения
        /// </summary>
        /// <param name="Root">Корневой узел, в котором сохранена конфигурация коммутатора</param>
        public void Load(XmlNode Root)
        {
            if (Root != null && Root.HasChildNodes)
            {
                foreach (XmlNode Child in Root.ChildNodes)
                {
                    switch (Child.Name)
                    {
                        case UriName:

                            try
                            {
                                DevManClient.DevManUri = new Uri(Child.InnerText);
                            }
                            catch { }
                            break;

                        case ParametersName:

                            try
                            {
                                LoadParameters(Child);
                            }
                            catch { }
                            break;

                        case speed_1Name:

                            try
                            {
                                LoadAsyParameter(Child, speed_1);
                            }
                            catch { }
                            break;

                        case speed_2Name:

                            try
                            {
                                LoadAsyParameter(Child, speed_2);
                            }
                            catch { }
                            break;
                            
                        case speed_rotorName:

                            try
                            {
                                LoadAsyParameter(Child, speed_rotor);
                            }
                            catch { }
                            break;

                        case torque_rotorName:

                            try
                            {
                                LoadAsyParameter(Child, torque_rotor);
                            }
                            catch { }
                            break;
                            
                        case wedges_stateName:

                            try
                            {
                                LoadAsyParameter(Child, wedges_state);
                            }
                            catch { }
                            break;

                        case diameter_1Name:

                            try
                            {
                                LoadAsyParameter(Child, diameter_1);
                            }
                            catch { }
                            break;

                        case diameter_2Name:

                            try
                            {
                                LoadAsyParameter(Child, diameter_2);
                            }
                            catch { }
                            break;

                        case forceName:

                            try
                            {
                                LoadAsyParameter(Child, force);
                            }
                            catch { }
                            break;

                        case codeButtonName:

                            try
                            {
                                LoadAsyParameter(Child, code_button);
                            }
                            catch { }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузить параметр АСУ
        /// </summary>
        /// <param name="root">Корневой узел, в которос содержится параметр</param>
        /// <param name="parameter">Параметр в который загрузить данные</param>
        protected void LoadAsyParameter(XmlNode root, Parameter parameter)
        {
            if (root != null && root.HasChildNodes)
            {
                foreach (XmlNode child in root.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case Parameter.parameterRoot:

                            try
                            {
                                parameter.Load(child);
                            }
                            catch { }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузить параметры коммутатора
        /// </summary>
        /// <param name="p_root">Корневой узел списка параметров</param>
        protected void LoadParameters(XmlNode p_root)
        {
            if (p_slim.TryEnterWriteLock(300))
            {
                try
                {
                    if (p_root != null && p_root.HasChildNodes)
                    {
                        int free_number = 0;
                        foreach (XmlNode Child in p_root)
                        {
                            switch (Child.Name)
                            {
                                case Parameter.parameterRoot:

                                    try
                                    {
                                        parameters[free_number++].Load(Child);
                                    }
                                    catch { }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    p_slim.ExitWriteLock();
                }
            }
        }

        // ---------------------------------------------------------------------------------
    }
}