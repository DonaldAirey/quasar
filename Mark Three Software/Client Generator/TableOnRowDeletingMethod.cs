namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Deleting event.
	/// </summary>
	/// <copyright>Copyright � 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableOnRowDeletingMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Deleting event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public TableOnRowDeletingMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
			//                base.OnRowDeleting(e);
			//                if ((this.AccountRowDeleting != null)) {
			//                    this.AccountRowDeleting(this, new AccountRowChangeEvent(((AccountRow)(e.Row)), e.Action));
			//                }
			//            }
			//            
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Raises the {0}Deleting event.", rowTypeName), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"e\">Provides data for the {0} changing and deleting events.</param>", rowTypeName), true));
			this.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			this.Name = "OnRowDeleting";
			this.Parameters.Add(new CodeParameterDeclarationExpression("DataRowChangeEventArgs", "e"));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "OnRowDeleting", new CodeExpression[] { new CodeArgumentReferenceExpression("e") }));
			CodeExpression comparisonDeleting = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("{0}Deleting", rowTypeName)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			CodeStatement[] trueStatementsDeleting = new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), string.Format("{0}Deleting", rowTypeName), new CodeExpression[] { new CodeThisReferenceExpression(), new CodeObjectCreateExpression(string.Format("{0}ChangeEvent", rowTypeName), new CodeExpression[] { new CodeCastExpression(rowTypeName, new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Row")), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Action") }) })) };
			this.Statements.Add(new CodeConditionStatement(comparisonDeleting, trueStatementsDeleting));

		}

	}

}
