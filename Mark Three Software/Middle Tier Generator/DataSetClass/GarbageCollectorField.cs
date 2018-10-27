namespace MarkThree.MiddleTier.DataSetClass
{

	using MarkThree.MiddleTier;
	using System;
	using System.CodeDom;

	class GarbageCollectorField : CodeMemberField
	{

		/// <summary>
		/// A private field.
		/// </summary>
		public GarbageCollectorField()
		{

			this.Comments.Add(new CodeCommentStatement("A thread that purges deleted fields when they have become obsolete."));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			this.Type = new CodeTypeReference(typeof(System.Threading.Thread));
			this.Name = "garbageCollector";

		}

	}

}
