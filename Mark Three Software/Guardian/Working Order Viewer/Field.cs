namespace MarkThree.Guardian.Forms
{

	using MarkThree.Forms;
	using System;

	[Flags]
	internal enum Field
	{
		LastPrice,
		BidPrice,
		AskPrice,
		LastSize,
		BidSize,
		AskSize,
		Status,
		Blotter,
		OrderType,
		TimeInForce,
		Timer,
		Security,
		SourceOrder,
		DestinationOrder,
		Execution,
		Allocation,
		StartTime,
		StopTime,
		MaximumVolatility,
		MinimumQuantity,
		SubmittedQuantity,
		NewsFreeTime,
		IsBrokerMatch,
		IsInstitutionMatch,
		IsHedgeMatch,
		SubmissionTypeCode,
		WorkingOrder,
        Volume,
        InterpolatedVolume,
        VolumeWeightedAveragePrice,
        AutoExecute,
        LimitPrice
	};

}
