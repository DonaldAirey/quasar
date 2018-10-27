/*************************************************************************************************************************
*
*	File:			ProposedOrder.cs
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
	public delegate void ProposedOrderEvent(object sender, ProposedOrderEventArgs proposedOrderEventArgs);
	
	/// <summary>
	/// Event argument for a proposed order event.
	/// </summary>
	public class ProposedOrderEventArgs : EventArgs
	{

		private Action action;
		private ProposedOrder proposedOrder;

		/// <summary>
		/// The action taken on the proposed order (add, delete, modify)
		/// </summary>
		public Action Action {get {return this.action;}}

		/// <summary>
		/// The proposed order record.
		/// </summary>
		public ProposedOrder ProposedOrder {get {return this.proposedOrder;}}
		
		/// <summary>
		/// Constructs and argument for passing a proposed order event to a listener.
		/// </summary>
		/// <param name="action">The action taken on the proposed order.</param>
		/// <param name="proposedOrder">The proposed order record.</param>
		public ProposedOrderEventArgs(Action action, ProposedOrder proposedOrder)
		{

			// Initialize the record.
			this.action = action;
			this.proposedOrder = proposedOrder;

		}

	}

	/// <summary>
	/// A list of ProposedOrderEventArgs.
	/// </summary>
	public class ProposedOrderEventArgList : ArrayList
	{

		/// <summary>
		/// Insures that the generic version of this method isn't used.
		/// </summary>
		/// <param name="value">The value to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public override int Add(object value)
		{

			// Insure that generic items aren't added to the list.
			throw new Exception("Illegal value in ProposedOrderEventArgList");

		}

		/// <summary>
		/// Adds a ProposedOrderEventArgs to the list.
		/// </summary>
		/// <param name="proposedOrderEventArgs">The event arguments to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public int Add(ProposedOrderEventArgs proposedOrderEventArgs)
		{

			// Add the event arguments to the list.
			return base.Add(proposedOrderEventArgs);

		}

	}
	
	/// <summary>
	/// A ProposedOrder record.
	/// </summary>
	public class ProposedOrder
	{

		// This list is used to handle pass event arguments outside the normal table locking scheme for events.
		private static ProposedOrderEventArgList proposedOrderEventArgList;
		
		/// <summary>This event is triggered when a proposed order is added, modified or deleted.</summary>
		public static ProposedOrderEvent Changed;

		/// <summary>The unique identifier of this proposed order.</summary>
		private int proposedOrderId;
		/// <summary>The distinct position of this order.</summary>
		private Position position;
		/// <summary>The type of transaction (buy, sell, buy cover, sell short).</summary>
		private TransactionType transactionType;
		/// <summary>The time limit for the trade (DAY, GTC, OPG, etc).</summary>
		private TIF tif;
		/// <summary>The number of units being traded.</summary>
		private PricedAt pricedAt;
		/// <summary>The limit or stop price for this order.</summary>
		private decimal quantity;
		/// <summary>The pricing instructions for the order (MKT, LMT, STPLMT, etc).</summary>
		private object price1;
		/// <summary>The stop limit price for this order.</summary>
		private object price2;

		/// <summary>Returns the elements of a position (account, security, long or short).</summary>
		public Position Position {get {return this.position;}}
		/// <summary>Gets the transaction type of the order.</summary>
		public TransactionType TransactionType {get {return this.transactionType;}}
		/// <summary>Gets the time limit of this order.</summary>
		public TIF TIF {get {return this.tif;}}
		/// <summary>Gets the pricing for this order.</summary>
		public PricedAt PricedAt {get {return this.pricedAt;}}
		/// <summary>Gets the quantity of this order.</summary>
		public decimal Quantity {get {return this.quantity;}}
		/// <summary>Gets the limit or stop price of this order.</summary>
		public object Price1 {get {return this.price1;}}
		/// <summary>Gets the stop limit price of this order.</summary>
		public object Price2 {get {return this.price2;}}

		/// <summary>
		/// Initializer for all proposed orders.
		/// </summary>
		static ProposedOrder()
		{

			// IMPORTANT CONCEPT: To simplify the programming model, the event handlers are not called directly.  The issue is that
			// all the tables are locked during the handling of the primary event handlers off the Market Data model.  The language
			// primitives also assume the responsibility of locking tables.  Remember that nested locking has been prohibited to
			// insure that deadlocks won't happen.  So, we're left with a problem: how can the financial language primitives be
			// called from inside the event handlers.  The solution: they can't, so we set up a system where the events are
			// rebroadcast after the merge.  This list will collect the events from the primary data model.  Later, during the
			// handing of the 'EndMerge' event, they will be rebroadcast (when the tables are no longer locked).
			ProposedOrder.proposedOrderEventArgList = new ProposedOrderEventArgList();
			
			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will chain the data model event handler into the Rules Language handler.
				ClientMarketData.ProposedOrder.ProposedOrderRowChanged += new ClientMarketData.ProposedOrderRowChangeEventHandler(ProposedOrderHandler);
				ClientMarketData.ProposedOrder.ProposedOrderRowDeleted += new ClientMarketData.ProposedOrderRowChangeEventHandler(ProposedOrderHandler);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="proposedOrderId">The primary identifer of the record.</param>
		/// <param name="position">Identifies the account/security/position type code for the order.</param>
		/// <param name="tif">Time in Force</param>
		/// <param name="pricedAt">How the order is priced.</param>
		/// <param name="quantity">The number of units being purchased.</param>
		/// <param name="price1">The limit/stop price.</param>
		/// <param name="price2">The stop limit price.</param>
		public ProposedOrder(int proposedOrderId, Position position, TIF tif, PricedAt pricedAt, decimal quantity, object price1, object price2)
		{

			// Initialize the object.
			this.proposedOrderId = proposedOrderId;
			this.position = position;
			this.transactionType = transactionType;
			this.tif = tif;
			this.pricedAt = pricedAt;
			this.quantity = quantity;
			this.price1 = price1;
			this.price2 = price2;

		}
		
		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="proposedOrderRow">A proposed order record from the primary ADO database.</param>
		/// <returns>A ProposedOrder record based on the ADO record.</returns>
		internal static ProposedOrder Make(ClientMarketData.ProposedOrderRow proposedOrderRow)
		{

			// Initialize the object
			DataRowVersion dataRowVersion = proposedOrderRow.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;

			// Extract the data from the ADO record.
			int proposedOrderId = (int)proposedOrderRow[ClientMarketData.ProposedOrder.ProposedOrderIdColumn, dataRowVersion];
			TransactionType transactionType = (TransactionType)proposedOrderRow[ClientMarketData.ProposedOrder.TransactionTypeCodeColumn, dataRowVersion];
			int accountId = (int)proposedOrderRow[ClientMarketData.ProposedOrder.AccountIdColumn, dataRowVersion];
			int securityId = (int)proposedOrderRow[ClientMarketData.ProposedOrder.SecurityIdColumn, dataRowVersion];
			int positionTypeCode = Common.TransactionType.GetPosition((int)transactionType);
			Position position = Position.Make(accountId, securityId, positionTypeCode);
			TIF tif = (TIF)proposedOrderRow[ClientMarketData.ProposedOrder.TimeInForceCodeColumn, dataRowVersion];
			PricedAt pricedAt = (PricedAt)proposedOrderRow[ClientMarketData.ProposedOrder.OrderTypeCodeColumn, dataRowVersion];
			decimal quantity = (decimal)proposedOrderRow[ClientMarketData.ProposedOrder.QuantityColumn, dataRowVersion];
			object price1 = proposedOrderRow[ClientMarketData.ProposedOrder.Price1Column, dataRowVersion];
			object price2 = proposedOrderRow[ClientMarketData.ProposedOrder.Price2Column, dataRowVersion];

			// Create a new record based on the data extracted from the ADO database.
			return new ProposedOrder(proposedOrderId, position, tif, pricedAt, quantity, price1, price2);

		}
		
		/// <summary>
		/// Handles the start of a Database Merge.
		/// </summary>
		public static void OnBeginMerge()
		{

			// This list is used to queue up the proposed orders during the primary database event handlers.  They will be passed
			// on to the langauge event handlers when the primary database during the handling of the 'EndMerge' event, when the
			// tables are no longer locked.
			ProposedOrder.proposedOrderEventArgList.Clear();

		}
		
		/// <summary>
		/// Handles the primary Market Data events and passes the events along to the Langauge Primitives.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="proposedOrderRowChangeEvent">The record change event argument.</param>
		public static void ProposedOrderHandler(object sender, ClientMarketData.ProposedOrderRowChangeEvent proposedOrderRowChangeEvent)
		{

			// Extract the record from the event argument.
			ClientMarketData.ProposedOrderRow proposedOrderRow = proposedOrderRowChangeEvent.Row;

			// Translate the ADO.NET row states into a record state used by the Rules Engine.
			Action action = Action.Nothing;
			switch (proposedOrderRowChangeEvent.Action)
			{
				case DataRowAction.Add: action = Action.Add; break;
				case DataRowAction.Delete: action = Action.Delete; break;
				case DataRowAction.Change: action = Action.Change; break;
				case DataRowAction.Commit: return;
			}

			// Place the event into a list that will be processed when the tables are no longer locked.
			ProposedOrder.proposedOrderEventArgList.Add(new ProposedOrderEventArgs(action, ProposedOrder.Make(proposedOrderRow)));

		}

		/// <summary>
		/// Handles the end of a Database Merge.
		/// </summary>
		public static void OnEndMerge()
		{

			// Call the virtual method to broadcast the events to the Rules Engine.
			foreach (ProposedOrderEventArgs proposedOrderEventArgs in ProposedOrder.proposedOrderEventArgList)
				OnProposedOrderChanged(proposedOrderEventArgs);

		}
		
		/// <summary>
		/// Broadcasts a change of record event.
		/// </summary>
		/// <param name="proposedOrdersEventArgs">The event argument.</param>
		public static void OnProposedOrderChanged(ProposedOrderEventArgs proposedOrdersEventArgs)
		{

			// Broadcast the event to any listeners.
			if (ProposedOrder.Changed != null)
				ProposedOrder.Changed(typeof(ProposedOrder), proposedOrdersEventArgs);

		}

	}

}
