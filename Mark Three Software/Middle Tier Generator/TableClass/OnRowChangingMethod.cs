namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Changing event.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class OnRowChangingMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changing event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public OnRowChangingMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//            protected override void OnRowChanging(DataRowChangeEventArgs e) {
			//                base.OnRowChanging(e);
			//                if ((this.AccountRowChanging != null)) {
			//                    this.AccountRowChanging(this, new AccountRowChangeEvent(((AccountRow)(e.Row)), e.Action));
			//                }
			//            }
			//            
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Raises the {0}Changing event.", rowTypeName), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"e\">Provides data for the {0} changing and deleting events.</param>", rowTypeName), true));
			this.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			this.Name = "OnRowChanging";
			this.Parameters.Add(new CodeParameterDeclarationExpression("DataRowChangeEventArgs", "e"));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "OnRowChanging", new CodeExpression[] { new CodeArgumentReferenceExpression("e") }));
			CodeExpression comparisonChanging = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("{0}Changing", rowTypeName)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			CodeStatement[] trueStatementsChanging = new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), string.Format("{0}Changing", rowTypeName), new CodeExpression[] { new CodeThisReferenceExpression(), new CodeObjectCreateExpression(string.Format("{0}ChangeEvent", rowTypeName), new CodeExpression[] { new CodeCastExpression(rowTypeName, new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Row")), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Action") }) })) };
			this.Statements.Add(new CodeConditionStatement(comparisonChanging, trueStatementsChanging));

		}

	}

}
