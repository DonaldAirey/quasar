namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class ConstraintSchemaCollection
	{

		private SortedList<string, ConstraintSchema> constraintList;

		public ConstraintSchemaCollection()
		{

			// Initialize the object
			this.constraintList = new SortedList<string, ConstraintSchema>();

		}

		public void Add(ConstraintSchema constraintSchema)
		{

			this.constraintList.Add(constraintSchema.Name, constraintSchema);
		
		}

		public void Remove(string name) { this.constraintList.Remove(name); }

		public ConstraintSchema Find(string name)
		{

			ConstraintSchema constraintSchema;
			if (this.constraintList.TryGetValue(name, out constraintSchema))
				return constraintSchema;

			return null;

		}

		public IEnumerator<ConstraintSchema> GetEnumerator() { return constraintList.Values.GetEnumerator(); }

	}

}
