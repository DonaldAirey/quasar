namespace MarkThree.MiddleTier.Row
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	/// <summary>
	/// Represents a declaration of a property indicates whether the underlying data is null.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class IsNullProperty : CodeMemberProperty
	{

		/// <summary>
		/// Generates a property to indicate whether the underlying value is null.
		/// </summary>
		/// <param name="tableSchema">The table to which this row belongs.</param>
		/// <param name="columnSchema">The nullable column.</param>
		public IsNullProperty(TableSchema tableSchema, ColumnSchema columnSchema)
		{

			//			/// <summary>
			//			/// Gets or sets the Null property of the Name column.
			//			/// </summary>
			//			[System.ComponentModel.Browsable(false)]
			//			[System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
			//			public bool IsNameNull
			//			{
			//				get
			//				{
			//					return (this[this.tableDepartment.NameColumn] == System.DBNull.Value);
			//				}
			//				set
			//				{
			//					this[this.tableDepartment.NameColumn] = System.DBNull.Value;
			//				}
			//			}
			//		}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Gets or sets the Null property of the {0} column.", columnSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.Browsable", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(false)) }));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerSerializationVisibilityAttribute", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeTypeReferenceExpression("System.ComponentModel.DesignerSerializationVisibility.Content")) }));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			this.Type = new CodeTypeReference(typeof(bool));
			this.Name = string.Format("Is{0}Null", columnSchema.Name);
			this.GetStatements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodeIndexerExpression(new CodeThisReferenceExpression(), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), string.Format("{0}Column", columnSchema.Name))), CodeBinaryOperatorType.IdentityEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value"))));
			this.SetStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeThisReferenceExpression(), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("table{0}", tableSchema.Name)), string.Format("{0}Column", columnSchema.Name))), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value")));

		}

	}

}
