/*************************************************************************************************************************
*
*	File:			SpreadsheetColumn.cs
*	Description:	Used to parse the columns from spreadsheet references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Text.RegularExpressions;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used for parsing Spreadsheet Column references.
	/// </summary>
	public class SpreadsheetColumn
	{

		public const int DoesNotExist = -1;

		/// <summary>
		/// Extracts a column from a spreadsheet reference.
		/// </summary>
		/// <param name="reference">An Sheet!RC Style expression.</param>
		/// <returns>The value of the column in the reference.</returns>
		public static int Parse(string reference)
		{

			// This regular expression will extract the column value from a spreadsheet reference.
			Match match = Regex.Match(reference, @".*[^0-9]C(?<column>\d+)");
			if (match.Success)
				return Convert.ToInt32(match.Groups["column"].Value);

			// This is the error return if the text can't be parsed.
			return SpreadsheetColumn.DoesNotExist;

		}

	}

}
