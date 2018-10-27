namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class MasterRowVersionField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public MasterRowVersionField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("Keeps track of the row version for the entire data model."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Int64));
			this.Name = "masterRowVersion";

		}

	}

}
