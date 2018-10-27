/*************************************************************************************************************************
*
*	File:			Update.cs
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

	public class Update : ExternalInterfaceMethod
	{

		public Update(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable) : base(ExternalInterfaceSchema, middleTierTable)
		{

			// The method may have different formats depending on the use of identity columns or the presense of a base class.
			if (!this.MiddleTierTable.IsIdentityClass)
				UpdateWithoutIdentity();
			else
			{
				if (this.MiddleTierTable.ElementBaseTable == null)
					UpdateWithoutBase();
				else
					UpdateWithBase();
			}

		}

		/// <summary>
		/// Creates a Update Method for a table with no identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void UpdateWithoutIdentity()
		{

			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Updates a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Update(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Updates a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Update";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement elementPrimaryColumn = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					elementPrimaryColumn = xmlSchemaElement;
			string primaryVariableName = elementPrimaryColumn.Name[0].ToString().ToLower() + elementPrimaryColumn.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the AlgorithmType Table.
			//                ServerMarketData.AlgorithmTypeDataTable algorithmTypeTable = ServerMarketData.AlgorithmType;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Update' method from the command batch.
			//                // Extract the parameters from the command batch.
			//                string configurationId = method.Parameters["configurationId"];
			//                int algorithmTypeCode = (System.Int32)method.Parameters["algorithmTypeCode"];
			//                object name = method.Parameters["name"];
			//                object description = method.Parameters["description"];
			//                object externalId0 = method.Parameters["externalId0"];
			//                object externalId1 = method.Parameters["externalId1"];
			this.Statements.Add(new CodeCommentStatement("Extract the parameters from the command batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("adoTransaction")})));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("sqlTransaction")})));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("configurationId")}), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name != "RowVersion" && elementColumn.Name.IndexOf("ExternalId") == -1)
				{
					bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(this.MiddleTierTable.ElementTable, elementColumn);
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = isPrimaryKeyColumn ? (isExternalIdColumn ? typeof(string) : typeParameter) : typeof(object);
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = isExternalIdColumn ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)}), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)})));
				}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

			// Resolve External Identifiers
			//                    // Resolve External Identifiers
			//                    int internalAlgorithmTypeCode = AlgorithmType.FindRequiredKey(configurationId, algorithmTypeCode);
			bool hasExternalKeyComment = false;
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.FixedValue == null)
				{
					// Primary key columns are required variables.
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					// This will determine if a key value can be used to look up an external user id.
					XmlSchemaKeyref keyrefSource = null;
					foreach (XmlSchemaKeyref innerKeyref in this.ExternalInterfaceSchema.GetParentKeys(this.MiddleTierTable.ElementTable))
					{
						if (this.ExternalInterfaceSchema.RemoveXPath(innerKeyref.Fields[0]) == elementColumn.Name)
							keyrefSource = innerKeyref;
					}
					XmlSchemaIdentityConstraint keySource = keyrefSource == null ? (isPrimaryKeyColumn ? this.MiddleTierTable.PrimaryKey : null) :
						this.ExternalInterfaceSchema.FindKey(keyrefSource.Refer.Name);
					if (keySource != null && keySource.Fields.Count == 1)
					{

						XmlSchemaElement elementSourceTable = this.ExternalInterfaceSchema.FindTable(this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector));
						XmlSchemaObject elementSourceColumn = this.ExternalInterfaceSchema.FindColumn(elementSourceTable.Name, this.ExternalInterfaceSchema.RemoveXPath(keySource.Fields[0]));
						bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(elementSourceTable, elementSourceColumn);

						// If a source table exists for the key value, the add an instruction to look up the internal identifier.
						if (isExternalIdColumn)
						{
							// Display a comment for the first external lookup issued.
							if (!hasExternalKeyComment)
							{
								this.Statements.Add(new CodeCommentStatement("Resolve External Identifiers"));
								hasExternalKeyComment = true;
							}
							Type typeVariable = isPrimaryKeyColumn ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
							string externalVariableName = string.Format("external{0}", elementColumn.Name);
							string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
							string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
							string methodName = isPrimaryKeyColumn ? "FindRequiredKey" : "FindOptionalKey";
							this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName)})));
						}
					}
				}

			// Bypass the concurrency checks.
			//                    // This will bypass the internal optimistic concurrency checking by providing the current rowVersion to the 
			//                    // internal method.
			//                    ServerMarketData.AccountTypeRow accountTypeRow = accountTypeTable.FindByAccountTypeCode(accountTypeCode);
			//                    rowVersion = ((long)(accountTypeRow[accountTypeTable.RowVersionColumn]));
			this.Statements.Add(new CodeCommentStatement("This will bypass the internal optimistic concurrency checking by providing the current rowVersion to the "));
			this.Statements.Add(new CodeCommentStatement("internal method."));
			string keyColumns = string.Empty;
			string exeptionFormat = string.Empty;
			CodeExpression[] keyVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count];
			CodeExpression[] exceptionVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count + 1];
			for (int index = 0; index < this.MiddleTierTable.PrimaryKey.Fields.Count; index++)
			{
				string columnName = this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[index]);
				string variableName = columnName[0].ToString().ToLower() + columnName.Remove(0, 1);
				keyColumns += columnName;
				exeptionFormat += string.Format("{{0}}", index);
				keyVariables[index] = new CodeVariableReferenceExpression(variableName);
				exceptionVariables[index + 1] = new CodeVariableReferenceExpression(variableName);
			}
			this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name), rowVariable, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariable), string.Format("FindBy{0}", keyColumns), keyVariables)));
			this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] {new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn")}))));

			// Call the internal 'Update' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Update(method.Transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection updateArgs = new CodeExpressionCollection();
			updateArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			updateArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			updateArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name.IndexOf("ExternalId") != -1)
					updateArgs.Add(new CodePrimitiveExpression(null));
				else
					updateArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
			CodeExpression[] updateArgArray = new CodeExpression[updateArgs.Count];
			updateArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));
			
			// The Return parameters.
			//                    // Return values.
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("rowVersion")}), new CodeVariableReferenceExpression("rowVersion")));

		}

		/// <summary>
		/// Creates a Update Method for a table with no identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void UpdateWithoutBase()
		{

			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Updates a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Update(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Updates a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Update";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement elementPrimaryColumn = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					elementPrimaryColumn = xmlSchemaElement;
			string primaryVariableName = elementPrimaryColumn.Name[0].ToString().ToLower() + elementPrimaryColumn.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the AlgorithmType Table.
			//                ServerMarketData.AlgorithmTypeDataTable algorithmTypeTable = ServerMarketData.AlgorithmType;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Update' method from the command batch.
			//                // Extract the parameters from the command batch.
			//                string configurationId = method.Parameters["configurationId"];
			//                int algorithmTypeCode = (System.Int32)method.Parameters["algorithmTypeCode"];
			//                object name = method.Parameters["name"];
			//                object description = method.Parameters["description"];
			//                object externalId0 = method.Parameters["externalId0"];
			//                object externalId1 = method.Parameters["externalId1"];
			this.Statements.Add(new CodeCommentStatement("Extract the parameters from the command batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("adoTransaction")})));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("sqlTransaction")})));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("configurationId")}), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name != "RowVersion" && elementColumn.Name.IndexOf("ExternalId") == -1)
				{
					bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(this.MiddleTierTable.ElementTable, elementColumn);
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = isPrimaryKeyColumn ? (isExternalIdColumn ? typeof(string) : typeParameter) : typeof(object);
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = isExternalIdColumn ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)}), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeCastExpression(typeof(string), new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)}))));
				}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

			// Resolve External Identifiers
			//                    // Resolve External Identifiers
			//                    int internalAlgorithmTypeCode = AlgorithmType.FindRequiredKey(configurationId, algorithmTypeCode);
			bool hasExternalKeyComment = false;
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.FixedValue == null)
				{
					// Primary key columns are required variables.
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					// This will determine if a key value can be used to look up an external user id.
					XmlSchemaKeyref keyrefSource = null;
					foreach (XmlSchemaKeyref innerKeyref in this.ExternalInterfaceSchema.GetParentKeys(this.MiddleTierTable.ElementTable))
					{
						if (this.ExternalInterfaceSchema.RemoveXPath(innerKeyref.Fields[0]) == elementColumn.Name)
							keyrefSource = innerKeyref;
					}
					XmlSchemaIdentityConstraint keySource = keyrefSource == null ? (isPrimaryKeyColumn ? this.MiddleTierTable.PrimaryKey : null) :
						this.ExternalInterfaceSchema.FindKey(keyrefSource.Refer.Name);
					if (keySource != null && keySource.Fields.Count == 1)
					{

						XmlSchemaElement elementSourceTable = this.ExternalInterfaceSchema.FindTable(this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector));
						XmlSchemaObject elementSourceColumn = this.ExternalInterfaceSchema.FindColumn(elementSourceTable.Name, this.ExternalInterfaceSchema.RemoveXPath(keySource.Fields[0]));
						bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(elementSourceTable, elementSourceColumn);

						// If a source table exists for the key value, the add an instruction to look up the internal identifier.
						if (isExternalIdColumn)
						{
							// Display a comment for the first external lookup issued.
							if (!hasExternalKeyComment)
							{
								this.Statements.Add(new CodeCommentStatement("Resolve External Identifiers"));
								hasExternalKeyComment = true;
							}
							Type typeVariable = isPrimaryKeyColumn ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
							string externalVariableName = string.Format("external{0}", elementColumn.Name);
							string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
							string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
							string methodName = isPrimaryKeyColumn ? "FindRequiredKey" : "FindOptionalKey";
							this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName)})));
						}
					}
				}

			// Bypass the concurrency checks.
			//                        // While the optimistic concurrency checking is disabled for the external methods, the internal methods 
			//                        // still need to perform the check.  This ncurrency checking logic by finding the current row version to be
			//                        // will bypass the coused when the internal method is called.
			//                        ServerMarketData.AlgorithmRow algorithmRow = algorithmTable.FindByAlgorithmId(algorithmId);
			//                        rowVersion = ((long)(algorithmRow[algorithmTable.RowVersionColumn]));
			this.Statements.Add(new CodeCommentStatement("While the optimistic concurrency checking is disabled for the external methods, the internal methods"));
			this.Statements.Add(new CodeCommentStatement("still need to perform the check.  This ncurrency checking logic by finding the current row version to be"));
			this.Statements.Add(new CodeCommentStatement("will bypass the coused when the internal method is called."));
			string keyColumns = string.Empty;
			string exeptionFormat = string.Empty;
			CodeExpression[] keyVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count];
			CodeExpression[] exceptionVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count + 1];
			for (int index = 0; index < this.MiddleTierTable.PrimaryKey.Fields.Count; index++)
			{
				string columnName = this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[index]);
				string variableName = columnName[0].ToString().ToLower() + columnName.Remove(0, 1);
				keyColumns += columnName;
				exeptionFormat += string.Format("{{0}}", index);
				keyVariables[index] = new CodeVariableReferenceExpression(variableName);
				exceptionVariables[index + 1] = new CodeVariableReferenceExpression(variableName);
			}
			this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name), rowVariable, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariable), string.Format("FindBy{0}", keyColumns), keyVariables)));
			this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] {new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn")}))));

			// Call the internal 'Update' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Update(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection updateArgs = new CodeExpressionCollection();
			updateArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			updateArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			updateArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name.IndexOf("ExternalId") != -1)
					updateArgs.Add(new CodePrimitiveExpression(null));
				else
					updateArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
			CodeExpression[] updateArgArray = new CodeExpression[updateArgs.Count];
			updateArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));
			
			// The Return parameters.
			//                    // Return values.
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("rowVersion")}), new CodeVariableReferenceExpression("rowVersion")));

		}

		/// <summary>
		/// Creates a Update Method for a table having an automatically generated identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void UpdateWithBase()
		{

			// These variables are used in serveral places to describe the table, columns and variables.
			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Updates a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Update(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Updates a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.New | MemberAttributes.Static;
			this.Name = "Update";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement elementPrimaryColumn = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					elementPrimaryColumn = xmlSchemaElement;
			string primaryVariableName = elementPrimaryColumn.Name[0].ToString().ToLower() + elementPrimaryColumn.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the Algorithm Table.
			//                ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Update' method from the command batch.
			//                // Extract the parameters from the command batch.
			//                string configurationId = method.Parameters["configurationId"];
			//                string externalAlgorithmId = (System.String)method.Parameters["algorithmId"];
			//                string externalAlgorithmTypeCode = (System.String)method.Parameters["algorithmTypeCode"];
			//                string name = (System.String)method.Parameters["name"];
			//                object description = method.Parameters["description"];
			//                string assembly = (System.String)method.Parameters["assembly"];
			//                string type = (System.String)method.Parameters["type"];
			//                string method = (System.String)method.Parameters["transaction"];
			this.Statements.Add(new CodeCommentStatement("Extract the parameters from the command batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("adoTransaction")})));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("sqlTransaction")})));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("configurationId")}), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsIdentityColumn(this.MiddleTierTable.ElementBaseTable, elementColumn) &&
					elementColumn.FixedValue == null && elementColumn.Name != "RowVersion" &&
					elementColumn.Name.IndexOf("ExternalId") == -1)
				{
					bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(this.MiddleTierTable.ElementTable, elementColumn);
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = isPrimaryKeyColumn ? (isExternalIdColumn ? typeof(string) : typeParameter) : typeof(object);
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = isExternalIdColumn ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)}), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeCastExpression(typeof(string), new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)}))));
				}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			//                    // event it's needed for operations within the batch.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations.  The value is returned to the caller in the"));
			this.Statements.Add(new CodeCommentStatement("event it's needed for operations within the batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

			// Resolve External Identifiers
			//                    // Resolve External Identifiers
			//                    int internalAlgorithmTypeCode = AlgorithmType.FindRequiredKey(configurationId, algorithmTypeCode);
			bool hasExternalKeyComment = false;
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementBaseTable, elementColumn) &&
					elementColumn.FixedValue == null)
				{
					// Primary key columns are required variables.
					bool isPrimaryKeyColumn = this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn);
					// This will determine if a key value can be used to look up an external user id.
					XmlSchemaKeyref keyrefSource = null;
					foreach (XmlSchemaKeyref innerKeyref in this.ExternalInterfaceSchema.GetParentKeys(this.MiddleTierTable.ElementTable))
					{
						if (this.ExternalInterfaceSchema.RemoveXPath(innerKeyref.Fields[0]) == elementColumn.Name)
							keyrefSource = innerKeyref;
					}
					XmlSchemaIdentityConstraint keySource = keyrefSource == null ? (isPrimaryKeyColumn ? this.MiddleTierTable.PrimaryKey : null) :
						this.ExternalInterfaceSchema.FindKey(keyrefSource.Refer.Name);
					if (keySource != null && keySource.Fields.Count == 1)
					{

						XmlSchemaElement elementSourceTable = this.ExternalInterfaceSchema.FindTable(this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector));
						XmlSchemaObject elementSourceColumn = this.ExternalInterfaceSchema.FindColumn(elementSourceTable.Name, this.ExternalInterfaceSchema.RemoveXPath(keySource.Fields[0]));
						bool isExternalIdColumn = this.ExternalInterfaceSchema.IsExternalIdColumn(elementSourceTable, elementSourceColumn);

						// If a source table exists for the key value, the add an instruction to look up the internal identifier.
						if (isExternalIdColumn)
						{
							// Display a comment for the first external lookup issued.
							if (!hasExternalKeyComment)
							{
								this.Statements.Add(new CodeCommentStatement("Resolve External Identifiers"));
								hasExternalKeyComment = true;
							}
							Type typeVariable = isPrimaryKeyColumn ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
							string externalVariableName = string.Format("external{0}", elementColumn.Name);
							string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
							string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
							string methodName = isPrimaryKeyColumn ? "FindRequiredKey" : "FindOptionalKey";
							this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName)})));
						}
					}
				}

			//                    // This disables the concurrency checking logic by finding the current row version and passing it to the 
			//                    // internal method.
			//                    ServerMarketData.LoginRow loginRow = loginTable.FindByLoginId(loginId);
			//                    rowVersion = ((long)(loginRow[loginTable.RowVersionColumn]));
			this.Statements.Add(new CodeCommentStatement("This disables the concurrency checking logic by finding the current row version and passing it to the"));
			this.Statements.Add(new CodeCommentStatement("internal method."));
			string keyColumns = string.Empty;
			string exeptionFormat = string.Empty;
			CodeExpression[] keyVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count];
			CodeExpression[] exceptionVariables = new CodeExpression[this.MiddleTierTable.PrimaryKey.Fields.Count + 1];
			for (int index = 0; index < this.MiddleTierTable.PrimaryKey.Fields.Count; index++)
			{
				string columnName = this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[index]);
				string variableName = columnName[0].ToString().ToLower() + columnName.Remove(0, 1);
				keyColumns += columnName;
				exeptionFormat += string.Format("{{0}}", index);
				keyVariables[index] = new CodeVariableReferenceExpression(variableName);
				exceptionVariables[index + 1] = new CodeVariableReferenceExpression(variableName);
			}
			this.Statements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name), rowVariable, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariable), string.Format("FindBy{0}", keyColumns), keyVariables)));
			this.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] {new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn")}))));

			// Call the internal 'Update' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Update(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection updateArgs = new CodeExpressionCollection();
			updateArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			updateArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			updateArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementBaseTable, elementColumn) && elementColumn.FixedValue == null)
				{
					if (elementColumn.Name.IndexOf("ExternalId") != -1)
						updateArgs.Add(new CodePrimitiveExpression(null));
					else
						updateArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
				}
			CodeExpression[] updateArgArray = new CodeExpression[updateArgs.Count];
			updateArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));
			
			// The Return parameters.
			//                    // Return values.
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression("rowVersion")}), new CodeVariableReferenceExpression("rowVersion")));

		}

	}

}
