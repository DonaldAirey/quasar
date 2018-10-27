/**********************************************************************************************************************************
*
*	File:			AppraisalViewer.cs
*	Description:	This control is used to display and manage an appraisal (View of Portfolios).
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
**********************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Appraisal
{

	using AxMicrosoft.Office.Interop.Owc11;
	using Microsoft.Office.Interop.Owc11;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Common.Controls;
	using Shadows.Quasar.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Reflection;
	using System.Security.Policy;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.IO;
	using System.Diagnostics;
	using System.Resources;
	using System.Threading;

	/// <summary>
	/// Provides a user interface for interacting with an appraisal.
	/// </summary>
	public class AppraisalViewer : Shadows.Quasar.Viewers.SpreadsheetViewer
	{

		private System.Threading.Thread threadPricer;
		private System.Threading.Thread threadPriceAger;
		private System.Threading.ReaderWriterLock tickTableLock;
		private System.Threading.ReaderWriterLock addressMapLock;
		private System.Threading.ManualResetEvent priceUpdateSignal = new ManualResetEvent(false);
		private System.Xml.Xsl.XslTransform xslTransform;
		private delegate bool SetPositionDelegate(Position position, int appraisalColumnType, object cellValue);
		private delegate bool SetSectorDelegate(int sectorId, int appraisalColumnType, object cellValue);
		private SetPositionDelegate setPositionDelegate;
		private SetSectorDelegate setSectorDelegate;
		private int accountId;
		private int[] childAccountIds;
		private int modelId;
		private Pricing pricing;
		private Ticker ticker;
		private TickTable tickTable;
		private TickTable animatedTickTable;
		private AddressMap addressMap;
		private System.ComponentModel.IContainer components = null;
		private Shadows.Quasar.Client.ClientMarketData clientMarketData;
		private ResourceManager resourceManager;
		private PositionTable changedAllocations = new PositionTable();
		private PositionTable changedOrders = new PositionTable();
		private PositionTable changedProposedOrders = new PositionTable();
		private PositionTable changedTaxLots = new PositionTable();
		private PositionTable changedViolations = new PositionTable();

		/// <summary>
		/// Constructor for the Appraisal Viewer.
		/// </summary>
		public AppraisalViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// Initialize the price queue.  This is the pipeline for new ticks.  The 'TickTable' is used for coloring 
			// the new prices, making them fade as time goes by until they appear to be unchanged.
			this.ticker = new Ticker();
			this.tickTable = new TickTable();
			this.animatedTickTable = new TickTable();

			// The pricing field determines which prices to display in the appraisal (e.g. Last known price, the previous closing 
			// price, the opening price, etc.) for the valuations.
			this.pricing = Pricing.Close;

			// A filter is used to prevent the foreground from being flooded with tick information.  The background uses this table
			// when examining the new ticks.  A lock is used to prevent the ticker threads from using the tables for filtering 
			// while the foreground is creating the address table.
			this.addressMapLock = new ReaderWriterLock();

			// This lock is used to coordinate activity between the thread that reads the ticker queue and the thread that
			// anmiates the prices.
			this.tickTableLock = new ReaderWriterLock();

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
			this.setPositionDelegate = new SetPositionDelegate(SetPositionForeground);
			this.setSectorDelegate = new SetSectorDelegate(SetSectorForeground);
			
			// This thread will refresh the document from the background.  That is, all the hard work is done in the
			// background.  Delegates will be called when the data is actually ready to be put in the control.
			this.threadPricer = new Thread(new ThreadStart(PricerThread));
			this.threadPricer.IsBackground = true;
			this.threadPricer.Name = "PricerThread";
			this.threadPricer.Start();

			// This thread anmiates the ticks by gradually changing color of the tick until it's no longer highlighted.
			this.threadPriceAger = new Thread(new ThreadStart(PriceAgerThread));
			this.threadPriceAger.IsBackground = true;
			this.threadPriceAger.Name = "PriceAgerThread";
			this.threadPriceAger.Start();

			// Open a resource manager to handle the localization of the viewer.
			this.resourceManager = new ResourceManager("Shadows.Quasar.Viewers.Appraisal.Resource",
				Assembly.GetExecutingAssembly());

		}

		#region Dispose Method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// This will prevent recursive destruction of resources.
			if (disposing)
			{

				// Terminate the pricer thread when we're finished.
				if (this.threadPricer != null)
					this.threadPricer.Abort();

				// Terminate the thread that animages the upticks and downticks.
				if (this.threadPriceAger != null)
					this.threadPriceAger.Abort();

				// Remove any components that have been added in.
				if (components != null)
					components.Dispose();

				// It's possible the background threads may have left an 'Invoke' call to update a spreadsheet cell in the
				// system, even after they've been aborted.  Changing the document id insures that anything in the 
				// 'Invoke' queue will not try to update an expired spreadsheet.
				this.IncrementDocumentVersion();

			}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AppraisalViewer));
			this.clientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).BeginInit();
			// 
			// axSpreadsheet
			// 
			this.axSpreadsheet.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.axSpreadsheet.Name = "axSpreadsheet";
			this.axSpreadsheet.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSpreadsheet.OcxState")));
			// 
			// AppraisalViewer
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
			this.Name = "AppraisalViewer";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			((System.ComponentModel.ISupportInitialize)(this.axSpreadsheet)).EndInit();

		}
		#endregion

		// Thread safe access to the account id.
		[Browsable(false)]
		public int AccountId
		{
			get {lock (this) return this.accountId;}
			set {lock (this) this.accountId = value;}
		}

		// Thread safe access to the account id.
		[Browsable(false)]
		public int[] ChildAccountIds
		{
			get {lock (this) return this.childAccountIds;}
			set {lock (this) this.childAccountIds = value;}
		}

		// Thread safe access to the model id.
		[Browsable(false)]
		public int ModelId
		{
			get {lock (this) return this.modelId;}
			set {lock (this) this.modelId = value;}
		}

		/// <summary>
		/// Determines if an account is part of this document.
		/// </summary>
		/// <param name="accountId">The account identifier to be tested.</param>
		/// <returns>true if the account is part of the current document, false otherwise.</returns>
		private bool ContainsAccount(int accountId)
		{

			// When the document is initialized, the 'ChildAccountId' array is populated with each of the accounts in this
			// appraisal.  For a single account, there's only one entry, but a group account could have many. This filter can be
			// replaced with a hash table if it turns out that there's too much iterating through the array.
			foreach (int childId in this.ChildAccountIds)
				if (childId == accountId)
					return true;

			// At this point, the given account isn't part of the document.
			return false;

		}
		
		/// <summary>
		/// Set the contents of a cell in the foreground based on the position and the column type.
		/// </summary>
		/// <param name="position">The key elements of the position line.</param>
		/// <param name="appraisalColumnType">The column type.</param>
		/// <param name="cellValue">The value of the cell.</param>
		private bool SetPositionForeground(Position position, int appraisalColumnType, object cellValue)
		{

			// The calling thread often wants to know if the element was updated or not.  If this is still null when the method is
			// finished, the addressed cell isn't part of this document.
			AddressMapSet.PositionMapRow positionMapRow = null;
			
			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the cell that corresponds to the given position and update it with the new quantity.
				positionMapRow = this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(
					position.SecurityId, position.PositionTypeCode, appraisalColumnType);
				if (positionMapRow != null)
					SetCell(new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex,
						positionMapRow.ColumnIndex), cellValue);

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

			// This will return true if the position is part of this document, false otherwise.
			return (positionMapRow != null);

		}

		/// <summary>
		/// Set the text of a cell in the foreground based on the sector and the column type.
		/// </summary>
		/// <param name="sector">The key elements of the sector line.</param>
		/// <param name="appraisalColumnType">The column type.</param>
		/// <param name="cellValue">The value of the cell.</param>
		private bool SetSectorForeground(int sectorId, int appraisalColumnType, object cellValue)
		{

			// The calling thread often wants to know if the element was updated or not.  If this is still null when the method is
			// finished, the addressed cell isn't part of this document.
			AddressMapSet.SectorMapRow sectorMapRow = null;
			
			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the cell that corresponds to the given sector and update it with the new quantity.
				sectorMapRow = this.addressMap.SectorMap.FindBySectorIdColumnId(sectorId, appraisalColumnType);
				if (sectorMapRow != null)
					SetCell(new CellAddress(sectorMapRow.DocumentVersion,
						sectorMapRow.RowIndex, sectorMapRow.ColumnIndex), cellValue);

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

			// This will return true if the sector is part of this document, false otherwise.
			return (sectorMapRow != null);

		}

		/// <summary>
		/// Gets a list of the selected positions.
		/// </summary>
		/// <returns>A list of the currently selected positions.</returns>
		private ArrayList GetSelectedSectors()
		{

			// The main idea here is to collect the selected positions in the foreground and pass that list off to the background
			// for processing.  Sending the proposed orders requires access to the data model which is prohibited from foreground
			// threads.
			ArrayList selectedSectors = new ArrayList();
			
			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Cycle through the current selection in the appraisal looking for proposed orders that can be turned into tradable
				// orders.
				foreach (Range selectedRow in this.axSpreadsheet.Selection)
				{

					// Get the row type from the selected row.
					int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
					int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, selectedRow.Row, rowTypeColumnIndex)));

					// Execute the command if we're on a position line.
					if (rowType == RowType.Sector)
					{

						// Get the sector id from the selected row.
						int sectorIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SectorId);
						int sectorId = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion,
							selectedRow.Row, sectorIdColumnIndex)));

						// Add the position to the list that will be passed to the background thread for processing.
						selectedSectors.Add(sectorId);

					}

				}
			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

			// Return a list of the selected blocks.
			return selectedSectors;

		}

		/// <summary>
		/// Gets a list of the selected positions.
		/// </summary>
		/// <returns>A list of the currently selected positions.</returns>
		public ArrayList GetSelectedPositions()
		{

			// The main idea here is to collect the selected positions in the foreground and pass that list off to the background
			// for processing.  Sending the proposed orders requires access to the data model which is prohibited from foreground
			// threads.
			ArrayList selectedPositions = new ArrayList();
			
			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Cycle through the current selection in the appraisal looking for proposed orders that can be turned into tradable
				// orders.
				foreach (Range selectedRow in this.axSpreadsheet.Selection)
				{

					// Get the row type from the selected row.
					int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
					int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, selectedRow.Row, rowTypeColumnIndex)));

					// Execute the command if we're on a position line.
					if (rowType == RowType.Position)
					{

						// Get the security id from the selected row.
						int securityIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SecurityId);
						int securityId = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion,
							selectedRow.Row, securityIdColumnIndex)));

						// Get the position type code from the selected row.
						int positionTypeCodeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PositionTypeCode);
						int positionTypeCode = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion,
							selectedRow.Row, positionTypeCodeColumnIndex)));

						// Add the position to the list that will be passed to the background thread for processing.
						selectedPositions.Add(new Position(this.AccountId, securityId, positionTypeCode));

					}

				}

			}
			finally
			{

				// Release the address map
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

			// Return a list of the selected blocks.
			return selectedPositions;

		}

		/// <summary>
		/// Background command to open the appraisal.
		/// </summary>
		/// <param name="accountId">The account id of the appraisal</param>
		private void OpenViewerCommand(params object[] argument)
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install Event Handlers
				ClientMarketData.BeginMerge += new EventHandler(BeginMerge);
				ClientMarketData.Account.AccountRowChanged += new ClientMarketData.AccountRowChangeEventHandler(this.AccountRowChangeEvent);
				ClientMarketData.Account.AccountRowDeleted += new ClientMarketData.AccountRowChangeEventHandler(this.AccountRowChangeEvent);
				ClientMarketData.Allocation.AllocationRowChanged += new ClientMarketData.AllocationRowChangeEventHandler(this.AllocationRowChangeEvent);
				ClientMarketData.Allocation.AllocationRowDeleted += new ClientMarketData.AllocationRowChangeEventHandler(this.AllocationRowChangeEvent);
				ClientMarketData.Currency.CurrencyRowChanged += new ClientMarketData.CurrencyRowChangeEventHandler(this.CurrencyRowChangeEvent);
				ClientMarketData.Currency.CurrencyRowDeleted += new ClientMarketData.CurrencyRowChangeEventHandler(this.CurrencyRowChangeEvent);
				ClientMarketData.Debt.DebtRowChanged += new ClientMarketData.DebtRowChangeEventHandler(this.DebtRowChangeEvent);
				ClientMarketData.Debt.DebtRowDeleted += new ClientMarketData.DebtRowChangeEventHandler(this.DebtRowChangeEvent);
				ClientMarketData.Equity.EquityRowChanged += new ClientMarketData.EquityRowChangeEventHandler(this.EquityRowChangeEvent);
				ClientMarketData.Equity.EquityRowDeleted += new ClientMarketData.EquityRowChangeEventHandler(this.EquityRowChangeEvent);
				ClientMarketData.Model.ModelRowChanged += new ClientMarketData.ModelRowChangeEventHandler(this.ModelRowChangeEvent);
				ClientMarketData.Model.ModelRowDeleted += new ClientMarketData.ModelRowChangeEventHandler(this.ModelRowDeleteEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowDeleted += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged += new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.Order.OrderRowDeleted += new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.PositionTarget.PositionTargetRowChanged += new ClientMarketData.PositionTargetRowChangeEventHandler(this.PositionTargetRowChangeEvent);
				ClientMarketData.PositionTarget.PositionTargetRowDeleted += new ClientMarketData.PositionTargetRowChangeEventHandler(this.PositionTargetRowChangeEvent);
				ClientMarketData.ProposedOrder.ProposedOrderRowChanged += new ClientMarketData.ProposedOrderRowChangeEventHandler(this.ProposedOrderRowChangeEvent);
				ClientMarketData.ProposedOrder.ProposedOrderRowDeleted += new ClientMarketData.ProposedOrderRowChangeEventHandler(this.ProposedOrderRowDeleteEvent);
				ClientMarketData.Restriction.RestrictionRowChanged += new ClientMarketData.RestrictionRowChangeEventHandler(this.RestrictionRowChangeEvent);
				ClientMarketData.Restriction.RestrictionRowDeleted += new ClientMarketData.RestrictionRowChangeEventHandler(this.RestrictionRowChangeEvent);
				ClientMarketData.Scheme.SchemeRowChanged += new ClientMarketData.SchemeRowChangeEventHandler(this.SchemeRowChangeEvent);
				ClientMarketData.Scheme.SchemeRowDeleted += new ClientMarketData.SchemeRowChangeEventHandler(this.SchemeRowChangeEvent);
				ClientMarketData.Sector.SectorRowChanged += new ClientMarketData.SectorRowChangeEventHandler(this.SectorRowChangeEvent);
				ClientMarketData.Sector.SectorRowDeleted += new ClientMarketData.SectorRowChangeEventHandler(this.SectorRowChangeEvent);
				ClientMarketData.SectorTarget.SectorTargetRowChanged += new ClientMarketData.SectorTargetRowChangeEventHandler(this.SectorTargetRowChangeEvent);
				ClientMarketData.SectorTarget.SectorTargetRowDeleted += new ClientMarketData.SectorTargetRowChangeEventHandler(this.SectorTargetRowChangeEvent);
				ClientMarketData.Security.SecurityRowChanged += new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Security.SecurityRowDeleted += new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted += new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.TaxLot.TaxLotRowChanged += new ClientMarketData.TaxLotRowChangeEventHandler(this.TaxLotRowChangeEvent);
				ClientMarketData.TaxLot.TaxLotRowDeleted += new ClientMarketData.TaxLotRowChangeEventHandler(this.TaxLotRowChangeEvent);
				ClientMarketData.Violation.ViolationRowChanged += new ClientMarketData.ViolationRowChangeEventHandler(this.ViolationRowChangeEvent);
				ClientMarketData.Violation.ViolationRowDeleted += new ClientMarketData.ViolationRowChangeEventHandler(this.ViolationRowDeleteEvent);
				ClientMarketData.EndMerge += new EventHandler(EndMerge);

				// If real-time pricing is requested, then install the handlers for the ticks.  Otherwise, clear the signal that
				// drives the background processing of the real-time animation.
				if (ClientPreferences.Pricing == Pricing.Last)
				{
					ClientMarketData.Price.PriceRowChanged += new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);
					this.priceUpdateSignal.Set();
				}
				else
				{
					this.priceUpdateSignal.Reset();
				}

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
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsWriterLockHeld) ClientMarketData.ModelLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectTreeLock.IsWriterLockHeld) ClientMarketData.ObjectTreeLock.ReleaseWriterLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PositionTargetLock.IsWriterLockHeld) ClientMarketData.PositionTargetLock.ReleaseWriterLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SectorTargetLock.IsWriterLockHeld) ClientMarketData.SectorTargetLock.ReleaseWriterLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the document is now open.  The container will use this as an indication that it can make the
			// viewer the top-level window.
			OnEndOpenViewer();

		}

		/// <summary>
		/// Closes the Appraisal Viewer.
		/// </summary>
		private void CloseViewerCommand(params object[] argument)
		{

			// Catch any problems trying to disengage the viewer.
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Disengage the event handlers from the data model.
				ClientMarketData.BeginMerge -= new EventHandler(BeginMerge);
				ClientMarketData.Account.AccountRowChanged -= new ClientMarketData.AccountRowChangeEventHandler(this.AccountRowChangeEvent);
				ClientMarketData.Account.AccountRowDeleted -= new ClientMarketData.AccountRowChangeEventHandler(this.AccountRowChangeEvent);
				ClientMarketData.Allocation.AllocationRowChanged -= new ClientMarketData.AllocationRowChangeEventHandler(this.AllocationRowChangeEvent);
				ClientMarketData.Allocation.AllocationRowDeleted -= new ClientMarketData.AllocationRowChangeEventHandler(this.AllocationRowChangeEvent);
				ClientMarketData.Currency.CurrencyRowChanged -= new ClientMarketData.CurrencyRowChangeEventHandler(this.CurrencyRowChangeEvent);
				ClientMarketData.Currency.CurrencyRowDeleted -= new ClientMarketData.CurrencyRowChangeEventHandler(this.CurrencyRowChangeEvent);
				ClientMarketData.Debt.DebtRowChanged -= new ClientMarketData.DebtRowChangeEventHandler(this.DebtRowChangeEvent);
				ClientMarketData.Debt.DebtRowDeleted -= new ClientMarketData.DebtRowChangeEventHandler(this.DebtRowChangeEvent);
				ClientMarketData.Equity.EquityRowChanged -= new ClientMarketData.EquityRowChangeEventHandler(this.EquityRowChangeEvent);
				ClientMarketData.Equity.EquityRowDeleted -= new ClientMarketData.EquityRowChangeEventHandler(this.EquityRowChangeEvent);
				ClientMarketData.Model.ModelRowChanged -= new ClientMarketData.ModelRowChangeEventHandler(this.ModelRowChangeEvent);
				ClientMarketData.Model.ModelRowDeleted -= new ClientMarketData.ModelRowChangeEventHandler(this.ModelRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowDeleted -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.Order.OrderRowChanged -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.Order.OrderRowDeleted -= new ClientMarketData.OrderRowChangeEventHandler(this.OrderRowChangeEvent);
				ClientMarketData.PositionTarget.PositionTargetRowChanged -= new ClientMarketData.PositionTargetRowChangeEventHandler(this.PositionTargetRowChangeEvent);
				ClientMarketData.PositionTarget.PositionTargetRowDeleted -= new ClientMarketData.PositionTargetRowChangeEventHandler(this.PositionTargetRowChangeEvent);
				ClientMarketData.ProposedOrder.ProposedOrderRowChanged -= new ClientMarketData.ProposedOrderRowChangeEventHandler(this.ProposedOrderRowChangeEvent);
				ClientMarketData.ProposedOrder.ProposedOrderRowDeleted -= new ClientMarketData.ProposedOrderRowChangeEventHandler(this.ProposedOrderRowDeleteEvent);
				ClientMarketData.Restriction.RestrictionRowChanged -= new ClientMarketData.RestrictionRowChangeEventHandler(this.RestrictionRowChangeEvent);
				ClientMarketData.Restriction.RestrictionRowDeleted -= new ClientMarketData.RestrictionRowChangeEventHandler(this.RestrictionRowChangeEvent);
				ClientMarketData.Scheme.SchemeRowChanged -= new ClientMarketData.SchemeRowChangeEventHandler(this.SchemeRowChangeEvent);
				ClientMarketData.Scheme.SchemeRowDeleted -= new ClientMarketData.SchemeRowChangeEventHandler(this.SchemeRowChangeEvent);
				ClientMarketData.Sector.SectorRowChanged -= new ClientMarketData.SectorRowChangeEventHandler(this.SectorRowChangeEvent);
				ClientMarketData.Sector.SectorRowDeleted -= new ClientMarketData.SectorRowChangeEventHandler(this.SectorRowChangeEvent);
				ClientMarketData.SectorTarget.SectorTargetRowChanged -= new ClientMarketData.SectorTargetRowChangeEventHandler(this.SectorTargetRowChangeEvent);
				ClientMarketData.SectorTarget.SectorTargetRowDeleted -= new ClientMarketData.SectorTargetRowChangeEventHandler(this.SectorTargetRowChangeEvent);
				ClientMarketData.Security.SecurityRowChanged -= new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Security.SecurityRowDeleted -= new ClientMarketData.SecurityRowChangeEventHandler(this.SecurityRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowChanged -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.Stylesheet.StylesheetRowDeleted -= new ClientMarketData.StylesheetRowChangeEventHandler(this.StylesheetRowChangeEvent);
				ClientMarketData.TaxLot.TaxLotRowChanged -= new ClientMarketData.TaxLotRowChangeEventHandler(this.TaxLotRowChangeEvent);
				ClientMarketData.TaxLot.TaxLotRowDeleted -= new ClientMarketData.TaxLotRowChangeEventHandler(this.TaxLotRowChangeEvent);
				ClientMarketData.Violation.ViolationRowChanged -= new ClientMarketData.ViolationRowChangeEventHandler(this.ViolationRowChangeEvent);
				ClientMarketData.Violation.ViolationRowDeleted -= new ClientMarketData.ViolationRowChangeEventHandler(this.ViolationRowDeleteEvent);
				ClientMarketData.EndMerge -= new EventHandler(EndMerge);

				// If real-time pricing is active, disengage the handler for it.
				if (ClientPreferences.Pricing == Pricing.Last)
					ClientMarketData.Price.PriceRowChanged -= new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);

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
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsWriterLockHeld) ClientMarketData.ModelLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectTreeLock.IsWriterLockHeld) ClientMarketData.ObjectTreeLock.ReleaseWriterLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PositionTargetLock.IsWriterLockHeld) ClientMarketData.PositionTargetLock.ReleaseWriterLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SectorTargetLock.IsWriterLockHeld) ClientMarketData.SectorTargetLock.ReleaseWriterLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Broadcast the fact that the document is now closed.  This is required to initialize the document properly the next
			// time it is opened.
			OnEndCloseViewer();

		}

		/// <summary>
		/// Opens the Appraisal Document.
		/// </summary>
		/// <param name="accountId">The account id of the appraisal</param>
		private void OpenDocumentCommand(params object[] argument)
		{

			// Extract the command argument.
			int accountId = (int)argument[0];

			// Set the account identifier of this viewer.
			this.AccountId = accountId;

			// Select a model for this document.
			this.ModelId = Models.SelectModel(accountId);

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Collect the list of child accounts.  This list is used in the event handlers to filter out events that are not
				// relevant to this appraisal.  Since this document is an aggregation of one or more accounts, all events that are 
				// not related to this account -- or the child accounts -- can be ignored.  This list allows the logic to ignore
				// messages from non-child accounts.
				this.ChildAccountIds = Shadows.Quasar.Common.Account.GetAccountList(accountId);

				// Find the account record that is being opened.  This will be used to configure the stylesheet and construct a
				// model appropriate for the viewer.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", accountId));

				// Find the model created by the logic above.
				ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(modelId);
				if (modelRow == null)
					throw new Exception(String.Format("Model {0} has been deleted", modelId));

				// See if a stylesheet has been associated with this account.
				ClientMarketData.StylesheetRow stylesheetRow = accountRow.StylesheetRow;
			
				// As an optimization, don't reload the stylesheet if the prevous document used the same stylesheet.
				if (this.IsStylesheetChanged || this.StylesheetId != stylesheetRow.StylesheetId)
				{

					// Keep track of the stylesheet id in case it is changed while we're viewing it.  The event handler will use
					// this id to determine if an incoming stylesheet will trigger a refresh of the document.
					this.StylesheetId = stylesheetRow.StylesheetId;

					// Load the stylesheet from the table.
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

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsWriterLockHeld) ClientMarketData.ModelLock.ReleaseWriterLock();
				if (ClientMarketData.SectorTargetLock.IsWriterLockHeld) ClientMarketData.SectorTargetLock.ReleaseWriterLock();
				if (ClientMarketData.PositionTargetLock.IsWriterLockHeld) ClientMarketData.PositionTargetLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectLock.IsWriterLockHeld) ClientMarketData.ObjectLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectTreeLock.IsWriterLockHeld) ClientMarketData.ObjectTreeLock.ReleaseWriterLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// This will reset the viewer to the home position when it opens.
			this.ResetViewer = true;
			
			// Once the document has been configured, it is drawn for the first time.  Note that this has to be outside of the
			// table locks because the 'DrawDocumentCommand' method will aquire it's own locks (and to prevent deadlocking, locking can't
			// be nested).
			DrawDocumentCommand(this.AccountId, this.ModelId);

			// Broadcast the fact that the document is now open.  The container will use this as an indication that it can make the
			// viewer the top-level window.
			OnEndOpenDocument();

		}

		/// <summary>
		/// Close the Order Document.
		/// </summary>
		private void CloseDocumentCommand(params object[] argument)
		{

			// Call the base class to close the document.
			OnEndCloseDocument();

			RemoteBatch remoteBatch = null;

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ModelLock.AcquireWriterLock(CommonTimeout.LockWait);

				ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(this.ModelId);
				if (modelRow == null)
					throw new Exception(String.Format("Model {0} has been deleted", this.ModelId));

				if (modelRow.Temporary)
				{

					// This command batch will delete the model if it is temporary.
					remoteBatch = new RemoteBatch();
					RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

					// Delete the temporary model.
					RemoteMethod remoteMethod = remoteType.Methods.Add("Delete");
					remoteMethod.Parameters.Add("modelId", modelRow.ModelId);
					remoteMethod.Parameters.Add("rowVersion", modelRow.RowVersion);

				}
				
			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ModelLock.IsWriterLockHeld) ClientMarketData.ModelLock.ReleaseWriterLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}
				
			// If the process to close the document produced a batch for cleaning up temporary records, then send it off to the
			// server to be processed.
			if (remoteBatch != null)
				ClientMarketData.Send(remoteBatch);
			
		}
		
		/// <summary>
		/// Redraws the document with the current data.
		/// </summary>
		public override void DrawDocument()
		{

			// Use the command thread to redraw this document.
			CommandQueue.Execute(new ThreadHandler(DrawDocumentCommand), this.AccountId, this.ModelId);

		}
		
		/// <summary>
		/// Creates the DOM and Refreshes the spreadsheet control in the background.
		/// </summary>
		/// <remarks>
		/// This thread is responsible for recreating the Document Object Model for the account and translating
		/// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
		/// DOM is complete to either update the entire document, if the structure has changed, or only update
		/// the changed cells, when only the data has changed.
		/// </remarks>
		private void DrawDocumentCommand(params object[] argument)
		{

			// Extract the command argument.
			int accountId = (int)argument[0];
			int modelId = (int)argument[1];

			// This XML document will be populated by this method and sent to the foreground as the
			// data for the spreadsheet control.
			XmlDocument spreadsheetDocument = new XmlDocument();

			try
			{

				// Lock the tables needed to create the document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AlgorithmLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Lock the Address Map
				this.addressMapLock.AcquireWriterLock(CommonTimeout.LockWait);

				// Make sure the account still exists in the in-memory database.  We need it to rebalance the appraisal.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted.", accountId));

				// Make sure the model still exists in the in-memory database.  We need it to rebalance the appraisal.
				ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(modelId);
				if (modelRow == null)
					throw new Exception(String.Format("Model {0} has been deleted.", modelId));

				// This will create am appraisal DOM using the account and model information.
				AppraisalDocument appraisalDocument = new AppraisalDocument(accountRow, modelRow);
			
#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Dump the DOM to a file.  This is very useful when debugging.
					appraisalDocument.Save("appraisalDOM.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// These variables indicate what cells should have highlighting for editing.
				XsltArgumentList xsltArgumentList = new XsltArgumentList();
				xsltArgumentList.AddParam("isSectorModel", string.Empty, modelRow.ModelTypeCode == ModelType.Sector);
				xsltArgumentList.AddParam("isPositionModel", string.Empty, modelRow.ModelTypeCode == ModelType.Security);

				// Create an XML document in the Microsoft XML Spreadsheet format.  This document will either be fed into the
				// spreadsheet control directly, when there is a major format change, or will be updated incrementally when only
				// the content has changed.
				spreadsheetDocument.Load(xslTransform.Transform(appraisalDocument, xsltArgumentList, (XmlResolver)null));

#if DEBUG
				// We need to make sure that an error writing the debug file doesn't disturb the debug session.
				try
				{

					// Write out the intermediate spreadsheet document when debugging.
					spreadsheetDocument.Save("AppraisalSpreadsheet.xml");

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
#endif

				// Parse the xml document for cell addreses.  The address map must be made in the foreground so it can be 
				// used in the foreground without locks.
				this.addressMap = new AddressMap(spreadsheetDocument, this.IncrementDocumentVersion());

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AlgorithmLock.IsReaderLockHeld) ClientMarketData.AlgorithmLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsReaderLockHeld) ClientMarketData.ModelLock.ReleaseReaderLock();
				if (ClientMarketData.SectorTargetLock.IsReaderLockHeld) ClientMarketData.SectorTargetLock.ReleaseReaderLock();
				if (ClientMarketData.PositionTargetLock.IsReaderLockHeld) ClientMarketData.PositionTargetLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the address map
				if (this.addressMapLock.IsWriterLockHeld) this.addressMapLock.ReleaseWriterLock();

			}

			// At this point, we've constructed a string that is compatible with the XML format of the spreadsheet control.  We
			// can invoke the foreground thread to move the data into the control.
			SetDocument(spreadsheetDocument);

		}

		/// <summary>
		/// SetS the target percentage for a sector model.
		/// </summary>
		/// <param name="argument">Thread initialization arguments</param>
		private void SetSectorTarget(params object[] argument)
		{

			// This batch will be filled in with a command to insert, update or remote the sector target.
			RemoteBatch remoteBatch = null;

			try
			{
			
				// Extract the command parameters.
				int sectorId = (int)argument[0];
				decimal percent = Percent.Parse((string)argument[1]);

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.SectorTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the record.
				ClientMarketData.SectorTargetRow sectorTargetRow = ClientMarketData.SectorTarget.FindByModelIdSectorId(this.ModelId, sectorId);

				// Create a sector target if one doesn't exist.
				if (sectorTargetRow == null)
				{

					// Create a command batch to insert a sector target.
					remoteBatch = new RemoteBatch();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.SectorTarget");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
					remoteMethod.Parameters.Add("modelId", this.ModelId);
					remoteMethod.Parameters.Add("sectorId", sectorId);
					remoteMethod.Parameters.Add("percent", percent);

				}
				else
				{

					// Create a command batch to update an existing sector target.
					remoteBatch = new RemoteBatch();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.SectorTarget");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
					remoteMethod.Parameters.Add("modelId", this.ModelId);
					remoteMethod.Parameters.Add("rowVersion", sectorTargetRow.RowVersion);
					remoteMethod.Parameters.Add("sectorId", sectorId);
					remoteMethod.Parameters.Add("percent", percent);

				}				
				
			}
			catch (System.FormatException formatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				MessageBox.Show(formatException.Message, resourceManager.GetString("quasarError"), MessageBoxButtons.OK,
					MessageBoxIcon.Error);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.SectorTargetLock.IsReaderLockHeld) ClientMarketData.SectorTargetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Execute the command batch on the server.
			ClientMarketData.Send(remoteBatch);
		
		}

		/// <summary>
		/// SetS the target percentage for a sector model.
		/// </summary>
		/// <param name="argument">Thread initialization arguments</param>
		private void ClearSectorTarget(params object[] argument)
		{

			// This batch will be filled in with a command to insert, update or remote the sector target.
			RemoteBatch remoteBatch = null;

			try
			{
			
				// Extract the command parameters.
				ArrayList arrayList = (ArrayList)argument[0];

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.SectorTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Create a command batch to delete the sector target.
				remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

				foreach (int sectorId in arrayList)
				{

					// Delete the record if it still exists.
					ClientMarketData.SectorTargetRow sectorTargetRow = ClientMarketData.SectorTarget.FindByModelIdSectorId(this.ModelId, sectorId);
					if (sectorTargetRow != null)
					{

						RemoteMethod remoteMethod = remoteType.Methods.Add("DeleteSector");
						remoteMethod.Parameters.Add("rowVersion", sectorTargetRow.RowVersion);
						remoteMethod.Parameters.Add("modelId", this.ModelId);
						remoteMethod.Parameters.Add("sectorId", sectorId);

					}

				}
				
			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.SectorTargetLock.IsReaderLockHeld) ClientMarketData.SectorTargetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Execute the command batch on the server.
			ClientMarketData.Send(remoteBatch);
		
		}

		/// <summary>
		/// SetS the target percentage for a sector model.
		/// </summary>
		/// <param name="argument">Thread initialization arguments</param>
		private void SetPositionTarget(params object[] argument)
		{

			// This batch will be filled in with a command to insert, update or remote the sector target.
			RemoteBatch remoteBatch = null;

			try
			{
			
				// Extract the command parameters.
				Position position = (Position)argument[0];
				decimal percent = Percent.Parse((string)argument[1]);

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.PositionTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the record.
				ClientMarketData.PositionTargetRow positionTargetRow =
					ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(this.ModelId, position.SecurityId,
					position.PositionTypeCode);

				// Create a position target if one doesn't exist.
				if (positionTargetRow == null)
				{

					// Create a command batch to insert a position target.
					remoteBatch = new RemoteBatch();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.PositionTarget");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
					remoteMethod.Parameters.Add("modelId", this.ModelId);
					remoteMethod.Parameters.Add("securityId", position.SecurityId);
					remoteMethod.Parameters.Add("positionTypeCode", position.PositionTypeCode);
					remoteMethod.Parameters.Add("percent", percent);

				}
				else
				{

					// Create a command batch to update an existing sector target.
					remoteBatch = new RemoteBatch();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.PositionTarget");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
					remoteMethod.Parameters.Add("modelId", this.ModelId);
					remoteMethod.Parameters.Add("rowVersion", positionTargetRow.RowVersion);
					remoteMethod.Parameters.Add("securityId", position.SecurityId);
					remoteMethod.Parameters.Add("positionTypeCode", position.PositionTypeCode);
					remoteMethod.Parameters.Add("percent", percent);

				}				
				
			}
			catch (System.FormatException formatException)
			{

				// Call a common method to handle all the exceptions when setting local fields.
				MessageBox.Show(formatException.Message, resourceManager.GetString("quasarError"), MessageBoxButtons.OK,
					MessageBoxIcon.Error);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.PositionTargetLock.IsReaderLockHeld) ClientMarketData.PositionTargetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Execute the command batch on the server.
			ClientMarketData.Send(remoteBatch);
		
		}

		/// <summary>
		/// SetS the target percentage for a sector model.
		/// </summary>
		/// <param name="argument">Thread initialization arguments</param>
		private void ClearPositionTarget(params object[] argument)
		{

			// This batch will be filled in with a command to insert, update or remote the sector target.
			RemoteBatch remoteBatch = null;

			try
			{
			
				// Extract the command parameters.
				ArrayList arrayList = (ArrayList)argument[0];

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.PositionTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Create a command batch to delete the sector target.
				remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

				foreach (Position position in arrayList)
				{
				
					// Find the position target.
					ClientMarketData.PositionTargetRow positionTargetRow =
						ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(this.ModelId, position.SecurityId,
						position.PositionTypeCode);

					// Delete the record if it still exists.
					if (positionTargetRow != null)
					{

						RemoteMethod remoteMethod = remoteType.Methods.Add("DeletePosition");
						remoteMethod.Parameters.Add("rowVersion", positionTargetRow.RowVersion);
						remoteMethod.Parameters.Add("modelId", this.ModelId);
						remoteMethod.Parameters.Add("securityId", position.SecurityId);
						remoteMethod.Parameters.Add("positionTypeCode", position.PositionTypeCode);

					}

				}
				
			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.PositionTargetLock.IsReaderLockHeld) ClientMarketData.PositionTargetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Execute the command batch on the server.
			ClientMarketData.Send(remoteBatch);
		
		}

		/// <summary>
		/// SetS the proposed quantity for a position
		/// </summary>
		/// <param name="position">The destination position.</param>
		/// <param name="quantity">The quantity for the destination position.</param>
		private void SetProposedQuantityCommand(params object[] argument)
		{

			// Extract the command parameters.
			Position position = (Position)argument[0];
			decimal quantity = Quantity.Parse((string)argument[1]);

			// This will do the hard work of sending the order to the server, updating the in-memory data model and refreshing the
			// viewer with the results.
			ServiceQueue.Execute(new ThreadHandler(ProposedOrder.Update), accountId, position.SecurityId, position.PositionTypeCode,
				quantity);

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

			// Clear out the table of updated positions at the begining of each merge.  When the end of merge event is triggered,
			// this list can be scanned for changed positions.  The idea is to accelerate the update my getting rid of redundant
			// updates when looking at an aggregate position.
			this.changedAllocations.Clear();
			this.changedOrders.Clear();
			this.changedProposedOrders.Clear();
			this.changedTaxLots.Clear();
			this.changedViolations.Clear();

		}

		/// <summary>
		/// Will initiate a account refresh if the data or the structure of the document has changed.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		protected void EndMerge(object sender, EventArgs e)
		{

			// This method is designed to handle updating the displayed document when the merge of new data is complete.  If the
			// document hasn't been opened yet, then there's nothing to do.
			if (!this.IsDocumentOpen)
				return;

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Make sure the address map isn't written while we read it.
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will capture individual updates to the display document.  If -- after processing the result of the merge -- the
				// displayed document can be updated with individual updates to the screen, this array will hold the commands to do a
				// surgical update.  If the structure of the document has changed, the document will be recreated and redrawn.
				ArrayList displayBatch = new ArrayList();

				// Fill in the command batch with the results of merging the various tables.
				UpdateAllocations(displayBatch);
				UpdateOrders(displayBatch);
				UpdateProposedOrders(displayBatch);
				UpdateTaxLots(displayBatch);
				UpdateViolations(displayBatch);

				// If new data or a new structure is available after the data model has changed, then will initiate a refresh of the
				// account viewer contents.
				if (this.IsRefreshNeeded)
				{

					// Completely redraw the current document with the new data.
					DrawDocument();

				}
				else
				{
					if (displayBatch.Count != 0)
						this.ExecuteDisplayBatch(displayBatch);
				}
			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

				// Release the mapping tables (and the document)
				if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

			}

		}

		/// <summary>
		/// Populates a batch of commands that will update the display document with the changed Proposed Orders.
		/// </summary>
		/// <param name="displayBatch">A batch of dispay commands.</param>
		private void UpdateAllocations(ArrayList displayBatch)
		{

			// Create the update conditions for each of the changed proposed orders.
			foreach (PositionRow positionRow in this.changedAllocations)
			{

				// Find the parent account row of this appraisal report.  All market value caluclations will be in terms of the
				// displayed appraisal, even though it may have been one of the child accounts that was modified.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(positionRow.AccountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", positionRow.AccountId));
				
				// Look up the parent security row.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(positionRow.SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} has been deleted", positionRow.SecurityId));
				
				// Calculate the proposed order quantity.
				decimal quantity = Quantity.Calculate(accountRow, securityRow, positionRow.PositionTypeCode,
					MarketValueFlags.IncludeChildAccounts | MarketValueFlags.IncludeAllocation);

				// Find the location of the cell on the current document.  If the cell doesn't exist, then the entire document will
				// need to be redrawn.  If the cell is currently part of the displayed document, then an individual update will 
				// cause a less disturbing update to the document.
				AddressMapSet.PositionMapRow positionMapRow =
					this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(positionRow.SecurityId,
					positionRow.PositionTypeCode, ColumnType.AllocatedQuantity);
				if (positionMapRow == null)
					this.IsRefreshNeeded = true;
				else
					displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell,
						new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
						quantity));

			}

		}

		/// <summary>
		/// Populates a batch of commands that will update the display document with the changed Proposed Orders.
		/// </summary>
		/// <param name="displayBatch">A batch of dispay commands.</param>
		private void UpdateOrders(ArrayList displayBatch)
		{

			// Create the update conditions for each of the changed proposed orders.
			foreach (PositionRow positionRow in this.changedOrders)
			{

				// Find the parent account row of this appraisal report.  All market value caluclations will be in terms of the
				// displayed appraisal, even though it may have been one of the child accounts that was modified.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(positionRow.AccountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", positionRow.AccountId));
				
				// Look up the parent security row.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(positionRow.SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} has been deleted", positionRow.SecurityId));
				
				// Calculate the proposed order quantity.
				decimal quantity = Quantity.Calculate(accountRow, securityRow, positionRow.PositionTypeCode,
					MarketValueFlags.IncludeChildAccounts | MarketValueFlags.IncludeOrder);

				// Find the location of the cell on the current document.  If the cell doesn't exist, then the entire document will
				// need to be redrawn.  If the cell is currently part of the displayed document, then an individual update will 
				// cause a less disturbing update to the document.
				AddressMapSet.PositionMapRow positionMapRow =
					this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(positionRow.SecurityId,
					positionRow.PositionTypeCode, ColumnType.OrderedQuantity);
				if (positionMapRow == null)
					this.IsRefreshNeeded = true;
				else
					displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell,
						new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
						quantity));

			}

		}

		/// <summary>
		/// Populates a batch of commands that will update the display document with the changed Proposed Orders.
		/// </summary>
		/// <param name="displayBatch">A batch of dispay commands.</param>
		private void UpdateProposedOrders(ArrayList displayBatch)
		{

			// Create the update conditions for each of the changed proposed orders.
			foreach (PositionRow positionRow in this.changedProposedOrders)
			{

				// Find the parent account row of this appraisal report.  All market value caluclations will be in terms of the
				// displayed appraisal, even though it may have been one of the child accounts that was modified.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(positionRow.AccountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", positionRow.AccountId));
				
				// Look up the parent security row.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(positionRow.SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} has been deleted", positionRow.SecurityId));
				
				// Calculate the proposed order quantity.
				decimal quantity = Quantity.Calculate(accountRow, securityRow, positionRow.PositionTypeCode,
					MarketValueFlags.IncludeChildAccounts | MarketValueFlags.IncludeProposedOrder);

				// Find the location of the cell on the current document.  If the cell doesn't exist, then the entire document will
				// need to be redrawn.  If the cell is currently part of the displayed document, then an individual update will 
				// cause a less disturbing update to the document.
				AddressMapSet.PositionMapRow positionMapRow =
					this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(positionRow.SecurityId,
					positionRow.PositionTypeCode, ColumnType.ProposedQuantity);
				if (positionMapRow == null)
					this.IsRefreshNeeded = true;
				else
					displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell,
						new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
						quantity));

			}

		}

		/// <summary>
		/// Populates a batch of commands that will update the display document with the changed Proposed TaxLots.
		/// </summary>
		/// <param name="displayBatch">A batch of dispay commands.</param>
		private void UpdateTaxLots(ArrayList displayBatch)
		{

			// Create the update conditions for each of the changed proposed taxLots.
			foreach (PositionRow positionRow in this.changedTaxLots)
			{

				// Find the parent account row of this appraisal report.  All market value caluclations will be in terms of the
				// displayed appraisal, even though it may have been one of the child accounts that was modified.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(positionRow.AccountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", positionRow.AccountId));
				
				// Look up the parent security row.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(positionRow.SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} has been deleted", positionRow.SecurityId));
				
				// Calculate the proposed taxLot quantity.
				decimal quantity = Quantity.Calculate(accountRow, securityRow, positionRow.PositionTypeCode,
					MarketValueFlags.IncludeChildAccounts | MarketValueFlags.IncludeTaxLot);

				// Find the location of the cell on the current document.  If the cell doesn't exist, then the entire document will
				// need to be redrawn.  If the cell is currently part of the displayed document, then an individual update will 
				// cause a less disturbing update to the document.
				AddressMapSet.PositionMapRow positionMapRow =
					this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(positionRow.SecurityId,
					positionRow.PositionTypeCode, ColumnType.PositionQuantity);
				if (positionMapRow == null)
					this.IsRefreshNeeded = true;
				else
					displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell,
						new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
						quantity));

			}

		}

		/// <summary>
		/// Populates a batch of commands that will update the display document with the changed Proposed Violations.
		/// </summary>
		/// <param name="displayBatch">A batch of dispay commands.</param>
		private void UpdateViolations(ArrayList displayBatch)
		{

			// Create the update conditions for each of the changed proposed violations.
			foreach (PositionRow positionRow in this.changedViolations)
			{

				int violationCount = 0;
				foreach (int accountId in this.ChildAccountIds)
					foreach (DataRowView dataRowView in
						ClientMarketData.Violation.UKViolationAccountIdSecurityIdPositionTypeCode.FindRows(
						new object[] {accountId, positionRow.SecurityId, positionRow.PositionTypeCode}))
					{
						ClientMarketData.ViolationRow violationRow = (ClientMarketData.ViolationRow)dataRowView.Row;
						if (violationRow.RestrictionRow.Severity > 0)
							violationCount++;
					}
					
				// Find the location of the cell on the current document.  If the cell doesn't exist, then the entire document will
				// need to be redrawn.  If the cell is currently part of the displayed document, then an individual update will 
				// cause a less disturbing update to the document.
				AddressMapSet.PositionMapRow positionMapRow =
					this.addressMap.PositionMap.FindBySecurityIdPositionTypeCodeColumnId(positionRow.SecurityId,
					positionRow.PositionTypeCode, ColumnType.SecurityName);

				if (positionMapRow == null)
					this.IsRefreshNeeded = true;
				else
				{
					if (violationCount == 0)
						displayBatch.Add(new DisplayCommand(DisplayCommand.SetBorder,
							new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
							XlBordersIndex.xlEdgeBottom, XlLineStyle.xlLineStyleNone, null, null, null));
					else
						displayBatch.Add(new DisplayCommand(DisplayCommand.SetBorder,
							new CellAddress(positionMapRow.DocumentVersion, positionMapRow.RowIndex, positionMapRow.ColumnIndex),
							XlBordersIndex.xlEdgeBottom, XlLineStyle.xlDot, XlBorderWeight.xlMedium, null, Color.Red));
				}

			}

		}

		/// <summary>
		/// Handles the account data being changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="positionTargetRowChangedEvent">Event argument.</param>
		private void AccountRowChangeEvent(object sender, ClientMarketData.AccountRowChangeEvent accountRowChangeEvent)
		{

			// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
			ClientMarketData.AccountRow accountRow = accountRowChangeEvent.Row;

			// If the account is the same as the one being displayed, but the model has changed, open the document again with the
			// new model.
			if (this.IsDocumentOpen && accountRow.AccountId == this.AccountId && accountRow.ModelId != this.ModelId)
				CommandQueue.Execute(new ThreadHandler(OpenDocumentCommand), accountRow.AccountId);

		}
		
		/// <summary>
		/// Event handler for a change to the proposed allocations table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		public void AllocationRowChangeEvent(object sender, ClientMarketData.AllocationRowChangeEvent allocationRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{
			
				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
				ClientMarketData.AllocationRow allocationRow = allocationRowChangeEvent.Row;

				// HACK - We need some logic here to filter out accounts that are not part of this appraisal.  Something like
				// an 'IsAccountChild'.  Otherwise, this function will get bogged down handling updates intended for other
				// appraisals.

				// The parent account for this document is used to aggregate the allocations and display them in the updated
				// cell.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", this.AccountId));

				// The parent account for this document is used to aggregate the allocations and display them in the updated
				// cell.
				ClientMarketData.SecurityRow securityRow =
					ClientMarketData.Security.FindBySecurityId(allocationRow.SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} has been deleted", allocationRow.SecurityId));

				// Calculate the proposed allocation quantity.
				decimal quantity = Quantity.Calculate(accountRow, securityRow, allocationRow.PositionTypeCode,
					MarketValueFlags.IncludeChildAccounts | MarketValueFlags.IncludeAllocation);

				// Invoke the foreground to update the cell containing the aggregate position.  If the cell isn't found in the
				// address map used by the foreground delegate, then we know it's time to redraw the appraisal completely.
				Position position = new Position(this.AccountId, allocationRow.SecurityId, allocationRow.PositionTypeCode);
				this.IsRefreshNeeded = !(bool)Invoke(this.setPositionDelegate,
					new object[] {position, ColumnType.AllocatedQuantity, quantity});

			}

		}
		
		/// <summary>
		/// Event driver for a change to the debt table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="debtRowChangeEvent">The arguments for the event handler.</param>
		private void DebtRowChangeEvent(object sender, ClientMarketData.DebtRowChangeEvent debtRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the currency table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="currencyRowChangeEvent">The arguments for the event handler.</param>
		private void CurrencyRowChangeEvent(object sender, ClientMarketData.CurrencyRowChangeEvent currencyRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the equity table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="equityRowChangeEvent">The arguments for the event handler.</param>
		private void EquityRowChangeEvent(object sender, ClientMarketData.EquityRowChangeEvent equityRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Updates the Appraisal when a security level model target has changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="positionTargetRowChangedEvent">Event argument.</param>
		private void ModelRowDeleteEvent(object sender, ClientMarketData.ModelRowChangeEvent modelRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
				ClientMarketData.ModelRow modelRow = modelRowChangeEvent.Row;

				// If the model that was updated is the current model used by this appraisal, then an attempt is made to update the
				// value.  If a cell already exists in the current report for the value, then the update is made surgically by only
				// updating that cell.  If no cell can be found, a request to redraw the entire document is genrated.
				if (this.ModelId == (int)modelRow[ClientMarketData.Model.ModelIdColumn, DataRowVersion.Original])
					this.IsRefreshNeeded = true;

			}

		}
		
		/// <summary>
		/// Updates the Appraisal when a security level model target has changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="positionTargetRowChangedEvent">Event argument.</param>
		private void ModelRowChangeEvent(object sender, ClientMarketData.ModelRowChangeEvent modelRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
				ClientMarketData.ModelRow modelRow = modelRowChangeEvent.Row;

				// If the model that was updated is the current model used by this appraisal, then an attempt is made to update the
				// value.  If a cell already exists in the current report for the value, then the update is made surgically by only
				// updating that cell.  If no cell can be found, a request to redraw the entire document is genrated.
				int modelId = (int)modelRow[ClientMarketData.Model.ModelIdColumn, modelRow.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
				if (this.ModelId == modelId)
					this.IsRefreshNeeded = true;

			}

		}
		
		/// <summary>
		/// Event driver for a change to the objectTree table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="objectTreeRowChangeEvent">The arguments for the event handler.</param>
		private void ObjectTreeRowChangeEvent(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectTreeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Updates the Appraisal when a security level model target has changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="positionTargetRowChangedEvent">Event argument.</param>
		private void PositionTargetRowChangeEvent(object sender, ClientMarketData.PositionTargetRowChangeEvent positionTargetRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
				ClientMarketData.PositionTargetRow positionTargetRow = positionTargetRowChangeEvent.Row;

				// If the model that was updated is the current model used by this appraisal, then an attempt is made to update the
				// value.  If a cell already exists in the current report for the value, then the update is made surgically by only
				// updating that cell.  If no cell can be found, a request to redraw the entire document is genrated.
				if (this.ModelId == positionTargetRow.ModelId)
				{
					
					// Deleted records are cleared, the rest are filled in with the new target percentage.
					object target = null;
					if (positionTargetRowChangeEvent.Action != DataRowAction.Delete)
						target = positionTargetRow.Percent;

					Position position = new Position(this.AccountId, positionTargetRow.SecurityId, positionTargetRow.PositionTypeCode);
					this.IsRefreshNeeded = !(bool)Invoke(this.setPositionDelegate,
						new object[] {position, ColumnType.ModelPercent, target});

				}

			}

		}
		
		/// <summary>
		/// Updates the Appraisal when a security level model target has changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="restrictionRowChangedEvent">Event argument.</param>
		private void RestrictionRowChangeEvent(object sender, ClientMarketData.RestrictionRowChangeEvent restrictionRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				this.IsRefreshNeeded = true;

			}

		}
		
		/// <summary>
		/// Event driver for a change to the scheme table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="schemeRowChangeEvent">The arguments for the event handler.</param>
		private void SchemeRowChangeEvent(object sender, ClientMarketData.SchemeRowChangeEvent schemeRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event driver for a change to the sector table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="sectorRowChangeEvent">The arguments for the event handler.</param>
		private void SectorRowChangeEvent(object sender, ClientMarketData.SectorRowChangeEvent sectorRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Updates the Appraisal when a security level model target has changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="positionTargetRowChangedEvent">Event argument.</param>
		private void SectorTargetRowChangeEvent(object sender, ClientMarketData.SectorTargetRowChangeEvent sectorTargetRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Cast the generic DataRow into a strongly typed row.  This makes it much easier to deal with the new record.
				ClientMarketData.SectorTargetRow sectorTargetRow = sectorTargetRowChangeEvent.Row;

				// If the model that was updated is the current model used by this appraisal, then an attempt is made to update the
				// value.  If a cell already exists in the current report for the value, then the update is made surgically by only
				// updating that cell.  If no cell can be found, a request to redraw the entire document is genrated.
				if (this.ModelId == sectorTargetRow.ModelId)
				{

					// Deleted records are cleared, the rest are filled in with the new target percentage.
					object target = null;
					if (sectorTargetRowChangeEvent.Action != DataRowAction.Delete)
						target = sectorTargetRow.Percent;

					// Invoke the foreground method to update the given sector.  The foreground will pass back a boolean that
					// indicates whether a mapping was found for the value.
					this.IsRefreshNeeded = !(bool)Invoke(this.setSectorDelegate,
						new object[] {sectorTargetRow.SectorId, ColumnType.ModelPercent, target});

				}

			}

		}
		
		/// <summary>
		/// Event driver for a change to the security table.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="securityRowChangeEvent">The arguments for the event handler.</param>
		private void SecurityRowChangeEvent(object sender, ClientMarketData.SecurityRowChangeEvent securityRowChangeEvent)
		{

			// This will force a refresh.
			this.IsRefreshNeeded = true;

		}
	
		/// <summary>
		/// Event handler for a change to the orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void OrderRowDeleteEvent(object sender, ClientMarketData.OrderRowChangeEvent OrderRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Shorthand reference to the changed row.
				ClientMarketData.OrderRow OrderRow = OrderRowChangeEvent.Row;
				
				// Extract the key elements from the deleted record.  These values are used to determine what cells on the
				// appraisal will have to be updated.
				int accountId = (int)OrderRow[ClientMarketData.Order.AccountIdColumn, DataRowVersion.Original];
				int securityId = (int)OrderRow[ClientMarketData.Order.SecurityIdColumn, DataRowVersion.Original];
				int positionTypeCode = (int)OrderRow[ClientMarketData.Order.PositionTypeCodeColumn, DataRowVersion.Original];

				// Filter out any accounts that are not part of this document.
				if (!this.ContainsAccount(accountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many orders for a given position.  Updating each position for the
				// many orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedOrders.AddPosition(this.AccountId, securityId, positionTypeCode);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void OrderRowChangeEvent(object sender, ClientMarketData.OrderRowChangeEvent OrderRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded)
			{

				// Shorthand reference to the changed row.
				ClientMarketData.OrderRow OrderRow = OrderRowChangeEvent.Row;
				
				// Filter out all accounts that don't belong in this appraisal.
				if (!this.ContainsAccount(OrderRow.AccountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many orders for a given position.  Updating each position for the
				// many orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedOrders.AddPosition(this.AccountId, OrderRow.SecurityId, OrderRow.PositionTypeCode);

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
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ProposedOrderRowDeleteEvent(object sender, ClientMarketData.ProposedOrderRowChangeEvent proposedOrderRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && proposedOrderRowChangeEvent.Action == DataRowAction.Delete)
			{

				// Shorthand reference to the changed row.
				ClientMarketData.ProposedOrderRow proposedOrderRow = proposedOrderRowChangeEvent.Row;
				
				// Extract the key elements from the deleted record.  These values are used to determine what cells on the
				// appraisal will have to be updated.
				int accountId = (int)proposedOrderRow[ClientMarketData.ProposedOrder.AccountIdColumn, DataRowVersion.Original];
				int securityId = (int)proposedOrderRow[ClientMarketData.ProposedOrder.SecurityIdColumn, DataRowVersion.Original];
				int positionTypeCode = (int)proposedOrderRow[ClientMarketData.ProposedOrder.PositionTypeCodeColumn, DataRowVersion.Original];

				// Filter out any accounts that are not part of this document.
				if (!this.ContainsAccount(accountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedProposedOrders.AddPosition(this.AccountId, securityId, positionTypeCode);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ProposedOrderRowChangeEvent(object sender, ClientMarketData.ProposedOrderRowChangeEvent proposedOrderRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && (proposedOrderRowChangeEvent.Action == DataRowAction.Add ||
				proposedOrderRowChangeEvent.Action == DataRowAction.Change))
			{

				// Shorthand reference to the changed row.
				ClientMarketData.ProposedOrderRow proposedOrderRow = proposedOrderRowChangeEvent.Row;
				
				// Filter out all accounts that don't belong in this appraisal.
				if (!this.ContainsAccount(proposedOrderRow.AccountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedProposedOrders.AddPosition(this.AccountId, proposedOrderRow.SecurityId, proposedOrderRow.PositionTypeCode);

			}

		}
		
		/// <summary>
		/// The event handler for changes to the Stylesheets table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void StylesheetRowChangeEvent(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
		{

			// This will make it easier to operate on the changed record.
			ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

			// Reopen the document if the style sheet has changed.
			if (this.IsDocumentOpen && stylesheetRow.StylesheetId == this.StylesheetId)
			{
				this.IsStylesheetChanged = true;
				OpenDocument(this.AccountId);
			}

		}

		/// <summary>
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void TaxLotRowDeleteEvent(object sender, ClientMarketData.TaxLotRowChangeEvent taxLotRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && taxLotRowChangeEvent.Action == DataRowAction.Delete)
			{

				// Shorthand reference to the changed row.
				ClientMarketData.TaxLotRow taxLotRow = taxLotRowChangeEvent.Row;
				
				// Extract the key elements from the deleted record.  These values are used to determine what cells on the
				// appraisal will have to be updated.
				int accountId = (int)taxLotRow[ClientMarketData.TaxLot.AccountIdColumn, DataRowVersion.Original];
				int securityId = (int)taxLotRow[ClientMarketData.TaxLot.SecurityIdColumn, DataRowVersion.Original];
				int positionTypeCode = (int)taxLotRow[ClientMarketData.TaxLot.PositionTypeCodeColumn, DataRowVersion.Original];

				// Filter out any accounts that are not part of this document.
				if (!this.ContainsAccount(accountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedTaxLots.AddPosition(this.AccountId, securityId, positionTypeCode);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void TaxLotRowChangeEvent(object sender, ClientMarketData.TaxLotRowChangeEvent taxLotRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && (taxLotRowChangeEvent.Action == DataRowAction.Add ||
				taxLotRowChangeEvent.Action == DataRowAction.Change))
			{

				// Shorthand reference to the changed row.
				ClientMarketData.TaxLotRow taxLotRow = taxLotRowChangeEvent.Row;
				
				// Filter out all accounts that don't belong in this appraisal.
				if (!this.ContainsAccount(taxLotRow.AccountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedTaxLots.AddPosition(this.AccountId, taxLotRow.SecurityId, taxLotRow.PositionTypeCode);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ViolationRowDeleteEvent(object sender, ClientMarketData.ViolationRowChangeEvent violationRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && violationRowChangeEvent.Action == DataRowAction.Delete)
			{

				// Shorthand reference to the changed row.
				ClientMarketData.ViolationRow violationRow = violationRowChangeEvent.Row;
				
				// Extract the key elements from the deleted record.  These values are used to determine what cells on the
				// appraisal will have to be updated.
				int accountId = (int)violationRow[ClientMarketData.Violation.AccountIdColumn, DataRowVersion.Original];
				int securityId = (int)violationRow[ClientMarketData.Violation.SecurityIdColumn, DataRowVersion.Original];
				int positionTypeCode = (int)violationRow[ClientMarketData.Violation.PositionTypeCodeColumn, DataRowVersion.Original];

				// Filter out any accounts that are not part of this document.
				if (!this.ContainsAccount(accountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedViolations.AddPosition(this.AccountId, securityId, positionTypeCode);

			}

		}
		
		/// <summary>
		/// Event handler for a change to the proposed orders table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ViolationRowChangeEvent(object sender, ClientMarketData.ViolationRowChangeEvent violationRowChangeEvent)
		{

			// Don't attempt to modify individual cells if the entire document is to be refreshed.
			if (this.IsDocumentOpen && !this.IsRefreshNeeded && (violationRowChangeEvent.Action == DataRowAction.Add ||
				violationRowChangeEvent.Action == DataRowAction.Change))
			{

				// Shorthand reference to the changed row.
				ClientMarketData.ViolationRow violationRow = violationRowChangeEvent.Row;
				
				// Filter out all accounts that don't belong in this appraisal.
				if (!this.ContainsAccount(violationRow.AccountId))
					return;

				// This will add the position to a list of distinct positions that will be processed together at the end of the 
				// merge. An group appraisal may have many proposed orders for a given position.  Updating each position for the
				// many proposed orders is time-consuming and redudant.  The 'AddPosition' operation will throw away duplicate
				// positions.  A list of distinct positions makes the update much faster, especially during rebalancing.
				this.changedViolations.AddPosition(this.AccountId, violationRow.SecurityId, violationRow.PositionTypeCode);

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

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// The main idea of this method is to take the results of the editing and spawn a thread that will handle the new
				// data in the background.  There are some tasks that have to be done in the foreground (Message Loop Thread), such
				// as operating on the spreadsheet control, and there are other tasks that have to be done in the background
				// (Locking tables and examining the data model).  Here, we do what can be done in the foreground, and construct a
				// command for the background.
				ThreadHandler commandHandler = null;
				object key = null;

				// Extract the current row and column from the event argument.  This is the address of the cell where the editing
				// took place. Note that, because the user can move the focus at any time to another cell, this is not necessarily
				// the address of the currently active cell.
				int rowIndex = e.CellAddress.RowIndex;
				int columnIndex = e.CellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// Handler for Position Rows
				if (rowType == RowType.Position)
				{

					// Get the security id from the selected row.
					int securityIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SecurityId);
					int securityId = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						securityIdColumnIndex)));

					// Get the position type code from the selected row.
					int positionTypeCodeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.PositionTypeCode);
					int positionTypeCode = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						positionTypeCodeColumnIndex)));

					// Create a key from the security and position type codes.  This position record will uniquely identify a
					// position row on the appraisal report.
					key = new Position(this.AccountId, securityId, positionTypeCode);

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.ModelPercent:
							
							// Update the Position Target Percentage
							commandHandler = new ThreadHandler(SetPositionTarget);
							break;

						case ColumnType.ProposedQuantity:

							// Update the Proposed Quantity
							commandHandler = new ThreadHandler(SetProposedQuantityCommand);
							break;

					}

				}

				// Handler for Sector Rows
				if (rowType == RowType.Sector)
				{

					// Get the sector id from the selected row.
					int sectorIdColumnIndex = this.addressMap.GetColumnIndex(ColumnType.SectorId);
					key = Convert.ToInt32(this.GetCell(new CellAddress(this.DocumentVersion, rowIndex,
						sectorIdColumnIndex)));

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.ModelPercent:

							// Set the model percentage for the sector.
							ServiceQueue.Execute(new ThreadHandler(SetSectorTarget), key, e.Result);
							break;

					}

				}

				// If a command was decoded from the column information above, then execute the command in the background giving
				// the primary key and the data to a thread that will execute outside the message loop thread.
				if (commandHandler != null)
					CommandQueue.Execute(commandHandler, key, e.Result);

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
		/// Deletes the value under the cell.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event parameters.</param>
		protected override void OnCommandClear(System.EventArgs e)
		{

			try
			{

				// Lock the Address Map
				this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Extract the current row and column indices from the current selection.
				CellAddress activeCellAddress = GetActiveCellAddress();
				int rowIndex = activeCellAddress.RowIndex;
				int columnIndex = activeCellAddress.ColumnIndex;

				// Get the row type from the selected row.
				int rowTypeColumnIndex = this.addressMap.GetColumnIndex(ColumnType.RowType);
				int rowType = Convert.ToInt32(GetCell(new CellAddress(this.DocumentVersion, rowIndex, rowTypeColumnIndex)));

				// Handler for Position Rows
				if (rowType == RowType.Position)
				{

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.ModelPercent:
							
							// Clear the Position Target Percentage
							ServiceQueue.Execute(new ThreadHandler(this.ClearPositionTarget), GetSelectedPositions());
							break;

						case ColumnType.ProposedQuantity:

							// Clear the Proposed Order
							ServiceQueue.Execute(new ThreadHandler(ProposedOrder.ClearPosition), GetSelectedPositions());
							break;

					}

				}

				// Handler for Sector Rows
				if (rowType == RowType.Sector)
				{

					// Switch to a section that will parse and handle each of the column functions.
					switch (this.addressMap.GetColumnType(columnIndex))
					{

						case ColumnType.ModelPercent:

							// Clear the model percentage for the sector.
							ServiceQueue.Execute(new ThreadHandler(ClearSectorTarget), GetSelectedSectors());
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
		/// This thread will update the prices in the appraisal document.
		/// </summary>
		private void PricerThread()
		{

			// The 'tick' table is a hash table indexed by the security id that contains the most recent tick off the price
			// stream.  The prices are kept in this table long enough to animate them on the document.  When they've reached a
			// certain age, they will be removed from table and won't be animated any longer.
			while (true)
			{

				// Wait here for the next tick to come in.
				this.ticker.WaitForTick();

				// The animation of the tickers is state driven.  Before an up or down direction can be determined, the previous 
				// price needs to be known.  The very first tick doesn't know what the previous price was, so it can't be colored
				// or animated. These first-time ticks are handled differently than the animated ticks; they are sent directly to
				// the display without animation effects.
				ArrayList displayBatch = new ArrayList();

				// This loop takes care of updating the tick table with the new prices from the queue.
				while (this.ticker.Count != 0)
				{

					// Take the next price record off the queue.
					Tick newTick = this.ticker.Dequeue();

					try
					{

						// Lock the Address Map
						this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);

						// Lock the Tick Table
						this.tickTableLock.AcquireWriterLock(CommonTimeout.LockWait);
					
						// The 'tickTable' contains the last known state of the price changes coming from the ticker plant.  This
						// list is built dynamically: if the tick record doesn't exist yet, add it to the table.  Otherwise, we're
						// going to see if any of the prices have changed from the last state.
						Tick oldTick = this.tickTable[newTick.securityId];
						if (oldTick == null)
						{

							// Save this tick in the cache of ticks recognized by this document.  This will be used to determine 
							// whether the next tick is a plus or minus tick.
							this.tickTable[newTick.securityId] = newTick;

							// Change the price on every security to the last price and color it black.
							AddressMapSet.SecurityMapRow securityMapRow = this.addressMap.SecurityMap.FindBySecurityId(newTick.securityId);
							if (securityMapRow != null)
								foreach (AddressMapSet.LastPriceAddressRow lastPriceAddressRow in securityMapRow.GetLastPriceAddressRows())
								{
									CellAddress cellAddress = new CellAddress(lastPriceAddressRow.DocumentVersion,
										lastPriceAddressRow.RowIndex, lastPriceAddressRow.ColumnIndex);
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetFormat, cellAddress, Color.Black));
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, newTick.lastPrice));
								}

						}
						else
						{

							// See if the new 'last executed price' is different from the last known price.  If it is we'll animate
							// the price change.
							if (oldTick.lastPrice != newTick.lastPrice)
							{

								// See if the security is already being animated.  If it isn't in the animation cache, then we'll
								// install it.  Otherwise, we'll reset the direction and age of the tick.
								Tick animatedTick = this.animatedTickTable[newTick.securityId];
								if (animatedTick == null)
									this.animatedTickTable[newTick.securityId] = animatedTick = newTick;
	
								// Reset the animation sequence with the latest 'last executed price', the direction 
								// (based on the change from the existing tick to the new tick) and the age, which will
								// cause the displayed price to change color shades until it's finally faded to the
								// standard text color.
								animatedTick.lastPrice = newTick.lastPrice;
								animatedTick.lastPriceDirection = oldTick.lastPrice < newTick.lastPrice ?
									TickDirection.Up : TickDirection.Down;
								animatedTick.lastPriceAge = 0;

								// The price stored in the ticker state table is updated at this point to the latest executed 
								// price.
								oldTick.lastPrice = newTick.lastPrice;

							}

						}

					}
					finally
					{

						// Release the address map
						if (this.addressMapLock.IsReaderLockHeld) this.addressMapLock.ReleaseReaderLock();

						// Release the tables shared by the threads.
						if (this.tickTableLock.IsWriterLockHeld) this.tickTableLock.ReleaseWriterLock();

					}

					// Send the batch of commands to the foreground for processing.
					if (displayBatch.Count != 0)
						ExecuteDisplayBatch(displayBatch);

					// This can be a CPU intensive task, so give the other threads a chance to run while the ticks are processed.
					Thread.Sleep(0);

				}

			}

		}

		/// <summary>
		/// Thread to animate the up and down ticks.
		/// </summary>
		private void PriceAgerThread()
		{

			// This loop will animate the prices in the TickTable.  As time goes by, the the ticks will age.  They will change in
			// hue from a bright color to black.  This loop takes care of aging the ticks in the table and eventually removing them
			// when they expire.
			while (true)
			{

				// Don't update the screen if the real-time prices have been turned off.
				this.priceUpdateSignal.WaitOne();
				
				// Make sure table locks are released.
				try
				{

					// The document also conains the lookup map to find cells on the spreadsheet.  The TickTable will be updated
					// with the new age of the tick and expunged of old ticks.
					this.addressMapLock.AcquireReaderLock(CommonTimeout.LockWait);
					this.tickTableLock.AcquireWriterLock(CommonTimeout.LockWait);

					// IMPORTANT CONCEPT: There is considerable overhead in calling the foreground message loop to have it process
					// results.  The best way around this is to collect a set of operations and send them as a batch to be executed
					// when the message loop gets around to it.  The animation may have many tickers updated every second.  This
					// architecture is the most effective way of using the foreground 'Invoke' method for passing data from a
					// background thread to the foreground user interface objects.
					ArrayList displayBatch = new ArrayList();
					
					// This list is used to delete elements from the tickTable when they've ended their lifetime.  Since we can't
					// delete an element from the hash table while we're iterating through it, we'll copy the key value into this
					// list.  After the hash table has been searched, we'll delete the elements of the tickTable using this list.
					ArrayList deleteList = new ArrayList();

					// Cycle through each element of the animatedTickTable and animate the price value with a color that is based
					// on the age of the tick.
					foreach (DictionaryEntry dictionaryEntry in this.animatedTickTable)
					{

						// Get the next tick we're to animate.
						Tick tick = (Tick)dictionaryEntry.Value;

						// This will find each cell in the spreadsheet that needs to be updated.  Note that a tick may still be in
						// the animation table, but been moved or removed from the document. Also note that we could have both a
						// long and short position that both need to be updated with the same price. That's why we have a loop
						// after finding the security in the SecurityMap table.
						AddressMapSet.SecurityMapRow securityMapRow = this.addressMap.SecurityMap.FindBySecurityId(tick.securityId);
						if (securityMapRow != null)
						{

							// If the price has changed, then run through all the addresses where the given security can be found
							// and animate the price found in that cell.
							if (tick.lastPriceDirection != TickDirection.Unchanged)
								foreach (AddressMapSet.LastPriceAddressRow lastPriceAddressRow in securityMapRow.GetLastPriceAddressRows())
								{

									// The intensity of the color is related to the age.  The color is based on whether we have an
									// up tick or a down tick.  Note that at the end of it's lifetime, it will be black.  Once
									// we've calculated the color, call the delegate to update the price.
									int intensity = (tick.lastPriceAge < Tick.AgeLimit) ? 0xFF - tick.lastPriceAge * 0x16 : 0x00;
									Color color = Color.FromArgb((tick.lastPriceDirection == TickDirection.Down) ? intensity : 0x00,
										(tick.lastPriceDirection == TickDirection.Up) ? intensity : 0x00, 0x00);

									// This is the cell that will be animated with color hues for the age of the tick.
									CellAddress cellAddress = new CellAddress(lastPriceAddressRow.DocumentVersion, lastPriceAddressRow.RowIndex,
										lastPriceAddressRow.ColumnIndex);

									// The color of the cell will change with every click of this thread.
									displayBatch.Add(new DisplayCommand(DisplayCommand.SetFormat, cellAddress, color));

									// If the tick is brand new, then update the value of the cell after setting the initial color.
									if (tick.lastPriceAge == 0)
										displayBatch.Add(new DisplayCommand(DisplayCommand.SetCell, cellAddress, tick.lastPrice));

								}

						}
						
						// Age all of the ticks.  When it's reached it's limit, we'll set the state of the tick to 'Unchanged' to
						// prevent it from being updated again until a new price arrives.
						if (++tick.lastPriceAge > Tick.AgeLimit) tick.lastPriceDirection = TickDirection.Unchanged;

						// At the end of the lifetime of the tick, we add it to a list of items that will be deleted. Note that we
						// can't delete the tick here because it's part of the iteration.
						if (tick.lastPriceAge > Tick.AgeLimit)
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

				// Sleep a second between animation shades.  This can be made programmable someday if the user wants control over
				// how long the element is highlighted.
				Thread.Sleep(1000);

			}

		}
		
		/// <summary>
		/// Handles the user request to save the document.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemSaveAppraisal_Click(object sender, System.EventArgs e)
		{

			// Pass the operation on to the base class.
			this.SaveAs();

		}

		/// <summary>
		/// Handles the user's request for the closing prices.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
		
			// Change the pricing method.  This is available to any method that needs to do a calculation.
			ClientPreferences.Pricing = Pricing.Close;

//			// Update the menu items to reflect the state of the user's pricing peference.
//			this.menuItemLast.Checked = false;
//			this.menuItemClose.Checked = true;

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);

				// We don't want to be notified of price changes if we're just using the closing price.
				ClientMarketData.Price.PriceRowChanged -= new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();

			}
			
			// Disable the price update thread.  This will keep elements that are queued up from modifying the prices after the
			// user has changed his pricing option.
			this.priceUpdateSignal.Reset();

			// Redraw the document.
			DrawDocument();

		}

		/// <summary>
		/// Handles the user's request for the real-time prices.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemLast_Click(object sender, System.EventArgs e)
		{
		
//			// Update the menu items to reflect the state of the user's pricing peference.
//			this.menuItemLast.Checked = true;
//			this.menuItemClose.Checked = false;
//			
			// Change the pricing method.  This is available to any method that needs to do a calculation.
			ClientPreferences.Pricing = Pricing.Last;

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// We do want to be notified of price changes if we're using the latest price.
				ClientMarketData.Price.PriceRowChanged += new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();

			}
			
			// Enable the price aging thread to update the screen.
			this.priceUpdateSignal.Set();

			// Redraw the document.
			DrawDocument();

		}

		private void menuItemFile_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuItemCheckCompliance_Click(object sender, System.EventArgs e)
		{

			try
			{
			
				Assembly assembly = Assembly.Load("Library.Rules");
				foreach (Type type in assembly.GetTypes())
					assembly.CreateInstance(type.FullName);

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

		}

		public Pricing Pricing
		{
			get {return this.pricing;}

			set
			{

				switch (value)
				{

				case Pricing.Close:

					try
					{

						// Lock the tables
						Debug.Assert(!ClientMarketData.AreLocksHeld);
						ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);

						// We don't want to be notified of price changes if we're just using the closing price.
						ClientMarketData.Price.PriceRowChanged -= new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);

					}
					catch (Exception exception)
					{

						// Write the error and stack trace out to the debug listener
						Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

					}
					finally
					{

						// Release the table locks.
						if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();

					}
			
					// Disable the price update thread.  This will keep elements that are queued up from modifying the prices after the
					// user has changed his pricing option.
					this.priceUpdateSignal.Reset();

					break;

				case Pricing.Last:

					try
					{

						// Lock the tables
						Debug.Assert(!ClientMarketData.AreLocksHeld);
						ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
			
						// We do want to be notified of price changes if we're using the latest price.
						ClientMarketData.Price.PriceRowChanged += new ClientMarketData.PriceRowChangeEventHandler(this.PriceRowChangeEvent);

					}
					catch (Exception exception)
					{

						// Write the error and stack trace out to the debug listener
						Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

					}
					finally
					{

						// Release the table locks.
						if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();

					}
			
					// Enable the price aging thread to update the screen.
					this.priceUpdateSignal.Set();

					break;

				}

				// Redraw the document if it's open.
				if (this.IsDocumentOpen)
					DrawDocument();

			}

		}

	}

}
