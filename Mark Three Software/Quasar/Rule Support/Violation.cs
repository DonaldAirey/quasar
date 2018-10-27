namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Violation.
	/// </summary>
	public class Violation
	{

		private int violationId;
		private Restriction restriction;
		private Position position;
		private string description;

		public int ViolationId {get {return this.violationId;}}
		public Restriction Restriction {get {return this.restriction;}}
		public Position Position {get {return this.position;}}
		public string Description {get {return this.description;}}

		public Violation(ClientMarketData.ViolationRow violationRow)
		{

			// Initialize the record.
			this.violationId = violationRow.ViolationId;
			this.restriction = Restriction.Make(violationRow.RestrictionRow);
			this.position = Position.Make(violationRow.AccountId, violationRow.SecurityId, (int)violationRow.PositionTypeCode);
			this.description = description;

		}

		public static Violation Find(Restriction restriction, Position position)
		{

			Violation violation = null;
			
			try
			{

				// Lock the tables.
				if (!ClientMarketData.AreLocksHeld)
				{
					ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);
				}

				object[] key = new object[] {restriction.RestrictionId, position.Account.AccountId, position.Security.SecurityId,
												(int)position.PositionType};
				int recordIndex =
					ClientMarketData.Violation.UKViolationRestrictionIdAccountIdSecurityIdPositionTypeCode.Find(key);

				if (recordIndex != -1)
				{
					DataRow dataRow = ClientMarketData.Violation.UKViolationRestrictionIdAccountIdSecurityIdPositionTypeCode[recordIndex].Row;
					violation = new Violation((ClientMarketData.ViolationRow)dataRow);
				}

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			return violation;

		}

		public static Violation[] Find(Restriction restriction, Account account)
		{

			ArrayList violationList = new ArrayList();
			
			try
			{

				// Lock the tables.
				if (!ClientMarketData.AreLocksHeld)
				{
					ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
					ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);
				}

				object[] key = new object[] {restriction.RestrictionId, account.AccountId};
				DataRowView[] violatedRows =
					ClientMarketData.Violation.UKViolationRestrictionIdAccountId.FindRows(key);

				foreach (DataRowView dataRowView in violatedRows)
					violationList.Add(new Violation((ClientMarketData.ViolationRow)dataRowView.Row));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.PositionLock.IsReaderLockHeld) ClientMarketData.PositionLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			Violation[] violationArray = new Violation[violationList.Count];
			violationList.CopyTo(violationArray);
			return violationArray;

		}

		public static void Add(Restriction restriction, Position position, params object[] argument)
		{

			RemoteAssembly remoteAssembly = CommandBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Violation");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("restrictionId", restriction.RestrictionId);
			remoteMethod.Parameters.Add("accountId", position.Account.AccountId);
			remoteMethod.Parameters.Add("securityId", position.Security.SecurityId);
			remoteMethod.Parameters.Add("positionTypeCode", (int)position.PositionType);
			remoteMethod.Parameters.Add("description", String.Format(restriction.Description, argument));

		}

		public void Remove()
		{

			long rowVersion = long.MinValue;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Prevent duplicates.
                ClientMarketData.ViolationRow violationRow = ClientMarketData.Violation.FindByViolationId(this.violationId);
				if (violationRow == null)
					return;

				rowVersion = violationRow.RowVersion;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ViolationLock.IsReaderLockHeld) ClientMarketData.ViolationLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			RemoteAssembly remoteAssembly = CommandBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Violation");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Delete");
			remoteMethod.Parameters.Add("violationId", this.violationId);
			remoteMethod.Parameters.Add("rowVersion", rowVersion);

		}

	}

}
