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

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            instanceTypeBox.Text = instanceType;
            trainingDurationBox.Text = trainingDuration;
            trainingStatusBox.Text = status;
            descBox.Text = description;
            // Update other labels and controls as needed
        }

        public void DisplayLogMessage(string logMessage)
        {
            // Append log messages to the TextBox
            logBox.AppendText(logMessage + Environment.NewLine);

            // Scroll to the end to show the latest log messages
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }
    }
}
