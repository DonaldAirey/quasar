namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class DeletedEventField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public DeletedEventField()
		{

			this.Comments.Add(new CodeCommentStatement("Used to turn the garbage collector off when there are no records to purge."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Threading.ManualResetEvent));
			this.Name = "deletedEvent";

		}

	}

}
