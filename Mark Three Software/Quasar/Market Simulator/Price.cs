/*************************************************************************************************************************
*
*	File:			Market Price.cs
*	Description:	This module contains the simulation of a broker executing orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Market.Simulator
{

	using Shadows.Quasar.Server;
	using Shadows.Quasar.Common;
	using Shadows.WebService;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for Market.
	/// </summary>
	public class Price
	{

		// Constants
		private static decimal DefaultTickerRate = 50.0M;

		// Members
		private static int simulatorCounter;
		private static int timer;
		private static Hashtable marketThreadTable;
		private static Random random = new Random(1);
		private static Shadows.Quasar.Server.ServerMarketData serverMarketData;
		private static System.ComponentModel.IContainer components;
		private static System.Threading.ManualResetEvent pausePrice;

		/// <summary>
		/// This object will simulate an auction market on equities.
		/// </summary>
		static Price()
		{

			// Install the Server Market Data in this container.
			Price.components = new System.ComponentModel.Container();
			Price.serverMarketData = new Shadows.Quasar.Server.ServerMarketData(Price.components);

			// One or more simulators can be running at a time to simulate different market feeds.  This counter keeps track of how
			// many are running at a time.  The counter is also used in an implicit name created for new threads.  If a name isn't 
			// provided when the thread is started, this counter will be used to create a default name.
			Price.simulatorCounter = 0;

			// This hash table is used to manage the multiple simulated threads.
			Price.marketThreadTable = new Hashtable();

			// This signal is used for pausing -- without terminating -- a simulation thread.
			Price.pausePrice = new ManualResetEvent(false);

			// This is the initial time to sleep between generating simulated ticks. The value is in terms of milliseconds.
			Price.timer = Convert.ToInt32(1000.0M / DefaultTickerRate);

			try
			{

				// Lock the tables.
				Debug.Assert(!ServerMarketData.AreLocksHeld);
				ServerMarketData.PriceLock.AcquireWriterLock(Timeout.Infinite);

				// Initialize the bid and ask prices around the last known price.
				foreach (ServerMarketData.PriceRow priceRow in ServerMarketData.Price)
				{

					// Initialize the bid price.
					decimal bidChange = (decimal)(Math.Round(Random.NextDouble() * 0.25, 2));
					priceRow.BidPrice = priceRow.LastPrice - bidChange;
					priceRow.BidSize = (decimal)(Math.Round(Random.NextDouble() * 10.0, 0) * 100.0 + 100);

					// Initialize the ask price.
					decimal askChange = (decimal)(Math.Round(Random.NextDouble(), 2) * 0.25);
					priceRow.AskPrice = priceRow.LastPrice + askChange;
					priceRow.AskSize = (decimal)(Math.Round(Random.NextDouble() * 10.0, 0) * 100.0 + 100);

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the global tables.
				if (ServerMarketData.PriceLock.IsWriterLockHeld) ServerMarketData.PriceLock.ReleaseWriterLock();
				Debug.Assert(!ServerMarketData.AreLocksHeld);

			}

		}

		private static int Timer
		{
			get {lock(typeof(Price)) {return Price.timer;}}
			set {lock (typeof(Price)) {Price.timer = value == 0 ? 1 : value;}}
		}
		
		// Thread safe access to random number generator.
		private static Random Random {get {lock (Price.random) return Price.random;}}

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

			// This keeps track of the number of threads running.
			Price.simulatorCounter++;
			
			// This will be the name and primary identifier of the market simulation thread, unless an explicit name is provided 
			// with the method parameters.
			string defaultThreadName = string.Format("Market Price {0}", simulatorCounter);
			
			// Extract the parameters from the batch.
			decimal tickerRate = remoteMethod.Parameters.GetRequiredDecimal("tickerRate", Price.DefaultTickerRate);
			string name = remoteMethod.Parameters.GetRequiredString("name", defaultThreadName);

			// This governs how fast the ticks are generated.  Note that logic will prevent time between ticks from being set to
			// zero.  It's important that the timer is never set to zero: with no time to rest between ticks, it's difficult for
			// any new tasks to get connected.
			Price.Timer = Convert.ToInt32(1000.0M / tickerRate);
			
			// Start the thread that will simulate the market conditions.  Note that the Hash table allows the thread to be 
			// referenced by name on any later method invokations.
			marketThreadTable[name] = new Thread(new ThreadStart(MarketThread));
			Thread thread = (Thread)marketThreadTable[name];
			thread.Name = name;
			thread.Start();

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Stop(Transaction transaction)
		{

			// This transaction doesn't require a database connection.
			transaction.IsPersistent = false;

		}
		
		/// <summary>
		/// Terminates the simulation of a price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Stop(Transaction transaction, RemoteMethod remoteMethod)
		{

			// If the user doesn't provide an explit name of a thread, the last one will be terminated.
			string defaultThreadName = string.Format("Market Price {0}", Price.simulatorCounter);
			
			// Extract the method parameters.
			string name = remoteMethod.Parameters.GetRequiredString("name", defaultThreadName);

			// Find the thread based on the name and terminate it.
			Thread thread = (Thread)marketThreadTable[name];
			if (thread == null)
				throw new Exception(string.Format("There is no thread named {0}.", name));
			thread.Abort();

			// This keeps track of the number of threads running.
			--Price.simulatorCounter;
			
		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void SetHeuristics(Transaction transaction)
		{

			// This transaction doesn't require a database connection.
			transaction.IsPersistent = false;

		}
		
		/// <summary>
		/// Sets the operating parameters of the simulator.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void SetHeuristics(Transaction transaction, RemoteMethod remoteMethod)
		{

			// Extract the method parameters.
			decimal tickerRate = remoteMethod.Parameters.GetRequiredDecimal("tickerRate", Price.DefaultTickerRate);

			// Set the time between simulated ticks.  This governs how many ticks will be generated per second.
			Price.Timer = Convert.ToInt32(1000.0M / tickerRate);

		}

		/// <summary>
		/// Generates random price movements.
		/// </summary>
		private static void MarketThread()
		{

			// Keep looping until the thread is terminated.
			while (true)
			{

				try
				{

					// Lock the tables.
					Debug.Assert(!ServerMarketData.AreLocksHeld);
					ServerMarketData.EquityLock.AcquireReaderLock(Timeout.Infinite);
					ServerMarketData.PriceLock.AcquireWriterLock(Timeout.Infinite);
					ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);

					// Pick a random security to move from the tax lot tables.  This insures us that the ticks we get are
					// relative to the demo.
					int index = Random.Next(0, ServerMarketData.Equity.Rows.Count);
					ServerMarketData.EquityRow equityRow = ServerMarketData.Equity[index];
					ServerMarketData.SecurityRow securityRow = equityRow.SecurityRowByFKSecurityEquityEquityId;

					// Find a price that matches the equity's default settlement.  This is the price record that will be updated
					// with the simulated market conditions.
					ServerMarketData.PriceRow priceRow = ServerMarketData.Price.FindBySecurityIdCurrencyId(equityRow.EquityId, equityRow.SettlementId);
					if (priceRow == null)
						continue;

					// Randomly change the bid, last or asked price.
					switch (Random.Next(3))
					{

					case 0:

						// The bid price is moved randomly away from the last price.
						decimal bidChange = (decimal)(Math.Round(Random.NextDouble() * 0.25, 2));
						priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
						priceRow.BidPrice = priceRow.LastPrice - bidChange;
						priceRow.BidSize = (decimal)(Math.Round(Random.NextDouble() * 10.0, 0) * 100.0 + 100);
						break;

					case 1:

						// The price is moved randomly from one to 10 tenths of a percent.
						decimal randomPercent = (decimal)(Math.Round(Random.NextDouble() * 0.010, 3) - 0.005);
						priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
						priceRow.PriceChange = Math.Round(priceRow.LastPrice * randomPercent, 2);
						priceRow.LastPrice += priceRow.PriceChange;
						priceRow.LastSize = (decimal)(Math.Round(Random.NextDouble() * 10.0, 0) * 100.0 + 100);
						break;

					case 2:

						// The ask price is moved randomly away from the last price.
						decimal askChange = (decimal)(Math.Round(Random.NextDouble(), 2) * 0.25);
						priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
						priceRow.AskPrice = priceRow.LastPrice + askChange;
						priceRow.AskSize = (decimal)(Math.Round(Random.NextDouble() * 10.0, 0) * 100.0 + 100);
						break;

					}

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release the global tables.
					if (ServerMarketData.EquityLock.IsReaderLockHeld) ServerMarketData.EquityLock.ReleaseReaderLock();
					if (ServerMarketData.PriceLock.IsWriterLockHeld) ServerMarketData.PriceLock.ReleaseWriterLock();
					if (ServerMarketData.SecurityLock.IsReaderLockHeld) ServerMarketData.SecurityLock.ReleaseReaderLock();
					Debug.Assert(!ServerMarketData.AreLocksHeld);

				}

				// The controls how often the simulator picks another price.
				Thread.Sleep(Price.Timer);

			}

		}

	}

}
