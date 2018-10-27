namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class TableRowChangingMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public TableRowChangingMethod(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Handles a deleted row from the server data model.
			//		/// </summary>
			//		/// <param name="sender">The object that originated the event.</param>
			//		/// <param name="e">The event arguments.</param>
			//		static private void TableRowChanging(object sender, DataRowChangeEventArgs e)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Handles a deleted row from the server data model.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<param name=\"sender\">The object that originated the event.</param>", true));
			this.Comments.Add(new CodeCommentStatement("<param name=\"e\">The event arguments.</param>", true));
			this.Name = "TableRowChanging";
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.Object), "sender"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.Data.DataRowChangeEventArgs), "e"));
			
			//			// IMPORTANT CONCEPT: Deleted rows need to be reconciled to the client data model.  Once they have been deleted from
			//			// the active data model they are placed in a temporary holding DataSet while each of the clients has a chance to get a
			//			// copy of the deleted record.  Only the primary key is required to delete the record on the client, so the primary key
			//			// is extracted from the deleted record before it is purged and is stored along with the row version and a timestamp.
			//			if (e.Action == DataRowAction.Commit && e.Row.RowState == DataRowState.Deleted)
			//			{
			this.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: Deleted rows need to be reconciled to the client data model.  Once they have been deleted from"));
			this.Statements.Add(new CodeCommentStatement("the active data model they are placed in a temporary holding DataSet while each of the clients has a chance to get a"));
			this.Statements.Add(new CodeCommentStatement("copy of the deleted record.  Only the primary key is required to delete the record on the client, so the primary key"));
			this.Statements.Add(new CodeCommentStatement("is extracted from the deleted record before it is purged and is stored along with the row version and a timestamp."));
			CodeConditionStatement ifDeleting = new CodeConditionStatement();
			ifDeleting.Condition = new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("e"), "Action"), CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowAction)), "Commit")), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("e"), "Row"), "RowState"), CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowState)), "Deleted")));

			//				try
			//				{
			CodeTryCatchFinallyStatement tryMutex = new CodeTryCatchFinallyStatement();

			//					// The data in the deleted data model needs to be protected from the reconcillation threads and the background 
			//					// thread that cleand up the table of deleted rows that have become obsolete.
			//					DataModel.deletedExclusion.WaitOne();
			//					// These values are used to create a record in the deleted data model that is a copy of the primary key values
			//					// from the deleted row.
			//					Row sourceRow = e.Row as Row;
			//					Table sourceTable = sourceRow.Table;
			//					DataTable destinationTable = DataModel.deletedDataSet.Tables[sourceTable.TableName];
			//					DataRow destinationRow = destinationTable.NewRow();
			//					// This will create a generic row containing the row version, a time stamp that marks the time the deleted
			//					// record was created, and the primary key fields from the deleted record.  This is all that's needed to purge
			//					// the record from the client data model.
			//					destinationRow[DataModel.rowVersionColumn] = sourceRow.RowVersion;
			//					destinationRow[DataModel.deletedTimeColumn] = DateTime.Now;
			//					for (int primaryKeyIndex = 0; primaryKeyIndex < sourceTable.PrimaryKey.Length; primaryKeyIndex++)
			//						destinationRow[DataModel.primaryKeyOffset + primaryKeyIndex] =
			//							sourceRow[sourceTable.PrimaryKey[primaryKeyIndex], DataRowVersion.Original];
			//					destinationTable.Rows.Add(destinationRow);
			//					// This signals the garbage collector that it should wake up and scan the deleted data model for obsolete 
			//					// records.
			//					DataModel.deletedEvent.Set();
			//				}
			tryMutex.TryStatements.Add(new CodeCommentStatement("The data in the deleted data model needs to be protected from the reconcillation threads and the background"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("thread that cleand up the table of deleted rows that have become obsolete."));
			tryMutex.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "WaitOne"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("These values are used to create a record in the deleted data model that is a copy of the primary key values"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("from the deleted row."));
			tryMutex.TryStatements.Add(new CodeVariableDeclarationStatement("Row", "sourceRow", new CodeCastExpression(new CodeTypeReference("Row"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("e"), "Row"))));
			tryMutex.TryStatements.Add(new CodeVariableDeclarationStatement("Table", "sourceTable", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceRow"), "Table")));
			tryMutex.TryStatements.Add(new CodeVariableDeclarationStatement("DataTable", "destinationTable", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "TableName"))));
			tryMutex.TryStatements.Add(new CodeVariableDeclarationStatement("DataRow", "destinationRow", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("destinationTable"), "NewRow")));
			tryMutex.TryStatements.Add(new CodeCommentStatement("This will create a generic row containing the row version, a time stamp that marks the time the deleted"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("record was created, and the primary key fields from the deleted record.  This is all that's needed to purge"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("the record from the client data model."));
			tryMutex.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("destinationRow"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "rowVersionColumn")), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceRow"), "RowVersion")));
			tryMutex.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("destinationRow"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedTimeColumn")), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DateTime)), "Now")));
			CodeIterationStatement indexLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "primaryKeyIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("primaryKeyIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "PrimaryKey"), "Length")), new CodeAssignStatement(new CodeVariableReferenceExpression("primaryKeyIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("primaryKeyIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			indexLoop.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("destinationRow"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "primaryKeyOffset"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("primaryKeyIndex"))),
				new CodeIndexerExpression(new CodeVariableReferenceExpression("sourceRow"), new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "PrimaryKey"), new CodeVariableReferenceExpression("primaryKeyIndex")), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowVersion)), "Original"))));
			tryMutex.TryStatements.Add(indexLoop);
			tryMutex.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("destinationTable"), "Rows"), "Add", new CodeVariableReferenceExpression("destinationRow")));
			tryMutex.TryStatements.Add(new CodeCommentStatement("This signals the garbage collector that it should wake up and scan the deleted data model for obsolete"));
			tryMutex.TryStatements.Add(new CodeCommentStatement("records."));
			tryMutex.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedEvent"), "Set"));

			//				finally
			//				{
			//					// Allow the other threads to examine and modify the deleted data model.
			//					DataModel.deletedExclusion.ReleaseMutex();
			//				}
			tryMutex.FinallyStatements.Add(new CodeCommentStatement("Allow the other threads to examine and modify the deleted data model."));
			tryMutex.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "ReleaseMutex"));

			//			}
			ifDeleting.TrueStatements.Add(tryMutex);
			
			//		}
			this.Statements.Add(ifDeleting);

		}

	}
}
