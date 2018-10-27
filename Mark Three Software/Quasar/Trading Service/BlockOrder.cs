/*************************************************************************************************************************
*
*	File:			BlockOrder.cs
*	Description:	Methods to block orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.WebService.Trading
{
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Server;
	using Shadows.WebService;
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Reflection;

	/// <summary>
	/// Handles server-side blocking of orders.
	/// </summary>
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Diagnostics.DebuggerStepThrough()]
	public class BlockOrder
	{

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Insert(Transaction transaction)
		{
			// These table lock(s) are required for the 'Insert' operation.
			transaction.Locks.AddWriterLock(ServerMarketData.BlockOrderLock);
		}
        
		/// <summary>Inserts a BlockOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Insert(Transaction transaction, RemoteMethod remoteMethod)
		{
			try
			{
				// Extract the parameters from the command batch.
				int blotterId = remoteMethod.Parameters.GetRequiredInt32("blotterId");
				object accountId = remoteMethod.Parameters.GetOptionalInt32("accountId");
				int securityId = remoteMethod.Parameters.GetRequiredInt32("securityId");
				int settlementId = remoteMethod.Parameters.GetRequiredInt32("settlementId");
				object brokerId = remoteMethod.Parameters.GetOptionalInt32("brokerId");
				int transactionTypeCode = remoteMethod.Parameters.GetRequiredInt32("transactionTypeCode");
				object timeInForceCode = remoteMethod.Parameters.GetOptionalInt32("timeInForceCode");
				object orderTypeCode = remoteMethod.Parameters.GetOptionalInt32("orderTypeCode");
				object conditionCode = remoteMethod.Parameters.GetOptionalInt32("conditionCode");
				object isAgency = remoteMethod.Parameters.GetOptionalBoolean("isAgency");
				object price1 = remoteMethod.Parameters.GetOptionalDecimal("price1");
				object price2 = remoteMethod.Parameters.GetOptionalDecimal("price2");
				// Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
				// from being called with bad data, but provides for error checking on all the parameters.
				if ((remoteMethod.HasExceptions == false))
				{
					// The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
					long rowVersion = long.MinValue;
					// Call the internal method to complete the operation.
					int blockOrderId = BlockOrder.Insert(transaction, blotterId, accountId, securityId, settlementId, brokerId, transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, ref rowVersion, isAgency, price1, price2);
					// Return values.
					remoteMethod.Parameters.ReturnValue("rowVersion", rowVersion);
					remoteMethod.Parameters.ReturnValue(blockOrderId);
				}
			}
			catch (SqlException sqlException)
			{
				// Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
				for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
				{
				}
			}
			catch (Exception exception)
			{
				// This will pass the general exception back to the caller.
				remoteMethod.Exceptions.Add(exception);
			}
		}
        
		/// <summary>Inserts a Order record.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		/// <param name="blockOrderId">The value for the BlockOrderId column.</param>
		/// <param name="accountId">The value for the AccountId column.</param>
		/// <param name="securityId">The value for the SecurityId column.</param>
		/// <param name="settlementId">The value for the SettlementId column.</param>
		/// <param name="brokerId">The value for the BrokerId column.</param>
		/// <param name="transactionTypeCode">The value for the TransactionTypeCode column.</param>
		/// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
		/// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
		/// <param name="conditionCode">The value for the ConditionCode column.</param>
		/// <param name="rowVersion">The value for the RowVersion column.</param>
		/// <param name="isAgency">The value for the Agency column.</param>
		/// <param name="quantity">The value for the Quantity column.</param>
		/// <param name="price1">The value for the Price1 column.</param>
		/// <param name="price2">The value for the Price2 column.</param>
		/// <param name="note">The value for the Note column.</param>
		public static int Insert(
			Transaction transaction,
			int blotterId,
			object accountId,
			int securityId,
			int settlementId,
			object brokerId, 
			int transactionTypeCode, 
			object timeInForceCode, 
			object orderTypeCode, 
			object conditionCode, 
			ref long rowVersion, 
			object isAgency, 
			object price1, 
			object price2)
		{

			// These values are provided by the server to initialize a block order.
			DateTime createdTime = DateTime.Now;
			int createdLoginId = ServerMarketData.LoginId;
			int statusCode = Shadows.Quasar.Common.Status.New;
			decimal quantityExecuted = 0.0M;
			decimal quantityPlaced = 0.0M;
			
			// If there is no blocking algorithm associated with this block order, then create a new block for every individual
			// order that hits the desk.
			return Shadows.WebService.Core.BlockOrder.Insert(transaction, blotterId, accountId, securityId, settlementId, brokerId,
				statusCode, transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, ref rowVersion, quantityExecuted,
				quantityPlaced, null, isAgency, price1, price2, createdTime, createdLoginId, createdTime, createdLoginId);

		}

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Open(Transaction transaction)
		{
			// These table lock(s) are required for the 'Insert' operation.
			transaction.Locks.AddReaderLock(ServerMarketData.AlgorithmLock);
			transaction.Locks.AddReaderLock(ServerMarketData.BlotterLock);
			transaction.Locks.AddWriterLock(ServerMarketData.BlockOrderLock);
		}
        
		/// <summary>Opens a BlockOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Open(Transaction transaction, RemoteMethod remoteMethod)
		{
			try
			{
				// Extract the parameters from the command batch.
				int blotterId = remoteMethod.Parameters.GetRequiredInt32("blotterId");
				object accountId = remoteMethod.Parameters.GetOptionalInt32("accountId");
				int securityId = remoteMethod.Parameters.GetRequiredInt32("securityId");
				int settlementId = remoteMethod.Parameters.GetRequiredInt32("settlementId");
				object brokerId = remoteMethod.Parameters.GetOptionalInt32("brokerId");
				int transactionTypeCode = remoteMethod.Parameters.GetRequiredInt32("transactionTypeCode");
				object timeInForceCode = remoteMethod.Parameters.GetOptionalInt32("timeInForceCode");
				object orderTypeCode = remoteMethod.Parameters.GetOptionalInt32("orderTypeCode");
				object conditionCode = remoteMethod.Parameters.GetOptionalInt32("conditionCode");
				object isAgency = remoteMethod.Parameters.GetOptionalBoolean("isAgency");
				object price1 = remoteMethod.Parameters.GetOptionalDecimal("price1");
				object price2 = remoteMethod.Parameters.GetOptionalDecimal("price2");
				// Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
				// from being called with bad data, but provides for error checking on all the parameters.
				if ((remoteMethod.HasExceptions == false))
				{
					// The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
					long rowVersion = long.MinValue;
					// Call the internal method to complete the operation.
					int blockOrderId = BlockOrder.Open(transaction, blotterId, accountId, securityId, settlementId, brokerId, transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, ref rowVersion, isAgency, price1, price2);
					// Return values.
					remoteMethod.Parameters.ReturnValue("rowVersion", rowVersion);
					remoteMethod.Parameters.ReturnValue(blockOrderId);
				}
			}
			catch (SqlException sqlException)
			{
				// Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
				for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
				{
				}
			}
			catch (Exception exception)
			{
				// This will pass the general exception back to the caller.
				remoteMethod.Exceptions.Add(exception);
			}
		}
        
		/// <summary>Inserts a Order record.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		/// <param name="blockOrderId">The value for the BlockOrderId column.</param>
		/// <param name="accountId">The value for the AccountId column.</param>
		/// <param name="securityId">The value for the SecurityId column.</param>
		/// <param name="settlementId">The value for the SettlementId column.</param>
		/// <param name="brokerId">The value for the BrokerId column.</param>
		/// <param name="transactionTypeCode">The value for the TransactionTypeCode column.</param>
		/// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
		/// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
		/// <param name="conditionCode">The value for the ConditionCode column.</param>
		/// <param name="rowVersion">The value for the RowVersion column.</param>
		/// <param name="isAgency">The value for the Agency column.</param>
		/// <param name="quantity">The value for the Quantity column.</param>
		/// <param name="price1">The value for the Price1 column.</param>
		/// <param name="price2">The value for the Price2 column.</param>
		/// <param name="note">The value for the Note column.</param>
		public static int Open(
			Transaction transaction,
			int blotterId,
			object accountId, 
			int securityId, 
			int settlementId, 
			object brokerId, 
			int transactionTypeCode, 
			object timeInForceCode, 
			object orderTypeCode, 
			object conditionCode, 
			ref long rowVersion, 
			object isAgency, 
			object price1, 
			object price2)
		{

			// Find the destination blotter for the block order.
			ServerMarketData.BlotterRow blotterRow = ServerMarketData.Blotter.FindByBlotterId(blotterId);
			if (blotterRow == null)
				throw new Exception(String.Format("Blotter {0} has been deleted.", blotterId));
			
			// See if the blotter has an algorithm associated with it for blocking orders.  If it does, the library will 
			// be dynamically loaded and called to see if there's a block order matching the criteria.
			if (!blotterRow.IsAlgorithmIdNull())
			{

				// Find the algorithm row in the data model.
				ServerMarketData.AlgorithmRow algorithmsRow = blotterRow.AlgorithmRow;

				// Load the assembly specified by the algorithm.  This is basically the DLL module where the type and
				// method of the algorithm.
				Assembly assembly = Assembly.Load(blotterRow.AlgorithmRow.Assembly);
				if (assembly == null)
					throw new Exception(String.Format("Unable to load assembly {0}", blotterRow.AlgorithmRow.Assembly));

				// Get the type information.
				Type type = assembly.GetType(blotterRow.AlgorithmRow.Type);
				if (type == null)
					throw new Exception(String.Format("Unable to create type {0} in assembly {1}", blotterRow.AlgorithmRow.Type, blotterRow.AlgorithmRow.Assembly));

				// This is the method that actually performs the work.
				MethodInfo methodInfo = type.GetMethod(blotterRow.AlgorithmRow.Method);
				if (methodInfo == null)
					throw new Exception(String.Format("Unable to find method {0}.{1}", blotterRow.AlgorithmRow.Type, blotterRow.AlgorithmRow.Method));

				// This is the meat of the rebalancing process.  This dynamic function will call the rebalancing algorithm
				// that is specified in the model.  The results are returned in an OrderForm which can be 
				// processed by the server.
				return (int)methodInfo.Invoke(null, new object[] {transaction, blotterId, accountId, securityId, settlementId,
																	 brokerId, transactionTypeCode,
																	 timeInForceCode, orderTypeCode, conditionCode, price1,
																	 price2});

			}

			DateTime createdTime = DateTime.Now;
			int createdLoginId = ServerMarketData.LoginId;
			int statusCode = Shadows.Quasar.Common.Status.New;
			decimal quantityExecuted = 0.0M;
			decimal quantityPlaced = 0.0M;
			
			// If there is no blocking algorithm associated with this block order, then create a new block for every individual
			// order that hits the desk.
			return Shadows.WebService.Core.BlockOrder.Insert(transaction, blotterId, accountId, securityId, settlementId, brokerId, statusCode,
				transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, ref rowVersion, quantityExecuted, quantityPlaced, null, isAgency, price1, 
				price2, createdTime, createdLoginId, createdTime, createdLoginId);

		}

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Execute(Transaction transaction)
		{

			// This method will call the 'Insert' placement method.
			Shadows.WebService.Core.Placement.Insert(transaction);

			// These table lock(s) are required for the 'Execute' operation.
			transaction.Locks.AddWriterLock(ServerMarketData.BlockOrderLock);

		}
        
		/// <summary>Executes a BlockOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Execute(Transaction transaction, RemoteMethod remoteMethod)
		{
			try
			{

				// Extract the parameters from the command batch.
				int blockOrderId = remoteMethod.Parameters.GetRequiredInt32("blockOrderId");
				int brokerId = remoteMethod.Parameters.GetRequiredInt32("brokerId");
				int timeInForceCode = remoteMethod.Parameters.GetRequiredInt32("timeInForceCode");
				int orderTypeCode = remoteMethod.Parameters.GetRequiredInt32("orderTypeCode");
				object conditionCode = remoteMethod.Parameters.GetOptionalInt32("conditionCode");
				decimal quantity = remoteMethod.Parameters.GetRequiredDecimal("quantity");
				object price1 = remoteMethod.Parameters.GetOptionalDecimal("price1");
				object price2 = remoteMethod.Parameters.GetOptionalDecimal("price2");
				// Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
				// from being called with bad data, but provides for error checking on all the parameters.
				if ((remoteMethod.HasExceptions == false))
				{
					// The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
					long rowVersion = long.MinValue;
					// Call the internal method to complete the operation.
					BlockOrder.Execute(transaction, blockOrderId, brokerId, timeInForceCode, orderTypeCode, conditionCode, ref rowVersion, quantity, price1, price2);
					// Return values.
					remoteMethod.Parameters.ReturnValue("rowVersion", rowVersion);
					remoteMethod.Parameters.ReturnValue(blockOrderId);
				}
			}
			catch (SqlException sqlException)
			{
				// Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
				for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
				{
				}
			}
			catch (Exception exception)
			{
				// This will pass the general exception back to the caller.
				remoteMethod.Exceptions.Add(exception);
			}
		}
        
		/// <summary>Inserts a Order record.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		/// <param name="blockOrderId">The value for the BlockOrderId column.</param>
		/// <param name="accountId">The value for the AccountId column.</param>
		/// <param name="securityId">The value for the SecurityId column.</param>
		/// <param name="settlementId">The value for the SettlementId column.</param>
		/// <param name="brokerId">The value for the BrokerId column.</param>
		/// <param name="transactionTypeCode">The value for the TransactionTypeCode column.</param>
		/// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
		/// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
		/// <param name="conditionCode">The value for the ConditionCode column.</param>
		/// <param name="rowVersion">The value for the RowVersion column.</param>
		/// <param name="isAgency">The value for the Agency column.</param>
		/// <param name="quantity">The value for the Quantity column.</param>
		/// <param name="price1">The value for the Price1 column.</param>
		/// <param name="price2">The value for the Price2 column.</param>
		/// <param name="note">The value for the Note column.</param>
		public static int Execute(
			Transaction transaction,
			int blockOrderId,
			int brokerId,
			int timeInForceCode,
			int orderTypeCode,
			object conditionCode, 
			ref long rowVersion,
			decimal quantity,
			object price1, 
			object price2)
		{

			// Find the destination blockOrder for the block order.
			ServerMarketData.BlockOrderRow blockOrderRow = ServerMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
			if (blockOrderRow == null)
				throw new Exception(String.Format("BlockOrder {0} has been deleted.", blockOrderId));
			
			bool isRouted = true;
			bool isDeleted = false;
			DateTime createdTime = DateTime.Now;
			int createdLoginId = ServerMarketData.LoginId;
			
			// If there is no blocking algorithm associated with this block order, then create a new block for every individual
			// order that hits the desk.
			return Shadows.WebService.Core.Placement.Insert(transaction, blockOrderRow.BlockOrderId, brokerId, timeInForceCode,
				orderTypeCode, ref rowVersion, isDeleted, isRouted, quantity, price1, price2, createdTime, createdLoginId, createdTime,
				createdLoginId);

		}

	}

}
