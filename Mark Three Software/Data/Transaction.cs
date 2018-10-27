namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Transactions;
	using System.Threading;

	/// <summary>
	/// A collection of operations on one or more data stores that is committed or rejected as a unit.
	/// </summary>
	public class Transaction : IDisposable
	{

		// Private Static Members
		private static Dictionary<int, Stack<Transaction>> threadMappings;

		// Private Members
		private System.Transactions.CommittableTransaction committableTransaction;
		private System.Collections.Generic.Dictionary<string, ResourceManager> resourceTable;

		/// <summary>
		/// Creates the static elements of a transaction.
		/// </summary>
		static Transaction()
		{

			// Provides a mapping to the ambient transaction.  An operation may need to add a record to the transaction from
			// anywhere in the calling stack.  To remove the requirement that the transaction is passed from every calling method
			// to the caller, the ambient property allows for the transaction to be found using the current thread identifier.
			Transaction.threadMappings = new Dictionary<int, Stack<Transaction>>();

		}

		/// <summary>
		/// Creates a transaction.
		/// </summary>
		public Transaction()
		{

			// This class is intended to be a thin wrapper around the Framework class.  The transactions have an unlimited 
			// lifetime, but that will likely change as the deadlocking scenarios are tested.
			this.committableTransaction = new CommittableTransaction(TimeSpan.Zero);

			// Every transaction can have one or more resources (which is another name for a data store).  Typically there will be
			// one ADO data model in memory and one or more SQL data stores which serve as the non-volatile repository for the
			// data.  The resources are referenced by name once a transaction is started.  A method that implements an operation in
			// the transaction needs to only know the names of the resources, not the details of the connection to them.
			this.resourceTable = new Dictionary<string, ResourceManager>();

			// Every thread has a stack of transactions that allows for nested transactions like their Framework counterparts.  If
			// a transaction is already running in this thread, then that transaction is used.  Otherwise a new stack for the
			// thread is created.
			Stack<Transaction> transactionStack = null;
			if (!Transaction.threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out transactionStack))
			{
				transactionStack = new Stack<Transaction>();
				Transaction.threadMappings.Add(Thread.CurrentThread.ManagedThreadId, transactionStack);
			}
			transactionStack.Push(this);

		}

		/// <summary>
		/// Gets the ambient transaction for this thread.
		/// </summary>
		public static Transaction Current
		{

			get
			{

				// This uses the current thread identifier to find the transaction in the static properties.  Only one thread stack
				// is available per thread and only one transaction at a time can be the ambient transaction.  This is designed to
				// work the same way as the 'TransactionScope' in the Framework.
				Stack<Transaction> transactionStack = new Stack<Transaction>();
				if (!Transaction.threadMappings.TryGetValue(Thread.CurrentThread.ManagedThreadId, out transactionStack))
					return null;
				return transactionStack.Peek();

			}

		}

		/// <summary>
		/// Adds an ADO Resource Manager to the transaction.
		/// </summary>
		/// <param name="adoResourceManager">Manages an ADO data store in a transaction.</param>
		/// <returns>The resource manager that was added to the transaction.</returns>
		public AdoResourceManager Add(AdoResourceManager adoResourceManager)
		{

			// Adds the resource manager to the transaction.
			this.resourceTable.Add(adoResourceManager.Name, adoResourceManager);

			// Enlists the resource manager as part of the transaction.
			this.EnlistVolatile(adoResourceManager, EnlistmentOptions.None);

			// This is provided as a notiational convinience.
			return adoResourceManager;

		}

		/// <summary>
		/// Adds an SQL Resource Manager to the transaction.
		/// </summary>
		/// <param name="sqlResourceManager">Manages an SQL data store in a transaction.</param>
		/// <returns>THe resource manager that was added to the transaction.</returns>
		public SqlResourceManager Add(SqlResourceManager sqlResourceManager)
		{

			// Adds the resource manager to the transaction.
			this.resourceTable.Add(sqlResourceManager.Name, sqlResourceManager);

			// The SQL Server provides its own methods for enlisting in the transaction.
			sqlResourceManager.SqlConnection.EnlistTransaction(this.committableTransaction);

			// This is provided as a notiational convinience.
			return sqlResourceManager;

		}

		/// <summary>
		/// Enlists a volatile resource manager that supports single phase commit optimization to participate in a transaction.
		/// </summary>
		/// <param name="enlistmentNotification">Describes an interface that a resource manager should implement to two phase
		/// commit notifications callbacks for the transaction manager upon enlisting for participation.</param>
		/// <param name="enlistmentOptions">Detemines whether the object should be enlisted during the prepare phase.</param>
		public void EnlistVolatile(IEnlistmentNotification enlistmentNotification, EnlistmentOptions enlistmentOptions)
		{

			// Enlist the volatile resource manager in this transaction for the two phase commit callback notifications.
			this.committableTransaction.EnlistVolatile(enlistmentNotification, enlistmentOptions);

		}

		/// <summary>
		/// Enlist a durable resource manager that supports single phase commit optimization to participate in a transaction.
		/// </summary>
		/// <param name="resourceManagerIdentifier">A global resource identifier for the durable resource.</param>
		/// <param name="enlistmentNotification">Describes an interface that a resource manager should implement to two phase
		/// commit notifications callbacks for the transaction manager upon enlisting for participation.</param>
		/// <param name="enlistmentOptions">Detemines whether the object should be enlisted during the prepare phase.</param>
		public void EnlistDurable(Guid resourceManagerIdentifier, IEnlistmentNotification enlistmentNotification, EnlistmentOptions enlistmentOptions)
		{

			// Enlist the volatile resource manager in this transaction for the two phase commit callback notifications.
			this.committableTransaction.EnlistDurable(resourceManagerIdentifier, enlistmentNotification, enlistmentOptions);

		}

		/// <summary>
		/// Attempts to commit the transaction.
		/// </summary>
		public void Commit() { this.committableTransaction.Commit(); }

		/// <summary>
		/// Rolls back (aborts) the transaction.
		/// </summary>
		public void Rollback() { this.committableTransaction.Rollback(); }

		/// <summary>
		/// Finds a resource manager by name.
		/// </summary>
		/// <param name="index">The name of the resource manager.</param>
		/// <returns>The resource manager matching the name.</returns>
		public ResourceManager this[string index] { get { return this.resourceTable[index]; } set { this.resourceTable[index] = value; } }

		#region IDisposable Members

		/// <summary>
		/// Destroys the resources allocated for this transaction.
		/// </summary>
		public void Dispose()
		{

			// If the transaction is still active when it is disposed then any pending transaction is aborted.
			if (this.committableTransaction.TransactionInformation.Status == TransactionStatus.Active)
				this.committableTransaction.Rollback();

			// Dispose of each of the resource managers used for this transaction.
			foreach (ResourceManager resourceManager in this.resourceTable.Values)
				resourceManager.Dispose();

			// This will pop the ambient transaction stack off the stack mappings and restore the previous ambient transaction. If
			// there are no ambient transactions remaining on the stack, then all transaction mappings to this thread are removed
			// from the static table that manages the connection between the thread identifier and the transaction stack.
			Stack<Transaction> transactionStack = Transaction.threadMappings[Thread.CurrentThread.ManagedThreadId];
			transactionStack.Pop();
			if (transactionStack.Count == 0)
				Transaction.threadMappings.Remove(Thread.CurrentThread.ManagedThreadId);

			// The base transaction needs to be able to clean also.
			this.committableTransaction.Dispose();

		}

		#endregion

	}

}
