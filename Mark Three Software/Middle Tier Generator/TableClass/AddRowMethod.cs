namespace MarkThree.MiddleTier.TableClass
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
	class AddRowMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public AddRowMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);
			string rowVariableName = string.Format("{0}Row", tableSchema.Name[0].ToString().ToLower() + tableSchema.Name.Remove(0, 1));

			// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
			// all the table locks that are used for this operation and organizes them in a list that is used to generate the
			// locking and releasing statements below.
			List<LockRequest> tableLockList = new List<LockRequest>();
			tableLockList.Add(new WriteRequest(tableSchema));
			foreach (KeyrefSchema parentKeyref in tableSchema.ParentKeyrefs)
				tableLockList.Add(new ReadRequest(parentKeyref.Refer.Selector));
			tableLockList.Sort();

			//			/// <summary>
			//			/// Add a Department row to the collection.
			//			/// </summary>
			//			/// <param name="departmentRow">The row to be added to the Department table.</param>
			//			public void AddDepartmentRow(DepartmentRow departmentRow)
			//			{
			//				try
			//				{
			//					// This adds a new row to the Department table.
			//					this.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//					this.Rows.Add(departmentRow);
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
			this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The row to be added to the Department table.</param>", rowVariableName), true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Name = string.Format("Add{0}Row", tableSchema.Name);
			this.Parameters.Add(new CodeParameterDeclarationExpression(rowTypeName, rowVariableName));
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement(string.Format("This adds a new row to the {0} table.", tableSchema.Name)));
			foreach (LockRequest lockRequest in tableLockList)
			{
				CodeExpression tableExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), lockRequest.TableSchema.Name);
				if (lockRequest is ReadRequest)
					tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
				if (lockRequest is WriteRequest)
					tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			}
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "Add", new CodeExpression[] { new CodeArgumentReferenceExpression(rowVariableName) }));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row is added."));
			foreach (LockRequest lockRequest in tableLockList)
			{
				CodeExpression tableExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), lockRequest.TableSchema.Name);
				if (lockRequest is ReadRequest)
					tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "ReleaseReaderLock"));
				if (lockRequest is WriteRequest)
					tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(tableExpression, "ReaderWriterLock"), "ReleaseWriterLock"));
			}
			this.Statements.Add(tryCatchFinallyStatement);

		}

	}

}
