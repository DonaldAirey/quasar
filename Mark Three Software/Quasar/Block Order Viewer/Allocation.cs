/*************************************************************************************************************************
*
*	File:			BlockOrderViewer.cs
*	Description:	This control is used to display and manage a blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Client;
using Shadows.Quasar.Common;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Web.Services.Protocols;

namespace Shadows.Quasar.Viewers.BlockOrder
{

	/// <summary>
	/// Summary description for Allocation.
	/// </summary>
	public class Allocation
	{

		public static void Distribute(int blockOrderId)
		{

			// This batch will be filled in with the allocations.
			RemoteBatch remoteBatch = null;

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the block order that is to be allocated.
				ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (blockOrderRow == null)
					throw new Exception(String.Format("Block Id {0} doesn't exist", blockOrderId));

				// Aggregate the total quantity ordered.  This becomes the demoninator for the pro-rata calculation.
				decimal orderedQuantity = 0.0M;
				foreach (ClientMarketData.OrderRow orderRow in blockOrderRow.GetOrderRows())
					orderedQuantity += orderRow.Quantity;

				// These values will total up all the executions posted against this block order.  The pro-rata 
				// allocation will divide all the executions up against the ratio of the quantity ordered against the
				// total quantity ordered.  The price is an average price of all the executions.
				decimal executedQuantity = 0.0M;
				decimal executedPrice = 0.0M;
				decimal executedCommission = 0.0M;
				decimal executedAccruedInterest = 0.0M;
				decimal executedUserFee0 = 0.0M;
				decimal executedUserFee1 = 0.0M;
				decimal executedUserFee2 = 0.0M;
				decimal executedUserFee3 = 0.0M;
				DateTime tradeDate = DateTime.MinValue;
				DateTime settlementDate = DateTime.MinValue;

				// Total up all the executions against this block.
				foreach (ClientMarketData.ExecutionRow executionRow in blockOrderRow.GetExecutionRows())
				{
					executedQuantity += executionRow.Quantity;
					executedPrice += executionRow.Price * executionRow.Quantity;
					executedCommission += executionRow.Commission;
					executedAccruedInterest += executionRow.AccruedInterest;
					executedUserFee0 += executionRow.UserFee0;
					executedUserFee1 += executionRow.UserFee1;
					executedUserFee2 += executionRow.UserFee2;
					executedUserFee3 += executionRow.UserFee3;
					tradeDate = executionRow.TradeDate;
					settlementDate = executionRow.SettlementDate;
				}

				// Calculate the average price.
				decimal averagePrice = Math.Round((executedQuantity > 0.0M) ? executedPrice / executedQuantity : 0.0M, 2);

				// These values are used to keep track of how much has been allocated.  Because of the nature of a
				// pro-rata allocation, there will be an odd remainder after the allocation is finished.  We will
				// arbitrarily pick the last order in the block order and give it the remainer of the order.  To do that,
				// totals have to be kept of all the allocations that have been created before the last one.
				decimal allocatedQuantity = 0.0M;
				decimal allocatedCommission = 0.0M;
				decimal allocatedAccruedInterest = 0.0M;
				decimal allocatedUserFee0 = 0.0M;
				decimal allocatedUserFee1 = 0.0M;
				decimal allocatedUserFee2 = 0.0M;
				decimal allocatedUserFee3 = 0.0M;

				// Put all the allocations in a single batch.
				remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Allocation");

				// Allocate the order back to the original accounts.  Because odd values can be generated when dividing by the
				// pro-rata value, the last order will get the remaining portions of the execution.
				int orderCounter = blockOrderRow.GetOrderRows().Length;
				foreach (ClientMarketData.OrderRow orderRow in blockOrderRow.GetOrderRows())
				{

					decimal quantity;
					decimal commission;
					decimal accruedInterest;
					decimal userFee0;
					decimal userFee1;
					decimal userFee2;
					decimal userFee3;
					
					// The last account in the order is arbitrarily given the remaining parts of the execution.
					if (--orderCounter == 0)
					{

						quantity = executedQuantity - allocatedQuantity;
						commission = executedCommission - allocatedCommission;
						accruedInterest = executedAccruedInterest = allocatedAccruedInterest;
						userFee0 = executedUserFee0 - allocatedUserFee0;
						userFee1 = executedUserFee1 - allocatedUserFee1;
						userFee2 = executedUserFee2 - allocatedUserFee2;
						userFee3 = executedUserFee3 - allocatedUserFee3;

					}
					else
					{

						// Calcuation the proportion of the trade destined for the current order.  The proportion is 
						// based on the amount of the original order against the block total.
						quantity = Math.Round(executedQuantity * orderRow.Quantity / orderedQuantity, 0);
						commission = Math.Round(executedCommission * orderRow.Quantity / orderedQuantity, 2);
						accruedInterest = Math.Round(executedAccruedInterest * orderRow.Quantity / orderedQuantity, 2);
						userFee0 = Math.Round(executedUserFee0 * orderRow.Quantity / orderedQuantity, 2);
						userFee1 = Math.Round(executedUserFee1 * orderRow.Quantity / orderedQuantity, 2);
						userFee2 = Math.Round(executedUserFee2 * orderRow.Quantity / orderedQuantity, 2);
						userFee3 = Math.Round(executedUserFee3 * orderRow.Quantity / orderedQuantity, 2);

					}

					// Keep a running total of the amount allocated so far.  This will be used to calculate the last order's
					// portion when the loop has finished.
					allocatedQuantity += quantity;
					allocatedCommission += commission;
					allocatedAccruedInterest += accruedInterest;
					allocatedUserFee0 += userFee0;
					allocatedUserFee1 += userFee1;
					allocatedUserFee2 += userFee2;
					allocatedUserFee3 += userFee3;

					// If the allocation has a positive quantity to register, then post it to the server.
					if (quantity > 0.0M)
					{
						// Call the web service to add the new execution.
						RemoteMethod remoteMethod = remoteType.Methods.Add("Insert");
						remoteMethod.Parameters.Add("blockOrderId", orderRow.BlockOrderId);
						remoteMethod.Parameters.Add("accountId", orderRow.AccountId);
						remoteMethod.Parameters.Add("securityId", orderRow.SecurityId);
						remoteMethod.Parameters.Add("settlementId", orderRow.SettlementId);
						remoteMethod.Parameters.Add("positionTypeCode", orderRow.PositionTypeCode);
						remoteMethod.Parameters.Add("transactionTypeCode", orderRow.TransactionTypeCode);
						remoteMethod.Parameters.Add("quantity", quantity);
						remoteMethod.Parameters.Add("price", averagePrice);
						remoteMethod.Parameters.Add("commission", commission);
						remoteMethod.Parameters.Add("accruedInterest", accruedInterest);
						remoteMethod.Parameters.Add("userFee0", userFee0);
						remoteMethod.Parameters.Add("userFee1", userFee1);
						remoteMethod.Parameters.Add("userFee2", userFee2);
						remoteMethod.Parameters.Add("userFee3", userFee3);
						remoteMethod.Parameters.Add("tradeDate", tradeDate);
						remoteMethod.Parameters.Add("settlementDate", settlementDate);
						remoteMethod.Parameters.Add("createdTime", DateTime.Now);
						remoteMethod.Parameters.Add("createdLoginId", ClientPreferences.LoginId);
						remoteMethod.Parameters.Add("modifiedTime", DateTime.Now);
						remoteMethod.Parameters.Add("modifiedLoginId", ClientPreferences.LoginId);

					}

				}

			}
			catch (Exception exception)
			{

				// This signals that the batch isn't valid and shouldn't be sent.
				remoteBatch = null;
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the locks obtained to produce the blotter report.
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Once the locks are release, the batch can be sent to the server.
			if (remoteBatch != null)
				ClientMarketData.Send(remoteBatch);

		}

	}

}
