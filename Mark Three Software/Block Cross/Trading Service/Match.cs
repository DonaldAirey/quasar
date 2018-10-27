namespace MarkThree.Guardian.Trading
{

	using MarkThree.Guardian;
	using MarkThree.Guardian.Server;
	using MarkThree;
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for Match.
	/// </summary>
	public class Match
	{

		/// This value is used to map the object to a persistent storage device.  The parameters for the storage
		/// are found in the configuration file for this service.
		public static string PersistentStore = "Guardian";
        
		/// This member provides access to the in-memory database.
		private static ServerMarketData serverGuardianData = new ServerMarketData();

		private static int destinationId;

		static Match()
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);
				ServerMarketData.DestinationLock.AcquireReaderLock(Timeout.Infinite);

				// The destination for all orders matched through this system is fixed by a setting in the configuration file.
				object guardianEcn = ConfigurationManager.AppSettings["GuardianEcn"];
				Match.destinationId = MarkThree.Guardian.External.Destination.FindKey(null, "destinationId", guardianEcn);
				if (Match.destinationId == -1)
					throw new Exception("There is no Destination specified in the configuration file for matched trades.");

			}
			finally
			{

				// Release the tables.
				if (ServerMarketData.DestinationLock.IsReaderLockHeld)
					ServerMarketData.DestinationLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);

			}

		}

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Decline(AdoTransaction adoTransaction)
		{

			// The Core Library will take care of the main part of the record.
			Core.Match.Update(adoTransaction);

		}
        
		/// <summary>Inserts a Match record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Decline(ParameterList parameters)
		{

			// Accessor for the Match Table.
			ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			int matchId = parameters["matchId"];
			long rowVersion = parameters["rowVersion"];

			ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
			if (matchRow != null)
			{
			
				// Time stamps and user stamps
				int modifiedUserId = ServerMarketData.UserId;
				DateTime modifiedTime = DateTime.Now;

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null, matchRow.MatchId, null, Status.Declined, null);

				// Call the internal method to decline the contra side of this match.
				foreach (ServerMarketData.MatchRow contraMatchRow in matchTable.Rows)
					if (contraMatchRow.WorkingOrderId == matchRow.ContraOrderId &&
						contraMatchRow.ContraOrderId == matchRow.WorkingOrderId)
					{
						rowVersion = contraMatchRow.RowVersion;
						MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null, contraMatchRow.MatchId, null, Status.Declined, null);
					}

			}

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Accept(AdoTransaction adoTransaction)
		{

			// The Core Library will take care of the main part of the record.
			Core.Match.Update(adoTransaction);
			Core.DestinationOrder.Insert(adoTransaction);
			Core.Execution.Insert(adoTransaction);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.WorkingOrderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceLock);

		}
        
		/// <summary>Inserts a Match record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Accept(ParameterList parameters)
		{

			// Accessor for the Match Table.
			ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			int matchId = parameters["matchId"];
			long rowVersion = parameters["rowVersion"];

			ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
			if (matchRow != null)
			{
			
				// Time stamps and user stamps
				int createdUserId = ServerMarketData.UserId;
				DateTime createdTime = DateTime.Now;
				int modifiedUserId = ServerMarketData.UserId;
				DateTime modifiedTime = DateTime.Now;

				// Call the internal method to complete the operation.
				MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null,
					matchRow.MatchId, null, Status.Accepted, null);

				// This is the working order associated with the match.
				ServerMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;

				// This will find the contra order.
				int contraMatchIndex =
					ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId.Find(new object[] { matchRow.ContraOrderId, matchRow.WorkingOrderId });
				if (contraMatchIndex == -1)
					throw new Exception(string.Format("Corruption: the match record for {0}, {1} can't be found", matchRow.ContraOrderId, matchRow.WorkingOrderId));
				ServerMarketData.MatchRow contraMatchRow =
					(ServerMarketData.MatchRow)ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId[contraMatchIndex].Row;

				// When both sides have agreed to the match, the Destination Orders are generated.
				if (contraMatchRow.StatusCode != Status.Accepted)
					return;

				ServerMarketData.WorkingOrderRow contraOrderRow = contraMatchRow.WorkingOrderRow;

				decimal quantity = workingOrderRow.SubmittedQuantity < contraOrderRow.SubmittedQuantity ? workingOrderRow.SubmittedQuantity : contraOrderRow.SubmittedQuantity;

				long rowVersionDestionation = long.MinValue;
				int destinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction,
					ref rowVersionDestionation, null, null, createdTime, createdUserId, Match.destinationId, null, null,
					null, workingOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
					workingOrderRow.OrderTypeCode, quantity, workingOrderRow.PriceTypeCode, State.Acknowledged, Status.New,
					workingOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId, workingOrderRow.TimeInForceCode,
					workingOrderRow.WorkingOrderId);

				long rowVersionExecution = long.MinValue;
				MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction, ref rowVersionExecution, null, null, null,
					null, createdTime, createdUserId, destinationOrderId, State.Acknowledged, workingOrderRow.PriceRow.LastPrice,
					quantity, null, null, null, modifiedTime, modifiedUserId, null, null, null, DateTime.Now, null, State.Sent,
					DateTime.Now, null, null, null, null);

				rowVersion = contraMatchRow.RowVersion;
				MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null,
					contraMatchRow.MatchId, null, Status.Accepted, null);

				int contraDestinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction,
					ref rowVersionDestionation, null, null, createdTime, createdUserId, Match.destinationId, null, null,
					null, contraOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
					contraOrderRow.OrderTypeCode, quantity, contraOrderRow.PriceTypeCode, State.Acknowledged, Status.New,
					contraOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId, contraOrderRow.TimeInForceCode,
					contraOrderRow.WorkingOrderId);

				MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction, ref rowVersionExecution, null, null, null,
					null, createdTime, createdUserId, contraDestinationOrderId, State.Acknowledged,
					contraOrderRow.PriceRow.LastPrice, quantity, null, null, null, modifiedTime, modifiedUserId, null, null, null,
					DateTime.Now, null, State.Sent, DateTime.Now, null, null, null, null);
			
			}

			// Return values.
			parameters["rowVersion"] = rowVersion;

		}

	}

}
