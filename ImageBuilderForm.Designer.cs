
namespace LSC_Trainer
{
    partial class ImageBuilderForm
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
            this.imageBuilderPanel = new System.Windows.Forms.Panel();
            this.buildButton = new System.Windows.Forms.Button();
            this.tag = new System.Windows.Forms.TextBox();
            this.repoName = new System.Windows.Forms.TextBox();
            this.tagLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.repoNameLabel = new System.Windows.Forms.Label();
            this.imageBuilderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBuilderPanel
            // 
            this.imageBuilderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBuilderPanel.Controls.Add(this.buildButton);
            this.imageBuilderPanel.Controls.Add(this.tag);
            this.imageBuilderPanel.Controls.Add(this.repoName);
            this.imageBuilderPanel.Controls.Add(this.tagLabel);
            this.imageBuilderPanel.Controls.Add(this.label3);
            this.imageBuilderPanel.Controls.Add(this.repoNameLabel);
            this.imageBuilderPanel.Location = new System.Drawing.Point(71, 51);
            this.imageBuilderPanel.Name = "imageBuilderPanel";
            this.imageBuilderPanel.Size = new System.Drawing.Size(554, 205);
            this.imageBuilderPanel.TabIndex = 6;
            // 
            // buildButton
            // 
            this.buildButton.Location = new System.Drawing.Point(395, 134);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(104, 31);
            this.buildButton.TabIndex = 3;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // tag
            // 
            this.tag.Location = new System.Drawing.Point(180, 91);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(319, 22);
            this.tag.TabIndex = 2;
            // 
            // repoName
            // 
            this.repoName.Location = new System.Drawing.Point(180, 53);
            this.repoName.Name = "repoName";
            this.repoName.Size = new System.Drawing.Size(319, 22);
            this.repoName.TabIndex = 1;
            // 
            // tagLabel
            // 
            this.tagLabel.AutoSize = true;
            this.tagLabel.Location = new System.Drawing.Point(36, 91);
            this.tagLabel.Name = "tagLabel";
            this.tagLabel.Size = new System.Drawing.Size(32, 16);
            this.tagLabel.TabIndex = 5;
            this.tagLabel.Text = "Tag";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 16);
            this.label3.TabIndex = 3;
            // 
            // repoNameLabel
            // 
            this.repoNameLabel.AutoSize = true;
            this.repoNameLabel.Location = new System.Drawing.Point(36, 53);
            this.repoNameLabel.Name = "repoNameLabel";
            this.repoNameLabel.Size = new System.Drawing.Size(113, 16);
            this.repoNameLabel.TabIndex = 2;
            this.repoNameLabel.Text = "Repository Name";
            // 
            // ImageBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 305);
            this.Controls.Add(this.imageBuilderPanel);
            this.Name = "ImageBuilderForm";
            this.Text = "ImageBuilderForm";
            this.imageBuilderPanel.ResumeLayout(false);
            this.imageBuilderPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel imageBuilderPanel;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.TextBox tag;
        private System.Windows.Forms.TextBox repoName;
        private System.Windows.Forms.Label tagLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label repoNameLabel;
    }
}