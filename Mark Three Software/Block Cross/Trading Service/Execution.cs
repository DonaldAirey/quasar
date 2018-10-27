namespace MarkThree.Guardian.Trading
{

	using MarkThree.Guardian;
	using MarkThree.Guardian.Server;
	using MarkThree;
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Execution.
	/// </summary>
	public class Execution
	{

		/// This value is used to map the object to a persistent storage device.  The parameters for the storage
		/// are found in the configuration file for this service.
		public static string PersistentStore = "Guardian";
        
		/// This member provides access to the in-memory database.
		private static ServerMarketData serverGuardianData = new ServerMarketData();

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void InsertFile(AdoTransaction adoTransaction)
		{

			// These table lock(s) are required for resloving the external ids.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationOrderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.WorkingOrderLock);

			// The Core Library will take care of the main part of the record.
			Core.Execution.Insert(adoTransaction);

		}
        
		/// <summary>Inserts a Execution record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void InsertFile(ParameterList parameters)
		{

			// Accessor for the Execution Table.
			ServerMarketData.ExecutionDataTable executionTable = ServerMarketData.Execution;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			object sourceExecutionId = parameters["sourceExecutionId"].Value;
			string externalDestinationOrderId = parameters["destinationOrderId"];
			object externalExecutionId = parameters["executionId"].Value;
			object externalId0 = parameters["externalId0"].Value;
			System.Decimal executionPrice = parameters["executionPrice"];
			System.Decimal executionQuantity = parameters["executionQuantity"];
			System.DateTime settlementDate = parameters["settlementDate"];
			System.DateTime tradeDate = parameters["tradeDate"];

			// Resolve External Identifiers
			int destinationOrderId = External.DestinationOrder.FindRequiredKey(configurationId, "destinationOrderId", externalDestinationOrderId);
			int executionId = External.Execution.FindKey(configurationId, "executionId", (string)externalExecutionId);

			// The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			// event it's needed for operations within the batch.
			long rowVersion = long.MinValue;

			// Optional Parameters
			object accruedInterest = parameters["accruedInterest"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["accruedInterest"].Value);
			object commission = parameters["commission"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["commission"].Value);
			object isHidden = parameters["isHidden"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isHidden"].Value);
			object userFee0 = parameters["userFee0"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["userFee0"].Value);
			object userFee1 = parameters["userFee1"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["userFee1"].Value);
			object userFee2 = parameters["userFee2"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["userFee2"].Value);
			object userFee3 = parameters["userFee3"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["userFee3"].Value);

			// Default Status codes
			int destinationStateCode = State.Acknowledged;
			int sourceStateCode = State.Initial;

			// Time stamps and user stamps
			int createdUserId = ServerMarketData.UserId;
			DateTime createdTime = DateTime.Now;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = DateTime.Now;

			// The load operation will create a record if it doesn't exist, or update an existing record.  The external
			// identifier is used to determine if a record exists with the same key.
			if ((executionId == int.MinValue))
			{

				// Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
				// external method is called with the same 'configurationId' parameter.
				int externalKeyIndex = External.Execution.GetExternalKeyIndex(configurationId, "executionId");
				object[] externalIdArray = new object[1];
				externalIdArray[externalKeyIndex] = externalExecutionId;
				externalId0 = externalIdArray[0];

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction, ref rowVersion, accruedInterest,
					null, null, commission, createdTime, createdUserId, destinationOrderId, destinationStateCode, executionPrice,
					executionQuantity, externalId0, null, isHidden, modifiedTime, modifiedUserId, null, null, null,
					settlementDate, sourceExecutionId, sourceStateCode, tradeDate, userFee0, userFee1, userFee2, userFee3);

			}
			else
			{

				// While the optimistic concurrency checking is disabled for the external methods, the internal methods
				// still need to perform the check.  This ncurrency checking logic by finding the current row version to be
				// will bypass the coused when the internal method is called.
				ServerMarketData.ExecutionRow executionRow = executionTable.FindByExecutionId(executionId);
				rowVersion = ((long)(executionRow[executionTable.RowVersionColumn]));

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.Execution.Update(adoTransaction, sqlTransaction, ref rowVersion, accruedInterest,
					null, null, commission, createdTime, createdUserId, destinationOrderId, destinationStateCode, executionId,
					executionPrice, executionQuantity, externalId0, null, isHidden, modifiedTime, modifiedUserId, null, null,
					null, settlementDate, sourceExecutionId, sourceStateCode, tradeDate, userFee0, userFee1, userFee2, userFee3);

			}

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

		/// <summary>Collects the table lock request(s) for an 'Delete' operation</summary>
		/// <param name="adoTransaction">A collection of table locks required for this operation</param>
		public static void Delete(AdoTransaction adoTransaction)
		{
			// Call the internal methods to lock the tables required for an insert or update operation.
			MarkThree.Guardian.Core.Execution.Delete(adoTransaction);
			// These table lock(s) are required for the 'Delete' operation.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ExecutionLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationOrderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
		}
        
		/// <summary>Deletes a Execution record using Metadata Parameters.</summary>
		/// <param name="transaction">Contains the parameters and exceptions for this command.</param>
		public static void Delete(ParameterList parameters)
		{
			// Accessor for the Execution Table.
			ServerMarketData.ExecutionDataTable executionTable = ServerMarketData.Execution;
			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			string externalExecutionId = parameters["executionId"];
			// The row versioning is largely disabled for external operations.
			long rowVersion = long.MinValue;
			// Find the internal identifier using the primar key elements.
			// identifier is used to determine if a record exists with the same key.
			int executionId = External.Execution.FindRequiredKey(configurationId, "executionId", externalExecutionId);
			// While the optimistic concurrency checking is disabled for the external methods, the internal methods
			// still need to perform the check.  This ncurrency checking logic by finding the current row version to be
			// will bypass the coused when the internal method is called.
			ServerMarketData.ExecutionRow executionRow = executionTable.FindByExecutionId(executionId);
			rowVersion = ((long)(executionRow[executionTable.RowVersionColumn]));
			// Call the internal method to complete the operation.
			MarkThree.Guardian.Core.Execution.Delete(adoTransaction, sqlTransaction, rowVersion, executionId);
		}
        
	}

}
