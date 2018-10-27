/*************************************************************************************************************************
*
*	File:			RowType.cs
*	Description:	Enumerations used to identify rows in an Excel Spreadsheet.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.BlockOrder
{

	/// <summary>
	/// Enumeration for the row types in the block order viewer.
	/// </summary>
	internal class RowType
	{

		/// <summary>Unused Row</summary>
		public const int Unused = 0;
		/// <summary>A Position Detail line</summary>
		public const int BlockOrder = 1;
		/// <summary>A Placeholder for new orders</summary>
		public const int Placeholder = 2;

	};

}
