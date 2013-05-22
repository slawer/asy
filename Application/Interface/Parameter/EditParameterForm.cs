using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WCF;

namespace ASY
{
    public partial class EditParameterForm : Form
    {
        private Parameter edited = null;                // редактируемый параметр
        private PDescription channel = null;            // канал

        public EditParameterForm(Parameter parameter)
        {
            InitializeComponent();

            if (parameter != null)
            {
                edited = parameter;
                channel = edited.Channel;
            }
            else
            {
                MessageBox.Show("jnjnjnj");
                this.Close();
            }
        }

        private void EditParameterForm_Load(object sender, EventArgs e)
        {
            if (edited != null)
            {
                textBoxParameterName.Text = edited.Name;
                if (edited.Channel != null)
                {
                    textBoxParameterChannelName.Text = edited.Channel.Description;
                }
            }
        }

        /// <summary>
        /// Выбрать канал для параметра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectChannel_Click(object sender, EventArgs e)
        {
            DevManParametersForm frm = new DevManParametersForm(false);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                channel = frm.SelectedParameter;
                textBoxParameterChannelName.Text = channel.Description;
            }
        }

        /// <summary>
        /// Сохранить результат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accept_Click(object sender, EventArgs e)
        {
            edited.Channel = channel;
            edited.Name = textBoxParameterName.Text;
        }

        private void selectChannel_Click_1(object sender, EventArgs e)
        {
            DevManParametersForm frm = new DevManParametersForm(false);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                channel = frm.SelectedParameter;
                textBoxParameterChannelName.Text = channel.Description;
            }
        }
    }
}