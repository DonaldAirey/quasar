/*************************************************************************************************************************
*
*	File:			MarketValueFlags
*	Description:	These flags direct the scoping of a market value calculation.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Controls the scoping levels for market value calculations.
	/// </summary>
	[Flags]
	public enum MarketValueFlags
	{
		IncludeNone = 0x00,
		IncludeTaxLot = 0x01,
		IncludeProposedOrder = 0x02,
		IncludeOrder = 0x04,
		IncludeAllocation = 0x08,
		EntirePosition = IncludeTaxLot | IncludeProposedOrder | IncludeOrder,
		ExcludeProposedOrder = IncludeTaxLot | IncludeOrder,
		IncludeChildAccounts= 0x10
	};
	
}
