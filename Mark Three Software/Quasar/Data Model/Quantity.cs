/*************************************************************************************************************************
*
*	File:			Quantity.cs
*	Description:	Calculates Various Quantity values.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using MarkThree.Quasar;
using System;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Calculates Various Quantity values.
	/// </summary>
	public class Quantity
	{

		/// <summary>
		/// Parses a quantity from a string.
		/// </summary>
		/// <param name="stringQuantity">Contains the text that is parsed into a decimal value.</param>
		/// <returns>The text string converted into a decimal value.</returns>
		public static decimal Parse(string stringQuantity)
		{

			Match match;

			// Don't try to parse and empty string, it ends up confusing the parser and generating an exception.
			if (stringQuantity != String.Empty)
			{

				// Attempt to match the input string against a decimal number with a percent sign '%' after it.  If a 
				// match is found, convert the numeric portion of the string to a decimal number and return it after
				// applying the percent factor.  This matching should be exactly the same as Microsoft Excel's input
				// handler for percent formatted cells.
				match = Regex.Match(stringQuantity, @"(?<quantity>[\-\+]?\d*\.?\d*)[mM][mM]");
				if (match.Success)
					return Convert.ToDecimal(match.Groups["quantity"].Value) * 1000000.0M;

				// Attempt to match the input string against a decimal number with a percent sign '%' after it.  If a 
				// match is found, convert the numeric portion of the string to a decimal number and return it after
				// applying the percent factor.  This matching should be exactly the same as Microsoft Excel's input
				// handler for percent formatted cells.
				match = Regex.Match(stringQuantity, @"(?<quantity>[\-\+]?\d*\.?\d*)[mM]");
				if (match.Success)
					return Convert.ToDecimal(match.Groups["quantity"].Value) * 1000.0M;

				// Attempt to match the input string against a decimal number with a percent sign '%' after it.  If a 
				// match is found, convert the numeric portion of the string to a decimal number and return it after
				// applying the percent factor.  This matching should be exactly the same as Microsoft Excel's input
				// handler for percent formatted cells.
				match = Regex.Match(stringQuantity, @"(?<quantity>[\-\+]?\d*\.?\d*)");
				if (match.Success)
					return Convert.ToDecimal(match.Groups["quantity"].Value);

			}

			// If the input string didn't match any of the above patters, return zero as a default value.
			return 0.0M;

		}

		/// <summary>
		/// Calculates the number of shares in a given position.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="AccountId">The identifier of the account.</param>
		/// <param name="SecurityId">The identifier of the security.</param>
		/// <param name="positionTypeCode">The PositionTypeCode (long or short).</param>
		/// <param name="quantityFlags">These flags direct what elements are included and whether to include
		/// children.</param>
		/// <returns></returns>
		public static decimal Calculate(DataModel.AccountRow accountRow, DataModel.SecurityRow securityRow,
			int positionTypeCode, MarketValueFlags quantityFlags)
		{

			// This is the accumulator for market value.
			decimal quantity = 0.0M;

			// This key is used to find records that match the given position.
			object[] key = new object[] {accountRow.AccountId, securityRow.SecurityId, positionTypeCode};
			
			// Aggregate Tax Lots
			if ((quantityFlags & MarketValueFlags.IncludeTaxLot) != 0)
				foreach (DataRowView dataRowView in DataModel.TaxLot.UKTaxLotAccountIdSecurityIdPositionTypeCode.FindRows(key))
                    quantity += ((DataModel.TaxLotRow)dataRowView.Row).Quantity;
			
			// Aggregate Proposed Order
			if ((quantityFlags & MarketValueFlags.IncludeProposedOrder) != 0)
				foreach (DataRowView dataRowView in DataModel.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.ProposedOrderRow proposedProposedOrderRow = (DataModel.ProposedOrderRow)dataRowView.Row;
					quantity += proposedProposedOrderRow.Quantity * proposedProposedOrderRow.TransactionTypeRow.QuantitySign;
				}

			// Aggregate Order
			if ((quantityFlags & MarketValueFlags.IncludeOrder) != 0)
				foreach (DataRowView dataRowView in DataModel.Order.UKOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.OrderRow orderRow = (DataModel.OrderRow)dataRowView.Row;
					quantity += orderRow.Quantity * orderRow.TransactionTypeRow.QuantitySign;
				}

			// Aggregate Allocation
			if ((quantityFlags & MarketValueFlags.IncludeAllocation) != 0)
				foreach (DataRowView dataRowView in DataModel.Allocation.UKAllocationAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.AllocationRow allocationRow = (DataModel.AllocationRow)dataRowView.Row;
					quantity += allocationRow.Quantity * allocationRow.TransactionTypeRow.QuantitySign;
				}

			// If sub-account are to be included, recurse into the account structures.
			if ((quantityFlags & MarketValueFlags.IncludeChildAccounts) != 0)
				foreach (DataModel.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (DataModel.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						quantity += Calculate(childAccount, securityRow, positionTypeCode, quantityFlags);

			// This is the market value of the given account/security/position combination.
			return quantity;

		}

		/// <summary>
		/// Calculates the number of shares in a given position.
		/// </summary>
		/// <param name="baseCurrency">The base currency for the market value calculation.</param>
		/// <param name="AccountId">The identifier of the account.</param>
		/// <param name="SecurityId">The identifier of the security.</param>
		/// <param name="positionTypeCode">The PositionTypeCode (long or short).</param>
		/// <param name="quantityFlags">These flags direct what elements are included and whether to include
		/// children.</param>
		/// <returns></returns>
		public static decimal Calculate(int accountId, int securityId, int positionTypeCode, MarketValueFlags quantityFlags)
		{

			// This is the accumulator for market value.
			decimal quantity = 0.0M;

			// This key is used to find records that match the given position.
			object[] key = new object[] {accountId, securityId, positionTypeCode};
			
			// Aggregate Tax Lots
			if ((quantityFlags & MarketValueFlags.IncludeTaxLot) != 0)
				foreach (DataRowView dataRowView in DataModel.TaxLot.UKTaxLotAccountIdSecurityIdPositionTypeCode.FindRows(key))
					quantity += ((DataModel.TaxLotRow)dataRowView.Row).Quantity;
			
			// Aggregate Proposed Order
			if ((quantityFlags & MarketValueFlags.IncludeProposedOrder) != 0)
				foreach (DataRowView dataRowView in DataModel.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.ProposedOrderRow proposedProposedOrderRow = (DataModel.ProposedOrderRow)dataRowView.Row;
					quantity += proposedProposedOrderRow.Quantity * proposedProposedOrderRow.TransactionTypeRow.QuantitySign;
				}

			// Aggregate Order
			if ((quantityFlags & MarketValueFlags.IncludeOrder) != 0)
				foreach (DataRowView dataRowView in DataModel.Order.UKOrderAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.OrderRow orderRow = (DataModel.OrderRow)dataRowView.Row;
					quantity += orderRow.Quantity * orderRow.TransactionTypeRow.QuantitySign;
				}

			// Aggregate Allocation
			if ((quantityFlags & MarketValueFlags.IncludeAllocation) != 0)
				foreach (DataRowView dataRowView in DataModel.Allocation.UKAllocationAccountIdSecurityIdPositionTypeCode.FindRows(key))
				{
					DataModel.AllocationRow allocationRow = (DataModel.AllocationRow)dataRowView.Row;
					quantity += allocationRow.Quantity * allocationRow.TransactionTypeRow.QuantitySign;
				}

			// If sub-account are to be included, recurse into the account structures.
			if ((quantityFlags & MarketValueFlags.IncludeChildAccounts) != 0)
			{

				DataModel.AccountRow accountRow = DataModel.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(string.Format("Account {0} does not exists", accountId));

				foreach (DataModel.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (DataModel.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						quantity += Calculate(childAccount.AccountId, securityId, positionTypeCode, quantityFlags);

			}

			// This is the market value of the given account/security/position combination.
			return quantity;

		}

		/// <summary>
		/// Calculates the quantity of child order for a given position
		/// </summary>
		/// <param name="AccountId">The identifier of the account.</param>
		/// <param name="SecurityId">The identifier of the security.</param>
		/// <param name="positionTypeCode">The positionTypeCode (long or short).</param>
		/// <returns>The quantity of the child proposedOrder.</returns>
		public static decimal CalculateChildProposedQuantity(DataModel.AccountRow accountRow, DataModel.SecurityRow securityRow,
			int positionTypeCode)
		{

			// This is the accumulator for market value.
			decimal quantity = 0.0M;

			// Aggregate Proposed Order
			foreach (DataModel.ProposedOrderRow proposedOrderRow in accountRow.GetProposedOrderRows())
				if (Relationship.IsChildProposedOrder(proposedOrderRow) &&
					proposedOrderRow.SecurityId == securityRow.SecurityId &&
					proposedOrderRow.PositionTypeCode == positionTypeCode)
					quantity += proposedOrderRow.Quantity * proposedOrderRow.TransactionTypeRow.QuantitySign;

			// If sub-account are to be included, recurse into the account structures.
			foreach (DataModel.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (DataModel.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					quantity += CalculateChildProposedQuantity(childAccount, securityRow, positionTypeCode);

			// This is the market value of the given account/security/position combination.
			return quantity;

		}

	}

}
