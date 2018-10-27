/*************************************************************************************************************************
*
*	File:			Position.cs
*	Description:	Position Types
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
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
