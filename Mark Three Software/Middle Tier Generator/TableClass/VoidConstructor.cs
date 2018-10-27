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
	class TableVoidConstructor : CodeConstructor
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableVoidConstructor(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Creates the Department table.
			//			/// </summary>
			//			internal DepartmentDataTable() : 
			//					base("Department")
			//			{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Creates the {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Assembly;
			this.BaseConstructorArgs.Add(new CodePrimitiveExpression(tableSchema.Name));

			// Initialize the 'IsPersistent' property.
			// e.g.				this.IsPersistent = false;
			if (tableSchema.IsPersistent)
			{
				this.Statements.Add(new CodeCommentStatement("This indicates that the table should not be saved to the durable storage."));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "IsPersistent"), new CodePrimitiveExpression(tableSchema.IsPersistent)));
			}

			//				// The DepartmentId Column
			//				this.columnDepartmentId = new Column("DepartmentId", typeof(int));
			//				this.columnDepartmentId.AllowDBNull = false;
			//				this.Columns.Add(this.columnDepartmentId);
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
				{

					// Create the column using the datatype specified in the schema.
					this.Statements.Add(new CodeCommentStatement(string.Format("The {0} Column", columnSchema.Name)));
					CodeExpression right = new CodeObjectCreateExpression("Column", new CodeExpression[] { new CodePrimitiveExpression(columnSchema.Name), new CodeTypeOfExpression(columnSchema.DataType) });
					this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), right));

					// Persistent property
					if (tableSchema.IsPersistent)
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "IsPersistent"), new CodePrimitiveExpression(columnSchema.IsPersistent)));

					// AutoIncrement, AutoIncrementSeed and AutoIncrementStep Properties.
					if (columnSchema.IsAutoIncrement)
					{
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "AutoIncrement"), new CodePrimitiveExpression(true)));
						if (columnSchema.AutoIncrementSeed > 0)
							this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "AutoIncrementSeed"), new CodePrimitiveExpression(columnSchema.AutoIncrementSeed)));
						if (columnSchema.AutoIncrementStep > 1)
							this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "AutoIncrementStep"), new CodePrimitiveExpression(columnSchema.AutoIncrementStep)));
					}

					// AllowDBNull Column property
					if (columnSchema.MinOccurs == 1)
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "AllowDBNull"), new CodePrimitiveExpression(false)));

					// The default value exists as a string in the Xml Schema.  It must be stronly typed before being inserted into
					// the target code. Unfortunately, the type information for the destination column is needed to convert the
					// value properly.
					if (columnSchema.MinOccurs == 0 && columnSchema.DefaultValue != null)
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)), "DefaultValue"), new CodePrimitiveExpression(columnSchema.DefaultValue)));

					// This will add the column created above to the table.
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Columns"), "Add", new CodeExpression[] { new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)) }));

				}

			//				// The DepartmentKey Index
			//				this.indexDepartmentKey = new DepartmentKeyIndex("DepartmentKey", new Column[] {
			//							this.columnDepartmentId});
			//				this.Constraints.Add(this.indexDepartmentKey);
			// 				this.Constraints.Add(new UniqueConstraint(new Column[] { this.columnObjectId }));
			foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("The {0} Index", constraintSchema.Name)));
				string indexVariableName = string.Format("index{0}", constraintSchema.Name);
				List<CodeExpression> keyColumns = new List<CodeExpression>();
				foreach (ColumnSchema columnSchema in constraintSchema.Fields)
					keyColumns.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("column{0}", columnSchema.Name)));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), indexVariableName),
					new CodeObjectCreateExpression(new CodeTypeReference(string.Format("{0}Index", constraintSchema.Name)), new CodeArrayCreateExpression(new CodeTypeReference("Column"),
					keyColumns.ToArray()))));
				if (!constraintSchema.IsNullable && !constraintSchema.IsPrimaryKey)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Constraints"), "Add",
						new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.Data.UniqueConstraint)), new CodeArrayCreateExpression(new CodeTypeReference("Column"),
					keyColumns.ToArray()))));
			}

			//			}

		}

	}

}
