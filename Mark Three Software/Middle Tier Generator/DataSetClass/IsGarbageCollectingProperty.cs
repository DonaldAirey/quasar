namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class IsGarbageCollectingProperty : CodeMemberProperty
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public IsGarbageCollectingProperty(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Gets or sets a flag that controls the execution status of the garbage collection thread.
			//		/// </summary>
			//		private static bool IsGarbageCollecting
			//		{
			//			get
			//			{
			//				try
			//				{
			//					// Prevent other threads from modifying the flag while it is returned to the caller.
			//					System.Threading.Monitor.Enter(typeof(DataModel));
			//					return DataModel.isGarbageCollecting;
			//				}
			//				finally
			//				{
			//					System.Threading.Monitor.Exit(typeof(DataModel));
			//				}
			//			}
			//			set
			//			{
			//				try
			//				{
			//					// Prevent other threads from modifying the flag while it is set.
			//					System.Threading.Monitor.Enter(typeof(DataModel));
			//					DataModel.isGarbageCollecting = value;
			//				}
			//				finally
			//				{
			//					System.Threading.Monitor.Exit(typeof(DataModel));
			//				}
			//			}
			//		}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Gets or sets a flag that controls the execution status of the garbage collection thread.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = "IsGarbageCollecting";
			this.Type = new CodeTypeReference(typeof(System.Boolean));
			this.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			CodeTryCatchFinallyStatement getTry = new CodeTryCatchFinallyStatement();
			getTry.TryStatements.Add(new CodeCommentStatement("Prevent other threads from modifying the flag while it is returned to the caller."));
			getTry.TryStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)), "Enter", new CodeTypeOfExpression(schema.Name)));
			getTry.TryStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "isGarbageCollecting")));
			getTry.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)), "Exit", new CodeTypeOfExpression(schema.Name)));
			this.GetStatements.Add(getTry);
			CodeTryCatchFinallyStatement setTry = new CodeTryCatchFinallyStatement();
			setTry.TryStatements.Add(new CodeCommentStatement("Prevent other threads from modifying the flag while it is set."));
			setTry.TryStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)), "Enter", new CodeTypeOfExpression(schema.Name)));
			setTry.TryStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "isGarbageCollecting"), new CodeArgumentReferenceExpression("value")));
			setTry.FinallyStatements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)), "Exit", new CodeTypeOfExpression(schema.Name)));
			this.SetStatements.Add(setTry);
		
		}

	}
}
