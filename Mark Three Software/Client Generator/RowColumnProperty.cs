namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets or sets column data of a row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowColumnProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get or set the column data of a row.
		/// </summary>
		/// <param name="tableSchema">The table to which this row belongs.</param>
		/// <param name="columnSchema">The nullable column.</param>
		public RowColumnProperty(TableSchema tableSchema, ColumnSchema columnSchema)
		{

			//			/// <summary>
			//			/// Gets or sets the data in the DepartmentId column.
			//			/// </summary>
			//			public int DepartmentId
			//			{
			//				get
			//				{
			//					return ((int)(this[this.tableDepartment.DepartmentIdColumn]));
			//				}
			//				set
			//				{
			//					this[this.tableDepartment.DepartmentIdColumn] = value;
			//				}
			//			}
			Type columnType = columnSchema.DataType;
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets or sets the data in the {0} column.", columnSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(columnType);
			this.Name = columnSchema.Name;
			if (columnSchema.MinOccurs == 0)
			{
				CodeTryCatchFinallyStatement tryCatchBlock = new CodeTryCatchFinallyStatement();
				tryCatchBlock.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(columnType, new CodeArrayIndexerExpression(new CodeThisReferenceExpression(), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), string.Format("{0}Column", columnSchema.Name)) }))));
				CodeStatement[] catchStatements = new CodeStatement[] { new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("StrongTypingException"), new CodeExpression[] { new CodePrimitiveExpression("Cannot get value because it is DBNull."), new CodeArgumentReferenceExpression("e") })) };
				tryCatchBlock.CatchClauses.Add(new CodeCatchClause("e", new CodeTypeReference("InvalidCastException"), catchStatements));
				this.GetStatements.Add(tryCatchBlock);
			}
			else
				this.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(columnType, new CodeArrayIndexerExpression(new CodeThisReferenceExpression(), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), string.Format("{0}Column", columnSchema.Name)) }))));
			this.SetStatements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeThisReferenceExpression(), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), string.Format("{0}Column", columnSchema.Name)) }), new CodePropertySetValueReferenceExpression()));

		}

	}

}
