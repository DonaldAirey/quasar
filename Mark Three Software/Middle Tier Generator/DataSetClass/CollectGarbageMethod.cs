namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class CollectGarbageMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public CollectGarbageMethod(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Purges the deleted data model of obsolete rows.
			//		/// </summary>
			//		private static void CollectGarbage()
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Purges the deleted data model of obsolete rows.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = "CollectGarbage";
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			
			//			// The tread can be terminated gracefully from another thread by clearing this value.
			//			DataModel.IsGarbageCollecting = true;
			//			for (
			//			; DataModel.IsGarbageCollecting == true;
			//			)
			//			{
			this.Statements.Add(new CodeCommentStatement("The tread can be terminated gracefully from another thread by clearing this value."));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "IsGarbageCollecting"), new CodePrimitiveExpression(true)));
			CodeIterationStatement whileCollecting = new CodeIterationStatement(new CodeSnippetStatement(), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "IsGarbageCollecting"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)), new CodeSnippetStatement());

			//				// To keep this thread from using up too much of the CPU time, it will wait here when there is no data in the
			//				// deleted data model.  The flag is set when records are added to the deleted data model and cleared when there are
			//				// no more deleted records to process.
			//				DataModel.deletedEvent.WaitOne();
			whileCollecting.Statements.Add(new CodeCommentStatement("To keep this thread from using up too much of the CPU time, it will wait here when there is no data in the"));
			whileCollecting.Statements.Add(new CodeCommentStatement("deleted data model.  The flag is set when records are added to the deleted data model and cleared when there are"));
			whileCollecting.Statements.Add(new CodeCommentStatement("no more deleted records to process."));
			whileCollecting.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedEvent"), "WaitOne"));

			//				// These values are used to calculate the amount of time to sleep between scanning the tables for deleted records
			//				// that have expired.  Each time through the loop an attempt is made to guess at the time the oldest deleted 
			//				// record will expire.  The looping mechanism tries to spend as little time as possible checking the tables for
			//				// records that are ready to be purged.
			//				TimeSpan sleepTime = TimeSpan.Zero;
			//				DateTime maxTime = DateTime.MinValue;
			//				try
			//				{
			whileCollecting.Statements.Add(new CodeCommentStatement("These values are used to calculate the amount of time to sleep between scanning the tables for deleted records"));
			whileCollecting.Statements.Add(new CodeCommentStatement("that have expired.  Each time through the loop an attempt is made to guess at the time the oldest deleted"));
			whileCollecting.Statements.Add(new CodeCommentStatement("record will expire.  The looping mechanism tries to spend as little time as possible checking the tables for"));
			whileCollecting.Statements.Add(new CodeCommentStatement("records that are ready to be purged."));
			whileCollecting.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.TimeSpan), "sleepTime", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.TimeSpan)), "Zero")));
			whileCollecting.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.DateTime), "maxTime", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DateTime)), "MinValue")));
			CodeTryCatchFinallyStatement tryMutex = new CodeTryCatchFinallyStatement();

			//					// The deleted data model can't be modified while it is purged.
			//					DataModel.deletedExclusion.WaitOne();
			tryMutex.TryStatements.Add(new CodeCommentStatement("The deleted data model can't be modified while it is purged."));
			tryMutex.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "WaitOne"));

			//					// This time is used to calculate the expiration time of all the records in this pass through the deleted data 
			//					// model.
			//					DateTime now = DateTime.Now;
			//					// Examine each table looking for expired rows.
			//					for (int tableIndex = 0; tableIndex < DataModel.deletedDataSet.Tables.Count; tableIndex = (tableIndex + 1))
			//					{
			tryMutex.TryStatements.Add(new CodeCommentStatement("This time is used to calculate the expiration time of all the records in this pass through the deleted data "));
			tryMutex.TryStatements.Add(new CodeCommentStatement("model."));
			tryMutex.TryStatements.Add(new CodeVariableDeclarationStatement(typeof(System.DateTime), "now", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DateTime)), "Now")));
			tryMutex.TryStatements.Add(new CodeCommentStatement("Examine each table looking for expired rows."));
			CodeIterationStatement tableLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//						// Select the next table in the deleted data model.
			//						DataTable dataTable = DataModel.deletedDataSet.Tables[tableIndex];
			//						// Examine each row for expired records.
			//						for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; )
			//						{
			tableLoop.Statements.Add(new CodeCommentStatement("Select the next table in the deleted data model."));
			tableLoop.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataTable), "dataTable", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));
			tableLoop.Statements.Add(new CodeCommentStatement("Examine each row for expired records."));
			CodeIterationStatement rowLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "rowIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataTable"), "Rows"), "Count")), new CodeSnippetStatement());

			//							// Extract the time that the record was deleted.
			//							DataRow dataRow = dataTable.Rows[rowIndex];
			//							DateTime deletedTime = (DateTime)dataRow[DataModel.deletedTimeColumn];
			rowLoop.Statements.Add(new CodeCommentStatement("Extract the time that the record was deleted."));
			rowLoop.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataRow), "dataRow", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataTable"), "Rows"), new CodeVariableReferenceExpression("rowIndex"))));
			rowLoop.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.DateTime), "deletedTime", new CodeCastExpression(typeof(System.DateTime), new CodeIndexerExpression(new CodeVariableReferenceExpression("dataRow"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedTimeColumn")))));

			//							// This value will be used to determine how much time to sleep between cycles that read the deleted
			//							// data model. This variable will hold the oldest record that hasn't been deleted after reading the
			//							// entire data model.  That time can be used to calculate how long to sleep until the next cycle.
			//							if (deletedTime > maxTime)
			//							{
			//								maxTime = deletedTime;
			//							}
			rowLoop.Statements.Add(new CodeCommentStatement("This value will be used to determine how much time to sleep between cycles that read the deleted"));
			rowLoop.Statements.Add(new CodeCommentStatement("data model. This variable will hold the oldest record that hasn't been deleted after reading the"));
			rowLoop.Statements.Add(new CodeCommentStatement("entire data model.  That time can be used to calculate how long to sleep until the next cycle."));
			rowLoop.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("deletedTime"), CodeBinaryOperatorType.GreaterThan, new CodeVariableReferenceExpression("maxTime")),
				new CodeAssignStatement(new CodeVariableReferenceExpression("maxTime"), new CodeVariableReferenceExpression("deletedTime"))));

			//							// A row is obsolete after a constant time (generally about a minute).  At this time it is assumed that
			//							// all the client data models have had a chance to reconcile with the server and have removed the
			//							// deleted records.  If the row just read from this table has not yet expired, then there's no need to
			//							// continue deleting because the rest of the records in the table are younger than this one.  
			//							// Otherwise, the record is purged from the data model.
			//							if (now.Subtract(deletedTime) < DataModel.freshnessTime)
			//								rowIndex = dataTable.Rows.Count;
			//							else
			//								dataTable.Rows.Remove(dataRow);
			rowLoop.Statements.Add(new CodeCommentStatement("A row is obsolete after a constant time (generally about a minute).  At this time it is assumed that"));
			rowLoop.Statements.Add(new CodeCommentStatement("all the client data models have had a chance to reconcile with the server and have removed the"));
			rowLoop.Statements.Add(new CodeCommentStatement("deleted records.  If the row just read from this table has not yet expired, then there's no need to"));
			rowLoop.Statements.Add(new CodeCommentStatement("continue deleting because the rest of the records in the table are younger than this one."));
			rowLoop.Statements.Add(new CodeCommentStatement("Otherwise, the record is purged from the data model."));
			CodeConditionStatement ifObsolete = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("now"), "Subtract", new CodeVariableReferenceExpression("deletedTime")), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "freshnessTime")));
			ifObsolete.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowIndex"), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataTable"), "Rows"), "Count")));
			ifObsolete.FalseStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("dataTable"), "Rows"), "Remove", new CodeVariableReferenceExpression("dataRow")));
			rowLoop.Statements.Add(ifObsolete);
			
			//						}
			tableLoop.Statements.Add(rowLoop);

			//					}
			tryMutex.TryStatements.Add(tableLoop);

			//				}
			//				finally
			//				{
			//					// Allow the other threads to examine the deleted data model.
			//					DataModel.deletedExclusion.ReleaseMutex();
			//				}
			tryMutex.FinallyStatements.Add(new CodeCommentStatement("Allow the other threads to examine the deleted data model."));
			tryMutex.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), "ReleaseMutex"));
			whileCollecting.Statements.Add(tryMutex);

			//				// This thread is put to sleep when there are no more values in the deleted data model.
			//				if (maxTime == DateTime.MinValue)
			//				{
			//					DataModel.deletedEvent.Reset();
			//				}
			//				// This sleep time is calculated to put this thread to sleep until the row at the output end of the queue is ready
			//				// to expire.  The idea is to spend as much time in this low level worker thread as possible.
			//				sleepTime = DataModel.freshnessTime.Subtract(DateTime.Now.Subtract(maxTime));
			//				if (sleepTime < TimeSpan.Zero)
			//					sleepTime = TimeSpan.Zero;
			//				Thread.Sleep(sleepTime);
			whileCollecting.Statements.Add(new CodeCommentStatement("This thread is put to sleep when there are no more values in the deleted data model."));
			whileCollecting.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("maxTime"), CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DateTime)), "MinValue")),
				new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedEvent"), "Reset"))));
			whileCollecting.Statements.Add(new CodeCommentStatement("This sleep time is calculated to put this thread to sleep until the row at the output end of the queue is ready"));
			whileCollecting.Statements.Add(new CodeCommentStatement("to expire.  The idea is to spend as much time in this low level worker thread as possible."));
			whileCollecting.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sleepTime"), new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "freshnessTime"), "Subtract", new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.DateTime)), "Now"), "Subtract", new CodeVariableReferenceExpression("maxTime")))));
			whileCollecting.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("sleepTime"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.TimeSpan)), "Zero")),
				new CodeAssignStatement(new CodeVariableReferenceExpression("sleepTime"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.TimeSpan)), "Zero"))));
			whileCollecting.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Thread)), "Sleep", new CodeVariableReferenceExpression("sleepTime")));

			//			}
			this.Statements.Add(whileCollecting);
			
			//		}

		}

	}
}
