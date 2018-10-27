namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableParentRelationField : CodeMemberField
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableParentRelationField(KeyrefSchema keyrefSchema)
		{

			//			// The Relation between the Department and Employee tables
			//			private Relation relationDepartmentEmployee;
			this.Type = new CodeTypeReference("Relation");
			this.Name = string.Format("relation{0}{1}", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name);
			this.Comments.Add(new CodeCommentStatement(string.Format("The Relation between the {0} and {1} tables", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)));

		}

	}

}
