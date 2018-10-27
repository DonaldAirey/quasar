/*************************************************************************************************************************
*
*	File:			SpreadsheetRow.Cs
*	Description:	Used to parse the columns from spreadsheet references.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Text.RegularExpressions;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used for parsing Spreadsheet Row references.
	/// </summary>
	public class SpreadsheetRow
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
			Match match = Regex.Match(reference, @".*[^0-9]R(?<row>\d+)");
			if (match.Success)
				return Convert.ToInt32(match.Groups["row"].Value);

			// This is the error return if the text can't be parsed.
			return SpreadsheetRow.DoesNotExist;

		}

	}

}
