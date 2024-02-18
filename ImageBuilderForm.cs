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

            UserConnectionInfo.EcrUri = $"{accountId}.dkr.ecr.{region}.amazonaws.com/{repositoryName}:{imageTag}";
        }

        
        public void ExecuteShellScript(string accountId, string region, string repoName, string imageTag)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .sample.sh").Replace("\\", "/");
            string command = $"wsl bash -c '\"{scriptPath}\" \"{accountId}\" \"{region}\" \"{repoName}\" \"{imageTag}\"'";
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            //processStartInfo.Arguments = $"-Command \"wsl bash -c '{scriptPath}'\" \"{accountId}\" \"{region}\" \"{repoName}\" \"{imageTag}\"'";
            processStartInfo.Arguments = $"-Command {command}";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            Console.WriteLine("done"); 
        }
    }
}
