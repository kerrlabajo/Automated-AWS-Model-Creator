﻿
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.regionDropdown = new System.Windows.Forms.ComboBox();
            this.roleARN = new System.Windows.Forms.TextBox();
            this.secretKeyID = new System.Windows.Forms.TextBox();
            this.accessKeyID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.accountID = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.regionDropdown);
            this.panel1.Controls.Add(this.roleARN);
            this.panel1.Controls.Add(this.secretKeyID);
            this.panel1.Controls.Add(this.accessKeyID);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.accountID);
            this.panel1.Location = new System.Drawing.Point(60, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(554, 355);
            this.panel1.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(395, 298);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 31);
            this.button1.TabIndex = 11;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // regionDropdown
            // 
            this.regionDropdown.FormattingEnabled = true;
            this.regionDropdown.Items.AddRange(new object[] {
            "ap-southeast-1",
            "ap-southeast-2",
            "ap-southeast-3",
            "ap-southeast-4",
            "ap-northeast-1",
            "ap-northeast-2",
            "ap-northeast-3",
            "ap-east-1",
            "ap-south-1",
            "ap-south-2"});
            this.regionDropdown.Location = new System.Drawing.Point(180, 171);
            this.regionDropdown.Name = "regionDropdown";
            this.regionDropdown.Size = new System.Drawing.Size(319, 24);
            this.regionDropdown.TabIndex = 10;
            this.regionDropdown.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // roleARN
            // 
            this.roleARN.Location = new System.Drawing.Point(180, 221);
            this.roleARN.Name = "roleARN";
            this.roleARN.Size = new System.Drawing.Size(319, 22);
            this.roleARN.TabIndex = 9;
            // 
            // secretKeyID
            // 
            this.secretKeyID.Location = new System.Drawing.Point(180, 121);
            this.secretKeyID.Name = "secretKeyID";
            this.secretKeyID.Size = new System.Drawing.Size(319, 22);
            this.secretKeyID.TabIndex = 7;
            this.secretKeyID.UseSystemPasswordChar = true;
            // 
            // accessKeyID
            // 
            this.accessKeyID.Location = new System.Drawing.Point(180, 80);
            this.accessKeyID.Name = "accessKeyID";
            this.accessKeyID.Size = new System.Drawing.Size(319, 22);
            this.accessKeyID.TabIndex = 6;
            this.accessKeyID.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 221);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Role ARN";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "Region";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Secret Key ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Access Key ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Account ID";
            // 
            // accountID
            // 
            this.accountID.Location = new System.Drawing.Point(180, 38);
            this.accountID.Name = "accountID";
            this.accountID.Size = new System.Drawing.Size(319, 22);
            this.accountID.TabIndex = 0;
            // 
            // CreateConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 450);
            this.Controls.Add(this.panel1);
            this.Name = "CreateConnectionForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox accountID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox regionDropdown;
        private System.Windows.Forms.TextBox roleARN;
        private System.Windows.Forms.TextBox secretKeyID;
        private System.Windows.Forms.TextBox accessKeyID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
    }
}