namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description of a strongly typed Row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TypedRowClass : CodeTypeDeclaration
	{

		/// <summary>
		/// Creates the CodeDOM for a strongly typed row in a strongly typed table.
		/// </summary>
		/// <param name="tableSchema">The table schema that describes this row.</param>
		public TypedRowClass(TableSchema tableSchema)
		{

			//		/// <summary>
			//		/// Represents a row of data in the Department table.
			//		/// </summary>
			//		[System.Diagnostics.DebuggerStepThrough()]
			//		public class DepartmentRow : Row
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Represents a row of data in the {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			CodeTypeDeclaration tableClass = new CodeTypeDeclaration();
			this.IsClass = true;
			this.Name = string.Format("{0}Row", tableSchema.Name);
			this.TypeAttributes = TypeAttributes.Public;
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.BaseTypes.Add("Row");

			// This field is used to reference the table that ownes this row.
			this.Members.Add(new RowTableField(tableSchema));

			// Generate a constructor for new strongly typed rows.
			this.Members.Add(new RowConstructor(tableSchema));

			// This will create strongly typed accessor properties for each of the columns in the row.  Note that nullable columns
			// require extra handling when converting to a basic datatype.  The 'IsColumnNull' allows for explicit testing and
			// setting of the column to a null value when the strongly typed accessor is used.
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
				{
					this.Members.Add(new RowColumnProperty(tableSchema, columnSchema));
					if (columnSchema.MinOccurs == 0)
						this.Members.Add(new RowIsNullProperty(tableSchema, columnSchema));
				}

			// Generate a property to access the parent row.
			foreach (KeyrefSchema keyrefSchema in tableSchema.MemberParentKeys)
				this.Members.Add(new RowParentRowProperty(keyrefSchema));

			// Generate methods to get a list of the children rows.
			foreach (KeyrefSchema foreignKey in tableSchema.ChildKeyrefs)
				this.Members.Add(new RowChildRowsMethod(foreignKey));

			// The BeginEdit and EditEdit methods allow for thread safe access to the columns in a row.
			this.Members.Add(new RowBeginEditMethod(tableSchema));
			this.Members.Add(new RowEndEditMethod(tableSchema));

		}

	}

}
