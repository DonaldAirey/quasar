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
	/// Creates a CodeDOM class used to sort the rows of a table.
	/// </summary>
	public class SortClass : System.CodeDom.CodeTypeDeclaration
	{

		/// <summary>
		/// Creates a CodeDOM class used to sort the rows of a table.
		/// </summary>
		/// <param name="templateNode">Contains the specification for the template.</param>
		public SortClass(DataTransform.ApplyTemplateNode applyTemplateNode)
		{

			//	// Compars elements in a WorkingOrder Table
			//	public class WorkingOrderRowComparer : System.Collections.Generic.IComparer<MarkThree.Forms.ViewerRow>
			//	{
			this.BaseTypes.Add(typeof(IComparer<ViewerRow>));
			this.Name = applyTemplateNode.Select + "RowComparer";
			this.Comments.Add(new CodeCommentStatement(string.Format("Compars elements in a {0} Table", applyTemplateNode.Select)));

			// This will add the comparision method which is the raison d'etre for this class.
			this.Members.Add(new SortCompareMethod(applyTemplateNode));

		}

	}

}
