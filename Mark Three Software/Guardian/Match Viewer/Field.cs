namespace MarkThree.Guardian.Forms
{

	using MarkThree.Forms;
	using System;

	[Flags]
	internal enum Field
	{
		LastPrice, BidPrice, AskPrice, LastSize, BidSize, AskSize, Status, Blotter, Security, StartTime, StopTime,
		MaximumVolatility, MinimumQuantity, NewsFreeTime, SubmittedQuantity, OrderTypeCode, Match, Volume, InterpolatedVolume,
        VolumeWeightedAveragePrice};

}
