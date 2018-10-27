namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class IncrementRowVersionMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public IncrementRowVersionMethod(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Increments the row version on a column.
			//		/// </summary>
			//		public static long IncrementRowVersion()
			//		{
			//			// In order to reconcile the server data model to the client, the server maintains views on the data that are organized
			//			// by row version.  When a reconcile operating is trying to determine which rows should be returned to the client, it
			//			// is critical that the order of this table doesn't change.  A monitor on the 'RowVersionColumn' of the table acts as
			//			// the gatekeeper for the information used to sort the view.  This will prevent other threads from updating the row
			//			// version until the reconile operation can collect all the rows that need to be returned to the client.
			//			return Interlocked.Increment(ref DataModel.masterRowVersion);
			//		}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Increments the row version on a column.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = "IncrementRowVersion";
			this.ReturnType = new CodeTypeReference(typeof(System.Int64));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			this.Statements.Add(new CodeCommentStatement("In order to reconcile the server data model to the client, the server maintains views on the data that are organized"));
			this.Statements.Add(new CodeCommentStatement("by row version.  When a reconcile operating is trying to determine which rows should be returned to the client, it"));
			this.Statements.Add(new CodeCommentStatement("is critical that the order of this table doesn't change.  A monitor on the 'RowVersionColumn' of the table acts as"));
			this.Statements.Add(new CodeCommentStatement("the gatekeeper for the information used to sort the view.  This will prevent other threads from updating the row"));
			this.Statements.Add(new CodeCommentStatement("version until the reconile operation can collect all the rows that need to be returned to the client."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Interlocked)), "Increment", new CodeDirectionExpression(FieldDirection.Ref, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "masterRowVersion")))));
			
		}

	}
}
