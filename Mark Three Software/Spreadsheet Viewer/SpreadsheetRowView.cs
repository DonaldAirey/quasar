namespace MarkThree.Forms
{

	using System;
	using System.Data;

	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetRowView : DataView
	{

		public SpreadsheetRowView(Spreadsheet spreadsheet) : base(spreadsheet) { }

		public new SpreadsheetRow this[int recordIndex] { get { return (SpreadsheetRow)base[recordIndex].Row; } }

		public new SpreadsheetRow Find(object key)
		{

			int recordIndex = base.Find(key);
			if (recordIndex == -1)
				return null;

			return (SpreadsheetRow)base[recordIndex].Row;

		}

		public new SpreadsheetRow Find(object[] keys)
		{

			int recordIndex = base.Find(keys);
			if (recordIndex == -1)
				return null;

			return (SpreadsheetRow)base[recordIndex].Row;

		}

	}

}
