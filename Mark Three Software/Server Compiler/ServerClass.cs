namespace MarkThree.MiddleTier
{
	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	public class ServerClass : CodeTypeDeclaration
	{

		// Private Fields
		private MarkThree.MiddleTier.ServerSchema coreInterfaceSchema;
		private MarkThree.MiddleTier.TableSchema tableSchema;

		/// <summary>
		/// Creates a CodeDOM class that provides a transacted interface to a data table.
		/// </summary>
		/// <param name="coreInterfaceSchema">The schema that defines the data model.</param>
		/// <param name="tableSchema">The schema that defines the table.</param>
		public ServerClass(ServerSchema serverSchema, TableSchema tableSchema)
		{

			// Initialize the object.
			this.coreInterfaceSchema = serverSchema;
			this.tableSchema = tableSchema;

			//	/// <summary>
			//	/// Provides transaction operations for the Department table.
			//	/// </summary>
			//	[System.ComponentModel.DesignerCategoryAttribute("code")]
			//	[System.Diagnostics.DebuggerStepThrough()]
			//	public class Department
			//	{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement(string.Format("Provides transaction operations for the {0} table.", tableSchema.Name), true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerCategoryAttribute", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression("code")) }));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.TypeAttributes = TypeAttributes.Public;
			this.IsClass = true;
			this.Name = tableSchema.Name;

			//		// This is the name of the volatile resource manager used by this class.
			//		private const string VolatileResource = "DataModel";
			CodeMemberField volatileResourceField = new CodeMemberField(typeof(string), "VolatileResource");
			volatileResourceField.Comments.Add(new CodeCommentStatement("This is the name of the volatile resource manager used by this class."));
			volatileResourceField.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			volatileResourceField.InitExpression = new CodePrimitiveExpression(this.coreInterfaceSchema.VolatileStoreName);
			this.Members.Add(volatileResourceField);

			//		// This is the name of the durable resource manager used by this class.
			//		private const string DurableResource = "UnitTest";
			CodeMemberField durableResourceField = new CodeMemberField(typeof(string), "DurableResource");
			durableResourceField.Comments.Add(new CodeCommentStatement("This is the name of the durable resource manager used by this class."));
			durableResourceField.Attributes = MemberAttributes.Private | MemberAttributes.Const;
			durableResourceField.InitExpression = new CodePrimitiveExpression(this.coreInterfaceSchema.DurableStoreName);
			this.Members.Add(durableResourceField);

			// Add the member methods to the class.
			this.Members.Add(new Insert(tableSchema));
			this.Members.Add(new Update(tableSchema));
			this.Members.Add(new Delete(tableSchema));
			//this.Members.Add(new Archive(coreInterfaceSchema, tableSchema));

		}

	}

}
