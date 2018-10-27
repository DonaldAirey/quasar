namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	public class Load : ExternalInterfaceMethod
	{

		public Load(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable) : base (ExternalInterfaceSchema, middleTierTable)
		{

			// The method may have different formats depending on the use of identity columns or the presense of a base class.
			if (!this.MiddleTierTable.IsIdentityClass)
				LoadWithoutIdentity();
			else
			{
				if (this.MiddleTierTable.ElementBaseTable == null)
					LoadWithIdentity();
				else
					LoadWithBase();
			}

		}

		/// <summary>
		/// Creates a Load Method for a table with no identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void LoadWithoutIdentity()
		{

			// Declare the method:
			//        /// <summary>Loads a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Load(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Loads a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Load";
			this.Parameters.Add(new CodeParameterDeclarationExpression("ParameterList", "parameters"));

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//                // Accessor for the AlgorithmType Table.
			//                ServerMarketData.AlgorithmTypeDataTable algorithmTypeTable = ServerMarketData.AlgorithmType;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(tableTypeName), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			// Initialize each parameter to the 'Load' method from the command batch.
			//                // Extract the parameters from the command batch.
			//                string configurationId = method.Parameters["configurationId"];
			//                int algorithmTypeCode = (System.Int32)method.Parameters["algorithmTypeCode"];
			//                object name = method.Parameters["name"];
			//                object description = method.Parameters["description"];
			//                object externalId0 = method.Parameters["externalId0"];
			//                object externalId1 = method.Parameters["externalId1"];
			this.Statements.Add(new CodeCommentStatement("Extract the parameters from the command batch."));
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("adoTransaction") })));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("sqlTransaction") })));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("configurationId") }), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name != "RowVersion")
				{
					bool isKeyColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isKeyColumn = true;
							break;
						}
					Type typeParameter = elementColumn.ElementSchemaType.Datatype.ValueType;
					Type typeVariable = (elementColumn.MinOccurs == 0 || elementColumn.DefaultValue != null) ? typeof(object) : isKeyColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = isKeyColumn ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) }), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) })));
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
					XmlSchemaKeyref keyrefSource = this.ExternalInterfaceSchema.FindForeignKey(this.MiddleTierTable.ElementTable, new XmlSchemaElement[] { elementColumn });
					XmlSchemaIdentityConstraint keySource = keyrefSource == null ? null : this.ExternalInterfaceSchema.FindKey(keyrefSource.Refer.Name);
					// If a source table exists for the key value, the add an instruction to look up the internal identifier.
					if (keySource != null && keySource.Fields.Count == 1)
					{

						XmlSchemaElement elementSourceTable = this.ExternalInterfaceSchema.FindTable(this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector));
						XmlSchemaObject xmlSchemaObject = this.ExternalInterfaceSchema.FindColumn(elementSourceTable.Name, this.ExternalInterfaceSchema.RemoveXPath(keySource.Fields[0]));

						if (xmlSchemaObject is XmlSchemaElement)
						{

							XmlSchemaElement elementSourceColumn = (XmlSchemaElement)xmlSchemaObject;

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
								Type typeVariable = (isPrimaryKeyColumn || elementColumn.MinOccurs != 0) ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
								string externalVariableName = string.Format("external{0}", elementColumn.Name);
								string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
								string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
								string methodName = (isPrimaryKeyColumn || elementColumn.MinOccurs != 0) ? "FindRequiredKey" : "FindOptionalKey";
								this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] { new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName) })));
							}

						}

					}

				}

			// Find the record using the unique code.  If it doesn't exist, then create it.  If it does exist, then update it.
			//                    // Find the record using the unique identifier.  If it doesn't exist, it will be inserted, if it does exist,
			//                    // it will be updated.
			//                    ServerMarketData.AlgorithmTypeRow algorithmTypeRow = algorithmTypeTable.FindByAlgorithmTypeCode(algorithmTypeCode);
			//                    if ((algorithmTypeRow == null))
			//                    {
			this.Statements.Add(new CodeCommentStatement("Find the record using the unique identifier.  If it doesn't exist, it will be inserted, if it does exist,"));
			this.Statements.Add(new CodeCommentStatement("it will be updated."));
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
			CodeExpression innerCondition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariable), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
			CodeStatementCollection innerTrue = new CodeStatementCollection();

			// Call the internal 'Insert' method.
			//                        // Call the internal 'Insert' method to complete the operation.
			//                        Shadows.Web.Service.AlgorithmType.Insert(transaction, algorithmTypeCode, ref rowVersion, name, description, externalId0, externalId1);
			innerTrue.Add(new CodeCommentStatement("Call the internal 'Insert' method to complete the operation."));
			CodeExpressionCollection insertArgs = new CodeExpressionCollection();
			insertArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			insertArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			insertArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				insertArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
			CodeExpression[] insertArgArray = new CodeExpression[insertArgs.Count];
			insertArgs.CopyTo(insertArgArray, 0);
			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;
			innerTrue.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Insert", insertArgArray));

			//                    }
			//                    else
			//                    {
			CodeStatementCollection innerFalse = new CodeStatementCollection();

			// Create a rowVersion to satisfy the optimistic concurrency check of the internal method.
			//                        // The rowVersion is returned to the caller in the event it's needed for other commands in the batch.
			//                        rowVersion = ((long)(algorithmTypeRow[algorithmTypeTable.RowVersionColumn]));
			innerFalse.Add(new CodeCommentStatement("This will bypass the optimistic concurrency checking required by the internal method."));
			innerFalse.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn") }))));

			// An 'Update' method is undefined if the table only has primary key data.
			if (this.MiddleTierTable.NonPrimaryKeyColumns != 0)
			{

				// Call the internal 'Update' method.
				//                        // Call the internal 'Update' method to complete the operation.
				//                        Shadows.Web.Service.AlgorithmType.Update(transaction, algorithmTypeCode, ref rowVersion, name, description, externalId0, externalId1);
				innerFalse.Add(new CodeCommentStatement("Call the internal 'Update' method to complete the operation."));
				CodeExpressionCollection updateArgs = new CodeExpressionCollection();
				updateArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
				updateArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
				updateArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
				foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
					updateArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
				CodeExpression[] updateArgArray = new CodeExpression[updateArgs.Count];
				updateArgs.CopyTo(updateArgArray, 0);
				innerFalse.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));

			}

			// Assemble all the parts of the inner conditional clause.
			//                    }
			CodeStatement[] innerTrueArray = new CodeStatement[innerTrue.Count];
			innerTrue.CopyTo(innerTrueArray, 0);
			CodeStatement[] innerFalseArray = new CodeStatement[innerFalse.Count];
			innerFalse.CopyTo(innerFalseArray, 0);
			this.Statements.Add(new CodeConditionStatement(innerCondition, innerTrueArray, innerFalseArray));

			// The Return parameters.
			//                    // Return values
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values"));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("rowVersion") }), new CodeVariableReferenceExpression("rowVersion")));

		}

		/// <summary>
		/// Creates a Load Method for a table having an automatically generated identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void LoadWithIdentity()
		{

			// Declare the method:
			//        /// <summary>Loads a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Load(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Loads a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.Name = "Load";
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

			// Initialize each parameter to the 'Load' method from the command batch.
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
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("adoTransaction") })));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("sqlTransaction") })));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("configurationId") }), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name != "RowVersion")
				{
					bool isKeyColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isKeyColumn = true;
							break;
						}
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = (this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn) || elementColumn.MinOccurs == 0 || elementColumn.DefaultValue != null) ? typeof(object) : isKeyColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = (isKeyColumn) ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) }), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) })));
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
					// If a source table exists for the key value, the add an instruction to look up the internal identifier.
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
							bool isRequired = elementColumn.MinOccurs != 0 && elementColumn.FixedValue == null && elementColumn.DefaultValue == null;
							Type typeVariable = (isPrimaryKeyColumn || isRequired) ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
							string externalVariableName = string.Format("external{0}", elementColumn.Name);
							string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
							string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
							string methodName = isPrimaryKeyColumn ? "FindKey" : isRequired ? "FindRequiredKey" : "FindOptionalKey";
							this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] { new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName) })));
						}
					}
				}

			// See if the record already exists using it's primary key.
			//                    // The load operation will create a record if it doesn't exist, or update an existing record.  The external
			//                    // identifier is used to determine if a record exists with the same key.
			//                    int algorithmId = Algorithm.FindKey(configurationId, externalAlgorithmId);
			//                    if ((algorithmId == -1))
			//                    {
			this.Statements.Add(new CodeCommentStatement("The load operation will create a record if it doesn't exist, or update an existing record.  The external"));
			this.Statements.Add(new CodeCommentStatement("identifier is used to determine if a record exists with the same key."));
			CodeStatementCollection innerTrue = new CodeStatementCollection();

			// Populate the set of 'ExternalId' variables so the external identifier ends up in the variable index found in the code above.
			//				          int externalKeyIndex = GetExternalKeyIndex(configurationId, "objectId");
			//                        object[] externalIdArray = new object[2];
			//                        externalIdArray[externalKeyIndex] = algorithmId;
			//                        object externalId0 = externalIdArray[0];
			//                        object externalId1 = externalIdArray[1];
			innerTrue.Add(new CodeCommentStatement("Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an"));
			innerTrue.Add(new CodeCommentStatement("external method is called with the same 'configurationId' parameter."));
			innerTrue.Add(new CodeVariableDeclarationStatement(typeof(int), "externalKeyIndex", new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "GetExternalKeyIndex", new CodeExpression[] { new CodeArgumentReferenceExpression("configurationId"), new CodePrimitiveExpression(primaryVariableName) })));
			int externalIdCount = 0;
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name.IndexOf("ExternalId") != -1)
					externalIdCount++;
			innerTrue.Add(new CodeVariableDeclarationStatement(typeof(object[]), "externalIdArray", new CodeArrayCreateExpression(typeof(object), externalIdCount)));
			innerTrue.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("externalIdArray"), new CodeExpression[] { new CodeVariableReferenceExpression("externalKeyIndex") }), new CodeVariableReferenceExpression(string.Format("external{0}", primaryColumnElement.Name))));
			for (int index = 0; index < externalIdCount; index++)
			{
				string externalIdColumnName = string.Format("ExternalId{0}", index);
				string variableName = string.Format("externalId{0}", index);
				innerTrue.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(variableName), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("externalIdArray"), new CodeExpression[] { new CodePrimitiveExpression(index) })));
			}

			// Call the internal 'Insert' method.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Insert(transaction, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection insertArgs = new CodeExpressionCollection();
			insertArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			insertArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			insertArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn))
					insertArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
			CodeExpression[] insertArgArray = new CodeExpression[insertArgs.Count];
			insertArgs.CopyTo(insertArgArray, 0);
			innerTrue.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;
			innerTrue.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Insert", insertArgArray));

			// If the external identifier already exists, then update the record.
			//                    }
			//                    else
			//                    {
			CodeStatementCollection innerFalse = new CodeStatementCollection();

			// Bypass the concurrency checks.
			//                        // While the optimistic concurrency checking is disabled for the external methods, the internal methods 
			//                        // still need to perform the check.  This ncurrency checking logic by finding the current row version to be
			//                        // will bypass the coused when the internal method is called.
			//                        ServerMarketData.AlgorithmRow algorithmRow = algorithmTable.FindByAlgorithmId(algorithmId);
			//                        rowVersion = ((long)(algorithmRow[algorithmTable.RowVersionColumn]));
			innerFalse.Add(new CodeCommentStatement("While the optimistic concurrency checking is disabled for the external methods, the internal methods"));
			innerFalse.Add(new CodeCommentStatement("still need to perform the check.  This ncurrency checking logic by finding the current row version to be"));
			innerFalse.Add(new CodeCommentStatement("will bypass the coused when the internal method is called."));
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
			innerFalse.Add(new CodeVariableDeclarationStatement(string.Format("{0}.{1}Row", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name), rowVariable, new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression(tableVariable), string.Format("FindBy{0}", keyColumns), keyVariables)));
			innerFalse.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn") }))));

			// An 'Update' method is undefined if the table only has primary key data.
			if (this.MiddleTierTable.NonPrimaryKeyColumns != 0)
			{

				// Call the internal 'Update' method with the metadata parameters, the resolved parent keys and the current row version.
				//                        // Call the internal method to complete the operation.
				//                        Shadows.Web.Service.Algorithm.Update(transaction, algorithmId, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
				CodeExpressionCollection updateArgs = new CodeExpressionCollection();
				updateArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
				updateArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
				updateArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
				foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
					updateArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
				CodeExpression[] updateArgArray = new CodeExpression[updateArgs.Count];
				updateArgs.CopyTo(updateArgArray, 0);
				innerFalse.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
				innerFalse.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));

			}

			// Put all the pieces of the inner conditional statement together.
			//                    }
			Type returnType = ((XmlSchemaDatatype)primaryColumnElement.ElementType).ValueType;
			CodeExpression returnValue = returnType == typeof(int) ?
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue") :
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Empty");
			CodeStatement[] innerTrueArray = new CodeStatement[innerTrue.Count];
			innerTrue.CopyTo(innerTrueArray, 0);
			CodeStatement[] innerFalseArray = new CodeStatement[innerFalse.Count];
			innerFalse.CopyTo(innerFalseArray, 0);
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(primaryVariableName), CodeBinaryOperatorType.IdentityEquality, returnValue), innerTrueArray, innerFalseArray));

			// The Return parameters.
			//                    // Return values.
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("rowVersion") }), new CodeVariableReferenceExpression("rowVersion")));

		}

		/// <summary>
		/// Creates a Load Method for a table having an automatically generated identity key.
		/// </summary>
		/// <param name="this.ParentClass"></param>
		private void LoadWithBase()
		{

			// Declare the method:
			//        /// <summary>Loads a Algorithm record using Metadata Parameters.</summary>
			//        /// <param name="parameters">Contains the metadata parameters for this method.</param>
			//        public static void Load(ParameterList parameters)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Loads a {0} record using Metadata Parameters.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""transaction"">Contains the parameters and exceptions for this command.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.New | MemberAttributes.Static;
			this.Name = "Load";
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

			// Initialize each parameter to the 'Load' method from the command batch.
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
			this.Statements.Add(new CodeVariableDeclarationStatement("AdoTransaction", "adoTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("adoTransaction") })));
			this.Statements.Add(new CodeVariableDeclarationStatement("SqlTransaction", "sqlTransaction", new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("sqlTransaction") })));
			if (this.MiddleTierTable.ElementTable.Name != "Configuration")
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "configurationId", new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("configurationId") }), "Value")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsIdentityColumn(this.MiddleTierTable.ElementBaseTable, elementColumn) &&
					elementColumn.FixedValue == null && elementColumn.Name != "RowVersion" &&
					elementColumn.Name.IndexOf("ExternalId") == -1)
				{
					bool isKeyColumn = this.ExternalInterfaceSchema.IsIdentityColumn(elementColumn);
					foreach (XmlSchemaKeyref parentKeyref in this.MiddleTierTable.KeyrefParents)
						if (this.ExternalInterfaceSchema.RemoveXPath(parentKeyref.Fields[0]) == elementColumn.Name)
						{
							isKeyColumn = true;
							break;
						}
					Type typeParameter = ((XmlSchemaDatatype)elementColumn.ElementType).ValueType;
					Type typeVariable = (elementColumn.MinOccurs == 0 || elementColumn.DefaultValue != null) ? typeof(object) : isKeyColumn ? typeof(string) : typeParameter;
					string parameterName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
					string variableName = (isKeyColumn) ? string.Format("external{0}", elementColumn.Name) : parameterName;
					if (typeVariable == typeof(object))
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) }), "Value")));
					else
						this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression(parameterName) })));
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
					// If a source table exists for the key value, the add an instruction to look up the internal identifier.
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
							bool isRequired = elementColumn.MinOccurs != 0 && elementColumn.FixedValue == null && elementColumn.DefaultValue == null;
							Type typeVariable = (isPrimaryKeyColumn || isRequired) ? ((XmlSchemaDatatype)elementColumn.ElementType).ValueType : typeof(object);
							string externalVariableName = string.Format("external{0}", elementColumn.Name);
							string variableName = elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1);
							string className = this.ExternalInterfaceSchema.RemoveXPath(keySource.Selector);
							string methodName = isPrimaryKeyColumn ? "FindKey" : isRequired ? "FindRequiredKey" : "FindOptionalKey";
							this.Statements.Add(new CodeVariableDeclarationStatement(typeVariable, variableName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(className), methodName, new CodeExpression[] { new CodeVariableReferenceExpression("configurationId"), new CodePrimitiveExpression(variableName), new CodeVariableReferenceExpression(externalVariableName) })));
						}
					}
				}

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

			// See if the record already exists using it's primary key.
			//                    // The load operation will create a record if it doesn't exist, or update an existing record.  The external
			//                    // identifier is used to determine if a record exists with the same key.
			//                    int algorithmId = Algorithm.FindKey(configurationId, externalAlgorithmId);
			//                    if ((algorithmId == -1))
			//                    {
			this.Statements.Add(new CodeCommentStatement("The load operation will create a record if it doesn't exist, or update an existing record.  The external"));
			this.Statements.Add(new CodeCommentStatement("identifier is used to determine if a record exists with the same key."));
			CodeStatementCollection innerTrue = new CodeStatementCollection();

			// Populate the set of 'ExternalId' variables so the external identifier ends up in the variable index found in the code above.
			//                        object[] externalIdArray = new object[2];
			//                        externalIdArray[externalKeyIndex] = algorithmId;
			//                        object externalId0 = externalIdArray[0];
			//                        object externalId1 = externalIdArray[1];
			innerTrue.Add(new CodeCommentStatement("Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an"));
			innerTrue.Add(new CodeCommentStatement("external method is called with the same 'configurationId' parameter."));
			innerTrue.Add(new CodeVariableDeclarationStatement(typeof(int), "externalKeyIndex", new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "GetExternalKeyIndex", new CodeExpression[] { new CodeArgumentReferenceExpression("configurationId"), new CodePrimitiveExpression(primaryVariableName) })));
			int externalIdCount = 0;
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (elementColumn.Name.IndexOf("ExternalId") != -1)
					externalIdCount++;
			innerTrue.Add(new CodeVariableDeclarationStatement(typeof(object[]), "externalIdArray", new CodeArrayCreateExpression(typeof(object), externalIdCount)));
			innerTrue.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("externalIdArray"), new CodeExpression[] { new CodeVariableReferenceExpression("externalKeyIndex") }), new CodeVariableReferenceExpression(string.Format("external{0}", primaryColumnElement.Name))));
			for (int index = 0; index < externalIdCount; index++)
			{
				string externalIdColumnName = string.Format("ExternalId{0}", index);
				string variableName = string.Format("externalId{0}", index);
				innerTrue.Add(new CodeVariableDeclarationStatement(typeof(object), variableName, new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("externalIdArray"), new CodeExpression[] { new CodePrimitiveExpression(index) })));
			}

			// Call the internal 'Insert' method.
			//                        // Call the internal method to complete the operation.
			//                        Shadows.Web.Service.Algorithm.Insert(transaction, algorithmTypeCode, ref rowVersion, name, description, assembly, type, method, externalId0, externalId1);
			CodeExpressionCollection insertArgs = new CodeExpressionCollection();
			insertArgs.Add(new CodeArgumentReferenceExpression("adoTransaction"));
			insertArgs.Add(new CodeArgumentReferenceExpression("sqlTransaction"));
			insertArgs.Add(new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("rowVersion")));
			foreach (XmlSchemaElement elementColumn in this.MiddleTierTable.ElementColumns)
				if (!this.ExternalInterfaceSchema.IsPrimaryKeyColumn(this.MiddleTierTable.ElementTable, elementColumn) && elementColumn.FixedValue == null)
					insertArgs.Add(new CodeVariableReferenceExpression(elementColumn.Name[0].ToString().ToLower() + elementColumn.Name.Remove(0, 1)));
			CodeExpression[] insertArgArray = new CodeExpression[insertArgs.Count];
			insertArgs.CopyTo(insertArgArray, 0);
			innerTrue.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
			string internalNamespaceName = this.ExternalInterfaceSchema.InternalNamespace;
			innerTrue.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Insert", insertArgArray));

			// If the external identifier already exists, then update the record.
			//                    }
			//                    else
			//                    {
			CodeStatementCollection innerFalse = new CodeStatementCollection();

			// Bypass the concurrency checks.
			//                        // While the optimistic concurrency checking is disabled for the external methods, the internal methods 
			//                        // still need to perform the check.  This ncurrency checking logic by finding the current row version to be
			//                        // will bypass the coused when the internal method is called.
			//                        ServerMarketData.AlgorithmRow algorithmRow = algorithmTable.FindByAlgorithmId(algorithmId);
			//                        rowVersion = ((long)(algorithmRow[algorithmTable.RowVersionColumn]));
			innerFalse.Add(new CodeCommentStatement("While the optimistic concurrency checking is disabled for the external methods, the internal methods"));
			innerFalse.Add(new CodeCommentStatement("still need to perform the check.  This ncurrency checking logic by finding the current row version to be"));
			innerFalse.Add(new CodeCommentStatement("will bypass the coused when the internal method is called."));
			innerFalse.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rowVersion"), new CodeCastExpression(typeof(long), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(rowVariable), new CodeExpression[] { new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), "RowVersionColumn") }))));

			// An 'Update' method is undefined if the table only has primary key data.
			if (this.MiddleTierTable.NonPrimaryKeyColumns != 0)
			{
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
				innerFalse.Add(new CodeCommentStatement("Call the internal method to complete the operation."));
				innerFalse.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(internalNamespaceName), this.MiddleTierTable.ElementTable.Name), "Update", updateArgArray));

			}

			// Put all the pieces of the inner conditional statement together.
			//                    }
			CodeStatement[] innerTrueArray = new CodeStatement[innerTrue.Count];
			innerTrue.CopyTo(innerTrueArray, 0);
			CodeStatement[] innerFalseArray = new CodeStatement[innerFalse.Count];
			innerFalse.CopyTo(innerFalseArray, 0);
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(rowVariable), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), innerTrueArray, innerFalseArray));

			// The Return parameters.
			//                    // Return values.
			//                    method.Parameters.ReturnValue("rowVersion", rowVersion);
			this.Statements.Add(new CodeCommentStatement("Return values."));
			this.Statements.Add(new CodeAssignStatement(new CodeArrayIndexerExpression(new CodeArgumentReferenceExpression("parameters"), new CodeExpression[] { new CodePrimitiveExpression("rowVersion") }), new CodeVariableReferenceExpression("rowVersion")));

		}

	}

}
