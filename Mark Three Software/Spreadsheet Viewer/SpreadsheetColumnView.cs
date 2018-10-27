namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetColumnView : IEnumerable
	{

		private MarkThree.Forms.Spreadsheet spreadsheet;
		private System.Collections.Generic.List<SpreadsheetColumn> list;
		private System.Collections.Generic.Dictionary<string, SpreadsheetColumn> dictionary;
		private System.String columnFilter;

		public SpreadsheetColumnView(Spreadsheet spreadsheet)
		{

			// Initialize the object.
			this.spreadsheet = spreadsheet;
			this.list = new List<SpreadsheetColumn>();
			this.dictionary = new Dictionary<string, SpreadsheetColumn>();
			this.columnFilter = string.Empty;

		}

		public int Count { get { return list.Count; } }

		public void Clear()
		{

			this.list.Clear();
			this.dictionary.Clear();

		}

		public string ColumnFilter
		{

			get { return this.ColumnFilter; }
			set
			{

				int columnIndex = 0;

				// Columns that do not appear in the view will have an invalid index.
				foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.Columns)
					spreadsheetColumn.ColumnViewIndex = int.MinValue;

				foreach (string columnName in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					DataColumn dataColumn = this.spreadsheet.BaseColumns[columnName.Trim()];
					if (dataColumn != null)
					{
						SpreadsheetColumn spreadsheetColumn = (SpreadsheetColumn)dataColumn;
						Add(spreadsheetColumn);
						spreadsheetColumn.ColumnViewIndex = columnIndex++;
					}

				}

			}

		}

		public void Add(SpreadsheetColumn spreadsheetColumn)
		{

			this.list.Add(spreadsheetColumn);
			this.dictionary.Add(spreadsheetColumn.ColumnName, spreadsheetColumn);

		}

		public void Remove(SpreadsheetColumn spreadsheetColumn)
		{

			this.list.Remove(spreadsheetColumn);
			this.dictionary.Remove(spreadsheetColumn.ColumnName);

		}

		public void Move(int sourceIndex, int destinationIndex)
		{

			SpreadsheetColumn spreadsheetColumn = this.list[sourceIndex];
			this.list.RemoveAt(sourceIndex);
			this.list.Insert(sourceIndex < destinationIndex ? destinationIndex - 1 : destinationIndex, spreadsheetColumn);

		}

		public int IndexOf(SpreadsheetColumn spreadsheetColumn)
		{

			return this.list.IndexOf(spreadsheetColumn);

		}

		public SpreadsheetColumn this[string columnName]
		{

			get { return this.dictionary[columnName]; }

		}

		public SpreadsheetColumn this[int columnIndex]
		{

			get { return this.list[columnIndex]; }

		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{

			return this.list.GetEnumerator();

		}

		#endregion
	}

}
