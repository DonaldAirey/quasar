namespace MarkThree.Quasar
{

	using MarkThree;
	using MarkThree.Quasar;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Calculates the price of a security.
	/// </summary>
	public class Price
	{

		/// <summary>
		/// Calculates the price of a security.
		/// </summary>
		/// <param name="baseCurrency">The demonimation of the price.</param>
		/// <param name="securityRow">The security to be priced.</param>
		/// <returns>The price of the security in the requested denomination.</returns>
		public static decimal Security(DataModel.CurrencyRow baseCurrency, DataModel.SecurityRow securityRow)
		{

			// Calculate the price of a Currency.
			foreach (DataModel.CurrencyRow currencyRow in securityRow.GetCurrencyRows())
				return Price.Currency(baseCurrency, currencyRow);

			// Calculate the price of a Debt.
			foreach (DataModel.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
				return Price.Debt(baseCurrency, debtRow);

			// Calculate the price of an Equity.
			foreach (DataModel.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
				return Price.Equity(baseCurrency, equityRow);

			// If the security isn't one of the major securityType, then it can't be priced.
			return 0.0M;

		}

		/// <summary>
		/// Calculates the price of a debt issue.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the requested price.</param>
		/// <param name="debtId">The identifier of a debt.</param>
		/// <returns>The price of the debt in the base currency</returns>
		public static decimal Debt(DataModel.CurrencyRow baseCurrency, DataModel.DebtRow debtRow)
		{

			// Look up the price in the security's native currency.
			DataModel.PriceRow securityPriceRow = DataModel.Price.FindBySecurityIdCurrencyId(
				debtRow.DebtId, debtRow.SettlementId);
			if (securityPriceRow == null)
				return 0.0M;

			// Look up the cross currency value.  This is needed to return the value in the requested base currency.
			DataModel.PriceRow crossPriceRow = DataModel.Price.FindBySecurityIdCurrencyId(
				debtRow.SettlementId, baseCurrency.CurrencyId);
			if (crossPriceRow == null)
				return 0.0M;

			// Use the User Preferences to determine which price (Last, Closing) we should use.
			decimal securityPrice = 0.0M;
			decimal crossPrice = 0.0M;
			if (Preferences.Pricing == Pricing.Last)
			{
				securityPrice = securityPriceRow.LastPrice;
				crossPrice = crossPriceRow.LastPrice;
			}
			if (Preferences.Pricing == Pricing.Close)
			{
				securityPrice = securityPriceRow.ClosePrice;
				crossPrice = crossPriceRow.ClosePrice;
			}
			
			// The price, in the requested currency, is the native price multiplied by the cross currency price and any factors.
			return securityPrice * crossPrice;

		}

		/// <summary>
		/// Calculates the price of one currency in another base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the requested price.</param>
		/// <param name="CurrencyId">The identifier of the currency.</param>
		/// <returns>The price of the currency in the base currency</returns>
		public static decimal Currency(DataModel.CurrencyRow baseCurrency, DataModel.CurrencyRow currencyRow)
		{

			// Look up the price in the security's native currency.
			DataModel.PriceRow securityPriceRow =
				DataModel.Price.FindBySecurityIdCurrencyId(currencyRow.CurrencyId, baseCurrency.CurrencyId);
			if (securityPriceRow == null)
				return 0.0M;

			// Use the User Preferences to determine which price (Last, Closing) we should use.
			decimal securityPrice = 0.0M;
			if (Preferences.Pricing == Pricing.Last)
				securityPrice = securityPriceRow.LastPrice;
			if (Preferences.Pricing == Pricing.Close)
				securityPrice = securityPriceRow.ClosePrice;

			// The price, in the requested currency, is the native price multiplied by the cross currency price.
			return securityPrice;

		}
		
		/// <summary>
		/// Calculates the price of an equity in the base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the requested price.</param>
		/// <param name="equityId">The identifier of the equity.</param>
		/// <returns>The price of the equity in the base currency</returns>
		public static decimal Equity(DataModel.CurrencyRow baseCurrency, DataModel.EquityRow equityRow)
		{

			// Look up the price in the security's native currency.
			DataModel.PriceRow securityPriceRow = DataModel.Price.FindBySecurityIdCurrencyId(
				equityRow.EquityId, equityRow.SettlementId);
			if (securityPriceRow == null)
				return 0.0M;

			// Look up the cross currency value.  This is needed to return the value in the requested base currency.
			DataModel.PriceRow crossPriceRow = DataModel.Price.FindBySecurityIdCurrencyId(
				equityRow.SettlementId, baseCurrency.CurrencyId);
			if (crossPriceRow == null)
				return 0.0M;

			// Use the User Preferences to determine which price (Last, Closing) we should use.
			decimal securityPrice = 0.0M;
			decimal crossPrice = 0.0M;
			if (Preferences.Pricing == Pricing.Last)
			{
				securityPrice = securityPriceRow.LastPrice;
				crossPrice = crossPriceRow.LastPrice;
			}
			if (Preferences.Pricing == Pricing.Close)
			{
				securityPrice = securityPriceRow.ClosePrice;
				crossPrice = crossPriceRow.ClosePrice;
			}

			// The price, in the requested currency, is the native price multiplied by the cross currency price.
			return securityPrice * crossPrice;

		}

	}

}
