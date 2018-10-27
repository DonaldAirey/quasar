/*************************************************************************************************************************
*
*	File:			SecurityType.cs
*	Description:	Security Types
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Used to classify and handle securities.
	/// </summary>
	public enum SecurityType
	{
		/// <summary>Currency</summary>
		Currency = 0,
		/// <summary>Stocks, Preferred Stocks, Warrants, etc.</summary>
		Equity = 1,
		/// <summary>Debt, Bills, Notes, etc.</summary>
		Debt = 2,
		/// <summary>Managed group of securities traded as a single unit</summary>
		Fund = 3,
		/// <summary>Annuities</summary>
		Annuity = 4,
		/// <summary>Futures, forward contracts, options, etc.</summary>
		Derivative = 5
	};

}
