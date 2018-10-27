namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Data;

	/// <summary>
	/// Summary description for ExternalKeyArray.
	/// </summary>
	public class ExternalKeyArray : CodeMemberField
	{

		public ExternalKeyArray(ExternalInterfaceSchema ExternalInterfaceSchema)
		{

			this.Comments.Add(new CodeCommentStatement(@"This is an array of indices used to find a record based on an external identifier.", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
			this.Name = "externalKeyArray";
			this.Type = new CodeTypeReference(typeof(DataView[]));

		}

	}
}
