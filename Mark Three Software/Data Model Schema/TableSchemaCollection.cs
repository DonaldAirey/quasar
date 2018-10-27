namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TableSchemaCollection
	{

		private ArrayList tableList;

		public TableSchemaCollection()
		{

			// Initialize the object
			this.tableList = new ArrayList();

		}

		public void Add(TableSchema tableSchema)
		{

			int index = this.tableList.BinarySearch(tableSchema);
			if (index < 0)
				this.tableList.Insert(~index, tableSchema);
		
		}

		public int Count { get { return this.tableList.Count; } }

		public bool Contains(XmlQualifiedName xmlQualifiedName)
		{
			return this.tableList.Contains(xmlQualifiedName);
		}

		public void Remove(TableSchema tableSchema)
		{
			this.tableList.Remove(tableSchema);
		}

		public TableSchema Find(XmlQualifiedName xmlQualifiedName)
		{

			int index = this.tableList.BinarySearch(xmlQualifiedName);
			return index >= 0 ? this.tableList[index] as TableSchema : null;

		}

		public IEnumerator GetEnumerator() { return this.tableList.GetEnumerator(); }

	}

}
