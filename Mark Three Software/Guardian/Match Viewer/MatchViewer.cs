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
	using System.Windows.Forms;
	using System.Xml;

	/// <summary>
	/// Summary description for MatchViewer.
	/// </summary>
	public class MatchViewer : MarkThree.Forms.SpreadsheetViewer
	{

		// Private Members
		private MarkThree.Forms.FragmentList fragmentList;
		private MarkThree.Guardian.Blotter blotter;
		private MarkThree.Guardian.Match[] matches;
		private MarkThree.Guardian.Client.ClientMarketData.BlotterRow blotterRow;
		private MarkThree.Guardian.Client.ClientMarketData clientMarketData;
		private System.ComponentModel.IContainer components;
		private System.Int32 stylesheetId;
		private System.Windows.Forms.MenuItem menuItemNegotiate;
		private System.Windows.Forms.MenuItem menuItemDecline;
		private System.Windows.Forms.MenuItem menuItemAccept;
		private System.Windows.Forms.ContextMenu contextMenuMatch;

		/// <summary>
		/// Constructor for the MatchViewer
		/// </summary>
		public MatchViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// This collects the changes to the data model and is used to generate incremental changes to the displayed document.
			this.fragmentList = new FragmentList();

			// This keeps track of the stylesheet associated with this viewer.
			this.stylesheetId = Int32.MinValue;

			// This allows the base classes to access the context menu created for the block order viewer.  There is likely a
			// better way to use the 'ContextMenu'.
			//this.ContextMenu = this.contextMenuMatch;

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
			this.contextMenuMatch = new System.Windows.Forms.ContextMenu();
			this.menuItemNegotiate = new System.Windows.Forms.MenuItem();
			this.menuItemDecline = new System.Windows.Forms.MenuItem();
			this.clientMarketData = new MarkThree.Guardian.Client.ClientMarketData(this.components);
			this.menuItemAccept = new System.Windows.Forms.MenuItem();
			// 
			// contextMenuMatch
			// 
			this.contextMenuMatch.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menuItemAccept,
																							 this.menuItemNegotiate,
																							 this.menuItemDecline});
			// 
			// menuItemNegotiate
			// 
			this.menuItemNegotiate.Enabled = false;
			this.menuItemNegotiate.Index = 1;
			this.menuItemNegotiate.Text = "&Negotiate";
			// 
			// menuItemDecline
			// 
			this.menuItemDecline.Index = 2;
			this.menuItemDecline.Text = "&Decline";
			this.menuItemDecline.Click += new System.EventHandler(this.menuItemDecline_Click);
			// 
			// menuItemAccept
			// 
			this.menuItemAccept.Index = 0;
			this.menuItemAccept.Text = "&Accept";
			this.menuItemAccept.Click += new System.EventHandler(this.menuItemAccept_Click);
			// 
			// MatchViewer
			// 
			this.Name = "MatchViewer";

		}
		#endregion

		/// <summary>
		/// Opens the viewer.
		/// </summary>
		/// <param name="tag">An object to be displayed in the viewer.</param>
		public override void Open(object tag)
		{

			// This viewer can only display blotters.
			if (!(tag is Blotter || tag is BlotterMatchDetail))
				throw new Exception(string.Format("{0} can't display objects of type {1}", this.GetType(), this.Tag.GetType()));

			// This is the identity of the object being viewed in this viewer.
			if (tag is Blotter)
				this.blotter = (Blotter)tag;

			// This will open the document with one or more items selected.
			if (tag is BlotterMatchDetail)
			{

				this.blotter = ((BlotterMatchDetail)tag).Blotter;
				this.matches = ((BlotterMatchDetail)tag).Matches;

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
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, equity traders
				// Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				this.blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.blotter.BlotterId);
				if (this.blotterRow == null)
					throw new Exception(String.Format("Blotter {0} does not exist", this.blotter.BlotterId));

				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = this.blotterRow.IsMatchStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(this.blotterRow.MatchStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Match stylesheet assigned", this.blotter.BlotterId));

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

				// Draw the document.
				DrawDocument();

				if (this.Tag is BlotterMatchDetail)
					this.Tag = ((BlotterMatchDetail)this.Tag).Blotter;

				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanging += new ClientMarketData.BlotterRowChangeEventHandler(this.ChangeBlotterRow);
				ClientMarketData.Match.MatchRowChanging += new ClientMarketData.MatchRowChangeEventHandler(this.ChangeMatchRow);
				ClientMarketData.Object.ObjectRowChanging += new ClientMarketData.ObjectRowChangeEventHandler(this.ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging += new ClientMarketData.OrderTypeRowChangeEventHandler(this.ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging += new ClientMarketData.PriceRowChangeEventHandler(this.ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging += new ClientMarketData.PriceTypeRowChangeEventHandler(this.ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging += new ClientMarketData.SecurityRowChangeEventHandler(this.ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging += new ClientMarketData.StatusRowChangeEventHandler(this.ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging += new ClientMarketData.StylesheetRowChangeEventHandler(this.ChangeStylesheetRow);
				ClientMarketData.Timer.TimerRowChanging += new ClientMarketData.TimerRowChangeEventHandler(this.ChangeTimerRow);
				ClientMarketData.EndMerge += new EventHandler(this.EndMerge);

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
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
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
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
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
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanging -= new ClientMarketData.BlotterRowChangeEventHandler(this.ChangeBlotterRow);
				ClientMarketData.Match.MatchRowChanging -= new ClientMarketData.MatchRowChangeEventHandler(this.ChangeMatchRow);
				ClientMarketData.Object.ObjectRowChanging -= new ClientMarketData.ObjectRowChangeEventHandler(this.ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging -= new ClientMarketData.OrderTypeRowChangeEventHandler(this.ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging -= new ClientMarketData.PriceRowChangeEventHandler(this.ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging -= new ClientMarketData.PriceTypeRowChangeEventHandler(this.ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging -= new ClientMarketData.SecurityRowChangeEventHandler(this.ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging -= new ClientMarketData.StatusRowChangeEventHandler(this.ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging -= new ClientMarketData.StylesheetRowChangeEventHandler(this.ChangeStylesheetRow);
				ClientMarketData.Timer.TimerRowChanging -= new ClientMarketData.TimerRowChangeEventHandler(this.ChangeTimerRow);
				ClientMarketData.EndMerge -= new EventHandler(this.EndMerge);

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
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
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
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
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
						foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
							fragmentList.Add(DataAction.Update, matchRow, Field.Blotter);

			}

		}
	
		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
		private void ChangeMatchRow(object sender, ClientMarketData.MatchRowChangeEvent matchRowChangeEvent)
		{

			// Several events are generated as the row is processed.  This trigger is only interested in the rows as they're being
			// comitted to the data model.
			if (matchRowChangeEvent.Action == DataRowAction.Commit)
			{

				// Extract the modified row from the event parameters.
				ClientMarketData.MatchRow matchRow = matchRowChangeEvent.Row;

				// The work is broken up into inserts, updates and deletes.  This section will create a fragment for deleted rows 
				// when they belong to this blotter.
				if (matchRow.RowState == DataRowState.Deleted)
				{

					// If this order was displayed in the current blotter then create an instruction to delete it.
					int workingOrderId = (int)matchRow[ClientMarketData.Match.WorkingOrderIdColumn, DataRowVersion.Original];
					ClientMarketData.WorkingOrderRow workingOrderRow =
						ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
					if (workingOrderRow != null)
						if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, workingOrderRow.BlotterRow.ObjectRow))
							this.fragmentList.Add(DataAction.Delete, matchRow);

				}
				else
				{

					// This next section will determine whether the row was added or modified.
					if (matchRow.HasVersion(DataRowVersion.Original))
					{

						// This array will collect the fields that have changed.
						FieldArray fields = FieldArray.Clear;

						int originalStatusCode = (int)matchRow[ClientMarketData.Match.StatusCodeColumn,
							DataRowVersion.Original];
						int currentStatusCode = (int)matchRow[ClientMarketData.Match.StatusCodeColumn,
							DataRowVersion.Current];
						if (currentStatusCode != Status.Active && originalStatusCode == Status.Active)
						{
							this.fragmentList.Add(DataAction.Delete, matchRow);
							return;
						}

						// A modified blotter identifier is very tricky to handle.  If the order has been moved to another blotter,
						// then it can still be visible, if it was moved to a child blotter, or invisible if the new location
						// doesn't appear in the current blotter's hierarchy.  Additionally, an order from another blotter may be
						// moved onto this one.  Depending on the combination, either an insert, update or delete instruction can
						// be generated.  The first step is to see if the blotter identifier has been changed.
						int originalBlotterId = (int)matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
							DataRowVersion.Original];
						int currentBlotterId = (int)matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.BlotterIdColumn,
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
									this.fragmentList.Add(DataAction.Insert, matchRow);
								else
								{
									if (isOriginalDescendant && !isCurrentDescendant)
										this.fragmentList.Add(DataAction.Delete, matchRow);
								}

								// At this point, the modified record is either not on the viewer or totally new to the viewer.  
								// There is no need to check the remaining fields.
								return;

							}

						}

						// StartTime Field
						if (!matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.StartTimeColumn, DataRowVersion.Original].Equals(
							matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.StartTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.StartTime] = true;
						}

						// StopTime Field
						if (!matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.StopTimeColumn, DataRowVersion.Original].Equals(
							matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.StopTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.StopTime] = true;
						}

						// MaximumVolatility Field
						if (!matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.MaximumVolatilityColumn, DataRowVersion.Original].Equals(
							matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.MaximumVolatilityColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.MaximumVolatility] = true;
						}

						// NewsFreeTime Field
						if (!matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.NewsFreeTimeColumn, DataRowVersion.Original].Equals(
							matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.NewsFreeTimeColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.NewsFreeTime] = true;
						}

						// Security Field
						if (!matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Original].Equals(
							matchRow.WorkingOrderRow[ClientMarketData.WorkingOrder.SecurityIdColumn, DataRowVersion.Current]))
						{
							fields[Field.Status] = true;
							fields[Field.Security] = true;
						}

						// If any of the fields need to be updated, then generate a change instruction.  This will be combined with
						// other change instructions to create fragments that will update only the requested fields in the viewer.
						if (fields)
							this.fragmentList.Add(DataAction.Update, matchRow, fields);

					}
					else
					{

						// The entire row is added if it hasn't been declined already and the order is new to this viewer.
						if (matchRow.StatusCode != Status.Declined && matchRow.StatusCode != Status.Accepted)
							if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, matchRow.WorkingOrderRow.BlotterRow.ObjectRow))
								this.fragmentList.Add(DataAction.Insert, matchRow);

					}

				}

			}

		}

		/// <summary>
		/// Event driver for a change to the Object table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeObject">The parameters for the event handler.</param>
		private void ChangeObjectRow(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

		}
	
		/// <summary>
		/// Event driver for a change to the ObjectTree table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangeObjectTree">The parameters for the event handler.</param>
		private void ChangeObjectTreeRow(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectChangeTreeRow)
		{

		}
	
		/// <summary>
		/// Event driver for a change to the transactionType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="transactionChangeType">The parameters for the event handler.</param>
		private void ChangeOrderTypeRow(object sender, ClientMarketData.OrderTypeRowChangeEvent transactionChangeTypeRow)
		{
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

                    // Note: any time the volume changes, then we compute a new InterpolatedVolume
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
							foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
								this.fragmentList.Add(DataAction.Update, matchRow, fields);

			}

		}
	
		/// <summary>
		/// Event driver for a change to the PriceType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ChangePriceType">The parameters for the event handler.</param>
		private void ChangePriceTypeRow(object sender, ClientMarketData.PriceTypeRowChangeEvent orderChangeTypeRow)
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
						foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
							fragmentList.Add(DataAction.Update, matchRow, Field.Security);

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
						foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
							fragmentList.Add(DataAction.Update, matchRow, Field.Status);

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

				foreach (ClientMarketData.MatchRow matchRow in timerRowChangeEvent.Row.GetMatchRows())
					if (Hierarchy.IsDescendant(this.blotterRow.ObjectRow, matchRow.WorkingOrderRow.BlotterRow.ObjectRow))
						fragmentList.Add(DataAction.Update, matchRow, Field.Status);

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
		/// Creates the DOM and Refreshes the spreadsheet control in the background.
		/// </summary>
		/// <remarks>
		/// This thread is responsible for recreating the Document Object Model for the blotter and translating
		/// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
		/// DOM is complete to either update the entire document, if the structure has changed, or only update
		/// the changed cells, when only the data has changed.
		/// </remarks>
		protected override void DrawDocumentCommand(object parameter)
		{

			// Extract the document from the thread parameters.
			Blotter blotter = null;
			if (parameter is Blotter)
				blotter = (Blotter)parameter;

			if (parameter is BlotterMatchDetail)
				blotter = ((BlotterMatchDetail)parameter).Blotter;

#if DEBUGXML
			// Create the blotter document.
			MatchDocument matchDocument = new MatchDocument(blotter.BlotterId);

			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				matchDocument.Save("matchDOM.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will display the
			this.XmlDocument = matchDocument;
#else
			// This will display the
			this.XmlDocument = new MatchDocument(blotter.BlotterId);
#endif

			if (parameter is BlotterMatchDetail)
			{

				BlotterMatchDetail blotterMatchDetail = (BlotterMatchDetail)parameter;

				object[] selectedItems = new object[blotterMatchDetail.Matches.Length];
				for (int itemIndex = 0; itemIndex < selectedItems.Length; itemIndex++)
					selectedItems[itemIndex] = new object[] {blotterMatchDetail.Matches[itemIndex].MatchId};

				this.SelectedItems = selectedItems;

			}

		}

		/// <summary>
		/// Creates the DOM and Refreshes the spreadsheet control in the background.
		/// </summary>
		/// <remarks>
		/// This thread is responsible for recreating the Document Object Model for the blotter and translating
		/// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
		/// DOM is complete to either update the entire document, if the structure has changed, or only update
		/// the changed cells, when only the data has changed.
		/// </remarks>
		private void DrawFragmentCommand(object parameter)
		{

			// Extract the list of fragments from the thread parameters.
			FragmentList fragmentList = (FragmentList)parameter;

#if DEBUGXML
			// This will create an XML Document containing only the records and fields that have changed since the viewer was
			// last updated.
			XmlDocument stagedOrderFragment = new MatchDocument(fragmentList);
					
			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				stagedOrderFragment.Save("stagedOrderFragment.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will write the changes to the viewer by passing it through the stylesheet.
			this.XmlDocument = stagedOrderFragment;
#else
			// This will write the changes to the viewer by passing it through the stylesheet.
			this.XmlDocument = new MatchDocument(fragmentList);
#endif
				
		}

		private void menuItemDecline_Click(object sender, System.EventArgs e)
		{
		
			Batch batch = new Batch();

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);

				TransactionPlan transactionPlan = batch.Transactions.Add();
				AssemblyPlan assemblyPlan = batch.Assemblies.Add("Trading Service");
				TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Trading.Match");

				foreach (object[] matchKey in this.SelectedItems)
				{

					ClientMarketData.MatchRow matchRow = (ClientMarketData.MatchRow)ClientMarketData.Match.Rows.Find(matchKey);
					if (matchRow != null)
					{

						// This will save the users personal preferences for the stylesheet back to the database.
						MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Decline");
						methodPlan.Parameters.Add(new InputParameter("matchId", matchRow.MatchId));
						methodPlan.Parameters.Add(new InputParameter("rowVersion", matchRow.RowVersion));

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
				if (ClientMarketData.MatchLock.IsReaderLockHeld) ClientMarketData.MatchLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Execute the command.
			ClientMarketData.Execute(batch, this, new BatchExceptionEventHandler(ShowBatchExceptions));

		}

		private void menuItemAccept_Click(object sender, System.EventArgs e)
		{
		
			Batch batch = new Batch();

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);

				TransactionPlan transactionPlan = batch.Transactions.Add();
				AssemblyPlan assemblyPlan = batch.Assemblies.Add("Trading Service");
				TypePlan typePlan = assemblyPlan.Types.Add("MarkThree.Guardian.Trading.Match");

				foreach (object[] matchKey in this.SelectedItems)
				{

					ClientMarketData.MatchRow matchRow = (ClientMarketData.MatchRow)ClientMarketData.Match.Rows.Find(matchKey);
					if (matchRow != null)
					{

						// This will save the users personal preferences for the stylesheet back to the database.
						MethodPlan methodPlan = transactionPlan.Methods.Add(typePlan, "Accept");
						methodPlan.Parameters.Add(new InputParameter("matchId", matchRow.MatchId));
						methodPlan.Parameters.Add(new InputParameter("rowVersion", matchRow.RowVersion));

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
				if (ClientMarketData.MatchLock.IsReaderLockHeld) ClientMarketData.MatchLock.ReleaseReaderLock();
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

	}

}
