using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSC_Trainer
{
    public partial class ImageBuilderForm : Form
    {
        public ImageBuilderForm()
        {
            InitializeComponent();
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            bool allTextBoxesFilled = !string.IsNullOrWhiteSpace(accountID.Text) &&
                             !string.IsNullOrWhiteSpace(repoName.Text) &&
                             !string.IsNullOrWhiteSpace(regionDropdown.GetItemText(regionDropdown.SelectedItem)) &&
                             !string.IsNullOrWhiteSpace(tag.Text);

            if (!allTextBoxesFilled)
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            string accountId = accountID.Text;
            string accessKey = repoName.Text;
            string region = regionDropdown.GetItemText(regionDropdown.SelectedItem);
            string roleArn = tag.Text;

        }
    }
}
