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
    public partial class ParametersTunerForm : Form
    {
        private AsyApplication _app = null;

        /// <summary>
        /// инициализирует новый экземпляр класса
        /// </summary>
        public ParametersTunerForm()
        {
            InitializeComponent();

            _app = AsyApplication.CreateInstance();
            if (_app == null)
            {
                MessageBox.Show("Не удалось получить доступ к параметрам приложения");
            }
        }

        /// <summary>
        /// загружаемся
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParametersTunerForm_Load(object sender, EventArgs e)
        {
            Parameter[] parameters = _app.Commutator.Parameters;
            if (parameters != null)
            {
                foreach (Parameter parameter in parameters)
                {
                    InsertParameterToList(parameter);
                }
            }
        }

        /// <summary>
        /// добавить параметр в список
        /// </summary>
        /// <param name="parameter">Добавляемый параметр</param>
        private void InsertParameterToList(Parameter parameter)
        {
            int number = listViewParameters.Items.Count + 1;

            ListViewItem item = new ListViewItem(number.ToString());
            ListViewItem.ListViewSubItem des = new ListViewItem.ListViewSubItem(item, parameter.Name);

            item.Tag = parameter;
            item.SubItems.Add(des);

            listViewParameters.Items.Add(item);
        }

        /// <summary>
        /// редактируем параметр
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (listViewParameters.SelectedItems != null)
            {
                if (listViewParameters.SelectedItems.Count > 0)
                {
                    Parameter selected = listViewParameters.SelectedItems[0].Tag as Parameter;
                    if (selected != null)
                    {
                        EditParameterForm frm = new EditParameterForm(selected);
                        if (frm.ShowDialog(this) == DialogResult.OK)
                        {
                            // ------- тут необходимо применить в силу внесенные изменения -------

                            if (listViewParameters.SelectedItems != null)
                            {
                                if (listViewParameters.SelectedItems.Count > 0)
                                {
                                    ListViewItem selItem = listViewParameters.SelectedItems[0];
                                    if (selItem != null)
                                    {
                                        Parameter selPar = selItem.Tag as Parameter;
                                        if (selPar != null)
                                        {
                                            selItem.SubItems[1].Text = selPar.Name;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}