namespace MarkThree.Guardian
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
	/// Summary description for DestinationOrderViewer.
	/// </summary>
	public class DestinationOrderViewer : MarkThree.Forms.SpreadsheetViewer
	{

		// Private Members
		private MarkThree.Guardian.Blotter blotter;
		private MarkThree.Guardian.WorkingOrder[] workingOrders;
		private MarkThree.Forms.FragmentList fragmentList;
		private MarkThree.Guardian.Client.ClientMarketData clientMarketData;
		private MarkThree.Guardian.Client.ClientMarketData.BlotterRow blotterRow;
		private System.ComponentModel.IContainer components;
		private System.Int32 stylesheetId;
		private System.Windows.Forms.ContextMenu contextMenuDestinationOrder;
		private System.Windows.Forms.MenuItem menuItemManualExecution;

		// Public Events
		public event System.EventHandler Execution;
		public event System.EventHandler DestinationOrder;

		/// <summary>
		/// Constructor for the DestinationOrderViewer
		/// </summary>
		public DestinationOrderViewer()
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
			this.ContextMenu = this.contextMenuDestinationOrder;

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
			this.contextMenuDestinationOrder = new System.Windows.Forms.ContextMenu();
			this.menuItemManualExecution = new System.Windows.Forms.MenuItem();
			this.clientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
			// 
			// contextMenuDestinationOrder
			// 
			this.contextMenuDestinationOrder.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									this.menuItemManualExecution});
			// 
			// menuItemManualExecution
			// 
			this.menuItemManualExecution.Index = 0;
			this.menuItemManualExecution.Text = "&Manual Execution";
			// 
			// DestinationOrderViewer
			// 
			this.Name = "DestinationOrderViewer";
			this.StylesheetChanged += new System.EventHandler(this.DestinationOrderViewer_StylesheetChanged);
			this.EndEdit += new MarkThree.Forms.EndEditEventHandler(this.DestinationOrderViewer_EndEdit);

		}
		#endregion

		/// <summary>
		/// Opens the viewer.
		/// </summary>
		/// <param name="tag">An object to be displayed in the viewer.</param>
		public override void Open(object tag)
		{

			// This viewer can only display blotters.
			if (!(tag is Blotter || tag is BlotterWorkingOrderDetail || tag is BlotterMatchDetail))
				throw new Exception(string.Format("{0} can't display objects of type {1}", this.GetType(), this.Tag.GetType()));

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
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to any of its panels it so Fixed Income traders view Fixed Income
				// data, equity traders Equity data, and so forth.  This will pick out the stylesheet used for the Destination 
				// Orders found on the current blotter.
				this.blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (this.blotterRow == null)
					throw new Exception(String.Format("Blotter {0} does not exist", this.blotter.BlotterId));
				ClientMarketData.StylesheetRow stylesheetRow = this.Tag is BlotterWorkingOrderDetail ? (blotterRow.IsDestinationOrderDetailStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.DestinationOrderDetailStylesheetId)) :
					this.Tag is Blotter ? (blotterRow.IsDestinationOrderStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.DestinationOrderStylesheetId)) : null;
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Destination Order stylesheet assigned.", this.blotter.BlotterId));

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
				ClientMarketData.Destination.DestinationRowChanging += new ClientMarketData.DestinationRowChangeEventHandler(ChangeDestinationRow);
				ClientMarketData.DestinationOrder.DestinationOrderRowChanging += new ClientMarketData.DestinationOrderRowChangeEventHandler(ChangeDestinationOrderRow);
				ClientMarketData.Execution.ExecutionRowChanging += new ClientMarketData.ExecutionRowChangeEventHandler(ChangeExecutionRow);
				ClientMarketData.Object.ObjectRowChanging += new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging += new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging += new ClientMarketData.OrderTypeRowChangeEventHandler(ChangeOrderTypeRow);
				ClientMarketData.PriceType.PriceTypeRowChanging += new ClientMarketData.PriceTypeRowChangeEventHandler(ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging += new ClientMarketData.SecurityRowChangeEventHandler(ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging += new ClientMarketData.StatusRowChangeEventHandler(ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging += new ClientMarketData.StylesheetRowChangeEventHandler(ChangeStylesheetRow);
				ClientMarketData.TimeInForce.TimeInForceRowChanging += new ClientMarketData.TimeInForceRowChangeEventHandler(ChangeTimeInForceRow);
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
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
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
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanging -= new ClientMarketData.BlotterRowChangeEventHandler(ChangeBlotterRow);
				ClientMarketData.Destination.DestinationRowChanging -= new ClientMarketData.DestinationRowChangeEventHandler(ChangeDestinationRow);
				ClientMarketData.DestinationOrder.DestinationOrderRowChanging -= new ClientMarketData.DestinationOrderRowChangeEventHandler(ChangeDestinationOrderRow);
				ClientMarketData.Execution.ExecutionRowChanging -= new ClientMarketData.ExecutionRowChangeEventHandler(ChangeExecutionRow);
				ClientMarketData.Object.ObjectRowChanging -= new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging -= new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging -= new ClientMarketData.OrderTypeRowChangeEventHandler(ChangeOrderTypeRow);
				ClientMarketData.PriceType.PriceTypeRowChanging -= new ClientMarketData.PriceTypeRowChangeEventHandler(ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging -= new ClientMarketData.SecurityRowChangeEventHandler(ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging -= new ClientMarketData.StatusRowChangeEventHandler(ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging -= new ClientMarketData.StylesheetRowChangeEventHandler(ChangeStylesheetRow);
				ClientMarketData.TimeInForce.TimeInForceRowChanging -= new ClientMarketData.TimeInForceRowChangeEventHandler(ChangeTimeInForceRow);
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
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}
				
		}

		/// <summary>
		/// Determines whether a given Destination Order belongs on this document.
		/// </summary>
		/// <param name="blotterRow"></param>
		/// <param name="workingOrderRow"></param>
		/// <returns></returns>
		private bool IsDescendant(ClientMarketData.WorkingOrderRow workingOrderRow)
		{

			if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
			{

				if (this.workingOrders == null)
					return true;

				foreach (WorkingOrder workingOrder in this.workingOrders)
					if (workingOrderRow.WorkingOrderId == workingOrder.WorkingOrderId)
						return true;

			}

			return false;

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
					if (IsDescendant(workingOrderRow))
						foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							fragmentList.Add(DataAction.Update, destinationOrderRow, Field.Blotter);

			}

		}

		/// <summary>
		/// Event handler for a change to the destinations table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void ChangeDestinationRow(object sender, ClientMarketData.DestinationRowChangeEvent destinationRowChangeEvent)
		{

			// When the order type has changed, every row in the report containing a relation to the order type item will be updated.
			if (destinationRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in destinationRowChangeEvent.Row.GetDestinationOrderRows())
					if (IsDescendant(destinationOrderRow.WorkingOrderRow))
						fragmentList.Add(DataAction.Update, destinationOrderRow, Field.Destination);

		}
		
		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeDestinationOrderRow(object sender, ClientMarketData.DestinationOrderRowChangeEvent destinationOrderRowChangeEvent)
		{

			// Several events are generated as the row is processed.  This trigger is only interested in the rows as they're being
			// comitted to the data model.
			if (destinationOrderRowChangeEvent.Action == DataRowAction.Commit)
			{

				// Extract the modified row from the event parameters.
				ClientMarketData.DestinationOrderRow destinationOrderRow = destinationOrderRowChangeEvent.Row;

				// The work is broken up into inserts, updates and deletes.  This section will create a fragment for deleted rows
				// when they belong to this blotter (or rather, should be removed from this blotter).
				if (destinationOrderRow.RowState == DataRowState.Deleted)
				{

					int workingOrderId = (int)destinationOrderRow[ClientMarketData.DestinationOrder.WorkingOrderIdColumn, DataRowVersion.Original];
					ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
					if (IsDescendant(workingOrderRow))
						this.fragmentList.Add(DataAction.Delete, destinationOrderRow);

				}
				else
				{

					// This next section will determine whether the row was added or modified.
					if (destinationOrderRow.HasVersion(DataRowVersion.Original))
					{

						// This array will collect the fields that have changed.
						FieldArray fields = FieldArray.Clear;

						// A modified blotter identifier is very tricky to handle.  If the order has been moved to another blotter,
						// then it can still be visible, if it was moved to a child blotter, or invisible if the new location
						// doesn't appear in the current blotter's hierarchy.  Additionally, an order from another blotter may be
						// moved onto this one.  Depending on the combination, either an insert, update or delete instruction can
						// be generated.  The first step is to see if the blotter identifier has been changed.
						int originalBlotterId = (int)destinationOrderRow.WorkingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
							DataRowVersion.Original];
						int currentBlotterId = (int)destinationOrderRow.WorkingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
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
									this.fragmentList.Add(DataAction.Insert, destinationOrderRow);
								else
								{
									if (isOriginalDescendant && !isCurrentDescendant)
										this.fragmentList.Add(DataAction.Delete, destinationOrderRow);
								}

								// At this point, the modified record is either not on the viewer or totally new to the viewer, but
								// there is no need to check the remaining fields.
								return;

							}

						}

						if (IsDescendant(destinationOrderRow.WorkingOrderRow))
						{

							// If the status has changed then give the template a chance to redraw all the fields.
							int originalStatusCode = (int)destinationOrderRow[ClientMarketData.DestinationOrder.StatusCodeColumn,
								DataRowVersion.Original];
							int currentStatusCode = (int)destinationOrderRow[ClientMarketData.DestinationOrder.StatusCodeColumn,
								DataRowVersion.Current];
							if (originalStatusCode != currentStatusCode)
								fields = FieldArray.Set;

							// Ordered Quantity Field
							if (!destinationOrderRow[ClientMarketData.DestinationOrder.OrderedQuantityColumn, DataRowVersion.Original].Equals(
								destinationOrderRow[ClientMarketData.DestinationOrder.OrderedQuantityColumn, DataRowVersion.Current]))
							{
								fields[Field.Status] = true;
								fields[Field.OrderedQuantity] = true;
							}

							// Canceled Quantity Field
							if (!destinationOrderRow[ClientMarketData.DestinationOrder.CanceledQuantityColumn, DataRowVersion.Original].Equals(
								destinationOrderRow[ClientMarketData.DestinationOrder.CanceledQuantityColumn, DataRowVersion.Current]))
							{
								fields[Field.Status] = true;
								fields[Field.CanceledQuantity] = true;
							}

							// OrderType Field
							if (!destinationOrderRow[ClientMarketData.DestinationOrder.OrderTypeCodeColumn, DataRowVersion.Original].Equals(
								destinationOrderRow[ClientMarketData.DestinationOrder.OrderTypeCodeColumn, DataRowVersion.Current]))
							{
								fields[Field.Status] = true;
								fields[Field.OrderType] = true;
							}

							// TimeInForce Field
							if (!destinationOrderRow[ClientMarketData.DestinationOrder.TimeInForceCodeColumn, DataRowVersion.Original].Equals(
								destinationOrderRow[ClientMarketData.DestinationOrder.TimeInForceCodeColumn, DataRowVersion.Current]))
							{
								fields[Field.Status] = true;
								fields[Field.TimeInForce] = true;
							}

							// Security Field
							if (!destinationOrderRow.WorkingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Original].Equals(
								destinationOrderRow.WorkingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Current]))
							{
								fields[Field.Status] = true;
								fields[Field.Security] = true;
							}

						}

						// If any of the fields need to be updated, then generate a change instruction.  This will be combined with
						// other change instructions to create fragments that will update only the requested fields in the viewer.
						if (fields)
							this.fragmentList.Add(DataAction.Update, destinationOrderRow, fields);

					}
					else
					{

						// The entire row is added if the order is new to this viewer.
						if (IsDescendant(destinationOrderRow.WorkingOrderRow))
							this.fragmentList.Add(DataAction.Insert, destinationOrderRow);

					}

				}

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
				if (destinationOrderRow != null && IsDescendant(destinationOrderRow.WorkingOrderRow))
					fragmentList.Add(DataAction.Update, destinationOrderRow, Field.ExecutedQuantity);

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
					if (IsDescendant(workingOrderRow))
						foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							fragmentList.Add(DataAction.Update, destinationOrderRow, Field.OrderType);

		}
	
		/// <summary>
		/// Event driver for a change to the PriceType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangePriceTypeRow">The parameters for the event handler.</param>
		private void ChangePriceTypeRow(object sender, ClientMarketData.PriceTypeRowChangeEvent orderTypeRowChangeEvent)
		{

			// When the order type has changed, every row in the report containing a relation to the order type item will be updated.
			if (orderTypeRowChangeEvent.Action == DataRowAction.Commit)
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in orderTypeRowChangeEvent.Row.GetWorkingOrderRows())
					if (IsDescendant(workingOrderRow))
						foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							fragmentList.Add(DataAction.Update, destinationOrderRow, Field.PriceType);
		}
	
		/// <summary>
		/// Event driver for a change to the Security table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeSecurityRow">The parameters for the event handler.</param>
		private void ChangeSecurityRow(object sender, ClientMarketData.SecurityRowChangeEvent securityRowChangeEvent)
		{

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
				foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in statusRowChangeEvent.Row.GetDestinationOrderRows())
					if (IsDescendant(destinationOrderRow.WorkingOrderRow))
						fragmentList.Add(DataAction.Update, destinationOrderRow, Field.Status);

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
					if (IsDescendant(workingOrderRow))
						foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							fragmentList.Add(DataAction.Update, destinationOrderRow, Field.TimeInForce);

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

			// Extract the thread arguments.
			Blotter blotter = null;
			WorkingOrder[] workingOrders = null;

			// Extract the blotter to be drawn from the thread arguments.
			if (parameter is Blotter)
			{
				blotter = (Blotter)parameter;
				workingOrders = null;
			}

			// Extract the blotter details to be drawn from the thread arguments.
			if (parameter is BlotterWorkingOrderDetail)
			{
				blotter = ((BlotterWorkingOrderDetail)parameter).Blotter;
				workingOrders = ((BlotterWorkingOrderDetail)parameter).WorkingOrders;
			}

			// Extract the blotter to be drawn from the thread arguments.
			if (parameter is BlotterMatchDetail)
			{
				blotter = ((BlotterMatchDetail)parameter).Blotter;
				workingOrders = null;
			}

#if DEBUGXML
			// Create the blotter Document Object Model.
			DestinationOrderDocument destinationOrderDocument = new DestinationOrderDocument(blotter, workingOrders);

			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				destinationOrderDocument.Save("destinationOrderDOM.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will draw the document by passing it through the stylesheet.
			this.XmlDocument = destinationOrderDocument;
#else
			// This will draw the document by passing it through the stylesheet.
			this.XmlDocument = new DestinationOrderDocument(blotter, workingOrders);
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
			XmlDocument destinationOrderFragment = new DestinationOrderDocument(fragmentList);
			
			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				destinationOrderFragment.Save("destinationOrderFragment.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will write the incremental changes out to the viewer.
			this.XmlDocument = destinationOrderFragment;
#else
			// This will write the incremental changes out to the viewer.
			this.XmlDocument = new DestinationOrderDocument(fragmentList);
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
					MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Update");
					methodPlan.Parameters.Add(new InputParameter("stylesheetId", stylesheetRow.StylesheetId));
					methodPlan.Parameters.Add(new InputParameter("rowVersion", stylesheetRow.RowVersion));
					methodPlan.Parameters.Add(new InputParameter("text", this.XslStylesheet.OuterXml));
				}

			}
			catch (Exception exception)
			{

				// This signals that the batch should not be sent
				batch = null;

				// Record the error in the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			if (ClientMarketData.Execute(batch, this, new BatchExceptionEventHandler(ShowBatchExceptions)))
				this.stylesheetId = (int)insertMethod.Parameters.Return;

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

		private void DestinationOrderViewer_EndEdit(object sender, MarkThree.Forms.EndEditEventArgs e)
		{

			Batch batch = new Batch();

			try
			{

				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(Timeout.Infinite);

				foreach (object[] key in this.SelectedItems)
				{

					int destinationOrderId = (int)key[0];

					ClientMarketData.DestinationOrderRow destinationOrderRow = ClientMarketData.DestinationOrder.FindByDestinationOrderId(destinationOrderId);
					if (destinationOrderRow != null)
					{

						// This will save the users personal preferences for the stylesheet back to the database.
						TransactionPlan transactionPlan = batch.Transactions.Add();
						AssemblyPlan assemblyPlan = batch.Assemblies.Add("Core Service");
						TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Core.DestinationOrder");
						MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Update");
						methodPlan.Parameters.Add(new InputParameter("destinationOrderId", destinationOrderRow.DestinationOrderId));
						methodPlan.Parameters.Add(new InputParameter("rowVersion", destinationOrderRow.RowVersion));

						try
						{

							if (this.ActiveColumn.ColumnName == "IsSubmitted")
							{
								bool isSubmitted = Char.ToUpper(Convert.ToString(e.Result)[0]) == 'Y';
								methodPlan.Parameters.Add(new InputParameter("isSubmitted", isSubmitted));
							}

							if (this.ActiveColumn.ColumnName == "SubmittedQuantity")
								methodPlan.Parameters.Add(new InputParameter("submittedQuantity", Convert.ToDecimal(e.Result)));

							if (this.ActiveColumn.ColumnName == "MaximumVolatility")
								methodPlan.Parameters.Add(new InputParameter("maximumVolatility", Convert.ToDecimal(e.Result)));

							if (this.ActiveColumn.ColumnName == "MinimumQuantity")
								methodPlan.Parameters.Add(new InputParameter("minimumQuantity", Convert.ToDecimal(e.Result)));

							if (this.ActiveColumn.ColumnName == "StartTime")
								methodPlan.Parameters.Add(new InputParameter("startTime", Convert.ToDateTime(e.Result)));

							if (this.ActiveColumn.ColumnName == "StopTime")
								methodPlan.Parameters.Add(new InputParameter("stopTime", Convert.ToDateTime(e.Result)));

							if (this.ActiveColumn.ColumnName == "newsFreeTime")
								methodPlan.Parameters.Add(new InputParameter("newsFreeTime", Convert.ToInt32(e.Result)));

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

				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			if (batch.Transactions.Count != 0)
				ClientMarketData.Execute(batch);

		}

		/// <summary>
		/// Handles a change to the stylesheet in this viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event arguments</param>
		private void DestinationOrderViewer_StylesheetChanged(object sender, System.EventArgs e)
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

	}

}
