namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class TableParentRelationProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public TableParentRelationProperty(KeyrefSchema keyrefSchema)
		{

			//			/// <summary>
			//			/// Gets the parent relation between the Department and Employee tables.
			//			/// </summary>
			//			internal Relation DepartmentEmployeeRelation
			//			{
			//				get
			//				{
			//					return this.relationDepartmentEmployee;
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the parent relation between the {0} and {1} tables.", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Assembly | MemberAttributes.Final;
			this.Type = new CodeTypeReference("Relation");
			this.Name = string.Format("{0}{1}Relation", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name);
			this.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("relation{0}{1}", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name))));

		}

	}

}
