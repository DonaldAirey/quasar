namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class RowParentRowProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public RowParentRowProperty(KeyrefSchema keyrefSchema)
		{

			// These constructs are used several times to generate the property.
			TableSchema childTable = keyrefSchema.Selector;
			TableSchema parentTable = keyrefSchema.Refer.Selector;
			string rowTypeName = string.Format("{0}Row", parentTable.Name);
			string tableFieldName = string.Format("table{0}", childTable.Name);
			string relationName = string.Format("{0}{1}Relation", parentTable.Name, childTable.Name);

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
			foreach (KeyrefSchema otherKeyrefSchema in childTable.MemberParentKeys)
				if (otherKeyrefSchema != keyrefSchema && otherKeyrefSchema.Refer.Name == keyrefSchema.Refer.Name)
					isDuplicateKey = true;

			//			/// <summary>
			//			/// Gets the parent row in the Race table.
			//			/// </summary>
			//			public RaceRow RaceRow
			//			{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the parent row in the {0} table.", keyrefSchema.Refer.Selector.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(rowTypeName);
			this.Name = isDuplicateKey ? string.Format("{0}By{1}", rowTypeName, keyrefSchema.Name) : rowTypeName;

			//					if (this.ReaderWriterLock.IsReaderLockHeld == false && this.ReaderWriterLock.IsWriterLockHeld == false)
			//						throw new LockException("An attempt was made to access an Employee Row without a lock");
			this.GetStatements.Add(new CodeCommentStatement("This insures the row is locked before attempting to access the parent row."));
			this.GetStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "IsReaderLockHeld"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false)), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ReaderWriterLock"), "IsWriterLockHeld"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false))),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("MarkThree.LockException"), new CodePrimitiveExpression(string.Format("Attempt was made to access a row in {0} without a lock.", childTable.Name))))));

			//				get
			//				{
			CodeTryCatchFinallyStatement getTryStatement = new CodeTryCatchFinallyStatement();

			//					try
			//					{
			//						// The parent table must be locked before attempting to access the parent row.
			//						this.tableEmployee.RaceEmployeeRelation.ParentTable.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//						return ((RaceRow)(this.GetParentRow(this.tableEmployee.RaceEmployeeRelation)));
			//					}
			//					finally
			//					{
			//						// The parent table can be released once the parent row is found.
			//						this.tableEmployee.RaceEmployeeRelation.ParentTable.ReaderWriterLock.ReleaseReaderLock();
			//					}
			getTryStatement.TryStatements.Add(new CodeCommentStatement("The parent table must be locked to insure it doesn't change before attempting to access the parent row."));
			CodeExpression parentRelationExpression = new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), tableFieldName), relationName);
			foreach (LockRequest lockRequest in tableLockList)
				getTryStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(keyrefSchema.Selector.DataModelSchema.Name), lockRequest.TableSchema.Name), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			getTryStatement.TryStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(rowTypeName, new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "GetParentRow", new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), tableFieldName), string.Format("{0}{1}Relation", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name))))));
			getTryStatement.FinallyStatements.Add(new CodeCommentStatement("The parent table can be released once the parent row is found."));
			foreach (LockRequest lockRequest in tableLockList)
				getTryStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(keyrefSchema.Selector.DataModelSchema.Name), lockRequest.TableSchema.Name), "ReaderWriterLock"), "ReleaseReaderLock"));

			//				}
			this.GetStatements.Add(getTryStatement);

			//			}

		}

	}

}
