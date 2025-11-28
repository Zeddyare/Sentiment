namespace Sentiment
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnPredict;
        private System.Windows.Forms.CheckBox chkLabelPositive;
        private System.Windows.Forms.Button btnSubmit;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
            // txtOutput
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.txtOutput.Location = new System.Drawing.Point(12, 12);
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(776, 370);
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.ReadOnly = true;
            // btnRun
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRun.Location = new System.Drawing.Point(12, 390);
            this.btnRun.Size = new System.Drawing.Size(100, 30);
            this.btnRun.Text = "Run";
            this.btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // lblInput
            this.lblInput = new System.Windows.Forms.Label();
            this.lblInput.Location = new System.Drawing.Point(130, 390);
            this.lblInput.Size = new System.Drawing.Size(40, 23);
            this.lblInput.Text = "Input:";
            this.lblInput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            // txtInput
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtInput.Location = new System.Drawing.Point(170, 392);
            this.txtInput.Size = new System.Drawing.Size(450, 23);
            this.txtInput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            // btnPredict
            this.btnPredict = new System.Windows.Forms.Button();
            this.btnPredict.Location = new System.Drawing.Point(640, 390);
            this.btnPredict.Size = new System.Drawing.Size(100, 30);
            this.btnPredict.Text = "Predict";
            this.btnPredict.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnPredict.Click += new System.EventHandler(this.btnPredict_Click);
            // chkLabelPositive
            this.chkLabelPositive = new System.Windows.Forms.CheckBox();
            this.chkLabelPositive.Location = new System.Drawing.Point(12, 425);
            this.chkLabelPositive.Size = new System.Drawing.Size(140, 20);
            this.chkLabelPositive.Text = "Label as Positive";
            this.chkLabelPositive.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            // btnSubmit
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnSubmit.Location = new System.Drawing.Point(170, 420);
            this.btnSubmit.Size = new System.Drawing.Size(100, 25);
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // Add controls
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnPredict);
            this.Controls.Add(this.chkLabelPositive);
            this.Controls.Add(this.btnSubmit);
        }

        #endregion
    }
}
