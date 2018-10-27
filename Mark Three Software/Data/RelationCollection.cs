namespace MarkThree
{

	using System;
	using System.Data;
	using System.Collections;

	/// <summary>
	/// A collection of relations between two tables.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class RelationCollection : System.Collections.IEnumerable
	{
        
		// Private Members
		private DataRelationCollection dataRelationCollection;
        
		/// <summary>
		/// Create a collection of relationships between tables.
		/// </summary>
		/// <param name="dataRelationCollection">The original collection based on the DataRelationCollection.</param>
		public RelationCollection(DataRelationCollection dataRelationCollection)
		{

			// The main idea of this class is to provide a strongly typed collection of Relation objects.
			this.dataRelationCollection = dataRelationCollection;

		}
        
		/// <summary>
		/// Gets a Relation based on the name.
		/// </summary>
		/// <param name="index">The name of the relation.</param>
		/// <returns>The relation with the given name or null if the relation doesn't exist.</returns>
		public Relation this[string index] { get { return ((Relation)(this.dataRelationCollection[index])); } }

		/// <summary>
		/// Gets an enumerator used to iterate through a collection of Relations.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through a collection of Relations.</returns>
		public System.Collections.IEnumerator GetEnumerator() { return this.dataRelationCollection.GetEnumerator(); }

		/// <summary>
		/// Adds a Relation to the collection.
		/// </summary>
		/// <param name="relation">The relation to be added to the collection.</param>
		public void Add(Relation relation) { this.dataRelationCollection.Add(relation); }

	}
    
}
