/*************************************************************************************************************************
*
*	File:			OrderType.cs
*	Description:	Order Types
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Defines how the order is to be priced.
	/// </summary>
	public class OrderType
	{
		/// <summary>Order is executed at the current market price</summary>
		public const int Market = 0;
		/// <summary>Order is executed only at the specified price or better</summary>
		public const int Limit = 1;
		/// <summary>Order becomes a market order when it falls below the specified price</summary>
		public const int StopLoss = 2;
		/// <summary>Order becomes a limit order when it falls below the specified price</summary>
		public const int StopLimit = 3;
	}

}
