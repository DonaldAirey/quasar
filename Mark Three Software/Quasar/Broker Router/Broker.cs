/*************************************************************************************************************************
*
*	File:			Broker.cs
*	Description:	This module contains the simulation of a broker executing orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Market.Router
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
		private static System.ComponentModel.IContainer components;
		private static Shadows.Quasar.Server.ServerMarketData serverMarketData;
		private static PlacementQueue placementQueue;
		private static ArrayList brokerList;

		static Broker()
		{

			// Install the Server Market Data in this container.
			Broker.components = new System.ComponentModel.Container();
			Broker.serverMarketData = new Shadows.Quasar.Server.ServerMarketData(Broker.components);

			// This is a list of brokers that service the simulated orders.
			Broker.brokerList = new ArrayList();

			// This queue is used to pass along a changed placement record to a thread that will create an order in the order book.
			Broker.placementQueue = new PlacementQueue();

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

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release the global tables.
					if (ServerMarketData.BlockOrderLock.IsReaderLockHeld) ServerMarketData.BlockOrderLock.ReleaseReaderLock();
					if (ServerMarketData.PlacementLock.IsReaderLockHeld) ServerMarketData.PlacementLock.ReleaseReaderLock();
					Debug.Assert(!ServerMarketData.AreLocksHeld);

				}

			}

		}

	}

}
