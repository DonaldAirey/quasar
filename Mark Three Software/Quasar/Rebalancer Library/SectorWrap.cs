/*************************************************************************************************************************
*
*	File:			SecurityLevelRebalancer.cs
*	Description:	This module contains the algorithm for rebalancing an appraisal to sector sector targets.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Client;
using Shadows.Quasar.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Shadows.Quasar.Library.Rebalancer
{

	/// <summary>
	/// Reblancing according to percentages of securities.
	/// </summary>
	public class SectorWrap
	{

		/// <summary>
		/// Recursively calculates proposed orders for a sector.
		/// </summary>
		/// <param name="sector">Gives the current sector (sector) for the calculation.</param>
		private static void RecurseSectors(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction, ClientMarketData.ModelRow modelRow,
			AppraisalSet.SectorRow driverSector, decimal actualSectorMarketValue, decimal targetSectorMarketValue)
		{

			// The main idea here is to keep the ratio of the security to the sector constant, while changing the market
			// value of the sector.  Scan each of the securities belonging to this sector.
			foreach (AppraisalSet.ObjectTreeRow objectTreeRow in
				driverSector.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
			{

				// Cycle through each of the securities in the sector.  We're going to keep the ratio of the security the
				// same as we target a different sector total.
				foreach (AppraisalSet.SecurityRow driverSecurity in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSecurityRows())
					foreach (AppraisalSet.PositionRow driverPosition in driverSecurity.GetPositionRows())
					{

						// We need to reference the security record for calculating proposed orders and the market value
						// of the trade.
						ClientMarketData.SecurityRow securityRow =
							ClientMarketData.Security.FindBySecurityId(driverSecurity.SecurityId);

						// In this rebalancing operation, the cash balance is dependant on the securities bought and 
						// sold.  When stocks are bought or sold below, they will impact the underlying currency.  A cash
						// target can be reached by setting all the other percentages up properly.  As long as the total
						// percentage in a model is 100%, the proper cash target will be calculated.  We don't have to do
						// anything with this asset type.
						if (securityRow.SecurityTypeCode == SecurityType.Currency)
							continue;

						// The ratio of the security within the sector will stay constant, even though the sector may 
						// increase or decrease with the target in the model.  Note that there's only one account in the
						// 'Accounts' table of the driver because this is a 'Wrap' operation.
						foreach (AppraisalSet.AccountRow driverAccount in driverPosition.GetAccountRows())
						{

							// Find the account associated with the driver record.
							ClientMarketData.AccountRow accountRow =
								ClientMarketData.Account.FindByAccountId(driverAccount.AccountId);

							// The market value of all the securities are normalized to the base currency of the account 
							// so they can be aggregated.
							ClientMarketData.CurrencyRow currencyRow =
								ClientMarketData.Currency.FindByCurrencyId(accountRow.CurrencyId);

							// Sector rebalancing keeps the percentage of a security within the sector constant.  Only the
							// overall percentage of the sector with respect to the NAV changes.  The first step in this
							// rebalancing operation is to calculate the market value of the given position.
							decimal actualPositionMarketValue = MarketValue.Calculate(currencyRow, accountRow,
								securityRow, driverPosition.PositionTypeCode, MarketValueFlags.EntirePosition);
					
							// The target market value operation keeps the percentage of the position constant while
							// changing the overall sector percentage.
							decimal targetPositionMarketValue = (actualSectorMarketValue == 0) ? 0.0M :
								actualPositionMarketValue * targetSectorMarketValue / actualSectorMarketValue;

							// Calculate the market value of an order that will achieve the target.  Note that we're not
							// including the existing proposed orders in the market value, but we did include them when
							// calculating the account's market value.  This allows us to put in what-if orders that will
							// impact the market value before we do the rebalancing.
							decimal proposedMarketValue = targetPositionMarketValue - MarketValue.Calculate(currencyRow,
								accountRow, securityRow, driverPosition.PositionTypeCode,
								MarketValueFlags.ExcludeProposedOrder);

							// Calculate the quantity needed to hit the target market value and round it according to the
							// model.  Note that the market values and prices are all denominated in the currency of the
							// parent account.  Also note the quantityFactor is needed for the proper quantity 
							// calculation.
							decimal proposedQuantity = proposedMarketValue /
								(Price.Security(currencyRow, securityRow) * securityRow.QuantityFactor);

							// If we have an equity, round to the model's lot size.
							if (securityRow.SecurityTypeCode == SecurityType.Equity)
								proposedQuantity = Math.Round(proposedQuantity / modelRow.EquityRounding, 0) *
									modelRow.EquityRounding;

							// A debt generally needs to be rounded to face.
							if (securityRow.SecurityTypeCode == SecurityType.Debt)
								proposedQuantity = Math.Round(proposedQuantity / modelRow.DebtRounding, 0) *
									modelRow.DebtRounding;

							// Have the OrderForm object construct an order based on the quantity we've calcuated 
							// from the market value.  This will fill in the defaults for the order and translate the
							// signed quantities into transaction codes.
							ProposedOrder.Create(remoteBatch, remoteTransaction, accountRow, securityRow,
								driverAccount.PositionTypeCode, proposedQuantity);

						}

					}

				// Recurse into each of the sub-sectors.  This allows us to rebalance with any number of levels to the
				// hierarchy.  Eventually, we will run across a sector with security positions in it and end up doing some
				// real work.
				foreach (AppraisalSet.SectorRow childSector in objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
					SectorWrap.RecurseSectors(remoteBatch, remoteTransaction, modelRow, childSector, actualSectorMarketValue,  targetSectorMarketValue);

			}

		}
		
		/// <summary>
		/// Rebalances an account to the sector targets, then recursively rebalances the children accounts.
		/// </summary>
		/// <param name="orderFormBuilder">A collection of orders.</param>
		/// <param name="accountRow">The parent account to be rebalanced.</param>
		/// <param name="modelRow">The model containing the sector targets.</param>
		/// <param name="schemeRow">The outline scheme used to define the sector contents.</param>
		private static void RecurseAccounts(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow, ClientMarketData.SchemeRow schemeRow)
		{

			// All the market values of all the securities in this account are normalized to a single currency so they can
			// be aggregated.
			ClientMarketData.CurrencyRow currencyRow = ClientMarketData.Currency.FindByCurrencyId(accountRow.CurrencyId);

			// Calculate the total market value for the appraisal without including child accounts.  This is a 'Wrap'
			// rebalancing, so we're only concerned with what's in this account.  The account's market value will be the
			// denominator in all calculations involving sector percentages.
			decimal accountMarketValue = MarketValue.Calculate(currencyRow, accountRow,
				MarketValueFlags.EntirePosition);

			// The outline of the appraisal will be needed to make market value calculations based on a sector.  Note that
			// we're not including the child accounts in the outline.  Wrap rebalancing works only on a single account at
			// a time.
			AppraisalSet appraisalSet = new Appraisal(accountRow, schemeRow, false);
				
			// By cycling through all the immediate children of the scheme record, we'll have covered the top-level
			// sectors in this appraisal.
			foreach (AppraisalSet.SchemeRow driverScheme in appraisalSet.Scheme)
				foreach (AppraisalSet.ObjectTreeRow driverTree in
					driverScheme.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (AppraisalSet.SectorRow driverSector in
						driverTree.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
					{

						// Find the sectors row record that corresponds to the current sector in the appraisal set.
						ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(driverSector.SectorId);
						
						// Get the market value of the top-level sector, including all sub-sectors and all positions 
						// belonging to only the current account.
						decimal actualSectorMarketValue = MarketValue.Calculate(currencyRow, accountRow, sectorRow,
							MarketValueFlags.EntirePosition);

						// This will find the model percentage of the current top-level sector.  If the sector wasn't
						// specified in the model, assume a value of zero, which would indicate that we're to sell the
						// entire sector.
						ClientMarketData.SectorTargetRow sectorTargetRow =
							ClientMarketData.SectorTarget.FindByModelIdSectorId(modelRow.ModelId, driverSector.SectorId);
						decimal targetPercent = (sectorTargetRow == null) ? 0.0M : sectorTargetRow.Percent;

						// The sector's target market value is calculated from the model percentage and the current 
						// account market value.  This is placed in a member variable so it's available to the methods
						// when we recurse.
						decimal targetSectorMarketValue = accountMarketValue * targetPercent;

						// Now that we have a sector target to shoot for, recursively descend into the structure 
						// calculating proposed orders.
						SectorWrap.RecurseSectors(remoteBatch, remoteTransaction, modelRow, driverSector, actualSectorMarketValue,
							targetSectorMarketValue);

					}

			// Now that we've rebalanced the parent account, cycle through all the children accounts and rebalance them.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					SectorWrap.RecurseAccounts(remoteBatch, remoteTransaction, childAccount, modelRow, schemeRow);
		
		}
		
		/// <summary>
		/// Returns a set of orders that will achieve the targets specified by the model.
		/// </summary>
		/// <param name="accountRow">The account or parent account to be rebalanced.</param>
		/// <param name="modelRow">The target percentages to use for rebalancing.</param>
		/// <returns>A Dataset of new, updated and deleted orders.</returns>
		public static RemoteBatch Rebalance(ClientMarketData.AccountRow accountRow, ClientMarketData.ModelRow modelRow)
		{

			// Make sure the scheme still exists in the in-memory database.  We need it to rebalance the appraisal.
			ClientMarketData.SchemeRow schemeRow;
			if ((schemeRow = ClientMarketData.Scheme.FindBySchemeId(modelRow.SchemeId)) == null)
				throw new ArgumentException("Scheme doesn't exist in the ClientMarketData", modelRow.SchemeId.ToString());
			
			// The final result of this method is a command batch that can be sent to the server.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

			// Rebalance the parent account and all it's children.
			RecurseAccounts(remoteBatch, remoteTransaction, accountRow, modelRow, schemeRow);

			// The sucessful result of rebalancing.
			return remoteBatch;
		
		}

	}

}
