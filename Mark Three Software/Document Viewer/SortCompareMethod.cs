namespace MarkThree.Forms
{

	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Creates a CodeDOM method to compare two rows of a document for the purpose of sorting.
	/// </summary>
	public class SortCompareMethod : System.CodeDom.CodeMemberMethod
	{

		/// <summary>
		/// Creates a CodeDOM method to compare two rows of a document for the purpose of sorting.
		/// </summary>
		/// <param name="templateNode">Contains the specification for the template.</param>
		public SortCompareMethod(DataTransform.ApplyTemplateNode applyTemplateNode)
		{

			// The sort works very efficiently by examining the tiles using the column ordinals as indices into the row.  The View
			// class defines these ordinals as public constants.
			DataTransform dataTransform = applyTemplateNode.TopLevelNode as DataTransform;

			//			#region IComparer<ViewerRow> Members
			this.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "IComparer<ViewerRow> Members"));

			//		// <summary>
			//		// Compares a row with another row using the column values to determine the order.
			//		// </summary>
			//		// <param name="x">The first row to be compared.</param>
			//		// <param name="y">The second row to be compared.</param>
			//		// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
			//		public virtual int Compare(MarkThree.Forms.ViewerRow x, MarkThree.Forms.ViewerRow y)
			//		{
			this.Comments.Add(new CodeCommentStatement(@"<summary>"));
			this.Comments.Add(new CodeCommentStatement(@"Compares a row with another row using the column values to determine the order."));
			this.Comments.Add(new CodeCommentStatement(@"</summary>"));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""x"">The first row to be compared.</param>"));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""y"">The second row to be compared.</param>"));
			this.Comments.Add(new CodeCommentStatement(@"<returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>"));
			this.Attributes = MemberAttributes.Public;
			this.ReturnType = new CodeTypeReference(typeof(int));
			this.Name = "Compare";
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ViewerRow), "x"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ViewerRow), "y"));

			//			// The order of the comparison determines whether the rows are sorted in ascending order or descending.  If the values
			//			// compared in one set of tiles is equal, then the next set of tiles in the sort order will be compared.
			//			MarkThree.Forms.ViewerTile xTile0 = ((MarkThree.Forms.ViewerTile)(x.Tile[PrototypeView.symbolIndex]));
			//			MarkThree.Forms.ViewerTile yTile0 = ((MarkThree.Forms.ViewerTile)(y.Tile[PrototypeView.symbolIndex]));
			//			int compare0 = xTile0.CompareTo(yTile0);
			//			if ((compare0 != 0))
			//			{
			//				return compare0;
			//			}
			//			return ((IComparable)y.Data[PrototypeView.sourceOrderQuantityIndex]).CompareTo(x.Data[PrototypeView.sourceOrderQuantityIndex]);
			this.Statements.Add(new CodeCommentStatement("The order of the comparison determines whether the rows are sorted in ascending order or descending.  If the values"));
			this.Statements.Add(new CodeCommentStatement("compared in one set of tiles is equal, then the next set of tiles in the sort order will be compared."));
			for (int index = 0; index < applyTemplateNode.Sorts.Count; index++)
			{
				DataTransform.SortNode sortNode = applyTemplateNode.Sorts[index];
				string columnReference = string.Format("{0}Index", CamelCase.Convert(sortNode.Column.ColumnId));
				string operand1 = sortNode.Direction == SortOrder.Ascending ? "x" : "y";
				string operand2 = sortNode.Direction == SortOrder.Ascending ? "y" : "x";
				if (index != applyTemplateNode.Sorts.Count - 1)
				{
					string compareVariableName = string.Format("compare{0}", index);
					this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), compareVariableName, new CodeMethodInvokeExpression(new CodeCastExpression(typeof(IComparable),
						new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(operand1), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), columnReference))), "CompareTo",
						new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(operand2), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), columnReference)))));
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(compareVariableName), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0)), new CodeMethodReturnStatement(new CodeVariableReferenceExpression(compareVariableName))));
				}
				else
				{
					this.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeCastExpression(typeof(IComparable),
						new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(operand1), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), columnReference))), "CompareTo",
						new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(operand2), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), columnReference)))));
				}

			}

			//		#endregion
			this.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

		}

	}

}
