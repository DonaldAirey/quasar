namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class IsGarbageCollectingField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public IsGarbageCollectingField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("Gets or sets the state of the background thread used to purge deleted records."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Boolean));
			this.Name = "isGarbageCollecting";

		}

	}

}
