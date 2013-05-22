using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;

namespace ASY
{
    public partial class ShowTimeForm : Form
    {
        private FinderDelegate finder = null;
        private List<DirectoryEntry> computers = null;

        private bool closed = false;
        private IAsyncResult iResult = null;

        public ShowTimeForm()
        {
            InitializeComponent();

            finder = new FinderDelegate(FinderFunction);
            iResult = finder.BeginInvoke(new AsyncCallback(CallBack), null);

            if (iResult.AsyncWaitHandle.WaitOne(1500))
            {
                closed = true;
            }
        }

        /// <summary>
        /// Найденные компьютеры в сети
        /// </summary>
        public List<DirectoryEntry> Computers
        {
            get { return computers; }
        }

        /// <summary>
        /// найти все компьютеры в сети
        /// </summary>
        /// <returns></returns>
        private List<DirectoryEntry> FindComputers()
        {
            try
            {
                List<DirectoryEntry> entries = new List<DirectoryEntry>();

                DirectoryEntry parent = new DirectoryEntry("WinNT:");
                foreach (DirectoryEntry dm in parent.Children)
                {
                    DirectoryEntry coParent = new DirectoryEntry("WinNT://" + dm.Name);
                    DirectoryEntries dent = coParent.Children;
                    dent.SchemaFilter.Add("Computer");
                    foreach (DirectoryEntry client in dent)
                    {
                        entries.Add(client);
                    }
                }

                return entries;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        protected delegate void FinderDelegate();
        protected void FinderFunction()
        {
            computers = FindComputers();
        }

        protected void CallBack(IAsyncResult iResult)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void ShowTimeForm_Load(object sender, EventArgs e)
        {
            if (closed)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
                Opacity = 1.0f;
        }
    }
}