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
	using System.Threading;

	/// <summary>A Match simulation of an auction market.</summary>
	/// <copyright>Copyright (C) 1998-2005 Mark Three Software -- All Rights Reserved.</copyright>
	public class BusinessRules : System.ComponentModel.Component
	{

		// Private Members
		private static bool isStarted;
		private static bool isBusinessRulesThreadRunning;
		private static System.ComponentModel.IContainer components;
		private static MarkThree.WaitQueue businessRuleQueue;
		private static Thread businessRulesThread;

		/// This member provides access to the in-memory database.
		private static ServerMarketData serverMarketData = new ServerMarketData();

		private delegate void ObjectHandler(object[] key, params object[] parameters);

		private struct ObjectAction
		{

			// Public Members
			public ObjectHandler DoAction;
			public object[] Key;
			public object[] Parameters;

			public ObjectAction(ObjectHandler doAction, object[] key, params object[] parameters)
			{

				// Initialize the object
				this.DoAction = doAction;
				this.Key = key;
				this.Parameters = parameters;

			}

		}

		/// <summary>
		/// This object will simulate an auction market on equities.
		/// </summary>
		static BusinessRules()
		{

			// Install the Server Market Data in this container.
			BusinessRules.components = new System.ComponentModel.Container();

			// This queue is filled up with Business Ruless that need to be serviced because something changed the matching 
			// criteria.
			BusinessRules.businessRuleQueue = new WaitQueue();

			BusinessRules.isBusinessRulesThreadRunning = false;

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Start(AdoTransaction adoTransaction)
		{

			// These tables must be locked to start the Matching Service.
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.DestinationOrderLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ExecutionLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);
		
		}

		/// <summary>
		/// Starts a simulated price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Start(ParameterList parameters)
		{

			if (BusinessRules.isStarted)
				throw new Exception("Business Rules are already running.");

			ServerMarketData.DestinationOrder.DestinationOrderRowChanging += new MarkThree.Guardian.DataSetMarket.DestinationOrderRowChangeEventHandler(DestinationOrder_DestinationOrderRowChanging);
			ServerMarketData.Execution.ExecutionRowChanging += new MarkThree.Guardian.DataSetMarket.ExecutionRowChangeEventHandler(Execution_ExecutionRowChanging);
			ServerMarketData.Execution.ExecutionRowDeleted += new MarkThree.Guardian.DataSetMarket.ExecutionRowChangeEventHandler(Execution_ExecutionRowDeleted);
			ServerMarketData.WorkingOrder.WorkingOrderRowChanging += new MarkThree.Guardian.DataSetMarket.WorkingOrderRowChangeEventHandler(WorkingOrder_WorkingOrderRowChanging);
			
			// This thread will pull the working order actions off the queue and handle them.
			BusinessRules.businessRulesThread = new Thread(new ThreadStart(BusinessRulesThread));
			BusinessRules.businessRulesThread.Name = "Business Rules Thread";
			BusinessRules.businessRulesThread.Start();

			BusinessRules.isStarted = true;

			MarkThree.EventLog.Information("Business Rules are active.");

		}

		/// <summary>
		/// Initializes a transaction.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		public static void Stop(AdoTransaction adoTransaction)
		{

			// These tables must be locked to start the Matching Service.
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.DestinationOrderLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ExecutionLock);
			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

		}
		
		/// <summary>
		/// Terminates the simulation of a price feed.
		/// </summary>
		/// <param name="transaction">Used to commit or reject one or more operations as a unit.</param>
		/// <param name="remoteMethod">A metadata description of the method to be executed.</param>
		public static void Stop(ParameterList parameters)
		{

			if (!BusinessRules.isStarted)
				throw new Exception("Business Rules are not activated.");

			// Terminate the Queue Handling Thread.  Give it 100 ms to end gracefully before terminating the thread the hard way.
			BusinessRules.IsBusinessRulesThreadRunning = false;
			if (!BusinessRules.businessRulesThread.Join(100))
				BusinessRules.businessRulesThread.Abort();
			
			BusinessRules.isStarted = false;

			MarkThree.EventLog.Information("Business Rules have been halted.");

		}

		private static bool IsBusinessRulesThreadRunning
		{
			get {lock (typeof(BusinessRules)) return BusinessRules.isBusinessRulesThreadRunning;}
			set {lock (typeof(BusinessRules)) BusinessRules.isBusinessRulesThreadRunning = value;}
		}

		private static void BusinessRulesThread()
		{

			try
			{

				BusinessRules.IsBusinessRulesThreadRunning = true;

				while (BusinessRules.IsBusinessRulesThreadRunning)
				{

					ObjectAction objectAction = (ObjectAction)BusinessRules.businessRuleQueue.Dequeue();
					objectAction.DoAction(objectAction.Key, objectAction.Parameters);

				}

			}
			catch (Exception exception)
			{

				MarkThree.EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}

		}

		/// <summary>
		/// Add a match of two orders to begin the negociation process.
		/// </summary>
		/// <param name="workingOrderId">The DestinationOrderId of the first order.</param>
		/// <param name="contraOrderId">The DestinationOrderId of the second order.</param>
		private static void SetDestinationOrderStatus(object[] key, params object[] parameters)
		{

			int destinationOrderId = (int)key[0];
			object stateCode = parameters[0];
			object statusCode = parameters[1];

			// Create a transaction for adding the Match.
			Transaction transaction = new Transaction(Core.DestinationOrder.PersistentStore);

			try
			{

				// These tables are needed for the transaction.
				transaction.AdoTransaction.LockRequests.AddReaderLock(ServerMarketData.WorkingOrderLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.ExecutionLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.DestinationOrderLock);

				// Start the Transaction.
				transaction.Begin();

				// The SqlInfo contains information about the SQL transaction which must be passed on to the method during a
				// transaction.  There is only one ADO.NET store, but there can be one or more persistent stores.  In this case, 
				// the ‘Core.Match’ class specifies where to store the data permanently.
				SqlInfo sqlInfo = transaction.SqlInfoList[Core.DestinationOrder.PersistentStore];

				// Add the match of the primary order to the contra order.
				ServerMarketData.DestinationOrderRow destinationOrderRow = ServerMarketData.DestinationOrder.FindByDestinationOrderId(destinationOrderId);
				if (destinationOrderRow != null)
				{
					long rowVersion = destinationOrderRow.RowVersion;
					Core.DestinationOrder.Update(transaction.AdoTransaction, sqlInfo.Transaction, ref rowVersion, null, null, null, null, null,
						destinationOrderId, null, null, null, null, null, null, null, null, null, stateCode, statusCode, null,
						null, null, null);
				}

				// These two methods can now be committed to the ADO.NET and SQL data stores.  They will be added as a unit or 
				// rolled back as a unit.
				transaction.Commit();

			}
			catch (Exception exception)
			{

				// Log the error.
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
		private static void SetWorkingOrderStatus(object[] key, params object[] parameters)
		{

			int workingOrderId = (int)key[0];
			int statusCode = (int)parameters[0];

			// Create a transaction for adding the Match.
			Transaction transaction = new Transaction(Core.WorkingOrder.PersistentStore);

			try
			{

				// These tables are needed for the transaction.
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.DestinationOrderLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.ExecutionLock);
				transaction.AdoTransaction.LockRequests.AddReaderLock(ServerMarketData.MatchLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.SourceOrderLock);
				transaction.AdoTransaction.LockRequests.AddWriterLock(ServerMarketData.WorkingOrderLock);

				// Start the Transaction.
				transaction.Begin();

				// The SqlInfo contains information about the SQL transaction which must be passed on to the method during a
				// transaction.  There is only one ADO.NET store, but there can be one or more persistent stores.  In this case, 
				// the ‘Core.Match’ class specifies where to store the data permanently.
				SqlInfo sqlInfo = transaction.SqlInfoList[Core.WorkingOrder.PersistentStore];

				// Add the match of the primary order to the contra order.
				ServerMarketData.WorkingOrderRow workingOrderRow = ServerMarketData.WorkingOrder.FindByWorkingOrderId(workingOrderId);
				if (workingOrderRow != null)
				{
					long rowVersion = workingOrderRow.RowVersion;
					object submittedTime = statusCode == Status.Submitted ? (object)DateTime.Now : (object)DBNull.Value;
					Core.WorkingOrder.Update(transaction.AdoTransaction, sqlInfo.Transaction, ref rowVersion, null, null, null, null,
						null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, statusCode,
						null, null, null, null, submittedTime, null, null, null, workingOrderId);
				}

				// These two methods can now be committed to the ADO.NET and SQL data stores.  They will be added as a unit or 
				// rolled back as a unit.
				transaction.Commit();

			}
			catch (Exception exception)
			{

				// Log the error.
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
		
		private static void Execution_ExecutionRowChanging(object sender, MarkThree.Guardian.DataSetMarket.ExecutionRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit)
			{

				int destinationOrderId = (int)e.Row[ServerMarketData.Execution.DestinationOrderIdColumn,
					e.Row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
				ServerMarketData.DestinationOrderRow destinationOrderRow = ServerMarketData.DestinationOrder.FindByDestinationOrderId(destinationOrderId);
				if (destinationOrderRow == null)
					return;

				decimal orderQuantity = destinationOrderRow.OrderedQuantity - destinationOrderRow.CanceledQuantity;

				decimal executionQuantity = 0.0m;
				foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
					executionQuantity += executionRow.ExecutionQuantity;

				if (executionQuantity > orderQuantity && destinationOrderRow.StatusCode != Status.Error)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
						new object[] {destinationOrderId}, null, Status.Error));

				if (executionQuantity < orderQuantity && executionQuantity != 0.0m && destinationOrderRow.StatusCode != Status.PartiallyFilled)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
						new object[] {destinationOrderId}, null, Status.PartiallyFilled));

				if (executionQuantity == orderQuantity && executionQuantity != 0.0m && destinationOrderRow.StatusCode != Status.Filled)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
						new object[] {destinationOrderId}, null, Status.Filled));

				if (executionQuantity == 0.0m && destinationOrderRow.StatusCode != Status.New)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
						new object[] {destinationOrderId}, null, Status.New));

			}

		}

		private static void Execution_ExecutionRowDeleted(object sender, MarkThree.Guardian.DataSetMarket.ExecutionRowChangeEvent e)
		{

			int destinationOrderId = (int)e.Row[ServerMarketData.Execution.DestinationOrderIdColumn,
				e.Row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current];
			ServerMarketData.DestinationOrderRow destinationOrderRow = ServerMarketData.DestinationOrder.FindByDestinationOrderId(destinationOrderId);
			if (destinationOrderRow == null)
				return;

			decimal orderQuantity = destinationOrderRow.OrderedQuantity - destinationOrderRow.CanceledQuantity;

			decimal executionQuantity = 0.0m;
			foreach (ServerMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
				executionQuantity += executionRow.ExecutionQuantity;

			if (executionQuantity < orderQuantity && executionQuantity != 0.0m && destinationOrderRow.StatusCode != Status.PartiallyFilled)
				BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
					new object[] {destinationOrderId}, null, Status.PartiallyFilled));

			if (executionQuantity == 0.0m && destinationOrderRow.StatusCode != Status.New)
				BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetDestinationOrderStatus),
					new object[] {destinationOrderId}, null, Status.New));

		}

		private static void DestinationOrder_DestinationOrderRowChanging(object sender, MarkThree.Guardian.DataSetMarket.DestinationOrderRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit)
			{

				ServerMarketData.DestinationOrderRow destinationOrderRow = e.Row;

				int currentStatusCode = destinationOrderRow.StatusCode;
				int previousStatusCode = destinationOrderRow.HasVersion(DataRowVersion.Original) ?
					(int)destinationOrderRow[ServerMarketData.DestinationOrder.StatusCodeColumn, DataRowVersion.Original] :
					destinationOrderRow.StatusCode;

				if (currentStatusCode != previousStatusCode)
				{

					ServerMarketData.WorkingOrderRow workingOrderRow = e.Row.WorkingOrderRow;

					if (destinationOrderRow.StatusCode == Status.Error)
						BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
							new object[] {workingOrderRow.WorkingOrderId}, Status.Error));

					if ((workingOrderRow.StatusCode == Status.New || workingOrderRow.StatusCode == Status.Filled) &&
						destinationOrderRow.StatusCode == Status.PartiallyFilled)
						BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
							new object[] {workingOrderRow.WorkingOrderId}, Status.PartiallyFilled));

					if ((workingOrderRow.StatusCode == Status.New || workingOrderRow.StatusCode == Status.PartiallyFilled) &&
						destinationOrderRow.StatusCode == Status.Filled)
					{

						bool isFilled = true;
						foreach (ServerMarketData.DestinationOrderRow innerDestinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							if (innerDestinationOrderRow.StatusCode != Status.Filled)
							{
								isFilled = false;
								break;
							}

						if (isFilled)
							BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
								new object[] {workingOrderRow.WorkingOrderId}, Status.Filled));

					}

					if (previousStatusCode != Status.New && currentStatusCode == Status.New)
					{

						bool isNew = true;
						foreach (ServerMarketData.DestinationOrderRow innerDestinationOrderRow in workingOrderRow.GetDestinationOrderRows())
							if (innerDestinationOrderRow.StatusCode != Status.New)
							{
								isNew = false;
								break;
							}

						if (isNew)
							BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
								new object[] {workingOrderRow.WorkingOrderId}, Status.New));

					}

					if (previousStatusCode == Status.Error && workingOrderRow.StatusCode == Status.Error)
					{

						int statusCode = Status.New;

						foreach (ServerMarketData.DestinationOrderRow innerDestinationOrderRow in workingOrderRow.GetDestinationOrderRows())
						{

							if (innerDestinationOrderRow.StatusCode == Status.Error)
							{
								statusCode = Status.Error;
								break;
							}

							if (innerDestinationOrderRow.StatusCode == Status.PartiallyFilled && (statusCode == Status.New || statusCode == Status.Filled))
								statusCode = Status.PartiallyFilled;

							if (innerDestinationOrderRow.StatusCode == Status.Filled && statusCode == Status.New)
								statusCode = Status.Filled;

						}

						if (statusCode != Status.Error)
							BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
								new object[] {workingOrderRow.WorkingOrderId}, statusCode));

					}

				}

			}
			
		}

		private static void WorkingOrder_WorkingOrderRowChanging(object sender, MarkThree.Guardian.DataSetMarket.WorkingOrderRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit)
			{

				bool isSubmitted = false;

				if ((e.Row.SubmissionTypeCode & SubmissionType.Away) == SubmissionType.None)
				{

					switch (e.Row.SubmissionTypeCode & ~SubmissionType.Away)
					{

					case SubmissionType.AlwaysMatch:

						isSubmitted = true;
						break;

					case SubmissionType.UsePeferences:

						isSubmitted = e.Row.IsAwake;

						if (!e.Row.IsStartTimeNull() && DateTime.Now.TimeOfDay < e.Row.StartTime.TimeOfDay)
							isSubmitted = false;

						if (!e.Row.IsStopTimeNull() && DateTime.Now.TimeOfDay > e.Row.StopTime.TimeOfDay)
							isSubmitted = false;

						break;

					}

				}

				if (isSubmitted && e.Row.StatusCode != Status.Submitted)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
						new object[] {e.Row.WorkingOrderId}, Status.Submitted));

				if (!isSubmitted && e.Row.StatusCode == Status.Submitted)
					BusinessRules.businessRuleQueue.Enqueue(new ObjectAction(new ObjectHandler(SetWorkingOrderStatus),
						new object[] {e.Row.WorkingOrderId}, Status.New));

			}

		}

	}

}
