namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Something that can be traded for another broker.
	/// </summary>
	/// <remarks>This is a remark for the broker</remarks>
	public class Broker : Shadows.Quasar.Rule.Object
	{

		private string symbol;

		/// <summary>Gets the internal broker identifier.</summary>
		public int BrokerId {get {return this.ObjectId;}}
		/// <summary>Gets the symbol most commonly used to identify the broker.</summary>
		public string Symbol {get {return this.symbol;}}

		/// <summary>
		/// Creates a broker using the internal identifier.
		/// </summary>
		/// <param name="brokerId">The broker identifier.</param>
		public Broker(int brokerId)
		{

			// Initialize the object using the internal broker identifier.
			Initialize(brokerId);

		}
		
		/// <summary>
		/// Creates a broker using the external identifier.
		/// </summary>
		/// <param name="brokerId">The broker identifier.</param>
		public Broker(string brokerId)
		{

			// Initialize the object using the default configuration and the external broker identifier.
			Initialize("DEFAULT", brokerId);

		}
		
		/// <summary>
		/// Creates a broker using the external identifier.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="brokerId">The broker identifier.</param>
		public Broker(string configurationId, string brokerId)
		{

			// Initialize the object using the specified configuration and external broker identifier.
			Initialize(configurationId, brokerId);

		}
		
		/// <summary>
		/// Resolves external identifiers before initializing the object.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="brokerId">The broker identifier.</param>
		private void Initialize(string configurationId, string brokerId)
		{

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ConfigurationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Use the specified configuration to find the internal broker identifier.
				int internalBrokerId = Rule.Object.FindKey(configurationId, "brokerId", brokerId);
				if (internalBrokerId == Identifier.NotFound)
					throw new Exception(String.Format("Broker {0} doesn't exist", brokerId));

				// Initialize the broker using the internal identifier.
				Initialize(internalBrokerId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ConfigurationLock.IsReaderLockHeld) ClientMarketData.ConfigurationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld) ClientMarketData.BrokerLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}
		
		/// <summary>
		/// Initializes a Broker.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="brokerId">The broker identifier.</param>
		/// <returns>A broker record, null if the identifier doesn't exist.</returns>
		protected override void Initialize(int brokerId)
		{

			// Use the specified configuration to find the internal broker identifier.
			ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(brokerId);
			if (brokerRow == null)
				throw new Exception(String.Format("Broker {0} doesn't exist", brokerId));

			// Initialize the base class.
			base.Initialize(brokerId);

			// Initialize the record from the data model.
			this.symbol = brokerRow.Symbol;

		}

	}

}
