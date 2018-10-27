namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a field used to reference the table that owns this row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowTableField : CodeMemberField
	{

		/// <summary>
		/// Represents a declaration of a field used to reference the table that owns this row.
		/// </summary>
		/// <param name="tableSchema">The table that owns this row.</param>
		public RowTableField(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string tableTypeName = string.Format("{0}DataTable", tableSchema.Name);
			string tableFieldName = string.Format("table{0}", tableSchema.Name);

			//			/// <summary>
			//			/// The parent Department table.
			//			/// </summary>
			//			private DepartmentDataTable tableDepartment;
			this.Type = new CodeTypeReference(tableTypeName);
			this.Name = tableFieldName;
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("The parent {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));

		}

	}

}
