namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class ColumnSchemaCollection
	{

		private SortedList<string, ColumnSchema> columnList;

		public ColumnSchemaCollection()
		{

			// Initialize the object
			this.columnList = new SortedList<string, ColumnSchema>();

		}

		public void Add(ColumnSchema columnSchema)
		{

			this.columnList.Add(columnSchema.Name, columnSchema);
		
		}

		public void Remove(string name) { this.columnList.Remove(name); }

		public ColumnSchema Find(string name)
		{

			ColumnSchema columnSchema;
			if (this.columnList.TryGetValue(name, out columnSchema))
				return columnSchema;

			return null;

		}

		public bool ContainsKey(string key) { return this.columnList.ContainsKey(key); }

		public int IndexOfKey(string key) { return this.columnList.IndexOfKey(key); }

		public bool TryGetValue(string key, out ColumnSchema value) { return this.columnList.TryGetValue(key, out value); }

		public ColumnSchema this[string index] { get { return this.columnList[index]; } }

		public IEnumerator<ColumnSchema> GetEnumerator() { return columnList.Values.GetEnumerator(); }

	}

}
