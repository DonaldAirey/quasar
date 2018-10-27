/*************************************************************************************************************************
*
*	File:			CommonTimeouts.cs
*	Description:	This module contains constants for controlling the heuristics of the data model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Threading;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used for tuning the performance of the Data Model background thread.
	/// </summary>
	public class CommonTimeout
	{

		/// <summary>The amout of time to wait for a lock.</summary>
#if DEBUG		
		public const int LockWait = Timeout.Infinite;
		public const int TickAnimation = 750;
#else
		public const int LockWait = Timeout.Infinite;
		public const int TickAnimation = 750;
#endif

	};

}
