using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    public partial class devManConnectorForm : Form
    {
        // net.tcp://localhost:57000

        public devManConnectorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ищем компьютеры в сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFindComputers_Click(object sender, EventArgs e)
        {
            ShowTimeForm frm = new ShowTimeForm();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                List<DirectoryEntry> comps = frm.Computers;
                if (comps != null)
                {
                    listView1.Items.Clear();
                    foreach (DirectoryEntry comp in comps)
                    {
                        InsertToListView(comp);
                    }                    
                }
            }
        }

        /// <summary>
        /// Добавить комптюткр в листвью
        /// </summary>
        /// <param name="entry"></param>
        protected void InsertToListView(DirectoryEntry entry)
        {
            try
            {
                string myIP = System.Net.Dns.GetHostByName(entry.Name).AddressList[0].ToString();

                ListViewItem item = new ListViewItem((listView1.Items.Count + 1).ToString());

                ListViewItem.ListViewSubItem name = new ListViewItem.ListViewSubItem(item, entry.Name);
                ListViewItem.ListViewSubItem ip = new ListViewItem.ListViewSubItem(item, myIP);

                item.SubItems.Add(name);
                item.SubItems.Add(ip);

                item.Tag = entry;

                listView1.Items.Add(item);
            }
            catch { }
        }



        /// <summary>
        /// выбрали элемент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null && listView1.SelectedItems.Count > 0)
            {
                DirectoryEntry entry = listView1.SelectedItems[0].Tag as DirectoryEntry;
                if (entry != null)
                {
                    textBoxTotalAddress.Text = string.Format("net.tcp://{0}:57000", entry.Name);
                }
            }
        }

        /// <summary>
        /// пробуем отсоединиться и соединиться
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accept_Click(object sender, EventArgs e)
        {
            Uri last = DevManClient.Uri;
            try
            {
                if (textBoxTotalAddress.Text != string.Empty)
                {
                    
                    if (last.OriginalString != textBoxTotalAddress.Text)
                    {
                        Uri newUri = null;
                        try
                        {
                            newUri = new Uri(textBoxTotalAddress.Text);
                            AsyApplication _app = AsyApplication.CreateInstance();
                            if (_app != null)
                            {
                                _app.Commutator.DevManUri = newUri;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void devManConnectorForm_Load(object sender, EventArgs e)
        {
            textBoxTotalAddress.Text = DevManClient.Uri.OriginalString;
        }
    }
}