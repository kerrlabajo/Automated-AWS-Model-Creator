using Amazon;
using Amazon.SageMaker;
using Amazon.IdentityManagement;
using LSC_Trainer.Functions;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LSC_Trainer
{
    public partial class CreateConnectionForm : Form
    {
        private MainForm mainForm;
        private bool isDefaultConstructorUsed;

        public CreateConnectionForm()
        {
            InitializeComponent();
            btnConnect.Enabled = false;
            isDefaultConstructorUsed = true;
        }

        public CreateConnectionForm(MainForm mainForm)
        {
            InitializeComponent();
            btnConnect.Enabled = false;
            this.mainForm = mainForm;
            isDefaultConstructorUsed = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (isDefaultConstructorUsed)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                if (File.Exists(config.FilePath) && new FileInfo(config.FilePath).Length > 0)
                {
                    // Load settings from user.config
                    var userConnectionInfoProperties = typeof(UserConnectionInfo).GetProperties();
                    var settings = Properties.Settings.Default;

                    foreach (var property in userConnectionInfoProperties)
                    {
                        var settingProperty = settings.Properties[property.Name];
                        if (settingProperty != null)
                        {
                            property.SetValue(UserConnectionInfo.Instance, settings[property.Name]);
                        }
                    }

                    // Proceed to MainForm
                    var t = new Thread(() => Application.Run(new MainForm(false)));
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                    this.Close();
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            bool allTextBoxesFilled = !string.IsNullOrWhiteSpace(accountID.Text) &&
                             !string.IsNullOrWhiteSpace(accessKeyID.Text) &&
                             !string.IsNullOrWhiteSpace(secretKeyID.Text) &&
                             !string.IsNullOrWhiteSpace(regionDropdown.GetItemText(regionDropdown.SelectedItem)) &&
                             !string.IsNullOrWhiteSpace(roleARN.Text);

            if (!allTextBoxesFilled)
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            UserConnectionInfo.AccountId = accountID.Text;
            UserConnectionInfo.AccessKey = accessKeyID.Text;
            UserConnectionInfo.SecretKey = secretKeyID.Text;
            UserConnectionInfo.Region = GetRegionCode(regionDropdown.GetItemText(regionDropdown.SelectedItem));
            UserConnectionInfo.RoleArn = roleARN.Text;
            UserConnectionInfo.SetBucketAndURIs();

            var userConnectionInfoProperties = typeof(UserConnectionInfo).GetProperties();
            var settings = Properties.Settings.Default;

            foreach (var property in userConnectionInfoProperties)
            {
                var settingProperty = settings.Properties[property.Name];
                if (settingProperty != null)
                {
                    settings[property.Name] = property.GetValue(UserConnectionInfo.Instance);
                }
            }
            settings.Save();

            MessageBox.Show("Successfully created a connection");
            var t = new Thread(() => Application.Run(new MainForm(false)));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            mainForm?.Close();
            this.Close();
        }

        private string GetRegionCode(string region)
        {
            switch (region)
            {
                case "US East (N. Virginia)":
                    return "us-east-1";
                case "US East (Ohio)":
                    return "us-east-2";
                case "US West (N. California)":
                    return "us-west-1";
                case "US West (Oregon)":
                    return "us-west-2";
                case "Asia Pacific (Mumbai)":
                    return "ap-south-1";
                case "Asia Pacific (Osaka)":
                    return "ap-northeast-3";
                case "Asia Pacific (Seoul)":
                    return "ap-northeast-2";
                case "Asia Pacific (Singapore)":
                    return "ap-southeast-1";
                case "Asia Pacific (Sydney)":
                    return "ap-southeast-2";
                case "Asia Pacific (Tokyo)":
                    return "ap-northeast-1";
                case "Canada (Central)":
                    return "ca-central-1";
                case "Europe (Frankfurt)":
                    return "eu-central-1";
                case "Europe (Ireland)":
                    return "eu-west-1";
                case "Europe (London)":
                    return "eu-west-2";
                case "Europe (Paris)":
                    return "eu-west-3";
                case "Europe (Stockholm)":
                    return "eu-north-1";
                case "South America (São Paulo)":
                    return "sa-east-1";
                default:
                    return "us-east-1";
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                UserConnectionInfo.AccessKey = accessKeyID.Text;
                AWS_Helper.CheckCredentials(
                    new AmazonIdentityManagementServiceClient(
                        accessKeyID.Text, 
                        secretKeyID.Text, 
                        RegionEndpoint.GetBySystemName(GetRegionCode(regionDropdown.GetItemText(regionDropdown.SelectedItem)))
                    )
                );
                MessageBox.Show("Connection successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnConnect.Enabled = true;
            }
            catch (Exception error)
            {
                Console.WriteLine($"Unexpected error: {error.Message}");
                MessageBox.Show($"Connection failed: {error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnect.Enabled = false;
            }
        }
    }
}
