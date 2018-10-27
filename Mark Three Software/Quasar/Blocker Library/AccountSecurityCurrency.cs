/*************************************************************************************************************************
*
*	File:			SecurityBlocker.cs
*	Description:	Method to block orders by the account, security and currency attributes.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Library.Blocker
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Server;
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Method to block orders by the security attribute.
	/// </summary>
	public class AccountSecurityCurrency
	{

		/// <summary>
		/// Find a block order by the account, security and currency attributes.
		/// </summary>
		/// <param name="sqlConnection">A connection to the database server.</param>
		/// <param name="sqlTransaction">The transaction to use if a block order is created.</param>
		/// <param name="commitList">The new block order will be added to this list for the final commit or rollback.</param>
		/// <param name="blotterId">The source or destination blotter for the block order.</param>
		/// <param name="accountId">The account attribute</param>
		/// <param name="securityId">The security attribute</param>
		/// <param name="settlementId">The currency attribute</param>
		/// <param name="transactionTypeCode">The transaction type attribute</param>
		/// <param name="timeInForceCode">The time in force attribute</param>
		/// <param name="orderTypeCode">The type of order attribute</param>
		/// <param name="conditionCode">Conditions associated with the trade</param>
		/// <param name="price1">The limit or stop price</param>
		/// <param name="price2">The stop-limit price</param>
		/// <returns>A block order that matches the blocking algorithm and attributes.</returns>
		public static int Open(Transaction transaction, int blotterId, object accountId, int securityId, int settlementId,
			object brokerId, int transactionTypeCode, object timeInForceCode, object orderTypeCode,
			object conditionCode, object price1, object price2)
		{

			ServerMarketData.BlockOrderRow blockOrderRow;

			// Create a key based on the security and the transaction code.  This is the primary index on the view that
			// was created for finding block orders based on attributes.
			object[] key = new object[] {blotterId, securityId, transactionTypeCode};

			// Search each row that matches the security/transaction code key.
			foreach (DataRowView dataRowView in ServerMarketData.BlockOrder.UKBlockOrderBlotterIdSecurityIdTransactionTypeCode.FindRows(key))
			{

				// This is a shortcut to the current row of the iteration.
				blockOrderRow = (ServerMarketData.BlockOrderRow)dataRowView.Row;

				// If the block hasn't been touched yet and the account and currency id are matchs, then this block is a 
				// match for this blocking algorithm.
				if (blockOrderRow.StatusCode == Status.New && !blockOrderRow.IsAccountIdNull() && accountId != null &&
					blockOrderRow.AccountId == (int)accountId && blockOrderRow.SettlementId == settlementId)
					return blockOrderRow.BlockOrderId;

			}

			long rowVersion = 0;
			bool agency = false;
			DateTime createdTime = DateTime.Now;
			int createdLoginId = ServerMarketData.LoginId;

			return Shadows.WebService.Core.BlockOrder.Insert(transaction, blotterId, accountId, securityId, settlementId,
				brokerId, Status.New, transactionTypeCode, timeInForceCode, orderTypeCode,
				conditionCode, ref rowVersion, 0.0M, 0.0M, false, agency, price1, price2, createdTime, createdLoginId,
				createdTime, createdLoginId);

		}

	}

}
