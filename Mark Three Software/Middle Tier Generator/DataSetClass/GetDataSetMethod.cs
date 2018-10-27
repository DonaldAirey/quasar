namespace MarkThree.MiddleTier.DataSetClass
{

	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
	/// </summary>
	class GetDataSetMethod : CodeMemberMethod
	{

		/// <summary>
		/// Creates a method to handle moving the deleted records from the active data model to the deleted data model.
		/// </summary>
		/// <param name="schema">The data model schema.</param>
		public GetDataSetMethod(DataModelSchema schema)
		{

			//		/// <summary>
			//		/// Gets the instance of the data model.
			//		/// </summary>
			//		public static System.Data.DataSet GetDataSet()
			//		{
			//			// This is used by the ADO Resource Manager to identify the instance of the data model.
			//			return DataModel.dataSet;
			//		}
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Gets the instance of the data model.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Name = "GetDataSet";
			this.ReturnType = new CodeTypeReference(typeof(System.Data.DataSet));
			this.Attributes = MemberAttributes.Final | MemberAttributes.Static | MemberAttributes.Public;
			this.Statements.Add(new CodeCommentStatement("This is used by the ADO Resource Manager to identify the instance of the data model."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(schema.Name), "dataSet")));

		}

	}
}
