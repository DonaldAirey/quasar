namespace MarkThree.Forms
{
    partial class ADVSlider
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBucketOn = new System.Windows.Forms.CheckBox();
            this.labelBucket = new System.Windows.Forms.Label();
            this.autoExecute = new MarkThree.Forms.IntegerPicker();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.LargeChange = 0;
            this.trackBar.Location = new System.Drawing.Point(66, -1);
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 115);
            this.trackBar.TabIndex = 0;
            this.trackBar.TabStop = false;
            this.trackBar.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(269, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Auto Execute Quantity:";
            // 
            // checkBucketOn
            // 
            this.checkBucketOn.AutoSize = true;
            this.checkBucketOn.Checked = true;
            this.checkBucketOn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBucketOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.checkBucketOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.checkBucketOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBucketOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBucketOn.Location = new System.Drawing.Point(4, 4);
            this.checkBucketOn.Name = "checkBucketOn";
            this.checkBucketOn.Size = new System.Drawing.Size(45, 17);
            this.checkBucketOn.TabIndex = 5;
            this.checkBucketOn.Text = "High";
            this.checkBucketOn.UseVisualStyleBackColor = true;
            this.checkBucketOn.CheckedChanged += new System.EventHandler(this.checkBucketOn_CheckedChanged);
            // 
            // labelBucket
            // 
            this.labelBucket.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBucket.Location = new System.Drawing.Point(265, 91);
            this.labelBucket.Name = "labelBucket";
            this.labelBucket.Size = new System.Drawing.Size(139, 23);
            this.labelBucket.TabIndex = 6;
            this.labelBucket.Text = "Over 250K shares";
            this.labelBucket.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // autoExecute
            // 
            this.autoExecute.CustomFormat = "0";
            this.autoExecute.CustomUnit = "";
            this.autoExecute.Interval = 0;
            this.autoExecute.Location = new System.Drawing.Point(294, 21);
            this.autoExecute.Name = "autoExecute";
            this.autoExecute.Size = new System.Drawing.Size(113, 20);
            this.autoExecute.StartInteger = 0;
            this.autoExecute.StopInteger = 0;
            this.autoExecute.TabIndex = 3;
            // 
            // ADVSlider
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelBucket);
            this.Controls.Add(this.checkBucketOn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.autoExecute);
            this.Controls.Add(this.trackBar);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ADVSlider";
            this.Size = new System.Drawing.Size(410, 117);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Label label1;
        private IntegerPicker autoExecute;
        private System.Windows.Forms.CheckBox checkBucketOn;
        private System.Windows.Forms.Label labelBucket;
    }
}
