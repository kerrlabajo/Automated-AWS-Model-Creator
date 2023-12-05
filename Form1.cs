using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.SageMaker;

namespace LSC_Trainer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ///TODO: Initialize AmazonSageMakerClient by fetching from an .env variable with your AWS credentials.
            DotNetEnv.Env.Load();

            string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            string region = Environment.GetEnvironmentVariable("AWS_REGION");

            AmazonSageMakerClient amazonSageMakerClient = new AmazonSageMakerClient(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));


        }

        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ///TODO: To test if your AWS credentials is accessible and able to connect, execute this event with the AmazonSageMakerClient.
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "ZIP Files (*.zip)|*.zip|RAR Files (*.rar)|*.rar";
                openFileDialog.Title = "Select a Zip or Rar File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Display the selected file path (optional)
                    lblZipFile.Text = selectedFilePath;
                 
                    // Handle the zip file as needed
                    // For example, you can extract its contents or upload it to a server
                    // For simplicity, we'll just show a message box with the file path
                    MessageBox.Show($"Selected file: {selectedFilePath}");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void lblZipFile_Click(object sender, EventArgs e)
        {

        }

        ///TODO: Create a button to upload a dataset in .rar/.zip file.

        ///TODO: Create a text area to get the training parameters:
        ///TODO: number of images to train [NOT YET SURE/IMPLEMENTED IN YOLOV5 REPO]
        ///TODO: image size (default 1280), batch size, epochs, 
        ///TODO: weights (default yolov5n6.pt), project (default S3 destination), 
        ///TODO: patience (default 100), workers (default 8), optimizer (default SGD), 
        ///TODO: hyperparameters (default hyp.scratch-low.yamL)

        ///TODO: Create a text area to print the ff:
        ///TODO: 1. AmazonSageMakerClient connection duration.
        ///TODO: 2. Dataset file size, and uploading network connectivity and duration
        ///TODO: 3. Virtual machine specs used for training (the instance we selected to create the training job).
        ///TODO: 4. Duration of the training in AWS SageMaker Training Job creation and training.
        ///TODO: 5. Model url that was saved in AWS S3.

        ///TODO: Create a proceed training button to create a training job in AWS.
        ///TODO: Implement the create training job using SageMaker with the .env variable that contains the AWS ECR location 
        ///TODO: of the team's training algorithm, specific EC2 Instance (Virtual Machine) that is free version, and other
        ///TODO: specific configurations in the web interface that can be replicated here in AWSSDK.SageMaker.

    }
}
