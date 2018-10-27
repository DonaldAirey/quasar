/*************************************************************************************************************************
*
*	File:			LoadLock.cs
*	Description:	Collects the locks for the External Load Method.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml.Schema;

	public class LoadLock : ExternalInterfaceMethod
	{

		/// <summary>
		/// Creates a procedure that collects table locks needed for the 'Load' operation.
		/// </summary>
		/// <param name="externalInterfaceClass"></param>
		public LoadLock(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
			: base(ExternalInterfaceSchema, middleTierTable)
		{

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			//        /// <summary>Collects the table lock request(s) for an 'Load' operation</summary>
			//        /// <param name="requestLocks">A collection of table locks required for this operation.</param>
			//        public static void Load(LockRequestList lockRequests)
			//        {
			this.Comments.Add(new CodeCommentStatement("<summary>Collects the table lock request(s) for an 'Load' operation</summary>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""adoTransaction"">A collection of table locks required for this operation</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			if (this.MiddleTierTable.ElementBaseTable != null)
				this.Attributes |= MemberAttributes.New;
			this.Name = "Load";
			this.Parameters.Add(new CodeParameterDeclarationExpression("AdoTransaction", "adoTransaction"));

			//            // Call the internal methods to lock the tables required for an insert or update operation.
			//            Shadows.Web.Service.Equity.Insert(transaction);
			//            Shadows.Web.Service.Equity.Update(transaction);
			this.Statements.Add(new CodeCommentStatement("Call the internal methods to lock the tables required for an insert or update operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ExternalInterfaceSchema.InternalNamespace, this.MiddleTierTable.ElementTable.Name)), "Insert", new CodeExpression[] {new CodeVariableReferenceExpression("adoTransaction")}));
			if (this.MiddleTierTable.NonPrimaryKeyColumns != 0)
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ExternalInterfaceSchema.InternalNamespace, this.MiddleTierTable.ElementTable.Name)), "Update", new CodeExpression[] {new CodeVariableReferenceExpression("adoTransaction")}));

			//            // These table lock(s) are required for the 'Load' operation.
			//            transaction.Locks.AddReaderLock(ServerMarketData.ConfigurationLock);
			//            transaction.Locks.AddWriterLock(ServerMarketData.AlgorithmLock);
			//            transaction.Locks.AddReaderLock(ServerMarketData.AlgorithmTypeLock);
			this.Statements.Add(new CodeCommentStatement("These table lock(s) are required for the 'Load' operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "LockRequests"), "AddReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), string.Format("{0}Lock", this.MiddleTierTable.ElementTable.Name))}));
			ArrayList arrayLock = new ArrayList();
			foreach (XmlSchemaKeyref keyrefParent in this.MiddleTierTable.KeyrefParents)
			{
				// Get the name of the parent table.
				XmlSchemaIdentityConstraint keyParent = this.ExternalInterfaceSchema.FindKey(keyrefParent.Refer.Name);
				XmlSchemaElement elementParentTable = this.ExternalInterfaceSchema.FindTable(this.ExternalInterfaceSchema.RemoveXPath(keyParent.Selector));
				// Collect the locks of all the parent tables.
				CollectTableLocks(arrayLock, elementParentTable);
			}
			// This will set a flag is the configuration table is needed to resolve external identifiers.
			bool needsCondigurationTable = this.MiddleTierTable.IsExternalIdClass;
			foreach (XmlSchemaElement elementParentTable in arrayLock)
				if (this.ExternalInterfaceSchema.HasExternalIdColumn(elementParentTable))
				{
					needsCondigurationTable = true;
					break;
				}
			if (needsCondigurationTable)
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "LockRequests"), "AddReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), "ConfigurationLock")}));
			foreach (XmlSchemaElement elementParentTable in arrayLock)
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "LockRequests"), "AddReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), string.Format("{0}Lock", elementParentTable.Name))}));

		}

		/// <summary>
		/// Recursively search for table locks in all the parent tables.
		/// </summary>
		/// <param name="arrayLock"></param>
		/// <param name="elementParentTable"></param>
		private void CollectTableLocks(ArrayList arrayLock, XmlSchemaElement elementParentTable)
		{

			// The parent table lock is only necessary if it has external identifiers.
			if (this.ExternalInterfaceSchema.HasExternalIdColumn(elementParentTable))
			{

				XmlSchemaComplexType complexTypeGrandParent = this.ExternalInterfaceSchema.GetBaseClass(elementParentTable);
				if (complexTypeGrandParent != null)
					CollectTableLocks(arrayLock, this.ExternalInterfaceSchema.GetBaseTable(complexTypeGrandParent));

				// Add the element only if it doesn't aready exist in the list.  That is, throw away redundant table locks.
				bool found = false;
				foreach (XmlSchemaElement xmlSchemaElement in arrayLock)
					if (xmlSchemaElement.Name == elementParentTable.Name)
					{
						found = true;
						break;
					}

				// If the table is not already part of the list of lock requests, then add it.
				if (!found)
					arrayLock.Add(elementParentTable);

			}

		}

	}

}
