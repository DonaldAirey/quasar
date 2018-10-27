/*************************************************************************************************************************
*
*	File:			TicketViewer.cs
*	Description:	This control is used to display and manage a execution.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Ticket
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
	public class TicketViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private bool isTicketSelected;
		private int blotterId;
		private int selectedRowType;
		private int selectedExecutionId;
		private Shadows.Quasar.Client.ClientMarketData clientClientMarketData;
		private System.Xml.Xsl.XslTransform xslTransform;
		private System.ComponentModel.IContainer components = null;
		private AddressMap addressMap;
		private delegate void InvokeCreateAddressMapDelegate(XmlDocument xmlDocument);
		private delegate bool TicketDelegate(Ticket execution);
		private delegate bool SelectCellDelegate(int executionId, int columnType);
		private InvokeCreateAddressMapDelegate createAddressMapDelegate;
		private TicketDelegate setTicketDelegate;
		private TicketDelegate getTicketDelegate;
		private SelectCellDelegate selectCellDelegate;
		private ReaderWriterLock ticketSetLock = new ReaderWriterLock();
		private ManualResetEvent handleCreatedEvent;

		/// <summary>
		/// Constructor for the TicketViewer
		/// </summary>
		public TicketViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// Add the debt calculation library to the functions recognized by this spreadsheet control.
			this.ExternalLibraries.Add(new Viewers.DebtLibrary.Debt());

			// An 'id' of zero is reserved and will never exist.  This will force the initial view of the spreadsheet 
			// control to be a blank document with just the headers shown.  This is the desired effect on initialization.
			this.BlotterId = 0;

			// This will prevent the document from drawing until a handle has been created.
			this.HandleCreated += new EventHandler(TicketViewer_HandleCreatedEventHandler);
			this.handleCreatedEvent = new ManualResetEvent(false);

			// These variables help control the state of the viewer.  Since we're using the spreadsheet control like a 
			// ledger, leaving a line after it's been modified creates a command to update the database.  Since the user
			// is free to click most anywhere on the input screen, we have no way of knowing what the last record was
			// unless we remember it using these members.
			this.isTicketSelected = false;

			// This is the accelerator for the spreadsheet.  It will map a row type, row and column set to an address on 
			// the spreadsheet.  It is built after the XML spreadsheet document is constructed and used primarily when
			// events fire to change the state of the report.
			this.addressMap = new AddressMap();

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
			this.createAddressMapDelegate = new InvokeCreateAddressMapDelegate(InvokeCreateAddressMap);
			this.setTicketDelegate = new TicketDelegate(SetTicket);
			this.getTicketDelegate = new TicketDelegate(GetTicket);
			this.selectCellDelegate = new SelectCellDelegate(SelectCell);

		}

		#region Dispose method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Kill any threads that are still pending.
			this.CommandQueue.Abort();

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TicketViewer));
			this.clientClientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			this.SuspendLayout();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			// 
			// TicketViewer
			// 
			this.MoveAfterReturn = true;
			this.Name = "TicketViewer";
			this.Enter += new System.EventHandler(this.TicketViewer_Enter);
			this.Leave += new System.EventHandler(this.TicketViewer_Leave);
			this.SelectionChange += new Shadows.Quasar.Viewers.SelectionChangeHandler(this.TicketViewer_SelectionChange);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Thread safe access to the blotter id.
		/// </summary>
		public int BlotterId
		{
			get {lock (this) return this.blotterId;}
			set {lock (this) this.blotterId = value;}
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
		/// Modifies the spreadsheet row to change a local execution id to a global execution id.
		/// </summary>
		/// <param name="localExecutionId">The identifier that is known only to this client</param>
		/// <param name="globalExecutionId">The identifier that is know to all clients of the data model.</param>
		private void GlobalizeTicket(int localExecutionId, int globalExecutionId)
		{

			// Find the location of the execution based on its local identifier.  If it exists, then the identifier will
			// be change and the row type will reflect the new location of the record.
			AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByExecutionId(localExecutionId);
			if (localMapRow != null)
			{

				// The row index and document version will be used several times below.
				int rowIndex = localMapRow.RowIndex;
				int documentVersion = localMapRow.DocumentVersion;

				// Change the row type to reflect that the record is now located in the global tables.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.RowType);
				if (rowTypeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, rowTypeColumnIndex), TicketRowType.GlobalTicket);
				
				// Change the execution identifier to the global id.
				int executionIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ExecutionId);
				if (executionIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, executionIdColumnIndex), globalExecutionId);

				// If the record that we just updated is also the currently selected on, then update the state values to
				// reflect the new id and row type.
				if (this.selectedExecutionId == localExecutionId)
				{
					this.selectedExecutionId = globalExecutionId;
					this.selectedRowType = TicketRowType.GlobalTicket;
				}

				// Swap around the mappings.  Add a mapping to the global table to the row that contains the execution and
				// remove the record the pointed the way to the local execution.
				this.addressMap.GlobalMap.AddGlobalMapRow(globalExecutionId, documentVersion, rowIndex);
				localMapRow.Delete();
				localMapRow.AcceptChanges();

			}

		}
		
		/// <summary>
		/// Updates an execution line on the spreadsheet.
		/// </summary>
		/// <param name="execution">An execution record.</param>
		private bool SetTicket(Ticket execution)
		{

			// The record can either be a local or global execution.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// Find the line number and document version based on the global identifier.
			AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByExecutionId(execution.ExecutionId);
			if (globalMapRow != null)
			{
				rowIndex = globalMapRow.RowIndex;
				documentVersion = globalMapRow.DocumentVersion;
			}

			// If the row doesn't exist in the mappings, return immediately with a failure status.  The invoking method 
			// will generally use this to redraw the entire document.  Otherwise, we will look at each field and see if it
			// needs to be updated in the spreadsheet.  Sometimes, the entire record will have changed, such as when we
			// get an update from the server's data model.  Other times, only one field will change, such as when a user
			// enters a new quantity or other attribute.  This method was built to handle any updates to the spreadsheet
			// from the background.
			if (rowIndex == SpreadsheetRow.DoesNotExist)
				return false;

			// BrokerId
			if (execution.IsBrokerIdModified())
			{
				int brokerIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.BrokerId);
				if (brokerIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerIdColumnIndex), execution.BrokerId);
			}

			// BrokerName
			if (execution.IsBrokerNameModified())
			{
				int brokerNameColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.BrokerName);
				if (brokerNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerNameColumnIndex), execution.BrokerName);
			}

			// BrokerSymbol
			if (execution.IsBrokerSymbolModified())
			{
				int brokerSymbolColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.BrokerSymbol);
				if (brokerSymbolColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerSymbolColumnIndex), execution.BrokerSymbol);
			}

			// ExecutionId
			if (execution.IsExecutionIdModified())
			{
				int executionIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ExecutionId);
				if (executionIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, executionIdColumnIndex), execution.ExecutionId);
			}

			// Quantity
			if (execution.IsQuantityModified())
			{
				int quantityColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.Quantity);
				if (quantityColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, quantityColumnIndex), execution.Quantity);
			}

			// Price
			if (execution.IsPriceModified())
			{
				int priceColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.Price);
				if (priceColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, priceColumnIndex), execution.Price);
			}

			// Commission
			if (execution.IsCommissionModified())
			{
				int commissionColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.Commission);
				if (commissionColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, commissionColumnIndex), execution.Commission);
			}

			// UserFee0
			if (execution.IsUserFee0Modified())
			{
				int userFee0ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee0);
				if (userFee0ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, userFee0ColumnIndex), execution.UserFee0);
			}

			// UserFee1
			if (execution.IsUserFee1Modified())
			{
				int userFee1ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee1);
				if (userFee1ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, userFee1ColumnIndex), execution.UserFee1);
			}

			// UserFee2
			if (execution.IsUserFee2Modified())
			{
				int userFee2ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee2);
				if (userFee2ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, userFee2ColumnIndex), execution.UserFee2);
			}

			// UserFee3
			if (execution.IsUserFee3Modified())
			{
				int userFee3ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee3);
				if (userFee3ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, userFee3ColumnIndex), execution.UserFee3);
			}
						
			// TradeDate
			if (execution.IsTradeDateModified())
			{
				int tradeDateColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.TradeDate);
				if (tradeDateColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, tradeDateColumnIndex), execution.TradeDate);
			}

			// SettlementDate
			if (execution.IsSettlementDateModified())
			{
				int settlementDateColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.SettlementDate);
				if (settlementDateColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, settlementDateColumnIndex), execution.SettlementDate);
			}

			// CreatedTime
			if (execution.IsCreatedTimeModified())
			{
				int createdTimeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.CreatedTime);
				if (createdTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdTimeColumnIndex), execution.CreatedTime);
			}

			// CreatedLoginId
			if (execution.IsCreatedLoginIdModified())
			{
				int createdLoginIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.CreatedLoginId);
				if (createdLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdLoginIdColumnIndex), execution.CreatedLoginId);
			}

			// CreatedLoginName
			if (execution.IsCreatedLoginNameModified())
			{
				int createdLoginNameColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.CreatedLoginName);
				if (createdLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdLoginNameColumnIndex), execution.CreatedLoginName);
			}

			// ModifiedTime
			if (execution.IsModifiedTimeModified())
			{
				int modifiedTimeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ModifiedTime);
				if (modifiedTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedTimeColumnIndex), execution.ModifiedTime);
			}

			// ModifiedLoginId
			if (execution.IsModifiedLoginIdModified())
			{
				int modifiedLoginIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ModifiedLoginId);
				if (modifiedLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginIdColumnIndex), execution.ModifiedLoginId);
			}

			// ModifiedLoginName
			if (execution.IsModifiedLoginNameModified())
			{
				int modifiedLoginNameColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ModifiedLoginName);
				if (modifiedLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginNameColumnIndex), execution.ModifiedLoginName);
			}

			// A successful return indicates that a valid record was updated.
			return true;

		}

		/// <summary>
		/// Gets the calculated fields from an execution on the screen.
		/// </summary>
		/// <param name="execution">An execution record.</param>
		private bool GetTicket(Ticket execution)
		{

			// The record can either be a local or global execution.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The Ticket record passed into this method will indicate whether a local or global record is being 
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
				return false;

			// Commission
			if (execution.IsCommissionCalculated())
			{
				int commissionColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.Commission);
				if (commissionColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.Commission = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, commissionColumnIndex)));
			}

			// AccruedInterest
			if (execution.IsAccruedInterestCalculated())
			{
				int accruedInterestColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.AccruedInterest);
				if (accruedInterestColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.AccruedInterest = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, accruedInterestColumnIndex)));
			}

			// UserFee0
			if (execution.IsUserFee0Calculated())
			{
				int userFee0ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee0);
				if (userFee0ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee0 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee0ColumnIndex)));
			}

			// UserFee1
			if (execution.IsUserFee1Calculated())
			{
				int userFee1ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee1);
				if (userFee1ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee1 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee1ColumnIndex)));
			}

			// UserFee2
			if (execution.IsUserFee2Calculated())
			{
				int userFee2ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee2);
				if (userFee2ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee2 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee2ColumnIndex)));
			}

			// UserFee3
			if (execution.IsUserFee3Calculated())
			{
				int userFee3ColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.UserFee3);
				if (userFee3ColumnIndex != SpreadsheetColumn.DoesNotExist)
					execution.UserFee3 = Convert.ToDecimal(GetCell(new CellAddress(documentVersion, rowIndex, userFee3ColumnIndex)));
			}

			// A successful return indicates that a valid record was found and retrieved.
			return true;

		}

		/// <summary>
		/// Selects a cell in the execution document based on the execution id and column type.
		/// </summary>
		/// <param name="isLocal">Indicates whether the row is a local or global execution.</param>
		/// <param name="executionId">Primary identifier of the record to be selected.</param>
		/// <param name="columnType">Column type of the cell to be selected.</param>
		private bool SelectCell(int executionId, int columnType)
		{

			// The record can either be a local or global execution.  The 'rowIndex' will have the line number in the
			// spreadsheet when the decoding logic has worked out where the line is.  The documentVersion is used in the
			// call to update each cell and will also be extracted from the AddressMap DataSet.  This section will find
			// the row index and document version for either a local or global execution record.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// Find the line number and document version based on the global identifier.
			AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByExecutionId(executionId);
			if (globalMapRow != null)
			{
				rowIndex = globalMapRow.RowIndex;
				documentVersion = globalMapRow.DocumentVersion;
			}

			// Find the column index of the requested cell.  If a valid row and column are found, then the cell can be 
			// selected.  A sucessful return indicates that the specified cell was found.
			int columnIndex = this.addressMap.GetColumnIndex(columnType);
			if (rowIndex != SpreadsheetRow.DoesNotExist && columnIndex != SpreadsheetColumn.DoesNotExist)
			{
				Select(new CellAddress(documentVersion, rowIndex, columnIndex));
				return true;
			}

			// This indicates that either the addressed cell doesn't exist in the document.
			return false;

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
				ExecuteCommand(Command.Draw, this.BlotterId);

				// We assume that no refresh is needed again until an event request one.
				this.IsRefreshNeeded = false;

			}

		}


		/// <summary>
		/// Creates an displays an execution report.
		/// </summary>
		/// <param name="blockOrderId">The block order of the executions.</param>
		/// <param name="execution">Optional temporary execution.</param>
		private void DrawCommand(params object[] argument)
		{

			// Extract the command arguments.
			int blotterId = (int)argument[0];

			// Wait until the handle is created before drawing this document.
			this.handleCreatedEvent.WaitOne();

			try
			{

				// Lock all the tables that we'll reference while building a placement document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the block order still exists.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null && blotterId != 0)
					throw new ArgumentException("The block order has been deleted.", blotterId.ToString());

				// Create the execution document.
				TicketDocument ticketDocument = new TicketDocument(blotterId);

#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Dump the DOM to a file.  This is very useful when debugging.
					ticketDocument.Save("ticketDOM.xml");

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
				spreadsheetDocument.Load(this.xslTransform.Transform(ticketDocument, xsltArgumentList, (XmlResolver)null));

				// Parse the XML file to create a quick lookup table for cells.  Looking through a spreadsheet control is
				// exteremly slow, so we parse the XML file that created the spreadsheet and construct a lookup table for
				// all the cells based on the key elements in each spreadsheet row.  This allows us to find any given cell
				// address quickly given the key information.  The 'DocumentVersion' is used to make sure we don't sent
				// data from the background to a document in the foreground that doesn't exist any more.
				Invoke(this.createAddressMapDelegate, new object[] {spreadsheetDocument});

#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Write out the spreadsheet document when debugging.
					spreadsheetDocument.Save("TicketSpreadsheet.xml");

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
					
				// Release the locks
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Opens the Ticket Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Ticket Viewer in the background.
			ExecuteCommand(Command.OpenViewer);

		}

		/// <summary>
		/// Closes the Ticket Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Ticket Viewer in the background.
			ExecuteCommand(Command.CloseViewer);

		}

		
		/// <summary>
		/// Opens the Ticket Document.
		/// </summary>
		/// <param name="blotterId">The primary identifier of the object to open.</param>
		public override void OpenDocument(int blotterId)
		{

			// Open the Ticket Document in the background.
			ExecuteCommand(Command.OpenDocument, blotterId);

		}

		/// <summary>
		/// Closes the Ticket Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Close the Ticket Document in the background.
			ExecuteCommand(Command.CloseDocument);

		}

		/// <summary>
		/// Opens the Ticket Viewer.
		/// </summary>
		public void OpenViewerCommand()
		{

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

			// Broadcast the event that the Ticket Viewer is now open.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Opens the Ticket Document.
		/// </summary>
		/// <param name="blockOrderId">The primary identifier the document.</param>
		public void OpenDocumentCommand(int blotterId)
		{

			try
			{
			
				// Store the id of the blotter.
				this.BlotterId = blotterId;

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the block order and extract the securty level data if it exists.  This security level data is
				// needed to calculate the trade and settlement dates and other defaults for the executions.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null)
					throw new Exception(String.Format("Blotter {0} has been deleted", blotterId));

				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsTicketStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.TicketStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Ticket stylesheet", blotterId));

				// As an optimization, don't reload the stylesheet if the prevous document used the same stylesheet. This
				// will save a few hundred milliseconds when scrolling through similar documents.
				if (this.IsStylesheetChanged || this.StylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case it is changed while we're viewing it.  The event handler 
					// will use this id to determine if an incoming stylesheet will trigger a refresh of the document.
					this.StylesheetId = stylesheetRow.StylesheetId;

					// Load the stylesheet from the BLOB in the in-memory table.
					this.xslTransform = new XslTransform();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					this.xslTransform.Load(xmlTextReader, (XmlResolver)null, (Evidence)null);

				}

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

			// Display the header and clear out the document.
			this.CommandQueue.Enqueue(new ThreadHandler(DrawCommand), blotterId);

			// Broadcast the event that the Ticket Document is now open.
			OnEndOpenDocument();

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
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();

			}

			// Broadcast the fact that the viewer is now closed.
			OnEndCloseViewer();

		}

		private bool IsChildBlotter(ClientMarketData.BlotterRow parentBlotter, int blotterId)
		{

			if (parentBlotter.BlotterId == this.BlotterId)
				return true;
			
			ClientMarketData.ObjectRow parentObject = parentBlotter.ObjectRow;
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in parentObject.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.BlotterRow childBlotter in objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetBlotterRows())
					if (IsChildBlotter(childBlotter, blotterId))
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

			// This will signal that a complete refresh is required.
			this.IsRefreshNeeded = true;

		}
		
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void BrokerRowChangeEvent(object sender, ClientMarketData.BrokerRowChangeEvent executionRowChangeEvent)
		{

			// This will signal that a complete refresh is required.
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
			if (!this.IsRefreshNeeded && executionRowChangeEvent.Action != DataRowAction.Commit)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the fields.
				ClientMarketData.ExecutionRow executionRow = executionRowChangeEvent.Row;

				if (executionRowChangeEvent.Action == DataRowAction.Delete)
					this.IsRefreshNeeded = true;
				
				if (executionRowChangeEvent.Action == DataRowAction.Add)
				{

					// To determine whether this execution belongs to this viewer, a recursive search will be done on
					// the parent blotter.
					ClientMarketData.BlotterRow parentBlotter = ClientMarketData.Blotter.FindByBlotterId(this.BlotterId);
					if (parentBlotter == null)
						throw new Exception(String.Format("Blotter {0} has been deleted", this.BlotterId));

					// The first test is to make sure that this ticket belongs on this report.  The blotter identifies all the 
					// execution tickets that should be shown here.
					if (IsChildBlotter(parentBlotter, executionRow.BlockOrderRow.BlotterRow.BlotterId))
					{

						// Call the foreground to update the ticket.  If there is no existing record in the document to update, a
						// complete refresh of the document will be generated.  Otherwise, the foreground will update only the impacted
						// cells.
						if (!(bool)Invoke(this.setTicketDelegate, new object[] {new GlobalTicket(executionRow)}))
							this.IsRefreshNeeded = true;

						if (executionRow.ExecutionId == this.selectedExecutionId)
							this.isTicketSelected = false;

					}

				}

			}

		}
		
		/// <summary>
		/// Handles the changing of an execution record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void StylesheetRowChangeEvent(object sender, ClientMarketData.StylesheetRowChangeEvent executionRowChangeEvent)
		{

			// This will signal that a complete refresh is required.
			this.IsRefreshNeeded = true;

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
			if (this.IsDocumentOpen && stylesheetRow.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.BlotterId);
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

				// This section will parse the key element and argument, then call a method to handle the command.  
				// There is also some basic field validation that takes place here.
				switch (command)
				{

					case Command.OpenViewer:

						// Open the Ticket Viewer.
						OpenViewerCommand();
						break;

					case Command.CloseViewer:

						// Close the Ticket Viewer.
						CloseViewerCommand();
						break;

					case Command.OpenDocument:

						// Open the Ticket Document.
						OpenDocumentCommand((int)key);
						break;

					case Command.CloseDocument:

						// Close the Document
						OnEndCloseDocument();
						break;

					case Command.Draw:

						// Draw the Ticket Document.
						DrawCommand((int)key);
						break;

					case Command.UpdateGlobal:

						// Update the server with a global execution.
						UpdateGlobalTicket((int)key);
						break;

					case Command.DeleteGlobal:

						// Delete an execution from the server.
						DeleteGlobalTicket((int)key);
						break;
					
					case Command.SetGlobalBrokerSymbol:
					
						// Global Broker Symbol
						if (argument == null)
						{
							Invoke(this.selectCellDelegate, new object[] {(int)key, TicketColumnType.BrokerSymbol});
							MessageBox.Show("Please enter a valid broker symbol", "Quasar Error");
						}
						else
							SetGlobalBrokerSymbol((int)key, (int)argument);

						break;

					case Command.SetGlobalQuantity:
					
						// Global Quantity
						SetGlobalQuantity((int)key, Quantity.Parse((string)argument));
						break;

					case Command.SetGlobalPrice:
					
						// Global Price
						SetGlobalPrice((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalCommission:
					
						// Global Commission
						SetGlobalCommission((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalAccruedInterest:
					
						// Global AccruedInterest
						SetGlobalAccruedInterest((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee0:
					
						// Global UserFee0
						SetGlobalUserFee0((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee1:
					
						// Global UserFee1
						SetGlobalUserFee1((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee2:
					
						// Global UserFee2
						SetGlobalUserFee2((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalUserFee3:
					
						// Global UserFee3
						SetGlobalUserFee3((int)key, Convert.ToDecimal(argument));
						break;

					case Command.SetGlobalTradeDate:
					
						// Global Trade Date
						SetGlobalTradeDate((int)key, Convert.ToDateTime(argument));
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
		/// Set the broker id of a global execution record based on a symbol.
		/// </summary>
		/// <param name="executionId">Identifer of the execution.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetGlobalBrokerSymbol(int executionId, int brokerId)
		{

			try
			{

				// Lock the local tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BrokerLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.BrokerId = brokerId;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.BrokerId = brokerId;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the global locks.
				if (ClientMarketData.BrokerLock.IsWriterLockHeld) ClientMarketData.BrokerLock.ReleaseWriterLock();
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectLock.IsWriterLockHeld) ClientMarketData.ObjectLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Quantity = quantity;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.Quantity = executionRow.Quantity;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Price = price;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.Price = executionRow.Price;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.Commission = commission;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.Commission = executionRow.Commission;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.AccruedInterest = accruedInterest;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.AccruedInterest = executionRow.AccruedInterest;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee0 = userFee0;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.UserFee0 = executionRow.UserFee0;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee1 = userFee1;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.UserFee1 = executionRow.UserFee1;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee2 = userFee2;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.UserFee2 = executionRow.UserFee2;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the global record
				ClientMarketData.ExecutionRow executionRow =
					ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} not found.", executionId));

				// Update the global record
				executionRow.UserFee3 = userFee3;

				// Now that the data model reflects the changed record, we will update the field in the spreadsheet. A
				// delegate must be used because we can't modify the spreadsheet from the background.
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.UserFee3 = executionRow.UserFee3;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

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
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.TradeDate = executionRow.TradeDate;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

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

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

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
				GlobalTicket globalTicket = new GlobalTicket(executionRow.ExecutionId);
				globalTicket.SettlementDate = executionRow.SettlementDate;
				Invoke(this.setTicketDelegate, new object[] {globalTicket});

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Updates the execution record on the server.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void UpdateGlobalTicketThread(params object[] argument)
		{

			// IMPORTANT CONCEPT: For performance, all changes to the server made by the user are assumed to be 
			// successful and udpated immediately on the client side.  If there is an error with the server, the data
			// model is restored from the server.  This flag will be used as a signal at the end of this operation that we
			// need to revert back to the server's data.  Note that we can't attempt to refresh the data model from inside
			// a 
			bool isSuccessful = true;
			
			// Extract the initialization parameters.
			Ticket ticket = (Ticket)argument[0];

			// This record will be rolled back if the update isn't successful.
			ClientMarketData.ExecutionRow globalTicket = null;

			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the global tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the ticket record in the global data tables.
				globalTicket = ClientMarketData.Execution.FindByExecutionId(ticket.ExecutionId);
				if (globalTicket == null)
					throw new Exception(String.Format("Execution {0} not found.", ticket.ExecutionId));

				// These parameters are returned from the call to update the record and are used to track the
				// ticket until we get a refresh from the server.
				int modifiedLoginId = 0;
				long rowVersion = ticket.RowVersion;
				DateTime modifiedTime = DateTime.MinValue;

				// Call the web service to add the new placement.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
				remoteMethod.Parameters.Add("executionId", ticket.ExecutionId);
				remoteMethod.Parameters.Add("blockOrderId", ticket.BlockOrderId);
				remoteMethod.Parameters.Add("brokerId", ticket.BrokerId);
				remoteMethod.Parameters.Add("rowVersion", rowVersion);
				remoteMethod.Parameters.Add("quantity", ticket.Quantity);
				remoteMethod.Parameters.Add("price", ticket.Price);
				remoteMethod.Parameters.Add("commission", ticket.Commission);
				remoteMethod.Parameters.Add("accruedInterest", ticket.AccruedInterest);
				remoteMethod.Parameters.Add("userFee0", ticket.UserFee0);
				remoteMethod.Parameters.Add("userFee1", ticket.UserFee0);
				remoteMethod.Parameters.Add("userFee2", ticket.UserFee0);
				remoteMethod.Parameters.Add("tradeDate", ticket.TradeDate);
				remoteMethod.Parameters.Add("settlementDate", ticket.SettlementDate);
				ClientMarketData.Execute(remoteBatch);

				// Update the internal data model with the new data.  Note that we update the rowVersion, modified time
				// and login id from the data returned from the server. If the server finished the update operation
				// without an error, we can assume that the members of the 'ticket' record have been accepted.  We will
				// update the data model from that record to give an accurate state until the next refresh.
				globalTicket.RowVersion = rowVersion;
				globalTicket.BlockOrderId = ticket.BlockOrderId;
				globalTicket.BrokerId = ticket.BrokerId;
				globalTicket.ExecutionId = ticket.ExecutionId;
				globalTicket.Quantity = ticket.Quantity;
				globalTicket.Price = ticket.Price;
				globalTicket.Commission = ticket.Commission;
				globalTicket.AccruedInterest = ticket.AccruedInterest;
				globalTicket.UserFee0 = ticket.UserFee0;
				globalTicket.UserFee1 = ticket.UserFee1;
				globalTicket.UserFee2 = ticket.UserFee2;
				globalTicket.UserFee3 = ticket.UserFee3;
				globalTicket.TradeDate = ticket.TradeDate;
				globalTicket.SettlementDate = ticket.SettlementDate;
				globalTicket.ModifiedTime = modifiedTime;
				globalTicket.ModifiedLoginId = modifiedLoginId;
				globalTicket.AcceptChanges();

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
				
				// Reject the changes that were made to the local record.
				if (globalTicket != null) globalTicket.RejectChanges();

				// This will catch all remaining exceptions.
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// Indicate that a data model refresh is needed when we're done with the locks.
				isSuccessful = false;

			}
			finally
			{

				// Release the global tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Force the document to redraw the original data.  The call to 'RefreshClientMarketData' must be made outside of any
			// table locking to prevent deadlocks.  The flag 'isSuccessful' is used to signal whether we need to revert 
			// back to the server data model after the operation above.
			if (!isSuccessful)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlotterId);

		}

		private class DeleteExecutionArgs
		{

			// Members
			private long rowVersion;
			private int executionId;
			private ClientMarketData.ExecutionRow executionRow;

			// Public access
			public long RowVersion {get {return this.rowVersion;}}
			public int ExecutionId {get {return this.executionId;}}
			public ClientMarketData.ExecutionRow ExecutionRow {get {return this.executionRow;}}

			public DeleteExecutionArgs(long rowVersion, int executionId, ClientMarketData.ExecutionRow executionRow)
			{

				// Initialize Members
				this.rowVersion = rowVersion;
				this.executionId = executionId;
				this.executionRow = executionRow;

			}

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void DeleteExecutionThread(params object[] argument)
		{

			// Extract the parameters used by this thread.
			DeleteExecutionArgs deleteExecutionArgs = (DeleteExecutionArgs)argument[0];

			// Make sure that the table locks are released.		
			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the table while we make our changes.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Call the web service to add the new placement.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Execution");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
				remoteMethod.Parameters.Add("executionId", deleteExecutionArgs.ExecutionId);
				remoteMethod.Parameters.Add("rowVersion", deleteExecutionArgs.RowVersion);
				ClientMarketData.Execute(remoteBatch);

				// Delete the record from the local tables once the server has confirmed that it's gone.
				deleteExecutionArgs.ExecutionRow.AcceptChanges();

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
				
				// If the operation fails, restore the row that was deleted.
				deleteExecutionArgs.ExecutionRow.RejectChanges();
				
				// After rejecting the changes, we need to redraw the document to restore the record.
				// HACK - this needs to be reworked with the new refrsh logic.
//				DrawDocument(false);
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		private void UpdateGlobalTicket(int executionId)
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

				Ticket ticket = new GlobalTicket(executionRow);
				Invoke(this.getTicketDelegate, new object[] {ticket});

				// If the ticket record has been modified, launch a service thread that will modify the record on the server.
				if (executionRow.RowState != DataRowState.Unchanged)
					this.ServiceQueue.Enqueue(new ThreadHandler(UpdateGlobalTicketThread), ticket);

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

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

		/// <summary>
		/// Deletes an execution from the shared data model.
		/// </summary>
		/// <param name="executionId">Primary identifier of the execution.</param>
		private void DeleteGlobalTicket(int executionId)
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

				// The thread that interacts with the server will confirm the deletion by removing the record completely
				// from the local tables.  If there is an error with the server, then the transaction is rolled back.  The
				// model is to do a 'soft' delete here, in the quick response thread.  That way we can redraw the screen
				// quickly and give the user some indication that the record was deleted.  If there is a problem on the
				// server -- for instance, a concurrency check fails -- then the thread that deals with the server will
				// roll the transaction back and redraw the document.
				DeleteExecutionArgs deleteExecutionArgs = new DeleteExecutionArgs(executionRow.RowVersion, executionRow.ExecutionId, executionRow);

				// Delete the record from the local tables.  We don't accept the changes yet until the background thread
				// has confirmed the deletion with the server.
				executionRow.Delete();

				// Queue up a request for another, longer lasting thread to take care of the interaction with the server.
				this.ServiceQueue.Enqueue(new ThreadHandler(DeleteExecutionThread), deleteExecutionArgs);

				// Redraw the document with one less execution.
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlotterId);

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
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// HACK - Fill this in some other time.
		/// </summary>
		public void DeleteActiveRow()
		{

			// Extract the current row and column.  The task will be handled in the background thread since a change in
			// the selection can impact the data model.  So we collect the foreground information here and spawn a thread
			// to handle the action.
			CellAddress activeCellAddress = GetActiveCellAddress();
			int rowIndex = activeCellAddress.RowIndex;
			int columnIndex = activeCellAddress.ColumnIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.RowType);
			int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			if (rowType == TicketRowType.GlobalTicket)
			{

				// Get the execution id from the selected row.
				int executionIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ExecutionId);
				int executionId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					executionIdColumnIndex)));

				ExecuteCommand(Command.DeleteGlobal, executionId, null);

				this.isTicketSelected = false;
				CommandMoveUp();

			}
	
		}

		private void DeleteExecution(int executionId)
		{

			// Any exceptions will indicate to the refresh logic that a complete document refresh is needed at the 
			// end of this thread.  Otherwise, the intelligent update will change the effected cells.
			// HACK this logic need to be done over.
//			bool isSuccessful = true;

			// Make sure that the table locks are released.		
			try
			{

				// Lock the table while we make our changes.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ExecutionLock.AcquireWriterLock(CommonTimeout.LockWait);

				ClientMarketData.ExecutionRow executionRow = ClientMarketData.Execution.FindByExecutionId(executionId);
				if (executionRow == null)
					throw new Exception(String.Format("Execution {0} doesn't exist", executionId));

				// The timestamp is given to the new execution at the moment we commit it to the data server.
				executionRow.ModifiedTime = DateTime.Now;
				executionRow.ModifiedLoginId = ClientPreferences.LoginId;

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
			
				// If an error occurs, force a refresh of the viewer.
				// HACK
//				isSuccessful = false;

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.ExecutionLock.IsWriterLockHeld) ClientMarketData.ExecutionLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Update the appraisal.  If there was an error, the entire document will be refreshed, otherwise, the
			// minimal data model update logic will apply.
			CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlotterId);

		}

		/// <summary>
		/// Handles an escape command.
		/// </summary>
		public override void CommandEscape()
		{

			// Give the focus back to whoever wants it.
			OnReleaseFocus();

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
				int key = 0;

				// Extract the current row and column from the event argument.  This is the address of the cell where 
				// the editing took place. Note that, because the user can move the focus at any time to another cell,
				// this is not necessarily the address of the currently active cell.
				int rowIndex = e.CellAddress.RowIndex;
				int columnIndex = e.CellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a global record.
				if (rowType == TicketRowType.GlobalTicket)
				{

					// Get the execution id from the selected row.
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ExecutionId);
					key = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						executionIdColumnIndex)));

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{
						case TicketColumnType.BrokerSymbol: command = Command.SetGlobalBrokerSymbol; break;
						case TicketColumnType.Quantity: command = Command.SetGlobalQuantity; break;
						case TicketColumnType.Price: command = Command.SetGlobalPrice; break;
						case TicketColumnType.Commission: command = Command.SetGlobalCommission; break;
						case TicketColumnType.AccruedInterest: command = Command.SetGlobalAccruedInterest; break;
						case TicketColumnType.UserFee0: command = Command.SetGlobalUserFee0; break;
						case TicketColumnType.UserFee1: command = Command.SetGlobalUserFee1; break;
						case TicketColumnType.UserFee2: command = Command.SetGlobalUserFee2; break;
						case TicketColumnType.UserFee3: command = Command.SetGlobalUserFee3; break;
						case TicketColumnType.TradeDate: command = Command.SetGlobalTradeDate; break;
						case TicketColumnType.SettlementDate: command = Command.SetGlobalSettlementDate; break;
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
		/// Called when the Handle is created.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="e">Parameters specific to the event.</param>
		private void TicketViewer_HandleCreatedEventHandler(object sender, EventArgs e)
		{

			// Signal the background thread that the TreeView can now accept data.
			this.handleCreatedEvent.Set();

		}

		/// <summary>
		/// Called when the focus is given to the viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void TicketViewer_Enter(object sender, System.EventArgs e)
		{

			// Interpret the focus coming into the viewer as if a new cell were selected.  This is done to force the
			// 'Prompting' action of new executions.
			OnSelectionChange(new SelectionChangeArgs(GetActiveCellAddress()));

		}

		/// <summary>
		/// Called when the focus leaves this viewer.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		private void TicketViewer_Leave(object sender, System.EventArgs e)
		{

			// Clearing this flag will cause the element to be reselected when the focus
			// is returned.  This is essential for the 'prompt' line to work intuitively.
			this.isTicketSelected = false;

		}

		/// <summary>
		/// Handles the selection changing from one cell to another.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="selectionChangeArgs"></param>
		private void TicketViewer_SelectionChange(object sender, Shadows.Quasar.Viewers.SelectionChangeArgs selectionChangeArgs)
		{

			// If the document versions don't match, we don't want to process the event.  It means that we selected a cell
			// and the document changed before we could process the change.
			if (selectionChangeArgs.CellAddress.DocumentVersion != this.DocumentVersion)
			{

				if (this.isTicketSelected)
				{

					if (this.selectedRowType == TicketRowType.GlobalTicket)
						ExecuteCommand(Command.UpdateGlobal, this.selectedExecutionId, null);

					this.isTicketSelected = false;

				}

				return;

			}

			// Extract the current row and column from the event argument. 
			int rowIndex = selectionChangeArgs.CellAddress.RowIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.RowType);
			int rowType = (rowTypeColumnIndex == SpreadsheetColumn.DoesNotExist) ?
				TicketRowType.Unused :
				Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			switch (rowType)
			{

				case TicketRowType.Unused:

					// If we had previously selected an execution record, see if it needs to be inserted or updated before we
					// move on to the next record.
					if (this.isTicketSelected)
					{

						if (this.selectedRowType == TicketRowType.GlobalTicket)
							ExecuteCommand(Command.UpdateGlobal, this.selectedExecutionId, null);

						this.isTicketSelected = false;

					}
				
					break;

				case TicketRowType.GlobalTicket:

					// Get the row type from the selected row.
					int executionIdColumnIndex = this.addressMap.GetColumnIndex(TicketColumnType.ExecutionId);
					int executionId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, executionIdColumnIndex)));

					// If we had previously selected an execution record, see if it needs to be inserted or updated before we
					// move on to the next record.
					if (this.isTicketSelected && this.selectedExecutionId != executionId)
					{

						if (this.selectedRowType == TicketRowType.GlobalTicket)
							ExecuteCommand(Command.UpdateGlobal, this.selectedExecutionId, null);

					}
					
					// This catpures the state of the selection so we can test it the next time the selection changes.
					this.isTicketSelected = true;
					this.selectedExecutionId = executionId;
					this.selectedRowType = rowType;
						 
					break;

			}
	
		}

	}

}
