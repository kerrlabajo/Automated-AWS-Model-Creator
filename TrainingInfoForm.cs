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
        public string PrevStatusMessage { get; set; }
        public string PrevLogMessage { get; set; } 
        public int PrevLogIndex { get; set; }
        public TrainingInfoForm()
        {
            PrevLogMessage = "";
            PrevLogIndex = 0;
            InitializeComponent();
        }

        public void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description)
        {
            instanceTypeBox.Text = instanceType;
            trainingDurationBox.Text = trainingDuration;
            trainingStatusBox.Text = status;
            descBox.Text = description;
            PrevStatusMessage = status;
        }
        public void UpdateTrainingStatus(string trainingDuration)
        {
            trainingDurationBox.Text = trainingDuration;
        }
        public void UpdateTrainingStatus(string status, string description)
        {
            trainingStatusBox.Text = status;
            descBox.Text = description;
            PrevStatusMessage = status;
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
