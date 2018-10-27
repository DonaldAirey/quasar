namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;

	class ThreadWaitTimeField : CodeMemberField
	{

		/// <summary>
		/// Create a member to wait for a thread to expire.
		/// </summary>
		public ThreadWaitTimeField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("The time to wait for a thread to respond before aborting it."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			this.Type = new CodeTypeReference(typeof(System.Int32));
			this.Name = "threadWaitTime";
			this.InitExpression = new CodePrimitiveExpression(1000);

		}

	}

}
