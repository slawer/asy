using System;
using System.Xml;

namespace ASY
{
    /// <summary>
    /// Реализует базовый параметр, которым оперирует приложение
    /// </summary>
    public partial class Parameter
    {
        /// <summary>
        /// узел в который сохраняется параметр
        /// </summary>
        public const string parameterRoot = "parameter";

        /// <summary>
        /// узел в котором хранится название параметра
        /// </summary>
        private const string parameterName = "name";

        /// <summary>
        /// узел в котором хранится описание параметра (краткое имя параметра)
        /// </summary>
        private const string descriptionName = "desc";

        /// <summary>
        /// узел в котором храниться единицы измерения параметра
        /// </summary>
        private const string unitsName = "units";

        /// <summary>
        /// узел в котором хранится аварийное значение
        /// </summary>
        private const string alarmValueName = "alarm";

        /// <summary>
        /// узел в котором хранится блокировочное значение
        /// </summary>
        private const string blockingValueName = "blocking";

        /// <summary>
        /// узел в котором хранится контролировать или нет аварийное значение параметра
        /// </summary>
        private const string controlAlarmName = "iscontrolalarm";

        /// <summary>
        /// узел в котором хранится контролировать или нет блокировочное значение параметра
        /// </summary>
        private const string controlBlockingName = "iscontrolblocking";

        /// <summary>
        /// имя узла в котором хранится интервал значений параметра
        /// </summary>
        private const string rangeName = "ParameterRange";

        /// <summary>
        /// имя узла в котором хранится сохранять параметр в БД или нет
        /// </summary>
        private const string saveToDBName = "issavetodb";

        /// <summary>
        /// имя узла в котором хранится интервал записи параметра в БД
        /// </summary>
        private const string intervalToSaveName = "intervaltosave";

        /// <summary>
        /// имя узла в котором хранится пороговое значение
        /// </summary>
        private const string thresholdToBDName = "threashold";

        /// <summary>
        /// имя узла в котором хранится номер канала который ассоциирован с параметром(источник данных для параметра)
        /// </summary>
        private const string channelNumberName = "channelnumber";

        /// <summary>
        /// имя узла в котором хранится текущее значение параметра
        /// </summary>
        private const string currentValueName = "currentvalue";

        /// <summary>
        /// имя узла в котором хранится собственный индекс
        /// </summary>
        private const string selfIndexName = "selfindex";

        /// <summary>
        /// имя узла в котором хранится идентификатор параметра
        /// </summary>
        private const string guidName = "guid";

        /// <summary>
        /// имя узла в котором хранится номер параметра в списке от devMan
        /// </summary>
        private const string devManindexName = "devManindex";

        /// <summary>
        /// имя узла в котором хранится текстовое описание параметра от devMan
        /// </summary>
        private const string devManDescriptionName = "devManDescription";

        /// <summary>
        /// имя узла в котором хранится количество точек после запятой
        /// </summary>
        private const string decimalPointsName = "decimal_points";

        /// <summary>
        /// Сохранить параметр
        /// </summary>
        /// <param name="doc">Документ в который сохраняется параметр</param>
        /// <returns>Узел в котором сохранен параметр</returns>
        public XmlNode Save(XmlDocument doc)
        {
            try
            {
                XmlNode root = doc.CreateElement(parameterRoot);

                XmlNode nameNode = doc.CreateElement(parameterName);
                XmlNode descNode = doc.CreateElement(descriptionName);

                XmlNode unitsNode = doc.CreateElement(unitsName);
                XmlNode alarmNode = doc.CreateElement(alarmValueName);

                XmlNode blockingNode = doc.CreateElement(blockingValueName);
                XmlNode controlAlarmNode = doc.CreateElement(controlAlarmName);

                XmlNode controlBlockingNode = doc.CreateElement(controlBlockingName);
                XmlNode rangeNode = range.Save(doc);

                XmlNode isSaveToDBNode = doc.CreateElement(saveToDBName);
                XmlNode intervalToSaveNode = doc.CreateElement(intervalToSaveName);

                XmlNode thresholdToBDNode = doc.CreateElement(thresholdToBDName);
                //XmlNode channelNumberNode = doc.CreateElement(channelNumberName);

                XmlNode currentValueNode = doc.CreateElement(currentValueName);
                XmlNode selfIndexNode = doc.CreateElement(selfIndexName);

                XmlNode guidNode = doc.CreateElement(guidName);

                XmlNode devManindexNode = doc.CreateElement(devManindexName);
                XmlNode devManDescriptionNode = doc.CreateElement(devManDescriptionName);
                XmlNode decimalPointsNode = doc.CreateElement(decimalPointsName);
                
                XmlNode transformationNode = null;
                if (transformation != null)
                {
                    transformationNode = transformation.CreateXmlNode(doc);
                }

                nameNode.InnerText = Name;
                descNode.InnerText = Description;

                unitsNode.InnerText = Units;
                alarmNode.InnerText = Alarm.ToString();

                blockingNode.InnerText = Blocking.ToString();
                controlAlarmNode.InnerText = IsControlAlarm.ToString();

                controlBlockingNode.InnerText = IsControlBlocking.ToString();
                isSaveToDBNode.InnerText = SaveToDB.ToString();

                intervalToSaveNode.InnerText = IntervalToSaveToDB.ToString();
                thresholdToBDNode.InnerText = ThresholdToBD.ToString();

                //channelNumberNode.InnerText = channelNumber.ToString();
                currentValueNode.InnerText = CalculatedValue.ToString();

                //selfIndexNode.InnerText = selfIndex.ToString();
                guidNode.InnerText = identifier.ToString();

                devManindexNode.InnerText = devManindex.ToString();
                devManDescriptionNode.InnerText = devManDescription;

                decimalPointsNode.InnerText = decimalPoints.ToString();

                root.AppendChild(nameNode);
                root.AppendChild(descNode);

                root.AppendChild(unitsNode);
                root.AppendChild(alarmNode);

                root.AppendChild(blockingNode);
                root.AppendChild(controlAlarmNode);

                root.AppendChild(controlBlockingNode);
                if (rangeNode != null)
                {
                    root.AppendChild(rangeNode);
                }

                root.AppendChild(isSaveToDBNode);
                root.AppendChild(intervalToSaveNode);

                root.AppendChild(thresholdToBDNode);
                //root.AppendChild(channelNumberNode);

                root.AppendChild(currentValueNode);
                //root.AppendChild(selfIndexNode);

                root.AppendChild(devManindexNode);
                root.AppendChild(devManDescriptionNode);

                root.AppendChild(decimalPointsNode);

                root.AppendChild(guidNode);
                if (transformationNode != null)
                {
                    root.AppendChild(transformationNode);
                }

                return root;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Загрузить параметр
        /// </summary>
        /// <param name="node">Узел в котором сохранен параметр</param>
        public void Load(XmlNode node)
        {
            try
            {
                if (node != null && node.HasChildNodes)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case parameterRoot:

                                break;

                            case parameterName:

                                try
                                {
                                    name = child.InnerText;
                                }
                                catch { }
                                break;

                            case descriptionName:

                                try
                                {
                                    description = child.InnerText;
                                }
                                catch { }
                                break;

                            case unitsName:

                                try
                                {
                                    units = child.InnerText;
                                }
                                catch { }
                                break;

                            case alarmValueName:

                                try
                                {
                                    alarmValue = float.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case blockingValueName:

                                try
                                {
                                    blockingValue = float.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case controlAlarmName:

                                try
                                {
                                    controlAlarm = bool.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case controlBlockingName:

                                try
                                {
                                    controlBlocking = bool.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case rangeName:

                                try
                                {
                                    range.Load(child);
                                }
                                catch { }
                                break;

                            case saveToDBName:

                                try
                                {
                                    saveToDB = bool.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case intervalToSaveName:

                                try
                                {
                                    intervalToSave = int.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case thresholdToBDName:

                                try
                                {
                                    thresholdToBD = int.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case channelNumberName:

                                try
                                {
                                    //channelNumber = int.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case currentValueName:

                                try
                                {
                                    currentValue = float.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case selfIndexName:

                                try
                                {
                                    int preSelf = int.Parse(child.InnerText);
                                    if (preSelf > -1)
                                    {
                                        //selfIndex = preSelf;
                                    }
                                }
                                catch { }
                                break;

                            case guidName:

                                try
                                {
                                    identifier = new Guid(child.InnerText);
                                }
                                catch { }
                                break;

                            case devManindexName:

                                try
                                {
                                    devManindex = int.Parse(child.InnerText);
                                }
                                catch { }
                                break;

                            case devManDescriptionName:

                                try
                                {
                                    devManDescription = child.InnerText;
                                }
                                catch { }
                                break;

                            case decimalPointsName:

                                try
                                {
                                    decimalPoints = int.Parse(child.InnerText);
                                    format = "{0:F" + decimalPoints.ToString() + "}";
                                }
                                catch { }
                                break;

                            case "macros":

                                try
                                {
                                    transformation.InstanceMacrosFromXmlNode(child);
                                }
                                catch { }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch { }
        }

    }
}