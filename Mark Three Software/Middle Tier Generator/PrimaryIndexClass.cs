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
	class PrimaryIndexClass : CodeTypeDeclaration
	{

		// Private Fields
		private ConstraintSchema constraintSchema;

		/// <summary>
		/// Creates the CodeDOM for a strongly typed index on a table.
		/// </summary>
		/// <param name="schema">The parent schema.</param>
		/// <param name="keySchema">The key schema.</param>
		public PrimaryIndexClass(ConstraintSchema constraintSchema)
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

			//			// Primary indicies use the primary key of the table to find the rows.
			//			private Table table;
			CodeMemberField tableField = new CodeMemberField();
			tableField.Comments.Add(new CodeCommentStatement("Primary indicies use the primary key of the table to find the rows."));
			tableField.Attributes = MemberAttributes.Private;
			tableField.Type = new CodeTypeReference("Table");
			tableField.Name = "table";
			this.Members.Add(tableField);

			//			/// <summary>
			//			/// Create a unique, primary index on the Department table.
			//			/// </summary>
			//			/// <param name="name">The name of the index.</param>
			//			/// <param name="columns">The columns that describe a unique key.</param>
			//			public DepartmentKeyIndex(Column[] columns)
			//			{
			//				// While technically an index could be created with no columns, it would require a run-time check that would
			//				// needlessly steal processor power.
			//				if ((columns.Length == 0))
			//					throw new Exception("Can't create an index with an empty key");
			//				// The primary index uses the native 'Find' method of the base table to search for records.
			//				this.table = columns[0].Table;
			//				this.table.PrimaryKey = columns;
			//			}
			CodeConstructor constructor = new CodeConstructor();
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("Create a primary, unique index on the {0} table.", constraintSchema.Selector.Name), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Comments.Add(new CodeCommentStatement("<param name=\"columns\">The columns that describe a unique key.</param>", true));
			constructor.Attributes = MemberAttributes.Public;
			constructor.Name = indexTypeName;
			constructor.Parameters.Add(new CodeParameterDeclarationExpression("Column[]", "columns"));
			constructor.Statements.Add(new CodeCommentStatement("While technically an index could be created with no columns, it would require a run-time check that would"));
			constructor.Statements.Add(new CodeCommentStatement("needlessly steal processor power."));
			List<CodeStatement> constructorTrueStatements = new List<CodeStatement>();
			constructorTrueStatements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(System.Exception), new CodePrimitiveExpression("Can't create an index with an empty key"))));
			constructor.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("columns"), "Length"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)), constructorTrueStatements.ToArray()));
			constructor.Statements.Add(new CodeCommentStatement("The primary index uses the native 'Find' method of the base table to search for records."));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "table"), new CodeFieldReferenceExpression(new CodeIndexerExpression(new CodeArgumentReferenceExpression("columns"), new CodePrimitiveExpression(0)), "Table")));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "table"), "PrimaryKey"), new CodeArgumentReferenceExpression("columns")));
			this.Members.Add(constructor);

			CodeMemberMethod findByPrimaryKey = new CodeMemberMethod();
			findByPrimaryKey.Comments.Add(new CodeCommentStatement("<summary>", true));
			findByPrimaryKey.Comments.Add(new CodeCommentStatement(string.Format("Finds a row in the {0} table containing the key elements.", this.constraintSchema.Selector.Name), true));
			findByPrimaryKey.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				findByPrimaryKey.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The {1} element of the key.</param>", camelCaseColumnName, columnSchema.Name), true));
			}
			findByPrimaryKey.Comments.Add(new CodeCommentStatement("<returns>A DepartmentRow that contains the key elements, or null if there is no match.</returns>", true));
			findByPrimaryKey.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			findByPrimaryKey.ReturnType = new CodeTypeReference(indexRowName);
			findByPrimaryKey.Name = "Find";
			List<CodeExpression> findByArguments = new List<CodeExpression>();
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				findByPrimaryKey.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, camelCaseColumnName));
				findByArguments.Add(new CodeArgumentReferenceExpression(camelCaseColumnName));
			}
			CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "table"), "Rows"), "Find", new CodeArrayCreateExpression(typeof(System.Object), findByArguments.ToArray()));
			findByPrimaryKey.Statements.Add(new CodeCommentStatement("Use the base table to find a row containing the key elements."));
			findByPrimaryKey.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(indexRowName, codeMethodInvokeExpression)));
			this.Members.Add(findByPrimaryKey);

			//		}

		}

	}

}
