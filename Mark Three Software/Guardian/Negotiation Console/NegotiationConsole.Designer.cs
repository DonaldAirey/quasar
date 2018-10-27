namespace MarkThree.Guardian.Forms
{
	partial class NegotiationConsole
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
            this.labelQuantity = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.buttonTrade = new System.Windows.Forms.Button();
            this.buttonPass = new System.Windows.Forms.Button();
            this.clientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.labelMinimum = new System.Windows.Forms.Label();
            this.labelMinimumQuantity = new System.Windows.Forms.Label();
            this.labelLeaves = new System.Windows.Forms.Label();
            this.labelLeavesQuantity = new System.Windows.Forms.Label();
            this.decimalDomainQuantity = new MarkThree.Forms.DecimalDomain();
            this.labelTicker = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelQuantity
            // 
            this.labelQuantity.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelQuantity.Enabled = false;
            this.labelQuantity.Location = new System.Drawing.Point(11, 37);
            this.labelQuantity.Name = "labelQuantity";
            this.labelQuantity.Size = new System.Drawing.Size(161, 25);
            this.labelQuantity.TabIndex = 1;
            this.labelQuantity.Text = "Quantity";
            this.labelQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxLogo.Location = new System.Drawing.Point(177, 103);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(135, 52);
            this.pictureBoxLogo.TabIndex = 6;
            this.pictureBoxLogo.TabStop = false;
            // 
            // buttonTrade
            // 
            this.buttonTrade.Enabled = false;
            this.buttonTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTrade.Location = new System.Drawing.Point(178, 66);
            this.buttonTrade.Name = "buttonTrade";
            this.buttonTrade.Size = new System.Drawing.Size(64, 29);
            this.buttonTrade.TabIndex = 2;
            this.buttonTrade.Text = "Go";
            this.buttonTrade.UseVisualStyleBackColor = true;
            this.buttonTrade.Click += new System.EventHandler(this.buttonTrade_Click);
            // 
            // buttonPass
            // 
            this.buttonPass.Enabled = false;
            this.buttonPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPass.Location = new System.Drawing.Point(248, 66);
            this.buttonPass.Name = "buttonPass";
            this.buttonPass.Size = new System.Drawing.Size(64, 29);
            this.buttonPass.TabIndex = 4;
            this.buttonPass.Text = "Pass";
            this.buttonPass.UseVisualStyleBackColor = true;
            this.buttonPass.Click += new System.EventHandler(this.buttonPass_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // labelMinimum
            // 
            this.labelMinimum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMinimum.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelMinimum.Enabled = false;
            this.labelMinimum.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelMinimum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinimum.ForeColor = System.Drawing.Color.Blue;
            this.labelMinimum.Location = new System.Drawing.Point(11, 68);
            this.labelMinimum.Name = "labelMinimum";
            this.labelMinimum.Size = new System.Drawing.Size(161, 25);
            this.labelMinimum.TabIndex = 55;
            this.labelMinimum.Text = "Minimum";
            this.labelMinimum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelMinimum.Click += new System.EventHandler(this.labelMinimum_Click);
            // 
            // labelMinimumQuantity
            // 
            this.labelMinimumQuantity.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelMinimumQuantity.Location = new System.Drawing.Point(71, 74);
            this.labelMinimumQuantity.Name = "labelMinimumQuantity";
            this.labelMinimumQuantity.Size = new System.Drawing.Size(70, 13);
            this.labelMinimumQuantity.TabIndex = 56;
            this.labelMinimumQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelMinimumQuantity.Click += new System.EventHandler(this.labelMinimum_Click);
            // 
            // labelLeaves
            // 
            this.labelLeaves.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelLeaves.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelLeaves.Enabled = false;
            this.labelLeaves.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLeaves.ForeColor = System.Drawing.Color.Blue;
            this.labelLeaves.Location = new System.Drawing.Point(11, 6);
            this.labelLeaves.Name = "labelLeaves";
            this.labelLeaves.Size = new System.Drawing.Size(161, 25);
            this.labelLeaves.TabIndex = 57;
            this.labelLeaves.Text = "Leaves";
            this.labelLeaves.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelLeaves.Click += new System.EventHandler(this.labelLeaves_Click);
            // 
            // labelLeavesQuantity
            // 
            this.labelLeavesQuantity.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelLeavesQuantity.Location = new System.Drawing.Point(71, 12);
            this.labelLeavesQuantity.Name = "labelLeavesQuantity";
            this.labelLeavesQuantity.Size = new System.Drawing.Size(70, 13);
            this.labelLeavesQuantity.TabIndex = 58;
            this.labelLeavesQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelLeavesQuantity.Click += new System.EventHandler(this.labelLeaves_Click);
            // 
            // decimalDomainQuantity
            // 
            this.decimalDomainQuantity.CustomFormat = "#,##0";
            this.decimalDomainQuantity.CustomUnit = "";
            this.decimalDomainQuantity.Enabled = false;
            this.decimalDomainQuantity.Interval = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalDomainQuantity.Location = new System.Drawing.Point(68, 39);
            this.decimalDomainQuantity.Name = "decimalDomainQuantity";
            this.decimalDomainQuantity.Size = new System.Drawing.Size(92, 20);
            this.decimalDomainQuantity.StartDecimal = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalDomainQuantity.StopDecimal = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.decimalDomainQuantity.TabIndex = 59;
            this.decimalDomainQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelTicker
            // 
            this.labelTicker.Font = new System.Drawing.Font("Times New Roman", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTicker.Location = new System.Drawing.Point(179, 6);
            this.labelTicker.Name = "labelTicker";
            this.labelTicker.Size = new System.Drawing.Size(133, 56);
            this.labelTicker.TabIndex = 60;
            this.labelTicker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelName
            // 
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(11, 103);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(160, 52);
            this.labelName.TabIndex = 61;
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NegotiationConsole
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.labelTicker);
            this.Controls.Add(this.decimalDomainQuantity);
            this.Controls.Add(this.labelLeavesQuantity);
            this.Controls.Add(this.labelLeaves);
            this.Controls.Add(this.labelMinimumQuantity);
            this.Controls.Add(this.labelMinimum);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.buttonPass);
            this.Controls.Add(this.buttonTrade);
            this.Controls.Add(this.labelQuantity);
            this.Name = "NegotiationConsole";
            this.Size = new System.Drawing.Size(324, 158);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelQuantity;
		private System.Windows.Forms.PictureBox pictureBoxLogo;
		private System.Windows.Forms.Button buttonTrade;
		private System.Windows.Forms.Button buttonPass;
		private MarkThree.Guardian.Client.ClientMarketData clientMarketData;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Label labelMinimum;
		private System.Windows.Forms.Label labelMinimumQuantity;
		private System.Windows.Forms.Label labelLeaves;
		private System.Windows.Forms.Label labelLeavesQuantity;
		private MarkThree.Forms.DecimalDomain decimalDomainQuantity;
        private System.Windows.Forms.Label labelTicker;
        private System.Windows.Forms.Label labelName;

	}
}
