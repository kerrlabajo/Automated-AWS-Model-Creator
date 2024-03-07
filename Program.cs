using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // Keep this as true for development.
            // There is still a problem when running the application in production wherein
            // two forms will be loaded and some of the auto role checker are persisting due to
            // adding `mainForms` as a parameter in the constructor of the forms.
            bool development = true;
            DotNetEnv.Env.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(development));
        }
    }
}
