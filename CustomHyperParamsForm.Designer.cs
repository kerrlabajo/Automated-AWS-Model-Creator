
namespace AutomatedAWSModelCreator
{
    partial class CustomHyperParamsForm
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
            this.CancelButton = new System.Windows.Forms.Button();
            this.CreateFile = new System.Windows.Forms.Button();
            this.copy_paste = new System.Windows.Forms.TextBox();
            this.hsv_h = new System.Windows.Forms.TextBox();
            this.hsv_s = new System.Windows.Forms.TextBox();
            this.hsv_v = new System.Windows.Forms.TextBox();
            this.translate = new System.Windows.Forms.TextBox();
            this.degrees = new System.Windows.Forms.TextBox();
            this.scale = new System.Windows.Forms.TextBox();
            this.shear = new System.Windows.Forms.TextBox();
            this.perspective = new System.Windows.Forms.TextBox();
            this.flipud = new System.Windows.Forms.TextBox();
            this.fliplr = new System.Windows.Forms.TextBox();
            this.mosaic = new System.Windows.Forms.TextBox();
            this.mixup = new System.Windows.Forms.TextBox();
            this.fl_gamma = new System.Windows.Forms.TextBox();
            this.imageMixupLabel = new System.Windows.Forms.Label();
            this.segmentCopyPasteLabel = new System.Windows.Forms.Label();
            this.imageMosaicLabel = new System.Windows.Forms.Label();
            this.imageFlipLeftRightLabel = new System.Windows.Forms.Label();
            this.imageFlipUpDownLabel = new System.Windows.Forms.Label();
            this.imagePerspectiveLabel = new System.Windows.Forms.Label();
            this.imageShearLabel = new System.Windows.Forms.Label();
            this.imageScaleLabel = new System.Windows.Forms.Label();
            this.imageTranslationLabel = new System.Windows.Forms.Label();
            this.imageRotationLabel = new System.Windows.Forms.Label();
            this.hsvValueAugLabel = new System.Windows.Forms.Label();
            this.hsvSaturationAugLabel = new System.Windows.Forms.Label();
            this.hsvHueAugLabel = new System.Windows.Forms.Label();
            this.focalLossGammaLabel = new System.Windows.Forms.Label();
            this.anchors = new System.Windows.Forms.TextBox();
            this.anchor_t = new System.Windows.Forms.TextBox();
            this.lrf = new System.Windows.Forms.TextBox();
            this.momentum = new System.Windows.Forms.TextBox();
            this.weight_decay = new System.Windows.Forms.TextBox();
            this.warmup_momentum = new System.Windows.Forms.TextBox();
            this.warmup_epochs = new System.Windows.Forms.TextBox();
            this.warmup_bias_lr = new System.Windows.Forms.TextBox();
            this.box = new System.Windows.Forms.TextBox();
            this.cls = new System.Windows.Forms.TextBox();
            this.cls_pw = new System.Windows.Forms.TextBox();
            this.obj = new System.Windows.Forms.TextBox();
            this.obj_pw = new System.Windows.Forms.TextBox();
            this.iou_t = new System.Windows.Forms.TextBox();
            this.lr0 = new System.Windows.Forms.TextBox();
            this.iouTrainingThresholdLabel = new System.Windows.Forms.Label();
            this.anchorMultipleThresholdLabel = new System.Windows.Forms.Label();
            this.anchorsPerOutputLayerLabel = new System.Windows.Forms.Label();
            this.objectPositiveWeightLabel = new System.Windows.Forms.Label();
            this.objectLossGainLabel = new System.Windows.Forms.Label();
            this.clsPositiveWeightLabel = new System.Windows.Forms.Label();
            this.clsLossGainLabel = new System.Windows.Forms.Label();
            this.boxLossGainLabel = new System.Windows.Forms.Label();
            this.warmupBiasLrLabel = new System.Windows.Forms.Label();
            this.warmupMomentumLabel = new System.Windows.Forms.Label();
            this.warmupEpochsLabel = new System.Windows.Forms.Label();
            this.weightDecayLabel = new System.Windows.Forms.Label();
            this.sgdMomentumLabel = new System.Windows.Forms.Label();
            this.finalLearningRateLabel = new System.Windows.Forms.Label();
            this.initialLearningRateLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.CreateFile);
            this.panel1.Controls.Add(this.copy_paste);
            this.panel1.Controls.Add(this.hsv_h);
            this.panel1.Controls.Add(this.hsv_s);
            this.panel1.Controls.Add(this.hsv_v);
            this.panel1.Controls.Add(this.translate);
            this.panel1.Controls.Add(this.degrees);
            this.panel1.Controls.Add(this.scale);
            this.panel1.Controls.Add(this.shear);
            this.panel1.Controls.Add(this.perspective);
            this.panel1.Controls.Add(this.flipud);
            this.panel1.Controls.Add(this.fliplr);
            this.panel1.Controls.Add(this.mosaic);
            this.panel1.Controls.Add(this.mixup);
            this.panel1.Controls.Add(this.fl_gamma);
            this.panel1.Controls.Add(this.imageMixupLabel);
            this.panel1.Controls.Add(this.segmentCopyPasteLabel);
            this.panel1.Controls.Add(this.imageMosaicLabel);
            this.panel1.Controls.Add(this.imageFlipLeftRightLabel);
            this.panel1.Controls.Add(this.imageFlipUpDownLabel);
            this.panel1.Controls.Add(this.imagePerspectiveLabel);
            this.panel1.Controls.Add(this.imageShearLabel);
            this.panel1.Controls.Add(this.imageScaleLabel);
            this.panel1.Controls.Add(this.imageTranslationLabel);
            this.panel1.Controls.Add(this.imageRotationLabel);
            this.panel1.Controls.Add(this.hsvValueAugLabel);
            this.panel1.Controls.Add(this.hsvSaturationAugLabel);
            this.panel1.Controls.Add(this.hsvHueAugLabel);
            this.panel1.Controls.Add(this.focalLossGammaLabel);
            this.panel1.Controls.Add(this.anchors);
            this.panel1.Controls.Add(this.anchor_t);
            this.panel1.Controls.Add(this.lrf);
            this.panel1.Controls.Add(this.momentum);
            this.panel1.Controls.Add(this.weight_decay);
            this.panel1.Controls.Add(this.warmup_momentum);
            this.panel1.Controls.Add(this.warmup_epochs);
            this.panel1.Controls.Add(this.warmup_bias_lr);
            this.panel1.Controls.Add(this.box);
            this.panel1.Controls.Add(this.cls);
            this.panel1.Controls.Add(this.cls_pw);
            this.panel1.Controls.Add(this.obj);
            this.panel1.Controls.Add(this.obj_pw);
            this.panel1.Controls.Add(this.iou_t);
            this.panel1.Controls.Add(this.lr0);
            this.panel1.Controls.Add(this.iouTrainingThresholdLabel);
            this.panel1.Controls.Add(this.anchorMultipleThresholdLabel);
            this.panel1.Controls.Add(this.anchorsPerOutputLayerLabel);
            this.panel1.Controls.Add(this.objectPositiveWeightLabel);
            this.panel1.Controls.Add(this.objectLossGainLabel);
            this.panel1.Controls.Add(this.clsPositiveWeightLabel);
            this.panel1.Controls.Add(this.clsLossGainLabel);
            this.panel1.Controls.Add(this.boxLossGainLabel);
            this.panel1.Controls.Add(this.warmupBiasLrLabel);
            this.panel1.Controls.Add(this.warmupMomentumLabel);
            this.panel1.Controls.Add(this.warmupEpochsLabel);
            this.panel1.Controls.Add(this.weightDecayLabel);
            this.panel1.Controls.Add(this.sgdMomentumLabel);
            this.panel1.Controls.Add(this.finalLearningRateLabel);
            this.panel1.Controls.Add(this.initialLearningRateLabel);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(630, 540);
            this.panel1.TabIndex = 5;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(365, 488);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(104, 29);
            this.CancelButton.TabIndex = 30;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // CreateFile
            // 
            this.CreateFile.AutoSize = true;
            this.CreateFile.Location = new System.Drawing.Point(498, 488);
            this.CreateFile.Name = "CreateFile";
            this.CreateFile.Size = new System.Drawing.Size(104, 29);
            this.CreateFile.TabIndex = 31;
            this.CreateFile.Text = "Set";
            this.CreateFile.UseVisualStyleBackColor = true;
            this.CreateFile.Click += new System.EventHandler(this.CreateFile_Click);
            // 
            // copy_paste
            // 
            this.copy_paste.Location = new System.Drawing.Point(514, 400);
            this.copy_paste.Name = "copy_paste";
            this.copy_paste.Size = new System.Drawing.Size(88, 22);
            this.copy_paste.TabIndex = 29;
            // 
            // hsv_h
            // 
            this.hsv_h.Location = new System.Drawing.Point(514, 67);
            this.hsv_h.Name = "hsv_h";
            this.hsv_h.Size = new System.Drawing.Size(88, 22);
            this.hsv_h.TabIndex = 17;
            // 
            // hsv_s
            // 
            this.hsv_s.Location = new System.Drawing.Point(514, 92);
            this.hsv_s.Name = "hsv_s";
            this.hsv_s.Size = new System.Drawing.Size(88, 22);
            this.hsv_s.TabIndex = 18;
            // 
            // hsv_v
            // 
            this.hsv_v.Location = new System.Drawing.Point(514, 118);
            this.hsv_v.Name = "hsv_v";
            this.hsv_v.Size = new System.Drawing.Size(88, 22);
            this.hsv_v.TabIndex = 19;
            // 
            // translate
            // 
            this.translate.Location = new System.Drawing.Point(514, 178);
            this.translate.Name = "translate";
            this.translate.Size = new System.Drawing.Size(88, 22);
            this.translate.TabIndex = 21;
            // 
            // degrees
            // 
            this.degrees.Location = new System.Drawing.Point(514, 148);
            this.degrees.Name = "degrees";
            this.degrees.Size = new System.Drawing.Size(88, 22);
            this.degrees.TabIndex = 20;
            // 
            // scale
            // 
            this.scale.Location = new System.Drawing.Point(514, 207);
            this.scale.Name = "scale";
            this.scale.Size = new System.Drawing.Size(88, 22);
            this.scale.TabIndex = 22;
            // 
            // shear
            // 
            this.shear.Location = new System.Drawing.Point(514, 235);
            this.shear.Name = "shear";
            this.shear.Size = new System.Drawing.Size(88, 22);
            this.shear.TabIndex = 23;
            // 
            // perspective
            // 
            this.perspective.Location = new System.Drawing.Point(514, 262);
            this.perspective.Name = "perspective";
            this.perspective.Size = new System.Drawing.Size(88, 22);
            this.perspective.TabIndex = 24;
            // 
            // flipud
            // 
            this.flipud.Location = new System.Drawing.Point(514, 291);
            this.flipud.Name = "flipud";
            this.flipud.Size = new System.Drawing.Size(88, 22);
            this.flipud.TabIndex = 25;
            // 
            // fliplr
            // 
            this.fliplr.Location = new System.Drawing.Point(514, 318);
            this.fliplr.Name = "fliplr";
            this.fliplr.Size = new System.Drawing.Size(88, 22);
            this.fliplr.TabIndex = 26;
            // 
            // mosaic
            // 
            this.mosaic.Location = new System.Drawing.Point(514, 345);
            this.mosaic.Name = "mosaic";
            this.mosaic.Size = new System.Drawing.Size(88, 22);
            this.mosaic.TabIndex = 27;
            // 
            // mixup
            // 
            this.mixup.Location = new System.Drawing.Point(514, 372);
            this.mixup.Name = "mixup";
            this.mixup.Size = new System.Drawing.Size(88, 22);
            this.mixup.TabIndex = 28;
            // 
            // fl_gamma
            // 
            this.fl_gamma.Location = new System.Drawing.Point(514, 43);
            this.fl_gamma.Name = "fl_gamma";
            this.fl_gamma.Size = new System.Drawing.Size(88, 22);
            this.fl_gamma.TabIndex = 16;
            // 
            // imageMixupLabel
            // 
            this.imageMixupLabel.AutoSize = true;
            this.imageMixupLabel.Location = new System.Drawing.Point(362, 374);
            this.imageMixupLabel.Name = "imageMixupLabel";
            this.imageMixupLabel.Size = new System.Drawing.Size(45, 16);
            this.imageMixupLabel.TabIndex = 62;
            this.imageMixupLabel.Text = "Mixup:";
            // 
            // segmentCopyPasteLabel
            // 
            this.segmentCopyPasteLabel.AutoSize = true;
            this.segmentCopyPasteLabel.Location = new System.Drawing.Point(362, 402);
            this.segmentCopyPasteLabel.Name = "segmentCopyPasteLabel";
            this.segmentCopyPasteLabel.Size = new System.Drawing.Size(138, 16);
            this.segmentCopyPasteLabel.TabIndex = 61;
            this.segmentCopyPasteLabel.Text = "Segment Copy-Paste:";
            // 
            // imageMosaicLabel
            // 
            this.imageMosaicLabel.AutoSize = true;
            this.imageMosaicLabel.Location = new System.Drawing.Point(362, 346);
            this.imageMosaicLabel.Name = "imageMosaicLabel";
            this.imageMosaicLabel.Size = new System.Drawing.Size(54, 16);
            this.imageMosaicLabel.TabIndex = 59;
            this.imageMosaicLabel.Text = "Mosaic:";
            // 
            // imageFlipLeftRightLabel
            // 
            this.imageFlipLeftRightLabel.AutoSize = true;
            this.imageFlipLeftRightLabel.Location = new System.Drawing.Point(362, 319);
            this.imageFlipLeftRightLabel.Name = "imageFlipLeftRightLabel";
            this.imageFlipLeftRightLabel.Size = new System.Drawing.Size(91, 16);
            this.imageFlipLeftRightLabel.TabIndex = 58;
            this.imageFlipLeftRightLabel.Text = "Flip Left-Right:";
            // 
            // imageFlipUpDownLabel
            // 
            this.imageFlipUpDownLabel.AutoSize = true;
            this.imageFlipUpDownLabel.Location = new System.Drawing.Point(362, 293);
            this.imageFlipUpDownLabel.Name = "imageFlipUpDownLabel";
            this.imageFlipUpDownLabel.Size = new System.Drawing.Size(91, 16);
            this.imageFlipUpDownLabel.TabIndex = 57;
            this.imageFlipUpDownLabel.Text = "Flip Up-Down:";
            // 
            // imagePerspectiveLabel
            // 
            this.imagePerspectiveLabel.AutoSize = true;
            this.imagePerspectiveLabel.Location = new System.Drawing.Point(362, 267);
            this.imagePerspectiveLabel.Name = "imagePerspectiveLabel";
            this.imagePerspectiveLabel.Size = new System.Drawing.Size(82, 16);
            this.imagePerspectiveLabel.TabIndex = 56;
            this.imagePerspectiveLabel.Text = "Perspective:";
            // 
            // imageShearLabel
            // 
            this.imageShearLabel.AutoSize = true;
            this.imageShearLabel.Location = new System.Drawing.Point(362, 237);
            this.imageShearLabel.Name = "imageShearLabel";
            this.imageShearLabel.Size = new System.Drawing.Size(46, 16);
            this.imageShearLabel.TabIndex = 55;
            this.imageShearLabel.Text = "Shear:";
            // 
            // imageScaleLabel
            // 
            this.imageScaleLabel.AutoSize = true;
            this.imageScaleLabel.Location = new System.Drawing.Point(362, 208);
            this.imageScaleLabel.Name = "imageScaleLabel";
            this.imageScaleLabel.Size = new System.Drawing.Size(45, 16);
            this.imageScaleLabel.TabIndex = 54;
            this.imageScaleLabel.Text = "Scale:";
            // 
            // imageTranslationLabel
            // 
            this.imageTranslationLabel.AutoSize = true;
            this.imageTranslationLabel.Location = new System.Drawing.Point(362, 180);
            this.imageTranslationLabel.Name = "imageTranslationLabel";
            this.imageTranslationLabel.Size = new System.Drawing.Size(67, 16);
            this.imageTranslationLabel.TabIndex = 53;
            this.imageTranslationLabel.Text = "Translate:";
            // 
            // imageRotationLabel
            // 
            this.imageRotationLabel.AutoSize = true;
            this.imageRotationLabel.Location = new System.Drawing.Point(362, 152);
            this.imageRotationLabel.Name = "imageRotationLabel";
            this.imageRotationLabel.Size = new System.Drawing.Size(60, 16);
            this.imageRotationLabel.TabIndex = 52;
            this.imageRotationLabel.Text = "Rotation:";
            // 
            // hsvValueAugLabel
            // 
            this.hsvValueAugLabel.AutoSize = true;
            this.hsvValueAugLabel.Location = new System.Drawing.Point(362, 123);
            this.hsvValueAugLabel.Name = "hsvValueAugLabel";
            this.hsvValueAugLabel.Size = new System.Drawing.Size(77, 16);
            this.hsvValueAugLabel.TabIndex = 51;
            this.hsvValueAugLabel.Text = "HSV-Value:";
            // 
            // hsvSaturationAugLabel
            // 
            this.hsvSaturationAugLabel.AutoSize = true;
            this.hsvSaturationAugLabel.Location = new System.Drawing.Point(362, 95);
            this.hsvSaturationAugLabel.Name = "hsvSaturationAugLabel";
            this.hsvSaturationAugLabel.Size = new System.Drawing.Size(102, 16);
            this.hsvSaturationAugLabel.TabIndex = 50;
            this.hsvSaturationAugLabel.Text = "HSV-Saturation:";
            // 
            // hsvHueAugLabel
            // 
            this.hsvHueAugLabel.AutoSize = true;
            this.hsvHueAugLabel.Location = new System.Drawing.Point(362, 69);
            this.hsvHueAugLabel.Name = "hsvHueAugLabel";
            this.hsvHueAugLabel.Size = new System.Drawing.Size(67, 16);
            this.hsvHueAugLabel.TabIndex = 49;
            this.hsvHueAugLabel.Text = "HSV-Hue:";
            // 
            // focalLossGammaLabel
            // 
            this.focalLossGammaLabel.AutoSize = true;
            this.focalLossGammaLabel.Location = new System.Drawing.Point(362, 43);
            this.focalLossGammaLabel.Name = "focalLossGammaLabel";
            this.focalLossGammaLabel.Size = new System.Drawing.Size(127, 16);
            this.focalLossGammaLabel.TabIndex = 48;
            this.focalLossGammaLabel.Text = "Focal Loss Gamma:";
            // 
            // anchors
            // 
            this.anchors.Location = new System.Drawing.Point(212, 433);
            this.anchors.Name = "anchors";
            this.anchors.Size = new System.Drawing.Size(88, 22);
            this.anchors.TabIndex = 15;
            // 
            // anchor_t
            // 
            this.anchor_t.Location = new System.Drawing.Point(212, 400);
            this.anchor_t.Name = "anchor_t";
            this.anchor_t.Size = new System.Drawing.Size(88, 22);
            this.anchor_t.TabIndex = 14;
            // 
            // lrf
            // 
            this.lrf.Location = new System.Drawing.Point(212, 67);
            this.lrf.Name = "lrf";
            this.lrf.Size = new System.Drawing.Size(88, 22);
            this.lrf.TabIndex = 2;
            // 
            // momentum
            // 
            this.momentum.Location = new System.Drawing.Point(212, 92);
            this.momentum.Name = "momentum";
            this.momentum.Size = new System.Drawing.Size(88, 22);
            this.momentum.TabIndex = 3;
            // 
            // weight_decay
            // 
            this.weight_decay.Location = new System.Drawing.Point(212, 118);
            this.weight_decay.Name = "weight_decay";
            this.weight_decay.Size = new System.Drawing.Size(88, 22);
            this.weight_decay.TabIndex = 4;
            // 
            // warmup_momentum
            // 
            this.warmup_momentum.Location = new System.Drawing.Point(212, 178);
            this.warmup_momentum.Name = "warmup_momentum";
            this.warmup_momentum.Size = new System.Drawing.Size(88, 22);
            this.warmup_momentum.TabIndex = 6;
            // 
            // warmup_epochs
            // 
            this.warmup_epochs.Location = new System.Drawing.Point(212, 148);
            this.warmup_epochs.Name = "warmup_epochs";
            this.warmup_epochs.Size = new System.Drawing.Size(88, 22);
            this.warmup_epochs.TabIndex = 5;
            // 
            // warmup_bias_lr
            // 
            this.warmup_bias_lr.Location = new System.Drawing.Point(212, 207);
            this.warmup_bias_lr.Name = "warmup_bias_lr";
            this.warmup_bias_lr.Size = new System.Drawing.Size(88, 22);
            this.warmup_bias_lr.TabIndex = 7;
            // 
            // box
            // 
            this.box.Location = new System.Drawing.Point(212, 235);
            this.box.Name = "box";
            this.box.Size = new System.Drawing.Size(88, 22);
            this.box.TabIndex = 8;
            // 
            // cls
            // 
            this.cls.Location = new System.Drawing.Point(212, 262);
            this.cls.Name = "cls";
            this.cls.Size = new System.Drawing.Size(88, 22);
            this.cls.TabIndex = 9;
            // 
            // cls_pw
            // 
            this.cls_pw.Location = new System.Drawing.Point(212, 291);
            this.cls_pw.Name = "cls_pw";
            this.cls_pw.Size = new System.Drawing.Size(88, 22);
            this.cls_pw.TabIndex = 10;
            // 
            // obj
            // 
            this.obj.Location = new System.Drawing.Point(212, 318);
            this.obj.Name = "obj";
            this.obj.Size = new System.Drawing.Size(88, 22);
            this.obj.TabIndex = 11;
            // 
            // obj_pw
            // 
            this.obj_pw.Location = new System.Drawing.Point(212, 345);
            this.obj_pw.Name = "obj_pw";
            this.obj_pw.Size = new System.Drawing.Size(88, 22);
            this.obj_pw.TabIndex = 12;
            // 
            // iou_t
            // 
            this.iou_t.Location = new System.Drawing.Point(212, 372);
            this.iou_t.Name = "iou_t";
            this.iou_t.Size = new System.Drawing.Size(88, 22);
            this.iou_t.TabIndex = 13;
            // 
            // lr0
            // 
            this.lr0.Location = new System.Drawing.Point(212, 43);
            this.lr0.Name = "lr0";
            this.lr0.Size = new System.Drawing.Size(88, 22);
            this.lr0.TabIndex = 1;
            // 
            // iouTrainingThresholdLabel
            // 
            this.iouTrainingThresholdLabel.AutoSize = true;
            this.iouTrainingThresholdLabel.Location = new System.Drawing.Point(22, 374);
            this.iouTrainingThresholdLabel.Name = "iouTrainingThresholdLabel";
            this.iouTrainingThresholdLabel.Size = new System.Drawing.Size(147, 16);
            this.iouTrainingThresholdLabel.TabIndex = 32;
            this.iouTrainingThresholdLabel.Text = "IoU Training Threshold:";
            // 
            // anchorMultipleThresholdLabel
            // 
            this.anchorMultipleThresholdLabel.AutoSize = true;
            this.anchorMultipleThresholdLabel.Location = new System.Drawing.Point(22, 402);
            this.anchorMultipleThresholdLabel.Name = "anchorMultipleThresholdLabel";
            this.anchorMultipleThresholdLabel.Size = new System.Drawing.Size(166, 16);
            this.anchorMultipleThresholdLabel.TabIndex = 31;
            this.anchorMultipleThresholdLabel.Text = "Anchor-multiple Threshold:";
            // 
            // anchorsPerOutputLayerLabel
            // 
            this.anchorsPerOutputLayerLabel.AutoSize = true;
            this.anchorsPerOutputLayerLabel.Location = new System.Drawing.Point(22, 436);
            this.anchorsPerOutputLayerLabel.Name = "anchorsPerOutputLayerLabel";
            this.anchorsPerOutputLayerLabel.Size = new System.Drawing.Size(160, 16);
            this.anchorsPerOutputLayerLabel.TabIndex = 30;
            this.anchorsPerOutputLayerLabel.Text = "Anchors per Output Layer:";
            // 
            // objectPositiveWeightLabel
            // 
            this.objectPositiveWeightLabel.AutoSize = true;
            this.objectPositiveWeightLabel.Location = new System.Drawing.Point(22, 346);
            this.objectPositiveWeightLabel.Name = "objectPositiveWeightLabel";
            this.objectPositiveWeightLabel.Size = new System.Drawing.Size(145, 16);
            this.objectPositiveWeightLabel.TabIndex = 29;
            this.objectPositiveWeightLabel.Text = "Object Positive Weight:";
            // 
            // objectLossGainLabel
            // 
            this.objectLossGainLabel.AutoSize = true;
            this.objectLossGainLabel.Location = new System.Drawing.Point(22, 319);
            this.objectLossGainLabel.Name = "objectLossGainLabel";
            this.objectLossGainLabel.Size = new System.Drawing.Size(112, 16);
            this.objectLossGainLabel.TabIndex = 28;
            this.objectLossGainLabel.Text = "Object Loss Gain:";
            // 
            // clsPositiveWeightLabel
            // 
            this.clsPositiveWeightLabel.AutoSize = true;
            this.clsPositiveWeightLabel.Location = new System.Drawing.Point(22, 293);
            this.clsPositiveWeightLabel.Name = "clsPositiveWeightLabel";
            this.clsPositiveWeightLabel.Size = new System.Drawing.Size(131, 16);
            this.clsPositiveWeightLabel.TabIndex = 27;
            this.clsPositiveWeightLabel.Text = "CLS Positive Weight:";
            // 
            // clsLossGainLabel
            // 
            this.clsLossGainLabel.AutoSize = true;
            this.clsLossGainLabel.Location = new System.Drawing.Point(22, 267);
            this.clsLossGainLabel.Name = "clsLossGainLabel";
            this.clsLossGainLabel.Size = new System.Drawing.Size(98, 16);
            this.clsLossGainLabel.TabIndex = 26;
            this.clsLossGainLabel.Text = "CLS Loss Gain:";
            // 
            // boxLossGainLabel
            // 
            this.boxLossGainLabel.AutoSize = true;
            this.boxLossGainLabel.Location = new System.Drawing.Point(22, 237);
            this.boxLossGainLabel.Name = "boxLossGainLabel";
            this.boxLossGainLabel.Size = new System.Drawing.Size(96, 16);
            this.boxLossGainLabel.TabIndex = 25;
            this.boxLossGainLabel.Text = "Box Loss Gain:";
            // 
            // warmupBiasLrLabel
            // 
            this.warmupBiasLrLabel.AutoSize = true;
            this.warmupBiasLrLabel.Location = new System.Drawing.Point(22, 208);
            this.warmupBiasLrLabel.Name = "warmupBiasLrLabel";
            this.warmupBiasLrLabel.Size = new System.Drawing.Size(144, 16);
            this.warmupBiasLrLabel.TabIndex = 24;
            this.warmupBiasLrLabel.Text = "Warmup Initial Bias LR:";
            // 
            // warmupMomentumLabel
            // 
            this.warmupMomentumLabel.AutoSize = true;
            this.warmupMomentumLabel.Location = new System.Drawing.Point(22, 180);
            this.warmupMomentumLabel.Name = "warmupMomentumLabel";
            this.warmupMomentumLabel.Size = new System.Drawing.Size(163, 16);
            this.warmupMomentumLabel.TabIndex = 23;
            this.warmupMomentumLabel.Text = "Warmup Initial Momentum:";
            // 
            // warmupEpochsLabel
            // 
            this.warmupEpochsLabel.AutoSize = true;
            this.warmupEpochsLabel.Location = new System.Drawing.Point(22, 152);
            this.warmupEpochsLabel.Name = "warmupEpochsLabel";
            this.warmupEpochsLabel.Size = new System.Drawing.Size(110, 16);
            this.warmupEpochsLabel.TabIndex = 22;
            this.warmupEpochsLabel.Text = "Warmup Epochs:";
            // 
            // weightDecayLabel
            // 
            this.weightDecayLabel.AutoSize = true;
            this.weightDecayLabel.Location = new System.Drawing.Point(22, 123);
            this.weightDecayLabel.Name = "weightDecayLabel";
            this.weightDecayLabel.Size = new System.Drawing.Size(154, 16);
            this.weightDecayLabel.TabIndex = 21;
            this.weightDecayLabel.Text = "Optimizer Weight Decay:";
            // 
            // sgdMomentumLabel
            // 
            this.sgdMomentumLabel.AutoSize = true;
            this.sgdMomentumLabel.Location = new System.Drawing.Point(22, 95);
            this.sgdMomentumLabel.Name = "sgdMomentumLabel";
            this.sgdMomentumLabel.Size = new System.Drawing.Size(108, 16);
            this.sgdMomentumLabel.TabIndex = 20;
            this.sgdMomentumLabel.Text = "SGD Momentum:";
            // 
            // finalLearningRateLabel
            // 
            this.finalLearningRateLabel.AutoSize = true;
            this.finalLearningRateLabel.Location = new System.Drawing.Point(22, 69);
            this.finalLearningRateLabel.Name = "finalLearningRateLabel";
            this.finalLearningRateLabel.Size = new System.Drawing.Size(126, 16);
            this.finalLearningRateLabel.TabIndex = 19;
            this.finalLearningRateLabel.Text = "Final Learning Rate:";
            // 
            // initialLearningRateLabel
            // 
            this.initialLearningRateLabel.AutoSize = true;
            this.initialLearningRateLabel.Location = new System.Drawing.Point(22, 43);
            this.initialLearningRateLabel.Name = "initialLearningRateLabel";
            this.initialLearningRateLabel.Size = new System.Drawing.Size(127, 16);
            this.initialLearningRateLabel.TabIndex = 18;
            this.initialLearningRateLabel.Text = "Initial Learning Rate:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(199, 16);
            this.label8.TabIndex = 17;
            this.label8.Text = "Custom Hyperparameter Values";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 16);
            this.label2.TabIndex = 5;
            // 
            // CustomHyperParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 569);
            this.Controls.Add(this.panel1);
            this.Name = "CustomHyperParamsForm";
            this.Text = "CustomHyperParamsForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label initialLearningRateLabel;
        private System.Windows.Forms.Label sgdMomentumLabel;
        private System.Windows.Forms.Label finalLearningRateLabel;
        private System.Windows.Forms.Label iouTrainingThresholdLabel;
        private System.Windows.Forms.Label anchorMultipleThresholdLabel;
        private System.Windows.Forms.Label anchorsPerOutputLayerLabel;
        private System.Windows.Forms.Label objectPositiveWeightLabel;
        private System.Windows.Forms.Label objectLossGainLabel;
        private System.Windows.Forms.Label clsPositiveWeightLabel;
        private System.Windows.Forms.Label clsLossGainLabel;
        private System.Windows.Forms.Label boxLossGainLabel;
        private System.Windows.Forms.Label warmupBiasLrLabel;
        private System.Windows.Forms.Label warmupMomentumLabel;
        private System.Windows.Forms.Label warmupEpochsLabel;
        private System.Windows.Forms.Label weightDecayLabel;
        private System.Windows.Forms.TextBox lr0;
        private System.Windows.Forms.TextBox copy_paste;
        private System.Windows.Forms.TextBox hsv_h;
        private System.Windows.Forms.TextBox hsv_s;
        private System.Windows.Forms.TextBox hsv_v;
        private System.Windows.Forms.TextBox translate;
        private System.Windows.Forms.TextBox degrees;
        private System.Windows.Forms.TextBox scale;
        private System.Windows.Forms.TextBox shear;
        private System.Windows.Forms.TextBox perspective;
        private System.Windows.Forms.TextBox flipud;
        private System.Windows.Forms.TextBox fliplr;
        private System.Windows.Forms.TextBox mosaic;
        private System.Windows.Forms.TextBox mixup;
        private System.Windows.Forms.TextBox fl_gamma;
        private System.Windows.Forms.Label imageMixupLabel;
        private System.Windows.Forms.Label segmentCopyPasteLabel;
        private System.Windows.Forms.Label imageMosaicLabel;
        private System.Windows.Forms.Label imageFlipLeftRightLabel;
        private System.Windows.Forms.Label imageFlipUpDownLabel;
        private System.Windows.Forms.Label imagePerspectiveLabel;
        private System.Windows.Forms.Label imageShearLabel;
        private System.Windows.Forms.Label imageScaleLabel;
        private System.Windows.Forms.Label imageTranslationLabel;
        private System.Windows.Forms.Label imageRotationLabel;
        private System.Windows.Forms.Label hsvValueAugLabel;
        private System.Windows.Forms.Label hsvSaturationAugLabel;
        private System.Windows.Forms.Label hsvHueAugLabel;
        private System.Windows.Forms.Label focalLossGammaLabel;
        private System.Windows.Forms.TextBox anchors;
        private System.Windows.Forms.TextBox anchor_t;
        private System.Windows.Forms.TextBox lrf;
        private System.Windows.Forms.TextBox momentum;
        private System.Windows.Forms.TextBox weight_decay;
        private System.Windows.Forms.TextBox warmup_momentum;
        private System.Windows.Forms.TextBox warmup_epochs;
        private System.Windows.Forms.TextBox warmup_bias_lr;
        private System.Windows.Forms.TextBox box;
        private System.Windows.Forms.TextBox cls;
        private System.Windows.Forms.TextBox cls_pw;
        private System.Windows.Forms.TextBox obj;
        private System.Windows.Forms.TextBox obj_pw;
        private System.Windows.Forms.TextBox iou_t;
        private System.Windows.Forms.Button CreateFile;
        private System.Windows.Forms.Button CancelButton;
    }
}