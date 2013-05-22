using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    public partial class mainForm : Form
    {
        private AsyApplication _app = null;         // основное приложение

        public mainForm()
        {
            InitializeComponent();
            
            _app = AsyApplication.CreateInstance();
            _app.OnTec += new EventHandler(_app_OnTec);

            dStatuser = new devMnStatuser(DevStatuse);

            DevManClient.onConnected += new EventHandler(DevManClient_onConnected);
            DevManClient.onDisconnected += new EventHandler(DevManClient_onDisconnected);

            setter = new setterText(setterTextF);
        }

        private delegate void devMnStatuser(string status);

        private devMnStatuser dStatuser;
        private void DevStatuse(string status)
        {
            toolStripStatusLabelDevManStatus.Text = status;
        }

        /// <summary>
        /// установлено соединение с сервером данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevManClient_onConnected(object sender, EventArgs e)
        {
            try
            {
                Invoke(dStatuser, "Подключен с серверу данных");
            }
            catch { }
        }

        /// <summary>
        /// разорвано соединенеи с сервером данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevManClient_onDisconnected(object sender, EventArgs e)
        {
            try
            {
                Invoke(dStatuser, "Не подключен с серверу данных");
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void настройкаСоединенияССерверомДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devManConnectorForm frm = new devManConnectorForm();
            frm.ShowDialog(this);

            _app.Save();
        }

        /// <summary>
        /// настраиваем параметры devMan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void настройкаПараметровDevManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParametersTunerForm frm = new ParametersTunerForm();
            frm.ShowDialog(this);
        }

        /// <summary>
        /// настраиваем параметры от асу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void настройкаПараметровОтАСУToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AsyParametersForm frm = new AsyParametersForm();
            frm.ShowDialog(this);
        }

        /// <summary>
        /// настраиваем с оединение с devMan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void настройкаСоединенияСDevManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devManConnectorForm frm = new devManConnectorForm();
            frm.ShowDialog(this);

            _app.Save();
        }

        /// <summary>
        /// настраиваем Tcp сервер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void настройкаTcpСервераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TcpOptions options = new TcpOptions();

            options.Port = _app.Server.Port;
            options.CountClients = _app.Server.CountConnections;

            options.ClientBufferSize = _app.Server.ReceiverBufferSize;

            if (options.ShowDialog(this) == DialogResult.OK)
            {
                _app.Server.Port = options.Port;
                _app.Server.CountConnections = options.CountClients;

                _app.Server.ReceiverBufferSize = options.ClientBufferSize;
            }
        }

        /// <summary>
        /// выводим данные пакета из АСУ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _app_OnTec(object sender, EventArgs e)
        {
            _labelNagruzka.BackColor = Color.Salmon;
            _labelRashod.BackColor = Color.Salmon;
            _labelTalblok.BackColor = Color.Salmon;
            _labelPodasa.BackColor = Color.Salmon;
            _labeObem.BackColor = Color.Salmon;

            switch (_app.CodeButton)
            {
                case CodeButtonAsy.Load:

                    _labelNagruzka.BackColor = Color.LimeGreen;
                    break;

                case CodeButtonAsy.Consumption:

                    _labelRashod.BackColor = Color.LimeGreen;
                    break;

                case CodeButtonAsy.Talblok:

                    _labelTalblok.BackColor = Color.LimeGreen;
                    break;

                case CodeButtonAsy.Flow:

                    _labelPodasa.BackColor = Color.LimeGreen;
                    break;

                case CodeButtonAsy.Volume:

                    _labeObem.BackColor = Color.LimeGreen;
                    break;

                default:
                    break;
            }

            Invoke(setter, _textBoxSkorostDvig1, _app.Speed_1);
            Invoke(setter, _textBoxSkorostDvig2, _app.Speed_2);

            Invoke(setter, _textBoxSkorostRotor, _app.Speed_rotor);
            Invoke(setter, _textBoxMomentDvigRotor, _app.Torque_rotor);

            Invoke(setter, _textBoxKlinia, _app.Wedges_state);

            Invoke(setter, _textBoxDiameterPorsna1, _app.Diameter_1);
            Invoke(setter, _textBoxDiameterPorsna2, _app.Diameter_2);

            Invoke(setter, textBox1, _app.Force);
        }

        setterText setter = null;
        delegate void setterText(TextBox box, float val);
        private void setterTextF(TextBox box, float val)
        {
            box.Text = string.Format("{0:F2}", val);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}