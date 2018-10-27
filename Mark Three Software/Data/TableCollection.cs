namespace MarkThree
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	/// <summary>
	/// Represents a collection of tables.
	/// </summary>
	public class TableCollection
	{

		// Private Members
		private DataTableCollection dataTableCollection;

		/// <summary>
		/// Creates a collection of tables.
		/// </summary>
		/// <param name="dataTableCollection">The System.Data.DataTableCollection that provides the data for this collection.</param>
		public TableCollection(DataTableCollection dataTableCollection) { this.dataTableCollection = dataTableCollection; }

		/// <summary>
		/// Gets a table in a collection of tables.
		/// </summary>
		/// <param name="index">The index into the collection of tables.</param>
		/// <returns>The table with the name.</returns>
		public Table this[int index] { get { return ((Table)(this.dataTableCollection[index])); } }

		/// <summary>
		/// Gets a table in a collection of tables.
		/// </summary>
		/// <param name="index">The index into the collection of tables.</param>
		/// <returns>The table with the name.</returns>
		public Table this[string name] { get { return ((Table)(this.dataTableCollection[name])); } }

		/// <summary>
		/// Adds a table to the collection.
		/// </summary>
		/// <param name="table">The name of the table.</param>
		public void Add(Table table) { this.dataTableCollection.Add(table); }

		/// <summary>
		/// Gets the number of MarkThree.Tables in the collection.
		/// </summary>
		public int Count { get { return this.dataTableCollection.Count; } }

		/// <summary>
		/// Gets a System.Collections.IEnumerator for the collection.
		/// </summary>
		/// <returns>An enumerator that allows iteration through the collection.</returns>
		public IEnumerator GetEnumerator() { return this.dataTableCollection.GetEnumerator(); }

	}

}
