namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class DeletedDataSetField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public DeletedDataSetField()
		{

			//		// The time to wait for a thread to respond before aborting it.
			//		private static int threadWaitTime = 1000;
			this.Comments.Add(new CodeCommentStatement("Temporary storage used to hold deleted data until it is propogated to the client data models."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Data.DataSet));
			this.Name = "deletedDataSet";

		}

	}

}
