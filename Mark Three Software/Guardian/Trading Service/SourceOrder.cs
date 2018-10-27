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
	public class SourceOrder
	{

		/// This value is used to map the object to a persistent storage device.  The parameters for the storage
		/// are found in the configuration file for this service.
		public static string PersistentStore = "Guardian";
        
		/// This member provides access to the in-memory database.
		private static ServerMarketData serverGuardianData = new ServerMarketData();

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Insert(AdoTransaction adoTransaction)
		{

			// These table lock(s) are required for resloving the external ids.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.BlotterLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.DestinationLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.OrderTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TimeInForceLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TraderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);

			// The Core Library will take care of the main part of the record.
			Core.SourceOrder.Insert(adoTransaction);
			Core.WorkingOrder.Insert(adoTransaction);
			Core.Equity.Update(adoTransaction);

		}
        
		/// <summary>Inserts a SourceOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Insert(ParameterList parameters)
		{

			// Accessor for the SourceOrder Table.
			ServerMarketData.SourceOrderDataTable sourceOrderTable = ServerMarketData.SourceOrder;
			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object blotterId = parameters["blotterId"].Value;
			object configurationId = parameters["configurationId"].Value;
			object destinationId = parameters["destinationId"].Value;
			object externalBlotterId = parameters["blotterId"].Value;
			object externalSourceOrderId = parameters["sourceOrderId"].Value;
			object externalDestinationId = parameters["destinationId"].Value;
			object externalId0 = parameters["externalId0"].Value;
			object isAdvertised = parameters["isAdvertised"].Value;
			object isAgencyMatch = parameters["isAgencyMatch"].Value;
			object isBrokerMatch = parameters["isBrokerMatch"].Value;
			object isCanceled = parameters["isCanceled"].Value;
			object isSteppedIn = parameters["isSteppedIn"].Value;
			object isHedgeMatch = parameters["isHedgeMatch"].Value;
			object isHeld = parameters["isHeld"].Value;
			object isInstitutionMatch = parameters["isInstitutionMatch"].Value;
			object limitPrice = parameters["limitPrice"].Value;
			object maximumVolatility = parameters["maximumVolatility"].Value;
			object newsFreeTime = parameters["newsFreeTime"].Value;
			System.Decimal orderedQuantity = parameters["orderedQuantity"];
			int orderTypeCode = parameters["orderTypeCode"];
			int priceTypeCode = parameters["priceTypeCode"];
			object receivedTime = parameters["receivedTime"].Value;
			string externalSecurityId = parameters["securityId"];
			string externalSettlementId = parameters["settlementId"];
			object workingOrderId = parameters["workingOrderId"].Value;
			object startTime = parameters["startTime"].Value;
			object stopTime = parameters["stopTime"].Value;
			object stopPrice = parameters["stopPrice"].Value;
			object submissionTypeCode = parameters["submissionTypeCode"].Value;
			object submittedQuantity = parameters["submittedQuantity"].Value;
			object targetPrice = parameters["targetPrice"].Value;
			int timeInForceCode = parameters["timeInForceCode"];
			object uploadedTime = parameters["uploadedTime"].Value;
			object externalWorkingOrderId = parameters["workingOrderId"].Value;

			// Look up the external values.
			int securityId = External.Security.FindRequiredKey("US TICKER", "securityId", externalSecurityId);
			int settlementId = External.Security.FindRequiredKey("US TICKER", "settlementId", externalSettlementId);

			// This will get the row version of the newly added Source Order.
			long rowVersion = long.MinValue;

			// Time stamps and user stamps
			DateTime currentTime = DateTime.Now;
			int createdUserId = ServerMarketData.UserId;
			DateTime createdTime = DateTime.Now;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = DateTime.Now;
			int statusCode = Status.New;

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

			// Rule #1 - Blotter must be specified if the working order is not specified.
			if (blotterId == null && workingOrderId == null)
				throw new Exception("Either a blotter or a Working Order must be specified as a location for this Order");

			// If the incoming order specified a submitted quantity, then automatically enter the order into the cross.
			if (submissionTypeCode == null)
				submissionTypeCode = SubmissionType.NeverMatch;

			object submittedTime = null;
			if ((int)submissionTypeCode != SubmissionType.NeverMatch)
			{
				submittedTime = DateTime.Now;
				if (submittedQuantity == null)
					submittedQuantity = orderedQuantity;
			}

			// Update the rowversion of the object associated with this trade.
			ServerMarketData.SecurityRow securityRow = ServerMarketData.Security.FindBySecurityId(securityId);
			if (securityRow != null)
			{

				foreach (ServerMarketData.EquityRow equityRow in securityRow.GetEquityRowsBySecurityEquityEquityId())
				{

					long equityRowVersion = equityRow.RowVersion;
					Core.Equity.Update(adoTransaction, sqlTransaction, ref equityRowVersion, null, null, null, null, null,
						null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
						equityRow.EquityId, null, null, null, null, null, null, null);

				}

				foreach (ServerMarketData.PriceRow priceRow in securityRow.GetPriceRows())
				{

					long priceRowVersion = priceRow.RowVersion;
					Core.Price.Update(adoTransaction, sqlTransaction, ref priceRowVersion, null, null, null, null, null, null, null,
						null, null, null, null, null, priceRow.SecurityId, null, null);

				}

			}

			ServerMarketData.SecurityRow settlementRow = ServerMarketData.Security.FindBySecurityId(settlementId);
			if (settlementRow != null)
			{
				foreach (ServerMarketData.CurrencyRow currencyRow in settlementRow.GetCurrencyRows())
				{
					long currencyRowVersion = currencyRow.RowVersion;
					Core.Currency.Update(adoTransaction, sqlTransaction, ref currencyRowVersion, null, null, null, null, null,
						null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 
						currencyRow.CurrencyId, null, null, null);

				}
			}

			// An order can be automatically associated with a working order if the working order is specified when the order is
			// created.
			if (workingOrderId == null)
			{
				long timerRowVersion = long.MinValue;
				int timerId = Core.Timer.Insert(adoTransaction, sqlTransaction, ref timerRowVersion, currentTime, null, false, currentTime, 0);
				workingOrderId = Core.WorkingOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, (int)blotterId,
					createdTime, createdUserId, destinationId, null, isAgencyMatch, null, true, isBrokerMatch, isHedgeMatch, isInstitutionMatch, limitPrice,
					maximumVolatility, modifiedTime, modifiedUserId, newsFreeTime, orderTypeCode, priceTypeCode, securityId,
					settlementId, startTime, statusCode, stopPrice, stopTime, (int)submissionTypeCode, submittedQuantity, submittedTime,
					timeInForceCode, timerId, null);
			}

			// Call the internal method to complete the operation.
			MarkThree.Guardian.Core.SourceOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, createdTime, createdUserId,
				destinationId, externalId0, isAdvertised, isHeld, isCanceled, isHeld, isSteppedIn, submissionTypeCode,
				limitPrice, maximumVolatility, modifiedTime, modifiedUserId, newsFreeTime, orderTypeCode,
				orderedQuantity, priceTypeCode, receivedTime, securityId, settlementId, startTime, statusCode,
				stopPrice, stopTime, submittedQuantity, submittedTime, targetPrice, timeInForceCode, (int)workingOrderId);

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

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
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SubmissionTypeLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TimeInForceLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TraderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);

			// The Core Library will take care of the main part of the record.
			Core.SourceOrder.Insert(adoTransaction);
			Core.WorkingOrder.Insert(adoTransaction);
			Core.Equity.Update(adoTransaction);

		}

		/// <summary>Inserts a SourceOrder record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void InsertFile(ParameterList parameters)
		{

			// Accessor for the SourceOrder Table.
			ServerMarketData.SourceOrderDataTable sourceOrderTable = ServerMarketData.SourceOrder;
			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			object configurationId = parameters["configurationId"].Value;
			object externalBlotterId = parameters["blotterId"].Value;
			object externalSourceOrderId = parameters["sourceOrderId"].Value;
			object externalDestinationId = parameters["destinationId"].Value;
			object externalId0 = parameters["externalId0"].Value;
			object isAdvertised = parameters["isAdvertised"].Value;
			object isCanceled = parameters["isCanceled"].Value;
			object isSteppedIn = parameters["isSteppedIn"].Value;
			object limitPrice = parameters["limitPrice"].Value;
			System.Decimal orderedQuantity = parameters["orderedQuantity"];
			string externalOrderTypeCode = parameters["orderTypeCode"];
			string externalPriceTypeCode = parameters["priceTypeCode"];
			object receivedTime = parameters["receivedTime"].Value;
			string externalSecurityId = parameters["securityId"];
			string externalSettlementId = parameters["settlementId"];
			object stopPrice = parameters["stopPrice"].Value;
			object targetPrice = parameters["targetPrice"].Value;
			object externalSubmissionTypeCode = parameters["submissionTypeCode"].Value;
			string externalTimeInForceCode = parameters["timeInForceCode"];
			object uploadedTime = parameters["uploadedTime"].Value;
			object externalWorkingOrderId = parameters["workingOrderId"].Value;
			// The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			// event it's needed for operations within the batch.
			long rowVersion = long.MinValue;
			// Resolve External Identifiers
			object blotterId = External.Blotter.FindOptionalKey(configurationId, "blotterId", externalBlotterId);
			int sourceOrderId = External.SourceOrder.FindKey(configurationId, "sourceOrderId", (string)externalSourceOrderId);
			int orderTypeCode = External.OrderType.FindRequiredKey(configurationId, "orderTypeCode", externalOrderTypeCode);
			object destinationId = External.Destination.FindOptionalKey(configurationId, "destinationId", externalDestinationId);
			int priceTypeCode = External.PriceType.FindRequiredKey(configurationId, "priceTypeCode", externalPriceTypeCode);
			int securityId = External.Security.FindRequiredKey(configurationId, "securityId", externalSecurityId);
			int settlementId = External.Security.FindRequiredKey(configurationId, "settlementId", externalSettlementId);
			int timeInForceCode = External.TimeInForce.FindRequiredKey(configurationId, "timeInForceCode", externalTimeInForceCode);
			object submissionTypeCode = External.SubmissionType.FindOptionalKey(configurationId, "submissionTypeCode", externalSubmissionTypeCode);
			object workingOrderId = External.WorkingOrder.FindOptionalKey(configurationId, "workingOrderId", externalWorkingOrderId);

			// Optional Parameters
			object canceledQuantity = parameters["canceledQuantity"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["canceledQuantity"].Value);
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
			object specifiedLimit = parameters["specifiedLimit"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["specifiedLimit"].Value);
			object maximumVolatility = parameters["maximumVolatility"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["maximumVolatility"].Value);
			object submittedQuantity = parameters["submittedQuantity"] is MissingParameter ? (object)null :
				Convert.ToDecimal(parameters["submittedQuantity"].Value);
			object startTime = parameters["startTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["startTime"].Value);
			object stopTime = parameters["stopTime"] is MissingParameter ? (object)null :
				Convert.ToDateTime(parameters["stopTime"].Value);
			object newsFreeTime = parameters["newsFreeTime"] is MissingParameter ? (object)null :
				Convert.ToInt32(parameters["newsFreeTime"].Value);

			// Time stamps and user stamps
			int createdUserId = ServerMarketData.UserId;
			DateTime createdTime = DateTime.Now;
			int modifiedUserId = ServerMarketData.UserId;
			DateTime modifiedTime = DateTime.Now;
			int statusCode = Status.New;

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

			// Rule #1 - Blotter must be specified if the working order is not specified.
			if (blotterId == null && workingOrderId == null)
				throw new Exception("Either a blotter or a Working Order must be specified as a location for this Order");

			// If the incoming order specified a submitted quantity, then automatically enter the order into the cross.
			if (submissionTypeCode == null)
				submissionTypeCode = SubmissionType.NeverMatch;
			
			object submittedTime = null;
			if ((int)submissionTypeCode != SubmissionType.NeverMatch)
			{
				submittedTime = DateTime.Now;
				if (submittedQuantity == null)
					submittedQuantity = orderedQuantity;
			}

			// Update the rowversion of the object associated with this trade.
			ServerMarketData.SecurityRow securityRow = ServerMarketData.Security.FindBySecurityId(securityId);
			if (securityRow != null)
			{

				foreach (ServerMarketData.EquityRow equityRow in securityRow.GetEquityRowsBySecurityEquityEquityId())
				{

					long equityRowVersion = equityRow.RowVersion;
					Core.Equity.Update(adoTransaction, sqlTransaction, ref equityRowVersion, null, null, null, null, null,
						null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
						null, equityRow.EquityId, null, null, null, null, null, null, null);

				}

				foreach (ServerMarketData.PriceRow priceRow in securityRow.GetPriceRows())
				{

					long priceRowVersion = priceRow.RowVersion;
					Core.Price.Update(adoTransaction, sqlTransaction, ref priceRowVersion, null, null, null, null, null, null, null,
						null, null, null, null, null, priceRow.SecurityId, null, null);

				}

			}

			ServerMarketData.SecurityRow settlementRow = ServerMarketData.Security.FindBySecurityId(settlementId);
			if (settlementRow != null)
			{
				foreach (ServerMarketData.CurrencyRow currencyRow in settlementRow.GetCurrencyRows())
				{
					long currencyRowVersion = currencyRow.RowVersion;
					Core.Currency.Update(adoTransaction, sqlTransaction, ref currencyRowVersion, null, null, null, null, null,
						null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
						currencyRow.CurrencyId, null, null, null);

				}
			}

			// An order can be automatically associated with a working order if the working order is specified when the order is
			// created.
			if (workingOrderId == null)
			{
				long timerRowVersion = long.MinValue;
				DateTime currentTime = DateTime.Now;
				int timerId = Core.Timer.Insert(adoTransaction, sqlTransaction, ref timerRowVersion, currentTime, null, false, currentTime, 0);
				workingOrderId = Core.WorkingOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, (int)blotterId,
					createdTime, createdUserId, destinationId, null, isAgencyMatch, null, true, isBrokerMatch, isHedgeMatch,
					isInstitutionMatch, limitPrice, maximumVolatility,
					modifiedTime, modifiedUserId, newsFreeTime, orderTypeCode, priceTypeCode, securityId,
					settlementId, startTime, statusCode, stopPrice, stopTime, (int)submissionTypeCode, submittedQuantity,
					submittedTime, timeInForceCode, timerId, null);
			}

			// The load operation will create a record if it doesn't exist, or update an existing record.  The external
			// identifier is used to determine if a record exists with the same key.
			if (sourceOrderId == int.MinValue)
			{

				// Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
				// external method is called with the same 'configurationId' parameter.
				int externalKeyIndex = External.SourceOrder.GetExternalKeyIndex(configurationId, "sourceOrderId");
				object[] externalIdArray = new object[1];
				externalIdArray[externalKeyIndex] = externalSourceOrderId;
				externalId0 = externalIdArray[0];

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.SourceOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, createdTime, createdUserId,
					destinationId, externalId0, isAdvertised, isHeld, isCanceled, isHeld, isSteppedIn, submissionTypeCode,
					limitPrice, maximumVolatility, modifiedTime, modifiedUserId, newsFreeTime, orderTypeCode,
					orderedQuantity, priceTypeCode, receivedTime, securityId, settlementId, startTime, statusCode,
					stopPrice, stopTime, submittedQuantity, submittedTime, targetPrice, timeInForceCode, (int)workingOrderId);

			}
			else
			{

				// While the optimistic concurrency checking is disabled for the external methods, the internal methods
				// still need to perform the check.  This ncurrency checking logic by finding the current row version to be
				// will bypass the coused when the internal method is called.
				ServerMarketData.SourceOrderRow sourceOrderRow = sourceOrderTable.FindBySourceOrderId((int)sourceOrderId);
				rowVersion = ((long)(sourceOrderRow[sourceOrderTable.RowVersionColumn]));

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.SourceOrder.Update(adoTransaction, sqlTransaction, ref rowVersion, createdTime, createdUserId,
					(int)sourceOrderId, destinationId, stopTime, externalId0, isAdvertised, isHeld, isCanceled, isHeld,
					isSteppedIn, submissionTypeCode, limitPrice, maximumVolatility, modifiedTime, modifiedUserId, newsFreeTime,
					orderTypeCode, orderedQuantity, priceTypeCode, receivedTime, securityId, settlementId,
					startTime, statusCode, stopPrice, submittedQuantity, submittedTime, targetPrice, timeInForceCode,
					workingOrderId);

			}

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

	}

}
