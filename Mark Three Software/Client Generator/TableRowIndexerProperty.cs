namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableRowIndexerProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableRowIndexerProperty(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Indexer to a row in the Department table.
			//			/// </summary>
			//			/// <param name="index">The integer index of the row.</param>
			//			/// <returns>The Department row found at the given index.</returns>
			//			public DepartmentRow this[int index]
			//			{
			//				get
			//				{
			//					try
			//					{
			//						// The table can't be modified while the row is accessed.
			//						this.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//						return ((DepartmentRow)(this.Rows[index]));
			//					}
			//					finally
			//					{
			//						// The table can be accessed by other threads once the row is returned to the caller.
			//						this.ReaderWriterLock.ReleaseReaderLock();
			//					}
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Indexer to a row in the {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<param name=\"index\">The integer index of the row.</param>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<returns>The {0} row found at the given index.</returns>", tableSchema.Name), true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(rowTypeName);
			this.Name = "Item";
			this.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(System.Int32)), "index"));
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement("The table can't be modified while the row is accessed."));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference(String.Format("{0}Row", tableSchema.Name)), new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), new CodeExpression[] { new CodeArgumentReferenceExpression("index") }))));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the row is returned to the caller."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseReaderLock"));
			this.GetStatements.Add(tryCatchFinallyStatement);

		}

	}

}
