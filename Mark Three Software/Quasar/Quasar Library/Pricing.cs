/*************************************************************************************************************************
*
*	File:			Pricing.cs
*	Description:	Enum for specifying which price should be used.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used to specify which price to use in a calculation or display.
	/// </summary>
	public enum Pricing
	{

		/// <summary>
		/// Use the real-time price.
		/// </summary>
		Last,
			
		/// <summary>
		/// Use last night's closing price.
		/// </summary>
		Close,

		/// <summary>
		/// Use the price at the start of the month.
		/// </summary>
		StartOfMonth,

		/// <summary>
		/// Use the price at the start of the year.
		/// </summary>
		StartOfYear
		
	};
}
