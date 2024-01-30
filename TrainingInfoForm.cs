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
    public partial class TrainingInfoForm : Form
    {
        public TrainingInfoForm()
        {
            InitializeComponent();
        }

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string logMessage, string status, string description)
        {
            instanceTypelbl.Text = instanceType;
            trainingDurationlbl.Text = trainingDuration;

            // Update other labels and controls as needed
        }

        public void DisplayLogMessage(string logMessage)
        {
            // Display log messages in your desired control (e.g., a TextBox)
            // logTextBox.AppendText(logMessage + Environment.NewLine);
        }
    }
}
