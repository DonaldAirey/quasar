/*************************************************************************************************************************
*
*	File:			ClientTimeouts.cs
*	Description:	This module contains constants for controlling the heuristics of the data model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Client
{

	/// <summary>
	/// Used for tuning the performance of the Data Model background thread.
	/// </summary>
	public class ClientTimeout
	{

		/// <summary>The amout of time to wait for a lock.</summary>
		public const int LockWait = -1;
		/// <summary>The amount of time to wait between refreshes.</summary>
		public const int RefreshInterval = 2000;

	};

}
