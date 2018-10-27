namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class ReconcileMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public ReconcileMethod(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Collects the set of modified records that will reconcile the client data model to the master data model.
			//		/// </summary>
			//		/// <param name="clientGap">A list of missing row versions on the client data model.</param>
			//		/// <returns>A set of records that can be merged with the client data model.</returns>
			//		public static ArrayList Reconcile(long[][] clientGap)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Collects the set of modified records that will reconcile the client data model to the master data model.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<param name=\"clientGap\">A list of missing row versions on the client data model.</param>", true));
			this.Comments.Add(new CodeCommentStatement("<returns>A set of records that can be merged with the client data model.</returns>", true));
			this.Name = "Reconcile";
			this.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			this.ReturnType = new CodeTypeReference(typeof(System.Collections.ArrayList));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(long[][]), "clientGap"));

			//			// IMPORTANT CONCEPT: The data returned to the client is the set of rows that have been modified since the last call to
			//			// the 'reconcile' method.  The new data destined for the client is arranged in generic lists and arrays. This should
			//			// be done with structures, but the transmission of the type details of a structure over the network takes a measurable
			//			// amount of time.  This generic data structure of base types has the advantage of requiring less bandwidth to
			//			// serialize.  The big picture is that the data is passed back as an ArrayList containing essentially the table
			//			// information and a list of inserted, updated and deleted rows associated with each table structure.
			//			ArrayList reconciledData = null;
			this.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: The data returned to the client is the set of rows that have been modified since the last call to"));
			this.Statements.Add(new CodeCommentStatement("the 'reconcile' method.  The new data destined for the client is arranged in generic lists and arrays. This should"));
			this.Statements.Add(new CodeCommentStatement("be done with structures, but the transmission of the type details of a structure over the network takes a measurable"));
			this.Statements.Add(new CodeCommentStatement("amount of time.  This generic data structure of base types has the advantage of requiring less bandwidth to"));
			this.Statements.Add(new CodeCommentStatement("serialize.  The big picture is that the data is passed back as an ArrayList containing essentially the table"));
			this.Statements.Add(new CodeCommentStatement("information and a list of inserted, updated and deleted rows associated with each table structure."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Collections.ArrayList), "reconciledData", new CodePrimitiveExpression(null)));

			//			// Scan through the tables looking for any row version that falls within the gaps on the client data model.  When the
			//			// client merges this 'incremental' data structure with the existing set, the server and client will be synchronized.
			//			// The first pass will examine only the active table for modified records.
			//			for (int tableIndex = 0; (tableIndex < DataModel.Tables.Count); tableIndex = (tableIndex + 1))
			//			{
			this.Statements.Add(new CodeCommentStatement("Scan through the tables looking for any row version that falls within the gaps on the client data model.  When the"));
			this.Statements.Add(new CodeCommentStatement("client merges this 'incremental' data structure with the existing set, the server and client will be synchronized."));
			this.Statements.Add(new CodeCommentStatement("The first pass will examine only the active table for modified records."));
			CodeIterationStatement tableLoop0 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//				// Select the next table from the list.
			//				Table sourceTable = DataModel.Tables[tableIndex];
			tableLoop0.Statements.Add(new CodeCommentStatement("Select the next table from the list."));
			tableLoop0.Statements.Add(new CodeVariableDeclarationStatement("Table", "sourceTable", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));

			//				// An array of target rows is only created when the logic below has found at least one row that needs to be 
			//				// returned to the client.  This is an optimization to prevent a massive number of small allocations that need to
			//				// be reclaimed after the reconcilliation.
			//				ArrayList targetRows = null;
			//				try
			//				{
			tableLoop0.Statements.Add(new CodeCommentStatement("An array of target rows is only created when the logic below has found at least one row that needs to be "));
			tableLoop0.Statements.Add(new CodeCommentStatement("returned to the client.  This is an optimization to prevent a massive number of small allocations that need to"));
			tableLoop0.Statements.Add(new CodeCommentStatement("be reclaimed after the reconcilliation."));
			tableLoop0.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Collections.ArrayList), "targetRows", new CodePrimitiveExpression(null)));
			CodeTryCatchFinallyStatement tryScanTable0 = new CodeTryCatchFinallyStatement();

			//					// IMPORTANT CONCEPT: Locking the table will prevent the default view from changing while the table is examined
			//					// for records that are needed to reconcile the client data model with the server.  The default view filters
			//					// everything but the original values.  The 'AcceptChanges' method requires a table lock, and the
			//					// 'AcceptChanges' is the only method that can change the original values of the row.  Therefore, iterating
			//					// through the default view is safe as long as the table is locked.
			//					sourceTable.ReaderWriterLock.AcquireReaderLock(Timeout.Infinite);
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: Locking the table will prevent the default view from changing while the table is examined"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("for records that are needed to reconcile the client data model with the server.  The default view filters"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("everything but the original values.  The 'AcceptChanges' method requires a table lock, and the"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("'AcceptChanges' is the only method that can change the original values of the row.  Therefore, iterating"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("through the default view is safe as long as the table is locked."));
			tryScanTable0.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));

			//					// The list of gaps from the client is examined for missing row versions.  Since the list of gaps is ordered by
			//					// row version and the default view is ordered by row version, there is no need to examine the whole list of
			//					// gaps each time a new row is read from the default view.  This index will step through the list of gaps and
			//					// eliminate the higher ranges as the row versions are counted down.
			//					int gapIndex = clientGap.Length - 1;
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("The list of gaps from the client is examined for missing row versions.  Since the list of gaps is ordered by"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("row version and the default view is ordered by row version, there is no need to examine the whole list of"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("gaps each time a new row is read from the default view.  This index will step through the list of gaps and"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("eliminate the higher ranges as the row versions are counted down."));
			tryScanTable0.TryStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Int32), "gapIndex", new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("clientGap"), "Length"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));

			//					// The default view has the rows of the table ordered by the 'RowVersion' column.  This means basically that
			//					// the most recently modified records will be at the start of this view, the oldest ones at the end of the
			//					// view.  This loop will examine the rows in the order they were modified and send any rows back that fall in
			//					// the array of row version gaps sent by the client.
			//					for (int viewIndex = 0; (viewIndex < sourceTable.DefaultView.Count); viewIndex = (viewIndex + 1))
			//					{
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("The default view has the rows of the table ordered by the 'RowVersion' column.  This means basically that"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("the most recently modified records will be at the start of this view, the oldest ones at the end of the"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("view.  This loop will examine the rows in the order they were modified and send any rows back that fall in"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("the array of row version gaps sent by the client."));
			CodeIterationStatement viewLoop0 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "viewIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("viewIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "DefaultView"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("viewIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("viewIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//						// Extract the next row of data from the view.
			//						System.Data.DataRowView dataRowView = sourceTable.DefaultView[viewIndex];
			//						Row sourceRow = (Row)dataRowView.Row;
			viewLoop0.Statements.Add(new CodeCommentStatement("Extract the next row of data from the view."));
			viewLoop0.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataRowView), "dataRowView", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "DefaultView"), new CodeVariableReferenceExpression("viewIndex"))));
			viewLoop0.Statements.Add(new CodeVariableDeclarationStatement("Row", "sourceRow", new CodeCastExpression("Row", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataRowView"), "Row"))));

			//						// IMPORTANT CONCEPT:  This record's row version will determine whether it is returned to the client or 
			//						// not.  Note that only committed values are examined and returned to the client.  This leaves the
			//						// 'current' version of the row free for modifications by the transaction logic.  The 'original' version of
			//						// the record can be read without having to lock the row.
			//						long sourceRowVersion = (long)sourceRow[sourceTable.RowVersionColumn, DataRowVersion.Original];
			viewLoop0.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT:  This record's row version will determine whether it is returned to the client or"));
			viewLoop0.Statements.Add(new CodeCommentStatement("not.  Note that only committed values are examined and returned to the client.  This leaves the"));
			viewLoop0.Statements.Add(new CodeCommentStatement("'current' version of the row free for modifications by the transaction logic.  The 'original' version of"));
			viewLoop0.Statements.Add(new CodeCommentStatement("the record can be read without having to lock the row."));
			viewLoop0.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Int64), "sourceRowVersion", new CodeCastExpression(typeof(System.Int64), new CodeIndexerExpression(new CodeVariableReferenceExpression("sourceRow"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "RowVersionColumn"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowVersion)), "Original")))));

			//						// IMPORTANT CONCEPT:  This is intended to lighten the load on the server by reducing the number of records
			//						// that need to be examined.  The list of gaps is organized from lowest to highest.  The default view is
			//						// organized from highest to lowest.  As row versions are read from the default view, gaps can be
			//						// eliminated if the row version is below the starting index of the gap.
			//						for (
			//						; (sourceRowVersion < clientGap[gapIndex][0]); gapIndex = (gapIndex - 1))
			//						{
			//							if (gapIndex == 0)
			//							{
			//								goto ViewExit0;
			//							}
			//						}
			viewLoop0.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT:  This is intended to lighten the load on the server by reducing the number of records"));
			viewLoop0.Statements.Add(new CodeCommentStatement("that need to be examined.  The list of gaps is organized from lowest to highest.  The default view is"));
			viewLoop0.Statements.Add(new CodeCommentStatement("organized from highest to lowest.  As row versions are read from the default view, gaps can be"));
			viewLoop0.Statements.Add(new CodeCommentStatement("eliminated if the row version is below the starting index of the gap."));
			CodeIterationStatement whileGap0 = new CodeIterationStatement(new CodeSnippetStatement(), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("sourceRowVersion"), CodeBinaryOperatorType.LessThan, new CodeIndexerExpression(new CodeIndexerExpression(new CodeArgumentReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(0))), new CodeAssignStatement(new CodeVariableReferenceExpression("gapIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("gapIndex"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));
			whileGap0.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("gapIndex"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)),
				new CodeGotoStatement("ViewExit0")));
			viewLoop0.Statements.Add(whileGap0);

			//						// A row is returned to the client only if the version of the record read from the default view falls
			//						// within the gap calculated by the client.
			//						if (clientGap[gapIndex][0] < sourceRowVersion && sourceRowVersion < clientGap[gapIndex][1])
			//						{
			viewLoop0.Statements.Add(new CodeCommentStatement("A row is returned to the client only if the version of the record read from the default view falls"));
			viewLoop0.Statements.Add(new CodeCommentStatement("within the gap calculated by the client."));
			CodeConditionStatement ifInGap0 = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(0)), CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression("sourceRowVersion")), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("sourceRowVersion"), CodeBinaryOperatorType.LessThan, new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(1)))));

			//							// The bucket to hold the rows isn't created until at least one row has been discovered that needs to
			//							// be returned to the client.  This is to keep the size of the returned data structure to a minimum
			//							// when transmitting the incremental records.
			//							if (targetRows == null)
			//							{
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("The bucket to hold the rows isn't created until at least one row has been discovered that needs to"));
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("be returned to the client.  This is to keep the size of the returned data structure to a minimum"));
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("when transmitting the incremental records."));
			CodeConditionStatement ifTargetRows0 = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("targetRows"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)));

			//								// Like the bucket to hold the rows, the bucket that holds the tables -- the 'reconciledData'
			//								// variable -- isn't created until there is at least one table to be returned to the client.
			//								if (reconciledData == null)
			//									reconciledData = new ArrayList();
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("Like the bucket to hold the rows, the bucket that holds the tables -- the 'reconciledData'"));
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("variable -- isn't created until there is at least one table to be returned to the client."));
			ifTargetRows0.TrueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("reconciledData"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
				new CodeAssignStatement(new CodeVariableReferenceExpression("reconciledData"), new CodeObjectCreateExpression(typeof(System.Collections.ArrayList)))));

			//								// The table-level record holds the name of the table, which will be used to look up the table on
			//								// the client from the DataSet's "Table" member.  It also holds a list that contains all the
			//								// records in that table.  There is one of these records for every table that has updated data that
			//								// needs to be transmitted back to the client.
			//								object[] targetTable = new object[2];
			//								targetTable[0] = sourceTable.TableName;
			//								targetTable[1] = targetRows = new ArrayList();
			//								reconciledData.Add(targetTable);
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("The table-level record holds the name of the table, which will be used to look up the table on"));
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("the client from the DataSet's \"Table\" member.  It also holds a list that contains all the"));
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("records in that table.  There is one of these records for every table that has updated data that"));
			ifTargetRows0.TrueStatements.Add(new CodeCommentStatement("needs to be transmitted back to the client."));
			ifTargetRows0.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Object[]), "targetTable", new CodeArrayCreateExpression(typeof(System.Object[]), 2)));
			ifTargetRows0.TrueStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("targetTable"), new CodePrimitiveExpression(0)), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "TableName")));
			ifTargetRows0.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("targetRows"), new CodeObjectCreateExpression(typeof(System.Collections.ArrayList))));
			ifTargetRows0.TrueStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("targetTable"), new CodePrimitiveExpression(1)), new CodeVariableReferenceExpression("targetRows")));
			ifTargetRows0.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("reconciledData"), "Add", new CodeVariableReferenceExpression("targetTable")));

			//							}
			ifInGap0.TrueStatements.Add(ifTargetRows0);

			//							// This will pass the data back to the client.  Deleted rows only need to include the primary key
			//							// elements.  Added and modified rows pass back the entire record.  Note again that only the committed
			//							// data is passed back to the client.  This obviates the need to lock the record before reading it.  A
			//							// table lock is enough to insure that the data isn't modified while it is placed in the buffer.
			//							object[] data = new object[sourceTable.Columns.Count];
			//							for (int columnIndex = 0; columnIndex < sourceTable.Columns.Count; columnIndex = (columnIndex + 1))
			//							{
			//								Column column = sourceTable.Columns[columnIndex];
			//								data[column.Ordinal] = sourceRow[column, DataRowVersion.Original];
			//							}
			//							targetRows.Add(new object[2] { DataRowState.Unchanged, data });
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("This will pass the data back to the client.  Deleted rows only need to include the primary key"));
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("elements.  Added and modified rows pass back the entire record.  Note again that only the committed"));
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("data is passed back to the client.  This obviates the need to lock the record before reading it.  A"));
			ifInGap0.TrueStatements.Add(new CodeCommentStatement("table lock is enough to insure that the data isn't modified while it is placed in the buffer."));
			ifInGap0.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Object[]), "data", new CodeArrayCreateExpression(typeof(System.Object), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "Columns"), "Count"))));
			CodeIterationStatement columnLoop0 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "columnIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "Columns"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("columnIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			columnLoop0.Statements.Add(new CodeVariableDeclarationStatement("Column", "column", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "Columns"), new CodeVariableReferenceExpression("columnIndex"))));
			columnLoop0.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("data"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "Ordinal")), new CodeIndexerExpression(new CodeVariableReferenceExpression("sourceRow"), new CodeVariableReferenceExpression("column"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowVersion)), "Original"))));
			ifInGap0.TrueStatements.Add(columnLoop0);
			ifInGap0.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("targetRows"), "Add", new CodeArrayCreateExpression(typeof(System.Object), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowState)), "Unchanged"), new CodeVariableReferenceExpression("data"))));
			
			//						}
			viewLoop0.Statements.Add(ifInGap0);

			//					}
			tryScanTable0.TryStatements.Add(viewLoop0);

			//					// This statement allows the loop above to exit when there are no more gaps to be evaluated for the current 
			//					// table.  There is no purpose to setting the 'gapIndex' here, but the CodeDOM can't generate a 'break'
			//					// statement or an empty statement.  This makes for some pretty ugly code.
			//				ViewExit0:
			//					gapIndex = 0;
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("This statement allows the loop above to exit when there are no more gaps to be evaluated for the current "));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("table.  There is no purpose to setting the 'gapIndex' here, but the CodeDOM can't generate a 'break'"));
			tryScanTable0.TryStatements.Add(new CodeCommentStatement("statement or an empty statement.  This makes for some pretty ugly code."));
			tryScanTable0.TryStatements.Add(new CodeLabeledStatement("ViewExit0", new CodeAssignStatement(new CodeVariableReferenceExpression("gapIndex"), new CodePrimitiveExpression(0))));

			//				finally
			//				{
			//					// This table doesn't need to be locked any longer.
			//					sourceTable.ReaderWriterLock.ReleaseReaderLock();
			//				}
			tryScanTable0.FinallyStatements.Add(new CodeCommentStatement("This table doesn't need to be locked any longer."));
			tryScanTable0.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "ReaderWriterLock"), "ReleaseReaderLock"));
			
			//				}
			tableLoop0.Statements.Add(tryScanTable0);

			//			}
			this.Statements.Add(tableLoop0);

			//			// The second pass of the reconciliation operation returns the deleted rows.
			//			for (int tableIndex = 0; (tableIndex < DataModel.deletedDataSet.Tables.Count; tableIndex = (tableIndex + 1))
			//			{
			this.Statements.Add(new CodeCommentStatement("The second pass of the reconciliation operation returns the deleted rows."));
			CodeIterationStatement tableLoop1 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//				// Select the next table of deleted records from the list.
			//				DataTable sourceTable = DataModel.deletedDataSet.Tables[tableIndex];
			tableLoop1.Statements.Add(new CodeCommentStatement("Select the next table of deleted records from the list."));
			tableLoop1.Statements.Add(new CodeVariableDeclarationStatement("DataTable", "sourceTable", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));

			//				// An array of target rows is only created when the logic below has found at least one row that needs to be 
			//				// returned to the client.  This is an optimization to prevent a massive number of small allocations that need to
			//				// be reclaimed after the reconcilliation.
			//				ArrayList targetRows = null;
			//				try
			//				{
			tableLoop1.Statements.Add(new CodeCommentStatement("An array of target rows is only created when the logic below has found at least one row that needs to be "));
			tableLoop1.Statements.Add(new CodeCommentStatement("returned to the client.  This is an optimization to prevent a massive number of small allocations that need to"));
			tableLoop1.Statements.Add(new CodeCommentStatement("be reclaimed after the reconcilliation."));
			tableLoop1.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Collections.ArrayList), "targetRows", new CodePrimitiveExpression(null)));
			CodeTryCatchFinallyStatement tryScanTable1 = new CodeTryCatchFinallyStatement();

			//					// Wait for exclusive control over the deleted data model.
			//					DataModel.deletedExclusion.WaitOne();
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("Wait for exclusive control over the deleted data model."));
			tryScanTable1.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "WaitOne"));

			//					// The list of gaps from the client is examined for missing row versions.  Since the list of gaps is ordered by
			//					// row version and the default view is ordered by row version, there is no need to examine the whole list of
			//					// gaps each time a new row is read from the default view.  This index will step through the list of gaps and
			//					// eliminate the higher ranges as the row versions are counted down.
			//					int gapIndex = clientGap.Length - 1;
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("The list of gaps from the client is examined for missing row versions.  Since the list of gaps is ordered by"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("row version and the default view is ordered by row version, there is no need to examine the whole list of"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("gaps each time a new row is read from the default view.  This index will step through the list of gaps and"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("eliminate the higher ranges as the row versions are counted down."));
			tryScanTable1.TryStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Int32), "gapIndex", new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("clientGap"), "Length"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));

			//					// The default view has the rows of the table ordered by the 'RowVersion' column.  This means basically that
			//					// the most recently modified records will be at the start of this view, the oldest ones at the end of the
			//					// view.  This loop will examine the rows in the order they were modified and send any rows back that fall in
			//					// the array of row version gaps sent by the client.
			//					for (int viewIndex = 0; (viewIndex < sourceTable.DefaultView.Count); viewIndex = (viewIndex + 1))
			//					{
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("The default view has the rows of the table ordered by the 'RowVersion' column.  This means basically that"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("the most recently modified records will be at the start of this view, the oldest ones at the end of the"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("view.  This loop will examine the rows in the order they were modified and send any rows back that fall in"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("the array of row version gaps sent by the client."));
			CodeIterationStatement viewLoop1 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "viewIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("viewIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "DefaultView"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("viewIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("viewIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//						// Extract the next row of data from the view.
			//						System.Data.DataRowView dataRowView = sourceTable.DefaultView[viewIndex];
			//						DataRow sourceRow = dataRowView.Row;
			viewLoop1.Statements.Add(new CodeCommentStatement("Extract the next row of data from the view."));
			viewLoop1.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataRowView), "dataRowView", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "DefaultView"), new CodeVariableReferenceExpression("viewIndex"))));
			viewLoop1.Statements.Add(new CodeVariableDeclarationStatement("DataRow", "sourceRow", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataRowView"), "Row")));

			//						// IMPORTANT CONCEPT:  This record's row version will determine whether it is returned to the client or 
			//						// not.  Note that only committed values are examined and returned to the client.  This leaves the
			//						// 'current' version of the row free for modifications by the transaction logic.  The 'original' version of
			//						// the record can be read without having to lock the row.
			//						long sourceRowVersion = (long)sourceRow[sourceTable.RowVersionColumn, DataRowVersion.Original];
			viewLoop1.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT:  This record's row version will determine whether it is returned to the client or"));
			viewLoop1.Statements.Add(new CodeCommentStatement("not.  Note that only committed values are examined and returned to the client.  This leaves the"));
			viewLoop1.Statements.Add(new CodeCommentStatement("'current' version of the row free for modifications by the transaction logic.  The 'original' version of"));
			viewLoop1.Statements.Add(new CodeCommentStatement("the record can be read without having to lock the row."));
			viewLoop1.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Int64), "sourceRowVersion", new CodeCastExpression(typeof(System.Int64), new CodeIndexerExpression(new CodeVariableReferenceExpression("sourceRow"), new CodePrimitiveExpression(0)))));

			//						// IMPORTANT CONCEPT:  This is intended to lighten the load on the server by reducing the number of records
			//						// that need to be examined.  The list of gaps is organized from lowest to highest.  The default view is
			//						// organized from highest to lowest.  As row versions are read from the default view, gaps can be
			//						// eliminated if the row version is below the starting index of the gap.
			//						for (
			//						; (sourceRowVersion < clientGap[gapIndex][0]); gapIndex = (gapIndex - 1))
			//						{
			//							if (gapIndex == 0)
			//							{
			//								goto ViewExit0;
			//							}
			//						}
			viewLoop1.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT:  This is intended to lighten the load on the server by reducing the number of records"));
			viewLoop1.Statements.Add(new CodeCommentStatement("that need to be examined.  The list of gaps is organized from lowest to highest.  The default view is"));
			viewLoop1.Statements.Add(new CodeCommentStatement("organized from highest to lowest.  As row versions are read from the default view, gaps can be"));
			viewLoop1.Statements.Add(new CodeCommentStatement("eliminated if the row version is below the starting index of the gap."));
			CodeIterationStatement whileGap1 = new CodeIterationStatement(new CodeSnippetStatement(), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("sourceRowVersion"), CodeBinaryOperatorType.LessThan, new CodeIndexerExpression(new CodeIndexerExpression(new CodeArgumentReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(0))), new CodeAssignStatement(new CodeVariableReferenceExpression("gapIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("gapIndex"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))));
			whileGap1.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("gapIndex"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(0)),
				new CodeGotoStatement("ViewExit1")));
			viewLoop1.Statements.Add(whileGap1);

			//						// A row is returned to the client only if the version of the record read from the default view falls
			//						// within the gap calculated by the client.
			//						if (clientGap[gapIndex][0] < sourceRowVersion && sourceRowVersion < clientGap[gapIndex][1])
			//						{
			viewLoop1.Statements.Add(new CodeCommentStatement("A row is returned to the client only if the version of the record read from the default view falls"));
			viewLoop1.Statements.Add(new CodeCommentStatement("within the gap calculated by the client."));
			CodeConditionStatement ifInGap1 = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(0)), CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression("sourceRowVersion")), CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("sourceRowVersion"), CodeBinaryOperatorType.LessThan, new CodeIndexerExpression(new CodeIndexerExpression(new CodeVariableReferenceExpression("clientGap"), new CodeVariableReferenceExpression("gapIndex")), new CodePrimitiveExpression(1)))));

			//							// The bucket to hold the rows isn't created until at least one row has been discovered that needs to
			//							// be returned to the client.  This is to keep the size of the returned data structure to a minimum
			//							// when transmitting the incremental records.
			//							if (targetRows == null)
			//							{
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("The bucket to hold the rows isn't created until at least one row has been discovered that needs to"));
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("be returned to the client.  This is to keep the size of the returned data structure to a minimum"));
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("when transmitting the incremental records."));
			CodeConditionStatement ifTargetRows1 = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("targetRows"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)));

			//								// Like the bucket to hold the rows, the bucket that holds the tables -- the 'reconciledData'
			//								// variable -- isn't created until there is at least one table to be returned to the client.
			//								if (reconciledData == null)
			//									reconciledData = new ArrayList();
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("Like the bucket to hold the rows, the bucket that holds the tables -- the 'reconciledData'"));
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("variable -- isn't created until there is at least one table to be returned to the client."));
			ifTargetRows1.TrueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("reconciledData"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
				new CodeAssignStatement(new CodeVariableReferenceExpression("reconciledData"), new CodeObjectCreateExpression(typeof(System.Collections.ArrayList)))));

			//								// The table-level record holds the name of the table, which will be used to look up the table on
			//								// the client from the DataSet's "Table" member.  It also holds a list that contains all the
			//								// records in that table.  There is one of these records for every table that has updated data that
			//								// needs to be transmitted back to the client.
			//								object[] targetTable = new object[2];
			//								targetTable[0] = sourceTable.TableName;
			//								targetTable[1] = targetRows = new ArrayList();
			//								reconciledData.Add(targetTable);
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("The table-level record holds the name of the table, which will be used to look up the table on"));
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("the client from the DataSet's \"Table\" member.  It also holds a list that contains all the"));
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("records in that table.  There is one of these records for every table that has updated data that"));
			ifTargetRows1.TrueStatements.Add(new CodeCommentStatement("needs to be transmitted back to the client."));
			ifTargetRows1.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Object[]), "targetTable", new CodeArrayCreateExpression(typeof(System.Object[]), 2)));
			ifTargetRows1.TrueStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("targetTable"), new CodePrimitiveExpression(0)), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceTable"), "TableName")));
			ifTargetRows1.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("targetRows"), new CodeObjectCreateExpression(typeof(System.Collections.ArrayList))));
			ifTargetRows1.TrueStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("targetTable"), new CodePrimitiveExpression(1)), new CodeVariableReferenceExpression("targetRows")));
			ifTargetRows1.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("reconciledData"), "Add", new CodeVariableReferenceExpression("targetTable")));

			//							}
			ifInGap1.TrueStatements.Add(ifTargetRows1);

			//							// This will pass the data back to the client.  Deleted rows only need to include the primary key
			//							// elements.  Added and modified rows pass back the entire record.  Note again that only the committed
			//							// data is passed back to the client.  This obviates the need to lock the record before reading it.  A
			//							// table lock is enough to insure that the data isn't modified while it is placed in the buffer.
			//							object[] data = new object[sourceTable.Columns.Count];
			//							for (int columnIndex = 0; columnIndex < sourceTable.Columns.Count; columnIndex = (columnIndex + 1))
			//							{
			//								Column column = sourceTable.Columns[columnIndex];
			//								data[column.Ordinal] = sourceRow[column, DataRowVersion.Original];
			//							}
			//							targetRows.Add(new object[2] { DataRowState.Unchanged, data });
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("This will pass the data back to the client.  Deleted rows only need to include the primary key"));
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("elements.  Added and modified rows pass back the entire record.  Note again that only the committed"));
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("data is passed back to the client.  This obviates the need to lock the record before reading it.  A"));
			ifInGap1.TrueStatements.Add(new CodeCommentStatement("table lock is enough to insure that the data isn't modified while it is placed in the buffer."));
			ifInGap1.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("targetRows"), "Add", new CodeArrayCreateExpression(typeof(System.Object), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataRowState)), "Deleted"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sourceRow"), "ItemArray"))));

			//						}
			viewLoop1.Statements.Add(ifInGap1);

			//					}
			tryScanTable1.TryStatements.Add(viewLoop1);

			//					// This statement allows the loop above to exit when there are no more gaps to be evaluated for the current 
			//					// table.  There is no purpose to setting the 'gapIndex' here, but the CodeDOM can't generate a 'break'
			//					// statement or an empty statement.  This makes for some pretty ugly code.
			//				ViewExit1:
			//					gapIndex = 0;
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("This statement allows the loop above to exit when there are no more gaps to be evaluated for the current "));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("table.  There is no purpose to setting the 'gapIndex' here, but the CodeDOM can't generate a 'break'"));
			tryScanTable1.TryStatements.Add(new CodeCommentStatement("statement or an empty statement.  This makes for some pretty ugly code."));
			tryScanTable1.TryStatements.Add(new CodeLabeledStatement("ViewExit1", new CodeAssignStatement(new CodeVariableReferenceExpression("gapIndex"), new CodePrimitiveExpression(0))));

			//				finally
			//				{
			//					// Allow other threads to use the deleted data set now.
			//					DataModel.deletedExclusion.ReleaseMutex();
			//				}
			tryScanTable1.FinallyStatements.Add(new CodeCommentStatement("Allow other threads to use the deleted data set now."));
			tryScanTable1.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "ReleaseMutex"));

			//				}
			tableLoop1.Statements.Add(tryScanTable1);

			//			}
			this.Statements.Add(tableLoop1);

			//			// When this data structure is merged with the client data model, the server and client databases will be in synch.  A 
			//			// 'null' in the return data indicates that there is no data that is new since the last reconcilliation.
			//			return reconciledData;
			this.Statements.Add(new CodeCommentStatement("When this data structure is merged with the client data model, the server and client databases will be in synch.  A"));
			this.Statements.Add(new CodeCommentStatement("'null' in the return data indicates that there is no data that is new since the last reconcilliation."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("reconciledData")));

		}

	}

}
