namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to find a row based on a primary key.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableFindByMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to find a record given a unique key.
		/// </summary>
		/// <param name="tableSchema">The table to which this method belongs.</param>
		/// <param name="constraintSchema">The constraint used to find the record.</param>
		public TableFindByMethod(TableSchema tableSchema, ConstraintSchema constraintSchema)
		{

			// This construct is used several times to generate the method.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Finds a row in the Department table containing the key elements.
			//			/// </summary>
			//			/// <param name="departmentId">The DepartmentId element of the key.</param>
			//			/// <returns>A DepartmentRow that contains the key elements, or null if there is no match.</returns>
			//			public DepartmentRow FindByDepartmentId(int departmentId)
			//			{
			//				// Use the index to find a row containing the key elements.
			//				try
			//				{
			//					this.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//					return ((DepartmentRow)(this.Rows.Find(new object[] {
			//								departmentId})));
			//				}
			//				finally
			//				{
			//					this.ReaderWriterLock.ReleaseReaderLock();
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Finds a row in the {0} table containing the key elements.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The {1} element of the key.</param>", camelCaseColumnName, columnSchema.Name), true));
			}
			this.Comments.Add(new CodeCommentStatement("<returns>A DepartmentRow that contains the key elements, or null if there is no match.</returns>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.ReturnType = new CodeTypeReference(rowTypeName);
			this.Name = "FindBy";
			List<CodeExpression> findByArguments = new List<CodeExpression>();
			foreach (ColumnSchema columnSchema in constraintSchema.Fields)
			{
				string camelCaseColumnName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
				this.Name += columnSchema.Name;
				this.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, camelCaseColumnName));
				findByArguments.Add(new CodeArgumentReferenceExpression(camelCaseColumnName));
			}
			CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "Find", new CodeArrayCreateExpression(typeof(System.Object), findByArguments.ToArray()));
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement("The table must be locked to use the index to find a row containing the key elements."));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(rowTypeName, codeMethodInvokeExpression)));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row is found."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseReaderLock"));
			this.Statements.Add(tryCatchFinallyStatement);

		}

	}

}
