namespace MarkThree.MiddleTier.Row
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a constuctor for a strongly typed row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowConstructor : CodeConstructor
	{

		/// <summary>
		/// Generates a constuctor for a strongly typed row.
		/// </summary>
		/// <param name="tableSchema">The table to which this constructor belongs.</param>
		public RowConstructor(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string tableTypeName = string.Format("{0}DataTable", tableSchema.Name);
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Creates a row of data from the Department table schema.
			//			/// </summary>
			//			/// <param name="rb">An internal data structure used to build the row from the parent table schema.</param>
			//			internal DepartmentRow(DataRowBuilder rb) : 
			//					base(rb)
			//			{
			//				this.tableDepartment = ((DepartmentDataTable)(this.Table));
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Creates a row of data from the {0} table schema.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<param name=\"rb\">An internal data structure used to build the row from the parent table schema.</param>", true));
			this.Attributes = MemberAttributes.Assembly;
			this.Name = rowTypeName;
			this.Parameters.Add(new CodeParameterDeclarationExpression("DataRowBuilder", "rb"));
			this.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("rb"));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), new CodeCastExpression(tableTypeName, new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Table"))));

		}

	}

}
