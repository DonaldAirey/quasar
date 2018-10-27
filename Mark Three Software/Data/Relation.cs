namespace MarkThree
{

	using System;
	using System.Data;

	/// <summary>
	/// Defines a relationship between two tables in a DataSet.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Relation : DataRelation
	{
        
		/// <summary>
		/// Creates a relationship between two tables.
		/// </summary>
		/// <param name="relationName">The name of the relationship.</param>
		/// <param name="parentColumns">The columns of the parent table involved in the relationship.</param>
		/// <param name="childColumns">The columns of the child table involved in the relationship.</param>
		/// <param name="createConstraints">The constraints imposed on the relationship.</param>
		public Relation(string relationName, Column[] parentColumns, Column[] childColumns, bool createConstraints) :
			base(relationName, parentColumns, childColumns, createConstraints)
		{

			// This constructor left intentionally blank because the automatic formatting makes it look retarded.

		}

		/// <summary>
		/// The parent table of this Relation.
		/// </summary>
		public new Table ParentTable { get { return base.ParentTable as Table; } }

		/// <summary>
		/// The child table of this Relation.
		/// </summary>
		public new Table ChildTable { get { return base.ChildTable as Table; } }

	}
    
}
