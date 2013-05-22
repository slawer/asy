using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ASY
{
    /// <summary>
    /// Точка входа в программу
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Идентификационный номер системного мьютекса, по которому определяется запещено или нет приложение
        /// </summary>
        private const string identifier = "c04c385b-e024-4687-804a-466858ab30ba";

        private static Mutex mutex = null;              // определяет запуск приложения
        private static bool isNotRunning = false;       // содержит значение на основе которого определяет возможность запуска программы        

        private static AsyApplication app = null;       // основное приложение

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                mutex = new Mutex(true, identifier, out isNotRunning);
                if (isNotRunning)
                {
                    app = AsyApplication.CreateInstance();
                    if (app != null)
                    {
                        app.Load();                        
                        app.Connect();

                        System.Windows.Forms.Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

                        System.Windows.Forms.Application.EnableVisualStyles();
                        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                        System.Windows.Forms.Application.Run(new mainForm());
                    }
                }
                else
                    MessageBox.Show("Приложение уже запущено", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
        }

        /// <summary>
        /// приложение завершает свою работу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            app.Save();
        }
    }
}