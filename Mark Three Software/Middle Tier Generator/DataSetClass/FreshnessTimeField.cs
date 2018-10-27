namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class FreshnessTimeField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public FreshnessTimeField()
		{

			this.Comments.Add(new CodeCommentStatement("The time that deleted records are allowed to hang around until they are purged for good."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.TimeSpan));
			this.Name = "freshnessTime";
			this.InitExpression = new CodeObjectCreateExpression(typeof(TimeSpan), new CodePrimitiveExpression(0), new CodePrimitiveExpression(1), new CodePrimitiveExpression(0));

		}

	}

}
