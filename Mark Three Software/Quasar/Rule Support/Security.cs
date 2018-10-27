namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Something that can be traded for another security.
	/// </summary>
	/// <remarks>This is a remark for the security</remarks>
	public class Security : Shadows.Quasar.Rule.Object
	{

		private int securityTypeCode;
		private int settlementId;
		private decimal quantityFactor;
		private decimal priceFactor;
		private string symbol;

		/// <summary>The security identifier.</summary>
		public int SecurityId {get {return this.ObjectId;}}
		/// <summary>The default settlement security.</summary>
		public int SettlementId {get {return this.settlementId;}}
		/// <summary>Gets the security type.</summary>
		public int SecurityTypeCode {get {return this.securityTypeCode;}}
		/// <summary>Gets the factor used to translate from quoted units to actual units.</summary>
		public decimal QuantityFactor {get {return this.quantityFactor;}}
		/// <summary>Gets the factor used to translate from quoted prices to actual prices.</summary>
		public decimal PriceFactor {get {return this.priceFactor;}}
		/// <summary>Gets the symbol most commonly used to identify the security.</summary>
		public string Symbol {get {return this.symbol;}}

		internal Security() {}
			
		public static Security Make(int securityId)
		{

			Security security = new Security();
			security.Initialize(securityId);
			return security;

		}
		
		internal static Security Make(ClientMarketData.SecurityRow securityRow)
		{

			Security security = new Security();
			security.Initialize(securityRow);
			return security;

		}
			
		/// <summary>
		/// Creates a security using the external identifier.
		/// </summary>
		/// <param name="securityId">The security identifier.</param>
		public static Security Find(string securityId)
		{

			// Initialize the object using the default configuration and the external security identifier.
			return Find("DEFAULT", securityId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="securityId">The security identifier.</param>
		public static Security Find(string configurationId, string securityId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal security identifier.
				int internalSecurityId = Rule.Object.FindKey(configurationId, "securityId", securityId);
				if (internalSecurityId == Identifier.NotFound)
					throw new Exception(String.Format("Security {0} doesn't exist", securityId));

				// Initialize the security using the internal identifier.
				return Make(internalSecurityId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Security.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="securityId">The security identifier.</param>
		/// <returns>A security record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int securityId)
		{

			// Use the specified configuration to find the internal security identifier.
			ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(securityId);
			if (securityRow == null)
				throw new Exception(String.Format("Security {0} doesn't exist", securityId));

			Initialize(securityRow);

		}

		protected void Initialize(ClientMarketData.SecurityRow securityRow)
		{

			// Initialize the base class.
			base.Initialize(securityRow.ObjectRow);

			// Find the default settlement security for equities.
			if (securityRow.SecurityTypeCode == Common.SecurityType.Debt)
				foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
					this.settlementId = debtRow.SettlementId;

			// Find the default settlement security for equities.
			if (securityRow.SecurityTypeCode == Common.SecurityType.Equity)
				foreach (ClientMarketData.EquityRow equityRow in securityRow.GetEquityRowsByFKSecurityEquityEquityId())
					this.settlementId = equityRow.SettlementId;

			// Initialize the record from the data model.
			this.securityTypeCode = securityRow.SecurityTypeCode;
			this.quantityFactor = securityRow.QuantityFactor;
			this.priceFactor = securityRow.PriceFactor;
			this.symbol = securityRow.Symbol;

		}

	}

}
