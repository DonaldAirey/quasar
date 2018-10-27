namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Data;

	[Serializable]
	public class Result
	{

		// Public Readonly Members
		public readonly Transaction[] Transactions;

		public Result(Batch batch)
		{

			// Copy the exceptions and methods from the original batch into the results.
			List<Transaction> transactionList = new List<Transaction>();
			foreach (Batch.Transaction batchTransaction in batch.Transactions)
			{
				Transaction resultTransaction = new Transaction(batchTransaction);
				if (resultTransaction.Exceptions.Length != 0 || resultTransaction.Methods.Length != 0)
					transactionList.Add(resultTransaction);
			}
			this.Transactions = transactionList.ToArray();

		}

		[Serializable]
		public class Transaction
		{

			// Public Readonly Members
			public readonly int Index;
			public readonly Method[] Methods;
			public readonly Exception[] Exceptions;

			public Transaction(Batch.Transaction batchTransaction)
			{

				// Initialize the object
				this.Index = batchTransaction.Index;
				this.Exceptions = batchTransaction.Exceptions.ToArray();

				// Copy the framework of the method out of the original structure and place it in the results.  Note that unless
				// there was an exception or some return values, the method doesn't need to be passed back to the caller.
				List<Method> methodList = new List<Method>();
				foreach (Batch.Method batchMethod in batchTransaction.Methods)
				{
					Result.Method resultMethod = new Result.Method(batchMethod);
					if (resultMethod.Exceptions.Length != 0 || resultMethod.Results != null)
						methodList.Add(resultMethod);
				}
				this.Methods = methodList.ToArray();

			}

		}

		[Serializable]
		public class Method
		{

			// Public Readonly Members
			public readonly int Index;
			public readonly object[] Results;
			public readonly Exception[] Exceptions;

			public Method(Batch.Method batchMethod)
			{

				// Initialize the object
				this.Index = batchMethod.Index;
				this.Results = batchMethod.Results;
				this.Exceptions = batchMethod.Exceptions.ToArray();

			}
			
		}

	}

}
