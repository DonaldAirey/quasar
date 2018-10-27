namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// A collection of orders.
	/// </summary>
	public class Blotter : Shadows.Quasar.Rule.Object
	{

		private BlotterType blotterType;

		/// <summary>Gets the internal blotter identifier.</summary>
		public int BlotterId {get {return this.ObjectId;}}
		/// <summary>Gets the blotter type.</summary>
		public BlotterType BlotterType {get {return this.blotterType;}}

		/// <summary>
		/// Creates a blotter using the internal identifier.
		/// </summary>
		/// <param name="blotterId">The blotter identifier.</param>
		public Blotter(int blotterId)
		{

			// Initialize the object using the internal blotter identifier.
			Initialize(blotterId);

		}
		
		/// <summary>
		/// Creates a blotter using the external identifier.
		/// </summary>
		/// <param name="blotterId">The blotter identifier.</param>
		public Blotter(string blotterId)
		{

			// Initialize the object using the default configuration and the external blotter identifier.
			Initialize("DEFAULT", blotterId);

		}
		
		/// <summary>
		/// Creates a blotter using the external identifier.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="blotterId">The blotter identifier.</param>
		public Blotter(string configurationId, string blotterId)
		{

			// Initialize the object using the specified configuration and external blotter identifier.
			Initialize(configurationId, blotterId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="blotterId">The blotter identifier.</param>
		private void Initialize(string configurationId, string blotterId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal blotter identifier.
				int internalBlotterId = Rule.Object.FindKey(configurationId, "blotterId", blotterId);
				if (internalBlotterId == Identifier.NotFound)
					throw new Exception(String.Format("Blotter {0} doesn't exist", blotterId));

				// Initialize the blotter using the internal identifier.
				Initialize(internalBlotterId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Blotter.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="blotterId">The blotter identifier.</param>
		/// <returns>A blotter record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int blotterId)
		{

			// Use the specified configuration to find the internal blotter identifier.
			ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
			if (blotterRow == null)
				throw new Exception(String.Format("Blotter {0} doesn't exist", blotterId));

			// Initialize the base class.
			base.Initialize(blotterId);

			// Initialize the record from the data model.
			this.blotterType = (BlotterType)blotterRow.BlotterTypeCode;

		}

	}

}
