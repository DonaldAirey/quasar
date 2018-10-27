namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class DeletedExclusionField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public DeletedExclusionField()
		{

			this.Comments.Add(new CodeCommentStatement("Grants exclusive access to the data model of deleted records."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Threading.Mutex));
			this.Name = "deletedExclusion";

		}

	}

}
