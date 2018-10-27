namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TypeSchemaCollection
	{

		private SortedList<string, TypeSchema> classList;

		public TypeSchemaCollection()
		{

			// Initialize the object
			this.classList = new SortedList<string, TypeSchema>();

		}

		public void Add(TypeSchema classSchema)
		{

			this.classList.Add(classSchema.Name, classSchema);
		
		}

		public int Count { get { return this.classList.Count; } }

		public bool Contains(string name) { return this.classList.ContainsKey(name); }

		public void Remove(string name) { this.classList.Remove(name); }

		public TypeSchema Find(string name)
		{

			TypeSchema classSchema;
			if (this.classList.TryGetValue(name, out classSchema))
				return classSchema;

			return null;

		}

		public IEnumerator<TypeSchema> GetEnumerator() { return classList.Values.GetEnumerator(); }

	}

}
