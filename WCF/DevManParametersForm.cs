using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ASY;
using WCF.WCF_Client;

namespace WCF
{
    public partial class DevManParametersForm : Form
    {
        private bool need = false;
        PDescription[] parameters;

        AsyApplication _app = null;

        public DevManParametersForm(bool sahvat)
        {
            need = sahvat;
            InitializeComponent();

            _app = AsyApplication.CreateInstance();

            try
            {
                parameters = DevManClient.Parameters;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Выбранный параметр
        /// </summary>
        public PDescription SelectedParameter
        {
            get
            {
                if (listView1.SelectedItems != null)
                {
                    if (listView1.SelectedItems.Count > 0)
                    {
                        PDescription selected = listView1.SelectedItems[0].Tag as PDescription;
                        return selected;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Загружаемся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevManParametersForm_Load(object sender, EventArgs e)
        {
            try
            {
                //PDescription[] parameters = DevManClient.Parameters;
                if (parameters != null)
                {
                    int index = 1;
                    foreach (PDescription parameter in parameters)
                    {
                        if (need)
                        {
                            if (parameter.Type == DeviceManager.FormulaType.Capture)
                            {
                                ListViewItem item = new ListViewItem(index.ToString());
                                ListViewItem.ListViewSubItem desc = new ListViewItem.ListViewSubItem(item, parameter.Description);

                                item.Tag = parameter;
                                item.SubItems.Add(desc);

                                listView1.Items.Add(item);
                                index = index + 1;
                            }
                        }
                        else
                        {
                            ListViewItem item = new ListViewItem(index.ToString());
                            ListViewItem.ListViewSubItem desc = new ListViewItem.ListViewSubItem(item, parameter.Description);

                            item.Tag = parameter;
                            item.SubItems.Add(desc);

                            listView1.Items.Add(item);
                            index = index + 1;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "Не удалось получить список параметров. Повторите попытку.", "Сообщение",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}