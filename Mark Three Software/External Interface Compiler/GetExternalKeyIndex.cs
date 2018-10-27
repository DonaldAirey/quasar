/*************************************************************************************************************************
*
*	File:			GetExternalKeyIndex.cs
*	Description:	Creates a CodeDOM procedure for finding the external identifier index (used to search for external
*					ids) based on a configuration and a parameter specification.
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

	public class GetExternalKeyIndex : ExternalInterfaceMethod
	{

		/// <summary>
		/// Construct a CodeDOM that will find a record based on an external identifier and a configuration.
		/// </summary>
		public GetExternalKeyIndex(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable) : base(ExternalInterfaceSchema, middleTierTable)
		{

			// These are shorthand notations for values that are use often to construct the tables:
			string tableVariable = string.Format("{0}Table", this.MiddleTierTable.ElementTable.Name[0].ToString().ToLower() + this.MiddleTierTable.ElementTable.Name.Remove(0, 1));
			string primaryKeyName = this.ExternalInterfaceSchema.RemoveXPath(this.MiddleTierTable.PrimaryKey.Fields[0]);

			// Method Header:
			//        /// <summary>Finds a a Algorithm record using a configuration and an external identifier.</summary>
			//        /// <param name="configurationId">Specified which mappings (user id columns) to use when looking up external identifiers.</param>
			//        /// <param name="externalId">The external (user supplied) identifier for the record.</param>
			//        public static int GetExternalKeyIndex(string configurationId, string parameterId, string externalId)
			//        {


			// Method Header:
			///       /// <summary>Calculates which index to uses when searching for external identifiers.</summary>
			///       /// <param name="configurationId">Specified which mappings (user id columns) to use when looking up external identifiers.</param>
			///       /// <param name="parameterId">The name of the parameter as specified in the configuration table.</param>
			///       /// <returns>An index into the array of keys to search for an external identifier.</returns>
			this.Comments.Add(new CodeCommentStatement(@"<summary>Calculates which index to uses when searching for external identifiers.</summary>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""configurationId"">Specified which mappings (user id columns) to use when looking up external identifiers.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""parameterId"">The name of the parameter as specified in the configuration table.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<returns>An index into the array of keys to search for an external identifier.</returns>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.ReturnType = new CodeTypeReference(typeof(int));
			this.Name = "GetExternalKeyIndex";
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "configurationId"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "parameterId"));

			// Translate the configuration identifier into an index into the user Id columns:
			//            // Translate the configurationId and the predefined parameter name into an index into the array of user ids.  The index
			//            // is where we expect to find the identifier.  That is, an index of 1 will guide the lookup logic to use the external
			//            // identifiers found in the 'ExternalId1' column.
			//            ServerMarketData.ConfigurationRow configurationRow = ServerMarketData.Configuration.FindByConfigurationIdParameterId(configurationId, "AlgorithmId");
			//            int externalKeyIndex = 0;
			//            if ((configurationRow != null))
			//            {
			//                externalKeyIndex = configurationRow.Index;
			//            }
			this.Statements.Add(new CodeCommentStatement("Translate the configurationId and the predefined parameter name into an index into the array of user ids.  The index"));
			this.Statements.Add(new CodeCommentStatement("is where we expect to find the identifier.  That is, an index of 1 will guide the lookup logic to use the external"));
			this.Statements.Add(new CodeCommentStatement("identifiers found in the 'ExternalId1' column."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "externalKeyIndex", new CodePrimitiveExpression(0)));
			CodeStatementCollection trueStatements = new CodeStatementCollection();
			trueStatements.Add(new CodeCommentStatement("Attempt to find a external column specification for the given configuration and parameter.  This record tells us"));
			trueStatements.Add(new CodeCommentStatement("which column to use in the array of external columns."));
			trueStatements.Add(new CodeVariableDeclarationStatement(string.Format("{0}.ConfigurationRow", this.ExternalInterfaceSchema.DataSetName), "configurationRow", new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ExternalInterfaceSchema.DataSetName), "Configuration"), "FindByConfigurationIdParameterId", new CodeExpression[] {new CodeCastExpression(typeof(string), new CodeArgumentReferenceExpression("configurationId")), new CodeArgumentReferenceExpression("parameterId")})));
			CodeExpression[] exceptionVariables = new CodeExpression[] {new CodePrimitiveExpression(@"The parameter {1} isn't defined for configuration '{0}'"), new CodeArgumentReferenceExpression("configurationId"), new CodeArgumentReferenceExpression("parameterId")};
			CodeStatement externalKeyIndex = new CodeAssignStatement(new CodeVariableReferenceExpression("externalKeyIndex"), new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("configurationRow"), "ColumnIndex"));
			trueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("configurationRow"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), externalKeyIndex));
			CodeStatement[] trueStatementBlock = new CodeStatement[trueStatements.Count];
			trueStatements.CopyTo(trueStatementBlock, 0);
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("configurationId"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), trueStatementBlock));

			//            // This is the index into the array of keys to be used when searching for an external identifier.
			//            return externalKeyIndex;
			this.Statements.Add(new CodeCommentStatement("This is the index into the array of keys to be used when searching for an external identifier."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("externalKeyIndex")));

		}

	}

}
