using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASY
{
    public partial class SelectParameterForm : Form
    {
        AsyApplication _app = null;

        public SelectParameterForm()
        {
            InitializeComponent();
            _app = AsyApplication.CreateInstance();
        }

        /// <summary>
        /// Загружаемся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectParameterForm_Load(object sender, EventArgs e)
        {
            int number = 1;
            foreach (Parameter parameter in _app.Commutator.Parameters)
            {
                ListViewItem item = new ListViewItem(number.ToString());
                ListViewItem.ListViewSubItem name = new ListViewItem.ListViewSubItem(item, parameter.Name);

                number = number + 1;
                item.SubItems.Add(name);

                item.Tag = parameter;
                listView1.Items.Add(item);
            }
        }

        /// <summary>
        /// Возвращяет выбранный параметр
        /// </summary>
        public Parameter SelectedParameter
        {
            get
            {
                if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
                {
                    Parameter selected = listView1.SelectedItems[0].Tag as Parameter;
                    return selected;
                }

                return null;
            }
        }
    }
}