namespace MarkThree.Guardian.Forms
{

	using MarkThree.Client;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for BlotterDetailViewer.
	/// </summary>
	public class BlotterDetailViewer : MarkThree.Forms.Viewer
	{

		// Private Members
		private bool hasDestinationOrderViewer;
		private bool hasExecutionViewer;
		private bool hasSourceOrderViewer;
		private Blotter blotter;
		private WorkingOrder[] workingOrders;
		private System.Windows.Forms.TabPage tabPageExecution;
		private System.Windows.Forms.TabControl tabControl;
		private MarkThree.Guardian.ExecutionViewer executionViewer;

		// Private Events
		private System.EventHandler endOpenEventHandler;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExecutionViewer ExecutionViewer { get { return this.executionViewer; } }

		/// <summary>
		/// The blotter detail viewer is a set of panels used to see all the details of one or more working orders.
		/// </summary>
		public BlotterDetailViewer()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// This delegate is called from a background thread when all the child viewers have completed their 'Open' commands.
			this.endOpenEventHandler = new System.EventHandler(EndOpenForeground);

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{

			if (this.tabControl != null)
			{

				// Manually dispose of any controls that have been removed from the tab control in order to hid them.
				if (!this.tabControl.TabPages.Contains(this.tabPageExecution))
					this.tabPageExecution.Dispose();

			}

			if (disposing)
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageExecution = new System.Windows.Forms.TabPage();
			this.executionViewer = new MarkThree.Guardian.ExecutionViewer();
			this.tabControl.SuspendLayout();
			this.tabPageExecution.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabControl.Controls.Add(this.tabPageExecution);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(512, 256);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageExecution
			// 
			this.tabPageExecution.Controls.Add(this.executionViewer);
			this.tabPageExecution.Location = new System.Drawing.Point(4, 4);
			this.tabPageExecution.Name = "tabPageExecution";
			this.tabPageExecution.Size = new System.Drawing.Size(504, 224);
			this.tabPageExecution.TabIndex = 2;
			this.tabPageExecution.Text = "Executions";
			this.tabPageExecution.UseVisualStyleBackColor = true;
			// 
			// executionViewer
			// 
			this.executionViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.executionViewer.Icon = null;
			this.executionViewer.Location = new System.Drawing.Point(0, 0);
			this.executionViewer.Name = "executionViewer";
			this.executionViewer.Size = new System.Drawing.Size(504, 224);
			this.executionViewer.TabIndex = 0;
			this.executionViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			// 
			// BlotterDetailViewer
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "BlotterDetailViewer";
			this.Size = new System.Drawing.Size(512, 256);
			this.tabControl.ResumeLayout(false);
			this.tabPageExecution.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public override void Open(object tag)
		{

			if (!(tag is Blotter || tag is BlotterWorkingOrderDetail || tag is BlotterMatchDetail))
				throw new Exception(string.Format("Can't display an object of type {0} in this viewer", tag.GetType()));

			// Extract the document identity from the Blotter tag.
			if (tag is Blotter)
			{
				this.blotter = (Blotter)tag;
				this.workingOrders = null;
			}

			// Extract the document identity from the BlotterWorkingOrderDetail tag.
			if (tag is BlotterWorkingOrderDetail)
			{
				this.blotter = ((BlotterWorkingOrderDetail)tag).Blotter;
				this.workingOrders = ((BlotterWorkingOrderDetail)tag).WorkingOrders;
			}

			// Extract the document identity from the BlotterMatchDetail tag.
			if (tag is BlotterMatchDetail)
			{
				this.blotter = ((BlotterMatchDetail)tag).Blotter;
				this.workingOrders = null;
			}

			base.Open(tag);

		}

		protected override void OpenCommand()
		{

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, 
				// equity traders Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (blotterRow == null)
					throw new ArgumentException("This blotter has been deleted", this.blotter.BlotterId.ToString());

				// If a viewer is avaiable for the objects associated with the blotter, then we'll enable the viewers for those
				// objects.  For example, debt blotters don't require destinationOrder viewers, so there won't be one associated
				// with that blotter.
				this.hasDestinationOrderViewer = !blotterRow.IsDestinationOrderStylesheetIdNull();
				this.hasExecutionViewer = !blotterRow.IsExecutionStylesheetIdNull();
				this.hasSourceOrderViewer = !blotterRow.IsSourceOrderStylesheetIdNull();

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			if (this.hasExecutionViewer)
				this.executionViewer.Open(this.Tag);

		}

		/// <summary>
		/// Called when a child window is ready to be displayed.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_EndOpenDocument(object sender, System.EventArgs e)
		{

			// The blotter isn't considered 'Opened' until all the child viewers have been opened.
			if (this.executionViewer.IsOpen || !this.hasExecutionViewer)
			{

				// Once all the children have completed their opening operations, the blotter configure them in the 
				// foreground.
				Invoke(this.endOpenEventHandler, new object[] {this, EventArgs.Empty});

			}

		}

		/// <summary>
		/// Configures the child viewers based on the menu selections.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void EndOpenForeground(object sender, EventArgs eventArgs)
		{

			this.tabControl.SuspendLayout();

			if (this.hasExecutionViewer && !this.tabControl.TabPages.Contains(this.tabPageExecution))
				this.tabControl.TabPages.Add(this.tabPageExecution);

			if (!this.hasExecutionViewer && this.tabControl.TabPages.Contains(this.tabPageExecution))
				this.tabControl.TabPages.Remove(this.tabPageExecution);

			this.tabControl.ResumeLayout();

		}

		/// <summary>
		/// Closes out the blotter document and all the child viewers.
		/// </summary>
		public override void Close()
		{

			// Close out the viewers if they've been opened.

			if (this.hasExecutionViewer)
				this.executionViewer.Close();

			// Allow the base classes to close their resources.
			base.Close();

		}
		
	}

}
