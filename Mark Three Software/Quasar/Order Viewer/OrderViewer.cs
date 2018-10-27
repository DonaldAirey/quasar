/*************************************************************************************************************************
*
*	File:			OrderViewer.cs
*	Description:	This control is used to display and manage a order.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Order
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
	using System.Security.Policy;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;
	using System.Resources;
	using System.Web.Services.Protocols;

	/// <summary>
	/// A Viewer for Order Orders
	/// </summary>
	public class OrderViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private bool isManualSelection;
		private bool isOrderValid;
		private bool isOrderChanged;
		private bool isOrderSelected;
		private bool isFieldValid;
		private bool isLedger;
		private int blotterId;
		private int blockOrderId;
		private int selectedRowType;
		private int selectedOrderId;
		private System.Xml.Xsl.XslTransform xslTransform;
		private System.ComponentModel.IContainer components = null;
		private AddressMap addressMap;
		private LocalOrderSet localOrderSet;
		private delegate void OrderDelegate(OrderSet.OrderRow orderRow);
		private delegate void SelectCellDelegate(int OrderId, int columnType);
		private delegate void GlobalizeOrderDelegate(int localOrderId, int globalOrderId);
		private OrderDelegate setOrderDelegate;
		private OrderDelegate getOrderDelegate;
		private OrderDelegate initializeRowDelegate;
		private SelectCellDelegate selectLocalFieldDelegate;
		private SelectCellDelegate selectGlobalFieldDelegate;
		private GlobalizeOrderDelegate globalizeOrderDelegate;
		private ReaderWriterLock localOrderSetLock = new ReaderWriterLock();
		private Shadows.Quasar.Client.ClientMarketData clientMarketData;
		private ResourceManager resourceManager;

		/// <summary>
		/// Constructor for the OrderViewer
		/// </summary>
		public OrderViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// Add the debt calculation library to the functions recognized by this spreadsheet control.
			this.ExternalLibraries.Add(new Viewers.DebtLibrary.Debt());

			// This determines whether input is handled ledger-style or dialog box style.  Ledger style will update the records
			// by moving the input focus off the updated record, like in a spreadsheet input.  The dialog box style will return
			// a dataset containing all the elements.  This is useful when the control is part of a dialog box that requires other
			// interaction, such as the user to press the 'OK' button before all the results are recorded.
			this.isLedger = true;

			// These variables help control the state of the viewer.  Since we're using the spreadsheet control like a
			// ledger, leaving a line after it's been modified creates a command to update the database.  Since the user
			// is free to click most anywhere on the input screen, we have no way of knowing what the last record was
			// unless we remember it using these members.
			this.isManualSelection = true;
			this.isOrderValid = true;
			this.isOrderChanged = false;
			this.isOrderSelected = false;
			this.isFieldValid = true;

			// This DataSet is used to hold local orders.  These are orders that can't be placed in the global
			// data model because they don't have an identifier yet.  So we keep a local copy until the server can give us
			// an identifier, then they'll be moved into the 'ClientMarketData'.
			this.localOrderSet = new LocalOrderSet();

			// IMPORTANT CONCEPT: Create the delegates for updating the spreadsheet control from the background.  The
			// address map is used to find specific cells in the viewer with the idea that we're going to read or write to
			// those cells.  This presents a bit of a problem for a multithreaded application.  We can't use table locking
			// in the foreground: if a lock on that table is obtained in the background, then the foreground will be
			// blocked.  If that same background thread attempts to use an 'Invoke', an insidious deadlock will happen
			// because the message loop (foeground thread) is waiting on the table lock that the background thread has.
			// The background thread is waiting on the message loop to process the 'Invoke' command.  For this reason,
			// waiting on locks has been prohibited in the foreground.  Background threads must use delegates to
			// communicate with the foreground message loop.  The ugly impact is these methods that must execute in the
			// foreground and can only be called using the 'Invoke' method.
			this.setOrderDelegate = new OrderDelegate(SetOrder);
			this.getOrderDelegate = new OrderDelegate(GetOrder);
			this.initializeRowDelegate = new OrderDelegate(InitializeRow);
			this.selectLocalFieldDelegate = new SelectCellDelegate(SelectLocalField);
			this.selectGlobalFieldDelegate = new SelectCellDelegate(SelectGlobalField);
			this.globalizeOrderDelegate = new GlobalizeOrderDelegate(GlobalizeOrder);

			// Open a resource manager to handle the localization of the viewer.
			this.resourceManager = new ResourceManager("Shadows.Quasar.Viewers.Order.Resource",
				Assembly.GetExecutingAssembly());

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(OrderViewer));
			this.clientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			this.SuspendLayout();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			// 
			// OrderViewer
			// 
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MoveAfterReturn = true;
			this.Name = "OrderViewer";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.TabIndex = ((int)(resources.GetObject("$this.TabIndex")));
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Thread safe indication of a changed order record.
		/// </summary>
		[Browsable(false)]
		public LocalOrderSet LocalOrderSet {get {return this.localOrderSet;}}
		
		/// <summary>
		/// Thread safe access to the blotter id.
		/// </summary>
		[Browsable(true)]
		public bool IsLedger
		{
			get {lock (this) return this.isLedger;}
			set {lock (this) this.isLedger = value;}
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
		/// Updates an Order line on the spreadsheet.
		/// </summary>
		/// <param name="orderRow">An orderRow record.</param>
		private void SetOrder(OrderSet.OrderRow orderRow)
		{

			// The record can either be a local or global Order.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The record passed into this method will indicate whether a local or global record is being changed.  Each
			// has its own set of identifiers and mappings from the identifier to a row in the document.
			if (orderRow.IsLocal)
			{

				// Find the line number and document version based on the local identifier.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByOrderId(orderRow.OrderId);
				if (localMapRow != null)
				{
					rowIndex = localMapRow.RowIndex;
					documentVersion = localMapRow.DocumentVersion;
				}

			}
			else
			{

				// Find the line number and document version based on the global identifier.
				AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByOrderId(orderRow.OrderId);
				if (globalMapRow != null)
				{
					rowIndex = globalMapRow.RowIndex;
					documentVersion = globalMapRow.DocumentVersion;
				}

			}

			// If the row doesn't exist in the mappings, we'll have to refresh the entire document to include it.
			// Otherwise, we will look at each field and see if it needs to be updated in the spreadsheet.  Sometimes, the
			// entire record will have changed, such as when we get an update from the server's data model.  Other times,
			// only one field will change, such as when a user enters a new Quantity or other attribute.  This method was
			// built to handle any updates to the spreadsheet from the background.
			if (rowIndex == SpreadsheetRow.DoesNotExist)
			{

				// Abort the incremental upates and force a refresh of the whole document.
				this.IsRefreshNeeded = true;

			}
			else
			{

				// AccountId
				int accountIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.AccountId);
				if (accountIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, accountIdColumnIndex),
						orderRow.IsAccountIdNull() ? (object)null : orderRow.AccountId);

				// AccountName
				int accountNameColumnIndex = this.addressMap.GetColumnIndex(ColumnType.AccountName);
				if (accountNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, accountNameColumnIndex),
						orderRow.IsAccountNameNull() ? (object)null : orderRow.AccountName);

				// AccountMnemonic
				int accountMnemonicColumnIndex = this.addressMap.GetColumnIndex(ColumnType.AccountMnemonic);
				if (accountMnemonicColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, accountMnemonicColumnIndex),
						orderRow.IsAccountMnemonicNull() ? (object)null : orderRow.AccountMnemonic);

				// SecurityId
				int securityIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SecurityId);
				if (securityIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, securityIdColumnIndex),
						orderRow.IsSecurityIdNull() ? (object)null : orderRow.SecurityId);

				// SecurityName
				int securityNameColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SecurityName);
				if (securityNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, securityNameColumnIndex),
						orderRow.IsSecurityNameNull() ? (object)null : orderRow.SecurityName);

				// SecuritySymbol
				int securitySymbolColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SecuritySymbol);
				if (securitySymbolColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, securitySymbolColumnIndex),
						orderRow.IsSecuritySymbolNull() ? (object)null : orderRow.SecuritySymbol);

				// TransactionTypeCode
				int transactionTypeCodeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.TransactionTypeCode);
				if (transactionTypeCodeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, transactionTypeCodeColumnIndex),
						orderRow.IsTransactionTypeCodeNull() ? (object)null : orderRow.TransactionTypeCode);

				// TransactionTypeMnemonic
				int transactionTypeMnemonicColumnIndex = this.addressMap.GetColumnIndex(ColumnType.TransactionTypeMnemonic);
				if (transactionTypeMnemonicColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, transactionTypeMnemonicColumnIndex),
						orderRow.IsTransactionTypeMnemonicNull() ? (object)null : orderRow.TransactionTypeMnemonic);
				
				// BrokerId
				int brokerIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BrokerId);
				if (brokerIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerIdColumnIndex),
						orderRow.IsBrokerIdNull() ? (object)null : orderRow.BrokerId);

				// BrokerName
				int brokerNameColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BrokerName);
				if (brokerNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerNameColumnIndex),
						orderRow.IsBrokerNameNull() ? (object)null : orderRow.BrokerName);

				// BrokerSymbol
				int brokerSymbolColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BrokerSymbol);
				if (brokerSymbolColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, brokerSymbolColumnIndex),
						orderRow.IsBrokerSymbolNull() ? (object)null : orderRow.BrokerSymbol);

				// TimeInForceCode
				int timeInForceCodeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.TimeInForceCode);
				if (timeInForceCodeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, timeInForceCodeColumnIndex),
						orderRow.IsTimeInForceCodeNull() ? (object)null : orderRow.TimeInForceCode);

				// TimeInForceMnemonic
				int timeInForceMnemonicColumnIndex = this.addressMap.GetColumnIndex(ColumnType.TimeInForceMnemonic);
				if (timeInForceMnemonicColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, timeInForceMnemonicColumnIndex),
						orderRow.IsTimeInForceMnemonicNull() ? (object)null : orderRow.TimeInForceMnemonic);
				
				// OrderTypeCode
				int OrderTypeCodeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderTypeCode);
				if (OrderTypeCodeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, OrderTypeCodeColumnIndex),
						orderRow.IsOrderTypeCodeNull() ? (object)null : orderRow.OrderTypeCode);

				// OrderTypeMnemonic
				int orderTypeMnemonicColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderTypeMnemonic);
				if (orderTypeMnemonicColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, orderTypeMnemonicColumnIndex),
						orderRow.IsOrderTypeMnemonicNull() ? (object)null : orderRow.OrderTypeMnemonic);
				
				// Quantity
				int QuantityColumnIndex = this.addressMap.GetColumnIndex(ColumnType.Quantity);
				if (QuantityColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, QuantityColumnIndex),
						orderRow.IsQuantityNull() ? (object)null : orderRow.Quantity);

				// Price1
				int price1ColumnIndex = this.addressMap.GetColumnIndex(ColumnType.Price1);
				if (price1ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, price1ColumnIndex),
						orderRow.IsPrice1Null() ? (object)null : orderRow.Price1);

				// Price2
				int price2ColumnIndex = this.addressMap.GetColumnIndex(ColumnType.Price2);
				if (price2ColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, price2ColumnIndex),
						orderRow.IsPrice2Null() ? (object)null : orderRow.Price2);

				// CreatedTime
				int createdTimeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.CreatedTime);
				if (createdTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdTimeColumnIndex),
						orderRow.IsCreatedTimeNull() ? (object)null : orderRow.CreatedTime);

				// CreatedLoginId
				int createdLoginIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.CreatedLoginId);
				if (createdLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdLoginIdColumnIndex),
						orderRow.IsCreatedLoginIdNull() ? (object)null : orderRow.CreatedLoginId);

				// CreatedLoginName
				int createdLoginNameColumnIndex = this.addressMap.GetColumnIndex(ColumnType.CreatedLoginName);
				if (createdLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, createdLoginNameColumnIndex),
						orderRow.IsCreatedLoginNameNull() ? (object)null : orderRow.CreatedLoginName);

				// ModifiedTime
				int modifiedTimeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.ModifiedTime);
				if (modifiedTimeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedTimeColumnIndex),
						orderRow.IsModifiedTimeNull() ? (object)null : orderRow.ModifiedTime);

				// ModifiedLoginId
				int modifiedLoginIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.ModifiedLoginId);
				if (modifiedLoginIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginIdColumnIndex),
						orderRow.IsModifiedLoginIdNull() ? (object)null : orderRow.ModifiedLoginId);

				// ModifiedLoginName
				int modifiedLoginNameColumnIndex = this.addressMap.GetColumnIndex(ColumnType.ModifiedLoginName);
				if (modifiedLoginNameColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, modifiedLoginNameColumnIndex),
						orderRow.IsModifiedLoginNameNull() ? (object)null : orderRow.ModifiedLoginName);

			}

		}

		/// <summary>
		/// Gets the calculated fields from an order on the screen.
		/// </summary>
		/// <param name="order">An order record.</param>
		private void GetOrder(OrderSet.OrderRow orderRow)
		{

			// The record can either be a local or global order.  The updating of the line is the same either way, but
			// finding the record is different.  The 'rowIndex' will have the line number in the spreadsheet when the
			// decoding logic has worked out where the line is.  The documentVersion is used in the call to update each
			// cell and will also be extracted from the AddressMap DataSet.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// The Order record passed into this method will indicate whether a local or global record is being 
			// changed.
			if (orderRow.IsLocal)
			{

				// Find the line number and document version based on the local identifier.
				AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByOrderId(orderRow.OrderId);
				if (localMapRow != null)
				{
					rowIndex = localMapRow.RowIndex;
					documentVersion = localMapRow.DocumentVersion;
				}

			}
			else
			{

				// Find the line number and document version based on the global identifier.
				AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByOrderId(orderRow.OrderId);
				if (globalMapRow != null)
				{
					rowIndex = globalMapRow.RowIndex;
					documentVersion = globalMapRow.DocumentVersion;
				}

			}

			// If the row doesn't exist in the mappings, there is nothing to retrieve.
			if (rowIndex == SpreadsheetRow.DoesNotExist)
				return;

			// This method doesn't do anything at present, but this is where we would extract the calculated fields from
			// the order document.  The manually entered values are already part of the data model at this point.

		}

		/// <summary>
		/// Selects a cell in the order document based on the order id and column type.
		/// </summary>
		/// <param name="OrderId">Primary identifier of the record to be selected.</param>
		/// <param name="columnType">Column type of the cell to be selected.</param>
		private void SelectLocalField(int OrderId, int columnType)
		{

			// The record can either be a local or global order.  The 'rowIndex' will have the line number in the
			// spreadsheet when the decoding logic has worked out where the line is.  The documentVersion is used in the
			// call to update each cell and will also be extracted from the AddressMap DataSet.  This section will find
			// the row index and document version for either a local or global order record.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// Find the line number and document version based on the local identifier.
			AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByOrderId(OrderId);
			if (localMapRow != null)
			{
				rowIndex = localMapRow.RowIndex;
				documentVersion = localMapRow.DocumentVersion;
			}

			// If the row and column exist, we can move the selection there.
			if (rowIndex != SpreadsheetRow.DoesNotExist)
			{
				int ColumnIndex = this.addressMap.GetColumnIndex(columnType);
				if (ColumnIndex != SpreadsheetColumn.DoesNotExist)
					Select(new CellAddress(documentVersion, rowIndex, ColumnIndex));

				// An error will invalidate the selected record.
				this.isOrderChanged = true;
				this.isOrderValid = false;

			}

		}

		/// <summary>
		/// Selects a cell in the order document based on the order id and column type.
		/// </summary>
		/// <param name="OrderId">Primary identifier of the record to be selected.</param>
		/// <param name="columnType">Column type of the cell to be selected.</param>
		private void SelectGlobalField(int OrderId, int columnType)
		{

			// The record can either be a local or global order.  The 'rowIndex' will have the line number in the
			// spreadsheet when the decoding logic has worked out where the line is.  The documentVersion is used in the
			// call to update each cell and will also be extracted from the AddressMap DataSet.  This section will find
			// the row index and document version for either a local or global order record.
			int rowIndex = SpreadsheetRow.DoesNotExist;
			int documentVersion = 0;

			// Find the line number and document version based on the global identifier.
			AddressMap.GlobalMapRow globalMapRow = this.addressMap.GlobalMap.FindByOrderId(OrderId);
			if (globalMapRow != null)
			{
				rowIndex = globalMapRow.RowIndex;
				documentVersion = globalMapRow.DocumentVersion;
			}

			// If the row and column exist, we can move the selection there.
			if (rowIndex != SpreadsheetRow.DoesNotExist)
			{
				int ColumnIndex = this.addressMap.GetColumnIndex(columnType);
				if (ColumnIndex != SpreadsheetColumn.DoesNotExist)
					Select(new CellAddress(documentVersion, rowIndex, ColumnIndex));

				// An error will invalidate the selected record.
				this.isOrderChanged = true;
				this.isOrderValid = false;

			}

		}

		/// <summary>
		/// Modifies the spreadsheet row to change a local order id to a global order id.
		/// </summary>
		/// <param name="localOrderId">The identifier that is known only to this client</param>
		/// <param name="globalOrderId">The identifier that is know to all clients of the data model.</param>
		private void GlobalizeOrder(int localOrderId, int globalOrderId)
		{

			// Find the location of the order based on its local identifier.  If it exists, then the identifier will
			// be change and the row type will reflect the new location of the record.
			AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByOrderId(localOrderId);
			if (localMapRow != null)
			{

				// The row index and document version will be used several times below.
				int rowIndex = localMapRow.RowIndex;
				int documentVersion = localMapRow.DocumentVersion;

				// Change the row type to reflect that the record is now located in the global tables.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				if (rowTypeColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, rowTypeColumnIndex), RowType.GlobalOrder);
				
				// Change the order identifier to the global id.
				int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
				if (OrderIdColumnIndex != SpreadsheetColumn.DoesNotExist)
					SetCell(new CellAddress(documentVersion, rowIndex, OrderIdColumnIndex), globalOrderId);

				// If the record that we just updated is also the currently selected on, then update the state values to
				// reflect the new id and row type.
				if (this.selectedOrderId == localOrderId)
				{
					this.selectedOrderId = globalOrderId;
					this.selectedRowType = RowType.GlobalOrder;
				}

				// Add a mapping to the global table to the row that contains the order and remove the record that
				// pointed the way to the local order.
				this.addressMap.GlobalMap.AddGlobalMapRow(globalOrderId, documentVersion, rowIndex);
				localMapRow.Delete();
				localMapRow.AcceptChanges();

			}

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
			// because the stylesheet can change, the column that a particular value shows up in (identifier, Quantity,
			// price, etc) can change also.  So we need a way of mapping the columns to the function they provide.  This
			// object has to be created in the foreground and can only be used by the foreground (message thread) because
			// waiting for events and table locks are prophibited in the foreground.  So the only way to insure that there
			// is no corruption to the table is to limit access to a single thread.
			this.addressMap = new AddressMap(xmlDocument, this.IncrementDocumentVersion());

		}

		/// <summary>
		/// Locks the local tables for writting.
		/// </summary>
		private void LockLocalOrders()
		{

			// Lock the local table.
			this.localOrderSetLock.AcquireWriterLock(CommonTimeout.LockWait);

		}

		/// <summary>
		/// Releases the local table locks.
		/// </summary>
		private void ReleaseLocalOrders()
		{

			// Release the local table
			if (this.localOrderSetLock.IsWriterLockHeld) this.localOrderSetLock.ReleaseWriterLock();

		}
		
		/// <summary>
		/// Locks the global tables for all operations involving orders.
		/// </summary>
		private void LockGlobalOrders()
		{

			// Lock the global tables.
			Debug.Assert(!ClientMarketData.AreLocksHeld);
			ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ConditionLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.OrderLock.AcquireWriterLock(CommonTimeout.LockWait);
			ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
			ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

		}

		/// <summary>
		/// Releases the global table locks.
		/// </summary>
		private void ReleaseGlobalOrders()
		{

			// Release the tables.
			if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
			if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
			if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
			if (ClientMarketData.ConditionLock.IsReaderLockHeld) ClientMarketData.ConditionLock.ReleaseReaderLock();
			if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
			if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
			if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
			if (ClientMarketData.OrderLock.IsWriterLockHeld) ClientMarketData.OrderLock.ReleaseWriterLock();
			if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
			if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
			if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
			if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
			if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
			if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
			Debug.Assert(!ClientMarketData.AreLocksHeld);

		}

		/// <summary>
		/// Finds a order record in the local tables.
		/// </summary>
		/// <param name="OrderId">The unique identifier for a local record.</param>
		/// <returns>A local order record.</returns>
		private LocalOrderSet.OrderRow FindLocalOrder(int OrderId)
		{

			// Find the record in the local tables.
			LocalOrderSet.OrderRow orderRow =
				this.localOrderSet.Order.FindByOrderId(OrderId);

			// Throw an exception if it can't be found.			
			if (orderRow == null)
				throw new Exception(String.Format("Local Order {0} not found.", OrderId));

			// This record can be used to access the local order.
			return orderRow;

		}

		/// <summary>
		/// Finda a order record in the global tables.
		/// </summary>
		/// <param name="OrderId">The unique identifier for a global record.</param>
		/// <returns>A global order record.</returns>
		private ClientMarketData.OrderRow FindGlobalOrder(int OrderId)
		{

			// Find the global record
			ClientMarketData.OrderRow orderRow = ClientMarketData.Order.FindByOrderId(OrderId);

			// Throw an exception if it can't be found.
			if (orderRow == null)
				throw new Exception(String.Format("Global Order {0} not found.", OrderId));

			// This record can be used to access the global order.
			return orderRow;

		}
		
		/// <summary>
		/// Selects a default value for a broker.
		/// </summary>
		/// <returns>The default broker id for this order, null if the broker is not determined yet.</returns>
		private object GetDefaultBrokerId()
		{

			// This indicates that the broker for the order can't be determined.  This should be replaced with logic 
			// that extracts directed broker information from the block order.
			return null;

		}

		/// <summary>
		/// Handles field errors to local records.
		/// </summary>
		/// <param name="localOrderException">Exception information for a local field error.</param>
		private void LocalFieldExceptionHandler(LocalFieldException localOrderException)
		{

			// This will cause all record validation command to be ignored until it is set again.  Because of the ledger 
			// style input, a single action can cause both a field and record validation errors.  For example, hitting the
			// 'down arrow' key will move the selection to the next record at the same time it creates a command to
			// validate the the field what was being edited.  This can cause a field validation error (e.g. the field was
			// entered incorrectly), and will also generate a record error (e.g. the same field in the record may not have
			// a valid entry).
			this.isFieldValid = false;
			
			// Force the selection back to the offending field of the offending record.
			Invoke(this.selectLocalFieldDelegate,
				new object[] {localOrderException.RecordId, localOrderException.FieldId});

			// Display the error.
			MessageBox.Show(this, resourceManager.GetString(localOrderException.StringId),
				resourceManager.GetString("quasarError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

			// All record-level validation that is in the command queue between the current command and this new command 
			// will be ignored.
			CommandQueue.Execute(new ThreadHandler(ValidateFieldCommand));
			
		}

		/// <summary>
		/// Handles field errors to global records.
		/// </summary>
		/// <param name="globalOrderException">Exception information for a global field error.</param>
		private void GlobalFieldExceptionHandler(GlobalFieldException globalOrderException)
		{

			// This will cause all record validation command to be ignored until it is set again.  Because of the ledger 
			// style input, a single action can cause both a field and record validation errors.  For example, hitting the
			// 'down arrow' key will move the selection to the next record at the same time it creates a command to
			// validate the the field what was being edited.  This can cause a field validation error (e.g. the field was
			// entered incorrectly), and will also generate a record error (e.g. the same field in the record may not have
			// a valid entry).
			this.isFieldValid = false;
			
			// Force the selection back to the offending field of the offending record.
			Invoke(this.selectGlobalFieldDelegate,
				new object[] {globalOrderException.RecordId, globalOrderException.FieldId});

			// Display the error.
			MessageBox.Show(this, resourceManager.GetString(globalOrderException.StringId),
				resourceManager.GetString("quasarError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				
			// All record-level validation that is in the command queue between the current command and this new command 
			// will be ignored.
			CommandQueue.Execute(new ThreadHandler(ValidateFieldCommand));
			
		}

		/// <summary>
		/// Opens the viewer.
		/// </summary>
		public void OpenViewerCommand(params object[] argument)
		{


			try
			{

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanged += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Broker.BrokerRowChanged += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Broker.BrokerRowDeleted += new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged += new ClientMarketData.OrderRowChangeEventHandler(this.OrderChangedEvent);
				ClientMarketData.Order.OrderRowDeleted += new ClientMarketData.OrderRowChangeEventHandler(this.OrderChangedEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetChangedEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetChangedEvent);
				ClientMarketData.EndMerge += new EventHandler(this.EndMerge);

			}
			finally
			{
					
				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Closes the Order Viewer
		/// </summary>
		public void CloseViewerCommand(params object[] argument)
		{

			try
			{

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanged -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Broker.BrokerRowChanged -= new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Broker.BrokerRowDeleted -= new ClientMarketData.BrokerRowChangeEventHandler(this.BrokerRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderChangedEvent);
				ClientMarketData.Order.OrderRowDeleted -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderChangedEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetChangedEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetChangedEvent);
				ClientMarketData.EndMerge -= new EventHandler(this.EndMerge);

			}
			finally
			{
					
				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the viewer is now closed.
			OnEndCloseViewer();

		}

		/// <summary>
		/// Opens the Order Document
		/// </summary>
		private void OpenDocumentCommand(params object[] argument)
		{

			// The key to this command is a blotter identifier.
			int blotterId = (int)argument[0];

			try
			{

				// This identifies the blotter this document belongs to.
				this.BlotterId = blotterId;

				// Lock the tables used.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// HACK - Stylesheet is taken from the first order form.  This should be driven by some decision about
				// what kind of instrument is being added.
				foreach (ClientMarketData.StylesheetRow stylesheetRow in ClientMarketData.Stylesheet)
					if (stylesheetRow.StylesheetTypeCode == StylesheetType.OrderForm)
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

				// Create an empty order document.
				OrderDocument orderDocument = new OrderDocument();

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed into
				// the spreadsheet control directly, when there is a major format change, or will be updated incrementally
				// when only the content has changed.
				XmlDocument spreadsheetDocument = new XmlDocument();
				spreadsheetDocument.Load(xslTransform.Transform(orderDocument, null, (XmlResolver)null));

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				InvokeCreateAddressMap(spreadsheetDocument);

				// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet
				// control.  We can invoke the foreground thread to move the data into the control.
				SetDocument(spreadsheetDocument);

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
		/// Close the Order Document.
		/// </summary>
		private void CloseDocumentCommand(params object[] argument)
		{

			// Call the base class to close the document.
			OnEndCloseDocument();

		}
		
		/// <summary>
		/// Displays an order report.
		/// </summary>
		/// <param name="BlockOrderId">The block order of the orders.</param>
		/// <param name="order">Optional temporary order.</param>
		private void DrawCommand(params object[] argument)
		{

			// Extract the command argument.
			int BlockOrderId = (int)argument[0];
			LocalOrderSet localOrderSet = (LocalOrderSet)argument[1];

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the block order still exists.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(BlockOrderId);
				if (blockOrderRow == null && BlockOrderId != 0)
					throw new Exception(String.Format("Block Order {0} has been deleted.", BlockOrderId));

				// Create the order document.
				OrderDocument orderDocument = new OrderDocument(BlockOrderId, this.localOrderSet);

#if DEBUG
				// Make sure that an writing the debug file doesn't disturb the session.
				try
				{
					
					// Dump the DOM to a file.  This is very useful when debugging.
					orderDocument.Save("orderDOM.xml");

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
				XmlReader xmlReader = xslTransform.Transform(orderDocument, xsltArgumentList, (XmlResolver)null);
				spreadsheetDocument.Load(xmlReader);

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				InvokeCreateAddressMap(spreadsheetDocument);

#if DEBUG
				// Make sure that an writing the debug file doesn't disturb the session.
				try
				{
					
					// Write out the spreadsheet document when debugging.
					spreadsheetDocument.Save("OrderSpreadsheet.xml");

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
					
				// Release the locks obtained to produce the order report.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Command to indicate that a field exception has been processed.
		/// </summary>
		/// <remarks>A field validation error can confuse the stream of commands because of the ledger style of input. For
		/// example, if the user were to type in an invalid field, and the selection were to be moved to the next record,
		/// we have a situation where the field can cause an error and, because that field is bad, the record validation
		/// also causes an error message.  To prevent more than one error message from popping up, record validation is
		/// disabled when the field exception handler executes.  The record-level validation is re-enabled by placing 
		/// this message in the command queue.</remarks>
		private void ValidateFieldCommand(params object[] argument)
		{

			// This command will clear the field validation flag after an error has occurred.  All record validation
			// commands that execute between the field error and this command will be ignored.  This prevents the
			// situation where an action may generate both a field validation error and a record validation error.  It's
			// confusing for the user to have to resond to both errors, especially when they may be for the same action.
			this.isFieldValid = true;

		}
		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="BlockOrderId">The primary identifier of the object to open.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public void OpenBlockOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int blockOrderId = (int)argument[0];

			// This will force an empty order and order document to appear until we get some data.  It's the same
			// as forcing a drawing of the headers only.
			this.BlockOrderId = blockOrderId;

			// Clear any previous selected record.
			this.isOrderSelected = false;

			// This will reset the viewer to the home position when it opens.
			this.ResetViewer = true;
			
			// Draw the document without any temporary positions.
			DrawCommand(BlockOrderId, null);

		}

		/// <summary>
		/// Initialize an order row.
		/// </summary>
		/// <returns></returns>
		private void InitializeOrderCommand(params object[] argument)
		{

			// If a field validation error is being processed, then ignore a request to validate the record.
			if (!this.isFieldValid) return;

			// This record will be filled in and passed to the foreground to be placed in the viewer.
			OrderSet.OrderRow orderRow = null;

			try
			{

				// Lock the tables.
				LockLocalOrders();
				LockGlobalOrders();

				// Search for a default broker.  A directed order will set this value.  If the order isn't directed, then the 
				// most recent order will define the value.  If there are no orders, then the last order is used.
				// If none of these are correct, well, the user can put his own d*mned broker symbol in.
				object brokerId = GetDefaultBrokerId();

				// The local tables maintain a concept of block orders.  This makes it easy to find and group the local
				// orders.  This will add a block to the local tables if it doesn't exist.  After the block is 
				// created, orders can be associated with a 'block id'.
//				LocalOrderSet.BlockOrderRow blockOrderRow = this.localOrderSet.BlockOrder.FindByBlockOrderId(this.BlockOrderId);
//				if (blockOrderRow == null)
//				{
//					blockOrderRow = this.localOrderSet.BlockOrder.AddBlockOrderRow(this.BlockOrderId);
//					blockOrderRow.AcceptChanges();
//				}

				// Initialize the new record.  Note that we use a Quantity value that can be initialized from the outside.
				// This was done because there were no other methods to pass the value in.  It was highly desirable to 
				// have a single point where the new orders are created.  This ended up being the 'Select' logic,
				// because the user could 'Select' the placeholder row.  Also note that the order id for this element
				// is an artificial construct.  We use negative numbers to keep the new orders distinct from the read
				// ones on the middle tier.  However, it's useful for constructing the report to have temporary order
				// records until the server gets around to creating the records and handing us back a real id to use.
				LocalOrderSet.OrderRow localOrder = this.localOrderSet.Order.NewOrderRow();
				localOrder.BlockOrderId = this.BlockOrderId;
				if (brokerId != null) localOrder.BrokerId = (int)brokerId;
				this.localOrderSet.Order.AddOrderRow(localOrder);
				localOrder.AcceptChanges();

				// A record needs to be passed on to the foreground so the sheet can be updated.  The record needs to be
				// created while the tables are still locked so ancillary tables can be used.  Once the structure has been
				// filled in, it can be used by the foreground without having to reference the tables.
				orderRow = Order.Create(localOrder);

			}
			finally
			{

				// Release the tables
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

			// At this point a blank order has been created in the local table.  Redraw the document with the local
			// records.
			DrawCommand(this.BlockOrderId, this.localOrderSet);

			// Call the foreground to insert a new order record based on the partial record constructed above.
			if (orderRow != null)
				Invoke(this.initializeRowDelegate, new object[] {orderRow});

		}

		/// <summary>
		/// Updates a local order and spawns a thread to update the server.
		/// </summary>
		/// <param name="OrderId">Primary identifier of the record.</param>
		private void ValidateLocalOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			// If a field validation error is being processed, then ignore a request to validate the record.
			if (!this.isFieldValid) return;

			try
			{

				// Lock the table
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local table.  Note that there is no explicit check for the existence of the
				// order. Since it is local, there is no chance that an exogenic event could erase it, so the only
				// reason the order wouldn't be found is from programmer error.
				LocalOrderSet.OrderRow localOrder = FindLocalOrder(OrderId);

				// Rule #1: Quantity must be provided.
				if (localOrder.IsQuantityNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Quantity,
						"invalidQuantity");

				// Rule #2: Orders must have a Time In Force Instruction
				if (localOrder.IsTransactionTypeCodeNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.TransactionTypeMnemonic,
						"invalidTransactionTypeMnemonic");

				// Rule #3: Orders must have a valid account
				if (localOrder.IsAccountIdNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.AccountMnemonic,
						"invalidAccountMnemonic");

				// Rule #4: Orders must have a valid security
				if (localOrder.IsSecurityIdNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.SecuritySymbol,
						"invalidSecuritySymbol");

				// Rule #5: Quantity must be greater than zero.
				if (localOrder.Quantity < 0.0M)
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Quantity,
						"negativeQuantity");

				// Rule #6: Orders must have a Time In Force Instruction
				if (localOrder.IsTimeInForceCodeNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.TimeInForceMnemonic,
						"invalidTimeInForceMnemonic");

				// Rule #7: Orders must have a valid Order Type Instruction
				if (localOrder.IsOrderTypeCodeNull())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.OrderTypeMnemonic,
						"invalidOrderTypeMnemonic");

				// Rule #8: A market price instruction can't have a limit or stop loss price.
				if (localOrder.OrderTypeCode == OrderType.Market && !localOrder.IsPrice1Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price1,
						"invalidMarketPrice1");

				// Rule #9: A market price instruction can't specify a stop limit price.
				if (localOrder.OrderTypeCode == OrderType.Market && !localOrder.IsPrice2Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price2,
						"invalidMarketPrice2");

				// Rule #10: A limit price instruction must specify a limit price.
				if (localOrder.OrderTypeCode == OrderType.Limit && localOrder.IsPrice1Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price1,
						"missingLimitPrice1");

				// Rule #11: A limit price instruction must not specify a stop limit price.
				if (localOrder.OrderTypeCode == OrderType.Limit && !localOrder.IsPrice2Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price2,
						"invalidLimitPrice2");

				// Rule #12: A stop loss instruction must specify a limit price.
				if (localOrder.OrderTypeCode == OrderType.StopLoss && localOrder.IsPrice1Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price1,
						"missingStopLossPrice1");

				// Rule #13: A stop loss instruction must not specify a stop limit price.
				if (localOrder.OrderTypeCode == OrderType.StopLoss && !localOrder.IsPrice2Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price2,
						"invalidStopLossPrice2");

				// Rule #14: A stop limit must have a stop price
				if (localOrder.OrderTypeCode == OrderType.StopLimit && localOrder.IsPrice1Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price1,
						"missingStopLimitPrice1");

				// Rule #15: A stop limit must have a limit price
				if (localOrder.OrderTypeCode == OrderType.StopLimit && localOrder.IsPrice2Null())
					throw new LocalFieldException(localOrder.OrderId, ColumnType.Price2,
						"missingStopLimitPrice2");

				// Create a order record that is independant of the local database to be passed on to the background
				// thread.  The queueuing of the threads acts as a 'stack' for the changes made to a local record.  That
				// is why it's important to capture the 'state' of the order record at the time it's modified.
				OrderSet.OrderRow orderRow = Order.Create(localOrder);

				// Call the foreground to update the viewer with the new local record.
				Invoke(this.getOrderDelegate, new object[] {orderRow});

				if (this.IsLedger)
				{

					// If there is no mapping of a local id to a global id, then an 'Insert' operation is generated for the
					// order.  If a mapping exists, the order has been created, but changes were made to the record
					// before a global identifier could be assigned.  If that is the case, the mapping is used to switch
					// identifiers and a modification is made to the global record.  This allows the application to tolerate
					// long delays to the server without impacting the user.
					if (this.localOrderSet.OrderMap.FindByLocalId(localOrder.OrderId) == null)
					{

						// Add the record using a service queue rather than the command queue.
						ServiceQueue.Execute(new ThreadHandler(InsertLocalOrderThread), orderRow);

					}
					else
					{

						// Update the record using the service queue rather than the command queue.
						ServiceQueue.Execute(new ThreadHandler(UpdateLocalOrderThread), orderRow);

					}

				}

			}
			catch (LocalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				LocalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Synchronizes a record in the local data model with the server.
		/// </summary>
		/// <param name="OrderId">Identifies the record to be updated.</param>
		private void ValidateGlobalOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			// If a field validation error is being processed, then ignore a request to validate the record.
			if (!this.isFieldValid) return;

			try
			{

				// Lock the tables needed.
				LockGlobalOrders();

				// Find the global record.
				ClientMarketData.OrderRow globalOrder = FindGlobalOrder(OrderId);

				// Rule #1: Make sure the order hasn't been deleted.
				if (globalOrder == null)
					throw new Exception(String.Format("Order {0} has been deleted.", OrderId));

				// Rule #2: Quantity must be greater than zero.
				if (globalOrder.Quantity < 0.0M)
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Quantity, "negativeQuantity");

				// Rule #3: A market price instruction can't have a limit or stop loss price.
				if (globalOrder.OrderTypeCode == OrderType.Market && !globalOrder.IsPrice1Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price1,
						"invalidMarketPrice1");

				// Rule #4: A market price instruction can't specify a stop limit price.
				if (globalOrder.OrderTypeCode == OrderType.Market && !globalOrder.IsPrice2Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price2,
						"invalidMarketPrice2");

				// Rule #5: A limit price instruction must specify a limit price.
				if (globalOrder.OrderTypeCode == OrderType.Limit && globalOrder.IsPrice1Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price1,
						"missingLimitPrice1");

				// Rule #6: A limit price instruction must not specify a stop limit price.
				if (globalOrder.OrderTypeCode == OrderType.Limit && !globalOrder.IsPrice2Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price2,
						"invalidLimitPrice2");

				// Rule #7: A stop loss instruction must specify a limit price.
				if (globalOrder.OrderTypeCode == OrderType.StopLoss && globalOrder.IsPrice1Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price1,
						"missingStopLossPrice1");

				// Rule #8: A stop loss instruction must not specify a stop limit price.
				if (globalOrder.OrderTypeCode == OrderType.StopLoss && !globalOrder.IsPrice2Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price2,
						"invalidStopLossPrice2");

				// Rule #9: A stop limit must have a stop price
				if (globalOrder.OrderTypeCode == OrderType.StopLimit && globalOrder.IsPrice1Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price1,
						"missingStopLimitPrice1");

				// Rule #10: A stop limit must have a limit price
				if (globalOrder.OrderTypeCode == OrderType.StopLimit && globalOrder.IsPrice2Null())
					throw new LocalFieldException(globalOrder.OrderId, ColumnType.Price2,
						"missingStopLimitPrice2");

				// Don't bother synchronizing with either the viewer or the server if the record hasn't changed.  Note 
				// that the record can only be changed in the command threads.
				if (globalOrder.RowState != DataRowState.Unchanged)
				{

					// Create a record that is not dependant on the global data model.  This record can be passed into 
					// other threads without the need to lock tables.
					OrderSet.OrderRow orderRow = Order.Create(globalOrder);

					// Update the viewer with the independant record.
					Invoke(this.getOrderDelegate, new object[] {orderRow});

					// This thread will update the server with the data-model independant version of the order record.
					if (this.IsLedger)
						ServiceQueue.Execute(new ThreadHandler(UpdateGlobalOrderThread), orderRow);

				}

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Deletes a local order record.
		/// </summary>
		/// <param name="OrderId"></param>
		private void DeleteLocalOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			// Make sure that the table locks are released.		
			try
			{

				// Lock the local tables.
				LockLocalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = this.localOrderSet.Order.FindByOrderId(OrderId);
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} doesn't exist", orderRow.OrderId.ToString()));

				// Purge the row from the temporary tables.
				orderRow.Delete();
				orderRow.AcceptChanges();

				// Use the command thread to redraw this document.
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

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
				
				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the local tables.
				ReleaseLocalOrders();

			}

		}

		/// <summary>
		/// Deletes an order from the shared data model.
		/// </summary>
		/// <param name="OrderId">Primary identifier of the order.</param>
		private void DeleteGlobalOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			// Make sure that the table locks are released.		
			try
			{

				// Lock the table while we make our changes.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.OrderLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Find the record in the global tables.
				ClientMarketData.OrderRow orderRow = ClientMarketData.Order.FindByOrderId(OrderId);
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} doesn't exist", orderRow.OrderId.ToString()));

				// The thread that interacts with the server will confirm the deletion by removing the record completely
				// from the local tables.  If there is an error with the server, then the transaction is rolled back.  The
				// model is to do a 'soft' delete here, in the quick response thread.  That way we can redraw the screen
				// quickly and give the user some indication that the record was deleted.  If there is a problem on the
				// server -- for instance, a concurrency check fails -- then the thread that deals with the server will
				// roll the transaction back and redraw the document.
				DeleteArgs deleteOrderArgs = new DeleteArgs(orderRow.RowVersion, orderRow.OrderId, orderRow);

				// Delete the record from the local tables.  We don't accept the changes yet until the background thread
				// has confirmed the deletion with the server.
				orderRow.Delete();

				// Queue up a request for another, longer lasting thread to take care of the interaction with the server.
				ServiceQueue.Execute(new ThreadHandler(DeleteGlobalOrderThread), deleteOrderArgs);

				// Use the command thread to redraw this document.
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

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
				
				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.OrderLock.IsWriterLockHeld) ClientMarketData.OrderLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Clears the local table of unused orders.
		/// </summary>
		/// <returns>true to indicate that unused records were removed.</returns>
		private void ClearLocalOrdersCommand(params object[] argument)
		{

			// This will be set to true if the local table is modified.  This can be used by the calling method to
			// indicate that a redraw of the document is needed.
			bool isChanged = false;

			try
			{

				// Lock the local tables.
				LockLocalOrders();

				// This array will hold the records that are to be deleted.
				ArrayList arrayList = new ArrayList();
				
				// Find the local block order that is used to group the local orders in this window.  Then cycle through
				// all those local records and mark them for deletion if the Quantity was never set.  Note that the elements
				// can't be deleted in this operation because the iterator would be corrupted.
				foreach (LocalOrderSet.OrderRow orderRow in this.localOrderSet.Order)
					if (orderRow.IsQuantityNull())
						arrayList.Add(orderRow);

				// Delete each of the records that was marked for deletion.
				foreach (LocalOrderSet.OrderRow orderRow in arrayList)
				{

					// We'll keep the local record if someone bothered to enter a Quantity.  The 'isChanged' flag will
					// let the caller know that the data behind the document has changed.
					orderRow.Delete();
					orderRow.AcceptChanges();
					isChanged = true;

				}

			}
			finally
			{

				// Release the local tables.
				this.ReleaseLocalOrders();

			}

			// Return an indicator that shows whether the local table was modified by this method.  This is generally used
			// to trigger a redraw of the document when the focus is leaving.
			if (isChanged)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

		}

		/// <summary>
		/// Set the account id of a local order record based on the symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="accountSymbol">Symbolic name of the account.</param>
		private void SetLocalAccountIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A account must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.AccountMnemonic, "invalidAccountSymbol");
				
				// Update the account id in the local table.
				orderRow.AccountId = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
	
		/// <summary>
		/// Set the account id of a global order record based on a symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="accountSymbol">Symbolic name of the account.</param>
		private void SetGlobalAccountIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Local Order {0} has been deleted", OrderId));

				// Rule #2: A account must be specified.
				if (argument[1] == null)
					throw new GlobalFieldException(OrderId, ColumnType.AccountMnemonic, "invalidAccountSymbol");
				
				// Update the account id in the global table.
				orderRow.AccountId = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Set the security id of a local order record based on the symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="securitySymbol">Symbolic name of the security.</param>
		private void SetLocalSecurityIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A security must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSecuritySymbol");
				
				// Rule #2: The security must still exist in the data model.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId((int)argument[1]);
				if (securityRow == null)
					throw new LocalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSecuritySymbol");

				// Rule #3: The order must have a valid settlement security.
				// A settlement security is needed for the order.  For many securities (equities, fixed income, options), the
				// settlement security is a relationship that is fixed ahead of time.  Other transactions,
				// such as currency transactions, convertibles and SWAPs, require the user to specify the settlement
				// security.  This will find the settlement security for most securities.
				object settlementId = null;

				foreach (ClientMarketData.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
					settlementId = equityRow.SettlementId;
				foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
					settlementId = debtRow.SettlementId;

				if (settlementId == null)
					throw new LocalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSettlementSecurity");

				// Update the security id in the local table.
				orderRow.SecurityId = (int)argument[1];
				orderRow.SettlementId = (int)settlementId;

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
	
		/// <summary>
		/// Set the security id of a global order record based on a symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="securitySymbol">Symbolic name of the security.</param>
		private void SetGlobalSecurityIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Local Order {0} has been deleted", OrderId));

				// Rule #2: A security must be specified.
				if (argument[1] == null)
					throw new GlobalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSecuritySymbol");
				
				// Rule #3: The security must still exist in the data model.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId((int)argument[1]);
				if (securityRow == null)
					throw new LocalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSecuritySymbol");

				// Rule #4: The order must have a valid settlement security.
				// A settlement security is needed for the order.  For many securities (equities, fixed income, options), the
				// settlement security is a relationship that is fixed ahead of time.  Other transactions,
				// such as currency transactions, convertibles and SWAPs, require the user to specify the settlement
				// security.  This will find the settlement security for most securities.
				object settlementId = null;

				foreach (ClientMarketData.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
					settlementId = equityRow.SettlementId;
				foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
					settlementId = debtRow.SettlementId;

				if (settlementId == null)
					throw new LocalFieldException(OrderId, ColumnType.SecuritySymbol, "invalidSettlementSecurity");

				// Update the security id and the settlement security in the global table.
				orderRow.SecurityId = (int)argument[1];
				orderRow.SettlementId = (int)settlementId;

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Set the broker id of a local order record based on the symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetLocalBrokerIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A broker must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.BrokerSymbol, "invalidBrokerSymbol");
				
				// Update the broker id in the local table.
				orderRow.BrokerId = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
	
		/// <summary>
		/// Set the broker id of a global order record based on a symbol.
		/// </summary>
		/// <param name="OrderId">Identifer of the order.</param>
		/// <param name="brokerSymbol">Symbolic name of the broker.</param>
		private void SetGlobalBrokerIdCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Local Order {0} has been deleted", OrderId));

				// Rule #2: A broker must be specified.
				if (argument[1] == null)
					throw new GlobalFieldException(OrderId, ColumnType.BrokerSymbol, "invalidBrokerSymbol");
				
				// Update the broker id in the global table.
				orderRow.BrokerId = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Set the Time In Force Instruction for a local order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="transactionTypeCode">TransactionTypeCode executed.</param>
		private void SetLocalTransactionTypeCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A Time In Force must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.TransactionTypeMnemonic,
						"invalidTransactionTypeMnemonic");
				
				// Update the Time In Force Code.
				orderRow.TransactionTypeCode = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the Time In Force Instruction for a local order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="transactionTypeCode">TransactionTypeCode executed.</param>
		private void SetGlobalTransactionTypeCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A Time In Force must be specified.
				if (argument[1] == null)
					throw new GlobalFieldException(OrderId, ColumnType.TransactionTypeMnemonic,
						"invalidTransactionTypeMnemonic");
				
				// Update the Time In Force Code.
				orderRow.TransactionTypeCode = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				GlobalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the Time In Force Instruction for a local order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="timeInForceCode">TimeInForceCode executed.</param>
		private void SetLocalTimeInForceCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: A Time In Force must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.TimeInForceMnemonic,
						"invalidTimeInForceMnemonic");
				
				// Update the Time In Force Code.
				orderRow.TimeInForceCode = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the Time In Force Instruction for a global order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="timeInForceCode">TimeInForceCode executed.</param>
		private void SetGlobalTimeInForceCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} has been deleted", OrderId));

				// Rule #2: A time-in-force must be specified.
				if (argument[1] == argument[1])
					throw new GlobalFieldException(OrderId, ColumnType.TimeInForceMnemonic,
						"invalidTimeInForceMnemonic");
				
				// Update the global record
				orderRow.TimeInForceCode = (int)argument[1];

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the OrderTypeCode of a local order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="OrderTypeCode">OrderTypeCode executed.</param>
		private void SetLocalOrderTypeCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Rule #1: An order type code must be specified.
				if (argument[1] == null)
					throw new LocalFieldException(OrderId, ColumnType.OrderTypeMnemonic,
						"invalidOrderTypeMnemonic");
				
				// Update the OrderTypeCode
				orderRow.OrderTypeCode = (int)argument[1];

				// Clear out the unused price fields based on the type of order pricing instructions.
				switch (orderRow.OrderTypeCode)
				{

					case OrderType.Market:

						// A market order doesn't require any prices to be specified.
						orderRow.SetPrice1Null();
						orderRow.SetPrice2Null();
						break;

					case OrderType.Limit:
					case OrderType.StopLoss:

						// A limit order and stop loss order require a single price.
						orderRow.SetPrice2Null();
						break;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the OrderTypeCode of a global order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="OrderTypeCode">OrderTypeCode executed.</param>
		private void SetGlobalOrderTypeCodeCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} has been deleted", OrderId));

				// Rule #2: A price instruction must be specified.
				if (argument[1] == null)
					throw new GlobalFieldException(OrderId, ColumnType.OrderTypeMnemonic,
						"invalidOrderTypeMnemonic");
				
				// Update the global record
				orderRow.OrderTypeCode = (int)argument[1];

				// Clear out the unused price fields based on the type of order pricing instructions.
				switch (orderRow.OrderTypeCode)
				{

					case OrderType.Market:

						// A market order doesn't require any prices to be specified.
						orderRow.SetPrice1Null();
						orderRow.SetPrice2Null();
						break;

					case OrderType.Limit:
					case OrderType.StopLoss:

						// A limit order and stop loss order require a single price.
						orderRow.SetPrice2Null();
						break;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Set the Quantity of a local order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="Quantity">Quantity executed.</param>
		private void SetLocalQuantityCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// Convert the Quantity string to a decimal number.
				decimal Quantity = Convert.ToDecimal(argument[1]);

				// Rule #1: Quantities must be positive.
				if (Quantity < 0.0M)
					throw new LocalFieldException(OrderId, ColumnType.Quantity, "negativeQuantity");
			
				// Update the Quantity
				orderRow.Quantity = Quantity;

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(new LocalFieldException(OrderId, ColumnType.Quantity, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the Quantity of a global order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="Quantity">Quantity executed.</param>
		private void SetGlobalQuantityCommand(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Convert the Quantity string to a decimal number.
				decimal Quantity = Shadows.Quasar.Common.Quantity.Parse((string)argument[1]);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} has been deleted", OrderId));

				// Rule #2: Quantities must be positive.
				if (Quantity < 0.0M)
					throw new GlobalFieldException(OrderId, ColumnType.Quantity, "negativeQuantity");
				
				// Update the global record
				orderRow.Quantity = Quantity;

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				GlobalFieldExceptionHandler(new GlobalFieldException(OrderId, ColumnType.Price1, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the price1 of a local order.
		/// </summary>
		/// <param name="argument">Generic command argument.</param>
		private void SetLocalPrice1Command(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field 
				// needs to be turned into a price.
				if (argument[1] == null)
					orderRow.SetPrice1Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price1 = Convert.ToDecimal(argument[1]);

					// Rule #1: Price must be positive.
					if (price1 < 0.0M)
						throw new LocalFieldException(OrderId, ColumnType.Price1, "negativePrice");
				
					// Update the price1
					orderRow.Price1 = price1;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(new LocalFieldException(OrderId, ColumnType.Price1, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the price1 of a global order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="price1">Price1 executed.</param>
		private void SetGlobalPrice1Command(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} has been deleted", OrderId));

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field
				// needs to be turned into a price.
				if (argument[1] == null)
					orderRow.SetPrice1Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price1 = Convert.ToDecimal(argument[1]);

					// Rule #2: Price must be positive.
					if (price1 < 0.0M)
						throw new GlobalFieldException(OrderId, ColumnType.Price1, "negativePrice");
				
					// Update the price1
					orderRow.Price1 = price1;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(new LocalFieldException(OrderId, ColumnType.Price1, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the price2 of a local order.
		/// </summary>
		/// <param name="argument">Generic command argument.</param>
		private void SetLocalPrice2Command(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockLocalOrders();
				LockGlobalOrders();

				// Find the record in the local tables.
				LocalOrderSet.OrderRow orderRow = FindLocalOrder(OrderId);

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field 
				// needs to be turned into a price.
				if (argument[1] == null)
					orderRow.SetPrice2Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price2 = Convert.ToDecimal(argument[1]);

					// Rule #1: Price must be positive.
					if (price2 < 0.0M)
						throw new LocalFieldException(OrderId, ColumnType.Price2, "negativePrice");
				
					// Update the price2
					orderRow.Price2 = price2;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (LocalFieldException localFieldException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(localFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(new LocalFieldException(OrderId, ColumnType.Price2, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// Set the price2 of a global order.
		/// </summary>
		/// <param name="OrderId">Identifier of the record.</param>
		/// <param name="price2">Price2 executed.</param>
		private void SetGlobalPrice2Command(params object[] argument)
		{

			// Extract the command argument.
			int OrderId = (int)argument[0];

			try
			{

				// Aquire the locks.
				LockGlobalOrders();

				// Find the global record
				ClientMarketData.OrderRow orderRow = FindGlobalOrder(OrderId);

				// Rule #1: The record must exist.
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} has been deleted", OrderId));

				// If the field is being cleared, there are no checks, just clear it out.  Otherwise, the input field
				// needs to be turned into a price.
				if (argument[1] == null)
					orderRow.SetPrice2Null();
				else
				{

					// Convert the price string to a decimal number.
					decimal price2 = Convert.ToDecimal(argument[1]);

					// Rule #2: Price must be positive.
					if (price2 < 0.0M)
						throw new GlobalFieldException(OrderId, ColumnType.Price2, "negativePrice");
				
					// Update the price2
					orderRow.Price2 = price2;

				}

				// Create a record that is not dependant on the data model and update the viewer with it.
				Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

			}
			catch (GlobalFieldException globalFieldException)
			{

				// Call a common method to handle all the exceptions when setting global fields.
				GlobalFieldExceptionHandler(globalFieldException);

			}
			catch (System.FormatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				LocalFieldExceptionHandler(new LocalFieldException(OrderId, ColumnType.Price2, "incorrectFormat"));

			}
			finally
			{

				// Release the locks.
				ReleaseGlobalOrders();

			}

		}
		
		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void InsertLocalOrderThread(params object[] argument)
		{

			// Extract the initialization parameters.
			OrderSet.OrderRow orderRow = (OrderSet.OrderRow)argument[0];

			// An insert will modify these records.  If there's an error, those changes will be rolled back and the next
			// synchronization of the data model will take care of the rest.
			LocalOrderSet.OrderRow localOrder = null;
			LocalOrderSet.OrderMapRow orderMapRow = null;

			try
			{

				// Lock the tables.
				LockLocalOrders();

				// Add the order.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
				RemoteType orderType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Order");
				RemoteMethod remoteMethod = orderType.Methods.Add("Insert");
				remoteMethod.Parameters.Add("OrderId", DataType.Int, Direction.ReturnValue);
				remoteMethod.Parameters.Add("blotterId", this.BlotterId);
				remoteMethod.Parameters.Add("accountId", orderRow.AccountId);
				remoteMethod.Parameters.Add("securityId", orderRow.SecurityId);
				remoteMethod.Parameters.Add("settlementId", orderRow.SettlementId);
				remoteMethod.Parameters.Add("brokerId", orderRow.IsBrokerIdNull() ? (object)DBNull.Value : orderRow.BrokerId);
				remoteMethod.Parameters.Add("transactionTypeCode", orderRow.TransactionTypeCode);
				remoteMethod.Parameters.Add("timeInForceCode", orderRow.TimeInForceCode);
				remoteMethod.Parameters.Add("OrderTypeCode", orderRow.OrderTypeCode);
				remoteMethod.Parameters.Add("conditionCode", orderRow.IsConditionCodeNull() ? (object)DBNull.Value : orderRow.ConditionCode);
				remoteMethod.Parameters.Add("Quantity", orderRow.Quantity);
				remoteMethod.Parameters.Add("price1", orderRow.IsPrice1Null() ? (object)DBNull.Value: orderRow.Price1);
				remoteMethod.Parameters.Add("price2", orderRow.IsPrice2Null() ? (object)DBNull.Value: orderRow.Price2);
				remoteMethod.Parameters.Add("note", orderRow.IsNoteNull() ? (object)DBNull.Value: orderRow.Note);
				ClientMarketData.Execute(remoteBatch);

				// Create a mapping between the local record and it's global equivalent.  The 'ValidateLocalPlacementCommand'
				// procedure will check to see if a mapping has been made between the local id and a global id before the local
				// updates are made.  The idea is to allow for a record to be Modified between the time it's Created locally and
				// the time the database gets around to finally assigning an id.
				orderMapRow = this.localOrderSet.OrderMap.NewOrderMapRow();
				orderMapRow.LocalId = orderRow.OrderId;
				orderMapRow.GlobalId = (int)remoteMethod.Parameters["orderId"].Value;
				this.localOrderSet.OrderMap.AddOrderMapRow(orderMapRow);

				// Once there is a confirmation that the local record has been created, the local version of this record
				// can be deleted.  While there may be changes in the thread queue to the original record, those changes
				// are self contained and will be executed in the order they were placed in the queue.
				localOrder = FindLocalOrder(orderRow.OrderId);
				localOrder.Delete();

				// At this point, all changes to the data model can be accepted.  They will be refreshed on the next data
				// model update, but in the mean time, they can be used locally.
				orderMapRow.AcceptChanges();
				localOrder.AcceptChanges();

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
				
				// Rollback the changes made to the data model
				if (orderMapRow != null) orderMapRow.RejectChanges();
				if (orderRow != null) orderRow.RejectChanges();

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks
				this.ReleaseLocalOrders();

			}

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void UpdateLocalOrderThread(params object[] argument)
		{

			// Extract the initialization parameters.
			OrderSet.OrderRow orderRow = (OrderSet.OrderRow)argument[0];

			// This record is rolled back if there's an error.
			ClientMarketData.OrderRow globalOrder = null;

			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the tables.
				LockLocalOrders();
				LockGlobalOrders();

				// This method is called on the rare occation where a record has been created but not yet confirmed with 
				// the server.  In this situation, a user may modify the local record.  Since the service threads are
				// queued, just like the command threads, we are guaranteed that the thread to insert the order will
				// execute before this one.  The only task remaining is to take the mapping created when the record is
				// acknowledged by the server, and swap it for the local id which was in use when this record was created.
				LocalOrderSet.OrderMapRow orderMapRow =
					this.localOrderSet.OrderMap.FindByLocalId(orderRow.OrderId);
				if (orderMapRow == null)
					throw new Exception(String.Format("Mapping for local order id {0} doesn't exist",
						orderRow.OrderId));

				// Find the order record in the global data tables using the translated order identifier.
				globalOrder = FindGlobalOrder(orderMapRow.GlobalId);

				// These parameters are returned from the call to update the record and are used to track the order
				// until we get a refresh from the server.
				int modifiedLoginId = 0;
				long rowVersion = orderRow.RowVersion;
				DateTime modifiedTime = DateTime.MinValue;

				// Call the web service to update the order record.  If successful, it will return the new rowVersion
				// and the modified time and user of record.
//				WebClient.UpdateOrder(ref rowVersion, orderRow.OrderId, orderRow.BlockOrderId,
//					orderRow.BrokerId, orderRow.TimeInForceCode, orderRow.OrderTypeCode, 
//					orderRow.Quantity, orderRow.IsPrice1Null() ? (object)DBNull.Value : orderRow.Price1,
//					orderRow.IsPrice2Null() ? (object)DBNull.Value : orderRow.Price2, ref modifiedTime, ref modifiedLoginId);

				// Update the internal data model with the new data.  Note that we update the rowVersion, modified time
				// and login id from the data returned from the server. If the server finished the update operation
				// without an error, we can assume that the members of the 'order' record have been accepted.  We will
				// update the data model from that record to give an accurate state until the next refresh.
				globalOrder.RowVersion = rowVersion;
				globalOrder.BlockOrderId = orderRow.BlockOrderId;
				globalOrder.BrokerId = orderRow.BrokerId;
				globalOrder.OrderId = orderRow.OrderId;
				globalOrder.TimeInForceCode = orderRow.TimeInForceCode;
				globalOrder.OrderTypeCode = orderRow.OrderTypeCode;
				globalOrder.Quantity = orderRow.Quantity;
				if (orderRow.IsPrice1Null()) globalOrder.SetPrice1Null(); else globalOrder.Price1 = orderRow.Price1;
				if (orderRow.IsPrice2Null()) globalOrder.SetPrice2Null(); else globalOrder.Price2 = orderRow.Price2;
				globalOrder.ModifiedTime = modifiedTime;
				globalOrder.ModifiedLoginId = modifiedLoginId;

				// At this point, the changes can be committed to the local data model.
				globalOrder.AcceptChanges();

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
				
				// Rollback any modifications to the global record.
				if (globalOrder != null) globalOrder.RejectChanges();

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the locks
				ReleaseLocalOrders();
				ReleaseGlobalOrders();

			}

		}

		/// <summary>
		/// Updates the order record on the server.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void UpdateGlobalOrderThread(params object[] argument)
		{

			// IMPORTANT CONCEPT: For performance, all changes to the server made by the user are assumed to be successful
			// and udpated immediately on the client side.  If there is an error with the server, the data model is
			// restored from the server.  This flag will be used as a signal at the end of this operation that we need to
			// revert back to the server's data.
			bool isSuccessful = true;
			
			// Extract the initialization parameters.
			OrderSet.OrderRow orderRow = (OrderSet.OrderRow)argument[0];

			// This record will be rolled back if the update isn't successful.
			ClientMarketData.OrderRow globalOrder = null;

			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the tables.
				LockGlobalOrders();

				// Find the order record in the global data tables.
				globalOrder = FindGlobalOrder(orderRow.OrderId);

				// These parameters are returned from the call to update the record and are used to track the order
				// until we get a refresh from the server.
				int modifiedLoginId = 0;
				long rowVersion = orderRow.RowVersion;
				DateTime modifiedTime = DateTime.MinValue;

				// Call the web service to update the order record.  If successful, it will return the new rowVersion
				// and the modified time and user of record.
//				WebClient.UpdateOrder(ref rowVersion, orderRow.OrderId, orderRow.BlockOrderId,
//					orderRow.BrokerId, orderRow.TimeInForceCode, orderRow.OrderTypeCode, orderRow.Quantity,
//					orderRow.IsPrice1Null() ? (object)DBNull.Value : orderRow.Price1,
//					orderRow.IsPrice2Null() ? (object)DBNull.Value : orderRow.Price2, ref modifiedTime, ref modifiedLoginId);

				// Update the internal data model with the new data.  Note that we update the rowVersion, modified time
				// and login id from the data returned from the server. If the server finished the update operation
				// without an error, we can assume that the members of the 'order' record have been accepted.  We will
				// update the data model from that record to give an accurate state until the next refresh.
				globalOrder.RowVersion = rowVersion;
				globalOrder.BlockOrderId = orderRow.BlockOrderId;
				globalOrder.BrokerId = orderRow.BrokerId;
				globalOrder.TimeInForceCode = orderRow.TimeInForceCode;
				globalOrder.OrderTypeCode = orderRow.OrderTypeCode;
				globalOrder.Quantity = orderRow.Quantity;
				if (orderRow.IsPrice1Null()) globalOrder.SetPrice1Null(); else globalOrder.Price1 = orderRow.Price1;
				if (orderRow.IsPrice2Null()) globalOrder.SetPrice2Null(); else globalOrder.Price2 = orderRow.Price2;
				globalOrder.ModifiedTime = modifiedTime;
				globalOrder.ModifiedLoginId = modifiedLoginId;

				// At this point, the changes can be committed to the local data model.				
				globalOrder.AcceptChanges();

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
				if (globalOrder != null) globalOrder.RejectChanges();

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// Indicate that a data model refresh is needed when we're done with the locks.
				isSuccessful = false;

			}
			finally
			{

				// Release the tables.
				ReleaseGlobalOrders();

			}

			// Force the document to redraw the original data.  The call to 'RefreshClientMarketData' must be made outside of any
			// table locking to prevent deadlocks.  The flag 'isSuccessful' is used to signal whether we need to revert 
			// back to the server data model after the operation above.
			if (!isSuccessful)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void DeleteGlobalOrderThread(params object[] argument)
		{

			// IMPORTANT CONCEPT: For performance, all changes to the server made by the user are assumed to be successful
			// and udpated immediately on the client side.  If there is an error with the server, the data model is
			// restored from the server.  This flag will be used as a signal at the end of this operation that we need to
			// revert back to the server's data.
			bool isSuccessful = true;
			
			// Extract the parameters used by this thread.
			DeleteArgs deleteOrderArgs = (DeleteArgs)argument[0];

			RemoteBatch remoteBatch = null;
			
			// Make sure that the table locks are released.		
			try
			{

				// Create a web services connection for this command.
				WebClient webClient = new WebClient();

				// Lock the tables.
				LockGlobalOrders();

				// This is the command batch that will be sent to the server when we have created all the orders.
				remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType orderType = remoteAssembly.Types.Add("Shadows.WebService.Core.Order");

				// Add the order.
				RemoteMethod remoteMethod = orderType.Methods.Add("Delete");
				remoteMethod.Parameters.Add("OrderId", deleteOrderArgs.OrderId);
				remoteMethod.Parameters.Add("rowVersion", deleteOrderArgs.RowVersion);

				// The order record was deleted back in the command thread so that the document could be redrawn
				// without the deleted execution.  However, the record isn't permanently deleted from the client-side data
				// model until the server confirms the operation.  This is where the order is deleted forever from the
				// client side.
				deleteOrderArgs.OrderRow.AcceptChanges();

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
				deleteOrderArgs.OrderRow.RejectChanges();
				
				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// Indicate that a data model refresh is needed when we're done with the locks.
				isSuccessful = false;

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				ReleaseGlobalOrders();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Force the document to redraw the original data.  The call to 'RefreshClientMarketData' must be made outside of any
			// table locking to prevent deadlocks.  The flag 'isSuccessful' is used to signal whether we need to revert 
			// back to the server data model after the operation above.
			if (!isSuccessful)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

		}

		/// <summary>
		/// Opens the Order Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Order Viewer in the background.
			CommandQueue.Execute(new ThreadHandler(OpenViewerCommand));

		}

		/// <summary>
		/// Closes the Order Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Order Viewer in the background.
			CommandQueue.Execute(new ThreadHandler(CloseViewerCommand));

		}

		/// <summary>
		/// Opens the Order Document.
		/// </summary>
		/// <param name="blotterId">The primary identifier of the object to open.</param>
		public override void OpenDocument(int objectId)
		{

			// Open the Order Document in the background.
			CommandQueue.Execute(new ThreadHandler(OpenDocumentCommand), objectId);

		}

		/// <summary>
		/// Closes the Order Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Close the Order Document in the background.
			CommandQueue.Execute(new ThreadHandler(CloseDocumentCommand));

		}

		/// <summary>
		/// Handles an escape command.
		/// </summary>
		public override void CommandEscape()
		{

			// This will prevent validation on any records we might be working on.
			this.isOrderSelected = false;

			// Move the selection home position.
			CommandHome();

			// Give the focus back to whoever wants it.  This will cause 'OnLeave' to be called which will clear out the
			// empty records and attempt to validate any current work.  But since the selection flag has been turned off,
			// the validation will be skipped on the current record and it will disappear.
			OnReleaseFocus();

		}

		/// <summary>
		/// Called when the focus enters the window
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnEnter(EventArgs e)
		{

			// If the document is open, then interpret the focus coming into the viewer as if a new cell were selected.
			// This is done to force the 'Prompting' action of new orders and a re-selection of existing orders.
			// The document will be entered one or more times during the initialization, so these events are filtered out
			// because all the document metrics haven't been calculated yet.
			if (this.IsDocumentOpen)
				OnSelectionChange(new SelectionChangeArgs(GetActiveCellAddress()));

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnEnter(e);

		}

		/// <summary>
		/// Called when the focus leaves this viewer.
		/// </summary>
		/// <param name="e">Event parameters.</param>
		protected override void OnLeave(EventArgs e)
		{

			// Make sure that all changes are committed when the focus leaves this viewer.
			CommitChanges();

			// Clear the local table of any unused records.  This must be done in the background to avoid deadlocks.  This
			// command will also redraw the document if anything has change that would impact the viewer.
			CommandQueue.Execute(new ThreadHandler(ClearLocalOrdersCommand));

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnLeave(e);

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
				ThreadHandler commandHandler = null;
				object key = null;

				// Extract the current row and column from the event argument.  This is the address of the cell where 
				// the editing took place. Note that, because the user can move the focus at any time to another cell,
				// this is not necessarily the address of the currently active cell.
				int rowIndex = e.CellAddress.RowIndex;
				int ColumnIndex = e.CellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a local record.
				if (rowType == RowType.LocalOrder)
				{

					// Get the order id from the selected row.
					int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
					key = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						OrderIdColumnIndex)));

					// The column type of the cell tells us which command to execute in the background for updating the
					// cell.  Note also that every time a field is updated, the record is invalidated.  This will force a
					// record validation command to be executed when the focus is moved from the current record.
					switch (this.addressMap.GetColumnType(ColumnIndex))
					{

						case ColumnType.AccountName:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalAccountIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.AccountMnemonic:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalAccountIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.SecurityName:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalSecurityIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.SecuritySymbol:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalSecurityIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.TransactionTypeMnemonic:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalTransactionTypeCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.TimeInForceMnemonic:

							// Update Local TimeInForceCode field
							commandHandler = new ThreadHandler(SetLocalTimeInForceCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.OrderTypeMnemonic:
							
							// Update Local OrderTypeCode field
							commandHandler = new ThreadHandler(SetLocalOrderTypeCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Quantity:
							
							// Update Local Quantity field
							commandHandler = new ThreadHandler(SetLocalQuantityCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price1:
							
							// Update Local Price1 field
							commandHandler = new ThreadHandler(SetLocalPrice1Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price2:
							
							// Update Local Price2 field
							commandHandler = new ThreadHandler(SetLocalPrice2Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

					}

				}

				// This will create a command to update a global record.
				if (rowType == RowType.GlobalOrder)
				{

					// Get the order id from the selected row.
					int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
					key = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						OrderIdColumnIndex)));

					// The column type of the cell tells us which command to execute in the background for updating the
					// cell.  Note also that every time a field is updated, the record is invalidated.  This will force a
					// record validation command to be executed when the focus is moved from the current record.
					switch (this.addressMap.GetColumnType(ColumnIndex))
					{

						case ColumnType.AccountName:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalAccountIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.AccountMnemonic:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalAccountIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.SecurityName:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalSecurityIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.SecuritySymbol:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalSecurityIdCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;
						
						case ColumnType.TransactionTypeMnemonic:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalTransactionTypeCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.TimeInForceMnemonic:

							// Update Global TimeInForceCode field
							commandHandler = new ThreadHandler(SetGlobalTimeInForceCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.OrderTypeMnemonic:
							
							// Update Global OrderTypeCode field
							commandHandler = new ThreadHandler(SetGlobalOrderTypeCodeCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Quantity:
							
							// Update Global Quantity field
							commandHandler = new ThreadHandler(SetGlobalQuantityCommand);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price1:
							
							// Update Global Price1 field
							commandHandler = new ThreadHandler(SetGlobalPrice1Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price2:
							
							// Update Global Price2 field
							commandHandler = new ThreadHandler(SetGlobalPrice2Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

					}

				}

				// If a command was decoded from the column information above, then execute the command in the background
				// giving the primary key and the data to a thread that will execute outside the message loop thread.
				if (commandHandler != null)
					CommandQueue.Execute(commandHandler, key, e.Result);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnEndEdit(e);

		}

		/// <summary>
		/// Clears the values from a field in the viewer.
		/// </summary>
		/// <param name="e">The event argument.</param>
		protected override void OnCommandClear(System.EventArgs e)
		{
		
			try
			{

				// The main idea of this method is to take the results of the editing and spawn a thread that will handle
				// the new data in the background.  There are some tasks that have to be done in the foreground (Message 
				// Loop Thread), such as operating on the spreadsheet control, and there are other tasks that have to be
				// done in the background (Locking tables and examining the data model).  Here, we do what can be done in
				// the foreground, and construct a ThreadHandler for the background.
				ThreadHandler commandHandler = null;
				object key = null;

				// Extract the current row and column.  The task will be handled in the background thread since a change
				// in the selection can impact the data model.  So we collect the foreground information here and spawn a
				// thread to handle the action.
				CellAddress activeCellAddress = GetActiveCellAddress();
				int rowIndex = activeCellAddress.RowIndex;
				int ColumnIndex = activeCellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a ThreadHandler to update a local record.
				if (rowType == RowType.LocalOrder)
				{

					// Get the order id from the selected row.
					int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
					key = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						OrderIdColumnIndex)));

					// The column type of the cell tells us which ThreadHandler to execute in the background for updating the
					// cell.  This is only marginally better to look at than some lookup table.
					switch (this.addressMap.GetColumnType(ColumnIndex))
					{

						case ColumnType.BrokerSymbol:
						case ColumnType.TimeInForceMnemonic:
						case ColumnType.OrderTypeMnemonic:
						case ColumnType.Quantity:

							// Give the user feedback that these values can't be modified.
							Sounds.PlaySound(Sounds.MB_ICONASTERISK);
							break;

						case ColumnType.Price1:
							
							commandHandler = new ThreadHandler(SetLocalPrice1Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price2:
							
							commandHandler = new ThreadHandler(SetLocalPrice2Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

					}

				}

				// This will create a ThreadHandler to update a global record.
				if (rowType == RowType.GlobalOrder)
				{

					// Get the order id from the selected row.
					int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
					key = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						OrderIdColumnIndex)));

					// The column type of the cell tells us which ThreadHandler to execute in the background for updating the
					// cell.  This is only marginally better to look at than some lookup table.
					switch (this.addressMap.GetColumnType(ColumnIndex))
					{
						case ColumnType.BrokerSymbol:
						case ColumnType.TimeInForceMnemonic:
						case ColumnType.OrderTypeMnemonic:
						case ColumnType.Quantity:

							// Give the user feedback that these values can't be modified.
							Sounds.PlaySound(Sounds.MB_ICONASTERISK);
							break;
					
						case ColumnType.Price1:
							
							commandHandler = new ThreadHandler(SetGlobalPrice1Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

						case ColumnType.Price2:
							
							commandHandler = new ThreadHandler(SetGlobalPrice2Command);
							this.isOrderChanged = true;
							this.isOrderValid = false;
							break;

					}

				}

				// If a ThreadHandler was decoded from the column information above, then execute the ThreadHandler in the background
				// giving the primary key and the data to a thread that will execute outside the message loop thread.
				if (commandHandler != null)
					CommandQueue.Execute(new ThreadHandler(commandHandler), key, null);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

			// Call the base class to insure that other registered delegates receive the event. 
			base.OnCommandClear(e);

		}

		/// <summary>
		/// Handles the selection changing from one cell to another.
		/// </summary>
		/// <param name="e">Event argument.</param>
		protected override void OnSelectionChange(Shadows.Quasar.Viewers.SelectionChangeArgs e)
		{

			// Extract the current row and column from the event argument.
			int rowIndex = e.CellAddress.RowIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
			int rowType = (rowTypeColumnIndex == SpreadsheetColumn.DoesNotExist) ?
				RowType.Unused :
				Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			// The row type defines what action to take based on the selection changing.
			switch (rowType)
			{

				case RowType.Unused:

					// If we had previously selected an order record, see if it needs to be inserted or updated 
					// before we move on to the next record.
					CommitChanges();

					// Indicate that an order is not selected.
					this.isOrderSelected = false;

					break;

				case RowType.Placeholder:

					// If we had previously selected an order record, see if it needs to be inserted or updated before
					// we move on to the next record.
					CommitChanges();

					// It's possible for the placeholder to be selected when an empty order is drawn, since the
					// placeholder ends up in the first row.  However, the line shouldn't be initialized until the window
					// gains the focus.  Also, there are times when selecting the placeholder should be ignored, such as
					// when the program is taking care of initializing a row.  In all other cases, when the user clicks on
					// a placeholder row, a line is initialized for them.
					if (this.ContainsFocus && this.isManualSelection)
						CommandQueue.Execute(new ThreadHandler(InitializeOrderCommand));

					// Indicate that an order is not selected.
					this.isOrderSelected = false;

					break;

				case RowType.LocalOrder:
				case RowType.GlobalOrder:

					// Get the order identifier from the spreadsheet row.
					int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
					int OrderId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, OrderIdColumnIndex)));

					// If the currently selected order not the same as the previously selected order, then commit
					// the changes.
					if (this.selectedRowType != rowType || this.selectedOrderId != OrderId)
						CommitChanges();

					// This indicates that an order is selected and saves state information about that order.  
					// These state variables are needed by the ledger style input to determine when to commit the changes
					// to the server.  The row type tells the 'commit' logic whether a local or global record is
					// identified in the 'selectedOrderId' member.
					this.isOrderSelected = true;
					this.selectedRowType = rowType;
					this.selectedOrderId = OrderId;
						 
					break;

			}
	
			// Call the base class to insure that other registered delegates receive the event. 
			base.OnSelectionChange(e);

		}

		/// <summary>
		/// Draws the document in the background.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// If new data or a new structure is available after the data model has changed, then will initiate a refresh
			// of the viewer contents.
			if (this.IsDocumentOpen && this.IsRefreshNeeded)
			{

				// Use the command thread to redraw this document.
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlockOrderId, this.localOrderSet);

				// We assume that no refresh is needed again until an event request one.
				this.IsRefreshNeeded = false;

			}

		}

		/// <summary>
		/// Opens the a block order in the execution viewer.
		/// </summary>
		/// <param name="BlockOrderId">The primary identifier of the object to open.</param>
		public void OpenBlockOrder(int BlockOrderId)
		{

			// Execute a command in the background to open up the document.  Constructing an appraisal will require access
			// to the data model to build a model and select a stylesheet.
			CommandQueue.Execute(new ThreadHandler(OpenBlockOrderCommand), BlockOrderId);

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
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
			int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			// Delete a local order
			if (rowType == RowType.LocalOrder)
			{

				// Get the order id from the selected row.
				int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
				int OrderId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					OrderIdColumnIndex)));

				// Execute a command on the worker thread to delete the local record.
				CommandQueue.Execute(new ThreadHandler(DeleteLocalOrderCommand), OrderId);

				// This will prevent any attempt to update the deleted record when the selection changes.
				this.isOrderSelected = false;

				// Move the selection to the previous order.
				CommandMoveUp();

			}

			// Delete a global order.
			if (rowType == RowType.GlobalOrder)
			{

				// Get the order id from the selected row.
				int OrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.OrderId);
				int OrderId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					OrderIdColumnIndex)));

				// Execute a command on the worker thread to delete the global record.
				CommandQueue.Execute(new ThreadHandler(DeleteGlobalOrderCommand), OrderId);

				// This will prevent any attempt to update the deleted record when the selection changes.
				this.isOrderSelected = false;

				// Move the selection to the previous order.
				CommandMoveUp();

			}
	
		}

		/// <summary>
		/// Selects the starting cell for a new orderRow line.
		/// </summary>
		/// <param name="orderRow">The data in the new line.</param>
		private void InitializeRow(OrderSet.OrderRow orderRow)
		{

			// After the row had been initialized, we want to pick the best column to begin getting data.  The address map
			// was reconstructed in the 'Draw' method above, so we can us the map to find the record we just entered.
			AddressMap.LocalMapRow localMapRow = this.addressMap.LocalMap.FindByOrderId(orderRow.OrderId);
			if (localMapRow == null)
				return;

			// Find the columns used for the initial selection based on the column type.
			int ColumnIndex = this.addressMap.GetColumnIndex(ColumnType.TransactionTypeMnemonic);

			// This will leave the selection in the the most natural cell for the user based on what fields have already
			// been filled in by defaults.
			Select(new CellAddress(this.DocumentVersion, localMapRow.RowIndex, ColumnIndex));

			// All new rows have to be validated.
			this.isOrderChanged = false;
			this.isOrderValid = false;

		}

		/// <summary>
		/// Commits any temporary changes to the selected order.
		/// </summary>
		private void CommitChanges()
		{

			// Only update the record if it was previously selected.
			if (this.isOrderSelected && !this.isOrderValid)
			{

				// Local records are handled differenly than global records.  The local records are created and managed 
				// locally until a connection can be established with the server.  Until a global identifier can be
				// associated with the record, it is managed in a different data model.  This will start the process of
				// storing the newly created record to the server.
				if (this.selectedRowType == RowType.LocalOrder && this.isOrderChanged)
					CommandQueue.Execute(new ThreadHandler(ValidateLocalOrderCommand), this.selectedOrderId);

				// Global records are modified locally until the entire row is ready to be accepted.  Then a single
				// transaction updates the record.  This starts the task of updating the server using the changes made to
				// the local data model.
				if (this.selectedRowType == RowType.GlobalOrder)
					CommandQueue.Execute(new ThreadHandler(ValidateGlobalOrderCommand), this.selectedOrderId);

				// After the changes are committed, an assumption is made that the record is valid.  If the command to
				// validate the record turns up an error, the record will be selected again and invalidated.  For the 
				// moment, however, the record is considered valid.
				this.isOrderChanged = false;
				this.isOrderValid = true;

			}

		}
		
		/// <summary>
		/// Event driver for the Blotter table.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="blotterRowChangeEvent">The event arguments.</param>
		public void BlotterRowChangeEvent(object sender, ClientMarketData.BlotterRowChangeEvent blotterRowChangeEvent)
		{

			// This will force a refresh of the entire document.
			this.IsRefreshNeeded = true;

		}

		/// <summary>
		/// Event driver for the Broker table.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="brokerRowChangeEvent">The event arguments.</param>
		public void BrokerRowChangeEvent(object sender, ClientMarketData.BrokerRowChangeEvent brokerRowChangeEvent)
		{

			// This will force a refresh of the entire document.
			this.IsRefreshNeeded = true;

		}

		/// <summary>
		/// Handles the changing of an order record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event argument.</param>
		public void OrderChangedEvent(object sender, ClientMarketData.OrderRowChangeEvent orderRowChangeEvent)
		{

			// Don't attempt to modify an individual row if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the fields.
				ClientMarketData.OrderRow orderRow = orderRowChangeEvent.Row;

				// If the order doesn't belong to the block in this document, then there's nothing to update.
				if (this.BlockOrderId != orderRow.BlockOrderId)
					return;
	
				try
				{
				
					// Update the order on the spreadsheet.
					Invoke(this.setOrderDelegate, new object[] {Order.Create(orderRow)});

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}

			}

		}
		
		/// <summary>
		/// Handles a changed stylesheet in the data model.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="stylesheetRowChangeEvent">The event argument.</param>
		private void StylesheetChangedEvent(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// Reopen the document if the style sheet has changed.  Note that the stylesheet is associated with the 
			// blotter, so the document has to be opened first, to re-install the stylesheet, then the block order can be
			// displayed with the new template.
			if (this.IsDocumentOpen && stylesheetRowChangeEvent.Row.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.BlotterId);
				OpenBlockOrder(this.BlockOrderId);
			}

		}

	}

}
