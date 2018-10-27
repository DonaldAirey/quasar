namespace MarkThree
{

	using System;

	/// <summary>
	/// Used to pass parameters to a Batch Handler.
	/// </summary>
	public class BatchExceptionEventArgs : EventArgs
	{

		// Public Members
		public BatchException BatchException;

		public BatchExceptionEventArgs(BatchException batchException)
		{

			// Initialize the object
			this.BatchException = batchException;

		}

	}

}
