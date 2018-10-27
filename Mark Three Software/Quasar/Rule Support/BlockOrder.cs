/*************************************************************************************************************************
*
*	File:			BlockOrder.cs
*	Description:	Block Order Primitives for the Quasar Trading Language.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// A collection of orders that can be traded as a unit.
	/// </summary>
	public class BlockOrder
	{

		private int blockOrderId;
		private Security security;
		private Security settlement;
		private TransactionType transactionType;

		/// <summary>Identifies the block order.</summary>
		public int BlockOrderId {get {return this.blockOrderId;}}

		/// <summary>Indicates what type of transaction is grouped into this block order.</summary>
		public Security Security {get {return this.security;}}

		/// <summary>Indicates what type of transaction is grouped into this block order.</summary>
		public Security Settlement {get {return this.settlement;}}

		/// <summary>Indicates what type of transaction is grouped into this block order.</summary>
		public TransactionType TransactionType {get {return this.transactionType;}}

		/// <summary>
		/// Creates a block order.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="blotterId">The destination blotter for the trade.</param>
		/// <param name="securityId">The security.</param>
		/// <param name="transactionType">The type of transaction.</param>
		public BlockOrder(Blotter blotter, Security security, TransactionType transactionType)
		{

			// Create a block order on the server.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.BlockOrder");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("blockOrderId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("blotterId", blotter.BlotterId);
			remoteMethod.Parameters.Add("securityId", security.SecurityId);
			remoteMethod.Parameters.Add("settlementId", security.SettlementId);
			remoteMethod.Parameters.Add("transactionTypeCode", (int)transactionType);
			ClientMarketData.Execute(remoteBatch);

			// Now that the block order is created, construct the in-memory version of the record.
			int blockOrderId = (int)remoteMethod.Parameters["blockOrderId"].Value;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null)
					throw new Exception(String.Format("BlockOrder {0} doesn't exist", blockOrderId));

				this.blockOrderId = blockOrderRow.BlockOrderId;
				this.security = Security.Make(blockOrderRow.SecurityRowByFKSecurityBlockOrderSecurityId.SecurityId);
				this.settlement = Security.Make(blockOrderRow.SecurityRowByFKSecurityBlockOrderSettlementId.SecurityId);
				this.transactionType = (TransactionType)blockOrderRow.TransactionTypeCode;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

		}

		/// <summary>
		/// Adds an order to a block order.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="accountId">The destination account for the order.</param>
		/// <param name="tif">Specifies a time limit on the order.</param>
		/// <param name="quantity">The number of units to be traded.</param>
		/// <returns>An internal identifier used to track the order.</returns>
		public int AddOrder(Account account, TIF tif, decimal quantity)
		{

			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Order");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("blockOrderId", this.blockOrderId);
			remoteMethod.Parameters.Add("accountId", account.AccountId);
			remoteMethod.Parameters.Add("securityId", this.Security.SecurityId);
			remoteMethod.Parameters.Add("settlementId", this.Settlement.SecurityId);
			remoteMethod.Parameters.Add("timeInForceCode", (int)tif);
			remoteMethod.Parameters.Add("transactionTypeCode", (int)this.transactionType);
			remoteMethod.Parameters.Add("orderTypeCode", (int)PricedAt.Market);
			remoteMethod.Parameters.Add("quantity", quantity);
			ClientMarketData.Execute(remoteBatch);

			return (int)remoteMethod.Parameters["orderId"].Value;

		}

		/// <summary>
		/// Adds an order to a block order.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="accountId">The destination account for the order.</param>
		/// <param name="tif">Specifies a time limit on the order.</param>
		/// <param name="quantity">The number of units to be traded.</param>
		/// <param name="pricedAt">Specifies how the order is to be priced.</param>
		/// <param name="limitPrice">A limit price for the order.</param>
		/// <returns>An internal identifier used to track the order.</returns>
		public int AddOrder(Account account, TIF tif, decimal quantity, PricedAt pricedAt, decimal limitPrice)
		{

			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Order");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
			remoteMethod.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
			remoteMethod.Parameters.Add("blockOrderId", this.blockOrderId);
			remoteMethod.Parameters.Add("accountId", account.AccountId);
			remoteMethod.Parameters.Add("securityId", this.Security.SecurityId);
			remoteMethod.Parameters.Add("settlementId", this.Settlement.SecurityId);
			remoteMethod.Parameters.Add("timeInForceCode", (int)tif);
			remoteMethod.Parameters.Add("transactionTypeCode", (int)this.transactionType);
			remoteMethod.Parameters.Add("orderTypeCode", (int)pricedAt);
			remoteMethod.Parameters.Add("quantity", quantity);
			remoteMethod.Parameters.Add("price1", limitPrice);
			remoteMethod.Parameters.Add("price2", limitPrice);
			ClientMarketData.Execute(remoteBatch);

			return (int)remoteMethod.Parameters["orderId"].Value;

		}

		/// <summary>
		/// Executes a block order.
		/// </summary>
		/// <param name="configurationId">Defines which external fields are used to identify an object.</param>
		/// <param name="brokerId">The destination broker for the order.</param>
		/// <param name="tif">Specifies a time limit on the order.</param>
		/// <param name="quantity">The number of units to be traded.</param>
		/// <param name="pricedAt">Specifies how the order is to be priced.</param>
		/// <param name="limitPrice">A limit price for the order.</param>
		public void Execute(Broker broker, TIF tif, decimal quantity, PricedAt pricedAt, decimal limitPrice)
		{

			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Trading.BlockOrder");
			RemoteMethod remoteMethod = remoteType.Methods.Add("Execute");
			remoteMethod.Parameters.Add("blockOrderId", this.blockOrderId);
			remoteMethod.Parameters.Add("brokerId", broker.BrokerId);
			remoteMethod.Parameters.Add("timeInForceCode", (int)tif);
			remoteMethod.Parameters.Add("quantity", quantity);
			remoteMethod.Parameters.Add("orderTypeCode", (int)pricedAt);
			remoteMethod.Parameters.Add("price1", limitPrice);
			ClientMarketData.Execute(remoteBatch);

		}

	}

}
