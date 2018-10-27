namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.Collections.Generic;
	using System.CodeDom;

	/// <summary>
	/// Creates the CodeDOM of a method to update a record using transacted logic.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Delete : CodeMemberMethod
	{

		private TableSchema tableSchema;

		/// <summary>
		/// Creates the CodeDOM for a method to delete a record from a table using transacted logic.
		/// </summary>
		/// <param name="tableSchema">A description of the table.</param>
		public Delete(TableSchema tableSchema)
		{

			// Initialize the object.
			this.tableSchema = tableSchema;

			// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
			// all the table locks that are used for this operation and organizes them in a list that is used to generate the
			// locking and releasing statements below.
			List<LockRequest> tableLockList = new List<LockRequest>();
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
				tableLockList.Add(new WriteRequest(familyTable));
			foreach (TableSchema childTable in tableSchema.Descendants)
				tableLockList.Add(new WriteRequest(childTable));
			tableLockList.Sort();

			// Shorthand notations for the elements used to construct the interface to this table:
			string tableTypeName = string.Format("{0}.{1}DataTable", tableSchema.DataModelSchema.Name, tableSchema.Name);
			string tableVariableName = string.Format("{0}Table", tableSchema.Name[0].ToString().ToLower() + tableSchema.Name.Remove(0, 1));
			string rowTypeName = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, tableSchema.Name);
			string rowVariableName = string.Format("{0}Row", tableSchema.Name[0].ToString().ToLower() + tableSchema.Name.Remove(0, 1));
			string identityColumnName = tableSchema.PrimaryKey == null ? string.Empty : tableSchema.PrimaryKey.Fields[0].Name;
			string identityVariableName = tableSchema.PrimaryKey == null ? string.Empty : identityColumnName[0].ToString().ToLower() + identityColumnName.Remove(0, 1);

			//		/// <summary>Deletes a Employee record.</summary>
			//		/// <param name="employeeId">The value for the EmployeeId column.</param>
			//		/// <param name="rowVersion">Used for Optimistic Concurrency Checking.</param>
			//		public static void Delete(int employeeId, ref long rowVersion)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Deletes a {0} record.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (tableSchema.IsPrimaryKeyColumn(columnSchema) && columnSchema.DeclaringType == tableSchema.TypeSchema)
					this.Comments.Add(new CodeCommentStatement(string.Format(@"<param name=""{0}"">The value for the {1} column.</param>", Generate.CamelCase(columnSchema.Name), columnSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""rowVersion"">Used for Optimistic Concurrency Checking.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Delete";
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (tableSchema.IsPrimaryKeyColumn(columnSchema) && columnSchema.DeclaringType == tableSchema.TypeSchema)
				{
					Type typeColumn = columnSchema.DataType;
					Type parameterType = tableSchema.IsPrimaryKeyColumn(columnSchema) ? typeColumn : typeof(object);
					CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression(parameterType, Generate.CamelCase(columnSchema.Name));
					this.Parameters.Add(parameter);
				}
			CodeParameterDeclarationExpression rowVersionParameter = new CodeParameterDeclarationExpression(typeof(long), "rowVersion");
			this.Parameters.Add(rowVersionParameter);

			//			// This method is part of a larger transaction.  Instead of passing the transaction and the resource managers down
			//			// through several layers of methods, they are acccessed as ambient properties of the Transaction class.
			//			Transaction transaction = Transaction.Current;
			//			AdoResourceManager adoResourceManager = ((AdoResourceManager)(transaction["ADO Data Model"]));
			//			SqlResourceManager sqlResourceManager = ((SqlResourceManager)(transaction["SQL Data Model"]));
			this.Statements.Add(new CodeCommentStatement("This method is part of a larger transaction.  Instead of passing the transaction and the resource managers down"));
			this.Statements.Add(new CodeCommentStatement("through several layers of methods, they are acccessed as ambient properties of the Transaction class."));
			this.Statements.Add(new CodeVariableDeclarationStatement("Transaction", "transaction", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Transaction"), "Current")));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoResourceManager", "adoResourceManager", new CodeCastExpression("AdoResourceManager", new CodeIndexerExpression(new CodeVariableReferenceExpression("transaction"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}DataTable", tableSchema.Name)), "VolatileResource")))));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlResourceManager", "sqlResourceManager", new CodeCastExpression("SqlResourceManager", new CodeIndexerExpression(new CodeVariableReferenceExpression("transaction"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}DataTable", tableSchema.Name)), "DurableResource")))));

			//				// This is used below to assemble the SQL commands.
			//				SqlCommand sqlCommand = null;
			this.Statements.Add(new CodeCommentStatement("This is used below to assemble the SQL commands."));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlCommand", "sqlCommand", new CodePrimitiveExpression(null)));
			
			//				// The Department record is locked for the duration of the transaction.
			//				ServerDataModel.DepartmentRow departmentRow = ((ServerDataModel.DepartmentRow)(ServerDataModel.Department.FindByDepartmentId(departmentId)));
			//				if ((departmentRow == null))
			//				{
			//					throw new System.Exception(string.Format("Attempt to update a Department record ({0}) that doesn\'t exist", departmentId));
			//				}
			//				departmentRow.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			TableSchema rootTable = tableSchema.RootTable;
			this.Statements.Add(new CodeCommentStatement(string.Format("The {0} record is deleted from the most distant descendant back up to the root object in order to preserved", rootTable.Name)));
			this.Statements.Add(new CodeCommentStatement("the integrity of the cascading relations."));
			string rowVariable = string.Format("{0}Row", Generate.CamelCase(rootTable.Name));
			string rowType = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, rootTable.Name);
			string findMethodName = string.Format("FindBy");
			string exceptionFormat = string.Empty;
			int parameterCounter = 0;
			List<CodeExpression> methodParameters = new List<CodeExpression>();
			List<CodeExpression> exceptionParameters = new List<CodeExpression>();
			if (tableSchema.PrimaryKey != null)
			{
				foreach (ColumnSchema columnSchema in tableSchema.PrimaryKey.Fields)
				{
					methodParameters.Add(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)));
					exceptionParameters.Add(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)));
					exceptionFormat += string.Format("{{0}}", parameterCounter++);
				}
				foreach (ColumnSchema columnSchema in rootTable.PrimaryKey.Fields)
					findMethodName += columnSchema.Name;
			}
			exceptionParameters.Insert(0, new CodePrimitiveExpression(string.Format("Attempt to delete a {0} record ({1}) that doesn't exist", rootTable.Name, exceptionFormat)));
			this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, rootTable.Name), rowVariable, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), rootTable.Name), findMethodName, methodParameters.ToArray())));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariable), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression("MarkThree.RecordNotFoundException", exceptionParameters.ToArray()))));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			this.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"))));

			DeleteChildren(rootTable, this.Statements);

			//					// Delete the Department record from the SQL data model.
			//					sqlCommand = new SqlCommand("delete \"Department\" where \"DepartmentId\"=@departmentId", sqlResourceManager.SqlConnection);
			//					sqlCommand.Parameters.Add(new SqlParameter("@departmentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, departmentRow[ServerDataModel.Department.DepartmentIdColumn]));
			//					sqlCommands.Add(sqlCommand);
			if (tableSchema.IsPersistent)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("Delete the {0} record from the SQL data model.", rootTable.Name)));
				string whereClause = string.Empty;
				if (rootTable.PrimaryKey != null)
					foreach (ColumnSchema columnSchema in rootTable.PrimaryKey.Fields)
						whereClause += string.Format("\"{0}\"=@{1}", columnSchema.Name, Generate.CamelCase(columnSchema.Name));
				string deleteCommandText = string.Format("delete \"{0}\" where {1}", rootTable.Name, whereClause);
				this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sqlCommand"), new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(deleteCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
				if (rootTable.PrimaryKey != null)
					foreach (ColumnSchema columnSchema in rootTable.PrimaryKey.Fields)
					{
						string variableName = Generate.CamelCase(columnSchema.Name);
						CodeExpression codeExpression = new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(rootTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, rootTable.Name)), string.Format("{0}Column", columnSchema.Name)));
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), codeExpression }) }));
					}
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteNonQuery"));
			}

			//					// Delete the Department record from the ADO data model.
			//					adoResourceManager.Add(departmentRow);
			//					departmentRow[ServerDataModel.Department.RowVersionColumn] = ServerDataModel.IncrementRowVersion();
			//					departmentRow.Delete();
			this.Statements.Add(new CodeCommentStatement(string.Format("Delete the {0} record from the ADO data model.", rootTable.Name)));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeVariableReferenceExpression(rowVariable)));
			CodeTryCatchFinallyStatement tryFinallyStatement = new CodeTryCatchFinallyStatement();
			tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "BeginEdit"));
			tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, rootTable.Name)), "RowVersionColumn")), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), "IncrementRowVersion")));
			tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "Delete"));
			tryFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "EndEdit"));
			this.Statements.Add(tryFinallyStatement);

		}

		private void DeleteChildren(TableSchema parentTable, CodeStatementCollection codeStatements)
		{

			//				// Delete each of the child Department records in a cascade.
			//				ServerDataModel.DepartmentRow[] departmentRows = objectRow.GetDepartmentRows();
			//				for (int departmentIndex = 0; departmentIndex < departmentRows.Length; departmentIndex = (departmentIndex + 1))
			//				{
			//					// Get the next department in the list.
			//					ServerDataModel.DepartmentRow departmentRow = departmentRows[departmentIndex];
			//					// Lock the ADO record.
			//					departmentRow.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//					adoResourceManager.Add(departmentRow.ReaderWriterLock);
			//					// Optimistic Concurrency Check.
			//					if ((departmentRow.RowVersion != rowVersion))
			//					{
			//						throw new System.Exception("This record is busy.  Please try again later.");
			//					}
			//					// Delete the SQL record as part of a transaction.
			//					SqlCommand departmentCommand = new SqlCommand(@"delete ""Department"" where ""DepartmentId""=@departmentId", sqlResourceManager.SqlConnection);
			//					departmentCommand.Parameters.Add(new SqlParameter("@departmentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, departmentRow[ServerDataModel.Department.DepartmentIdColumn]));
			//					sqlCommands.Add(departmentCommand);
			//					// Delete the ADO record as part of a transaction.
			//					adoResourceManager.Add(departmentRow);
			//					departmentRow[ServerDataModel.Department.RowVersionColumn] = ServerDataModel.IncrementRowVersion();
			//					departmentRow.Delete();
			//				}
			foreach (KeyrefSchema childKeyref in parentTable.ChildKeyrefs)
			{

				TableSchema childTable = childKeyref.Selector;
				if (childTable.RootTable == parentTable && childTable != this.tableSchema)
					continue;

				string parentRowVariable = string.Format("{0}Row", Generate.CamelCase(parentTable.Name));
				string rowArrayType = string.Format("{0}.{1}Row[]", tableSchema.DataModelSchema.Name, childTable.Name);
				string rowArrayVariable = string.Format("{0}{1}Rows", Generate.CamelCase(parentTable.Name), childTable.Name);
				string rowType = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, childTable.Name);
				string rowVariable = string.Format("{0}Row", Generate.CamelCase(childTable.Name));
				string methodName = string.Format("Get{0}Rows", childTable.Name);
				string iteratorVariable = string.Format("{0}Index", Generate.CamelCase(childTable.Name));
				codeStatements.Add(new CodeCommentStatement(string.Format("Delete each of the child {0} records in a cascade.", childTable.Name)));
				codeStatements.Add(new CodeVariableDeclarationStatement(rowArrayType, rowArrayVariable, new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(parentRowVariable), methodName)));

				CodeStatementCollection iteratorStatements = new CodeStatementCollection();
				iteratorStatements.Add(new CodeCommentStatement(string.Format("Get the next {0} row in the list of children and lock it for the duration of the transaction.", childTable.Name)));
				iteratorStatements.Add(new CodeVariableDeclarationStatement(rowType, rowVariable, new CodeIndexerExpression(new CodeVariableReferenceExpression(rowArrayVariable), new CodeVariableReferenceExpression(iteratorVariable))));
				iteratorStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite"))));
				iteratorStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"))));

				//				// The Optimistic Concurrency check allows only one client to update a record at a time.
				//				if ((departmentRow.RowVersion != rowVersion))
				//				{
				//					throw new System.Exception("This record is busy.  Please try again later.");
				//				}
				if (childTable == this.tableSchema)
				{
					iteratorStatements.Add(new CodeCommentStatement("The Optimistic Concurrency check allows only one client to update a record at a time."));
					iteratorStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "RowVersion"), CodeBinaryOperatorType.IdentityInequality, new CodeArgumentReferenceExpression("rowVersion")),
						new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(System.Exception), new CodePrimitiveExpression("This record is busy.  Please try again later.")))));
				}

				DeleteChildren(childTable, iteratorStatements);

				//					// Delete the Department record from the SQL data model.
				//					sqlCommand = new SqlCommand("delete \"Department\" where \"DepartmentId\"=@departmentId", sqlResourceManager.SqlConnection);
				//					sqlCommand.Parameters.Add(new SqlParameter("@departmentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, departmentRow[ServerDataModel.Department.DepartmentIdColumn]));
				//					sqlCommands.Add(sqlCommand);
				if (childTable.IsPersistent)
				{
					iteratorStatements.Add(new CodeCommentStatement(string.Format("Delete the {0} record from the SQL data model.", childTable.Name)));
					string whereClause = string.Empty;
					if (childTable.PrimaryKey != null)
						foreach (ColumnSchema columnSchema in childTable.PrimaryKey.Fields)
							whereClause += string.Format("\"{0}\"=@{1}", columnSchema.Name, Generate.CamelCase(columnSchema.Name));
					string deleteCommandText = string.Format("delete \"{0}\" where {1}", childTable.Name, whereClause);
					iteratorStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sqlCommand"), new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(deleteCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
					if (childTable.PrimaryKey != null)
						foreach (ColumnSchema columnSchema in childTable.PrimaryKey.Fields)
						{
							string variableName = Generate.CamelCase(columnSchema.Name);
							CodeExpression codeExpression = new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(childTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, childTable.Name)), string.Format("{0}Column", columnSchema.Name)));
							iteratorStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), codeExpression }) }));
						}
					iteratorStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteNonQuery"));
				}

				//					// Delete the Department record from the ADO data model.
				//					adoResourceManager.Add(departmentRow);
				//					departmentRow[ServerDataModel.Department.RowVersionColumn] = ServerDataModel.IncrementRowVersion();
				//					departmentRow.Delete();
				iteratorStatements.Add(new CodeCommentStatement(string.Format("Delete the {0} record from the ADO data model.", childTable.Name)));
				iteratorStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeVariableReferenceExpression(rowVariable)));

				CodeTryCatchFinallyStatement tryFinallyStatement = new CodeTryCatchFinallyStatement();
				tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "BeginEdit"));
				tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, childTable.Name)), "RowVersionColumn")), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), "IncrementRowVersion")));
				tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "Delete"));
				tryFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariable), "EndEdit"));
				iteratorStatements.Add(tryFinallyStatement);
				
				CodeStatement[] iteratorArray = new CodeStatement[iteratorStatements.Count];
				iteratorStatements.CopyTo(iteratorArray, 0);
				codeStatements.Add(new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(System.Int32), iteratorVariable, new CodePrimitiveExpression(0)),
					new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(iteratorVariable), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowArrayVariable), "Length")),
					new CodeAssignStatement(new CodeVariableReferenceExpression(iteratorVariable), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(iteratorVariable), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))), iteratorArray));

			}

		}

	}

}
