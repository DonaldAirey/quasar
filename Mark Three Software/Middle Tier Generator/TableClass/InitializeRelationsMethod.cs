namespace MarkThree.MiddleTier.TableClass
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
	class InitializeRelationsMethod : CodeMemberMethod
	{

		/// <summary>
		/// Generates the method used to handle the Row Changed event.
		/// </summary>
		/// <param name="tableSchema">The table to which this event belongs.</param>
		public InitializeRelationsMethod(TableSchema tableSchema)
		{

			//			/// <summary>
			//			/// Initializes the relation fields with the parent and child relations.
			//			/// </summary>
			//			internal void InitializeRelations()
			//			{
			//				// The Relation between the Object and Department tables
			//				this.relationObjectDepartment = this.ChildRelations["FK_Object_Department"];
			//				// The Relation between the Object and Employee tables
			//				this.relationObjectEmployee = this.ChildRelations["FK_Object_Employee"];
			//				// The Relation between the Object and Project tables
			//				this.relationObjectProject = this.ChildRelations["FK_Object_Project"];
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Initializes the relation fields with the parent and child relations.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Assembly | MemberAttributes.Final;
			this.Name = "InitializeRelations";
			foreach (KeyrefSchema keyrefSchema in tableSchema.ParentKeyrefs)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("The Relation between the {0} and {1} tables", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("relation{0}{1}", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)),
					new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ParentRelations"), new CodePrimitiveExpression(keyrefSchema.Name))));
			}
			foreach (KeyrefSchema keyrefSchema in tableSchema.ChildKeyrefs)
			{
				this.Statements.Add(new CodeCommentStatement(string.Format("The Relation between the {0} and {1} tables", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("relation{0}{1}", keyrefSchema.Refer.Selector.Name, keyrefSchema.Selector.Name)),
					new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "ChildRelations"), new CodePrimitiveExpression(keyrefSchema.Name))));
			}

		}

	}

}
