using Amazon.Runtime.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedAWSModelCreator
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            helpBox.LinkClicked += HelpBox_LinkClicked;

            string readmeUrl = "https://raw.githubusercontent.com/kerrlabajo/Automated-AWS-Model-Creator/main/README.md";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string readmeText = "You may refer to https://github.com/kerrlabajo/Automated-AWS-Model-Creator.\n" + client.GetStringAsync(readmeUrl).Result;
                    helpBox.Text = readmeText;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching README.md file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void HelpBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Open the link in the default browser
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
