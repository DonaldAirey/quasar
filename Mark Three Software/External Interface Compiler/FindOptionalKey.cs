/*************************************************************************************************************************
*
*	File:			FindOptionalKey.cs
*	Description:	Finds an optional key.
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

	public class FindOptionalKey : ExternalInterfaceMethod
	{

		/// <summary>
		/// Construct a CodeDOM that will find a record based on an external identifier and a configuration.
		/// </summary>
		public FindOptionalKey(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
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
			//        public static object FindOptionalKey(string configurationId, object externalId)
			//        {
			this.Comments.Add(new CodeCommentStatement(string.Format("<summary>Finds a a {0} record using a configuration and an external identifier.</summary>", this.MiddleTierTable.ElementTable.Name), true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""configurationId"">Specified which mappings (user id columns) to use when looking up external identifiers.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""externalId"">The external (user supplied) identifier for the record.</param>", true));
			this.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			this.ReturnType = new CodeTypeReference(typeof(object));
			this.Name = "FindOptionalKey";
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "configurationId"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "parameterId"));
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "externalId"));

			//            // Look up the internal identifier using the the configuration to specify which ExternalId column to use as an index.
			//            object internalId = null;
			//            if ((externalId != null))
			//            {
			//                internalId = Algorithm.FindKey(configurationId, ((string)(externalId)));
			//                if ((((int)(internalId)) == -1))
			//                {
			//                    throw new Exception(string.Format("The Algorithm table does not have a record identified by \'{0}\'", externalId));
			//                }
			//            }
			Type returnType = ((XmlSchemaDatatype)primaryColumnElement.ElementType).ValueType;
			CodeExpression returnValue = returnType == typeof(int) ?
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue") :
				new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Empty");
			this.Statements.Add(new CodeCommentStatement("Look up the internal identifier using the the configuration to specify which ExternalId column to use as an index."));
			this.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "internalId", new CodePrimitiveExpression(null)));
			CodeStatementCollection trueStatements = new CodeStatementCollection();
			trueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("internalId"), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.MiddleTierTable.ElementTable.Name), "FindKey", new CodeExpression[] {new CodeArgumentReferenceExpression("configurationId"), new CodeArgumentReferenceExpression("parameterId"), new CodeCastExpression(typeof(string), new CodeArgumentReferenceExpression("externalId"))})));
			CodeExpression[] exceptionVariables = new CodeExpression[] {new CodePrimitiveExpression(string.Format("The {0} table does not have a record identified by '{1}'", this.MiddleTierTable.ElementTable.Name, "{0}")), new CodeArgumentReferenceExpression("externalId")};
			trueStatements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeCastExpression(returnType, new CodeVariableReferenceExpression("internalId")), CodeBinaryOperatorType.IdentityEquality, returnValue), new CodeThrowExceptionStatement(new CodeObjectCreateExpression("Exception", new CodeExpression[] {new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Format", exceptionVariables)}))));
			CodeStatement[] trueStatementArray = new CodeStatement[trueStatements.Count];
			trueStatements.CopyTo(trueStatementArray, 0);
			this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("externalId"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), trueStatementArray));

			//            // Return the internal identifier.
			//            return internalId;
			this.Statements.Add(new CodeCommentStatement("Return the internal identifier."));
			this.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("internalId")));
		
		}

	}

}
