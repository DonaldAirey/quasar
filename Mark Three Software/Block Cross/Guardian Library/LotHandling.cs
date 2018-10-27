using System;

namespace MarkThree.Guardian
{

	/// <summary>
	/// Defines the duration of the order
	/// </summary>
	public class LotHandling
	{
		/// <summary>Last In First Out</summary>
		public const int LIFO = 0;
		/// <summary>First In First Out</summary>
		public const int FIFO = 1;
		/// <summary>Minimize Tax Impact</summary>
		public const int MINTAX = 2;
	}
	
}
