using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASY
{
    public partial class DevManDataForm : Form
    {
        private Mutex mutex = null;
        private AsyApplication _app = null;

        public DevManDataForm()
        {
            InitializeComponent();

            mutex = new Mutex();
            _app = AsyApplication.CreateInstance();
        }

        /// <summary>
        /// загружаемся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevManDataForm_Load(object sender, EventArgs e)
        {
            Parameter[] parameters = _app.Commutator.Parameters;
            if (parameters != null)
            {
                int rowIndex = 0;
                foreach (Parameter parameter in parameters)
                {
                    dataGridView10.Rows.Add();

                    DataGridViewCell channelNumber = dataGridView10.Rows[rowIndex].Cells[0];
                    DataGridViewCell channelDesc = dataGridView10.Rows[rowIndex].Cells[1];

                    DataGridViewCell fIteration = dataGridView10.Rows[rowIndex].Cells[2];

                    channelNumber.Value = (rowIndex + 1);
                    channelDesc.Value = parameter.Name;

                    dataGridView10.Rows[rowIndex].Tag = parameter;
                    rowIndex = rowIndex + 1;
                }

                _app.Commutator.onUpdated += new CommutatorEventHandler(Serial_OnStaticComplete);
            }
        }

        /// <summary>
        /// закрывается форма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevManDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _app.Commutator.onUpdated -= Serial_OnStaticComplete;
        }

        /// <summary>
        /// Завершен цикл опроса устройств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Serial_OnStaticComplete(Object sender, CommutatorEventArgs e)
        {
            bool blocked = false;
            try
            {
                if (mutex.WaitOne(1000, false))
                {
                    blocked = true;
                    foreach (DataGridViewRow row in dataGridView10.Rows)
                    {
                        if (row.Index < dataGridView10.Rows.Count - 1)
                        {
                            Parameter parameter = row.Tag as Parameter;
                            if (parameter != null)
                            {
                                row.Cells[2].Value = parameter.FormattedCurrentValue;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (blocked) mutex.ReleaseMutex();
            }
        }
    }
}