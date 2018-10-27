namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableColumnProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableColumnProperty(TableSchema tableSchema, ColumnSchema columnSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Gets the DepartmentId column of the Department table.
			//			/// </summary>
			//			public Column DepartmentIdColumn
			//			{
			//				get
			//				{
			//					return this.columnDepartmentId;
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the {0} column of the {1} table.", columnSchema.Name, tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference("Column");
			this.Name = string.Format("{0}Column", columnSchema.Name);
			this.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name))));

		}

	}

}
