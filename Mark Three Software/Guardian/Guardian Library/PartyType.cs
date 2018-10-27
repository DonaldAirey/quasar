namespace MarkThree.Guardian
{

	using System;

	/// <summary>
	/// Indicates whether a held position is long (owned) or borrowed (short)
	/// </summary>
	public class PartyType
	{
		public const int Agency = 0;
		public const int Broker = 1;
		public const int Hedge = 2;
		public const int Instutition = 3;
		public const int UseParent = 4;
		public const int NotValid = 5;
	};
	
}
