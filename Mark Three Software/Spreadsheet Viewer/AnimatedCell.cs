namespace MarkThree.Forms
{

	using System;
	using System.Data;

	/// <summary>Tracks the animation in a cell.</summary>
	public class AnimatedCell : IComparable
	{

		public MarkThree.Forms.SpreadsheetColumn SpreadsheetColumn;
		public MarkThree.Forms.SpreadsheetRow SpreadsheetRow;
		public MarkThree.Forms.Style[] AnimationArray;
		public System.Int32 AnimationIndex;

		public AnimatedCell(SpreadsheetColumn spreadsheetColumn, SpreadsheetRow spreadsheetRow, Style[] animationArray)
		{

			// Initialize the object. Note that the first color in the sequence was set when the change in value was discovered, so
			// the sequencing actually begins with the second style in the sequence of colors.
			this.AnimationIndex = 1;
			this.AnimationArray = animationArray;
			this.SpreadsheetColumn = spreadsheetColumn;
			this.SpreadsheetRow = spreadsheetRow;

		}

		#region IComparable Members

		/// <summary>
		/// Compares an AnimatedCell address to another object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{

			// Compare this against another AnimatedCell.
			if (obj is AnimatedCell)
			{

				AnimatedCell operand = (AnimatedCell)obj;

				// The rows are compared by testing the primary key elements of each row.
				foreach (DataColumn dataColumn in this.SpreadsheetRow.Table.PrimaryKey)
				{
					int comparison = this.SpreadsheetRow[dataColumn].CompareTo(operand.SpreadsheetRow[dataColumn]);
					if (comparison != 0)
						return comparison;
				}

				// If the two rows are the same, compare the column.
				return this.SpreadsheetColumn.Ordinal.CompareTo(operand.SpreadsheetColumn.Ordinal);

			}

			// Compare this object against the column and row addresses.
			if (obj is object[])
			{

				// Extract the row and column from the arguments.
				SpreadsheetColumn spreadsheetColumn = (SpreadsheetColumn)((object[])obj)[0];
				SpreadsheetRow spreadsheetRow = (SpreadsheetRow)((object[])obj)[1];

				// The rows are compared by testing the primary key elements of each row.
				foreach (DataColumn dataColumn in this.SpreadsheetRow.Table.PrimaryKey)
				{
					int comparison = this.SpreadsheetRow[dataColumn].CompareTo(spreadsheetRow[dataColumn]);
					if (comparison != 0)
						return comparison;
				}

				// If the two rows are the same, compare the column.
				return this.SpreadsheetColumn.Ordinal.CompareTo(spreadsheetColumn.Ordinal);

			}

			// Any other comparison isn't supported.
			throw new Exception(string.Format("Can't compare {0} to type {1}", obj.GetType().ToString(),
				this.GetType().ToString()));

		}

		#endregion

	}

}
