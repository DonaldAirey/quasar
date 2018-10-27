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
	/// Summary description for SourceOrder.
	/// </summary>
	public class WorkingOrder
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
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.BlotterLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.OrderTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TimeInForceLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TraderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);

			// The Core Library will take care of the main part of the record.
			Core.WorkingOrder.Insert(adoTransaction);

		}
        
		/// <summary>Inserts a SourceOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void InsertFile(ParameterList parameters)
		{

			// Accessor for the WorkingOrder Table.
			ServerMarketData.WorkingOrderDataTable workingOrderTable = ServerMarketData.WorkingOrder;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			string externalBlotterId = parameters["blotterId"];
			object externalDestinationId = parameters["destinationId"].Value;
			string externalWorkingOrderId = parameters["workingOrderId"];
			string externalOrderTypeCode = parameters["orderTypeCode"];
			string externalPriceTypeCode = parameters["priceTypeCode"];
			string externalSecurityId = parameters["securityId"];
			object externalSettlementId = parameters["settlementId"].Value;
			object externalSubmissionTypeCode = parameters["submissionTypeCode"].Value;
			object submittedQuantity = parameters["submittedQuantity"].Value;
			object submittedTime = parameters["submittedTime"].Value;
			string externalTimeInForceCode = parameters["timeInForceCode"];
			object uploadedTime = parameters["uploadedTime"].Value;
			object externalId0 = parameters["externalId0"].Value;

			// The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			// event it's needed for operations within the batch.
			long rowVersion = long.MinValue;

			// Resolve External Identifiers
			int blotterId = External.Blotter.FindRequiredKey(configurationId, "blotterId", externalBlotterId);
			int workingOrderId = External.WorkingOrder.FindKey(configurationId, "workingOrderId", externalWorkingOrderId);
			object destinationId = External.Destination.FindOptionalKey(configurationId, "destinationId", externalDestinationId);
			int orderTypeCode = External.OrderType.FindRequiredKey(configurationId, "orderTypeCode", externalOrderTypeCode);
			int priceTypeCode = External.PriceType.FindRequiredKey(configurationId, "priceTypeCode", externalPriceTypeCode);
			int securityId = External.Security.FindRequiredKey(configurationId, "securityId", externalSecurityId);
			object submissionTypeCode = External.SubmissionType.FindOptionalKey(configurationId, "submissionTypeCode", externalSubmissionTypeCode);
			object settlementId = External.Security.FindOptionalKey(configurationId, "settlementId", externalSettlementId);
			int timeInForceCode = External.TimeInForce.FindRequiredKey(configurationId, "timeInForceCode", externalTimeInForceCode);

			// Convert the parameters into binary values.
			object isAgencyMatch = parameters["isAgencyMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isAgencyMatch"].Value);
			object isBrokerMatch = parameters["isBrokerMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isBrokerMatch"].Value);
			object isHedgeMatch = parameters["isHedgeMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isHedgeMatch"].Value);
			object isHeld = parameters["isHeld"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isHeld"].Value);
			object isInstitutionMatch = parameters["isInstitutionMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isInstitutionMatch"].Value);
			object limitPrice = parameters["limitPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["limitPrice"].Value);
			object maximumVolatility = parameters["maximumVolatility"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["maximumVolatility"].Value);
			object startTime = parameters["startTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["startTime"].Value);
			object stopPrice = parameters["stopPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["stopPrice"].Value);
			object stopTime = parameters["stopTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["stopTime"].Value);
			object newsFreeTime = parameters["newsFreeTime"] is MissingParameter ? (object)null :
				Convert.ToInt32(parameters["newsFreeTime"].Value);

			// Defaults
			int createdUserId = ServerMarketData.UserId;
			DateTime createdTime = DateTime.Now;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = createdTime;
			DateTime timeReceived = createdTime;
			int statusCode = Status.New;

			// Provide a default for the submission type code.
			if (submissionTypeCode == null)
				submissionTypeCode = SubmissionType.NeverMatch;

			// The Trader table contains additional defaults for incoming orders.
			int traderId = ServerMarketData.UserId;
			ServerMarketData.TraderRow traderRow = ServerMarketData.Trader.FindByTraderId(traderId);
			if (isAgencyMatch == null)
				isAgencyMatch = traderRow.IsAgencyMatch;
			if (isBrokerMatch == null)
				isBrokerMatch = traderRow.IsBrokerMatch;
			if (isHedgeMatch == null)
				isHedgeMatch = traderRow.IsHedgeMatch;
			if (isInstitutionMatch == null)
				isInstitutionMatch = traderRow.IsInstitutionMatch;
			if (maximumVolatility == null && !traderRow.IsMaximumVolatilityDefaultNull())
				maximumVolatility = traderRow.MaximumVolatilityDefault;
			if (startTime == null && !traderRow.IsStartTimeDefaultNull())
				startTime = traderRow.StartTimeDefault;
			if (stopTime == null && !traderRow.IsStopTimeDefaultNull())
				stopTime = traderRow.StopTimeDefault;
			if (newsFreeTime == null && !traderRow.IsNewsFreeTimeDefaultNull())
				newsFreeTime = traderRow.NewsFreeTimeDefault;

			// The load operation will create a record if it doesn't exist, or update an existing record.  The external
			// identifier is used to determine if a record exists with the same key.
			if ((workingOrderId == int.MinValue))
			{

				// Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
				// external method is called with the same 'configurationId' parameter.
				int externalKeyIndex = External.WorkingOrder.GetExternalKeyIndex(configurationId, "workingOrderId");
				object[] externalIdArray = new object[1];
				externalIdArray[externalKeyIndex] = externalWorkingOrderId;
				externalId0 = externalIdArray[0];

				// Create a timer for this working order for sleeping while in the matching box.
				long timerRowVersion = long.MinValue;
				int timerId = Core.Timer.Insert(adoTransaction, sqlTransaction, ref timerRowVersion, DateTime.MinValue, null, false, DateTime.MinValue, 0);

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.WorkingOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, blotterId, createdTime,
					createdUserId, destinationId, externalId0, isAgencyMatch, null, null, isBrokerMatch, isHedgeMatch, isInstitutionMatch, limitPrice,
					maximumVolatility, modifiedTime, modifiedUserId, newsFreeTime, orderTypeCode, priceTypeCode, securityId,
					settlementId, startTime, statusCode, stopPrice, stopTime, (int)submissionTypeCode, submittedQuantity,
					submittedTime, timeInForceCode, timerId, uploadedTime);

			}
			else

				throw new Exception(string.Format("There is already a working order with the identifier {0}", externalWorkingOrderId));

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

		/// <summary>Collects the table lock request(s) for an 'Update' operation</summary>
		/// <param name="adoTransaction">A collection of table locks required for this operation</param>
		public static void UpdateFile(AdoTransaction adoTransaction)
		{

			// Call the internal methods to lock the tables required for an insert or update operation.
			MarkThree.Guardian.Core.WorkingOrder.Update(adoTransaction);
			// These table lock(s) are required for the 'Update' operation.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.BlotterLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.StatusLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.OrderTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TraderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TimeInForceLock);

		}
        
		/// <summary>Updates a WorkingOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Contains the parameters and exceptions for this command.</param>
		public static void UpdateFile(ParameterList parameters)
		{

			// Accessor for the WorkingOrder Table.
			ServerMarketData.WorkingOrderDataTable workingOrderTable = ServerMarketData.WorkingOrder;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			object externalBlotterId = parameters["blotterId"].Value;
			string externalWorkingOrderId = ((string)(parameters["workingOrderId"]));
			object externalDestinationId = parameters["destinationId"].Value;
			object externalOrderTypeCode = parameters["orderTypeCode"].Value;
			object externalPriceTypeCode = parameters["priceTypeCode"].Value;
			object externalSecurityId = parameters["securityId"].Value;
			object externalSubmissionTypeCode = parameters["submissionTypeCode"].Value;
			object externalSettlementId = parameters["settlementId"].Value;
			object submittedQuantity = parameters["submittedQuantity"].Value;
			object submittedTime = parameters["submittedTime"].Value;
			object externalTimeInForceCode = parameters["timeInForceCode"].Value;
			object uploadedTime = parameters["uploadedTime"].Value;

			// The row versioning is largely disabled for external operations.
			long rowVersion = long.MinValue;

			// Resolve External Identifiers
			object blotterId = External.Blotter.FindOptionalKey(configurationId, "blotterId", externalBlotterId);
			object destinationId = External.Destination.FindOptionalKey(configurationId, "destinationId", externalDestinationId);
			object orderTypeCode = External.OrderType.FindOptionalKey(configurationId, "orderTypeCode", externalOrderTypeCode);
			object priceTypeCode = External.PriceType.FindOptionalKey(configurationId, "priceTypeCode", externalPriceTypeCode);
			object securityId = External.Security.FindOptionalKey(configurationId, "securityId", externalSecurityId);
			object settlementId = External.Security.FindOptionalKey(configurationId, "settlementId", externalSettlementId);
			object timeInForceCode = External.TimeInForce.FindOptionalKey(configurationId, "timeInForceCode", externalTimeInForceCode);
			object submissionTypeCode = External.SubmissionType.FindOptionalKey(configurationId, "submissionTypeCode", externalSubmissionTypeCode);
			int workingOrderId = External.WorkingOrder.FindRequiredKey(configurationId, "workingOrderId", externalWorkingOrderId);

			// Convert the parameters into binary values.
			object isAgencyMatch = parameters["isAgencyMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isAgencyMatch"].Value);
			object isBrokerMatch = parameters["isBrokerMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isBrokerMatch"].Value);
			object isHedgeMatch = parameters["isHedgeMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isHedgeMatch"].Value);
			object isInstitutionMatch = parameters["isInstitutionMatch"] is MissingParameter ? (object)null :
				Convert.ToBoolean(parameters["isInstitutionMatch"].Value);
			object limitPrice = parameters["limitPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["limitPrice"].Value);
			object maximumVolatility = parameters["maximumVolatility"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["maximumVolatility"].Value);
			object startTime = parameters["startTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["startTime"].Value);
			object stopPrice = parameters["stopPrice"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["stopPrice"].Value);
			object stopTime = parameters["stopTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["stopTime"].Value);
			object newsFreeTime = parameters["newsFreeTime"] is MissingParameter ? (object)null :
				Convert.ToInt32(parameters["newsFreeTime"].Value);

			// Defaults
			int statusCode = Status.New;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = DateTime.Now;
			DateTime timeReceived = modifiedTime;

			// Provide a default for the submission type code.
			if (submissionTypeCode == null)
				submissionTypeCode = SubmissionType.NeverMatch;

			// While the optimistic concurrency checking is disabled for the external methods, the internal methods
			// still need to perform the check.  This ncurrency checking logic by finding the current row version to be
			// will bypass the coused when the internal method is called.
			ServerMarketData.WorkingOrderRow workingOrderRow = workingOrderTable.FindByWorkingOrderId(workingOrderId);
			rowVersion = ((long)(workingOrderRow[workingOrderTable.RowVersionColumn]));

			// Call the internal method to complete the operation.
			MarkThree.Guardian.Core.WorkingOrder.Update(adoTransaction, sqlTransaction, ref rowVersion, null, blotterId, null, null, destinationId,
				null, isAgencyMatch, null, null, isBrokerMatch, isHedgeMatch, isInstitutionMatch, limitPrice, maximumVolatility, newsFreeTime, modifiedUserId,
				modifiedTime, orderTypeCode, priceTypeCode, securityId, settlementId, startTime, statusCode, stopPrice, stopTime, null, 
				submittedQuantity, submittedTime, timeInForceCode, null, uploadedTime, workingOrderId);

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

	}

}
