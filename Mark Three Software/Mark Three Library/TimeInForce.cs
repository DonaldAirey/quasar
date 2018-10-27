namespace MarkThree
{

	using System;

	/// <summary>
	/// The time for which an order is valid.
	/// </summary>
	[Serializable()]
	public enum TimeInForce
	{
		Day = 0,
		GoodTillCancel = 1,
		AtTheOpening = 2,
		ImmediateOrCancel = 3,
		FillOrKill = 4,
		GoodTillCrossing = 5,
		GoodTillDate = 6,
		AtTheClose = 7
	}

}
