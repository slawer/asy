using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ASY
{
    /// <summary>
    /// Реализует пакет обмена данными с АСУ Буровой "Уралмаш"
    /// </summary>
    public class AsyPacket
    {
        protected byte _codePacket;             // код пакета
        protected byte _codeButton;             // код кнопки

        protected List<float> _data;            // данные пакета

        /// <summary>
        /// инициализирует новый экземпляр класса
        /// </summary>
        public AsyPacket()
        {
            _data = new List<float>();

            _codeButton = 0;
            _codePacket = 0;
        }

        /// <summary>
        /// Возвращяет код пакета
        /// </summary>
        public byte CodePacket
        {
            get { return _codePacket; }
        }

        /// <summary>
        /// Возвращяет код кнопки
        /// </summary>
        public byte CodeButton
        {
            get { return _codeButton; }
        }

        /// <summary>
        /// Возвращяет данные пакета
        /// </summary>
        public float[] Data
        {
            get { return _data.ToArray(); }
        }

        /// <summary>
        /// Преобразует строковое представление пакета в его эквивалент типа Packet.
        /// </summary>
        /// <param name="text">Строка, содержащая преобразуемый пакет.</param>
        /// <returns>Экземпляр класса типа Packet, эквивалентый пакету, содержащемуся в строке text.</returns>
        public static AsyPacket Parse(string text)
        {
            try
            {
                Regex regex = new Regex(":[0-9a-fA-F]*[\0]");
                Match match = regex.Match(text);

                if (match.Success)
                {
                    // ---- есть пакет ----

                    switch (GetCodePacket(match.Value))
                    {
                        case AsyQuestionCodePacket:

                            return ParseQuestionFromAsy(match.Value);

                        case AsyAnswerCodePacket:

                            return ParseAnswerToAsy(match.Value);

                        default:

                            break;
                    }

                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Создать пакет-ответ на запров от АСУ Буровой "Уралмаш"
        /// </summary>
        /// <returns></returns>
        public AsyPacket TranslateToAnswer(Single[] data)
        {
            AsyPacket p = new AsyPacket();

            p._codeButton = _codeButton;
            p._codePacket = AsyAnswerCodePacket;
            
            foreach (var item in data)
            {
                p._data.Add(item);                
            }

            return p;
        }

        /// <summary>
        /// Получить текстовое предстваление пакета
        /// </summary>
        /// <returns>Текстовое представление пакета</returns>
        public override string ToString()
        {
            int crc = _codeButton + AsyAnswerCodePacket;

            byte _crc = 0;
            List<byte> p_array = new List<byte>();

            p_array.Add((byte)((_codePacket << 4) | (_codePacket >> 4)));
            p_array.Add((byte)((_codeButton << 4) | (_codeButton >> 4)));

            for (int i = 0; i < 50; i++)
            {
                byte[] bytes = BitConverter.GetBytes(_data[i]);
                foreach (var item in bytes)
                {
                    p_array.Add((byte)((item << 4) | (item >> 4)));
                }
                crc += bytes[0] + bytes[1] + bytes[2] + bytes[3];
            }

            _crc = (byte)crc;
            p_array.Add((byte)((_crc << 4) | (_crc >> 4)));

            byte[] p_data = p_array.ToArray();
            string psring = ":";

            foreach (var item in p_data)
            {
                psring += string.Format("{0:X2}", item);
            }
            psring += "\0";
            return psring;
        }

        /// <summary>
        /// Преобразует строковое представление пакета в его эквивалент типа Packet.
        /// Строка пакета которую отправляет АСУ, запрос на передачу данных.
        /// </summary>
        /// <param name="text">Строка, содержащая преобразуемый пакет.</param>
        /// <returns>Экземпляр класса типа Packet, эквивалентый пакету, содержащемуся в строке text.</returns>
        private static AsyPacket ParseQuestionFromAsy(string text)
        {
            byte _codePacket = GetCodePacket(text);
            byte _codeButton = GetCodeButton(text);

            float[] _values = new float[8];
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = GetParameterFromPacket(text, i);
            }

            AsyPacket packet = new AsyPacket();

            packet._codePacket = _codePacket;
            packet._codeButton = _codeButton;

            foreach (var value in _values)
            {
                packet._data.Add(value);
            }

            return packet;
        }

        /// <summary>
        /// Преобразует строковое представление пакета в его эквивалент типа Packet.
        /// Строка пакета которая отправляет в АСУ, ответ на запрос данных от АСУ.
        /// </summary>
        /// <param name="text">Строка, содержащая преобразуемый пакет.</param>
        /// <returns>Экземпляр класса типа Packet, эквивалентый пакету, содержащемуся в строке text.</returns>
        private static AsyPacket ParseAnswerToAsy(string text)
        {
            byte _codePacket = GetCodePacket(text);
            byte _codeButton = GetCodeButton(text);

            float[] _values = new float[50];
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = GetParameterFromPacket(text, i);
            }

            AsyPacket packet = new AsyPacket();

            packet._codePacket = _codePacket;
            packet._codeButton = _codeButton;

            foreach (var value in _values)
            {
                packet._data.Add(value);
            }

            return packet;
        }

        /// <summary>
        /// Извлечь код пакета
        /// </summary>
        /// <param name="text">Строка пакета</param>
        /// <returns>Значение поля код пакета</returns>
        private static byte GetCodePacket(string text)
        {
            string v_codePacket = text.Substring(1, 2);
            byte b_codePacket = byte.Parse(v_codePacket, NumberStyles.AllowHexSpecifier);

            return (byte)((b_codePacket << 4) | (b_codePacket >> 4));
        }

        /// <summary>
        /// Извлечь код кнопки
        /// </summary>
        /// <param name="text">Строка пакета</param>
        /// <returns>Значение поля код пакета</returns>
        private static byte GetCodeButton(string text)
        {
            string v_codeButton = text.Substring(3, 2);
            byte b_codeButton = byte.Parse(v_codeButton, NumberStyles.AllowHexSpecifier);

            return (byte)((b_codeButton << 4) | (b_codeButton >> 4));
        }

        /// <summary>
        /// Извлечь значение параметра из строки пакета
        /// </summary>
        /// <param name="text">Строка пакета из которой извлечь значение параметра</param>
        /// <param name="index">Индекс параметра</param>
        /// <returns>Значение параметра</returns>
        private static float GetParameterFromPacket(string text, int index)
        {
            try
            {
                int position = 5 + index * 8;

                string p_value = text.Substring(position, 8);
                byte[] p_array = new byte[4];

                for (int i = 0; i < p_array.Length; i++)
                {
                    p_array[i] = byte.Parse(p_value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
                    p_array[i] = (byte)((p_array[i] << 4) | (p_array[i] >> 4));
                }

                return BitConverter.ToSingle(p_array, 0);
            }
            catch { }
            return float.NaN;
        }

        /// <summary>
        /// Пакет запроса данных от АСУ Буровой "Уралмаш"
        /// </summary>
        public const byte AsyQuestionCodePacket = 0x43;

        /// <summary>
        /// Пакет ответа на запрос данных от АСУ Буровой "Уралмаш"
        /// </summary>
        public const byte AsyAnswerCodePacket = 0x44;
    }
}