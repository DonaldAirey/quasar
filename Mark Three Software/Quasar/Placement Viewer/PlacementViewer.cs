/**********************************************************************************************************************************
*
*	File:			PlacementViewer.cs
*	Description:	A Viewer for adding and maintaining placements using a ledger style of input and display.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
**********************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Placement
{

	using AxMicrosoft.Office.Interop.Owc11;
	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Common.Controls;
	using Shadows.Quasar.Viewers;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Security.Policy;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// A Viewer for Order Placement
	/// </summary>
	public class PlacementViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private int blotterId;
		private int blockOrderId;
		private int selectedRowType;
		private int selectedPlacementId;
		private XslTransform xslTransform;
		private AddressMap addressMap;
		private ReaderWriterLock addressMapLock;
		private PlacementSet placementSet;
		private DataView globalPlacementView;
		private ArrayList updatedPlacements;
		private ResourceManager resourceManager;
		private System.ComponentModel.IContainer components;
		private Shadows.Quasar.Client.ClientMarketData clientMarketData;

		/// <summary>
		/// Constructor for the PlacementViewer
		/// </summary>
		public PlacementViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// The address map is used to quickly find the location of a cell in viewer.  It is created by parsing the Excel Xml
			// used to create the reports.
			this.addressMapLock = new ReaderWriterLock();
			
			// IMPORTANT CONCEPT: This DataSet is used to hold a copy of the global data tables used to present a Placement
			// document.  This 'local' table is able to have it's own indices which can't be confused with the global identifiers.
			// This local set of identifiers is critical to building an on screen document that has both existing placements and
			// new placements that are being constructed.  For instance, if a user is entering a new placement, several events
			// could change the screen before that placement has a universally understood identifier.  A new placement could come
			// in from another user or an electronic trade.  In any event, the document needs to be able to handle the event, while
			// still allowing the user to construct a new placement.  This problem is solved with the local table which is
			// basically a copy of the global records plus any new records being constructed by the user.
			this.placementSet = new PlacementSet();

			// This DataView is used to access the elements of the local table using the globally recognized identifiers.  As
			// opposed to the locally recognized identifiers given the record for managing the on screen document.
			this.globalPlacementView = new DataView(this.placementSet.Placement, null, "PlacementId",
				DataViewRowState.CurrentRows);

			// This list is used by the event handlers.  A list of placements that have been changed during a reconcilliation is
			// placed in this structure.  The 'EndMerge' event will trigger a thread that will decided how each of the placements
			// has changed the on screen reports.
			this.updatedPlacements = new ArrayList();

			// Add the debt calculation library to the functions recognized by this spreadsheet control.
			this.ExternalLibraries.Add(new Viewers.DebtLibrary.Debt());

			// Open a resource manager to handle the localization of the viewer.
			resourceManager = new ResourceManager("Shadows.Quasar.Viewers.Placement.Resource", Assembly.GetExecutingAssembly());

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

			// It's possible the background threads may have left an 'Invoke' call to update a spreadsheet cell in the
			// system, even after they've been aborted.  Changing the document id insures that anything in the 'Invoke'
			// queue will not try to update an expired spreadsheet.
			this.IncrementDocumentVersion();

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PlacementViewer));
			this.clientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.Name = "axSpreadsheet";
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			// 
			// PlacementViewer
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MoveAfterReturn = true;
			this.Name = "PlacementViewer";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();

		}
		#endregion

		/// <summary>
		/// Thread safe access to the blotter id.
		/// </summary>
		[Browsable(false)]
		public int BlotterId
		{
			get {lock (this) return this.blotterId;}
			set {lock (this) this.blotterId = value;}
		}

		/// <summary>
		/// Thread safe access to the block order id.
		/// </summary>
		[Browsable(false)]
		public int BlockOrderId
		{
			get {lock (this) return this.blockOrderId;}
			set {lock (this) this.blockOrderId = value;}
		}

		/// <summary>
		/// Locks the global tables used by this viewer.
		/// </summary>
		private void LockGlobalPlacement()
		{

			// Lock the global tables used by the viewer.
			Debug.Assert(!ClientMarketData.AreLocksHeld);
			ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ConditionLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);

		}

		/// <summary>
		/// Releases the global table locks.
		/// </summary>
		private void ReleaseGlobalPlacement()
		{

			// Release the tables.
			if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
			if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
			if (ClientMarketData.ConditionLock.IsReaderLockHeld) ClientMarketData.ConditionLock.ReleaseReaderLock();
			if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
			if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
			if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
			if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
			if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
			Debug.Assert(!ClientMarketData.AreLocksHeld);

		}

		/// <summary>
		/// Selects a default value for a broker.
		/// </summary>
		/// <returns>The default broker id for this placement, null if the broker is not determined yet.</returns>
		private object GetDefaultBrokerId()
		{

			// This indicates that the broker for the placement can't be determined.  This should be replaced with logic 
			// that extracts directed broker information from the block order.
			return null;

		}

		/// <summary>
		/// Handles an exception to a record.
		/// </summary>
		/// <param name="recordException">Information about the offending record.</param>
		private void RecordExceptionHandler(RecordException recordException)
		{

			try
			{

				// Lock the table
				LockGlobalPlacement();

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow =
					this.placementSet.Placement.FindByLocalPlacementId(recordException.RecordId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"),
						recordException.RecordId));

				// An error will invalidate the selected record and set a flag that will prevent re-validation until something else
				// has changed in the collection of placements associated with this block.
				placementRow.IsValid = false;
				placementRow.IsError = true;

				// The idea behind clearing up a record error is to find the original record and restore the local record to the
				// original values.
				if (!placementRow.IsPlacementIdNull())
				{

					// Find the original record, construct and execute a dislay batch to return the values to their original state.
					ClientMarketData.PlacementRow globalPlacementRow =
						ClientMarketData.Placement.FindByPlacementId(placementRow.PlacementId);
					if (globalPlacementRow != null)
						ExecuteDisplayBatch(ReconcilePlacement(new ArrayList(), placementRow, globalPlacementRow));

				}

				// Force the selection back to the offending field of the offending record.  If a mapping to the offending cell
				// exists, then select it (a foreground operation).  Setting the 'isPlacementValid' to false will indicate that
				// some change has to take place before the record will attempt validation again.
				AddressMap.AddressMapRow addressMapRow =
					this.addressMap.AddressMap.FindByPlacementIdColumnType(recordException.RecordId, ColumnType.BrokerSymbol);
				if (addressMapRow != null)
					Select(new CellAddress(addressMapRow.DocumentVersion, addressMapRow.RowIndex, addressMapRow.ColumnIndex));

			}
			finally
			{

				// Release the tables.
				ReleaseGlobalPlacement();

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();
				
			}
			
		}
		
		/// <summary>
		/// Handles field errors to local records.
		/// </summary>
		/// <param name="fieldException">Exception information for a local field error.</param>
		private void FieldExceptionHandler(FieldException fieldException)
		{

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow =
					this.placementSet.Placement.FindByLocalPlacementId(fieldException.RecordId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"),
						fieldException.RecordId));

				// An error will invalidate the selected record and set a flag that will prevent re-validation until something else
				// has changed in the collection of placements associated with this block.
				placementRow.IsValid = false;
				placementRow.IsError = true;

				// Force the selection back to the offending field of the offending record.  If a mapping to the offending cell
				// exists, then select it (a foreground operation).  Setting the 'isPlacementValid' to false will indicate that
				// some change has to take place before the record will attempt validation again.
				AddressMap.AddressMapRow addressMapRow =
					this.addressMap.AddressMap.FindByPlacementIdColumnType(fieldException.RecordId, fieldException.FieldId);
				if (addressMapRow != null)
					Select(new CellAddress(addressMapRow.DocumentVersion, addressMapRow.RowIndex, addressMapRow.ColumnIndex));

				// Display the error on screen.
				MessageBox.Show(this, resourceManager.GetString(fieldException.StringId),
					resourceManager.GetString("quasarError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
			finally
			{

				// Release the tables.
				ReleaseGlobalPlacement();

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();
				
			}
			
		}

		/// <summary>
		/// Prepares the document for data merged from the server.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments</param>
		private void BeginMerge(object sender, EventArgs e)
		{

			// This flag will trigger a refresh of the entire document, obviating any individual cell update.  It is set while
			// handling the merge of data when the screen update logic determines that the new data has changed the format of the
			// document.  For instance, a new stylesheet would require the entire document to be redrawn, whereas a new quantity
			// can be handled with an individual cell update.
			this.IsRefreshNeeded = false;

			// This ArrayList is used to hold the placement records that are updated by external events.  During the 'EndMerge'
			// event, this list will drive updates into the on screen document.
			this.updatedPlacements = new ArrayList();

		}

		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The arguments for the event handler.</param>
		private void BlotterRowChanged(object sender, ClientMarketData.BlotterRowChangeEvent blotterRowChangeEvent)
		{

			// This will force a refresh of the entire document if the current blotter has changed.
			if (blotterRowChangeEvent.Row.BlotterId == this.BlotterId)
				this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the broker table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="brokerRowChangeEvent">The arguments for the event handler.</param>
		private void BrokerRowChanged(object sender, ClientMarketData.BrokerRowChangeEvent brokerRowChangeEvent)
		{

			// This will force a refresh of the entire document.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the placement table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="placementRowChangeEvent">The arguments for the event handler.</param>
		private void PlacementRowChanged(object sender, ClientMarketData.PlacementRowChangeEvent placementRowChangeEvent)
		{

			// These conditions are intended to filter out the records that are of no interest to the on screen document.  Note
			// that all actions but the commit are ignored.  Only when the records are committed to the tables is this event
			// handler intersted in acting on them.  This is mainly to deal with the steps that are taken when a record is deleted.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && placementRowChangeEvent.Action != DataRowAction.Commit)
			{

				// Shorthand notation for the changed row.
				ClientMarketData.PlacementRow placementRow = placementRowChangeEvent.Row;

				// Both active and deleted records are handled by this event handler.  The deleted records can't be accessed the
				// same way as current records: the DataRowVersion field needs to be used to access the original record when trying
				// to find the fields of a deleted record.
				DataRowVersion dataRowVersion = placementRow.RowState == DataRowState.Deleted ? DataRowVersion.Original :
					DataRowVersion.Current;

				// If the placement belongs to the currently displayed block order, the put it in a list of placements that will be
				// used to update the screen when the data merge is complete.
				int blockOrderId = (int)placementRow[ClientMarketData.Placement.BlockOrderIdColumn, dataRowVersion];
				if (this.BlockOrderId == blockOrderId)
					this.updatedPlacements.Add((int)placementRow[ClientMarketData.Placement.PlacementIdColumn, dataRowVersion]);

			}

		}
	
		/// <summary>
		/// Event driver for a change to the stylesheet table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="stylesheetRowChangeEvent">The arguments for the event handler.</param>
		private void StylesheetRowChanged(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// This will make it easier to operate on the changed record.
			ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

			// Reopen the document if the style sheet has changed.  Note that the stylesheet is associated with the blotter, so the
			// document has to be opened first, to re-install the stylesheet, then the block order can be displayed with the new
			// template.
			if (this.IsDocumentOpen && stylesheetRow.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.BlotterId);
				OpenBlockOrder(this.BlockOrderId);
			}

		}

		/// <summary>
		/// This event handler is called after the internal data has been reconcilled with the server.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// If the document is open, then start a command that will update the placements on the screen with any records that
			// were modified during the reconcilliation.
			if (this.IsDocumentOpen && this.updatedPlacements.Count != 0)
				QueueCommand(new ThreadHandler(PlacementUpdated), this.updatedPlacements);

		}

		/// <summary>
		/// Updates the placements on the screen after a reconcilliation.
		/// </summary>
		/// <param name="argument">Thread parameters</param>
		private void PlacementUpdated(params object[] argument)
		{

			// Extract the list of updated placements from the argument list.
			ArrayList updatedPlacements = (ArrayList)argument[0];

			// This will capture individual updates to the display document.  When all the placement records have been analyzed to
			// see if there are any changes, a list of commands to update the user interface will be stored in this structure.
			ArrayList displayBatch = new ArrayList();

			try
			{

				// Lock the tables
				LockGlobalPlacement();

				// Lock the Address map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Run through all the placements that were modified in the last reconcilliation with the database.
				foreach (int placementId in updatedPlacements)
				{

					// Find the placement record in the global tables based on the identifier.  This record will be compared 
					// against the local record to see what has changed.  The changes will generate one or more commands to update
					// the on screen document.
					ClientMarketData.PlacementRow globalPlacementRow = ClientMarketData.Placement.FindByPlacementId(placementId);

					// This index is sued to find the placement in the local table -- the one used to drive the on screen report 
					// -- based on the global identifier.  Note that the global identifier is not the primary identifier for this
					// DataTable, so the additional DataView is created to help index into the local records quickly.
					int index = this.globalPlacementView.Find(placementId);

					// If the record has been deleted in the global records, but still exists in the local records, then purge it.
					if (globalPlacementRow == null)
					{

						if (index != -1)
						{

							// Find the row in the local table and delete it.
							PlacementSet.PlacementRow localPlacementRow =
								(PlacementSet.PlacementRow)this.globalPlacementView[index].Row;
							localPlacementRow.Delete();
							localPlacementRow.AcceptChanges();

							// This will force a complete refresh of the document.
							this.IsRefreshNeeded = true;

						}

					}
					else
					{

						// If the global record doesn't exist in the local table, the make a copy of it and refresh the on screen
						// document.
						if (index == -1)
						{

							// Import the record
							ImportPlacement(globalPlacementRow);

							// This will force a complete refresh of the document.
							this.IsRefreshNeeded = true;

						}
						else
						{

							// At this point, a global record has been modified and the local version is outdated.  This section
							// will reconcile the global record with the local one and generate the screen commands to change the
							// viewer.  The local version of the placement record can be found using the global identifier view.
							PlacementSet.PlacementRow localPlacementRow =
								(PlacementSet.PlacementRow)this.globalPlacementView[index].Row;

							// This will generate the display commands needed to reconcile the on screen placement with the new 
							// global record.
							ReconcilePlacement(displayBatch, localPlacementRow, globalPlacementRow);

						}

					}

				}

			}
			finally
			{

				// Release the table locks
				ReleaseGlobalPlacement();

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();
				
			}

			// If new data or a new structure is available after the data model has changed, then will initiate a refresh of 
			// the account viewer contents.  Otherwise, send the batch of commands to incrementally update the display.
			if (this.IsRefreshNeeded)
				DrawDocumentCommand();
			else
			{
				if (displayBatch.Count != 0)
					this.ExecuteDisplayBatch(displayBatch);
			}

		}

		/// <summary>
		/// Creates a local placement record from the global record.
		/// </summary>
		/// <param name="globalPlacementRow">A global placement record.</param>
		/// <returns>A local placement record.</returns>
		private PlacementSet.PlacementRow ImportPlacement(ClientMarketData.PlacementRow globalPlacementRow)
		{

			// Create a local record from the global one.  The local record can be writen to and hold state information that can't
			// be stored in the global records.
			PlacementSet.PlacementRow localPlacementRow = this.placementSet.Placement.NewPlacementRow();
			foreach (DataColumn destinationDataColumn in this.placementSet.Placement.Columns)
			{
				DataColumn sourceDataColumn = ClientMarketData.Placement.Columns[destinationDataColumn.ColumnName];
				if (sourceDataColumn != null)
					localPlacementRow[destinationDataColumn] = globalPlacementRow[sourceDataColumn];
			}

			// Imported records are valid (since they got their data from a valid record) and they are not empty.  These state 
			// variables help when inputting and validating user entered fields.
			localPlacementRow.IsValid = true;
			localPlacementRow.IsEmpty = false;

			// Add the imported placement into the local table.  This will give the viewer a version of the record that can be
			// modified by the user.  If the changes need to be rolled back, the viewer can use the original version of this record
			// in the global tables.
			this.placementSet.Placement.AddPlacementRow(localPlacementRow);
			localPlacementRow.AcceptChanges();

			// Return the imported version of the placement record.
			return localPlacementRow;

		}
		
		/// <summary>
		/// Reconcile the local record with the global placement record.
		/// </summary>
		/// <param name="displayBatch">A batch containing the commands to update the on screen document.</param>
		/// <param name="localPlacementRow">The local placement record.</param>
		/// <param name="globalPlacementRow">The global placement record.</param>
		/// <returns>The commands that will update the on screen document to reflect the changes in the placement record.</returns>
		private ArrayList ReconcilePlacement(ArrayList displayBatch, PlacementSet.PlacementRow localPlacementRow,
			ClientMarketData.PlacementRow globalPlacementRow)
		{

			// Reconcile the Broker Identifier
			if (localPlacementRow.BrokerId != globalPlacementRow.BrokerId)
			{
				localPlacementRow.BrokerId = globalPlacementRow.BrokerId;
				UpdateBrokerFields(displayBatch, localPlacementRow);
			}

			// Reconcile the TimeInForceCode
			if (localPlacementRow.TimeInForceCode != globalPlacementRow.TimeInForceCode)
			{
				localPlacementRow.TimeInForceCode = globalPlacementRow.TimeInForceCode;
				UpdateTimeInForceFields(displayBatch, localPlacementRow);
			}

			// Reconcile the OrderTypeCode
			if (localPlacementRow.OrderTypeCode != globalPlacementRow.OrderTypeCode)
			{
				localPlacementRow.OrderTypeCode = globalPlacementRow.OrderTypeCode;
				UpdateOrderTypeFields(displayBatch, localPlacementRow);
			}

			// Reconcile the RowVersion
			if (localPlacementRow.IsRowVersionNull() || localPlacementRow.RowVersion != globalPlacementRow.RowVersion)
			{
				localPlacementRow.RowVersion = globalPlacementRow.RowVersion;
				UpdateRowVersionField(displayBatch, localPlacementRow);
			}

			// Reconcile the Quantity
			if (localPlacementRow.IsQuantityNull() || localPlacementRow.Quantity != globalPlacementRow.Quantity)
			{
				localPlacementRow.Quantity = globalPlacementRow.Quantity;
				UpdateQuantityField(displayBatch, localPlacementRow);
			}

			// Reconcile the Price1 Field.
			if ((localPlacementRow.IsPrice1Null() && !globalPlacementRow.IsPrice1Null()) ||
				(!localPlacementRow.IsPrice1Null() && globalPlacementRow.IsPrice1Null()) ||
				(!localPlacementRow.IsPrice1Null() && !globalPlacementRow.IsPrice1Null() &&
				localPlacementRow.Price1 != globalPlacementRow.Price1))
			{
				if (globalPlacementRow.IsPrice1Null())
					localPlacementRow.SetPrice1Null();
				else
					localPlacementRow.Price1 = globalPlacementRow.Price1;
				UpdatePrice1Field(displayBatch, localPlacementRow);
			}

			// Reconcile the Price2 Field.
			if ((localPlacementRow.IsPrice2Null() && !globalPlacementRow.IsPrice2Null()) ||
				(!localPlacementRow.IsPrice2Null() && globalPlacementRow.IsPrice2Null()) ||
				(!localPlacementRow.IsPrice2Null() && !globalPlacementRow.IsPrice2Null() &&
				localPlacementRow.Price2 != globalPlacementRow.Price2))
			{
				if (globalPlacementRow.IsPrice2Null())
					localPlacementRow.SetPrice2Null();
				else
					localPlacementRow.Price2 = globalPlacementRow.Price2;
				UpdatePrice2Field(displayBatch, localPlacementRow);
			}

			// Reconcile the CreatedTime Field.
			if (localPlacementRow.IsCreatedTimeNull() || localPlacementRow.CreatedTime != globalPlacementRow.CreatedTime)
			{
				localPlacementRow.CreatedTime = globalPlacementRow.CreatedTime;
				UpdateCreatedTimeField(displayBatch, localPlacementRow);
			}

			// Reconcile the CreatedLoginId Fields.
			if (localPlacementRow.IsCreatedLoginIdNull() || localPlacementRow.CreatedLoginId != globalPlacementRow.CreatedLoginId)
			{
				localPlacementRow.CreatedLoginId = globalPlacementRow.CreatedLoginId;
				UpdateCreatedLoginFields(displayBatch, localPlacementRow);
			}

			// Reconcile the ModifiedTime Field.
			if (localPlacementRow.IsModifiedTimeNull() || localPlacementRow.ModifiedTime != globalPlacementRow.ModifiedTime)
			{
				localPlacementRow.ModifiedTime = globalPlacementRow.ModifiedTime;
				UpdateModifiedTimeField(displayBatch, localPlacementRow);
			}

			// Reconcile the ModifiedLoginId Fields.
			if (localPlacementRow.IsModifiedLoginIdNull() ||
				localPlacementRow.ModifiedLoginId != globalPlacementRow.ModifiedLoginId)
			{
				localPlacementRow.ModifiedLoginId = globalPlacementRow.ModifiedLoginId;
				UpdateModifiedLoginFields(displayBatch, localPlacementRow);
			}

			// Accept the changes to the local record.
			localPlacementRow.AcceptChanges();

			// Return the display batch generated from comparing the global record to the local one.
			return displayBatch;

		}

		/// <summary>
		/// Updates the broker fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateBrokerFields(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Find the broker record associated with this placement.  This contains the data that will populate the on screen
			// fields.
			ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(placementRow.BrokerId);

			// Broker Id
			AddressMapSet.AddressMapRow brokerIdAddress =
				this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.BrokerId);
			if (brokerIdAddress != null)
			{
				CellAddress cellAddress = new CellAddress(brokerIdAddress.DocumentVersion, brokerIdAddress.RowIndex,
					brokerIdAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.BrokerId));
			}
						
			// Broker Symbol
			AddressMapSet.AddressMapRow brokerSymbolAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.BrokerSymbol);
			if (brokerRow != null && brokerSymbolAddress != null)
			{
				CellAddress cellAddress = new CellAddress(brokerSymbolAddress.DocumentVersion, brokerSymbolAddress.RowIndex, brokerSymbolAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, brokerRow.Symbol));
			}

			// Broker Name
			AddressMapSet.AddressMapRow brokerNameAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.BrokerName);
			if (brokerRow != null && brokerNameAddress != null)
			{
				CellAddress cellAddress = new CellAddress(brokerNameAddress.DocumentVersion, brokerNameAddress.RowIndex, brokerNameAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, brokerRow.ObjectRow.Name));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the time-in-force fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateTimeInForceFields(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Find the time in force record associated with this placement.  This contains the data that will populate the on screen
			// fields.
			ClientMarketData.TimeInForceRow timeInForceRow = ClientMarketData.TimeInForce.FindByTimeInForceCode(placementRow.TimeInForceCode);

			// Time In Force Code
			AddressMapSet.AddressMapRow timeInForceCodeAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.TimeInForceCode);
			if (timeInForceCodeAddress != null)
			{
				CellAddress cellAddress = new CellAddress(timeInForceCodeAddress.DocumentVersion, timeInForceCodeAddress.RowIndex, timeInForceCodeAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.TimeInForceCode));
			}
						
			// Time In Force Mnemonic
			AddressMapSet.AddressMapRow timeInForceMnemonicAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.TimeInForceMnemonic);
			if (timeInForceMnemonicAddress != null)
			{
				CellAddress cellAddress = new CellAddress(timeInForceMnemonicAddress.DocumentVersion, timeInForceMnemonicAddress.RowIndex, timeInForceMnemonicAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, timeInForceRow.Mnemonic));
			}
						
			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the order type fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateOrderTypeFields(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Find the order type record associated with this placement.  This contains the data that will populate the on screen
			// fields.
			ClientMarketData.OrderTypeRow orderTypeRow = ClientMarketData.OrderType.FindByOrderTypeCode(placementRow.OrderTypeCode);

			// Order Type Code
			AddressMapSet.AddressMapRow orderTypeCodeAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.OrderTypeCode);
			if (orderTypeCodeAddress != null)
			{
				CellAddress cellAddress = new CellAddress(orderTypeCodeAddress.DocumentVersion, orderTypeCodeAddress.RowIndex, orderTypeCodeAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.OrderTypeCode));
			}
						
			// Order Type Mnemonic
			AddressMapSet.AddressMapRow orderTypeMnemonicAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.OrderTypeMnemonic);
			if (orderTypeMnemonicAddress != null)
			{
				CellAddress cellAddress = new CellAddress(orderTypeMnemonicAddress.DocumentVersion, orderTypeMnemonicAddress.RowIndex, orderTypeMnemonicAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, orderTypeRow.Mnemonic));
			}
						
			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the RowVersion fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateRowVersionField(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// RowVersion
			AddressMapSet.AddressMapRow rowVersionAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.RowVersion);
			if (rowVersionAddress != null)
			{
				CellAddress cellAddress = new CellAddress(rowVersionAddress.DocumentVersion, rowVersionAddress.RowIndex, rowVersionAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.RowVersion));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the quantity field.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateQuantityField(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Quantity
			AddressMapSet.AddressMapRow quantityAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.Quantity);
			if (quantityAddress != null)
			{
				CellAddress cellAddress = new CellAddress(quantityAddress.DocumentVersion, quantityAddress.RowIndex, quantityAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.Quantity));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the Price1 field.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdatePrice1Field(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Price1
			AddressMapSet.AddressMapRow price1Address = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.Price1);
			if (price1Address != null)
			{
				CellAddress cellAddress = new CellAddress(price1Address.DocumentVersion, price1Address.RowIndex, price1Address.ColumnIndex);
				object price1 = placementRow.IsPrice1Null() ? (object)null : placementRow.Price1;
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, price1));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the Price2 fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdatePrice2Field(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Price2
			AddressMapSet.AddressMapRow price2Address = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.Price2);
			if (price2Address != null)
			{
				CellAddress cellAddress = new CellAddress(price2Address.DocumentVersion, price2Address.RowIndex, price2Address.ColumnIndex);
				object price2 = placementRow.IsPrice2Null() ? (object)null : placementRow.Price2;
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, price2));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the CreatedTime field.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateCreatedTimeField(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// CreatedTime
			AddressMapSet.AddressMapRow createdTimeAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.CreatedTime);
			if (createdTimeAddress != null)
			{
				CellAddress cellAddress = new CellAddress(createdTimeAddress.DocumentVersion, createdTimeAddress.RowIndex, createdTimeAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.CreatedTime));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the CreatedLoginId fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateCreatedLoginFields(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Find the Created Login record associated with this placement.  This contains the data that will populate the on screen
			// fields.
			ClientMarketData.LoginRow loginRow = ClientMarketData.Login.FindByLoginId(placementRow.CreatedLoginId);

			// CreatedLoginId
			AddressMapSet.AddressMapRow createdLoginIdAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.CreatedLoginId);
			if (createdLoginIdAddress != null)
			{
				CellAddress cellAddress = new CellAddress(createdLoginIdAddress.DocumentVersion, createdLoginIdAddress.RowIndex, createdLoginIdAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.CreatedLoginId));
			}

			// CreatedLoginName
			AddressMapSet.AddressMapRow createdLoginNameAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.CreatedLoginName);
			if (loginRow != null && createdLoginNameAddress != null)
			{
				CellAddress cellAddress = new CellAddress(createdLoginNameAddress.DocumentVersion, createdLoginNameAddress.RowIndex, createdLoginNameAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, loginRow.ObjectRow.Name));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the ModifiedTime field.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateModifiedTimeField(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// ModifiedTime
			AddressMapSet.AddressMapRow modifiedTimeAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.ModifiedTime);
			if (modifiedTimeAddress != null)
			{
				CellAddress cellAddress = new CellAddress(modifiedTimeAddress.DocumentVersion, modifiedTimeAddress.RowIndex, modifiedTimeAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.ModifiedTime));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Updates the time-in-force fields.
		/// </summary>
		/// <param name="displayBatch">A batch of display commands.</param>
		/// <param name="placementRow">A local placement record.</param>
		/// <returns>A Batch of commands that will update the screen to reflect the broker records.</returns>
		private ArrayList UpdateModifiedLoginFields(ArrayList displayBatch, PlacementSet.PlacementRow placementRow)
		{

			// Find the Modified Login record associated with this placement.  This contains the data that will populate the on screen
			// fields.
			ClientMarketData.LoginRow loginRow = ClientMarketData.Login.FindByLoginId(placementRow.ModifiedLoginId);

			// ModifiedLoginId
			AddressMapSet.AddressMapRow modifiedLoginIdAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.ModifiedLoginId);
			if (modifiedLoginIdAddress != null)
			{
				CellAddress cellAddress = new CellAddress(modifiedLoginIdAddress.DocumentVersion, modifiedLoginIdAddress.RowIndex, modifiedLoginIdAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, placementRow.ModifiedLoginId));
			}

			// ModifiedLoginName
			AddressMapSet.AddressMapRow modifiedLoginNameAddress = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId, ColumnType.ModifiedLoginName);
			if (loginRow != null && modifiedLoginNameAddress != null)
			{
				CellAddress cellAddress = new CellAddress(modifiedLoginNameAddress.DocumentVersion, modifiedLoginNameAddress.RowIndex, modifiedLoginNameAddress.ColumnIndex);
				displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, loginRow.ObjectRow.Name));
			}

			// This batch can be used to update the on screen document.
			return displayBatch;

		}

		/// <summary>
		/// Opens the Placement Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Placement Viewer in the background.
			QueueCommand(new ThreadHandler(OpenViewerCommand));

		}

		/// <summary>
		/// Closes the Placement Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Placement Viewer in the background.
			QueueCommand(new ThreadHandler(CloseViewerCommand));

		}

		/// <summary>
		/// Opens the Placement Document.
		/// </summary>
		/// <param name="blotterId">The primary identifier of the object to open.</param>
		public override void OpenDocument(int blotterId)
		{

			// Open the Placement Document in the background.
			QueueCommand(new ThreadHandler(OpenDocumentCommand), blotterId);

		}

		/// <summary>
		/// Closes the Placement Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Close the Placement Document in the background.
			QueueCommand(new ThreadHandler(CloseDocumentCommand));

		}

		/// <summary>
		/// Draws the document.
		/// </summary>
		public override void DrawDocument()
		{

			// Launch a thread to draw the document.
			QueueCommand(new ThreadHandler(DrawDocumentCommand));

		}

		/// <summary>
		/// Validates a placement record and sends it to the server.
		/// </summary>
		/// <param name="placementRow">The record that is to be validated against the business rules.</param>
		private void ValidatePlacement(PlacementSet.PlacementRow placementRow)
		{

			try
			{

				// Rule #1: Quantity must be provided.
				if (placementRow.IsQuantityNull())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Quantity, "invalidQuantity");

				// Rule #2: Quantity must be greater than zero.
				if (!placementRow.IsQuantityNull() && placementRow.Quantity < 0.0M)
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Quantity, "negativeQuantity");

				// Rule #3: Placement must have a valid broker
				if (placementRow.IsBrokerIdNull())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.BrokerSymbol, "invalidBrokerSymbol");

				// Rule #4: Placement must have a Time In Force Instruction
				if (placementRow.IsTimeInForceCodeNull())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.TimeInForceMnemonic, "invalidTimeInForceMnemonic");

				// Rule #5: Placement must have a valid Order Type Instruction
				if (placementRow.IsOrderTypeCodeNull())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.OrderTypeMnemonic, "invalidOrderTypeMnemonic");

				// Rule #6: A market price instruction can't have a limit or stop loss price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.Market && !placementRow.IsPrice1Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price1, "invalidMarketPrice1");

				// Rule #7: A market price instruction can't specify a stop limit price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.Market && !placementRow.IsPrice2Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price2, "invalidMarketPrice2");

				// Rule #8: A limit price instruction must specify a limit price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.Limit && placementRow.IsPrice1Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price1, "missingLimitPrice1");

				// Rule #9: A limit price instruction must not specify a stop limit price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.Limit && !placementRow.IsPrice2Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price2, "invalidLimitPrice2");

				// Rule #10: A stop loss instruction must specify a limit price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.StopLoss && placementRow.IsPrice1Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price1, "missingStopLossPrice1");

				// Rule #11: A stop loss instruction must not specify a stop limit price.
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.StopLoss && !placementRow.IsPrice2Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price2, "invalidStopLossPrice2");

				// Rule #12: A stop limit must have a stop price
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.StopLimit && placementRow.IsPrice1Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price1, "missingStopLimitPrice1");

				// Rule #13: A stop limit must have a limit price
				if (!placementRow.IsOrderTypeCodeNull() && placementRow.OrderTypeCode == OrderType.StopLimit && placementRow.IsPrice2Null())
					throw new FieldException(placementRow.LocalPlacementId, ColumnType.Price2, "missingStopLimitPrice2");

				// Commit the record to the persistent store.  If the record doesn't have a globally recognized identifier, then 
				// it will be inserted. If the record has a globally recognized identifier, then this operation translates to an
				// update on that record.
				if (placementRow.IsPlacementIdNull())
					InsertLocalPlacement(placementRow);
				else
					UpdateGlobalPlacement(placementRow);

				// At this point, the record is valid.
				placementRow.IsValid = true;

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, resourceManager.GetString("quasarError"),
						MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

				// Call a common method to handle all the exceptions when inserting or updating a record.
				RecordExceptionHandler(new RecordException(placementRow.LocalPlacementId, string.Empty));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				FieldExceptionHandler(fieldException);

			}
			catch (Exception exception)
			{
				
				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Opens the viewer.
		/// </summary>
		public void OpenViewerCommand(params object[] argument)
		{

			try
			{
			
				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.BeginMerge += new EventHandler(this.BeginMerge);
				ClientMarketData.Blotter.BlotterRowChanged += new ClientMarketData.BlotterRowChangeEventHandler(BlotterRowChanged);
				ClientMarketData.Blotter.BlotterRowDeleted += new ClientMarketData.BlotterRowChangeEventHandler(BlotterRowChanged);
				ClientMarketData.Broker.BrokerRowChanged += new ClientMarketData.BrokerRowChangeEventHandler(BrokerRowChanged);
				ClientMarketData.Broker.BrokerRowDeleted += new ClientMarketData.BrokerRowChangeEventHandler(BrokerRowChanged);
				ClientMarketData.Placement.PlacementRowChanged += new ClientMarketData.PlacementRowChangeEventHandler(PlacementRowChanged);
				ClientMarketData.Placement.PlacementRowDeleted += new ClientMarketData.PlacementRowChangeEventHandler(PlacementRowChanged);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(StylesheetRowChanged);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(StylesheetRowChanged);
				ClientMarketData.EndMerge += new EventHandler(this.EndMerge);

			}
			finally
			{

				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Closes the Placement Viewer
		/// </summary>
		public void CloseViewerCommand(params object[] argument)
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.BeginMerge -= new EventHandler(this.BeginMerge);
				ClientMarketData.Blotter.BlotterRowChanged -= new ClientMarketData.BlotterRowChangeEventHandler(BlotterRowChanged);
				ClientMarketData.Blotter.BlotterRowDeleted -= new ClientMarketData.BlotterRowChangeEventHandler(BlotterRowChanged);
				ClientMarketData.Broker.BrokerRowChanged -= new ClientMarketData.BrokerRowChangeEventHandler(BrokerRowChanged);
				ClientMarketData.Broker.BrokerRowDeleted -= new ClientMarketData.BrokerRowChangeEventHandler(BrokerRowChanged);
				ClientMarketData.Placement.PlacementRowChanged -= new ClientMarketData.PlacementRowChangeEventHandler(PlacementRowChanged);
				ClientMarketData.Placement.PlacementRowDeleted -= new ClientMarketData.PlacementRowChangeEventHandler(PlacementRowChanged);
				ClientMarketData.Stylesheet.StylesheetRowChanged -= new ClientMarketData.StylesheetRowChangeEventHandler(StylesheetRowChanged);
				ClientMarketData.Stylesheet.StylesheetRowDeleted -= new ClientMarketData.StylesheetRowChangeEventHandler(StylesheetRowChanged);
				ClientMarketData.EndMerge -= new EventHandler(this.EndMerge);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the viewer is now closed.
			OnEndCloseViewer();

		}

		/// <summary>
		/// Opens the Placement Document
		/// </summary>
		private void OpenDocumentCommand(params object[] argument)
		{

			// The key to this command is a blotter identifier.
			this.BlotterId = (int)argument[0];

			// The viewer is populated with an Excel XML Document.  This document is constructed by first
			// creating a Xml DOM, then translating that DOM through a stylesheet to create the Excel XML
			// that will populate the spreadsheet control.
			XmlDocument spreadsheetDocument = new XmlDocument();
			
			try
			{

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Lock the Address Map
				this.addressMapLock.AcquireWriterLock(CommonTimeout.LockWait);
			
				// Find the blotter used to define the stylesheets used by this placement document.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(this.BlotterId);
				if (blotterRow == null)
					throw new Exception(String.Format(resourceManager.GetString("blotterNotFound"), blotterId));
				
				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsPlacementStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.PlacementStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format(resourceManager.GetString("missingPlacementStylesheet"), blotterId));

				// As an optimization, don't reload the stylesheet if the prevous document used the same stylesheet. This
				// will save a few hundred milliseconds when scrolling through similar documents.
				if (this.IsStylesheetChanged || this.StylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case it is changed while we're viewing it.  The event handler 
					// will use this id to determine if an incoming stylesheet will trigger a refresh of the document.
					this.StylesheetId = stylesheetRow.StylesheetId;

					// Load the stylesheet from the BLOB found in the in-memory stylesheet table.
					this.xslTransform = new XslTransform();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					this.xslTransform.Load(xmlTextReader, (XmlResolver)null, (Evidence)null);

				}

				// Create an empty placement document.
				PlacementDocument placementDocument = new PlacementDocument();

#if DEBUG
				// Dump the DOM to a file.  This is very useful when debugging.
				placementDocument.Save("placementDOM.xml");
#endif

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed into
				// the spreadsheet control directly, when there is a major format change, or will be updated incrementally
				// when only the content has changed.
				spreadsheetDocument.Load(xslTransform.Transform(placementDocument, null, (XmlResolver)null));

#if DEBUG
				// Write out the spreadsheet document when debugging.
				spreadsheetDocument.Save("PlacementSpreadsheet.xml");
#endif

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				this.addressMap = new AddressMap(spreadsheetDocument, this.IncrementDocumentVersion());

			}
			finally
			{
					
				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the address map
				if (this.addressMapLock.IsWriterLockHeld) this.addressMapLock.ReleaseWriterLock();
				
			}

			// The document has been constructed and the table locks have been released.  It's now safe to invoke
			// the foreground method to place the Xml Document in the spreadsheet control.  Note that there is a deadlock
			// risk if the foreground is called while locks are in place.
			SetDocument(spreadsheetDocument);

			// Broadcast the fact that the document is now open.
			OnEndOpenDocument();

		}

		/// <summary>
		/// Close the Placement Document.
		/// </summary>
		private void CloseDocumentCommand(params object[] argument)
		{

			// Close the current block order.
			CloseBlockOrderCommand(this.BlockOrderId);

			// Call the base class to close the document.
			OnEndCloseDocument();

		}
		
		/// <summary>
		/// Creates and displays a placement report.
		/// </summary>
		private void DrawDocumentCommand(params object[] argument)
		{

			// The viewer is populated with an Excel XML Document.  This document is constructed by first
			// creating a Xml DOM, then translating that DOM through a stylesheet to create the Excel XML
			// that will populate the spreadsheet control.
			XmlDocument spreadsheetDocument = new XmlDocument();

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Lock the Address Map
				this.addressMapLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Create the placement document.
				PlacementDocument placementDocument = new PlacementDocument(this.BlockOrderId, this.placementSet);

#if DEBUG
				// Dump the DOM to a file.  This is very useful when debugging.
				placementDocument.Save("placementDOM.xml");
#endif

				// Create an XML document in the Microsoft XML Spreadsheet format.
				spreadsheetDocument.Load(xslTransform.Transform(placementDocument, (XsltArgumentList)null, (XmlResolver)null));

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				this.addressMap = new AddressMap(spreadsheetDocument, this.IncrementDocumentVersion());

#if DEBUG
				// Write out the spreadsheet document when debugging.
				spreadsheetDocument.Save("PlacementSpreadsheet.xml");
#endif

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{
					
				// Release the locks obtained to produce the placement report.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the address map
				if (this.addressMapLock.IsWriterLockHeld) this.addressMapLock.ReleaseWriterLock();

			}

			// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet
			// control.  We can invoke the foreground thread to move the data into the control.  Note that we can't call
			// the foreground (e.g. anything that involves an 'Invoke' command) while we have tables locked.  This would
			// result in a deadlock situation.
			SetDocument(spreadsheetDocument);

		}

		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="argument">Optional arguments for starting the thread.</param>
		public void OpenBlockOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int blockOrderId = (int)argument[0];

			// This will force an empty placement and placement document to appear until we get some data.  It's the same
			// as forcing a drawing of the headers only.
			this.BlockOrderId = blockOrderId;

			try
			{

				// Lock the tables.
				LockGlobalPlacement();

				// Import the placements from the global tables into the local table.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow != null)
					foreach (ClientMarketData.PlacementRow placementRow in blockOrderRow.GetPlacementRows())
						ImportPlacement(placementRow);
				
			}
			finally
			{

				// Release the tables
				ReleaseGlobalPlacement();

			}

			// This will reset the viewer to the home position when it opens.
			this.ResetViewer = true;

			// Draw the document.
			DrawDocumentCommand();

		}

		/// <summary>
		/// Closes the document for the given object and it's argument.
		/// </summary>
		/// <param name="argument">Optional arguments for starting the thread.</param>
		public void CloseBlockOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int blockOrderId = (int)argument[0];

			try
			{

				// Lock the tables.
				LockGlobalPlacement();

				// Clear out the data in the table used to construct this document.
				ArrayList arrayList = new ArrayList();
				foreach (PlacementSet.PlacementRow placementRow in this.placementSet.Placement)
					if (placementRow.IsValid && placementRow.BlockOrderId == blockOrderId)
						arrayList.Add(placementRow);
				foreach (PlacementSet.PlacementRow placementRow in arrayList)
				{
					placementRow.Delete();
					placementRow.AcceptChanges();
				}

			}
			finally
			{

				// Release the tables
				ReleaseGlobalPlacement();

			}

		}

		/// <summary>
		/// Inserts a new placement record into the viewer.
		/// </summary>
		/// <param name="argument">Thread parameters (not used)</param>
		private void InsertPlacementCommand(params object[] argument)
		{

			int localPlacementId;

			try
			{

				// Lock the global tables.
				LockGlobalPlacement();

				// Search for a default broker.  A directed order will set this value. If the order isn't directed, then the most
				// recent placement will define the value.  If there are no placements, then the last placement is used. If none of
				// these are correct, well, the user can put his own d*mned broker symbol in.
				object brokerId = GetDefaultBrokerId();

				// Initialize the new record.  Note that we use a quantity value that can be initialized from the outside. This was
				// done because there were no other methods to pass the value in.  It was highly desirable to have a single point
				// where the new placements are Created.  This ended up being the 'Select' logic, because the user could 'Select'
				// the placeholder row.  Also note that the placement id for this element is an artificial construct.  We use
				// negative numbers to keep the new placements distinct from the read ones on the middle tier.  However, it's
				// useful for constructing the report to have temporary placement records until the server gets around to creating
				// the records and handing us back a real id to use.
				PlacementSet.PlacementRow newPlacementRow = this.placementSet.Placement.NewPlacementRow();
				newPlacementRow.BlockOrderId = this.BlockOrderId;
				if (brokerId != null) newPlacementRow.BrokerId = (int)brokerId;
				this.placementSet.Placement.AddPlacementRow(newPlacementRow);
				newPlacementRow.AcceptChanges();

				// All new rows have to be validated and are initially empty.
				newPlacementRow.IsValid = false;
				newPlacementRow.IsEmpty = true;

				// The next step is to select the first cell for the user input.  The document must be drawn in order to use the 
				// address map to select a cell in the on screen document.  This variable passes the new placement id along to the
				// code following the 'DrawDocument' call.  That section of code is responsible for selecting a cell for the user.
				localPlacementId = newPlacementRow.LocalPlacementId;

			}
			finally
			{

				// Release the Global Tables
				ReleaseGlobalPlacement();

			}

			// There is no graceful way to insert (or delete) a record without a template for the line already in the document.
			// This will redraw the complete document with the newly created, blank line ready for the user's input.
			DrawDocumentCommand();

			// The final step is to help the user out by selecting the most likely field for the input to begin.
			CellAddress cellAddress = null;

			// Find the local placement that was just added.  This record is used to find the first field in the on screen
			// document that sould be selected.
			PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
			if (placementRow == null)
				throw new Exception(string.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// The order of selection goes from Time In Force Instruction, to quantity to broker.  If data has been provided
				// for the broker and quantity, the cursor will be left in the TIF instruction column.
				AddressMap.AddressMapRow addressMapRow =
					this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId,
					ColumnType.TimeInForceMnemonic);
				if (placementRow.IsQuantityNull())
					addressMapRow = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId,
						ColumnType.Quantity);
				if (placementRow.IsBrokerIdNull())
					addressMapRow = this.addressMap.AddressMap.FindByPlacementIdColumnType(placementRow.LocalPlacementId,
						ColumnType.BrokerSymbol);

				// This will address the starting cell for the user's input once the record is created.
				cellAddress = new CellAddress(addressMapRow.DocumentVersion, addressMapRow.RowIndex, addressMapRow.ColumnIndex);
			
			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();
				
			}

			// This will help the user out by selecting the first field for input in the new record.
			if (cellAddress != null)
				Select(cellAddress);

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void InsertLocalPlacement(PlacementSet.PlacementRow placementRow)
		{

			// Call the web service to add the new placementRow.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Placement");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("localPlacementId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("blockOrderId", placementRow.BlockOrderId);
			remoteMethod.Parameters.Add("brokerId", placementRow.BrokerId);
			remoteMethod.Parameters.Add("timeInForceCode", placementRow.TimeInForceCode);
			remoteMethod.Parameters.Add("orderTypeCode", placementRow.OrderTypeCode);
			remoteMethod.Parameters.Add("isRouted", placementRow.IsRouted);
			remoteMethod.Parameters.Add("quantity", placementRow.Quantity);
			remoteMethod.Parameters.Add("price1", placementRow.IsPrice1Null() ? (object)DBNull.Value : placementRow.Price1);
			remoteMethod.Parameters.Add("price2", placementRow.IsPrice2Null() ? (object)DBNull.Value : placementRow.Price2);
			ClientMarketData.Execute(remoteBatch);

			// This is where the old switcheroo takes place.  The global record that was just created will contain a new local 
			// identifier that is unrelated to the original record that was used to create the global record.  The local record was
			// deleted above and the local identifier of the new record will be changed to match the deleted one.  This
			placementRow.PlacementId = (int)remoteMethod.Parameters["localPlacementId"].Value;

		}

		/// <summary>
		/// Updates the placement record on the server.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void UpdateGlobalPlacement(PlacementSet.PlacementRow placementRow)
		{

			// Update the global placement record.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Placement");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
			remoteMethod.Parameters.Add("placementId", placementRow.PlacementId);
			remoteMethod.Parameters.Add("blockOrderId", placementRow.BlockOrderId);
			remoteMethod.Parameters.Add("brokerId", placementRow.BrokerId);
			remoteMethod.Parameters.Add("timeInForceCode", placementRow.TimeInForceCode);
			remoteMethod.Parameters.Add("orderTypeCode", placementRow.OrderTypeCode);
			remoteMethod.Parameters.Add("rowVersion", placementRow.RowVersion);
			remoteMethod.Parameters.Add("isRouted", placementRow.IsRouted);
			remoteMethod.Parameters.Add("quantity", placementRow.Quantity);
			remoteMethod.Parameters.Add("price1", placementRow.IsPrice1Null() ? (object)DBNull.Value : placementRow.Price1);
			remoteMethod.Parameters.Add("price2", placementRow.IsPrice2Null() ? (object)DBNull.Value : placementRow.Price2);
			ClientMarketData.Execute(remoteBatch);

		}

		/// <summary>
		/// Deletes a local placement record.
		/// </summary>
		/// <param name="localPlacementId"></param>
		private void DeletePlacementCommand(params object[] argument)
		{

			// Extract the command argument.
			int localPlacementId = (int)argument[0];

			// Make sure that the table locks are released.		
			try
			{

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// If the record exists in the global tables, then delete it.
				if (!placementRow.IsPlacementIdNull())
				{

					// Call the server to delete the record.
					RemoteBatch remoteBatch = new RemoteBatch();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Placement");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Delete");
					remoteMethod.Parameters.Add("placementId", placementRow.PlacementId);
					remoteMethod.Parameters.Add("rowVersion", placementRow.RowVersion);
					ClientMarketData.Execute(remoteBatch);

				}
			
				// Remove the record from the local table.
				placementRow.Delete();
				placementRow.AcceptChanges();

				// Draw the document now that the record has been removed.
				DrawDocument();

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, resourceManager.GetString("quasarError"),
						MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Clears the local table of unused placements.
		/// </summary>
		/// <returns>true to indicate that unused records were removed.</returns>
		private void PurgeUnusedPlacementCommand(params object[] argument)
		{

			// The uncommitted placements are going to be cleared out of the display.  This involves clearing them out of the data
			// structure also.  Since you can't delete while iterating, the records that are marked for deletion will be placed in
			// this list first.
			ArrayList deleteList = new ArrayList();

			// We'll keep the local record if someone bothered to enter a quantity.  The 'isChanged' flag will let the caller know
			// that the data behind the document has changed.
			foreach (PlacementSet.PlacementRow placementRow in this.placementSet.Placement)
				if (placementRow.IsQuantityNull() || placementRow.Quantity == 0.0M)
					deleteList.Add(placementRow);

			// Remove each of the placement records that were marked for deletion.
			foreach (PlacementSet.PlacementRow placementRow in deleteList)
			{
				placementRow.Delete();
				placementRow.AcceptChanges();
			}

			// If any of the local records were deleted, then redraw the document.
			if (deleteList.Count != 0)
				QueueCommand(new ThreadHandler(DrawDocumentCommand));

		}

		/// <summary>
		/// Set the broker id of a local placement record based on the symbol.
		/// </summary>
		/// <param name="localPlacementId">Identifer of the placement.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetBrokerId(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks needed for the 'UpdateBrokerFields' call.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// Rule #1: A broker must be specified.
				if (editValue == null)
					throw new FieldException(localPlacementId, ColumnType.BrokerSymbol, "invalidBrokerSymbol");
				
				// Update the broker id in the local table.
				placementRow.BrokerId = Convert.ToInt32(editValue);

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a field.  Finally, the 'IsEmpty' is used to track empty records that
				// can be deleted automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				placementRow.IsError = false;
				placementRow.IsEmpty = false;

				// Create and execute a spreadsheet command to update the broker fields.
				ExecuteDisplayBatch(UpdateBrokerFields(new ArrayList(), placementRow));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
	
		/// <summary>
		/// Set the Time In Force Instruction for a local placement.
		/// </summary>
		/// <param name="localPlacementId">Identifier of the record.</param>
		/// <param name="timeInForceCode">TimeInForceCode executed.</param>
		private void SetTimeInForceCode(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// Rule #1: A Time In Force must be specified.
				if (editValue == null)
					throw new FieldException(localPlacementId, ColumnType.TimeInForceMnemonic, "invalidTimeInForceMnemonic");
				
				// Update the Time In Force Code.
				placementRow.TimeInForceCode = Convert.ToInt32(editValue);

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a field.  Finally, the 'IsEmpty' is used to track empty records that
				// can be deleted automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				placementRow.IsError = false;
				placementRow.IsEmpty = false;

				// Create and execute a spreadsheet command to update the time in force field.
				ExecuteDisplayBatch(UpdateTimeInForceFields(new ArrayList(), placementRow));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
		
		/// <summary>
		/// Set the OrderTypeCode of a local placement.
		/// </summary>
		/// <param name="localPlacementId">Identifier of the record.</param>
		/// <param name="OrderTypeCode">OrderTypeCode executed.</param>
		private void SetOrderTypeCode(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// Rule #1: An order type code must be specified.
				if (editValue == null)
					throw new FieldException(localPlacementId, ColumnType.OrderTypeMnemonic, "invalidOrderTypeMnemonic");
				
				// Update the OrderTypeCode
				placementRow.OrderTypeCode = Convert.ToInt32(editValue);

				// Clear out the unused price fields based on the type of order pricing instructions.
				switch (placementRow.OrderTypeCode)
				{

				case OrderType.Market:

					// A market order doesn't require any prices to be specified.
					placementRow.SetPrice1Null();
					placementRow.SetPrice2Null();
					break;

				case OrderType.Limit:
				case OrderType.StopLoss:

					// A limit order and stop loss order require a single price.
					placementRow.SetPrice2Null();
					break;

				}

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a field.  Finally, the 'IsEmpty' is used to track empty records that
				// can be deleted automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				placementRow.IsError = false;
				placementRow.IsEmpty = false;

				// Create and execute a spreadsheet command to update the order type and pricing fields.
				ArrayList displayBatch = new ArrayList();
				UpdateOrderTypeFields(displayBatch, placementRow);
				UpdatePrice1Field(displayBatch, placementRow);
				UpdatePrice2Field(displayBatch, placementRow);
				ExecuteDisplayBatch(displayBatch);

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
		
		/// <summary>
		/// Set the Quantity of a local placement.
		/// </summary>
		/// <param name="localPlacementId">Identifier of the record.</param>
		/// <param name="quantity">quantity executed.</param>
		private void SetQuantity(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// Rule #1: An quantity must be specified.
				if (editValue == null)
					throw new FieldException(localPlacementId, ColumnType.OrderTypeMnemonic, "invalidQuantity");
				
				// Convert the Quantity string to a decimal number.
				decimal quantity = Convert.ToDecimal(editValue);

				// Rule #2: Quantities must be positive.
				if (quantity < 0.0M)
					throw new FieldException(localPlacementId, ColumnType.Quantity, "negativeQuantity");
			
				// Update the Quantity
				placementRow.Quantity = quantity;

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a related field the viewer.  Note that the quantity field is an 
				// aggregate field. Changing one record can impact the validity of another (such as when the quantity placed is
				// greater than the quantity ordered).  Finally, the 'IsEmpty' is used to track empty records that can be deleted
				// automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				foreach (PlacementSet.PlacementRow siblingPlacementRow in this.placementSet.Placement)
					siblingPlacementRow.IsError = false;
				placementRow.IsError = false;

				// Invoke the foreground to update the quantity field.
				ExecuteDisplayBatch(UpdateQuantityField(new ArrayList(), placementRow));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(new FieldException(localPlacementId, ColumnType.Quantity, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
		
		/// <summary>
		/// Set the price1 of a local placement.
		/// </summary>
		/// <param name="argument">Generic command argument.</param>
		private void SetPrice1(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field 
				// needs to be turned into a price.
				if (editValue == null)
					placementRow.SetPrice1Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price1 = Convert.ToDecimal(editValue);

					// Rule #1: Price must be positive.
					if (price1 < 0.0M)
						throw new FieldException(localPlacementId, ColumnType.Price1, "negativePrice");
				
					// Update the price1
					placementRow.Price1 = price1;

				}

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a field.  Finally, the 'IsEmpty' is used to track empty records that
				// can be deleted automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				placementRow.IsError = false;
				placementRow.IsEmpty = false;

				// Create and execute a spreadsheet command to update the limit price field.
				ExecuteDisplayBatch(UpdatePrice1Field(new ArrayList(), placementRow));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(new FieldException(localPlacementId, ColumnType.Price1, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
		
		/// <summary>
		/// Set the price2 of a local placement.
		/// </summary>
		/// <param name="argument">Generic command argument.</param>
		private void SetPrice2(int localPlacementId, object editValue)
		{

			try
			{

				// Aquire the locks.
				LockGlobalPlacement();

				// Find the record in the local table.
				PlacementSet.PlacementRow placementRow = this.placementSet.Placement.FindByLocalPlacementId(localPlacementId);
				if (placementRow == null)
					throw new Exception(String.Format(resourceManager.GetString("localPlacementNotFound"), localPlacementId));

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field 
				// needs to be turned into a price.
				if (editValue == null)
					placementRow.SetPrice2Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price2 = Convert.ToDecimal(editValue);

					// Rule #1: Price must be positive.
					if (price2 < 0.0M)
						throw new FieldException(localPlacementId, ColumnType.Price2, "negativePrice");
				
					// Update the price2
					placementRow.Price2 = price2;

				}

				// The 'IsValid' and 'IsError' flags control the handling of records and errors.  The 'IsValid' indicates that the
				// record is valid and agrees with the global version of the record.  When any modifications are made to the
				// record, it will be invalidated until checked again.  However, if an error occurs while validating the record, it
				// shouldn't be validated again until the user changes something.  The 'IsError' flag is set when an error is
				// detected and cleared when the user modifies a field.  Finally, the 'IsEmpty' is used to track empty records that
				// can be deleted automatically when the viewer looses the focus.
				placementRow.IsValid = false;
				placementRow.IsError = false;
				placementRow.IsEmpty = false;

				// Create and execute a spreadsheet command to update the stop limit price field.
				ExecuteDisplayBatch(UpdatePrice2Field(new ArrayList(), placementRow));

			}
			catch (FieldException fieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(fieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				FieldExceptionHandler(new FieldException(localPlacementId, ColumnType.Price2, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalPlacement();

			}

		}
		
		/// <summary>
		/// Handles a new value for a cell in an on screen document.
		/// </summary>
		/// <param name="arguments">Thread arguments.</param>
		private void OnEndEditCommand(params object[] arguments)
		{

			// Extract the command arguments.
			CellAddress cellAddress = (CellAddress)arguments[0];
			object result = arguments[1];

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Extract the current row and column from the event argument.  This is the address of the cell where 
				// the editing took place. Note that, because the user can move the focus at any time to another cell,
				// this is not necessarily the address of the currently active cell.
				int rowIndex = cellAddress.RowIndex;
				int columnIndex = cellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a local record.
				if (rowType == RowType.Placement)
				{

					// Get the placement id from the selected row.
					int localPlacementIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PlacementId);
					int localPlacementId = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						localPlacementIdColumnIndex)));

					// The column type of the cell tells us which command to execute in the background for updating the cell.  Note
					// also that every time a field is updated, the record is invalidated.  This will force a record validation
					// command to be executed when the focus is moved from the current record.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

					case ColumnType.BrokerSymbol: SetBrokerId(localPlacementId, result); break;
					case ColumnType.TimeInForceMnemonic: SetTimeInForceCode(localPlacementId, result); break;
					case ColumnType.OrderTypeMnemonic: SetOrderTypeCode(localPlacementId, result); break;
					case ColumnType.Quantity: SetQuantity(localPlacementId, result); break;
					case ColumnType.Price1: SetPrice1(localPlacementId, result); break;
					case ColumnType.Price2: SetPrice2(localPlacementId, result); break;

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Handles an escape command.
		/// </summary>
		public override void CommandEscape()
		{

			// Move the selection home position.
			CommandHome();

			// Give the focus back to whoever wants it.  This will cause 'OnLeave' to be called which will clear out the
			// empty records and attempt to validate any current work.  But since the selection flag has been turned off,
			// the validation will be skipped on the current record and it will disappear.
			OnReleaseFocus();

		}

		/// <summary>
		/// Clears a cell.
		/// </summary>
		/// <param name="arguments">The command arguments.</param>
		private void ClearCommand(params object[] arguments)
		{
		
			// Extract the command arguments.
			CellAddress cellAddress = (CellAddress)arguments[0];

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Extract the current row and column.  The task will be handled in the background thread since a change
				// in the selection can impact the data model.  So we collect the foreground information here and spawn a
				// thread to handle the action.
				int rowIndex = cellAddress.RowIndex;
				int columnIndex = cellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a ThreadHandler to update a local record.
				if (rowType == RowType.Placement)
				{

					// Get the placement id from the selected row.
					int localPlacementIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PlacementId);
					int localPlacementId = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						localPlacementIdColumnIndex)));

					// The column type of the cell tells us which ThreadHandler to execute in the background for updating the
					// cell.  This is only marginally better to look at than some lookup table.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

					case ColumnType.BrokerSymbol:
					case ColumnType.TimeInForceMnemonic:
					case ColumnType.OrderTypeMnemonic:
					case ColumnType.Quantity:

						// Give the user feedback that these values can't be Modified.
						Sounds.PlaySound(Sounds.MB_ICONASTERISK);
						break;

					case ColumnType.Price1:
							
						SetPrice1(localPlacementId, null);
						break;

					case ColumnType.Price2:
							
						SetPrice2(localPlacementId, null);
						break;

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Called when the selection moves from one cell to another.
		/// </summary>
		/// <param name="arguments">Command arguments.</param>
		private void SelectionChangeCommand(params object[] arguments)
		{

			// Extract the command arguments.
			CellAddress cellAddress = (CellAddress)arguments[0];
			bool containsFocus = (bool)arguments[1];
			
			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = (rowTypeColumnIndex == SpreadsheetColumn.DoesNotExist) ? RowType.Unused :
					Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, cellAddress.RowIndex, rowTypeColumnIndex)));

				// The row type defines what action to take based on the selection changing.
				switch (rowType)
				{

				case RowType.Unused:

					// If we had previously selected a placement record, see if it needs to be inserted or updated before we move
					// on to the next record.
					if (this.selectedRowType == RowType.Placement)
						QueueCommand(new ThreadHandler(CommitChangesCommand));

					break;

				case RowType.Placeholder:

					// Commit any placement records that have been modified.
					if (this.selectedRowType == RowType.Placement)
						QueueCommand(new ThreadHandler(CommitChangesCommand));

					// It's possible for the placeholder to be selected when an empty placement is drawn, since the placeholder
					// ends up in the first row.  However, the line shouldn't be initialized until the window gains the focus.
					// Also, there are times when selecting the placeholder should be ignored, such as when the program is taking
					// care of initializing a row.  In all other cases, when the user clicks on a placeholder row, a line is
					// initialized for them.
					if (containsFocus)
						QueueCommand(new ThreadHandler(InsertPlacementCommand));

					break;

				case RowType.Placement:

					// Get the placement identifier from the spreadsheet row.
					int localPlacementIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PlacementId);
					int localPlacementId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, cellAddress.RowIndex, localPlacementIdColumnIndex)));

					// The record is only validated when the selection has changed from the previously selected placement record.
					if (this.selectedRowType == RowType.Placement && this.selectedPlacementId != localPlacementId)
						QueueCommand(new ThreadHandler(CommitChangesCommand));

					// This will save the identifier of the placement record that was selected.  When the selection is changed to
					// another placement record or another row type, it is time to commit the prevous record.
					this.selectedPlacementId = localPlacementId;
						 
					break;

				}

				// This will preserve the selected row type for the next time the selection is changed.  The state driven logic can
				// use this state to determine if the record needs to be committed or not.
				this.selectedRowType = rowType;

			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}
	
		}

		/// <summary>
		/// Deletes the active row.
		/// </summary>
		/// <param name="arguments">Command arguments.</param>
		private void DeleteActiveRowCommand(params object[] arguments)
		{

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Extract the current row and column.
				CellAddress activeCellAddress = GetActiveCellAddress();
				int rowIndex = activeCellAddress.RowIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// Delete a local placement
				if (rowType == RowType.Placement)
				{

					// Get the placement id from the selected row.
					int localPlacementIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PlacementId);
					int localPlacementId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						localPlacementIdColumnIndex)));

					// Execute a command on the worker thread to delete the local record.
					QueueCommand(new ThreadHandler(DeletePlacementCommand), localPlacementId);

					// Move the selection to the previous placement.
					CommandMoveUp();

				}

			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Commits the changes made to the placement records to the server.
		/// </summary>
		private void CommitChangesCommand(params object[] arguments)
		{

			// Validate any row that isn't already valid and isn't empty.
			foreach (PlacementSet.PlacementRow placementRow in this.placementSet.Placement)
				if (placementRow.BlockOrderId == this.BlockOrderId)
					if (!placementRow.IsValid && !placementRow.IsError && !placementRow.IsEmpty)
						ValidatePlacement(placementRow);

		}
		
		/// <summary>
		/// Called when the focus enters the window
		/// </summary>
		/// <param name="eventArgs">Event parameters.</param>
		protected override void OnEnter(EventArgs eventArgs)
		{

			// If the document is open, then interpret the focus coming into the viewer as if a new cell were selected. This is
			// done to force the 'Prompting' action of new placements and a re-selection of existing placements. The document will
			// be entered one or more times during the initialization, so these events are filtered out because all the document
			// metrics haven't been calculated yet.
			if (this.IsDocumentOpen)
				OnSelectionChange(new SelectionChangeArgs(GetActiveCellAddress()));

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnEnter(eventArgs);

		}

		/// <summary>
		/// Called when the focus leaves this viewer.
		/// </summary>
		/// <param name="eventArgs">Event parameters.</param>
		protected override void OnLeave(EventArgs eventArgs)
		{

			// Make sure the document is open before cleaning it up.
			if (this.IsDocumentOpen)
			{

				// Make sure that all changes are committed when the focus leaves this viewer.
				if (this.selectedRowType == RowType.Placement)
					QueueCommand(new ThreadHandler(CommitChangesCommand));

				// Clear the local table of any unused records.  This must be done in the background to avoid deadlocks.  This
				// command will also redraw the document if anything has change that would impact the viewer.
				QueueCommand(new ThreadHandler(PurgeUnusedPlacementCommand));

			}

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnLeave(eventArgs);

		}

		/// <summary>
		/// Called when a cell in the spreadsheet control has finished editing.
		/// </summary>
		/// <param name="endEditEventArgs">Event parameters.</param>
		protected override void OnEndEdit(Shadows.Quasar.Common.Controls.EndEditEventArgs endEditEventArgs)
		{

			// Execute this command on the command thread queue.
			QueueCommand(new ThreadHandler(OnEndEditCommand), endEditEventArgs.CellAddress, endEditEventArgs.Result);

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnEndEdit(endEditEventArgs);

		}

		/// <summary>
		/// Clears the values from a field in the viewer.
		/// </summary>
		/// <param name="eventArgs">The event argument.</param>
		protected override void OnCommandClear(EventArgs eventArgs)
		{

			// Execute this command on the command thread queue.
			QueueCommand(new ThreadHandler(ClearCommand), GetActiveCellAddress());

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnCommandClear(eventArgs);

		}

		/// <summary>
		/// Opens the a block order in the execution viewer.
		/// </summary>
		/// <param name="blockOrderId">The primary identifier of the object to open.</param>
		public void OpenBlockOrder(int blockOrderId)
		{

			// Execute a command in the background to open up the document.  Constructing an appraisal will require access
			// to the data model to build a model and select a stylesheet.
			QueueCommand(new ThreadHandler(OpenBlockOrderCommand), blockOrderId);

		}

		/// <summary>
		/// Closes the a block order in the execution viewer.
		/// </summary>
		/// <param name="blockOrderId">The primary identifier of the object to close.</param>
		public void CloseBlockOrder(int blockOrderId)
		{

			// Execute a command in the background to close up the document.  Constructing an appraisal will require access
			// to the data model to build a model and select a stylesheet.
			QueueCommand(new ThreadHandler(CloseBlockOrderCommand), blockOrderId);

		}

		/// <summary>
		/// Deletes the current row from the viewer.
		/// </summary>
		public void DeleteActiveRow()
		{

			// Execute a command in the background to open up the document.  Constructing a placement document will require access 
			// to the data model to build a DOM and select a stylesheet.
			QueueCommand(new ThreadHandler(DeleteActiveRowCommand));

		}

		/// <summary>
		/// Handles the selection changing from one cell to another.
		/// </summary>
		/// <param name="selectionChangeArgs">Event argument.</param>
		protected override void OnSelectionChange(Shadows.Quasar.Viewers.SelectionChangeArgs selectionChangeArgs)
		{

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnSelectionChange(selectionChangeArgs);

			// This command must be completed in the background because it has to wait for resources.
			QueueCommand(new ThreadHandler(SelectionChangeCommand), selectionChangeArgs.CellAddress, this.ContainsFocus);

		}

	}

}
