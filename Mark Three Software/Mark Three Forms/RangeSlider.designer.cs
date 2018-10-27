namespace MarkThree.Forms
{
    partial class RangeSlider
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
            this.labelADV = new System.Windows.Forms.Label();
            this.checkAutoEx = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.LargeChange = 0;
            this.trackBar.Location = new System.Drawing.Point(42, 0);
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 115);
            this.trackBar.TabIndex = 0;
            this.trackBar.TabStop = false;
            this.trackBar.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // labelADV
            // 
            this.labelADV.BackColor = System.Drawing.SystemColors.ControlLight;
            this.labelADV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelADV.Location = new System.Drawing.Point(4, 4);
            this.labelADV.Name = "labelADV";
            this.labelADV.Size = new System.Drawing.Size(34, 15);
            this.labelADV.TabIndex = 2;
            this.labelADV.Text = "High";
            // 
            // checkAutoEx
            // 
            this.checkAutoEx.AutoSize = true;
            this.checkAutoEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkAutoEx.Location = new System.Drawing.Point(273, 4);
            this.checkAutoEx.Name = "checkAutoEx";
            this.checkAutoEx.Size = new System.Drawing.Size(90, 17);
            this.checkAutoEx.TabIndex = 3;
            this.checkAutoEx.Text = "Auto-Execute";
            this.checkAutoEx.UseVisualStyleBackColor = true;
            // 
            // RangeSlider
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.checkAutoEx);
            this.Controls.Add(this.labelADV);
            this.Controls.Add(this.trackBar);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "RangeSlider";
            this.Size = new System.Drawing.Size(410, 117);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Label labelADV;
        private System.Windows.Forms.CheckBox checkAutoEx;
    }
}
