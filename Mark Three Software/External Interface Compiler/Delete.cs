/*************************************************************************************************************************
*
*	File:			Delete.cs
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

	public class Delete : ExternalInterfaceMethod
	{

		public Delete(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable) : base(ExternalInterfaceSchema, middleTierTable)
		{

			// The method may have different formats depending on the use of identity columns or the presense of a base class.
			if (!this.MiddleTierTable.IsIdentityClass)
				DeleteWithoutIdentity();
			else
			{
				if (this.MiddleTierTable.ElementBaseTable == null)
					DeleteWithoutBase();
				else
					DeleteWithBase();
			}

		}

		/// <summary>
		/// Creates a Delete Method for a table with no identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void DeleteWithoutIdentity()
		{

			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Deletes a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Delete(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Deletes a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Delete";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement primaryColumnElement = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					primaryColumnElement = xmlSchemaElement;
			string primaryVariableName = primaryColumnElement.Name[0].ToString().ToLower() + primaryColumnElement.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the AlgorithmType Table.
			//                ServerMarketData.AlgorithmTypeDataTable algorithmTypeTable = ServerMarketData.AlgorithmType;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Delete' method from the command batch.
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
				if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
				{
					bool isIdentityColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isIdentityColumn = true;
							break;
						}
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = (elementColumn.MinOccurs == 0 || elementColumn.DefaultValue != null) ? typeof(object) : isIdentityColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = (isIdentityColumn) ? string.Format("external{0}", elementColumn.Name) : parameterName;
					this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)})));
				}

			// Resolve External Identifier
			//                    // Resolve External Identifiers
			//                    int internalAlgorithmTypeCode = AlgorithmType.FindRequiredKey(configurationId, algorithmTypeCode);
			if (this.MiddleTierTable.KeyrefParents.Length != 0)
			{
				this.Statements.Add(new CodeCommentStatement("Resolve External Identifiers"));
				foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
					if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
						foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
							if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
							{
								XmlSchemaIdentityConstraint parentKey = this.ExternalInterfaceSchema.FindKey(parentKeyref.Refer.Name);
								Type typeVariable = elementColumn.MinOccurs == 0 ? typeof(object) : ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
								string externalVariableName = string.Format("external{0}", elementColumn.Name);
								string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
								string className = this.ExternalInterfaceSchema.RemoveXPath(parentKey.Selector);
								string methodName = elementColumn.MinOccurs == 0 ? "FindOptionalKey" : "FindRequiredKey";
								this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName)})));
							}
			}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

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

			// Call the internal 'Delete' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Delete(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection deleteArgs = new CodeExpressionCollection();
			deleteArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("rowVersion"));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
				{
					string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					deleteArgs.Add(new CodeVariableReferenceExpression(variableName));
				}
			CodeExpression[] updateArgArray = new CodeExpression[deleteArgs.Count];
			deleteArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Delete", updateArgArray));
			
		}

		/// <summary>
		/// Creates a Delete Method for a table with no identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void DeleteWithoutBase()
		{

			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Deletes a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Delete(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Deletes a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Delete";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement primaryColumnElement = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					primaryColumnElement = xmlSchemaElement;
			string primaryVariableName = primaryColumnElement.Name[0].ToString().ToLower() + primaryColumnElement.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the AlgorithmType Table.
			//                ServerMarketData.AlgorithmTypeDataTable algorithmTypeTable = ServerMarketData.AlgorithmType;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Delete' method from the command batch.
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
				if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
				{
					bool isIdentityColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isIdentityColumn = true;
							break;
						}
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = (elementColumn.MinOccurs == 0 || elementColumn.DefaultValue != null) ? typeof(object) : isIdentityColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = (isIdentityColumn) ? string.Format("external{0}", elementColumn.Name) : parameterName;
					this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)})));
				}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

			// See if the record already exists using it's primary key.
			//                    // The load operation will create a record if it doesn't exist, or update an existing record.  The external
			//                    // identifier is used to determine if a record exists with the same key.
			//                    int algorithmId = Algorithm.FindKey(configurationId, externalAlgorithmId);
			this.Statements.Add(new CodeCommentStatement("Find the internal identifier using the primar key elements."));
			this.Statements.Add(new CodeCommentStatement("identifier is used to determine if a record exists with the same key."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), primaryVariableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "FindRequiredKey", new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(primaryVariableName), new CodeVariableReferenceExpression(string.Format("external{0}", primaryColumnElement.Name))})));

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

			// Call the internal 'Delete' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Delete(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection deleteArgs = new CodeExpressionCollection();
			deleteArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("rowVersion"));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
				{
					string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					deleteArgs.Add(new CodeVariableReferenceExpression(variableName));
				}
			CodeExpression[] updateArgArray = new CodeExpression[deleteArgs.Count];
			deleteArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Delete", updateArgArray));
			
		}

		/// <summary>
		/// Creates a Delete Method for a table having an automatically generated identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void DeleteWithBase()
		{

			// These variables are used in serveral places to describe the table, columns and variables.
			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;

			// Declare the method:
			//        /// <summary>Deletes a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Delete(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Deletes a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.New | MemberAttributes.Static;
			this.Name = "Delete";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Find the primary column element.
			XmlSchemaElement primaryColumnElement = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					primaryColumnElement = xmlSchemaElement;
			string primaryVariableName = primaryColumnElement.Name[0].ToString().ToLower() + primaryColumnElement.Name.Remove(0, 1);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the Algorithm Table.
			//                ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Delete' method from the command batch.
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
				if (this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn) &&
					!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementBaseTable, elementColumn))
				{
					bool isIdentityColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isIdentityColumn = true;
							break;
						}
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = isIdentityColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = (isIdentityColumn) ? string.Format("external{0}", elementColumn.Name) : parameterName;
					this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] {new CodePrimitiveExpression(parameterName)})));

				}

			// Declare a rowVersion
			//                    // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
			//                    // event it's needed for operations within the batch.
			//                    long rowVersion = long.MinValue;
			this.Statements.Add(new CodeCommentStatement("The row versioning is largely disabled for external operations.  The value is returned to the caller in the"));
			this.Statements.Add(new CodeCommentStatement("event it's needed for operations within the batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(long), "rowVersion", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(long)), "MinValue")));

			// See if the record already exists using it's primary key.
			//                    // Find the internal identifier using the primary key elements.
			//                    int loginId = Login.FindKey(configurationId, "loginId", externalLoginId);
			this.Statements.Add(new CodeCommentStatement("Find the internal identifier using the primary key elements."));
			this.Statements.Add(new CodeCommentStatement("identifier is used to determine if a record exists with the same key."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), primaryVariableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "FindRequiredKey", new CodeExpression[] {new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(primaryVariableName), new CodeVariableReferenceExpression(string.Format("external{0}", primaryColumnElement.Name))})));

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

			// Call the internal 'Delete' method with the metadata parameters, the resolved parent keys and the current row version.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Delete(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection deleteArgs = new CodeExpressionCollection();
			deleteArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			deleteArgs.Add(new CodeArgumentReferenceExpression("rowVersion"));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if ((this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn) &&
					!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementBaseTable, elementColumn)))
				{
					string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					deleteArgs.Add(new CodeVariableReferenceExpression(variableName));
				}
			CodeExpression[] updateArgArray = new CodeExpression[deleteArgs.Count];
			deleteArgs.CopyTo(updateArgArray, 0);
			this.Statements.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Delete", updateArgArray));
			
		}

	}

}
