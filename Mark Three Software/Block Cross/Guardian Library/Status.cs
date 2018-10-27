namespace MarkThree.Guardian
{

	using System;

	/// <summary>
	/// General purpose statusCodes for object.
	/// </summary>
	public class Status
	{

		/// <summary>The object hasn't been changed.</summary>
		public const int None = -1;
		/// <summary>The object is new and hasn't be touched.</summary>
		public const int New = 0;
		/// <summary>The object thas been touched by the trader.</summary>
		public const int Open = 1;
		/// <summary>The object has been closed and is ready for archiving.</summary>
		public const int Closed = 2;
		/// <summary>The object is waiting to be sent.</summary>
		public const int Pending = 3;
		/// <summary>The object has been confirmed by the target.</summary>
		public const int Confirmed = 4;
		/// <summary>The object has been rejected by a counter party.</summary>
		public const int Declined = 5;
		/// <summary>An order has been completely filled.</summary>
		public const int Filled = 6;
		/// <summary>An order has been some executions but is not completed yet.</summary>
		public const int PartiallyFilled = 7;
		/// <summary>Some error has occurred.</summary>
		public const int Error = 8;
		/// <summary>Submitted into the crossing network.</summary>
		public const int Submitted = 9;
		/// <summary>This order is negotiating a trade.</summary>
		public const int Negotiating = 10;
		/// <summary>This negotiation is successful.</summary>
		public const int Accepted = 11;
		/// <summary>This match is active.</summary>
		public const int Active = 12;
		/// <summary>This match has expired.</summary>
		public const int Expired = 13;

	};
	
}
