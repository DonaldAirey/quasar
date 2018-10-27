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
	/// Creates the method that is the starting point for the document creation.
	/// </summary>
	public class DataTransformInitializeViewMethod : System.CodeDom.CodeMemberMethod
	{

		/// <summary>
		/// Creates the method that is the starting point for the document creation.
		/// </summary>
		/// <param name="dataTransform">Describes how to create a document from an abstract data model.</param>
		public DataTransformInitializeViewMethod(DataTransform dataTransform)
		{

			//			/// <summary>
			//			/// Builds the styles used to display data in the document.
			//			/// </summary>
			//			public override void InitializeView(List<ViewerCommand> viewerCommands)
			//			{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Builds the styles used to display data in the document.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			this.ReturnType = new CodeTypeReference(typeof(void));
			this.Name = "InitializeView";

			//			// Split the screen into quadrants.
			//			this.documentViewer.ViewerCommandQueue.Enqueue(new MarkThree.Forms.ViewerCommand(new MarkThree.Forms.ViewerSplit(new System.Drawing.SizeF(54F, 55F)), new MarkThree.Forms.ViewerCommandDelegate(this.documentViewer.SetSplit)));
			SizeF splitSize = dataTransform.SplitSize;
			if (splitSize != SizeF.Empty)
			{
				this.Statements.Add(new CodeCommentStatement("Split the screen into quadrants."));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "ViewerCommandQueue"), "Enqueue",
					new CodeObjectCreateExpression(typeof(ViewerCommand), new CodeObjectCreateExpression(typeof(ViewerSplit), new CodeObjectCreateExpression(typeof(SizeF), new CodePrimitiveExpression(splitSize.Width), new CodePrimitiveExpression(splitSize.Height))),
					new CodeObjectCreateExpression(typeof(ViewerCommandDelegate), new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "SetSplit")))));
			}

			//			// Set the magnification factor.
			//			this.documentViewer.ViewerCommandQueue.Enqueue(new MarkThree.Forms.ViewerCommand(new MarkThree.Forms.ViewerScale(1.25F), new MarkThree.Forms.ViewerCommandDelegate(this.documentViewer.SetScale)));
			float scaleFactor = dataTransform.ScaleFactor;
			if (scaleFactor != DefaultDocument.ScaleFactor)
			{
				this.Statements.Add(new CodeCommentStatement("Set the magnification factor."));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "ViewerCommandQueue"), "Enqueue",
					new CodeObjectCreateExpression(typeof(ViewerCommand), new CodeObjectCreateExpression(typeof(ViewerScale), new CodePrimitiveExpression(scaleFactor)),
					new CodeObjectCreateExpression(typeof(ViewerCommandDelegate), new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "SetScale")))));
			}

			// This will generate each of the styles in turn and add them to the range of styles that is sent to the viewer.
			// The viewer will then use these styles to match up the incoming tiles to instructions to present them in the
			// document.
			foreach (DataTransform.StyleNode styleNode in dataTransform.Styles)
			{

				//					// Header Style
				//					MarkThree.Forms.ViewerStyle headerStyle = this.documentViewer.GetStyle("Default");
				//					if (("Default" != headerStyle.ParentId))
				//					{
				//						headerStyle.ParentId = "Default";
				//						headerStyle.IsModified = true;
				//					}
				//					headerStyle.Attributes.Add(new MarkThree.Forms.ViewerBottomBorder(System.Drawing.Color.DarkGray, 1F));
				//					headerStyle.Attributes.Add(new MarkThree.Forms.ViewerRightBorder(System.Drawing.Color.DarkGray, 1F));
				//					headerStyle.Attributes.Add(new MarkThree.Forms.ViewerInteriorBrush(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control)));
				//					headerStyle.Attributes.Add(new MarkThree.Forms.ViewerImage(this.documentViewer.Variables["GreenCheckImage"]));
				this.Statements.Add(new CodeCommentStatement(string.Format("{0} Style", styleNode.StyleId)));
				string variableName = string.Format("{0}Style", CamelCase.Convert(styleNode.StyleId));
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(ViewerStyle), variableName, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "GetStyle", new CodePrimitiveExpression(styleNode.StyleId))));
				if (styleNode.ParentStyle != null)
				{
					CodeStatement[] trueStatements0 = new CodeStatement[2];
					trueStatements0[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "ParentId"), new CodePrimitiveExpression(styleNode.ParentStyle.StyleId));
					trueStatements0[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "IsModified"), new CodePrimitiveExpression(true));
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(styleNode.ParentStyle.StyleId), CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "ParentId")), trueStatements0));
				}
				if (styleNode.HasAnimation)
				{
					if (styleNode.Animation.Effect == Effect.Fade)
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerFadeAnimation), CodeSnippet.Convert(styleNode.Animation.Up), CodeSnippet.Convert(styleNode.Animation.Down), CodeSnippet.Convert(styleNode.Animation.Same), new CodePrimitiveExpression(styleNode.Animation.Steps))));
					if (styleNode.Animation.Effect == Effect.Flash)
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerFlashAnimation), CodeSnippet.Convert(styleNode.Animation.Foreground), CodeSnippet.Convert(styleNode.Animation.Background), new CodePrimitiveExpression(styleNode.Animation.On), new CodePrimitiveExpression(styleNode.Animation.Off), new CodePrimitiveExpression(styleNode.Animation.Repeat))));
				}
				if (styleNode.HasBottomBorder)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerBottomBorder), CodeSnippet.Convert(styleNode.BottomBorder.Color), new CodePrimitiveExpression(styleNode.BottomBorder.Width))));
				if (styleNode.HasLeftBorder)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerLeftBorder), CodeSnippet.Convert(styleNode.LeftBorder.Color), new CodePrimitiveExpression(styleNode.LeftBorder.Width))));
				if (styleNode.HasRightBorder)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerRightBorder), CodeSnippet.Convert(styleNode.RightBorder.Color), new CodePrimitiveExpression(styleNode.RightBorder.Width))));
				if (styleNode.HasTopBorder)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerTopBorder), CodeSnippet.Convert(styleNode.TopBorder.Color), new CodePrimitiveExpression(styleNode.TopBorder.Width))));
				if (styleNode.HasFont)
				{
					if (styleNode.Font.Style == FontStyle.Regular)
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerFont), CodeSnippet.Convert(styleNode.Font.FontFamily), new CodePrimitiveExpression(styleNode.Font.Size))));
					else
						this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerFont), CodeSnippet.Convert(styleNode.Font.FontFamily), new CodePrimitiveExpression(styleNode.Font.Size), CodeSnippet.Convert(styleNode.Font.Style))));
				}
				if (styleNode.HasFontBrush)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerFontBrush), CodeSnippet.Convert(styleNode.ForeColor))));
				if (styleNode.HasStringFormat)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerStringFormat), CodeSnippet.Convert(styleNode.StringFormat.Alignment), CodeSnippet.Convert(styleNode.StringFormat.LineAlignment))));
				if (styleNode.HasNumberFormat)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerNumberFormat), new CodePrimitiveExpression(styleNode.NumberFormat))));
				if (styleNode.HasInterior)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerInteriorBrush), CodeSnippet.Convert(styleNode.BackColor))));
				if (styleNode.HasImage)
					this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(variableName), "Attributes"), "Add", new CodeObjectCreateExpression(typeof(ViewerImage), new CodeCastExpression(typeof(Image), new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "Variables"), new CodePrimitiveExpression(styleNode.Image))))));

			}

		}

	}

}
