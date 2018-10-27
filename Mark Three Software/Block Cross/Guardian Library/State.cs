namespace MarkThree.Guardian
{

	using System;

	/// <summary>
	/// States are internal values for the condition of a process.
	/// </summary>
	public class State 
	{

		/// <summary>This object hasn't been altered since it was created.</summary>
		public const int Initial = 0;

		/// <summary>This object been sent to some destination.</summary>
		public const int Sent = 1;
		
		/// <summary>This object was received by the destination.</summary>
		public const int Acknowledged = 2;

		/// <summary>This object was rejected by the destination.</summary>
		public const int Rejected = 3;

		/// <summary>This object has been canceled, but no confirmation yet from the destination.</summary>
		public const int CancelPending = 4;

		/// <summary>The destination has confirmed the cancel instruction but not yet canceled the object.</summary>
		public const int CancelAcknowledged = 5;

		/// <summary>This object could not be canceled by the destination.</summary>
		public const int CancelRejected = 6;

		/// <summary>This object canceled by the destination.</summary>
		public const int Cancelled = 7;

		/// <summary>This object has requested a replacement with the destination.</summary>
		public const int ReplacePending = 8;

		/// <summary>The destination has received the request to change the object.</summary>
		public const int ReplaceAcknowledged = 9;

		/// <summary>The destination has rejected the request to change the object.</summary>
		public const int ReplaceRejected = 10;

		/// <summary>The object has been replaced an earlier object.</summary>
		public const int Replaced = 11;

		/// <summary>No more activity on this object is expected today.</summary>
		public const int DoneForDay = 12;

		/// <summary>The object has beeen manually stopped from being processed.</summary>
		public const int Stopped = 13;

		/// <summary>The object has some error that prevents any further activity.</summary>
		public const int Error = 14;

	}

}
