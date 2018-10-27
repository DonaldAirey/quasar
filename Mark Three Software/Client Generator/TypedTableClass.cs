namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description of a strongly typed Table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TypedTableClass : CodeTypeDeclaration
	{

		/// <summary>
		/// Create a CodeDOM description of a strongly typed Table.
		/// </summary>
		/// <param name="tableSchema">The schema that describes the table.</param>
		public TypedTableClass(TableSchema tableSchema)
		{

			//		/// <summary>
			//		/// The Department table.
			//		/// </summary>
			//		[System.Diagnostics.DebuggerStepThrough()]
			//		public class DepartmentDataTable : Table, System.Collections.IEnumerable
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("The {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			CodeTypeDeclaration tableClass = new CodeTypeDeclaration(tableSchema.Name);
			this.IsClass = true;
			this.Name = string.Format("{0}DataTable", tableSchema.Name);
			this.TypeAttributes = TypeAttributes.Public;
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.BaseTypes.Add("Table");
			this.BaseTypes.Add("System.Collections.IEnumerable");

			// Create a field for each of the non-inherited columns in this table.
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
					this.Members.Add(new TableColumnField(columnSchema));

			// Create fields for the parent and child relations.
			foreach (KeyrefSchema keyrefSchema in tableSchema.ParentKeyrefs)
				this.Members.Add(new TableParentRelationField(keyrefSchema));
			foreach (KeyrefSchema keyrefSchema in tableSchema.ChildKeyrefs)
				this.Members.Add(new TableChildRelationField(keyrefSchema));

			// Create an index for each of the keys defined for this table.
			foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
				this.Members.Add(new TableIndexField(constraintSchema));

			// Create a constructor for the table with no arguments.
			this.Members.Add(new TableVoidConstructor(tableSchema));

			// Create the property that counts the number of rows in the table.
			this.Members.Add(new TableCountProperty(tableSchema));

			// Create the properties to access each of the column in the table.
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
					this.Members.Add(new TableColumnProperty(tableSchema, columnSchema));

			// Create a method to access a row given the index of that row.
			this.Members.Add(new TableRowIndexerProperty(tableSchema));

			// Add public properties for the parent and child relations on this table.
			foreach (KeyrefSchema keyrefSchema in tableSchema.ParentKeyrefs)
				this.Members.Add(new TableParentRelationProperty(keyrefSchema));
			foreach (KeyrefSchema keyrefSchema in tableSchema.ChildKeyrefs)
				this.Members.Add(new TableChildRelationProperty(keyrefSchema));

			// Add a property for all the keys defined for this table.
			foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
				this.Members.Add(new TableKeyProperty(constraintSchema));

			// Create the strongly typed event handlers for modified and deleted rows.
			this.Members.Add(new TableRowChangedEvent(tableSchema));
			this.Members.Add(new TableRowChangingEvent(tableSchema));
			this.Members.Add(new TableRowDeletedEvent(tableSchema));
			this.Members.Add(new TableRowDeletingEvent(tableSchema));

			// This method is used to initialize the relations properties after all the rest of the table has been initialized.
			this.Members.Add(new TableInitializeRelationsMethod(tableSchema));
			
			// Create methods to add a row to the table.
			this.Members.Add(new TableAddRowMethod(tableSchema));
			this.Members.Add(new TableAddRowWithValuesMethod(tableSchema));

			// Create methods to create and build a new row.
			this.Members.Add(new TableNewTableRowMethod(tableSchema));
			this.Members.Add(new TableNewRowFromBuilderMethod(tableSchema));

			// Create methods to find a record in the table using a unique index.
			foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
				if (constraintSchema.IsPrimaryKey)
					this.Members.Add(new TableFindByMethod(tableSchema, constraintSchema));

			// Method to create a strongly typed enumerator.
			this.Members.Add(new TableGetEnumeratorMethod(tableSchema));

			// Method to query the row type.
			this.Members.Add(new RowGetRowTypeMethod(tableSchema));

			// Method to handle the strongly typed Row Changed event.
			this.Members.Add(new TableOnRowChangedMethod(tableSchema));

			// Method to handle the strongly typed Row Changing event.
			this.Members.Add(new TableOnRowChangingMethod(tableSchema));

			// Method to handle the strongly typed Row Deleted event.
			this.Members.Add(new TableOnRowDeletedMethod(tableSchema));

			// Method to handle the strongly typed Row Deleting event.
			this.Members.Add(new TableOnRowDeletingMethod(tableSchema));

			// Method to remove a strongly typed row.
			this.Members.Add(new TableRemoveMethod(tableSchema));

			//		}

		}

	}

}
