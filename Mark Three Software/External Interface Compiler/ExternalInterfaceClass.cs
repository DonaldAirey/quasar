namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	public class ExternalInterfaceClass : CodeTypeDeclaration
	{

		private ExternalInterfaceSchema ExternalInterfaceSchema;
		private MiddleTierTable middleTierTable;

		public ExternalInterfaceClass(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
		{

			// Initialize the object
			this.ExternalInterfaceSchema = ExternalInterfaceSchema;
			this.middleTierTable = middleTierTable;

			// Find out if the class has external identifiers or has a parent table that uses external identifiers.  The external
			// interface methods are only meaningful for resolving these external identifiers.
			bool isExternalInterfaceNeeded = middleTierTable.IsExternalIdClass || this.middleTierTable.ElementTable.Name == "Configuration";
			foreach (XmlSchemaKeyref keyrefParent in middleTierTable.KeyrefParents)
			{
				XmlSchemaIdentityConstraint keyParent = ExternalInterfaceSchema.FindKey(keyrefParent.Refer.Name);
				XmlSchemaElement elementParent = ExternalInterfaceSchema.FindTable(ExternalInterfaceSchema.RemoveXPath(keyParent.Selector));
				if (this.ExternalInterfaceSchema.HasExternalIdColumn(elementParent))
					isExternalInterfaceNeeded = true;
			}

			// Construct the header for an inherited and independent class.
			if (middleTierTable.ElementBaseTable == null)
				ExternalInterfaceClassWithoutBase();
			else
				ExternalInterfaceClassWithBase();

			// The base classes need to define a persistent data store and gain a reference to the static, in-memory
			// data model.  Inheritied classes don't need to be bothered because the base class has already defined these
			// resources.
			if (this.middleTierTable.ElementBaseTable == null)
			{

				// This field is a string which accesses the persistent memory store parameters from the configuration file.
				this.Members.Add(new PersistentStore(this.ExternalInterfaceSchema));

				// This field provides access to the in-memory data model.
				this.Members.Add(new AdoDatabase(this.ExternalInterfaceSchema));

			}

			// Optional Methods for finding a record based on an external identifier.
			if (middleTierTable.IsExternalIdClass && this.middleTierTable.ElementBaseTable == null)
			{
				this.Members.Add(new ExternalKeyArray(ExternalInterfaceSchema));
				this.Members.Add(new StaticConstructor(ExternalInterfaceSchema, middleTierTable));
				this.Members.Add(new GetExternalKeyIndex(ExternalInterfaceSchema, middleTierTable));
				this.Members.Add(new FindKey(ExternalInterfaceSchema, middleTierTable));
				this.Members.Add(new FindRequiredKey(ExternalInterfaceSchema, middleTierTable));
				this.Members.Add(new FindOptionalKey(ExternalInterfaceSchema, middleTierTable));
			}

			// Lock and Load
			this.Members.Add(new LoadLock(ExternalInterfaceSchema, middleTierTable));
			this.Members.Add(new Load(ExternalInterfaceSchema, middleTierTable));

			// If a table consists only of primary key elements (such as a mapping table), an update operation is undefined.  That is,
			// there are no columns to update.
			if (middleTierTable.NonPrimaryKeyColumns != 0)
			{
				this.Members.Add(new UpdateLock(ExternalInterfaceSchema, middleTierTable));
				this.Members.Add(new Update(ExternalInterfaceSchema, middleTierTable));
			}

			// Delete and Archive Methods
			this.Members.Add(new DeleteLock(ExternalInterfaceSchema, middleTierTable));
			this.Members.Add(new Delete(ExternalInterfaceSchema, middleTierTable));
			this.Members.Add(new ArchiveLock(ExternalInterfaceSchema, middleTierTable));
			this.Members.Add(new Archive(ExternalInterfaceSchema, middleTierTable));

		}

		private void ExternalInterfaceClassWithoutBase()
		{

			// Class Declaration
			//    [Serializable()]
			//    [System.Diagnostics.DebuggerStepThrough()]
			//    public class Algorithm {
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerCategoryAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression("code"))}));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.TypeAttributes = TypeAttributes.Public;
			this.IsClass = true;
			this.Name = this.middleTierTable.ElementTable.Name;

		}

		private void ExternalInterfaceClassWithBase()
		{

			// Class Declaration
			//    [Serializable()]
			//    [System.Diagnostics.DebuggerStepThrough()]
			//    public class Algorithm {
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.ComponentModel.DesignerCategoryAttribute", new CodeAttributeArgument[] {new CodeAttributeArgument(new CodePrimitiveExpression("code"))}));
			this.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThrough"));
			this.TypeAttributes = TypeAttributes.Public;
			this.IsClass = true;
			this.Name = this.middleTierTable.ElementTable.Name;
			this.BaseTypes.Add(this.middleTierTable.ElementBaseTable.Name);

		}
	
	}

}
