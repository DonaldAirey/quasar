/*************************************************************************************************************************
*
*	File:			ProposedOrder.cs
*	Description:	This module contains the command to clear all the proposed orders from an appraisal.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Client;
using Shadows.Quasar.Common;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace Shadows.Quasar.Viewers.Appraisal
{

	/// <summary>
	/// Clears the proposed orders from an appraisal.
	/// </summary>
	public class ProposedOrder
	{

		/// <summary>
		/// Clears all proposed orders for the viewed account and it's children.
		/// </summary>
		public static void ClearAccount(params object[] argument)
		{

			// Extract the thread argument
			int accountId = (int)argument[0];

			// This batch of commands will be populated with all the commands needed to delete account of all it's proposed orders.
			RemoteBatch remoteBatch = null;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Make sure the account record still exists.
				ClientMarketData.AccountRow accountRow;
				if ((accountRow = ClientMarketData.Account.FindByAccountId(accountId)) == null)
					throw new Exception(String.Format("Account {0} has been deleted", accountId));

				// The command batch will contain a single transaction.  All the proposed orders and the relationships between them
				// must be deleted as a unit or the transaction will be rejected.  The command batch also needs to know what
				// assembly and what object types have to be instantiated in order to call the methods to delete the proposed
				// orders.  This effectively sets up the header of the command batch so the methods below can recursively create
				// commands to drill down into the account groups and the relations between parent a child orders.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

				// The relations must be deleted first.  This will create commands to delete all the parent/child order relations 
				// found in the account group.
				ProposedOrder.DeleteRelationship(remoteBatch, remoteTransaction, accountRow);

				// This will recursively create comands to delete the proposed orders in the account.
				ProposedOrder.Delete(remoteBatch, remoteTransaction, accountRow);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// The server won't be called if any part of the batch construction fails.
				remoteBatch = null;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderTreeLock.IsReaderLockHeld) ClientMarketData.ProposedOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Clears all proposed orders for the viewed account and it's children.
		/// </summary>
		public static void ClearPosition(params object[] argument)
		{

			// This batch of commands will be populated with all the commands needed to delete account of all it's proposed orders.
			RemoteBatch remoteBatch = null;

			try
			{

				// Extract the thread parameters.
				ArrayList selectedPositions = (ArrayList)argument[0];

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// The command batch will contain a single transaction.  All the proposed orders and the relationships between them
				// must be deleted as a unit or the transaction will be rejected.  The command batch also needs to know what
				// assembly and what object types have to be instantiated in order to call the methods to delete the proposed
				// orders.  This effectively sets up the header of the command batch so the methods below can recursively create
				// commands to drill down into the account groups and the relations between parent a child orders.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

				// For each selected position, create a proposed order command in the command batch.
				foreach (Position position in selectedPositions)
				{

					// Find the account record and make sure it still exists.
					ClientMarketData.AccountRow accountRow;
					if ((accountRow = ClientMarketData.Account.FindByAccountId(position.AccountId)) == null)
						throw new Exception(String.Format("Account {0} has been deleted", position.AccountId));

					// Find the security record and make sure it still exists.
					ClientMarketData.SecurityRow securityRow;
					if ((securityRow = ClientMarketData.Security.FindBySecurityId(position.SecurityId)) == null)
						throw new Exception(String.Format("Security {0} has been deleted", position.SecurityId));

					// Delete the position.
					ProposedOrder.Delete(remoteBatch, remoteTransaction, accountRow, securityRow, position.PositionTypeCode);

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// The server won't be called if any part of the batch construction fails.
				remoteBatch = null;

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderTreeLock.IsReaderLockHeld) ClientMarketData.ProposedOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);
		
		}

		/// <summary>
		/// This thread will update a proposed order.
		/// </summary>
		/// <param name="objectStart">A structure containing the parameters for the call.</param>
		public static void Update(params object[] argument)
		{

			// This batch of commands will be used for this operation.
			RemoteBatch remoteBatch = null;

			// Make sure that the table locks are released.		
			try
			{

				// Unpack the parameters for the thread.
				int accountId = (int)argument[0];
				int securityId = (int)argument[1];
				int positionTypeCode = (int)argument[2];
				decimal quantity = (decimal)argument[3];

				// Lock the spreadsheet and tables while we adjust this order and display it.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterMapLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Find the account record in the data model.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", accountId));

				// Find the security record in the data model.
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(securityId);
				if (securityRow == null)
					throw new ArgumentException(String.Format("Security {0} has been deleted", securityId));

				// The command batch will contain a single transaction.  All the proposed orders and the relationships between them
				// must be deleted as a unit or the transaction will be rejected.  The command batch also needs to know what
				// assembly and what object types have to be instantiated in order to call the methods to add and delete the
				// proposed orders.  This section of code effectively sets up the header of the command batch so the methods below 
				// can recursively create commands to drill down into the account groups and the relations between parent a child
				// orders.  When the operation is complete, a complex command batch is ready to be sent to the server.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

				// Create a proposed order -- with the associated settlement currency transaction -- from the user's entry.  This
				// will also clear out any multiple proposed orders for the same position in the parent account.
				ProposedOrder.Create(remoteBatch, remoteTransaction, accountRow, securityRow, positionTypeCode, quantity);

				// Clear out all proposed orders the children for this position.  When a user enters a value directly into the
				// cell, we assume that it is a single order for the parent that will be allocated back to the children later.  To 
				// make this operation intuitive, all the child accounts need to be cleared of this position.
				foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
					accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
					foreach (ClientMarketData.AccountRow childAccount in
						objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
						ProposedOrder.Delete(remoteBatch, remoteTransaction, childAccount, securityRow, positionTypeCode);


			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

				// The server won't be called if any part of the batch construction fails.
				remoteBatch = null;

			}
			finally
			{

				// Release the locks on the spreadsheet and tables.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderTreeLock.IsReaderLockHeld) ClientMarketData.ProposedOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterMapLock.IsReaderLockHeld) ClientMarketData.BlotterMapLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Recursively constructs a set of orders from a parent position.
		/// </summary>
		/// <param name="proposedOrdersSet">Destination for the proposed orders</param>
		/// <param name="accountRow">The parent account record</param>
		/// <param name="securityRow">The security</param>
		/// <param name="positionTypeCode">The position type</param>
		private static void RecurseAddProposedOrders(RemoteBatch remoteBatch, ClientMarketData.AccountRow accountRow, ClientMarketData.SecurityRow securityRow,
			int positionTypeCode)
		{

			// Assemblies and Types used in the RemoteBatch.
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");
			RemoteType orderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.OrderTree");
			RemoteType blockOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.BlockOrder");
			RemoteAssembly tradingAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType orderType = tradingAssembly.Types.Add("Shadows.WebService.Trading.Order");

			// Iterate through every position that matches the selected criteria and construct a transaction to delete the 
			// proposed order and create a blotter order from the same information.
			object[] key = new object[] {accountRow.AccountId, securityRow.SecurityId, positionTypeCode};
			DataRowView[] rowArray = ClientMarketData.ProposedOrder.UKProposedOrderAccountIdSecurityIdPositionTypeCode.FindRows(key);
			foreach (DataRowView dataRowView in rowArray)
			{

				// This is a shorthand notation for the Proposed Order that matches the criteria in the search above.
				ClientMarketData.ProposedOrderRow parentProposedOrder = (ClientMarketData.ProposedOrderRow)dataRowView.Row;

				// If this is a child order, then ignore it.  Child orders can only be added when it's parent is added.
				if (Relationship.IsChildProposedOrder(parentProposedOrder))
					continue;

				// At this point the proposed order matches all the tests for being turned into an order on the blotter.  The 
				// operation of deleting the proposed order and creating a trading order has to be done as a unit.  This
				// transaction will insure that there are no redundant orders left around in the event of an exception.
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

				// The proposed orders need to be deleted in the same transaction that creates the orders.  This will guarantee
				// that there are no redundant orders sitting around, even temporarily.  In order to preserve referential
				// integrity, the proposed orders must be deleted in the opposite order they were created.  That is, the
				// relationship between the orders must be deleted first, becuase it references the proposed orders.  Then the
				// parent and the child must be deleted.  Note that this is all part of a complex transaction that is committed or
				// rejected as a unit of work.
				foreach (ClientMarketData.ProposedOrderTreeRow proposedOrderTreeRow in
					parentProposedOrder.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId())
				{

					// Shorthand notation for the current proposed order in the list of children.
					ClientMarketData.ProposedOrderRow childProposedOrder =
						proposedOrderTreeRow.ProposedOrderRowByFKProposedOrderProposedOrderTreeChildId;

					// Delete the relationship between the parent proposed order and its child.
					RemoteMethod relationDelete = proposedOrderTreeType.Methods.Add("Delete");
					relationDelete.Transaction = remoteTransaction;
					relationDelete.Parameters.Add("rowVersion", proposedOrderTreeRow.RowVersion);
					relationDelete.Parameters.Add("parentId", proposedOrderTreeRow.ParentId);
					relationDelete.Parameters.Add("childId", proposedOrderTreeRow.ChildId);
					
					// Delete the child proposed order.
					RemoteMethod childDelete = proposedOrderType.Methods.Add("Delete");
					childDelete.Transaction = remoteTransaction;
					childDelete.Parameters.Add("rowVersion", childProposedOrder.RowVersion);
					childDelete.Parameters.Add("proposedOrderId", childProposedOrder.ProposedOrderId);

				}
				
				// Delete the parent proposed order.
				RemoteMethod parentDelete = proposedOrderType.Methods.Add("Delete");
				parentDelete.Transaction = remoteTransaction;
				parentDelete.Parameters.Add("rowVersion", parentProposedOrder.RowVersion);
				parentDelete.Parameters.Add("proposedOrderId", parentProposedOrder.ProposedOrderId);
				
				// After the proposed order and all its dependants have been deleted, the blotter order is created from the 
				// proposed order.
				RemoteMethod parentInsert = orderType.Methods.Add("Insert");
				parentInsert.Transaction = remoteTransaction;
				parentInsert.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
				parentInsert.Parameters.Add("blotterId", parentProposedOrder.BlotterId);
				parentInsert.Parameters.Add("accountId", parentProposedOrder.AccountId);
				parentInsert.Parameters.Add("securityId", parentProposedOrder.SecurityId);
				parentInsert.Parameters.Add("settlementId", parentProposedOrder.SettlementId);
				parentInsert.Parameters.Add("brokerId", parentProposedOrder.IsBrokerIdNull() ? (object)DBNull.Value : (object)parentProposedOrder.BrokerId);
				parentInsert.Parameters.Add("positionTypeCode", parentProposedOrder.PositionTypeCode);
				parentInsert.Parameters.Add("transactionTypeCode", parentProposedOrder.TransactionTypeCode);
				parentInsert.Parameters.Add("timeInForceCode", parentProposedOrder.TimeInForceCode);
				parentInsert.Parameters.Add("orderTypeCode", parentProposedOrder.OrderTypeCode);
				parentInsert.Parameters.Add("conditionCode", parentProposedOrder.IsConditionCodeNull() ? (object)DBNull.Value : (object)parentProposedOrder.ConditionCode);
				parentInsert.Parameters.Add("quantity", parentProposedOrder.Quantity);
				parentInsert.Parameters.Add("price1", parentProposedOrder.IsPrice1Null() ? (object)DBNull.Value: (object)parentProposedOrder.Price1);
				parentInsert.Parameters.Add("price2", parentProposedOrder.IsPrice2Null() ? (object)DBNull.Value: (object)parentProposedOrder.Price2);
				parentInsert.Parameters.Add("note", parentProposedOrder.IsNoteNull() ? (object)DBNull.Value: (object)parentProposedOrder.Note);

				// Add all the child orders and their relationships to the DataSet.  The server-side processing needs
				// to insure that we have the right records by checking the rowVersion on all the proposed orders (and
				// their relations).
				foreach (ClientMarketData.ProposedOrderTreeRow proposedOrderTreeRow in parentProposedOrder.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId())
				{

					// Find the child proposed order
					ClientMarketData.ProposedOrderRow childProposedOrder = proposedOrderTreeRow.ProposedOrderRowByFKProposedOrderProposedOrderTreeChildId;

					// Create an entry in the batch for the child proposed order.
					RemoteMethod childInsert = orderType.Methods.Add("Insert");
					childInsert.Transaction = remoteTransaction;
					childInsert.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
					childInsert.Parameters.Add("blotterId", childProposedOrder.BlotterId);
					childInsert.Parameters.Add("accountId", childProposedOrder.AccountId);
					childInsert.Parameters.Add("securityId", childProposedOrder.SecurityId);
					childInsert.Parameters.Add("brokerId", childProposedOrder.IsBrokerIdNull() ? (object)DBNull.Value : (object)childProposedOrder.BrokerId);
					childInsert.Parameters.Add("settlementId", childProposedOrder.SettlementId);
					childInsert.Parameters.Add("positionTypeCode", childProposedOrder.PositionTypeCode);
					childInsert.Parameters.Add("transactionTypeCode", childProposedOrder.TransactionTypeCode);
					childInsert.Parameters.Add("timeInForceCode", childProposedOrder.TimeInForceCode);
					childInsert.Parameters.Add("orderTypeCode", childProposedOrder.OrderTypeCode);
					childInsert.Parameters.Add("conditionCode", childProposedOrder.IsConditionCodeNull() ? (object)DBNull.Value : (object)childProposedOrder.ConditionCode);
					childInsert.Parameters.Add("quantity", childProposedOrder.Quantity);
					childInsert.Parameters.Add("price1", childProposedOrder.IsPrice1Null() ? (object)DBNull.Value: (object)childProposedOrder.Price1);
					childInsert.Parameters.Add("price2", childProposedOrder.IsPrice2Null() ? (object)DBNull.Value: (object)childProposedOrder.Price2);
					childInsert.Parameters.Add("note", childProposedOrder.IsNoteNull() ? (object)DBNull.Value: (object)childProposedOrder.Note);

					// Create an entry in the batch for the relationship between the child and the parent.
					RemoteMethod relationInsert = orderTreeType.Methods.Add("Insert");
					relationInsert.Transaction = remoteTransaction;
					relationInsert.Parameters.Add("parentId", parentInsert.Parameters["orderId"]);
					relationInsert.Parameters.Add("childId", childInsert.Parameters["orderId"]);

				}

			}

			// After the parent account has been scanned for positions that need to be sent, cycle through the children 
			// and recursively call this method until all the children have been scanned for proposed orders that match
			// the given position.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					RecurseAddProposedOrders(remoteBatch, childAccount, securityRow, positionTypeCode);

		}

		/// <summary>
		/// Sends the selected proposed orders to the trade blotters.
		/// </summary>
		/// <param name="selectedPosition">A list of positions to be sent.</param>
		public static void Send(params object[] argument)
		{

			// Extract the command argument.
			ArrayList selectedPositions = (ArrayList)argument[0];

			RemoteBatch remoteBatch = null;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AlgorithmLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlockOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This is the command batch that will be sent to the server when we have created all the orders.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();

				// For each selected position, create a proposed order command in the command batch.
				foreach (Position position in selectedPositions)
				{

					// Find the account record in the data model.
					ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(position.AccountId);
					if (accountRow == null)
						throw new Exception(String.Format("Account {0} has been deleted", position.AccountId));

					// Use the security row to find the equivalent record in the data model.
					ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(position.SecurityId);
					if (securityRow == null)
						throw new Exception(String.Format("Security {0} has been deleted", position.SecurityId));

					// Recursively add all the proposed orders that match this account or any of it's children into the
					// batch.
					RecurseAddProposedOrders(remoteBatch, accountRow, securityRow, position.PositionTypeCode);

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AlgorithmLock.IsReaderLockHeld) ClientMarketData.AlgorithmLock.ReleaseReaderLock();
				if (ClientMarketData.BlockOrderLock.IsReaderLockHeld) ClientMarketData.BlockOrderLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTreeLock.IsReaderLockHeld) ClientMarketData.OrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderTreeLock.IsReaderLockHeld) ClientMarketData.ProposedOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Generates the trades as required by a security or sector model.
		/// </summary>
		public static void Rebalance(params object[] argument)
		{

			// Extract the thread argument
			int accountId = (int)argument[0];
			int modelId = (int)argument[1];

			RemoteBatch remoteBatch = null;

#if DEBUG
			DateTime time1 = DateTime.Now;
#endif

			try
			{

				// Locks the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AlgorithmLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterMapLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Make sure that the account hasn't been deleted since we opened the document.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} doesn't exist in the ClientMarketData", accountId));

				// Make sure that the model hasn't been deleted since we opened the document.
				ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(modelId);
				if (modelRow == null)
					throw new Exception(String.Format("Model {0} doesn't exist in the ClientMarketData", modelId));

				// Load the assembly specified by the algorithm.  This is basically the DLL module where the type and
				// method of the algorithm.
				Assembly assembly = Assembly.Load(modelRow.AlgorithmRow.Assembly);
				if (assembly == null)
					throw new Exception(String.Format("Unable to load assembly {0}", modelRow.AlgorithmRow.Assembly));

				// Get the type information.
				Type type = assembly.GetType(modelRow.AlgorithmRow.Type);
				if (type == null)
					throw new Exception(String.Format("Unable to create type {0} in assembly {1}", modelRow.AlgorithmRow.Type, modelRow.AlgorithmRow.Assembly));

				// This is the method that actually performs the work.
				MethodInfo methodInfo = type.GetMethod(modelRow.AlgorithmRow.Method, new System.Type[] {typeof(ClientMarketData.AccountRow), typeof(ClientMarketData.ModelRow)});
				if (methodInfo == null)
					throw new Exception(String.Format("Unable to find method {0}.{1}", modelRow.AlgorithmRow.Type, modelRow.AlgorithmRow.Method));

				// This is the meat of the rebalancing process.  This dynamic function will call the rebalancing algorithm
				// that is specified in the model.  The results are returned in an OrderForm which can be 
				// processed by the server.
				remoteBatch = (RemoteBatch)methodInfo.Invoke(null, new object[] {accountRow, modelRow});

			}
			catch (Exception exception)
			{
				
				// This will catch all remaining exceptions.
				Debug.WriteLine(exception.Message);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AlgorithmLock.IsReaderLockHeld) ClientMarketData.AlgorithmLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsReaderLockHeld) ClientMarketData.ModelLock.ReleaseReaderLock();
				if (ClientMarketData.SectorTargetLock.IsReaderLockHeld) ClientMarketData.SectorTargetLock.ReleaseReaderLock();
				if (ClientMarketData.PositionTargetLock.IsReaderLockHeld) ClientMarketData.PositionTargetLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderTreeLock.IsReaderLockHeld) ClientMarketData.ProposedOrderTreeLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterMapLock.IsReaderLockHeld) ClientMarketData.BlotterMapLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

#if DEBUG
			// For debugging, get the number of ticks entering the client.
			Console.WriteLine(string.Format("Time to Rebalance: {0}", DateTime.Now.Subtract(time1).TotalMilliseconds));
#endif

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Rebalances only the selected positions.
		/// </summary>
		/// <param name="selectedPositions">A list of selected positions.</param>
		public static void RebalanceSelection(params object[] argument)
		{

			// Extract the command argument.
			int accountId = (int)argument[0];
			int modelId = (int)argument[1];
			ArrayList selectedPositions = (ArrayList)argument[2];

			RemoteBatch remoteBatch = null;
			RemoteParameter modelIdParameter = null;

			try
			{

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ModelLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the original model used by this appraisal.  A copy will be made of the top level values, but only the
				// selected targets will be used to populate the copy.
				ClientMarketData.ModelRow originalModel = ClientMarketData.Model.FindByModelId(modelId);
				if (originalModel == null)
					throw new Exception(String.Format("Model {0} has been deleted.", modelId));
				
				// This batch of commands will create a temporary model and populate it with the selected positions from the
				// original model.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();
				RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.Model");
				RemoteType positionTargetType = coreAssembly.Types.Add("Shadows.WebService.Core.PositionTarget");
				
				RemoteMethod modelInsert = proposedOrderType.Methods.Add("Insert");
				modelInsert.Transaction = remoteTransaction;
				modelInsert.Parameters.Add("modelId", DataType.Int, Direction.ReturnValue);
				modelInsert.Parameters.Add("rowVersion", DataType.Long, Direction.Output);
				modelInsert.Parameters.Add("name", String.Format("Copy of {0}", originalModel.ObjectRow.Name));
				modelInsert.Parameters.Add("objectTypeCode", originalModel.ObjectRow.ObjectTypeCode);
				modelInsert.Parameters.Add("schemeId", originalModel.SchemeId);
				modelInsert.Parameters.Add("algorithmId", originalModel.AlgorithmId);

				// At the end of this method, the rebalancer will be called with the temporary model created here.
				modelIdParameter = modelInsert.Parameters["modelId"];

				// This loop will add a target value in the model on the server for each of the targets extracted from the
				// spreadsheet.
				foreach (Position position in selectedPositions)
				{
				
					// Extract the record key from the spreadsheet.
					int securityId = position.SecurityId;
					int positionTypeCode = position.PositionTypeCode;

					// The original model contains the percentage that will be used in the temporary model.
					ClientMarketData.PositionTargetRow positionTargetRow =
						ClientMarketData.PositionTarget.FindByModelIdSecurityIdPositionTypeCode(modelId, securityId,
						positionTypeCode);

					// If the position exists in the original model, then add the record to the database.
					if (positionTargetRow != null)
					{

						// Create an entry in the temporary model based on the original model's target position.
						RemoteMethod insertPosition = positionTargetType.Methods.Add("Insert");
						insertPosition.Transaction = remoteTransaction;
						insertPosition.Parameters.Add("modelId", modelInsert.Parameters["modelId"]);
						insertPosition.Parameters.Add("securityId", securityId);
						insertPosition.Parameters.Add("positionTypeCode", positionTypeCode);
						insertPosition.Parameters.Add("percent", positionTargetRow.Percent);
						
					}

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// We no longer need to lock the document from being refreshed.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsReaderLockHeld) ClientMarketData.ModelLock.ReleaseReaderLock();
				if (ClientMarketData.PositionTargetLock.IsReaderLockHeld) ClientMarketData.PositionTargetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);

			// The next step is to rebalance the portfolio and delete the temporary model that was created for the rebalancing
			// operation.  If the temporary model can't be created, there's no sense in going on.
			if (remoteBatch.HasExceptions)
				return;
			
			// The temporary model identifier can be extracted from the batch after it's completed.
			int temporaryModelId = (int)modelIdParameter.Value;

			// This will force the client to refresh the data model immediately with the changes and then call the rebalancer.  An
			// optimization for the future would have the local data model being updated as soon as the server acknowledged the new
			// values in the loop above.  This would save a modest amount of time to retrieve values that have been set by this
			// client.  Once the new model is in memory, the standard rebalancing algorithm can be called.
			Rebalance(accountId, temporaryModelId);

			try
			{
			
				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ModelLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the model record.  The current rowVersion is needed to delete the record.
				ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(temporaryModelId);
				if (modelRow == null)
					throw new Exception(String.Format("Model {0} has been deleted.", temporaryModelId));
				
				// This batch will remote the temporary model now that the rebalancing is complete.
				remoteBatch = new RemoteBatch();
				RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();
				RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = coreAssembly.Types.Add("Shadows.WebService.Core.Model");
				
				// This command will remote the temporary model.
				RemoteMethod modelDelete = remoteType.Methods.Add("Delete");
				modelDelete.Transaction = remoteTransaction;
				modelDelete.Parameters.Add("rowVersion", modelRow.RowVersion);
				modelDelete.Parameters.Add("modelId", modelRow.ModelId);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.ModelLock.IsReaderLockHeld) ClientMarketData.ModelLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Send the batch to the server and process the results.
			ClientMarketData.Send(remoteBatch);
		
		}
		
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
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

			// Find the default settlement for this order.
			int settlementId = Security.GetDefaultSettlementId(securityRow);

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
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

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
		/// Recursively creates instructions to delete the relationships between proposed orders in the account group.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		private static void RecurseDeleteRelationship(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow)
		{

			// These define the assembly and the types within those assemblies that will be used to create the proposed orders on
			// the middle tier.
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

			// Run through each of the proposed orders in this account construct a command to delete the relationship between the
			// parent and child.
			foreach (ClientMarketData.ProposedOrderRow parentProposedOrderRow in accountRow.GetProposedOrderRows())
				foreach (ClientMarketData.ProposedOrderTreeRow proposedOrderTree in
					parentProposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId())
				{

					// Create a command to delete the relationship between the parent and child.
					RemoteMethod deleteRelation = proposedOrderTreeType.Methods.Add("Delete");
					deleteRelation.Transaction = remoteTransaction;
					deleteRelation.Parameters.Add("rowVersion", proposedOrderTree.RowVersion);
					deleteRelation.Parameters.Add("parentId", proposedOrderTree.ParentId);
					deleteRelation.Parameters.Add("childId", proposedOrderTree.ChildId);

				}

			// Now, recursively cycle through all the child accounts doing the same thing.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					RecurseDeleteRelationship(remoteBatch, remoteTransaction, childAccount);

		}

		/// <summary>
		/// Recursively creates instructions to delete the relationships between proposed orders in the account group.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		public static void DeleteRelationship(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow)
		{

			// Recursively delete the relationships to this proposed order.
			RecurseDeleteRelationship(remoteBatch, remoteTransaction, accountRow);

		}

		/// <summary>
		/// Recursively creates instructions to delete the relationships between proposed orders in the account group.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		private static void RecurseDelete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow)
		{

			// These define the assembly and the types within those assemblies that will be used to create the proposed orders on
			// the middle tier.
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");

			// Run through each of the proposed orders in this account and create a record to have them deleted.
			foreach (ClientMarketData.ProposedOrderRow parentProposedOrderRow in accountRow.GetProposedOrderRows())
			{

				// Create a command to delete the proposed order.
				RemoteMethod deleteParent = proposedOrderType.Methods.Add("Delete");
				deleteParent.Transaction = remoteTransaction;
				deleteParent.Parameters.Add("rowVersion", parentProposedOrderRow.RowVersion);
				deleteParent.Parameters.Add("proposedOrderId", parentProposedOrderRow.ProposedOrderId);

			}

			// Now, recursively cycle through all the child accounts and delete their proposed orders.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					RecurseDelete(remoteBatch, remoteTransaction, childAccount);

		}

		/// <summary>
		/// Recursively creates instructions to delete the relationships between proposed orders in the account group.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		public static void Delete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction, ClientMarketData.AccountRow accountRow)
		{

			// Recursively delete the account and all the children accounts.
			RecurseDelete(remoteBatch, remoteTransaction, accountRow);

		}

		/// <summary>
		/// Recursively creates instructions to delete proposed order of the given position.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		private static void RecurseDelete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
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

			// Now, recursively cycle through all the child accounts and delete their proposed orders for the given position.
			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in
				accountRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.AccountRow childAccount in
					objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
					RecurseDelete(remoteBatch, remoteTransaction, childAccount, securityRow, positionTypeCode);

		}

		/// <summary>
		/// Recursively creates instructions to delete proposed order of the given position.
		/// </summary>
		/// <param name="remoteBatch">The object type containing the method to delete the order relationship.</param>
		/// <param name="remoteTransaction">Groups several commands into a unit for execution.</param>
		/// <param name="accountRow">An account record, used to select proposed order records.</param>
		public static void Delete(RemoteBatch remoteBatch, RemoteTransaction remoteTransaction,
			ClientMarketData.AccountRow accountRow, ClientMarketData.SecurityRow securityRow, int positionTypeCode)
		{

			// Recursively delete the position and all related positions.
			RecurseDelete(remoteBatch, remoteTransaction, accountRow, securityRow, positionTypeCode);

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
			RemoteAssembly coreAssembly = remoteBatch.Assemblies.Add("Service.Core");
			RemoteType proposedOrderType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrder");
			RemoteType proposedOrderTreeType = coreAssembly.Types.Add("Shadows.WebService.Core.ProposedOrderTree");

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
