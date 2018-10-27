/*************************************************************************************************************************
*
*	File:			MarketValue.cs
*	Description:	Methods used to calculate the market value of account, security and position.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using MarkThree.Quasar;
using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace MarkThree.Quasar
{

	/// <summary>
	/// A set of market value calculations for various object.
	/// </summary>
	public class MarketValue
	{

		/// <summary>
		/// Calculates the market value of the given Tax Lot in the base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="proposedOrderRow">The subject of the calculation.</param>
		/// <returns>The market value of the Tax Lot in the base currency.</returns>
		private static decimal Calculate(DataModel.CurrencyRow baseCurrency, DataModel.TaxLotRow taxLotRow)
		{

			return taxLotRow.Quantity * taxLotRow.SecurityRow.QuantityFactor *
				Price.Security(baseCurrency, taxLotRow.SecurityRow) * taxLotRow.SecurityRow.PriceFactor;

		}

		/// <summary>
		/// Calculates the market value of the given Proposed Order in the base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="proposedOrderRow">The subject of the calculation.</param>
		/// <returns>The market value of the Proposed Order in the base currency.</returns>
		private static decimal Calculate(DataModel.CurrencyRow baseCurrency,
			DataModel.ProposedOrderRow proposedOrderRow)
		{

			return proposedOrderRow.Quantity *
				proposedOrderRow.SecurityRowByFKSecurityProposedOrderSecurityId.QuantityFactor *
				Price.Security(baseCurrency, proposedOrderRow.SecurityRowByFKSecurityProposedOrderSecurityId) *
				proposedOrderRow.SecurityRowByFKSecurityProposedOrderSecurityId.PriceFactor *
				proposedOrderRow.TransactionTypeRow.QuantitySign;

		}

		/// <summary>
		/// Calculates the market value of the given Order in the base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="proposedOrderRow">The subject of the calculation.</param>
		/// <returns>The market value of the Order in the base currency.</returns>
		private static decimal Calculate(DataModel.CurrencyRow baseCurrency, DataModel.OrderRow SetPrice)
		{

			return SetPrice.Quantity *
				SetPrice.SecurityRowByFKSecurityOrderSecurityId.QuantityFactor *
				Price.Security(baseCurrency, SetPrice.SecurityRowByFKSecurityOrderSecurityId) *
				SetPrice.SecurityRowByFKSecurityOrderSecurityId.PriceFactor *
				SetPrice.TransactionTypeRow.QuantitySign;

		}

		/// <summary>
		/// Calculates the market value of the given Allocation in the base currency.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="proposedOrderRow">The subject of the calculation.</param>
		/// <returns>The market value of the Allocation in the base currency.</returns>
		private static decimal Calculate(DataModel.CurrencyRow baseCurrency,
			DataModel.AllocationRow allocationRow)
		{

			return allocationRow.Quantity *
				allocationRow.SecurityRowByFKSecurityAllocationSecurityId.QuantityFactor *
				Price.Security(baseCurrency, allocationRow.SecurityRowByFKSecurityAllocationSecurityId) *
				allocationRow.SecurityRowByFKSecurityAllocationSecurityId.PriceFactor *
				allocationRow.TransactionTypeRow.QuantitySign;

		}

		/// <summary>
		/// Calculates the market value of a position.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="accountRow">The identifier of the account.</param>
		/// <param name="SecurityId">The identifier of the security.</param>
		/// <param name="PositionTypeCode">The PositionTypeCode (long or short).</param>
		/// <param name="marketValueFlags">These flags direct what elements are included and whether to include
		/// children.</param>
		/// <returns></returns>
		public static decimal Calculate(DataModel.CurrencyRow baseCurrency, DataModel.AccountRow accountRow,
			DataModel.SecurityRow securityRow, int PositionTypeCode, MarketValueFlags marketValueFlags)
		{

			// This is the accumulator for market value.
			decimal marketValue = 0.0M;

			// Aggregate Tax Lots
			if ((marketValueFlags & MarketValueFlags.IncludeTaxLot) != 0)
				foreach (DataModel.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
					if (taxLotRow.SecurityId == securityRow.SecurityId &&
						taxLotRow.PositionTypeCode == PositionTypeCode)
						marketValue += Calculate(baseCurrency, taxLotRow);

			// Aggregate Proposed Order
			if ((marketValueFlags & MarketValueFlags.IncludeProposedOrder) != 0)
				foreach (DataModel.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
					if (proposedOrderRow.SecurityId == securityRow.SecurityId &&
						proposedOrderRow.PositionTypeCode == PositionTypeCode)
						marketValue += Calculate(baseCurrency, proposedOrderRow);

			// Aggregate Order
			if ((marketValueFlags & MarketValueFlags.IncludeOrder) != 0)
				foreach (DataModel.OrderRow SetPrice in accountRow.GetOrderRows())
					if (SetPrice.SecurityId == securityRow.SecurityId &&
						SetPrice.PositionTypeCode == PositionTypeCode)
						marketValue += Calculate(baseCurrency, SetPrice);

			// Aggregate Allocation
			if ((marketValueFlags & MarketValueFlags.IncludeAllocation) != 0)
				foreach (DataModel.AllocationRow allocationRow in accountRow.GetAllocationRows())
					if (allocationRow.SecurityId == securityRow.SecurityId &&
						allocationRow.PositionTypeCode == PositionTypeCode)
						marketValue += Calculate(baseCurrency, allocationRow);

			// If sub-account are to be included, recurse into the account structures.
			if ((marketValueFlags & MarketValueFlags.IncludeChildAccounts) != 0)
				foreach (DataModel.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (DataModel.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						marketValue += Calculate(baseCurrency, childAccount, securityRow,
							PositionTypeCode, marketValueFlags);

			// This is the market value of the given account/security/position combination.
			return marketValue;

		}

		/// <summary>
		/// Calculates the market value of a sector in an account.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="accountRow">The identifier of the account.</param>
		/// <param name="SecurityId">The identifier of the security.</param>
		/// <param name="marketValueFlags">These flags direct what elements are included and whether to include
		/// children.</param>
		/// <returns>The market value of the position.</returns>
		public static decimal Calculate(DataModel.CurrencyRow baseCurrency, DataModel.AccountRow accountRow,
			DataModel.SectorRow sectorRow, MarketValueFlags marketValueFlags)
		{

			// This is the accumulator for market value.
			decimal marketValue = 0.0M;

			// Aggregate the market value of sub-sector.
			foreach (DataModel.ObjectTreeRow objectTreeRow in
				sectorRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (DataModel.SectorRow childSector in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
					marketValue += Calculate(baseCurrency, accountRow, childSector, marketValueFlags);

			// Aggregate the market value of all security belonging to this sector.
			foreach (DataModel.ObjectTreeRow objectTreeRow in
				sectorRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (DataModel.SecurityRow childSecurity in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetSecurityRows())
				{

					// Aggregate the Long Position in this security in this account.
					marketValue += Calculate(baseCurrency, accountRow, childSecurity, PositionType.Long,
						marketValueFlags);

					// Aggregate the Short Position in this security in this account.
					marketValue += Calculate(baseCurrency, accountRow, childSecurity, PositionType.Short,
						marketValueFlags);

				}
		
			// This is the market value of the given account/sector combination.
			return marketValue;

		}
			
		/// <summary>
		/// Calculates the market market value of the account and, optinally, all the sub-account.
		/// </summary>
		/// <param name="baseCurrency">The identifier of the base currency.</param>
		/// <param name="accountRow">The identifier of the account.</param>
		/// <returns>The market value of the account and, optinally, all the sub account in the base currency.</returns>
		public static decimal Calculate(DataModel.CurrencyRow baseCurrency, DataModel.AccountRow accountRow,
			MarketValueFlags marketValueFlags)
		{

			// This is the accumulator for market value.
			decimal marketValue = 0.0M;

			// Add up all the taxLot.
			if ((marketValueFlags & MarketValueFlags.IncludeTaxLot) != 0)
				foreach (DataModel.TaxLotRow taxLotRow in accountRow.GetTaxLotRows())
					marketValue += Calculate(baseCurrency, taxLotRow);

			// Add up all the proposedOrder.
			if ((marketValueFlags & MarketValueFlags.IncludeProposedOrder) != 0)
				foreach (DataModel.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
					marketValue += Calculate(baseCurrency, proposedOrderRow);

			// Add up all the order.
			if ((marketValueFlags & MarketValueFlags.IncludeOrder) != 0)
				foreach (DataModel.OrderRow SetPrice in accountRow.GetOrderRows())
					marketValue += Calculate(baseCurrency, SetPrice);

			// Add up all the allocation.
			if ((marketValueFlags & MarketValueFlags.IncludeAllocation) != 0)
				foreach (DataModel.AllocationRow allocationRow in accountRow.GetAllocationRows())
					marketValue += Calculate(baseCurrency, allocationRow);

			// Add in the market value of the sub-account.
			if ((marketValueFlags & MarketValueFlags.IncludeChildAccounts) != 0)
				foreach (DataModel.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (DataModel.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						marketValue += Calculate(baseCurrency, childAccount, marketValueFlags);

			// This is the market value of the given account in the base currency.
			return marketValue;

		}

	}

}
