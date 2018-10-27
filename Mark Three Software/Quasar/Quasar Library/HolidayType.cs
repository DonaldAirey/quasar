/*************************************************************************************************************************
*
*	File:			HolidayType.cs
*	Description:	Defines the activities that are prohibited on holiday.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Identifies the different kinds of holiday.
	/// </summary>
	public class HolidayType
	{

		/// <summary>Both Trading and Settlement are prohibited.</summary>
		public const int Both = 0;
		/// <summary>Trading is prohibited.</summary>
		public const int Trading = 1;
		/// <summary>Settlement is prohibited.</summary>
		public const int Settlement = 2;

	};

}
