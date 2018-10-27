namespace MarkThree.Guardian.Forms
{

	using MarkThree.Forms;
	using System;

	[Flags]
	internal enum Field
	{
		Allocation, AskPrice, AskSize, BidPrice, BidSize, Blotter, CanceledQuantity, Destination, DestinationOrder,
		ExecutedQuantity, IsHeld, IsSubmitted, LastSize, LastPrice, OrderType, PriceType, OrderedQuantity,
		Security, Status, TimeInForce
	};

}
