using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Collection of helper functions fo constructing order.
	/// </summary>
	public class Helper
	{

		/// <summary>
		/// Translates a major asset type, PositionTypeCode and signed quantity into a buy, sell, buy cover or sell short
		/// to a TransactionTypeCode.
		/// </summary>
		/// <param name="securityType">The major asset category for a security</param>
		/// <param name="PositionTypeCode">The long or short attribute of the holding</param>
		/// <param name="quantity">The relative quantity of the trade</param>
		/// <returns>The proper TransactionTypeCode (buy, sell, buy cover, sell short) for the trade.</returns>
		static public int TransactionTypeCode(int securityType, int PositionTypeCode, decimal quantity)
		{

			// Assume an error if we don't match up the parameters to aCode below.
			int TransactionTypeCode = -1;

			// Different assets have different TransactionTypeCodes.
			switch(securityType)
			{

				case SecurityType.Currency:

					// Translate the quantity into a deposit or withdrawl transaction.
					TransactionTypeCode = (quantity > 0) ? TransactionType.Deposit : TransactionType.Withdraw;
					break;

				default:
	
					// Translate the position and signed quantity into a transaction.
					if (PositionTypeCode == PositionType.Long)
						TransactionTypeCode = (quantity > 0) ? TransactionType.Buy : TransactionType.Sell;
					if (PositionTypeCode == PositionType.Short)
						TransactionTypeCode = (quantity > 0) ? TransactionType.SellShort : TransactionType.BuyCover;
					break;

			}

			// Return the translatedCode.
			return TransactionTypeCode;

		}

	}

}
