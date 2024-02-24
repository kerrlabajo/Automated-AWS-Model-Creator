using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSC_Trainer
{
    public partial class EmployeeEcrUriInputForm : Form
    {
        private MainForm mainForm;
        public EmployeeEcrUriInputForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            UserConnectionInfo.EcrUri = ecrUri.Text;

            if (!string.IsNullOrWhiteSpace(UserConnectionInfo.EcrUri))
            {
                MessageBox.Show("Successful!");
                var t = new Thread(() => Application.Run(new MainForm(mainForm.development)));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                mainForm.Close();
                this.Close();
            }
            else
            {
                MessageBox.Show("Input field is required!");
            }
        }
    }
}
