using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedAWSModelCreator.Functions
{
    public interface IUIUpdater
    {
        void SetLogPanelVisibility(bool visibility);
        void UpdateTrainingStatus(string instanceType, string trainingDuration, string status, string description);
        void UpdateTrainingStatus(string trainingDuration);
        void UpdateTrainingStatus(string status, string description);
        void DisplayLogMessage(string logMessage);
        string ConvertAnsiToRtf(string ansiText);
        void UpdateDownloadStatus(int percentage);
    }
}
