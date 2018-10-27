namespace MarkThree.Guardian.Server
{

	using MarkThree;
	using MarkThree.Guardian.Server;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>A Price simulation of an auction market.</summary>
	/// <copyright>Copyright (C) 1998-2005 Mark Three Software -- All Rights Reserved.</copyright>
	public class Price : System.ComponentModel.Component
	{

		// Private Members
		private static bool isRunning;
		private static Random random = new Random(1);
		private static System.ComponentModel.IContainer components;
        private static System.Threading.Thread marketThread;
        private static Hashtable volumeDivergenceMap;
        private static Hashtable openingPriceMap;


		/// This member provides access to the in-memory database.
		private static ServerMarketData serverMarketData = new ServerMarketData();

		/// <summary>
		/// This object will simulate an auction market on equities.
		/// </summary>
		static Price()
		{

			// Install the Server Market Data in this container.
			Price.components = new System.ComponentModel.Container();
            volumeDivergenceMap = new Hashtable();
            openingPriceMap = new Hashtable();

            isRunning = false;

 			try
			{

				// Lock the tables.
				Debug.Assert(!ServerMarketData.IsLocked);
				ServerMarketData.PriceLock.AcquireWriterLock(Timeout.Infinite);

				// Initialize the bid and ask prices around the last known price.
				foreach (ServerMarketData.PriceRow priceRow in ServerMarketData.Price)
				{
                    // make sure the last price is valid
                    if (priceRow.LastPrice < 0.50m)
                        priceRow.LastPrice = random.Next(10, 50);
                    
					// Initialize the bid/ask prices.
					priceRow.BidPrice = priceRow.LastPrice - 0.01m;
                    priceRow.BidSize = 100;
					priceRow.AskPrice = priceRow.LastPrice + 0.01m;
					priceRow.AskSize = 100;
                    priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
                    priceRow.VolumeWeightedAveragePrice = priceRow.LastPrice;
                    priceRow.HighPrice = priceRow.LastPrice;
                    priceRow.LowPrice = priceRow.LastPrice;
                    priceRow.OpenPrice = priceRow.LastPrice;
                    priceRow.ClosePrice = priceRow.LastPrice;

                    openingPriceMap.Add(priceRow.SecurityId, priceRow.LastPrice);
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
				Debug.Assert(!ServerMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Start(AdoTransaction adoTransaction)
		{
			// No tables locks are required to start the simulator.
		
		}

		/// <summary>
		/// Starts a simulated price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Start(ParameterList parameters)
		{
            // only let one run
            if(Price.isRunning)
                throw new Exception("Price simulator is already running.");
	
			// Start the thread that will simulate the market conditions
            marketThread = new Thread(new ThreadStart(MarketThread));
            marketThread.Name = "Price Simulator";
            marketThread.Start();

			MarkThree.EventLog.Information("Price Simulator started.");
		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Stop(AdoTransaction adoTransaction)
		{

			// There are no table locks required to stop the simulator.

		}
		
		/// <summary>
		/// Terminates the simulation of a price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Stop(ParameterList parameters)
		{

			// terminate the thread
            marketThread.Abort();
		
			MarkThree.EventLog.Information("Price Simulator stopped.");
		}

		/// <summary>
		/// Generates random price movements.
		/// </summary>
		private static void MarketThread()
		{


            // Keep looping until the thread is terminated.
			while (true)
			{
                long thisLoopTicks = DateTime.Now.Ticks;

				try
				{
					// Lock the tables.
					Debug.Assert(!ServerMarketData.IsLocked);
                    ServerMarketData.WorkingOrderLock.AcquireReaderLock(Timeout.Infinite);
					
                    // iterate through the working orders and process each security
                    ServerMarketData.WorkingOrderRow[] workingOrders = (ServerMarketData.WorkingOrderRow[])ServerMarketData.WorkingOrder.Select();
                    
                    // release the locks so other procedures CancelEventArgs update things
                    if (ServerMarketData.WorkingOrderLock.IsReaderLockHeld) ServerMarketData.WorkingOrderLock.ReleaseReaderLock();

                    foreach (ServerMarketData.WorkingOrderRow workingOrder in workingOrders)
                    {
                        ProcessSecurity(workingOrder.SecurityId, workingOrder.SettlementId);
                    }
				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					MarkThree.EventLog.Error(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release the locks
                    if (ServerMarketData.WorkingOrderLock.IsReaderLockHeld) ServerMarketData.WorkingOrderLock.ReleaseReaderLock();
					Debug.Assert(!ServerMarketData.IsLocked);

				}

				// make sure we do not go through the loop faster than once a second (100,000 ticks or 1000 ms)
                long loopTimeMs = (DateTime.Now.Ticks - thisLoopTicks) * 100;
			    Thread.Sleep((int)Math.Max(100, (1000 - loopTimeMs)));
			}

		}

        private static void ProcessSecurity(int securityId, int currencyId )
        {
            bool didDataChange = false;

            try
            {
                // Lock the tables.
                Debug.Assert(!ServerMarketData.IsLocked);
                ServerMarketData.PriceLock.AcquireWriterLock(Timeout.Infinite);
                ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);


                // get the price row associated with this security
                ServerMarketData.PriceRow priceRow = ServerMarketData.Price.FindBySecurityId(securityId);

                // if the row does not exist add it
                if (priceRow == null)
                {
                    priceRow = ServerMarketData.Price.NewPriceRow();
                    //priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
                    priceRow.SecurityId = securityId;
                    priceRow.CurrencyId = currencyId;
                    priceRow.LastPrice = 1.0M;
                    priceRow.AskPrice = 1.01M;
                    priceRow.BidPrice = 0.99M;
                    priceRow.Volume = 0.0M;
                    priceRow.VolumeWeightedAveragePrice = priceRow.LastPrice;
                    priceRow.HighPrice = priceRow.LastPrice;
                    priceRow.LowPrice = priceRow.LastPrice;
                    priceRow.OpenPrice = priceRow.LastPrice;
                    priceRow.ClosePrice = priceRow.LastPrice;

                    ServerMarketData.Price.AddPriceRow(priceRow);

                    openingPriceMap.Add(priceRow.SecurityId, priceRow.LastPrice);
                }
                
                // if we got a row then process it
                if (priceRow != null)
                {
                    // get the opening price
                    decimal openingPrice = priceRow.LastPrice;
                    if (openingPriceMap.Contains(securityId))
                        openingPrice = (decimal)openingPriceMap[securityId];
                    if(openingPrice == 0)
                        openingPrice = 10;

                    // if we do not already have a volume or approx every 50th(ish) time update the volume
                    if (priceRow.Volume <= 0 || (random.Next(50) == 0))
                    {
                        // if this security ID is not already in the volume table pick a randon volume divergrance (-50% to 50%)
                        // and add it to the table
                        double volumeDivergence = 0.0;
                        if (volumeDivergenceMap.Contains(securityId))
                            volumeDivergence = (double)volumeDivergenceMap[securityId];
                        else
                            volumeDivergence = (double)random.Next(-50, 50);

                        // increase or decrease by up to 1%
                        int divergenceChangePercent = random.Next(-100,100);

                        // don't go over 50% up or down
                        volumeDivergence = Math.Max(-50.0, Math.Min(50.0, volumeDivergence + ((double)divergenceChangePercent / 100.0) ));

                        // update the volume divergence table
                        volumeDivergenceMap[securityId] = volumeDivergence;
                        
                        // get the estimated volume for this time of day
                        decimal projectVolume = ProjectedVolume(DateTime.Now, priceRow.SecurityRow.AverageDailyVolume);

                        // adjust it by the divergence we are using and update the price table
                        priceRow.Volume = (int) ( projectVolume - (int)((double)projectVolume * ( volumeDivergence / 100.0) ));
                        didDataChange = true;
                    }
                    
                    // base the frequency of price updates to the ADV of the security
                    int updateFrequency = 10;
                    if (priceRow.SecurityRow.AverageDailyVolume < 1000000)      /* UNDER 1M about every 75 loops*/
                        updateFrequency = 75;
                    else
                        if (priceRow.SecurityRow.AverageDailyVolume < 5000000)  /* 1M to 5M about every 30 loops */
                            updateFrequency = 30;
                        else 
                            updateFrequency = 20;                                /* OVER 5M about every 20 loops*/

                    // about once every X loops update the price
                    if (random.Next(updateFrequency) == 0)
                    {
                        // the anount of change will be based on stock price
                        // < $10 ==     always 0.01 (plus or minus)
                        // $10 - 25 ==  0.01 to 0.02 (plus or minus)
                        // $25 - 100 == 0.01 to 0.03 (plus or minus)
                        // > $100  == 0.01 to 0.05 (plus or minus)
                        double priceChange = 0;
                        if (priceRow.LastPrice < 10)
                            priceChange = 0.01 - ((double)random.Next(3) * 0.01);
                        else
                            if (priceRow.LastPrice < 25)
                                priceChange = 0.02 - ((double)random.Next(5) * 0.01);
                            else
                                if (priceRow.LastPrice < 100)
                                    priceChange = 0.03 - ((double)random.Next(7) * 0.01);
                                else
                                    priceChange = 0.05 - ((double)random.Next(11) * 0.01);

                        // change the last price
                        decimal newPrice = priceRow.LastPrice + (decimal)priceChange;
                        decimal lastChange = openingPrice - newPrice;
                        double lastChangepercent = ((double)(openingPrice - lastChange)) / (double)openingPrice;

                        // don't let it go over 20% change per day
                        if( Math.Abs(lastChangepercent) < 20)
                            priceRow.LastPrice = newPrice;

                        // set the spread to be 1 to 5 cents
                        int spreadInCents = random.Next(1, 5);
                        decimal spread = (decimal)spreadInCents / 100m;
                        
                        // set the bid to be 0 to (spread - 1) cents below the price
                        priceRow.BidPrice = priceRow.LastPrice - ((decimal)random.Next(spreadInCents) / 100m);

                        // set the ask based on the spread
                        priceRow.AskPrice = priceRow.BidPrice + spread;
                    
                        // set the bid ask size
                        priceRow.BidSize = random.Next(1, 999) * 100;
                        priceRow.AskSize = random.Next(1, 999) * 100;

                        // check to see if we are at a new high or low for the day
                        if (priceRow.LastPrice > priceRow.HighPrice)
                            priceRow.HighPrice = priceRow.LastPrice;
                        if (priceRow.LastPrice < priceRow.LowPrice)
                            priceRow.LowPrice = priceRow.LastPrice;


                        // set the VWAP to be the average of the open and last
                        // however don't let it be more than 5% from the last
                        decimal newVwap = (priceRow.LastPrice + priceRow.OpenPrice) / 2;
                        decimal fivePercentOfLast = priceRow.LastPrice * 0.05M;
                        priceRow.VolumeWeightedAveragePrice = Math.Max(Math.Min(newVwap, (priceRow.LastPrice + fivePercentOfLast)), (priceRow.LastPrice - fivePercentOfLast));
                        didDataChange = true;
                    }
                }

                // if something changeds update the data model
                if (didDataChange)
                {
                    priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
                    priceRow.AcceptChanges();
                }

            }
            catch (Exception exception)
            {

                // Write the error and stack trace out to the debug listener
                MarkThree.EventLog.Error(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

            }
            finally
            {

                // Release the global tables.
                if (ServerMarketData.PriceLock.IsWriterLockHeld) ServerMarketData.PriceLock.ReleaseWriterLock();
                if (ServerMarketData.SecurityLock.IsReaderLockHeld) ServerMarketData.SecurityLock.ReleaseReaderLock();
                Debug.Assert(!ServerMarketData.IsLocked);

            }

        }
        public static decimal ProjectedVolume(DateTime now, decimal averageDailyVolume)
        {
            const int nineThirtyAm = (9 * 60) + 30;
            const int fourPm = 16 * 60;
            const int minutesInTradingDay = fourPm - nineThirtyAm;
            // get the current time of day in seconds
            int nowMinutes = (now.Hour * 60) + now.Minute;
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            /// The following code is a placeholder until the real formula can be 
            /// developed. For now the trading day is divided into 78 5 minute periods.
            /// each one of these periods has the percent up to that point calulated.
            /// The value are based on the guassian curve, but in reality the function by 
            /// more dynamic and break the day into smaller intervals.
            double[] percentToInterval = {
                        2.345, 4.605, 6.782, 8.878, 10.895, 12.836, 14.703, 16.498, 18.224, 19.882,
                        21.475, 23.006, 24.476, 25.888, 27.244, 28.546, 29.796, 30.997, 32.151, 33.260, 
                        34.326, 35.352, 36.340, 37.292, 38.210, 39.097, 39.955, 40.786, 41.592, 42.376, 
                        43.139, 43.884, 44.614, 45.330, 46.035, 46.731, 47.420, 48.105, 48.788, 49.471, 
                        50.156, 50.845, 51.541, 52.246, 52.962, 53.692, 54.437, 55.200, 55.984, 56.790, 
                        57.621, 58.479, 59.366, 60.284, 61.236, 62.224, 63.250, 64.316, 65.425, 66.579, 
                        67.780, 69.030, 70.332, 71.688, 73.100, 74.570, 76.101, 77.694, 79.352, 81.078, 
                        82.873, 84.740, 86.681, 88.698, 90.794, 92.971, 95.231, 97.576 };

            int intervalCount = 78;
            // compute which 5 minute interval we are in
            int currentInterval = (int)(((double)(nowMinutes - nineThirtyAm) / (double)minutesInTradingDay) * (double)intervalCount);
            // make sure we are in range
            if (currentInterval < 0)
                return 0;
            if (currentInterval >= intervalCount)
                return averageDailyVolume;
            // since we now we are inside the trading day take the average volume and multiple it by the expected volume percent
            decimal projectVolume = (decimal)((double)averageDailyVolume * (percentToInterval[currentInterval] / 100.0));
            return projectVolume;
        }
	}

}
