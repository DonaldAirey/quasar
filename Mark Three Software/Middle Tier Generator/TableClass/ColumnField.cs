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
	class TableColumnField : CodeMemberField
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableColumnField(ColumnSchema columnSchema)
		{

			//			// The DepartmentId Column
			//			private Column columnDepartmentId;
			this.Type = new CodeTypeReference("Column");
			this.Name = string.Format("column{0}", columnSchema.Name);
			this.Comments.Add(new CodeCommentStatement(string.Format("The {0} Column", columnSchema.Name)));

		}

	}

}
