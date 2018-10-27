namespace MarkThree
{

	using System;

	/// <summary>
	/// Specifies the type of transaction.
	/// </summary>
	[Serializable()]
	public enum Side
	{
		Buy = 0,
		Sell = 1,
		BuyMinus = 2,
		SellPlus = 3,
		SellShort = 4,
		SellShortExempt = 5,
		Undisclosed = 6,
		Cross = 7,
		CrossShort = 8,
		CrossShortExempt = 9,
		AsDefined = 10,
		Opposite = 11,
		Subscribe = 12,
		Redeem = 13,
		Lend = 14,
		Borrow = 15
	}

}
