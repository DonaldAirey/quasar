namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;

	/// <summary>
	/// Summary description for PersistentStore.
	/// </summary>
	public class PersistentStore : CodeMemberField
	{

		public PersistentStore(DataModelSchema schema)
		{

			this.Comments.Add(new CodeCommentStatement(@"This value is used to map the object to a persistent storage device.  The parameters for the storage", true));
			this.Comments.Add(new CodeCommentStatement(@"are found in the configuration file for this service.", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final;
			this.Name = "PersistentStore";
			this.Type = new CodeTypeReference(typeof(string));
			this.InitExpression = new CodePrimitiveExpression(schema.DurableStoreName);

		}

	}
}
