/*************************************************************************************************************************
*
*	File:			SecurityRebalancer.cs
*	Description:	This module contains the algorithm for rebalancing an appraisal to security level targets.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Library.Rebalancer
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Reblancing according to percentages of securities.
	/// </summary>
	public class Security
	{

		/// <summary>
		/// Recursively rebalances an account and all it's children.
		/// </summary>
		/// <param name="accountRow">The parent account to be rebalanced.</param>
		private static void RecurseAccounts(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			AppraisalSet appraisalSet, ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow)
		{

			// The base currency of the account is used to cacluate market values.
			ClientMarketData.CurrencyRow currencyRow = ClientMarketData.Currency.FindByCurrencyId(accountRow.CurrencyId);
			
			// Calculate the total market value for the appraisal.  This will be the denominator in all calculations involving
			// portfolio percentages.
			decimal accountMarketValue = MarketValue.Calculate(currencyRow, accountRow, MarketValueFlags.EntirePosition);

			// Cycle through all the positions of the appraisal using the current account and calculate the size and direction of
			// the trade needed to bring it to the model's target percent.
			foreach (AppraisalSet.SecurityRow driverSecurity in appraisalSet.Security)
			{

				// We need to reference the security row in the ClientMarketData to price this item.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(driverSecurity.SecurityId);

				// In this rebalancing operation, the cash balance is dependant on the securities bought and sold. The assumption
				// is made that we won't implicitly add or remove cash to accomplish the reblancing operation. When stocks are
				// bought or sold below, they will impact the underlying currency.  A cash target can be reached by setting all the
				// other percentages up properly.  As long as the total percentage in a model is 100%, the proper cash target will
				// be calculated.  We don't have to do anything with this asset type.
				if (securityRow.SecurityTypeCode == SecurityType.Currency)
					continue;

				// This section will calculate the difference in between the actual and target market values for each
				// position and create orders that will bring the account to the targeted percentages.
				foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
				{

					// Calculate the proposed quantity needed to bring this asset/account combination to the percentage given by
					// the model.  First, find the target percent.  If it's not there, we assume a target of zero (meaning sell all
					// holdings).
					ClientMarketData.PositionTargetRow positionTargetRow =
						ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(modelRow.ModelId,
						securityRow.SecurityId, driverPosition.PositionTypeCode);
					decimal targetPositionPercent = positionTargetRow == null ? 0.0M : positionTargetRow.Percent;

					// The market value of this trade will be the target market value less the current market value of
					// this position (without including the existing proposed orders in the current market value
					// calculation).
					decimal targetPositionMarketValue = targetPositionPercent * accountMarketValue;
					decimal actualPositionMarketValue = MarketValue.Calculate(currencyRow, accountRow, securityRow,
						driverPosition.PositionTypeCode, MarketValueFlags.ExcludeProposedOrder);
					decimal proposedMarketValue = targetPositionMarketValue - actualPositionMarketValue;

					// Calculate the quantity needed to hit the target market value and round it according to the model. Note that
					// the market values and prices are all denominated in the currency of the parent account. Also note the
					// quantityFactor is needed for the proper quantity calculation.
					decimal price = Price.Security(currencyRow, securityRow);
					decimal proposedQuantity = price == 0.0M ? 0.0M : proposedMarketValue / (price * securityRow.QuantityFactor);

					// If we have an equity, round to the model's lot size.  Common values are 100 and 1.
					if (securityRow.SecurityTypeCode == SecurityType.Equity)
						proposedQuantity = Math.Round(proposedQuantity / modelRow.EquityRounding, 0) * modelRow.EquityRounding;

					// A debt generally needs to be rounded to face.
					if (securityRow.SecurityTypeCode == SecurityType.Debt)
						proposedQuantity = Math.Round(proposedQuantity / modelRow.DebtRounding, 0) * modelRow.DebtRounding;

					// Have the Order Form Builder object construct an order based on the new proposed quantity.  This method will
					// fill in the defaults needed for a complete Proposed Order.  It will also create an deposit or widthdrawal
					// from an account to cover the transaction.
					ProposedOrder.Create(remoteBatch, remoteTransaction, accountRow, securityRow, driverPosition.PositionTypeCode,
						proposedQuantity);

				}

			}

			// Now that we've rebalanced the parent account, cycle through all the children accounts and rebalance them.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					Security.RecurseAccounts(remoteBatch, remoteTransaction, appraisalSet, childAccount, modelRow);
		
		}
		
		/// <summary>
		/// Returns a set of orders that will achieve the targets specified by the model.
		/// </summary>
		/// <param name="accountRow">The account or parent account to be rebalanced.</param>
		/// <param name="modelRow">The target percentages to use for rebalancing.</param>
		/// <returns>A Dataset of new, updated and deleted orders.</returns>
		public static RemoteBatch Rebalance(ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow)
		{

			// The orders to insert, update and delete orders to achieve the target percentages will be put in this 
			// DataSet.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

			// The outline of the appraisal will be needed to make calculations based on a position, that is a security,
			// account, position type combination.  Note that we're also including all the model securities in the
			// outline.  This triggers a rebalance if a security exists in the model, but doesn't exist yet in the
			// appraisal.
			AppraisalSet appraisalSet = new Appraisal(accountRow, modelRow, true);

			// Rebalance the parent account and all it's children.
			Security.RecurseAccounts(remoteBatch, remoteTransaction, appraisalSet, accountRow, modelRow);

			// This is the sucessful result of rebalancing.
			return remoteBatch;

		}

	}

}
