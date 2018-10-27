namespace MarkThree.MiddleTier.TableClass
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property that gets the parent row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class KeyProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to get a parent row.
		/// </summary>
		/// <param name="keyrefSchema">The foreign key that references the parent table.</param>
		public KeyProperty(ConstraintSchema constraintSchema)
		{

			//			/// <summary>
			//			/// Gets the DepartmentKey index on the Department table.
			//			/// </summary>
			//			public DepartmentKeyIndex DepartmentKey
			//			{
			//				get
			//				{
			//					return this.indexDepartmentKey;
			//				}
			//			}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets the {0} index on the {1} table.", constraintSchema.Name, constraintSchema.Selector.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(string.Format("{0}Index", constraintSchema.Name));
			this.Name = constraintSchema.Name;
			this.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("index{0}", constraintSchema.Name))));

		}

	}

}
