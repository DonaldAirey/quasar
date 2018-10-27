namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class PrimaryKeyOffsetField : CodeMemberField
	{

		/// <summary>
		/// Create a member to wait for a thread to expire.
		/// </summary>
		public PrimaryKeyOffsetField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("The offset to the primary key in the deleted data model records."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			this.Type = new CodeTypeReference(typeof(System.Int32));
			this.Name = "primaryKeyOffset";
			this.InitExpression = new CodePrimitiveExpression(2);

		}

	}

}
