namespace MarkThree.MiddleTier
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
	class TableRowChangingEvent : CodeMemberEvent
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableRowChangingEvent(TableSchema tableSchema)
		{

			//			/// <summary>
			//			/// Occurs when a Department row is changing.
			//			/// </summary>
			//			public event DepartmentRowChangeEventHandler DepartmentRowChanging;
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Occurs when a {0} row is changing.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public;
			this.Name = string.Format("{0}RowChanging", tableSchema.Name);
			this.Type = new CodeTypeReference(string.Format("{0}RowChangeEventHandler", tableSchema.Name));

		}

	}

}
