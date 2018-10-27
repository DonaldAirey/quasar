namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Forms;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Form for selecting user options and preferences.
	/// </summary>
	public class Options : System.Windows.Forms.Form
	{

        // Private Members
		private MarkThree.Guardian.Forms.Options.InitializeDelegate initializeDelegate;
		private MarkThree.Guardian.Forms.Options.ApplyEndDelegate applyEndDelegate;
		private System.ComponentModel.IContainer components = new System.ComponentModel.Container();
		private System.Windows.Forms.CheckBox checkBoxBroker;
		private System.Windows.Forms.CheckBox checkBoxHedge;
		private System.Windows.Forms.CheckBox checkBoxInstitution;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPagePreferences;
		private System.Windows.Forms.TabPage tabPageMatch;
		private System.Windows.Forms.Label labelMaximumVolatility;
		private System.Windows.Forms.Label labelNewsFreeTime;
		private System.Windows.Forms.Label labelStopTime;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxMatchAgainst;
		private System.Threading.ManualResetEvent handleCreatedEvent;
		private Label labelStartTime;
		private TimePicker timePickerStartTime;
		private TimePicker timePickerStopTime;
		private PercentPicker percentPickerVolatility;
		private IntegerPicker integerPickerNewsFreeTime;
        private ADVSlider sliderLow;
        private ADVSlider sliderMid;
        private ADVSlider sliderHigh;
        private Label label1;
        private GroupBox groupBox1;
        private TimeRangeSlider timeRangeSlider;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
		private System.Windows.Forms.Button buttonApply;
        private IntegerPicker marketSleepPicker;
        private IntegerPicker reviewPicker;
        private Hashtable volumeCategories; // hash of mnemonic --> volume category (min, max, mnemonic, description)

		private delegate void InitializeDelegate(TraderOptions traderOptions);
		private delegate void ApplyEndDelegate(bool exitDialog, BatchException batchException);

		/// <summary>
		/// Creates a form used to prompt the user for options and preferences.
		/// </summary>
		public Options()
		{
            // This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// This event is used to hold up the background initialization of the form until a window handle has been created.  If 
			// the background thread doesn't wait for this signal, it can cause an exception when trying to pass information to the
			// foreground using the 'Invoke' commands.
			this.handleCreatedEvent = new ManualResetEvent(false);

			// Delegates for handling Windows thread actions from the background.
			this.initializeDelegate = new InitializeDelegate(InitializeDialog);
			this.applyEndDelegate = new ApplyEndDelegate(ApplyEnd);

#if DEBUG
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// Launch a thread to initialize the dialog.  This thread will read the data from the data model, fill in a 
				// structure and call the foreground delegate that will finish the job of filling in the form.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeDialogThread));

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			// Destroy the managed resources.
			if (disposing)
				if (components != null)
					components.Dispose();

			// Allow the base class to complete operation.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMatch = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelMaximumVolatility = new System.Windows.Forms.Label();
            this.labelNewsFreeTime = new System.Windows.Forms.Label();
            this.labelStopTime = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.groupBoxMatchAgainst = new System.Windows.Forms.GroupBox();
            this.checkBoxBroker = new System.Windows.Forms.CheckBox();
            this.checkBoxInstitution = new System.Windows.Forms.CheckBox();
            this.checkBoxHedge = new System.Windows.Forms.CheckBox();
            this.tabPagePreferences = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.reviewPicker = new MarkThree.Forms.IntegerPicker();
            this.marketSleepPicker = new MarkThree.Forms.IntegerPicker();
            this.timeRangeSlider = new MarkThree.Forms.TimeRangeSlider();
            this.percentPickerVolatility = new MarkThree.Forms.PercentPicker();
            this.integerPickerNewsFreeTime = new MarkThree.Forms.IntegerPicker();
            this.timePickerStopTime = new MarkThree.Forms.TimePicker();
            this.timePickerStartTime = new MarkThree.Forms.TimePicker();
            this.sliderLow = new MarkThree.Forms.ADVSlider();
            this.sliderMid = new MarkThree.Forms.ADVSlider();
            this.sliderHigh = new MarkThree.Forms.ADVSlider();
            this.tabControl1.SuspendLayout();
            this.tabPageMatch.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxMatchAgainst.SuspendLayout();
            this.tabPagePreferences.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMatch);
            this.tabControl1.Controls.Add(this.tabPagePreferences);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(441, 425);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageMatch
            // 
            this.tabPageMatch.Controls.Add(this.groupBox3);
            this.tabPageMatch.Controls.Add(this.groupBox2);
            this.tabPageMatch.Controls.Add(this.groupBox1);
            this.tabPageMatch.Controls.Add(this.groupBoxMatchAgainst);
            this.tabPageMatch.Location = new System.Drawing.Point(4, 22);
            this.tabPageMatch.Name = "tabPageMatch";
            this.tabPageMatch.Size = new System.Drawing.Size(433, 399);
            this.tabPageMatch.TabIndex = 1;
            this.tabPageMatch.Text = "Match";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.reviewPicker);
            this.groupBox3.Location = new System.Drawing.Point(231, 233);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(165, 62);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Review Window";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.marketSleepPicker);
            this.groupBox2.Location = new System.Drawing.Point(9, 233);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(165, 62);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Market Sleep";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.timeRangeSlider);
            this.groupBox1.Controls.Add(this.percentPickerVolatility);
            this.groupBox1.Controls.Add(this.labelMaximumVolatility);
            this.groupBox1.Controls.Add(this.labelNewsFreeTime);
            this.groupBox1.Controls.Add(this.integerPickerNewsFreeTime);
            this.groupBox1.Controls.Add(this.labelStopTime);
            this.groupBox1.Controls.Add(this.labelStartTime);
            this.groupBox1.Controls.Add(this.timePickerStopTime);
            this.groupBox1.Controls.Add(this.timePickerStartTime);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 215);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Eligibility";
            // 
            // labelMaximumVolatility
            // 
            this.labelMaximumVolatility.AutoSize = true;
            this.labelMaximumVolatility.Location = new System.Drawing.Point(102, 23);
            this.labelMaximumVolatility.Name = "labelMaximumVolatility";
            this.labelMaximumVolatility.Size = new System.Drawing.Size(95, 13);
            this.labelMaximumVolatility.TabIndex = 4;
            this.labelMaximumVolatility.Text = "Maximum &Volatility:";
            this.labelMaximumVolatility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelMaximumVolatility.Visible = false;
            // 
            // labelNewsFreeTime
            // 
            this.labelNewsFreeTime.AutoSize = true;
            this.labelNewsFreeTime.Location = new System.Drawing.Point(110, 47);
            this.labelNewsFreeTime.Name = "labelNewsFreeTime";
            this.labelNewsFreeTime.Size = new System.Drawing.Size(87, 13);
            this.labelNewsFreeTime.TabIndex = 6;
            this.labelNewsFreeTime.Text = "&News Free Time:";
            this.labelNewsFreeTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelNewsFreeTime.Visible = false;
            // 
            // labelStopTime
            // 
            this.labelStopTime.AutoSize = true;
            this.labelStopTime.Location = new System.Drawing.Point(112, 47);
            this.labelStopTime.Name = "labelStopTime";
            this.labelStopTime.Size = new System.Drawing.Size(58, 13);
            this.labelStopTime.TabIndex = 2;
            this.labelStopTime.Text = "S&top Time:";
            this.labelStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelStopTime.Visible = false;
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(112, 23);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(58, 13);
            this.labelStartTime.TabIndex = 0;
            this.labelStartTime.Text = "&Start Time:";
            this.labelStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelStartTime.Visible = false;
            // 
            // groupBoxMatchAgainst
            // 
            this.groupBoxMatchAgainst.Controls.Add(this.checkBoxBroker);
            this.groupBoxMatchAgainst.Controls.Add(this.checkBoxInstitution);
            this.groupBoxMatchAgainst.Controls.Add(this.checkBoxHedge);
            this.groupBoxMatchAgainst.Location = new System.Drawing.Point(9, 318);
            this.groupBoxMatchAgainst.Name = "groupBoxMatchAgainst";
            this.groupBoxMatchAgainst.Size = new System.Drawing.Size(387, 46);
            this.groupBoxMatchAgainst.TabIndex = 8;
            this.groupBoxMatchAgainst.TabStop = false;
            this.groupBoxMatchAgainst.Text = "Match Against";
            // 
            // checkBoxBroker
            // 
            this.checkBoxBroker.AutoSize = true;
            this.checkBoxBroker.Location = new System.Drawing.Point(324, 19);
            this.checkBoxBroker.Name = "checkBoxBroker";
            this.checkBoxBroker.Size = new System.Drawing.Size(57, 17);
            this.checkBoxBroker.TabIndex = 5;
            this.checkBoxBroker.Text = "Broker";
            // 
            // checkBoxInstitution
            // 
            this.checkBoxInstitution.AutoSize = true;
            this.checkBoxInstitution.Location = new System.Drawing.Point(6, 19);
            this.checkBoxInstitution.Name = "checkBoxInstitution";
            this.checkBoxInstitution.Size = new System.Drawing.Size(71, 17);
            this.checkBoxInstitution.TabIndex = 0;
            this.checkBoxInstitution.Text = "Institution";
            // 
            // checkBoxHedge
            // 
            this.checkBoxHedge.AutoSize = true;
            this.checkBoxHedge.Location = new System.Drawing.Point(151, 19);
            this.checkBoxHedge.Name = "checkBoxHedge";
            this.checkBoxHedge.Size = new System.Drawing.Size(85, 17);
            this.checkBoxHedge.TabIndex = 1;
            this.checkBoxHedge.Text = "Hedge Fund";
            // 
            // tabPagePreferences
            // 
            this.tabPagePreferences.Controls.Add(this.sliderLow);
            this.tabPagePreferences.Controls.Add(this.sliderMid);
            this.tabPagePreferences.Controls.Add(this.sliderHigh);
            this.tabPagePreferences.Controls.Add(this.label1);
            this.tabPagePreferences.Location = new System.Drawing.Point(4, 22);
            this.tabPagePreferences.Name = "tabPagePreferences";
            this.tabPagePreferences.Size = new System.Drawing.Size(433, 399);
            this.tabPagePreferences.TabIndex = 0;
            this.tabPagePreferences.Text = "Liquidity";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "ADV Liquidity Buckets";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(111, 439);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(191, 439);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(271, 439);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "Apply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // reviewPicker
            // 
            this.reviewPicker.CustomFormat = "0";
            this.reviewPicker.CustomUnit = "Minutes";
            this.reviewPicker.Interval = 5;
            this.reviewPicker.Location = new System.Drawing.Point(13, 23);
            this.reviewPicker.Name = "reviewPicker";
            this.reviewPicker.Size = new System.Drawing.Size(117, 20);
            this.reviewPicker.StartInteger = 0;
            this.reviewPicker.StopInteger = 60;
            this.reviewPicker.TabIndex = 2;
            // 
            // marketSleepPicker
            // 
            this.marketSleepPicker.CustomFormat = "0";
            this.marketSleepPicker.CustomUnit = "Minutes";
            this.marketSleepPicker.Interval = 5;
            this.marketSleepPicker.Location = new System.Drawing.Point(13, 23);
            this.marketSleepPicker.Name = "marketSleepPicker";
            this.marketSleepPicker.Size = new System.Drawing.Size(117, 20);
            this.marketSleepPicker.StartInteger = 0;
            this.marketSleepPicker.StopInteger = 60;
            this.marketSleepPicker.TabIndex = 1;
            // 
            // timeRangeSlider
            // 
            this.timeRangeSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeRangeSlider.Location = new System.Drawing.Point(5, 17);
            this.timeRangeSlider.Name = "timeRangeSlider";
            this.timeRangeSlider.Size = new System.Drawing.Size(410, 188);
            this.timeRangeSlider.TabIndex = 0;
            // 
            // percentPickerVolatility
            // 
            this.percentPickerVolatility.CustomFormat = "0.0%";
            this.percentPickerVolatility.Interval = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.percentPickerVolatility.Location = new System.Drawing.Point(204, 19);
            this.percentPickerVolatility.Name = "percentPickerVolatility";
            this.percentPickerVolatility.Size = new System.Drawing.Size(104, 20);
            this.percentPickerVolatility.StartPercent = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.percentPickerVolatility.StopPercent = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.percentPickerVolatility.TabIndex = 5;
            this.percentPickerVolatility.Visible = false;
            // 
            // integerPickerNewsFreeTime
            // 
            this.integerPickerNewsFreeTime.CustomFormat = "0";
            this.integerPickerNewsFreeTime.CustomUnit = "minutes";
            this.integerPickerNewsFreeTime.Interval = 5;
            this.integerPickerNewsFreeTime.Location = new System.Drawing.Point(202, 43);
            this.integerPickerNewsFreeTime.Name = "integerPickerNewsFreeTime";
            this.integerPickerNewsFreeTime.Size = new System.Drawing.Size(104, 20);
            this.integerPickerNewsFreeTime.StartInteger = 0;
            this.integerPickerNewsFreeTime.StopInteger = 120;
            this.integerPickerNewsFreeTime.TabIndex = 7;
            this.integerPickerNewsFreeTime.Visible = false;
            // 
            // timePickerStopTime
            // 
            this.timePickerStopTime.CustomFormat = "h:mm tt";
            this.timePickerStopTime.Interval = System.TimeSpan.Parse("00:15:00");
            this.timePickerStopTime.Location = new System.Drawing.Point(175, 43);
            this.timePickerStopTime.Name = "timePickerStopTime";
            this.timePickerStopTime.Size = new System.Drawing.Size(104, 20);
            this.timePickerStopTime.StartTime = new System.DateTime(2005, 12, 17, 8, 0, 0, 0);
            this.timePickerStopTime.StopFormattedDateTime = new System.DateTime(2005, 12, 17, 18, 0, 0, 0);
            this.timePickerStopTime.TabIndex = 3;
            this.timePickerStopTime.Visible = false;
            // 
            // timePickerStartTime
            // 
            this.timePickerStartTime.CustomFormat = "h:mm tt";
            this.timePickerStartTime.Interval = System.TimeSpan.Parse("00:15:00");
            this.timePickerStartTime.Location = new System.Drawing.Point(175, 19);
            this.timePickerStartTime.Name = "timePickerStartTime";
            this.timePickerStartTime.Size = new System.Drawing.Size(104, 20);
            this.timePickerStartTime.StartTime = new System.DateTime(2005, 12, 17, 8, 0, 0, 0);
            this.timePickerStartTime.StopFormattedDateTime = new System.DateTime(2005, 12, 17, 18, 0, 0, 0);
            this.timePickerStartTime.TabIndex = 1;
            this.timePickerStartTime.Visible = false;
            // 
            // sliderLow
            // 
            this.sliderLow.AutoExecute = 50000;
            this.sliderLow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sliderLow.BucketSharesLabel = "Over 250K shares";
            this.sliderLow.Location = new System.Drawing.Point(12, 274);
            this.sliderLow.Margin = new System.Windows.Forms.Padding(0);
            this.sliderLow.Maximum = 5000000;
            this.sliderLow.Minimum = 10000;
            this.sliderLow.Name = "sliderLow";
            this.sliderLow.Size = new System.Drawing.Size(410, 117);
            this.sliderLow.TabIndex = 3;
            this.sliderLow.Threshold = this.sliderLow.Minimum;
            // 
            // sliderMid
            // 
            this.sliderMid.AutoExecute = 125000;
            this.sliderMid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sliderMid.BucketLabel = "Mid";
            this.sliderMid.BucketSharesLabel = "Over 1M shares";
            this.sliderMid.Location = new System.Drawing.Point(12, 153);
            this.sliderMid.Margin = new System.Windows.Forms.Padding(0);
            this.sliderMid.Maximum = 250000;
            this.sliderMid.Minimum = 25000;
            this.sliderMid.Name = "sliderMid";
            this.sliderMid.Size = new System.Drawing.Size(410, 117);
            this.sliderMid.TabIndex = 2;
            this.sliderMid.Threshold = this.sliderMid.Minimum;
            this.sliderMid.Load += new System.EventHandler(this.sliderMid_Load);
            // 
            // sliderHigh
            // 
            this.sliderHigh.AutoExecute = 250000;
            this.sliderHigh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sliderHigh.BucketLabel = "High";
            this.sliderHigh.BucketSharesLabel = "Over 5M shares";
            this.sliderHigh.Location = new System.Drawing.Point(12, 32);
            this.sliderHigh.Margin = new System.Windows.Forms.Padding(0);
            this.sliderHigh.Maximum = 500000;
            this.sliderHigh.Minimum = 50000;
            this.sliderHigh.Name = "sliderHigh";
            this.sliderHigh.Size = new System.Drawing.Size(410, 117);
            this.sliderHigh.TabIndex = 1;
            this.sliderHigh.Threshold = this.sliderHigh.Minimum;
            // 
            // Options
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(449, 468);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageMatch.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxMatchAgainst.ResumeLayout(false);
            this.groupBoxMatchAgainst.PerformLayout();
            this.tabPagePreferences.ResumeLayout(false);
            this.tabPagePreferences.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Displays the User Options Property Pages.
		/// </summary>
		/// <param name="iWin32Window">The owner window of this dialog.</param>
		public new void Show(IWin32Window iWin32Window)
		{

			// Display the dialog.  When the dialog is dismissed by the user, dispose of it explicitly, otherwise the resources
			// tend to hang around until the garbage is taken out.
			ShowDialog(iWin32Window);
			Dispose();

		}

		/// <summary>
		/// Raised when the window has completed the initialization.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{

			// The initialization thread can complete when the handle for this window has been created.
			this.handleCreatedEvent.Set();

			// Allow the base class to complete the initialization of the window.
			base.OnHandleCreated(e);

		}

        private void InitializeVolumeCategory()
        {
            try
            {
                // Lock the tables
                System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
                ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.VolumeCategoryLock.AcquireReaderLock(CommonTimeout.LockWait);

                // create a hashtable to store the volume categories
                volumeCategories = new Hashtable(ClientMarketData.VolumeCategory.Rows.Count);

                // iterate through the records in the VolumeCategory to find the min/max ranges and mnemonics for each volume category
                foreach (ClientMarketData.VolumeCategoryRow volumeRow in ClientMarketData.VolumeCategory)
                {
                    // create a new volume category struct
                    VolumeCategory volumeCategory = new VolumeCategory();
                    
                    // set the MinRange value to either the value in the data model, or 0
                    if (volumeRow[ClientMarketData.VolumeCategory.LowVolumeRangeColumn] is DBNull)
                    {
                        volumeCategory.MinRange = 0;
                    }
                    else
                    {
                        volumeCategory.MinRange = (int)volumeRow.LowVolumeRange;
                    }

                    // set the MaxRange value to either the value in the data model, or 5 * the minimum
                    if (volumeRow[ClientMarketData.VolumeCategory.HighVolumeRangeColumn] is DBNull)
                    {
                        volumeCategory.MaxRange = 5 * volumeCategory.MinRange;
                    }
                    else
                    {
                        volumeCategory.MaxRange = (int)volumeRow.HighVolumeRange;
                    }      

                    volumeCategory.Mnemonic = volumeRow.Mnemonic;
                    volumeCategory.VolumeCategoryId = volumeRow.VolumeCategoryId;
                    volumeCategory.Description = volumeRow.Description;

                    if (volumeCategory.Mnemonic.Length > 0)
                    {
                        // save the volume category in the hash.
                        volumeCategories[volumeCategory.Mnemonic] = volumeCategory;
                    }
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Exception occurred initializing volume categories: " + exception.Message);
            }
            finally
            {

                // Release the locks
                if (ClientMarketData.ObjectLock.IsReaderLockHeld)
                    ClientMarketData.ObjectLock.ReleaseReaderLock();
                if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld)
                    ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
                System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
            }

        }

		/// <summary>
		/// Reads the contents of the form from the data model.
		/// </summary>
		private void InitializeDialogThread(object parameter)
		{

			// This structure will be filled in with items from the data model and passed to the foreground so the foreground
			// thread doesn't need to worry about the locks.
			TraderOptions traderOptions = new TraderOptions();

            try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TraderLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.TraderVolumeSettingLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.VolumeCategoryLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Fill in the structure with the Trader values.
				ClientMarketData.TraderRow traderRow = ClientMarketData.Trader.FindByTraderId(Preferences.UserId);
				if (traderRow != null)
				{
					traderOptions.MaximumVolatility = traderRow[ClientMarketData.Trader.MaximumVolatilityDefaultColumn];
					traderOptions.StartTime = traderRow[ClientMarketData.Trader.StartTimeDefaultColumn];
					traderOptions.StopTime = traderRow[ClientMarketData.Trader.StopTimeDefaultColumn];
					traderOptions.NewsFreeTime = traderRow[ClientMarketData.Trader.NewsFreeTimeDefaultColumn];
					traderOptions.IsBrokerMatch = traderRow.IsBrokerMatch;
					traderOptions.IsHedgeMatch = traderRow.IsHedgeMatch;
					traderOptions.IsInstitutionMatch = traderRow.IsInstitutionMatch;
                    traderOptions.MarketSleep = traderRow.MarketSleep / 60;
                    traderOptions.ReviewWindow = traderRow.ReviewWindow / 60;
                   
                    // extract the ADV bucket settings from the data model
                    ClientMarketData.TraderVolumeSettingRow[] traderADVRows = traderRow.GetTraderVolumeSettingRows();
                    if (traderADVRows != null)
                    {
                        for (int index = 0; index < traderADVRows.GetLength(0); index++)
                        {
                            ClientMarketData.TraderVolumeSettingRow advRow = traderADVRows[index];
                            switch (advRow.VolumeCategoryRow.Mnemonic.ToUpper())
                            {
                                case "LOW":
                                    traderOptions.ThresholdLow = advRow.ThresholdQuantity;
                                    traderOptions.AutoExecuteLow = advRow.AutoExecuteQuantity;
                                    break;
                                
                                case "MID":
                                    traderOptions.ThresholdMid = advRow.ThresholdQuantity;
                                    traderOptions.AutoExecuteMid = advRow.AutoExecuteQuantity;
                                    break;

                                case "HIGH":
                                    traderOptions.ThresholdHigh = advRow.ThresholdQuantity;
                                    traderOptions.AutoExecuteHigh = advRow.AutoExecuteQuantity;
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
				}
                
            }
			finally
			{

				// Release the locks
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TraderLock.IsReaderLockHeld)
					ClientMarketData.TraderLock.ReleaseReaderLock();
                if (ClientMarketData.TraderVolumeSettingLock.IsReaderLockHeld)
                    ClientMarketData.TraderVolumeSettingLock.ReleaseReaderLock();
                if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld)
                    ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// The last step is to send the trader options to the foreground.  However, if the window hasn't been created by the 
			// time this thread is complete, the 'BeginInvoke' will cause an exeption (the handle hasn't been created, so it can't
			// be used to invoke a foreground method).  This will hold up the initialition until the window is created and then
			// pass it the data it needs to fill in the form.
			this.handleCreatedEvent.WaitOne();
			BeginInvoke(this.initializeDelegate, new object[] { traderOptions });

		}

		/// <summary>
		/// Initialize the dialog with the data read from the data model.
		/// </summary>
		/// <param name="traderOptions">A structure containing the data for the form.</param>
		private void InitializeDialog(TraderOptions traderOptions)
		{
             
			// Initialize the dialog box members with the data read from the data model.  The data must be collected in a 
			// background thread, but it can only be added to the dialog in the thread that created the dialog.
			this.percentPickerVolatility.Value = traderOptions.MaximumVolatility;
			this.timePickerStartTime.Value = traderOptions.StartTime;
			this.timePickerStopTime.Value = traderOptions.StopTime;
            this.timeRangeSlider.StartTime = traderOptions.StartTime;
            this.timeRangeSlider.EndTime = traderOptions.StopTime;
            this.integerPickerNewsFreeTime.Value = traderOptions.NewsFreeTime;
			this.checkBoxBroker.Checked = traderOptions.IsBrokerMatch;
			this.checkBoxHedge.Checked = traderOptions.IsHedgeMatch;
			this.checkBoxInstitution.Checked = traderOptions.IsInstitutionMatch;
            this.sliderHigh.Threshold = (int)traderOptions.ThresholdHigh;
            this.sliderHigh.AutoExecute = (int)traderOptions.AutoExecuteHigh;
            this.sliderMid.Threshold = (int)traderOptions.ThresholdMid;
            this.sliderMid.AutoExecute = (int)traderOptions.AutoExecuteMid;
            this.sliderLow.Threshold = (int)traderOptions.ThresholdLow;
            this.sliderLow.AutoExecute = (int)traderOptions.AutoExecuteLow;
            this.marketSleepPicker.Value = traderOptions.MarketSleep;
            this.reviewPicker.Value = traderOptions.ReviewWindow;
		}

		/// <summary>
		/// Apply the options to the data model.
		/// </summary>
		/// <param name="exitDialog">true to exit the dialog once the changes have been applied.</param>
		private void Apply(bool exitDialog)
		{

			// This structure is passed to a background thread to be applied to the server data model.
			TraderOptions traderOptions = new TraderOptions();

			// Fill in the options structure with the values found in this tab of the control.
			traderOptions.IsBrokerMatch = this.checkBoxBroker.Checked;
			traderOptions.IsHedgeMatch = this.checkBoxHedge.Checked;
			traderOptions.IsInstitutionMatch = this.checkBoxInstitution.Checked;
			traderOptions.MaximumVolatility = this.percentPickerVolatility.Value;
			traderOptions.NewsFreeTime = this.integerPickerNewsFreeTime.Value;
			//traderOptions.StartTime = this.timePickerStartTime.Value;
			//traderOptions.StopTime = this.timePickerStopTime.Value;
            traderOptions.StartTime = this.timeRangeSlider.StartTime;
            traderOptions.StopTime = this.timeRangeSlider.EndTime;
            traderOptions.ThresholdHigh = this.sliderHigh.Threshold;
            traderOptions.AutoExecuteHigh = this.sliderHigh.AutoExecute;
            traderOptions.ThresholdMid = this.sliderMid.Threshold;
            traderOptions.AutoExecuteMid = this.sliderMid.AutoExecute;
            traderOptions.ThresholdLow = this.sliderLow.Threshold;
            traderOptions.AutoExecuteLow = this.sliderLow.AutoExecute;
            traderOptions.MarketSleep = 60 * (int)this.marketSleepPicker.Value;
            traderOptions.ReviewWindow = 60 * (int)this.reviewPicker.Value;
			// Updating the server data model must be done in the background so as to not disturbe the flow of the message loop.
			ThreadPool.QueueUserWorkItem(new WaitCallback(ApplyThread), new Object[] { exitDialog, traderOptions });

		}

		/// <summary>
		/// Begins the task of applying the changes in the form to the data model.
		/// </summary>
		private void ApplyThread(object parameter)
		{

			// Extract the parameters from the threads parameter.
			object[] parameters = (object[])parameter;
			bool exitDialog = (bool)parameters[0];
			TraderOptions traderOptions = (TraderOptions)parameters[1];

			// This batch will contain a command to update the user's peferences with the values extracted from the form in the
			// foreground thread.
			Batch batch = new Batch();

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.TraderLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.TraderVolumeSettingLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.VolumeCategoryLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This section will fill in the batch with the values retrieved from the foreground.  It will
				// also add in the rowVersion for optimistic concurrency checking.
				int traderId = Preferences.UserId;
				ClientMarketData.TraderRow traderRow = ClientMarketData.Trader.FindByTraderId(traderId);
				if (traderRow != null)
				{
                    // This will save the users personal preferences for the stylesheet back to the database.
					TransactionPlan transactionPlan = batch.Transactions.Add();
					AssemblyPlan assemblyPlan = batch.Assemblies.Add("Core Service");
					TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.Trader");
					MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Update");
					methodPlan.Parameters.Add(new InputParameter("isBrokerMatch", traderOptions.IsBrokerMatch));
					methodPlan.Parameters.Add(new InputParameter("isHedgeMatch", traderOptions.IsHedgeMatch));
					methodPlan.Parameters.Add(new InputParameter("isInstitutionMatch", traderOptions.IsInstitutionMatch));
					methodPlan.Parameters.Add(new InputParameter("maximumVolatilityDefault", traderOptions.MaximumVolatility));
					methodPlan.Parameters.Add(new InputParameter("newsFreeTimeDefault", traderOptions.NewsFreeTime));
					methodPlan.Parameters.Add(new InputParameter("startTimeDefault", traderOptions.StartTime));
					methodPlan.Parameters.Add(new InputParameter("stopTimeDefault", traderOptions.StopTime));
                    methodPlan.Parameters.Add(new InputParameter("marketSleep", traderOptions.MarketSleep));
                    methodPlan.Parameters.Add(new InputParameter("reviewWindow", traderOptions.ReviewWindow));
                    methodPlan.Parameters.Add(new InputParameter("traderId", traderId));
					methodPlan.Parameters.Add(new InputParameter("rowVersion", traderRow.RowVersion));

                    // extract the ADV bucket settings from the data model
                    TypePlan volumeTypePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.TraderVolumeSetting");
                    ClientMarketData.TraderVolumeSettingRow[] traderADVRows = traderRow.GetTraderVolumeSettingRows();
                    if (traderADVRows != null)
                    {
                        if (traderADVRows.GetLength(0) > 0)
                        {

                            for (int index = 0; index < traderADVRows.GetLength(0); index++)
                            {
                                ClientMarketData.TraderVolumeSettingRow advRow = traderADVRows[index];
                                switch (advRow.VolumeCategoryRow.Mnemonic)
                                {
                                    case "Low":
                                        MethodPlan lowMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Update");
                                        lowMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdLow));
                                        lowMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                        lowMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", advRow.VolumeCategoryId));
                                        lowMethodPlan.Parameters.Add(new InputParameter("rowVersion", advRow.RowVersion));
                                        lowMethodPlan.Parameters.Add(new InputParameter("traderVolumeSettingId", advRow.TraderVolumeSettingId));
                                        lowMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteLow));
                                        break;

                                    case "Mid":
                                        MethodPlan midMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Update");
                                        midMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdMid));
                                        midMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                        midMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", advRow.VolumeCategoryId));
                                        midMethodPlan.Parameters.Add(new InputParameter("rowVersion", advRow.RowVersion));
                                        midMethodPlan.Parameters.Add(new InputParameter("traderVolumeSettingId", advRow.TraderVolumeSettingId));
                                        midMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteMid));
                                        break;

                                    case "High":
                                        MethodPlan highMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Update");
                                        highMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdHigh));
                                        highMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                        highMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", advRow.VolumeCategoryId));
                                        highMethodPlan.Parameters.Add(new InputParameter("rowVersion", advRow.RowVersion));
                                        highMethodPlan.Parameters.Add(new InputParameter("traderVolumeSettingId", advRow.TraderVolumeSettingId));
                                        highMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteHigh));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            // low
                            if (volumeCategories.ContainsKey("Low"))
                            {
                                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["Low"];
                                MethodPlan lowMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Insert");
                                lowMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdLow));
                                lowMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                lowMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", volumeCategory.VolumeCategoryId));
                                lowMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteLow));
                            }

                            // mid
                            if (volumeCategories.ContainsKey("Mid"))
                            {
                                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["Mid"];
                                MethodPlan midMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Insert");
                                midMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdMid));
                                midMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                midMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", volumeCategory.VolumeCategoryId));
                                midMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteMid));
                            }

                            // high
                            if (volumeCategories.ContainsKey("High"))
                            {
                                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["High"];
                                MethodPlan highMethodPlan = transactionPlan.Methods.Add(volumeTypePlan, "Insert");
                                highMethodPlan.Parameters.Add(new InputParameter("thresholdQuantity", traderOptions.ThresholdHigh));
                                highMethodPlan.Parameters.Add(new InputParameter("traderId", traderId));
                                highMethodPlan.Parameters.Add(new InputParameter("volumeCategoryId", volumeCategory.VolumeCategoryId));
                                highMethodPlan.Parameters.Add(new InputParameter("autoExecuteQuantity", traderOptions.AutoExecuteHigh));
                            }
                        }
                    }
                    
				}

			}
			catch (Exception exception)
			{

				// Record the error in the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.TraderLock.IsReaderLockHeld) ClientMarketData.TraderLock.ReleaseReaderLock();
                if (ClientMarketData.TraderVolumeSettingLock.IsReaderLockHeld) ClientMarketData.TraderVolumeSettingLock.ReleaseReaderLock();
                if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld) ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
                System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// This will execute the command on the server and return any exceptions.
			BatchException batchException = null;

			try
			{

				// Execute the batch.
				ClientMarketData.Execute(batch);

			}
			catch (BatchException exception)
			{

				// Any exceptions will be captured here and passed on to the foreground.
				batchException = exception;

			}

			// Call the foreground thread with the results of executing the batch on the server.  Also, in some cases the dialog is
			// going to be dismissed when the server data model has finished updating successfully.  Pass on the flag to the
			// foreground that will indicate whether the form is closed once the results are processed.
			BeginInvoke(this.applyEndDelegate, new object[] { exitDialog, batchException });

		}

		/// <summary>
		/// Completes the task of applying the user's changes to the form.
		/// </summary>
		/// <param name="exitDialog">If true, the form is dismiss.</param>
		/// <param name="batchException">Contains any exceptions from processing the server update.</param>
		private void ApplyEnd(bool exitDialog, BatchException batchException)
		{

			// Display any errors from the background processing.
			if (batchException != null)
				foreach (Exception exception in batchException.Exceptions)
					MessageBox.Show(this, exception.Message, Resource.StringGuardianError);

			// If there were no exceptions and the user asked to exit the dialog, it is dismissed.
			if (batchException == null && exitDialog)
				Close();

		}

		/// <summary>
		/// Apply the changes in the form but don't exit the form.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonApply_Click(object sender, System.EventArgs e)
		{

			// Start the chain of events that will apply the changes to the data model.
			Apply(false);

		}

		/// <summary>
		/// Apply the changes in the form and exit.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void buttonOK_Click(object sender, System.EventArgs e)
		{

			// Start the chain of events that will apply the changes to the data model.
			Apply(true);

		}

        private void Options_Load(object sender, EventArgs e)
        {
            // grab the metadata for each ADV bucket from the data model into a hashtable
            InitializeVolumeCategory();

            // low
            if (volumeCategories.ContainsKey("Low"))
            {
                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["Low"];
                //sliderLow.Minimum = volumeCategory.MinRange;
                //sliderLow.Maximum = volumeCategory.MaxRange;
                sliderLow.BucketLabel = volumeCategory.Mnemonic;
            }

            // mid
            if (volumeCategories.ContainsKey("Mid"))
            {
                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["Mid"];
                //sliderMid.Minimum = volumeCategory.MinRange;
                //sliderMid.Maximum = volumeCategory.MaxRange;
                sliderMid.BucketLabel = volumeCategory.Mnemonic;
                
            }

            // high
            if (volumeCategories.ContainsKey("High"))
            {
                VolumeCategory volumeCategory = (VolumeCategory)volumeCategories["High"];
                //sliderHigh.Minimum = volumeCategory.MinRange;
                //sliderHigh.Maximum = volumeCategory.MaxRange;
                sliderHigh.BucketLabel = volumeCategory.Mnemonic;
                
            }
        }

        private void sliderMid_Load(object sender, EventArgs e)
        {

        }
	}

}
