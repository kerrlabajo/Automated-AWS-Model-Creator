
namespace LSC_Trainer
{
    partial class CreateConnectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.createConnectionPanel = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.regionDropdown = new System.Windows.Forms.ComboBox();
            this.roleARN = new System.Windows.Forms.TextBox();
            this.secretKeyID = new System.Windows.Forms.TextBox();
            this.accessKeyID = new System.Windows.Forms.TextBox();
            this.roleARNLabel = new System.Windows.Forms.Label();
            this.regionLabel = new System.Windows.Forms.Label();
            this.secretKeyIdLabel = new System.Windows.Forms.Label();
            this.accessKeyIdLabel = new System.Windows.Forms.Label();
            this.accountIdLabel = new System.Windows.Forms.Label();
            this.accountID = new System.Windows.Forms.TextBox();
            this.createConnectionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // createConnectionPanel
            // 
            this.createConnectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.createConnectionPanel.Controls.Add(this.btnConnect);
            this.createConnectionPanel.Controls.Add(this.regionDropdown);
            this.createConnectionPanel.Controls.Add(this.roleARN);
            this.createConnectionPanel.Controls.Add(this.secretKeyID);
            this.createConnectionPanel.Controls.Add(this.accessKeyID);
            this.createConnectionPanel.Controls.Add(this.roleARNLabel);
            this.createConnectionPanel.Controls.Add(this.regionLabel);
            this.createConnectionPanel.Controls.Add(this.secretKeyIdLabel);
            this.createConnectionPanel.Controls.Add(this.accessKeyIdLabel);
            this.createConnectionPanel.Controls.Add(this.accountIdLabel);
            this.createConnectionPanel.Controls.Add(this.accountID);
            this.createConnectionPanel.Location = new System.Drawing.Point(60, 48);
            this.createConnectionPanel.Name = "createConnectionPanel";
            this.createConnectionPanel.Size = new System.Drawing.Size(554, 355);
            this.createConnectionPanel.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(395, 298);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 31);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConect_Click);
            // 
            // regionDropdown
            // 
            this.regionDropdown.FormattingEnabled = true;
            this.regionDropdown.Items.AddRange(new object[] {
            "US East (N. Virginia)",
            "US East (Ohio)",
            "US West (N. California)",
            "US West (Oregon)",
            "Asia Pacific (Mumbai)",
            "Asia Pacific (Osaka)",
            "Asia Pacific (Seoul)",
            "Asia Pacific (Singapore)",
            "Asia Pacific (Sydney)",
            "Asia Pacific (Tokyo)",
            "Canada (Central)",
            "Europe (Frankfurt)",
            "Europe (Ireland)",
            "Europe (London)",
            "Europe (Paris)",
            "Europe (Stockholm)",
            "South America (São Paulo)"});
            this.regionDropdown.Location = new System.Drawing.Point(180, 171);
            this.regionDropdown.Name = "regionDropdown";
            this.regionDropdown.Size = new System.Drawing.Size(319, 24);
            this.regionDropdown.TabIndex = 4;
            // 
            // roleARN
            // 
            this.roleARN.Location = new System.Drawing.Point(180, 221);
            this.roleARN.Name = "roleARN";
            this.roleARN.Size = new System.Drawing.Size(319, 22);
            this.roleARN.TabIndex = 5;
            // 
            // secretKeyID
            // 
            this.secretKeyID.Location = new System.Drawing.Point(180, 121);
            this.secretKeyID.Name = "secretKeyID";
            this.secretKeyID.Size = new System.Drawing.Size(319, 22);
            this.secretKeyID.TabIndex = 3;
            this.secretKeyID.UseSystemPasswordChar = true;
            // 
            // accessKeyID
            // 
            this.accessKeyID.Location = new System.Drawing.Point(180, 80);
            this.accessKeyID.Name = "accessKeyID";
            this.accessKeyID.Size = new System.Drawing.Size(319, 22);
            this.accessKeyID.TabIndex = 2;
            this.accessKeyID.UseSystemPasswordChar = true;
            // 
            // roleARNLabel
            // 
            this.roleARNLabel.AutoSize = true;
            this.roleARNLabel.Location = new System.Drawing.Point(36, 221);
            this.roleARNLabel.Name = "roleARNLabel";
            this.roleARNLabel.Size = new System.Drawing.Size(68, 16);
            this.roleARNLabel.TabIndex = 5;
            this.roleARNLabel.Text = "Role ARN";
            // 
            // regionLabel
            // 
            this.regionLabel.AutoSize = true;
            this.regionLabel.Location = new System.Drawing.Point(36, 171);
            this.regionLabel.Name = "regionLabel";
            this.regionLabel.Size = new System.Drawing.Size(51, 16);
            this.regionLabel.TabIndex = 4;
            this.regionLabel.Text = "Region";
            // 
            // secretKeyIdLabel
            // 
            this.secretKeyIdLabel.AutoSize = true;
            this.secretKeyIdLabel.Location = new System.Drawing.Point(36, 124);
            this.secretKeyIdLabel.Name = "secretKeyIdLabel";
            this.secretKeyIdLabel.Size = new System.Drawing.Size(88, 16);
            this.secretKeyIdLabel.TabIndex = 3;
            this.secretKeyIdLabel.Text = "Secret Key ID";
            // 
            // accessKeyIdLabel
            // 
            this.accessKeyIdLabel.AutoSize = true;
            this.accessKeyIdLabel.Location = new System.Drawing.Point(36, 80);
            this.accessKeyIdLabel.Name = "accessKeyIdLabel";
            this.accessKeyIdLabel.Size = new System.Drawing.Size(94, 16);
            this.accessKeyIdLabel.TabIndex = 2;
            this.accessKeyIdLabel.Text = "Access Key ID";
            // 
            // accountIdLabel
            // 
            this.accountIdLabel.AutoSize = true;
            this.accountIdLabel.Location = new System.Drawing.Point(36, 41);
            this.accountIdLabel.Name = "accountIdLabel";
            this.accountIdLabel.Size = new System.Drawing.Size(71, 16);
            this.accountIdLabel.TabIndex = 1;
            this.accountIdLabel.Text = "Account ID";
            // 
            // accountID
            // 
            this.accountID.Location = new System.Drawing.Point(180, 38);
            this.accountID.Name = "accountID";
            this.accountID.Size = new System.Drawing.Size(319, 22);
            this.accountID.TabIndex = 1;
            // 
            // CreateConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 450);
            this.Controls.Add(this.createConnectionPanel);
            this.Name = "CreateConnectionForm";
            this.Text = "Create Connection";
            this.createConnectionPanel.ResumeLayout(false);
            this.createConnectionPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel createConnectionPanel;
        private System.Windows.Forms.TextBox accountID;
        private System.Windows.Forms.Label accessKeyIdLabel;
        private System.Windows.Forms.Label accountIdLabel;
        private System.Windows.Forms.Label secretKeyIdLabel;
        private System.Windows.Forms.Label regionLabel;
        private System.Windows.Forms.ComboBox regionDropdown;
        private System.Windows.Forms.TextBox roleARN;
        private System.Windows.Forms.TextBox secretKeyID;
        private System.Windows.Forms.TextBox accessKeyID;
        private System.Windows.Forms.Label roleARNLabel;
        private System.Windows.Forms.Button btnConnect;
    }
}