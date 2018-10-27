namespace MarkThree.Guardian
{

	using MarkThree.Forms;
	using MarkThree.Client;
	using MarkThree.Guardian;
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
	using System.Security.Policy;

	/// <summary>
	/// A Viewer for Order Execution
	/// </summary>
	public class AdvertisementViewer : MarkThree.Forms.SpreadsheetViewer
	{

		private int stylesheetId;
		private MarkThree.Guardian.Blotter blotter;
		private MarkThree.Guardian.Client.ClientMarketData clientClientMarketData;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Constructor for the AdvertisementViewer
		/// </summary>
		public AdvertisementViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

		}

		#region Dispose method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Remove any components that have been added in.
			if (disposing && components != null)
				components.Dispose();

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AdvertisementViewer));
			this.clientClientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
			this.SuspendLayout();
			// 
			// AdvertisementViewer
			// 
			this.Name = "AdvertisementViewer";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Draws the document in the background.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Creates an displays an execution report.
		/// </summary>
		/// <param name="workingOrderId">The block order of the executions.</param>
		/// <param name="execution">Optional temporary execution.</param>
		protected override void DrawDocumentCommand(object parameter)
		{

			// Extract the command arguments.
			Blotter blotter = (Blotter)parameter;

			try
			{

				// Lock all the tables that we'll reference while building a destinationOrder document.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the block order still exists.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (blotterRow == null && this.blotter.BlotterId != 0)
					throw new ArgumentException("The block order has been deleted.", this.blotter.BlotterId.ToString());

				// Create the execution document.
				AdvertisementDocument ticketDocument = new AdvertisementDocument(blotter.BlotterId);

#if DEBUGXML
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Dump the DOM to a file.  This is very useful when debugging.
					ticketDocument.Save("ticketDOM.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				}
#endif

				// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet
				// control.  We can invoke the foreground thread to move the data into the control.
				this.XmlDocument = ticketDocument;

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{
					
				// Release the locks
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld) ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld) ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Opens the Advertisement Viewer.
		/// </summary>
		protected void OpenCommand(params object[] parameters)
		{

			this.blotter = (Blotter)parameters[0];

			try
			{
			
				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanged += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Broker.BrokerRowChanged += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Broker.BrokerRowDeleted += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowChanged += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowDeleted += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.EndMerge += new EventHandler(this.EndMerge);

				// Find the block order and extract the securty level data if it exists.  This security level data is
				// needed to calculate the trade and settlement dates and other defaults for the executions.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (blotterRow == null)
					throw new Exception(String.Format("Blotter {0} has been deleted", this.blotter.BlotterId));

				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsAdvertisementStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.AdvertisementStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Advertisement stylesheet", this.blotter.BlotterId));

				// As an optimization, don't reload the stylesheet if the prevous document used the same stylesheet. This
				// will save a few hundred milliseconds when scrolling through similar documents.
				if (this.stylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case it is changed while we're viewing it.  The event handler 
					// will use this id to determine if an incoming stylesheet will trigger a refresh of the document.
					this.stylesheetId = stylesheetRow.StylesheetId;

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Closes the Blotter Viewer.
		/// </summary>
		public void CloseViewerCommand()
		{

			try
			{

				// Lock the tables.
				ClientMarketData.BlotterLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(ClientTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanged += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Broker.BrokerRowChanged += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Broker.BrokerRowDeleted += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowChanged += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowDeleted += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.EndMerge -= new EventHandler(this.EndMerge);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();

			}

		}

		private bool IsChildBlotter(ClientMarketData.BlotterRow parentBlotter, int blotterId)
		{

			if (parentBlotter.BlotterId == blotterId)
				return true;
			
			ClientMarketData.ObjectRow parentObject = parentBlotter.ObjectRow;
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in parentObject.GetObjectTreeRowsByObjectObjectTreeParentId())
				foreach (ClientMarketData.BlotterRow childBlotter in objectTreeRow.ObjectRowByObjectObjectTreeChildId.GetBlotterRows())
					if (IsChildBlotter(childBlotter, this.blotter.BlotterId))
						return true;

			return false;

		}
		
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void BlotterRowChangeEvent(object sender, ClientMarketData.BlotterRowChangeEvent executionRowChangeEvent)
		{

		}
		
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void BrokerRowChangeEvent(object sender, ClientMarketData.BrokerRowChangeEvent executionRowChangeEvent)
		{

		}

		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void ExecutionRowChangeEvent(object sender, ClientMarketData.ExecutionRowChangeEvent executionRowChangeEvent)
		{


		}
		
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void StylesheetRowChangeEvent(object sender, ClientMarketData.StylesheetRowChangeEvent executionRowChangeEvent)
		{

		}

		/// <summary>
		/// Handles a changed stylesheet in the data model.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="stylesheetRowChangeEvent">The event argument.</param>
		private void StylesheetChangedEvent(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// This will make it easier to operate on the changed record.
			ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

			// Reopen the document if the style sheet has changed.
			if (this.IsOpen && stylesheetRow.StylesheetId == this.stylesheetId)
			{
				DrawDocument();
			}

		}

	}

}
