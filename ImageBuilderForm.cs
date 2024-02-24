using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSC_Trainer
{
    public partial class ImageBuilderForm : Form
    {
        private string accountId;
        private string accessKey;
        private string secretKey;
        private string region;
        private MainForm mainForm;

        private string repositoryName;
        private string imageTag;

        private readonly string INTELLISYS_ECR_URI;
        public ImageBuilderForm(string accountId, string accessKey, string secretKey, string region, MainForm mainForm)
        {
            InitializeComponent();
            this.accountId = accountId;
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.region = region;
            this.mainForm = mainForm;

            // Load environment variables
            // The env var to be loaded should only be the INTELLISYS_ECR_URI.
            string ENV_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, " .env").Replace("\\", "/");
            DotNetEnv.Env.Load(ENV_PATH);
            INTELLISYS_ECR_URI = Environment.GetEnvironmentVariable("INTELLISYS_ECR_URI");
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            bool allTextBoxesFilled = !string.IsNullOrWhiteSpace(repoName.Text) && !string.IsNullOrWhiteSpace(tag.Text);

            if (!allTextBoxesFilled)
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            repositoryName = repoName.Text;
            imageTag = tag.Text;

            //var response = LaunchInstancePushPrivateECR();
            UserConnectionInfo.EcrUri = $"{accountId}.dkr.ecr.{region}.amazonaws.com/{repositoryName}:{imageTag}";
            var t = new Thread(() => Application.Run(new MainForm(mainForm.development)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            mainForm.Close();
            this.Close();
        }
        
        public RunInstancesResponse LaunchInstancePushPrivateECR()
        {
            string userDataScript = $@"
                #!/bin/bash
                aws configure set aws_access_key_id {accessKey}
                aws configure set aws_secret_access_key {secretKey}
                aws configure set region {region}
                sudo docker tag {INTELLISYS_ECR_URI} {UserConnectionInfo.EcrUri}
                sudo docker push {UserConnectionInfo.EcrUri}
            ";
            string userData = Convert.ToBase64String(Encoding.UTF8.GetBytes(userDataScript));
            var client = new AmazonEC2Client();

            return client.RunInstances(new RunInstancesRequest
            {
                //Still temporary and not tested
                ImageId = "ami-abc12345",
                InstanceType = "t2.micro",
                KeyName = "my-key-pair",
                MaxCount = 1,
                MinCount = 1,
                SecurityGroupIds = new List<string> { "sg-1a2b3c4d" },
                SubnetId = "subnet-6e7f829e",
                UserData = userData,
                IamInstanceProfile = new IamInstanceProfileSpecification { Name = "ecsInstanceRole" }
            });
        }
    }
}
