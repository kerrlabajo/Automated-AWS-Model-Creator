namespace LSC_Trainer
{
    partial class TrainingInfoForm
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
            this.label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.instanceTypelbl = new System.Windows.Forms.Label();
            this.trainingDurationlbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(43, 41);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(128, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Training Job Name: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Training Duration: ";
            // 
            // trainingJobNamelbl
            // 
            this.instanceTypelbl.AutoSize = true;
            this.instanceTypelbl.Location = new System.Drawing.Point(213, 41);
            this.instanceTypelbl.Name = "trainingJobNamelbl";
            this.instanceTypelbl.Size = new System.Drawing.Size(30, 16);
            this.instanceTypelbl.TabIndex = 2;
            this.instanceTypelbl.Text = "N/A";
            // 
            // trainingDurationlbl
            // 
            this.trainingDurationlbl.AutoSize = true;
            this.trainingDurationlbl.Location = new System.Drawing.Point(213, 86);
            this.trainingDurationlbl.Name = "trainingDurationlbl";
            this.trainingDurationlbl.Size = new System.Drawing.Size(30, 16);
            this.trainingDurationlbl.TabIndex = 3;
            this.trainingDurationlbl.Text = "N/A";
            // 
            // TrainingInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.trainingDurationlbl);
            this.Controls.Add(this.instanceTypelbl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label);
            this.Name = "TrainingInfoForm";
            this.Text = "TrainingInfoForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label instanceTypelbl;
        private System.Windows.Forms.Label trainingDurationlbl;
    }
}