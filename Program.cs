using System;
using System.Windows.Forms;

namespace LSC_Trainer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool development = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (development)
            {
                Application.Run(new MainForm(development));
            }
            else
            {
                Application.Run(new CreateConnectionForm());
            }
        }
    }
}
