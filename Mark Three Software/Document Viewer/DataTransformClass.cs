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
	/// Generates the class that transforms a data model into a human read-able document.
	/// </summary>
	public class DataTransformClass : System.CodeDom.CodeTypeDeclaration
	{

		/// <summary>
		/// Generates the class that transforms a data model into a human read-able document.
		/// </summary>
		/// <param name="dataTransform">Describes how to transform abstract data into a document.</param>
		public DataTransformClass(DataTransform dataTransform)
		{

			//	/// <summary>
			//	/// Transforms a data model into a document.
			//	/// </summary>
			//	public class PrototypeView : DocumentView
			//	{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Transforms a data model into a document.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = dataTransform.DataTransformId;
			this.BaseTypes.Add("DocumentView");

			// The Column Index (Absolute Order) Constants
			//		// Constant Column Indices
			//		internal const int workingStatusImageIndex = 0;
			//		internal const int submittedStatusImageIndex = 1;
			//		internal const int recordIdIndex = 2;
			bool isFirstColumnIndexConstant = true;
			for (int columnIndex = 0; columnIndex < dataTransform.Columns.Count; columnIndex++)
			{
				DataTransform.ColumnNode columnNode = dataTransform.Columns[columnIndex];
				string constantName = CamelCase.Convert(columnNode.ColumnId) + "Index";
				CodeMemberField codeMemberField = new CodeMemberField(typeof(Int32), constantName);
				if (isFirstColumnIndexConstant)
				{
					isFirstColumnIndexConstant = false;
					codeMemberField.Comments.Add(new CodeCommentStatement("Constant Column Indices (Column Order)"));
				}
				codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Assembly;
				codeMemberField.InitExpression = new CodePrimitiveExpression(columnIndex);
				this.Members.Add(codeMemberField);
			}

			// The Column Ordinal (Viewer Order) Constants
			//		// Constant Column Indices
			//		internal const int workingStatusImageOrdinal = 0;
			//		internal const int submittedStatusImageOrdinal = 1;
			//		internal const int recordIdOrdinal = 2;
			bool isFirstColumnOrdinalConstant = true;
			for (int columnOrdinal = 0; columnOrdinal < dataTransform.View.Count; columnOrdinal++)
			{
				DataTransform.ColumnNode columnNode = dataTransform.View[columnOrdinal].Column;
				string constantName = CamelCase.Convert(columnNode.ColumnId) + "Ordinal";
				CodeMemberField codeMemberField = new CodeMemberField(typeof(Int32), constantName);
				if (isFirstColumnOrdinalConstant)
				{
					isFirstColumnOrdinalConstant = false;
					codeMemberField.Comments.Add(new CodeCommentStatement("Constant Column Ordinals (Viewer Order)"));
				}
				codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Assembly;
				codeMemberField.InitExpression = new CodePrimitiveExpression(columnOrdinal);
				this.Members.Add(codeMemberField);
			}

			// The Row Height Constants.
			//		// Constant Row Heights
			//		private const float headerRowHeight = 40F;
			//		private const float workingOrderRowHeight = 20F;
			//		private const float sourceOrderRowHeight = 20F;
			//		private const float destinationOrderRowHeight = 20F;
			//		private const float executionRowHeight = 20F;
			bool isFirstRowHeightConstant = true;
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				foreach (DataTransform.RowNode rowNode in templateNode.Rows)
				{
					string constantName = CamelCase.Convert(rowNode.RowId) + "Height";
					CodeMemberField codeMemberField = new CodeMemberField(typeof(Single), constantName);
					if (isFirstRowHeightConstant)
					{
						isFirstRowHeightConstant = false;
						codeMemberField.Comments.Add(new CodeCommentStatement("Constant Row Heights"));
					}
					codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Private;
					codeMemberField.InitExpression = new CodePrimitiveExpression(rowNode.Height);
					this.Members.Add(codeMemberField);
				}

			// The Row Width Constants.
			//		// Constant Row Widths
			//		private const float headerRowHeight = 745F;
			//		private const float workingOrderRowHeight = 745F;
			//		private const float sourceOrderRowHeight = 745F;
			//		private const float destinationOrderRowHeight = 745F;
			//		private const float executionRowHeight = 745F;
			bool isFirstRowWidthConstant = true;
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				foreach (DataTransform.RowNode rowNode in templateNode.Rows)
				{
					string constantName = CamelCase.Convert(rowNode.RowId) + "Width";
					CodeMemberField codeMemberField = new CodeMemberField(typeof(Single), constantName);
					if (isFirstRowWidthConstant)
					{
						isFirstRowWidthConstant = false;
						codeMemberField.Comments.Add(new CodeCommentStatement("Constant Row Widths"));
					}
					codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Private;
					float rowWidth = 0.0f;
					foreach (DataTransform.ColumnReferenceNode columnReferenceNode in dataTransform.View)
						rowWidth += columnReferenceNode.Column.Width;
					codeMemberField.InitExpression = new CodePrimitiveExpression(rowWidth);
					this.Members.Add(codeMemberField);
				}

			// The Column Width Constants.
			//		// Constant Column Widths
			//		private const float askPriceWidth = 53F;
			//		private const float availableQuantityWidth = 56F;
			//		private const float bidPriceWidth = 53F;
			//		private const float destinationOrderQuantityWidth = 56F;
			bool isFirstColumnWidthConstant = true;
			foreach (DataTransform.ColumnNode columnNode in dataTransform.Columns)
			{
				string constantName = CamelCase.Convert(columnNode.ColumnId) + "Width";
				CodeMemberField codeMemberField = new CodeMemberField(typeof(Single), constantName);
				if (isFirstColumnWidthConstant)
				{
					isFirstColumnWidthConstant = false;
					codeMemberField.Comments.Add(new CodeCommentStatement("Constant Column Widths"));
				}
				codeMemberField.Attributes = MemberAttributes.Const | MemberAttributes.Private;
				codeMemberField.InitExpression = new CodePrimitiveExpression(columnNode.Width);
				this.Members.Add(codeMemberField);
			}

			// The Comparers
			//		// Comparers
			//		private System.Collections.Generic.IComparer<MarkThree.Forms.ViewerRow> workingOrderRowComparer;
			//		private System.Collections.Generic.IComparer<MarkThree.Forms.ViewerRow> sourceOrderRowComparer;
			//		private System.Collections.Generic.IComparer<MarkThree.Forms.ViewerRow> destinationOrderRowComparer;
			bool isFirstComparer = true;
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				foreach (DataTransform.RowNode rowNode in templateNode.Rows)
					foreach (DataTransform.ApplyTemplateNode applyTemplateNode in rowNode.ApplyTemplates)
						if (applyTemplateNode.Sorts.Count != 0)
						{
							string comparerName = CamelCase.Convert(applyTemplateNode.Select) + "RowComparer";
							CodeMemberField codeMemberField = new CodeMemberField(typeof(IComparer<ViewerRow>), comparerName);
							if (isFirstComparer)
							{
								isFirstComparer = false;
								codeMemberField.Comments.Add(new CodeCommentStatement("Comparers"));
							}
							codeMemberField.Attributes = MemberAttributes.Private;
							this.Members.Add(codeMemberField);
						}


			// Add the constructor to the namespace.
			this.Members.Add(new DataTransformConstructor(dataTransform));

			// Add the Build method used to construct the styles.
			this.Members.Add(new DataTransformInitializeViewMethod(dataTransform));

			// Add the Build method used to construct the document.
			this.Members.Add(new DataTransformBuildViewMethod(dataTransform));

			// Add the code for all the template handlers.
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				this.Members.Add(new DataTransformTemplateMethod(templateNode));

		}

	}

}
