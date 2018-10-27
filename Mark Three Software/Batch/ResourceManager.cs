namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Transactions;

	public class AdoResourceManager : IEnlistmentNotification
	{

		private List<Row> rows;
		private List<ExecutionPlan> executionPlans;
		private System.Collections.Generic.List<LockRequest> lockRequests;

		public AdoResourceManager()
		{

			this.rows = new List<Row>();
			this.executionPlans = new List<ExecutionPlan>();
			this.lockRequests = new List<LockRequest>();

			System.Transactions.Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);

		}

		public void AcquireLocks()
		{

			foreach (LockRequest lockRequest in this.lockRequests)
				lockRequest.AcquireLock();

		}

		public void ReleaseLocks()
		{

			foreach (LockRequest lockRequest in this.lockRequests)
				lockRequest.ReleaseLock();

		}

		public void Add(ExecutionPlan executionPlan)
		{

			this.executionPlans.Add(executionPlan);

		}

		public void Add(Row row)
		{

			this.rows.Add(row);

		}

		public void Add(LockRequest lockRequest)
		{

			this.lockRequests.Add(lockRequest);

		}

		public void DoWork()
		{

			foreach (ExecutionPlan executionPlan in this.executionPlans)
				executionPlan.VerbHandler(this, executionPlan.Row, executionPlan.Arguments);

		}

		#region IEnlistmentNotification Members

		public void Commit(Enlistment enlistment)
		{

			foreach (Row row in this.rows)
				row.AcceptChanges();

			ReleaseLocks();

		}

		public void InDoubt(Enlistment enlistment)
		{
			Console.WriteLine("In Doubt");
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Rollback(Enlistment enlistment)
		{

			foreach (Row row in this.rows)
				row.RejectChanges();

			ReleaseLocks();

		}

		#endregion

	}
}
