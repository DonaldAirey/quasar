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
	public class DataTransformBuildViewMethod : System.CodeDom.CodeMemberMethod
	{

		/// <summary>
		/// Creates the method that is the starting point for the document creation.
		/// </summary>
		/// <param name="dataTransform">Describes how to create a document from an abstract data model.</param>
		public DataTransformBuildViewMethod(DataTransform dataTransform)
		{

			//		/// <summary>
			//		/// Builds the root of the document.
			//		/// </summary>
			//		/// <returns>The root table of the document</returns>
			//		public override MarkThree.Forms.ViewerTable BuildView()
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Builds the root of the document.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Comments.Add(new CodeCommentStatement("<returns>The root table of the document</returns>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			this.ReturnType = new CodeTypeReference(typeof(ViewerTable));
			this.Name = "BuildView";

			//			// This will be the document root.
			//			MarkThree.Forms.ViewerTable viewerTable = new MarkThree.Forms.ViewerTable();
			this.Statements.Add(new CodeCommentStatement("This will be the document root."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(ViewerTable), "viewerTable", new CodeObjectCreateExpression(typeof(ViewerTable))));

			//			try
			//			{
			//				// Prevent the data model from being modified while the document is created.
			//				SimulatedDatabase.Lock.AcquireReaderLock(System.Threading.Timeout.Infinite);
			CodeStatementCollection tryStatementCollection = new CodeStatementCollection();
			tryStatementCollection.Add(new CodeCommentStatement("Prevent the data model from being modified while the document is created."));
			foreach (DataTransform.LockNode lockNode in dataTransform.Locks)
				tryStatementCollection.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(lockNode.Lock), "AcquireReaderLock", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Timeout)), "Infinite")));
			//				// Call each of the templates that match the document root.
			//				this.SlashTemplate(viewerTable, this.documentViewer.constantRow);
			//			}
			tryStatementCollection.Add(new CodeCommentStatement("Call each of the templates that match the document root."));
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				if (templateNode.Match.StartsWith("/"))
				{
					string methodName = string.Format("{0}Template", templateNode.Match.Replace("/", "Slash"));
					tryStatementCollection.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), methodName), new CodeVariableReferenceExpression("viewerTable"), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "ConstantRow")));
				}
			//			finally
			//			{
			//				// Release the locks on the document data.
			//				SimulatedDatabase.Lock.ReleaseReaderLock();
			//			}
			CodeStatementCollection finallyStatementCollection = new CodeStatementCollection();
			finallyStatementCollection.Add(new CodeCommentStatement("Release the locks on the document data."));
			foreach (DataTransform.LockNode lockNode in dataTransform.Locks)
				finallyStatementCollection.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(lockNode.Lock), "ReleaseReaderLock"));

			// Assemble the Try/Catch/Finally block.
			CodeStatement[] tryStatements = new CodeStatement[tryStatementCollection.Count];
			tryStatementCollection.CopyTo(tryStatements, 0);
			CodeStatement[] finallyStatements = new CodeStatement[finallyStatementCollection.Count];
			finallyStatementCollection.CopyTo(finallyStatements, 0);
			CodeCatchClause[] codeCatchClause = new CodeCatchClause[0];
			this.Statements.Add(new CodeTryCatchFinallyStatement(tryStatements, codeCatchClause, finallyStatements));

			//			// This is the final set of rows and their children that is ready to be displayed in a viewer.
			//			return viewerTable;
			this.Statements.Add(new CodeCommentStatement("This is the final set of rows and their children that is ready to be displayed in a viewer."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("viewerTable")));

		}

	}

}
