namespace MarkThree.Guardian.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

    internal struct TraderOptions
	{

		public object NewsFreeTime;
		public object MaximumVolatility;
		public object StartTime;
		public object StopTime;
		public bool IsBrokerMatch;
		public bool IsHedgeMatch;
		public bool IsInstitutionMatch;

        public int MarketSleep;
        public int ReviewWindow;
        
        // high ADV bucket
        public decimal ThresholdHigh;
        public decimal AutoExecuteHigh;

        // mid ADV bucket
        public decimal ThresholdMid;
        public decimal AutoExecuteMid;

        // low ADV bucket
        public decimal ThresholdLow;
        public decimal AutoExecuteLow;
	}

    internal struct VolumeCategory
    {
        public string Description;
        public string Mnemonic;
        public int MinRange;
        public int MaxRange;
        public int VolumeCategoryId;
    }

}
