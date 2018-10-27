namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description of a strongly typed index.
	/// </summary>
	class TypedIndexClass : CodeTypeDeclaration
	{

		// Private Fields
		private ConstraintSchema constraintSchema;

		/// <summary>
		/// Creates the CodeDOM for a strongly typed index on a table.
		/// </summary>
		/// <param name="schema">The parent schema.</param>
		/// <param name="keySchema">The key schema.</param>
		public TypedIndexClass(ConstraintSchema constraintSchema)
		{

			// Initialize the object.
			this.constraintSchema = constraintSchema;

			// Construct the type names for the table and rows within the table.
			string indexTypeName = string.Format("{0}Index", constraintSchema.Name);
			string indexRowName = string.Format("{0}Row", constraintSchema.Selector.Name);

			//		/// <summary>
			//		/// Represents a means of identifying a Department row using a set of columns in which all values must be unique.
			//		/// </summary>
			//		[System.Diagnostics.DebuggerStepThrough()]
			//		public class DepartmentKeyIndex
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Represents a means of identifying a {0} row using a set of columns in which all values must be unique.", constraintSchema.Selector.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			CodeTypeDeclaration tableClass = new CodeTypeDeclaration();
			this.IsClass = true;
			this.Name = indexTypeName;
			this.TypeAttributes = TypeAttributes.Public;
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));

			//			// A non-primary index is created using a DataView find rows matching the keys.
			//			private DataView dataView;
			CodeMemberField dataViewField = new CodeMemberField();
			dataViewField.Comments.Add(new CodeCommentStatement("A non-primary index is created using a DataView find rows matching the keys."));
			dataViewField.Attributes = MemberAttributes.Private;
			dataViewField.Type = new CodeTypeReference(typeof(System.Data.DataView));
			dataViewField.Name = "dataView";
			this.Members.Add(dataViewField);

			//			/// <summary>
			//			/// Create an index on the Object table.
			//			/// </summary>
			//			/// <param name="name">The name of the index.</param>
			//			/// <param name="columns">The columns that describe a unique key.</param>
			//			public ObjectKeyExternalId0Index(Column[] columns)
			//			{
			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("Create an index on the {0} table.", constraintSchema.Selector.Name), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Comments.Add(new CodeCommentStatement("<param name=\"name\">The name of the index.</param>", true));
			constructor.Comments.Add(new CodeCommentStatement("<param name=\"columns\">The columns that describe a unique key.</param>", true));
			constructor.Attributes = MemberAttributes.Public;
			constructor.Name = indexTypeName;
			constructor.Parameters.Add(new CodeParameterDeclarationExpression("Column[]", "columns"));

			//				// The DataView is used to implement an index on the table that can be used to find elements.
			//				this.dataView = new System.Data.DataView();
			constructor.Statements.Add(new CodeCommentStatement("The DataView is used to implement an index on the table that can be used to find elements."));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), new CodeObjectCreateExpression(typeof(System.Data.DataView))));

			//				// When a meaningful key is specified, the DataView is used to find current rows containing non-null values in the 
			//				// specified columns that match key elements.
			//				if ((columns.Length != 0))
			//				{
			constructor.Statements.Add(new CodeCommentStatement("When a meaningful key is specified, the DataView is used to find current rows containing non-null values in the "));
			constructor.Statements.Add(new CodeCommentStatement("specified columns that match key elements."));
			List<CodeStatement> constructorTrueStatements = new List<CodeStatement>();

			//					// This will construct the strings that the DataView uses to sort and filter the rows.  Note that only non-null
			//					// keys are allowed into the view.
			//					string sort = string.Empty;
			//					string rowFilter = string.Empty;
			//					for (int columnIndex = 0; (columnIndex < columns.Length); columnIndex = (columnIndex + 1))
			//					{
			//						Column column = columns[columnIndex];
			//						if ((columnIndex 
			//									< (columns.Length - 1)))
			//						{
			//							sort = string.Format("{0}{1},", sort, column.ColumnName);
			//							rowFilter = string.Format("{0}IsNull({1}, \'null\')<>\'null\',", rowFilter, column.ColumnName);
			//						}
			//						else
			//						{
			//							sort = string.Format("{0}{1}", sort, column.ColumnName);
			//							rowFilter = string.Format("{0}IsNull({1}, \'null\')<>\'null\'", rowFilter, column.ColumnName);
			//						}
			//					}
			constructorTrueStatements.Add(new CodeCommentStatement("This will construct the strings that the DataView uses to sort and filter the rows.  Note that only non-null"));
			constructorTrueStatements.Add(new CodeCommentStatement("keys are allowed into the view."));
			constructorTrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.String), "sort", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.String)), "Empty")));
			List<CodeStatement> constructorIterationStatements = new List<CodeStatement>();
			constructorIterationStatements.Add(new CodeVariableDeclarationStatement("Column", "column", new CodeIndexerExpression(new CodeVariableReferenceExpression("columns"), new CodeVariableReferenceExpression("columnIndex"))));
			constructorIterationStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.LessThan, new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("columns"), "Length"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
				new CodeStatement[] {
					new CodeAssignStatement(new CodeVariableReferenceExpression("sort"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(System.String)), "Format"), new CodePrimitiveExpression("{0}{1},"), new CodeVariableReferenceExpression("sort"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "ColumnName"))),
					},
				new CodeStatement[] {
					new CodeAssignStatement(new CodeVariableReferenceExpression("sort"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(System.String)), "Format"), new CodePrimitiveExpression("{0}{1}"), new CodeVariableReferenceExpression("sort"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "ColumnName"))),
				}));
			constructorTrueStatements.Add(new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "columnIndex", new CodePrimitiveExpression(0)),
				new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("columns"), "Length")),
				new CodeAssignStatement(new CodeVariableReferenceExpression("columnIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
				constructorIterationStatements.ToArray()));

			//					// This DataView will be used to find the current rows in the Department table that contain the non-null key 
			//					// elements.
			//					this.dataView.Table = columns[0].Table;
			//					this.dataView.Sort = sort;
			//					this.dataView.RowStateFilter = System.Data.DataViewRowState.CurrentRows;
			constructorTrueStatements.Add(new CodeCommentStatement("This DataView will be used to find the current rows in the Department table that contain the non-null key "));
			constructorTrueStatements.Add(new CodeCommentStatement("elements."));
			constructorTrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), "Table"), new CodeFieldReferenceExpression(new CodeIndexerExpression(new CodeArgumentReferenceExpression("columns"), new CodePrimitiveExpression(0)), "Table")));
			constructorTrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), "Sort"), new CodeVariableReferenceExpression("sort")));
			constructorTrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), "RowStateFilter"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataViewRowState)), "CurrentRows")));

			//				}
			constructor.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("columns"), "Length"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0)), constructorTrueStatements.ToArray()));

			//			}
			this.Members.Add(constructor);

			//			/// <summary>
			//			/// Finds a row in the Department table containing the key elements.
			//			/// </summary>
			//			/// <param name="departmentId">The DepartmentId element of the key.</param>
			//			/// <returns>A DepartmentRow that contains the key elements, or null if there is no match.</returns>
			//			public DepartmentRow Find(int departmentId)
			//			{
			//				// This will find the first row in the DataView that contains the key values.  A 'null' indicates that there was no
			//				// matching column.
			//				int index = this.dataView.Find(new object[] {
			//							departmentId});
			//				if ((index == -1))
			//				{
			//					return null;
			//				}
			//				else
			//				{
			//					return ((DepartmentRow)(this.dataView[index].Row));
			//				}
			//			}
			CodeMemberMethod findMethod = new CodeMemberMethod();
			findMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			findMethod.Comments.Add(new CodeCommentStatement(string.Format("Finds a row in the {0} table containing the key elements.", this.constraintSchema.Selector.Name), true));
			findMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				findMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The {1} element of the key.</param>", camelCaseColumnName, columnSchema.Name), true));
			}
			findMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>A {0} that contains the key elements, or null if there is no match.</returns>", indexRowName), true));
			findMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			findMethod.ReturnType = new CodeTypeReference(indexRowName);
			findMethod.Name = "Find";
			List<CodeExpression> findByArguments = new List<CodeExpression>();
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				findMethod.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, camelCaseColumnName));
				findByArguments.Add(new CodeArgumentReferenceExpression(camelCaseColumnName));
			}
			findMethod.Statements.Add(new CodeCommentStatement("This will find the first row in the DataView that contains the key values.  A 'null' indicates that there was no"));
			findMethod.Statements.Add(new CodeCommentStatement("matching column."));
			findMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Int32), "index", new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), "Find", new CodeArrayCreateExpression(typeof(System.Object), findByArguments.ToArray()))));
			findMethod.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("index"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(-1)),
				new CodeStatement[] {new CodeMethodReturnStatement(new CodePrimitiveExpression(null))},
				new CodeStatement[] {new CodeMethodReturnStatement(new CodeCastExpression(indexRowName, new CodeFieldReferenceExpression(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataView"), new CodeVariableReferenceExpression("index")), "Row")))}));
			this.Members.Add(findMethod);

			//		}

		}

	}

}
