using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    public partial class AsyParametersForm : Form
    {
        AsyApplication _app = null;

        public AsyParametersForm()
        {
            InitializeComponent();
            _app = AsyApplication.CreateInstance();
        }

        /// <summary>
        /// загружаемся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsyParametersForm_Load(object sender, EventArgs e)
        {
            InsertParameterToList(_app.Commutator.Speed_1);
            InsertParameterToList(_app.Commutator.Speed_2);

            InsertParameterToList(_app.Commutator.Speed_rotor);
            InsertParameterToList(_app.Commutator.Torque_rotor);

            InsertParameterToList(_app.Commutator.Wedges_state);

            InsertParameterToList(_app.Commutator.Diameter_1);
            InsertParameterToList(_app.Commutator.Diameter_2);

            InsertParameterToList(_app.Commutator.Force);
            InsertParameterToList(_app.Commutator.CodeButton);
        }

        /// <summary>
        /// добавить параметр в список
        /// </summary>
        /// <param name="parameter">Добавляемый параметр</param>
        private void InsertParameterToList(Parameter parameter)
        {
            int number = listView1.Items.Count + 1;

            ListViewItem item = new ListViewItem(number.ToString());
            ListViewItem.ListViewSubItem des = new ListViewItem.ListViewSubItem(item, parameter.Name);

            ListViewItem.ListViewSubItem dev = new ListViewItem.ListViewSubItem(item, string.Empty);

            if (parameter.Channel != null)
            {
                dev.Text = parameter.Channel.Description;
            }

            item.Tag = parameter;

            item.SubItems.Add(des);
            item.SubItems.Add(dev);

            listView1.Items.Add(item);
        }

        /// <summary>
        /// настраиваем параметр
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    Parameter selected = listView1.SelectedItems[0].Tag as Parameter;
                    if (selected != null)
                    {
                        DevManParametersForm frm = new DevManParametersForm(true);
                        if (frm.ShowDialog(this) == DialogResult.OK)
                        {
                            if (frm.SelectedParameter != null)
                            {
                                selected.Channel = frm.SelectedParameter;
                                listView1.SelectedItems[0].SubItems[2].Text = selected.Channel.Description;
                            }
                        }
                    }
                }
            }
        }
    }
}