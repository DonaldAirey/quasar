namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Price.
	/// </summary>
	public class Price
	{

		/// <summary>The last execution price.</summary>
		private decimal lastPrice;
		/// <summary>The size of the last execution.</summary>
		private decimal lastSize;
		/// <summary>The current ask price.</summary>
		private decimal askPrice;
		/// <summary>Number of units at the ask price.</summary>
		private decimal askSize;
		/// <summary>The current bid price.</summary>
		private decimal bidPrice;
		/// <summary>Number of units at the bid price.</summary>
		private decimal bidSize;

		/// <summary>Gets the price of the last execution.</summary>
		public decimal LastPrice {get {return this.lastPrice;}}
		/// <summary>Gets the size of the last execution.</summary>
		public decimal LastSize {get {return this.lastSize;}}
		/// <summary>Gets the current ask price.</summary>
		public decimal AskPrice {get {return this.askPrice;}}
		/// <summary>Gets the size of the current ask.</summary>
		public decimal AskSize {get {return this.askSize;}}
		/// <summary>Gets the current bid price.</summary>
		public decimal BidPrice {get {return this.bidPrice;}}
		/// <summary>Gets the size of the current bid.</summary>
		public decimal BidSize {get {return this.bidSize;}}

		/// <summary>
		/// Finds the price of a security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="currency">The underlying currency for the price.</param>
		/// <returns>The price of the security in the specified currency.</returns>
		public Price(Security security, Security currency)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Look up the price record.
				ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityIdCurrencyId(security.SecurityId, currency.SecurityId);

				// If we have a record for the price/currency combination, then update it with the new price.
				if (priceRow != null)
				{

					// Initialize the record.
					this.lastPrice = priceRow.LastPrice;
					this.lastSize = priceRow.LastSize;
					this.askPrice = priceRow.AskPrice;
					this.askSize = priceRow.AskSize;
					this.bidPrice = priceRow.BidPrice;
					this.bidSize = priceRow.BidSize;

				}
				else
				{

					// Initialize the record.
					this.lastPrice = 0.0M;
					this.lastSize = 0.0M;
					this.askPrice = 0.0M;
					this.askSize = 0.0M;
					this.bidPrice = 0.0M;
					this.bidSize = 0.0M;

				}

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

	}

}
