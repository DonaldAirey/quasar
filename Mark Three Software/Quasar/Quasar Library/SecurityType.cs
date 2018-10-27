/*************************************************************************************************************************
*
*	File:			SecurityType.cs
*	Description:	Security Types
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used to classify and handle securities.
	/// </summary>
	public class SecurityType
	{
		/// <summary>Currency</summary>
		public const int Currency = 0;
		/// <summary>Stocks, Preferred Stocks, Warrants, etc.</summary>
		public const int Equity = 1;
		/// <summary>Debt, Bills, Notes, etc.</summary>
		public const int Debt = 2;
		/// <summary>Managed group of securities traded as a single unit</summary>
		public const int Fund = 3;
		/// <summary>Annuities</summary>
		public const int Annuity = 4;
		/// <summary>Futures, forward contracts, options, etc.</summary>
		public const int Derivative = 5;
	};

}
