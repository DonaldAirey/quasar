namespace MarkThree.Quasar
{

	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public class Security
	{

		public static int GetDefaultSettlementId(int SecurityId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!DataModel.IsLocked);
				DataModel.DebtLock.AcquireReaderLock(Timeout.Infinite);
				DataModel.EquityLock.AcquireReaderLock(Timeout.Infinite);
				DataModel.SecurityLock.AcquireReaderLock(Timeout.Infinite);

				// Make sure the security exists.  We need it to find the default settlement security.
				DataModel.SecurityRow securityRow = DataModel.Security.FindBySecurityId(SecurityId);
				if (securityRow == null)
					throw new Exception(String.Format("Security {0} doesn't exist", SecurityId));

				// This version of the method assumes that the locks have been aquired.
				return GetDefaultSettlementId(securityRow);
				
			}
			finally
			{

				// Release the table locks.
				if (DataModel.DebtLock.IsReaderLockHeld) DataModel.DebtLock.ReleaseReaderLock();
				if (DataModel.EquityLock.IsReaderLockHeld) DataModel.EquityLock.ReleaseReaderLock();
				if (DataModel.SecurityLock.IsReaderLockHeld) DataModel.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!DataModel.IsLocked);

			}

		}

		public static int GetDefaultSettlementId(DataModel.SecurityRow securityRow)
		{

			// A settlement price is required to find the price of a security.  For many issues, the default can be found from
			// the defaults.  Others, such as currency transaction and exotic deals require an explicit settlement currency to
			// determine the price.
			foreach (DataModel.CurrencyRow currencyRow in securityRow.GetCurrencyRows())
				return currencyRow.CurrencyId;
			foreach (DataModel.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
				return debtRow.SettlementId;
			foreach (DataModel.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
				return equityRow.SettlementId;

			// Failure to find a settlement currency is a serious matter.
			throw new Exception(String.Format("Security {0} has no settlement currecy.", securityRow.SecurityId));
				
		}

		/// <summary>
		/// Finds a object identifier based on a configuration and an external identifier.
		/// </summary>
		/// <param name="configurationId">Defines the 'User Id' column for the external identifier.</param>
		/// <param name="externalId">An external identifier.</param>
		/// <returns>The internal identifier for the object, 'NotFound' if the externalId isn't found.</returns>
		public static int FindSecurityId(int configurationId, string externalId)
		{

			// This indicates that the identifier wasn't found.
			return int.MinValue;

		}

	}

}