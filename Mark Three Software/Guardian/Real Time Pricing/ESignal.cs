
namespace MarkThree.Guardian.Server
{
    using MarkThree;
    using MarkThree.Guardian.Server;
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;


    public partial class ESignal : Form
    {
        private ArrayList listSymbols = new ArrayList();

        // event for DM connect/disconnect
        public delegate void DMConnectHandler(object wrapper, ConnectEventArgs args);
        public event DMConnectHandler OnConnectChangeDM;

        // event for symbol update
        public delegate void DMUpdateSymbolHandler(object wrapper, DMUpdateSymbolArgs args);
        public event DMUpdateSymbolHandler OnUpdateSymbolDM;

        public ESignal()
        {
            InitializeComponent();
        }

        private void ESignal_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        // when the eSignal object is closing (ie, the form hosting the ActiveX control is closing),
        // then we have to remove all of the symbols from the eSignal Data Manager 
        private void ESignal_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveAllRealTimeSymbols();
        }
      
        /// <summary>
        /// Opens a connection to the local eSignal Data Manager
        /// </summary>
        /// <param name="username">eSignal username</param>
        /// <param name="password">eSignal password</param>
        public void Connect(string username, string password)
        {
            // set the app identifiers for the eSignal control
            this.dataManager.SetAppIdentifiers("C# Sample", 1, 10, 12, 2005, "eSig");

            // connect to the local data manager
            int retCode = this.dataManager.OpenConnection("localhost", username, password);

            
            if (retCode != 0)
                throw new Exception("Error: Unable to connect to the local eSignal Data Manager.  Code=" + retCode);
        }

        /// <summary>
        /// Disconnect from the local eSignal Data Manager.
        /// </summary>
        public void Disconnect()
        {
            this.dataManager.CloseConnection();
        }
                    

        /*
         * Operations - Data manager
         *  A bunch of operations that can be called on the Data Manager ActiveX
         *  control.  The interaction with the eSignal servers is done through the
         *  ActiveX controls.  For requests, a method is called on the control
         *  (ie, AdviseSymbol).  For responses, an event is fired by the control.
         */

        /// <summary>
        /// Adds a symbol to the eSignal Data Manager for real-time updates.  The
        /// AdviseSymbol() method will register for real-time update events.
        /// </summary>
        /// <param name="symbol">the symbol to be added, ie "MSFT"</param>
        /// <returns>true if successful, false otherwise</returns>
		public bool AddRealTimeSymbol(string symbol)
		{
            // check if the symbol is valid, and add the symbol to the data manager
            if (dataManager.IsValidSymbol(symbol)==1 && dataManager.AdviseSymbol(symbol)==0) 
            {
                // rememeber the symbol so we remember when we close.
				listSymbols.Add(symbol);
                //Debug.WriteLine("Total symbols: " + listSymbols.Count);
			    return true;
		    }

			return false;
		}

        /// <summary>
        /// Removes all of the symbols that have been added to the eSignal Data manager.
        /// </summary>
        public void RemoveAllRealTimeSymbols()
        {
            foreach (string symbol in listSymbols)
            {
                // TODO: do we care if we can/can't remove the symbol?
                this.RemoveRealTimeSymbol(symbol);
                //Debug.WriteLine("Removing symbol: " + symbol);
            }

            // empty out our list of added symbols
            listSymbols.Clear();
        }

        public bool AreSymbolsAdded
        {
            get { return listSymbols.Count > 0; }
        }

        public int SymbolCount
        {
            get { return listSymbols.Count; }
        }
        
        /// <summary>
        /// Removes a symbol from the eSignal Data Manager for real-time updates.
        /// </summary>
        /// <param name="symbol">the symbol to be removed, ie "MSFT"</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool RemoveRealTimeSymbol(string symbol)
        {
            return (dataManager.DeleteSymbol(symbol)==0);
        }

        /// <summary>
        /// Returns true if the symbol is an international symbol (what does this mean?)
        /// </summary>
        /// <param name="symbol">the symbol to be checked</param>
        /// <returns>true if international, false otherwise</returns>
        public bool IsInternationalDMSymbol(string symbol)
        {
            return (dataManager.IsInternationalSymbol(symbol) == 1);
        }

        private void DMConnected(object sender, EventArgs e)
        {
            ConnectEventArgs connectArgs = new ConnectEventArgs(true);
            if (OnConnectChangeDM != null)
                OnConnectChangeDM(this, connectArgs);
        }


        private void DMDisconnected(object sender, AxDBCCTRLLib._IDataManagerEvents_DisconnectedEvent e)
        {
            ConnectEventArgs connectArgs = new ConnectEventArgs(false);

            if (OnConnectChangeDM != null)
                OnConnectChangeDM(this, connectArgs);
        }

        private void DMUpdateInternationalLong(object sender, AxDBCCTRLLib._IDataManagerEvents_UpdateInternationalLongEvent e)
        {
            //Debug.WriteLine("UPDATE SYMBOL!");
            // build a StockInfo object for the update event
            StockInfo stockInfo = new StockInfo();

            // symbol
            stockInfo.Symbol = e.symbol;

            // TODO: Do we care about category?  e.il.cat? 

            // time
            if (e.il.MonthUpdate != null)
            {
                string t = e.il.MonthUpdate.ToString() + "/" + e.il.DayUpdate.ToString() + "/" +
                    e.il.YearUpdate.ToString() + " " + Convert.ToSByte(e.il.HourUpdate).ToString() + ":" +
                    e.il.MinuteUpdate.ToString() + ":" + e.il.SecondUpdate.ToString();
                DateTime tDate = Convert.ToDateTime(t);
                stockInfo.LastTime = DataManager.UTCToLocalTime(tDate);
            }

            if (e.il.Last != null)
                stockInfo.LastPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Last), Convert.ToInt16(e.il.Base));

            if (e.il.Bid != null)
                stockInfo.BidPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Bid), Convert.ToInt16(e.il.Base));

            if (e.il.BidSize != null)
                stockInfo.BidSize = Convert.ToUInt32(e.il.BidSize);

            if (e.il.BidExg != null)
                stockInfo.BidExchange = e.il.BidExg.ToString();

            if (e.il.Ask != null)
                stockInfo.AskPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Ask), Convert.ToInt16(e.il.Base));

            if (e.il.AskSize != null)
                stockInfo.AskSize = Convert.ToUInt32(e.il.AskSize);

            if (e.il.AskExg != null)
                stockInfo.AskExchange = e.il.AskExg.ToString();

            if (e.il.High != null)
                stockInfo.HighPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.High), Convert.ToInt16(e.il.Base));

            if (e.il.Low != null)
                stockInfo.LowPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Low), Convert.ToInt16(e.il.Base));

            if (e.il.Open != null)
                stockInfo.OpenPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Open), Convert.ToInt16(e.il.Base));

            if (e.il.Prev != null)
                stockInfo.PreviousPrice = dataManager.BLToDouble(Convert.ToInt32(e.il.Prev), Convert.ToInt16(e.il.Base));

            if (e.il.TradeVol != null)
                stockInfo.TradeVolume = Convert.ToUInt32(e.il.TradeVol.ToString());

            if (e.il.TradeExg != null)
                stockInfo.TradeExchange = e.il.TradeExg.ToString();

            if (e.il.TotalVol != null)
                stockInfo.TotalVolume = Convert.ToUInt32(e.il.TotalVol);

            // call our wrapped event handler
            DMUpdateSymbolArgs args = new DMUpdateSymbolArgs(stockInfo);
            if (OnUpdateSymbolDM != null)
                OnUpdateSymbolDM(this, args);
                        
        }


    }

    // event args class for Connect/Disconnect events
    public class ConnectEventArgs : EventArgs
    {
        public readonly bool connected;

        public ConnectEventArgs(bool connected)
        {
            this.connected = connected;
        }
    }

    // event args class for real-time symbol updates
    public class DMUpdateSymbolArgs : EventArgs
    {
        public readonly StockInfo stockInfo;

        public DMUpdateSymbolArgs(StockInfo stockInfo)
        {
            this.stockInfo = stockInfo;
        }
    }
    
}