namespace MarkThree
{

	using System;
	using System.Threading;

	/// <summary>
	/// Used for tuning the performance of the Data Model background thread.
	/// </summary>
	public class CommonTimeout
	{

		/// <summary>The amout of time to wait for a lock.</summary>
#if DEBUG		
		public const int LockWait = Timeout.Infinite;
		public const int TickAnimation = 1000;
#else
		public const int LockWait = Timeout.Infinite;
		public const int TickAnimation = 1000;
#endif

	};

}
