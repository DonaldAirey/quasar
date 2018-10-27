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
	/// Summary description for DestinationOrder.
	/// </summary>
	public class DestinationOrder
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
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.BlotterLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.OrderTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TimeInForceLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.WorkingOrderLock);

			// The Core Library will take care of the main part of the record.
			Core.DestinationOrder.Insert(adoTransaction);

		}

		/// <summary>Inserts a DestinationOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void InsertFile(ParameterList parameters)
		{

			// Accessor for the DestinationOrder Table.
			ServerMarketData.DestinationOrderDataTable destinationOrderTable = ServerMarketData.DestinationOrder;
			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			object externalBlotterId = parameters["blotterId"].Value;
			string externalDestinationOrderId = parameters["destinationOrderId"];
			string externalDestinationId = parameters["destinationId"];
			object externalId0 = parameters["externalId0"].Value;
			object isAdvertised = parameters["isAdvertised"].Value;
			object isAutoExecute = parameters["isAutoExecute"].Value;
			object isCanceled = parameters["isCanceled"].Value;
			object isSteppedIn = parameters["isSteppedIn"].Value;
			System.Decimal orderedQuantity = parameters["orderedQuantity"];
			string externalOrderTypeCode = parameters["orderTypeCode"];
			string externalPriceTypeCode = parameters["priceTypeCode"];
			object receivedTime = parameters["receivedTime"].Value;
			string externalSecurityId = parameters["securityId"];
			object externalSettlementId = parameters["settlementId"].Value;
			object targetPrice = parameters["targetPrice"].Value;
			string externalTimeInForceCode = parameters["timeInForceCode"];
			object uploadedTime = parameters["uploadedTime"].Value;
			string externalWorkingOrderId = parameters["workingOrderId"];
			// The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			// event it's needed for operations within the batch.
			long rowVersion = long.MinValue;
			// Resolve External Identifiers
			object blotterId = External.Blotter.FindOptionalKey(configurationId, "blotterId", externalBlotterId);
			int destinationOrderId = External.DestinationOrder.FindKey(configurationId, "destinationOrderId", externalDestinationOrderId);
			int orderTypeCode = External.OrderType.FindRequiredKey(configurationId, "orderTypeCode", externalOrderTypeCode);
			int destinationId = External.Destination.FindRequiredKey(configurationId, "destinationId", externalDestinationId);
			int priceTypeCode = External.PriceType.FindRequiredKey(configurationId, "priceTypeCode", externalPriceTypeCode);
			int securityId = External.Security.FindRequiredKey(configurationId, "securityId", externalSecurityId);
			object settlementId = External.Security.FindOptionalKey(configurationId, "settlementId", externalSettlementId);
			int timeInForceCode = External.TimeInForce.FindRequiredKey(configurationId, "timeInForceCode", externalTimeInForceCode);
			int workingOrderId = External.WorkingOrder.FindRequiredKey(configurationId, "workingOrderId", externalWorkingOrderId);

			// All parameters from the xml file arrive as strings.  Any required or optional parameter needs to be converted to a
			// native type here.
			decimal quantityOrdered = Convert.ToDecimal(parameters["quantity"].Value);

			// Optional Parameters
			object isHidden = parameters["isHidden"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isHidden"].Value);
			object limitPrice = parameters["limitPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["limitPrice"].Value);
			object stopPrice = parameters["stopPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["stopPrice"].Value);

			// Time stamps and user stamps
			int createdUserId = ServerMarketData.UserId;
			DateTime createdTime = DateTime.Now;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = DateTime.Now;
			int statusCode = Status.New;
			int stateCode = State.Initial;

			// The load operation will create a record if it doesn't exist, or update an existing record.  The external
			// identifier is used to determine if a record exists with the same key.
			if (destinationOrderId == int.MinValue)
			{

				// Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
				// external method is called with the same 'configurationId' parameter.
				int externalKeyIndex = External.DestinationOrder.GetExternalKeyIndex(configurationId, "destinationOrderId");
				object[] externalIdArray = new object[1];
				externalIdArray[externalKeyIndex] = externalDestinationOrderId;
				externalId0 = externalIdArray[0];

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, null, createdTime,
					createdUserId, destinationId, externalId0, null, isHidden, limitPrice, modifiedTime, modifiedUserId,
					orderTypeCode, orderedQuantity, priceTypeCode, stateCode, statusCode, stopPrice, createdUserId, timeInForceCode,
					workingOrderId);

			}
			else
			{

				// While the optimistic concurrency checking is disabled for the external methods, the internal methods
				// still need to perform the check.  This ncurrency checking logic by finding the current row version to be
				// will bypass the coused when the internal method is called.
				ServerMarketData.DestinationOrderRow destinationOrderRow = destinationOrderTable.FindByDestinationOrderId((int)destinationOrderId);
				rowVersion = ((long)(destinationOrderRow[destinationOrderTable.RowVersionColumn]));

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.DestinationOrder.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null, createdTime,
					createdUserId, destinationId, (int)destinationOrderId, externalId0, null, isHidden, limitPrice, modifiedTime,
					modifiedUserId, orderedQuantity, orderTypeCode, priceTypeCode, stateCode, statusCode, stopPrice, modifiedUserId,
					timeInForceCode, workingOrderId);

			}

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

	}

}
