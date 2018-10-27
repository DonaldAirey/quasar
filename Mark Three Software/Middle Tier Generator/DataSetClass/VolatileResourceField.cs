namespace MarkThree.MiddleTier
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class VolatileResourceField : CodeMemberField
	{

		/// <summary>
		/// Create a member to wait for a thread to expire.
		/// </summary>
		public VolatileResourceField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("Describes the volatile resources used for transactions."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			this.Type = new CodeTypeReference(typeof(System.String));
			this.Name = "volatileResource";
			this.InitExpression = new CodePrimitiveExpression("ADO Data Model");

		}

	}

}
