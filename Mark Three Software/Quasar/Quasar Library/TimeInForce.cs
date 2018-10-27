/*************************************************************************************************************************
*
*	File:			TimeInForce.cs
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
	public class TimeInForce
	{
		/// <summary>Day Order - Good from now until the market closes</summary>
		public const int DAY = 0;
		/// <summary>Good 'til Canceled</summary>
		public const int GTC = 1;
		/// <summary>Fill or Kill</summary>
		public const int FOK = 2;
		/// <summary>Immediate or Cancel</summary>
		public const int IOC = 3;
		/// <summary>Execute on Market Opening</summary>
		public const int OPG = 4;
		/// <summary>Execute on Market Close</summary>
		public const int CLO = 5;
	}
	
}
