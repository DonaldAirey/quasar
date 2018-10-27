namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.Collections.Generic;
	using System.CodeDom;

	/// <summary>
	/// Creates the CodeDOM of a method to update a record using transacted logic.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Update	: CodeMemberMethod
	{

		/// <summary>
		/// Creates the CodeDOM for a method to update a record in a table using transacted logic.
		/// </summary>
		/// <param name="tableSchema">A description of the table.</param>
		public Update(TableSchema tableSchema)
		{

			// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
			// all the table locks that are used for this operation and organizes them in a list that is used to generate the
			// locking and releasing statements below.
			List<LockRequest>tableLockList = new List<LockRequest>();
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
				tableLockList.Add(new WriteRequest(familyTable));
			foreach (KeyrefSchema parentKeyref in tableSchema.ParentKeyrefs)
				if (!tableSchema.IsBaseKeyref(parentKeyref))
					tableLockList.Add(new ReadRequest(parentKeyref.Refer.Selector));
			tableLockList.Sort();

			//		/// <summary>Updates a Employee record.</summary>
			//		/// <param name="age">The value for the Age column.</param>
			//		/// <param name="departmentId">The value for the DepartmentId column.</param>
			//		/// <param name="description">The value for the Description column.</param>
			//		/// <param name="employeeId">The value for the EmployeeId column.</param>
			//		/// <param name="externalId0">The value for the ExternalId0 column.</param>
			//		/// <param name="externalId1">The value for the ExternalId1 column.</param>
			//		/// <param name="name">The value for the Name column.</param>
			//		/// <param name="raceCode">The value for the RaceCode column.</param>
			//		/// <param name="typeCode">The value for the TypeCode column.</param>
			//		public static void Update(object age, object departmentId, object description, int employeeId, object externalId0, object externalId1, object name, object raceCode, object typeCode, ref long rowVersion)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Updates a {0} record.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (!tableSchema.IsPrimaryKeyColumn(columnSchema) || columnSchema.DeclaringType == tableSchema.TypeSchema)
					this.Comments.Add(new CodeCommentStatement(string.Format(@"<param name=""{0}"">The value for the {1} column.</param>", Generate.CamelCase(columnSchema.Name), columnSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""rowVersion"">Used for Optimistic Concurrency Checking.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Update";
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (!tableSchema.IsPrimaryKeyColumn(columnSchema) || columnSchema.DeclaringType == tableSchema.TypeSchema)
				{
					Type typeColumn = columnSchema.DataType;
					Type parameterType = tableSchema.IsPrimaryKeyColumn(columnSchema) ? typeColumn : typeof(object);
					CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression(parameterType, Generate.CamelCase(columnSchema.Name));
					this.Parameters.Add(parameter);
				}
			CodeParameterDeclarationExpression rowVersionParameter = new CodeParameterDeclarationExpression(typeof(long), "rowVersion");
			rowVersionParameter.Direction = FieldDirection.Ref;
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

			//				// The Department record is locked for the duration of the transaction.
			//				ServerDataModel.DepartmentRow departmentRow = ((ServerDataModel.DepartmentRow)(ServerDataModel.Department.FindByDepartmentId(departmentId)));
			//				if ((departmentRow == null))
			//				{
			//					throw new System.Exception(string.Format("Attempt to update a Department record ({0}) that doesn\'t exist", departmentId));
			//				}
			//				departmentRow.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			//				adoResourceManager.Add(departmentRow.ReaderWriterLock);
			this.Statements.Add(new CodeCommentStatement(string.Format("The {0} record is locked for the duration of the transaction.", tableSchema.Name)));
			string rowVariable = string.Format("{0}Row", Generate.CamelCase(tableSchema.Name));
			string rowType = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, tableSchema.Name);
			string findMethodName = string.Format("FindBy");
			string exceptionFormat = string.Empty;
			int parameterCounter = 0;
			List<CodeExpression> methodParameters = new List<CodeExpression>();
			List<CodeExpression> exceptionParameters = new List<CodeExpression>();
			if (tableSchema.PrimaryKey != null)
				foreach (ColumnSchema columnSchema in tableSchema.PrimaryKey.Fields)
				{
					findMethodName += columnSchema.Name;
					methodParameters.Add(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)));
					exceptionParameters.Add(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)));
					exceptionFormat += string.Format("{{0}}", parameterCounter++);
				}
			exceptionParameters.Insert(0, new CodePrimitiveExpression(string.Format("Attempt to update a {0} record ({1}) that doesn't exist", tableSchema.Name, exceptionFormat)));
			this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, tableSchema.Name), rowVariable, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), tableSchema.Name), findMethodName, methodParameters.ToArray())));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariable), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(Exception)), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Format", exceptionParameters.ToArray())))));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			this.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "ReaderWriterLock"))));

			//				// The Optimistic Concurrency check allows only one client to update a record at a time.
			//				if ((departmentRow.RowVersion != rowVersion))
			//				{
			//					throw new System.Exception("This record is busy.  Please try again later.");
			//				}
			this.Statements.Add(new CodeCommentStatement("The Optimistic Concurrency check allows only one client to update a record at a time."));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariable), "RowVersion"), CodeBinaryOperatorType.IdentityInequality, new CodeArgumentReferenceExpression("rowVersion")),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(System.Exception), new CodePrimitiveExpression("This record is busy.  Please try again later.")))));

			//				// The base Department record is locked for the duration of the transaction.
			//				ServerDataModel.ObjectRow objectRow = departmentRow.ObjectRow;
			//				objectRow.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
			TableSchema baseTable0 = tableSchema.BaseTable;
			CodeExpression baseRowAccessor = new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(tableSchema.Name)));
			while (baseTable0 != null)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("The base {0} record is locked for the duration of the transaction.", baseTable0.Name)));
				string baseRowVariable = string.Format("{0}Row", Generate.CamelCase(baseTable0.Name));
				baseRowAccessor = new CodeFieldReferenceExpression(baseRowAccessor, string.Format("{0}Row", baseTable0.Name));
				this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, baseTable0.Name), baseRowVariable, baseRowAccessor));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(baseRowVariable), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
				this.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(baseRowVariable), "ReaderWriterLock"))));
				baseTable0 = baseTable0.BaseTable;
			}

			//				// Lock the current parent Department record for the duration of the transaction.
			//				ServerDataModel.DepartmentRow currentDepartmentRow = employeeRow.DepartmentRow;
			//				currentDepartmentRow.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//				adoResourceManager.Add(currentDepartmentRow);
			//				// Lock the current parent Race record for the duration of the transaction.
			//				if ((employeeRow.RaceRow != null))
			//				{
			//					ServerDataModel.RaceRow currentRaceRow = employeeRow.RaceRow;
			//					currentRaceRow.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//					adoResourceManager.Add(currentRaceRow);
			//				}
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
				foreach (KeyrefSchema parentKeyref in familyTable.ParentKeyrefs)
					if (!familyTable.IsBaseKeyref(parentKeyref))
					{
						this.Statements.Add(new CodeCommentStatement(string.Format("Lock the current parent {0} record for the duration of the transaction.", parentKeyref.Refer.Selector.Name)));
						string parentRowType = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, parentKeyref.Refer.Selector.Name);
						string parentRowVariable = string.Format("current{0}Row", parentKeyref.Refer.Selector.Name);
						string parentAccessor = familyTable.ParentKeyrefCount(parentKeyref.Refer.Selector) == 1 ? string.Format("{0}Row", parentKeyref.Refer.Selector.Name) : string.Format("{0}RowBy{1}", parentKeyref.Refer.Selector.Name, parentKeyref.Name);
						List<CodeStatement> parentRowLock = new List<CodeStatement>();
						parentRowLock.Add(new CodeVariableDeclarationStatement(parentRowType, parentRowVariable, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), parentAccessor)));
						parentRowLock.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowVariable), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite"))));
						parentRowLock.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowVariable), "ReaderWriterLock"))));
						if (parentKeyref.IsNullable)
							this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), parentAccessor), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), parentRowLock.ToArray()));
						else
							this.Statements.AddRange(parentRowLock.ToArray());
					}

			//				// This will provide the defaults elements of the Object table that haven't changed.
			//				if ((description == null))
			//				{
			//					description = objectRow[ServerDataModel.Object.DescriptionColumn];
			//				}
			//				if ((externalId0 == null))
			//				{
			//					externalId0 = objectRow[ServerDataModel.Object.ExternalId0Column];
			//				}
			//				if ((externalId1 == null))
			//				{
			//					externalId1 = objectRow[ServerDataModel.Object.ExternalId1Column];
			//				}
			//				if ((name == null))
			//				{
			//					name = objectRow[ServerDataModel.Object.NameColumn];
			//				}
			//				if ((typeCode == null))
			//				{
			//					typeCode = objectRow[ServerDataModel.Object.TypeCodeColumn];
			//				}
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
			{
				bool isDefaultCommentEmitted = false;
				foreach (ColumnSchema columnSchema in tableSchema.Columns)
					if (!familyTable.IsPrimaryKeyColumn(columnSchema) && columnSchema.DeclaringType == familyTable.TypeSchema)
					{
						if (!isDefaultCommentEmitted)
						{
							this.Statements.Add(new CodeCommentStatement(string.Format("This will provide the defaults elements of the {0} table that haven't changed.", familyTable.Name)));
							isDefaultCommentEmitted = true;
						}
						Type typeVariable = columnSchema.MinOccurs == 0 ? typeof(object) : columnSchema.DataType;
						this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)),
							new CodeAssignStatement(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)), new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), familyTable.Name), string.Format("{0}Column", columnSchema.Name))))));
					}
			}

			//				// Lock the proposed parent Department record for the duration of the transaction.
			//				ServerDataModel.DepartmentRow proposedDepartmentRow = ServerDataModel.Department.FindByDepartmentId(((int)(departmentId)));
			//				proposedDepartmentRow.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//				adoResourceManager.Add(proposedDepartmentRow);
			//				// Lock the proposed parent Race record for the duration of the transaction.
			//				if ((raceCode != null))
			//				{
			//					ServerDataModel.RaceRow proposedRaceRow = ServerDataModel.Race.FindByRaceCode(((int)(raceCode)));
			//					proposedRaceRow.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//					adoResourceManager.Add(proposedRaceRow);
			//				}
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
				foreach (KeyrefSchema parentKeyref in familyTable.ParentKeyrefs)
					if (!familyTable.IsBaseKeyref(parentKeyref))
					{
						this.Statements.Add(new CodeCommentStatement(string.Format("Lock the proposed parent {0} record for the duration of the transaction.", parentKeyref.Refer.Selector.Name)));
						List<CodeStatement> parentRowLock = new List<CodeStatement>();
						string parentRowVariable = string.Format("proposed{0}Row", parentKeyref.Refer.Selector.Name);
						string parentRowType = string.Format("{0}.{1}Row", tableSchema.DataModelSchema.Name, parentKeyref.Refer.Selector.Name);
						string findByMethod0 = string.Format("FindBy");
						List<CodeExpression> methodParameters0 = new List<CodeExpression>();
						foreach (ColumnSchema columnSchema in parentKeyref.Fields)
						{
							findByMethod0 += columnSchema.Name;
							methodParameters0.Add(new CodeCastExpression(columnSchema.DataType, new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name))));
						}
						parentRowLock.Add(new CodeVariableDeclarationStatement(parentRowType, parentRowVariable, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), parentKeyref.Refer.Selector.Name), findByMethod0, methodParameters0.ToArray())));
						parentRowLock.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowVariable), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite"))));
						parentRowLock.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowVariable), "ReaderWriterLock"))));
						CodeExpression lockProposedConditions = null;
						foreach (ColumnSchema columnSchema in parentKeyref.Fields)
							if (columnSchema.MinOccurs == 0)
								lockProposedConditions = lockProposedConditions == null ? new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value")) :
									new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), CodeBinaryOperatorType.BitwiseAnd, lockProposedConditions);
						if (lockProposedConditions == null)
							this.Statements.AddRange(parentRowLock.ToArray());
						else
							this.Statements.Add(new CodeConditionStatement(lockProposedConditions, parentRowLock.ToArray()));
					}

			// This will generate the ADO and SQL updates to the data model for each table in the class hierarchy.
			bool isCommandDeclared = false;
			foreach (TableSchema familyTable in tableSchema.TableHierarchy)
			{

				// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
				// all the table locks that are used for this operation and organizes them in a list that is used to generate the
				// locking and releasing statements below.
				List<TableSchema> parentList = new List<TableSchema>();
				foreach (KeyrefSchema parentKeyref in familyTable.ParentKeyrefs)
					parentList.Add(parentKeyref.Refer.Selector);
				parentList.Sort();

				//			// Update the Object record in the ADO data model.
				//			adoResourceManager.Add(objectRow);
				//			try
				//			{
				//				objectRow.BeginEdit();
				//				objectRow[ServerDataModel.Object.DescriptionColumn] = description;
				//				objectRow[ServerDataModel.Object.ExternalId0Column] = externalId0;
				//				objectRow[ServerDataModel.Object.ExternalId1Column] = externalId1;
				//				objectRow[ServerDataModel.Object.NameColumn] = name;
				//				objectRow[ServerDataModel.Object.TypeCodeColumn] = typeCode;
				//				objectRow[ServerDataModel.Object.RowVersionColumn] = ServerDataModel.IncrementRowVersion();
				//			}
				//			finally
				//			{
				//				objectRow.EndEdit();
				//			}
				this.Statements.Add(new CodeCommentStatement(string.Format("Update the {0} record in the ADO data model.", familyTable.Name)));
				string rowName = string.Format("{0}Row", Generate.CamelCase(familyTable.Name));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeVariableReferenceExpression(rowName)));
				CodeTryCatchFinallyStatement tryFinallyStatement = new CodeTryCatchFinallyStatement();
				tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowName), "BeginEdit"));
				foreach (ColumnSchema columnSchema in familyTable.Columns)
					if (columnSchema.DeclaringType == familyTable.TypeSchema)
					{
						if (columnSchema.IsAutoIncrement || familyTable.IsPrimaryKeyColumn(columnSchema))
							continue;
						tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, familyTable.Name)), string.Format("{0}Column", columnSchema.Name))), new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name))));
					}
				tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, familyTable.Name)), "RowVersionColumn")), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(tableSchema.DataModelSchema.Name), "IncrementRowVersion")));
				tryFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowName), "EndEdit"));
				this.Statements.Add(tryFinallyStatement);

				//				// Update the Object record in the SQL data model.
				//				SqlCommand sqlCommand = new SqlCommand("update \"Object\" set \"Description\"=@description,\"ExternalId0\"=@externalId0,\"Extern" +
				//						"alId1\"=@externalId1,\"Name\"=@name,\"TypeCode\"=@typeCode,\"RowVersion\"=@rowVersion w" +
				//						"here \"ObjectId\"=@objectId", sqlResourceManager.SqlConnection);
				//				sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
				//				sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
				//				sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
				//				sqlCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, name));
				//				sqlCommand.Parameters.Add(new SqlParameter("@objectId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, objectRow[ServerDataModel.Object.ObjectIdColumn]));
				//				sqlCommand.Parameters.Add(new SqlParameter("@typeCode", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, typeCode));
				//				sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, objectRow[ServerDataModel.Object.RowVersionColumn]));
				//				sqlCommand.ExecuteNonQuery();
				//				sqlCommands.Add(sqlCommand);
				if (tableSchema.IsPersistent)
				{
					this.Statements.Add(new CodeCommentStatement(string.Format("Update the {0} record in the SQL data model.", familyTable.Name)));
					string setList = string.Empty;
					foreach (ColumnSchema columnSchema in familyTable.Columns)
						if (columnSchema.IsPersistent && !familyTable.IsPrimaryKeyColumn(columnSchema) && columnSchema.DeclaringType == familyTable.TypeSchema)
							setList += string.Format("\"{0}\"=@{1},", columnSchema.Name, Generate.CamelCase(columnSchema.Name));
					setList += "\"RowVersion\"=@rowVersion";
					string whereClause = string.Empty;
					if (familyTable.PrimaryKey != null)
						foreach (ColumnSchema columnSchema in familyTable.PrimaryKey.Fields)
							whereClause += string.Format("\"{0}\"=@{1}", columnSchema.Name, Generate.CamelCase(columnSchema.Name));
					string insertCommandText = string.Format("update \"{0}\" set {1} where {2}", familyTable.Name, setList, whereClause);
					if (isCommandDeclared)
						this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sqlCommand"), new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(insertCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
					else
					{
						isCommandDeclared = true;
						this.Statements.Add(new CodeVariableDeclarationStatement("SqlCommand", "sqlCommand", new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(insertCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
					}
					foreach (ColumnSchema columnSchema in tableSchema.Columns)
						if (columnSchema.IsPersistent && columnSchema.DeclaringType == familyTable.TypeSchema)
						{
							string variableName = Generate.CamelCase(columnSchema.Name);
							if (familyTable.IsPrimaryKeyColumn(columnSchema))
							{
								CodeExpression codeExpression = new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, familyTable.Name)), string.Format("{0}Column", columnSchema.Name)));
								this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), codeExpression }) }));
							}
							else
								this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), new CodeArgumentReferenceExpression(variableName) }) }));
						}
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeObjectCreateExpression("SqlParameter", new CodePrimitiveExpression("@rowVersion"), new CodeTypeReferenceExpression("SqlDbType.BigInt"), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, familyTable.Name)), "RowVersionColumn")))));
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteNonQuery"));
				}

			}

			//			// Autogenerated values are returned to the caller.
			//			rowVersion = ((long)(employeeRow[ServerDataModel.Employee.RowVersionColumn]));
			this.Statements.Add(new CodeCommentStatement("Autogenerated values are returned to the caller."));
			this.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(tableSchema.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", tableSchema.DataModelSchema.Name, tableSchema.Name)), "RowVersionColumn")))));

		}

	}

}
