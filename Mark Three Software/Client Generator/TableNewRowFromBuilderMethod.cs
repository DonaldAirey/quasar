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
	class TableNewRowFromBuilderMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public TableNewRowFromBuilderMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Initializes a new instance of a Department row.  Constructs a row from the builder.  Only for internal usage.
			//			/// </summary>
			//			/// <returns>A new row with the same schema as the table.</returns>
			//			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			//			{
			//				return new DepartmentRow(builder);
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Initializes a new instance of a {0} row.  Constructs a row from the builder.  Only for internal usage.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<returns>A new row with the same schema as the table.</returns>", true));
			this.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			this.ReturnType = new CodeTypeReference("DataRow");
			this.Name = "NewRowFromBuilder";
			this.Parameters.Add(new CodeParameterDeclarationExpression("DataRowBuilder", "builder"));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(rowTypeName, new CodeExpression[] { new CodeArgumentReferenceExpression("builder") })));

		}

	}

}
