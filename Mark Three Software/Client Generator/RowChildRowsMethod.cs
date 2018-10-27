namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a method that gets a list of the child rows.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowChildRowsMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates a method to get a list of child rows.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the child table.</param>
		public RowChildRowsMethod(KeyrefSchema keyrefSchema)
		{

			// These variables are used to construct the method.
			TableSchema childTable = keyrefSchema.Selector;
			TableSchema parentTable = keyrefSchema.Refer.Selector;
			string rowTypeName = string.Format("{0}Row", childTable.Name);
			string tableFieldName = string.Format("table{0}", parentTable.Name);
			string relationName = string.Format("{0}{1}Relation", parentTable.Name, childTable.Name);
			string childRowTypeName = string.Format("{0}Row", keyrefSchema.Selector.Name);

			// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
			// all the table locks that are used for this operation and organizes them in a list that is used to generate the
			// locking and releasing statements below.
			List<LockRequest> tableLockList = new List<LockRequest>();
			tableLockList.Add(new ReadRequest(childTable));
			tableLockList.Add(new ReadRequest(parentTable));
			tableLockList.Sort();

			// If the foreign keys share the same primary key, then the names of the methods will need to be decorated with the
			// key name in order to make them unique.  This will test the foreign keys for duplicate primary key names.
			bool isDuplicateKey = false;
			foreach (KeyrefSchema otherforeignKey in childTable.ChildKeyrefs)
				if (otherforeignKey != keyrefSchema && otherforeignKey.Selector == keyrefSchema.Selector)
					isDuplicateKey = true;

			//			/// <summary>
			//			/// Gets the children rows in the Engineer table.
			//			/// </summary>
			//			public EngineerRow[] GetEngineerRows()
			//			{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the children rows in the {0} table.", keyrefSchema.Selector.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.ReturnType = new CodeTypeReference(childRowTypeName, 1);
			this.Name = isDuplicateKey ? string.Format("Get{0}sBy{1}", childRowTypeName, keyrefSchema.Name) : string.Format("Get{0}s", childRowTypeName);

			//				if (this.ReaderWriterLock.IsReaderLockHeld == false && this.ReaderWriterLock.IsWriterLockHeld == false)
			//					throw new LockException("An attempt was made to access an Employee Row without a lock");
			this.Statements.Add(new CodeCommentStatement("This insures the row is locked before attempting to create a list of children."));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "IsReaderLockHeld"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false)), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "IsWriterLockHeld"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false))),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("MarkThree.LockException"), new CodePrimitiveExpression(string.Format("An attempt was made to access a {0} row without a lock", parentTable.Name))))));

			//				try
			//				{
			//					// The child table must be locked to insure it doesn't change while the relation is used to create a list of 
			//					// all the child rows of this row.
			//					this.tableEmployee.EmployeeEngineerRelation.ChildTable.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//					return ((EngineerRow[])(this.GetChildRows(this.tableEmployee.EmployeeEngineerRelation)));
			//				}
			//				finally
			//				{
			//					// The child table can be released once the list is built.
			//					this.tableEmployee.EmployeeEngineerRelation.ChildTable.ReaderWriterLock.ReleaseReaderLock();
			//				}
			//			}
			CodeTryCatchFinallyStatement getTryStatement = new CodeTryCatchFinallyStatement();
			CodeExpression parentRelationExpression = new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), tableFieldName), relationName);
			getTryStatement.TryStatements.Add(new CodeCommentStatement("The child table must be locked to insure it doesn't change while the relation is used to create a list of"));
			getTryStatement.TryStatements.Add(new CodeCommentStatement("all the child rows of this row."));
			foreach (LockRequest lockRequest in tableLockList)
				getTryStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(keyrefSchema.Selector.DataModelSchema.Name), lockRequest.TableSchema.Name), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			getTryStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference(childRowTypeName, 1), new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "GetChildRows", new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), tableFieldName), string.Format("{0}{1}Relation", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name))))));
			getTryStatement.FinallyStatements.Add(new CodeCommentStatement("The child table can be released once the list is built."));
			foreach (LockRequest lockRequest in tableLockList)
				getTryStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(keyrefSchema.Selector.DataModelSchema.Name), lockRequest.TableSchema.Name), "ReaderWriterLock"), "ReleaseReaderLock"));
			this.Statements.Add(getTryStatement);

		}

	}

}
