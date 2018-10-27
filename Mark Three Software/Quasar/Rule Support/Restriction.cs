namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Data;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Restriction.
	/// </summary>
	public class Restriction
	{

		private int restrictionId;
		private Severity severity;
		private Approval approval;
		private string description;

		public int RestrictionId {get {return this.restrictionId;}}
		public Severity Severity {get {return this.severity;}}
		public Approval Approval {get {return this.approval;}}
		public string Description {get {return this.description;}}

		private Restriction() {}
		
		public Restriction(int restrictionId)
		{

			Initialize(restrictionId);

		}
	
		public Restriction(ClientMarketData.RestrictionRow restrictionRow)
		{

			// Initialize the object from the ADO record.
			Initialize(restrictionRow);

		}

		public Restriction(string restrictionId, Severity severity, Approval approval, string description)
		{

			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Restriction");

			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("internalRestrictionId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("severity", (int)severity);
			remoteMethod.Parameters.Add("approval", (int)approval);
			remoteMethod.Parameters.Add("description", description);
			remoteMethod.Parameters.Add("userId0", restrictionId);
			ClientMarketData.Send(remoteBatch);

			Initialize((int)remoteMethod.Parameters["internalRestrictionId"].Value);

		}

		internal static Restriction Make(ClientMarketData.RestrictionRow restrictionRow)
		{

			Restriction restriction = new Restriction();
			restriction.Initialize(restrictionRow);
			return restriction;

		}
			
		private void Initialize(int restrictionId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the restriction exists.  We need it to find the default settlement restriction.
				ClientMarketData.RestrictionRow restrictionRow = ClientMarketData.Restriction.FindByRestrictionId(restrictionId);
				if (restrictionRow == null)
					throw new Exception(String.Format("Restriction {0} doesn't exist", restrictionId));

				// Initialize the object from the ADO record.
				Initialize(restrictionRow);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		private void Initialize(ClientMarketData.RestrictionRow restrictionRow)
		{

			// Initialize the record from the data model.
			this.restrictionId = restrictionRow.RestrictionId;
			this.severity = (Severity)restrictionRow.Severity;
			this.approval = (Approval)restrictionRow.Approval;
			this.description = restrictionRow.Description;

		}
		
		/// <summary>Finds a a Restriction record using a configuration and an external identifier.</summary>
		/// <param name="configurationId">Specified which mappings (user id columns) to use when looking up external identifiers.</param>
		/// <param name="externalId">The external (user supplied) identifier for the record.</param>
		private static int FindKey(string configurationId, string externalId)
		{
			// Accessor for the Restriction Table.
			ClientMarketData.RestrictionDataTable restrictionTable = ClientMarketData.Restriction;
			// Translate the configurationId and the predefined parameter name into an index into the array of user ids.  The index
			// is where we expect to find the identifier.  That is, an index of 1 will guide the lookup logic to use the external
			// identifiers found in the 'ExternalId1' column.
			ClientMarketData.ConfigurationRow configurationRow = ClientMarketData.Configuration.FindByConfigurationIdParameterId(configurationId, "restrictionId");
			int userIdIndex = 0;
			if ((configurationRow != null))
			{
				userIdIndex = configurationRow.ColumnIndex;
			}
			// This does an indirect lookup operation using the views created for the ExternalId columns.  Take the index of the user
			// identifier column calcualted above and use it to find a record containing the external identifier.
			DataView[] userIdIndexArray = new DataView[] {
															 restrictionTable.UKRestrictionExternalId0,
															 restrictionTable.UKRestrictionExternalId1};
			DataRowView[] dataRowView = userIdIndexArray[userIdIndex].FindRows(new object[] {
																								externalId});
			// If a row was found using the indirect operation above, then return the unique identifier of that row.  Otherwise,
			// indicate that the row doesn't exist.
			if ((dataRowView.Length == 0))
			{
				return Identifier.NotFound;
			}
			return ((int)(dataRowView[0].Row[restrictionTable.RestrictionIdColumn]));
		}
       
		public static Restriction Find(string restrictionId)
		{

			Restriction restriction = null;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				int internalRestrictionId = FindKey("DEFAULT", restrictionId);
				if (internalRestrictionId != Identifier.NotFound)
				{

					ClientMarketData.RestrictionRow restrictionRow = ClientMarketData.Restriction.FindByRestrictionId(internalRestrictionId);
					if (restrictionRow == null)
						throw new Exception(string.Format("Restriction {0} can'b be found", internalRestrictionId));

					restriction = new Restriction(restrictionRow);

				}

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.RestrictionLock.IsReaderLockHeld) ClientMarketData.RestrictionLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			return restriction;

		}

		public ViolationList GetViolations()
		{

			ViolationList violationList = new ViolationList();
			
			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.RestrictionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ViolationLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the restriction exists.  We need it to find the default settlement restriction.
				ClientMarketData.RestrictionRow restrictionRow = ClientMarketData.Restriction.FindByRestrictionId(this.restrictionId);
				if (restrictionRow == null)
					throw new Exception(String.Format("Restriction {0} doesn't exist", restrictionId));

				foreach (ClientMarketData.ViolationRow violationRow in restrictionRow.GetViolationRows())
					violationList.Add(new Violation(violationRow));

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

			return violationList;

		}

	}

}
