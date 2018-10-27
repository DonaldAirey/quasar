namespace Shadows.Quasar.Viewers.Appraisal
{

	using System;
	using System.Data;

	/// <summary>
	/// Summary description for PositionTable.
	/// </summary>
	public class PositionTable : DataTable, System.Collections.IEnumerable
	{

		DataColumn columnAccountId;
		DataColumn columnSecurityId;
		DataColumn columnPositionTypeCode;

		public DataColumn AccountIdColumn {get {return this.columnAccountId;}}
		public DataColumn SecurityIdColumn {get {return this.columnSecurityId;}}
		public DataColumn PositionTypeCodeColumn {get {return this.columnPositionTypeCode;}}

		public PositionTable()
		{
			this.columnAccountId = this.Columns.Add("AccountId", typeof(int));
			this.columnSecurityId = this.Columns.Add("SecurityId", typeof(int));
			this.columnPositionTypeCode = this.Columns.Add("PositionTypeCode", typeof(int));
			this.PrimaryKey = new DataColumn[] {this.columnAccountId, this.columnSecurityId, this.columnPositionTypeCode};
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.Rows.GetEnumerator();
		}
            
		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new PositionRow(builder);
		}
            
		public PositionRow AddPosition(int accountId, int securityId, int positionTypeCode)
		{

			DataRow dataRow = this.Rows.Find(new object[] {accountId, securityId, positionTypeCode});
			if (dataRow != null)
				return (PositionRow)dataRow;

			PositionRow positionRow = ((PositionRow)this.NewRow());
			positionRow[this.columnAccountId] = accountId;
			positionRow[this.columnSecurityId] = securityId;
			positionRow[this.columnPositionTypeCode] = positionTypeCode;
			this.Rows.Add(positionRow);
			return positionRow;

		}

	}

	public class PositionRow : DataRow
	{

		private PositionTable tablePosition;

		internal PositionRow(DataRowBuilder rb) : base(rb)
		{
			this.tablePosition = ((PositionTable)(this.Table));
		}
              
		public int AccountId
		{
			get
			{
				return ((int)(this[this.tablePosition.AccountIdColumn]));
			}
			set
			{
				this[this.tablePosition.AccountIdColumn] = value;
			}
		}
            
		public int SecurityId
		{
			get
			{
				return ((int)(this[this.tablePosition.SecurityIdColumn]));
			}
			set
			{
				this[this.tablePosition.SecurityIdColumn] = value;
			}
		}
            
		public int PositionTypeCode
		{
			get
			{
				return ((int)(this[this.tablePosition.PositionTypeCodeColumn]));
			}
			set
			{
				this[this.tablePosition.PositionTypeCodeColumn] = value;
			}
		}
            
	}

}
