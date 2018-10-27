namespace MarkThree.Guardian.Client
{

	using System;
	using System.Threading;

	/// <summary>
	/// Used for tuning the performance of the Data Model background thread.
	/// </summary>
	public class ClientTimeout
	{

		/// <summary>The amout of time to wait for a lock.</summary>
		public const int LockWait = Timeout.Infinite;
		/// <summary>The amount of time to wait between refreshes.</summary>
		public const int RefreshInterval = 1000;
		/// <summary>The amount of time to wait for a thread to finish before aborting it.</summary>
		public const int EndOfThreadWait = 5000;

	};

}
