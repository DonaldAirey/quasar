namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Generates the CodeDOM for a strongly typed DataSet from a schema description.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class TypedDataSetClass : CodeTypeDeclaration
	{

		// Private Fields
		private MiddleTierNamespace dataSetNamespace;

		/// <summary>
		/// Creates a strongly typed DataSet from a schema description.
		/// </summary>
		/// <param name="dataSetNamespace">The CodeDOM namespace that contains this strongly typed DataSet.</param>
		public TypedDataSetClass(MiddleTierNamespace dataSetNamespace)
		{

			// Initialize the object.
			this.dataSetNamespace = dataSetNamespace;

			//	/// <summary>
			//	/// A thread-safe, multi-tiered DataModel.
			//	/// </summary>
			//	[System.ComponentModel.DesignerCategoryAttribute("code")]
			//	[System.Diagnostics.DebuggerStepThrough()]
			//	[System.ComponentModel.ToolboxItem(true)]
			//	public class DataModel : System.ComponentModel.Component
			//	{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("A thread-safe, multi-tiered {0}.", this.Schema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.IsClass = true;
			this.Name = this.Schema.Name;
			this.TypeAttributes = TypeAttributes.Public;
			this.BaseTypes.Add(new CodeTypeReference(typeof(Component)));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerCategoryAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression("code"))}));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.ToolboxItem", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression(true))}));

			//		// Counts the number of the DataModel has been referenced.
			//		private static int referenceCount;
			CodeMemberField referenceCountField = new CodeMemberField();
			referenceCountField.Comments.Add(new CodeCommentStatement(string.Format("Counts the number of the {0} has been referenced.", this.Schema.Name)));
			referenceCountField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			referenceCountField.Type = new CodeTypeReference(typeof(System.Int32));
			referenceCountField.Name = "referenceCount";
			this.Members.Add(referenceCountField);

			//		// Represents the in-memory cache of data for the DataModel.
			//		private static DataSet dataSet;
			CodeMemberField dataSetField = new CodeMemberField();
			dataSetField.Comments.Add(new CodeCommentStatement(string.Format("Represents the in-memory cache of data for the {0}.", this.Schema.Name)));
			dataSetField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			dataSetField.Type = new CodeTypeReference("DataSet");
			dataSetField.Name = "dataSet";
			this.Members.Add(dataSetField);

			//		// The Department table
			//		private static DepartmentDataTable tableDepartment;
			foreach (TableSchema tableSchema in this.Schema.Tables)
			{
				CodeMemberField codeMemberField = new CodeMemberField();
				codeMemberField.Comments.Add(new CodeCommentStatement(string.Format("The {0} table", tableSchema.Name)));
				codeMemberField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
				codeMemberField.Type = new CodeTypeReference(String.Format("{0}DataTable", tableSchema.Name));
				codeMemberField.Name = string.Format("table{0}", tableSchema.Name);
				this.Members.Add(codeMemberField);
			}

			//		// Relates the Department table to the Employee table.
			//		private static Relation relationFK_Department_Employee;
			foreach (TableSchema tableSchema in this.Schema.Tables)
				foreach (ConstraintSchema constraintSchema in tableSchema.Constraints)
					if (constraintSchema is KeyrefSchema)
					{
						KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;
						CodeMemberField codeMemberField = new CodeMemberField();
						codeMemberField.Comments.Add(new CodeCommentStatement(string.Format("Relates the {0} table to the {1} table.", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)));
						codeMemberField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
						codeMemberField.Type = new CodeTypeReference("Relation");
						codeMemberField.Name = string.Format("relation{0}", keyrefSchema.Name);
						this.Members.Add(codeMemberField);
					}

			//		/// <summary>
			//		/// Static Constructor for the DataModel.
			//		/// </summary>
			//		static DataModel()
			//		{
			CodeTypeConstructor voidTypeConstructor = new CodeTypeConstructor();
			voidTypeConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			voidTypeConstructor.Comments.Add(new CodeCommentStatement(string.Format("Static Constructor for the {0}.", this.Schema.Name), true));
			voidTypeConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));

			//			// Create the DataModel DataSet
			//			DataModel.dataSet = new DataSet();
			//			DataModel.dataSet.Name = "DataModel";
			//			DataModel.dataSet.CaseSensitive = true;
			//			DataModel.dataSet.EnforceConstraints = true;
			voidTypeConstructor.Statements.Add(new CodeCommentStatement(string.Format("Create the {0} DataSet", this.Schema.Name)));
			voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), new CodeObjectCreateExpression("DataSet", new CodeExpression[] { })));
			voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "DataSetName"), new CodePrimitiveExpression(this.Schema.Name)));
			voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "CaseSensitive"), new CodePrimitiveExpression(true)));
			voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "EnforceConstraints"), new CodePrimitiveExpression(true)));

			//			// Create the Department Table.
			//			DataModel.tableDepartment = new DepartmentDataTable();
			//			DataModel.Tables.Add(DataModel.tableDepartment);
			foreach (TableSchema tableSchema in this.Schema.Tables)
			{
				voidTypeConstructor.Statements.Add(new CodeCommentStatement(string.Format("Create the {0} Table.", tableSchema.Name)));
				voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), String.Format("table{0}", tableSchema.Name)), new CodeObjectCreateExpression(string.Format("{0}DataTable", tableSchema.Name), new CodeExpression[] { })));
				voidTypeConstructor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "Tables"), "Add", new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), String.Format("table{0}", tableSchema.Name))));
			}

			//			// Create and enforce the relation between Department and Employee tables.
			//			DataModel.relationFK_Department_Employee = new Relation("FK_Department_Employee", new Column[] {
			//						DataModel.tableDepartment.DepartmentIdColumn}, new Column[] {
			//						DataModel.tableEmployee.DepartmentIdColumn}, true);
			//			DataModel.Relations.Add(DataModel.relationFK_Department_Employee);
			foreach (TableSchema tableSchema in this.Schema.Tables)
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
							parentFieldList.Add(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), parentTableName), parentColumnName));
						}

						// Collect the referenced fields in the child table.
						List<CodeExpression> childFieldList = new List<CodeExpression>();
						foreach (ColumnSchema columnSchema in keyrefSchema.Fields)
						{
							string childColumnName = String.Format("{0}Column", columnSchema.Name);
							string childTableName = String.Format("table{0}", childTable.Name);
							childFieldList.Add(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), childTableName), childColumnName));
						}

						// Create the CodeDOM statements to create the relationship between the two tables and the foreign key
						// constraint that insures the integrity of the relation.
						voidTypeConstructor.Statements.Add(new CodeCommentStatement(string.Format("Create and enforce the relation between {0} and {1} tables.", parentTable.Name, childTable.Name)));
						CodeObjectCreateExpression newForeignKey = new CodeObjectCreateExpression("Relation", new CodeExpression[] { new CodePrimitiveExpression(keyrefSchema.Name), new CodeArrayCreateExpression("Column", parentFieldList.ToArray()), new CodeArrayCreateExpression("Column", childFieldList.ToArray()), new CodePrimitiveExpression(true) });
						voidTypeConstructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), string.Format("relation{0}", keyrefSchema.Name)), newForeignKey));
						CodeExpression parameterExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), String.Format("relation{0}", keyrefSchema.Name));
						voidTypeConstructor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "Relations"), "Add", new CodeExpression[] { parameterExpression }));

					}

			foreach (TableSchema tableSchema in this.Schema.Tables)
			{

				//			// Initialze the relation fields for the Department table.
				//			DataModel.Department.InitializeRelations();
				voidTypeConstructor.Statements.Add(new CodeCommentStatement(string.Format("Initialze the relation fields for the {0} table.", tableSchema.Name), true));
				voidTypeConstructor.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), tableSchema.Name), "InitializeRelations"));

			}


			//		}
			this.Members.Add(voidTypeConstructor);

			//		/// <summary>
			//		/// Initializer for instances of DataModel not managed as a component.
			//		/// </summary>
			//		public DataModel()
			//		{
			//			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the
			//			// external resources (namely the thread) are destroyed.
			//			DataModel.referenceCount = (DataModel.referenceCount + 1);
			//		}
			CodeConstructor voidConstructor = new CodeConstructor();
			voidConstructor.Attributes = MemberAttributes.Public;
			voidConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			voidConstructor.Comments.Add(new CodeCommentStatement(string.Format("Initializer for instances of {0} not managed as a component.", this.Schema.Name), true));
			voidConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			voidConstructor.Statements.Add(new CodeCommentStatement("This is used to keep track of the number of references to this shared component.  When the count drops to zero, the"));
			voidConstructor.Statements.Add(new CodeCommentStatement("external resources (namely the thread) are destroyed."));
			voidConstructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			this.Members.Add(voidConstructor);

			//		/// <summary>
			//		/// Initializer for instances of DataModel not managed as a component.
			//		/// </summary>
			//		public DataModel(System.ComponentModel.IContainer iContainer)
			//		{
			//			// Add this component to the list of components managed by its client.
			//			iContainer.Add(this);
			//			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the
			//			// external resources (namely the thread) are destroyed.
			//			DataModel.referenceCount = (DataModel.referenceCount + 1);
			//		}
			CodeConstructor iComponentConstructor = new CodeConstructor();
			iComponentConstructor.Attributes = MemberAttributes.Public;
			iComponentConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.ComponentModel.IContainer), "iContainer"));
			iComponentConstructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			iComponentConstructor.Comments.Add(new CodeCommentStatement(string.Format("Initializer for instances of {0} not managed as a component.", this.Schema.Name), true));
			iComponentConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("Add this component to the list of components managed by its client."));
			iComponentConstructor.Statements.Add(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("iContainer"), "Add", new CodeThisReferenceExpression()));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("This is used to keep track of the number of references to this shared component.  When the count drops to zero, the"));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("external resources (namely the thread) are destroyed."));
			iComponentConstructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
			this.Members.Add(iComponentConstructor);

			//		/// <summary>
			//		/// The number of times the DataModel has been referenced.
			//		/// </summary>
			//		[System.ComponentModel.Browsable(false)]
			//		[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//		public static int ReferenceCount
			//		{
			//			get
			//			{
			//				return DataModel.referenceCount;
			//			}
			//		}
			CodeMemberProperty referenceCountProperty = new CodeMemberProperty();
			this.Members.Add(referenceCountProperty);
			referenceCountProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			referenceCountProperty.Comments.Add(new CodeCommentStatement(string.Format("The number of times the {0} has been referenced.", this.Schema.Name), true));
			referenceCountProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			referenceCountProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
			referenceCountProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content")) }));
			referenceCountProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			referenceCountProperty.Type = new CodeTypeReference(typeof(int));
			referenceCountProperty.Name = "ReferenceCount";
			referenceCountProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount")));

			//		/// <summary>
			//		/// Gets or sets a value indicating whether constraint rules are followed when attempting any update operation.
			//		/// </summary>
			//		[System.ComponentModel.Browsable(false)]
			//		[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//		public static bool EnforceConstraints
			//		{
			//			get
			//			{
			//				return DataModel.dataSet.EnforceConstraints;
			//			}
			//			set
			//			{
			//				DataModel.dataSet.EnforceConstraints = value;
			//			}
			//		}
			CodeMemberProperty enforceConstraintsProperty = new CodeMemberProperty();
			enforceConstraintsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			enforceConstraintsProperty.Comments.Add(new CodeCommentStatement("Gets or sets a value indicating whether constraint rules are followed when attempting any update operation.", true));
			enforceConstraintsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			enforceConstraintsProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
			enforceConstraintsProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content"))}));
			enforceConstraintsProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			enforceConstraintsProperty.Type = new CodeTypeReference(typeof(bool));
			enforceConstraintsProperty.Name = "EnforceConstraints";
			enforceConstraintsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "EnforceConstraints")));
			enforceConstraintsProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "EnforceConstraints"), new CodePropertySetValueReferenceExpression()));
			this.Members.Add(enforceConstraintsProperty);

			//		/// <summary>
			//		/// Gets the collection of tables contained in the DataModel.
			//		/// </summary>
			//		[System.ComponentModel.Browsable(false)]
			//		[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//		public static TableCollection Tables
			//		{
			//			get
			//			{
			//				return new TableCollection(DataModel.dataSet.Tables);
			//			}
			//		}
			CodeMemberProperty tablesProperty = new CodeMemberProperty();
			tablesProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			tablesProperty.Comments.Add(new CodeCommentStatement(string.Format("Gets the collection of tables contained in the {0}.", this.Schema.Name), true));
			tablesProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			tablesProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression(false))}));
			tablesProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content"))}));
			tablesProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			tablesProperty.Type = new CodeTypeReference("TableCollection");
			tablesProperty.Name = "Tables";
			tablesProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression("TableCollection", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "Tables")})));
			this.Members.Add(tablesProperty);

			//		/// <summary>
			//		/// Gets the collection of relations that link tables and allow navigation between parent tables and child tables.
			//		/// </summary>
			//		[System.ComponentModel.Browsable(false)]
			//		[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//		public static RelationCollection Relations
			//		{
			//			get
			//			{
			//				return new RelationCollection(DataModel.dataSet.Relations);
			//			}
			//		}
			CodeMemberProperty relationsProperty = new CodeMemberProperty();
			relationsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			relationsProperty.Comments.Add(new CodeCommentStatement("Gets the collection of relations that link tables and allow navigation between parent tables and child tables.", true));
			relationsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			relationsProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression(false))}));
			relationsProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content"))}));
			relationsProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			relationsProperty.Type = new CodeTypeReference("RelationCollection");
			relationsProperty.Name = "Relations";
			relationsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression("RelationCollection", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "Relations")})));
			this.Members.Add(relationsProperty);

			//		/// <summary>
			//		/// Gets an accessor for the Department table.
			//		/// </summary>
			//		[System.ComponentModel.Browsable(false)]
			//		[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//		public static DepartmentDataTable Department
			//		{
			//			get
			//			{
			//				return DataModel.tableDepartment;
			//			}
			//		}
			foreach (TableSchema tableSchema in this.Schema.Tables)
			{
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
				codeMemberProperty.Comments.Add(new CodeCommentStatement(string.Format("Gets an accessor for the {0} table.", tableSchema.Name), true));
				codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
				codeMemberProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
				codeMemberProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content"))}));
				codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
				codeMemberProperty.Type = new CodeTypeReference(string.Format("{0}DataTable", tableSchema.Name));
				codeMemberProperty.Name = tableSchema.Name;
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), string.Format("table{0}", tableSchema.Name))));
				this.Members.Add(codeMemberProperty);
			}

			//		/// <summary>
			//		/// Dispose of an instance of the DataModel.
			//		/// </summary>
			//		protected override void Dispose(bool disposing)
			//		{
			//			// This section disposes of the managed resources.
			//			if ((disposing == true))
			//			{
			//				// This controls the disposal of the static resources.  When the instance count reaches zero, then all static resources
			//				// should be released back to the operating system.
			//				DataModel.referenceCount = (DataModel.referenceCount - 1);
			//			}
			//			// Allow the base class to complete the disposal.
			//			base.Dispose(disposing);
			//		}
			CodeMemberMethod disposeMethod = new CodeMemberMethod();
			disposeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			disposeMethod.Comments.Add(new CodeCommentStatement(string.Format("Dispose of an instance of the {0}.", this.Schema.Name), true));
			disposeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			disposeMethod.Name = "Dispose";
			disposeMethod.Attributes = MemberAttributes.Override | MemberAttributes.Family;
			disposeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.Boolean), "disposing"));
			disposeMethod.Statements.Add(new CodeCommentStatement("This section disposes of the managed resources."));
			CodeStatement[] disposeMethodTrue = new CodeStatement[]
				{
					new CodeCommentStatement("This controls the disposal of the static resources.  When the instance count reaches zero, then all static resources"),
					new CodeCommentStatement("should be released back to the operating system."),
					new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "referenceCount"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)))
				};
			disposeMethod.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("disposing"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)), disposeMethodTrue));
			disposeMethod.Statements.Add(new CodeCommentStatement("Allow the base class to complete the disposal."));
			disposeMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "Dispose", new CodeArgumentReferenceExpression("disposing")));
			this.Members.Add(disposeMethod);

			//		/// <summary>
			//		/// Commits all the changes that were made to this DataModel since the last time DataModel.AcceptChanges was called.
			//		/// </summary>
			//		public static void AcceptChanges()
			//		{
			//			// Commit the changes.
			//			DataModel.dataSet.AcceptChanges();
			//		}
			CodeMemberMethod acceptChangesMethod = new CodeMemberMethod();
			acceptChangesMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			acceptChangesMethod.Comments.Add(new CodeCommentStatement(string.Format("Commits all the changes that were made to this {0} since the last time {0}.AcceptChanges was called.", this.Schema.Name), true));
			acceptChangesMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			acceptChangesMethod.Name = "AcceptChanges";
			acceptChangesMethod.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			acceptChangesMethod.Statements.Add(new CodeCommentStatement("Commit the changes."));
			acceptChangesMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "AcceptChanges", new CodeExpression[] {}));
			this.Members.Add(acceptChangesMethod);

			//		/// <summary>
			//		/// Clears the DataModel of any data by removing all rows in a table.
			//		/// </summary>
			//		public static void Clear()
			//		{
			//			// Clear the DataModel.
			//			DataModel.dataSet.Clear();
			//		}
			CodeMemberMethod clearMethod = new CodeMemberMethod();
			clearMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			clearMethod.Comments.Add(new CodeCommentStatement(string.Format("Clears the {0} of any data by removing all rows in a table.", this.Schema.Name), true));
			clearMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			clearMethod.Name = "Clear";
			clearMethod.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			clearMethod.Statements.Add(new CodeCommentStatement(string.Format("Clear the {0}.", this.Schema.Name)));
			clearMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet"), "Clear", new CodeExpression[] { }));
			this.Members.Add(clearMethod);

			//		/// <summary>
			//		/// Gets the instance of the data model.
			//		/// </summary>
			//		public static System.Data.DataSet GetDataSet()
			//		{
			//			// This is used by the ADO Resource Manager to identify the instance of the data model.
			//			return DataModel.dataSet;
			//		}
			CodeMemberMethod getDataModelMethod = new CodeMemberMethod();
			this.Members.Add(getDataModelMethod);
			getDataModelMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
			getDataModelMethod.Comments.Add(new CodeCommentStatement("Gets the instance of the data model.", true));
			getDataModelMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			getDataModelMethod.Name = "GetDataSet";
			getDataModelMethod.ReturnType = new CodeTypeReference(typeof(System.Data.DataSet));
			getDataModelMethod.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			getDataModelMethod.Statements.Add(new CodeCommentStatement("This is used by the ADO Resource Manager to identify the instance of the data model."));
			getDataModelMethod.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.Schema.Name), "dataSet")));

			//		/// <summary>
			//		/// Delegate for handling changes to the Department table.
			//		/// </summary>
			//		/// <param name="sender">The object that originated the event.</param>
			//		/// <param name="e">The event arguments.</param>
			//		public delegate void DepartmentRowChangeEventHandler(object sender, DepartmentRowChangeEvent e);
			foreach (TableSchema tableSchema in this.Schema.Tables)
			{
				CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate();
				codeTypeDelegate.Comments.Add(new CodeCommentStatement("<summary>", true));
				codeTypeDelegate.Comments.Add(new CodeCommentStatement(string.Format("Delegate for handling changes to the {0} table.", tableSchema.Name), true));
				codeTypeDelegate.Comments.Add(new CodeCommentStatement("</summary>", true));
				codeTypeDelegate.Comments.Add(new CodeCommentStatement("<param name=\"sender\">The object that originated the event.</param>", true));
				codeTypeDelegate.Comments.Add(new CodeCommentStatement("<param name=\"e\">The event arguments.</param>", true));
				codeTypeDelegate.Name = String.Format("{0}RowChangeEventHandler", tableSchema.Name);
				codeTypeDelegate.ReturnType = new CodeTypeReference(typeof(void));
				codeTypeDelegate.TypeAttributes = System.Reflection.TypeAttributes.Public;
				codeTypeDelegate.Parameters.AddRange(new CodeParameterDeclarationExpression[] {new CodeParameterDeclarationExpression(typeof(System.Object), "sender"), new CodeParameterDeclarationExpression(String.Format("{0}RowChangeEvent", tableSchema.Name), "e")});
				this.Members.Add(codeTypeDelegate);
			}			
			
			// This will create the strongly typed table, indices, rows and event handlers for each of the tables in the schema.
			foreach (TableSchema tableSchema in this.Schema.Tables)
			{

				// Add a strongly typed definition for each of the tables to the data model class.
				this.Members.Add(new TypedTableClass(tableSchema));

				// Add a strongly typed index for each of the keys on the table.
				foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
					if (constraintSchema.IsPrimaryKey)
						this.Members.Add(new TypedPrimaryIndexClass(constraintSchema));
					else
					{
						this.Members.Add(new TypedIndexClass(constraintSchema));
					}
				
				// Create the strongly typed rows of the table.
				this.Members.Add(new TypedRowClass(tableSchema));

				// Create the strongly typed event handlers for the table.
				this.Members.Add(new TypedChangeEventArgsClass(tableSchema));

			}
		
		}

		/// <summary>
		/// Gets the Schema that describes this strongly-typed DataSet.
		/// </summary>
		public DataModelSchema Schema { get { return this.dataSetNamespace.Schema; } }

	}

}
