/*************************************************************************************************************************
*
*	File:			TaxLot.cs
*	Description:	A class for managing TaxLot records.  Tax lots are records of holdings that share the same security,
*					cost and trade date.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;

	/// <summary>
	/// Used to pass proposed order events to listers.
	/// </summary>
	public delegate void TaxLotEvent(object sender, TaxLotEventArgs taxLotEventArgs);
	
	/// <summary>Event argument for a proposed order event.</summary>
	public class TaxLotEventArgs : EventArgs
	{

		private Action action;
		private TaxLot taxLot;

		/// <summary>
		/// The action taken on the proposed order (add, delete, modify)
		/// </summary>
		public Action Action {get {return this.action;}}

		/// <summary>
		/// The proposed order record.
		/// </summary>
		public TaxLot TaxLot {get {return this.taxLot;}}
		
		/// <summary>
		/// Constructs and argument for passing a proposed order event to a listener.
		/// </summary>
		/// <param name="action">The action taken on the proposed order.</param>
		/// <param name="taxLot">The proposed order record.</param>
		public TaxLotEventArgs(Action action, TaxLot taxLot)
		{

			// Initialize the record.
			this.action = action;
			this.taxLot = taxLot;

		}

	}
	
	/// <summary>
	/// A list of TaxLotEventArgs.
	/// </summary>
	public class TaxLotEventArgList : ArrayList
	{

		/// <summary>
		/// Insures that the generic version of this method isn't used.
		/// </summary>
		/// <param name="value">The value to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public override int Add(object value)
		{

			// Insure that generic items aren't added to the list.
			throw new Exception("Illegal value in TaxLotEventArgList");

		}

		/// <summary>
		/// Adds a TaxLotEventArgs to the list.
		/// </summary>
		/// <param name="taxLotEventArgs">The event arguments to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public int Add(TaxLotEventArgs taxLotEventArgs)
		{

			// Add the event arguments to the list.
			return base.Add(taxLotEventArgs);

		}

	}
	
	public class TaxLot
	{

		// This list is used to handle pass event arguments outside the normal table locking scheme for events.
		private static TaxLotEventArgList taxLotEventArgList;

		/// <summary>This event is triggered when a proposed order is added, modified or deleted.</summary>
		public static TaxLotEvent Changed;

		/// <summary>The unique identifier of this tax lot.</summary>
		private int taxLotId;
		/// <summary>The unique identifier of a position.</summary>
		private Position position;
		/// <summary>The number of units being traded.</summary>
		private decimal quantity;
		/// <summary>The stop limit price for this order.</summary>
		private decimal cost;

		/// <summary>Returns the elements of a position (account, security, long or short).</summary>
		public Position Position {get {return position;}}
		/// <summary>Gets the quantity of this order.</summary>
		public decimal Quantity {get {return this.quantity;}}
		/// <summary>Gets the cost of the tax lot.</summary>
		public decimal Cost {get {return this.cost;}}

		/// <summary>
		/// Initializer for all proposed orders.
		/// </summary>
		static TaxLot()
		{

			// IMPORTANT CONCEPT: To simplify the programming model, the event handlers are not called directly.  The issue is that
			// all the tables are locked during the handling of the primary event handlers off the Market Data model.  The language
			// primitives also assume the responsibility of locking tables.  Remember that nested locking has been prohibited to
			// insure that deadlocks won't happen.  So, we're left with a problem: how can the financial language primitives be
			// called from inside the event handlers.  The solution: they can't, so we set up a system where the events are
			// rebroadcast after the merge.  This list will collect the events from the primary data model.  Later, during the
			// handing of the 'EndMerge' event, they will be rebroadcast (when the tables are no longer locked).
			TaxLot.taxLotEventArgList = new TaxLotEventArgList();
			
			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will chain the data model event handler into the Rules Language handler.
				ClientMarketData.TaxLot.TaxLotRowChanged += new ClientMarketData.TaxLotRowChangeEventHandler(TaxLotHandler);
				ClientMarketData.TaxLot.TaxLotRowDeleted += new ClientMarketData.TaxLotRowChangeEventHandler(TaxLotHandler);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Create a TaxLot record.
		/// </summary>
		/// <param name="taxLotId"></param>
		/// <param name="accountId"></param>
		/// <param name="securityId"></param>
		/// <param name="positionTypeCode"></param>
		/// <param name="quantity"></param>
		/// <param name="cost"></param>
		public TaxLot(int taxLotId, Position position, decimal quantity, decimal cost)
		{

			// Initialize the object
			this.taxLotId = taxLotId;
			this.position = position;
			this.quantity = quantity;
			this.cost = cost;

		}

		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="taxLotRow">A proposed order record from the primary ADO database.</param>
		/// <returns>A TaxLot record based on the ADO record.</returns>
		internal static TaxLot Make(ClientMarketData.TaxLotRow taxLotRow)
		{

			// Initialize the object
			DataRowVersion dataRowVersion = taxLotRow.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;

			// Extract the data from the ADO record.
			int taxLotId = (int)taxLotRow[ClientMarketData.TaxLot.TaxLotIdColumn, dataRowVersion];
			int accountId = (int)taxLotRow[ClientMarketData.TaxLot.AccountIdColumn, dataRowVersion];
			int securityId = (int)taxLotRow[ClientMarketData.TaxLot.SecurityIdColumn, dataRowVersion];
			int positionTypeCode = (int)taxLotRow[ClientMarketData.TaxLot.PositionTypeCodeColumn, dataRowVersion];
			Position position = Position.Make(accountId, securityId, positionTypeCode);
			decimal quantity = (decimal)taxLotRow[ClientMarketData.TaxLot.QuantityColumn, dataRowVersion];
			decimal cost = (decimal)taxLotRow[ClientMarketData.TaxLot.CostColumn, dataRowVersion];

			// Create a new record based on the data extracted from the ADO database.
			return new TaxLot(taxLotId, position, quantity, cost);

		}
		
		/// <summary>
		/// Handles the start of a Database Merge.
		/// </summary>
		public static void OnBeginMerge()
		{

			// This list is used to queue up the proposed orders during the primary database event handlers.  They will be passed
			// on to the langauge event handlers when the primary database during the handling of the 'EndMerge' event, when the
			// tables are no longer locked.
			TaxLot.taxLotEventArgList.Clear();

		}
		
		/// <summary>
		/// Catches the Data Model events and rebroadcasts the event to the Rules Language Handlers.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="taxLotRowChangeEvent">The record change event argument.</param>
		public static void TaxLotHandler(object sender, ClientMarketData.TaxLotRowChangeEvent taxLotRowChangeEvent)
		{

			// Extract the record from the event argument.
			ClientMarketData.TaxLotRow taxLotRow = taxLotRowChangeEvent.Row;

			// Translate the ADO.NET row states into a record state used by the Rules Engine.
			Action action = Action.Nothing;
			switch (taxLotRowChangeEvent.Action)
			{
				case DataRowAction.Add: action = Action.Add; break;
				case DataRowAction.Delete: action = Action.Delete; break;
				case DataRowAction.Change: action = Action.Change; break;
			}

			// Place the event into a list that will be processed when the tables are no longer locked.
			TaxLot.taxLotEventArgList.Add(new TaxLotEventArgs(action, TaxLot.Make(taxLotRow)));

		}

		/// <summary>
		/// Handles the end of a Database Merge.
		/// </summary>
		public static void OnEndMerge()
		{

			// Call the virtual method to broadcast the events to the Rules Engine.
			foreach (TaxLotEventArgs taxLotEventArgs in TaxLot.taxLotEventArgList)
				OnTaxLotChanged(taxLotEventArgs);

		}
		
		/// <summary>
		/// Broadcasts a change of record event.
		/// </summary>
		/// <param name="taxLotsEventArgs">The event argument.</param>
		public static void OnTaxLotChanged(TaxLotEventArgs taxLotsEventArgs)
		{

			// Broadcast the event to any listeners.
			if (TaxLot.Changed != null)
				TaxLot.Changed(typeof(TaxLot), taxLotsEventArgs);

		}
	
	}

}
