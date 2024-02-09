using System;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace LSC_Trainer
{
    public partial class CreateConnectionForm : Form
    {

        private readonly AmazonIdentityManagementServiceClient _iamClient;
        
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

            List<string> lines = new List<string>
            {
                $"ACCOUNT_ID={accountId}",
                $"ROLE_ARN={roleArn}",
                $"ACCESS_KEY_ID={accessKey}",
                $"SECRET_ACCESS_KEY={secretKey}",
                $"REGION={region}",
                $"ROLE_ARN={roleArn}",
            };

            string envFilePath = Path.Combine(Application.StartupPath, "./env");
            File.WriteAllLines(envFilePath, lines);

            MessageBox.Show("Successfully created a connection");
            this.Close();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
           
        }

        public async Task<string> GetRoleDetailsAsync(string roleArn)
        {
            try
            {
                var response = await _iamClient.GetRoleAsync(new GetRoleRequest
                {
                    RoleName = ExtractRoleNameFromArn(roleArn)
                });

                bool isAdmin = await IsAdminRole(response.Role);

                return isAdmin ? "admin" : "employee";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving role: {ex.Message}");
                return null;
            }
        }

        private static string ExtractRoleNameFromArn(string roleArn)
        {
            var splitArn = roleArn.Split(':');
            var roleName = splitArn.Last().Split('/').Last();
            return roleName;
        }

        private async Task<bool> IsAdminRole(Role role)
        {
            bool isAdmin = false;

            var managedPoliciesResponse = await _iamClient.ListAttachedRolePoliciesAsync(new ListAttachedRolePoliciesRequest
            {
                RoleName = role.RoleName
            });

            isAdmin |= managedPoliciesResponse.AttachedPolicies.Any(policy => policy.PolicyName == "AdministratorAccess");

            return isAdmin;
        }
    }
}
