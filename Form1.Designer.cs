
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
            this.newTrainingJobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelectDataset = new System.Windows.Forms.Button();
            this.lblZipFile = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.descBox = new System.Windows.Forms.Label();
            this.trainingStatusBox = new System.Windows.Forms.Label();
            this.trainingDurationBox = new System.Windows.Forms.Label();
            this.instanceTypeBox = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnDownloadModel = new System.Windows.Forms.Button();
            this.btnUploadToS3 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDevice = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.instanceTypelbl = new System.Windows.Forms.Label();
            this.txtOptimizer = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.txtWorkers = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtPatience = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtHyperparameters = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnTraining = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.txtWeights = new System.Windows.Forms.TextBox();
            this.txtEpochs = new System.Windows.Forms.TextBox();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.txtImageSize = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.logBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SpaceBetween = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.helpToolStripMenuItem,
            this.newTrainingJobToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1327, 28);
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
            // newTrainingJobToolStripMenuItem
            // 
            this.newTrainingJobToolStripMenuItem.Name = "newTrainingJobToolStripMenuItem";
            this.newTrainingJobToolStripMenuItem.Size = new System.Drawing.Size(137, 24);
            this.newTrainingJobToolStripMenuItem.Text = "New Training Job";
            this.newTrainingJobToolStripMenuItem.Click += new System.EventHandler(this.newTrainingJobToolStripMenuItem_Click);
            // 
            // btnSelectDataset
            // 
            this.btnSelectDataset.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSelectDataset.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectDataset.Location = new System.Drawing.Point(33, 260);
            this.btnSelectDataset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectDataset.Name = "btnSelectDataset";
            this.btnSelectDataset.Size = new System.Drawing.Size(173, 30);
            this.btnSelectDataset.TabIndex = 2;
            this.btnSelectDataset.Text = "Select Dataset (.zip)";
            this.btnSelectDataset.UseVisualStyleBackColor = false;
            this.btnSelectDataset.Click += new System.EventHandler(this.btnSelectDataset_Click);
            // 
            // lblZipFile
            // 
            this.lblZipFile.AutoSize = true;
            this.lblZipFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblZipFile.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.lblZipFile.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.lblZipFile.Location = new System.Drawing.Point(132, 222);
            this.lblZipFile.MaximumSize = new System.Drawing.Size(450, 22);
            this.lblZipFile.MinimumSize = new System.Drawing.Size(145, 22);
            this.lblZipFile.Name = "lblZipFile";
            this.lblZipFile.Size = new System.Drawing.Size(145, 22);
            this.lblZipFile.TabIndex = 3;
            this.lblZipFile.Text = "No file selected";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.descBox);
            this.panel1.Controls.Add(this.trainingStatusBox);
            this.panel1.Controls.Add(this.trainingDurationBox);
            this.panel1.Controls.Add(this.instanceTypeBox);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.lblZipFile);
            this.panel1.Controls.Add(this.progressBar);
            this.panel1.Controls.Add(this.btnSelectFolder);
            this.panel1.Controls.Add(this.btnDownloadModel);
            this.panel1.Controls.Add(this.btnUploadToS3);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtDevice);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.instanceTypelbl);
            this.panel1.Controls.Add(this.txtOptimizer);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.label);
            this.panel1.Controls.Add(this.txtWorkers);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.txtPatience);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.txtHyperparameters);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.btnTraining);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtData);
            this.panel1.Controls.Add(this.txtWeights);
            this.panel1.Controls.Add(this.txtEpochs);
            this.panel1.Controls.Add(this.txtBatchSize);
            this.panel1.Controls.Add(this.txtImageSize);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnSelectDataset);
            this.panel1.Location = new System.Drawing.Point(12, 40);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1303, 356);
            this.panel1.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(686, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(355, 29);
            this.label12.TabIndex = 45;
            this.label12.Text = "TRAINING JOB INFORMATION";
            // 
            // instanceTypeBox
            // 
            this.instanceTypeBox.Enabled = false;
            this.instanceTypeBox.Location = new System.Drawing.Point(833, 69);
            this.instanceTypeBox.Name = "instanceTypeBox";
            this.instanceTypeBox.ReadOnly = true;
            this.instanceTypeBox.Size = new System.Drawing.Size(268, 22);
            this.instanceTypeBox.TabIndex = 32;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(232, 260);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(171, 23);
            this.progressBar.TabIndex = 44;
            // 
            // trainingDurationBox
            // 
            this.trainingDurationBox.Enabled = false;
            this.trainingDurationBox.Location = new System.Drawing.Point(833, 100);
            this.trainingDurationBox.Name = "trainingDurationBox";
            this.trainingDurationBox.ReadOnly = true;
            this.trainingDurationBox.Size = new System.Drawing.Size(268, 22);
            this.trainingDurationBox.TabIndex = 31;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSelectFolder.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectFolder.Location = new System.Drawing.Point(33, 294);
            this.btnSelectFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(176, 30);
            this.btnSelectFolder.TabIndex = 43;
            this.btnSelectFolder.Text = "Select Dataset (folder)";
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // trainingStatusBox
            // 
            this.trainingStatusBox.Enabled = false;
            this.trainingStatusBox.Location = new System.Drawing.Point(833, 128);
            this.trainingStatusBox.Name = "trainingStatusBox";
            this.trainingStatusBox.ReadOnly = true;
            this.trainingStatusBox.Size = new System.Drawing.Size(268, 22);
            this.trainingStatusBox.TabIndex = 30;
            // 
            // btnDownloadModel
            // 
            this.btnDownloadModel.BackColor = System.Drawing.Color.Honeydew;
            this.btnDownloadModel.Location = new System.Drawing.Point(437, 295);
            this.btnDownloadModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDownloadModel.Name = "btnDownloadModel";
            this.btnDownloadModel.Size = new System.Drawing.Size(120, 30);
            this.btnDownloadModel.TabIndex = 42;
            this.btnDownloadModel.Text = "Download Model";
            this.btnDownloadModel.UseVisualStyleBackColor = false;
            this.btnDownloadModel.Click += new System.EventHandler(this.btnDownloadModel_Click);
            // 
            // descBox
            // 
            this.descBox.Enabled = false;
            this.descBox.Location = new System.Drawing.Point(833, 156);
            this.descBox.Multiline = true;
            this.descBox.Name = "descBox";
            this.descBox.ReadOnly = true;
            this.descBox.Size = new System.Drawing.Size(268, 40);
            this.descBox.TabIndex = 29;
            // 
            // btnUploadToS3
            // 
            this.btnUploadToS3.BackColor = System.Drawing.Color.Yellow;
            this.btnUploadToS3.Location = new System.Drawing.Point(232, 295);
            this.btnUploadToS3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUploadToS3.Name = "btnUploadToS3";
            this.btnUploadToS3.Size = new System.Drawing.Size(171, 30);
            this.btnUploadToS3.TabIndex = 40;
            this.btnUploadToS3.Text = "Upload Dataset";
            this.btnUploadToS3.UseVisualStyleBackColor = false;
            this.btnUploadToS3.Click += new System.EventHandler(this.btnUploadToS3_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(688, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 16);
            this.label9.TabIndex = 28;
            this.label9.Text = "Description:";
            // 
            // txtDevice
            // 
            this.txtDevice.Location = new System.Drawing.Point(427, 184);
            this.txtDevice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDevice.Name = "txtDevice";
            this.txtDevice.Size = new System.Drawing.Size(145, 22);
            this.txtDevice.TabIndex = 39;
            this.txtDevice.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(688, 128);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 16);
            this.label10.TabIndex = 27;
            this.label10.Text = "Training Status:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(292, 184);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(53, 16);
            this.label24.TabIndex = 38;
            this.label24.Text = "Device:";
            // 
            // instanceTypelbl
            // 
            this.instanceTypelbl.AutoSize = true;
            this.instanceTypelbl.Location = new System.Drawing.Point(792, 73);
            this.instanceTypelbl.Name = "instanceTypelbl";
            this.instanceTypelbl.Size = new System.Drawing.Size(0, 16);
            this.instanceTypelbl.TabIndex = 25;
            // 
            // txtOptimizer
            // 
            this.txtOptimizer.Location = new System.Drawing.Point(427, 156);
            this.txtOptimizer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtOptimizer.Name = "txtOptimizer";
            this.txtOptimizer.Size = new System.Drawing.Size(145, 22);
            this.txtOptimizer.TabIndex = 37;
            this.txtOptimizer.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(688, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 16);
            this.label11.TabIndex = 24;
            this.label11.Text = "Training Duration:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(292, 156);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(66, 16);
            this.label23.TabIndex = 36;
            this.label23.Text = "Optimizer:";
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.BackColor = System.Drawing.SystemColors.Control;
            this.label.Location = new System.Drawing.Point(688, 72);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(101, 16);
            this.label.TabIndex = 23;
            this.label.Text = "Virtual Machine:";
            // 
            // txtWorkers
            // 
            this.txtWorkers.Location = new System.Drawing.Point(427, 128);
            this.txtWorkers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWorkers.Name = "txtWorkers";
            this.txtWorkers.Size = new System.Drawing.Size(145, 22);
            this.txtWorkers.TabIndex = 35;
            this.txtWorkers.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(292, 128);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 16);
            this.label22.TabIndex = 34;
            this.label22.Text = "Workers:";
            // 
            // txtPatience
            // 
            this.txtPatience.Location = new System.Drawing.Point(427, 99);
            this.txtPatience.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPatience.Name = "txtPatience";
            this.txtPatience.Size = new System.Drawing.Size(145, 22);
            this.txtPatience.TabIndex = 33;
            this.txtPatience.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(292, 100);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(63, 16);
            this.label21.TabIndex = 32;
            this.label21.Text = "Patience:";
            // 
            // txtHyperparameters
            // 
            this.txtHyperparameters.Location = new System.Drawing.Point(427, 72);
            this.txtHyperparameters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHyperparameters.Name = "txtHyperparameters";
            this.txtHyperparameters.Size = new System.Drawing.Size(145, 22);
            this.txtHyperparameters.TabIndex = 31;
            this.txtHyperparameters.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(292, 72);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(116, 16);
            this.label20.TabIndex = 30;
            this.label20.Text = "Hyperparameters:";
            // 
            // btnTraining
            // 
            this.btnTraining.BackColor = System.Drawing.Color.Chartreuse;
            this.btnTraining.Location = new System.Drawing.Point(437, 253);
            this.btnTraining.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTraining.Name = "btnTraining";
            this.btnTraining.Size = new System.Drawing.Size(120, 30);
            this.btnTraining.TabIndex = 28;
            this.btnTraining.Text = "Train";
            this.btnTraining.UseVisualStyleBackColor = false;
            this.btnTraining.Click += new System.EventHandler(this.btnTraining_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(21, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(297, 29);
            this.label8.TabIndex = 17;
            this.label8.Text = "TRAINING PARAMETERS";
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(132, 184);
            this.txtData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(145, 22);
            this.txtData.TabIndex = 16;
            this.txtData.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtWeights
            // 
            this.txtWeights.Location = new System.Drawing.Point(132, 156);
            this.txtWeights.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWeights.Name = "txtWeights";
            this.txtWeights.Size = new System.Drawing.Size(145, 22);
            this.txtWeights.TabIndex = 15;
            this.txtWeights.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtEpochs
            // 
            this.txtEpochs.Location = new System.Drawing.Point(132, 128);
            this.txtEpochs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEpochs.Name = "txtEpochs";
            this.txtEpochs.Size = new System.Drawing.Size(145, 22);
            this.txtEpochs.TabIndex = 14;
            this.txtEpochs.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.Location = new System.Drawing.Point(132, 99);
            this.txtBatchSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(145, 22);
            this.txtBatchSize.TabIndex = 13;
            this.txtBatchSize.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtImageSize
            // 
            this.txtImageSize.Location = new System.Drawing.Point(132, 72);
            this.txtImageSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtImageSize.Name = "txtImageSize";
            this.txtImageSize.Size = new System.Drawing.Size(145, 22);
            this.txtImageSize.TabIndex = 12;
            this.txtImageSize.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 180);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 16);
            this.label7.TabIndex = 11;
            this.label7.Text = "Data:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Weights:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Epochs:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Batch Size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Image Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 16);
            this.label2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 222);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Selected File:";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.HighlightText;
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logBox.Location = new System.Drawing.Point(26, 18);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(1262, 305);
            this.logBox.TabIndex = 26;
            this.logBox.WordWrap = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.logBox);
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(12, 417);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1303, 340);
            this.panel2.TabIndex = 33;
            this.panel2.Visible = false;
            // 
            // SpaceBetween
            // 
            this.SpaceBetween.Location = new System.Drawing.Point(11, 401);
            this.SpaceBetween.Name = "SpaceBetween";
            this.SpaceBetween.Size = new System.Drawing.Size(1304, 10);
            this.SpaceBetween.TabIndex = 34;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1327, 763);
            this.Controls.Add(this.SpaceBetween);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1092, 384);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtWeights;
        private System.Windows.Forms.TextBox txtEpochs;
        private System.Windows.Forms.TextBox txtBatchSize;
        private System.Windows.Forms.TextBox txtImageSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnTraining;
        private System.Windows.Forms.TextBox txtDevice;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtOptimizer;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtWorkers;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtPatience;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtHyperparameters;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnUploadToS3;
        private System.Windows.Forms.Button btnDownloadModel;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem newTrainingJobToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Label instanceTypelbl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label instanceTypeBox;
        private System.Windows.Forms.Label descBox;
        private System.Windows.Forms.Label trainingStatusBox;
        private System.Windows.Forms.Label trainingDurationBox;
        private System.Windows.Forms.Panel SpaceBetween;
    }
}

