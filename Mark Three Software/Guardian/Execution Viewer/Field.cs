namespace MarkThree.Guardian.Forms
{

	using MarkThree.Forms;
	using System;

	[Flags]
	internal enum Field
	{
		Blotter, Broker, BrokerAccount, Commission, Destination, DestinationState, Execution, ExecutionPrice, ExecutionQuantity,
		OrderType, PriceType, Security, SourceState, TimeInForce
	};

}
