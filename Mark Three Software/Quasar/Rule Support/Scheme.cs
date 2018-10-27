namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Something that can be traded for another scheme.
	/// </summary>
	/// <remarks>This is a remark for the scheme</remarks>
	public class Scheme : Shadows.Quasar.Rule.Object
	{

		/// <summary>Gets the internal scheme identifier.</summary>
		public int SchemeId {get {return this.ObjectId;}}

		/// <summary>
		/// Creates a scheme using the internal identifier.
		/// </summary>
		/// <param name="schemeId">The scheme identifier.</param>
		public Scheme(int schemeId)
		{

			// Initialize the object using the internal scheme identifier.
			Initialize(schemeId);

		}
		
		/// <summary>
		/// Creates a scheme using the external identifier.
		/// </summary>
		/// <param name="schemeId">The scheme identifier.</param>
		public Scheme(string schemeId)
		{

			// Initialize the object using the default configuration and the external scheme identifier.
			Initialize("DEFAULT", schemeId);

		}
		
		/// <summary>
		/// Creates a scheme using the external identifier.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="schemeId">The scheme identifier.</param>
		public Scheme(string configurationId, string schemeId)
		{

			// Initialize the object using the specified configuration and external scheme identifier.
			Initialize(configurationId, schemeId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="schemeId">The scheme identifier.</param>
		private void Initialize(string configurationId, string schemeId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal scheme identifier.
				int internalSchemeId = Rule.Object.FindKey(configurationId, "schemeId", schemeId);
				if (internalSchemeId == Identifier.NotFound)
					throw new Exception(String.Format("Scheme {0} doesn't exist", schemeId));

				// Initialize the scheme using the internal identifier.
				Initialize(internalSchemeId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Scheme.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="schemeId">The scheme identifier.</param>
		/// <returns>A scheme record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int schemeId)
		{

			// Use the specified configuration to find the internal scheme identifier.
			ClientMarketData.SchemeRow schemeRow = ClientMarketData.Scheme.FindBySchemeId(schemeId);
			if (schemeRow == null)
				throw new Exception(String.Format("Scheme {0} doesn't exist", schemeId));

			// Initialize the base class.
			base.Initialize(schemeId);

		}

	}

}
