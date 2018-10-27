/*************************************************************************************************************************
*
*	File:			DeleteLock.cs
*	Description:	Collects the locks for the External Delete Method.
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

	public class DeleteLock : ExternalInterfaceMethod
	{

		/// <summary>
		/// Creates a procedure that collects table locks needed for the 'Delete' operation.
		/// </summary>
		/// <param name="externalInterfaceClass"></param>
		public DeleteLock(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
			: base(ExternalInterfaceSchema, middleTierTable)
		{

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string rowVariable = string.Format("{0}Row", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string tableTypeName = string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name);

			//        /// <summary>Collects the table lock request(s) for an 'Delete' operation</summary>
			//        /// <param name="requestLocks">A collection of table locks required for this operation.</param>
			//        public static void Delete(Transaction transaction)
			//        {
			this.Comments.Add(new CodeCommentStatement("<summary>Collects the table lock request(s) for an 'Delete' operation</summary>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""adoTransaction"">A collection of table locks required for this operation</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			if (this.MiddleTierTable.ElementBaseTable != null)
				this.Attributes |= MemberAttributes.New;
			this.Name = "Delete";
			this.Parameters.Add(new CodeParameterDeclarationExpression("AdoTransaction", "adoTransaction"));

			//            // Call the internal methods to lock the tables required for an insert or update operation.
			//            Shadows.Web.Service.Equity.Insert(transaction);
			//            Shadows.Web.Service.Equity.Delete(transaction);
			this.Statements.Add(new CodeCommentStatement("Call the internal methods to lock the tables required for an insert or update operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}", this.ExternalInterfaceSchema.InternalNamespace, this.MiddleTierTable.ElementTable.Name)), "Delete", new CodeExpression[] {new CodeVariableReferenceExpression("adoTransaction")}));

			//            // These table lock(s) are required for the 'Delete' operation.
			//            transaction.Locks.AddReaderLock(ServerMarketData.ConfigurationLock);
			//            transaction.Locks.AddWriterLock(ServerMarketData.AlgorithmLock);
			//            transaction.Locks.AddReaderLock(ServerMarketData.AlgorithmTypeLock);
			this.Statements.Add(new CodeCommentStatement("These table lock(s) are required for the 'Delete' operation."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "LockRequests"), "AddReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), string.Format("{0}Lock", this.MiddleTierTable.ElementTable.Name))}));
			if (this.MiddleTierTable.IsExternalIdClass)
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeArgumentReferenceExpression("adoTransaction"), "LockRequests"), "AddReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), "ConfigurationLock")}));

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
