namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Schema;

	public class FieldSchemaCollection : IEnumerable<FieldSchema>
	{

		private List<FieldSchema> fieldList;

		public FieldSchemaCollection()
		{

			// Initialize the object
			this.fieldList = new List<FieldSchema>();

		}

		public void Add(FieldSchema fieldSchema)
		{

			this.Add(fieldSchema);

		}

		public int Count { get { return this.fieldList.Count; } }

		public FieldSchema this[int index] { get { return this.fieldList[index]; } }

		#region IEnumerable<FieldSchema> Members

		public IEnumerator<FieldSchema> GetEnumerator()
		{
			return this.fieldList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.fieldList.GetEnumerator();
		}

		#endregion
	}

}
