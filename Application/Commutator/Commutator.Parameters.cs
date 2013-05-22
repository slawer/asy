using System;
using System.Threading;
using System.Net.NetworkInformation;

using Tcp;
using Buffering;

using WCF;
using WCF.WCF_Client;

namespace ASY
{
    /// <summary>
    /// Реализует коммутатор, который отвечает за 
    /// соединение, прием и передачу данных от devMan
    /// </summary>
    public partial class Commutator
    {
        protected static Parameter[] parameters;            // параметры от devMan
        protected static ReaderWriterLockSlim p_slim;       // синхронизатор параметров

        protected static Parameter speed_1;                 // скорость двигателя насоса 1
        protected static Parameter speed_2;                 // скорость двигателя насоса 2

        protected static Parameter speed_rotor;             // скорость двигателя ротора
        protected static Parameter torque_rotor;            // крутящий момент ротора

        protected static Parameter wedges_state;            // клинья подняты/опущены
        protected static Parameter force;                   // усилие в гидрораскрепителе
        
        protected static Parameter diameter_1;              // диаметер поршня 1
        protected static Parameter diameter_2;              // диаметер поршня 2

        protected static Parameter code_button;             // Код нажатой кнопки


        /// <summary>
        /// Скорость двигателя насоса 1
        /// </summary>
        public Parameter Speed_1
        {
            get
            {
                return speed_1;
            }
        }

        /// <summary>
        /// Скорость двигателя насоса 2
        /// </summary>
        public Parameter Speed_2
        {
            get
            {
                return speed_2;
            }
        }

        /// <summary>
        /// Скорость двигателя ротора
        /// </summary>
        public Parameter Speed_rotor
        {
            get
            {
                return speed_rotor;
            }
        }

        /// <summary>
        /// Крутящий момент ротора
        /// </summary>
        public Parameter Torque_rotor
        {
            get
            {
                return torque_rotor;
            }
        }

        /// <summary>
        /// Клинья подняты/опущены
        /// </summary>
        public Parameter Wedges_state
        {
            get
            {
                return wedges_state;
            }
        }

        /// <summary>
        /// Диаметер поршня 1
        /// </summary>
        public Parameter Diameter_1
        {
            get
            {
                return diameter_1;
            }
        }

        /// <summary>
        /// Диаметер поршня 2
        /// </summary>
        public Parameter Diameter_2
        {
            get
            {
                return diameter_2;
            }
        }

        /// <summary>
        /// Усилие в гидрораскрепителе
        /// </summary>
        public Parameter Force
        {
            get
            {
                return force;
            }
        }

        /// <summary>
        /// Код нажатой кнопки
        /// </summary>
        public Parameter CodeButton
        {
            get
            {
                return code_button;
            }
        }
    }
}