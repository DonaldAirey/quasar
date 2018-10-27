namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class CountProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public CountProperty(TableSchema tableSchema)
		{

			//			/// <summary>
			//			/// Gets the number of rows in the Department table.
			//			/// </summary>
			//			[System.ComponentModel.Browsable(false)]
			//			public int Count
			//			{
			//				get
			//				{
			//					try
			//					{
			//						// The table can't be modified while the number of rows is accessed.
			//						this.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//						return this.Rows.Count;
			//					}
			//					finally
			//					{
			//						// The table can be accessed by other threads once the number of rows is accessed.
			//						this.ReaderWriterLock.ReleaseReaderLock();
			//					}
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the number of rows in the {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(typeof(System.Int32));
			this.Name = "Count";
			CodeTryCatchFinallyStatement tryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			tryCatchFinallyStatement.TryStatements.Add(new CodeCommentStatement("The table can't be modified while the number of rows is accessed."));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "Count")));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeCommentStatement("The table can be accessed by other threads once the number of rows is returned to the caller."));
			tryCatchFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "ReleaseReaderLock"));
			this.GetStatements.Add(tryCatchFinallyStatement);


		}

	}

}
