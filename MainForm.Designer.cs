﻿
namespace AutomatedAWSModelCreator
{
    partial class MainForm
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
            this.appMenuStrip = new System.Windows.Forms.MenuStrip();
            this.connectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.createConnectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.testConnectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeConnectionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newTrainingJobMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnSelectDataset = new System.Windows.Forms.Button();
            this.lblZipFile = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.txtGpuCount = new System.Windows.Forms.TextBox();
            this.datasetListComboBox = new System.Windows.Forms.ComboBox();
            this.btnFetchDatasets = new System.Windows.Forms.Button();
            this.txtInstanceCount = new System.Windows.Forms.TextBox();
            this.instanceCountLabel = new System.Windows.Forms.Label();
            this.instancesDropdown = new System.Windows.Forms.ComboBox();
            this.hyperparametersLabel = new System.Windows.Forms.Label();
            this.descBox = new System.Windows.Forms.Label();
            this.trainingStatusBox = new System.Windows.Forms.Label();
            this.trainingDurationBox = new System.Windows.Forms.Label();
            this.instanceTypeBox = new System.Windows.Forms.Label();
            this.trainingJobInformationLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnDownloadModel = new System.Windows.Forms.Button();
            this.btnUploadToS3 = new System.Windows.Forms.Button();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.trainingStatusLabel = new System.Windows.Forms.Label();
            this.gpuCountLabel = new System.Windows.Forms.Label();
            this.instanceTypelbl = new System.Windows.Forms.Label();
            this.txtOptimizer = new System.Windows.Forms.TextBox();
            this.trainingDurationLabel = new System.Windows.Forms.Label();
            this.optimizerLabel = new System.Windows.Forms.Label();
            this.virtualMachineLabel = new System.Windows.Forms.Label();
            this.txtWorkers = new System.Windows.Forms.TextBox();
            this.workersLabel = new System.Windows.Forms.Label();
            this.txtPatience = new System.Windows.Forms.TextBox();
            this.patienceLabel = new System.Windows.Forms.Label();
            this.btnTraining = new System.Windows.Forms.Button();
            this.trainingParametersLabel = new System.Windows.Forms.Label();
            this.txtWeights = new System.Windows.Forms.TextBox();
            this.txtEpochs = new System.Windows.Forms.TextBox();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.weightsLabel = new System.Windows.Forms.Label();
            this.epochsLabel = new System.Windows.Forms.Label();
            this.batchSizeLabel = new System.Windows.Forms.Label();
            this.imageSizeLabel = new System.Windows.Forms.Label();
            this.selectedFileLabel = new System.Windows.Forms.Label();
            this.imgSizeDropdown = new System.Windows.Forms.ComboBox();
            this.hyperparamsDropdown = new System.Windows.Forms.ComboBox();
            this.outputListComboBox = new System.Windows.Forms.ComboBox();
            this.btnFetchOutput = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.logPanel = new System.Windows.Forms.Panel();
            this.SpaceBetween = new System.Windows.Forms.Panel();
            this.appMenuStrip.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.logPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // appMenuStrip
            // 
            this.appMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.appMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionMenu,
            this.helpMenu,
            this.newTrainingJobMenu});
            this.appMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.appMenuStrip.Name = "appMenuStrip";
            this.appMenuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.appMenuStrip.Size = new System.Drawing.Size(1164, 28);
            this.appMenuStrip.TabIndex = 1;
            // 
            // connectionMenu
            // 
            this.connectionMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createConnectionMenu,
            this.testConnectionMenu,
            this.closeConnectionMenu});
            this.connectionMenu.Name = "connectionMenu";
            this.connectionMenu.Size = new System.Drawing.Size(98, 24);
            this.connectionMenu.Text = "Connection";
            // 
            // createConnectionMenu
            // 
            this.createConnectionMenu.Name = "createConnectionMenu";
            this.createConnectionMenu.Size = new System.Drawing.Size(214, 26);
            this.createConnectionMenu.Text = "Create Connection";
            this.createConnectionMenu.Click += new System.EventHandler(this.createConnectionMenu_Click);
            // 
            // testConnectionMenu
            // 
            this.testConnectionMenu.Name = "testConnectionMenu";
            this.testConnectionMenu.Size = new System.Drawing.Size(214, 26);
            this.testConnectionMenu.Text = "Test Connection";
            this.testConnectionMenu.Click += new System.EventHandler(this.testConnectionMenu_Click);
            // 
            // closeConnectionMenu
            // 
            this.closeConnectionMenu.Name = "closeConnectionMenu";
            this.closeConnectionMenu.Size = new System.Drawing.Size(214, 26);
            this.closeConnectionMenu.Text = "Close Connection";
            this.closeConnectionMenu.Click += new System.EventHandler(this.closeConnectionMenu_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(55, 24);
            this.helpMenu.Text = "Help";
            this.helpMenu.Click += new System.EventHandler(this.HelpMenu_Click);
            // 
            // newTrainingJobMenu
            // 
            this.newTrainingJobMenu.Name = "newTrainingJobMenu";
            this.newTrainingJobMenu.Size = new System.Drawing.Size(137, 24);
            this.newTrainingJobMenu.Text = "New Training Job";
            this.newTrainingJobMenu.Click += new System.EventHandler(this.NewTrainingJobMenu_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // btnSelectDataset
            // 
            this.btnSelectDataset.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSelectDataset.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectDataset.Location = new System.Drawing.Point(30, 307);
            this.btnSelectDataset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectDataset.Name = "btnSelectDataset";
            this.btnSelectDataset.Size = new System.Drawing.Size(173, 30);
            this.btnSelectDataset.TabIndex = 11;
            this.btnSelectDataset.Text = "Select Dataset (.zip)";
            this.btnSelectDataset.UseVisualStyleBackColor = false;
            this.btnSelectDataset.Click += new System.EventHandler(this.btnSelectZip_Click);
            // 
            // lblZipFile
            // 
            this.lblZipFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblZipFile.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.lblZipFile.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.lblZipFile.Location = new System.Drawing.Point(132, 222);
            this.lblZipFile.MaximumSize = new System.Drawing.Size(450, 22);
            this.lblZipFile.MinimumSize = new System.Drawing.Size(145, 22);
            this.lblZipFile.Name = "lblZipFile";
            this.lblZipFile.Size = new System.Drawing.Size(145, 22);
            this.lblZipFile.TabIndex = 3;
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add(this.downloadProgressBar);
            this.mainPanel.Controls.Add(this.txtGpuCount);
            this.mainPanel.Controls.Add(this.datasetListComboBox);
            this.mainPanel.Controls.Add(this.btnFetchDatasets);
            this.mainPanel.Controls.Add(this.txtInstanceCount);
            this.mainPanel.Controls.Add(this.instanceCountLabel);
            this.mainPanel.Controls.Add(this.instancesDropdown);
            this.mainPanel.Controls.Add(this.hyperparametersLabel);
            this.mainPanel.Controls.Add(this.descBox);
            this.mainPanel.Controls.Add(this.trainingStatusBox);
            this.mainPanel.Controls.Add(this.trainingDurationBox);
            this.mainPanel.Controls.Add(this.instanceTypeBox);
            this.mainPanel.Controls.Add(this.trainingJobInformationLabel);
            this.mainPanel.Controls.Add(this.lblZipFile);
            this.mainPanel.Controls.Add(this.progressBar);
            this.mainPanel.Controls.Add(this.btnSelectFolder);
            this.mainPanel.Controls.Add(this.btnDownloadModel);
            this.mainPanel.Controls.Add(this.btnUploadToS3);
            this.mainPanel.Controls.Add(this.descriptionLabel);
            this.mainPanel.Controls.Add(this.trainingStatusLabel);
            this.mainPanel.Controls.Add(this.gpuCountLabel);
            this.mainPanel.Controls.Add(this.instanceTypelbl);
            this.mainPanel.Controls.Add(this.txtOptimizer);
            this.mainPanel.Controls.Add(this.trainingDurationLabel);
            this.mainPanel.Controls.Add(this.optimizerLabel);
            this.mainPanel.Controls.Add(this.virtualMachineLabel);
            this.mainPanel.Controls.Add(this.txtWorkers);
            this.mainPanel.Controls.Add(this.workersLabel);
            this.mainPanel.Controls.Add(this.txtPatience);
            this.mainPanel.Controls.Add(this.patienceLabel);
            this.mainPanel.Controls.Add(this.btnTraining);
            this.mainPanel.Controls.Add(this.trainingParametersLabel);
            this.mainPanel.Controls.Add(this.txtWeights);
            this.mainPanel.Controls.Add(this.txtEpochs);
            this.mainPanel.Controls.Add(this.txtBatchSize);
            this.mainPanel.Controls.Add(this.weightsLabel);
            this.mainPanel.Controls.Add(this.epochsLabel);
            this.mainPanel.Controls.Add(this.batchSizeLabel);
            this.mainPanel.Controls.Add(this.imageSizeLabel);
            this.mainPanel.Controls.Add(this.selectedFileLabel);
            this.mainPanel.Controls.Add(this.btnSelectDataset);
            this.mainPanel.Controls.Add(this.imgSizeDropdown);
            this.mainPanel.Controls.Add(this.hyperparamsDropdown);
            this.mainPanel.Controls.Add(this.outputListComboBox);
            this.mainPanel.Controls.Add(this.btnFetchOutput);
            this.mainPanel.Location = new System.Drawing.Point(12, 42);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1140, 397);
            this.mainPanel.TabIndex = 4;
            // 
            // downloadProgressBar
            // 
            this.downloadProgressBar.Location = new System.Drawing.Point(940, 346);
            this.downloadProgressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(161, 23);
            this.downloadProgressBar.TabIndex = 55;
            // 
            // txtGpuCount
            // 
            this.txtGpuCount.Enabled = false;
            this.txtGpuCount.Location = new System.Drawing.Point(427, 184);
            this.txtGpuCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGpuCount.Name = "txtGpuCount";
            this.txtGpuCount.ReadOnly = true;
            this.txtGpuCount.Size = new System.Drawing.Size(145, 22);
            this.txtGpuCount.TabIndex = 53;
            this.txtGpuCount.TextChanged += new System.EventHandler(this.txtGpuCount_ValueChanged);
            // 
            // datasetListComboBox
            // 
            this.datasetListComboBox.Enabled = false;
            this.datasetListComboBox.FormattingEnabled = true;
            this.datasetListComboBox.Location = new System.Drawing.Point(353, 266);
            this.datasetListComboBox.Name = "datasetListComboBox";
            this.datasetListComboBox.Size = new System.Drawing.Size(279, 24);
            this.datasetListComboBox.TabIndex = 54;
            this.datasetListComboBox.SelectedValueChanged += new System.EventHandler(this.datasetListComboBox_SelectedValueChanged);
            // 
            // btnFetchDatasets
            // 
            this.btnFetchDatasets.Location = new System.Drawing.Point(229, 264);
            this.btnFetchDatasets.Name = "btnFetchDatasets";
            this.btnFetchDatasets.Size = new System.Drawing.Size(118, 30);
            this.btnFetchDatasets.TabIndex = 53;
            this.btnFetchDatasets.Text = "Fetch Dataset";
            this.btnFetchDatasets.UseVisualStyleBackColor = true;
            this.btnFetchDatasets.Click += new System.EventHandler(this.btnFetchDatasets_Click);
            // 
            // txtInstanceCount
            // 
            this.txtInstanceCount.Location = new System.Drawing.Point(427, 222);
            this.txtInstanceCount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtInstanceCount.Name = "txtInstanceCount";
            this.txtInstanceCount.Size = new System.Drawing.Size(145, 22);
            this.txtInstanceCount.TabIndex = 51;
            this.txtInstanceCount.TextChanged += new System.EventHandler(this.txtInstanceCount_ValueChanged);
            // 
            // instanceCountLabel
            // 
            this.instanceCountLabel.AutoSize = true;
            this.instanceCountLabel.Location = new System.Drawing.Point(292, 222);
            this.instanceCountLabel.Name = "instanceCountLabel";
            this.instanceCountLabel.Size = new System.Drawing.Size(97, 16);
            this.instanceCountLabel.TabIndex = 52;
            this.instanceCountLabel.Text = "Instance Count:";
            // 
            // instancesDropdown
            // 
            this.instancesDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.instancesDropdown.FormattingEnabled = true;
            this.instancesDropdown.Items.AddRange(new object[] {
            "ml.g4dn.xlarge"});
            this.instancesDropdown.Location = new System.Drawing.Point(452, 304);
            this.instancesDropdown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.instancesDropdown.Name = "instancesDropdown";
            this.instancesDropdown.Size = new System.Drawing.Size(180, 24);
            this.instancesDropdown.TabIndex = 14;
            this.instancesDropdown.SelectedValueChanged += new System.EventHandler(this.instancesDropdown_SelectedValueChanged);
            // 
            // hyperparametersLabel
            // 
            this.hyperparametersLabel.AutoSize = true;
            this.hyperparametersLabel.Location = new System.Drawing.Point(292, 72);
            this.hyperparametersLabel.Name = "hyperparametersLabel";
            this.hyperparametersLabel.Size = new System.Drawing.Size(116, 16);
            this.hyperparametersLabel.TabIndex = 50;
            this.hyperparametersLabel.Text = "Hyperparameters:";
            // 
            // descBox
            // 
            this.descBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.descBox.Cursor = System.Windows.Forms.Cursors.No;
            this.descBox.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.descBox.Location = new System.Drawing.Point(833, 155);
            this.descBox.MaximumSize = new System.Drawing.Size(268, 40);
            this.descBox.MinimumSize = new System.Drawing.Size(268, 40);
            this.descBox.Name = "descBox";
            this.descBox.Size = new System.Drawing.Size(268, 40);
            this.descBox.TabIndex = 49;
            // 
            // trainingStatusBox
            // 
            this.trainingStatusBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.trainingStatusBox.Cursor = System.Windows.Forms.Cursors.No;
            this.trainingStatusBox.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.trainingStatusBox.Location = new System.Drawing.Point(833, 125);
            this.trainingStatusBox.MaximumSize = new System.Drawing.Size(268, 22);
            this.trainingStatusBox.MinimumSize = new System.Drawing.Size(268, 22);
            this.trainingStatusBox.Name = "trainingStatusBox";
            this.trainingStatusBox.Size = new System.Drawing.Size(268, 22);
            this.trainingStatusBox.TabIndex = 48;
            // 
            // trainingDurationBox
            // 
            this.trainingDurationBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.trainingDurationBox.Cursor = System.Windows.Forms.Cursors.No;
            this.trainingDurationBox.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.trainingDurationBox.Location = new System.Drawing.Point(833, 98);
            this.trainingDurationBox.MaximumSize = new System.Drawing.Size(268, 22);
            this.trainingDurationBox.MinimumSize = new System.Drawing.Size(268, 22);
            this.trainingDurationBox.Name = "trainingDurationBox";
            this.trainingDurationBox.Size = new System.Drawing.Size(268, 22);
            this.trainingDurationBox.TabIndex = 47;
            // 
            // instanceTypeBox
            // 
            this.instanceTypeBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.instanceTypeBox.Cursor = System.Windows.Forms.Cursors.No;
            this.instanceTypeBox.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.instanceTypeBox.Location = new System.Drawing.Point(833, 71);
            this.instanceTypeBox.MaximumSize = new System.Drawing.Size(268, 22);
            this.instanceTypeBox.MinimumSize = new System.Drawing.Size(268, 22);
            this.instanceTypeBox.Name = "instanceTypeBox";
            this.instanceTypeBox.Size = new System.Drawing.Size(268, 22);
            this.instanceTypeBox.TabIndex = 46;
            // 
            // trainingJobInformationLabel
            // 
            this.trainingJobInformationLabel.AutoSize = true;
            this.trainingJobInformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainingJobInformationLabel.Location = new System.Drawing.Point(686, 12);
            this.trainingJobInformationLabel.Name = "trainingJobInformationLabel";
            this.trainingJobInformationLabel.Size = new System.Drawing.Size(355, 29);
            this.trainingJobInformationLabel.TabIndex = 45;
            this.trainingJobInformationLabel.Text = "TRAINING JOB INFORMATION";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(229, 307);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(171, 23);
            this.progressBar.TabIndex = 44;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSelectFolder.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectFolder.Location = new System.Drawing.Point(30, 341);
            this.btnSelectFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(176, 30);
            this.btnSelectFolder.TabIndex = 12;
            this.btnSelectFolder.Text = "Select Dataset (folder)";
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnDownloadModel
            // 
            this.btnDownloadModel.BackColor = System.Drawing.Color.Honeydew;
            this.btnDownloadModel.Location = new System.Drawing.Point(814, 342);
            this.btnDownloadModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDownloadModel.Name = "btnDownloadModel";
            this.btnDownloadModel.Size = new System.Drawing.Size(120, 30);
            this.btnDownloadModel.TabIndex = 18;
            this.btnDownloadModel.Text = "Download Model";
            this.btnDownloadModel.UseVisualStyleBackColor = false;
            this.btnDownloadModel.Click += new System.EventHandler(this.btnDownloadModel_Click);
            // 
            // btnUploadToS3
            // 
            this.btnUploadToS3.BackColor = System.Drawing.Color.Yellow;
            this.btnUploadToS3.Location = new System.Drawing.Point(229, 342);
            this.btnUploadToS3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUploadToS3.Name = "btnUploadToS3";
            this.btnUploadToS3.Size = new System.Drawing.Size(171, 30);
            this.btnUploadToS3.TabIndex = 13;
            this.btnUploadToS3.Text = "Upload Dataset";
            this.btnUploadToS3.UseVisualStyleBackColor = false;
            this.btnUploadToS3.Click += new System.EventHandler(this.btnUploadToS3_Click);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.BackColor = System.Drawing.SystemColors.Control;
            this.descriptionLabel.Location = new System.Drawing.Point(688, 156);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(78, 16);
            this.descriptionLabel.TabIndex = 28;
            this.descriptionLabel.Text = "Description:";
            // 
            // trainingStatusLabel
            // 
            this.trainingStatusLabel.AutoSize = true;
            this.trainingStatusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.trainingStatusLabel.Location = new System.Drawing.Point(688, 128);
            this.trainingStatusLabel.Name = "trainingStatusLabel";
            this.trainingStatusLabel.Size = new System.Drawing.Size(99, 16);
            this.trainingStatusLabel.TabIndex = 27;
            this.trainingStatusLabel.Text = "Training Status:";
            // 
            // gpuCountLabel
            // 
            this.gpuCountLabel.AutoSize = true;
            this.gpuCountLabel.Location = new System.Drawing.Point(292, 184);
            this.gpuCountLabel.Name = "gpuCountLabel";
            this.gpuCountLabel.Size = new System.Drawing.Size(76, 16);
            this.gpuCountLabel.TabIndex = 38;
            this.gpuCountLabel.Text = "GPU Count:";
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
            this.txtOptimizer.TabIndex = 9;
            this.txtOptimizer.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // trainingDurationLabel
            // 
            this.trainingDurationLabel.AutoSize = true;
            this.trainingDurationLabel.BackColor = System.Drawing.SystemColors.Control;
            this.trainingDurationLabel.Location = new System.Drawing.Point(688, 99);
            this.trainingDurationLabel.Name = "trainingDurationLabel";
            this.trainingDurationLabel.Size = new System.Drawing.Size(112, 16);
            this.trainingDurationLabel.TabIndex = 24;
            this.trainingDurationLabel.Text = "Training Duration:";
            // 
            // optimizerLabel
            // 
            this.optimizerLabel.AutoSize = true;
            this.optimizerLabel.Location = new System.Drawing.Point(292, 156);
            this.optimizerLabel.Name = "optimizerLabel";
            this.optimizerLabel.Size = new System.Drawing.Size(66, 16);
            this.optimizerLabel.TabIndex = 36;
            this.optimizerLabel.Text = "Optimizer:";
            // 
            // virtualMachineLabel
            // 
            this.virtualMachineLabel.AutoSize = true;
            this.virtualMachineLabel.BackColor = System.Drawing.SystemColors.Control;
            this.virtualMachineLabel.Location = new System.Drawing.Point(688, 72);
            this.virtualMachineLabel.Name = "virtualMachineLabel";
            this.virtualMachineLabel.Size = new System.Drawing.Size(101, 16);
            this.virtualMachineLabel.TabIndex = 23;
            this.virtualMachineLabel.Text = "Virtual Machine:";
            // 
            // txtWorkers
            // 
            this.txtWorkers.Location = new System.Drawing.Point(427, 128);
            this.txtWorkers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWorkers.Name = "txtWorkers";
            this.txtWorkers.Size = new System.Drawing.Size(145, 22);
            this.txtWorkers.TabIndex = 8;
            this.txtWorkers.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // workersLabel
            // 
            this.workersLabel.AutoSize = true;
            this.workersLabel.Location = new System.Drawing.Point(292, 128);
            this.workersLabel.Name = "workersLabel";
            this.workersLabel.Size = new System.Drawing.Size(61, 16);
            this.workersLabel.TabIndex = 34;
            this.workersLabel.Text = "Workers:";
            // 
            // txtPatience
            // 
            this.txtPatience.Location = new System.Drawing.Point(427, 99);
            this.txtPatience.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPatience.Name = "txtPatience";
            this.txtPatience.Size = new System.Drawing.Size(145, 22);
            this.txtPatience.TabIndex = 7;
            this.txtPatience.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // patienceLabel
            // 
            this.patienceLabel.AutoSize = true;
            this.patienceLabel.Location = new System.Drawing.Point(292, 100);
            this.patienceLabel.Name = "patienceLabel";
            this.patienceLabel.Size = new System.Drawing.Size(63, 16);
            this.patienceLabel.TabIndex = 32;
            this.patienceLabel.Text = "Patience:";
            // 
            // btnTraining
            // 
            this.btnTraining.BackColor = System.Drawing.Color.Chartreuse;
            this.btnTraining.Location = new System.Drawing.Point(486, 341);
            this.btnTraining.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTraining.Name = "btnTraining";
            this.btnTraining.Size = new System.Drawing.Size(120, 30);
            this.btnTraining.TabIndex = 15;
            this.btnTraining.Text = "Train";
            this.btnTraining.UseVisualStyleBackColor = false;
            this.btnTraining.Click += new System.EventHandler(this.btnTraining_Click);
            // 
            // trainingParametersLabel
            // 
            this.trainingParametersLabel.AutoSize = true;
            this.trainingParametersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainingParametersLabel.Location = new System.Drawing.Point(21, 12);
            this.trainingParametersLabel.Name = "trainingParametersLabel";
            this.trainingParametersLabel.Size = new System.Drawing.Size(297, 29);
            this.trainingParametersLabel.TabIndex = 17;
            this.trainingParametersLabel.Text = "TRAINING PARAMETERS";
            // 
            // txtWeights
            // 
            this.txtWeights.Location = new System.Drawing.Point(132, 156);
            this.txtWeights.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWeights.Name = "txtWeights";
            this.txtWeights.Size = new System.Drawing.Size(145, 22);
            this.txtWeights.TabIndex = 4;
            this.txtWeights.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtEpochs
            // 
            this.txtEpochs.Location = new System.Drawing.Point(132, 128);
            this.txtEpochs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEpochs.Name = "txtEpochs";
            this.txtEpochs.Size = new System.Drawing.Size(145, 22);
            this.txtEpochs.TabIndex = 3;
            this.txtEpochs.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.Location = new System.Drawing.Point(132, 99);
            this.txtBatchSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(145, 22);
            this.txtBatchSize.TabIndex = 2;
            this.txtBatchSize.Click += new System.EventHandler(this.SelectAllTextOnClick);
            // 
            // weightsLabel
            // 
            this.weightsLabel.AutoSize = true;
            this.weightsLabel.Location = new System.Drawing.Point(30, 153);
            this.weightsLabel.Name = "weightsLabel";
            this.weightsLabel.Size = new System.Drawing.Size(59, 16);
            this.weightsLabel.TabIndex = 10;
            this.weightsLabel.Text = "Weights:";
            // 
            // epochsLabel
            // 
            this.epochsLabel.AutoSize = true;
            this.epochsLabel.Location = new System.Drawing.Point(30, 126);
            this.epochsLabel.Name = "epochsLabel";
            this.epochsLabel.Size = new System.Drawing.Size(56, 16);
            this.epochsLabel.TabIndex = 9;
            this.epochsLabel.Text = "Epochs:";
            // 
            // batchSizeLabel
            // 
            this.batchSizeLabel.AutoSize = true;
            this.batchSizeLabel.Location = new System.Drawing.Point(30, 99);
            this.batchSizeLabel.Name = "batchSizeLabel";
            this.batchSizeLabel.Size = new System.Drawing.Size(73, 16);
            this.batchSizeLabel.TabIndex = 8;
            this.batchSizeLabel.Text = "Batch Size:";
            // 
            // imageSizeLabel
            // 
            this.imageSizeLabel.AutoSize = true;
            this.imageSizeLabel.Location = new System.Drawing.Point(30, 72);
            this.imageSizeLabel.Name = "imageSizeLabel";
            this.imageSizeLabel.Size = new System.Drawing.Size(77, 16);
            this.imageSizeLabel.TabIndex = 7;
            this.imageSizeLabel.Text = "Image Size:";
            // 
            // selectedFileLabel
            // 
            this.selectedFileLabel.AutoSize = true;
            this.selectedFileLabel.Location = new System.Drawing.Point(30, 222);
            this.selectedFileLabel.Name = "selectedFileLabel";
            this.selectedFileLabel.Size = new System.Drawing.Size(89, 16);
            this.selectedFileLabel.TabIndex = 4;
            this.selectedFileLabel.Text = "Selected File:";
            // 
            // imgSizeDropdown
            // 
            this.imgSizeDropdown.FormattingEnabled = true;
            this.imgSizeDropdown.Items.AddRange(new object[] {
            "1280",
            "640"});
            this.imgSizeDropdown.Location = new System.Drawing.Point(132, 72);
            this.imgSizeDropdown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.imgSizeDropdown.Name = "imgSizeDropdown";
            this.imgSizeDropdown.Size = new System.Drawing.Size(145, 24);
            this.imgSizeDropdown.TabIndex = 1;
            this.imgSizeDropdown.SelectionChangeCommitted += new System.EventHandler(this.ImgSizeDropdown_SelectionChangeCommitted);
            // 
            // hyperparamsDropdown
            // 
            this.hyperparamsDropdown.FormattingEnabled = true;
            this.hyperparamsDropdown.Items.AddRange(new object[] {
            "Custom",
            "hyp.no-augmentation.yaml",
            "hyp.Objects365.yaml",
            "hyp.scratch-low.yaml",
            "hyp.scratch-med.yaml",
            "hyp.scratch-high.yaml",
            "hyp.VOC.yaml"});
            this.hyperparamsDropdown.Location = new System.Drawing.Point(427, 72);
            this.hyperparamsDropdown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.hyperparamsDropdown.Name = "hyperparamsDropdown";
            this.hyperparamsDropdown.Size = new System.Drawing.Size(145, 24);
            this.hyperparamsDropdown.TabIndex = 6;
            this.hyperparamsDropdown.SelectedValueChanged += new System.EventHandler(this.HyperparamsDropdown_SelectedValueChanged);
            // 
            // outputListComboBox
            // 
            this.outputListComboBox.Enabled = false;
            this.outputListComboBox.FormattingEnabled = true;
            this.outputListComboBox.Location = new System.Drawing.Point(688, 304);
            this.outputListComboBox.Name = "outputListComboBox";
            this.outputListComboBox.Size = new System.Drawing.Size(410, 24);
            this.outputListComboBox.TabIndex = 16;
            this.outputListComboBox.SelectedValueChanged += new System.EventHandler(this.outputListComboBox_SelectedValueChanged);
            // 
            // btnFetchOutput
            // 
            this.btnFetchOutput.Location = new System.Drawing.Point(688, 342);
            this.btnFetchOutput.Name = "btnFetchOutput";
            this.btnFetchOutput.Size = new System.Drawing.Size(120, 30);
            this.btnFetchOutput.TabIndex = 17;
            this.btnFetchOutput.Text = "Fetch Output";
            this.btnFetchOutput.UseVisualStyleBackColor = true;
            this.btnFetchOutput.Click += new System.EventHandler(this.btnFetchOutput_Click);
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.HighlightText;
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logBox.Location = new System.Drawing.Point(26, 26);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(1088, 297);
            this.logBox.TabIndex = 26;
            this.logBox.Text = "";
            this.logBox.WordWrap = false;
            // 
            // logPanel
            // 
            this.logPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.logPanel.BackColor = System.Drawing.SystemColors.Control;
            this.logPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logPanel.Controls.Add(this.logBox);
            this.logPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logPanel.Location = new System.Drawing.Point(12, 460);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(1140, 340);
            this.logPanel.TabIndex = 33;
            this.logPanel.Visible = false;
            // 
            // SpaceBetween
            // 
            this.SpaceBetween.Location = new System.Drawing.Point(12, 444);
            this.SpaceBetween.Name = "SpaceBetween";
            this.SpaceBetween.Size = new System.Drawing.Size(1140, 10);
            this.SpaceBetween.TabIndex = 34;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1164, 835);
            this.Controls.Add(this.SpaceBetween);
            this.Controls.Add(this.appMenuStrip);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.logPanel);
            this.MainMenuStrip = this.appMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1182, 400);
            this.Name = "MainForm";
            this.Text = "Automated AWS Model Creator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.appMenuStrip.ResumeLayout(false);
            this.appMenuStrip.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.logPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip appMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem connectionMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button btnSelectDataset;
        private System.Windows.Forms.Label lblZipFile;
        private System.Windows.Forms.Label selectedFileLabel;
        private System.Windows.Forms.Label imageSizeLabel;
        private System.Windows.Forms.Label batchSizeLabel;
        private System.Windows.Forms.Label epochsLabel;
        private System.Windows.Forms.Label weightsLabel;
        private System.Windows.Forms.TextBox txtBatchSize;
        private System.Windows.Forms.TextBox txtEpochs;
        private System.Windows.Forms.TextBox txtWeights;
        private System.Windows.Forms.Label trainingParametersLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label trainingStatusLabel;
        private System.Windows.Forms.Label trainingDurationLabel;
        private System.Windows.Forms.Label trainingJobInformationLabel;
        private System.Windows.Forms.Button btnTraining;
        private System.Windows.Forms.Label patienceLabel;
        private System.Windows.Forms.TextBox txtPatience;
        private System.Windows.Forms.Label workersLabel;
        private System.Windows.Forms.TextBox txtWorkers;
        private System.Windows.Forms.Label optimizerLabel;
        private System.Windows.Forms.TextBox txtOptimizer;
        private System.Windows.Forms.Label gpuCountLabel;
        private System.Windows.Forms.Button btnUploadToS3;
        private System.Windows.Forms.Button btnDownloadModel;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnFetchOutput;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox imgSizeDropdown;
        private System.Windows.Forms.ComboBox hyperparamsDropdown;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolStripMenuItem createConnectionMenu;
        private System.Windows.Forms.ToolStripMenuItem closeConnectionMenu;
        private System.Windows.Forms.ToolStripMenuItem testConnectionMenu;
        private System.Windows.Forms.ComboBox outputListComboBox;
        private System.Windows.Forms.ToolStripMenuItem newTrainingJobMenu;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Label instanceTypelbl;
        private System.Windows.Forms.Label virtualMachineLabel;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.Label instanceTypeBox;
        private System.Windows.Forms.Label descBox;
        private System.Windows.Forms.Label trainingStatusBox;
        private System.Windows.Forms.Label trainingDurationBox;
        private System.Windows.Forms.Panel SpaceBetween;
        private System.Windows.Forms.Label hyperparametersLabel;
        private System.Windows.Forms.ComboBox instancesDropdown;
        private System.Windows.Forms.TextBox txtInstanceCount;
        private System.Windows.Forms.Label instanceCountLabel;
        private System.Windows.Forms.TextBox txtGpuCount;
        private System.Windows.Forms.ComboBox datasetListComboBox;
        private System.Windows.Forms.Button btnFetchDatasets;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
    }
}

