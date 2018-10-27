namespace MarkThree
{

	using System;
	using System.Data;
	using System.Text;

	/// <summary>
	/// A base class for all rows of data that need to be committed or rejected in a transaction.
	/// </summary>
	public abstract class TransactionRow : DataRow
	{

		// Private Members
		private Table table;

		/// <summary>
		/// Creates a row that can be managed by the Ado Resource Manager during transactions.
		/// </summary>
		/// <param name="dataRowBuilder"></param>
		public TransactionRow(System.Data.DataRowBuilder dataRowBuilder) : base(dataRowBuilder) { }

		/// <summary>
		/// Commits all the changes that were made to this row since the last time MarkThree.TransactionRow.AcceptChanges() was
		/// called.
		/// </summary>
		public new virtual void AcceptChanges() { base.AcceptChanges(); }

		/// <summary>
		/// Rejects all changes made to this row since the last time MarkThree.TransactionRow.RejectChanges() was called.
		/// </summary>
		public new virtual void RejectChanges() { base.RejectChanges(); }

		/// <summary>
		/// The table to which this row is a member.
		/// </summary>
		public new virtual Table Table { get { return this.table; } set { this.table = value; } }

	}

}
