namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Something that can be traded for another sector.
	/// </summary>
	/// <remarks>This is a remark for the sector</remarks>
	public class Sector : Shadows.Quasar.Rule.Object
	{

		/// <summary>Gets the internal sector identifier.</summary>
		public int SectorId {get {return this.ObjectId;}}

		/// <summary>
		/// Creates a sector using the internal identifier.
		/// </summary>
		/// <param name="sectorId">The sector identifier.</param>
		public Sector(int sectorId)
		{

			// Initialize the object using the internal sector identifier.
			Initialize(sectorId);

		}
		
		/// <summary>
		/// Creates a sector using the external identifier.
		/// </summary>
		/// <param name="sectorId">The sector identifier.</param>
		public Sector(string sectorId)
		{

			// Initialize the object using the default configuration and the external sector identifier.
			Initialize("DEFAULT", sectorId);

		}
		
		/// <summary>
		/// Creates a sector using the external identifier.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="sectorId">The sector identifier.</param>
		public Sector(string configurationId, string sectorId)
		{

			// Initialize the object using the specified configuration and external sector identifier.
			Initialize(configurationId, sectorId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="sectorId">The sector identifier.</param>
		private void Initialize(string configurationId, string sectorId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal sector identifier.
				int internalSectorId = Rule.Object.FindKey(configurationId, "sectorId", sectorId);
				if (internalSectorId == Identifier.NotFound)
					throw new Exception(String.Format("Sector {0} doesn't exist", sectorId));

				// Initialize the sector using the internal identifier.
				Initialize(internalSectorId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Sector.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="sectorId">The sector identifier.</param>
		/// <returns>A sector record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int sectorId)
		{

			// Use the specified configuration to find the internal sector identifier.
			ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(sectorId);
			if (sectorRow == null)
				throw new Exception(String.Format("Sector {0} doesn't exist", sectorId));

			// Initialize the base class.
			base.Initialize(sectorId);

		}

		private bool RecurseContains(ClientMarketData.ObjectRow objectRow, int securityId)
		{

			foreach (ClientMarketData.ObjectTreeRow objectsTreeRow in objectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
			{

				foreach (ClientMarketData.SectorRow sectorRow in objectsTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
					if (RecurseContains(sectorRow.ObjectRow, securityId))
						return true;

				foreach (ClientMarketData.SecurityRow securityRow in objectsTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSecurityRows())
					if (securityRow.SecurityId == securityId)
						return true;

			}

			return false;

		}
		
	}

}
