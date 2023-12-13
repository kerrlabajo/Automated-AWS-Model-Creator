
namespace LSC_Trainer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.connectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelectDataset = new System.Windows.Forms.Button();
            this.lblZipFile = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnDownloadModel = new System.Windows.Forms.Button();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.btnUploadToS3 = new System.Windows.Forms.Button();
            this.txtOptimiser = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtWorkers = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtProject = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtEpochs = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnTraining = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtHyperparameters = new System.Windows.Forms.TextBox();
            this.txtPatience = new System.Windows.Forms.TextBox();
            this.txtWeights = new System.Windows.Forms.TextBox();
            this.txtImgSize = new System.Windows.Forms.TextBox();
            this.txtImgNum = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(133, 28);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(132, 24);
            this.connectToolStripMenuItem.Text = "Connect";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem1,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(996, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // connectToolStripMenuItem1
            // 
            this.connectToolStripMenuItem1.Name = "connectToolStripMenuItem1";
            this.connectToolStripMenuItem1.Size = new System.Drawing.Size(77, 24);
            this.connectToolStripMenuItem1.Text = "Connect";
            this.connectToolStripMenuItem1.Click += new System.EventHandler(this.connectToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // btnSelectDataset
            // 
            this.btnSelectDataset.Location = new System.Drawing.Point(33, 308);
            this.btnSelectDataset.Name = "btnSelectDataset";
            this.btnSelectDataset.Size = new System.Drawing.Size(174, 30);
            this.btnSelectDataset.TabIndex = 2;
            this.btnSelectDataset.Text = "Select Dataset (.zip)";
            this.btnSelectDataset.UseVisualStyleBackColor = true;
            this.btnSelectDataset.Click += new System.EventHandler(this.btnSelectDataset_Click);
            // 
            // lblZipFile
            // 
            this.lblZipFile.AutoSize = true;
            this.lblZipFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblZipFile.Location = new System.Drawing.Point(176, 226);
            this.lblZipFile.Name = "lblZipFile";
            this.lblZipFile.Size = new System.Drawing.Size(107, 19);
            this.lblZipFile.TabIndex = 3;
            this.lblZipFile.Text = "No file selected";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Controls.Add(this.btnSelectFolder);
            this.panel1.Controls.Add(this.btnDownloadModel);
            this.panel1.Controls.Add(this.btnRemoveFile);
            this.panel1.Controls.Add(this.btnUploadToS3);
            this.panel1.Controls.Add(this.txtOptimiser);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.txtWorkers);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.txtProject);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.txtBatchSize);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.txtEpochs);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label19);
            this.panel1.Controls.Add(this.btnTraining);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtHyperparameters);
            this.panel1.Controls.Add(this.txtPatience);
            this.panel1.Controls.Add(this.txtWeights);
            this.panel1.Controls.Add(this.txtImgSize);
            this.panel1.Controls.Add(this.txtImgNum);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblZipFile);
            this.panel1.Controls.Add(this.btnSelectDataset);
            this.panel1.Location = new System.Drawing.Point(37, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(925, 355);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(213, 308);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(176, 30);
            this.btnSelectFolder.TabIndex = 43;
            this.btnSelectFolder.Text = "Select Dataset (folder)";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnDownloadModel
            // 
            this.btnDownloadModel.Location = new System.Drawing.Point(787, 308);
            this.btnDownloadModel.Name = "btnDownloadModel";
            this.btnDownloadModel.Size = new System.Drawing.Size(120, 30);
            this.btnDownloadModel.TabIndex = 42;
            this.btnDownloadModel.Text = "Download Model";
            this.btnDownloadModel.UseVisualStyleBackColor = true;
            this.btnDownloadModel.Click += new System.EventHandler(this.btnDownloadModel_Click);
            // 
            // btnRemoveFile
            // 
            this.btnRemoveFile.Location = new System.Drawing.Point(176, 248);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(97, 23);
            this.btnRemoveFile.TabIndex = 41;
            this.btnRemoveFile.Text = "Remove";
            this.btnRemoveFile.UseVisualStyleBackColor = true;
            this.btnRemoveFile.Visible = false;
            this.btnRemoveFile.Click += new System.EventHandler(this.btnRemoveFile_Click);
            // 
            // btnUploadToS3
            // 
            this.btnUploadToS3.Location = new System.Drawing.Point(406, 308);
            this.btnUploadToS3.Name = "btnUploadToS3";
            this.btnUploadToS3.Size = new System.Drawing.Size(170, 30);
            this.btnUploadToS3.TabIndex = 40;
            this.btnUploadToS3.Text = "Upload Dataset";
            this.btnUploadToS3.UseVisualStyleBackColor = true;
            this.btnUploadToS3.Click += new System.EventHandler(this.btnUploadToS3_Click);
            // 
            // txtOptimiser
            // 
            this.txtOptimiser.Location = new System.Drawing.Point(387, 183);
            this.txtOptimiser.Name = "txtOptimiser";
            this.txtOptimiser.Size = new System.Drawing.Size(107, 22);
            this.txtOptimiser.TabIndex = 39;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(302, 186);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 17);
            this.label24.TabIndex = 38;
            this.label24.Text = "Optimiser:";
            // 
            // txtWorkers
            // 
            this.txtWorkers.Location = new System.Drawing.Point(387, 155);
            this.txtWorkers.Name = "txtWorkers";
            this.txtWorkers.Size = new System.Drawing.Size(107, 22);
            this.txtWorkers.TabIndex = 37;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(302, 158);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(65, 17);
            this.label23.TabIndex = 36;
            this.label23.Text = "Workers:";
            // 
            // txtProject
            // 
            this.txtProject.Location = new System.Drawing.Point(387, 127);
            this.txtProject.Name = "txtProject";
            this.txtProject.Size = new System.Drawing.Size(107, 22);
            this.txtProject.TabIndex = 35;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(302, 130);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 17);
            this.label22.TabIndex = 34;
            this.label22.Text = "Project:";
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.Location = new System.Drawing.Point(387, 99);
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(107, 22);
            this.txtBatchSize.TabIndex = 33;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(302, 102);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(79, 17);
            this.label21.TabIndex = 32;
            this.label21.Text = "Batch Size:";
            // 
            // txtEpochs
            // 
            this.txtEpochs.Location = new System.Drawing.Point(387, 71);
            this.txtEpochs.Name = "txtEpochs";
            this.txtEpochs.Size = new System.Drawing.Size(107, 22);
            this.txtEpochs.TabIndex = 31;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(302, 74);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(59, 17);
            this.label20.TabIndex = 30;
            this.label20.Text = "Epochs:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(651, 58);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(155, 17);
            this.label19.TabIndex = 29;
            this.label19.Text = "TRAINING PROGRESS";
            // 
            // btnTraining
            // 
            this.btnTraining.Location = new System.Drawing.Point(625, 308);
            this.btnTraining.Name = "btnTraining";
            this.btnTraining.Size = new System.Drawing.Size(120, 30);
            this.btnTraining.TabIndex = 28;
            this.btnTraining.Text = "Train";
            this.btnTraining.UseVisualStyleBackColor = true;
            this.btnTraining.Click += new System.EventHandler(this.btnTraining_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(784, 196);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 17);
            this.label18.TabIndex = 27;
            this.label18.Text = "N/A";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(784, 169);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(31, 17);
            this.label17.TabIndex = 26;
            this.label17.Text = "N/A";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(784, 142);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(31, 17);
            this.label16.TabIndex = 25;
            this.label16.Text = "N/A";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(784, 115);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(31, 17);
            this.label15.TabIndex = 24;
            this.label15.Text = "N/A";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(784, 88);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 17);
            this.label14.TabIndex = 23;
            this.label14.Text = "N/A";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(626, 196);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(119, 17);
            this.label13.TabIndex = 22;
            this.label13.Text = "Model Save Path:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(626, 169);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(122, 17);
            this.label12.TabIndex = 21;
            this.label12.Text = "Training Duration:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(626, 142);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(152, 17);
            this.label11.TabIndex = 20;
            this.label11.Text = "Virtual Machine Specs:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(626, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 17);
            this.label10.TabIndex = 19;
            this.label10.Text = "Upload Speed:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.label9.Location = new System.Drawing.Point(626, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(139, 17);
            this.label9.TabIndex = 18;
            this.label9.Text = "Connection duration:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(84, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(171, 17);
            this.label8.TabIndex = 17;
            this.label8.Text = "TRAINING PARAMETERS";
            // 
            // txtHyperparameters
            // 
            this.txtHyperparameters.Location = new System.Drawing.Point(178, 183);
            this.txtHyperparameters.Name = "txtHyperparameters";
            this.txtHyperparameters.Size = new System.Drawing.Size(107, 22);
            this.txtHyperparameters.TabIndex = 16;
            // 
            // txtPatience
            // 
            this.txtPatience.Location = new System.Drawing.Point(178, 155);
            this.txtPatience.Name = "txtPatience";
            this.txtPatience.Size = new System.Drawing.Size(107, 22);
            this.txtPatience.TabIndex = 15;
            // 
            // txtWeights
            // 
            this.txtWeights.Location = new System.Drawing.Point(178, 127);
            this.txtWeights.Name = "txtWeights";
            this.txtWeights.Size = new System.Drawing.Size(107, 22);
            this.txtWeights.TabIndex = 14;
            // 
            // txtImgSize
            // 
            this.txtImgSize.Location = new System.Drawing.Point(178, 99);
            this.txtImgSize.Name = "txtImgSize";
            this.txtImgSize.Size = new System.Drawing.Size(107, 22);
            this.txtImgSize.TabIndex = 13;
            // 
            // txtImgNum
            // 
            this.txtImgNum.Location = new System.Drawing.Point(178, 71);
            this.txtImgNum.Name = "txtImgNum";
            this.txtImgNum.Size = new System.Drawing.Size(107, 22);
            this.txtImgNum.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(49, 182);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "Hyperparameters:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(49, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "Patience: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Weights:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Image Size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Number of Images:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 17);
            this.label2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Selected File:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(406, 273);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(170, 23);
            this.progressBar.TabIndex = 44;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 450);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button btnSelectDataset;
        private System.Windows.Forms.Label lblZipFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtHyperparameters;
        private System.Windows.Forms.TextBox txtPatience;
        private System.Windows.Forms.TextBox txtWeights;
        private System.Windows.Forms.TextBox txtImgSize;
        private System.Windows.Forms.TextBox txtImgNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnTraining;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtOptimiser;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtWorkers;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtProject;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtBatchSize;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtEpochs;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnUploadToS3;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Button btnDownloadModel;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

