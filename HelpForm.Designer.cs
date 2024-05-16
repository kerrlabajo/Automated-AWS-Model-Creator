
namespace AutomatedAWSModelCreator
{
    partial class HelpForm
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
            this.helpPanel = new System.Windows.Forms.Panel();
            this.helpBox = new System.Windows.Forms.RichTextBox();
            this.helpPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // helpPanel
            // 
            this.helpPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.helpPanel.Controls.Add(this.helpBox);
            this.helpPanel.Location = new System.Drawing.Point(12, 12);
            this.helpPanel.Name = "helpPanel";
            this.helpPanel.Size = new System.Drawing.Size(956, 574);
            this.helpPanel.TabIndex = 5;
            // 
            // helpBox
            // 
            this.helpBox.BackColor = System.Drawing.SystemColors.HighlightText;
            this.helpBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.helpBox.Location = new System.Drawing.Point(12, 16);
            this.helpBox.Name = "helpBox";
            this.helpBox.ReadOnly = true;
            this.helpBox.Size = new System.Drawing.Size(926, 542);
            this.helpBox.TabIndex = 27;
            this.helpBox.Text = "";
            this.helpBox.WordWrap = false;
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 598);
            this.Controls.Add(this.helpPanel);
            this.Name = "HelpForm";
            this.Text = "HelpForm";
            this.helpPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel helpPanel;
        private System.Windows.Forms.RichTextBox helpBox;
    }
}