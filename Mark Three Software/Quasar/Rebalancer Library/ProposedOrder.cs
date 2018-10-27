/*************************************************************************************************************************
*
*	File:			ProposedOrderCommand.cs
*	Description:	Support for the commands that maintain the proposed orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Library.Rebalancer
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using System;
	using System.Data;

	/// <summary>
	/// Updates a batch of proposed orders.
	/// </summary>
	public class ProposedOrder
	{

		/// <summary>
		/// Fills the OrderForm table with instructions to create, delete or update proposed orders.
		/// </summary>
		/// <param name="accountId">Identifiers the destination account of the proposed order.</param>
		/// <param name="securityId">Identifies the security being trade.</param>
		/// <param name="positionTypeCode">Identifies the long or short position of the trade.</param>
		/// <param name="settlementId"></param>
		/// <param name="proposedQuantity">The signed (relative) quantity of the trade.</param>
		public static void Create(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow, ClientMarketData.SecurityRow securityRow, int positionTypeCode,
			decimal proposedQuantity)
		{

			// If the proposed quantity is to be zero, we'll delete all proposed orders for this position in the parent and
			// descendant accounts.  Otherwise, a command batch will be created to clear any child proposed orders and create or
			// update a proposed order for the parent account.
			if (proposedQuantity == 0.0M)
				ProposedOrder.Delete(remoteBatch, remoteTransaction, accountRow, securityRow, positionTypeCode);
			else
			{

				// The strategy here is to cycle through all the existing proposed orders looking for any that match the account
				// id, security id and position type of the new order.  If none is found, we create a new order. If one is found,
				// we modify it for the new quantity.  Any additional proposed orders are deleted.  This flag lets us know if any
				// existing proposed orders match the position attributes.
				bool firstTime = true;

				// Cycle through each of the proposed orders in the given account looking for a matching position.
				object[] key = new object[] {accountRow.AccountId, securityRow.SecurityId, positionTypeCode};
				foreach (DataRowView dataRowView in ClientMarketData.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{

					// This is used to reference the current proposed order that matches the position criteria.
					ClientMarketData.ProposedOrderRow parentProposedOrderRow = (ClientMarketData.ProposedOrderRow)dataRowView.Row;

					// This check is provided for currency-like assets.  There may be many proposed orders for currency
					// transactions that are used to settle other trades.  The user can also enter currency orders directly into
					// the appraisal.  Any manual deposits or withdrawls should not impact settlement orders.  This check will skip
					// any trade that is linked to another order.
					if (Shadows.Quasar.Common.Relationship.IsChildProposedOrder(parentProposedOrderRow))
						continue;

					// Recycle the first proposed order that matches the position criteria.  Any additional proposed orders for the
					// same account, security, position type will be deleted.
					if (firstTime)
					{

						// Any proposed orders found after this one will be deleted.  This variable will also indicate that an
						// existing proposed order was recycled.  After the loop is run on this position, a new order will be
						// created if an existing order couldn't be recycled.
						firstTime = false;

						// Create the command to update this proposed order.
						Update(remoteBatch, remoteTransaction, parentProposedOrderRow, proposedQuantity);

					}
					else

						// Any order that isn't recycled is considered to be redundant.  That is, this order has been superceded by
						// the recycled order.  Clearing any redundant orders makes the operation more intuitive: the user knows
						// that the only order on the books is the one they entered.  They don't have to worry about artifacts from
						// other operations.
						Delete(remoteBatch, remoteTransaction, parentProposedOrderRow);

				}

				// This will create a new proposed order if an existing one couldn't be found above for recycling.
				if (firstTime == true)
					Insert(remoteBatch, remoteTransaction, accountRow, securityRow, positionTypeCode, proposedQuantity);

			}

		}

		/// <summary>
		/// Creates a command in a RemoteBatch structure to insert a proposed order.
		/// </summary>
		/// <param name="remoteBatch"></param>
		/// <param name="remoteTransaction"></param>
		/// <param name="accountRow"></param>
		/// <param name="securityRow"></param>
		/// <param name="positionTypeCode"></param>
		/// <param name="quantityInstruction"></param>
		private static void Insert(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow, ClientMarketData.SecurityRow securityRow, int positionTypeCode,
			decimal quantityInstruction)
		{
						
			// These define the assembly and the types within those assemblies that will be used to create the proposed orders on
			// the middle tier.
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = remoteAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = remoteAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

			// Find the default settlement for this order.
			int settlementId = Shadows.Quasar.Common.Security.GetDefaultSettlementId(securityRow);

			// As a convention between the rebalancing section and the order generation, the parentQuantity passed into this method
			// is a signed value where the negative values are treated as 'Sell' instructions and the positive values meaning
			// 'Buy'. This will adjust the parentQuantity so the trading methods can deal with an unsigned value, which is more
			// natural for trading.
			decimal parentQuantity = Math.Abs(quantityInstruction);
			
			// This will turn the signed parentQuantity into an absolute parentQuantity and a transaction code (e.g. -1000 is
			// turned into a SELL of 1000 shares).
			int parentTransactionTypeCode = TransactionType.Calculate(securityRow.SecurityTypeCode, positionTypeCode, quantityInstruction);

			// The time in force first comes from the user preferences, next, account settings and finally defaults to a day 
			// orders.
			int timeInForceCode = !ClientPreferences.IsTimeInForceCodeNull() ?
				ClientPreferences.TimeInForceCode : !accountRow.IsTimeInForceCodeNull() ? accountRow.TimeInForceCode :
				TimeInForce.DAY;

			// The destination blotter comes first from the user preferences, second from the account preferences, and finally uses
			// the auto-routing logic.
			int parentBlotterId = ClientPreferences.IsBlotterIdNull() ? (accountRow.IsBlotterIdNull() ?
				TradingSupport.AutoRoute(securityRow, parentQuantity) : accountRow.BlotterId) : ClientPreferences.BlotterId;

			// Create a command to delete the relationship between the parent and child.
			RemoteMethod insertParent = proposedOrderType.Methods.Add("Insert");
			insertParent.Transaction = remoteTransaction;
			insertParent.Parameters.Add("proposedOrderId", DataType.Int, Direction.ReturnValue);
			insertParent.Parameters.Add("blotterId", parentBlotterId);
			insertParent.Parameters.Add("accountId", accountRow.AccountId);
			insertParent.Parameters.Add("securityId", securityRow.SecurityId);
			insertParent.Parameters.Add("settlementId", settlementId);
			insertParent.Parameters.Add("positionTypeCode", positionTypeCode);
			insertParent.Parameters.Add("transactionTypeCode", parentTransactionTypeCode);
			insertParent.Parameters.Add("timeInForceCode", timeInForceCode);
			insertParent.Parameters.Add("orderTypeCode", OrderType.Market);
			insertParent.Parameters.Add("quantity", parentQuantity);
			
			// Now it's time to create an order for the settlement currency.
			if (securityRow.SecurityTypeCode == SecurityType.Equity || securityRow.SecurityTypeCode == SecurityType.Debt)
			{

				// The underlying currency is needed for the market value calculations.
				ClientMarketData.CurrencyRow currencyRow = MarketData.Currency.FindByCurrencyId(settlementId);

				decimal marketValue = parentQuantity * securityRow.QuantityFactor * Price.Security(currencyRow, securityRow)
					* securityRow.PriceFactor * TransactionType.GetCashSign(parentTransactionTypeCode);

				// The stragegy for handling the settlement currency changes is to calculate the old market value, calculate the
				// new market value, and add the difference to the running total for the settlement currency of this security. The
				// new market value is the impact of the trade that was just entered.
				int childTransactionTypeCode = TransactionType.Calculate(securityRow.SecurityTypeCode, positionTypeCode,
					marketValue);
				decimal childQuantity = Math.Abs(marketValue);

				// The destination blotter comes first from the user preferences, second from the account preferences, and finally
				// uses the auto-routing logic.
				int childBlotterId = ClientPreferences.IsBlotterIdNull() ? (accountRow.IsBlotterIdNull() ?
					TradingSupport.AutoRoute(currencyRow.SecurityRow, childQuantity) : accountRow.BlotterId) :
					ClientPreferences.BlotterId;

				// Fill in the rest of the fields and the defaulted fields for this order. Create a command to delete the
				// relationship between the parent and child.
				RemoteMethod insertChild = proposedOrderType.Methods.Add("Insert");
				insertChild.Transaction = remoteTransaction;
				insertChild.Parameters.Add("proposedOrderId", DataType.Int, Direction.ReturnValue);
				insertChild.Parameters.Add("blotterId", childBlotterId);
				insertChild.Parameters.Add("accountId", accountRow.AccountId);
				insertChild.Parameters.Add("securityId", settlementId);
				insertChild.Parameters.Add("settlementId", settlementId);
				insertChild.Parameters.Add("transactionTypeCode", childTransactionTypeCode);
				insertChild.Parameters.Add("positionTypeCode", positionTypeCode);
				insertChild.Parameters.Add("timeInForceCode", timeInForceCode);
				insertChild.Parameters.Add("orderTypeCode", OrderType.Market);
				insertChild.Parameters.Add("quantity", childQuantity);

				RemoteMethod insertRelation = proposedOrderTreeType.Methods.Add("Insert");
				insertRelation.Transaction = remoteTransaction;
				insertRelation.Parameters.Add("parentId", insertParent.Parameters["proposedOrderId"]);
				insertRelation.Parameters.Add("childId", insertChild.Parameters["proposedOrderId"]);

			}

		}
		
		private static void Update(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.ProposedOrderRow parentProposedOrder, decimal quantityInstruction)
		{
			
			// These define the assembly and the types within those assemblies that will be used to create the proposed orders on
			// the middle tier.
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = remoteAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");

			ClientMarketData.AccountRow accountRow = parentProposedOrder.AccountRow;
			ClientMarketData.SecurityRow securityRow = parentProposedOrder.SecurityRowByFKSecurityProposedOrderSecurityId;
			
			// This will turn the signed quantity into an absolute quantity and a transaction code (e.g. -1000 is turned into a
			// SELL of 1000 shares).
			decimal parentQuantity = Math.Abs(quantityInstruction);

			int parentTransactionTypeCode = TransactionType.Calculate(securityRow.SecurityTypeCode,
				parentProposedOrder.PositionTypeCode, quantityInstruction);
			
			// The time in force first comes from the user preferences, next, account settings and finally defaults to a day 
			// orders.
			int timeInForceCode = !ClientPreferences.IsTimeInForceCodeNull() ? ClientPreferences.TimeInForceCode :
				!accountRow.IsTimeInForceCodeNull() ? accountRow.TimeInForceCode : TimeInForce.DAY;

			// The destination blotter comes first from the user preferences, second from the account preferences, and finally uses
			// the auto-routing logic.
			int blotterId = !ClientPreferences.IsBlotterIdNull() ? ClientPreferences.BlotterId :
				!accountRow.IsBlotterIdNull() ? accountRow.BlotterId :
				TradingSupport.AutoRoute(securityRow, parentQuantity);

			// Create a command to update the proposed order.
			RemoteMethod updateParent = proposedOrderType.Methods.Add("Update");
			updateParent.Transaction = remoteTransaction;
			updateParent.Parameters.Add("rowVersion", parentProposedOrder.RowVersion);
			updateParent.Parameters.Add("proposedOrderId", parentProposedOrder.ProposedOrderId);
			updateParent.Parameters.Add("accountId", parentProposedOrder.AccountId);
			updateParent.Parameters.Add("securityId", parentProposedOrder.SecurityId);
			updateParent.Parameters.Add("settlementId", parentProposedOrder.SettlementId);
			updateParent.Parameters.Add("blotterId", blotterId);
			updateParent.Parameters.Add("positionTypeCode", parentProposedOrder.PositionTypeCode);
			updateParent.Parameters.Add("transactionTypeCode", parentTransactionTypeCode);
			updateParent.Parameters.Add("timeInForceCode", timeInForceCode);
			updateParent.Parameters.Add("orderTypeCode", OrderType.Market);
			updateParent.Parameters.Add("quantity", parentQuantity);

			foreach (ClientMarketData.ProposedOrderTreeRow proposedOrderTree in
				parentProposedOrder.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId())
			{

				ClientMarketData.ProposedOrderRow childProposedOrder =
					proposedOrderTree.ProposedOrderRowByFKProposedOrderProposedOrderTreeChildId;
							
				// If this is the settlement part of the order, then adjust the quantity.
				if (childProposedOrder.SecurityId == parentProposedOrder.SettlementId)
				{

					// The settlement security is needed for the calculation of the cash impact of this trade.
					ClientMarketData.CurrencyRow currencyRow =
						MarketData.Currency.FindByCurrencyId(childProposedOrder.SettlementId);

					decimal marketValue = parentQuantity * securityRow.QuantityFactor *
						Price.Security(currencyRow, securityRow) * securityRow.PriceFactor *
						TransactionType.GetCashSign(parentTransactionTypeCode);

					decimal childQuantity = Math.Abs(marketValue);

					int childTransactionTypeCode = TransactionType.Calculate(securityRow.SecurityTypeCode,
						parentProposedOrder.PositionTypeCode, marketValue);

					// Create a command to update the proposed order.
					RemoteMethod updateChild = proposedOrderType.Methods.Add("Update");
					updateChild.Transaction = remoteTransaction;
					updateChild.Parameters.Add("rowVersion", childProposedOrder.RowVersion);
					updateChild.Parameters.Add("proposedOrderId", childProposedOrder.ProposedOrderId);
					updateChild.Parameters.Add("accountId", childProposedOrder.AccountId);
					updateChild.Parameters.Add("securityId", childProposedOrder.SecurityId);
					updateChild.Parameters.Add("settlementId", childProposedOrder.SettlementId);
					updateChild.Parameters.Add("blotterId", blotterId);
					updateChild.Parameters.Add("positionTypeCode", parentProposedOrder.PositionTypeCode);
					updateChild.Parameters.Add("transactionTypeCode", childTransactionTypeCode);
					updateChild.Parameters.Add("timeInForceCode", timeInForceCode);
					updateChild.Parameters.Add("orderTypeCode", OrderType.Market);
					updateChild.Parameters.Add("quantity", childQuantity);

				}

			}

		}

		/// <summary>
		/// Recursively creates instructions to delete proposed order of the given position.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		private static void Delete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow, ClientMarketData.SecurityRow securityRow, int positionTypeCode)
		{

			// Run through each of the proposed orders in this account and create a record to have them deleted.
			object[] key = new object[] {accountRow.AccountId, securityRow.SecurityId, positionTypeCode};
			foreach (DataRowView dataRowView in
				MarketData.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
			{

				// This is used to reference the current proposed order that matches the position criteria.
				ClientMarketData.ProposedOrderRow proposedOrderRow = (ClientMarketData.ProposedOrderRow)dataRowView.Row;

				// Child proposed orders aren't deleted directly, they can only be deleted when the parent is deleted.  The best
				// example of this is cash.  An account can have both child cash (related to an equity trade) or parent cash (cash
				// added directly to the account with no offsetting trade).  If a reqest is made to delete cash, only the parent
				// cash should be deleted.  The account will appear to have a cash balance until the equity attached to the child
				// cash is deleted.
				if (!Relationship.IsChildProposedOrder(proposedOrderRow))
					Delete(remoteBatch, remoteTransaction, proposedOrderRow);
			
			}

		}

		/// <summary>
		/// Creates a batch command to delete a proposed order and it's links.
		/// </summary>
		/// <param name="remoteBatch"></param>
		/// <param name="remoteTransaction"></param>
		/// <param name="parentProposedOrder"></param>
		public static void Delete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.ProposedOrderRow parentProposedOrder)
		{

			// These define the assembly and the types within those assemblies that will be used to create the proposed orders on
			// the middle tier.
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = remoteAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = remoteAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

			// Proposed orders have a hierarchy.  For example, orders for equities will be linked to 
			foreach (ClientMarketData.ProposedOrderTreeRow proposedOrderTree in
				parentProposedOrder.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId())
			{

				// Create a command to delete the relationship between the parent and child.
				RemoteMethod deleteRelation = proposedOrderTreeType.Methods.Add("Delete");
				deleteRelation.Transaction = remoteTransaction;
				deleteRelation.Parameters.Add("rowVersion", proposedOrderTree.RowVersion);
				deleteRelation.Parameters.Add("parentId", proposedOrderTree.ParentId);
				deleteRelation.Parameters.Add("childId", proposedOrderTree.ChildId);
						
				// This relatioship will give us access to the child proposed order.
				ClientMarketData.ProposedOrderRow childProposedOrder =
					proposedOrderTree.ProposedOrderRowByFKProposedOrderProposedOrderTreeChildId;

				// Create a command to delete the child proposed order.
				RemoteMethod deleteChild = proposedOrderType.Methods.Add("Delete");
				deleteChild.Transaction = remoteTransaction;
				deleteChild.Parameters.Add("rowVersion", childProposedOrder.RowVersion);
				deleteChild.Parameters.Add("proposedOrderId", childProposedOrder.ProposedOrderId);

			}
					
			// Create a command to delete the parent proposed order.
			RemoteMethod deleteParent = proposedOrderType.Methods.Add("Delete");
			deleteParent.Transaction = remoteTransaction;
			deleteParent.Parameters.Add("rowVersion", parentProposedOrder.RowVersion);
			deleteParent.Parameters.Add("proposedOrderId", parentProposedOrder.ProposedOrderId);

		}

	}

}
