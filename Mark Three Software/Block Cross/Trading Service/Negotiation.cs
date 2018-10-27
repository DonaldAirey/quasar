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
	/// Summary description for Negotiation.
	/// </summary>
	public class Negotiation
	{

		/// This value is used to map the object to a persistent storage device.  The parameters for the storage
		/// are found in the configuration file for this service.
		public static string PersistentStore = "Guardian";
        
		/// This member provides access to the in-memory database.
		private static ServerMarketData serverGuardianData = new ServerMarketData();

		private static int destinationId;

		static Negotiation()
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);
				ServerMarketData.DestinationLock.AcquireReaderLock(Timeout.Infinite);

				// The destination for all orders matched through this system is fixed by a setting in the configuration file.
				object guardianEcn = ConfigurationManager.AppSettings["GuardianEcn"];
				Negotiation.destinationId = MarkThree.Guardian.External.Destination.FindKey(null, "destinationId", guardianEcn);
				if (Negotiation.destinationId == -1)
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

			// These tables are required for the operation.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.MatchLock);

			// The Core Library will take care of the main part of the record.
			Core.Negotiation.Insert(adoTransaction);

		}
        
		/// <summary>Inserts a Negotiation record using Metadata Parameters.</summary>
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

			int negotiationId = int.MinValue;
			long rowVersion = long.MinValue;

			ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
			if (matchRow != null)
			{

				// See if there is already a pending offer.
				bool isFound = false;
				foreach (ServerMarketData.NegotiationRow innerNegotiationRow in matchRow.GetNegotiationRows())
				{

					if (innerNegotiationRow.StatusCode == Status.Declined)
						throw new Exception("This offer has previously been declined.");

					if (innerNegotiationRow.StatusCode == Status.Pending)
					{

						// Call the internal method to complete the operation.
						rowVersion = innerNegotiationRow.RowVersion;
						negotiationId = innerNegotiationRow.NegotiationId;
						MarkThree.Guardian.Core.Negotiation.Update(adoTransaction, sqlTransaction, ref rowVersion,
							matchId, null, negotiationId, null, Status.Declined);

						isFound = true;

					}

				}

				// Call the internal method to complete the operation.
				if (!isFound)
					negotiationId = MarkThree.Guardian.Core.Negotiation.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, matchId,
						0.0m, Status.Declined);

				// If there's a counter offer, then notify the couter part that the offer has been declined.
				// This will find the contra matching record.
				int contraMatchIndex =
					ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId.Find(new object[] { matchRow.ContraOrderId, matchRow.WorkingOrderId });
				if (contraMatchIndex == -1)
					throw new Exception(string.Format("Corruption: the match record for {0}, {1} can't be found", matchRow.ContraOrderId, matchRow.WorkingOrderId));
				ServerMarketData.MatchRow contraMatchRow =
					(ServerMarketData.MatchRow)ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId[contraMatchIndex].Row;

				// When both sides have agreed to the Negotiation, the Destination Orders are generated.
				foreach (MarketData.NegotiationRow contraNegotiationRow in contraMatchRow.GetNegotiationRows())
					if (contraNegotiationRow.StatusCode == Status.Pending)
					{
						rowVersion = contraNegotiationRow.RowVersion;
						MarkThree.Guardian.Core.Negotiation.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null, contraNegotiationRow.NegotiationId, null, Status.Declined);
					}

			}

			// Return values.
			parameters["rowVersion"] = rowVersion;
			parameters.Return = negotiationId;

		}

		/// <summary>Collects the table lock request(s) for an Insert operation</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit</param>
		public static void Offer(AdoTransaction adoTransaction)
		{

			// These table must be locked for this operation.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.MatchLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.WorkingOrderLock);
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceLock);

			// The Core Library will take care of locking the primary table of this transaction.
			Core.Negotiation.Insert(adoTransaction);
			Core.DestinationOrder.Insert(adoTransaction);
			Core.Execution.Insert(adoTransaction);

		}
        
		/// <summary>Inserts a Negotiation record using Metadata Parameters.</summary>
		/// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
		/// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
		public static void Offer(ParameterList parameters)
		{

			// Accessor for the Match Table.
			ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;

			// Extract the parameters from the command batch.
			AdoTransaction adoTransaction = parameters["adoTransaction"];
			SqlTransaction sqlTransaction = parameters["sqlTransaction"];
			int matchId = parameters["matchId"];
			decimal quantity = parameters["quantity"];

			int negotiationId = int.MinValue;
			long rowVersion = long.MinValue;

			ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
			if (matchRow != null)
			{
			
				// Rule #1: Insure that there are no pending offers.
				foreach (ServerMarketData.NegotiationRow innerNegotiationRow in matchRow.GetNegotiationRows())
				{

					if (innerNegotiationRow.StatusCode == Status.Pending)
						throw new Exception("There is already an offer pending.");

					if (innerNegotiationRow.StatusCode == Status.Declined)
						throw new Exception("This offer has previously been declined.");

				}
				
				// Time stamps and user stamps
				int createdUserId = ServerMarketData.UserId;
				DateTime createdTime = DateTime.Now;
				int modifiedUserId = ServerMarketData.UserId;
				DateTime modifiedTime = DateTime.Now;

				// This will find the contra matching record.
				int contraMatchIndex =
					ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId.Find(new object[] { matchRow.ContraOrderId, matchRow.WorkingOrderId });
				if (contraMatchIndex == -1)
					throw new Exception(string.Format("Corruption: the match record for {0}, {1} can't be found", matchRow.ContraOrderId, matchRow.WorkingOrderId));
				ServerMarketData.MatchRow contraMatchRow =
					(ServerMarketData.MatchRow)ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId[contraMatchIndex].Row;

				// When both sides have agreed to the Negotiation, the Destination Orders are generated.
				ServerMarketData.NegotiationRow contraNegotiationRow = null;				
				foreach (MarketData.NegotiationRow innerNegotiationRow in contraMatchRow.GetNegotiationRows())
					if (innerNegotiationRow.StatusCode == Status.Pending)
					{
						contraNegotiationRow = innerNegotiationRow;
						break;
					}

				// This means that there's an offer on the other side.
				if (contraNegotiationRow == null)
				{

					// There is no opposite side of this transaction yet.  It will be placed in the negotation table and wait there
					// until it times out, or the other side accepts the offer.
					long negotiationRowVersion = long.MinValue;
					negotiationId = MarkThree.Guardian.Core.Negotiation.Insert(adoTransaction, sqlTransaction, ref negotiationRowVersion,
						null, matchId, quantity, Status.Pending);

				}
				else
				{

					// At this point, there is an offer on both sides of the match for a follow-on order.  We'll create orders and 
					// executions for both sides of the trade for the minimum agreed upon quantity.
					ServerMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;
					ServerMarketData.WorkingOrderRow contraOrderRow = contraNegotiationRow.MatchRow.WorkingOrderRow;

					// The quantity of this negotiation will be the minimum of the two offers.
					decimal matchedQuantity = quantity < contraNegotiationRow.Quantity ? quantity : contraNegotiationRow.Quantity;

					// Create the order on this side of the trade.
					long rowVersionDestionation = long.MinValue;
					int destinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction,
						ref rowVersionDestionation, null, null, createdTime, createdUserId, Negotiation.destinationId, null, null,
						null, workingOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
						workingOrderRow.OrderTypeCode, matchedQuantity, workingOrderRow.PriceTypeCode, State.Acknowledged, Status.New,
						workingOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId, workingOrderRow.TimeInForceCode,
						workingOrderRow.WorkingOrderId);

					// Create the Execution for this side of the trade.
					long rowVersionExecution = long.MinValue;
					int executionId = MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction,
						ref rowVersionExecution, null, null, createdTime, createdUserId, destinationOrderId, State.Acknowledged,
						workingOrderRow.PriceRow.LastPrice, matchedQuantity, null, null, null, modifiedTime, modifiedUserId,
						null, null, null, DateTime.Now, null, State.Sent, DateTime.Now, null, null, null, null);

					// There is no opposite side of this transaction yet.  It will be placed in the negotation table and wait there
					// until it times out, or the other side accepts the offer.
					long negotiationRowVersion = long.MinValue;
					negotiationId = MarkThree.Guardian.Core.Negotiation.Insert(adoTransaction, sqlTransaction, ref negotiationRowVersion,
						executionId, matchId, quantity, Status.Accepted);

					// Create an order for the agreed upon quantity.
					int contraDestinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction,
						ref rowVersionDestionation, null, null, createdTime, createdUserId, Negotiation.destinationId, null, null,
						null, contraOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
						contraOrderRow.OrderTypeCode, matchedQuantity, contraOrderRow.PriceTypeCode, State.Acknowledged, Status.New,
						contraOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId, contraOrderRow.TimeInForceCode,
						contraOrderRow.WorkingOrderId);

					// Create an execution for the agreed upon quantity
					int contraExecutionId = MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction,
						ref rowVersionExecution, null, null, createdTime, createdUserId, contraDestinationOrderId,
						State.Acknowledged, contraOrderRow.PriceRow.LastPrice, matchedQuantity, null, null, null, modifiedTime,
						modifiedUserId, null, null, null, DateTime.Now, null, State.Sent, DateTime.Now, null, null, null, null);

					// Update the contra offer.
					long contraNegotiationRowVersion = contraNegotiationRow.RowVersion;
					MarkThree.Guardian.Core.Negotiation.Update(adoTransaction, sqlTransaction, ref contraNegotiationRowVersion, contraExecutionId, null,
						contraNegotiationRow.NegotiationId, null, Status.Accepted);


				}
			
			}

			// Return values.
			parameters["rowVersion"] = rowVersion;
			parameters.Return = negotiationId;

		}

	}

}
