namespace MarkThree.Forms
{
    partial class TimeRangeSlider
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
            this.endTimeSlider = new System.Windows.Forms.TrackBar();
            this.startTimeSlider = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelBarPosition = new System.Windows.Forms.Label();
            this.endTimePicker = new MarkThree.Forms.TimePicker();
            this.startTimePicker = new MarkThree.Forms.TimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // endTimeSlider
            // 
            this.endTimeSlider.Location = new System.Drawing.Point(6, 140);
            this.endTimeSlider.Maximum = 84;
            this.endTimeSlider.Name = "endTimeSlider";
            this.endTimeSlider.Size = new System.Drawing.Size(395, 45);
            this.endTimeSlider.TabIndex = 0;
            this.endTimeSlider.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.endTimeSlider.Value = 84;
            this.endTimeSlider.ValueChanged += new System.EventHandler(this.OnValueChanged);
            this.endTimeSlider.Scroll += new System.EventHandler(this.OnEndTimeChanging);
            // 
            // startTimeSlider
            // 
            this.startTimeSlider.Location = new System.Drawing.Point(7, 37);
            this.startTimeSlider.Maximum = 84;
            this.startTimeSlider.Name = "startTimeSlider";
            this.startTimeSlider.Size = new System.Drawing.Size(399, 45);
            this.startTimeSlider.TabIndex = 1;
            this.startTimeSlider.ValueChanged += new System.EventHandler(this.OnValueChanged);
            this.startTimeSlider.Scroll += new System.EventHandler(this.OnStartTimeChanging);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "9:00AM";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(363, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "4:00PM";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "9:00AM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(363, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "4:00PM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(5, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Start Time";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(341, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "End Time";
            // 
            // labelBarPosition
            // 
            this.labelBarPosition.Location = new System.Drawing.Point(8, 82);
            this.labelBarPosition.Name = "labelBarPosition";
            this.labelBarPosition.Size = new System.Drawing.Size(393, 39);
            this.labelBarPosition.TabIndex = 12;
            this.labelBarPosition.Text = "Placeholder label";
            this.labelBarPosition.Visible = false;
            // 
            // endTimePicker
            // 
            this.endTimePicker.BackColor = System.Drawing.SystemColors.Window;
            this.endTimePicker.CustomFormat = "h:mm tt";
            this.endTimePicker.ForeColor = System.Drawing.Color.Blue;
            this.endTimePicker.Interval = System.TimeSpan.Parse("00:05:00");
            this.endTimePicker.Location = new System.Drawing.Point(312, 16);
            this.endTimePicker.Name = "endTimePicker";
            this.endTimePicker.Size = new System.Drawing.Size(89, 20);
            this.endTimePicker.StartTime = new System.DateTime(2006, 1, 1, 0, 0, 0, 0);
            this.endTimePicker.StopFormattedDateTime = new System.DateTime(2006, 1, 2, 0, 0, 0, 0);
            this.endTimePicker.TabIndex = 14;
            this.endTimePicker.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.endTimePicker.SelectedItemChanged += new System.EventHandler(this.OnEndTimeValueChanged);
            this.endTimePicker.TextChanged += new System.EventHandler(this.OnEndTimeChanged);
            // 
            // startTimePicker
            // 
            this.startTimePicker.BackColor = System.Drawing.SystemColors.Window;
            this.startTimePicker.CustomFormat = "h:mm tt";
            this.startTimePicker.ForeColor = System.Drawing.Color.Blue;
            this.startTimePicker.Interval = System.TimeSpan.Parse("00:05:00");
            this.startTimePicker.Location = new System.Drawing.Point(8, 16);
            this.startTimePicker.Name = "startTimePicker";
            this.startTimePicker.Size = new System.Drawing.Size(89, 20);
            this.startTimePicker.StartTime = new System.DateTime(2006, 1, 1, 0, 0, 0, 0);
            this.startTimePicker.StopFormattedDateTime = new System.DateTime(2006, 1, 2, 0, 0, 0, 0);
            this.startTimePicker.TabIndex = 13;
            this.startTimePicker.SelectedItemChanged += new System.EventHandler(this.OnStartTimeValueChanged);
            this.startTimePicker.TextChanged += new System.EventHandler(this.OnStartTimeChanged);
            // 
            // TimeRangeSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.endTimePicker);
            this.Controls.Add(this.startTimePicker);
            this.Controls.Add(this.labelBarPosition);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.startTimeSlider);
            this.Controls.Add(this.endTimeSlider);
            this.DoubleBuffered = true;
            this.Name = "TimeRangeSlider";
            this.Size = new System.Drawing.Size(410, 188);
            this.Load += new System.EventHandler(this.OnLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            ((System.ComponentModel.ISupportInitialize)(this.endTimeSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar endTimeSlider;
        private System.Windows.Forms.TrackBar startTimeSlider;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelBarPosition;
        private TimePicker startTimePicker;
        private TimePicker endTimePicker;
    }
}
