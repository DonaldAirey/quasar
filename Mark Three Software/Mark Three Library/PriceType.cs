namespace MarkThree
{

	using System;

	/// <summary>
	/// Pricing instructions for the transaction.
	/// </summary>
	[Serializable()]
	public enum PriceType
	{
		Market = 0,
		Limit = 1,
		Stop = 2,
		StopLimit = 3,
		MarketOnClose = 4,
		WithOrWithout = 5,
		LimitOrBetter = 6,
		LimitWithOrWithout = 7,
		OnBasis = 8,
		OnClose = 9,
		LimitOnClose = 10,
		PreviouslyQuoted = 11,
		PreviouslyIndicated = 12,
		ForexLimit = 13,
		ForexSwap = 14,
		ForexPreviouslyIndicated = 15,
		Funarii = 16,
		MarketIfTouched = 17,
		MarketWithLeftoverAsLimit = 18,
		PreviousFundValuationPoint = 19,
		NextFundValuationPoint = 20,
		Pegged = 21
	}

}
