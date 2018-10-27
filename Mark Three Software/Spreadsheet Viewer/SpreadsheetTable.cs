namespace MarkThree.Forms
{

	using System;
	using System.Data;
	
	/// <summary>
	/// Summary description for SpreadsheetTable.
	/// </summary>
	public class SpreadsheetTable : DataTable
	{
		public SpreadsheetTable(string tableName) : base(tableName) {}

		public SpreadsheetRow NewSpreadsheetRow()
		{

			return (SpreadsheetRow)this.NewRow();

		}

		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new SpreadsheetRow(builder);
		}

	}

}
