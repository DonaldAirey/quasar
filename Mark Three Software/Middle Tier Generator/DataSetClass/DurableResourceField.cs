namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class DurableResourceField : CodeMemberField
	{

		/// <summary>
		/// Create a member to wait for a thread to expire.
		/// </summary>
		public DurableResourceField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("Describes the volatile resources used for transactions."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			this.Type = new CodeTypeReference(typeof(System.String));
			this.Name = "durableResource";
			this.InitExpression = new CodePrimitiveExpression("SQL Data Model");

		}

	}

}
