namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to remove a row based from the table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableRemoveMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to remove a record from the table.
		/// </summary>
		/// <param name="tableSchema">The table to which this row belongs.</param>
		public TableRemoveMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);
			string rowVariableName = string.Format("{0}Row", tableSchema.Name[0].ToString().ToLower() + tableSchema.Name.Remove(0, 1));

			//			/// <summary>
			//			/// Removes a row from the table.
			//			/// </summary>
			//			/// <param name="departmentRow">The DepartmentRow to be removed from the table.</param>
			//			public void RemoveDepartmentRow(DepartmentRow departmentRow)
			//			{
			//				try
			//				{
			//					// This thread must have exclusive control over the table to remove a row.
			//					this.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//					this.Rows.Remove(departmentRow);
			//				}
			//				finally
			//				{
			//					// The table can now be used by other threads.
			//					this.ReaderWriterLock.ReleaseWriterLock();
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Removes a row from the table.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The {1} to be removed from the table.</param>", rowVariableName, rowTypeName), true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Name = string.Format("Remove{0}", rowTypeName);
			this.Parameters.Add(new CodeParameterDeclarationExpression(rowTypeName, rowVariableName));
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement("This thread must have exclusive control over the table to remove a row."));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "Remove", new CodeArgumentReferenceExpression(rowVariableName)));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can now be used by other threads."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseWriterLock"));
			this.Statements.Add(tryCatchFinallyStatement);

		}

	}

}
