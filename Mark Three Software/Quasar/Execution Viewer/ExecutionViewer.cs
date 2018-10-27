/*************************************************************************************************************************
*
*	File:			ExecutionViewer.cs
*	Description:	This control is used to display and manage a execution.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Execution
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
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;
	using System.Security.Policy;
	using System.Web.Services.Protocols;

	/// <summary>
	/// A Viewer for Order Execution
	/// </summary>
	public class ExecutionViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private bool isManualSelection;
		private bool isExecutionChanged;
		private bool isExecutionSelected;
		private int blotterId;
		private int blockOrderId;
		private int countryId;
		private int securityTypeCode;
		private int selectedRowType;
		private int selectedExecutionId;
		private Shadows.Quasar.Client.ClientMarketData clientClientMarketData;
		private System.Xml.Xsl.XslTransform xslTransform;
		private System.ComponentModel.IContainer components = null;
		private AddressMap addressMap;
		private ExecutionSet executionSet;
		private delegate void ExecutionDelegate(Execution execution);
		private delegate void SelectCellDelegate(bool isLocal, int executionId, int columnType);
		private ExecutionDelegate setExecutionDelegate;
		private ExecutionDelegate getExecutionDelegate;
		private ExecutionDelegate initializeRowDelegate;
		private SelectCellDelegate selectCellDelegate;
		private ReaderWriterLock executionSetLock = new ReaderWriterLock();

		/// <summary>
		/// Constructor for the ExecutionViewer
		/// </summary>
		public ExecutionViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// Add the debt calculation library to the functions recognized by this spreadsheet control.
			this.ExternalLibraries.Add(new Viewers.DebtLibrary.Debt());

			// An 'id' of zero is reserved and will never exist.  This will force the initial view of the spreadsheet 
			// control to be a blank document with just the headers shown.  This is the desired effect on initialization.
			this.blockOrderId = 0;

			// These variables help control the state of the viewer.  Since we're using the spreadsheet control like a 
			// ledger, leaving a line after it's been modified creates a command to update the database.  Since the user
			// is free to click most anywhere on the input screen, we have no way of knowing what the last record was
			// unless we remember it using these members.
			this.isManualSelection = true;
			this.isExecutionChanged = false;
			this.isExecutionSelected = false;

			// This is the accelerator for the spreadsheet.  It will map a row type, row and column set to an address on 
			// the spreadsheet.  It is built after the XML spreadsheet document is constructed and used primarily when
			// events fire to change the state of the report.
			this.addressMap = new AddressMap();

			// This DataSet is used to hold local executions.  These are executions that can't be placed in the global
			// data model because they don't have an identifier yet.  So we keep a local copy until the server can give us
			// an identifier, then they'll be moved into the 'ClientMarketData'.
			this.executionSet = new ExecutionSet();

			// IMPORTANT CONCEPT:  Create the delegates for updating the spreadsheet control from the background.  We use
			// the address map to find cells in the spreadsheet with the idea that we're going to read or write to those
			// cells.  This presents a bit of a problem for a multithreaded application.  We can't use table locking in
			// the foreground: if a lock on that table is obtained in the background, then the foreground will be
			// blocked.  If that same background thread attempts to use an 'Invoke', an insidious deadlock will happen
			// because the message loop (foeground thread) is waiting on the table lock that the background thread has.  
			// The background thread is waiting on the message loop to process the 'Invoke' command.  For this reason,
			// waiting on locks has been prohibited in the foreground.  Background threads must use delegates to
			// communicate with the foreground message loop.  The ugly impact is these methods that must execute in the
			// foreground and can only be called using the 'Invoke' method.
			this.setExecutionDelegate = new ExecutionDelegate(SetExecution);
			this.getExecutionDelegate = new ExecutionDelegate(GetExecution);
			this.initializeRowDelegate = new ExecutionDelegate(InitializeRow);
			this.selectCellDelegate = new SelectCellDelegate(SelectCell);

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExecutionViewer));
			this.clientClientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			this.SuspendLayout();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			// 
			// ExecutionViewer
			// 
			this.MoveAfterReturn = true;
			this.Name = "ExecutionViewer";
			this.Leave += new System.EventHandler(this.ExecutionViewer_Leave);
			this.SelectionChange += new Shadows.Quasar.Viewers.SelectionChangeHandler(this.ExecutionViewer_SelectionChange);
			this.Enter += new System.EventHandler(this.ExecutionViewer_Enter);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Thread safe indication of a changed execution record.
		/// </summary>
		[Browsable(false)]
		public bool IsExecutionSelected
		{
			get {lock (this) return this.isExecutionSelected;}
			set {lock (this) this.isExecutionSelected = value;}
		}

		/// <summary>
		/// Thread safe indication of a changed execution record.
		/// </summary>
		[Browsable(false)]
		public bool IsExecutionChanged
		{
			get {lock (this) return this.isExecutionChanged;}
			set {lock (this) this.isExecutionChanged = value;}
		}

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
		/// Thread safe acces to the selected execution id.
		/// </summary>
		[Browsable(false)]
		public int SelectedExecutionId
		{
			get {lock (this) return this.selectedExecutionId;}
			set {lock (this) this.selectedExecutionId = value;}
		}

		/// <summary>
		/// Thread safe acces to the selected execution id.
		/// </summary>
		[Browsable(false)]
		public int SelectedRowType
		{
			get {lock (this) return this.selectedRowType;}
			set {lock (this) this.selectedRowType = value;}
		}

		/// <summary>
		/// Create a mapping of key elements to row and column addresses in the spreadsheet.
		/// </summary>
		/// <param name="xmlDocument">The source document to parse for the key values.</param>
		protected override void CreateAddressMap(XmlDocument xmlDocument)
		{

			// IMPORTANT CONCEPT: The address map is used to quickly look up a cell in the spreadsheet control based on
			// some key information.  This method will parse the Xml Document for that key information and construct a
			// data structure to map the key elements to specific cells in the sheet.  This method will also parse out the
			// column definitions from the 'names' table so we have a flexible mapping of the column indices.  That is,
			// because the stylesheet can change, the column that a particular value shows up in (identifier, quantity,
			// price, etc) can change also.  So we need a way of mapping the columns to the function they provide.  This
			// object has to be created in the foreground and can only be used by the foreground (message thread) because
			// waiting for events and table locks are prophibited in the foreground.  So the only way to insure that there
			// is no corruption to the table is to limit access to a single thread.
			this.addressMap = new AddressMap(xmlDocument, this.IncrementDocumentVersion());

		}

		/// <summary>
		/// Locks the global tables for all operations involving placements.
		/// </summary>
		private void LockGlobalExecution()
		{

			// Lock the global tables.
			Debug.Assert(!ClientMarketData.AreLocksHeld);
			ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ConditionLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);
			ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);

		}

		/// <summary>
		/// Releases the global table locks.
		/// </summary>
		private void ReleaseGlobalExecution()
		{

			// Release the tables.
			if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
			if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
			if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
			if (ClientMarketData.ConditionLock.IsReaderLockHeld) ClientMarketData.ConditionLock.ReleaseReaderLock();
			if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
			if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
			if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
			if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
			if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
			Debug.Assert(!ClientMarketData.AreLocksHeld);

		}

		/// <summary>
		/// Updates an execution line on the spreadsheet.
		/// </summary>
		/// <param name="execution">An execution record.</param>
		private void SetExecution(Execution execution)
		{

			// The record can either be a local or global execution.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The Execution record passed into this method will indicate whether a local or global record is being 
			// changed.
			if (execution.IsLocal)
			{

				// Find the line number and document version based on the local identifier.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(execution.ExecutionId);
				if (localMapRow != null)
				{
					rowIndex = localMapRow.RowIndex;
					documentVersion = localMapRow.DocumentVersion;
				}

			}
			else
			{

				// Find the line number and document version based on the global identifier.
				AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByExecutionId(execution.ExecutionId);
				if (globalMapRow != null)
				{
					rowIndex = globalMapRow.RowIndex;
					documentVersion = globalMapRow.DocumentVersion;
				}

			}

			// If the row doesn't exist in the mappings, we'll have to refresh the entire document to include it.
			// Otherwise, we will look at each field and see if it needs to be updated in the spreadsheet.  Sometimes, the
			// entire record will have changed, such as when we get an update from the server's data model.  Other times,
			// only one field will change, such as when a user enters a new quantity or other attribute.  This method was
			// built to handle any updates to the spreadsheet from the background.
			if (rowIndex == SpreadsheetRow.DoesNotExist)
			{

				// Abort the incremental upates and force a refresh of the whole document.
				this.IsRefreshNeeded = true;

			}
			else
			{

				// BrokerId
				if (execution.IsBrokerIdModified())
				{
					int brokerIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.BrokerId);
					if (brokerIdColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, brokerIdColumnIndex), execution.BrokerId);
				}

				// BrokerName
				if (execution.IsBrokerNameModified())
				{
					int brokerNameColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.BrokerName);
					if (brokerNameColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, brokerNameColumnIndex), execution.BrokerName);
				}

				// BrokerSymbol
				if (execution.IsBrokerSymbolModified())
				{
					int brokerSymbolColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.BrokerSymbol);
					if (brokerSymbolColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, brokerSymbolColumnIndex), execution.BrokerSymbol);
				}

				// ExecutionId
				if (execution.IsExecutionIdModified())
				{
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
					if (executionIdColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, executionIdColumnIndex), execution.ExecutionId);
				}

				// Quantity
				if (execution.IsQuantityModified())
				{
					int quantityColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Quantity);
					if (quantityColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, quantityColumnIndex), execution.Quantity);
				}

				// Price
				if (execution.IsPriceModified())
				{
					int priceColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Price);
					if (priceColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, priceColumnIndex), execution.Price);
				}

				// Commission
				if (execution.IsCommissionModified())
				{
					int commissionColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Commission);
					if (commissionColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, commissionColumnIndex), execution.Commission);
				}

				// UserFee0
				if (execution.IsUserFee0Modified())
				{
					int userFee0ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee0);
					if (userFee0ColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, userFee0ColumnIndex), execution.UserFee0);
				}

				// UserFee1
				if (execution.IsUserFee1Modified())
				{
					int userFee1ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee1);
					if (userFee1ColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, userFee1ColumnIndex), execution.UserFee1);
				}

				// UserFee2
				if (execution.IsUserFee2Modified())
				{
					int userFee2ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee2);
					if (userFee2ColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, userFee2ColumnIndex), execution.UserFee2);
				}

				// UserFee3
				if (execution.IsUserFee3Modified())
				{
					int userFee3ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee3);
					if (userFee3ColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, userFee3ColumnIndex), execution.UserFee3);
				}
						
				// TradeDate
				if (execution.IsTradeDateModified())
				{
					int tradeDateColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.TradeDate);
					if (tradeDateColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, tradeDateColumnIndex), execution.TradeDate);
				}

				// SettlementDate
				if (execution.IsSettlementDateModified())
				{
					int settlementDateColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.SettlementDate);
					if (settlementDateColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, settlementDateColumnIndex), execution.SettlementDate);
				}

				// CreatedTime
				if (execution.IsCreatedTimeModified())
				{
					int createdTimeColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.CreatedTime);
					if (createdTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, createdTimeColumnIndex), execution.CreatedTime);
				}

				// CreatedLoginId
				if (execution.IsCreatedLoginIdModified())
				{
					int createdLoginIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.CreatedLoginId);
					if (createdLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, createdLoginIdColumnIndex), execution.CreatedLoginId);
				}

				// CreatedLoginName
				if (execution.IsCreatedLoginNameModified())
				{
					int createdLoginNameColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.CreatedLoginName);
					if (createdLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, createdLoginNameColumnIndex), execution.CreatedLoginName);
				}

				// ModifiedTime
				if (execution.IsModifiedTimeModified())
				{
					int modifiedTimeColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ModifiedTime);
					if (modifiedTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, modifiedTimeColumnIndex), execution.ModifiedTime);
				}

				// ModifiedLoginId
				if (execution.IsModifiedLoginIdModified())
				{
					int modifiedLoginIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ModifiedLoginId);
					if (modifiedLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginIdColumnIndex), execution.ModifiedLoginId);
				}

				// ModifiedLoginName
				if (execution.IsModifiedLoginNameModified())
				{
					int modifiedLoginNameColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ModifiedLoginName);
					if (modifiedLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
						SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginNameColumnIndex), execution.ModifiedLoginName);
				}

			}

		}

		/// <summary>
		/// Gets the calculated fields from an execution on the screen.
		/// </summary>
		/// <param name="execution">An execution record.</param>
		private void GetExecution(Execution execution)
		{

			// The record can either be a local or global execution.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The Execution record passed into this method will indicate whether a local or global record is being 
			// changed.
			if (execution.IsLocal)
			{

				// Find the line number and document version based on the local identifier.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(execution.ExecutionId);
				if (localMapRow != null)
				{
					rowIndex = localMapRow.RowIndex;
					documentVersion = localMapRow.DocumentVersion;
				}

			}
			else
			{

				// Find the line number and document version based on the global identifier.
				AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByExecutionId(execution.ExecutionId);
				if (globalMapRow != null)
				{
					rowIndex = globalMapRow.RowIndex;
					documentVersion = globalMapRow.DocumentVersion;
				}

			}

			// If the row doesn't exist in the mappings, there is nothing to retrieve.
			if (rowIndex == SpreadsheetRow.DoesNotExist)
				return;

			// Commission
			if (execution.IsCommissionCalculated())
			{
				int commissionColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Commission);
				if (commissionColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.Commission = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, commissionColumnIndex)));
			}

			// AccruedInterest
			if (execution.IsAccruedInterestCalculated())
			{
				int accruedInterestColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.AccruedInterest);
				if (accruedInterestColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.AccruedInterest = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, accruedInterestColumnIndex)));
			}

			// UserFee0
			if (execution.IsUserFee0Calculated())
			{
				int userFee0ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee0);
				if (userFee0ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee0 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee0ColumnIndex)));
			}

			// UserFee1
			if (execution.IsUserFee1Calculated())
			{
				int userFee1ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee1);
				if (userFee1ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee1 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee1ColumnIndex)));
			}

			// UserFee2
			if (execution.IsUserFee2Calculated())
			{
				int userFee2ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee2);
				if (userFee2ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee2 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee2ColumnIndex)));
			}

			// UserFee3
			if (execution.IsUserFee3Calculated())
			{
				int userFee3ColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.UserFee3);
				if (userFee3ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee3 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee3ColumnIndex)));
			}

		}

		/// <summary>
		/// Selects a cell in the execution document based on the execution id and column type.
		/// </summary>
		/// <param name="isLocal">Indicates whether the row is a local or global execution.</param>
		/// <param name="executionId">Primary identifier of the record to be selected.</param>
		/// <param name="columnType">Column type of the cell to be selected.</param>
		private void SelectCell(bool isLocal, int executionId, int columnType)
		{

			// The record can either be a local or global execution.  The 'rowIndex' will have the line number in the
			// spreadsheet when the decoding logic has worked out where the line is.  The documentVersion is used in the
			// call to update each cell and will also be extracted from the AddressMap DataSet.  This section will find
			// the row index and document version for either a local or global execution record.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The Execution record passed into this method will indicate whether a local or global record is being
			// changed.
			if (isLocal)
			{

				// Find the line number and document version based on the local identifier.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(executionId);
				if (localMapRow != null)
				{
					rowIndex = localMapRow.RowIndex;
					documentVersion = localMapRow.DocumentVersion;
				}

			}
			else
			{

				// Find the line number and document version based on the global identifier.
				AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByExecutionId(executionId);
				if (globalMapRow != null)
				{
					rowIndex = globalMapRow.RowIndex;
					documentVersion = globalMapRow.DocumentVersion;
				}

			}

			// If the row and column exist, we can move the selection there.
			if (rowIndex != SpreadsheetRow.DoesNotExist)
			{
				int columnIndex = this.addressMap.GetColumnIndex(columnType);
				if (columnIndex != SpreadsheetColumn.DoesNotExist)
					Select(new CellAddress(documentVersion, rowIndex, columnIndex));
			}

		}

		/// <summary>
		/// Draws the document in the background.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// If new data or a new structure is available after the data model has changed, then initiate a refresh of
			// the viewer contents on a background thread.
			if (this.IsDocumentOpen && this.IsRefreshNeeded)
			{

				// Use the command thread to redraw this document.
				ExecuteCommand(Command.Draw, this.BlockOrderId, new ExecutionDrawArgs(this.executionSet));

				// We assume that no refresh is needed again until an event request one.
				this.IsRefreshNeeded = false;

			}

		}

		/// <summary>
		/// Opens the Execution Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Execution Viewer in the background.
			ExecuteCommand(Command.OpenViewer);

		}

		/// <summary>
		/// Closes the Execution Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Execution Viewer in the background.
			ExecuteCommand(Command.CloseViewer);

		}

		
		/// <summary>
		/// Opens the Execution Document.
		/// </summary>
		/// <param name="blotterId">The primary identifier of the object to open.</param>
		public override void OpenDocument(int blotterId)
		{

			// Open the Execution Document in the background.
			ExecuteCommand(Command.OpenDocument, blotterId);

		}

		/// <summary>
		/// Closes the Execution Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Close the Execution Document in the background.
			ExecuteCommand(Command.CloseDocument);

		}

		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="blockOrderId">The primary identifier of the object to open.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public void OpenBlockOrder(int blockOrderId)
		{

			// Execute a command in the background to open up the document.  Constructing an appraisal will require access
			// to the data model to build a model and select a stylesheet.
			ExecuteCommand(Command.OpenBlockOrder, blockOrderId);

		}
			
		/// <summary>
		/// Closes the document for the given object and it's argument.
		/// </summary>
		/// <param name="blockOrderId">The primary identifier of the object to close.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public void CloseBlockOrder(int blockOrderId) {}
			
		/// <summary>
		/// Opens the Viewer
		/// </summary>
		private void OpenViewerCommand()
		{

			// This block will attach the appraisal viewer to the data model changes.
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
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
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();
		
		}

		/// <summary>
		/// Closes the Execution Viewer.
		/// </summary>
		private void CloseViewerCommand()
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanged -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Broker.BrokerRowChanged -= new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Broker.BrokerRowDeleted -= new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowChanged -= new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowDeleted -= new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
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
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the viewer is now closed.
			OnEndCloseViewer();

		}

		/// <summary>
		/// Clear the viewer and display the headings of the document.
		/// </summary>
		private void OpenDocumentCommand(int blotterId)
		{

			try
			{

				// This identifies the blotter this document belongs to.
				this.BlotterId = blotterId;

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the blotter used to define the stylesheets used by this execution document.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null)
					throw new Exception(String.Format("Blotter {0} has been deleted", blotterId));
				
				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsExecutionStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.ExecutionStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Execution stylesheet", blotterId));

				// As an optimization, don't reload the stylesheet if the prevous document used the same stylesheet. This
				// will save a few hundred milliseconds when scrolling through similar documents.
				if (this.IsStylesheetChanged || this.StylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case it is changed while we're viewing it.  The event handler will
					// use this id to determine if an incoming stylesheet will trigger a refresh of the document.
					this.StylesheetId = stylesheetRow.StylesheetId;

					// Load the stylesheet from the table.
					this.xslTransform = new XslTransform();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					this.xslTransform.Load(xmlTextReader, (XmlResolver)null, (Evidence)null);

				}
				
				// Create an empty execution document.
				ExecutionDocument executionDocument = new ExecutionDocument();

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed into
				// the spreadsheet control directly, when there is a major format change, or will be updated incrementally
				// when only the content has changed.
				XmlDocument spreadsheetDocument = new XmlDocument();
				spreadsheetDocument.Load(xslTransform.Transform(executionDocument, (XsltArgumentList)null, (XmlResolver)null));

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				InvokeCreateAddressMap(spreadsheetDocument);

				// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet
				// control.  We can invoke the foreground thread to move the data into the control.
				SetDocument(spreadsheetDocument);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{
					
				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the document is now open.
			OnEndOpenDocument();

		}

		/// <summary>
		/// Opens a Block Order in the Execution Viewer.
		/// </summary>
		/// <param name="blockOrderId">The block order identifier</param>
		private void OpenBlockOrderCommand(int blockOrderId)
		{

			// This will force an empty placement and execution document to appear until we get some data.  It's the same
			// as forcing a drawing of the headers only.
			this.blockOrderId = blockOrderId;

			// This block will attach the appraisal viewer to the data model changes.
			try
			{

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the block order.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null)
					throw new Exception(String.Format("Block Order {0} has been deleted", blockOrderId));

				// Find out if there is a security associated with this block order.
				ClientMarketData.SecurityRow securityRow = blockOrderRow.SecurityRowByFKSecurityBlockOrderSecurityId;
				foreach (ClientMarketData.CurrencyRow currencyRow in securityRow.GetCurrencyRows())
					this.countryId = currencyRow.SecurityRow.CountryId;
				this.securityTypeCode = securityRow.SecurityTypeCode;

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// This will reset the viewer to the home position when it opens.
			this.ResetViewer = true;
			
			// Draw the document without any temporary executions.
			CommandQueue.Execute(new ThreadHandler(DrawCommand), blockOrderId, this.executionSet);
		
		}

		/// <summary>
		/// Creates and displays an execution report.
		/// </summary>
		/// <param name="blockOrderId">The block order of the executions.</param>
		/// <param name="execution">Optional temporary execution.</param>
		private void DrawCommand(params object[] argument)
		{

			// Extract the command arguments.
			int blockOrderId = (int)argument[0];
			ExecutionSet executionSet = (ExecutionSet)argument[1];
			
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the block order still exists.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null && blockOrderId != 0)
					throw new ArgumentException("The block order has been deleted.", blockOrderId.ToString());

				// Create the execution document.
				ExecutionDocument executionDocument = new ExecutionDocument(blockOrderId, executionSet);

#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Dump the DOM to a file.  This is very useful when debugging.
					executionDocument.Save("executionDOM.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// Empty argument list for the XLS translation.  This is just a reminder in case, at a latter time, we 
				// want to add argument to the report generation.
				XsltArgumentList xsltArgumentList = new XsltArgumentList();

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed into
				// the spreadsheet control directly, when there is a major format change, or will be updated incrementally
				// when only the content has changed.
				XmlDocument spreadsheetDocument = new XmlDocument();
				spreadsheetDocument.Load(xslTransform.Transform(executionDocument, xsltArgumentList, (XmlResolver)null));

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				InvokeCreateAddressMap(spreadsheetDocument);

#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Write out the spreadsheet document when debugging.
					spreadsheetDocument.Save("ExecutionSpreadsheet.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet
				// control.  We can invoke the foreground thread to move the data into the control.
				SetDocument(spreadsheetDocument);

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
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The arguments for the event handler.</param>
		private void BlotterRowChangeEvent(object sender, ClientMarketData.BlotterRowChangeEvent blotterRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the broker table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="brokerRowChangeEvent">The arguments for the event handler.</param>
		private void BrokerRowChangeEvent(object sender, ClientMarketData.BrokerRowChangeEvent brokerRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void ExecutionRowChangeEvent(object sender, ClientMarketData.ExecutionRowChangeEvent executionRowChangeEvent)
		{

			// Don't attempt to modify an individual row if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Shorthand notation for the changed row.
				ClientMarketData.ExecutionRow executionRow = executionRowChangeEvent.Row;

				// Find the BlockOrderId and ExecutionId using the current record, or the original record if the record has been deleted.
				int blockOrderId = (int)executionRow[ClientMarketData.Execution.BlockOrderIdColumn,
					executionRowChangeEvent.Action == DataRowAction.Delete ? DataRowVersion.Original : DataRowVersion.Current];
				int executionId = (int)executionRow[ClientMarketData.Execution.ExecutionIdColumn,
					executionRowChangeEvent.Action == DataRowAction.Delete ? DataRowVersion.Original : DataRowVersion.Current];

				// If the execution doesn't belong to the block in this document, then there's nothing to update.
				if (this.BlockOrderId != blockOrderId)
					return;
	
				// If the execution isn't part of this document, then the entire document has to be redrawn to deal with the
				// structure change.  Otherwise, a more sugical appraoch is to just update the changed fields.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(executionId);
				if (localMapRow == null)
					this.IsRefreshNeeded = true;
				else
					Invoke(this.setExecutionDelegate, new object[] {new GlobalExecution(executionRow)});

			}

		}
		
		/// <summary>
		/// Handles a changed stylesheet in the data model.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="stylesheetRowChangeEvent">The event argument.</param>
		private void StylesheetRowChangeEvent(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// This will make it easier to operate on the changed record.
			ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

			// Reopen the document if the style sheet has changed.  Note that the stylesheet is associated with the 
			// blotter, so the document has to be opened first, to re-install the stylesheet, then the block order can be
			// displayed with the new template.
			if (this.IsDocumentOpen && stylesheetRow.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.BlotterId);
				OpenBlockOrder(this.BlockOrderId);
			}

		}

		/// <summary>
		/// Arbiter of commands to be executed on the command thread.
		/// </summary>
		/// <param name="command">The command</param>
		/// <param name="key">A command specific identifier for the object of the command.</param>
		/// <param name="argument">Command specific argument, that is, additional data used to execute the command.</param>
		protected override void ThreadHandler(int command, object key, object argument)
		{

			// The most likely errors will be parsing errors.
			try
			{

				// This section will parse the key element and argument, then call a method to handle the command.  There is also some
				// basic field validation that takes place here.
				switch (command)
				{

					case Command.OpenViewer:

						// Open the Execution Viewer
						OpenViewerCommand();
						break;

					case Command.CloseViewer:

						// Close the Execution Viewer
						CloseViewerCommand();
						break;

					case Command.OpenDocument:

						// Opens the Document in the Execution Viewer
						OpenDocumentCommand((int)key);
						break;

					case Command.CloseDocument:

						// Close the Document
						OnEndCloseDocument();
						break;

					case Command.Draw:

						// Draw the Document in the Viewer
						ExecutionDrawArgs executionDrawArgs = (ExecutionDrawArgs)argument;
						CommandQueue.Execute(new ThreadHandler(DrawCommand), (int)key, executionDrawArgs.ExecutionSet);
						break;

					case Command.OpenBlockOrder:

						// Opens a block order in the Execution Viewer
						OpenBlockOrderCommand((int)key);
						break;

					case Command.Initialize:

						// Initialize a new row
						InitializeExecution((decimal)argument);
						break;

					case Command.UpdateLocal:

						// Update the server with a local execution.
						UpdateLocalExecution((int)key);
						break;
					
					case Command.UpdateGlobal:

						// Update the server with a global execution.
						UpdateGlobalExecution((int)key);
						break;

					case Command.DeleteLocal:

						// Delete a local execution.
						DeleteLocalExecution((int)key);
						break;

					case Command.DeleteGlobal:

						// Delete an execution from the server.
						DeleteGlobalExecution((int)key);
						break;
					
					case Command.ClearLocal:

						// Delete an placement from the server.
						ClearLocalExecution();
						break;

					case Command.SetLocalBrokerSymbol:
					
						// Local Broker Symbol
						if (argument == null)
						{
							SelectCell(true, (int)key, ExecutionColumnType.BrokerSymbol);
							MessageBox.Show("Please enter a valid broker symbol", "Quasar Error");
						}
						else
							SetLocalBrokerSymbol((int)key, (int)argument);

						break;

					case Command.SetGlobalBrokerSymbol:
					
						// Global Broker Symbol
						if (argument == null)
						{
							SelectCell(false, (int)key, ExecutionColumnType.BrokerSymbol);
							MessageBox.Show("Please enter a valid broker symbol", "Quasar Error");
						}
						else
							SetGlobalBrokerSymbol((int)key, (int)argument);

						break;

					case Command.SetLocalQuantity:

						// Local Quantity
						SetLocalQuantity((int)key, Quantity.Parse((string)argument));
						break;

					case Command.SetGlobalQuantity:
					
						// Global Quantity
						SetGlobalQuantity((int)key, Quantity.Parse((string)argument));
						break;

					case Command.SetLocalPrice:

						// Local Price
						SetLocalPrice((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalPrice:
					
						// Global Price
						SetGlobalPrice((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalCommission:

						// Local Commission
						SetLocalCommission((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalCommission:
					
						// Global Commission
						SetGlobalCommission((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalAccruedInterest:

						// Local AccruedInterest
						SetLocalAccruedInterest((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalAccruedInterest:
					
						// Global AccruedInterest
						SetGlobalAccruedInterest((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalUserFee0:

						// Local UserFee0
						SetLocalUserFee0((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee0:
					
						// Global UserFee0
						SetGlobalUserFee0((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalUserFee1:

						// Local UserFee1
						SetLocalUserFee1((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee1:
					
						// Global UserFee1
						SetGlobalUserFee1((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalUserFee2:

						// Local UserFee2
						SetLocalUserFee2((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee2:
					
						// Global UserFee2
						SetGlobalUserFee2((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalUserFee3:

						// Local UserFee3
						SetLocalUserFee3((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee3:
					
						// Global UserFee3
						SetGlobalUserFee3((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetLocalTradeDate:
					
						// Local Trade Date
						SetLocalTradeDate((int)key, Convert.ToDateTime(argument));
						break;

					case Command.SetGlobalTradeDate:
					
						// Global Trade Date
						SetGlobalTradeDate((int)key, Convert.ToDateTime(argument));
						break;

					case Command.SetLocalSettlementDate:
					
						SetLocalSettlementDate((int)key, Convert.ToDateTime(argument));
						break;

					case Command.SetGlobalSettlementDate:
					
						SetGlobalSettlementDate((int)key, Convert.ToDateTime(argument));
						break;

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Set the broker id of a local execution record based on the symbol.
		/// </summary>
		/// <param name="executionId">Identifer of the execution.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetLocalBrokerSymbol(int executionId, int brokerId)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Lock the global tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BrokerLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Local Execution {0} not found.", executionId));

				// Update the quantity in the local table.
				executionRow.BrokerId = brokerId;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.BrokerId = brokerId;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

				// Release the global locks.
				if (ClientMarketData.BrokerLock.IsWriterLockHeld) ClientMarketData.BrokerLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectLock.IsWriterLockHeld) ClientMarketData.ObjectLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
	
		/// <summary>
		/// Set the broker id of a global execution record based on a symbol.
		/// </summary>
		/// <param name="executionId">Identifer of the execution.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetGlobalBrokerSymbol(int executionId, int brokerId)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.BrokerId = brokerId;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.BrokerId = brokerId;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the global locks.
				ReleaseGlobalExecution();
				
			}

		}

		/// <summary>
		/// Set the quantity of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="quantity">Quantity executed.</param>
		private void SetLocalQuantity(int executionId, decimal quantity)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the quantity
				executionRow.Quantity = quantity;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.Quantity = executionRow.Quantity;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the quantity of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="quantity">Quantity executed.</param>
		private void SetGlobalQuantity(int executionId, decimal quantity)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Quantity = quantity;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.Quantity = executionRow.Quantity;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}
		
		/// <summary>
		/// Set the price of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="price">Price executed.</param>
		private void SetLocalPrice(int executionId, decimal price)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the price
				executionRow.Price = price;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.Price = executionRow.Price;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the price of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="price">Price executed.</param>
		private void SetGlobalPrice(int executionId, decimal price)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Price = price;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.Price = executionRow.Price;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}
		
		/// <summary>
		/// Set the commission of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="commission">Commission executed.</param>
		private void SetLocalCommission(int executionId, decimal commission)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the commission
				executionRow.Commission = commission;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.Commission = executionRow.Commission;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the commission of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="commission">Commission executed.</param>
		private void SetGlobalCommission(int executionId, decimal commission)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Commission = commission;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.Commission = executionRow.Commission;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the accruedInterest of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="accruedInterest">Accrued Interest.</param>
		private void SetLocalAccruedInterest(int executionId, decimal accruedInterest)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the accruedInterest
				executionRow.AccruedInterest = accruedInterest;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.AccruedInterest = executionRow.AccruedInterest;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the accruedInterest of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="accruedInterest">Accrued Interest.</param>
		private void SetGlobalAccruedInterest(int executionId, decimal accruedInterest)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.AccruedInterest = accruedInterest;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.AccruedInterest = executionRow.AccruedInterest;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the userFee0 of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee0">Accrued Interest.</param>
		private void SetLocalUserFee0(int executionId, decimal userFee0)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the userFee0
				executionRow.UserFee0 = userFee0;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.UserFee0 = executionRow.UserFee0;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the userFee0 of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee0">Accrued Interest.</param>
		private void SetGlobalUserFee0(int executionId, decimal userFee0)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee0 = userFee0;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.UserFee0 = executionRow.UserFee0;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the userFee1 of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee1">Accrued Interest.</param>
		private void SetLocalUserFee1(int executionId, decimal userFee1)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the userFee1
				executionRow.UserFee1 = userFee1;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.UserFee1 = executionRow.UserFee1;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the userFee1 of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee1">Accrued Interest.</param>
		private void SetGlobalUserFee1(int executionId, decimal userFee1)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee1 = userFee1;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.UserFee1 = executionRow.UserFee1;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the userFee2 of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee2">Accrued Interest.</param>
		private void SetLocalUserFee2(int executionId, decimal userFee2)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the userFee2
				executionRow.UserFee2 = userFee2;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.UserFee2 = executionRow.UserFee2;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the userFee2 of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee2">Accrued Interest.</param>
		private void SetGlobalUserFee2(int executionId, decimal userFee2)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee2 = userFee2;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.UserFee2 = executionRow.UserFee2;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the userFee3 field of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee3">Accrued Interest.</param>
		private void SetLocalUserFee3(int executionId, decimal userFee3)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the userFee3
				executionRow.UserFee3 = userFee3;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.UserFee3 = executionRow.UserFee3;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the userFee3 field of a global execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="userFee3">Accrued Interest.</param>
		private void SetGlobalUserFee3(int executionId, decimal userFee3)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee3 = userFee3;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.UserFee3 = executionRow.UserFee3;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Set the trade date field of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="tradeDate">Trade Date of the execution.</param>
		private void SetLocalTradeDate(int executionId, DateTime tradeDate)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the tradeDate
				executionRow.TradeDate = tradeDate;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.TradeDate = executionRow.TradeDate;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the trade date of an execution in the global tables.
		/// </summary>
		/// <param name="executionId">Identifier of the execution.</param>
		/// <param name="tradeDate">Trade date of the execution.</param>
		private void SetGlobalTradeDate(int executionId, DateTime tradeDate)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.TradeDate = tradeDate;

				// Now that the data model reflects the changed record, we will update the field in the
				// spreadsheet. A delegate must be used because we can't modify the spreadsheet from the
				// background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.TradeDate = executionRow.TradeDate;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}
		
		/// <summary>
		/// Set the trade date field of a local execution.
		/// </summary>
		/// <param name="executionId">Identifier of the record.</param>
		/// <param name="settlementDate">Trade Date of the execution.</param>
		private void SetLocalSettlementDate(int executionId, DateTime settlementDate)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow =
					this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the settlementDate
				executionRow.SettlementDate = settlementDate;

				// Now that the local data model reflects the changed record, we will update the field in the spreadsheet.
				// A delegate must be used because we can't modify the spreadsheet from the background.
				LocalExecution localExecution = new LocalExecution(executionRow.ExecutionId);
				localExecution.SettlementDate = executionRow.SettlementDate;
				Invoke(this.setExecutionDelegate, new object[] {localExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

			}

		}
		
		/// <summary>
		/// Set the settlement date of an execution in the global tables.
		/// </summary>
		/// <param name="executionId">Identifier of the execution.</param>
		/// <param name="settlementDate">Trade date of the execution.</param>
		private void SetGlobalSettlementDate(int executionId, DateTime settlementDate)
		{

			try
			{

				// Lock the global tables.
				LockGlobalExecution();

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.SettlementDate = settlementDate;

				// Now that the data model reflects the changed record, we will update the field in the
				// spreadsheet. A delegate must be used because we can't modify the spreadsheet from the
				// background.
				GlobalExecution globalExecution = new GlobalExecution(executionRow.ExecutionId);
				globalExecution.SettlementDate = executionRow.SettlementDate;
				Invoke(this.setExecutionDelegate, new object[] {globalExecution});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalExecution();

			}

		}

		/// <summary>
		/// Selects a default value for a broker.
		/// </summary>
		/// <returns>The default broker id for this execution, null if the broker is not determined yet.</returns>
		private object GetDefaultBrokerId()
		{

			ExecutionSet.BlockOrderRow localBlockOrder = this.executionSet.BlockOrder.FindByBlockOrderId(this.BlockOrderId);
			if (localBlockOrder != null)
			{

				// We try the executions first.  Generally we'll be taking several executions over the phone from the 
				// same broker, so this appears to be a natural default.  If no executions exist, then we try the
				// placements and use the last broker on the last placement.
				int executionsCount = localBlockOrder.GetExecutionRows().Length;
				if (executionsCount > 0)
				{
					ExecutionSet.ExecutionRow localExecution = localBlockOrder.GetExecutionRows()[executionsCount - 1];
					return localExecution.IsBrokerIdNull() ? (object)null : (object)localExecution.BrokerId;
				}

			}
				
			// The block order is the starting point to search for a broker.  We assume some sort of reverse sequencing
			// to the executions.  First, we'll see if there are any executions, chances are best that it's the same broker
			// from the last execution.  If there are no executions, then the best chance is that the report is from the
			// most recent placement made to a broker.
			ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(this.BlockOrderId);
			if (blockOrderRow != null)
			{

				// We try the executions first.  Generally we'll be taking several executions over the phone from the 
				// same broker, so this appears to be a natural default.  If no executions exist, then we try the
				// placements and use the last broker on the last placement.
				int executionsCount = blockOrderRow.GetExecutionRows().Length;

				// If there are no executions, we'll look to see if there are any placements.
				if (executionsCount == 0)
				{

					// If a last placement exists, then use the broker symbol associated with it as a default.
					int placementsCount = blockOrderRow.GetPlacementRows().Length;
					if (placementsCount > 0)
					{
						ClientMarketData.PlacementRow PlacementRow =
							blockOrderRow.GetPlacementRows()[placementsCount - 1];
						return PlacementRow.BrokerRow.BrokerId;
					}

				}

				if (executionsCount > 0)
				{

					// If one or more executions have been entered against this block, use the last execution entered for 
					// the default broker symbol.
					ClientMarketData.ExecutionRow ExecutionRow = blockOrderRow.GetExecutionRows()[executionsCount - 1];
					return ExecutionRow.BrokerId;

				}

			}

			// This indicates that the broker for the execution can't be determined.
			return null;

		}

		/// <summary>
		/// Updates a local execution and spawns a thread to update the server.
		/// </summary>
		/// <param name="executionId">Primary identifier of the record.</param>
		private void UpdateLocalExecution(int executionId)
		{

			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Lock the global tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the record in the local table.
				ExecutionSet.ExecutionRow executionRow = this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution Identifier '{0}' doesn't exist", executionId));

				// Execution with zero quantity are noise.  We allow them to hang around as placeholders and we'll 
				// delete them if they're still around when we leave this viewer, but we don't want to try to add them to
				// the database.
				if (executionRow.Quantity == 0.0M)
					return;

				// Create a record from the local tables.  This record will capture the state of the execution at the 
				// time this thread was run.  Then call the foreground to fill it in with any of the calculated fields
				// from the stylesheet.
				Execution execution = new LocalExecution(executionRow);
				Invoke(this.getExecutionDelegate, new object[] {execution});

				// The rowVersion on a local table tells us how many times we've changed a local record.  The first time
				// through this method we should generate an 'Insert' operation for the server.  Each time after that, an 
				// 'Update' operation should be generated with the global execution id, which may not be available at the
				// time we initiate this operation (due to the lag in response or a connection being temporarily
				// unavailable).
				if (executionRow.RowVersion++ == 1)
				{

					// Add the record to the server 					
					this.ServiceQueue.Enqueue(new ThreadHandler(InsertLocalExecutionThread), execution);

				}
				else
				{

					// If the rowVersion is anything other than the initial value, we will update the record instead of
					// inserting it.
					this.ServiceQueue.Enqueue(new ThreadHandler(UpdateLocalExecutionThread), execution);

				}

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the local table
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();

				// Release the global locks.
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="argument">Thread initialization parameters.</param>
		private void InsertLocalExecutionThread(params object[] argument)
		{

			// Extract the initialization parameters.
			Execution execution = (Execution)argument[0];

			ExecutionSet.ExecutionRow localExecution = null;

			try
			{

				// Lock the local tables
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				localExecution = this.executionSet.Execution.FindByExecutionId(execution.ExecutionId);
				if (localExecution == null)
					throw new Exception(String.Format("Local Execution {0} not found.", execution.ExecutionId));

				// Remove the local version of the record.
				localExecution.Delete();
				localExecution.AcceptChanges();

				// Call the web service to add the new execution.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
				remoteMethod.Parameters.Add("blockOrderId", execution.BlockOrderId);
				remoteMethod.Parameters.Add("brokerId", execution.BrokerId);
				remoteMethod.Parameters.Add("quantity", execution.Quantity);
				remoteMethod.Parameters.Add("price", execution.Price);
				remoteMethod.Parameters.Add("commission", execution.Commission);
				remoteMethod.Parameters.Add("accruedInterest", execution.AccruedInterest);
				remoteMethod.Parameters.Add("userFee0", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee1", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee2", execution.UserFee0);
				remoteMethod.Parameters.Add("tradeDate", execution.TradeDate);
				remoteMethod.Parameters.Add("settlementDate", execution.SettlementDate);
				ClientMarketData.Execute(remoteBatch);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the local tables.
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseLock();

			}

		}

		/// <summary>
		/// Updates the execution record on the server.
		/// </summary>
		/// <param name="argument">Thread initialization parameters.</param>
		private void UpdateGlobalExecutionThread(params object[] argument)
		{

			// Extract the initialization parameters.
			Execution execution = (Execution)argument[0];

			try
			{

				// Call the web service to update the execution record.  If successful, it will return the new rowVersion
				// and the modified time and user of record.
				// Call the web service to add the new execution.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
				remoteMethod.Parameters.Add("executionId", execution.ExecutionId);
				remoteMethod.Parameters.Add("blockOrderId", execution.BlockOrderId);
				remoteMethod.Parameters.Add("brokerId", execution.BrokerId);
				remoteMethod.Parameters.Add("rowVersion", execution.RowVersion);
				remoteMethod.Parameters.Add("quantity", execution.Quantity);
				remoteMethod.Parameters.Add("price", execution.Price);
				remoteMethod.Parameters.Add("commission", execution.Commission);
				remoteMethod.Parameters.Add("accruedInterest", execution.AccruedInterest);
				remoteMethod.Parameters.Add("userFee0", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee1", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee2", execution.UserFee0);
				remoteMethod.Parameters.Add("tradeDate", execution.TradeDate);
				remoteMethod.Parameters.Add("settlementDate", execution.SettlementDate);
				ClientMarketData.Execute(remoteBatch);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="argument">Thread initialization parameters.</param>
		private void UpdateLocalExecutionThread(params object[] argument)
		{

			// Extract the initialization parameters.
			Execution execution = (Execution)argument[0];

			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the local tables
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);
				
				// Lock the global tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				ExecutionSet.ExecutionMapRow executionMapRow = this.executionSet.ExecutionMap.FindByLocalId(execution.ExecutionId);
				if (executionMapRow == null)
					throw new Exception(String.Format("Mapping for local execution id {0} doesn't exist", execution.ExecutionId));

				// Find the execution record in the global data tables.
				ClientMarketData.ExecutionRow globalExecution = ClientMarketData.Execution.FindByExecutionId(executionMapRow.GlobalId);
				if (globalExecution == null)
					throw new Exception(String.Format("Execution {0} not found.", execution.ExecutionId));

				// These parameters are returned from the call to update the record and are used to track the
				// execution until we get a refresh from the server.
				int modifiedLoginId = 0;
				long rowVersion = execution.RowVersion;
				DateTime modifiedTime = DateTime.MinValue;

				// Call the web service to update the execution record.  If successful, it will return the new rowVersion
				// and the modified time and user of record.
				// Call the web service to add the new execution.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
				remoteMethod.Parameters.Add("executionId", execution.ExecutionId);
				remoteMethod.Parameters.Add("blockOrderId", execution.BlockOrderId);
				remoteMethod.Parameters.Add("brokerId", execution.BrokerId);
				remoteMethod.Parameters.Add("rowVersion", execution.RowVersion);
				remoteMethod.Parameters.Add("quantity", execution.Quantity);
				remoteMethod.Parameters.Add("price", execution.Price);
				remoteMethod.Parameters.Add("commission", execution.Commission);
				remoteMethod.Parameters.Add("accruedInterest", execution.AccruedInterest);
				remoteMethod.Parameters.Add("userFee0", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee1", execution.UserFee0);
				remoteMethod.Parameters.Add("userFee2", execution.UserFee0);
				remoteMethod.Parameters.Add("tradeDate", execution.TradeDate);
				remoteMethod.Parameters.Add("settlementDate", execution.SettlementDate);
				ClientMarketData.Execute(remoteBatch);

				// Update the internal data model with the new data.  Note that we update the rowVersion, modified time
				// and login id from the data returned from the server. If the server finished the update operation
				// without an error, we can assume that the members of the 'execution' record have been accepted.  We will
				// update the data model from that record to give an accurate state until the next refresh.
				globalExecution.RowVersion = rowVersion;
				globalExecution.BlockOrderId = execution.BlockOrderId;
				globalExecution.BrokerId = execution.BrokerId;
				globalExecution.ExecutionId = execution.ExecutionId;
				globalExecution.Quantity = execution.Quantity;
				globalExecution.Price = execution.Price;
				globalExecution.Commission = execution.Commission;
				globalExecution.AccruedInterest = execution.AccruedInterest;
				globalExecution.UserFee0 = execution.UserFee0;
				globalExecution.UserFee1 = execution.UserFee1;
				globalExecution.UserFee2 = execution.UserFee2;
				globalExecution.UserFee3 = execution.UserFee3;
				globalExecution.TradeDate = execution.TradeDate;
				globalExecution.SettlementDate = execution.SettlementDate;
				globalExecution.ModifiedTime = modifiedTime;
				globalExecution.ModifiedLoginId = modifiedLoginId;
				globalExecution.AcceptChanges();

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the local tables.
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseLock();

				// Release the global tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="argument">Thread initialization parameters.</param>
		private void DeleteExecutionThread(params object[] argument)
		{

			// Extract the parameters used by this thread.
			int executionId = (int)argument[0];
			long rowVersion = (long)argument[1];

			// Make sure that the table locks are released.		
			try
			{

				// Call the Web Service to delete the record.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Delete");
				remoteMethod.Parameters.Add("executionId", executionId);
				remoteMethod.Parameters.Add("rowVersion", rowVersion);
				ClientMarketData.Execute(remoteBatch);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}

		}

		private void UpdateGlobalExecution(int executionId)
		{

			try
			{

				// Lock the tables needed.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.ExecutionRow executionRow = ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution Identifier '{0}' doesn't exist", executionId));

				Execution execution = new GlobalExecution(executionRow);
				Invoke(this.getExecutionDelegate, new object[] {execution});

				// If the execution record has been modified, launch a service thread that will modify the record on the server.
				if (executionRow.RowState != DataRowState.Unchanged)
					ServiceQueue.Execute(new ThreadHandler(UpdateGlobalExecutionThread), execution);

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		private void DeleteLocalExecution(int executionId)
		{


			// Make sure that the table locks are released.		
			try
			{

				// Lock the local table
				this.executionSetLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the local tables.
				ExecutionSet.ExecutionRow executionRow = this.executionSet.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} doesn't exist", executionRow.ExecutionId.ToString()));

				// Purge the row from the temporary tables.
				executionRow.Delete();
				executionRow.AcceptChanges();

				// Redraw the document without the new record.
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.executionSet);

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (this.executionSetLock.IsWriterLockHeld) this.executionSetLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Deletes an execution from the shared data model.
		/// </summary>
		/// <param name="executionId">Primary identifier of the execution.</param>
		private void DeleteGlobalExecution(int executionId)
		{


			// Make sure that the table locks are released.		
			try
			{

				// Lock the table while we make our changes.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the global tables.
				ClientMarketData.ExecutionRow executionRow = ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} doesn't exist", executionRow.ExecutionId.ToString()));

				// The rowVersion is used for optimistic concurrency checking.
				long rowVersion = executionRow.RowVersion;

				// Queue up a request for another, longer lasting thread to take care of the interaction with the server.
				this.ServiceQueue.Enqueue(new ThreadHandler(DeleteExecutionThread), executionId, rowVersion);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Deletes the current row from the viewer.
		/// </summary>
		public void DeleteActiveRow()
		{

			// Extract the current row and column.
			CellAddress activeCellAddress = GetActiveCellAddress();
			int rowIndex = activeCellAddress.RowIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.RowType);
			int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			// Delete a local execution
			if (rowType == ExecutionRowType.LocalExecution)
			{

				// Get the execution id from the selected row.
				int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
				int executionId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					executionIdColumnIndex)));

				// Execute a command on the worker thread to delete the local record.
				ExecuteCommand(Command.DeleteLocal, executionId);

				// This will prevent any attempt to update the deleted record when the selection changes.
				this.IsExecutionChanged = false;
				this.IsExecutionSelected = false;

				// Move the selection to the previous execution.
				CommandMoveUp();

			}

			// Delete a global execution.
			if (rowType == ExecutionRowType.GlobalExecution)
			{

				// Get the execution id from the selected row.
				int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
				int executionId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					executionIdColumnIndex)));

				// Execute a command on the worker thread to delete the global record.
				ExecuteCommand(Command.DeleteGlobal, executionId);

				// This will prevent any attempt to update the deleted record when the selection changes.
				this.IsExecutionChanged = false;
				this.IsExecutionSelected = false;

				// Move the selection to the previous execution.
				CommandMoveUp();

			}
	
		}

		/// <summary>
		/// Handles an escape command.
		/// </summary>
		public override void CommandEscape()
		{

			// Clear the local table of any unused records.  This must be done in the background to avoid deadlocks.  This
			// command will also redraw the document if anything has change that would impact the viewer.
			ExecuteCommand(Command.ClearLocal);

			// Give the focus back to whoever wants it.
			OnReleaseFocus();

		}

		/// <summary>
		/// Clears the local table of unused executions.
		/// </summary>
		/// <returns>true to indicate that unused records were removed.</returns>
		private void ClearLocalExecution()
		{

			// This will be set to true if the local table is modified.  This can be used by the calling method to
			// indicate that a redraw of the document is needed.
			bool isChanged = false;

			try
			{

				// Lock the local tables
				this.executionSetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the local block order that is used to group the local executions in this window.  Then cycle through
				// all those local records and delete them if they are unused.
				ExecutionSet.BlockOrderRow blockOrderRow =
					this.executionSet.BlockOrder.FindByBlockOrderId(this.BlockOrderId);
				if (blockOrderRow != null)
					foreach (ExecutionSet.ExecutionRow executionRow in blockOrderRow.GetExecutionRows())
					{

						// We'll keep the local record if someone bothered to enter a quantity.  The 'isChanged' flag will
						// let the caller know that the data behind the document has changed.
						if (executionRow.Quantity == 0.0M)
						{
							executionRow.Delete();
							executionRow.AcceptChanges();
							isChanged = true;
						}

					}

			}
			finally
			{

				// Release the local tables.
				if (this.executionSetLock.IsReaderLockHeld) this.executionSetLock.ReleaseReaderLock();

			}

			// Return an indicator that shows whether the local table was modified by this method.  This is generally used
			// to trigger a redraw of the document when the focus is leaving.
			if (isChanged)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.executionSet);

		}

		/// <summary>
		/// Initializes a new execution.
		/// </summary>
		/// <param name="quantity">Initial quantity for the execution.</param>
		private void InitializeExecution(decimal quantity)
		{

			// This record will be filled in and passed on to the foreground when it comes time to selecting a cell in the
			// new line in the control window.  The act of selecting a row and a cell requires access to the 'AddressMap'
			// structure which can only be accessed in the foreground.  This record acts as a method to pass informaton
			// from the background to the foreground.
			LocalExecution localExecution = null;

			try
			{

				// Lock the local tables
				this.executionSetLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Lock the tables needed.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.HolidayLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Search for a default broker.  A directed order will set this value.  If the order isn't directed, then the 
				// most recent placement will define the value.  If there are no placements, then the last execution is used.
				// If none of these are correct, well, the user can put his own d*mned broker symbol in.
				object brokerId = GetDefaultBrokerId();

				// The local tables maintain a concept of block orders.  This makes it easy to find and group the local
				// executions.  This will add a block to the local tables if it doesn't exist.  After the block is 
				// created, executions can be associated with a 'block id'.
				ExecutionSet.BlockOrderRow blockOrderRow = this.executionSet.BlockOrder.FindByBlockOrderId(this.BlockOrderId);
				if (blockOrderRow == null)
				{
					blockOrderRow = this.executionSet.BlockOrder.AddBlockOrderRow(this.BlockOrderId);
					blockOrderRow.AcceptChanges();
				}

				// Initialize the new record.  Note that we use a quantity value that can be initialized from the outside.
				// This was done because there were no other methods to pass the value in.  It was highly desirable to 
				// have a single point where the new executions are created.  This ended up being the 'Select' logic,
				// because the user could 'Select' the placeholder row.  Also note that the execution id for this element
				// is an artificial construct.  We use negative numbers to keep the new executions distinct from the read
				// ones on the middle tier.  However, it's useful for constructing the report to have temporary execution
				// records until the server gets around to creating the records and handing us back a real id to use.
				ExecutionSet.ExecutionRow executionRow = this.executionSet.Execution.NewExecutionRow();
				executionRow.BlockOrderId = this.BlockOrderId;
				if (brokerId != null) executionRow.BrokerId = (int)brokerId;
				executionRow.Quantity = quantity;
				executionRow.TradeDate = Trading.TradeDate(this.countryId, this.securityTypeCode, DateTime.Now);
				executionRow.SettlementDate = Trading.SettlementDate(this.countryId, this.securityTypeCode,
					executionRow.TradeDate);
				this.executionSet.Execution.AddExecutionRow(executionRow);
				executionRow.AcceptChanges();

				// A record needs to be passed on to the foreground so the sheet can be updated.  The record needs to be
				// created while the tables are still locked so ancillary tables can be used.  Once the structure has been
				// filled in, it can be used by the foreground without having to reference the tables.
				localExecution = new LocalExecution(executionRow);

			}
			finally
			{

				// Release the local tables.
				if (this.executionSetLock.IsReaderLockHeld) this.executionSetLock.ReleaseReaderLock();
				
				// Release the tables.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.HolidayLock.IsReaderLockHeld) ClientMarketData.HolidayLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// At this point a blank execution has been created in the local table.  Redraw the document with the local
			// records.
			DrawCommand(this.BlockOrderId, this.executionSet);

			// Call the foreground to select a cell based in the new record.
			if (localExecution != null)
				Invoke(this.initializeRowDelegate, new object[] {localExecution});

		}

		/// <summary>
		/// Selects the starting cell for a new execution line.
		/// </summary>
		/// <param name="execution">The data in the new line.</param>
		private void InitializeRow(Execution execution)
		{

			// After the row had been initialized, we want to pick the best column to begin getting data.  The address map
			// was reconstructed in the 'Draw' method above, so we can us the map to find the record we just entered.
			AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(execution.ExecutionId);
			if (localMapRow == null)
				return;

			// Find the columns used for the initial selection based on the column type.
			int priceColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Price);
			int quantityColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Quantity);
			int brokerSymbolColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.BrokerSymbol);

			// The order of selection goes from Time In Force Instruction, to quantity to broker.  If data has been
			// provided for the broker and quantity, the cursor will be left in the TIF instruction column.
			int columnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.Price);
			if (quantityColumnIndex != SpreadsheetColumn.DoesNotExist && execution.Quantity == 0.0M)
				columnIndex = quantityColumnIndex;
			if (brokerSymbolColumnIndex != SpreadsheetColumn.DoesNotExist && execution.IsBrokerIdNull())
				columnIndex = brokerSymbolColumnIndex;

			// This will leave the selection in the the most natural cell for the user based on what fields have already
			// been filled in by defaults.
			Select(new CellAddress(this.DocumentVersion, localMapRow.RowIndex, columnIndex));

		}

		/// <summary>
		/// Called when a cell in the spreadsheet control has finished editing.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnEndEdit(Shadows.Quasar.Common.Controls.EndEditEventArgs e)
		{

			try
			{

				// The main idea of this method is to take the results of the editing and spawn a thread that will handle
				// the new data in the background.  There are some tasks that have to be done in the foreground (Message 
				// Loop Thread), such as operating on the spreadsheet control, and there are other tasks that have to be
				// done in the background (Locking tables and examining the data model).  Here, we do what can be done in
				// the foreground, and construct a command for the background.
				int command = Command.None;
				object key = null;

				// Extract the current row and column from the event argument.  This is the address of the cell where 
				// the editing took place. Note that, because the user can move the focus at any time to another cell,
				// this is not necessarily the address of the currently active cell.
				int rowIndex = e.CellAddress.RowIndex;
				int columnIndex = e.CellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a local record.
				if (rowType == ExecutionRowType.LocalExecution)
				{

					// Get the execution id from the selected row.
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
					key = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						executionIdColumnIndex)));

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{
						case ExecutionColumnType.BrokerSymbol: command = Command.SetLocalBrokerSymbol; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Quantity: command = Command.SetLocalQuantity; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Price: command = Command.SetLocalPrice; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Commission: command = Command.SetLocalCommission; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.AccruedInterest: command = Command.SetLocalAccruedInterest; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee0: command = Command.SetLocalUserFee0; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee1: command = Command.SetLocalUserFee1; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee2: command = Command.SetLocalUserFee2; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee3: command = Command.SetLocalUserFee3; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.TradeDate: command = Command.SetLocalTradeDate; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.SettlementDate: command = Command.SetLocalSettlementDate; this.IsExecutionChanged = true; break;
					}

				}

				// This will create a command to update a global record.
				if (rowType == ExecutionRowType.GlobalExecution)
				{

					// Get the execution id from the selected row.
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
					key = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						executionIdColumnIndex)));

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{
						case ExecutionColumnType.BrokerSymbol: command = Command.SetGlobalBrokerSymbol; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Quantity: command = Command.SetGlobalQuantity; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Price: command = Command.SetGlobalPrice; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.Commission: command = Command.SetGlobalCommission; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.AccruedInterest: command = Command.SetGlobalAccruedInterest; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee0: command = Command.SetGlobalUserFee0; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee1: command = Command.SetGlobalUserFee1; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee2: command = Command.SetGlobalUserFee2; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.UserFee3: command = Command.SetGlobalUserFee3; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.TradeDate: command = Command.SetGlobalTradeDate; this.IsExecutionChanged = true; break;
						case ExecutionColumnType.SettlementDate: command = Command.SetGlobalSettlementDate; this.IsExecutionChanged = true; break;
					}

				}

				// If a command was decoded from the column information above, then execute the command in the background
				// giving the primary key and the data to a thread that will execute outside the message loop thread.
				if (command != Command.None)
					ExecuteCommand(command, key, e.Result);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Called when the focus is given to the viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void ExecutionViewer_Enter(object sender, System.EventArgs e)
		{

			// If the document is open, then interpret the focus coming into the viewer as if a new cell were selected.
			// This is done to force the 'Prompting' action of new executions and a re-selection of existing executions.  
			// The document will be entered one or more times during the initialization, so these events are filtered out
			// because all the document metrics haven't been calculated yet.
			if (this.IsDocumentOpen)
				OnSelectionChange(new SelectionChangeArgs(GetActiveCellAddress()));

		}

		/// <summary>
		/// Called when the focus leaves this viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void ExecutionViewer_Leave(object sender, System.EventArgs e)
		{

			// Make sure that all changes are committed when the focus leaves this viewer.
			CommitChanges();

			// Clear the local table of any unused records.  This must be done in the background to avoid deadlocks.  This
			// command will also redraw the document if anything has change that would impact the viewer.
			ExecuteCommand(Command.ClearLocal);

		}

		/// <summary>
		/// Commits any temporary changes to the selected execution.
		/// </summary>
		private void CommitChanges()
		{

			// Only update the record if it was previously selected.
			if (this.IsExecutionSelected && this.IsExecutionChanged)
			{

				// Local records are handled differenly than global records.  The local records are created and managed 
				// locally until a connection can be established with the server.  Until a global identifier can be
				// associated with the record, it is managed in a different data model.  This will start the process of
				// storing the newly created record to the server.
				if (this.SelectedRowType == ExecutionRowType.LocalExecution)
					ExecuteCommand(Command.UpdateLocal, this.SelectedExecutionId);

				// Global records are modified locally until the entire row is ready to be accepted.  Then a single
				// transaction updates the record.  This starts the task of updating the server using the changes made to
				// the local data model.
				if (this.SelectedRowType == ExecutionRowType.GlobalExecution)
					ExecuteCommand(Command.UpdateGlobal, this.SelectedExecutionId);

				// After the changes are committed, an assumption is made that the record is no longer selected.  This is 
				// usually the way this method will be used.  The record
				this.IsExecutionChanged = false;

			}

		}
		
		/// <summary>
		/// Handles the selection changing from one cell to another.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="selectionChangeArgs"></param>
		private void ExecutionViewer_SelectionChange(object sender, Shadows.Quasar.Viewers.SelectionChangeArgs selectionChangeArgs)
		{

			// Extract the current row and column from the event argument.
			int rowIndex = selectionChangeArgs.CellAddress.RowIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.RowType);
			int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			// The row type defines what action to take based on the selection changing.
			switch (rowType)
			{

				case ExecutionRowType.Unused:

					// If we had previously selected an execution record, see if it needs to be inserted or updated 
					// before we move on to the next record.
					CommitChanges();

					// Indicate that an execution is not selected.
					this.IsExecutionSelected = false;

					break;

				case ExecutionRowType.Placeholder:

					// If we had previously selected an execution record, see if it needs to be inserted or updated before we
					// move on to the next record.
					CommitChanges();

					// It's possible for the placeholder to be selected when an empty execution is drawn, since the 
					// placeholder ends up in the first row.  However, the line shouldn't be initialized until the window
					// gains the focus.  Also, there are times when selecting the placeholder should be ignored, such as
					// when the program is taking care of initializing a row.  In all other cases, when the user clicks on
					// a placeholder row, a line is initialized for them.
					if (this.ContainsFocus && this.isManualSelection)
						ExecuteCommand(Command.Initialize, null, 0.0M);

					// Indicate that an execution is not selected.
					this.IsExecutionSelected = false;

					break;

				case ExecutionRowType.LocalExecution:
				case ExecutionRowType.GlobalExecution:

					// Get the execution identifier from the spreadsheet row.
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(ExecutionColumnType.ExecutionId);
					int executionId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, executionIdColumnIndex)));

					// If the currently selected execution not the same as the previously selected execution, then commit
					// the changes.
					if (this.SelectedRowType != rowType || this.SelectedExecutionId != executionId)
						CommitChanges();

					// This indicates that an execution is selected and saves information about that execution.  These
					// state variables are needed by the ledger style input to determine when to commit the changes to the
					// server.  The row type tells the 'commit' logic whether a local or global record is identified in
					// the 'SelectedExecutionId' member.
					this.IsExecutionSelected = true;
					this.SelectedRowType = rowType;
					this.SelectedExecutionId = executionId;
						 
					break;

			}
	
		}

	}

}
