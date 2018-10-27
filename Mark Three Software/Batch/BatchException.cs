namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for BatchException.
	/// </summary>
	public class BatchException : Exception
	{

		// Private Members
		private ArrayList exceptionCollection;

		// Public Accessors
		public ArrayList Exceptions {get {return this.exceptionCollection;}}

		public BatchException(Exception exception)
		{

			// Simply add the exception to the collection.
			this.exceptionCollection = new ArrayList();
			this.exceptionCollection.Add(exception);

		}

		public BatchException(Batch batch)
		{

			// The exceptions from the batch will be collected into a single, easy to use list.
			this.exceptionCollection = new ArrayList();

			// Intialize a list with all the exceptions in the batch.
			foreach (Batch.Transaction transaction in batch.Transactions)
			{

				foreach (Exception exception in transaction.Exceptions)
					this.exceptionCollection.Add(exception);

				foreach (Batch.Method method in transaction.Methods)
					foreach (Exception exception in method.Exceptions)
						this.exceptionCollection.Add(exception);

			}

		}

	}

}
