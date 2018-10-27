namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Forms;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;

	public partial class BlotterViewer : MarkThree.Forms.Viewer
	{

		// Private Members
		private MarkThree.Guardian.Blotter blotter;
		private System.Boolean hasAdvertisementViewer;
		private System.Boolean hasMatchViewer;
		private System.Boolean hasMatchHistoryViewer;
		private System.Boolean hasDestinationOrderViewer;
		private System.Boolean hasExecutionViewer;
		private System.Boolean hasSourceOrderViewer;
		private System.Boolean hasWorkingOrderViewer;

		/// <summary>
		/// Public access for the Viewer's menus.
		/// </summary>
		[Browsable(false)]
		public override MenuStrip MenuStrip
		{
			get
			{
				return this.menuStrip;
			}
		}

		/// <summary>
		/// Public access for the Viewer's Toolbars.
		/// </summary>
		[Browsable(false)]
		public override ToolStrip ToolBarStandard
		{
			get { return this.toolStrip; }
		}

		public BlotterViewer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Opens the Blotter Viewer.
		/// </summary>
		/// <param name="tag">The object that is to be displayed in this viewer.</param>
		public override void Open(object tag)
		{

			// Make sure the object can be opened in this viewer.
			if (!(tag is Blotter || tag is BlotterWorkingOrderDetail || tag is BlotterMatchDetail))
				throw new Exception(string.Format("The Blotter Viewer can't display an object of type {0}", tag.GetType()));

			// If this viewer can display the requested object, then pass this on to the base class which will set of a chain of
			// events that will eventually initialize and display the object.
			base.Open(tag);

		}

		/// <summary>
		/// Opens the Blotter Document.
		/// </summary>
		/// <param name="blotterId">The blotter identifier.</param>
		protected override void OpenCommand()
		{

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				if (this.Tag is BlotterMatchDetail)
					this.blotter = ((BlotterMatchDetail)this.Tag).Blotter;

				if (this.Tag is Blotter)
					this.blotter = (Blotter)this.Tag;

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, 
				// equity traders Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (blotterRow == null)
					throw new ArgumentException("This blotter has been deleted", this.blotter.BlotterId.ToString());

				// If a viewer is avaiable for the objects associated with the blotter, then we'll enable the viewers for those
				// objects.  For example, debt blotters don't require destinationOrder viewers, so there won't be one associated
				// with that blotter.
				this.hasAdvertisementViewer = !blotterRow.IsAdvertisementStylesheetIdNull();
				this.hasDestinationOrderViewer = !blotterRow.IsDestinationOrderStylesheetIdNull();
				this.hasExecutionViewer = !blotterRow.IsExecutionStylesheetIdNull();
				this.hasMatchViewer = !blotterRow.IsMatchStylesheetIdNull();
				this.hasMatchHistoryViewer = !blotterRow.IsMatchHistoryStylesheetIdNull();
				this.hasSourceOrderViewer = !blotterRow.IsSourceOrderStylesheetIdNull();
				this.hasWorkingOrderViewer = !blotterRow.IsWorkingOrderStylesheetIdNull();

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			BeginInvoke(new EventHandler(OpenChildren));

		}

		private void OpenChildren(object sender, EventArgs eventArgs)
		{

			// Open up each of the views for the blotter.
			if (this.hasWorkingOrderViewer)
			{
				this.workingOrderViewer.Open(this.blotter);
				this.blotterDetailViewer.Open(new BlotterWorkingOrderDetail(this.blotter, new WorkingOrder[] { }));
			}

			if (this.hasMatchViewer)
			{

				if (this.Tag is Blotter)
				{
					this.matchViewer.Open(this.blotter);
					this.orderBookViewer.Open(this.blotter);
				}

				if (this.Tag is BlotterMatchDetail)
				{
					this.matchViewer.Open(this.Tag);
					this.tabControl.SelectedTab = this.tabPageMatch;
					if (Form.ActiveForm != null)
						Form.ActiveForm.Activate();

					int matchId = ((BlotterMatchDetail)this.Tag).Matches[0].MatchId;
					this.matchHistoryViewer.Open(((BlotterMatchDetail)this.Tag).Blotter);
					this.orderBookViewer.OpenMatch(matchId);
					this.volumeChartViewer.OpenMatch(matchId);
					this.negotiationConsole.OpenMatch(matchId);
                    this.quoteViewer.OpenQuote(matchId);

				}

			}

			if (this.hasMatchHistoryViewer)
				this.matchHistoryViewer.Open(this.blotter);

			if (this.hasExecutionViewer)
				this.executionViewer.Open(this.blotter);

			this.consoleViewer.Open(null);

		}

		/// <summary>
		/// Closes out the blotter document and all the child viewers.
		/// </summary>
		public override void Close()
		{

			// Close out the viewers if they've been opened.
			if (this.hasWorkingOrderViewer)
			{
				this.workingOrderViewer.Close();
				this.blotterDetailViewer.Close();
			}

			if (this.hasMatchViewer)
			{
				this.matchViewer.Close();
				this.orderBookViewer.Close();
			}

			if (this.hasMatchHistoryViewer)
				this.matchHistoryViewer.Close();

			if (this.hasExecutionViewer)
				this.executionViewer.Close();

			this.consoleViewer.Close();

			// Allow the base classes to close their resources.
			base.Close();

		}

		/// <summary>
		/// Saves the current document.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{

			// Call the base class to save the Spreadsheet data.
			this.workingOrderViewer.SaveAs();

		}

		/// <summary>
		/// Loads the configuration of the Blotter Screens from the application settings file.
		/// </summary>
		public override void LoadSettings()
		{

			// Load the state of the window configuration items.
			this.detailWindowToolStripMenuItem.Checked = Settings.Default.DetailWindowMenuStripItemChecked;
			this.consoleWindowToolStripMenuItem.Checked = Settings.Default.ConsoleWindowMenuStripItemChecked;
            this.quoteWindowToolStripMenuItem.Checked = Settings.Default.QuoteWindowMenuStripItemChecked;

		}

		/// <summary>
		/// Saves the configuration of the blotter.
		/// </summary>
		public override void SaveSettings()
		{

			// Save the configuration of the panes.
			Settings.Default.DetailWindowMenuStripItemChecked = this.detailWindowToolStripMenuItem.Checked;
			Settings.Default.ConsoleWindowMenuStripItemChecked = this.consoleWindowToolStripMenuItem.Checked;
            Settings.Default.QuoteWindowMenuStripItemChecked = this.quoteWindowToolStripMenuItem.Checked;
			
			// SplitContainer1
			Settings.Default.SplitContainer1Panel2Collapsed = this.splitContainer1.Panel2Collapsed;
			Settings.Default.SplitContainer1SplitterDistance = this.splitContainer1.SplitterDistance;
			Settings.Default.SplitContainer1Size = this.splitContainer1.Size;

			// SplitContainer2
			Settings.Default.SplitContainer2Panel1Collapsed = this.splitContainer2.Panel1Collapsed;
			Settings.Default.SplitContainer2Panel2Collapsed = this.splitContainer2.Panel2Collapsed;
			Settings.Default.SplitContainer2SplitterDistance = this.splitContainer2.SplitterDistance;
			Settings.Default.SplitContainer2Size = this.splitContainer2.Size;

			// SplitContainer3
			Settings.Default.SplitContainer3SplitterDistance = this.splitContainer3.SplitterDistance;
			Settings.Default.SplitContainer3Size = this.splitContainer3.Size;

			// SplitContainer4
			Settings.Default.SplitContainer4SplitterDistance = this.splitContainer4.SplitterDistance;
			Settings.Default.SplitContainer4Size = this.splitContainer4.Size;

			// SplitContainer5
			Settings.Default.SplitContainer5SplitterDistance = this.splitContainer5.SplitterDistance;
			Settings.Default.SplitContainer5Size = this.splitContainer5.Size;

            // SplitContainer 6
            Settings.Default.SplitContainer6SplitterDistance = this.splitContainer6.SplitterDistance;
            Settings.Default.SplitContainer6Size = this.splitContainer6.Size;

			// Save the persistent settings.
			Settings.Default.Save();

		}

		/// <summary>
		/// Called when a child window is ready to be displayed.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_EndOpenDocument(object sender, System.EventArgs e)
		{

			// The blotter isn't considered 'Opened' until all the child viewers have been opened.
			if ((this.executionViewer.IsOpen || !this.hasExecutionViewer) &&
				((this.matchViewer.IsOpen && this.orderBookViewer.IsOpen) || !this.hasMatchViewer) &&
				(this.matchHistoryViewer.IsOpen || !this.hasMatchHistoryViewer) &&
				((this.workingOrderViewer.IsOpen && this.blotterDetailViewer.IsOpen) || !this.hasWorkingOrderViewer) &&
				this.consoleViewer.IsOpen)
			{

				// Once all the children have completed their opening operations, the blotter configure them in the foreground.
				Invoke(new EventHandler(EndOpenForeground), new object[] { this, EventArgs.Empty });

			}

		}

		/// <summary>
		/// Configures the child viewers based on the menu selections.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void EndOpenForeground(object sender, EventArgs eventArgs)
		{

			// Suspend Updates while we re-arrange the viewers.
			SuspendLayout();

			// Add or remove the Execution Viewer depending on whether it has a stylesheet defined.
			if (this.hasExecutionViewer && !this.tabControl.TabPages.Contains(this.tabPageExecution))
				this.tabControl.TabPages.Add(this.tabPageExecution);
			if (!this.hasExecutionViewer && this.tabControl.TabPages.Contains(this.tabPageExecution))
				this.tabControl.TabPages.Remove(this.tabPageExecution);

			// Add or remove the Working Order Viewer depending on whether it has a stylesheet defined.
			if (this.hasWorkingOrderViewer && !this.tabControl.TabPages.Contains(this.tabPageWorking))
				this.tabControl.TabPages.Add(this.tabPageWorking);
			if (!this.hasWorkingOrderViewer && this.tabControl.TabPages.Contains(this.tabPageWorking))
				this.tabControl.TabPages.Remove(this.tabPageWorking);

			// Make the working orders and the detail visible.
			this.workingOrderViewer.Visible = this.hasWorkingOrderViewer;
			if (!this.blotterDetailViewer.Visible)
				this.workingOrderViewer.Height = this.tabPageWorking.ClientRectangle.Height;

			// Redraw the reconfigured screen.
			ResumeLayout();

		}

		private void menuItemMatchView_Click(object sender, System.EventArgs e)
		{

			// this will allow the user to select the columns for the match viewer.
			if (this.matchViewer != null)
				this.matchViewer.SelectColumns();

		}

		private void workingOrderViewer_OpenWorkingOrder(object sender, MarkThree.Guardian.WorkingOrderEventArgs e)
		{

			this.blotterDetailViewer.Open(new BlotterWorkingOrderDetail(this.blotter, e.WorkingOrders));

		}

		private void workingOrderViewer_CloseWorkingOrder(object sender, System.EventArgs e)
		{

			this.blotterDetailViewer.Close();

		}

		private void workingOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// This will allow the user to select the columns for the working order viewer.
			if (this.workingOrderViewer != null)
				this.workingOrderViewer.SelectColumns();

		}

		private void sourceOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void destinationOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void executionToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// This will allow the user to select the columns for the working order viewer.
			if (this.executionViewer != null)
				this.executionViewer.SelectColumns();

		}

		private void sourceWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void sourceWindowToolStripMenuItem_Click_1(object sender, EventArgs e)
		{

		}

		private void destinationOrderToolStripMenuItem_Click_1(object sender, EventArgs e)
		{

		}

		private void executionToolStripMenuItem_Click_1(object sender, EventArgs e)
		{

			// This will allow the user to select the columns for the working order viewer.
			if (this.blotterDetailViewer.ExecutionViewer != null)
				this.blotterDetailViewer.ExecutionViewer.SelectColumns();

		}

		private void orderFormToolStripMenuItem_Click(object sender, EventArgs e)
		{

			FormOrder formOrder = new FormOrder(this.blotter);
			formOrder.ShowDialog();
			formOrder.Dispose();

		}

		private void toolStripButton8_Click(object sender, EventArgs e)
		{

			this.toolStripButtonShowMatch.Checked = !this.toolStripButtonShowMatch.Checked;
			this.workingOrderViewer.ShowMatchingColumns(this.toolStripButtonShowMatch.Checked);

		}

		private void toolStripButton9_Click(object sender, EventArgs e)
		{
			this.workingOrderViewer.SetFilter(0);
		}

		private void toolStripButton10_Click(object sender, EventArgs e)
		{
			this.workingOrderViewer.SetFilter(1);
		}

		private void toolStripButton11_Click(object sender, EventArgs e)
		{
			this.workingOrderViewer.SetFilter(2);
		}

		private void matchViewer_SelectionChanged(object sender, EventArgs e)
		{
			object[] selectedItems = this.matchViewer.SelectedItems;
			if (selectedItems.Length != 0)
			{
				object[] key = (object[])selectedItems[0];
				this.orderBookViewer.OpenMatch((int)key[0]);
				this.volumeChartViewer.OpenMatch((int)key[0]);
				this.negotiationConsole.OpenMatch((int)key[0]);
                this.quoteViewer.OpenQuote((int)key[0]);
			}

		}

		private void menuItemDetailWindow_Click(object sender, System.EventArgs e)
		{

			this.detailWindowToolStripMenuItem.Checked = !this.detailWindowToolStripMenuItem.Checked;

			this.tabPageWorking.SuspendLayout();

			this.splitContainer2.Panel1Collapsed = !this.detailWindowToolStripMenuItem.Checked &&
				this.consoleWindowToolStripMenuItem.Checked;
			this.splitContainer2.Panel2Collapsed = this.detailWindowToolStripMenuItem.Checked &&
				!this.consoleWindowToolStripMenuItem.Checked;
			this.splitContainer1.Panel2Collapsed = !this.detailWindowToolStripMenuItem.Checked &&
				!this.consoleWindowToolStripMenuItem.Checked;

			this.tabPageWorking.ResumeLayout();

		}

		private void consoleWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{

			this.consoleWindowToolStripMenuItem.Checked = !this.consoleWindowToolStripMenuItem.Checked;

			this.tabPageWorking.SuspendLayout();

			this.splitContainer2.Panel1Collapsed = !this.detailWindowToolStripMenuItem.Checked &&
				this.consoleWindowToolStripMenuItem.Checked;
			this.splitContainer2.Panel2Collapsed = this.detailWindowToolStripMenuItem.Checked &&
				!this.consoleWindowToolStripMenuItem.Checked;
			this.splitContainer1.Panel2Collapsed = !this.detailWindowToolStripMenuItem.Checked &&
				!this.consoleWindowToolStripMenuItem.Checked;

			this.tabPageWorking.ResumeLayout();

		}


        private void quoteWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.quoteWindowToolStripMenuItem.Checked = !(this.quoteWindowToolStripMenuItem.Checked);

            this.tabPageMatch.SuspendLayout();

            this.splitContainer6.Panel1Collapsed = !this.quoteWindowToolStripMenuItem.Checked;

            this.tabPageMatch.ResumeLayout();

        }

		private void toolStripButton1_Click(object sender, EventArgs e)
		{

		}

		private void toolStripButtonShowVolume_Click(object sender, EventArgs e)
		{
			this.toolStripButtonShowVolume.Checked = !this.toolStripButtonShowVolume.Checked;
			this.workingOrderViewer.ShowVolumeColumns(this.toolStripButtonShowVolume.Checked);

		}

		private void toolStripButtonExitBox_Click(object sender, EventArgs e)
		{

			this.toolStripButtonExitBox.Checked = true;
			this.toolStripButtonEnterBox.Checked = false;
			this.workingOrderViewer.SetAway(this.toolStripButtonExitBox.Checked);

		}

		private void toolStripButtonEnterBox_Click(object sender, EventArgs e)
		{

			this.toolStripButtonExitBox.Checked = false;
			this.toolStripButtonEnterBox.Checked = true;
			this.workingOrderViewer.SetAway(this.toolStripButtonExitBox.Checked);

		}

	}

}

