

namespace MarkThree.Guardian.Server
{
    using System;
    
    public class StockInfo
    {

        ////////////////////////////////////////////////
        // Private members

        // time, symbol, and last price
        private DateTime timestamp;
        private string symbol;
        private double lastPrice;

        // bid data
        private double  bidPrice;
        private uint   bidSize;
        private string bidExg;

        // ask data
        private double askPrice;
        private uint   askSize;
        private string askExg;

        // previous and open price
        private double openPrice;
        private double prevPrice;

        // high/low price
        private double highPrice;
        private double lowPrice;
        
        // volume data
        private uint   tradeVolume;
        private string tradeExg;
        private uint   totalVolume;

        /////////////////////////////////////////////////
        // Properties for private members

        // symbol properties
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        public DateTime LastTime
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public double LastPrice
        {
            get { return lastPrice; }
            set { lastPrice = value; }
        }

        // bid properties
        public double BidPrice
        {
            get { return bidPrice; }
            set { bidPrice = value; }
        }

        public uint BidSize
        {
            get { return bidSize; }
            set { bidSize = value; }
        }

        public string BidExchange
        {
            get { return bidExg; }
            set { bidExg = value; }
        }

        // ask properties
        public double AskPrice
        {
            get { return askPrice; }
            set { askPrice = value; }
        }

        public uint AskSize
        {
            get { return askSize; }
            set { askSize = value; }
        }

        public string AskExchange
        {
            get { return askExg; }
            set { askExg = value; }
        }

        // price properties
        public double PreviousPrice
        {
            get { return prevPrice; }
            set { prevPrice = value; }
        }

        public double OpenPrice
        {
            get { return openPrice; }
            set { openPrice = value; }
        }

        public double LowPrice
        {
            get { return lowPrice; }
            set { lowPrice = value; }
        }

        public double HighPrice
        {
            get { return highPrice; }
            set { highPrice = value; }
        }

        // volume properties
        public uint TradeVolume
        {
            get { return tradeVolume; }
            set { tradeVolume = value; }
        }

        public uint TotalVolume
        {
            get { return totalVolume; }
            set { totalVolume = value; }
        }

        public string TradeExchange
        {
            get { return tradeExg; }
            set { tradeExg = value; }
        }

        // default constructor
        public StockInfo()
        {
        }

        // useful constructor of data we care about
        public StockInfo(string symbol, DateTime time, double lastPrice, uint tradeVolume,
                         double bidPrice, uint bidSize, double askPrice, uint askSize)
        {
            this.Symbol = symbol;
            this.LastTime = time;
            this.LastPrice = lastPrice;
            this.TradeVolume = tradeVolume;
            this.BidPrice = bidPrice;
            this.BidSize = bidSize;
            this.AskPrice = askPrice;
            this.AskSize = askSize;
        }

        // TODO: make this a real object, not struct
        // TODO: copy constructor
        // TODO: equals
        // 
    }
}
