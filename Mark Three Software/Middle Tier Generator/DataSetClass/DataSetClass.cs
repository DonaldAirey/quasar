namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
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
	public class DataSetClass : CodeTypeDeclaration
	{

		/// <summary>
		/// Creates a strongly typed DataSet from a schema description.
		/// </summary>
		/// <param name="dataSetNamespace">The CodeDOM namespace that contains this strongly typed DataSet.</param>
		public DataSetClass(MiddleTierNamespace dataSetNamespace)
		{

			DataModelSchema dataModelSchema = dataSetNamespace.DataModelSchema;

			//	/// <summary>
			//	/// A thread-safe, multi-tiered DataModel.
			//	/// </summary>
			//	[System.ComponentModel.DesignerCategoryAttribute("code")]
			//	[System.Diagnostics.DebuggerStepThrough()]
			//	[System.ComponentModel.ToolboxItem(true)]
			//	public class DataModel : System.ComponentModel.Component
			//	{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("A thread-safe, multi-tiered {0}.", dataModelSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.IsClass = true;
			this.Name = dataModelSchema.Name;
			this.TypeAttributes = TypeAttributes.Public;
			this.BaseTypes.Add(new CodeTypeReference(typeof(Component)));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerCategoryAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression("code"))}));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.ToolboxItem", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression(true))}));

			// Add the constants to the data model.
			this.Members.Add(new ThreadWaitTimeField());
			this.Members.Add(new RowVersionColumnField());
			this.Members.Add(new DeletedTimeColumnField());
			this.Members.Add(new PrimaryKeyOffsetField());
			this.Members.Add(new DurableResourceField());
			this.Members.Add(new VolatileResourceField());

			// Add the private properties to the data model.
			this.Members.Add(new IsGarbageCollectingField());
			this.Members.Add(new DeletedDataSetField());
			this.Members.Add(new MasterRowVersionField());
			this.Members.Add(new FreshnessTimeField());
			this.Members.Add(new GarbageCollectorField());
			this.Members.Add(new DeletedExclusionField());
			this.Members.Add(new DeletedEventField());

			//		// Counts the number of the DataModel has been referenced.
			//		private static int referenceCount;
			CodeMemberField referenceCountField = new CodeMemberField();
			referenceCountField.Comments.Add(new CodeCommentStatement(string.Format("Counts the number of the {0} has been referenced.", dataModelSchema.Name)));
			referenceCountField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			referenceCountField.Type = new CodeTypeReference(typeof(System.Int32));
			referenceCountField.Name = "referenceCount";
			this.Members.Add(referenceCountField);

			//		// Represents the in-memory cache of data for the DataModel.
			//		private static DataSet dataSet;
			CodeMemberField dataSetField = new CodeMemberField();
			dataSetField.Comments.Add(new CodeCommentStatement(string.Format("Represents the in-memory cache of data for the {0}.", dataModelSchema.Name)));
			dataSetField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			dataSetField.Type = new CodeTypeReference("DataSet");
			dataSetField.Name = "dataSet";
			this.Members.Add(dataSetField);

			//		// The Department table
			//		private static DepartmentDataTable tableDepartment;
			foreach (TableSchema tableSchema in dataModelSchema.Tables)
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
			foreach (TableSchema tableSchema in dataModelSchema.Tables)
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


			//		}
			this.Members.Add(new DataSetStaticConstructor(dataModelSchema));

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
			voidConstructor.Comments.Add(new CodeCommentStatement(string.Format("Initializer for instances of {0} not managed as a component.", dataModelSchema.Name), true));
			voidConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			voidConstructor.Statements.Add(new CodeCommentStatement("This is used to keep track of the number of references to this shared component.  When the count drops to zero, the"));
			voidConstructor.Statements.Add(new CodeCommentStatement("external resources (namely the thread) are destroyed."));
			voidConstructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "referenceCount"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
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
			iComponentConstructor.Comments.Add(new CodeCommentStatement(string.Format("Initializer for instances of {0} not managed as a component.", dataModelSchema.Name), true));
			iComponentConstructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("Add this component to the list of components managed by its client."));
			iComponentConstructor.Statements.Add(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("iContainer"), "Add", new CodeThisReferenceExpression()));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("This is used to keep track of the number of references to this shared component.  When the count drops to zero, the"));
			iComponentConstructor.Statements.Add(new CodeCommentStatement("external resources (namely the thread) are destroyed."));
			iComponentConstructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "referenceCount"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
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
			referenceCountProperty.Comments.Add(new CodeCommentStatement(string.Format("The number of times the {0} has been referenced.", dataModelSchema.Name), true));
			referenceCountProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			referenceCountProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
			referenceCountProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content")) }));
			referenceCountProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			referenceCountProperty.Type = new CodeTypeReference(typeof(int));
			referenceCountProperty.Name = "ReferenceCount";
			referenceCountProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "referenceCount")));

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
			enforceConstraintsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "EnforceConstraints")));
			enforceConstraintsProperty.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "EnforceConstraints"), new CodePropertySetValueReferenceExpression()));
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
			tablesProperty.Comments.Add(new CodeCommentStatement(string.Format("Gets the collection of tables contained in the {0}.", dataModelSchema.Name), true));
			tablesProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			tablesProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression(false))}));
			tablesProperty.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content"))}));
			tablesProperty.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			tablesProperty.Type = new CodeTypeReference("TableCollection");
			tablesProperty.Name = "Tables";
			tablesProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression("TableCollection", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "Tables")})));
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
			relationsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression("RelationCollection", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "Relations")})));
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
			foreach (TableSchema tableSchema in dataModelSchema.Tables)
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
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), string.Format("table{0}", tableSchema.Name))));
				this.Members.Add(codeMemberProperty);
			}

			// IsGarbageCollecting Property
			this.Members.Add(new IsGarbageCollectingProperty(dataModelSchema));

			// TableRowChanging Method
			this.Members.Add(new TableRowChangingMethod(dataModelSchema));

			// Dispose Method
			this.Members.Add(new DisposeMethod(dataModelSchema));

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
			acceptChangesMethod.Comments.Add(new CodeCommentStatement(string.Format("Commits all the changes that were made to this {0} since the last time {0}.AcceptChanges was called.", dataModelSchema.Name), true));
			acceptChangesMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			acceptChangesMethod.Name = "AcceptChanges";
			acceptChangesMethod.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			acceptChangesMethod.Statements.Add(new CodeCommentStatement("Commit the changes."));
			acceptChangesMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "AcceptChanges", new CodeExpression[] {}));
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
			clearMethod.Comments.Add(new CodeCommentStatement(string.Format("Clears the {0} of any data by removing all rows in a table.", dataModelSchema.Name), true));
			clearMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
			clearMethod.Name = "Clear";
			clearMethod.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			clearMethod.Statements.Add(new CodeCommentStatement(string.Format("Clear the {0}.", dataModelSchema.Name)));
			clearMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataModelSchema.Name), "dataSet"), "Clear", new CodeExpression[] { }));
			this.Members.Add(clearMethod);

			// GetDataSet method.
			this.Members.Add(new GetDataSetMethod(dataModelSchema));

			// Reconcile method.
			this.Members.Add(new ReconcileMethod(dataModelSchema));

			// IncrementRowVersion method.
			this.Members.Add(new IncrementRowVersionMethod(dataModelSchema));

			// CollectGarbage Method
			this.Members.Add(new CollectGarbageMethod(dataModelSchema));

			//		/// <summary>
			//		/// Delegate for handling changes to the Department table.
			//		/// </summary>
			//		/// <param name="sender">The object that originated the event.</param>
			//		/// <param name="e">The event arguments.</param>
			//		public delegate void DepartmentRowChangeEventHandler(object sender, DepartmentRowChangeEvent e);
			foreach (TableSchema tableSchema in dataModelSchema.Tables)
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
			foreach (TableSchema tableSchema in dataModelSchema.Tables)
			{

				// Add a strongly typed definition for each of the tables to the data model class.
				this.Members.Add(new TableClass.TableClass(tableSchema));

				// Add a strongly typed index for each of the keys on the table.
				foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
					if (constraintSchema.IsPrimaryKey)
						this.Members.Add(new PrimaryIndexClass(constraintSchema));
					else
					{
						this.Members.Add(new IndexClass(constraintSchema));
					}
				
				// Create the strongly typed rows of the table.
				this.Members.Add(new Row.RowClass(tableSchema));

				// Create the strongly typed event handlers for the table.
				this.Members.Add(new ChangeEventArgsClass(tableSchema));

			}

		}

	}

}
