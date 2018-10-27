namespace MarkThree.Guardian.Forms
{
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Forms;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.IO;
	using System.Threading;
	using System.Xml;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for WorkingOrderViewer.
	/// </summary>
	public class WorkingOrderViewer : MarkThree.Forms.SpreadsheetViewer
	{

		// Private Members
		private MarkThree.Forms.FragmentList fragmentList;
		private MarkThree.Guardian.Blotter blotter;
		private MarkThree.Guardian.Client.ClientMarketData clientMarketData;
		private MarkThree.Guardian.Client.ClientMarketData.BlotterRow blotterRow;
		private System.ComponentModel.IContainer components;
		private System.Int32 stylesheetId;
		private System.Windows.Forms.ContextMenu contextMenuWorkingOrder;
		private MenuItem menuItemMatchSeparator;
		private MenuItem menuItemMatchInstitution;
		private MenuItem menuItemMatchHedge;
        private MenuItem menuItemMatchBroker;
        private MenuItem menuItemPaste;
		private MenuItem menuItemSubmittionTypeSeperator;
		private MenuItem menuItemOverridePreferences;
		private MenuItem menuItemUsePreferences;
		private MenuItem menuItemPreventMatching;
        private MenuItem menuItemAutomaticSeperator;
        private MenuItem menuItemAutomatic;
		private delegate void WorkingOrderStateDelegate(WorkingOrderState workingOrderState);

		// Public Events
		public event MarkThree.Guardian.WorkingOrderEventHandler OpenWorkingOrder;
		public event System.EventHandler CloseWorkingOrder;
		public event System.EventHandler Execution;
		public event System.EventHandler DestinationOrder;
		public event System.EventHandler OrderFormSelected;

		/// <summary>
		/// Constructor for the WorkingOrderViewer
		/// </summary>
		public WorkingOrderViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// This is the identity of the viewer.
			this.blotter = null;

			// This list is used to generate the incremental changes that are written to the viewer after the data model has been
			// reconciled with the server's model.
			this.fragmentList = new FragmentList();

			// This keeps track of the stylesheet associated with this viewer.
			this.stylesheetId = Int32.MinValue;

			// This allows the base classes to access the context menu created for the block order viewer.  There is
			// likely a better way to use the 'ContextMenu'.
			this.ContextMenu = this.contextMenuWorkingOrder;

		}

		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Remove any components that have been added in.
			if (disposing)
				if (components != null)
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuWorkingOrder = new System.Windows.Forms.ContextMenu();
            this.menuItemPaste = new System.Windows.Forms.MenuItem();
            this.menuItemMatchSeparator = new System.Windows.Forms.MenuItem();
            this.menuItemMatchInstitution = new System.Windows.Forms.MenuItem();
            this.menuItemMatchHedge = new System.Windows.Forms.MenuItem();
            this.menuItemMatchBroker = new System.Windows.Forms.MenuItem();
            this.menuItemSubmittionTypeSeperator = new System.Windows.Forms.MenuItem();
            this.menuItemOverridePreferences = new System.Windows.Forms.MenuItem();
            this.menuItemUsePreferences = new System.Windows.Forms.MenuItem();
            this.menuItemPreventMatching = new System.Windows.Forms.MenuItem();
            this.menuItemAutomaticSeperator = new System.Windows.Forms.MenuItem();
            this.menuItemAutomatic = new System.Windows.Forms.MenuItem();
            this.clientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
            this.SuspendLayout();
            // 
            // contextMenuWorkingOrder
            // 
            this.contextMenuWorkingOrder.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPaste,
            this.menuItemMatchSeparator,
            this.menuItemMatchInstitution,
            this.menuItemMatchHedge,
            this.menuItemMatchBroker,
            this.menuItemSubmittionTypeSeperator,
            this.menuItemOverridePreferences,
            this.menuItemUsePreferences,
            this.menuItemPreventMatching,
            this.menuItemAutomaticSeperator,
            this.menuItemAutomatic});
            this.contextMenuWorkingOrder.Popup += new System.EventHandler(this.contextMenuWorkingOrder_Popup);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.Enabled = false;
            this.menuItemPaste.Index = 0;
            this.menuItemPaste.Text = "&Paste";
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // menuItemMatchSeparator
            // 
            this.menuItemMatchSeparator.Index = 1;
            this.menuItemMatchSeparator.Text = "-";
            // 
            // menuItemMatchInstitution
            // 
            this.menuItemMatchInstitution.Index = 2;
            this.menuItemMatchInstitution.Text = "Match &Institution";
            this.menuItemMatchInstitution.Click += new System.EventHandler(this.menuItemMatchInstitution_Click);
            // 
            // menuItemMatchHedge
            // 
            this.menuItemMatchHedge.Index = 3;
            this.menuItemMatchHedge.Text = "Match &Hedge";
            this.menuItemMatchHedge.Click += new System.EventHandler(this.menuItemMatchHedge_Click);
            // 
            // menuItemMatchBroker
            // 
            this.menuItemMatchBroker.Index = 4;
            this.menuItemMatchBroker.Text = "Match &Broker";
            this.menuItemMatchBroker.Click += new System.EventHandler(this.menuItemMatchBroker_Click);
            // 
            // menuItemSubmittionTypeSeperator
            // 
            this.menuItemSubmittionTypeSeperator.Index = 5;
            this.menuItemSubmittionTypeSeperator.Text = "-";
            // 
            // menuItemOverridePreferences
            // 
            this.menuItemOverridePreferences.Index = 6;
            this.menuItemOverridePreferences.Text = "&Override Preferences";
            this.menuItemOverridePreferences.Click += new System.EventHandler(this.menuItemOverridePreferences_Click);
            // 
            // menuItemUsePreferences
            // 
            this.menuItemUsePreferences.Index = 7;
            this.menuItemUsePreferences.Text = "&Use Preferences";
            this.menuItemUsePreferences.Click += new System.EventHandler(this.menuItemUsePreferences_Click);
            // 
            // menuItemPreventMatching
            // 
            this.menuItemPreventMatching.Index = 8;
            this.menuItemPreventMatching.Text = "&Prevent Matching";
            this.menuItemPreventMatching.Click += new System.EventHandler(this.menuItemPreventMatching_Click);
            // 
            // menuItemAutomaticSeperator
            // 
            this.menuItemAutomaticSeperator.Index = 9;
            this.menuItemAutomaticSeperator.Text = "-";
            // 
            // menuItemAutomatic
            // 
            this.menuItemAutomatic.Index = 10;
            this.menuItemAutomatic.Text = "Auto-E&xecute";
            this.menuItemAutomatic.Click += new System.EventHandler(this.menuItemAutomatic_Click);
            // 
            // WorkingOrderViewer
            // 
            this.Name = "WorkingOrderViewer";
            this.EndEdit += new MarkThree.Forms.EndEditEventHandler(this.WorkingOrderViewer_EndEdit);
            this.SelectionChanged += new System.EventHandler(this.WorkingOrderViewer_SelectionChanged);
            this.StylesheetChanged += new System.EventHandler(this.WorkingOrderViewer_StylesheetChanged);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Opens the viewer.
		/// </summary>
		/// <param name="tag">An object to be displayed in the viewer.</param>
		public override void Open(object tag)
		{

			// This viewer can only display blotters.
			if (!(tag is Blotter))
				throw new Exception(string.Format("{0} can't display objects of type {1}", this.GetType(), this.Tag.GetType()));

			// This is the identity of the object being viewed in this viewer.
			this.blotter = (Blotter)tag;

			// Initially none of the context menus for matching is visible.  Only when the selection is on a row can the status of
			// the line be read.
            this.menuItemPaste.Visible = false;
            this.menuItemMatchSeparator.Visible = false;
            this.menuItemMatchInstitution.Visible = false;
            this.menuItemMatchHedge.Visible = false;
            this.menuItemMatchBroker.Visible = false;
            this.menuItemSubmittionTypeSeperator.Visible = false;
            this.menuItemOverridePreferences.Visible = false;
            this.menuItemUsePreferences.Visible = false;
            this.menuItemPreventMatching.Visible = false;
            this.menuItemAutomatic.Visible = false;
            this.menuItemAutomaticSeperator.Visible = false;

			// The base class will spawn a background thread that will allow the operation to complete in the background.  Any
			// resources that are required by this viewer that may take some time can complete in a different thread than the main
			// message loop thread.  This often includes locking the data model to look at values needed for the viewer.
			base.Open(tag);

		}

		/// <summary>
		/// Opens the Block SourceOrder Viewer
		/// </summary>
		protected override void OpenCommand()
		{

			// This block will attach the appraisal viewer to the data model changes.
			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, equity traders
				// Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				this.blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (this.blotterRow == null)
					throw new Exception(String.Format("Blotter {0} does not exist", this.blotter.BlotterId));

				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsWorkingOrderStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.WorkingOrderStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Block SourceOrder stylesheet", this.blotter.BlotterId));

				// The stylesheets take a modest amount of time to parse from a string into an XSL structure.  As an optimization, 
				// this action is skipped if the new document in this viewer is using the same stylesheet as the last document 
				// loaded.
				if (this.stylesheetId != stylesheetRow.StylesheetId)
				{

					// This is the identity of the stylesheet used by this viewer.
					this.stylesheetId = stylesheetRow.StylesheetId;

					// This will load the TEXT object stored in the Stylesheet table into an internal structure that is used to
					// transform the document object model (DOM) into a viewable report.
					XmlDocument xslStylesheet = new XmlDocument();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					xslStylesheet.Load(xmlTextReader);
					this.XslStylesheet = xslStylesheet;

				}

				// Draw the document.  Note that while we are already in the background, the command must be queued so that 
				// document fragments are handled in the proper order as a brand new document.  That is, we wouldn't want a
				// fragment already in the queue to overwrite the document, especially if it doesn't belong.
				DrawDocument();

				// Install the event handlers.  The data model component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanging += new ClientMarketData.BlotterRowChangeEventHandler(ChangeBlotterRow);
				ClientMarketData.DestinationOrder.DestinationOrderRowChanging += new ClientMarketData.DestinationOrderRowChangeEventHandler(ChangeDestinationOrderRow);
				ClientMarketData.Execution.ExecutionRowChanging += new ClientMarketData.ExecutionRowChangeEventHandler(ChangeExecutionRow);
				ClientMarketData.Object.ObjectRowChanging += new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging += new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging += new ClientMarketData.OrderTypeRowChangeEventHandler(ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging += new ClientMarketData.PriceRowChangeEventHandler(ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging += new ClientMarketData.PriceTypeRowChangeEventHandler(ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging += new ClientMarketData.SecurityRowChangeEventHandler(ChangeSecurityRow);
				ClientMarketData.SourceOrder.SourceOrderRowChanging += new ClientMarketData.SourceOrderRowChangeEventHandler(ChangeSourceOrderRow);
				ClientMarketData.Status.StatusRowChanging += new ClientMarketData.StatusRowChangeEventHandler(ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging += new ClientMarketData.StylesheetRowChangeEventHandler(ChangeStylesheetRow);
				ClientMarketData.TimeInForce.TimeInForceRowChanging += new ClientMarketData.TimeInForceRowChangeEventHandler(ChangeTimeInForceRow);
				ClientMarketData.Timer.TimerRowChanging += new ClientMarketData.TimerRowChangeEventHandler(this.ChangeTimerRow);
				ClientMarketData.WorkingOrder.WorkingOrderRowChanging += new ClientMarketData.WorkingOrderRowChangeEventHandler(ChangeWorkingOrderRow);
				ClientMarketData.EndMerge += new EventHandler(EndMerge);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld)
					ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld)
					ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld)
					ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld)
					ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Closes the Block SourceOrder Viewer.
		/// </summary>
		protected override void CloseCommand()
		{

			// Make sure any exceptions are localized to this method.
			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanging -= new ClientMarketData.BlotterRowChangeEventHandler(ChangeBlotterRow);
				ClientMarketData.SourceOrder.SourceOrderRowChanging -= new ClientMarketData.SourceOrderRowChangeEventHandler(ChangeSourceOrderRow);
				ClientMarketData.DestinationOrder.DestinationOrderRowChanging -= new ClientMarketData.DestinationOrderRowChangeEventHandler(ChangeDestinationOrderRow);
				ClientMarketData.Execution.ExecutionRowChanging -= new ClientMarketData.ExecutionRowChangeEventHandler(ChangeExecutionRow);
				ClientMarketData.Object.ObjectRowChanging -= new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging -= new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging -= new ClientMarketData.OrderTypeRowChangeEventHandler(ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging -= new ClientMarketData.PriceRowChangeEventHandler(ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging -= new ClientMarketData.PriceTypeRowChangeEventHandler(ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging -= new ClientMarketData.SecurityRowChangeEventHandler(ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging -= new ClientMarketData.StatusRowChangeEventHandler(ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging -= new ClientMarketData.StylesheetRowChangeEventHandler(ChangeStylesheetRow);
				ClientMarketData.TimeInForce.TimeInForceRowChanging -= new ClientMarketData.TimeInForceRowChangeEventHandler(ChangeTimeInForceRow);
				ClientMarketData.Timer.TimerRowChanging -= new ClientMarketData.TimerRowChangeEventHandler(ChangeTimerRow);
				ClientMarketData.WorkingOrder.WorkingOrderRowChanging -= new ClientMarketData.WorkingOrderRowChangeEventHandler(ChangeWorkingOrderRow);
				ClientMarketData.EndMerge -= new EventHandler(EndMerge);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld)
					ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld)
					ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld)
					ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld)
					ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}
				
		}

		/// <summary>
		/// Handles a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeBlotterRow(object sender, ClientMarketData.BlotterRowChangeEvent blotterRowChangeEvent)
		{

			// The only events of interest for ths viewer are when rows are committed to the data model.
			if (blotterRowChangeEvent.Action == DataRowAction.Commit)
			{

				// When the database resets, the row used to filter the data becomes invalid because it has been deleted from the
				// data model.  A new row with the same blotter id is loaded from the server with the new data model.  This will
				// become the row used to filter this viewer from this point on.
				if (blotterRowChangeEvent.Row.BlotterId == this.blotter.BlotterId)
					this.blotterRow = blotterRowChangeEvent.Row;

				// When the blotter has changed, every row in the report containing a relation to that blotter item will be updated.
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in blotterRowChangeEvent.Row.GetWorkingOrderRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.Blotter);

			}

		}
	
		/// <summary>
		/// Event driver for a change to the SourceOrder table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeSourceOrderRow">The parameters for the event handler.</param>
		private void ChangeSourceOrderRow(object sender, ClientMarketData.SourceOrderRowChangeEvent customerOrderRowChangeEvent)
		{

			// When the customer order has changed, every row in the report containing a relation to that customer order item will 
			// be updated.
			if (customerOrderRowChangeEvent.Action == DataRowAction.Commit)
			{

				int workingOrderId = (int)customerOrderRowChangeEvent.Row[ClientMarketData.SourceOrder.WorkingOrderIdColumn,
					customerOrderRowChangeEvent.Row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
				ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
				if (workingOrderRow != null && Hierarchy.IsDescendant(this.blotterRow.ObjectRow,
					workingOrderRow.BlotterRow.ObjectRow))
					fragmentList.Add(DataAction.Update, workingOrderRow, Field.SourceOrder);

			}

		}
	
		/// <summary>
		/// Event handler for a change to the destinationOrders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void ChangeDestinationOrderRow(object sender, ClientMarketData.DestinationOrderRowChangeEvent destinationOrderRowChangeEvent)
		{

			// When the destination order has changed, every row in the report containing a relation to that destination order 
			// will be updated.
			if (destinationOrderRowChangeEvent.Action == DataRowAction.Commit)
			{

				int workingOrderId = (int)destinationOrderRowChangeEvent.Row[ClientMarketData.DestinationOrder.WorkingOrderIdColumn,
					destinationOrderRowChangeEvent.Row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
				ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
				if (workingOrderRow != null && Hierarchy.IsDescendant(this.blotterRow.ObjectRow,
					workingOrderRow.BlotterRow.ObjectRow))
					fragmentList.Add(DataAction.Update, workingOrderRow, Field.DestinationOrder);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the executions table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void ChangeExecutionRow(object sender, ClientMarketData.ExecutionRowChangeEvent executionRowChangeEvent)
		{

			// When the execution order has changed, every row in the report containing a relation to that execution order 
			// will be updated.
			if (executionRowChangeEvent.Action == DataRowAction.Commit)
			{

				int destinationOrderId = (int)executionRowChangeEvent.Row[ClientMarketData.Execution.DestinationOrderIdColumn,
					executionRowChangeEvent.Row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
				ClientMarketData.DestinationOrderRow destinationOrderRow = ClientMarketData.DestinationOrder.FindByDestinationOrderId(destinationOrderId);
                if (destinationOrderRow != null && Hierarchy.IsDescendant(this.blotterRow.ObjectRow,
					destinationOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					fragmentList.Add(DataAction.Update, destinationOrderRow.WorkingOrderRow, Field.Execution);

			}

		}
		
		/// <summary>
		/// Event driver for a change to the Object table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeObjectRow">The parameters for the event handler.</param>
		private void ChangeObjectRow(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

		}
	
		/// <summary>
		/// Event driver for a change to the ObjectTree table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeObjectTreeRow">The parameters for the event handler.</param>
		private void ChangeObjectTreeRow(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectTreeRowChangeEvent)
		{

		}
	
		/// <summary>
		/// Event driver for a change to the transactionType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="transactionTypeRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeOrderTypeRow(object sender, ClientMarketData.OrderTypeRowChangeEvent orderTypeRowChangeEvent)
		{

			// When the order type has changed, every row in the report containing a relation to the order type item will be updated.
			if (orderTypeRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in orderTypeRowChangeEvent.Row.GetWorkingOrderRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.OrderType);

		}
	
		/// <summary>
		/// This handler is called when prices have changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="priceRowChangingEvent">Event argument.</param>
		private void ChangePriceRow(object sender, ClientMarketData.PriceRowChangeEvent priceRowChangeEvent)
		{

			// A distinct list of Working Orders is constructed from the records when the changes are committed to the ADO.NET
			// database.  This list of Working Orders, along with any other Working Orders that are modified during the
			// reconcilliation perion, will be used to build an XML Fragment that is sent to the document to update the changes
			// incrementally.
			if (priceRowChangeEvent.Action == DataRowAction.Commit)
			{

				// Extract the price row from the event argumetns.
				ClientMarketData.PriceRow priceRow = priceRowChangeEvent.Row;

				// The code below will compare the current values to the original ones and set an indicator for which values have
				// changed.  This set of indicators will determine which values are included in the incremental XML fragment that
				// is created to update the viewer.
				FieldArray fields = FieldArray.Clear;

				// There will only be 'Current' rows after the first batch of data is read from the server into the data model.
				// This check is needed to prevent exceptions when comparing the current row to the original one.
				if (!priceRow.HasVersion(DataRowVersion.Original))
					return;

				// Set an indicator if the Last Price has changed.
				if (!priceRow[ClientMarketData.Price.LastPriceColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.LastPriceColumn, DataRowVersion.Current]))
					fields[Field.LastPrice] = true;

				// Set an indicator if the Ask Price has changed.
				if (!priceRow[ClientMarketData.Price.AskPriceColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.AskPriceColumn, DataRowVersion.Current]))
					fields[Field.AskPrice] = true;

				// Set an indicator if the Bid Price has changed.
				if (!priceRow[ClientMarketData.Price.BidPriceColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.BidPriceColumn, DataRowVersion.Current]))
					fields[Field.BidPrice] = true;

				// Set an indicator if the Last Size has changed.
				if (!priceRow[ClientMarketData.Price.LastSizeColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.LastSizeColumn, DataRowVersion.Current]))
					fields[Field.LastSize] = true;

				// Set an indicator if the Ask Size has changed.
				if (!priceRow[ClientMarketData.Price.AskSizeColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.AskSizeColumn, DataRowVersion.Current]))
					fields[Field.AskSize] = true;

				// Set an indicator if the Bid Size has changed.
				if (!priceRow[ClientMarketData.Price.BidSizeColumn, DataRowVersion.Original].Equals(
					priceRow[ClientMarketData.Price.BidSizeColumn, DataRowVersion.Current]))
					fields[Field.BidSize] = true;

                // Set an indicator if the Volume has changed.
                if (!priceRow[ClientMarketData.Price.VolumeColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.VolumeColumn, DataRowVersion.Current]))
                {
                    fields[Field.Volume] = true;
                    fields[Field.InterpolatedVolume] = true;
                }

                // Set an indicator if the VolumeWeightedAveragePrice has changed.
                if (!priceRow[ClientMarketData.Price.VolumeWeightedAveragePriceColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.VolumeWeightedAveragePriceColumn, DataRowVersion.Current]))
                    fields[Field.VolumeWeightedAveragePrice] = true;

				// This will generate a fragment for every Working Order in this viewer that contains the modified price.
				if (fields)
					foreach (ClientMarketData.WorkingOrderRow workingOrderRow in
						priceRow.SecurityRow.GetWorkingOrderRowsBySecurityWorkingOrderSecurityId())
						if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
							this.fragmentList.Add(DataAction.Update, workingOrderRow, fields);

			}

		}
	
		/// <summary>
		/// Event driver for a change to the PriceType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangePriceTypeRow">The parameters for the event handler.</param>
		private void ChangePriceTypeRow(object sender, ClientMarketData.PriceTypeRowChangeEvent orderTypeRowChangeEvent)
		{
		}
	
		/// <summary>
		/// Event driver for a change to the Security table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeSecurityRow">The parameters for the event handler.</param>
		private void ChangeSecurityRow(object sender, ClientMarketData.SecurityRowChangeEvent securityRowChangeEvent)
		{

			// When the security has changed, every row in the report containing a relation to the security item will be updated.
			if (securityRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in
					securityRowChangeEvent.Row.GetWorkingOrderRowsBySecurityWorkingOrderSecurityId())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.Security);

		}
	
		/// <summary>
		/// Event driver for a change to the status table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="statusRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeStatusRow(object sender, ClientMarketData.StatusRowChangeEvent statusRowChangeEvent)
		{

			// When the status has changed, every row in the report containing a relation to the status item will be updated.
			if (statusRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in statusRowChangeEvent.Row.GetWorkingOrderRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.Status);

		}
	
		/// <summary>
		/// Handles a changed stylesheet in the data model.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="stylesheetRowChangeEvent">The event argument.</param>
		private void ChangeStylesheetRow(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// The only changes that impact the viewer are committed records.
			if (stylesheetRowChangeEvent.Action == DataRowAction.Commit)
			{

				// This will make it easier to operate on the changed record.
				ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

				// Reload the stylesheet if the modified stylesheet is the one currently used by this viewer.
				if (stylesheetRow.StylesheetId == this.stylesheetId)
				{

					// This will read the stylesheet out of the changed record and into the viewers internal data structures.
					XmlDocument xslStylesheet = new XmlDocument();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					xslStylesheet.Load(xmlTextReader);
					this.XslStylesheet = xslStylesheet;

					// This indicates that the list of fragments is invalid and the entire document should be redrawn.
					this.fragmentList.IsValid = false;

				}

			}

		}

		/// <summary>
		/// Event driver for a change to the timeInForce table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="timeInForceRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeTimeInForceRow(object sender, ClientMarketData.TimeInForceRowChangeEvent timeInForceRowChangeEvent)
		{

			// When the order type has changed, every row in the report containing a relation to the order type item will be updated.
			if (timeInForceRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in timeInForceRowChangeEvent.Row.GetWorkingOrderRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.TimeInForce);

		}
	
		/// <summary>
		/// Event handler for a match.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void ChangeTimerRow(object sender, DataSetMarket.TimerRowChangeEvent timerRowChangeEvent)
		{

			// When a new, pending match record has been added to the data mode, start a thread that will
			// display the notification window.
			if (timerRowChangeEvent.Action == DataRowAction.Commit)
			{

				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in timerRowChangeEvent.Row.GetWorkingOrderRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, workingOrderRow, Field.Timer);

			}

		}

		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeWorkingOrderRow(object sender, ClientMarketData.WorkingOrderRowChangeEvent workingOrderRowChangeEvent)
		{

			// Several events are generated as the row is processed.  This trigger is only interested in the rows as they're being
			// comitted to the data model.
			if (workingOrderRowChangeEvent.Action == DataRowAction.Commit)
			{

				// Extract the modified row from the event parameters.
				ClientMarketData.WorkingOrderRow workingOrderRow = workingOrderRowChangeEvent.Row;

				// The work is broken up into inserts, updates and deletes.  This section will create a fragment for deleted rows 
				// when they belong to this blotter.
				if (workingOrderRow.RowState == DataRowState.Deleted)
				{

					// If this order was displayed in the current blotter then create an instruction to delete it.
					int originalBlotterId = (int)workingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn, DataRowVersion.Original];
					ClientMarketData.BlotterRow originalBlotterRow = ClientMarketData.Blotter.FindByBlotterId(originalBlotterId);
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, originalBlotterRow.ObjectRow))
						this.fragmentList.Add(DataAction.Delete, workingOrderRow);

				}
				else
				{

					// This next section will determine whether the row was added or modified.
					if (workingOrderRow.HasVersion(DataRowVersion.Original))
					{

						// This array will collect the fields that have changed.
						FieldArray fields = FieldArray.Clear;

						// A modified blotter identifier is very tricky to handle.  If the order has been moved to another blotter,
						// then it can still be visible, if it was moved to a child blotter, or invisible if the new location
						// doesn't appear in the current blotter's hierarchy.  Additionally, an order from another blotter may be
						// moved onto this one.  Depending on the combination, either an insert, update or delete instruction can
						// be generated.  The first step is to see if the blotter identifier has been changed.
						int originalBlotterId = (int)workingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
							DataRowVersion.Original];
						int currentBlotterId = (int)workingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
							DataRowVersion.Current];
						if (originalBlotterId != currentBlotterId)
						{

							// If the blotter associated with this order has changed then the hierarchies need to be searched to
							// see if the order should or should not be displayed in the current viewer.
							ClientMarketData.BlotterRow originalBlotterRow =
								ClientMarketData.Blotter.FindByBlotterId(originalBlotterId);
							ClientMarketData.BlotterRow currentBlotterRow =
								ClientMarketData.Blotter.FindByBlotterId(currentBlotterId);
							bool isOriginalDescendant = Hierarchy.IsDescendant(this.blotterRow.ObjectRow, originalBlotterRow.ObjectRow);
							bool isCurrentDescendant = Hierarchy.IsDescendant(this.blotterRow.ObjectRow, currentBlotterRow.ObjectRow);

							// This will take care of three of the four possibilities.  First, the order is displayed on the
							// current blotter and the new blotter.  In this case we have a simple update.  If the order previously
							// did not belong on the blotter but now it does, an "insert" instruction is created.  If the order
							// previously was displayed in this viewer but can't be viewed in it's new hierarchy, then a "delete"
							// instruction is created.  The final combination, the order was shown on another blotter and was moved
							// to a blotter that is still not in the view, is ignored.  Note that an insert or delete will override
							// and field changes when the fragments are added to the list.  Also note that fragments are combined
							// when possible.
							if (isOriginalDescendant && isCurrentDescendant)
								fields[Field.Blotter] = true;
							else
							{

								// If the order has been moved onto this blotter, then display the whole row.  If the order has
								// been removed from this blotter, then delete it from the viewer.
								if (!isOriginalDescendant && isCurrentDescendant)
									this.fragmentList.Add(DataAction.Insert, workingOrderRow);
								else
								{
									if (isOriginalDescendant && !isCurrentDescendant)
										this.fragmentList.Add(DataAction.Delete, workingOrderRow);
								}

								// At this point, the modified record is either not on the viewer or totally new to the viewer, but
								// there is no need to check the remaining fields.
								return;

							}

						}

						// If the status has changed then give the template a chance to redraw all the fields.
						int originalStatusCode = (int)workingOrderRow[ClientMarketData.WorkingOrder.StatusCodeColumn,
							DataRowVersion.Original];
						int currentStatusCode = (int)workingOrderRow[ClientMarketData.WorkingOrder.StatusCodeColumn,
							DataRowVersion.Current];
						if (originalStatusCode != currentStatusCode)
							fields = FieldArray.Set;

						// IsHeld Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.SubmissionTypeCodeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.SubmissionTypeCodeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.SubmissionTypeCode] = true;
						}

						// IsBrokerMatch Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.IsBrokerMatchColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.IsBrokerMatchColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.IsBrokerMatch] = true;
						}

						// IsInstitutionMatch Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.IsInstitutionMatchColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.IsInstitutionMatchColumn, DataRowVersion.Current]))
                        {
                            fields[Field.Status] = true;
							fields[Field.IsInstitutionMatch] = true;
                        }

						// IsHedgeMatch Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.IsHedgeMatchColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.IsHedgeMatchColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.IsHedgeMatch] = true;
						}

                        // IsAutomatic field
                        if (!workingOrderRow[ClientMarketData.WorkingOrder.IsAutomaticColumn, DataRowVersion.Original].Equals(
                            workingOrderRow[ClientMarketData.WorkingOrder.IsAutomaticColumn, DataRowVersion.Current]))
                        {
                            fields[Field.Status] = true;
                            fields[Field.AutoExecute] = true;
                        }

                        // AutomaticQuantity field
                        if (!workingOrderRow[ClientMarketData.WorkingOrder.AutomaticQuantityColumn, DataRowVersion.Original].Equals(
                            workingOrderRow[ClientMarketData.WorkingOrder.AutomaticQuantityColumn, DataRowVersion.Current]))
                        {
                            fields[Field.Status] = true;
                            fields[Field.AutoExecute] = true;
                        }

                        // LimitPrice field
                        if (!workingOrderRow[ClientMarketData.WorkingOrder.LimitPriceColumn, DataRowVersion.Original].Equals(
                            workingOrderRow[ClientMarketData.WorkingOrder.LimitPriceColumn, DataRowVersion.Current]))
                            fields[Field.LimitPrice] = true;

						// OrderType Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.OrderTypeCodeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.OrderTypeCodeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.OrderType] = true;
						}

						// TimeInForce Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.TimeInForceCodeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.TimeInForceCodeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.TimeInForce] = true;
						}

						// StartTime Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.StartTimeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.StartTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.StartTime] = true;
						}

						// StopTime Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.StopTimeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.StopTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.StopTime] = true;
						}

						// MaximumVolatility Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.MaximumVolatilityColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.MaximumVolatilityColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.MaximumVolatility] = true;
						}

						// SubmittedQuantity Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.SubmittedQuantityColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.SubmittedQuantityColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.SubmittedQuantity] = true;
						}

						// NewsFreeTime Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.NewsFreeTimeColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.NewsFreeTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.NewsFreeTime] = true;
						}

						// Security Field
						if (!workingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Original].Equals(
							workingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.Security] = true;
						}

						// If any of the fields need to be updated, then generate a change instruction.  This will be combined with
						// other change instructions to create fragments that will update only the requested fields in the viewer.
						if (fields)
							this.fragmentList.Add(DataAction.Update, workingOrderRow, fields);

					}
					else
					{

						// The entire row is added if the order is new to this viewer.
						if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
							this.fragmentList.Add(DataAction.Insert, workingOrderRow);

					}

				}

			}

		}

		/// <summary>
		/// Handles the event that is generated when all the data model changes have been incorporated.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// When all the fragments have been collected they are sent to be drawn in another thread.  Note that a clean, empty
			// fragment list is created each time the data model is changed.  This prevents the data model event handlers from
			// corrupting the list of fragments before they can be drawn.
			if (this.fragmentList.IsValid)
			{
				if (this.fragmentList.Count != 0)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(DrawFragmentCommand), this.fragmentList);
					this.fragmentList = new FragmentList();
				}
			}
			else
			{
				DrawDocument();
				this.fragmentList.Clear();
			}

		}

		/// <summary>
		/// Called when an destinationOrder quantity is added to the block order.
		/// </summary>
		/// <param name="destinationOrderEventArgs">Event parameters.</param>
		protected virtual void OnDestinationOrder()
		{
		
			// Broadcast the event to anyone listening.
			if (this.DestinationOrder != null)
				this.DestinationOrder(this, EventArgs.Empty);

		}

		/// <summary>
		/// Called when an execution quantity is added to the block order.
		/// </summary>
		/// <param name="executionEventArgs">Event parameters.</param>
		protected virtual void OnExecution()
		{
		
			// Broadcast the event to anyone listening.
			if (this.Execution != null)
				this.Execution(this, EventArgs.Empty);

		}

		/// <summary>
		/// Multicasts a change in the currently viewed block order.
		/// </summary>
		/// <param name="openWorkingOrderEventArgs">The event argument.</param>
		protected virtual void OnOpenWorkingOrder(WorkingOrderEventArgs workingOrderEventArgs)
		{

			// Multicast the fact that the currently viewed block order has changed to anyone
			// interested.  This will typically be the blotter which will inform the destinationOrder
			// and execution viewers that a new block is currently being viewed.
			if (OpenWorkingOrder != null)
				this.OpenWorkingOrder(this, workingOrderEventArgs);

		}

		/// <summary>
		/// Multicasts a change in the currently viewed block order.
		/// </summary>
		/// <param name="closeWorkingOrderEventArgs">The event argument.</param>
		protected virtual void OnCloseWorkingOrder(WorkingOrderEventArgs workingOrderEventArgs)
		{

			// Multicast the fact that the currently viewed block order has changed to anyone
			// interested.  This will typically be the blotter which will inform the destinationOrder
			// and execution viewers that a new block is currently being viewed.
			if (CloseWorkingOrder != null)
				this.CloseWorkingOrder(this, workingOrderEventArgs);

		}

		/// <summary>
		/// Writes the complete document out to the viewer.
		/// </summary>
		/// <remarks>
		/// This thread is responsible for recreating the Document Object Model for the blotter and translating
		/// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
		/// DOM is complete to either update the entire document, if the structure has changed, or only update
		/// the changed cells, when only the data has changed.
		/// </remarks>
		protected override void DrawDocumentCommand(object parameter)
		{

			// Extract the blotter to be drawn from the thread arguments.
			Blotter blotter = (Blotter)parameter;

#if DEBUG
			// Create the blotter Document Object Model.
			WorkingOrderDocument workingOrderDocument = new WorkingOrderDocument(blotter.BlotterId);

			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				workingOrderDocument.Save("workingOrderDOM.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will draw the document by passing it through the stylesheet.
			this.XmlDocument = workingOrderDocument;
#else
			// This will draw the document by passing it through the stylesheet.
			this.XmlDocument = new WorkingOrderDocument(blotter.BlotterId);
#endif

		}

		/// <summary>
		/// Writes the incremental changes out to the viewer.
		/// </summary>
		/// <param name="parameters">The thread arguments.</param>
		private void DrawFragmentCommand(object parameter)
		{

			// Extract the list of incremental changes from the thread parameters.
			FragmentList fragmentList = (FragmentList)parameter;

#if DEBUGXML
			// This will create an XML document from the consolidated list of changes.
			XmlDocument workingOrderFragment = new WorkingOrderDocument(fragmentList);
			
			// Make sure that writing the debug file doesn't interrupt the session.
				try
				{

					// During debugging, it's very useful to have a copy of the DOM on the disk.
					workingOrderFragment.Save("workingOrderFragment.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the event log.
					EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				}

			// This will write the incremental changes out to the viewer.
			this.XmlDocument = workingOrderFragment;
#else
			// This will write the incremental changes out to the viewer.
			this.XmlDocument = new WorkingOrderDocument(fragmentList);
#endif

		}

		/// <summary>
		/// Saves the stylesheet to the persistent store.
		/// </summary>
		/// <param name="parameters">Thread arguments (not used).</param>
		private void WriteStylesheetChanges(object parameter)
		{

			// This batch is used to save the stylesheet.
			Batch batch = new Batch();
			MethodPlan insertMethod = null;

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will save the stylesheet when the document is closed.  If the stylesheet is the generic one shared by all
				// users, then it will become the new local copy for this user.  If the user already has a personal copy of the
				// stylesheet, it is saved to the database.
				ClientMarketData.StylesheetRow stylesheetRow = ClientMarketData.Stylesheet.FindByStylesheetId(this.stylesheetId);
				if (stylesheetRow != null)
				{

					// This will save the users personal preferences for the stylesheet back to the database.
					TransactionPlan transactionPlan = batch.Transactions.Add();
					AssemblyPlan assemblyPlan = batch.Assemblies.Add("Core Service");
					TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.Stylesheet");
					insertMethod = transactionPlan.Methods.Add(typePlan, "Update");
					insertMethod.Parameters.Add(new InputParameter("stylesheetId", stylesheetRow.StylesheetId));
					insertMethod.Parameters.Add(new InputParameter("rowVersion", stylesheetRow.RowVersion));
					insertMethod.Parameters.Add(new InputParameter("text", this.XslStylesheet.OuterXml));
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
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Execute the command.  If it is successful, the stylesheet used by this document is updated.
			if (ClientMarketData.Execute(batch, this, new BatchExceptionEventHandler(ShowBatchExceptions)))
				this.stylesheetId = (int)insertMethod.Parameters.Return;

		}

		private void WorkingOrderViewer_EndEdit(object sender, MarkThree.Forms.EndEditEventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, this.ActiveColumn.ColumnName, e.Result });

		}

		private void UpdateWorkingOrder(object parameter)
		{

			// Extract the thread parameters
			object[] parameters = (object[])parameter;
			object[] selectedItems = (object[])parameters[0];
			string activeColumnName = (string)parameters[1];
			string result = (string)parameters[2];

			Batch batch = new Batch();

			try
			{

				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(Timeout.Infinite);
                ClientMarketData.TraderLock.AcquireReaderLock(Timeout.Infinite);
                ClientMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);
                ClientMarketData.TraderVolumeSettingLock.AcquireReaderLock(Timeout.Infinite);
                ClientMarketData.VolumeCategoryLock.AcquireReaderLock(Timeout.Infinite);
            
				foreach (object[] key in selectedItems)
				{

					int workingOrderId = (int)key[0];

					ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);

                   
					
                    if (workingOrderRow != null)
					{

						// This will save the users personal preferences for the stylesheet back to the database.
						TransactionPlan transactionPlan = batch.Transactions.Add();
						AssemblyPlan assemblyPlan = batch.Assemblies.Add("Core Service");
						TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.WorkingOrder");
						MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Update");
						methodPlan.Parameters.Add(new InputParameter("workingOrderId", workingOrderRow.WorkingOrderId));
						methodPlan.Parameters.Add(new InputParameter("rowVersion", workingOrderRow.RowVersion));

						try
						{

							if (activeColumnName == "SubmissionTypeCode" || activeColumnName == "SubmissionTypeImage")
							{
								char firstChar = Convert.ToString(result)[0];
								int submissionTypeCode = Char.ToUpper(firstChar) == 'Y' ? SubmissionType.AlwaysMatch :
									Char.ToUpper(firstChar) == 'P' ? SubmissionType.UsePeferences : SubmissionType.NeverMatch;
								methodPlan.Parameters.Add(new InputParameter("submissionTypeCode", submissionTypeCode));
							}

							if (activeColumnName == "SubmittedQuantity")
								methodPlan.Parameters.Add(new InputParameter("submittedQuantity", Convert.ToDecimal(result)));

							if (activeColumnName == "MaximumVolatility")
								methodPlan.Parameters.Add(new InputParameter("maximumVolatility", Convert.ToDecimal(result)));

							if (activeColumnName == "MinimumQuantity")
								methodPlan.Parameters.Add(new InputParameter("minimumQuantity", Convert.ToDecimal(result)));

							if (activeColumnName == "StartTime")
								methodPlan.Parameters.Add(new InputParameter("startTime", Convert.ToDateTime(result)));

							if (activeColumnName == "StopTime")
								methodPlan.Parameters.Add(new InputParameter("stopTime", Convert.ToDateTime(result)));

							if (activeColumnName == "NewsFreeTime")
								methodPlan.Parameters.Add(new InputParameter("newsFreeTime", Convert.ToInt32(result)));

							if (activeColumnName == "IsBrokerMatch")
								methodPlan.Parameters.Add(new InputParameter("isBrokerMatch", Convert.ToBoolean(result)));

							if (activeColumnName == "IsInstitutionMatch")
								methodPlan.Parameters.Add(new InputParameter("isInstitutionMatch", Convert.ToBoolean(result)));

							if (activeColumnName == "IsHedgeMatch")
								methodPlan.Parameters.Add(new InputParameter("isHedgeMatch", Convert.ToBoolean(result)));

                            if (activeColumnName == "LimitPrice")
                                methodPlan.Parameters.Add(new InputParameter("limitPrice", Convert.ToDecimal(result)));

                            if (activeColumnName == "IsAutomatic")
                            {
                                bool isAutomatic = Convert.ToBoolean(result);
                                if (isAutomatic)
                                {
                                    // get the volume category ID for this working order
                                    int volumeCategoryId = workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.VolumeCategoryRow.VolumeCategoryId;

                                    // iterate through each trader volume setting row to find the auto-execute quantity for this volume category
                                    foreach(ClientMarketData.TraderVolumeSettingRow traderVolumeRow 
                                        in ClientMarketData.Trader.FindByTraderId(Preferences.UserId).GetTraderVolumeSettingRows())
                                    {
                                        if (traderVolumeRow.VolumeCategoryId == volumeCategoryId)
                                        {
                                            methodPlan.Parameters.Add(new InputParameter("automaticQuantity", traderVolumeRow.AutoExecuteQuantity));
                                            break;
                                        }
                                    }
                                } 
                                else
                                {
                                    methodPlan.Parameters.Add(new InputParameter("automaticQuantity", 0));
                                }

                                methodPlan.Parameters.Add(new InputParameter("isAutomatic", isAutomatic));
                            }

                            if (activeColumnName == "AutomaticQuantity")
                            {
                                // Note: any format exception should be caught below
                                decimal automaticQuantity = Convert.ToDecimal(result);

                                methodPlan.Parameters.Add(new InputParameter("automaticQuantity", automaticQuantity));
                            }
                             

						}
						catch (Exception exception)
						{

							MessageBox.Show(exception.Message, "Guardian Error");

						}

					}

				}

			}
			finally
			{

				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld) ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
                if (ClientMarketData.TraderLock.IsReaderLockHeld) ClientMarketData.TraderLock.ReleaseReaderLock();
                if (ClientMarketData.TraderVolumeSettingLock.IsReaderLockHeld) ClientMarketData.TraderVolumeSettingLock.ReleaseReaderLock();
                if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
                if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld) ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Execute the command.
			ClientMarketData.Execute(batch, this, new BatchExceptionEventHandler(ShowBatchExceptions));

		}

		/// <summary>
		/// Show the exceptions generated from a batch command.
		/// </summary>
		/// <param name="sender">The object that originated the events.</param>
		/// <param name="batchExceptionEventArgs">The event arguments.</param>
		private void ShowBatchExceptions(object sender, BatchExceptionEventArgs batchExceptionEventArgs)
		{

			// Show each of the errors until either the user hits the 'Cancel' button, or all messages have been read.
			foreach (Exception exception in batchExceptionEventArgs.BatchException.Exceptions)
				if (MessageBox.Show(exception.Message, Application.SafeTopLevelCaptionFormat, MessageBoxButtons.OKCancel,
					MessageBoxIcon.Exclamation) == DialogResult.Cancel)
					return;

		}

		/// <summary>
		/// Handles a change to the stylesheet in this viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event arguments</param>
		private void WorkingOrderViewer_StylesheetChanged(object sender, System.EventArgs e)
		{

			// This command will write the stylesheet changes back out to the server in a background thread so it doesn't interrupt
			// the user's interface.
			ThreadPool.QueueUserWorkItem(new WaitCallback(WriteStylesheetChanges));
		
		}

		private void menuItemCusomizeCurrenView_Click(object sender, System.EventArgs e)
		{

			// This will allow the user to select, remove and arrange the order of the columns in the viewer.
			this.SelectColumns();

		}

		public virtual void OnOpenWorkingOrder(object sender, WorkingOrderEventArgs workingOrderEventArgs)
		{

			if (this.OpenWorkingOrder != null)
				this.OpenWorkingOrder(this, workingOrderEventArgs);

		}

		public void OnCloseWorkingOrder(object sender)
		{

			if (this.CloseWorkingOrder != null)
				this.CloseWorkingOrder(sender, EventArgs.Empty);

		}

		private void WorkingOrderViewer_SelectionChanged(object sender, System.EventArgs e)
		{

			OnCloseWorkingOrder(this);

			object[] selectedItems = this.SelectedItems;
			WorkingOrder[] workingOrderList = new WorkingOrder[selectedItems.Length];
			for (int index = 0; index < selectedItems.Length; index++)
			{
				object[] key = (object[])selectedItems[index];
				workingOrderList[index] = new WorkingOrder((int)key[0], this.blotter.BlotterId);
			}

			OnOpenWorkingOrder(this, new WorkingOrderEventArgs(workingOrderList));

		}

		private void menuItemOrderForm_Click(object sender, EventArgs e)
		{

			// If there is an order form associated with this viewer, then call it.
			if (this.OrderFormSelected != null)
				this.OrderFormSelected(this, EventArgs.Empty);

		}

		protected override void OnSelectionChanged(object sender)
		{

			string columnName = this.ActiveColumn != null ? this.ActiveColumn.ColumnName : string.Empty;
			ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateMenuItems), new object[] {this.SelectedItems, columnName});
			base.OnSelectionChanged(sender);

		}

		struct WorkingOrderState
		{

			public string ColumnName;
			public object IsBrokerMatch;
			public object IsHedgeMatch;
			public object IsInstitutionMatch;
            public object IsAutomatic;

		}

		private void CalculateMenuItems(object parameter)
		{

			// Extract the thread parameters
			object[] parameters = (object[])parameter;
			object[] keyArray = (object[])parameters[0];
			string columnName = (string)parameters[1];

			WorkingOrderState workingOrderState = new WorkingOrderState();
			workingOrderState.ColumnName = columnName;

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				int records = 0;
				int brokerMatches = 0;
				int institutionMatches = 0;
				int hedgeMatches = 0;
                int automaticMatches = 0;
				foreach (object[] key in keyArray)
				{

					int workingOrderId = (int)key[0];
					ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
					if (workingOrderRow != null)
					{
						records++;
						if (workingOrderRow.IsBrokerMatch)
							brokerMatches++;
						if (workingOrderRow.IsInstitutionMatch)
							institutionMatches++;
						if (workingOrderRow.IsHedgeMatch)
							hedgeMatches++;
                        if (workingOrderRow.IsAutomatic)
                            automaticMatches++;
					}

					workingOrderState.IsBrokerMatch = brokerMatches == 0 ? (object)false : brokerMatches == records ? (object)true : null;
					workingOrderState.IsInstitutionMatch = institutionMatches == 0 ? (object)false : institutionMatches == records ? (object)true : null;
					workingOrderState.IsHedgeMatch = hedgeMatches == 0 ? (object)false : hedgeMatches == records ? (object)true : null;
                    workingOrderState.IsAutomatic = automaticMatches == 0 ? (object)false : automaticMatches == records ? (object)true : null; 
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
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			Invoke(new WorkingOrderStateDelegate(SetContextMenu), new object[] {workingOrderState});

		}

		private void SetContextMenu(WorkingOrderState workingOrderState)
		{
            // hide all of the menu items
            this.menuItemPaste.Visible = false;
            this.menuItemMatchSeparator.Visible = false;
            this.menuItemMatchInstitution.Visible = false;
            this.menuItemMatchHedge.Visible = false;
            this.menuItemMatchBroker.Visible = false;
            this.menuItemSubmittionTypeSeperator.Visible = false;
            this.menuItemOverridePreferences.Visible = false;
            this.menuItemUsePreferences.Visible = false;
            this.menuItemPreventMatching.Visible = false;
            this.menuItemAutomatic.Visible = false;
            this.menuItemAutomaticSeperator.Visible = false;
            
            // show all of the menu items based on the column
			switch (workingOrderState.ColumnName)
			{
			case "IsInstitutionMatch":
                this.menuItemMatchInstitution.Checked = workingOrderState.IsInstitutionMatch != null &&
					(bool)workingOrderState.IsInstitutionMatch;
				this.menuItemMatchInstitution.Visible = true;
				break;

			case "IsHedgeMatch":
                this.menuItemMatchHedge.Checked = workingOrderState.IsHedgeMatch != null &&
					(bool)workingOrderState.IsHedgeMatch;
				this.menuItemMatchHedge.Visible = true;
				break;

			case "IsBrokerMatch":
                this.menuItemMatchBroker.Checked = workingOrderState.IsBrokerMatch != null &&
					(bool)workingOrderState.IsBrokerMatch;
				this.menuItemMatchBroker.Visible = true;
				break;

			case "SubmissionTypeImage":
               	this.menuItemOverridePreferences.Visible = true;
				this.menuItemUsePreferences.Visible = true;
				this.menuItemPreventMatching.Visible = true;
				break;

            case "IsAutomatic":
                this.menuItemAutomatic.Visible = true;
                this.menuItemAutomatic.Checked = workingOrderState.IsAutomatic != null &&
                    (bool)workingOrderState.IsAutomatic;
                break;

            case "":
                this.menuItemPaste.Visible = true;
                break;

			default:
                // by default, we don't show any menu items
				break;

			}

		}

		private void menuItemMatchInstitution_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "IsInstitutionMatch", this.menuItemMatchInstitution.Checked ? "false" : "true" });

		}

		private void menuItemMatchHedge_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "IsHedgeMatch", this.menuItemMatchHedge.Checked ? "false" : "true" });

		}

		private void menuItemMatchBroker_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "IsBrokerMatch", this.menuItemMatchBroker.Checked ? "false" : "true" });

		}

		private void menuItemOverridePreferences_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "SubmissionTypeImage", "Y" });

		}

		private void menuItemUsePreferences_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "SubmissionTypeImage", "P" });

		}

		private void menuItemPreventMatching_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "SubmissionTypeImage", "N" });

		}

        private void menuItemAutomatic_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWorkingOrder), new object[] { this.SelectedItems, "IsAutomatic", this.menuItemAutomatic.Checked ? "false" : "true" });
        }

		public void ShowMatchingColumns(bool isShown)
		{

			string[] matchingColumnList = { "IsInstitutionMatch", "IsHedgeMatch", "IsBrokerMatch", "StartTime", "StopTime", "NewsFreeTime", "MaximumVolatility" };

			try
			{

				this.Spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				foreach (Stylesheet.ColumnNode column in this.Stylesheet.Columns)
					foreach (string columnName in matchingColumnList)
						if (column.Identifier == columnName)
						{
							SpreadsheetColumn spreadsheetColumn = (SpreadsheetColumn)this.Spreadsheet.Columns[column.Identifier];
							spreadsheetColumn.IsVisible = isShown;
						}

			}
			finally
			{

				this.Spreadsheet.Lock.ReleaseReaderLock();

			}

			this.Spreadsheet.Refresh();

		}

		public void ShowVolumeColumns(bool isShown)
		{

			string[] matchingColumnList = { "RunningVolume", "VolumeCategoryMnemonic" };

			try
			{

				this.Spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);

				foreach (Stylesheet.ColumnNode column in this.Stylesheet.Columns)
					foreach (string columnName in matchingColumnList)
						if (column.Identifier == columnName)
						{
							SpreadsheetColumn spreadsheetColumn = (SpreadsheetColumn)this.Spreadsheet.Columns[column.Identifier];
							spreadsheetColumn.IsVisible = isShown;
						}

			}
			finally
			{

				this.Spreadsheet.Lock.ReleaseReaderLock();

			}

			this.Spreadsheet.Refresh();

		}

		public void SetFilter(int filterId)
		{

			switch (filterId)
			{

			case 0:

				// Set the filter and refresh the document with the new stylesheet.
				this.RowFilter = string.Empty;
				break;

			case 1:

				// Set the filter and refresh the document with the new stylesheet.
				this.RowFilter = "Convert(StatusCode, 'System.Int32')=9";
				break;

			case 2:

				this.RowFilter = "Convert(StatusCode, 'System.Int32')<>9";
				break;

			}

		}

		private void menuItemPaste_Click(object sender, EventArgs e)
		{

			IDataObject iDataObject = Clipboard.GetDataObject();
			MemoryStream memoryStream = (MemoryStream)iDataObject.GetData("XML Spreadsheet");
			if (memoryStream != null)
			{


				// This batch will collect the information needed to execute a complex transaction on the server.  The first part of 
				// the batch sets up the transaction: the assembly where the types and methods are found.  It also sets up a
				// transaction for a complex operation. In this case, the transaction is not that complex, just a single method to be
				// executed.
				Batch batch = new Batch();
				AssemblyPlan assembly = batch.Assemblies.Add("Trading Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Trading.SourceOrder");
				TransactionPlan transaction = batch.Transactions.Add();

				memoryStream.Position = 0L;
				XmlDataDocument xmlDataDocument = new XmlDataDocument();
				xmlDataDocument.DataSet.ReadXml(memoryStream, XmlReadMode.InferSchema);

				DataTable rowTable = xmlDataDocument.DataSet.Tables["Row"];
				int index = 0;
				foreach (DataRow dataRow in rowTable.Rows)
				{

					if (index++ == 0)
						continue;

					string side = (string)((dataRow.GetChildRows("Row_Cell")[0]).GetChildRows("Cell_Data")[0][1]);
					int orderTypeCode = side == "B" ? OrderType.Buy : OrderType.Sell;
					string symbol = (string)((dataRow.GetChildRows("Row_Cell")[1]).GetChildRows("Cell_Data")[0][1]);
					decimal	quantity = Convert.ToDecimal((string)((dataRow.GetChildRows("Row_Cell")[2]).GetChildRows("Cell_Data")[0][1]));
					object isInstitutionMatchObject = (dataRow.GetChildRows("Row_Cell")[3]).GetChildRows("Cell_Data")[0][1];
					bool isInstitutionMatch = isInstitutionMatchObject == null ? false : Convert.ToBoolean(((string)isInstitutionMatchObject).ToUpper() == "Y");
					object isHedgeMatchObject = (dataRow.GetChildRows("Row_Cell")[4]).GetChildRows("Cell_Data")[0][1];
					bool isHedgeMatch = isHedgeMatchObject == null ? false : Convert.ToBoolean(((string)isHedgeMatchObject).ToUpper() == "Y");
					object isBrokerMatchObject = (dataRow.GetChildRows("Row_Cell")[5]).GetChildRows("Cell_Data")[0][1];
					bool isBrokerMatch = isBrokerMatchObject == null ? false : Convert.ToBoolean(((string)isBrokerMatchObject).ToUpper() == "Y");

					// These are the parameters used to create a Source Order.  The Working Order will be created implicitly.
					MethodPlan methodPlan = transaction.Methods.Add(type, "Insert");
					methodPlan.Parameters.Add(new InputParameter("blotterId", this.blotter.BlotterId));
					methodPlan.Parameters.Add(new InputParameter("isBrokerMatch", isBrokerMatch));
					methodPlan.Parameters.Add(new InputParameter("isHedgeMatch", isHedgeMatch));
					methodPlan.Parameters.Add(new InputParameter("isInstitutionMatch", isInstitutionMatch));
					methodPlan.Parameters.Add(new InputParameter("submissionTypeCode", SubmissionType.UsePeferences));
					methodPlan.Parameters.Add(new InputParameter("orderedQuantity", quantity));
					methodPlan.Parameters.Add(new InputParameter("orderTypeCode", orderTypeCode));
					methodPlan.Parameters.Add(new InputParameter("priceTypeCode", PriceType.Market));
					methodPlan.Parameters.Add(new InputParameter("securityId", symbol));
					methodPlan.Parameters.Add(new InputParameter("settlementId", "USD"));
					methodPlan.Parameters.Add(new InputParameter("timeInForceCode", TimeInForce.Day));

					
				}

				try
				{

					// Execute the batch.
					ClientMarketData.Execute(batch);

				}
				catch (BatchException batchException)
				{

					foreach (Exception exception in batchException.Exceptions)
						if (MessageBox.Show(exception.Message, Application.SafeTopLevelCaptionFormat, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
							break;

				}

				// This will prevent the same results from being added twice.
				Clipboard.Clear();

			}

		}

		private void contextMenuWorkingOrder_Popup(object sender, EventArgs e)
		{

			bool isPasteEnabled = false;
			if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
			{
				IDataObject iDataObject = Clipboard.GetDataObject();
				MemoryStream memoryStream = (MemoryStream)iDataObject.GetData("XML Spreadsheet");
				if (memoryStream != null)
					isPasteEnabled = true;
			}
			this.menuItemPaste.Enabled = isPasteEnabled;

		}

		public void SetAway(bool isAway)
		{

			Batch batch = new Batch();

			try
			{

				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				this.Spreadsheet.Lock.AcquireReaderLock(Timeout.Infinite);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(Timeout.Infinite);

				foreach (SpreadsheetRow spreadsheetRow in ((DataTable)this.Spreadsheet).Rows)
				{

					int workingOrderId = Convert.ToInt32(spreadsheetRow["WorkingOrderId"]);
					ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
					if (workingOrderRow != null)
					{

						// This will save the users personal preferences for the stylesheet back to the database.
						TransactionPlan transactionPlan = batch.Transactions.Add();
						AssemblyPlan assemblyPlan = batch.Assemblies.Add("Core Service");
						TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.WorkingOrder");
						MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Update");
						methodPlan.Parameters.Add(new InputParameter("workingOrderId", workingOrderRow.WorkingOrderId));
						methodPlan.Parameters.Add(new InputParameter("rowVersion", workingOrderRow.RowVersion));

						try
						{

							int submissionTypeCode = workingOrderRow.SubmissionTypeCode;
							if (isAway)
								submissionTypeCode |= SubmissionType.Away;
							else
								submissionTypeCode &= ~SubmissionType.Away;

							methodPlan.Parameters.Add(new InputParameter("submissionTypeCode", submissionTypeCode));

						}
						catch (Exception exception)
						{

							MessageBox.Show(exception.Message, "Guardian Error");

						}

					}

				}

			}
			finally
			{

				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				this.Spreadsheet.Lock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Execute the command.
			ClientMarketData.Execute(batch, this, new BatchExceptionEventHandler(ShowBatchExceptions));

		}

	}

}
