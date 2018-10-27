/*************************************************************************************************************************
*
*	File:			Allocation.cs
*	Description:	A class for managing proposed order records.  The data model maintains a work area where 'what if'
*					scenarios can be played out without creating real orders for the blotter.
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
	using System.Threading;

	/// <summary>
	/// Used to pass proposed order events to listers.
	/// </summary>
	public delegate void AllocationEvent(object sender, AllocationEventArgs allocationEventArgs);
	
	/// <summary>
	/// Event argument for a proposed order event.
	/// </summary>
	public class AllocationEventArgs : EventArgs
	{

		private Action action;
		private Allocation allocation;

		/// <summary>
		/// The action taken on the proposed order (add, delete, modify)
		/// </summary>
		public Action Action {get {return this.action;}}

		/// <summary>
		/// The proposed order record.
		/// </summary>
		public Allocation Allocation {get {return this.allocation;}}
		
		/// <summary>
		/// Constructs and argument for passing a proposed order event to a listener.
		/// </summary>
		/// <param name="action">The action taken on the proposed order.</param>
		/// <param name="allocation">The proposed order record.</param>
		public AllocationEventArgs(Action action, Allocation allocation)
		{

			// Initialize the record.
			this.action = action;
			this.allocation = allocation;

		}

	}

	/// <summary>
	/// A list of AllocationEventArgs.
	/// </summary>
	public class AllocationEventArgList : ArrayList
	{

		/// <summary>
		/// Insures that the generic version of this method isn't used.
		/// </summary>
		/// <param name="value">The value to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public override int Add(object value)
		{

			// Insure that generic items aren't added to the list.
			throw new Exception("Illegal value in AllocationEventArgList");

		}

		/// <summary>
		/// Adds a AllocationEventArgs to the list.
		/// </summary>
		/// <param name="allocationEventArgs">The event arguments to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public int Add(AllocationEventArgs allocationEventArgs)
		{

			// Add the event arguments to the list.
			return base.Add(allocationEventArgs);

		}

	}
	
	/// <summary>
	/// A Allocation record.
	/// </summary>
	public class Allocation
	{

		// This list is used to handle pass event arguments outside the normal table locking scheme for events.
		private static AllocationEventArgList allocationEventArgList;
		
		/// <summary>This event is triggered when a proposed order is added, modified or deleted.</summary>
		public static AllocationEvent Changed;

		/// <summary>The unique identifier of this proposed order.</summary>
		private int allocationId;
		/// <summary>The distinct position of this order.</summary>
		private Position position;
		/// <summary>The limit or stop price for this order.</summary>
		private decimal quantity;
		/// <summary>The pricing instructions for the order (MKT, LMT, STPLMT, etc).</summary>
		private decimal price;
		/// <summary>The commission paid to the broker.</summary>
		private decimal commission;
		/// <summary>The accrued interest paid to the seller.</summary>
		private decimal accruedInterest;
		/// <summary>Miscellaneous Fees.</summary>
		private decimal userFee0;
		/// <summary>Miscellaneous Fees.</summary>
		private decimal userFee1;
		/// <summary>Miscellaneous Fees.</summary>
		private decimal userFee2;
		/// <summary>Miscellaneous Fees.</summary>
		private decimal userFee3;

		/// <summary>Returns the elements of a position (account, security, long or short).</summary>
		public Position Position {get {return this.position;}}
		/// <summary>Gets the quantity of this order.</summary>
		public decimal Quantity {get {return this.quantity;}}
		/// <summary>Gets the limit or stop price of this order.</summary>
		public decimal Price {get {return this.price;}}
		/// <summary>Gets the commission paid to the broker.</summary>
		public decimal Commission {get {return this.commission;}}
		/// <summary>Gets the Accrued Interest paid to the seller.</summary>
		public decimal AccruedInterest {get {return this.accruedInterest;}}
		/// <summary>Gets the miscellaneous fees associated with the execution.</summary>
		public decimal UserFee0 {get {return this.userFee0;}}
		/// <summary>Gets the miscellaneous fees associated with the execution.</summary>
		public decimal UserFee1 {get {return this.userFee1;}}
		/// <summary>Gets the miscellaneous fees associated with the execution.</summary>
		public decimal UserFee2 {get {return this.userFee2;}}
		/// <summary>Gets the miscellaneous fees associated with the execution.</summary>
		public decimal UserFee3 {get {return this.userFee3;}}

		/// <summary>
		/// Initializer for all proposed orders.
		/// </summary>
		static Allocation()
		{

			// IMPORTANT CONCEPT: To simplify the programming model, the event handlers are not called directly.  The issue is that
			// all the tables are locked during the handling of the primary event handlers off the Market Data model.  The language
			// primitives also assume the responsibility of locking tables.  Remember that nested locking has been prohibited to
			// insure that deadlocks won't happen.  So, we're left with a problem: how can the financial language primitives be
			// called from inside the event handlers.  The solution: they can't, so we set up a system where the events are
			// rebroadcast after the merge.  This list will collect the events from the primary data model.  Later, during the
			// handing of the 'EndMerge' event, they will be rebroadcast (when the tables are no longer locked).
			Allocation.allocationEventArgList = new AllocationEventArgList();
			
			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will chain the data model event handler into the Rules Language handler.
				ClientMarketData.Allocation.AllocationRowChanged += new ClientMarketData.AllocationRowChangeEventHandler(AllocationHandler);
				ClientMarketData.Allocation.AllocationRowDeleted += new ClientMarketData.AllocationRowChangeEventHandler(AllocationHandler);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="allocationId">The primary identifer of the record.</param>
		/// <param name="position">Identifies the account/security/position type code for the order.</param>
		/// <param name="tif">Time in Force</param>
		/// <param name="pricedAt">How the order is priced.</param>
		/// <param name="quantity">The number of units being purchased.</param>
		/// <param name="price1">The limit/stop price.</param>
		/// <param name="price2">The stop limit price.</param>
		public Allocation(int allocationId, Position position, decimal quantity, decimal price, decimal commission,
			decimal accruedInterest, decimal userFee0, decimal userFee1, decimal userFee2, decimal userFee3)
		{

			// Initialize the object.
			this.allocationId = allocationId;
			this.position = position;
			this.quantity = quantity;
			this.price = price;
			this.commission = commission;
			this.accruedInterest = accruedInterest;
			this.userFee0 = userFee0;
			this.userFee1 = userFee1;
			this.userFee2 = userFee2;
			this.userFee3 = userFee3;

		}
		
		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="allocationRow">A proposed order record from the primary ADO database.</param>
		/// <returns>A Allocation record based on the ADO record.</returns>
		internal static Allocation Make(ClientMarketData.AllocationRow allocationRow)
		{

			// Initialize the object
			DataRowVersion dataRowVersion = allocationRow.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;

			// Extract the data from the ADO record.
			int allocationId = (int)allocationRow[ClientMarketData.Allocation.AllocationIdColumn, dataRowVersion];
			TransactionType transactionType = (TransactionType)allocationRow[ClientMarketData.Allocation.TransactionTypeCodeColumn, dataRowVersion];
			int accountId = (int)allocationRow[ClientMarketData.Allocation.AccountIdColumn, dataRowVersion];
			int securityId = (int)allocationRow[ClientMarketData.Allocation.SecurityIdColumn, dataRowVersion];
			int positionTypeCode = Common.TransactionType.GetPosition((int)transactionType);
			Position position = Position.Make(accountId, securityId, positionTypeCode);
			decimal quantity = (decimal)allocationRow[ClientMarketData.Allocation.QuantityColumn, dataRowVersion];
			decimal price = (decimal)allocationRow[ClientMarketData.Allocation.PriceColumn, dataRowVersion];
			decimal commission = (decimal)allocationRow[ClientMarketData.Allocation.CommissionColumn, dataRowVersion];
			decimal accruedInterest = (decimal)allocationRow[ClientMarketData.Allocation.AccruedInterestColumn, dataRowVersion];
			decimal userFee0 = (decimal)allocationRow[ClientMarketData.Allocation.UserFee0Column, dataRowVersion];
			decimal userFee1 = (decimal)allocationRow[ClientMarketData.Allocation.UserFee1Column, dataRowVersion];
			decimal userFee2 = (decimal)allocationRow[ClientMarketData.Allocation.UserFee2Column, dataRowVersion];
			decimal userFee3 = (decimal)allocationRow[ClientMarketData.Allocation.UserFee3Column, dataRowVersion];

			// Create a new record based on the data extracted from the ADO database.
			return new Allocation(allocationId, position, quantity, price, commission, accruedInterest, userFee0, userFee1,
				userFee2, userFee3);

		}
		
		/// <summary>
		/// Handles the start of a Database Merge.
		/// </summary>
		public static void OnBeginMerge()
		{

			// This list is used to queue up the proposed orders during the primary database event handlers.  They will be passed
			// on to the langauge event handlers when the primary database during the handling of the 'EndMerge' event, when the
			// tables are no longer locked.
			Allocation.allocationEventArgList.Clear();

		}
		
		/// <summary>
		/// Handles the primary Market Data events and passes the events along to the Langauge Primitives.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="allocationRowChangeEvent">The record change event argument.</param>
		public static void AllocationHandler(object sender, ClientMarketData.AllocationRowChangeEvent allocationRowChangeEvent)
		{

			// Extract the record from the event argument.
			ClientMarketData.AllocationRow allocationRow = allocationRowChangeEvent.Row;

			// Translate the ADO.NET row states into a record state used by the Rules Engine.
			Action action = Action.Nothing;
			switch (allocationRowChangeEvent.Action)
			{
			case DataRowAction.Add: action = Action.Add; break;
			case DataRowAction.Delete: action = Action.Delete; break;
			case DataRowAction.Change: action = Action.Change; break;
			case DataRowAction.Commit: return;
			}

			// Place the event into a list that will be processed when the tables are no longer locked.
			Allocation.allocationEventArgList.Add(new AllocationEventArgs(action, Allocation.Make(allocationRow)));

		}

		/// <summary>
		/// Handles the end of a Database Merge.
		/// </summary>
		public static void OnEndMerge()
		{

			// Call the virtual method to broadcast the events to the Rules Engine.
			foreach (AllocationEventArgs allocationEventArgs in Allocation.allocationEventArgList)
				OnAllocationChanged(allocationEventArgs);

		}
		
		/// <summary>
		/// Broadcasts a change of record event.
		/// </summary>
		/// <param name="allocationsEventArgs">The event argument.</param>
		public static void OnAllocationChanged(AllocationEventArgs allocationsEventArgs)
		{

			// Broadcast the event to any listeners.
			if (Allocation.Changed != null)
				Allocation.Changed(typeof(Allocation), allocationsEventArgs);

		}

	}

}
