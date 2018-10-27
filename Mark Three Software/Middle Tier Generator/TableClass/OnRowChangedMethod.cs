namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Changed event.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class OnRowChangedMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public OnRowChangedMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Raises the DepartmentRowChanged event.
			//			/// </summary>
			//			/// <param name="e">Provides data for the DepartmentRow changing and deleting events.</param>
			//			protected override void OnRowChanged(DataRowChangeEventArgs e)
			//			{
			//				base.OnRowChanged(e);
			//				if ((this.DepartmentRowChanged != null))
			//				{
			//					this.DepartmentRowChanged(this, new DepartmentRowChangeEvent(((DepartmentRow)(e.Row)), e.Action));
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Raises the {0}Changed event.", rowTypeName), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("<param name=\"e\">Provides data for the {0} changing and deleting events.</param>", rowTypeName), true));
			this.Attributes = MemberAttributes.Family | MemberAttributes.Override;
			this.Name = "OnRowChanged";
			this.Parameters.Add(new CodeParameterDeclarationExpression("DataRowChangeEventArgs", "e"));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "OnRowChanged", new CodeExpression[] { new CodeArgumentReferenceExpression("e") }));
			CodeExpression comparisonChanged = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("{0}Changed", rowTypeName)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			CodeStatement[] trueStatementsChanged = new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), string.Format("{0}Changed", rowTypeName), new CodeExpression[] { new CodeThisReferenceExpression(), new CodeObjectCreateExpression(string.Format("{0}ChangeEvent", rowTypeName), new CodeExpression[] { new CodeCastExpression(rowTypeName, new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Row")), new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("e"), "Action") }) })) };
			this.Statements.Add(new CodeConditionStatement(comparisonChanged, trueStatementsChanged));

		}

	}

}
