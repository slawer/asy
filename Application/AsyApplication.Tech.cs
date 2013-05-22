using System;
using System.Collections.Generic;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    /// <summary>
    /// Реализует основное приложение
    /// </summary>
    public partial class AsyApplication
    {
        /// <summary>
        /// Возникает когда обработан пакет от АСУ
        /// </summary>
        public event EventHandler OnTec;

        protected CodeButtonAsy code_button = CodeButtonAsy.Default;

        /// <summary>
        /// Получен пакет от клиентс
        /// </summary>
        /// <param name="packet">Полученный пакет</param>
        public void server_OnPacket(string packet)
        {
            try
            {
                AsyPacket _packet = AsyPacket.Parse(packet);
                if (_packet != null)
                {
                    if (_packet.Data.Length == 7)
                    {
                        Parameter.SetCurrentValue(commutator.Speed_1, _packet.Data[0]);
                        Parameter.SetCurrentValue(commutator.Speed_2, _packet.Data[1]);

                        Parameter.SetCurrentValue(commutator.Speed_rotor, _packet.Data[2]);
                        Parameter.SetCurrentValue(commutator.Torque_rotor, _packet.Data[3]);

                        Parameter.SetCurrentValue(commutator.Wedges_state, _packet.Data[4]);

                        Parameter.SetCurrentValue(commutator.Diameter_1, _packet.Data[5]);
                        Parameter.SetCurrentValue(commutator.Diameter_2, _packet.Data[6]);
                    }
                    else
                        if (_packet.Data.Length >= 8)
                        {
                            Parameter.SetCurrentValue(commutator.Speed_1, _packet.Data[0]);
                            Parameter.SetCurrentValue(commutator.Speed_2, _packet.Data[1]);

                            Parameter.SetCurrentValue(commutator.Speed_rotor, _packet.Data[2]);
                            Parameter.SetCurrentValue(commutator.Torque_rotor, _packet.Data[3]);

                            Parameter.SetCurrentValue(commutator.Wedges_state, _packet.Data[4]);

                            Parameter.SetCurrentValue(commutator.Diameter_1, _packet.Data[5]);
                            Parameter.SetCurrentValue(commutator.Diameter_2, _packet.Data[6]);

                            Parameter.SetCurrentValue(commutator.Force, _packet.Data[7]);
                        }

                    code_button = CodeButtonAsy.Default;
                    switch (_packet.CodeButton)
                    {
                        case 0x00:

                            code_button = CodeButtonAsy.Default;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x00);

                            break;

                        case 0x1:

                            code_button = CodeButtonAsy.Load;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x1);
                            break;

                        case 0x2:

                            code_button = CodeButtonAsy.Consumption;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x02);
                            break;

                        case 0x4:

                            code_button = CodeButtonAsy.Talblok;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x04);
                            break;

                        case 0x8:

                            code_button = CodeButtonAsy.Flow;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x08);
                            break;

                        case 0x10:

                            code_button = CodeButtonAsy.Volume;
                            Parameter.SetCurrentValue(_app.Commutator.CodeButton, 0x10);
                            break;

                        default:
                            break;
                    }

                    AnswerAsy(_packet);
                    UpdateDevManData();

                    if (OnTec != null)
                    {
                        OnTec(this, EventArgs.Empty);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Передать данные АСУ
        /// </summary>
        /// <param name="_packet">Пакет запроса от АСУ</param>
        protected void AnswerAsy(AsyPacket _packet)
        {
            try
            {                
                Parameter[] parameters = commutator.Parameters;
                if (parameters != null)
                {
                    List<float> data = new List<float>();
                    foreach (Parameter parameter in parameters)
                    {
                        if (parameter != null)
                        {
                            PDescription channel = parameter.Channel;
                            if (channel != null)
                            {
                                if (channel.Number > -1)
                                {
                                    data.Add(parameter.CurrentValue);
                                }
                            }
                        }
                    }

                    if (data.Count >= 16)
                    {
                        AsyPacket answer_packet = _packet.TranslateToAnswer(data.ToArray());
                        if (answer_packet != null)
                        {
                            string spac = answer_packet.ToString();
                            server.Send(spac);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Передать данные серверу данных данные АСУ
        /// </summary>
        protected void UpdateDevManData()
        {
            try
            {
                Parameter[] updated = { commutator.Speed_1, commutator.Speed_2, commutator.Speed_rotor,
                                      commutator.Torque_rotor, commutator.Wedges_state, commutator.Force,
                                      commutator.Diameter_1, commutator.Diameter_2, commutator.CodeButton };

                if (updated != null)
                {
                    foreach (Parameter p_updated in updated)
                    {
                        if (p_updated != null)
                        {
                            PDescription channel = p_updated.Channel;
                            if (channel != null)
                            {
                                DevManClient.UpdateParameter(channel.Number, p_updated.CurrentValue);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Скорость двигателя насоса 1
        /// </summary>
        public float Speed_1
        {
            get
            {
                return commutator.Speed_1.CurrentValue;
            }
        }

        /// <summary>
        /// Скорость двигателя насоса 2
        /// </summary>
        public float Speed_2
        {
            get
            {
                return commutator.Speed_2.CurrentValue;
            }
        }

        /// <summary>
        /// Скорость двигателя ротора
        /// </summary>
        public float Speed_rotor
        {
            get
            {
                return commutator.Speed_rotor.CurrentValue;
            }
        }

        /// <summary>
        /// Крутящий момент ротора
        /// </summary>
        public float Torque_rotor
        {
            get
            {
                return commutator.Torque_rotor.CurrentValue;
            }
        }

        /// <summary>
        /// Клинья подняты/опущены
        /// </summary>
        public float Wedges_state
        {
            get
            {
                return commutator.Wedges_state.CurrentValue;
            }
        }

        /// <summary>
        /// Диаметер поршня 1
        /// </summary>
        public float Diameter_1
        {
            get
            {
                return commutator.Diameter_1.CurrentValue;
            }
        }

        /// <summary>
        /// Диаметер поршня 2
        /// </summary>
        public float Diameter_2
        {
            get
            {
                return commutator.Diameter_2.CurrentValue;
            }
        }

        /// <summary>
        /// Усилие в гидрораскрепителе
        /// </summary>
        public float Force
        {
            get
            {
                return commutator.Force.CurrentValue;
            }
        }

        /// <summary>
        /// Код кнопки
        /// </summary>
        public CodeButtonAsy CodeButton
        {
            get
            {
                return code_button;
            }
        }
    }

    /// <summary>
    /// Код кнопки АСУ буровой
    /// </summary>
    public enum CodeButtonAsy
    {
        /// <summary>
        /// Нагрузка
        /// </summary>
        Load,

        /// <summary>
        /// Расход
        /// </summary>
        Consumption,

        /// <summary>
        /// Тальблок
        /// </summary>
        Talblok,

        /// <summary>
        /// Подача
        /// </summary>
        Flow,

        /// <summary>
        /// Объем
        /// </summary>
        Volume,

        /// <summary>
        /// По умолчанию. Не нажата кнопка.
        /// </summary>
        Default
    }
}