/*************************************************************************************************************************
*
*	File:			FindKey.cs
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
	using System.Data;

	public class FindKey : ExternalInterfaceMethod
	{

		/// <summary>
		/// Construct a CodeDOM that will find a record based on an external identifier and a configuration.
		/// </summary>
		public FindKey(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
			: base(ExternalInterfaceSchema, middleTierTable)
		{

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string primaryKeyName = this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]);

			// Find the primary column element.
			XmlSchemaElement primaryColumnElement = null;
			foreach (XmlSchemaElement xmlSchemaElement in this.MiddleTierTable.ElementColumns)
				if (xmlSchemaElement.Name == this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]))
					primaryColumnElement = xmlSchemaElement;

			// Method Header:
			//        /// <summary>Finds a a Algorithm record using a configuration and an external identifier.</summary>
			//        /// <param name="configurationId">Specified which mappings (user id columns) to use when looking up external identifiers.</param>
			//        /// <param name="externalId">The external (user supplied) identifier for the record.</param>
			//        public static int FindKey(string configurationId, string parameterId, string externalId)
			//        {
			Type returnType = ((XmlSchemaDatatype)primaryColumnElement.ElementType).ValueType;
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Finds a a {0} record using a configuration and an external identifier.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""configurationId"">Specified which mappings (user id columns) to use when looking up external identifiers.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""parameterId"">The name of the parameter as specified in the configuration table.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""externalId"">The external (user supplied) identifier for the record.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.ReturnType = new CodeTypeReference(returnType);
			this.Name = "FindKey";
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "configurationId"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "parameterId"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "externalId"));

			// This calculates the return value when there is no match.
			CodeExpression returnValue = returnType == typeof(int) ?
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue") :
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Empty");

			//			// A missing key will never match a column.
			//			if ((externalId == null))
			//			{
			//				return;
			//			}
			this.Statements.Add(new CodeCommentStatement("A missing key will never match a column."));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("externalId"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), new CodeStatement[] {new CodeMethodReturnStatement(returnValue)}));

			// Get an accessor to the table schema information.  This makes accessing information about the table much faster as 
			// it doesn't need to do the lock checking each time it references the table.
			//            // Accessor for the Algorithm Table.
			//            ServerMarketData.AlgorithmDataTable algorithmTable = ServerMarketData.Algorithm;
			this.Statements.Add(new CodeCommentStatement(string.Format("Accessor for the {0} Table.", this.MiddleTierTable.ElementTable.Name)));
			this.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(string.Format("{0}.{1}DataTable", this.ExternalInterfaceSchema.DataSetName, this.MiddleTierTable.ElementTable.Name)), tableVariable, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), this.MiddleTierTable.ElementTable.Name)));

			//			int externalKeyIndex = GetExternalKeyIndex(configurationId, parameterId);
			//			DataView externalKeyView = externalKeyArray[externalKeyIndex];
			//			int recordIndex = externalKeyView.Find(new object[] {externalId});
			//			if (recordIndex == -1)
			//				return -1;
			//			return ((int)(externalIndexView[recordIndex].Row[objectTable.ObjectIdColumn]));
			this.Statements.Add(new CodeCommentStatement(@"Look for the record using the external identifier.  The configuration selected the key to use, which effectively"));
			this.Statements.Add(new CodeCommentStatement(@"selected the external id column to use for the search.  If a record is found in the view, a translation still needs"));
			this.Statements.Add(new CodeCommentStatement(@"to be made back to the original table before an index to the record can be returned to the caller."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "externalKeyIndex", new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "GetExternalKeyIndex", new CodeExpression[] {new CodeArgumentReferenceExpression("configurationId"), new CodeArgumentReferenceExpression("parameterId")})));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Data.DataView), "externalKeyView", new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "externalKeyArray"), new CodeExpression[] {new CodeVariableReferenceExpression("externalKeyIndex")})));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "recordIndex", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("externalKeyView"), "Find", new CodeExpression[] {new CodeArrayCreateExpression(typeof(object), new CodeExpression[] {new CodeArgumentReferenceExpression("externalId")})})));
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("recordIndex"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(-1)), new CodeMethodReturnStatement(returnValue)));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(returnType, new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("externalKeyView"), new CodeExpression[] {new CodeVariableReferenceExpression("recordIndex")}), "Row"), new CodeExpression[] {new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tableVariable), string.Format("{0}Column", primaryKeyName))}))));

		}

	}

}
