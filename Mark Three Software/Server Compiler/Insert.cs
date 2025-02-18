namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.CodeDom;
	using System.Data.SqlClient;

	/// <summary>
	/// Creates the CodeDOM of a method to insert a record using transacted logic.
	/// </summary>
	/// <copyright>Copyright � 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Insert : ServerMethod
	{

		/// <summary>
		/// Creates the CodeDOM for a method to insert a record into a table using transacted logic.
		/// </summary>
		/// <param name="tableSchema">A description of the table.</param>
		public Insert(TableSchema tableSchema)
			: base(tableSchema)
		{

			//		/// <summary>Inserts a Department record.</summary>
			//		/// <param name="departmentId">The value for the DepartmentId column.</param>
			//		/// <param name="description">The value for the Description column.</param>
			//		/// <param name="externalId0">The value for the ExternalId0 column.</param>
			//		/// <param name="externalId1">The value for the ExternalId1 column.</param>
			//		/// <param name="name">The value for the Name column.</param>
			//		/// <param name="typeCode">The value for the TypeCode column.</param>
			//		public static void Insert(out int departmentId, object description, object externalId0, object externalId1, string name, object typeCode, out long rowVersion)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Inserts a {0} record.", this.TableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (!this.TableSchema.IsPrimaryKeyColumn(columnSchema) || columnSchema.DeclaringType == this.TableSchema.TypeSchema)
					this.Comments.Add(new CodeCommentStatement(string.Format(@"<param name=""{0}"">The value for the {1} column.</param>", Generate.CamelCase(columnSchema.Name), columnSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""rowVersion"">Used for Optimistic Concurrency Checking.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Insert";
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (!this.TableSchema.IsPrimaryKeyColumn(columnSchema) || columnSchema.DeclaringType == this.TableSchema.TypeSchema)
				{
					Type typeColumn = columnSchema.DataType;
					Type typeVariable = columnSchema.MinOccurs == 0 || columnSchema.DefaultValue != null ? typeof(object) : typeColumn;
					CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression(typeVariable, Generate.CamelCase(columnSchema.Name));
					if (this.TableSchema.IsAutogeneratedColumn(columnSchema))
						parameter.Direction = FieldDirection.Out;
					this.Parameters.Add(parameter);
				}
			CodeParameterDeclarationExpression rowVersionParameter = new CodeParameterDeclarationExpression(typeof(long), "rowVersion");
			rowVersionParameter.Direction = FieldDirection.Out;
			this.Parameters.Add(rowVersionParameter);

			//			// This method is part of a larger transaction.  Instead of passing the transaction and the resource managers down
			//			// through several layers of methods, they are acccessed as ambient properties of the Transaction class.
			//			Transaction transaction = Transaction.Current;
			//			AdoResourceManager adoResourceManager = ((AdoResourceManager)(transaction["ADO Data Model"]));
			//			SqlResourceManager sqlResourceManager = ((SqlResourceManager)(transaction["SQL Data Model"]));
			this.Statements.Add(new CodeCommentStatement("This method is part of a larger transaction.  Instead of passing the transaction and the resource managers down"));
			this.Statements.Add(new CodeCommentStatement("through several layers of methods, they are acccessed as ambient properties of the Transaction class."));
			this.Statements.Add(new CodeVariableDeclarationStatement("Transaction", "transaction", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Transaction"), "Current")));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoResourceManager", "adoResourceManager", new CodeCastExpression("AdoResourceManager", new CodeIndexerExpression(new CodeVariableReferenceExpression("transaction"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.TableSchema.Name), "VolatileResource")))));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlResourceManager", "sqlResourceManager", new CodeCastExpression("SqlResourceManager", new CodeIndexerExpression(new CodeVariableReferenceExpression("transaction"), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.TableSchema.Name), "DurableResource")))));

			//				// Lock the parent Race record for the duration of the transaction.
			//				if (raceCode != null)
			//				{
			//					ServerDataModel.RaceRow raceRow = ServerDataModel.Race.FindByRaceCode(((int)raceCode));
			//					raceRow.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			//					adoResourceManager.Add(raceRow.ReaderWriterLock);
			//				}
			foreach (KeyrefSchema parentKey in this.TableSchema.ParentKeyrefs)
				if (!this.TableSchema.IsBaseKeyref(parentKey))
				{
					TableSchema parentTable = parentKey.Refer.Selector;
					this.Statements.Add(new CodeCommentStatement(string.Format("Lock the parent {0} record for the duration of the transaction.", parentTable.Name)));
					CodeExpression lockConditions = null;
					foreach (ColumnSchema columnSchema in parentKey.Fields)
						if (columnSchema.MinOccurs == 0)
							lockConditions = lockConditions == null ? new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)) :
								new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), CodeBinaryOperatorType.BitwiseAnd, lockConditions);
					List<CodeStatement> lockParentRow = new List<CodeStatement>();
					string parentRowName = string.Format("{0}Row", Generate.CamelCase(parentTable.Name));
					string methodName = "FindBy";
					List<CodeExpression> keyList = new List<CodeExpression>();
					for (int fieldIndex = 0; fieldIndex < parentKey.Fields.Length; fieldIndex++)
					{
						ColumnSchema parentColumn = parentKey.Refer.Fields[fieldIndex];
						ColumnSchema childColumn = parentKey.Fields[fieldIndex];
						methodName += parentColumn.Name;
						keyList.Add(childColumn.MinOccurs == 0 ? (CodeExpression)new CodeCastExpression(childColumn.DataType, new CodeArgumentReferenceExpression(Generate.CamelCase(childColumn.Name))) :
							(CodeExpression)new CodeArgumentReferenceExpression(Generate.CamelCase(childColumn.Name)));
					}
					lockParentRow.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(string.Format("{0}.{1}Row", this.ServerSchema.Name, parentTable.Name)), parentRowName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, parentTable.Name)), methodName, keyList.ToArray())));
					lockParentRow.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowName), "ReaderWriterLock"), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite"))));
					lockParentRow.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(parentRowName), "ReaderWriterLock"))));
					if (lockConditions == null)
						this.Statements.AddRange(lockParentRow.ToArray());
					else
						this.Statements.Add(new CodeConditionStatement(lockConditions, lockParentRow.ToArray()));
				}

			//            // Apply Fixed Values
			//            int objectTypeCode = 12;
			bool isFixedCommentEmitted = false;
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (columnSchema.FixedValue != null)
				{
					if (!isFixedCommentEmitted)
					{
						this.Statements.Add(new CodeCommentStatement("Apply Fixed Values"));
						isFixedCommentEmitted = true;
					}
					Type typeVariable = columnSchema.MinOccurs == 0 ? typeof(object) : columnSchema.DataType;
					this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, Generate.CamelCase(columnSchema.Name), Generate.PrimativeExpression(columnSchema.FixedValue)));
				}

			//			// Apply Defaults
			//			if ((description == null))
			//			{
			//				description = System.DBNull.Value;
			//			}
			//			if ((externalId0 == null))
			//			{
			//				externalId0 = System.DBNull.Value;
			//			}
			//			if ((externalId1 == null))
			//			{
			//				externalId1 = System.DBNull.Value;
			//			}
			//			if ((typeCode == null))
			//			{
			//				typeCode = "Department";
			//			}
			bool isDefaultCommentEmitted = false;
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (columnSchema.DefaultValue != null)
				{
					if (!isDefaultCommentEmitted)
					{
						this.Statements.Add(new CodeCommentStatement("Apply Defaults"));
						isDefaultCommentEmitted = true;
					}
					Type typeVariable = columnSchema.MinOccurs == 0 ? typeof(object) : columnSchema.DataType;
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), new CodeStatement[] { new CodeAssignStatement(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)), Generate.PrimativeExpression(columnSchema.DefaultValue)) }));
				}

			bool isCommandDeclared = false;
			foreach (TableSchema familyTable in this.TableSchema.TableHierarchy)
			{

				// To reduce the frequency of deadlocking, the tables are always locked in alphabetical order.  This section collects
				// all the table locks that are used for this operation and organizes them in a list that is used to generate the
				// locking and releasing statements below.
				List<TableSchema> parentList = new List<TableSchema>();
				foreach (KeyrefSchema parentKeyref in familyTable.ParentKeyrefs)
					parentList.Add(parentKeyref.Refer.Selector);
				parentList.Sort();

				//			// Add the Object record to the ADO data model.
				//			ServerDataModel.ObjectRow objectRow = ServerDataModel.Object.NewObjectRow();
				//			objectRow.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
				//			adoResourceManager.Add(objectRow.ReaderWriterLock);
				//			adoResourceManager.Add(objectRow);
				//			try
				//			{
				//				ServerDataModel.Object.ReaderWriterLock.AcquireWriterLock(System.Threading.Timeout.Infinite);
				//				ServerDataModel.Object.ReaderWriterLock.AcquireReaderLock(System.Threading.Timeout.Infinite);
				//				objectRow[ServerDataModel.Object.DescriptionColumn] = description;
				//				objectRow[ServerDataModel.Object.ExternalId0Column] = externalId0;
				//				objectRow[ServerDataModel.Object.ExternalId1Column] = externalId1;
				//				objectRow[ServerDataModel.Object.NameColumn] = name;
				//				objectRow[ServerDataModel.Object.TypeCodeColumn] = typeCode;
				//				objectRow[ServerDataModel.Object.RowVersionColumn] = ServerDataModel.IncrementRowVersion();
				//			}
				//			finally
				//			{
				//				ServerDataModel.Object.ReaderWriterLock.ReleaseWriterLock();
				//				ServerDataModel.Object.ReaderWriterLock.ReleaseReaderLock();
				//			}
				string rowName = string.Format("{0}Row", Generate.CamelCase(familyTable.Name));
				this.Statements.Add(new CodeCommentStatement(string.Format("Add the {0} record to the ADO data model.", familyTable.Name)));
				this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", this.ServerSchema.Name, familyTable.Name), rowName, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ServerSchema.Name), familyTable.Name), string.Format("New{0}Row", familyTable.Name))));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowName), "ReaderWriterLock"), "AcquireWriterLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowName), "ReaderWriterLock")));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("adoResourceManager"), "Add", new CodeVariableReferenceExpression(rowName)));
				CodeTryCatchFinallyStatement tryFinallyStatement = new CodeTryCatchFinallyStatement();
				tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowName), "BeginEdit"));
				foreach (ColumnSchema columnSchema in familyTable.Columns)
					if (columnSchema.DeclaringType == familyTable.TypeSchema)
					{
						if (columnSchema.IsAutoIncrement)
							continue;
						if (familyTable.IsInheritedKey(columnSchema))
						{
							TableSchema baseTable = familyTable.BaseTable;
							int keyIndex = Array.IndexOf(familyTable.PrimaryKey.Fields, columnSchema);
							tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), string.Format("{0}Column", columnSchema.Name))),
								new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(baseTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, baseTable.Name)), string.Format("{0}Column", baseTable.PrimaryKey.Fields[keyIndex].Name)))
								));
						}
						else
							tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), string.Format("{0}Column", columnSchema.Name))), new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name))));
					}
				tryFinallyStatement.TryStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeVariableReferenceExpression(rowName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), "RowVersionColumn")), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.ServerSchema.Name), "IncrementRowVersion")));
				tryFinallyStatement.TryStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), string.Format("Add{0}Row", familyTable.Name), new CodeVariableReferenceExpression(rowName)));
				tryFinallyStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowName), "EndEdit"));
				this.Statements.Add(tryFinallyStatement);

				//			// Add the Object record to the SQL data model.
				//			System.Data.SqlClient.SqlCommand sqlCommand = new SqlCommand("insert \"Object\" (\"Description\",\"ExternalId0\",\"ExternalId1\",\"Name\",\"ObjectId\",\"Typ" +
				//					"eCode\",\"RowVersion\") values (@description,@externalId0,@externalId1,@name,@objec" +
				//					"tId,@typeCode,@rowVersion)", sqlResourceManager.SqlConnection);
				//			sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
				//			sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
				//			sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
				//			sqlCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, name));
				//			sqlCommand.Parameters.Add(new SqlParameter("@objectId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, objectRow[ServerDataModel.Object.ObjectIdColumn]));
				//			sqlCommand.Parameters.Add(new SqlParameter("@typeCode", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, typeCode));
				//			sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, objectRow[ServerDataModel.Object.RowVersionColumn]));
				//			sqlCommand.ExecuteNonQuery();
				if (this.TableSchema.IsPersistent)
				{
					this.Statements.Add(new CodeCommentStatement(string.Format("Add the {0} record to the SQL data model.", familyTable.Name)));
					string columnList = string.Empty;
					string variableList = string.Empty;
					foreach (ColumnSchema columnSchema in familyTable.Columns)
						if (columnSchema.IsPersistent && columnSchema.DeclaringType == familyTable.TypeSchema)
						{
							columnList += string.Format("\"{0}\",", columnSchema.Name);
							variableList += string.Format("@{0},", Generate.CamelCase(columnSchema.Name));
						}
					columnList += "\"RowVersion\"";
					variableList += "@rowVersion";
					string insertCommandText = string.Format("insert \"{0}\" ({1}) values ({2})", familyTable.Name, columnList, variableList);
					if (isCommandDeclared)
						this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("sqlCommand"), new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(insertCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
					else
					{
						isCommandDeclared = true;
						this.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.SqlClient.SqlCommand), "sqlCommand", new CodeObjectCreateExpression("SqlCommand", new CodePrimitiveExpression(insertCommandText), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlResourceManager"), "SqlConnection"))));
					}
					foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
						if (columnSchema.IsPersistent && columnSchema.DeclaringType == familyTable.TypeSchema)
						{
							string variableName = Generate.CamelCase(columnSchema.Name);
							if (familyTable.IsAutogeneratedColumn(columnSchema))
							{
								CodeExpression codeExpression = new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), string.Format("{0}Column", columnSchema.Name)));
								this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), codeExpression }) }));
							}
							else
								this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] { new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] { new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), new CodeArgumentReferenceExpression(variableName) }) }));
						}
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeObjectCreateExpression("SqlParameter", new CodePrimitiveExpression("@rowVersion"), new CodeTypeReferenceExpression("SqlDbType.BigInt"), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(familyTable.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, familyTable.Name)), "RowVersionColumn")))));
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteNonQuery"));

				}

			}

			//			// Autogenerated values are returned to the caller.
			//			departmentId = (int)departmentRow[ServerDataModel.Department.DepartmentIdColumn];
			//			rowVersion = (long)departmentRow[ServerDataModel.Department.RowVersionColumn];
			this.Statements.Add(new CodeCommentStatement("Autogenerated values are returned to the caller."));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (this.TableSchema.IsAutogeneratedColumn(columnSchema) && columnSchema.DeclaringType == this.TableSchema.TypeSchema)
					this.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression(Generate.CamelCase(columnSchema.Name)), new CodeCastExpression(columnSchema.DataType, new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(this.TableSchema.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, this.TableSchema.Name)), string.Format("{0}Column", columnSchema.Name))))));
			this.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeIndexerExpression(new CodeVariableReferenceExpression(string.Format("{0}Row", Generate.CamelCase(this.TableSchema.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ServerSchema.Name, this.TableSchema.Name)), "RowVersionColumn")))));

		}

	}

}
