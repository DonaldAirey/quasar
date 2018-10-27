/*************************************************************************************************************************
*
*	File:			Broker.cs
*	Description:	This module contains the simulation of a broker executing orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Market.Simulator
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Server;
	using Shadows.WebService;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for Broker.
	/// </summary>
	public class Broker
	{

		private const decimal maxExecution = 10000;
		private static Random random = new Random(1);
		private static OrderBook orderBook;
		private static ReaderWriterLock orderBookLock;
		private static System.ComponentModel.IContainer components;
		private static Shadows.Quasar.Server.ServerMarketData serverMarketData;
		private static PlacementQueue placementQueue;
		private static ArrayList brokerList;
 		private static ManualResetEvent orderBookEvent;

		static Broker()
		{

			// Install the Server Market Data in this container.
			Broker.components = new System.ComponentModel.Container();
			Broker.serverMarketData = new Shadows.Quasar.Server.ServerMarketData(Broker.components);

			// Create an order book to hold the orders.
			orderBookLock = new ReaderWriterLock();
			orderBook = new OrderBook();

			// This is a list of brokers that service the simulated orders.
			Broker.brokerList = new ArrayList();

			// This queue is used to pass along a changed placement record to a thread that will create an order in the order book.
			Broker.placementQueue = new PlacementQueue();

			// This event is used to signal that there are orders in the order book that need simulated executions.
			Broker.orderBookEvent = new ManualResetEvent(false);

			try
			{

				// Lock the tables
				Debug.Assert(!ServerMarketData.AreLocksHeld);
				ServerMarketData.PlacementLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install an event handler that will pump proposed orders into a thread that makes them into simulated orders
				// serviced by an imaginary broker.
				ServerMarketData.Placement.PlacementRowChanged +=
					new ServerMarketData.PlacementRowChangeEventHandler(Broker.PlacementRowChangeEvent);

			}
			finally
			{

				// Release the table locks.
				if (ServerMarketData.PlacementLock.IsReaderLockHeld) ServerMarketData.PlacementLock.ReleaseReaderLock();
				Debug.Assert(!ServerMarketData.AreLocksHeld);

			}

		}

		// Thread safe access to random number generator.
		private static Random Random {get {lock (Broker.random) return Broker.random;}}

		/// <summary>
		/// Handles the changing of an placement record.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private static void PlacementRowChangeEvent(object sender, ServerMarketData.PlacementRowChangeEvent proposedOrderRowChangeEvent)
		{

			// If the order has been committed, and if the placement is an electronic order intended for this simulated broker,
			// then pass the placement on to a worker thread that will place it in the order book.
			if (proposedOrderRowChangeEvent.Action == DataRowAction.Add && proposedOrderRowChangeEvent.Row.IsRouted &&
				Broker.brokerList.Contains(proposedOrderRowChangeEvent.Row.BrokerId))
				Broker.placementQueue.Enqueue(proposedOrderRowChangeEvent.Row.PlacementId);

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Start(Transaction transaction)
		{

			// This transaction doesn't require a database connection.
			transaction.IsPersistent = false;

		}

		/// <summary>
		/// Starts a simulated price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Start(Transaction transaction, RemoteMethod remoteMethod)
		{

			// Extract the parameters from the batch.
			string name = remoteMethod.Parameters.GetRequiredString("name");

			// The 'name' used to start this thread is really a broker symbol.  Use the tables to find a broker symbol that matches
			// the name.  This will become the broker identifier that this simulation thread responds to.
			object brokerId = null;
			try
			{

				// Lock the tables
				Debug.Assert(!ServerMarketData.AreLocksHeld);
				ServerMarketData.BrokerLock.AcquireReaderLock(Timeout.Infinite);

				// Search, the hard way, for a symbol that matches the simulator name.
				foreach (ServerMarketData.BrokerRow brokerRow in ServerMarketData.Broker)
					if (brokerRow.Symbol == name)
						brokerId = brokerRow.BrokerId;

			}
			finally
			{

				// Release the broker table.
				if (ServerMarketData.BrokerLock.IsReaderLockHeld) ServerMarketData.BrokerLock.ReleaseReaderLock();

			}

			// If the broker identifier was found, then start the simulation threads to service all electronic placements that are
			// destined for that broker.
			if (brokerId != null)
			{

				// This list is used to filter out brokers that aren't recognized by this simulator.
				Broker.brokerList.Add(brokerId);

				// This thread will remove placements from the queue and create orders in the local order book.
				Thread placementThread = new Thread(new ThreadStart(PlacementHandler));
				placementThread.Start();

				// Start the thread to handle the random execution of orders in the local order book.
				ThreadArgument threadArgument = new ThreadArgument(new ThreadHandler(ExecutionThread), "Execution Thread", brokerId);
				Thread executionThread = new Thread(new ThreadStart(threadArgument.StartThread));
				executionThread.Start();

			}
			
		}

		/// <summary>
		/// This thread takes placements off the queue and creates orders in the local order book for them.
		/// </summary>
		private static void PlacementHandler()
		{

			while (true)
			{

				// Wait here for the next tick to come in.
				Broker.placementQueue.WaitOne();

				// Get the next placement from the queue.
				int placementId = Broker.placementQueue.Dequeue();

				try
				{

					// Lock the Order Book.
					Broker.orderBookLock.AcquireWriterLock(CommonTimeout.LockWait);

					// Lock the tables.
					Debug.Assert(!ServerMarketData.AreLocksHeld);
					ServerMarketData.BlockOrderLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.PlacementLock.AcquireReaderLock(Timeout.Infinite);

					// Find the placement row.
					ServerMarketData.PlacementRow placementRow = ServerMarketData.Placement.FindByPlacementId(placementId);
					if (placementRow == null)
						return;

					// Information about the security is found in the block.  It isn't part of the placement so a BlockOrder 
					// record is needed to get some of the ancillary data.
					ServerMarketData.BlockOrderRow blockOrderRow = placementRow.BlockOrderRow;

					// The execution quantity needs to be calculated to prevent over execution when the simulator is 
					// restarted.  Count up the quantity executed against this placement.  While this calcuation isn't 
					// perfect -- it will miss when there are multiple placements to the same broker -- it is good enough for
					// the simulator to restart itself.
					decimal quantityExecuted = 0.0M;
					foreach (ServerMarketData.ExecutionRow executionRow in blockOrderRow.GetExecutionRows())
						if (executionRow.BrokerId == placementRow.BrokerId)
							quantityExecuted += executionRow.Quantity;

					// Create a new order book record and copy the data from the placement into the order.
					OrderBook.OrderRow orderRow = Broker.orderBook.Order.NewOrderRow();
					orderRow.BlockOrderId = placementRow.BlockOrderId;
					orderRow.BrokerId = placementRow.BrokerId;
					orderRow.SecurityId = blockOrderRow.SecurityId;
					orderRow.QuantityOrdered = placementRow.Quantity;
					orderRow.QuantityExecuted = quantityExecuted;
					orderRow.TimeInForceCode = placementRow.TimeInForceCode;
					orderRow.OrderTypeCode = placementRow.OrderTypeCode;
					if (!placementRow.IsPrice1Null()) orderRow.Price1 = placementRow.Price1;
					if (!placementRow.IsPrice2Null()) orderRow.Price2 = placementRow.Price2;

					// Add the new record to the book and accept the changes.
					Broker.orderBook.Order.AddOrderRow(orderRow);
					orderRow.AcceptChanges();

					// Signal to the execution thread that there are orders in the local book to be filled.
					Broker.orderBookEvent.Set();

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release the order book.
					if (Broker.orderBookLock.IsWriterLockHeld) Broker.orderBookLock.ReleaseWriterLock();

					// Release the global tables.
					if (ServerMarketData.BlockOrderLock.IsReaderLockHeld) ServerMarketData.BlockOrderLock.ReleaseReaderLock();
					if (ServerMarketData.PlacementLock.IsReaderLockHeld) ServerMarketData.PlacementLock.ReleaseReaderLock();
					Debug.Assert(!ServerMarketData.AreLocksHeld);

				}

			}

		}
		
		/// <summary>
		/// Generates random Price movements.
		/// </summary>
		private static void ExecutionThread(params object[] argument)
		{

			// Extract the thread arguments.  The simulator will execute orders placed with this broker.
			int brokerId = (int)argument[0];
		
			// Continually execute trades from the local order book when they exist.
			while (true)
			{

				// Wait until there is something in the order book to execute.
				Broker.orderBookEvent.WaitOne();
					
				// These parameters are used to create the filled execution.
				int blockOrderId = 0;
				decimal quantity = 0.0M;
				decimal price = 0.0M;
				DateTime tradeDate = DateTime.MinValue;
				DateTime settlementDate = DateTime.MinValue;

				// This will insure that we survive any single error on a random execution.
				try
				{

					// Lock the Order Book.
					Broker.orderBookLock.AcquireWriterLock(CommonTimeout.LockWait);

					// Lock the tables.
					Debug.Assert(!ServerMarketData.AreLocksHeld);
					ServerMarketData.DebtLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.ExecutionLock.AcquireWriterLock(Timeout.Infinite);
					ServerMarketData.EquityLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.HolidayLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.PriceLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);

					// Now that the tables are locked, the trade and settlement dates can be calcualted.
					tradeDate = Trading.TradeDate(0, 0, DateTime.Now);
					settlementDate = Trading.SettlementDate(0, 0, tradeDate);

					// Pick a random order to fill.
					if (Broker.orderBook.Order.Rows.Count > 0)
					{

						int orderIndex = Random.Next(0, Broker.orderBook.Order.Rows.Count);
						OrderBook.OrderRow orderRow = Broker.orderBook.Order[orderIndex];
						blockOrderId = orderRow.BlockOrderId;

						// Find the security associated with this placement.
						ServerMarketData.SecurityRow securityRow = ServerMarketData.Security.FindBySecurityId(orderRow.SecurityId);
						if (securityRow == null)
							continue;

						// A settlement Price is required to find the Price of a security.  For many issues, the default can 
						// be found from the defaults.  If a settlement currency isn't available, this issue can't be executed.
						object settlementId = null;
						foreach (ServerMarketData.DebtRow debtsRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
							settlementId = debtsRow.SettlementId;
						foreach (ServerMarketData.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
							settlementId = equityRow.SettlementId;
						if (settlementId == null)
							continue;

						// Simulate an executed quantity for this order.
						decimal quantityLeaves = orderRow.QuantityOrdered - orderRow.QuantityExecuted;
						if (quantityLeaves >= 100.0M)
						{
							quantity = Random.Next(100, Convert.ToInt32((quantityLeaves > maxExecution) ? maxExecution : quantityLeaves));
							quantity = Math.Round(quantity / 100.0M) * 100.0M;
						}

						// When the quantity is zero, the order has been filled and can be removed from the local order book.  When
						// the book is empty, then this thread can be paused until a new order is generated from a new placement.  
						// In either event, when there are no shares in the order book, there can be no executions.  When there's
						// nothing to execute, delete the order and go look for another order to fill.
						if (quantity == 0.0M)
						{
							orderRow.Delete();
							orderRow.AcceptChanges();
							if (orderBook.Order.Rows.Count == 0)
								Broker.orderBookEvent.Reset();
							continue;
						}

						// Update the amount executed on this order.
						orderRow.QuantityExecuted += quantity;

						// Select a Price for the execution.
						ServerMarketData.PriceRow priceRow = ServerMarketData.Price.FindBySecurityIdCurrencyId(securityRow.SecurityId,
							(int)settlementId);
						if (priceRow != null)
							price = priceRow.LastPrice;

					}

				}
				catch (Exception exception)
				{

					// Display the error.
					Debug.WriteLine(exception.Message);

				}
				finally
				{

					// Release the order book.
					if (Broker.orderBookLock.IsWriterLockHeld) Broker.orderBookLock.ReleaseWriterLock();

					// Release the tables.
					if (ServerMarketData.DebtLock.IsReaderLockHeld) ServerMarketData.DebtLock.ReleaseReaderLock();
					if (ServerMarketData.EquityLock.IsReaderLockHeld) ServerMarketData.EquityLock.ReleaseReaderLock();
					if (ServerMarketData.ExecutionLock.IsWriterLockHeld) ServerMarketData.ExecutionLock.ReleaseWriterLock();
					if (ServerMarketData.HolidayLock.IsReaderLockHeld) ServerMarketData.HolidayLock.ReleaseReaderLock();
					if (ServerMarketData.PriceLock.IsReaderLockHeld) ServerMarketData.PriceLock.ReleaseReaderLock();
					if (ServerMarketData.SecurityLock.IsReaderLockHeld) ServerMarketData.SecurityLock.ReleaseReaderLock();
					Debug.Assert(!ServerMarketData.AreLocksHeld);

				}

				// Create an execution record for this fill.
				Transaction transaction = new Transaction();
				Shadows.WebService.Trading.Execution.Insert(transaction);
				transaction.BeginTransaction();
				try
				{
					long rowVersion = 0L;
					Shadows.WebService.Trading.Execution.Insert(transaction, blockOrderId, brokerId, ref rowVersion, quantity, price, null,
						null, null, null, null, null, tradeDate, settlementDate);
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
				}
				transaction.EndTransaction();

				// Sleep for a random amount of time between execution.
				Thread.Sleep(Random.Next(50, 300));

			}

		}

	}

}
