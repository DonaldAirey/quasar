/* This sample demonstrats how to use the code DOM to generate a strongly typed collection* compile with csc RuleBuilder.cs \r:System.dll*/

using System;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Shadows.Quasar.Rules
{

	public class RuleBuilder
	{

		public static CodeNamespace CreateNamespace()
		{

			// Name space and import declarations
			CodeNamespace nameSpaceRules = new CodeNamespace("Shadows.Quasar.Rules");
			nameSpaceRules.Imports.Add(new CodeNamespaceImport("System"));
			nameSpaceRules.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));

			// Class Rule1
			CodeTypeDeclaration rule1 = new CodeTypeDeclaration(String.Format("Rule{0}", 2));
			rule1.Comments.Add(new CodeCommentStatement("User Defined Rule"));
			rule1.IsClass = true;
			nameSpaceRules.Types.Add(rule1);
			rule1.TypeAttributes = TypeAttributes.Public;

			CodeMemberMethod methodTest = new CodeMemberMethod();
			methodTest.Name = "Test";
			methodTest.ReturnType = new CodeTypeReference(typeof(bool));
			methodTest.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			methodTest.Comments.Add(new CodeCommentStatement("User Defined Test"));

			CodeMethodInvokeExpression messageBoxStatement = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression("System.Windows.Forms.MessageBox"),
				"Show",
				new CodePrimitiveExpression("Hello World!"),
				new CodePrimitiveExpression("Quasar Rules"));
			methodTest.Statements.Add(messageBoxStatement);
			methodTest.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
			rule1.Members.Add(methodTest);

			return nameSpaceRules;

		}

	}

}