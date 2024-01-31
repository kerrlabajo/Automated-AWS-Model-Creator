using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LSC_Trainer.Functions;

namespace LSC_Trainer
{

    public partial class CustomHyperParamsForm : Form
    {
        private Utility utility = new Utility();
        public CustomHyperParamsForm()
        {
            InitializeComponent();

            //Default hyperparam values
            learningRate.Text = "0.01";
            finalLearningRate.Text = "0.01";
            momentum.Text = "0.937";
            weightDecay.Text = "0.0005";
            warmupEpochs.Text = "3.0";
            warmupMomentum.Text = "0.8";
            warmupBias.Text = "0.1";
            box.Text = "0.05";
            cls.Text = "0.5";
            clsPostiveWeight.Text = "1.0";
            obj.Text = "1.0";
            objPositiveWeight.Text = "1.0";
            iouTreshold.Text = "0.20";
            anchorTreshold.Text = "4.0";
            anchors.Text = "3";
            focalLossGamma.Text = "0.0";
            hsvHue.Text = "0.015";
            hsvSaturation.Text = "0.7";
            hsvValue.Text = "0.4";
            imageRotation.Text = "0.0";
            imageTranslation.Text = "0.1";
            imageScale.Text = "0.5";
            imageShear.Text = "0.0";
            imagePerspective.Text = "0.0";
            imageFlipUpDown.Text = "0.0";
            imageFlipLeftRight.Text = "0.5";
            imageMosaic.Text = "1.0";
            imageMixup.Text = "0.0";
            segementCopyPaste.Text = "0.0";
        }

        private void CreateFile_Click(object sender, EventArgs e)
        {
            var hyperparameters = SetHyperparameters();

            FileHandler.WriteYamlFile(hyperparameters);
            this.Close();
        }

        private Dictionary<string, string> SetHyperparameters()
        {
            var hyperparameters = new Dictionary<string, string>
            {
                {"learning_rate", utility.GetValueFromTextBox(learningRate)},
                {"final_learning_rate", utility.GetValueFromTextBox(finalLearningRate)},
                {"momentum", utility.GetValueFromTextBox(momentum)},
                {"weight_decay", utility.GetValueFromTextBox(weightDecay)},
                {"warmup_epochs", utility.GetValueFromTextBox(warmupEpochs)},
                {"warmup_momentum", utility.GetValueFromTextBox(warmupMomentum)},
                {"warmup_bias_lr", utility.GetValueFromTextBox(warmupBias)},
                {"box_value", utility.GetValueFromTextBox(box)},
                {"cls_value", utility.GetValueFromTextBox(cls)},
                {"cls_pw", utility.GetValueFromTextBox(clsPostiveWeight)},
                {"obj_value", utility.GetValueFromTextBox(obj)},
                {"obj_pw", utility.GetValueFromTextBox(objPositiveWeight)},
                {"iou_t", utility.GetValueFromTextBox(iouTreshold)},
                {"anchor_t", utility.GetValueFromTextBox(anchorTreshold)},
                {"anchor", utility.GetValueFromTextBox(anchors)},
                {"fl_gamma", utility.GetValueFromTextBox(focalLossGamma)},
                {"hsv_h", utility.GetValueFromTextBox(hsvHue)},
                {"hsv_s", utility.GetValueFromTextBox(hsvSaturation)},
                {"hsv_v", utility.GetValueFromTextBox(hsvValue)},
                {"degrees", utility.GetValueFromTextBox(imageRotation)},
                {"translate", utility.GetValueFromTextBox(imageTranslation)},
                {"scale", utility.GetValueFromTextBox(imageScale)},
                {"shear", utility.GetValueFromTextBox(imageShear)},
                {"perspective", utility.GetValueFromTextBox(imagePerspective)},
                {"flipUD", utility.GetValueFromTextBox(imageFlipUpDown)},
                {"flipLR", utility.GetValueFromTextBox(imageFlipLeftRight)},
                {"mosaic", utility.GetValueFromTextBox(imageMosaic)},
                {"mixup", utility.GetValueFromTextBox(imageMixup)},
                {"copy_paste", utility.GetValueFromTextBox(segementCopyPaste)}
            };

            return hyperparameters;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
