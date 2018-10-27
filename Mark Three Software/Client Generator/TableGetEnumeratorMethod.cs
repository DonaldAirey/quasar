namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Creates a CodeDOM description a method to handle the Row Changed event.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableGetEnumeratorMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public TableGetEnumeratorMethod(TableSchema tableSchema)
		{

			// Construct the type names for the table and rows within the table.
			string rowTypeName = string.Format("{0}Row", tableSchema.Name);

			//			/// <summary>
			//			/// Gets a System.Collections.IEnumerator for the collection.
			//			/// </summary>
			//			/// <returns>An enumerator that can be used to iterate through the collection.</returns>
			//			public System.Collections.IEnumerator GetEnumerator()
			//			{
			//				return this.Rows.GetEnumerator();
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Gets a System.Collections.IEnumerator for the collection.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<returns>An enumerator that can be used to iterate through the collection.</returns>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.ReturnType = new CodeTypeReference(typeof(System.Collections.IEnumerator));
			this.Name = "GetEnumerator";
			this.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Rows"), "GetEnumerator", new CodeExpression[] { })));

		}

	}

}
