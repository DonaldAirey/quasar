namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TableSchemaCollection
	{

		private SortedList<string, TableSchema> tableList;

		public TableSchemaCollection()
		{

			// Initialize the object
			this.tableList = new SortedList<string, TableSchema>();

		}

		public void Add(TableSchema tableSchema)
		{

			this.tableList.Add(tableSchema.Name, tableSchema);
		
		}

		public int Count { get { return this.tableList.Count; } }

		public bool Contains(string name) { return this.tableList.ContainsKey(name); }

		public void Remove(string name) { this.tableList.Remove(name); }

		public TableSchema Find(string name)
		{

			TableSchema tableSchema;
			if (this.tableList.TryGetValue(name, out tableSchema))
				return tableSchema;

			return null;

		}

		public IEnumerator<TableSchema> GetEnumerator() { return tableList.Values.GetEnumerator(); }

	}

}
