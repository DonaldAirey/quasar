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
	class TableRowDeletingEvent : CodeMemberEvent
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableRowDeletingEvent(TableSchema tableSchema)
		{

			//			/// <summary>
			//			/// Occurs before a Department row in the table is about to be deleted.
			//			/// </summary>
			//			public event DepartmentRowChangeEventHandler DepartmentRowDeleting;
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Occurs before a {0} row in the table is about to be deleted.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public;
			this.Name = string.Format("{0}RowDeleting", tableSchema.Name);
			this.Type = new CodeTypeReference(string.Format("{0}RowChangeEventHandler", tableSchema.Name));

		}

	}

}
