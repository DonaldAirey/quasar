namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Data;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description of a strongly typed changed row event argument.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class ChangeEventArgsClass : CodeTypeDeclaration
	{

		// Private Members
		private TableSchema tableSchema;

		/// <summary>
		/// Implements a DataTable.
		/// </summary>
		/// <param name="tableSchema">The table schema that describes the event arguments.</param>
		public ChangeEventArgsClass(TableSchema tableSchema)
		{

			// The row change arguments are designed for this table.
			this.tableSchema = tableSchema;

			// Construct the type names for the table and rows within the table.
			string tableTypeName = string.Format("{0}DataTable", tableSchema.Name);
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);
			string eventTypeName = string.Format("{0}ChangeEvent", rowTypeName);
			string rowVariableName = string.Format("{0}Row", tableSchema.Name[0].ToString().ToLower() + tableSchema.Name.Remove(0, 1));

			//		/// <summary>
			//		/// Arguments for the event that indicates a change in a Department table row.
			//		/// </summary>
			//		[System.Diagnostics.DebuggerStepThrough()]
			//		public class DepartmentRowChangeEvent : EventArgs
			//		{
			CodeTypeDeclaration tableClass = new CodeTypeDeclaration();
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Arguments for the event that indicates a change in a {0} table row.", this.tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.TypeAttributes = TypeAttributes.Public;
			this.IsClass = true;
			this.Name = eventTypeName;
			this.BaseTypes.Add("EventArgs");

			//			/// <summary>
			//			/// The Department row that has been changed.
			//			/// </summary>
			//			private DepartmentRow departmentRow;
			CodeMemberField tableRowField = new CodeMemberField(new CodeTypeReference(rowTypeName), rowVariableName);
			tableRowField.Attributes = MemberAttributes.Private;
			tableRowField.Comments.Add(new CodeCommentStatement("<summary>", true));
			tableRowField.Comments.Add(new CodeCommentStatement(string.Format("The {0} row that has been changed.", this.tableSchema.Name), true));
			tableRowField.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Members.Add(tableRowField);

			//			/// <summary>
			//			/// The action that caused the change to the row.
			//			/// </summary>
			//			private DataRowAction dataRowAction;
			CodeMemberField dataRowActionField = new CodeMemberField(new CodeTypeReference("DataRowAction"), "dataRowAction");
			dataRowActionField.Comments.Add(new CodeCommentStatement("<summary>", true));
			dataRowActionField.Comments.Add(new CodeCommentStatement("The action that caused the change to the row.", true));
			dataRowActionField.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Members.Add(dataRowActionField);

			//			/// <summary>
			//			/// Create the arguments for a changing Department row event.
			//			/// </summary>
			//			/// <param name="departmentRow">The Department row that has changed.</param>
			//			/// <param name="dataRowAction">The action that caused the change.</param>
			//			public DepartmentRowChangeEvent(DepartmentRow departmentRow, DataRowAction dataRowAction)
			//			{
			//				// Initialize the object.
			//				this.departmentRow = departmentRow;
			//				this.dataRowAction = dataRowAction;
			//			}
			CodeConstructor constructor = new CodeConstructor();
			this.Members.Add(constructor);
			constructor.Comments.Add(new CodeCommentStatement("<summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("Create the arguments for a changing {0} row event.", this.tableSchema.Name), true));
			constructor.Comments.Add(new CodeCommentStatement("</summary>", true));
			constructor.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"{0}\">The {1} row that has changed.</param>", rowVariableName, this.tableSchema.Name), true));
			constructor.Comments.Add(new CodeCommentStatement("<param name=\"dataRowAction\">The action that caused the change.</param>", true));
			constructor.Attributes = MemberAttributes.Public;
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(rowTypeName, rowVariableName));
			constructor.Parameters.Add(new CodeParameterDeclarationExpression("DataRowAction", "dataRowAction"));
			constructor.Statements.Add(new CodeCommentStatement("Initialize the object."));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), rowVariableName), new CodeArgumentReferenceExpression(rowVariableName)));
			constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataRowAction"), new CodeArgumentReferenceExpression("dataRowAction")));

			//			/// <summary>
			//			/// Gets the Department row that has been changed.
			//			/// </summary>
			//			public DepartmentRow DepartmentRow
			//			{
			//				get
			//				{
			//					return this.departmentRow;
			//				}
			//			}
			CodeMemberProperty tableRowProperty = new CodeMemberProperty();
			tableRowProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			tableRowProperty.Comments.Add(new CodeCommentStatement(string.Format("Gets the {0} row that has been changed.", this.tableSchema.Name), true));
			tableRowProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			tableRowProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			tableRowProperty.Type = new CodeTypeReference(rowTypeName);
			tableRowProperty.Name = rowTypeName;
			tableRowProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), rowVariableName)));
			this.Members.Add(tableRowProperty);

			//			/// <summary>
			//			/// Gets the action that caused the change to the row.
			//			/// </summary>
			//			public DataRowAction Action
			//			{
			//				get
			//				{
			//					return this.dataRowAction;
			//				}
			//			}
			CodeMemberProperty actionProperty = new CodeMemberProperty();
			actionProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
			actionProperty.Comments.Add(new CodeCommentStatement("Gets the action that caused the change to the row.", true));
			actionProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
			actionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			actionProperty.Type = new CodeTypeReference("DataRowAction");
			actionProperty.Name = "Action";
			actionProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dataRowAction")));
			this.Members.Add(actionProperty);

			//		}

		}

	}

}
