namespace MarkThree.Forms
{

	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Creates a constructor for a class that transforms abstract data into a human read-able document.
	/// </summary>
	public class DataTransformConstructor : System.CodeDom.CodeConstructor
	{

		/// <summary>
		/// Creates a constructor for a class that transforms abstract data into a human read-able document.
		/// </summary>
		public DataTransformConstructor(DataTransform dataTransform)
		{

			//		/// <summary>
			//		/// Create a view of the data.
			//		/// </summary>
			//		public PrototypeView(MarkThree.Forms.DocumentViewer documentViewer) : 
			//				base(documentViewer)
			//		{
			this.Comments.Add(new CodeCommentStatement("<summary>", true));
			this.Comments.Add(new CodeCommentStatement("Create a view of the data.", true));
			this.Comments.Add(new CodeCommentStatement("</summary>", true));
			this.Attributes = MemberAttributes.Public;
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(MarkThree.Forms.DocumentViewer), "documentViewer"));
			this.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("documentViewer"));

			// Initialize the comparers with instances of the comparison classes.
			//			// These objects are used to sort the rows.
			//			this.workingOrderRowComparer = new WorkingOrderRowComparer();
			//			this.sourceOrderRowComparer = new SourceOrderRowComparer();
			//			this.destinationOrderRowComparer = new DestinationOrderRowComparer();
			//			this.executionRowComparer = new ExecutionRowComparer();
			bool isFirstComparer = true;
			foreach (DataTransform.TemplateNode templateNode in dataTransform.Templates)
				foreach (DataTransform.RowNode rowNode in templateNode.Rows)
					foreach (DataTransform.ApplyTemplateNode applyTemplateNode in rowNode.ApplyTemplates)
						if (applyTemplateNode.Sorts.Count != 0)
						{
							string comparerName = CamelCase.Convert(applyTemplateNode.Select) + "RowComparer";
							CodeAssignStatement codeAssignmentStatement = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), comparerName), new CodeObjectCreateExpression(applyTemplateNode.Select + "RowComparer", new CodeExpression[] { }));
							if (isFirstComparer)
							{
								isFirstComparer = false;
								this.Statements.Add(new CodeCommentStatement("These objects are used to sort the rows."));
							}
							this.Statements.Add(codeAssignmentStatement);
						}

			// The 'Variable' table is a cache where objects that require significan resources (e.g. Bitmaps) can be kept.  The 
			// idea is to generate them once and store them in the variable table.  Any reference to the variable after that will
			// pick up the copy stored in the hash table instead of generating the object at the time it is referenced.
			//			// Initialize the global variables
			//			if (!this.variables.ContainsKey("WorkingStatusHeaderImage"))
			//				this.variables.Add("WorkingStatusHeaderImage", new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEQAACxEBf2RfkQAAAelJREFUOE+t0t9PUmEYB3C778/of2htuTBZZ24ao5GWKMrRA5wDIqCw+JEdDCkMGd20+GUoDQ+r7ALxQoaLETdsLBZ3navGRRPWYnLvtyNObl6nY/O9eS/e5/28z/O8z8DAda1IJMLHE5tHoVCwYzVrM4FV49hLDyNbeUaPuO3ad163XvQ46CPX8qxo5SY+Ee9ms9k/nU4H2x93sWSdQWjdguArM/w8B6+HxQvnHIJrJoQ3LFhenPxHAIIg/Gg2mwi/jYLVT3QDw28sWFvRw76o7gLrvjPAzI7/JoBEIrHfbrfxPrINHfOkB/ie6wiAnVP+JIBoNLrZarWQ3BLAzD+9FKCnRr8TQCwW8zcaDezsfJEAtdQDc7eE8wx413yvB5OqB/sEkEwmF0RRxO7XPakEDTYCC70eOCxTcNtn4FrS4LWPg+rhfYEAUqnU43q9jr3cATgDDb/XgIDPCOkbYdKpQKtHt6bHRyi1ijJI++2LMhisVqs4PPwGhmGgHBv+zNGPOEajMLBa5ZB04calM5fJZG6VSiWUy2VwRhPu3B3i+xrSXC53s1AonFQqFdhsNshksnBfwGlwsVg8rtVqcDqdkMvlH/oG8vn8L2mB53lQFEXO+1ViOp12xOPxv1IJdYVCce+q+Gs5/w/Ocy+mFPCPZQAAAABJRU5ErkJggg=="))));
			//			if (!this.variables.ContainsKey("SubmittedStatusHeaderImage"))
			//				this.variables.Add("SubmittedStatusHeaderImage", new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAf5JREFUOE+1UzmLmkEY3qQNKdKnTLN/ImkEtxAVb/G+L7wPVFDBY1HwBFHRKCooaiGiVmJj9j+ks0iRcgkEwrIu2SczU4TAZ8AU+WAY+Gae433ed66u/sfndrvfmc3m7mAwODqdztLFGqlUSjwejz/tdrufBPwsl8shkUiMfxC8+CuZ3W5/tdlsvubzefD5fNzc3EAgEEAoFH5Uq9Xvs9lsJRqNHj0ez9uzJCaTybrdbk88Hg8ikQhUXSaTIRQKodVqYTKZoNPpgJR3r9frrzkkuVzuczweZyCVSgWfz4dut4v5fI7RaIR2u41arQbihJ5/5xAkk8mnu7s7rNdrFAoFGAwGHA4HNBoNNJtNVCoVBAIB9l+pVD5xCIj6IykBy+USi8WCEZFMsN/vEQ6HYbPZ2CLdoQ64BLFY7HG1WmE2m2E4HLK6q9UqSHAMSEJmO8nqvINIJPIwnU4ZuNfrMduUIJFIMDBdFosFRqMRCoWC68Dv9z/0+32WNAXTwEqlEogzWK3W32CdTkc7xCXwer0/qO16vY5yuYxisQg6E8FgkAVHFwVrNBraqRMnRKKydblcp0wmw5Rpu9LpNMjgMJBWq2XtJfa/kP3D2WEiIb0hF29J0keHw/FM6ydvgYZGgd/ImZmM9suL3gZRuZZKpUMCvBeLxbeE5PVFwH+99Auih0hpPDuhBAAAAABJRU5ErkJggg=="))));
			//			if (!this.variables.ContainsKey("IsInstitutionMatchHeaderImage"))
			//				this.variables.Add("IsInstitutionMatchHeaderImage", new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEQAACxEBf2RfkQAAAlxJREFUOE+tU91L2lEYbu2DYH9EN7v1avcTwQoaSqCQmmL5XZafCyWFNMWPQFekDrZyhmYUjSDoIvFGhl7oxcb0ShAm1uVw93M8e8+BJHGX/eBwfpxz3ud9n+d534mJx/rsdvtzlUr14h7P6XS+XF9fr6+trW0/zLG4uPg0EAg8GclrNBqfuVyuKj3+oVQqp9jl1tbWe4vFgpWVlT8mk+k1O6PAKYfD8c1sNt+IRKLJIQgdbttsNvYYGo0mbbVa1QQ4cLvd0Ov1kMvl3zc2Nt5sbm6m7t8tLS29GwLQ5W0sFgPt0Gq1oErg8/n4YmfLy8ugrGAVhUIhEDXIZLKfQ4BIJFJMp9O4urrCYDDA2dkZut0uTk9PEY1GIRQKodPpsLCwgMvLS5BemJub+zAEODg4+MgADg8P0e/3kcvl0Ov1sL+/zzNKpVK+Ez0cHR2BUSPQ2BAgmUya2MXx8THu7u7QbrfRarXQaDRA1YGE5RUwCsFgEOQESETFQwqT4XD4S71eR6fTQbPZ5MGlUgkkMMglrgv7Z6LOzs5+ouBRK3d3dwOVSgXVahWZTAa1Wg3n5+fcGXKFB3s8Hi7mzMyMdaz/CMBbLpfB3FCr1SgWi1zE1dVVkKU82O/3cxpisVg3BhCPx+3X19dIJBI86OTkBPl8nmf2er18MRrsjvgrxwAos/ni4gJ7e3tc5UKhgGw2ywOpA/liYKw6Ang7BkBWvtrZ2alQwG9qlF+pVIpbxpqJ+U5z8nd+fv6G/HcLBILhzPx3FonztMFg+EwdeKtQKL5S1zkkEsn0Yw3uCM4/B7t+5RBCaBIAAAAASUVORK5CYII="))));
			//			if (!this.variables.ContainsKey("IsBrokerMatchHeaderImage"))
			//				this.variables.Add("IsBrokerMatchHeaderImage", new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEQAACxEBf2RfkQAAAd9JREFUOE/NUrur0mEYPhGnPfoXWs4gR0xFRLzkAf0hKnjFW97FRXTQwRsI4eAVQRAX0RBxCYMwBMstxHQQN4eWGmoIaqkzHOo8vd8PklryTNEHH9/yvs/33E5O/uuTTqe5fD7/mu7nVCr1IxKJXDmdzo8Gg+GVUqm8OJAPBAK3m82mtN1u3/9d0WAw+LDdbjGdTtHr9VCpVJDJZODz+aDRaL4cZovF4svFYoHhcHjdaDTmhUKBK5VKt4jBi263i1wuh2AwCJvNBo7j2DIUCsXTA0AoFLqs1+uYzWbY7XZgYK1W6y1RfpxMJj91Oh34/X4QdajVaqhUqq1QKDw9ALhcrpLdbr/2er2IxWKo1WqYz+dYLpfY7/dYr9eoVquIRqOwWq08A6lU+uiPAD0ezwWBFMikvtvtfkOgl/F4HKvVimfEpJCRcDgc0Gq1EIvFob82QKfT3SG9TyaTCfr9PsgnkCRYLBZehkgkMh6tEKVzzn5mCWSzWZBXMJvNzEAG8OAoAMm6m0gkvpbLZf73X/qJ/jeBQHDvJgD2zWaD8XiMcDgMo9HI65fJZIxB4CgAZX9Gffg+Go1AL98F1gOJRHIll8vPjwKwAZPJ9JDyf6bX699Rfd/T8nO6yhst/5Ohn3dZ+oOFdumLAAAAAElFTkSuQmCC"))));
			//			if (!this.variables.ContainsKey("IsHedgeMatchHeaderImage"))
			//				this.variables.Add("IsHedgeMatchHeaderImage", new Bitmap(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAklJREFUOE+tUzlPclEQ9RfSU7mEYEGDkJAQqCwgoTZIVISwKKtLYWiFAuKnEKQg7AGCIJDHvis538zNJ3z0vmTy7n3vzjkz59zZ2/uNx+FwdAOBwMrn8829Xu/U7XaPKQYul0vif06n8/P6+rp5dXXVsNvtNVpXbDbbnw233+9v4d+zWq3QaDSwXq9FFItF8Lflcol8Po/FYoGzszMQ2HwDQMyfnP/9/S0O1Go1kfD19SWS5vM5RqMRCoUCgsEg5HI5Li8vtwAej6dTLpdRrVbx/v4OhUIBg8GAer0OjUaDaDSKXq+HSCSCg4MDce7+/n61qYBQu1wB9QWZTIZQKASj0QjSAMfHx7BYLHh8fBTMr6+vGA6HXMkWgDSQDg8PodPpRKkfHx+YTqc4OTkRYFqtFiqVCuFwGP1+X/x7eHjYAejF43FRcqvVQi6XE+IxwP7+PqxWK05PT3FzcyM0YDCqcKsBWddn4UqlEiqVCrLZLMxmM1KpFF5eXtBsNpFIJHB0dASyl9lxfn4+/d+F4Ww2w2AwQLvdFokmkwmTyQSZTEYIyPH29ibe3AZVstgA3N7ejhmAE5hNrVbj6elJ7NPpNLrdLjqdjgDmFpmEcrYAZOOE7WPBlEol9Ho9np+fBXssFhPWMhDbyW/eUys7AFP2nstmj1kLSZIEczKZFIzMzBayQ7zeqYBsnLN4XCa3wLfvp1dmZDDe8/rnDAm/rYDvNV2aFQ3NjAZlTAMzoJDoe+fi4qJJ0aBLViXlKxQFsjVJ83D3G4O89xeGEXGAvz4xDAAAAABJRU5ErkJggg=="))));
			bool isFirstVariable = true;
			foreach (DataTransform.VariableNode variableNode in dataTransform.Variables)
			{
				if (isFirstVariable)
				{
					isFirstVariable = false;
					this.Statements.Add(new CodeCommentStatement("Initialize the global variables"));
				}
				this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "Variables"), "ContainsKey", new CodePrimitiveExpression(variableNode.Name)), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(false)),
					new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "Variables"), "Add"), new CodePrimitiveExpression(variableNode.Name), new CodeSnippetExpression(variableNode.Select)))));
			}

		}

	}

}
