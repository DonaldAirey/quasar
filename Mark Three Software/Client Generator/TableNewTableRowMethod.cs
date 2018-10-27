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
	class TableNewTableRowMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public TableNewTableRowMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Creates a new Department row with the same schema as the table.
			//			/// </summary>
			//			/// <returns>A new row with the same schema as the table.</returns>
			//			public DepartmentRow NewDepartmentRow()
			//			{
			//				return ((DepartmentRow)(this.NewRow()));
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Creates a new {0} row with the same schema as the table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<returns>A new row with the same schema as the table.</returns>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.ReturnType = new CodeTypeReference(rowTypeName);
			this.Name = string.Format("New{0}", rowTypeName);
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement(string.Format("This creates a new {0} row once the table is locked.", tableSchema.Name)));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(rowTypeName, new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "NewRow", new CodeExpression[] { }))));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row is created."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseWriterLock"));
			this.Statements.Add(tryCatchFinallyStatement);

		}

	}

}
