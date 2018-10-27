/*************************************************************************************************************************
*
*	File:			SecurityLevelRebalancer.cs
*	Description:	This module contains the algorithm for rebalancing an appraisal to sector level targets.  The
*					children of the account parameter are treated as components.  This means that any given sector
*					is modelled as a percentage of the aggregate appraisal market value.  This is useful when several
*					sub accounts are to be balanced against a single model.  For instance, a composite account having
*					equity, debt and currency sub-accounts would be rebalanced according to the aggregate market value
*					of the equities, debts and currencies.  This is different from a 'wrap' style rebalancing where the
*					percentages specified respect only the sub-account's market value.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Library.Rebalancer
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using System;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Reblancing according to percentages of sectors.
	/// </summary>
	public class SectorMerge
	{

		/// <summary>
		/// Recursively calculates proposed orders for a sector.
		/// </summary>
		/// <param name="sector">Gives the current sector (sector) for the calculation.</param>
		private static void RecurseSectors(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.CurrencyRow currencyRow, ClientMarketData.ModelRow modelRow, AppraisalSet.ObjectRow driverObject,
			decimal actualSectorMarketValue, decimal targetSectorMarketValue)
		{

			// Run through each of the positions in the sector and calculate the current percentage of the position within
			// the sector.  We're going to keep this percentage as we rebalance to the new sector market value.
			foreach (AppraisalSet.SecurityRow driverSecurity in driverObject.GetSecurityRows())
				foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
				{

					// We need to know what kind of security we're dealing with when calculating market values and quantities
					// below.
					ClientMarketData.SecurityRow securityRow =
						ClientMarketData.Security.FindBySecurityId(driverSecurity.SecurityId);

					// In this rebalancing operation, the cash balance is dependant on the securities bought and sold.  When
					// stocks are bought or sold below, they will impact the underlying currency.  We can not balance to a
					// currency target directly.
					if (securityRow.SecurityTypeCode == SecurityType.Currency)
						continue;

					// Calculate the proposed orders for each account.  The fraction of the security within the sector will
					// stay the same, even though the sector may increase or decrease with respect to the total market value.
					foreach (AppraisalSet.AccountRow driverAccount in driverPosition.GetAccountRows())
					{

						// The underlying currency is needed for the market value calculations.
						ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(driverAccount.AccountId);

						// Sector rebalancing keeps the percentage of a security within the sector constant.  Only the overall
						// percentage of the sector with respect to the NAV changes.  To accomplish this, we first calculate
						// the percentage of the security within the sector before we rebalance the sector.
						decimal actualPositionMarketValue = MarketValue.Calculate(currencyRow,
							accountRow, securityRow, driverPosition.PositionTypeCode,
							MarketValueFlags.EntirePosition);
					
						// Calculate the target market value as a percentage of the entire sector (use zero if the sector has 
						// no market value to prevent divide by zero errors).
						decimal targetPositionMarketValue = (actualSectorMarketValue == 0) ? 0.0M :
							actualPositionMarketValue * targetSectorMarketValue / actualSectorMarketValue;

						// The target proposed orders market value keeps the percentage of the position constant while 
						// changing the overall sector percentage.
						decimal proposedMarketValue = targetPositionMarketValue - MarketValue.Calculate(currencyRow,
							accountRow, securityRow, driverPosition.PositionTypeCode,
							MarketValueFlags.ExcludeProposedOrder);

						// Calculate the quantity needed to hit the target market value and round it according to the
						// model.  Note that the market values and prices are all denominated in the currency of the
						// parent account.  Also note the quantityFactor is needed for the proper quantity calculation.
						decimal proposedQuantity = proposedMarketValue / (Price.Security(currencyRow, securityRow) *
							securityRow.PriceFactor * securityRow.QuantityFactor);

						// If we have an equity, round to the model's lot size.
						if (securityRow.SecurityTypeCode == SecurityType.Equity)
							proposedQuantity = Math.Round(proposedQuantity / modelRow.EquityRounding, 0) *
								modelRow.EquityRounding;

						// A debt generally needs to be rounded to face.
						if (securityRow.SecurityTypeCode == SecurityType.Debt)
							proposedQuantity = Math.Round(proposedQuantity / modelRow.DebtRounding, 0) *
								modelRow.DebtRounding;

						// Have the Order Form Builder object construct an order based on the quantity we've calcuated from
						// the market value.  This method will fill in the defaults needed for a complete proposed order.
						ProposedOrder.Create(remoteBatch, remoteTransaction, accountRow, securityRow,
							driverAccount.PositionTypeCode, proposedQuantity);

					}

				}

			// Recurse into each of the sub-sectors.  This allows us to rebalance with any number of levels to the
			// hierarchy.  Eventually, we will run across a sector with security positions in it and end up doing some
			// real work.
			foreach (AppraisalSet.ObjectTreeRow driverTree in
				driverObject.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				SectorMerge.RecurseSectors(remoteBatch, remoteTransaction, currencyRow, modelRow,
					driverTree.ObjectRowByFKObjectObjectTreeChildId, actualSectorMarketValue,
					targetSectorMarketValue);

		}
		
		/// <summary>
		/// Rebalances an AppraisalModelSet to sector targets.  The model is applied to the aggregate market value of the
		/// account and it's children.
		/// </summary>
		/// <param name="accountId">The parent account to be rebalanced.</param>
		/// <param name="modelId">The sector model to be used.</param>
		/// <returns>A set of proposed orders.</returns>
		public static RemoteBatch Rebalance(ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow)
		{

			// Make sure the scheme still exists in the in-memory database.  We need it to rebalance to calculate
			// sector totals.
			ClientMarketData.SchemeRow schemeRow;
			if ((schemeRow = ClientMarketData.Scheme.FindBySchemeId(modelRow.SchemeId)) == null)
				throw new ArgumentException("Scheme doesn't exist in the ClientMarketData", modelRow.SchemeId.ToString());

			// All the market values need to be normalized to a single currency so the sectors can be aggregated. This
			// value is made available to all methods through a member rather than passed on the stack.
			ClientMarketData.CurrencyRow currencyRow = accountRow.CurrencyRow;
				
			// The final result of this method is a command batch that can be sent to the server.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

			// Calculate the total market value for the appraisal and all the sub-accounts.  This will be the denominator
			// in all calculations involving sector percentages.  This feature makes a 'Merge' rebalancer different from a
			// 'Wrap' rebalance.  The 'Wrap' uses the sub-account's market value as the denominator when calculating
			// sector market values.
			decimal accountMarketValue = MarketValue.Calculate(accountRow.CurrencyRow, accountRow,
				MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			// The outline of the appraisal will be needed to make calculations based on a position, that is a security,
			// account, position type combination grouped by a security classification scheme.
			AppraisalSet appraisalSet = new Appraisal(accountRow, schemeRow, true);
				
			// By cycling through all the immediate children of the scheme record, we'll have covered the top-level
			// sectors in this appraisal.
			foreach (AppraisalSet.SchemeRow driverScheme in appraisalSet.Scheme)
				foreach (AppraisalSet.ObjectTreeRow driverTree in driverScheme.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (AppraisalSet.SectorRow driverSector in driverTree.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
					{

						// The appraisal set collects the ids of the records used.  We need to look up the actual sector 
						// record from the ClientMarketData in order to search through it and aggregate sub-sectors and
						// securities.
						ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(driverSector.SectorId);

						// Get the market value of the top-level sector, including all subaccounts and all positions.
						decimal actualSectorMarketValue = MarketValue.Calculate(currencyRow, accountRow,
							sectorRow, MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

						// This will find the model percentage of the current top-level sector.  If the sector wasn't
						// specified in the model, assume a value of zero, which would indicate that we're to sell the
						// entire sector.
						ClientMarketData.SectorTargetRow sectorTargetRow =
							ClientMarketData.SectorTarget.FindByModelIdSectorId(modelRow.ModelId, driverSector.SectorId);
						decimal targetPercent = sectorTargetRow == null ? 0.0M : sectorTargetRow.Percent;

						// The target market value is calculated from the model percentage and the actual aggregate 
						// account market value.
						decimal targetSectorMarketValue = accountMarketValue * targetPercent;

						// Now that we have a target to shoot for, recursively descend into the structure calculating 
						// propsed orders.
						RecurseSectors(remoteBatch, remoteTransaction, currencyRow, modelRow, driverSector.ObjectRow,
							actualSectorMarketValue, targetSectorMarketValue);

					}

			// This object holds a complete set of proposed orders to achieve the sector targets in the model.
			return remoteBatch;

		}

	}

}
