namespace MarkThree.Guardian.Forms
{
	partial class FormOrder
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
			this.labelOrderType = new System.Windows.Forms.Label();
			this.labelQuantity = new System.Windows.Forms.Label();
			this.labelSymbol = new System.Windows.Forms.Label();
			this.labelPrice = new System.Windows.Forms.Label();
			this.textBoxPrice = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelNewsFreeTime = new System.Windows.Forms.Label();
			this.integerPickerNewsFreeTime = new MarkThree.Forms.IntegerPicker();
			this.checkBoxHedgeFund = new System.Windows.Forms.CheckBox();
			this.checkBoxInstitution = new System.Windows.Forms.CheckBox();
			this.percentPickerMaximumVolatility = new MarkThree.Forms.PercentPicker();
			this.labelMaximumVolatility = new System.Windows.Forms.Label();
			this.timePickerStopTime = new MarkThree.Forms.TimePicker();
			this.timePickerStartTime = new MarkThree.Forms.TimePicker();
			this.checkBoxBroker = new System.Windows.Forms.CheckBox();
			this.labelStopTime = new System.Windows.Forms.Label();
			this.labelStartTime = new System.Windows.Forms.Label();
			this.buttonPost = new System.Windows.Forms.Button();
			this.clientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
			this.quantityBox = new MarkThree.Forms.QuantityBox();
			this.orderTypePicker = new MarkThree.Guardian.Forms.OrderTypePicker();
			this.symbolBox = new MarkThree.Guardian.Forms.SymbolBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelOrderType
			// 
			this.labelOrderType.AutoSize = true;
			this.labelOrderType.Location = new System.Drawing.Point(12, 11);
			this.labelOrderType.Name = "labelOrderType";
			this.labelOrderType.Size = new System.Drawing.Size(31, 13);
			this.labelOrderType.TabIndex = 0;
			this.labelOrderType.Text = "&Side:";
			this.labelOrderType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelQuantity
			// 
			this.labelQuantity.AutoSize = true;
			this.labelQuantity.Location = new System.Drawing.Point(146, 11);
			this.labelQuantity.Name = "labelQuantity";
			this.labelQuantity.Size = new System.Drawing.Size(49, 13);
			this.labelQuantity.TabIndex = 2;
			this.labelQuantity.Text = "&Quantity:";
			this.labelQuantity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSymbol
			// 
			this.labelSymbol.AutoSize = true;
			this.labelSymbol.Location = new System.Drawing.Point(273, 11);
			this.labelSymbol.Name = "labelSymbol";
			this.labelSymbol.Size = new System.Drawing.Size(44, 13);
			this.labelSymbol.TabIndex = 4;
			this.labelSymbol.Text = "&Symbol:";
			this.labelSymbol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPrice
			// 
			this.labelPrice.AutoSize = true;
			this.labelPrice.Location = new System.Drawing.Point(391, 11);
			this.labelPrice.Name = "labelPrice";
			this.labelPrice.Size = new System.Drawing.Size(34, 13);
			this.labelPrice.TabIndex = 6;
			this.labelPrice.Text = "&Price:";
			this.labelPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxPrice
			// 
			this.textBoxPrice.Location = new System.Drawing.Point(431, 7);
			this.textBoxPrice.Name = "textBoxPrice";
			this.textBoxPrice.ReadOnly = true;
			this.textBoxPrice.Size = new System.Drawing.Size(100, 20);
			this.textBoxPrice.TabIndex = 7;
			this.textBoxPrice.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelNewsFreeTime);
			this.groupBox1.Controls.Add(this.integerPickerNewsFreeTime);
			this.groupBox1.Controls.Add(this.checkBoxHedgeFund);
			this.groupBox1.Controls.Add(this.checkBoxInstitution);
			this.groupBox1.Controls.Add(this.percentPickerMaximumVolatility);
			this.groupBox1.Controls.Add(this.labelMaximumVolatility);
			this.groupBox1.Controls.Add(this.timePickerStopTime);
			this.groupBox1.Controls.Add(this.timePickerStartTime);
			this.groupBox1.Controls.Add(this.checkBoxBroker);
			this.groupBox1.Controls.Add(this.labelStopTime);
			this.groupBox1.Controls.Add(this.labelStartTime);
			this.groupBox1.Location = new System.Drawing.Point(15, 35);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(516, 68);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Matching";
			// 
			// labelNewsFreeTime
			// 
			this.labelNewsFreeTime.AutoSize = true;
			this.labelNewsFreeTime.Location = new System.Drawing.Point(174, 42);
			this.labelNewsFreeTime.Name = "labelNewsFreeTime";
			this.labelNewsFreeTime.Size = new System.Drawing.Size(61, 13);
			this.labelNewsFreeTime.TabIndex = 6;
			this.labelNewsFreeTime.Text = "News Free:";
			// 
			// integerPickerNewsFreeTime
			// 
			this.integerPickerNewsFreeTime.CustomFormat = "0";
			this.integerPickerNewsFreeTime.CustomUnit = "min.";
			this.integerPickerNewsFreeTime.Interval = 5;
			this.integerPickerNewsFreeTime.Location = new System.Drawing.Point(241, 40);
			this.integerPickerNewsFreeTime.Name = "integerPickerNewsFreeTime";
			this.integerPickerNewsFreeTime.Size = new System.Drawing.Size(85, 20);
			this.integerPickerNewsFreeTime.StartInteger = 0;
			this.integerPickerNewsFreeTime.StopInteger = 60;
			this.integerPickerNewsFreeTime.TabIndex = 7;
			// 
			// checkBoxHedgeFund
			// 
			this.checkBoxHedgeFund.AutoSize = true;
			this.checkBoxHedgeFund.Location = new System.Drawing.Point(332, 41);
			this.checkBoxHedgeFund.Name = "checkBoxHedgeFund";
			this.checkBoxHedgeFund.Size = new System.Drawing.Size(85, 17);
			this.checkBoxHedgeFund.TabIndex = 9;
			this.checkBoxHedgeFund.Text = "Hedge Fund";
			this.checkBoxHedgeFund.UseVisualStyleBackColor = true;
			// 
			// checkBoxInstitution
			// 
			this.checkBoxInstitution.AutoSize = true;
			this.checkBoxInstitution.Location = new System.Drawing.Point(332, 15);
			this.checkBoxInstitution.Name = "checkBoxInstitution";
			this.checkBoxInstitution.Size = new System.Drawing.Size(67, 17);
			this.checkBoxInstitution.TabIndex = 8;
			this.checkBoxInstitution.Text = "Institition";
			this.checkBoxInstitution.UseVisualStyleBackColor = true;
			// 
			// percentPickerMaximumVolatility
			// 
			this.percentPickerMaximumVolatility.CustomFormat = "0.0%";
			this.percentPickerMaximumVolatility.Interval = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.percentPickerMaximumVolatility.Location = new System.Drawing.Point(241, 14);
			this.percentPickerMaximumVolatility.Name = "percentPickerMaximumVolatility";
			this.percentPickerMaximumVolatility.Size = new System.Drawing.Size(85, 20);
			this.percentPickerMaximumVolatility.StartPercent = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.percentPickerMaximumVolatility.StopPercent = new decimal(new int[] {
            5,
            0,
            0,
            131072});
			this.percentPickerMaximumVolatility.TabIndex = 5;
			// 
			// labelMaximumVolatility
			// 
			this.labelMaximumVolatility.AutoSize = true;
			this.labelMaximumVolatility.Location = new System.Drawing.Point(161, 16);
			this.labelMaximumVolatility.Name = "labelMaximumVolatility";
			this.labelMaximumVolatility.Size = new System.Drawing.Size(74, 13);
			this.labelMaximumVolatility.TabIndex = 4;
			this.labelMaximumVolatility.Text = "Max. &Volatility:";
			// 
			// timePickerStopTime
			// 
			this.timePickerStopTime.CustomFormat = "h:mm tt";
			this.timePickerStopTime.Interval = System.TimeSpan.Parse("00:15:00");
			this.timePickerStopTime.Location = new System.Drawing.Point(70, 40);
			this.timePickerStopTime.Name = "timePickerStopTime";
			this.timePickerStopTime.Size = new System.Drawing.Size(85, 20);
			this.timePickerStopTime.StartTime = new System.DateTime(2005, 12, 19, 8, 0, 0, 0);
			this.timePickerStopTime.StopFormattedDateTime = new System.DateTime(2005, 12, 19, 18, 0, 0, 0);
			this.timePickerStopTime.TabIndex = 3;
			// 
			// timePickerStartTime
			// 
			this.timePickerStartTime.CustomFormat = "h:mm tt";
			this.timePickerStartTime.Interval = System.TimeSpan.Parse("00:15:00");
			this.timePickerStartTime.Location = new System.Drawing.Point(70, 14);
			this.timePickerStartTime.Name = "timePickerStartTime";
			this.timePickerStartTime.Size = new System.Drawing.Size(85, 20);
			this.timePickerStartTime.StartTime = new System.DateTime(2005, 12, 19, 8, 0, 0, 0);
			this.timePickerStartTime.StopFormattedDateTime = new System.DateTime(2005, 12, 19, 18, 0, 0, 0);
			this.timePickerStartTime.TabIndex = 1;
			// 
			// checkBoxBroker
			// 
			this.checkBoxBroker.AutoSize = true;
			this.checkBoxBroker.Location = new System.Drawing.Point(423, 15);
			this.checkBoxBroker.Name = "checkBoxBroker";
			this.checkBoxBroker.Size = new System.Drawing.Size(57, 17);
			this.checkBoxBroker.TabIndex = 10;
			this.checkBoxBroker.Text = "Broker";
			this.checkBoxBroker.UseVisualStyleBackColor = true;
			// 
			// labelStopTime
			// 
			this.labelStopTime.AutoSize = true;
			this.labelStopTime.Location = new System.Drawing.Point(6, 42);
			this.labelStopTime.Name = "labelStopTime";
			this.labelStopTime.Size = new System.Drawing.Size(58, 13);
			this.labelStopTime.TabIndex = 2;
			this.labelStopTime.Text = "St&op Time:";
			this.labelStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStartTime
			// 
			this.labelStartTime.AutoSize = true;
			this.labelStartTime.Location = new System.Drawing.Point(6, 16);
			this.labelStartTime.Name = "labelStartTime";
			this.labelStartTime.Size = new System.Drawing.Size(58, 13);
			this.labelStartTime.TabIndex = 0;
			this.labelStartTime.Text = "St&art Time:";
			this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonPost
			// 
			this.buttonPost.Location = new System.Drawing.Point(537, 6);
			this.buttonPost.Name = "buttonPost";
			this.buttonPost.Size = new System.Drawing.Size(75, 23);
			this.buttonPost.TabIndex = 8;
			this.buttonPost.Text = "Post";
			this.buttonPost.UseVisualStyleBackColor = true;
			this.buttonPost.Click += new System.EventHandler(this.buttonPost_Click);
			// 
			// quantityBox
			// 
			this.quantityBox.CustomFormat = "#,##0";
			this.quantityBox.Location = new System.Drawing.Point(198, 7);
			this.quantityBox.Name = "quantityBox";
			this.quantityBox.Size = new System.Drawing.Size(69, 20);
			this.quantityBox.TabIndex = 3;
			// 
			// orderTypePicker
			// 
			this.orderTypePicker.Location = new System.Drawing.Point(50, 7);
			this.orderTypePicker.Name = "orderTypePicker";
			this.orderTypePicker.Size = new System.Drawing.Size(90, 20);
			this.orderTypePicker.TabIndex = 1;
			// 
			// symbolBox
			// 
			this.symbolBox.Location = new System.Drawing.Point(323, 7);
			this.symbolBox.Name = "symbolBox";
			this.symbolBox.SelectedSecurityId = "";
			this.symbolBox.Size = new System.Drawing.Size(62, 20);
			this.symbolBox.TabIndex = 5;
			// 
			// FormOrder
			// 
			this.AcceptButton = this.buttonPost;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(628, 114);
			this.Controls.Add(this.quantityBox);
			this.Controls.Add(this.orderTypePicker);
			this.Controls.Add(this.buttonPost);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBoxPrice);
			this.Controls.Add(this.labelPrice);
			this.Controls.Add(this.symbolBox);
			this.Controls.Add(this.labelSymbol);
			this.Controls.Add(this.labelQuantity);
			this.Controls.Add(this.labelOrderType);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOrder";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Manual Order";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelOrderType;
		private System.Windows.Forms.Label labelQuantity;
		private System.Windows.Forms.Label labelSymbol;
		private SymbolBox symbolBox;
		private System.Windows.Forms.Label labelPrice;
		private System.Windows.Forms.TextBox textBoxPrice;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelStartTime;
		private System.Windows.Forms.Label labelStopTime;
		private MarkThree.Forms.PercentPicker percentPickerMaximumVolatility;
		private System.Windows.Forms.Label labelMaximumVolatility;
		private MarkThree.Forms.TimePicker timePickerStopTime;
		private MarkThree.Forms.TimePicker timePickerStartTime;
		private System.Windows.Forms.CheckBox checkBoxHedgeFund;
		private System.Windows.Forms.CheckBox checkBoxInstitution;
		private System.Windows.Forms.Button buttonPost;
		private System.Windows.Forms.CheckBox checkBoxBroker;
		private OrderTypePicker orderTypePicker;
		private System.Windows.Forms.Label labelNewsFreeTime;
		private MarkThree.Forms.IntegerPicker integerPickerNewsFreeTime;
		private MarkThree.Guardian.Client.ClientMarketData clientMarketData;
		private MarkThree.Forms.QuantityBox quantityBox;
	}
}