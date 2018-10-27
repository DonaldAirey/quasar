/*************************************************************************************************************************
*
*	File:			BlockOrderViewer.cs
*	Description:	This control is used to display and manage a blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.BlockOrder
{

	using AxMicrosoft.Office.Interop.Owc11;
	using Microsoft.Office.Interop.Owc11;
	using Shadows.Quasar.Library.Debt;
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

	/// <summary>
	/// Summary description for BlockOrderViewer.
	/// </summary>
	public class BlockOrderViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private bool isBlockOrderChanged;
		private bool isBlockOrderSelected;
		private int blotterId;
		private int sortMethod;
		private int selectedRowType;
		private int selectedBlockOrderId;
		private Shadows.Quasar.Viewers.Ticker ticker;
		private System.Xml.Xsl.XslTransform xslTransform;
		private System.Threading.Thread threadPricer;
		private System.Threading.Thread threadPriceAger;
		private System.Threading.ReaderWriterLock tickTableLock;
		private System.Threading.ReaderWriterLock addressMapLock;
		private AddressMap addressMap;
		private TickTable tickTable;
		private TickTable animatedTickTable;
		public System.Windows.Forms.ContextMenu contextMenuBlockOrder;
		private System.Windows.Forms.MenuItem menuItemElectronicExecution;
		private System.Windows.Forms.MenuItem menuItemAllocate;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemPlacement;
		private System.Windows.Forms.MenuItem menuItemManualExecution;
		private System.Windows.Forms.MenuItem menuItemClose;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemTechnicalAnalysis;
		private System.Windows.Forms.MenuItem menuItem2;
		private Shadows.Quasar.Client.ClientMarketData clientMarketData;
		private System.ComponentModel.IContainer components = null;

		// Public Events
		public event BlockOrderEventHandler OpenBlockOrder;
		public event BlockOrderEventHandler CloseBlockOrder;
		public event System.EventHandler Execution;
		public event System.EventHandler Placement;

		/// <summary>
		/// Constructor for the BlockOrderViewer
		/// </summary>
		public BlockOrderViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// Add the debt calculation library to the functions recognized by the spreadsheet control.
			this.ExternalLibraries.Add(new Viewers.DebtLibrary.Debt());

			// These variables help control the state of the viewer.  Since we're using the spreadsheet control like a
			// ledger, leaving a line after it's been modified creates a command to update the database.  Since the user
			// is free to click most anywhere on the input screen, we have no way of knowing what the last record was
			// unless we remember it using these members.
			this.isBlockOrderChanged = false;
			IsBlockOrderSelected = false;

			// This allows the base classes to access the context menu created for the block order viewer.  There is
			// likely a better way to use the 'ContextMenu'.
			this.ContextMenu = this.contextMenuBlockOrder;

			// HACK - When you get some time, find out why this needs to be initialized here rather than in the 'BeginMerge' 
			// method.
			this.IsRefreshNeeded = true;
			
			// Initialize the document with a blank blotter.  The Database uses identity columns for Ids, which guarantees
			// that there will never be a 'blotterId' of zero.  This action prevents a 'blank' spreadsheet from appearing 
			// briefly the first time this document is made visible.
			this.blotterId = 0;
			this.sortMethod = 0;

			// Initialize the price queue.  This is the pipeline for new ticks.  The 'TickTable' is used for coloring 
			// the new prices, making them fade as time goes by until they appear to be unchanged.
			this.ticker = new Ticker();
			this.tickTable = new TickTable();
			this.animatedTickTable = new TickTable();

			// These locks are used to coordinate the activities between threads when using common data structures.
			this.tickTableLock = new ReaderWriterLock();
			this.addressMapLock = new ReaderWriterLock();

			// This thread will refresh the document from the background.  That is, all the hard work is done in
			// the background.  Delegates will be called when the data is actually ready to be put in the control.
			this.threadPricer = new Thread(new ThreadStart(PricerThread));
			this.threadPricer.IsBackground = true;
			this.threadPricer.Name = "PricerThread";
			this.threadPricer.Start();

			// This thread will refresh the document from the background.  That is, all the hard work is done in
			// the background.  Delegates will be called when the data is actually ready to be put in the control.
			this.threadPriceAger = new Thread(new ThreadStart(PriceAgerThread));
			this.threadPriceAger.IsBackground = true;
			this.threadPriceAger.Name = "PriceAgerThread";
			this.threadPriceAger.Start();

		}

		#region Dispose Method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Terminate the pricer thread when we're finished.
			if (this.threadPricer != null)
				this.threadPricer.Abort();

			// Terminate the thread that animages the upticks and downticks.
			if (this.threadPriceAger != null)
				this.threadPriceAger.Abort();

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BlockOrderViewer));
			this.contextMenuBlockOrder = new System.Windows.Forms.ContextMenu();
			this.menuItemPlacement = new System.Windows.Forms.MenuItem();
			this.menuItemManualExecution = new System.Windows.Forms.MenuItem();
			this.menuItemElectronicExecution = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemClose = new System.Windows.Forms.MenuItem();
			this.menuItemAllocate = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemTechnicalAnalysis = new System.Windows.Forms.MenuItem();
			this.clientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			this.SuspendLayout();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			this.axSpreadsheet.BeforeContextMenu += new AxMicrosoft.Office.Interop.Owc11.ISpreadsheetEventSink_BeforeContextMenuEventHandler(this.axSpreadsheet_BeforeContextMenu);
			// 
			// contextMenuBlockOrder
			// 
			this.contextMenuBlockOrder.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.menuItemPlacement,
																								  this.menuItemManualExecution,
																								  this.menuItemElectronicExecution,
																								  this.menuItem3,
																								  this.menuItemOpen,
																								  this.menuItemClose,
																								  this.menuItemAllocate,
																								  this.menuItem2,
																								  this.menuItemTechnicalAnalysis});
			// 
			// menuItemPlacement
			// 
			this.menuItemPlacement.Index = 0;
			this.menuItemPlacement.Text = "&Placement";
			this.menuItemPlacement.Click += new System.EventHandler(this.menuItemPlacement_Click);
			// 
			// menuItemManualExecution
			// 
			this.menuItemManualExecution.Index = 1;
			this.menuItemManualExecution.Text = "&Manual Execution";
			this.menuItemManualExecution.Click += new System.EventHandler(this.menuItemManualExecution_Click);
			// 
			// menuItemElectronicExecution
			// 
			this.menuItemElectronicExecution.Index = 2;
			this.menuItemElectronicExecution.Text = "&Electronic Execution";
			this.menuItemElectronicExecution.Click += new System.EventHandler(this.menuItemElectronicExecution_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 3;
			this.menuItem3.Text = "-";
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 4;
			this.menuItemOpen.Text = "&Open";
			// 
			// menuItemClose
			// 
			this.menuItemClose.Index = 5;
			this.menuItemClose.Text = "&Close";
			// 
			// menuItemAllocate
			// 
			this.menuItemAllocate.Index = 6;
			this.menuItemAllocate.Text = "&Allocate";
			this.menuItemAllocate.Click += new System.EventHandler(this.menuItemAllocate_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 7;
			this.menuItem2.Text = "-";
			// 
			// menuItemTechnicalAnalysis
			// 
			this.menuItemTechnicalAnalysis.Index = 8;
			this.menuItemTechnicalAnalysis.Text = "&Technical Chart";
			this.menuItemTechnicalAnalysis.Click += new System.EventHandler(this.menuItemTechnicalAnalysis_Click);
			// 
			// BlockOrderViewer
			// 
			this.MoveAfterReturn = true;
			this.Name = "BlockOrderViewer";
			this.SelectionChange += new Shadows.Quasar.Viewers.SelectionChangeHandler(this.BlockOrderViewer_SelectionChange);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Thread safe indication of a changed execution record.
		/// </summary>
		[Browsable(false)]
		public bool IsBlockOrderSelected
		{
			get {lock (this) return this.isBlockOrderSelected;}
			set {lock (this) this.isBlockOrderSelected = value;}
		}

		/// <summary>
		/// Thread safe indication of a changed execution record.
		/// </summary>
		[Browsable(false)]
		public bool IsBlockOrderChanged
		{
			get {lock (this) return this.isBlockOrderChanged;}
			set {lock (this) this.isBlockOrderChanged = value;}
		}

		// Thread safe access to the blotter id.
		[Browsable(false)]
		public int BlotterId
		{
			get {lock (this) return this.blotterId;}
			set {lock (this) this.blotterId = value;}
		}

		// Thread safe access to the sort method.
		[Browsable(false)]
		public int SortMethod
		{
			get {lock (this) return this.sortMethod;}
			set {lock (this) this.sortMethod = value;}
		}

		/// <summary>
		/// Thread safe acces to the selected block order id.
		/// </summary>
		[Browsable(false)]
		public int SelectedBlockOrderId
		{
			get {lock (this) return this.selectedBlockOrderId;}
			set {lock (this) this.selectedBlockOrderId = value;}
		}

		/// <summary>
		/// Thread safe acces to the selected block order id.
		/// </summary>
		[Browsable(false)]
		public int SelectedRowType
		{
			get {lock (this) return this.selectedRowType;}
			set {lock (this) this.selectedRowType = value;}
		}

		/// <summary>
		/// Gets a list of the selected block orders.
		/// </summary>
		/// <returns>A list of the currently selected block orders.</returns>
		private ArrayList GetSelectedBlockOrder()
		{

			// Create a list to hold the block orders selected.
			ArrayList blockOrderArray = new ArrayList();

			// Cycle through the current selection extracting the block order ids.
			foreach (Range lineItem in this.axSpreadsheet.Selection)
			{

				// The row index can be found from the current item in the selection.
				int rowIndex = lineItem.Row;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex,
					rowTypeColumnIndex)));

				// If the current line is a block order, the block id will be extracted and added to the list.
				if (rowType == RowType.BlockOrder)
				{

					// Get the block order id from the spreadsheet row.
					int blockOrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BlockOrderId);
					int blockOrderId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, blockOrderIdColumnIndex)));

					// Place the block id in a list that will be passed to the background worker thread.
					blockOrderArray.Add(blockOrderId);

				}

			}

			// Return a list of the selected blocks.
			return blockOrderArray;

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
		/// Moves to the next cell in the spreadsheet.
		/// </summary>
		public override void CommandMoveNext()
		{

			// Watch out for parsing errors and bad links to the currency row.  All equities, derivatives and most debts
			// will have a 'currency id' that links it back to a row that can be used to credit or debit the account.  Bad
			// data may force that link to be broken.  It's an error that should be fixed by the operations people.
			try
			{

				// Extract the current row and column.  The task will be handled in the background thread since a change in
				// the selection can impact the data model.  So we collect the foreground information here and spawn a thread
				// to handle the action.
				CellAddress activeCellAddress = GetActiveCellAddress();
				int rowIndex = activeCellAddress.RowIndex;
				int columnIndex = activeCellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a local record.
				if (rowType == RowType.BlockOrder)
				{

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.QuantityPlaced:
							
							OnPlacement();
							break;

						case ColumnType.QuantityExecuted:

							OnExecution();
							break;

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			
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
				int columnIndex = e.CellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// This will create a command to update a local record.
				if (rowType == RowType.BlockOrder)
				{

					// Get the blockOrder id from the selected row.
					int blockOrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BlockOrderId);
					key = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						blockOrderIdColumnIndex)));

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.LastPrice:
							
							commandHandler = new ThreadHandler(SetLastPriceCommand);
							break;

						case ColumnType.Yield:
							
							commandHandler = new ThreadHandler(SetYieldCommand);
							break;

						case ColumnType.SettlementDate:
							
							commandHandler = new ThreadHandler(SetSettlementDateCommand);
							break;

						case ColumnType.QuantityPlaced:
							
							// The Placement command will only be invoked if the key used to complete the command was an
							// 'Enter' key.  The other navigation keys can confuse the interface by starting an execution,
							// then changing the selected block.
							e.Handled = true;
							break;

						case ColumnType.QuantityExecuted:
							
							// The Execution command will only be invoked if the key used to complete the command was an
							// 'Enter' key.  The other navigation keys can confuse the interface by starting an execution,
							// then changing the selected block.
							e.Handled = true;
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
		/// Opens the Block Order Viewer
		/// </summary>
		private void OpenViewerCommand(params object[] argument)
		{

			// This block will attach the appraisal viewer to the data model changes.
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.BeginMerge += new EventHandler(this.BeginMerge);
				ClientMarketData.BlockOrderTree.BlockOrderTreeRowChanged += new ClientMarketData.BlockOrderTreeRowChangeEventHandler(this.BlockOrderTreeRowChangeEvent);
				ClientMarketData.BlockOrderTree.BlockOrderTreeRowDeleted += new ClientMarketData.BlockOrderTreeRowChangeEventHandler(this.BlockOrderTreeRowChangeEvent);
				ClientMarketData.BlockOrder.BlockOrderRowChanged += new ClientMarketData.BlockOrderRowChangeEventHandler(this.BlockOrderRowChangeEvent);
				ClientMarketData.BlockOrder.BlockOrderRowDeleted += new ClientMarketData.BlockOrderRowChangeEventHandler(this.BlockOrderRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowChanged += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted += new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowChanged += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowDeleted += new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowDeleteEvent);
				ClientMarketData.Object.ObjectRowChanged += new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.Object.ObjectRowDeleted += new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowDeleted += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged += new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.Order.OrderRowDeleted += new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.OrderType.OrderTypeRowChanged += new ClientMarketData.OrderTypeRowChangeEventHandler(this.OrderTypeRowChangeEvent);
				ClientMarketData.OrderType.OrderTypeRowDeleted += new ClientMarketData.OrderTypeRowChangeEventHandler(this.OrderTypeRowChangeEvent);
				ClientMarketData.Placement.PlacementRowChanged += new ClientMarketData.PlacementRowChangeEventHandler(this.PlacementRowChangeEvent);
				ClientMarketData.Placement.PlacementRowDeleted += new ClientMarketData.PlacementRowChangeEventHandler(this.PlacementRowDeleteEvent);
				ClientMarketData.Price.PriceRowChanged += new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);
				ClientMarketData.Price.PriceRowDeleted += new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);
				ClientMarketData.Security.SecurityRowChanged += new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Security.SecurityRowDeleted += new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Status.StatusRowChanged += new ClientMarketData.StatusRowChangeEventHandler(this.StatusRowChangeEvent);
				ClientMarketData.Status.StatusRowDeleted += new ClientMarketData.StatusRowChangeEventHandler(this.StatusRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.TimeInForce.TimeInForceRowChanged += new ClientMarketData.TimeInForceRowChangeEventHandler(this.TimeInForceRowChangeEvent);
				ClientMarketData.TimeInForce.TimeInForceRowDeleted += new ClientMarketData.TimeInForceRowChangeEventHandler(this.TimeInForceRowChangeEvent);
				ClientMarketData.TransactionType.TransactionTypeRowChanged += new ClientMarketData.TransactionTypeRowChangeEventHandler(this.TransactionTypeRowChangeEvent);
				ClientMarketData.TransactionType.TransactionTypeRowDeleted += new ClientMarketData.TransactionTypeRowChangeEventHandler(this.TransactionTypeRowChangeEvent);
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
				if (ClientMarketData.BlockOrderTreeLock.IsReaderLockHeld) ClientMarketData.BlockOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Closes the Block Order Viewer.
		/// </summary>
		private void CloseViewerCommand(params object[] argument)
		{

			// Make sure any exceptions are localized to this method.
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.BeginMerge -= new EventHandler(this.BeginMerge);
				ClientMarketData.BlockOrderTree.BlockOrderTreeRowChanged -= new ClientMarketData.BlockOrderTreeRowChangeEventHandler(this.BlockOrderTreeRowChangeEvent);
				ClientMarketData.BlockOrderTree.BlockOrderTreeRowDeleted -= new ClientMarketData.BlockOrderTreeRowChangeEventHandler(this.BlockOrderTreeRowChangeEvent);
				ClientMarketData.BlockOrder.BlockOrderRowChanged -= new ClientMarketData.BlockOrderRowChangeEventHandler(this.BlockOrderRowChangeEvent);
				ClientMarketData.BlockOrder.BlockOrderRowDeleted -= new ClientMarketData.BlockOrderRowChangeEventHandler(this.BlockOrderRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowChanged -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Blotter.BlotterRowDeleted -= new ClientMarketData.BlotterRowChangeEventHandler(this.BlotterRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowChanged -= new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowChangeEvent);
				ClientMarketData.Execution.ExecutionRowDeleted -= new ClientMarketData.ExecutionRowChangeEventHandler(this.ExecutionRowDeleteEvent);
				ClientMarketData.Object.ObjectRowChanged -= new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.Object.ObjectRowDeleted -= new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowDeleted -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.Order.OrderRowDeleted -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.OrderType.OrderTypeRowChanged -= new ClientMarketData.OrderTypeRowChangeEventHandler(this.OrderTypeRowChangeEvent);
				ClientMarketData.OrderType.OrderTypeRowDeleted -= new ClientMarketData.OrderTypeRowChangeEventHandler(this.OrderTypeRowChangeEvent);
				ClientMarketData.Placement.PlacementRowChanged -= new ClientMarketData.PlacementRowChangeEventHandler(this.PlacementRowChangeEvent);
				ClientMarketData.Placement.PlacementRowDeleted -= new ClientMarketData.PlacementRowChangeEventHandler(this.PlacementRowDeleteEvent);
				ClientMarketData.Price.PriceRowChanged -= new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);
				ClientMarketData.Price.PriceRowDeleted -= new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);
				ClientMarketData.Security.SecurityRowChanged -= new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Security.SecurityRowDeleted -= new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Status.StatusRowChanged -= new ClientMarketData.StatusRowChangeEventHandler(this.StatusRowChangeEvent);
				ClientMarketData.Status.StatusRowDeleted -= new ClientMarketData.StatusRowChangeEventHandler(this.StatusRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.TimeInForce.TimeInForceRowChanged -= new ClientMarketData.TimeInForceRowChangeEventHandler(this.TimeInForceRowChangeEvent);
				ClientMarketData.TimeInForce.TimeInForceRowDeleted -= new ClientMarketData.TimeInForceRowChangeEventHandler(this.TimeInForceRowChangeEvent);
				ClientMarketData.TransactionType.TransactionTypeRowChanged -= new ClientMarketData.TransactionTypeRowChangeEventHandler(this.TransactionTypeRowChangeEvent);
				ClientMarketData.TransactionType.TransactionTypeRowDeleted -= new ClientMarketData.TransactionTypeRowChangeEventHandler(this.TransactionTypeRowChangeEvent);
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
				if (ClientMarketData.BlockOrderTreeLock.IsReaderLockHeld) ClientMarketData.BlockOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the viewer is now closed.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Opens the Block Order Document
		/// </summary>
		/// <param name="blotterId">The unique identifier for the group of trades on the blotter.</param>
		private void OpenDocumentCommand(params object[] argument)
		{

			// Extract the command argument.
			int blotterId = (int)argument[0];

			// This block will attach the appraisal viewer to the data model changes.
			try
			{

				// Store the id of the blotter.  Also, initialize the state of the selected element.  This will be set to the
				// proper state the first time a row in the document is selected.
				this.BlotterId = blotterId;
				IsBlockOrderSelected = false;

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, equity
				// traders Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null)
					throw new Exception(String.Format("Blotter {0} has been deleted", blotterId));

				// See if a stylesheet has been associated with the blotter.
				ClientMarketData.StylesheetRow stylesheetRow = blotterRow.IsBlockOrderStylesheetIdNull() ? null :
					ClientMarketData.Stylesheet.FindByStylesheetId(blotterRow.BlockOrderStylesheetId);
				if (stylesheetRow == null)
					throw new Exception(String.Format("Blotter {0} has no Block Order stylesheet", blotterId));

				// The stylesheets take a modest amount of time to parse from a string into an XSL structure.  As an
				// optimization, this action is skipped if the block order document is using the same stylesheet as the 
				// last document loaded.
				if (this.IsStylesheetChanged || this.StylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case the stylesheet is updated in the data model.  This value is
					// also used the next time the document is opened to check if we should load another XSL style 
					// sheet.  The override logic is reset after each sucessful load of a stylesheet and will remain that
					// way until an external event changes the stylesheet.
					this.StylesheetId = stylesheetRow.StylesheetId;
					this.IsStylesheetChanged = false;

					// The stylesheet is stored in the data model as a text string.  This will load that XSL formatted
					// string into an XSL structure.
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

			// This will reset the viewer to the home position when it opens.
			this.ResetViewer = true;
			
			// Draw the document.  Note that we are already in a worker thread, so there's no need to launch a new thread.
			DrawCommand(this.BlotterId, this.SortMethod);

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
		/// Draws the current document in the viewer.
		/// </summary>
		public override void DrawDocument()
		{

			// This will start a background thread that will construct the document then push it into the viewer.
			CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlotterId, this.SortMethod);

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
		public void DrawCommand(params object[] argument)
		{

			try
			{

				// Extract the command argument.
				int blotterId = (int)argument[0];
				int sortMethod = (int)argument[1];

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Lock the address map while it's constructed.
				this.addressMapLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Make sure the blotter still exists in the in-memory database.  We need it to rebalance the appraisal.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null && blotterId != 0)
					throw new Exception(String.Format("Blotter {0} has been deleted.", blotterId));

				// Create the blotter document.
				BlockOrderDocument blockOrderDocument = new BlockOrderDocument(blotterId);

#if DEBUG
				// Make sure that writing the debug file doesn't interrupt the session.
				try
				{

					// During debugging, it's very useful to have a copy of the DOM on the disk.
					blockOrderDocument.Save("blockOrderDOM.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// Sorting is done by the stylesheet but it needs to know which sorting method has been chosen by the user.
				XsltArgumentList xsltArgumentList = new XsltArgumentList();
				xsltArgumentList.AddParam("sortMethod", string.Empty, sortMethod);

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed 
				// into the spreadsheet control directly, when there is a major format change, or will be updated
				// incrementally when only the content has changed.
				XmlDocument spreadsheetDocument = new XmlDocument();
				spreadsheetDocument.Load(this.xslTransform.Transform(blockOrderDocument, xsltArgumentList, (XmlResolver)null));

				// Give each new document a new identifier.  The background threads can use this identifier to insure that
				// updated cells are addressed to the right document.  Since the document can change between the time the
				// background thread generates a change and the time the foreground processes it, the document id insures
				// the update is discarded if the target document has changed.
				this.IncrementDocumentVersion();

#if DEBUG
				// Make sure that writing the debug file doesn't interrupt the session.
				try
				{

					// Write out the spreadsheet document when debugging.
					spreadsheetDocument.Save("BlockOrderSpreadsheet.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				InvokeCreateAddressMap(spreadsheetDocument);

				// Move the formatted spreadsheet document into the control.
				SetDocument(spreadsheetDocument);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderTreeLock.IsReaderLockHeld) ClientMarketData.BlockOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the address map lock after its been created.
				if (this.addressMapLock.IsWriterLockHeld) this.addressMapLock.ReleaseWriterLock();

			}

		}

		/// <summary>
		/// Displays the context menu.
		/// </summary>
		/// <param name="argument">A set of generic command argument.</param>
		private void ShowContextMenuCommand(params object[] argument)
		{

			try
			{
			
				// Show the context menu at the location passed to this method from the foreground.
				ShowContextMenu((Point)argument[0]);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}
		
		/// <summary>
		/// Updates the price column when the user overrides the current price.
		/// </summary>
		/// <param name="price">The new value for the price column.</param>
		private void SetLastPriceCommand(params object[] argument)
		{

			// Make sure the table locks are released.
			try
			{

				// Extract the command argument.
				int blockOrderId = (int)argument[0];
				decimal lastPrice = Decimal.Parse((string)argument[1]);

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the local tables are locked.
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the block order belonging to this spreadsheet cell.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null)
					throw new ArgumentException("This block order has been deleted", blockOrderId.ToString());

				AddressMapSet.SecurityMapRow securityMapRow = this.addressMap.SecurityMap.FindBySecurityId(blockOrderRow.SecurityId);
				foreach (AddressMapSet.LastPriceAddressRow lastPriceAddressRow in securityMapRow.GetLastPriceAddressRows())
					SetCell(new CellAddress(lastPriceAddressRow.DocumentVersion, lastPriceAddressRow.RowIndex, lastPriceAddressRow.ColumnIndex), Convert.ToDecimal(lastPrice));

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the mapping tables (and the document)
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Updates the price and yield column when the yield is changed.
		/// </summary>
		/// <param name="yield">The new value for the yield column.</param>
		private void SetYieldCommand(params object[] argument)
		{

			// Make sure the tables used are released when we're finished.
			try
			{

				// Extract the command argument.
				int blockOrderId = (int)argument[0];
				decimal yield = Percent.Parse((string)argument[1]);
				
				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the local tables are locked.
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the block order belonging to this spreadsheet cell.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null)
					throw new ArgumentException("This block order has been deleted", blockOrderId.ToString());

				// Find the debt that belongs to this spreadsheet cell.
				ClientMarketData.DebtRow debtRow = ClientMarketData.Debt.FindByDebtId(blockOrderRow.SecurityId);
				if (debtRow == null)
					throw new ArgumentException("This block order isn't a debt", blockOrderRow.SecurityId.ToString());

				// HACK - The settlement date should be calculated from the calendar days.
				DateTime settlementDate = DateTime.Now;

				// Calculate the price of the issue using the yield.
				decimal price = Debt.YieldFromPrice(debtRow.Coupon, debtRow.MaturityDate, settlementDate, yield);

				AddressMapSet.SecurityMapRow securityMapRow = this.addressMap.SecurityMap.FindBySecurityId(blockOrderRow.SecurityId);
				foreach (AddressMapSet.LastPriceAddressRow lastPriceAddressRow in securityMapRow.GetLastPriceAddressRows())
					SetCell(new CellAddress(lastPriceAddressRow.DocumentVersion, lastPriceAddressRow.RowIndex, lastPriceAddressRow.ColumnIndex), Convert.ToDecimal(price));

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the mapping tables (and the document)
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Updates the Settlement Date on the spreadsheet.
		/// </summary>
		/// <param name="settlementDate">The new settlement date for the active cell.</param>
		private void SetSettlementDateCommand(params object[] argument)
		{

			try
			{

				// Extract the command argument.
				DateTime settlementDate = DateTime.Parse((string)argument[0]);

				// These ranges are used several times below to access the spreadsheet.
				Range activeCell = this.Spreadsheet.ActiveCell;
				Worksheet activeSheet = this.Spreadsheet.ActiveSheet;

				// The settlement date can be poked directly into the cell.
				activeCell.Value2 = settlementDate;

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Place the block orders with electronic brokers.
		/// </summary>
		/// <param name="argument">The command arguments.</param>
		private void ExecuteBlockOrderCommand(params object[] argument)
		{

			// Extract the command argument.
			int brokerId = (int)argument[0];
			ArrayList blockOrderList = (ArrayList)argument[1];

			// Create a batch of electronic placements from the list of block orders selected by the caller.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Placement");

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);

				foreach (int blockOrderId in blockOrderList)
				{

					ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
					if (blockOrderRow == null)
						continue;

					decimal quantityOrdered = 0.0M;
					foreach (ClientMarketData.OrderRow SetPrice in blockOrderRow.GetOrderRows())
						quantityOrdered += SetPrice.Quantity;

					decimal quantityPlaced = 0.0M;
					foreach (ClientMarketData.PlacementRow placementRow in blockOrderRow.GetPlacementRows())
						quantityPlaced += placementRow.Quantity;
					decimal quantity = quantityOrdered - quantityPlaced;

					if (quantity > 0.0M)
					{

						// Call the web service to add the new placement.
						RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
						remoteMethod.Parameters.Add("blockOrderId", blockOrderId);
						remoteMethod.Parameters.Add("brokerId", brokerId);
						remoteMethod.Parameters.Add("timeInForceCode", TimeInForce.DAY);
						remoteMethod.Parameters.Add("orderTypeCode", OrderType.Market);
						remoteMethod.Parameters.Add("isRouted", true);
						remoteMethod.Parameters.Add("quantity", quantity);

					}

				}

			}
			finally
			{

				// Release the locks obtained to produce the blotter report.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PlacementLock.IsReaderLockHeld) ClientMarketData.PlacementLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// At this point, the batc of block orders can be sent to the server for processing.
			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Allocates the executions back to the original accounts.
		/// </summary>
		/// <param name="blockOrderList">A list of block orders to allocate.</param>
		private void AllocateBlockOrderCommand(params object[] argument)
		{

			try
			{

				// Extrac the command argument.
				ArrayList blockOrderList = (ArrayList)argument[0];
			
				// Allocate each of the blocks selected.
				foreach (int blockOrderId in blockOrderList)
					Allocation.Distribute(blockOrderId);

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The arguments for the event handler.</param>
		private void BlockOrderTreeRowChangeEvent(object sender, ClientMarketData.BlockOrderTreeRowChangeEvent blockOrderTreeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the blotter table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="blotterRowChangeEvent">The arguments for the event handler.</param>
		private void BlockOrderRowChangeEvent(object sender, ClientMarketData.BlockOrderRowChangeEvent blockOrderRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

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
		/// Event driver for a change to the Object table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ObjectRowChangeEvent">The arguments for the event handler.</param>
		private void ObjectRowChangeEvent(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the ObjectTree table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="ObjectTreeRowChangeEvent">The arguments for the event handler.</param>
		private void ObjectTreeRowChangeEvent(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectTreeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the Order table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="OrderRowChangeEvent">The arguments for the event handler.</param>
		private void OrderRowChangeEvent(object sender, ClientMarketData.OrderRowChangeEvent orderRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the OrderType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="OrderTypeRowChangeEvent">The arguments for the event handler.</param>
		private void OrderTypeRowChangeEvent(object sender, ClientMarketData.OrderTypeRowChangeEvent orderTypeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the Security table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="SecurityRowChangeEvent">The arguments for the event handler.</param>
		private void SecurityRowChangeEvent(object sender, ClientMarketData.SecurityRowChangeEvent securityRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the status table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="statusRowChangeEvent">The arguments for the event handler.</param>
		private void StatusRowChangeEvent(object sender, ClientMarketData.StatusRowChangeEvent statusRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the timeInForce table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="timeInForceRowChangeEvent">The arguments for the event handler.</param>
		private void TimeInForceRowChangeEvent(object sender, ClientMarketData.TimeInForceRowChangeEvent timeInForceRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the transactionType table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="transactionTypeRowChangeEvent">The arguments for the event handler.</param>
		private void TransactionTypeRowChangeEvent(object sender, ClientMarketData.TransactionTypeRowChangeEvent transactionTypeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event handler for a change to the executions table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void ExecutionRowChangeEvent(object sender, ClientMarketData.ExecutionRowChangeEvent executionRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.  Also, it's possible that the
			// event handler can be installed before the document is drawn, so don't try to update an individual cell if the mapping
			// information hasn't been constructed yet.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && executionRowChangeEvent.Action == DataRowAction.Commit &&
				executionRowChangeEvent.Row.RowState != DataRowState.Detached)
			{
			
				// Make sure the locks are released.
				try
				{
				
					// Lock the document while we lookup the locations.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
					ClientMarketData.ExecutionRow executionRow = executionRowChangeEvent.Row;
					int blockOrderId = (int)executionRow[ClientMarketData.Execution.BlockOrderIdColumn];

					// Find the cell that corresponds to the given position and update it with the new quantity.
					AddressMapSet.BlockOrderMapRow blockOrderMapRow = this.addressMap.BlockOrderMap.FindByBlockIdColumnId(
						blockOrderId, ColumnType.QuantityExecuted);
					if (blockOrderMapRow != null)
					{

						// Aggregate the executions against this block order.  First, find the block.
						ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);

						// Run through all the executions and count them up.
						decimal quantityExecuted = 0.0M;
						foreach (ClientMarketData.ExecutionRow execution in blockOrderRow.GetExecutionRows())
							quantityExecuted += execution.Quantity;

						// Once the quantity is calculated, this will update the spreadsheet with the new proposed
						// quantity.
						this.SetCell(new CellAddress(blockOrderMapRow.DocumentVersion, blockOrderMapRow.RowIndex,
							blockOrderMapRow.ColumnIndex), quantityExecuted);

					}

				}
				finally
				{

					// Release document.
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

				}

			}

		}
		
		/// <summary>
		/// Event handler for a change to the executions table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void ExecutionRowDeleteEvent(object sender, ClientMarketData.ExecutionRowChangeEvent executionRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.  Also, it's possible that the
			// event handler can be installed before the document is drawn, so don't try to update an individual cell if the mapping
			// information hasn't been constructed yet.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && executionRowChangeEvent.Action == DataRowAction.Delete)
			{
			
				// Make sure the locks are released.
				try
				{
				
					// Lock the document while we lookup the locations.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
					ClientMarketData.ExecutionRow executionRow = executionRowChangeEvent.Row;
					int blockOrderId = (int)executionRow[ClientMarketData.Execution.BlockOrderIdColumn, DataRowVersion.Original];

					// Find the cell that corresponds to the given position and update it with the new quantity.
					AddressMapSet.BlockOrderMapRow blockOrderMapRow = this.addressMap.BlockOrderMap.FindByBlockIdColumnId(
						blockOrderId, ColumnType.QuantityExecuted);
					if (blockOrderMapRow != null)
					{

						// Aggregate the executions against this block order.  First, find the block.
						ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);

						// Run through all the executions and count them up.
						decimal quantityExecuted = 0.0M;
						foreach (ClientMarketData.ExecutionRow execution in blockOrderRow.GetExecutionRows())
							quantityExecuted += execution.Quantity;

						// Once the quantity is calculated, this will update the spreadsheet with the new proposed
						// quantity.
						this.SetCell(new CellAddress(blockOrderMapRow.DocumentVersion, blockOrderMapRow.RowIndex,
							blockOrderMapRow.ColumnIndex), quantityExecuted);

					}

				}
				finally
				{

					// Release document.
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

				}

			}

		}
		
		/// <summary>
		/// Event handler for a change to the placements table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void PlacementRowChangeEvent(object sender, ClientMarketData.PlacementRowChangeEvent placementRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.  Also, it's possible that the
			// event handler can be installed before the document is drawn, so don't try to update an individual cell if the mapping
			// information hasn't been constructed yet.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && placementRowChangeEvent.Action == DataRowAction.Commit &&
				placementRowChangeEvent.Row.RowState != DataRowState.Detached)
			{
			
				// Make sure the locks are released.
				try
				{
				
					// Lock the document while we lookup the locations.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
					ClientMarketData.PlacementRow placementRow = placementRowChangeEvent.Row;
					int blockOrderId = (int)placementRow[ClientMarketData.Placement.BlockOrderIdColumn, DataRowVersion.Default];

					// Find the cell that corresponds to the given position and update it with the new quantity.
					AddressMapSet.BlockOrderMapRow blockOrderMapRow = this.addressMap.BlockOrderMap.FindByBlockIdColumnId(
						blockOrderId, ColumnType.QuantityPlaced);
					if (blockOrderMapRow != null)
					{

						// Aggregate the placements against this block order.  First, find the block.
						ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);

						// Run through all the placements and count them up.
						decimal quantityPlaced = 0.0M;
						foreach (ClientMarketData.PlacementRow placement in blockOrderRow.GetPlacementRows())
							quantityPlaced += placement.Quantity;

						// Once the quantity is calculated, this will update the spreadsheet with the new proposed
						// quantity.
						this.SetCell(new CellAddress(blockOrderMapRow.DocumentVersion, blockOrderMapRow.RowIndex,
							blockOrderMapRow.ColumnIndex), quantityPlaced);

					}

				}
				finally
				{

					// Release document.
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

				}

			}

		}
		
		/// <summary>
		/// Event handler for a change to the placements table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void PlacementRowDeleteEvent(object sender, ClientMarketData.PlacementRowChangeEvent placementRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.  Also, it's possible that the
			// event handler can be installed before the document is drawn, so don't try to update an individual cell if the mapping
			// information hasn't been constructed yet.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && placementRowChangeEvent.Action == DataRowAction.Delete)
			{
			
				// Make sure the locks are released.
				try
				{
				
					// Lock the document while we lookup the locations.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
					ClientMarketData.PlacementRow placementRow = placementRowChangeEvent.Row;
					int blockOrderId = (int)placementRow[ClientMarketData.Placement.BlockOrderIdColumn, DataRowVersion.Original];

					// Find the cell that corresponds to the given position and update it with the new quantity.
					AddressMapSet.BlockOrderMapRow blockOrderMapRow = this.addressMap.BlockOrderMap.FindByBlockIdColumnId(
						blockOrderId, ColumnType.QuantityPlaced);
					if (blockOrderMapRow != null)
					{

						// Aggregate the placements against this block order.  First, find the block.
						ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);

						// Run through all the placements and count them up.
						decimal quantityPlaced = 0.0M;
						foreach (ClientMarketData.PlacementRow placement in blockOrderRow.GetPlacementRows())
								quantityPlaced += placement.Quantity;

						// Once the quantity is calculated, this will update the spreadsheet with the new proposed
						// quantity.
						this.SetCell(new CellAddress(blockOrderMapRow.DocumentVersion, blockOrderMapRow.RowIndex,
							blockOrderMapRow.ColumnIndex), quantityPlaced);

					}

				}
				finally
				{

					// Release document.
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

				}

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

			// Reopen the document if the style sheet has changed.  Note that the stylesheet needs to be reset in order 
			// to coerce the 'OpenDocument' method to reload the stylesheet.
			if (this.IsDocumentOpen && stylesheetRow.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.BlotterId);
			}

		}

		/// <summary>
		/// This handler is called when prices have changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="priceRowChangedEvent">Event argument.</param>
		private void PriceRowChangeEvent(object sender, ClientMarketData.PriceRowChangeEvent priceRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{
			
				// This will make it easier to operate on the record.
				ClientMarketData.PriceRow priceRow = priceRowChangeEvent.Row;
				
				// Make sure the table locks are released.
				try
				{

					// Make sure the address map isn't written while we read it.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Use the address map to filter out ticks that are unused by the current document.  That is, if the security
					// is not part of the currently viewed document, we will discard it from the ticker queue.
					AddressMapSet.SecurityMapRow priceMapRow = this.addressMap.SecurityMap.FindBySecurityId(priceRow.SecurityId);
					if (priceMapRow != null)
					{
				
						// The main idea is to place the changed records into a queue.  In this way, we don't hold up the
						// background and the pricing thread can handle them in it's own time.  Note that we wait until the row has
						// been committed before taking any action on it.  Create a new Tick element and place it into the ticker.
						// This method will also signal waiting threads that a new price tick is in the queue.
						if (priceRowChangeEvent.Action == DataRowAction.Change)
							this.ticker.Enqueue(new Tick(priceRow));

					}

				}
				finally
				{

					// Release the mapping tables (and the document)
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

				}

			}

		}
	
		/// <summary>
		/// Prepares the document for data merged from the server.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments</param>
		private void BeginMerge(object sender, EventArgs e)
		{

			// If the current structure of the displayed document can't hold the data from the merge, then a new document
			// will have to be drawn.  If this is set by any of the event handlers, the individual cell updates will be
			// obviated.
			this.IsRefreshNeeded = false;

		}

		/// <summary>
		/// Will initiate a account refresh if the data or the structure of the document has changed.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// If new data or a new structure is available after the data model has changed, then will initiate a refresh of the
			// viewer contents.
			if (this.IsDocumentOpen && this.IsRefreshNeeded)
				CommandQueue.Execute(new ThreadHandler(DrawCommand), this.BlotterId, this.SortMethod);

		}

		/// <summary>
		/// Called when an placement quantity is added to the block order.
		/// </summary>
		/// <param name="placementEventArgs">Event parameters.</param>
		protected virtual void OnPlacement()
		{
		
			// Broadcast the event to anyone listening.
			if (this.Placement != null)
				this.Placement(this, EventArgs.Empty);

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
		/// <param name="openBlockOrderEventArgs">The event argument.</param>
		protected virtual void OnOpenBlockOrder(BlockOrderEventArgs blockOrderEventArgs)
		{

			// Multicast the fact that the currently viewed block order has changed to anyone
			// interested.  This will typically be the blotter which will inform the placement
			// and execution viewers that a new block is currently being viewed.
			if (OpenBlockOrder != null)
				this.OpenBlockOrder(this, blockOrderEventArgs);

		}

		/// <summary>
		/// Multicasts a change in the currently viewed block order.
		/// </summary>
		/// <param name="closeBlockOrderEventArgs">The event argument.</param>
		protected virtual void OnCloseBlockOrder(BlockOrderEventArgs blockOrderEventArgs)
		{

			// Multicast the fact that the currently viewed block order has changed to anyone
			// interested.  This will typically be the blotter which will inform the placement
			// and execution viewers that a new block is currently being viewed.
			if (CloseBlockOrder != null)
				this.CloseBlockOrder(this, blockOrderEventArgs);

		}

		/// <summary>
		/// This thread will update the prices in the appraisal document.
		/// </summary>
		private void PricerThread()
		{

			// The 'tick' table is a hash table indexed by the security id that contains the most recent tick off the
			// price stream.  The prices are kept in this table long enough to animate them on the document.  When they've
			// reached a certain age, they will be removed from table and won't be animated any longer.
			while (true)
			{

				// Wait here for the next tick to come in.
				this.ticker.WaitForTick();

				// This loop takes care of updating the tick table with the new prices from the queue.
				while (this.ticker.Count != 0)
				{

					// Take the next price record off the queue.
					Tick newTick = this.ticker.Dequeue();

					// Multithreaded objects are used below.  The will need table locks.
					try
					{

						// We will be reading from the Price Address table, writing to the tick table.  The Price address
						// table acts as a filter for incoming ticks.  If it's not in our document, we're not interested
						// in it.  The Tick table is used to animate the incoming price changes.  It remains in the table
						// while we go from a bright color to black.  Once it's completely aged, the tick is removed.
						this.tickTableLock.AcquireWriterLock(CommonTimeout.LockWait);

						// The 'tickTable' contains the last known state of the price changes coming from the ticker
						// plant.  This list is built dynamically: if the tick record doesn't exist yet, add it to the
						// table.  Otherwise, we're going to see if any of the prices have changed from the last state.
						Tick oldTick = this.tickTable[newTick.securityId];
						if (oldTick == null)
							this.tickTable[newTick.securityId] = newTick;
						else
						{

							// See if the new 'last executed price' is different from the last known price.  If it is
							// we'll animate the price change.
							if (oldTick.lastPrice != newTick.lastPrice)
							{

								// See if the security is already being animated.  If it isn't in the animation cache,
								// then we'll install it.  Otherwise, we'll reset the direction and age of the tick.
								Tick animatedTick = this.animatedTickTable[newTick.securityId];
								if (animatedTick == null)
									this.animatedTickTable[newTick.securityId] = animatedTick = newTick;
	
								// Reset the animation sequence with the latest 'last executed price', the direction 
								// (based on the change from the existing tick to the new tick) and the age, which will
								// cause the displayed price to change color shades until it's finally faded to the
								// standard text color.
								animatedTick.lastPrice = newTick.lastPrice;
								animatedTick.lastPriceDirection = (oldTick.lastPrice < newTick.lastPrice) ? TickDirection.Up : TickDirection.Down;
								animatedTick.lastPriceAge = 0;

								// The price stored in the ticker state table is updated at this point to the latest
								// executed price.
								oldTick.lastPrice = newTick.lastPrice;

							}

							// See if the new 'ask price' is different from the last ask price.  If it is we'll animate
							// the change.
							if (oldTick.askPrice != newTick.askPrice)
							{

								// See if the security is already being animated.  If it isn't in the animation cache,
								// then we'll install it.  Otherwise, we'll reset the direction and age of the tick.
								Tick animatedTick = this.animatedTickTable[newTick.securityId];
								if (animatedTick == null)
									this.animatedTickTable[newTick.securityId] = animatedTick = newTick;
	
								// Reset the animation sequence with the latest 'last executed price', the direction 
								// (based on the change from the existing tick to the new tick) and the age, which will
								// cause the displayed price to change color shades until it's finally faded to the
								// standard text color.
								animatedTick.askPrice = newTick.askPrice;
								animatedTick.askPriceDirection = (oldTick.askPrice < newTick.askPrice) ? TickDirection.Up : TickDirection.Down;
								animatedTick.askPriceAge = 0;

								// The price stored in the ticker state table is updated at this point to the latest
								// executed price.
								oldTick.askPrice = newTick.askPrice;

							}

							// See if the new 'bid price' is different from the last bid price.  If it is we'll animate
							// the change.
							if (oldTick.bidPrice != newTick.bidPrice)
							{

								// See if the security is already being animated.  If it isn't in the animation cache,
								// then we'll install it.  Otherwise, we'll reset the direction and age of the tick.
								Tick animatedTick = this.animatedTickTable[newTick.securityId];
								if (animatedTick == null)
									this.animatedTickTable[newTick.securityId] = animatedTick = newTick;
	
								// Reset the animation sequence with the latest 'last executed price', the direction 
								// (based on the change from the existing tick to the new tick) and the age, which will
								// cause the displayed price to change color shades until it's finally faded to the
								// standard text color.
								animatedTick.bidPrice = newTick.bidPrice;
								animatedTick.bidPriceDirection = (oldTick.bidPrice < newTick.bidPrice) ? TickDirection.Up : TickDirection.Down;
								animatedTick.bidPriceAge = 0;

								// The price stored in the ticker state table is updated at this point to the latest
								// executed price.
								oldTick.bidPrice = newTick.bidPrice;

							}
						
						}

					}
					finally
					{

						// Release the tables shared by the threads.
						if (this.tickTableLock.IsWriterLockHeld) this.tickTableLock.ReleaseWriterLock();

					}

					// Spacing the ticks out will make the price feed appear more real-time.  Since the interval can be
					// several seconds apart, this makes it look like the ticks are coming in from a real-time feed.
					Thread.Sleep(this.ticker.Count == 0 ? ClientTimeout.RefreshInterval :
						ClientTimeout.RefreshInterval / this.ticker.Count);

				}

			}

		}

		/// <summary>
		/// Thread to animate the up and down ticks.
		/// </summary>
		private void PriceAgerThread()
		{

			// This loop will animate the prices in the TickTable.  As time goes by, the the ticks will age.  They will
			// change in hue from a bright color to black.  This loop takes care of aging the ticks in the table and
			// eventually removing them when they expire.
			while (true)
			{

				// Make sure table locks are released.
				try
				{

					// The document also conains the lookup map to find cells on the spreadsheet.  The TickTable will be
					// updated with the new age of the tick and expunged of old ticks.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);
					this.tickTableLock.AcquireWriterLock(CommonTimeout.LockWait);

					// IMPORTANT CONCEPT: There is considerable overhead in calling the foreground message loop to have it process
					// results.  The best way around this is to collect a set of operations and send them as a batch to be executed
					// when the message loop gets around to it.  The animation may have many tickers updated every second.  This
					// architecture is the most effective way of using the foreground 'Invoke' method for passing data from a
					// background thread to the foreground user interface objects.
					ArrayList displayBatch = new ArrayList();
					
					// This list is used to delete elements from the tickTable when they've ended their lifetime.  Since
					// we can't delete an element from the hash table while we're iterating through it, we'll copy the key
					// value into this list.  After the hash table has been searched, we'll delete the elements of the
					// tickTable using this list.
					ArrayList deleteList = new ArrayList();

					// Cycle through each element of the animatedTickTable and animate the price value with a color that is based
					// on the age of the tick.
					foreach (DictionaryEntry dictionaryEntry in this.animatedTickTable)
					{

						// Get the next tick we're to animate.
						Tick tick = (Tick)dictionaryEntry.Value;

						// This will find each cell in the spreadsheet that needs to be updated.  Note that a tick may
						// still be in the animation table, but been moved or removed from the document. Also note that we
						// could have both a long and short position that both need to be updated with the same price.
						// That's why we have a loop after finding the security in the SecurityMap table.
						AddressMapSet.SecurityMapRow securityMapRow = this.addressMap.SecurityMap.FindBySecurityId(tick.securityId);
						if (securityMapRow != null)
						{

							// If the price has changed, then run through all the addresses where the given security can 
							// be found and animate the price found in that cell.
							if (tick.lastPriceDirection != TickDirection.Unchanged)
								foreach (AddressMapSet.LastPriceAddressRow lastPriceAddressRow in securityMapRow.GetLastPriceAddressRows())
								{

									// The intensity of the color is related to the age.  The color is based on whether we
									// have an up tick or a down tick.  Note that at the end of it's lifetime, it will be
									// black.  Once we've calculated the color, call the delegate to update the price.
									int intensity = (tick.lastPriceAge < Tick.AgeLimit) ? 0xFF - tick.lastPriceAge * 0x16 : 0x00;
									Color color = Color.FromArgb((tick.lastPriceDirection == TickDirection.Down) ? intensity : 0x00,
										(tick.lastPriceDirection == TickDirection.Up) ? intensity : 0x00, 0x00);

									// This is the cell that will be animated with color hues for the age of the tick.
									CellAddress cellAddress = new CellAddress(lastPriceAddressRow.DocumentVersion,
										lastPriceAddressRow.RowIndex, lastPriceAddressRow.ColumnIndex);

									// The color of the cell will change with every click of this thread.
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetFormat, cellAddress, color));

									// If the tick is brand new, then update the value of the cell after setting the initial color.
									if (tick.lastPriceAge == 0)
										displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, tick.lastPrice));

								}

							// If the price has changed, then run through all the addresses where the given security can 
							// be found and animate the price found in that cell.
							if (tick.askPriceDirection != TickDirection.Unchanged)
								foreach (AddressMapSet.AskPriceAddressRow askPriceAddressRow in securityMapRow.GetAskPriceAddressRows())
								{

									// The intensity of the color is related to the age.  The color is based on whether we
									// have an up tick or a down tick.  Note that at the end of it's lifetime, it will be
									// black.  Once we've calculated the color, call the delegate to update the price.
									int intensity = (tick.askPriceAge < Tick.AgeLimit) ? 0xFF - tick.askPriceAge * 0x16 : 0x00;
									Color color = Color.FromArgb((tick.askPriceDirection == TickDirection.Down) ? intensity : 0x00,
										(tick.askPriceDirection == TickDirection.Up) ? intensity : 0x00, 0x00);

									// This is the cell that will be animated with color hues for the age of the tick.
									CellAddress cellAddress = new CellAddress(askPriceAddressRow.DocumentVersion,
										askPriceAddressRow.RowIndex, askPriceAddressRow.ColumnIndex);

									// The color of the cell will change with every click of this thread.
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetFormat, cellAddress, color));

									// If the tick is brand new, then update the value of the cell after setting the initial color.
									if (tick.askPriceAge == 0)
										displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, tick.askPrice));

								}

							// If the price has changed, then run through all the addresses where the given security can
							// be found and animate the price found in that cell.
							if (tick.bidPriceDirection != TickDirection.Unchanged)
								foreach (AddressMapSet.BidPriceAddressRow bidPriceAddressRow in securityMapRow.GetBidPriceAddressRows())
								{

									// The intensity of the color is related to the age.  The color is based on whether we
									// have an up tick or a down tick.  Note that at the end of it's lifetime, it will be
									// black.  Once we've calculated the color, call the delegate to update the price.
									int intensity = (tick.bidPriceAge < Tick.AgeLimit) ? 0xFF - tick.bidPriceAge * 0x16 : 0x00;
									Color color = Color.FromArgb((tick.bidPriceDirection == TickDirection.Down) ? intensity : 0x00,
										(tick.bidPriceDirection == TickDirection.Up) ? intensity : 0x00, 0x00);

									// This is the cell that will be animated with color hues for the age of the tick.
									CellAddress cellAddress = new CellAddress(bidPriceAddressRow.DocumentVersion,
										bidPriceAddressRow.RowIndex, bidPriceAddressRow.ColumnIndex);

									// The color of the cell will change with every click of this thread.
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetFormat, cellAddress, color));

									// If the tick is brand new, then update the value of the cell after setting the initial color.
									if (tick.bidPriceAge == 0)
										displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, tick.bidPrice));

								}

						}
						
						// Age all of the ticks.  When it's reached it's limit, we'll set the state of the tick to
						// 'Unchanged' to prevent it from being updated again until a new price arrives.
						if (++tick.lastPriceAge > Tick.AgeLimit) tick.lastPriceDirection = TickDirection.Unchanged;
						if (++tick.askPriceAge > Tick.AgeLimit) tick.askPriceDirection = TickDirection.Unchanged;
						if (++tick.bidPriceAge > Tick.AgeLimit) tick.bidPriceDirection = TickDirection.Unchanged;

						// At the end of the lifetime of the tick, we add it to a list of items that will be deleted. Note
						// that we can't delete the tick here because it's part of the iteration.
						if (tick.lastPriceAge > Tick.AgeLimit && tick.askPriceAge > Tick.AgeLimit &&
							tick.bidPriceAge > Tick.AgeLimit)
							deleteList.Add(tick.securityId);

					}

					// Send the batch of commands to the foreground for processing.
					if (displayBatch.Count != 0)
						ExecuteDisplayBatch(displayBatch);

					// After we've animated each tick, check for expired ticks and remove them from the table.
					foreach (int securityId in deleteList)
						this.animatedTickTable.Remove(securityId);

				}
				finally
				{

					// Release the tables shared by the threads.
					if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();
					if (this.tickTableLock.IsWriterLockHeld) this.tickTableLock.ReleaseWriterLock();

				}

				// Sleep a three quarters of a second between animation shades.  This can be made programmable someday if 
				// the user wants control over how long the element is highlighted.
				Thread.Sleep(CommonTimeout.TickAnimation);

			}

		}

		/// <summary>
		/// Handles a command to show the context menu.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument.</param>
		private void axSpreadsheet_BeforeContextMenu(object sender, AxMicrosoft.Office.Interop.Owc11.ISpreadsheetEventSink_BeforeContextMenuEvent e)
		{

			// WORKAROUND: Due to some quirk in the spreadsheet, if the context menu is displayed when the document is 
			// refreshed, that context menu is damaged and produces a 'Null Reference' error in the windows message loop.
			// To work around this problem, we allow this event handler to return and schedule our own hander to be run.
			// Also, it won't do any good to try to create a context menu and pass it back to the spreadsheet: the problem
			// occurs with the standard context menu as well.
			CommandQueue.Execute(new ThreadHandler(ShowContextMenuCommand), new Point(e.x, e.y));

			// This will prevent the spreadsheet control from displaying it's own version of the context menu.
			e.cancel.Value = true;

		}

		/// <summary>
		/// Handles the selection changing from one cell to another.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="selectionChangeArgs">The event argument</param>
		private void BlockOrderViewer_SelectionChange(object sender, Shadows.Quasar.Viewers.SelectionChangeArgs selectionChangeArgs)
		{

			// Extract the current row and column from the event argument.
			int rowIndex = selectionChangeArgs.CellAddress.RowIndex;

			// Get the row type from the selected row.
			int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
			int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

			// The row type defines what action to take based on the selection changing.
			switch (rowType)
			{

			case RowType.Unused:

				// If a block order was selected, then close it.
				if (IsBlockOrderSelected)
					OnCloseBlockOrder(new BlockOrderEventArgs(SelectedBlockOrderId));
				
				// Indicate that an execution is not selected.
				IsBlockOrderSelected = false;

				break;

			case RowType.Placeholder:

				// If a block order was selected, then close it.
				if (IsBlockOrderSelected)
					OnCloseBlockOrder(new BlockOrderEventArgs(SelectedBlockOrderId));
				
				// Indicate that an execution is not selected.
				IsBlockOrderSelected = false;

				break;

			case RowType.BlockOrder:

				// Get the block order identifier from the spreadsheet row.
				int blockOrderIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.BlockOrderId);
				int blockOrderId = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, blockOrderIdColumnIndex)));

				// If the selected block order is not the same as the previously selected block order, then trigger the event that says
				// the block is now closed.
				if (IsBlockOrderSelected && SelectedBlockOrderId != blockOrderId)
					OnCloseBlockOrder(new BlockOrderEventArgs(SelectedBlockOrderId));
				
				// If the selected block is different from the last active block, then we'll trigger any events that
				// need to know about a change in the selected block.
				if (!IsBlockOrderSelected || SelectedBlockOrderId != blockOrderId)
					OnOpenBlockOrder(new BlockOrderEventArgs(blockOrderId));

				// This indicates that an execution is selected and saves information about that execution.  These
				// state variables are needed by the ledger style input to determine when to commit the changes to the
				// server.  The row type tells the 'commit' logic whether a local or global record is identified in
				// the 'selectedBlockOrderId' member.
				IsBlockOrderSelected = true;
				SelectedRowType = rowType;
				SelectedBlockOrderId = blockOrderId;
						 
				break;

			}
	
		}
		
		/// <summary>
		/// Saves the current document.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{

			// Call the base class to save the Spreadsheet data.
			this.SaveAs();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemAscendingName_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 0;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemAscendingQuantityExecuted_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 1;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemAscendingQuantityLeaves_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 2;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemAscendingQuantityOrdered_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 3;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemAscendingSymbol_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 4;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemDescendingName_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 100;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemDescendingQuantityExecuted_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 101;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemDescendingQuantityLeaves_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 102;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemDescendingQuantityOrdered_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 103;
			DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemDescendingSymbol_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.SortMethod = 104;
			DrawDocument();

		}

		private void menuItemTechnicalAnalysis_Click(object sender, System.EventArgs e)
		{

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				foreach (int blockOrderId in GetSelectedBlockOrder())
				{

					ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
					if (blockOrderRow == null)
						throw new Exception(String.Format("Block Order {0} has been deleted", blockOrderId));


					ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(blockOrderRow.SecurityId);

					string url = string.Format(@"C:\Program Files\Mark Three\Quasar\Web Pages\{0}_TA.htm", securityRow.Symbol);

					OnObjectOpen(this, new ObjectArgs(ObjectType.Folder, -1, url));

				}

			}
			finally
			{

				// Release the locks obtained to produce the blotter report.
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		private void menuItemPlacement_Click(object sender, System.EventArgs e)
		{
			OnPlacement();
		}

		private void menuItemManualExecution_Click(object sender, System.EventArgs e)
		{
			OnExecution();
		}

		private void menuItemElectronicExecution_Click(object sender, System.EventArgs e)
		{

			try
			{

				// Prompt the user for an electronic broker to send the order to.
				BrokerDialog brokerDialog = new BrokerDialog();
				if (brokerDialog.ShowDialog() != DialogResult.OK)
					return;

				// Call the thread that will turn the selected proposed orders into real orders.
				CommandQueue.Execute(new ThreadHandler(ExecuteBlockOrderCommand), brokerDialog.BrokerId,
					GetSelectedBlockOrder());

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		private void menuItemAllocate_Click(object sender, System.EventArgs e)
		{
		
			try
			{

				// Call the thread that will turn the selected proposed orders into real orders.
				CommandQueue.Execute(new ThreadHandler(AllocateBlockOrderCommand), GetSelectedBlockOrder());

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

	}

}
