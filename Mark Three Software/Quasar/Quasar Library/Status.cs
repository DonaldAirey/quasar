/*************************************************************************************************************************
*
*	File:			Status.cs
*	Description:	StatusCodes
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// General purpose statusCodes for object.
	/// </summary>
	public class Status
	{

		/// <summary>The object hasn't been changed.</summary>
		public const int None = -1;
		/// <summary>The object is new and hasn't be touched.</summary>
		public const int New = 0;
		/// <summary>The object thas been touched by the trader.</summary>
		public const int Open = 1;
		/// <summary>The object has been closed and is ready for archiving.</summary>
		public const int Closed = 2;
		/// <summary>The object is waiting to be sent.</summary>
		public const int Pending = 3;
		/// <summary>The object has been confirmed by the target.</summary>
		public const int Confirmed = 4;

	};
	
}
