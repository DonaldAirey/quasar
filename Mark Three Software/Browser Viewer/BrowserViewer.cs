namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Configuration;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// An HTML Document Viewer
	/// </summary>
	public class BrowserViewer : MarkThree.Forms.Viewer
	{

		private object commonObject;
		private Uri uri;
		private System.ComponentModel.IContainer components = null;
		private WebBrowser webBrowser;
		private ToolStrip toolStrip;
		private ToolStripButton toolStripButtonBack;
		private ToolStripButton toolStripButtonForward;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripButton toolStripButtonStop;
		private ToolStripButton toolStripButtonRefresh;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripButton toolStripButtonHome;
		private ToolStripButton toolStripButtonSearch;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripComboBox toolStripComboBoxHistory;
	
		private delegate void NavigateDelegate(string url);
		private NavigateDelegate navigateDelegate;

		/// <summary>
		/// Constructor for the Browswer Viewer used to display HTML pages and web sites.
		/// </summary>
		public BrowserViewer()
		{

			// IDE Supported Code to Initialize GUI Elements.
			InitializeComponent();

			// This delegate is used to load the viewer with a URL from a background thread.
			this.navigateDelegate = new NavigateDelegate(NavigateForeground);

		}

		protected override void Dispose(bool disposing)
		{

			// Dispose of the add-in components.
			if (disposing && components != null)
				components.Dispose();

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBoxHistory = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripButtonBack = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonForward = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonHome = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Location = new System.Drawing.Point(0, 0);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.ScriptErrorsSuppressed = true;
			this.webBrowser.ScrollBarsEnabled = false;
			this.webBrowser.Size = new System.Drawing.Size(512, 256);
			this.webBrowser.TabIndex = 0;
			// 
			// toolStrip
			// 
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBack,
            this.toolStripButtonForward,
            this.toolStripSeparator1,
            this.toolStripButtonStop,
            this.toolStripButtonRefresh,
            this.toolStripSeparator2,
            this.toolStripButtonHome,
            this.toolStripButtonSearch,
            this.toolStripSeparator3,
            this.toolStripComboBoxHistory});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(512, 31);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip";
			this.toolStrip.Visible = false;
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator1.MergeIndex = 2;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator2.MergeIndex = 5;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator3.MergeIndex = 8;
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
			// 
			// toolStripComboBoxHistory
			// 
			this.toolStripComboBoxHistory.Enabled = false;
			this.toolStripComboBoxHistory.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripComboBoxHistory.MergeIndex = 9;
			this.toolStripComboBoxHistory.Name = "toolStripComboBoxHistory";
			this.toolStripComboBoxHistory.Size = new System.Drawing.Size(150, 31);
			this.toolStripComboBoxHistory.Text = "guardian: today";
			// 
			// toolStripButtonBack
			// 
			this.toolStripButtonBack.Enabled = false;
			this.toolStripButtonBack.Image = global::MarkThree.Forms.Properties.Resources.ArrowLeftGreen;
			this.toolStripButtonBack.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonBack.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonBack.MergeIndex = 0;
			this.toolStripButtonBack.Name = "toolStripButtonBack";
			this.toolStripButtonBack.Size = new System.Drawing.Size(57, 28);
			this.toolStripButtonBack.Text = "Back";
			// 
			// toolStripButtonForward
			// 
			this.toolStripButtonForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonForward.Enabled = false;
			this.toolStripButtonForward.Image = global::MarkThree.Forms.Properties.Resources.ArrowRightGreen;
			this.toolStripButtonForward.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonForward.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonForward.MergeIndex = 1;
			this.toolStripButtonForward.Name = "toolStripButtonForward";
			this.toolStripButtonForward.Size = new System.Drawing.Size(28, 28);
			this.toolStripButtonForward.Text = "Forward";
			this.toolStripButtonForward.ToolTipText = "Forward";
			// 
			// toolStripButtonStop
			// 
			this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStop.Enabled = false;
			this.toolStripButtonStop.Image = global::MarkThree.Forms.Properties.Resources.delete;
			this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonStop.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonStop.MergeIndex = 3;
			this.toolStripButtonStop.Name = "toolStripButtonStop";
			this.toolStripButtonStop.Size = new System.Drawing.Size(28, 28);
			this.toolStripButtonStop.Text = "Stop";
			// 
			// toolStripButtonRefresh
			// 
			this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRefresh.Enabled = false;
			this.toolStripButtonRefresh.Image = global::MarkThree.Forms.Properties.Resources.DocumentRefresh;
			this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonRefresh.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonRefresh.MergeIndex = 4;
			this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
			this.toolStripButtonRefresh.Size = new System.Drawing.Size(28, 28);
			this.toolStripButtonRefresh.Text = "toolStripButton4";
			// 
			// toolStripButtonHome
			// 
			this.toolStripButtonHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHome.Enabled = false;
			this.toolStripButtonHome.Image = global::MarkThree.Forms.Properties.Resources.home;
			this.toolStripButtonHome.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonHome.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonHome.MergeIndex = 6;
			this.toolStripButtonHome.Name = "toolStripButtonHome";
			this.toolStripButtonHome.Size = new System.Drawing.Size(28, 28);
			this.toolStripButtonHome.Text = "toolStripButton5";
			// 
			// toolStripButtonSearch
			// 
			this.toolStripButtonSearch.Enabled = false;
			this.toolStripButtonSearch.Image = global::MarkThree.Forms.Properties.Resources.EarthView;
			this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonSearch.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonSearch.MergeIndex = 7;
			this.toolStripButtonSearch.Name = "toolStripButtonSearch";
			this.toolStripButtonSearch.Size = new System.Drawing.Size(68, 28);
			this.toolStripButtonSearch.Text = "Search";
			this.toolStripButtonSearch.ToolTipText = "toolStripButtonSearch";
			// 
			// BrowserViewer
			// 
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.webBrowser);
			this.Name = "BrowserViewer";
			this.Size = new System.Drawing.Size(512, 256);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Navigates to a given URL.
		/// </summary>
		/// <param name="url">The Universal Resource Link text.</param>
		private void NavigateForeground(string url)
		{

			try
			{
			
				// Open the Web Page given by the URL.
				this.webBrowser.Navigate(url);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}
		
		/// <summary>
		/// Opens the Document in the Viewer.
		/// </summary>
		/// <param name="objectId">Identifies a folder that contains the URL to be opened.</param>
		public override void Open(object commonObject)
		{

			this.commonObject = commonObject;
			if (this.commonObject is MarkThree.WebPage)
				this.uri = ((WebPage)this.commonObject).Uri;

			try
			{
			
				// Call the foreground to update the URL in the viewer.
				Invoke(this.navigateDelegate, new object[] {this.uri.ToString()});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

			base.Open(commonObject);

		}

		public override ToolStrip ToolBarStandard
		{
			get
			{
				return this.toolStrip;
			}
		}

	}

}
