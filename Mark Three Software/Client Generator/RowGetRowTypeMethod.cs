namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Changed event.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowGetRowTypeMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public RowGetRowTypeMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//            protected override System.Type GetRowType() {
			//                return typeof(DepartmentRow);
			//            }
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Returns the type of the row in a {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<returns>The {0} type.</returns>", rowTypeName), true));
			this.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			this.ReturnType = new CodeTypeReference(typeof(System.Type));
			this.Name = "GetRowType";
			this.Statements.Add(new CodeMethodReturnStatement(new CodeTypeOfExpression(rowTypeName)));

		}

	}

}
