namespace MarkThree.Guardian.Server
{

	using MarkThree;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Server;
	using System;
	using System.ComponentModel;
	using System.Configuration;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Threading;

	/// <summary>Finds matching orders and negotiations the exchange of securities.</summary>
	/// <copyright>Copyright (C) 1998-2005 Mark Three Software -- All Rights Reserved.</copyright>
	public class MatchingService : System.ComponentModel.Component
	{

		// Constants
		private const System.Int32 timerPeriod = 1000;
		private const System.Int32 threadWait = 100;
		private const System.Int32 marketSleep = 900;

		// Private Members
		private static MarkThree.Guardian.Server.ServerMarketData serverMarketData;
		private static MarkThree.ActionQueue workingOrderQueue;
		private static MarkThree.ActionQueue negotiationQueue;
		private static System.Boolean isStarted;
		private static System.Boolean isWorkingOrderQueueThreadRunning;
		private static System.Boolean isNegotiationQueueThreadRunning;
		private static System.ComponentModel.IContainer components;
		private static System.Data.DataView timerView;
		private static System.Int32 destinationId;
		private static System.Threading.Thread workingOrderQueueThread;
		private static System.Threading.Thread negotiationQueueThread;
		private static System.Threading.Timer timer;
		private static System.TimeSpan negotiationTime;

		/// <summary>
		/// This object will simulate an auction market on equities.
		/// </summary>
		static MatchingService()
		{

			// Install the Server Market Data in this container.
			MatchingService.components = new System.ComponentModel.Container();

			// This service will require access to the server's data model.
			MatchingService.serverMarketData = new ServerMarketData(MatchingService.components);

			// This queue is filled up with Working Orders that need to be serviced because something changed the matching 
			// criteria.
			MatchingService.workingOrderQueue = new ActionQueue();

			// This queue contains negotiation records that alter the state of the matching records.
			MatchingService.negotiationQueue = new ActionQueue();

			// These threads service the records that are identified in the event handlers.  The event handlers examine each 
			// records when the data changes to see if it requires more sophisticated handling (than an event handler can
			// provide).  If it does, it is placed on a queue and serviced in a seperate thread.  This prevents any single data
			// model action from taking too long.
			MatchingService.isWorkingOrderQueueThreadRunning = false;
			MatchingService.isNegotiationQueueThreadRunning = false;

			// This is the time a user is given to respond to the notification of a possible match.
			MatchingService.negotiationTime = TimeSpan.FromMilliseconds(30000);

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);
				ServerMarketData.DestinationLock.AcquireReaderLock(Timeout.Infinite);

				// The destination for all orders matched through this system is fixed by a setting in the configuration file.
				object guardianEcn = ConfigurationManager.AppSettings["GuardianEcn"];
				MatchingService.destinationId = MarkThree.Guardian.External.Destination.FindKey(null, "destinationId", guardianEcn);
				if (MatchingService.destinationId == -1)
					throw new Exception("There is no Destination specified in the configuration file for matched trades.");

			}
			finally
			{

				// Release the tables.
				if (ServerMarketData.DestinationLock.IsReaderLockHeld)
					ServerMarketData.DestinationLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);

			}

			// This timer is used to service all the time-sensitive records.  As it counts down, it will cycle through the 'Timer' 
			// table and update the 'currentTime' element until the timer has expired.
			MatchingService.timer = new Timer(new TimerCallback(TimerCallback), null, 0, MatchingService.timerPeriod);

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="adoTransaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Start(AdoTransaction adoTransaction)
		{

			// These tables must be locked to start the Matching Service.
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.NegotiationLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

		}

		/// <summary>
		/// Starts the matching service.
		/// </summary>
		/// <param name="parameters">A meta-data list of parameters (not used).</param>
		public static void Start(ParameterList parameters)
		{

			// This will prevent multiple instances of the service from being started.
			if (MatchingService.isStarted)
				throw new Exception("Market Matching Service is already running.");

			// These event handlers will update the matching conditions as the underlying records change.
			ServerMarketData.Negotiation.NegotiationRowChanging +=
				new ServerMarketData.NegotiationRowChangeEventHandler(Negotiation_NegotiationRowChanging);
			ServerMarketData.WorkingOrder.WorkingOrderRowChanging +=
				new ServerMarketData.WorkingOrderRowChangeEventHandler(WorkingOrder_WorkingOrderRowChanging);

			// This filter is provided for the Timer thread to reduce the number of timers it needs to update.
			MatchingService.timerView = new DataView(ServerMarketData.Timer, "IsActive=true", null, DataViewRowState.CurrentRows);

			// This thread will pull the modified Working Order record actions off the queue and handle them.
			MatchingService.workingOrderQueueThread = new Thread(new ThreadStart(WorkingOrderQueueThread));
			MatchingService.workingOrderQueueThread.Name = "Working Order Queue Handler";
			MatchingService.workingOrderQueueThread.IsBackground = true;
			MatchingService.workingOrderQueueThread.Start();

			// This thread will pull the modified Negotiation record actions off the queue and handle them.
			MatchingService.negotiationQueueThread = new Thread(new ThreadStart(NegotiationQueueThread));
			MatchingService.negotiationQueueThread.Name = "Working Order Queue Handler";
			MatchingService.negotiationQueueThread.IsBackground = true;
			MatchingService.negotiationQueueThread.Start();

			// This flag will prevent multiple instances of the service from running at the same time.
			MatchingService.isStarted = true;

			// Write an indication to the event log that the service is running.
			MarkThree.EventLog.Information("Matching Service started.");

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="adoTransaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Stop(AdoTransaction adoTransaction)
		{

			// These tables must be locked to stop the Matching Service.
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.NegotiationLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

		}

		/// <summary>
		/// Terminates the matching service.
		/// </summary>
		/// <param name="parameters">A meta-data list of parameters (not used).</param>
		public static void Stop(ParameterList parameters)
		{

			// Don't attempt to stop the service if it hasn't been started.
			if (!MatchingService.isStarted)
				throw new Exception("Market Matching has not been started.");

			// Terminate the queues that handle the modified record actions.
			MatchingService.IsWorkingOrderQueueThreadRunning = false;
			MatchingService.IsNegotiationQueueThreadRunning = false;

			// If the threads don't shut down on their own -- which is possible because they are waiting for an item to appear
			// in the queue -- then force them to shut down.
			if (!MatchingService.workingOrderQueueThread.Join(MatchingService.threadWait))
				MatchingService.workingOrderQueueThread.Abort();
			if (!MatchingService.workingOrderQueueThread.Join(MatchingService.threadWait))
				MatchingService.workingOrderQueueThread.Abort();

			// Disengage the service from handling the changes to the data model.
			ServerMarketData.Negotiation.NegotiationRowChanging -= new MarkThree.Guardian.DataSetMarket.NegotiationRowChangeEventHandler(Negotiation_NegotiationRowChanging);
			ServerMarketData.WorkingOrder.WorkingOrderRowChanging -= new MarkThree.Guardian.DataSetMarket.WorkingOrderRowChangeEventHandler(WorkingOrder_WorkingOrderRowChanging);

			// This service can be started up again after it is successfully shut down.
			MatchingService.isStarted = false;

			// Mark the stopping of the service in the event log.
			MarkThree.EventLog.Information("Matching Service stopped.");

		}

		/// <summary>
		/// Gets or sets an indication of whether the thread to handle Working Order changes is running.
		/// </summary>
		private static bool IsWorkingOrderQueueThreadRunning
		{
			get { lock (typeof(MatchingService)) return MatchingService.isWorkingOrderQueueThreadRunning; }
			set { lock (typeof(MatchingService)) MatchingService.isWorkingOrderQueueThreadRunning = value; }
		}

		/// <summary>
		/// Gets or sets an indication of whether the thread to handle Negotiation changes is running.
		/// </summary>
		private static bool IsNegotiationQueueThreadRunning
		{
			get { lock (typeof(MatchingService)) return MatchingService.isNegotiationQueueThreadRunning; }
			set { lock (typeof(MatchingService)) MatchingService.isNegotiationQueueThreadRunning = value; }
		}

		/// <summary>
		/// Handles a queue of actions to be taken for working orders.
		/// </summary>
		private static void WorkingOrderQueueThread()
		{

			// Pull actions off the queue and execute them until the thread is terminated.
			MatchingService.IsWorkingOrderQueueThreadRunning = true;
			while (MatchingService.IsWorkingOrderQueueThreadRunning)
				MatchingService.workingOrderQueue.Dequeue().DoAction();

		}

		/// <summary>
		/// Handles a queue of actions to be taken for working orders.
		/// </summary>
		private static void NegotiationQueueThread()
		{

			// Pull actions off the queue and execute them until the thread is terminated.
			MatchingService.IsNegotiationQueueThreadRunning = true;
			while (MatchingService.IsNegotiationQueueThreadRunning)
				MatchingService.negotiationQueue.Dequeue().DoAction();

		}

		/// <summary>
		/// Handles the creation of a working order.
		/// </summary>
		/// <param name="key">The unique identifier of the working order.</param>
		private static void InsertWorkingOrder(object[] parameter)
		{

			// Extract the elements of the Working Order key.
			int workingOrderId = (int)parameter[0];

			// The logic below will examine the order and see if a contra order is available for a match.  These values will 
			// indicate whether a match is possible after all the locks have been released.
			bool isMatched = false;
			int contraOrderId = int.MinValue;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);
				ServerMarketData.BlotterLock.AcquireReaderLock(Timeout.Infinite);
				ServerMarketData.MatchLock.AcquireReaderLock(Timeout.Infinite);
				ServerMarketData.ObjectLock.AcquireReaderLock(Timeout.Infinite);
				ServerMarketData.ObjectTreeLock.AcquireReaderLock(Timeout.Infinite);
				ServerMarketData.SecurityLock.AcquireReaderLock(Timeout.Infinite);
				ServerMarketData.WorkingOrderLock.AcquireReaderLock(Timeout.Infinite);

				// Locate the record that is the object of this action
				ServerMarketData.WorkingOrderRow workingOrderRow =
					ServerMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
				if (workingOrderRow == null)
					throw new Exception(string.Format("Working Order {0} has been deleted", workingOrderId));

				// The security data for this order is needed below.  The security provides the primarly list of orders that can be
				// on the opposite side of this order.
				ServerMarketData.SecurityRow securityRow = workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId;

				// Find the party to which this order belongs.
				int partyTypeCode = GetPartyCode(workingOrderRow.BlotterRow);
				if (partyTypeCode == PartyType.NotValid)
					return;

				// Reject any orders that are not submitted
				if (workingOrderRow.StatusCode != Status.Submitted)
					return;

				// Reject any orders that don't meet the minimums set for a given security.
				decimal submittedQuantity = workingOrderRow.SubmittedQuantity;
				foreach (ServerMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
					foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
						submittedQuantity -= executionRow.ExecutionQuantity;
				if (submittedQuantity < securityRow.MinimumQuantity)
					return;

				// Reject any working orders that are pending.
				if (IsActive(workingOrderRow))
					return;

				// If there is more than one possible match, the oldest one will go first.
				ServerMarketData.WorkingOrderRow oldestOrderRow = null;
				DateTime oldestSubmittedTime = DateTime.MinValue;

				// This section of code will search for a contra for the trade.  A contra order will satisfy several conditions, 
				// but the field of matching orders will always have the same security.  This loop will cycle through all the 
				// orders having the same security as order being examined.
				foreach (ServerMarketData.WorkingOrderRow contraOrderRow in
					securityRow.GetWorkingOrderRowsBySecurityWorkingOrderSecurityId())
				{

					// Test #1: Reject orders that are not on an opposite side
					if ((workingOrderRow.OrderTypeCode != OrderType.Buy || contraOrderRow.OrderTypeCode != OrderType.Sell) &&
						(workingOrderRow.OrderTypeCode != OrderType.Sell || contraOrderRow.OrderTypeCode != OrderType.Buy))
						continue;

					// Test #2: Reject any orders that are not submitted.
					if (contraOrderRow.StatusCode != Status.Submitted)
						continue;

					// Test #3: Reject any orders that don't meet the minimums set for a given security.
					decimal contraSubmittedQuantity = contraOrderRow.SubmittedQuantity;
					foreach (ServerMarketData.DestinationOrderRow destinationOrderRow in contraOrderRow.GetDestinationOrderRows())
						foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
							contraSubmittedQuantity -= executionRow.ExecutionQuantity;
					if (contraSubmittedQuantity < securityRow.MinimumQuantity)
						continue;

					// Test #4: Reject any parties that aren't valid for crossing.
					int counterPartyTypeCode = GetPartyCode(contraOrderRow.BlotterRow);
					if (counterPartyTypeCode == PartyType.NotValid)
						continue;

					// Test #5: Make sure this order matches the counter parties preference for a match.
					if ((!workingOrderRow.IsAgencyMatch || counterPartyTypeCode != PartyType.Agency) && 
						(!workingOrderRow.IsBrokerMatch || counterPartyTypeCode != PartyType.Broker) &&
						(!workingOrderRow.IsHedgeMatch || counterPartyTypeCode != PartyType.Hedge) &&
						(!workingOrderRow.IsInstitutionMatch || counterPartyTypeCode != PartyType.Instutition))
						continue;

					// Test #6: Make sure that the attrributes of this order match the counter party's preference for a match.
					if ((!contraOrderRow.IsAgencyMatch || partyTypeCode != PartyType.Agency) && 
						(!contraOrderRow.IsBrokerMatch || partyTypeCode != PartyType.Broker) &&
						(!contraOrderRow.IsHedgeMatch || partyTypeCode != PartyType.Hedge) &&
						(!contraOrderRow.IsInstitutionMatch || partyTypeCode != PartyType.Instutition))
						continue;

					// Test #7: Reject any orders pending other negotiations.
					if (IsActive(contraOrderRow))
						continue;

					// This will find the most recent attempt to match the contra order against this order.
					ServerMarketData.MatchRow youngestMatchRow = null;
					foreach (ServerMarketData.MatchRow contraMatchRow in contraOrderRow.GetMatchRows())
					{
						if (contraMatchRow.ContraOrderId == workingOrderRow.WorkingOrderId)
							if (youngestMatchRow == null || youngestMatchRow.CreatedTime < contraMatchRow.CreatedTime)
								youngestMatchRow = contraMatchRow;
					}

					// If there are no matches yet between these two orders, then use the time that the contra order was entered to
					// find the oldest of the orders.  Otherwise, the most recent attempt at a match determines which of the
					// possible contra orders will be selected.
					if (youngestMatchRow == null)
					{

						// Select this contra order if it is the oldest.
						if (oldestOrderRow == null || oldestSubmittedTime > contraOrderRow.SubmittedTime)
						{
							oldestSubmittedTime = contraOrderRow.SubmittedTime;
							oldestOrderRow = contraOrderRow;
						}

					}
					else
					{

						// If these two orders have attempted to match previously, then select the order that has the oldest
						// attempt at a match.
						if (youngestMatchRow.CreatedTime > oldestSubmittedTime)
						{
							oldestSubmittedTime = youngestMatchRow.CreatedTime;
							oldestOrderRow = contraOrderRow;
						}

					}

				}

				// After checking all the possible contra orders, select the one that has been dormant the longest for negotation.
				if (oldestOrderRow != null)
				{
					isMatched = true;
					contraOrderId = oldestOrderRow.WorkingOrderId;
				}

			}
			finally
			{

				if (ServerMarketData.BlotterLock.IsReaderLockHeld)
					ServerMarketData.BlotterLock.ReleaseReaderLock();
				if (ServerMarketData.MatchLock.IsReaderLockHeld)
					ServerMarketData.MatchLock.ReleaseReaderLock();
				if (ServerMarketData.ObjectLock.IsReaderLockHeld)
					ServerMarketData.ObjectLock.ReleaseReaderLock();
				if (ServerMarketData.ObjectTreeLock.IsReaderLockHeld)
					ServerMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ServerMarketData.SecurityLock.IsReaderLockHeld)
					ServerMarketData.SecurityLock.ReleaseReaderLock();
				if (ServerMarketData.WorkingOrderLock.IsReaderLockHeld)
					ServerMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ServerMarketData.IsLocked);

			}

			// Start a negotiation session if a match is found.
			if (isMatched)
				AddMatch(workingOrderId, contraOrderId);

		}

		/// <summary>
		/// Handles the creation of a working order.
		/// </summary>
		/// <param name="key">The unique identifier of the working order.</param>
		private static void InsertNegotiation(object[] parameter)
		{

			// Extract the elements of the Working Order key.
			int negotiationId = (int)parameter[0];

			// Create a transaction for adding the Match.
			Transaction transaction = new Transaction(Core.Match.PersistentStore);

			try
			{

				// These table must be locked for this operation.
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.DestinationOrderLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.ExecutionLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.NegotiationLock);
				transaction.AdoTransaction.LockRequests.AddReaderLock(ServerMarketData.PriceLock);
				transaction.AdoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
				transaction.AdoTransaction.LockRequests.AddReaderLock(ServerMarketData.TraderLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

				// Start the Transaction.
				transaction.Begin();

				// The SqlInfo contains information about the SQL transaction which must be passed on to the method during a
				// transaction.  There is only one ADO.NET store, but there can be one or more persistent stores.  In this case,
				// the ‘Core.Match’ class specifies where to store the data permanently.
				SqlInfo sqlInfo = transaction.SqlInfoList[Core.Match.PersistentStore];

				// These are shorthand notiations for the transactions that are used below.
				AdoTransaction adoTransaction = transaction.AdoTransaction;
				SqlTransaction sqlTransaction = sqlInfo.Transaction;

				// This will find the Negotiation record that has changed and the associated Match and Working Order records.  The 
				// main idea here is to examine the negotiation, see if there's a counter offer, and if there is, to examine the 
				// two sides of the negotiation.
				ServerMarketData.NegotiationRow negotiationRow = ServerMarketData.Negotiation.FindByNegotiationId(negotiationId);
				if (negotiationRow == null)
					throw new Exception(string.Format("Negotiation {0} has been deleted", negotiationId));
				ServerMarketData.MatchRow matchRow = negotiationRow.MatchRow;
				ServerMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;

				// This will find the records associated with the Counter offer.  Note that this isn't available through a
				// relation because the contra side of the trade is filtered from the client workstation.  For all intents and
				// purposes, the ContraOrderId is merely a token to the client workstation: it conveys no real information.  Also
				// note that the timestamp for each side of the match is identical and can be used as part of the index to find the
				// counter offer.
				int contraMatchIndex =
					ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId.Find(new object[] { matchRow.ContraOrderId,
						matchRow.WorkingOrderId, matchRow.CreatedTime });
				if (contraMatchIndex == -1)
					throw new Exception(string.Format("The match record for {0}, {1} at {2} can't be found",
						matchRow.ContraOrderId, matchRow.WorkingOrderId, matchRow.CreatedTime));
				ServerMarketData.MatchRow contraMatchRow =
					(ServerMarketData.MatchRow)ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId[contraMatchIndex].Row;
				ServerMarketData.WorkingOrderRow contraOrderRow = contraMatchRow.WorkingOrderRow;

				// When both sides have responded (or timed out) the two sides of the negotiation are resoved into a set of 
				// actions.  If both sides have accepted the chance to trade, then Destination Orders and Executions are
				// generated.  If either side has rejected the chance to trade, then this opportunity to match counterparties is
				// discarded.
				foreach (ServerMarketData.NegotiationRow contraNegotiationRow in contraMatchRow.GetNegotiationRows())
				{

					// These values are used when a Destination Order and Execution are created.
					DateTime currentTime = DateTime.Now;
					int createdUserId = ServerMarketData.UserId;
					DateTime createdTime = DateTime.Now;
					int modifiedUserId = ServerMarketData.UserId;
					DateTime modifiedTime = DateTime.Now;

					bool isWorkingOrderTired = false;
					bool isContraOrderTired = false;

					// Check to see if both sides have accepted the negotiation.
					if (negotiationRow.StatusCode == Status.Pending && contraNegotiationRow.StatusCode == Status.Pending)
					{

						// At this point, we've got a successful negotiation of a trade.  The quantity of this trade will be the 
						// minimum of the two offers and the price will be a simple calculation of the mid price.
						decimal matchedQuantity = negotiationRow.Quantity < contraNegotiationRow.Quantity ?
							negotiationRow.Quantity : contraNegotiationRow.Quantity;
						decimal matchedPrice = (workingOrderRow.PriceRow.BidPrice + workingOrderRow.PriceRow.AskPrice) / 2.0m;

						// Create a Destination Order on this side of the trade.
						long rowVersionDestionation = long.MinValue;
						int destinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction, sqlTransaction,
							ref rowVersionDestionation, null, null, createdTime, createdUserId, MatchingService.destinationId, null, null,
							null, workingOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
							workingOrderRow.OrderTypeCode, matchedQuantity, workingOrderRow.PriceTypeCode, State.Acknowledged, Status.New,
							workingOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId, workingOrderRow.TimeInForceCode,
							workingOrderRow.WorkingOrderId);

						// Create the Execution for this side of the trade.
						long rowVersionExecution = long.MinValue;
						int executionId = MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction,
							ref rowVersionExecution, null, null, null, null, createdTime, createdUserId, destinationOrderId, State.Acknowledged,
							matchedPrice, matchedQuantity, null, null, null, modifiedTime, modifiedUserId,
							null, null, null, DateTime.Now, null, State.Sent, DateTime.Now, null, null, null, null);

						// Update the negotiation to show that it was accepted and link it to the execution that was generated.
						long negotiationRowVersion = negotiationRow.RowVersion;
						MarkThree.Guardian.Core.Negotiation.Update(adoTransaction, sqlTransaction, ref negotiationRowVersion,
							executionId, null, negotiationRow.NegotiationId, null, Status.Accepted);

						// This will update the match to show that it was accepted.
						long matchRowVersion = matchRow.RowVersion;
						MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref matchRowVersion, null, null, 
							negotiationRow.MatchId, null, Status.Accepted, null);

						// Create a Destinatino Order for the other side of this trade.
						int contraDestinationOrderId = MarkThree.Guardian.Core.DestinationOrder.Insert(adoTransaction,
							sqlTransaction, ref rowVersionDestionation, null, null, createdTime, createdUserId,
							MatchingService.destinationId, null, null, null,
							contraOrderRow[ServerMarketData.WorkingOrder.LimitPriceColumn], modifiedTime, modifiedUserId,
							contraOrderRow.OrderTypeCode, matchedQuantity, contraOrderRow.PriceTypeCode, State.Acknowledged,
							Status.New, contraOrderRow[ServerMarketData.WorkingOrder.StopPriceColumn], createdUserId,
							contraOrderRow.TimeInForceCode, contraOrderRow.WorkingOrderId);

						// Create an Execution for the other side of the trade.
						int contraExecutionId = MarkThree.Guardian.Core.Execution.Insert(adoTransaction, sqlTransaction,
							ref rowVersionExecution, null, null, null, null, createdTime, createdUserId, contraDestinationOrderId,
							State.Acknowledged, matchedPrice, matchedQuantity, null, null, null, modifiedTime,
							modifiedUserId, null, null, null, DateTime.Now, null, State.Sent, DateTime.Now, null, null, null,
							null);

						// Update the contra Negotiation to reflect that it was accepted and link it to the execution.
						long contraNegotiationRowVersion = contraNegotiationRow.RowVersion;
						MarkThree.Guardian.Core.Negotiation.Update(adoTransaction, sqlTransaction, ref contraNegotiationRowVersion,
							contraExecutionId, null, contraNegotiationRow.NegotiationId, null, Status.Accepted);

						// This will update the contra match to show that it was accepted.
						long contraMatchRowVersion = contraMatchRow.RowVersion;
						MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref contraMatchRowVersion, null, null,
							contraNegotiationRow.MatchId, null, Status.Accepted, null);

						// The next step in handling a successful negotiation is to determine if the Working Order should be put to
						// sleep or remain in the box.  An order is put to sleep if the user negotiated a trade for less than the
						// leaves on the order.  Calculate the quantity left on this order to determine whether it should be taken
						// out of the box.
						decimal quantityLeaves = workingOrderRow.SubmittedQuantity;
						foreach (ServerMarketData.DestinationOrderRow destinationOrderRow in
							workingOrderRow.GetDestinationOrderRows())
							foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
								quantityLeaves -= executionRow.ExecutionQuantity;
						isWorkingOrderTired = negotiationRow.Quantity == matchedQuantity &&
							quantityLeaves >= workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.MinimumQuantity;

						// Calculate the contra left on this order to determine whether it should be taken out of the box.
						decimal contraLeaves = contraOrderRow.SubmittedQuantity;
						foreach (ServerMarketData.DestinationOrderRow destinationOrderRow in
							contraOrderRow.GetDestinationOrderRows())
							foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
								contraLeaves -= executionRow.ExecutionQuantity;
						isContraOrderTired = contraNegotiationRow.Quantity == matchedQuantity &&
							contraLeaves >= workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.MinimumQuantity;

					}
					else
					{

						// This will update the match to show that it was declined.
						long matchRowVersion = matchRow.RowVersion;
						MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref matchRowVersion, null, null, matchRow.MatchId, null,
							Status.Declined, null);

						// This will update the contra match to show that it was declined.
						long contraMatchRowVersion = contraMatchRow.RowVersion;
						MarkThree.Guardian.Core.Match.Update(adoTransaction, sqlTransaction, ref contraMatchRowVersion, null, null, contraMatchRow.MatchId, null,
							Status.Declined, null);

					}

					// This will put the order to sleep if the offer was declined or if it was accepted and the trader got
					// everything they were looking to get from this negotiation session.
					if (negotiationRow.StatusCode == Status.Declined || isWorkingOrderTired)
					{

						// The user who rejected the offer is put to sleep for the default time.  This section will initialize the
						// timer to wake the order up after an amount of time defined by the user.
						ServerMarketData.TraderRow traderRow = ServerMarketData.Trader.FindByTraderId(workingOrderRow.CreatedUserId);
						DateTime wakeupTime = currentTime + (traderRow == null ? TimeSpan.FromSeconds(MatchingService.marketSleep) :
							TimeSpan.FromSeconds(traderRow.MarketSleep));
						long workingWorkingOrderTimerRowVersion = workingOrderRow.TimerRow.RowVersion;
						MarkThree.Guardian.Core.Timer.Update(adoTransaction, sqlTransaction, ref workingWorkingOrderTimerRowVersion,
							null, null, true, wakeupTime, workingOrderRow.TimerId, null);

						// This will put the Working Order to sleep.
						long workingOrderRowVersion = workingOrderRow.RowVersion;
						MarkThree.Guardian.Core.WorkingOrder.Update(adoTransaction, sqlTransaction, ref workingOrderRowVersion, null, null, null, null,
							null, null, null, null, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
							null, null, null, null, null, null, workingOrderRow.WorkingOrderId);

					}

					// This will put the contra order to sleep if the counter offer was declined or if it was accepted and the 
					// contra trader got everything they were looking for from this negotiation session.
					if (contraNegotiationRow.StatusCode == Status.Declined || isContraOrderTired)
					{

						// The user who rejected the offer is put to sleep for the default time.  This section will initialize the timer to
						// wake the order up after a set amount of time.
						ServerMarketData.TraderRow traderRow = ServerMarketData.Trader.FindByTraderId(contraOrderRow.CreatedUserId);
						DateTime wakeupTime = currentTime + (traderRow == null ? TimeSpan.FromSeconds(MatchingService.marketSleep) :
							TimeSpan.FromSeconds(traderRow.MarketSleep));
						long contraWorkingOrderTimerRowVersion = contraOrderRow.TimerRow.RowVersion;
						MarkThree.Guardian.Core.Timer.Update(adoTransaction, sqlTransaction, ref contraWorkingOrderTimerRowVersion,
							null, null, true, wakeupTime, contraOrderRow.TimerId, null);

						// This will put the Contra Working Order to sleep.
						long contraOrderRowVersion = contraOrderRow.RowVersion;
						MarkThree.Guardian.Core.WorkingOrder.Update(adoTransaction, sqlTransaction, ref contraOrderRowVersion, null, null, null, null, null, null,
							null, null, false, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
							null, null, null, null, null, contraOrderRow.WorkingOrderId);

					}

					// Turn off the timer if it is still counting.  Note that the timer is shared by both parties so it only need 
					// to be shut down once.
					if (matchRow.TimerRow.IsActive)
					{
						long timerRowVersion = matchRow.TimerRow.RowVersion;
						MarkThree.Guardian.Core.Timer.Update(adoTransaction, sqlTransaction, ref timerRowVersion, null, null,
							null, currentTime, matchRow.TimerId, null);
					}

				}

				// These two methods can now be committed to the ADO.NET and SQL data stores.  They will be added as a unit or
				// rolled back as a unit.
				transaction.Commit();

			}
			catch (Exception exception)
			{

				// Log any errors.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

				// Any errors will cause the transaction to be rolled back.
				transaction.Rollback();

			}
			finally
			{

				// Whether successful or not, this will close out the resources for the transaction and end it.
				transaction.EndTransaction();

			}

		}


		private static int GetPartyCode(ServerMarketData.BlotterRow blotterRow)
		{

			if (blotterRow.PartyTypeCode != PartyType.UseParent && blotterRow.PartyTypeCode != PartyType.NotValid)
				return blotterRow.PartyTypeCode;

			if (blotterRow.PartyTypeCode == PartyType.UseParent)
				foreach (ServerMarketData.ObjectTreeRow objectTreeRow in blotterRow.ObjectRow.GetObjectTreeRowsByObjectObjectTreeChildId())
				{

					ServerMarketData.ObjectRow parentRow = objectTreeRow.ObjectRowByObjectObjectTreeParentId;
					foreach (ServerMarketData.BlotterRow parentBlotterRow in parentRow.GetBlotterRows())
					{

						int partyTypeCode = GetPartyCode(parentBlotterRow);
						if (partyTypeCode != PartyType.NotValid)
							return partyTypeCode;

					}

				}

			return PartyType.NotValid;

		}

		private static bool IsActive(ServerMarketData.WorkingOrderRow workingOrderRow)
		{

			foreach (ServerMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
				if (matchRow.StatusCode == Status.Active)
					return true;

			return false;

		}

		/// <summary>
		/// Add a match of two orders to begin the negociation process.
		/// </summary>
		/// <param name="workingOrderId">The WorkingOrderId of the first order.</param>
		/// <param name="contraOrderId">The WorkingOrderId of the second order.</param>
		private static void AddMatch(int workingOrderId, int contraOrderId)
		{

			long rowVersion = long.MinValue;

			// Create a transaction for adding the Match.
			Transaction transaction = new Transaction(Core.Match.PersistentStore);

			try
			{

				// These tables are needed for the transaction.
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);

				// Start the Transaction.
				transaction.Begin();

				// The SqlInfo contains information about the SQL transaction which must be passed on to the method during a
				// transaction.  There is only one ADO.NET store, but there can be one or more persistent stores.  In this case, 
				// the ‘Core.Match’ class specifies where to store the data permanently.
				SqlInfo sqlInfo = transaction.SqlInfoList[Core.Match.PersistentStore];

				DateTime startTime = DateTime.Now;
				DateTime currentTime = DateTime.Now;

				// Create a timer for this match.
				long timerRowVersion = 0L;
				int timerId = Core.Timer.Insert(transaction.AdoTransaction, sqlInfo.Transaction, ref timerRowVersion, DateTime.Now,
					null, true, DateTime.Now + MatchingService.negotiationTime, 0);

				// Add the match of the primary order to the contra order.
				Core.Match.Insert(transaction.AdoTransaction, sqlInfo.Transaction, ref rowVersion, workingOrderId, currentTime, timerId, Status.Active,
					contraOrderId);

				// Conversely, add the match of the contra order to the primary.
				Core.Match.Insert(transaction.AdoTransaction, sqlInfo.Transaction, ref rowVersion, contraOrderId, currentTime, timerId, Status.Active,
					workingOrderId);

				// These two methods can now be committed to the ADO.NET and SQL data stores.  They will be added as a unit or 
				// rolled back as a unit.
				transaction.Commit();

			}
			catch (Exception exception)
			{

				// Log any errors.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

				// Any errors will cause the transaction to be rolled back.
				transaction.Rollback();

			}
			finally
			{

				// Whether successful or not, this will close out the resources for the transaction and end it.
				transaction.EndTransaction();

			}

		}

		/// <summary>
		/// Add a match of two orders to begin the negociation process.
		/// </summary>
		/// <param name="workingOrderId">The WorkingOrderId of the first order.</param>
		/// <param name="contraOrderId">The WorkingOrderId of the second order.</param>
		public static void CloseMatch(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ServerMarketData.MatchRow matchRow)
		{

			if (matchRow.GetNegotiationRows().Length == 0)
			{
				long rowVersion = matchRow.RowVersion;
				Core.Negotiation.Insert(adoTransaction, sqlTransaction, ref rowVersion, null, matchRow.MatchId, 0.0m, Status.Declined);
			}

		}

		/// <summary>
		/// Add a match of two orders to begin the negociation process.
		/// </summary>
		/// <param name="workingOrderId">The WorkingOrderId of the first order.</param>
		/// <param name="contraOrderId">The WorkingOrderId of the second order.</param>
		private static void WakeWorkingOrder(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ServerMarketData.WorkingOrderRow workingOrderRow)
		{

			// Add the match of the primary order to the contra order.
			long rowVersion = workingOrderRow.RowVersion;
			Core.WorkingOrder.Update(adoTransaction, sqlTransaction, ref rowVersion, null, null, null, null, null, null,
				null, null, true, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
				null, null, null, workingOrderRow.WorkingOrderId);

		}

		private static void Negotiation_NegotiationRowChanging(object sender, MarkThree.Guardian.DataSetMarket.NegotiationRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit)
			{

				// Only new negotiations are handled here.
				if (!e.Row.HasVersion(DataRowVersion.Original))
					MatchingService.workingOrderQueue.Enqueue(new Action(new ActionHandler(InsertNegotiation), e.Row.NegotiationId));

			}

		}

		private static void WorkingOrder_WorkingOrderRowChanging(object sender, MarkThree.Guardian.DataSetMarket.WorkingOrderRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit)
			{

				if (!e.Row.HasVersion(DataRowVersion.Original))
					MatchingService.workingOrderQueue.Enqueue(new Action(new ActionHandler(InsertWorkingOrder), e.Row.WorkingOrderId));
				else
				{

					int previousStatus = (int)e.Row[ServerMarketData.WorkingOrder.StatusCodeColumn, DataRowVersion.Original];
					int currentStatus = (int)e.Row[ServerMarketData.WorkingOrder.StatusCodeColumn, DataRowVersion.Current];

					decimal previousQuantity = (decimal)e.Row[ServerMarketData.WorkingOrder.SubmittedQuantityColumn, DataRowVersion.Original];
					decimal currentQuantity = (decimal)e.Row[ServerMarketData.WorkingOrder.SubmittedQuantityColumn, DataRowVersion.Current];

					bool previousIsAgencyMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsAgencyMatchColumn, DataRowVersion.Original];
					bool currentIsAgencyMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsAgencyMatchColumn, DataRowVersion.Current];

					bool previousIsBrokerMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsBrokerMatchColumn, DataRowVersion.Original];
					bool currentIsBrokerMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsBrokerMatchColumn, DataRowVersion.Current];

					bool previousIsHedgeMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsHedgeMatchColumn, DataRowVersion.Original];
					bool currentIsHedgeMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsHedgeMatchColumn, DataRowVersion.Current];

					bool previousIsInstitutionMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsInstitutionMatchColumn, DataRowVersion.Original];
					bool currentIsInstitutionMatch = (bool)e.Row[ServerMarketData.WorkingOrder.IsInstitutionMatchColumn, DataRowVersion.Current];

					if ((previousStatus != Status.Submitted && currentStatus == Status.Submitted) ||
						(currentStatus == Status.Submitted &&
							(previousQuantity != currentQuantity || previousIsAgencyMatch != currentIsAgencyMatch ||
							previousIsBrokerMatch != currentIsBrokerMatch ||
							previousIsInstitutionMatch != currentIsInstitutionMatch ||
							previousIsHedgeMatch != currentIsHedgeMatch)))
						MatchingService.workingOrderQueue.Enqueue(new Action(new ActionHandler(InsertWorkingOrder), e.Row.WorkingOrderId));

					// If this order is being removed from the box, and it was matched prevously against another order, then wake 
					// that order up so it can look for another contra.
					if (previousStatus == Status.Submitted && currentStatus != Status.Submitted)
					{
						ServerMarketData.MatchRow[] matches = e.Row.GetMatchRows();
						if (matches.Length != 0)
						{
							ServerMarketData.MatchRow matchRow = matches[matches.Length - 1];
							int contraMatchIndex =
								ServerMarketData.Match.KeyMatchWorkingOrderIdContraOrderId.Find(new object[] { matchRow.ContraOrderId, matchRow.WorkingOrderId, matchRow.CreatedTime });
							if (contraMatchIndex == -1)
								throw new Exception(string.Format("The match record for {0}, {1} at {2} can't be found", matchRow.ContraOrderId, matchRow.WorkingOrderId, matchRow.CreatedTime));

							MatchingService.workingOrderQueue.Enqueue(new Action(new ActionHandler(InsertWorkingOrder), matchRow.ContraOrderId));

						}
					}

				}

			}

		}

		private static void TimerCallback(object startData)
		{

			DateTime currentTime = DateTime.Now;

			// Create a transaction for adding the Match.
			Transaction transaction = new Transaction(Core.Match.PersistentStore);

			try
			{

				// These tables are needed for the transaction.
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.NegotiationLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.TimerLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

				// Start the Transaction.
				transaction.Begin();

				// The SqlInfo contains information about the SQL transaction which must be passed on to the method during a
				// transaction.  There is only one ADO.NET store, but there can be one or more persistent stores.  In this case, 
				// the ‘Core.Match’ class specifies where to store the data permanently.
				SqlInfo sqlInfo = transaction.SqlInfoList[Core.Match.PersistentStore];

				foreach (DataRowView dataRowView in MatchingService.timerView)
				{

					ServerMarketData.TimerRow timerRow = (ServerMarketData.TimerRow)dataRowView.Row;

					long rowVersion = timerRow.RowVersion;
					bool isActive = true;
					DateTime stopTime = timerRow.StopTime;
					if (currentTime >= stopTime)
					{
						isActive = false;
						stopTime = currentTime;
						foreach (ServerMarketData.MatchRow matchRow in timerRow.GetMatchRows())
							CloseMatch(transaction.AdoTransaction, sqlInfo.Transaction, matchRow);
						foreach (ServerMarketData.WorkingOrderRow workingOrderRow in timerRow.GetWorkingOrderRows())
							WakeWorkingOrder(transaction.AdoTransaction, sqlInfo.Transaction, workingOrderRow);
					}

					Core.Timer.Update(transaction.AdoTransaction, sqlInfo.Transaction, ref rowVersion, currentTime,
						null, isActive, stopTime, timerRow.TimerId, null);

				}

				// These two methods can now be committed to the ADO.NET and SQL data stores.  They will be added as a unit or 
				// rolled back as a unit.
				transaction.Commit();

			}
			catch (Exception exception)
			{

				// Write the event to the log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				// Any errors will cause the transaction to be rolled back.
				transaction.Rollback();

			}
			finally
			{

				// Whether successful or not, this will close out the resources for the transaction and end it.
				transaction.EndTransaction();

			}

		}

	}

}
