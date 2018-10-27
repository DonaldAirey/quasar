namespace MarkThree.Guardian
{

	using System;

	/// <summary>
	/// SubmissionType Types
	/// </summary>
	public class SubmissionType
	{

		/// <summary>None</summary>
		public const int None = 0x00000000;
		/// <summary>Buy</summary>
		public const int AlwaysMatch = 0x00000001;
		/// <summary>Sell</summary>
		public const int UsePeferences = 0x00000002;
		/// <summary>Buy Cover</summary>
		public const int NeverMatch = 0x00000004;
		/// <summary>Away</summary>
		public const int Away = 0x00000008;
		/// <summary>Route to Destination</summary>
		public const int RouteToDestination = 0x00000010;

	};

}
