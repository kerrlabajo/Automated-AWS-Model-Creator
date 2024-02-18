
namespace LSC_Trainer
{
    partial class EmployeeEcrUriInputForm
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
            this.continueBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ecrUri = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.continueBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ecrUri);
            this.panel1.Location = new System.Drawing.Point(65, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(554, 166);
            this.panel1.TabIndex = 6;
            // 
            // continueBtn
            // 
            this.continueBtn.Location = new System.Drawing.Point(395, 92);
            this.continueBtn.Name = "continueBtn";
            this.continueBtn.Size = new System.Drawing.Size(104, 31);
            this.continueBtn.TabIndex = 11;
            this.continueBtn.Text = "Continue";
            this.continueBtn.UseVisualStyleBackColor = true;
            this.continueBtn.Click += new System.EventHandler(this.continueBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "ECR URI";
            // 
            // ecrUri
            // 
            this.ecrUri.Location = new System.Drawing.Point(180, 38);
            this.ecrUri.Name = "ecrUri";
            this.ecrUri.Size = new System.Drawing.Size(319, 22);
            this.ecrUri.TabIndex = 0;
            // 
            // EmployeeEcrUriInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 257);
            this.Controls.Add(this.panel1);
            this.Name = "EmployeeEcrUriInputForm";
            this.Text = "EmployeeEcrUriInputForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button continueBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ecrUri;
    }
}