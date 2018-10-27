/*************************************************************************************************************************
*
*	File:			Archive.cs
*	Description:	Creates the class that can be used to interface to tables in the ADO middle tier.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.MiddleTier
{
	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	public class Archive : ServerMethod
	{

		public Archive(TableSchema tableSchema)
			: base(tableSchema)
		{

			// A base class will change the output format of the generated method.
			if (this.TableSchema.BaseTable == null)
				ArchiveWithoutBase();
			else
				ArchiveWithBase();

		}

		/// <summary>
		/// Creates a method to delete a record in the ADO database.
		/// </summary>
		/// <param name="coreInterfaceClass"></param>
		/// <param name="this.TableSchema.ElementTable"></param>
		private void ArchiveWithoutBase()
		{

			// Shorthand notations for the elements used to construct the interface to this table:
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ServerSchema.Name, this.TableSchema.Name);
			string tableVariableName = string.Format("{0}Table", this.TableSchema.Name[0].ToString().ToLower() + this.TableSchema.Name.Remove(0, 1));
			string rowTypeName = string.Format("{0}.{1}Row", this.ServerSchema.Name, this.TableSchema.Name);
			string rowVariableName = string.Format("{0}Row", this.TableSchema.Name[0].ToString().ToLower() + this.TableSchema.Name.Remove(0, 1));

			// Method Header:
			//        /// <summary>Archives a Algorithm record.</summary>
			//        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
			//        /// <param name="algorithmId">The value for the AlgorithmId column.</param>
			//        /// <param name="rowVersion">The value for the RowVersion column.</param>
			//        public static void Archive(Transaction transaction, int algorithmId, long rowVersion)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Archives a {0} record.</summary>", this.TableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Commits or rejects a set of commands as a unit</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""RowVersion"">The version number of this row.</param>", true));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (this.TableSchema.IsPrimaryKeyColumn(columnSchema))
				{
					string variableName = columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1);
					this.Comments.Add(new CodeCommentStatement(string.Format(@"<param name=""{0}"">The value for the {1} column.</param>", variableName, columnSchema.Name), true));
				}
			this.Comments.Add(new CodeCommentStatement(@"<param name=""archive"">true to archive the object, false to unarchive it.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			if (this.TableSchema.BaseTable != null)
				this.Attributes |= MemberAttributes.New;
			this.Name = "Archive";
			this.Parameters.Add(new CodeParameterDeclarationExpression("AdoTransaction", "adoTransaction"));
			this.Parameters.Add(new CodeParameterDeclarationExpression("SqlTransaction", "sqlTransaction"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(long), "rowVersion"));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (this.TableSchema.IsPrimaryKeyColumn(columnSchema))
				{
					string variableName = columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1);
					this.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, variableName));
				}

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//            // Accessor for the Algorithm Table.
			//            ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.TableSchema.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariableName, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ServerSchema.Name), this.TableSchema.Name)));

			// Rule #1: Make sure the record exists.
			//            // Rule #1: Make sure the record exists before updating it.
			//            ServerMarketData.AlgorithmRow algorithmRow = algorithmTable.FindByAlgorithmId(algorithmId);
			//            if ((algorithmRow == null))
			//            {
			//                throw new Exception(string.Format("The Algorithm table does not have an element identified by {0}", algorithmId));
			//            }
			this.Statements.Add(new CodeCommentStatement(string.Format("Rule #1: Make sure the record exists before updating it.", this.TableSchema.Name)));
			string keyColumns = string.Empty;
			string exeptionFormat = string.Empty;
			CodeExpression[] keyVariables = new CodeExpression[this.TableSchema.PrimaryKey.Fields.Length];
			CodeExpression[] exceptionVariables = new CodeExpression[this.TableSchema.PrimaryKey.Fields.Length + 1];
			for (int index = 0; index < this.TableSchema.PrimaryKey.Fields.Length; index++)
			{
				string columnName = this.TableSchema.PrimaryKey.Fields[index].Name;
				string variableName = columnName[0].ToString().ToLower() + columnName.Remove(0, 1);
				keyColumns += columnName;
				exeptionFormat += string.Format("{{0}}", index);
				keyVariables[index] = new CodeVariableReferenceExpression(variableName);
				exceptionVariables[index + 1] = new CodeVariableReferenceExpression(variableName);
			}
			exceptionVariables[0] = new CodePrimitiveExpression(string.Format("The {0} table does not have an element identified by {1}", this.TableSchema.Name, exeptionFormat));
			this.Statements.Add(new CodeVariableDeclarationStatement(rowTypeName, rowVariableName, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariableName), string.Format("FindBy{0}", keyColumns), keyVariables)));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariableName), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), new CodeThrowExceptionStatement(new CodeObjectCreateExpression("Exception", new CodeExpression[] {new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Format", exceptionVariables)}))));
			
			// Rule #2: Optimistic Concurrency Check.
			//            // Rule #2: Optimistic Concurrency Check
			//            if ((algorithmRow.RowVersion != rowVersion))
			//            {
			//                throw new System.Exception("This record is busy.  Please try again later.");
			//            }
			this.Statements.Add(new CodeCommentStatement("Rule #2: Optimistic Concurrency Check"));
			CodeStatement[] trueTest2Array = new CodeStatement[] {new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(System.Exception), new CodeExpression[] {new CodePrimitiveExpression("This record is busy.  Please try again later.")}))};
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariableName), "RowVersion"), CodeBinaryOperatorType.IdentityInequality, new CodeVariableReferenceExpression("rowVersion")), trueTest2Array));

			//			// Archive the child records.
			//			for (int index = 0; index < securityRow.GetBlotterMapRows().Length; index = index)
			//			{
			//				ServerMarketData.BlotterMapRow blotterMapRow = securityRow.GetBlotterMapRows()[index];
			//				BlotterMap.Archive(transaction, blotterMapRow.BlotterMapId, blotterMapRow.RowVersion);
			//			}
			this.Statements.Add(new CodeCommentStatement("Archive the child records."));
			foreach (KeyrefSchema outerChild in this.TableSchema.ChildKeyrefs)
			{

				TableSchema childTable = outerChild.Selector;
				if (childTable == this.TableSchema)
					continue;
				string childRowTypeName = string.Format("{0}Row", childTable.Name);
				string childRowVariableName = string.Format("child{0}Row", childTable.Name);
				string getRowMethodName = string.Format("Get{0}Rows", childTable.Name);
				ConstraintSchema childPrimaryKey = outerChild.Refer;

				foreach (KeyrefSchema innerChild in this.TableSchema.ChildKeyrefs)
					if (outerChild != innerChild && outerChild.Selector == innerChild.Selector)
					{
						getRowMethodName = string.Format("Get{0}RowsBy{1}", childTable.Name, outerChild.Name);
						break;
					}

				CodeStatement initStatement = new CodeVariableDeclarationStatement(typeof(int), "index", new CodePrimitiveExpression(0));
				CodeExpression testExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("index"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariableName), getRowMethodName, new CodeExpression[] {}), "Length"));
				CodeStatement incrementStatement = new CodeAssignStatement(new CodeVariableReferenceExpression("index"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("index"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
				CodeStatement getRow = new CodeVariableDeclarationStatement(new CodeTypeReference(string.Format("{0}.{1}", this.ServerSchema.Name, childRowTypeName)), childRowVariableName, new CodeArrayIndexerExpression(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariableName), getRowMethodName, new CodeExpression[] {}), new CodeExpression[] {new CodeVariableReferenceExpression("index")}));
				CodeExpression[] deleteArguments = new CodeExpression[childPrimaryKey.Fields.Length + 3];
				deleteArguments[0] = new CodeArgumentReferenceExpression("adoTransaction");
				deleteArguments[1] = new CodeArgumentReferenceExpression("sqlTransaction");
				deleteArguments[2] = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(childRowVariableName), "RowVersion");
				for (int index = 0; index < childPrimaryKey.Fields.Length; index++)
					deleteArguments[index + 3] = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(childRowVariableName), childPrimaryKey.Fields[index].Name);
				string methodName = childTable.BaseTable == null ? "Archive" : "ArchiveChildren";
				CodeStatement deleteStatement = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(childTable.Name), methodName, deleteArguments));
				this.Statements.Add(new CodeIterationStatement(initStatement, testExpression, incrementStatement, new CodeStatement[] {getRow, deleteStatement}));

			}

			// Assign a RowVersion
			//            // Increment the row version.
			//            rowVersion = ServerMarketData.RowVersion.Increment();
			this.Statements.Add(new CodeCommentStatement("Increment the row version"));
			this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ServerSchema.Name), "RowVersion"), "Increment", new CodeExpression[] {})));

			// Delete the record in the ADO database.
			//            // Delete the record in the ADO database.
			//            transaction.AdoTransaction.DataRows.Add(algorithmRow);
			//            algorithmRow.Delete();
			this.Statements.Add(new CodeCommentStatement("Delete the record in the ADO database."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariableName), new CodeExpression[] {new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariableName), "RowVersionColumn")}), new CodeVariableReferenceExpression("rowVersion")));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "DataRows"), "Add", new CodeExpression[] {new CodeVariableReferenceExpression(rowVariableName)}));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(rowVariableName), "Delete", new CodeExpression[] {}));

			// If the table is persistent, then it will be deleted from the SQL database.
			if (this.TableSchema.IsPersistent)
			{

				// Archive the record in the SQL database
				//            // Archive the record in the SQL database.
				//            SqlCommand sqlCommand = new SqlCommand("delete Algorithm where AlgorithmId=@algorithmId");
				//            sqlCommand.Connection = transaction.SqlTransaction.Connection;
				//            sqlCommand.Transaction = transaction.SqlTransaction;
				//            sqlCommand.Parameters.Add("@algorithmId", @algorithmId);
				//            sqlCommand.ExecuteNonQuery();
				this.Statements.Add(new CodeCommentStatement("Archive the record in the SQL database."));
				string whereList = string.Empty;
				foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
					if (this.TableSchema.IsPrimaryKeyColumn(columnSchema))
					{

						string variableName = columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1);
						bool isPrimaryKeyColumn = this.TableSchema.IsPrimaryKeyColumn(columnSchema);

						if (columnSchema.IsPersistent)
						{

							if (isPrimaryKeyColumn)
								whereList += (whereList == string.Empty ? string.Empty : " and ") + string.Format("\"{0}\"=@{1}", columnSchema.Name, variableName);
						}
						else
						{
							if (isPrimaryKeyColumn)
								throw new Exception(string.Format("Primary key element {0} in table {1} is not doesn't exist in the SQL database.", columnSchema.Name, this.TableSchema.Name));
						}

					}
				string deleteCommandText = string.Format("update \"{0}\" set \"IsArchived\" = 1 where {1}", this.TableSchema.Name, whereList);
				this.Statements.Add(new CodeVariableDeclarationStatement("SqlCommand", "sqlCommand", new CodeObjectCreateExpression("SqlCommand", new CodeExpression[] {new CodePrimitiveExpression(deleteCommandText)})));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Connection"), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("sqlTransaction"), "Connection")));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Transaction"), new CodeArgumentReferenceExpression("sqlTransaction")));
				foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
					if (this.TableSchema.IsPrimaryKeyColumn(columnSchema) && columnSchema.IsPersistent)
					{
						string variableName = string.Format("{0}", columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1));
						CodeExpression dataExpression =  (CodeExpression)new CodeArgumentReferenceExpression(variableName);
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("sqlCommand"), "Parameters"), "Add", new CodeExpression[] {new CodeObjectCreateExpression("SqlParameter", new CodeExpression[] {new CodePrimitiveExpression(string.Format("@{0}", variableName)), TypeConverter.Convert(columnSchema.DataType), new CodePrimitiveExpression(0), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("ParameterDirection"), "Input"), new CodePrimitiveExpression(false), new CodePrimitiveExpression(0), new CodePrimitiveExpression(0), new CodePrimitiveExpression(null), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("DataRowVersion"), "Current"), dataExpression})}));
					}
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sqlCommand"), "ExecuteNonQuery", new CodeExpression[] {}));

			}

		}

		/// <summary>
		/// Creates a method to delete a record in the ADO database.
		/// </summary>
		/// <param name="coreInterfaceClass"></param>
		/// <param name="this.TableSchema.ElementTable"></param>
		private void ArchiveWithBase()
		{

			// Shorthand notations for the elements used to construct the interface to this table:
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ServerSchema.Name, this.TableSchema.Name);
			string tableVariableName = string.Format("{0}Table", this.TableSchema.Name[0].ToString().ToLower() + this.TableSchema.Name.Remove(0, 1));
			string rowTypeName = string.Format("{0}.{1}Row", this.ServerSchema.Name, this.TableSchema.Name);
			string rowVariableName = string.Format("{0}Row", this.TableSchema.Name[0].ToString().ToLower() + this.TableSchema.Name.Remove(0, 1));
			string identityColumnName = this.TableSchema.PrimaryKey.Fields[0].Name;
			string identityVariableName = identityColumnName[0].ToString().ToLower() + identityColumnName.Remove(0, 1);

			// Method Header:
			//        /// <summary>Archives a Algorithm record.</summary>
			//        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
			//        /// <param name="algorithmId">The value for the AlgorithmId column.</param>
			//        /// <param name="rowVersion">The value for the RowVersion column.</param>
			//        public static void Archive(Transaction transaction, int algorithmId, long rowVersion)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Archives a {0} record.</summary>", this.TableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Commits or rejects a set of commands as a unit</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""rowVersion"">The version number of this row.</param>", true));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (columnSchema.DeclaringType == this.TableSchema.TypeSchema && this.TableSchema.IsPrimaryKeyColumn(columnSchema))
				{
					string variableName = columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1);
					this.Comments.Add(new CodeCommentStatement(string.Format(@"<param name=""{0}"">The value for the {1} column.</param>", variableName, columnSchema.Name), true));
				}
			this.Comments.Add(new CodeCommentStatement(@"<param name=""archive"">true to archive the object, false to unarchive it.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.New;
			this.Name = "Archive";
			this.Parameters.Add(new CodeParameterDeclarationExpression("AdoTransaction", "adoTransaction"));
			this.Parameters.Add(new CodeParameterDeclarationExpression("SqlTransaction", "sqlTransaction"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(long), "rowVersion"));
			foreach (ColumnSchema columnSchema in this.TableSchema.Columns)
				if (columnSchema.DeclaringType == this.TableSchema.TypeSchema && this.TableSchema.IsPrimaryKeyColumn(columnSchema))
				{
					string variableName = columnSchema.Name[0].ToString().ToLower() + columnSchema.Name.Remove(0, 1);
					this.Parameters.Add(new CodeParameterDeclarationExpression(columnSchema.DataType, variableName));
				}

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//            // Accessor for the Algorithm Table.
			//            ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.TableSchema.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariableName, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ServerSchema.Name), this.TableSchema.Name)));

			// Rule #1: Make sure the record exists.
			//            // Rule #1: Make sure the record exists before updating it.
			//            ServerMarketData.AlgorithmRow algorithmRow = algorithmTable.FindByAlgorithmId(algorithmId);
			//            if ((algorithmRow == null))
			//            {
			//                throw new Exception(string.Format("The Algorithm table does not have an element identified by {0}", algorithmId));
			//            }
			this.Statements.Add(new CodeCommentStatement(string.Format("Rule #1: Make sure the record exists before updating it.", this.TableSchema.Name)));
			string keyColumns = string.Empty;
			string exeptionFormat = string.Empty;
			CodeExpression[] keyVariables = new CodeExpression[this.TableSchema.PrimaryKey.Fields.Length];
			CodeExpression[] exceptionVariables = new CodeExpression[this.TableSchema.PrimaryKey.Fields.Length + 1];
			for (int index = 0; index < this.TableSchema.PrimaryKey.Fields.Length; index++)
			{
				string columnName = this.TableSchema.PrimaryKey.Fields[index].Name;
				string variableName = columnName[0].ToString().ToLower() + columnName.Remove(0, 1);
				keyColumns += columnName;
				exeptionFormat += string.Format("{{0}}", index);
				keyVariables[index] = new CodeVariableReferenceExpression(variableName);
				exceptionVariables[index + 1] = new CodeVariableReferenceExpression(variableName);
			}
			exceptionVariables[0] = new CodePrimitiveExpression(string.Format("The {0} table does not have an element identified by {1}", this.TableSchema.Name, exeptionFormat));
			this.Statements.Add(new CodeVariableDeclarationStatement(rowTypeName, rowVariableName, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariableName), string.Format("FindBy{0}", keyColumns), keyVariables)));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariableName), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), new CodeThrowExceptionStatement(new CodeObjectCreateExpression("Exception", new CodeExpression[] {new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Format", exceptionVariables)}))));
			
			// Rule #2: Optimistic Concurrency Check.
			//            // Rule #2: Optimistic Concurrency Check
			//            if ((algorithmRow.RowVersion != rowVersion))
			//            {
			//                throw new System.Exception("This record is busy.  Please try again later.");
			//            }
			this.Statements.Add(new CodeCommentStatement("Rule #2: Optimistic Concurrency Check"));
			CodeStatement[] trueTest2Array = new CodeStatement[] {new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(System.Exception), new CodeExpression[] {new CodePrimitiveExpression("This record is busy.  Please try again later.")}))};
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariableName), "RowVersion"), CodeBinaryOperatorType.IdentityInequality, new CodeVariableReferenceExpression("rowVersion")), trueTest2Array));

			// This mess determines what the name of the parent row accessor is.  The complication occurs when there are more than 
			// one foreign keys back to the parent table.  In this case, the code generator decorates the name of the row accessor
			// with the Foreign key name.
			bool hasMultipleParentKeys = false;
			KeyrefSchema baseKeyref = null;
			foreach (KeyrefSchema outerKeyref in this.TableSchema.ParentKeyrefs)
			{

				if (outerKeyref.Selector == this.TableSchema.BaseTable)
					baseKeyref = outerKeyref;

				foreach (KeyrefSchema innerKeyref in this.TableSchema.ParentKeyrefs)
					if (outerKeyref != innerKeyref && outerKeyref.Selector == innerKeyref.Selector)
						hasMultipleParentKeys = true;

			}
			string parentRowAccessor = hasMultipleParentKeys ? string.Format("{0}RowBy{1}", this.TableSchema.BaseTable.Name, baseKeyref.Name) :
				string.Format("{0}Row", this.TableSchema.BaseTable.Name);
		
			// Call the base class to delete the base object
			this.Statements.Add(new CodeCommentStatement("Delete the base class record.  Note that optimistic concurrency is only used"));
			this.Statements.Add(new CodeCommentStatement("by the top level type in the hierarchy, it is bypassed after you pass the first test."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "baseRowVersion", new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(rowVariableName), parentRowAccessor), "RowVersion")));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.TableSchema.BaseTable.Name), "Archive", new CodeExpression[] {new CodeArgumentReferenceExpression("adoTransaction"), new CodeArgumentReferenceExpression("sqlTransaction"), new CodeArgumentReferenceExpression("baseRowVersion"), new CodeArgumentReferenceExpression(identityVariableName)}));

		}

	}

}
