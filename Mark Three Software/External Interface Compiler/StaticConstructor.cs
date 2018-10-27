/*************************************************************************************************************************
*
*	File:			TypeConstructor.cs
*	Description:	Creates a CodeDOM method that will find an element key based on an external identifier.
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

	public class StaticConstructor : CodeTypeConstructor
	{

		// Private Members
		ExternalInterfaceSchema ExternalInterfaceSchema;
		MiddleTierTable middleTierTable;

		/// <summary>
		/// Construct a CodeDOM that will find a record based on an external identifier and a configuration.
		/// </summary>
		public StaticConstructor(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
		{

			// Initialize the object.
			this.ExternalInterfaceSchema = ExternalInterfaceSchema;
			this.middleTierTable = middleTierTable;

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", middleTierTable.ElementTable.Name[0].ToString().ToLower() + middleTierTable.ElementTable.Name.Remove(0, 1));
			string primaryKeyName = ExternalInterfaceSchema.RemoveXPath(middleTierTable.PrimaryKey.Fields[0]);

			// Method Header:
			//        /// <summary>Initializes the static elements of an Object.</summary>
			//        public static int TypeConstructor(string configurationId, string parameterId, string externalId)
			//        {
			this.Comments.Add(new CodeCommentStatement(@"<summary>Initializes the static elements of an Object.</summary>", true));
			this.Attributes = MemberAttributes.Final | MemberAttributes.Static;


			//			// The table must be locked before the indices can be read into an accelerator array.
			//			ServerMarketData.AlgorithmTypeLock.AcquireReaderLock(Timeout.Infinite);
			this.Statements.Add(new CodeCommentStatement(@"The table must be locked before the indices can be read into an accelerator array."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}Lock", ExternalInterfaceSchema.DataSetName, middleTierTable.ElementTable.Name)), "AcquireReaderLock", new CodeExpression[] {new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Threading.Timeout"), "Infinite")}));
			
			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//            // Accessor for the Algorithm Table.
			//            ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", middleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(string.Format("{0}.{1}DataTable", ExternalInterfaceSchema.DataSetName, middleTierTable.ElementTable.Name)), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(ExternalInterfaceSchema.DataSetName), middleTierTable.ElementTable.Name)));

			// Indirect lookup operation using the external identifier to find a a row.
			//            // This does an indirect lookup operation using the views created for the ExternalId columns.  Take the index of the user
			//            // identifier column calcualted above and use it to find a record containing the external identifier.
			//            int externalIdIndex = GetExternalKeyIndex(configurationId, parameterId);
			//            DataView[] externalIdIndexArray = new DataView[] {
			//                    algorithmTable.UKAlgorithmExternalId0,
			//                    algorithmTable.UKAlgorithmExternalId1};
			//            DataRowView[] dataRowView = externalIdIndexArray[externalIdIndex].FindRows(new object[] {
			//                        externalId});
			this.Statements.Add(new CodeCommentStatement("This does an indirect lookup operation using the views created for the ExternalId columns.  Take the index of the user"));
			this.Statements.Add(new CodeCommentStatement("identifier column calcualted above and use it to find a record containing the external identifier."));
			CodeExpression[] userViewArray = new CodeExpression[this.middleTierTable.UniqueIndices.Length];
			for (int index = 0; index < this.middleTierTable.UniqueIndices.Length; index++)
				userViewArray[index] = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), this.middleTierTable.UniqueIndices[index].Name);
			this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(middleTierTable.ElementTable.Name), "externalKeyArray"), new CodeArrayCreateExpression("DataView", userViewArray)));

			//			// The table must be released after the array is constructed.
			//			ServerMarketData.AlgorithmTypeLock.AcquireReaderLock(Timeout.Infinite);
			this.Statements.Add(new CodeCommentStatement(@"The table must be released after the array is constructed."));
			this.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(string.Format("{0}.{1}Lock", ExternalInterfaceSchema.DataSetName, middleTierTable.ElementTable.Name)), "ReleaseReaderLock"));

		}

	}

}
