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
	class TableAddRowWithValuesMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public TableAddRowWithValuesMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Add a Department row to the collection.
			//			/// </summary>
			//			/// <param name="departmentId">The initial value of the DepartmentId column.</param>
			//			public DepartmentRow AddDepartmentRow(int departmentId)
			//			{
			//				try
			//				{
			//					// This adds a new row to the Department table table after populating the columns with the initial data.
			//					this.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//					DepartmentRow rowDepartmentRow = ((DepartmentRow)(this.NewRow()));
			//					rowDepartmentRow.ItemArray = new object[] {
			//							departmentId};
			//					this.Rows.Add(rowDepartmentRow);
			//					return rowDepartmentRow;
			//				}
			//				finally
			//				{
			//					// The table can be accessed by other threads once the row is added.
			//					this.ReaderWriterLock.ReleaseWriterLock();
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Add a {0} row to the collection.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
					if (!columnSchema.IsAutoIncrement)
					{
						string parameterName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
						this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The initial value of the {1} column.</param>", parameterName, columnSchema.Name), true));
					}
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.ReturnType = new CodeTypeReference(rowTypeName);
			this.Name = string.Format("Add{0}Row", tableSchema.Name);
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
					if (!columnSchema.IsAutoIncrement)
					{
						string parameterName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
						this.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, parameterName));
					}
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement(string.Format("This adds a new row to the {0} table table after populating the columns with the initial data.", tableSchema.Name)));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeVariableDeclarationStatement(String.Format("{0}Row", tableSchema.Name), String.Format("row{0}Row", tableSchema.Name), new CodeCastExpression(String.Format("{0}Row", tableSchema.Name), new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "NewRow", new CodeExpression[] { }))));
			CodeExpressionCollection codeExpressionCollection = new CodeExpressionCollection();
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
				{
					string parameterName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
					if (columnSchema.IsAutoIncrement)
						codeExpressionCollection.Add(new CodePrimitiveExpression(null));
					else
						codeExpressionCollection.Add(new CodeVariableReferenceExpression(parameterName));
				}
			CodeExpression[] itemArray = new CodeExpression[codeExpressionCollection.Count];
			for (int index = 0; index < codeExpressionCollection.Count; index++)
				itemArray[index] = codeExpressionCollection[index];
			tryCatchFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(string.Format("row{0}Row", tableSchema.Name)), "ItemArray"), new CodeArrayCreateExpression(typeof(System.Object), itemArray)));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "Add", new CodeExpression[] { new CodeArgumentReferenceExpression(string.Format("row{0}Row", tableSchema.Name)) }));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(string.Format("row{0}Row", tableSchema.Name))));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row is added."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseWriterLock"));
			this.Statements.Add(tryCatchFinallyStatement);

		}

	}

}
