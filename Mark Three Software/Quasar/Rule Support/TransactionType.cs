using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Types of transactions.
	/// </summary>
	public enum TransactionType 
	{
		/// <summary>Deposit into the account.</summary>
		Deposit = 0,
		/// <summary>Withdrawl from the account.</summary>
		Withdraw = 1,
		/// <summary>Buy a security.</summary>
		Buy = 2,
		/// <summary>Sell a security.</summary>
		Sell = 3,
		/// <summary>Buy to cover a short position.</summary>
		BuyCover = 4,
		/// <summary>Sell a borrowed security.</summary>
		SellShort = 5
	}

}
