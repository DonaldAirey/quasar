/*************************************************************************************************************************
*
*	File:			RowTypes.cs
*	Description:	Enumerations for rows in a placement document.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Placement
{

	/// <summary>
	/// Enumeration for the row types in the placement screen.
	/// </summary>
	internal class RowType
	{

		/// <summary>Unused Row</summary>
		public const int Unused = 0;
		/// <summary>An Placement Detail Line.</summary>
		public const int Placement = 1;
		/// <summary>A placeholder for the prompting text.</summary>
		public const int Placeholder = 2;

	};

}
