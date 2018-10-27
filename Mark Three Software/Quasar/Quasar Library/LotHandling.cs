/*************************************************************************************************************************
*
*	File:			LotHandling.cs
*	Description:	Time-in-ForceCodes.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
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
