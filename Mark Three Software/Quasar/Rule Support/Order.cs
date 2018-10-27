/*************************************************************************************************************************
*
*	File:			Order.cs
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
	public delegate void OrderEvent(object sender, OrderEventArgs orderEventArgs);
	
	/// <summary>
	/// Event argument for a proposed order event.
	/// </summary>
	public class OrderEventArgs : EventArgs
	{

		private Action action;
		private Order order;

		/// <summary>
		/// The action taken on the proposed order (add, delete, modify)
		/// </summary>
		public Action Action {get {return this.action;}}

		/// <summary>
		/// The proposed order record.
		/// </summary>
		public Order Order {get {return this.order;}}
		
		/// <summary>
		/// Constructs and argument for passing a proposed order event to a listener.
		/// </summary>
		/// <param name="action">The action taken on the proposed order.</param>
		/// <param name="order">The proposed order record.</param>
		public OrderEventArgs(Action action, Order order)
		{

			// Initialize the record.
			this.action = action;
			this.order = order;

		}

	}

	/// <summary>
	/// A list of OrderEventArgs.
	/// </summary>
	public class OrderEventArgList : ArrayList
	{

		/// <summary>
		/// Insures that the generic version of this method isn't used.
		/// </summary>
		/// <param name="value">The value to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public override int Add(object value)
		{

			// Insure that generic items aren't added to the list.
			throw new Exception("Illegal value in OrderEventArgList");

		}

		/// <summary>
		/// Adds a OrderEventArgs to the list.
		/// </summary>
		/// <param name="orderEventArgs">The event arguments to be added to the list.</param>
		/// <returns>The index of the new value.</returns>
		public int Add(OrderEventArgs orderEventArgs)
		{

			// Add the event arguments to the list.
			return base.Add(orderEventArgs);

		}

	}
	
	/// <summary>
	/// A Order record.
	/// </summary>
	public class Order
	{

		// This list is used to handle pass event arguments outside the normal table locking scheme for events.
		private static OrderEventArgList orderEventArgList;
		
		/// <summary>This event is triggered when a proposed order is added, modified or deleted.</summary>
		public static OrderEvent Changed;

		/// <summary>The unique identifier of this proposed order.</summary>
		private int orderId;
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
		static Order()
		{

			// IMPORTANT CONCEPT: To simplify the programming model, the event handlers are not called directly.  The issue is that
			// all the tables are locked during the handling of the primary event handlers off the Market Data model.  The language
			// primitives also assume the responsibility of locking tables.  Remember that nested locking has been prohibited to
			// insure that deadlocks won't happen.  So, we're left with a problem: how can the financial language primitives be
			// called from inside the event handlers.  The solution: they can't, so we set up a system where the events are
			// rebroadcast after the merge.  This list will collect the events from the primary data model.  Later, during the
			// handing of the 'EndMerge' event, they will be rebroadcast (when the tables are no longer locked).
			Order.orderEventArgList = new OrderEventArgList();
			
			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will chain the data model event handler into the Rules Language handler.
				ClientMarketData.Order.OrderRowChanged += new ClientMarketData.OrderRowChangeEventHandler(OrderHandler);
				ClientMarketData.Order.OrderRowDeleted += new ClientMarketData.OrderRowChangeEventHandler(OrderHandler);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Creates a proposed order record.
		/// </summary>
		/// <param name="orderId">The primary identifer of the record.</param>
		/// <param name="position">Identifies the account/security/position type code for the order.</param>
		/// <param name="tif">Time in Force</param>
		/// <param name="pricedAt">How the order is priced.</param>
		/// <param name="quantity">The number of units being purchased.</param>
		/// <param name="price1">The limit/stop price.</param>
		/// <param name="price2">The stop limit price.</param>
		public Order(int orderId, Position position, TIF tif, PricedAt pricedAt, decimal quantity, object price1, object price2)
		{

			// Initialize the object.
			this.orderId = orderId;
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
		/// <param name="orderRow">A proposed order record from the primary ADO database.</param>
		/// <returns>A Order record based on the ADO record.</returns>
		internal static Order Make(ClientMarketData.OrderRow orderRow)
		{

			// Initialize the object
			DataRowVersion dataRowVersion = orderRow.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;

			// Extract the data from the ADO record.
			int orderId = (int)orderRow[ClientMarketData.Order.OrderIdColumn, dataRowVersion];
			TransactionType transactionType = (TransactionType)orderRow[ClientMarketData.Order.TransactionTypeCodeColumn, dataRowVersion];
			int accountId = (int)orderRow[ClientMarketData.Order.AccountIdColumn, dataRowVersion];
			int securityId = (int)orderRow[ClientMarketData.Order.SecurityIdColumn, dataRowVersion];
			int positionTypeCode = Common.TransactionType.GetPosition((int)transactionType);
			Position position = Position.Make(accountId, securityId, positionTypeCode);
			TIF tif = (TIF)orderRow[ClientMarketData.Order.TimeInForceCodeColumn, dataRowVersion];
			PricedAt pricedAt = (PricedAt)orderRow[ClientMarketData.Order.OrderTypeCodeColumn, dataRowVersion];
			decimal quantity = (decimal)orderRow[ClientMarketData.Order.QuantityColumn, dataRowVersion];
			object price1 = orderRow[ClientMarketData.Order.Price1Column, dataRowVersion];
			object price2 = orderRow[ClientMarketData.Order.Price2Column, dataRowVersion];

			// Create a new record based on the data extracted from the ADO database.
			return new Order(orderId, position, tif, pricedAt, quantity, price1, price2);

		}
		
		/// <summary>
		/// Handles the start of a Database Merge.
		/// </summary>
		public static void OnBeginMerge()
		{

			// This list is used to queue up the proposed orders during the primary database event handlers.  They will be passed
			// on to the langauge event handlers when the primary database during the handling of the 'EndMerge' event, when the
			// tables are no longer locked.
			Order.orderEventArgList.Clear();

		}
		
		/// <summary>
		/// Handles the primary Market Data events and passes the events along to the Langauge Primitives.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="orderRowChangeEvent">The record change event argument.</param>
		public static void OrderHandler(object sender, ClientMarketData.OrderRowChangeEvent orderRowChangeEvent)
		{

			// Extract the record from the event argument.
			ClientMarketData.OrderRow orderRow = orderRowChangeEvent.Row;

			// Translate the ADO.NET row states into a record state used by the Rules Engine.
			Action action = Action.Nothing;
			switch (orderRowChangeEvent.Action)
			{
			case DataRowAction.Add: action = Action.Add; break;
			case DataRowAction.Delete: action = Action.Delete; break;
			case DataRowAction.Change: action = Action.Change; break;
			case DataRowAction.Commit: return;
			}

			// Place the event into a list that will be processed when the tables are no longer locked.
			Order.orderEventArgList.Add(new OrderEventArgs(action, Order.Make(orderRow)));

		}

		/// <summary>
		/// Handles the end of a Database Merge.
		/// </summary>
		public static void OnEndMerge()
		{

			// Call the virtual method to broadcast the events to the Rules Engine.
			foreach (OrderEventArgs orderEventArgs in Order.orderEventArgList)
				OnOrderChanged(orderEventArgs);

		}
		
		/// <summary>
		/// Broadcasts a change of record event.
		/// </summary>
		/// <param name="ordersEventArgs">The event argument.</param>
		public static void OnOrderChanged(OrderEventArgs ordersEventArgs)
		{

			// Broadcast the event to any listeners.
			if (Order.Changed != null)
				Order.Changed(typeof(Order), ordersEventArgs);

		}

		public void Initialize(Account account, Security security, TransactionType transactionType, TIF tif,
			PricedAt pricedAt, decimal quantity, object price1, object price2)
		{

			// Create a block order on the server.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Order");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("accountId", account.AccountId);
			remoteMethod.Parameters.Add("securityId", security.SecurityId);
			remoteMethod.Parameters.Add("settlementId", security.SettlementId);
			remoteMethod.Parameters.Add("transactionTypeCode", (int)transactionType);
			remoteMethod.Parameters.Add("timeInForceCode", (int)tif);
			remoteMethod.Parameters.Add("orderTypeCode", (int)pricedAt);
			remoteMethod.Parameters.Add("quantity", quantity);
			remoteMethod.Parameters.Add("price1", price1 == null ? (object)DBNull.Value : price1);
			remoteMethod.Parameters.Add("price2", price2 == null ? (object)DBNull.Value : price2);
			ClientMarketData.Execute(remoteBatch);

			// Now that the block order is created, construct the in-memory version of the record.
			int orderId = (int)remoteMethod.Parameters["orderId"].Value;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.OrderRow orderRow = ClientMarketData.Order.FindByOrderId(orderId);
				if (orderRow == null)
					throw new Exception(String.Format("Order {0} doesn't exist", orderId));

				Initialize(orderRow);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		public void Initialize(ClientMarketData.OrderRow orderRow)
		{

			this.orderId = orderRow.OrderId;
			this.transactionType = (TransactionType)orderRow.TransactionTypeCode;
			this.tif = (TIF)orderRow.TimeInForceCode;
			this.pricedAt = (PricedAt)orderRow.OrderTypeCode;
			this.quantity = orderRow.Quantity;
			this.price1 = orderRow.IsPrice1Null() ? (object)null : orderRow.Price1;
			this.price2 = orderRow.IsPrice2Null() ? (object)null : orderRow.Price2;

		}

		public Order(Account account, Security security, TransactionType transactionType, TIF tif, decimal quantity)
		{

			Initialize(account, security, transactionType, tif, PricedAt.Market, quantity, 0.0M, 0.0M);

		}

		public Order(Account account, Security security, TransactionType transactionType, TIF tif, PricedAt pricedAt,
			decimal quantity, decimal limitPrice)
		{

			Initialize(account, security, transactionType, tif, pricedAt, quantity, limitPrice, 0.0M);

		}

		public Order(Account account, Security security, TransactionType transactionType, TIF tif, PricedAt pricedAt,
			decimal quantity, decimal limitPrice, decimal stopLimitPrice)
		{

			Initialize(account, security, transactionType, tif, pricedAt, quantity, limitPrice, stopLimitPrice);

		}

	}

}
