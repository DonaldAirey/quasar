namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	class DataSetStaticConstructor : CodeTypeConstructor
	{

		public DataSetStaticConstructor(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Static Constructor for the DataModel.
			//		/// </summary>
			//		static DataModel()
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Static Constructor for the {0}.", schema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));

			//			// Create the DataModel DataSet
			//			DataModel.dataSet = new DataSet();
			//			DataModel.dataSet.Name = "DataModel";
			//			DataModel.dataSet.CaseSensitive = true;
			//			DataModel.dataSet.EnforceConstraints = true;
			this.Statements.Add(new CodeCommentStatement(string.Format("Create the {0} DataSet", schema.Name)));
			this.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "dataSet"), new CodeObjectCreateExpression("DataSet", new CodeExpression[] { })));
			this.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "dataSet"), "DataSetName"), new CodePrimitiveExpression(schema.Name)));
			this.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "dataSet"), "CaseSensitive"), new CodePrimitiveExpression(true)));
			this.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "dataSet"), "EnforceConstraints"), new CodePrimitiveExpression(true)));

			//			// Create the Department Table.
			//			DataModel.tableDepartment = new DepartmentDataTable();
			//			DataModel.Tables.Add(DataModel.tableDepartment);
			foreach (TableSchema tableSchema in schema.Tables)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("Create the {0} Table.", tableSchema.Name)));
				this.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(schema.Name), String.Format("table{0}", tableSchema.Name)), new CodeObjectCreateExpression(string.Format("{0}DataTable", tableSchema.Name), new CodeExpression[] { })));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), "Add", new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(schema.Name), String.Format("table{0}", tableSchema.Name))));
			}

			//			// Create and enforce the relation between Department and Employee tables.
			//			DataModel.relationFK_Department_Employee = new Relation("FK_Department_Employee", new Column[] {
			//						DataModel.tableDepartment.DepartmentIdColumn}, new Column[] {
			//						DataModel.tableEmployee.DepartmentIdColumn}, true);
			//			DataModel.Relations.Add(DataModel.relationFK_Department_Employee);
			foreach (TableSchema tableSchema in schema.Tables)
				foreach (ConstraintSchema constraintSchema in tableSchema.Constraints)
					if (constraintSchema is KeyrefSchema)
					{

						// Extract the parent and child tables from the keys.
						KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;
						ConstraintSchema referSchema = keyrefSchema.Refer;
						TableSchema parentTable = referSchema.Selector;
						TableSchema childTable = keyrefSchema.Selector;

						// Collect the key fields in the parent table.
						List<CodeExpression> parentFieldList = new List<CodeExpression>();
						foreach (ColumnSchema columnSchema in referSchema.Fields)
						{
							string parentColumnName = String.Format("{0}Column", columnSchema.Name);
							string parentTableName = String.Format("table{0}", parentTable.Name);
							parentFieldList.Add(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(schema.Name), parentTableName), parentColumnName));
						}

						// Collect the referenced fields in the child table.
						List<CodeExpression> childFieldList = new List<CodeExpression>();
						foreach (ColumnSchema columnSchema in keyrefSchema.Fields)
						{
							string childColumnName = String.Format("{0}Column", columnSchema.Name);
							string childTableName = String.Format("table{0}", childTable.Name);
							childFieldList.Add(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(schema.Name), childTableName), childColumnName));
						}

						// Create the CodeDOM statements to create the relationship between the two tables and the foreign key
						// constraint that insures the integrity of the relation.
						this.Statements.Add(new CodeCommentStatement(string.Format("Create and enforce the relation between {0} and {1} tables.", parentTable.Name, childTable.Name)));
						CodeObjectCreateExpression newForeignKey = new CodeObjectCreateExpression("Relation", new CodeExpression[] { new CodePrimitiveExpression(keyrefSchema.Name), new CodeArrayCreateExpression("Column", parentFieldList.ToArray()), new CodeArrayCreateExpression("Column", childFieldList.ToArray()), new CodePrimitiveExpression(true) });
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), string.Format("relation{0}", keyrefSchema.Name)), newForeignKey));
						CodeExpression parameterExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), String.Format("relation{0}", keyrefSchema.Name));
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Relations"), "Add", new CodeExpression[] { parameterExpression }));

					}

			foreach (TableSchema tableSchema in schema.Tables)
			{

				//			// Initialze the relation fields for the Department table.
				//			DataModel.Department.InitializeRelations();
				this.Statements.Add(new CodeCommentStatement(string.Format("Initialze the relation fields for the {0} table.", tableSchema.Name), true));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), tableSchema.Name), "InitializeRelations"));

			}

			//			// The SQL Resource Manager normally controls the resources in a transaction.  However, it is impractical to lock all
			//			// the records in the database as they are loaded into memory, so the static initialization of the database foregoes
			//			// the standard transaction support.  This resource manager is used here only for the connection string to the durable
			//			// database.
			//			SqlResourceManager sqlResourceManager = new SqlResourceManager(DataModel.durableResource);
			this.Statements.Add(new CodeCommentStatement("The SQL Resource Manager normally controls the resources in a transaction.  However, it is impractical to lock all"));
			this.Statements.Add(new CodeCommentStatement("the records in the database as they are loaded into memory, so the static initialization of the database foregoes"));
			this.Statements.Add(new CodeCommentStatement("the standard transaction support.  This resource manager is used here only for the connection string to the durable"));
			this.Statements.Add(new CodeCommentStatement("database."));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference("SqlResourceManager"), "sqlResourceManager", new CodeObjectCreateExpression(new CodeTypeReference("SqlResourceManager"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "durableResource"))));

			//			// This will keep track of the largest row number in the persistent data store.  This maximum row version will
			//			// the seed value for the in-memory database when it inserts, updates and deletes records.
			//			DataModel.masterRowVersion = 0L;
			this.Statements.Add(new CodeCommentStatement("This will keep track of the largest row number in the persistent data store.  This maximum row version will become"));
			this.Statements.Add(new CodeCommentStatement("the seed value for the in-memory database when it inserts, updates and deletes records."));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion"), new CodePrimitiveExpression(0L)));

			//			// This DataSet is used to hold rows that have been deleted from the main data model but still need to be transmitted
			//			// to all the clients.  This mutual exclusion lock will prevent the background thread from purging records while a
			//			// client data model is being reconciled to the server.
			//			DataModel.deletedExclusion = new Mutex();
			//			DataModel.deletedDataSet = new DataSet();
			//			DataModel.deletedEvent = new ManualResetEvent(false);
			this.Statements.Add(new CodeCommentStatement("This DataSet is used to hold rows that have been deleted from the main data model but still need to be transmitted"));
			this.Statements.Add(new CodeCommentStatement("to all the clients.  This mutual exclusion lock will prevent the background thread from purging records while a"));
			this.Statements.Add(new CodeCommentStatement("client data model is being reconciled to the server."));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedExclusion"), new CodeObjectCreateExpression(typeof(System.Threading.Mutex))));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), new CodeObjectCreateExpression(typeof(System.Data.DataSet))));
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedEvent"), new CodeObjectCreateExpression(typeof(System.Threading.ManualResetEvent), new CodePrimitiveExpression(false))));

			//			// Initialize each of the tables in the data model with the data in the durable store.
			//			for (int tableIndex = 0; tableIndex < DataModel.Tables.Count; tableIndex++)
			//			{
			this.Statements.Add(new CodeCommentStatement("Initialize each of the tables in the data model with the data in the durable store."));
			CodeIterationStatement tableLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//				// Select the next table in the list.
			//				Table table = DataModel.Tables[tableIndex];
			tableLoop.Statements.Add(new CodeCommentStatement("Select the next table in the list."));
			tableLoop.Statements.Add(new CodeVariableDeclarationStatement("Table", "table", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));

			//				// IMPORTANT CONCEPT: Modified records in the server data model need to be reconciled with the client data model.
			//				// This is accomplished using a row version column on each record that increments every time a change is made.  The
			//				// client will ask the server for any records that are missing.  The server responds with all the records that were
			//				// modified since the last time the client asked.  In order to make this search as efficient as possible, a
			//				// DataView is created for each of the tables to order the rows by the committed row version values.
			//				table.DefaultView.Sort = "RowVersion DESC";
			//				table.DefaultView.RowStateFilter = DataViewRowState.OriginalRows;
			tableLoop.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: Modified records in the server data model need to be reconciled with the client data model."));
			tableLoop.Statements.Add(new CodeCommentStatement("This is accomplished using a row version column on each record that increments every time a change is made.  The"));
			tableLoop.Statements.Add(new CodeCommentStatement("client will ask the server for any records that are missing.  The server responds with all the records that were"));
			tableLoop.Statements.Add(new CodeCommentStatement("modified since the last time the client asked.  In order to make this search as efficient as possible, a"));
			tableLoop.Statements.Add(new CodeCommentStatement("DataView is created for each of the tables to order the rows by the committed row version values."));
			tableLoop.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "DefaultView"), "Sort"), new CodePrimitiveExpression("RowVersion DESC")));
			tableLoop.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "DefaultView"), "RowStateFilter"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Data.DataViewRowState)), "OriginalRows")));

			//				// IMPORTANT CONCEPT: Rows are not deleted at the end of transactions.  They need to be propagated back to the 
			//				// client data models before they can be purged from the table.  This will set up an event handler that will catch
			//				// the deleted rows and place them in a queue.  The garbage collector thread will read the elements in this queue
			//				// and delete them when the time stamp on the row indicates that it is obsolete.
			//				table.RowChanging += new DataRowChangeEventHandler(TableRowChanging);
			tableLoop.Statements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: Rows are not deleted at the end of transactions.  They need to be propagated back to the"));
			tableLoop.Statements.Add(new CodeCommentStatement("client data models before they can be purged from the table.  This will set up an event handler that will catch"));
			tableLoop.Statements.Add(new CodeCommentStatement("the deleted rows and place them in a queue.  The garbage collector thread will read the elements in this queue"));
			tableLoop.Statements.Add(new CodeCommentStatement("and delete them when the time stamp on the row indicates that it is obsolete."));
			tableLoop.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(new CodeVariableReferenceExpression("table"), "RowChanging"), new CodeObjectCreateExpression(typeof(System.Data.DataRowChangeEventHandler), new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "TableRowChanging"))));

			//				// Deleted records also need to be reconciled to the client data model.  This is done by moving them to a seperate
			//				// DataSet once they've been deleted.  This DataSet basically only contains the keys of the deleted items plus some
			//				// housekeeping values that allows a background thread to clean up the deleted records once all the clients have
			//				// been notified.
			//				DataTable deletedTable = new DataTable(table.TableName);
			//				deletedTable.Columns.Add("RowVersion", typeof(long));
			//				deletedTable.Columns.Add("DeletedTime", typeof(DateTime));
			//				for (int columnIndex = 0; (columnIndex < table.PrimaryKey.Length); columnIndex = (columnIndex + 1))
			//				{
			//					Column column = (Column)table.PrimaryKey[columnIndex];
			//					deletedTable.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
			//				}
			//				DataModel.deletedDataSet.Tables.Add(deletedTable);
			//				deletedTable.DefaultView.Sort = "RowVersion DESC";
			tableLoop.Statements.Add(new CodeCommentStatement("Deleted records also need to be reconciled to the client data model.  This is done by moving them to a seperate"));
			tableLoop.Statements.Add(new CodeCommentStatement("DataSet once they've been deleted.  This DataSet basically only contains the keys of the deleted items plus some"));
			tableLoop.Statements.Add(new CodeCommentStatement("housekeeping values that allows a background thread to clean up the deleted records once all the clients have"));
			tableLoop.Statements.Add(new CodeCommentStatement("been notified."));
			tableLoop.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataTable), "deletedTable", new CodeObjectCreateExpression(typeof(System.Data.DataTable), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "TableName"))));
			tableLoop.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("deletedTable"), "Columns"), "Add", new CodePrimitiveExpression("RowVersion"), new CodeTypeOfExpression(typeof(System.Int64))));
			tableLoop.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("deletedTable"), "Columns"), "Add", new CodePrimitiveExpression("DeletedTime"), new CodeTypeOfExpression(typeof(System.DateTime))));
			CodeIterationStatement columnLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "columnIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "PrimaryKey"), "Length")), new CodeAssignStatement(new CodeVariableReferenceExpression("columnIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			columnLoop.Statements.Add(new CodeVariableDeclarationStatement("Column", "column", new CodeCastExpression(new CodeTypeReference("Column"), new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "PrimaryKey"), new CodeVariableReferenceExpression("columnIndex")))));
			columnLoop.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("deletedTable"), "Columns"), "Add", new CodeObjectCreateExpression(typeof(System.Data.DataColumn), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "ColumnName"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "DataType"))));
			tableLoop.Statements.Add(columnLoop);
			tableLoop.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "deletedDataSet"), "Tables"), "Add", new CodeVariableReferenceExpression("deletedTable")));
			tableLoop.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("deletedTable"), "DefaultView"), "Sort"), new CodePrimitiveExpression("RowVersion DESC")));
			
			//			}
			this.Statements.Add(tableLoop);

			//			try
			//			{
			//				// Constraints are disabled so the data can be loaded in a bulk operation.
			//				DataModel.EnforceConstraints = false;
			CodeTryCatchFinallyStatement tryLoadData = new CodeTryCatchFinallyStatement();
			tryLoadData.TryStatements.Add(new CodeCommentStatement("Constraints are disabled so the data can be loaded in a bulk operation."));
			tryLoadData.TryStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "EnforceConstraints"), new CodePrimitiveExpression(false)));

			//				// Read each of the persistent tables from the relational database store.
			//				for (int tableIndex = 0; tableIndex < DataModel.Tables.Count; tableIndex++)
			//				{
			tryLoadData.TryStatements.Add(new CodeCommentStatement("Read each of the persistent tables from the relational database store."));
			CodeIterationStatement tableLoop1 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));

			//					// Select the next table in the list.
			//					Table table = DataModel.Tables[tableIndex];
			//					if ((table.IsPersistent == true))
			//					{
			tableLoop1.Statements.Add(new CodeCommentStatement("Select the next table in the list."));
			tableLoop1.Statements.Add(new CodeVariableDeclarationStatement("Table", "table", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));
			CodeConditionStatement ifPersistent = new CodeConditionStatement();
			ifPersistent.Condition = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "IsPersistent"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));

			//						// IMPORTANT CONCEPT: Every record has an 'Archived' bit and a 'Deleted' bit which indicates whether the
			//						// record has been deleted from the active database.  The 'Archive' indicates the record was still valid
			//						// when it was migrated out of the in-memory data base.  The 'Deleted' flag indicates that it was purged
			//						// from the data model and is used for auditing purposes only.  This bit is used here to discard records
			//						// that shouldn't be included in the active database. While it would be possible to simply select all the
			//						// records where the 'Archived' bit is clear, they are added to the table and then deleted in order to keep
			//						// the identity columns of the ADO tables synchronized with the indices of the persistent SQL tables.  This
			//						// section of code will construct a statement that will select all the persistent columns from the
			//						// relational database.
			//						string columnList = "\"IsArchived\", \"IsDeleted\"";
			//						for (int columnIndex = 0; (columnIndex < table.Columns.Count); columnIndex = (columnIndex + 1))
			//						{
			//							Column column = table.Columns[columnIndex];
			//							if ((column.IsPersistent == true))
			//								columnList = columnList + ",\"" + column.ColumnName + "\"";
			//						}
			//						string selectCommand = String.Format("select {0} from \"{1}\"", columnList, table.TableName);
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("IMPORTANT CONCEPT: Every record has an 'Archived' bit and a 'Deleted' bit which indicates whether the"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("record has been deleted from the active database.  The 'Archive' indicates the record was still valid"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("when it was migrated out of the in-memory data base.  The 'Deleted' flag indicates that it was purged"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("from the data model and is used for auditing purposes only.  This bit is used here to discard records"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("that shouldn't be included in the active database. While it would be possible to simply select all the"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("records where the 'Archived' bit is clear, they are added to the table and then deleted in order to keep"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("the identity columns of the ADO tables synchronized with the indices of the persistent SQL tables.  This"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("section of code will construct a statement that will select all the persistent columns from the"));
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("relational database."));
			ifPersistent.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.String), "columnList", new CodePrimitiveExpression("\"IsArchived\", \"IsDeleted\"")));
			CodeIterationStatement columnLoop1 = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "columnIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Columns"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("columnIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			columnLoop1.Statements.Add(new CodeVariableDeclarationStatement("Column", "column", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Columns"), new CodeVariableReferenceExpression("columnIndex"))));
			columnLoop1.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "IsPersistent"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
				new CodeAssignStatement(new CodeVariableReferenceExpression("columnList"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("columnList"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePrimitiveExpression(",\""), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("column"), "ColumnName"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression("\"")))))));

			//						}
			//						string selectCommand = String.Format("select {0} from \"{1}\"", columnList, table.TableName);
			ifPersistent.TrueStatements.Add(columnLoop1);
			ifPersistent.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.String), "selectCommand", new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.String)), "Format", new CodePrimitiveExpression("select {0} from \"{1}\""), new CodeVariableReferenceExpression("columnList"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "TableName"))));

			//						// Execute the command to read the table.
			//						SqlCommand sqlCommand = new SqlCommand(selectCommand);
			//						sqlCommand.Connection = sqlResourceManager.SqlConnection;
			//						SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			ifPersistent.TrueStatements.Add(new CodeCommentStatement("Execute the command to read the table."));
			ifPersistent.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlCommand), "sqlCommand", new CodeObjectCreateExpression(typeof(System.Data.SqlClient.SqlCommand), new CodeVariableReferenceExpression("selectCommand"))));
			ifPersistent.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Connection"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlResourceManager"), "SqlConnection")));
			ifPersistent.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlDataReader), "sqlDataReader", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteReader")));

			//						try
			//						{
			CodeTryCatchFinallyStatement tryTableLoad = new CodeTryCatchFinallyStatement();

			//							// The table management logic will prevent a row from being added to a table without the proper lock.
			//							// So even though there are no other threads that could use the data model while it is created, the
			//							// locking logic must be satisfied.
			//							table.ReaderWriterLock.AcquireWriterLock(Timeout.Infinite);
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("The table management logic will prevent a row from being added to a table without the proper lock."));
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("So even though there are no other threads that could use the data model while it is created, the"));
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("locking logic must be satisfied."));
			tryTableLoad.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));

			//							// Read all records from the database; throw away the ones that are archived and deleted.  Note that 
			//							// even discarded records need to factor into the master row identifier and row version counters.
			//							for (
			//							; (sqlDataReader.Read() == true);
			//								)
			//							{
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("Read all records from the database."));
			CodeIterationStatement readLoop = new CodeIterationStatement(new CodeSnippetStatement(""), new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlDataReader"), "Read"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)), new CodeSnippetStatement(""));

			//								// This will throw away archived and deleted records after reading the row version.  Otherwise, the active records are read into the volatile data model.
			//								if ((bool)sqlDataReader["IsArchived"] == true || (bool)sqlDataReader["IsDeleted"] == true)
			//								{
			//									// Discarded records also have row versions and row ids which need to be factored into the 
			//									// master counters in order to prevent a duplicate record from making its way into the
			//									// persistent store.
			//									long rowVersion = (long)sqlDataReader["RowVersion"];
			//									if (rowVersion > DataModel.masterRowVersion)
			//										DataModel.masterRowVersion = rowVersion;
			//								}
			readLoop.Statements.Add(new CodeCommentStatement("This will throw away archived and deleted records after reading the row version.  Otherwise,"));
			readLoop.Statements.Add(new CodeCommentStatement("the active records are read into the volatile data model."));
			CodeConditionStatement activeRecords = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeCastExpression(typeof(System.Boolean), new CodeIndexerExpression(new CodeVariableReferenceExpression("sqlDataReader"), new CodePrimitiveExpression("IsArchived"))), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)), CodeBinaryOperatorType.BooleanOr, new CodeBinaryOperatorExpression(new CodeCastExpression(typeof(System.Boolean), new CodeIndexerExpression(new CodeVariableReferenceExpression("sqlDataReader"), new CodePrimitiveExpression("IsDeleted"))), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true))));
			activeRecords.TrueStatements.Add(new CodeCommentStatement("Discarded records also have row versions which need to be factored into the master counters"));
			activeRecords.TrueStatements.Add(new CodeCommentStatement("in order to prevent a repeated row version from making its way into the data model."));
			activeRecords.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(System.Int64), "rowVersion", new CodeCastExpression(typeof(System.Int64), new CodeIndexerExpression(new CodeVariableReferenceExpression("sqlDataReader"), new CodePrimitiveExpression("RowVersion")))));
			activeRecords.TrueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowVersion"), CodeBinaryOperatorType.GreaterThan, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion")),
				new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion"), new CodeVariableReferenceExpression("rowVersion"))));

			//								else
			//								{
			//									// This row will be populated with the data as it is read in from the SQL stream.  The data is
			//									// mapped from the SQL tables into the ADO tables using the column names.
			//									Row row = table.NewRow();
			//									try
			//									{
			activeRecords.FalseStatements.Add(new CodeCommentStatement("This row will be populated with the data as it is read in from the SQL stream.  The data is"));
			activeRecords.FalseStatements.Add(new CodeCommentStatement("mapped from the SQL tables into the ADO tables using the column names."));
			activeRecords.FalseStatements.Add(new CodeVariableDeclarationStatement("Row", "row", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("table"), "NewRow")));
			CodeTryCatchFinallyStatement tryRowLoad = new CodeTryCatchFinallyStatement();

			//										// The new row must be locked for writing before it can be initialized.
			//										row.ReaderWriterLock.AcquireWriterLock(Timeout.Infinite);
			//										// This will copy the values from the fields into the freshly created row.  The column
			//										// names on the SQL tables are mapped to the ADO columns.  If there is no mapping, the
			//										// value is discarded.
			//										for (int ordinal = 0; ordinal < sqlDataReader.FieldCount; ordinal++)
			//										{
			//											DataColumn destinationColumn = table.Columns[sqlDataReader.GetName(ordinal)];
			//											if (destinationColumn != null)
			//												row[destinationColumn] = sqlDataReader.GetValue(ordinal);
			//										}
			//										// Another row has joined the table.
			//										table.Rows.Add(row);
			//										row.AcceptChanges();
			//										// Keep track of the maximum version number read from the database.  This is the starting
			//										// version number that the data model will use for row events once the in-memory data model
			//										// has been loaded from the persistent store.
			//										DataModel.masterRowVersion = row.RowVersion > DataModel.masterRowVersion ?
			//											row.RowVersion : DataModel.masterRowVersion;
			//									}
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("The new row must be locked for writing before it can be initialized."));
			tryRowLoad.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("This will copy the values from the fields into the freshly created row.  The column"));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("names on the SQL tables are mapped to the ADO columns.  If there is no mapping, the"));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("value is discarded."));
			CodeIterationStatement ordinalLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "ordinal", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ordinal"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlDataReader"), "FieldCount")), new CodeAssignStatement(new CodeVariableReferenceExpression("ordinal"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ordinal"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			ordinalLoop.Statements.Add(new CodeVariableDeclarationStatement("Column", "destinationColumn", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Columns"), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlDataReader"), "GetName", new CodeVariableReferenceExpression("ordinal")))));
			ordinalLoop.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("destinationColumn"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
				new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression("row"), new CodeVariableReferenceExpression("destinationColumn")), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlDataReader"), "GetValue", new CodeVariableReferenceExpression("ordinal")))));
			tryRowLoad.TryStatements.Add(ordinalLoop);
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("Another row has joined the table."));
			tryRowLoad.TryStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Rows"), "Add", new CodeVariableReferenceExpression("row")));
			tryRowLoad.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("row"), "AcceptChanges"));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("Keep track of the maximum version number read from the database.  This is the starting"));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("version number that the data model will use for row events once the in-memory data model"));
			tryRowLoad.TryStatements.Add(new CodeCommentStatement("has been loaded from the persistent store."));
			tryRowLoad.TryStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "RowVersion"), CodeBinaryOperatorType.GreaterThan, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion")),
				new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "RowVersion"))));

			//									finally
			//									{
			//										// A transaction for the entire data model would be prohibitive for a large data set. This
			//										// initialization foregoes the transaction logic and instead locks each record just to get
			//										// around the locking logic that is used during the normal operation of the shared data
			//										// model.
			//										row.ReaderWriterLock.ReleaseWriterLock();
			//									}
			tryRowLoad.FinallyStatements.Add(new CodeCommentStatement("A transaction for the entire data model would be prohibitive for a large data set. This"));
			tryRowLoad.FinallyStatements.Add(new CodeCommentStatement("initialization foregoes the transaction logic and instead locks each record just to get"));
			tryRowLoad.FinallyStatements.Add(new CodeCommentStatement("around the locking logic that is used during the normal operation of the shared data"));
			tryRowLoad.FinallyStatements.Add(new CodeCommentStatement("model."));
			tryRowLoad.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "ReaderWriterLock"), "ReleaseWriterLock"));
			activeRecords.FalseStatements.Add(tryRowLoad);

			//								}
			readLoop.Statements.Add(activeRecords);

			//							}
			//							// This is the end of reading a table.  Close out the reader, accept the changes and move on to the 
			//							// next table in the DataSet.
			//							sqlDataReader.Close();
			tryTableLoad.TryStatements.Add(readLoop);
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("This is the end of reading a table.  Close out the reader, accept the changes and move on to the "));
			tryTableLoad.TryStatements.Add(new CodeCommentStatement("next table in the DataSet."));
			tryTableLoad.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlDataReader"), "Close"));

			//						finally
			//						{
			//							// Time to release this table and populate the next.
			//							table.ReaderWriterLock.ReleaseWriterLock();
			//						}
			tryTableLoad.FinallyStatements.Add(new CodeCommentStatement("Time to release this table and populate the next."));
			tryTableLoad.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "ReaderWriterLock"), "ReleaseWriterLock"));
			ifPersistent.TrueStatements.Add(tryTableLoad);
			
			//					}
			tableLoop1.Statements.Add(ifPersistent);

			//				}
			tryLoadData.TryStatements.Add(tableLoop1);

			//				// Once all the tables have been read, the constraints can be enforced again.  This is where any Relational
			//				// Integrity problems will kick out.
			//				DataModel.EnforceConstraints = true;
			//			}
			tryLoadData.TryStatements.Add(new CodeCommentStatement("Once all the tables have been read, the constraints can be enforced again.  This is where any Relational"));
			tryLoadData.TryStatements.Add(new CodeCommentStatement("Integrity problems will kick out."));
			tryLoadData.TryStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "EnforceConstraints"), new CodePrimitiveExpression(true)));

			//			catch (System.Data.ConstraintException constraintException)
			//			{
			//				// Write the exact location of each of the constraint violation to the event log.
			//				for (int tableIndex = 0; (tableIndex < DataModel.Tables.Count); tableIndex = (tableIndex + 1))
			//				{
			//					Table table = DataModel.Tables[tableIndex];
			//					for (int rowIndex = 0; (rowIndex < table.Rows.Count); rowIndex = (rowIndex + 1))
			//					{
			//						Row row = table.Rows[rowIndex];
			//						if (row.HasErrors)
			//							EventLog.Error("Error in '{0}': {1}", row.Table.TableName, row.RowError);
			//					}
			//				}
			//				// The desired response to an error is to have the initializer fail.
			//				throw constraintException;
			//			}
			CodeCatchClause catchConstraint = new CodeCatchClause("constraintException", new CodeTypeReference(typeof(System.Data.ConstraintException)));
			catchConstraint.Statements.Add(new CodeCommentStatement("Write the exact location of each of the constraint violation to the event log."));
			CodeIterationStatement constraintTableLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "tableIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("tableIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("tableIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			constraintTableLoop.Statements.Add(new CodeVariableDeclarationStatement("Table", "table", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "Tables"), new CodeVariableReferenceExpression("tableIndex"))));
			CodeIterationStatement constraintRowLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "rowIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Rows"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("rowIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			constraintRowLoop.Statements.Add(new CodeVariableDeclarationStatement("Row", "row", new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("table"), "Rows"), new CodeVariableReferenceExpression("rowIndex"))));
			constraintRowLoop.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "HasErrors"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
				new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("EventLog"), "Error", new CodePrimitiveExpression("Error in '{0}': {1}"), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "Table"), "TableName"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("row"), "RowError")))));
			constraintTableLoop.Statements.Add(constraintRowLoop);
			catchConstraint.Statements.Add(constraintTableLoop);
			catchConstraint.Statements.Add(new CodeCommentStatement("The desired response to an error is to have the initializer fail."));
			catchConstraint.Statements.Add(new CodeThrowExceptionStatement(new CodeVariableReferenceExpression("constraintException")));
			tryLoadData.CatchClauses.Add(catchConstraint);

			//			catch (System.Data.SqlClient.SqlException sqlException)
			//			{
			//				// Write each of the SQL errors to the event log.
			//				for (int errorIndex = 0; (errorIndex < sqlException.Errors.Count); errorIndex = (errorIndex + 1))
			//				{
			//					EventLog.Error(sqlException.Errors[errorIndex]);
			//				}
			//				// The desired response to an error is to have the initializer fail.
			//				throw sqlException;
			//			}
			CodeCatchClause catchSqlException = new CodeCatchClause("sqlException", new CodeTypeReference(typeof(System.Data.SqlClient.SqlException)));
			catchSqlException.Statements.Add(new CodeCommentStatement("Write each of the SQL errors to the event log."));
			CodeIterationStatement sqlErrorLoop = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), "errorIndex", new CodePrimitiveExpression(0)), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("errorIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlException"), "Errors"), "Count")), new CodeAssignStatement(new CodeVariableReferenceExpression("errorIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("errorIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			sqlErrorLoop.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression("EventLog"), "Error", new CodeFieldReferenceExpression(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlException"), "Errors"), new CodeVariableReferenceExpression("errorIndex")), "Message")));
			catchSqlException.Statements.Add(sqlErrorLoop);
			catchSqlException.Statements.Add(new CodeCommentStatement("The desired response to an error is to have the initializer fail."));
			catchSqlException.Statements.Add(new CodeThrowExceptionStatement(new CodeVariableReferenceExpression("sqlException")));
			tryLoadData.CatchClauses.Add(catchSqlException);
			
			//			}
			this.Statements.Add(tryLoadData);

			//			// This will insure that the background thread to access the server isn't spawned when in the design mode which will 
			//			// kill the designer.
			//			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			//			{
			//				// This thread is used to access the database for the objects that this user has access to.  The end product of 
			//				// this thread is a tree structure that contains the hierarchy of objects (accounts, users, trading desks,
			//				// securities, etc.) that the user can navigate.  The results are passed back to this thread through the delegate
			//				// procedure 'objectNode' after the control window is created.
			//				DataModel.garbageCollector = new Thread(CollectGarbage);
			//				DataModel.garbageCollector.Name = "Server Garbage Collector";
			//				DataModel.garbageCollector.Priority = ThreadPriority.Highest;
			//				DataModel.garbageCollector.Start();
			//			}
			this.Statements.Add(new CodeCommentStatement("This will insure that the background thread to access the server isn't spawned when in the design mode which will "));
			this.Statements.Add(new CodeCommentStatement("kill the designer."));
			CodeConditionStatement licenseCheck = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ComponentModel.LicenseManager)), "UsageMode"), CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.ComponentModel.LicenseUsageMode)), "Designtime")));
			licenseCheck.TrueStatements.Add(new CodeCommentStatement("This thread wil clean out the deleted data model when all the clients have a copy of the deleted rows."));
			licenseCheck.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "garbageCollector"), new CodeObjectCreateExpression(typeof(System.Threading.Thread), new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "CollectGarbage"))));
			licenseCheck.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "garbageCollector"), "Name"), new CodePrimitiveExpression("Server Garbage Collector")));
			licenseCheck.TrueStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "garbageCollector"), "Priority"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.ThreadPriority)), "Highest")));
			licenseCheck.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "garbageCollector"), "Start"));
			this.Statements.Add(licenseCheck);

		}

	}

}
