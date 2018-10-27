namespace MarkThree.Guardian.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	internal struct SourceOrder
	{

		public int OrderTypeCode;
		public decimal Quantity;
		public string SecurityId;
		public string SettlementId;
		public object NewsFreeTime;
		public object MaximumVolatility;
		public object StartTime;
		public object StopTime;
		public bool IsBrokerMatch;
		public bool IsHedgeMatch;
		public bool IsInstitutionMatch;

	}

}
