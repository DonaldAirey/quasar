namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// A collection of positions, orders and allocations.
	/// </summary>
	public class Account : Shadows.Quasar.Rule.Object
	{

		private object mnemonic;
		private AccountType accountType;
		private Security baseCurrency;

		/// <summary>The internal account identifier.</summary>
		public int AccountId {get {return this.ObjectId;}}

		/// <summary>The account classification.</summary>
		public AccountType AccountType {get {return this.accountType;}}

		/// <summary>Gets the base currency of the account.</summary>
		public Security BaseCurrency {get {return this.baseCurrency;}}

		/// <summary>Gets the short name of the account.</summary>
		public string Mnemonic {get {return (string)this.mnemonic;}}

		public bool IsMnemonicNull {get {return this.mnemonic == null;}}

		private Account() {}

		internal static Account Make(int accountId)
		{

			Account account = new Account();
			account.Initialize(accountId);
			return account;

		}

		/// <summary>
		/// Creates a account using the external identifier.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="accountId">The account identifier.</param>
		public static Account Find(string accountId)
		{

			// Initialize the object using the specified configuration and external account identifier.
			return Find("DEFAULT", accountId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="accountId">The account identifier.</param>
		public static Account Find(string configurationId, string accountId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal account identifier.
				int internalAccountId = Rule.Object.FindKey(configurationId, "accountId", accountId);
				if (internalAccountId == Identifier.NotFound)
					throw new Exception(String.Format("Account {0} doesn't exist", accountId));

				// Initialize the account using the internal identifier.
				return Account.Make(internalAccountId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Account.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="accountId">The account identifier.</param>
		/// <returns>A account record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int accountId)
		{

			// Use the specified configuration to find the internal account identifier.
			ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
			if (accountRow == null)
				throw new Exception(String.Format("Account {0} doesn't exist", accountId));

			// Initialize the base class.
			base.Initialize(accountId);

			// Initialize the record from the data model.
			this.accountType = (AccountType)accountRow.AccountTypeCode;
			this.baseCurrency = Security.Make(accountRow.CurrencyId);
			this.mnemonic = accountRow.IsMnemonicNull() ? string.Empty : accountRow.Mnemonic;

		}

		private void Initialize(ClientMarketData.AccountRow accountRow)
		{

			// Initialize the base class.
			base.Initialize(accountRow.ObjectRow);

			// Initialize the record from the data model.
			this.accountType = (AccountType)accountRow.AccountTypeCode;
			this.baseCurrency = Security.Make(accountRow.SecurityRow);
			this.mnemonic = accountRow.Mnemonic;

		}		
	
		public bool Contains(Security security, int positionType)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);

				foreach (ClientMarketData.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
					if (taxLotRow.SecurityId == security.SecurityId && taxLotRow.PositionTypeCode == positionType)
						return true;

				foreach (ClientMarketData.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
					if (proposedOrderRow.SecurityId == security.SecurityId && proposedOrderRow.PositionTypeCode == positionType)
						return true;

				foreach (ClientMarketData.OrderRow orderRow in accountRow.GetOrderRows())
					if (orderRow.SecurityId == security.SecurityId && orderRow.PositionTypeCode == positionType)
						return true;

				foreach (ClientMarketData.AllocationRow allocationRow in accountRow.GetAllocationRows())
					if (allocationRow.SecurityId == security.SecurityId && allocationRow.PositionTypeCode == positionType)
						return true;

				return false;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		public PositionList GetPositions()
		{

			PositionList positionList = new PositionList();

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				
				// Aggregate the tax lot quantities.
				foreach (ClientMarketData.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
					positionList.Add(Position.Make(this.AccountId, taxLotRow.SecurityId, taxLotRow.PositionTypeCode));

				// Aggregate the proposed ordered quantities
				foreach (ClientMarketData.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
					positionList.Add(Position.Make(this.AccountId, proposedOrderRow.SecurityId, proposedOrderRow.PositionTypeCode));

				// Aggregate the ordered quantities
				foreach (ClientMarketData.OrderRow orderRow in accountRow.GetOrderRows())
					positionList.Add(Position.Make(this.AccountId, orderRow.SecurityId, orderRow.PositionTypeCode));

				// Aggregate the allocated quantities
				foreach (ClientMarketData.AllocationRow allocationRow in accountRow.GetAllocationRows())
					positionList.Add(Position.Make(this.AccountId, allocationRow.SecurityId, allocationRow.PositionTypeCode));

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			return positionList;

		}
	
		public PositionList GetPositions(Sector sector)
		{

			PositionList positionList = new PositionList();

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);

				ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(sector.SectorId);
				
				// Aggregate the tax lot quantities.
				foreach (ClientMarketData.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
					if (IsSecurityInSector(sectorRow, taxLotRow.SecurityRow))
						positionList.Add(Position.Make(this.AccountId, taxLotRow.SecurityId, taxLotRow.PositionTypeCode));

				// Aggregate the proposed ordered quantities
				foreach (ClientMarketData.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
					if (IsSecurityInSector(sectorRow, proposedOrderRow.SecurityRowByFKSecurityProposedOrderSecurityId))
						positionList.Add(Position.Make(this.AccountId, proposedOrderRow.SecurityId, proposedOrderRow.PositionTypeCode));

				// Aggregate the ordered quantities
				foreach (ClientMarketData.OrderRow orderRow in accountRow.GetOrderRows())
					if (IsSecurityInSector(sectorRow, orderRow.SecurityRowByFKSecurityOrderSecurityId))
						positionList.Add(Position.Make(this.AccountId, orderRow.SecurityId, orderRow.PositionTypeCode));

				// Aggregate the allocated quantities
				foreach (ClientMarketData.AllocationRow allocationRow in accountRow.GetAllocationRows())
					if (IsSecurityInSector(sectorRow, allocationRow.SecurityRowByFKSecurityAllocationSecurityId))
						positionList.Add(Position.Make(this.AccountId, allocationRow.SecurityId, allocationRow.PositionTypeCode));

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			return positionList;

		}

		/// <summary>
		/// Indicates if a given security is part of the document's security classification scheme.
		/// </summary>
		/// <param name="securityRow">A security to be tested.</param>
		/// <returns>True if the security belongs to the document's classification scheme.</returns>
		/// <remarks>
		/// Table locks needed:
		///		Read:	ObjectTree
		///		Read:	Objects
		/// </remarks>
		private bool IsSecurityInSector(ClientMarketData.SectorRow sectorRow, ClientMarketData.SecurityRow securityRow)
		{

			// Call the generic method to determine the relationship between the sector and the security.
			return Shadows.Quasar.Common.Object.IsParent(sectorRow.ObjectRow, securityRow.ObjectRow);

		}

		public decimal GetMarketValue()
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				return Common.MarketValue.Calculate(accountRow.CurrencyRow, accountRow, MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		public decimal GetMarketValue(Security security)
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(security.SecurityId);

				decimal marketValue = 0.0M;
				foreach (PositionType positionType in Position.GetPositionTypes())
					marketValue += Common.MarketValue.Calculate(accountRow.CurrencyRow, accountRow, securityRow, (int)positionType,
						MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

				return marketValue;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		public decimal GetMarketValue(Security security, int positionType)
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(security.SecurityId);
				return Common.MarketValue.Calculate(accountRow.CurrencyRow, accountRow, securityRow, positionType,
					MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		public decimal GetMarketValue(Sector sector)
		{

			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(this.AccountId);
				ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(sector.SectorId);
				return Common.MarketValue.Calculate(accountRow.CurrencyRow, accountRow, sectorRow,
					MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

	}

}
