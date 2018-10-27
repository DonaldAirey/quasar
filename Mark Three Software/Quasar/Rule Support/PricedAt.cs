using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Specifies the way an order is priced.
	/// </summary>
	public enum PricedAt
	{
		/// <summary>Executes at the market price.</summary>
		Market = 0,
		/// <summary>Executes at a specified price.</summary>
		Limit = 1,
		/// <summary>Becomes a market order when the specified price is reached.</summary>
		StopLoss = 2,
		/// <summary>Becomes a limit order when the specified price is reached.</summary>
		StopLimit = 3
	};

}
