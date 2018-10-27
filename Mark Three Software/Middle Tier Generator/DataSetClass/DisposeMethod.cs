namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to dispose of the resources used by the data model.
	/// </summary>
	class DisposeMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to dispose of the resources used by the data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public DisposeMethod(DataModelSchema schema)
		{

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
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Dispose of an instance of the {0}.", schema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = "Dispose";
			this.Attributes = MemberAttributes.Override | MemberAttributes.Family;
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.Boolean), "disposing"));
			this.Statements.Add(new CodeCommentStatement("This section disposes of the managed resources."));
			CodeStatement[] disposeMethodTrue = new CodeStatement[]
				{
					new CodeCommentStatement("This controls the disposal of the static resources.  When the instance count reaches zero, then all static resources"),
					new CodeCommentStatement("should be released back to the operating system."),
					new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "referenceCount"), new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "referenceCount"), CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1)))
				};
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("disposing"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)), disposeMethodTrue));
			this.Statements.Add(new CodeCommentStatement("Allow the base class to complete the disposal."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "Dispose", new CodeArgumentReferenceExpression("disposing")));

		}

	}
}
