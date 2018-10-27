/*************************************************************************************************************************
*
*	File:			Condition.cs
*	Description:	Conditions for an Order.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Defines how the order is to be priced.
	/// </summary>
	public class Condition
	{

		/// <summary>Execute all the order or none of it.</summary>
		public const int AllOrNone = 0;
		/// <summary>Do Not Reduce</summary>
		public const int DoNotReduce = 1;
		/// <summary>All or None and Do Not Reduce</summary>
		public const int AllOrNoneDoNotReduce = 2;

	}

}
