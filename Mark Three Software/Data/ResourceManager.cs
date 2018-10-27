namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Transactions;

	/// <summary>
	/// Used to manage the resources of a data store during a transaction.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public abstract class ResourceManager : IEnlistmentNotification, IDisposable
	{

		// Public Members
		public readonly string Name;

		/// <summary>
		/// Creates a resource manager.
		/// </summary>
		/// <param name="name">The name of the resource manager.</param>
		public ResourceManager(string name)
		{

			// Initialize the object.
			this.Name = name;

		}

		#region IEnlistmentNotification Members

		/// <summary>
		/// Commit a transaction.
		/// </summary>
		/// <param name="enlistment">Facilitates communication bewtween an enlisted transaction participant and the transaction
		/// manager during the final phase of the transaction.</param>
		public virtual void Commit(Enlistment enlistment) { }

		/// <summary>
		/// Called when the state of a transaction is in doubt because the second phase of a two phase commit is in progress.
		/// </summary>
		/// <param name="enlistment">Facilitates communication bewtween an enlisted transaction participant and the transaction
		/// manager during the final phase of the transaction.</param>
		public virtual void InDoubt(Enlistment enlistment) { }

		/// <summary>
		/// Prepares a transaction for commitment.
		/// </summary>
		/// <param name="prepareEnlistment">Facilitates communication bewtween an enlisted transaction participant and the
		/// transaction manager during the final phase of the transaction.</param>
		public virtual void Prepare(PreparingEnlistment preparingEnlistment)
		{

			// This will signal the base transaction object that no special preparation required.  A class can override this
			// behavior if required, but since the preparation, commit and reject logic are done on different threads than the
			// thread where all the work to access the data occurs, any thread based locking is prohibited in the preparation phase
			// of the commit.
			preparingEnlistment.Prepared();

		}
		
		/// <summary>
		/// Restores the ADO data model to the state before the transaction began.
		/// </summary>
		/// <param name="enlistment">Facilitates communication bewtween an enlisted transaction participant and the transaction
		/// manager during the final phase of the transaction.</param>
		public virtual void Rollback(Enlistment enlistment) { }

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes of the resources allocated by this resource manager.
		/// </summary>
		public virtual void Dispose() { }

		#endregion

	}

}
