namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Data;
	
	public class PositionTable : DataTable, IEnumerator
	{

		private DataColumn accountIdColumn;
		private DataColumn securityIdColumn;
		private DataColumn positionTypeCodeColumn;
		protected IEnumerator iEnumerator;

		public PositionTable()
		{

			Initialize();
		
		}
	
		public PositionTable(AccountList accountList, SecurityList securityList)
		{

			Initialize();
			
			foreach (Account account in accountList)
				foreach (Security security in securityList)
					foreach (int positionTypeCode in Position.GetPositionTypes())
					{
						DataRow dataRow = this.NewRow();
						dataRow[this.accountIdColumn] = account.AccountId;
						dataRow[this.securityIdColumn] = security.SecurityId;
						dataRow[this.positionTypeCodeColumn] = positionTypeCode;
						this.Rows.Add(dataRow);
					}

		}

		private void Initialize()
		{

			this.accountIdColumn = this.Columns.Add("AccountId", typeof(int));
			this.securityIdColumn = this.Columns.Add("SecurityId", typeof(int));
			this.positionTypeCodeColumn = this.Columns.Add("PositionTypeCode", typeof(int));

			this.PrimaryKey = new DataColumn[] {this.accountIdColumn, this.securityIdColumn, this.positionTypeCodeColumn};

		}

		public int Count {get {return this.Rows.Count;}}

		public bool Contains(Position position)
		{

			return this.Rows.Find(new object[] {position.Account.AccountId, position.Security.SecurityId, (int)position.PositionType})
				== null ? false : true;

		}

		/// <summary>
		/// Gets the enumerator for this object.
		/// </summary>
		/// <returns>An interface to the enumeration classes.</returns>
		public IEnumerator GetEnumerator() {this.iEnumerator = this.Rows.GetEnumerator(); return this;}

		/// <summary>
		/// Gets the current object in the list.
		/// </summary>
		public virtual object Current
		{
			get
			{

				try
				{

					// Lock the tables.
					Debug.Assert(!ClientMarketData.AreLocksHeld);
					ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

					DataRow dataRow = (DataRow)this.iEnumerator.Current;
					return Position.Make((int)dataRow[this.accountIdColumn], (int)dataRow[this.securityIdColumn],
						(int)dataRow[this.positionTypeCodeColumn]);

				}
				finally
				{

					// Release the table locks.
					if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
					if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
					if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
					if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
					if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
					Debug.Assert(!ClientMarketData.AreLocksHeld);

				}

			}

		}

		/// <summary>
		/// Moves to the next object in the list.
		/// </summary>
		/// <returns>True if there are more object in the list, false if this is the last one.</returns>
		public bool MoveNext() {return this.iEnumerator.MoveNext();}

		/// <summary>
		/// Resets the enumerator to the beginning of the list.
		/// </summary>
		public new void Reset() {this.iEnumerator.Reset();}

		public Position Add(Position position)
		{

			if (this.Rows.Find(new object[] {position.Account.AccountId, position.Security.SecurityId, (int)position.PositionType})
				== null)
			{
				DataRow dataRow = this.NewRow();
				dataRow[this.accountIdColumn] = position.Account.AccountId;
				dataRow[this.securityIdColumn] = position.Security.SecurityId;
				dataRow[this.positionTypeCodeColumn] = (int)position.PositionType;
				this.Rows.Add(dataRow);
			}

			return position;

		}

	}

	public class PositionList : ArrayList
	{

		public void Add(Position newPosition)
		{

			foreach (Position position in this)
				if (position.Security.SecurityId == newPosition.Security.SecurityId &&
					position.Account.AccountId == newPosition.Account.AccountId &&
					position.PositionType == newPosition.PositionType)
					return;

			base.Add(newPosition);

		}

	}
	
	/// <summary>
	/// Information associated with an account, security, and long or short position type.
	/// </summary>
	public class Position
	{

		/// <summary>The account which holds this position.</summary>
		private Account account;
		/// <summary>The security identifier.</summary>
		private Security security;
		/// <summary>Indicates whether the position is owned (long) or borrowed (short).</summary>
		private PositionType positionType;
		/// <summary>User supplied data #0.</summary>
		private decimal userData0;
		/// <summary>User supplied data #1.</summary>
		private decimal userData1;
		/// <summary>User supplied data #2.</summary>
		private decimal userData2;
		/// <summary>User supplied data #3.</summary>
		private decimal userData3;
		/// <summary>User supplied data #4.</summary>
		private decimal userData4;
		/// <summary>User supplied data #5.</summary>
		private decimal userData5;
		/// <summary>User supplied data #6.</summary>
		private decimal userData6;
		/// <summary>User supplied data #7.</summary>
		private decimal userData7;

		/// <summary>Gets the account.</summary>
		public Account Account {get {return this.account;}}
		/// <summary>Gets the security.</summary>
		public Security Security {get {return this.security;}}
		/// <summary>Gets the long or short position type</summary>
		public PositionType PositionType {get {return this.positionType;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData0 {get {return this.userData0;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData1 {get {return this.userData1;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData2 {get {return this.userData2;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData3 {get {return this.userData3;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData4 {get {return this.userData4;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData5 {get {return this.userData5;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData6 {get {return this.userData6;}}
		/// <summary>Gets a user defined field.</summary>
		public decimal UserData7 {get {return this.userData7;}}

		internal Position() {}
			
		public static PositionTypes GetPositionTypes() {return new PositionTypes();}

		/// <summary>
		/// Finds a position.
		/// </summary>
		/// <param name="account">The account.</param>
		/// <param name="security">The security.</param>
		/// <param name="positionType">A Long or Short Position Type.</param>
		/// <returns>A Position.</returns>
		public Position(Account account, Security security, PositionType positionType)
		{

			// We can always create a position.  There may be no information associated with it, but it can be created just the
			// same.
			this.account = account;
			this.security = security;
			this.positionType = positionType;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Look up the position record.
				ClientMarketData.PositionRow positionRow =
					ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(account.AccountId, security.SecurityId, (int)positionType);
				
				// If we have a record for the price/currency combination, then update it with the new price.
				if (positionRow != null)
				{

					if (!positionRow.IsUserData0Null())
						this.userData0 = positionRow.UserData0;
					if (!positionRow.IsUserData1Null())
						this.userData1 = positionRow.UserData1;
					if (!positionRow.IsUserData2Null())
						this.userData2 = positionRow.UserData2;
					if (!positionRow.IsUserData3Null())
						this.userData3 = positionRow.UserData3;
					if (!positionRow.IsUserData4Null())
						this.userData4 = positionRow.UserData4;
					if (!positionRow.IsUserData5Null())
						this.userData5 = positionRow.UserData5;
					if (!positionRow.IsUserData6Null())
						this.userData6 = positionRow.UserData6;
					if (!positionRow.IsUserData7Null())
						this.userData7 = positionRow.UserData7;

				}

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		internal static Position Make(int accountId, int securityId, int positionTypeCode)
		{

			Position position = new Position();

			ClientMarketData.AccountRow accountRow =
				ClientMarketData.Account.FindByAccountId(accountId);
			if (accountRow == null)
				throw new Exception(string.Format("Account {0} doesn't exist", accountId));

			ClientMarketData.SecurityRow securityRow =
				ClientMarketData.Security.FindBySecurityId(securityId);
			if (securityRow == null)
				throw new Exception(string.Format("Security {0} doesn't exist", securityId));
			
			// Look up the position record.
			ClientMarketData.PositionRow positionRow =
				ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(accountId, securityId, positionTypeCode);
				
			// We can always create a position.  There may be no information associated with it, but it can be created just the
			// same.
			position.account = Account.Make(accountId);
			position.security = Security.Make(securityId);
			position.positionType = (PositionType)positionTypeCode;

			// If we have a record for the price/currency combination, then update it with the new price.
			if (positionRow != null)
			{

				if (!positionRow.IsUserData0Null())
					position.userData0 = positionRow.UserData0;
				if (!positionRow.IsUserData1Null())
					position.userData1 = positionRow.UserData1;
				if (!positionRow.IsUserData2Null())
					position.userData2 = positionRow.UserData2;
				if (!positionRow.IsUserData3Null())
					position.userData3 = positionRow.UserData3;
				if (!positionRow.IsUserData4Null())
					position.userData4 = positionRow.UserData4;
				if (!positionRow.IsUserData5Null())
					position.userData5 = positionRow.UserData5;
				if (!positionRow.IsUserData6Null())
					position.userData6 = positionRow.UserData6;
				if (!positionRow.IsUserData7Null())
					position.userData7 = positionRow.UserData7;

			}

			return position;

		}

		internal static Position Make(Account account, Security security, PositionType positionType)
		{

			Position position = new Position();

			// We can always create a position.  There may be no information associated with it, but it can be created just the
			// same.
			position.account = account;
			position.security = security;
			position.positionType = positionType;

			// Look up the position record.
			ClientMarketData.PositionRow positionRow =
				ClientMarketData.Position.FindByAccountIdSecurityIdPositionTypeCode(account.AccountId, security.SecurityId, (int)positionType);
				
			// If we have a record for the price/currency combination, then update it with the new price.
			if (positionRow != null)
			{

				if (!positionRow.IsUserData0Null())
					position.userData0 = positionRow.UserData0;
				if (!positionRow.IsUserData1Null())
					position.userData1 = positionRow.UserData1;
				if (!positionRow.IsUserData2Null())
					position.userData2 = positionRow.UserData2;
				if (!positionRow.IsUserData3Null())
					position.userData3 = positionRow.UserData3;
				if (!positionRow.IsUserData4Null())
					position.userData4 = positionRow.UserData4;
				if (!positionRow.IsUserData5Null())
					position.userData5 = positionRow.UserData5;
				if (!positionRow.IsUserData6Null())
					position.userData6 = positionRow.UserData6;
				if (!positionRow.IsUserData7Null())
					position.userData7 = positionRow.UserData7;

			}

			return position;

		}

		/// <summary>
		/// Detemines if a position exists in a given account, security and position type combination.
		/// </summary>
		/// <param name="account">The account.</param>
		/// <param name="security">The security.</param>
		/// <param name="positionType">A Long or Short Position Type.</param>
		/// <returns>True if the position exists, false otherwise.</returns>
		public static bool Contains(Account account, Security security, int positionType)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);

				if (ClientMarketData.TaxLot.UKTaxLotAccountIdSecurityIdPositionTypeCode.Find(new object[] {account.AccountId, security.SecurityId, positionType}) != -1)
					return true;

				if (ClientMarketData.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.Find(new object[] {account.AccountId, security.SecurityId, positionType}) != -1)
					return true;

				if (ClientMarketData.Order.UKOrderAccountIdSecurityIdPositionTypeCode.Find(new object[] {account.AccountId, security.SecurityId, positionType}) != -1)
					return true;

				if (ClientMarketData.Allocation.UKAllocationAccountIdSecurityIdPositionTypeCode.Find(new object[] {account.AccountId, security.SecurityId, positionType}) != -1)
					return true;

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			return false;

		}

		/// <summary>
		/// Gets the quantity of a position
		/// </summary>
		public decimal GetQuantity()
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				return Quantity.Calculate(this.account.AccountId, this.security.SecurityId, (int)this.positionType, MarketValueFlags.EntirePosition);

			}
			finally
			{

				// Locks are no longer needed on the price table.
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

	}

}
