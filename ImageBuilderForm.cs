using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            string repositoryName = repoName.Text;
            string region = regionDropdown.GetItemText(regionDropdown.SelectedItem);
            string imageTag = tag.Text;


            ExecuteShellScript(accountId, repositoryName, region, imageTag);
        }

        public string ExecuteShellScript(string accountId, string region, string repoName, string imageTag)
        {
            try
            {
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " training-container/scripts/build_and_push.sh").Replace("\\", "/");

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "/build_and_push.sh"; 
                psi.Arguments = $"{scriptPath} {accountId} {region} {repoName} {imageTag}";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd(); 
                        Console.WriteLine(result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }
    }
}
