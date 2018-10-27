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
	class TableIndexField : CodeMemberField
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableIndexField(ConstraintSchema constraintSchema)
		{

			//			// The DepartmentKey Index
			//			private DepartmentKeyIndex indexDepartmentKey;
			this.Type = new CodeTypeReference(string.Format("{0}Index", constraintSchema.Name));
			this.Name = string.Format("index{0}", constraintSchema.Name);
			this.Comments.Add(new CodeCommentStatement(string.Format("The {0} Index", constraintSchema.Name)));

		}

	}

}
