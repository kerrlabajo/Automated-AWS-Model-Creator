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
            this.logBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.descBox = new System.Windows.Forms.TextBox();
            this.trainingStatusBox = new System.Windows.Forms.TextBox();
            this.trainingDurationBox = new System.Windows.Forms.TextBox();
            this.instanceTypeBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(237, 35);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(101, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Virtual Machine:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(226, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Training Duration:";
            // 
            // instanceTypelbl
            // 
            this.instanceTypelbl.AutoSize = true;
            this.instanceTypelbl.Location = new System.Drawing.Point(378, 29);
            this.instanceTypelbl.Name = "instanceTypelbl";
            this.instanceTypelbl.Size = new System.Drawing.Size(0, 16);
            this.instanceTypelbl.TabIndex = 2;
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logBox.Location = new System.Drawing.Point(46, 199);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(752, 239);
            this.logBox.TabIndex = 4;
            this.logBox.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(239, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Training Status:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(260, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Description:";
            // 
            // descBox
            // 
            this.descBox.Location = new System.Drawing.Point(381, 136);
            this.descBox.Multiline = true;
            this.descBox.Name = "descBox";
            this.descBox.ReadOnly = true;
            this.descBox.Size = new System.Drawing.Size(277, 40);
            this.descBox.TabIndex = 9;
            // 
            // trainingStatusBox
            // 
            this.trainingStatusBox.Location = new System.Drawing.Point(381, 103);
            this.trainingStatusBox.Name = "trainingStatusBox";
            this.trainingStatusBox.ReadOnly = true;
            this.trainingStatusBox.Size = new System.Drawing.Size(277, 22);
            this.trainingStatusBox.TabIndex = 10;
            // 
            // trainingDurationBox
            // 
            this.trainingDurationBox.Location = new System.Drawing.Point(381, 68);
            this.trainingDurationBox.Name = "trainingDurationBox";
            this.trainingDurationBox.ReadOnly = true;
            this.trainingDurationBox.Size = new System.Drawing.Size(277, 22);
            this.trainingDurationBox.TabIndex = 11;
            // 
            // instanceTypeBox
            // 
            this.instanceTypeBox.Location = new System.Drawing.Point(384, 35);
            this.instanceTypeBox.Name = "instanceTypeBox";
            this.instanceTypeBox.ReadOnly = true;
            this.instanceTypeBox.Size = new System.Drawing.Size(277, 22);
            this.instanceTypeBox.TabIndex = 12;
            // 
            // TrainingInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 450);
            this.Controls.Add(this.instanceTypeBox);
            this.Controls.Add(this.trainingDurationBox);
            this.Controls.Add(this.trainingStatusBox);
            this.Controls.Add(this.descBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logBox);
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
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox descBox;
        private System.Windows.Forms.TextBox trainingStatusBox;
        private System.Windows.Forms.TextBox trainingDurationBox;
        private System.Windows.Forms.TextBox instanceTypeBox;
    }
}