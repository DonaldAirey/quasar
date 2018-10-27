namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Describes a collection of columns in a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ColumnSchemaCollection
	{

		// Private Members
		private System.Collections.ArrayList arrayList;

		/// <summary>
		/// Creates a collection of columns in a data model.
		/// </summary>
		public ColumnSchemaCollection()
		{

			// Initialize the object
			this.arrayList = new ArrayList();

		}

		/// <summary>
		/// Add a ColumnSchema to the collection.
		/// </summary>
		/// <param name="columnSchema">A column to be added to the list.</param>
		public void Add(ColumnSchema columnSchema)
		{

			// The collection is order by the qualified name of the column.
			int index = this.arrayList.BinarySearch(columnSchema);
			if (index < 0)
				this.arrayList.Insert(~index, columnSchema);
		
		}

		/// <summary>
		/// Removes a ColumnSchema from the collection.
		/// </summary>
		/// <param name="xmlQualifiedName">The key of the column to be removed.</param>
		public void Remove(XmlQualifiedName xmlQualifiedName)
		{

			// Remove the column that matches the qualified name.
			int index = this.arrayList.BinarySearch(xmlQualifiedName);
			if (index >= 0)
				this.arrayList.RemoveAt(index);

		}

		/// <summary>
		/// Finds a column with the specified qualified name.
		/// </summary>
		/// <param name="xmlQualifiedName">The qualified name of the column.</param>
		/// <returns>The column that matches the specified qualified name.</returns>
		public ColumnSchema Find(XmlQualifiedName xmlQualifiedName)
		{

			// The collection is ordered and indexed using the qualified name of the column.
			int index = this.arrayList.BinarySearch(xmlQualifiedName);
			return index <  0 ? null : this.arrayList[index] as ColumnSchema;

		}

		/// <summary>
		/// Indicates whether a specified key exists in the collection of ColumnSchemas.
		/// </summary>
		/// <param name="xmlQualifiedName">The name of a key.</param>
		/// <returns>true if the key exists in the list, false otherwise.</returns>
		public bool ContainsKey(XmlQualifiedName xmlQualifiedName) { return this.arrayList.BinarySearch(xmlQualifiedName) >= 0; }

		/// <summary>
		/// Accesses the list of ColumnSchemas using the qualified name.
		/// </summary>
		/// <param name="xmlQualifiedName">The qualified name of a ColumnSchema.</param>
		/// <returns>The ColumnSchema matching the qualified name, or null if the key doesn't exist in the table.</returns>
		public ColumnSchema this[XmlQualifiedName xmlQualifiedName]
		{
			get
			{

				// The table is randomly accessed using the qualified name of the column.
				int index = this.arrayList.BinarySearch(xmlQualifiedName);
				return index < 0 ? null : this.arrayList[index] as ColumnSchema;

			}
		}

		/// <summary>
		/// Returns an enumerator for iterating through the list of ColumnSchemas.
		/// </summary>
		/// <returns>An enumerator that can be used for iterating through the collection of ColumnSchemas.</returns>
		public IEnumerator GetEnumerator() { return this.arrayList.GetEnumerator(); }

	}

}
