using System.Windows.Forms;

namespace ASY
{
    public partial class TcpOptions : Form
    {
        public TcpOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Номер порта
        /// </summary>
        public int Port
        {
            get { return int.Parse(textBoxPort.Text); }
            set { textBoxPort.Text = value.ToString(); }
        }

        /// <summary>
        /// Максимальное количество обслуживаемых клиентов
        /// </summary>
        public int CountClients
        {
            get { return (int)numericUpDownClientsCount.Value; }
            set { numericUpDownClientsCount.Value = value; }
        }

        /// <summary>
        /// Размер буфера обмена выделяемый клиенту
        /// </summary>
        public int ClientBufferSize
        {
            get { return (int)numericUpDownBufferSize.Value; }
            set { numericUpDownBufferSize.Value = value; }
        }
    }
}