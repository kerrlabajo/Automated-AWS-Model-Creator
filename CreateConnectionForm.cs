using System;
using System.Windows.Forms;
using System.Windows.Forms;

namespace LSC_Trainer
{
    public partial class CreateConnectionForm : Form
    {
        
        public CreateConnectionForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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

            /*string roleArn = roleARN.Text;
            string roleType = await GetRoleDetailsAsync(roleArn);*/
            string accountId = accountID.Text;
            string accessKey = accessKeyID.Text;
            string secretKey = secretKeyID.Text;
            string region = regionDropdown.GetItemText(regionDropdown.SelectedItem);
            string roleArn = roleARN.Text;

            /*var newVars = new Dictionary<string, string>
            {
                {"ACCOUNT_ID", accountId},
                {"ACCESS_KEY_ID", accessKey},
                {"SECRET_ACCESS_KEY", secretKey},
                {"REGION", region},
                {"ROLE_ARN", roleArn}
            };*/

            Environment.SetEnvironmentVariable("ACCOUNT_ID", value: accountId);
            Environment.SetEnvironmentVariable("ACCESS_KEY_ID", value: accessKey);
            Environment.SetEnvironmentVariable("SECRET_ACCESS_KEY", value: secretKey);
            Environment.SetEnvironmentVariable("REGION", value: region);
            Environment.SetEnvironmentVariable("ROLE_ARN", value: roleArn);

            MessageBox.Show("Successfully created a connection");
            this.Close();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
           
        }

    }
}
