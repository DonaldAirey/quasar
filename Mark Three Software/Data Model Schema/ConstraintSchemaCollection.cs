namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Describes a column in a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ConstraintSchemaCollection
	{

		// Private Fields
		private System.Collections.ArrayList arrayList;

		// Public Events
		public ConstraintEvent ItemAdded;

		/// <summary>
		/// Creates a collection of constraints.
		/// </summary>
		public ConstraintSchemaCollection()
		{

			// Initialize the object
			this.arrayList = new ArrayList();

		}

		/// <summary>
		/// Add a constraint to the collection.
		/// </summary>
		/// <param name="constraintSchema">A constraint to be added to the collection.</param>
		public void Add(ConstraintSchema constraintSchema)
		{

			// Add the item a collection of constraints ordered by the qualified name.
			int index = this.arrayList.BinarySearch(constraintSchema);
			if (index < 0)
				this.arrayList.Insert(~index, constraintSchema);

			// Broadcast the addition to anyone listening.  For practical purposes, this means the TableSchema that needs to be 
			// informed about new relations so it can build relations between the tables.
			if (this.ItemAdded != null)
				this.ItemAdded(this, new ConstraintEventArgs(constraintSchema));
		
		}

		/// <summary>
		/// The number of ConstraintSchemas in the collection.
		/// </summary>
		public int Count { get { return this.arrayList.Count; } }

		/// <summary>
		/// Gets an enumerator used to iterate through the collection of ConstraintSchemas.
		/// </summary>
		/// <returns>An iterator that can be used to iterate through the collection of ConstraintSchemas.</returns>
		public IEnumerator GetEnumerator() { return this.arrayList.GetEnumerator(); }

	}

}
