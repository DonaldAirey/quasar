namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class ComplexTypeSchemaCollection
	{

		private SortedList<XmlQualifiedName, ComplexTypeSchema> classList;

		public ComplexTypeSchemaCollection()
		{

			// Initialize the object
			this.classList = new SortedList<XmlQualifiedName, ComplexTypeSchema>();

		}

		public void Add(ComplexTypeSchema classSchema)
		{

			this.classList.Add(classSchema.QualifiedName, classSchema);
		
		}

		public int Count { get { return this.classList.Count; } }

		public bool Contains(XmlQualifiedName xmlQualifiedName) { return this.classList.ContainsKey(xmlQualifiedName); }

		public void Remove(XmlQualifiedName xmlQualifiedName) { this.classList.Remove(xmlQualifiedName); }

		public ComplexTypeSchema Find(XmlQualifiedName xmlQualifiedName)
		{

			ComplexTypeSchema typeSchema;
			if (this.classList.TryGetValue(xmlQualifiedName, out typeSchema))
				return typeSchema;

			return null;

		}

		public IEnumerator<ComplexTypeSchema> GetEnumerator() { return classList.Values.GetEnumerator(); }

	}

}
