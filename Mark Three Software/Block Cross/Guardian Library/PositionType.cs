using System;

namespace MarkThree.Guardian
{

	/// <summary>
	/// Indicates whether a held position is long (owned) or borrowed (short)
	/// </summary>
	public class PositionType
	{
		/// <summary>This position is owned (Long).</summary>
		public const int Long = 0;
		/// <summary>This position is borrowed (Short).</summary>
		public const int Short = 1;
	};
	
}
