
namespace MarkThree.Guardian.Server
{
    using MarkThree;
	using MarkThree.Guardian.Server;
    using MarkThree.Guardian.External;
    using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;
    using System.Windows.Forms;
    
    public class RealTimePricing : System.ComponentModel.Component
    {
        //private static System.Threading.ManualResetEvent pausePrice;
        private static System.ComponentModel.IContainer components;

        // filepath to the XML file to be loaded into a DataSet
        // this file contains the stock symbols for the S&P 500
        //private static string SP500_EQUITY_SYMBOL_FILEPATH = "SP500 Equity.xml";
        private static string symbolFilepath = "";

        // hash: symbol --> securityId 
        private static Hashtable symbolToSecurityIdTable;
      
        // the eSignal wrapper object
        private static ESignal eSignal = null;
        private static DataManager dataManager = null;
   
        // the thread that the eSignal wrapper object will live in
        private static Thread priceThread = null;

        /// This member provides access to the in-memory database.
        private static ServerMarketData serverMarketData = new ServerMarketData();

        /// <summary>
		/// This object provides a real-time prices feed into the server data model
		/// </summary>
		static RealTimePricing()
		{
		}

        public static void OnConnectionChangeDM(object wrapper, ConnectEventArgs args)
        {
            // if we are connected, and we havne't added symbols yet, then add the symbols.
            if (args.connected)
            {
                // Add the symbols to the eSignal data manager
                if (!eSignal.AreSymbolsAdded)
                    LoadSymbols();
            }
            else if (!args.connected)
            {
                // If there are symbols, then remove them from the watch
                if (eSignal.AreSymbolsAdded)
                    eSignal.RemoveAllRealTimeSymbols();
            }
        }

        /// <summary>
        /// Loads a DataSet from the XML file defined by symbolFilepath.  Iterates through each (configurationId, equityId) pair
        /// and adds the ticker symbol to the eSignal Data Manager watch list.  
        /// </summary>
        private static void LoadSymbols()
        {
            try
            {
                // Lock the tables.
                // Acquire the configuration and object locks for the securityId lookup
                Debug.Assert(!ServerMarketData.IsLocked);
                ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);
                ServerMarketData.EquityLock.AcquireReaderLock(Timeout.Infinite);
                ServerMarketData.ObjectLock.AcquireReaderLock(Timeout.Infinite);
                ServerMarketData.ConfigurationLock.AcquireReaderLock(Timeout.Infinite);
               
                // initialize the dataset from the XMl file
                DataSet dataSetFromXML = new DataSet("SP500");
                dataSetFromXML.ReadXml(symbolFilepath, XmlReadMode.InferSchema);
                foreach (DataRow dataRow in dataSetFromXML.Tables["method"].Rows)
                {
                    // extract the configurationId and the externalEquityId from the dataset
                    object configurationId = dataRow["configurationId"];
                    string externalEquityId = (string)dataRow["equityId"];
                    try
                    {

                        // look up the equityId
                        int equityId = (int)Security.FindOptionalKey(configurationId, "equityId", externalEquityId);

                        // get the equity row
                        ServerMarketData.EquityRow equityRow = ServerMarketData.Equity.FindByEquityId(equityId);
                        if (equityRow != null)
                        {

                            // Get the security row from the equity row
                            ServerMarketData.SecurityRow securityRow = equityRow.SecurityRowBySecurityEquityEquityId;

                            // attempt to add the symbol to the eSignal data manager
                            if (eSignal.AddRealTimeSymbol(securityRow.Symbol))
                                symbolToSecurityIdTable[securityRow.Symbol] = securityRow.SecurityId;
                            else
                                MarkThree.EventLog.Warning("Market Data Service has no data for ticker {0}", externalEquityId);
                        }

                    }
                    catch (Exception exception)
                    {

						// Write the error to the log.
                        MarkThree.EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

                    }
                }

				// Mark the time that the real-time updates started.
				MarkThree.EventLog.Information("Real Time Symbols Loaded");
              
            }
            catch (Exception exception)
            {
                eSignal = null;

                // Write the error and stack trace out to the debug listener
                MarkThree.EventLog.Warning(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

            }
            finally
            {
                // Release the global tables.
                //if (ServerMarketData.PriceLock.IsWriterLockHeld) ServerMarketData.PriceLock.ReleaseWriterLock();
                if (ServerMarketData.SecurityLock.IsReaderLockHeld) ServerMarketData.SecurityLock.ReleaseReaderLock();
                if (ServerMarketData.EquityLock.IsReaderLockHeld) ServerMarketData.EquityLock.ReleaseReaderLock();
                if (ServerMarketData.ObjectLock.IsReaderLockHeld) ServerMarketData.ObjectLock.ReleaseReaderLock();
                if (ServerMarketData.ConfigurationLock.IsReaderLockHeld) ServerMarketData.ConfigurationLock.ReleaseReaderLock();
                Debug.Assert(!ServerMarketData.IsLocked);
            }
            

        }

        public static void OnUpdateSymbolDM(object wrapper, DMUpdateSymbolArgs args)
        {
            // StockInfo object holding the new symbol data for this update
            StockInfo stockInfo = args.stockInfo;

            try
            {
                // Lock the tables.
                Debug.Assert(!ServerMarketData.IsLocked);
                ServerMarketData.EquityLock.AcquireReaderLock(Timeout.Infinite);
                ServerMarketData.PriceLock.AcquireWriterLock(Timeout.Infinite);
                ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);

                // make sure the symbol is in the hash table - and that we added it to the data manager
                if (!symbolToSecurityIdTable.ContainsKey(stockInfo.Symbol))
                    throw new Exception("Received update for unknown symbol: " + stockInfo.Symbol);

                // get the security id of the symbol
                int securityID = (int)symbolToSecurityIdTable[stockInfo.Symbol];
               
                // get the security row
                ServerMarketData.SecurityRow securityRow = ServerMarketData.Security.FindBySecurityId(securityID);
                if (securityRow == null)
                    throw new Exception("Warning: Could not find security row for symbol: " + stockInfo.Symbol);

                //int equityId = Security.FindRequiredKey(configurationId, "equityId", externalEquityId);
                // Only Equities are priced by this simulator.  All others are ignored.
                ServerMarketData.EquityRow equityRow = ServerMarketData.Equity.FindByEquityId(securityRow.SecurityId);
                if (equityRow != null)
                {

                    // Find a price that matches the equity's default settlement.  This is the price record that will be updated
                    // with the simulated market conditions.
                    ServerMarketData.PriceRow priceRow = ServerMarketData.Price.FindBySecurityId(securityRow.SecurityId);
                    if (priceRow == null)
                    {
                        priceRow = ServerMarketData.Price.NewPriceRow();
                        //priceRow.RowVersion = ServerMarketData.RowVersion.Increment();
                        priceRow.SecurityId = equityRow.EquityId;
                        priceRow.CurrencyId = equityRow.SettlementId;
                        priceRow.LastPrice = 0.0M;
                        priceRow.AskPrice = 0.0M;
                        priceRow.BidPrice = 0.0M;
                        priceRow.Volume = 0.0M;
                        priceRow.ClosePrice = 0.0M;
                        priceRow.VolumeWeightedAveragePrice = 0.0M;
                        priceRow.HighPrice = 0.0M;
                        priceRow.LowPrice = 0.0M;
                        ServerMarketData.Price.AddPriceRow(priceRow);
                    }

                    // set the new values from the real time event into the price row
                    priceRow.BidSize = stockInfo.BidSize;
                    priceRow.BidPrice = Convert.ToDecimal(stockInfo.BidPrice);
                    priceRow.AskPrice = Convert.ToDecimal(stockInfo.AskPrice);
                    priceRow.AskSize = Convert.ToDecimal(stockInfo.AskSize);
                    priceRow.LastPrice = Convert.ToDecimal(stockInfo.LastPrice);
                    priceRow.LastSize = stockInfo.TradeVolume;
                    priceRow.Volume = Convert.ToDecimal(stockInfo.TotalVolume);
                    priceRow.VolumeWeightedAveragePrice = (Convert.ToDecimal(stockInfo.LastPrice) + Convert.ToDecimal(stockInfo.OpenPrice)) / 2;
                    priceRow.OpenPrice = Convert.ToDecimal(stockInfo.OpenPrice);
                    priceRow.LowPrice = Convert.ToDecimal(stockInfo.LowPrice);
                    priceRow.HighPrice = Convert.ToDecimal(stockInfo.HighPrice);
                    priceRow.ClosePrice = Convert.ToDecimal(stockInfo.PreviousPrice);

                    // increment the RowVersion so the client notices!!!
                    priceRow.RowVersion = ServerMarketData.RowVersion.Increment();

                    // commit the changes
                    priceRow.AcceptChanges();
                }
            }
            catch (Exception exception)
            {
                String msg = String.Format("{0}, {1}", exception.Message, exception.StackTrace);
                
                // Write the error and stack trace out to the debug listener
                //Debug.WriteLine(msg);
                MarkThree.EventLog.Warning(msg);

            }
            finally
            {

                // Release the global tables.
                if (ServerMarketData.EquityLock.IsReaderLockHeld) ServerMarketData.EquityLock.ReleaseReaderLock();
                if (ServerMarketData.PriceLock.IsWriterLockHeld) ServerMarketData.PriceLock.ReleaseWriterLock();
                if (ServerMarketData.SecurityLock.IsReaderLockHeld) ServerMarketData.SecurityLock.ReleaseReaderLock();
                Debug.Assert(!ServerMarketData.IsLocked);
            }
        }


        /// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Start(AdoTransaction adoTransaction)
		{
			
		}

        /// <summary>
		/// Starts a simulated price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Start(ParameterList parameters)
		{
            MarkThree.EventLog.Information("Starting Real Time Price service.");
            //System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //Debug.WriteLine("Name=" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            //Debug.WriteLine("ToString=" + System.Security.Principal.WindowsIdentity.GetCurrent().ToString());
            // extract the parameter for the path to the file containing the symbols to be loaded
            symbolFilepath = parameters["symbolFilepath"];
            if (symbolFilepath.Length == 0)
            {
                MarkThree.EventLog.Error("Real Time Pricing error: You must pass in a symbolFilepath parameter (path to the symbol list file) for this call.");
                return;
            }

            // check if the real time price thread is already running
            if (priceThread != null)
            {
                MarkThree.EventLog.Information("Real Time Price service already started!!");
                return;
            }

            // Install the Server Market Data in this container.
            RealTimePricing.components = new System.ComponentModel.Container();
            RealTimePricing.symbolToSecurityIdTable = new Hashtable();

            // create the local eSignal data manager - this object will start/stop the datamanager
            dataManager = new DataManager();
            if (!dataManager.Init())
            {
                MarkThree.EventLog.Error("Unable to initialize the eSignal data manager.  Is eSignal installed?");
                throw new Exception("Unable to initialize the eSignal data manager.  Is eSignal installed?");
            }

            // start the local eSignal data manager.
            dataManager.Start();

            // create and start the thread that will the eSignal dialog runs in
            priceThread= new Thread(new ThreadStart(RealTimePriceThread));
            priceThread.Name = "esignal";
            priceThread.TrySetApartmentState(ApartmentState.STA);
            priceThread.Start();
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
            MarkThree.EventLog.Information("Stopping Real Time Price service.");

            // stop the local eSignal data manager (if we started it)
            dataManager.Stop();

            // if we have a running thread, abort it
            if (priceThread != null)
            {
                // disconnect from the local eSignal data manager
                if (eSignal != null)
                    eSignal.Disconnect();

                // exit the application - this will stop the thread blocked at
                // Application.Run() from RealTimePricing.Start()
                Application.Exit();
                MarkThree.EventLog.Information("Real Time Price service stopped.");
            }
            else
            {
                MarkThree.EventLog.Information("Real Time Price can NOT stop because the service is not running.");
            }

            // nullify the eSignal and thread objects... so it can be started again     
            symbolToSecurityIdTable.Clear(); // empty the symbol list mapping
            eSignal = null;
            priceThread = null;
		}


        /// <summary>
        /// Thread to hold the dialog that hosts the eSignal ActiveX control
        /// </summary>
        private static void RealTimePriceThread()
        {
            if (eSignal != null)
            {
                MarkThree.EventLog.Information("Real Time Price service is already started... ignoring Start request.");
                return;
            }

            // create an instance of eSignal and set delegates for events
            eSignal = new ESignal();
            eSignal.OnConnectChangeDM += new ESignal.DMConnectHandler(RealTimePricing.OnConnectionChangeDM);
            eSignal.OnUpdateSymbolDM += new ESignal.DMUpdateSymbolHandler(RealTimePricing.OnUpdateSymbolDM);
            
            // connect to the local eSignal data manager
            // If connect succeeds, then we add the symbols to the data manager from the connect
            // handler.
            eSignal.Connect(dataManager.Username, dataManager.Password);

            // by this point, we've loaded all of the symbols into the data manager.
            // if we have an esignal, then we run the form to allow the event loop to start
            if (eSignal != null)
            {
                // NOTE: I don't think this does anything, because the application
                // will display the form in the run method
                eSignal.Visible = false;

                // RealTimePricing will now just wait for connect/update events.
                // *** the thread blocks here
                Application.Run(eSignal);
            }

        }

    }
}
