namespace Shadows.Quasar.Viewers.Order
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;

	internal class DeleteArgs
	{

		// Members
		private long rowVersion;
		private int orderId;
		private ClientMarketData.OrderRow SetPrice;

		// Public access
		public long RowVersion {get {return this.rowVersion;}}
		public int OrderId {get {return this.orderId;}}
		public ClientMarketData.OrderRow OrderRow {get {return this.SetPrice;}}

		public DeleteArgs(long rowVersion, int orderId, ClientMarketData.OrderRow SetPrice)
		{

			// Initialize Members
			this.rowVersion = rowVersion;
			this.orderId = orderId;
			this.SetPrice = SetPrice;

		}

	}

}
