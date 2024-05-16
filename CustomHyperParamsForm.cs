using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AutomatedAWSModelCreator.Functions;

namespace AutomatedAWSModelCreator
{

    public partial class CustomHyperParamsForm : Form
    {
        private Utility utility = new Utility();
        public Dictionary<string, string> HyperParameters { get; private set; }

        public CustomHyperParamsForm()
        {
            InitializeComponent();

            //Default hyperparam values (hyp.scratch-low.yaml)
            lr0.Text = "0.01";
            lrf.Text = "0.01";
            momentum.Text = "0.937";
            weight_decay.Text = "0.0005";
            warmup_epochs.Text = "3.0";
            warmup_momentum.Text = "0.8";
            warmup_bias_lr.Text = "0.1";
            box.Text = "0.05";
            cls.Text = "0.5";
            cls_pw.Text = "1.0";
            obj.Text = "1.0";
            obj_pw.Text = "1.0";
            iou_t.Text = "0.20";
            anchor_t.Text = "4.0";
            anchors.Text = "0";
            fl_gamma.Text = "0.0";
            hsv_h.Text = "0.015";
            hsv_s.Text = "0.7";
            hsv_v.Text = "0.4";
            degrees.Text = "0.0";
            translate.Text = "0.1";
            scale.Text = "0.5";
            shear.Text = "0.0";
            perspective.Text = "0.0";
            flipud.Text = "0.0";
            fliplr.Text = "0.5";
            mosaic.Text = "1.0";
            mixup.Text = "0.0";
            copy_paste.Text = "0.0";
        }

        private void CreateFile_Click(object sender, EventArgs e)
        {
            SetHyperparameters();
            this.Close();
        }

        private void SetHyperparameters()
        {
            HyperParameters = new Dictionary<string, string>
            {
                {"lr0", utility.GetValueFromTextBox(lr0)},
                {"lrf", utility.GetValueFromTextBox(lrf)},
                {"momentum", utility.GetValueFromTextBox(momentum)},
                {"weight_decay", utility.GetValueFromTextBox(weight_decay)},
                {"warmup_epochs", utility.GetValueFromTextBox(warmup_epochs)},
                {"warmup_momentum", utility.GetValueFromTextBox(warmup_momentum)},
                {"warmup_bias_lr", utility.GetValueFromTextBox(warmup_bias_lr)},
                {"box", utility.GetValueFromTextBox(box)},
                {"cls", utility.GetValueFromTextBox(cls)},
                {"cls_pw", utility.GetValueFromTextBox(cls_pw)},
                {"obj", utility.GetValueFromTextBox(obj)},
                {"obj_pw", utility.GetValueFromTextBox(obj_pw)},
                {"iou_t", utility.GetValueFromTextBox(iou_t)},
                {"anchor_t", utility.GetValueFromTextBox(anchor_t)},
                {"anchors", utility.GetValueFromTextBox(anchors)},
                {"fl_gamma", utility.GetValueFromTextBox(fl_gamma)},
                {"hsv_h", utility.GetValueFromTextBox(hsv_h)},
                {"hsv_s", utility.GetValueFromTextBox(hsv_s)},
                {"hsv_v", utility.GetValueFromTextBox(hsv_v)},
                {"degrees", utility.GetValueFromTextBox(degrees)},
                {"translate", utility.GetValueFromTextBox(translate)},
                {"scale", utility.GetValueFromTextBox(scale)},
                {"shear", utility.GetValueFromTextBox(shear)},
                {"perspective", utility.GetValueFromTextBox(perspective)},
                {"flipud", utility.GetValueFromTextBox(flipud)},
                {"fliplr", utility.GetValueFromTextBox(fliplr)},
                {"mosaic", utility.GetValueFromTextBox(mosaic)},
                {"mixup", utility.GetValueFromTextBox(mixup)},
                {"copy_paste", utility.GetValueFromTextBox(copy_paste)}
            };
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
