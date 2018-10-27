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
	/// Constructs the CodeDOM method used to transform abstract data into a readable document.
	/// </summary>
	public class DataTransformTemplateMethod : System.CodeDom.CodeMemberMethod
	{

		/// <summary>
		/// Constructs the CodeDOM method used to transform abstract data into a readable document.
		/// </summary>
		/// <param name="templateNode">Contains the specification for the template.</param>
		public DataTransformTemplateMethod(DataTransform.TemplateNode templateNode)
		{

			// This is a rudimentary test for the start of the document.  The idea is to follow the XSL language where possible 
			// but the rigours of an XPATH parser are beyond the scope of this function at the time of this writing.  In the future
			// it should be expanded to follow an XPATH like specification into the data model.
			bool isRootTemplate = templateNode.Match.StartsWith("/");

			// Some top-level information will be needed to construct the template, such as the class name to reference constants.
			DataTransform dataTransform = templateNode.TopLevelNode as DataTransform;

			//		/// <summary>
			//		/// Transforms a row of data into screen instructions.
			//		/// </summary>
			//		/// <param name="viewerTable">The current outline level of the document.</param>
			//		/// <param name="dataRow">The source of the data for this template.</param>
			//		public virtual void SlashTemplate(MarkThree.Forms.ViewerTable viewerTable, System.Data.DataRow dataRow)
			//		{
			this.Comments.Add(new CodeCommentStatement(@"<summary>", true));
			this.Comments.Add(new CodeCommentStatement(@"Transforms a row of data into screen instructions.", true));
			this.Comments.Add(new CodeCommentStatement(@"</summary>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""viewerTable"">The current outline level of the document.</param>", true));
			this.Comments.Add(new CodeCommentStatement(@"<param name=""dataRow"">The source of the data for this template.</param>", true));
			this.Name = string.Format("{0}Template", templateNode.Match.Replace("/", "Slash"));
			this.Attributes = MemberAttributes.Public;
			this.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ViewerTable), "viewerTable"));
			string rowType = isRootTemplate ? "DataRow" : string.Format(string.Format("{0}Row", templateNode.Match));
			string rowNamespace = isRootTemplate ? "System.Data" : dataTransform.Source;
			string rowVariableName = CamelCase.Convert(rowType);
			this.Parameters.Add(new CodeParameterDeclarationExpression(string.Format("{0}.{1}", rowNamespace, rowType), rowVariableName));

			// This will create the instructions for generating each row that appears in the template.
			int rowIndex = 0;
			foreach (DataTransform.RowNode rowNode in templateNode.Rows)
			{

				//				// Execution
				//				MarkThree.Forms.ViewerRow viewerRow0 = new MarkThree.Forms.ViewerRow();
				//				viewerRow0.Data = new object[17];
				//				viewerRow0.Tiles = new ViewerTile[17];
				//				viewerRow0.Children = new ViewerTable[2];

				string viewerRowVariableName = string.Format("viewerRow{0}", rowIndex++);
				this.Statements.Add(new CodeCommentStatement(rowNode.RowId));
				this.Statements.Add(new CodeVariableDeclarationStatement(typeof(ViewerRow), viewerRowVariableName, new CodeObjectCreateExpression(typeof(ViewerRow))));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Data"),
				new CodeArrayCreateExpression(typeof(object), new CodePrimitiveExpression(dataTransform.Columns.Count))));
				this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Tiles"),
					new CodeArrayCreateExpression(typeof(ViewerTile), new CodePrimitiveExpression(dataTransform.View.Count))));
				int childTableCount = 0;
				foreach (DataTransform.ApplyTemplateNode applyTemplateNode in rowNode.ApplyTemplates)
					if (applyTemplateNode.RowFilter.ToLower() != bool.FalseString.ToLower())
						childTableCount++;
				if (childTableCount != 0)
					this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Children"),
					new CodeArrayCreateExpression(typeof(ViewerTable), new CodePrimitiveExpression(childTableCount))));

				// Add in any temporary variables that need to be calculated.
				if (rowNode.ScratchList.Count != 0)
				{

					// This will add any temporary variables needed to compute the data in a column.
					this.Statements.Add(new CodeCommentStatement("This will calculate intermediate values used in the row."));
					foreach (DataTransform.ScratchNode scratchNode in rowNode.ScratchList)
						this.Statements.Add(new CodeSnippetExpression(scratchNode.Text.Trim().Trim(';')));

				}

				// This will create instructions for generating each of the tiles that appear in the row.
				this.Statements.Add(new CodeCommentStatement("Populate the data image of the row."));
				foreach (DataTransform.ColumnNode columnNode in dataTransform.Columns)
				{

					// The 'View' describes the order of the columns.  Attempt to find the corresponding column in the row
					// definition.  If it doesn't exist in the row, it will likely screw up the spacing of the document, but it 
					// won't crash with this check.
					DataTransform.TileNode tileNode;
					if (!rowNode.Tiles.TryGetValue(columnNode.ColumnId, out tileNode))
						continue;

					//					viewerRow0.Data[PrototypeViewer.workingStatusImageIndex] = this.variables["WorkingStatusHeaderImage"];
					CodeExpression dataReferenceExpression = tileNode.Data != string.Empty ? (CodeExpression)new CodeSnippetExpression(tileNode.Data) :
						tileNode.VariableName != string.Empty ? (CodeExpression)new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "Variables"), new CodePrimitiveExpression(tileNode.VariableName)) :
						(CodeExpression)new CodePrimitiveExpression(DBNull.Value);
					this.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Index", CamelCase.Convert(columnNode.ColumnId)))),
						dataReferenceExpression));

				}
				
				// This is a common value for all tiles in the row.
				CodeFieldReferenceExpression heightExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Height", CamelCase.Convert(rowNode.RowId)));

				// This will create instructions for generating each of the tiles that appear in the row.
				int tileIndex = 0;
				foreach (DataTransform.ColumnReferenceNode columnReferenceNode in dataTransform.View)
				{

					// The 'View' describes the order of the columns.  Attempt to find the corresponding column in the row
					// definition.  If it doesn't exist in the row, it will likely screw up the spacing of the document, but it 
					// won't crash with this check.
					DataTransform.TileNode tileNode;
					if (!rowNode.Tiles.TryGetValue(columnReferenceNode.ColumnId, out tileNode))
						continue;

					//					// WorkingStatusImage
					//					MarkThree.Forms.ViewerTile viewerTile0 = this.documentViewer.GetTile(executionRow, PrototypeView.workingStatusImageIndex);
					this.Statements.Add(new CodeCommentStatement(columnReferenceNode.ColumnId));
					string tileVariableName = string.Format("viewerTile{0}", tileIndex);
					string constantName = CamelCase.Convert(columnReferenceNode.ColumnId) + "Index";
					this.Statements.Add(new CodeVariableDeclarationStatement(typeof(ViewerTile), tileVariableName, new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "documentViewer"), "GetTile"), new CodeVariableReferenceExpression(rowVariableName), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), constantName))));

					//					string viewerStyleId0 = "HeaderImage";
					//					if (viewerStyleId0 != viewerTile0.ViewerStyleId)
					//					{
					//						viewerTile0.ViewerStyleId = viewerStyleId0;
					//						viewerTile0.IsModified = true;
					//					}
					if (tileNode.StyleId != string.Empty)
					{
						string styleIdVariableName = string.Format("viewerStyleId{0}", tileIndex);
						this.Statements.Add(new CodeVariableDeclarationStatement(typeof(string), styleIdVariableName, new CodeSnippetExpression(tileNode.StyleId)));
						CodeStatement[] trueStatements0 = new CodeStatement[2];
						trueStatements0[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "ViewerStyleId"), new CodeVariableReferenceExpression(styleIdVariableName));
						trueStatements0[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "IsModified"), new CodePrimitiveExpression(true));
						this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(styleIdVariableName), CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "ViewerStyleId")), trueStatements0));
					}

					//					if ((viewerRow0.Data[PrototypeView.workingStatusImageOrdinal].Equals(viewerTile0.Data) != true))
					//					{
					//						viewerTile0.Data = viewerRow0.Data[PrototypeView.workingStatusImageOrdinal];
					//						viewerTile0.IsModified = true;
					//					}
					CodeStatement[] trueStatements = new CodeStatement[2];
					trueStatements[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Data"), new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Index", CamelCase.Convert(columnReferenceNode.ColumnId)))));
					trueStatements[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "IsModified"), new CodePrimitiveExpression(true));
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Data"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Index", CamelCase.Convert(columnReferenceNode.ColumnId)))), "Equals",
						new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Data")), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(true)), trueStatements));

					//					SizeF sizeF0 = new SizeF(PrototypeView.workingStatusImageWidth, PrototypeView.headerHeight);
					//					if ((sizeF0 != viewerTile0.RectangleF.Size))
					//					{
					//						viewerTile0.RectangleF.Size = sizeF0;
					//						viewerTile0.IsModified = true;
					//					}
					//CodeFieldReferenceExpression widthExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Width", CamelCase.Convert(columnReferenceNode.ColumnId)));
					//string sizeVariableName = string.Format("sizeF{0}", tileIndex);
					//this.Statements.Add(new CodeVariableDeclarationStatement(typeof(SizeF), sizeVariableName, new CodeObjectCreateExpression(typeof(SizeF), widthExpression, heightExpression)));
					//CodeStatement[] trueStatements1 = new CodeStatement[2];
					//trueStatements1[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Size"), new CodeVariableReferenceExpression(sizeVariableName));
					//trueStatements1[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "IsModified"), new CodePrimitiveExpression(true));
					//this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(sizeVariableName), CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Size")), trueStatements1));

					//					if ((PrototypeView.workingStatusImageWidth != viewerTile0.RectangleF.Width))
					//					{
					//						viewerTile0.RectangleF.Width = PrototypeView.workingStatusImageWidth;
					//						viewerTile0.IsModified = true;
					//					}
					CodeFieldReferenceExpression widthExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Width", CamelCase.Convert(columnReferenceNode.ColumnId)));
					CodeStatement[] trueStatements1 = new CodeStatement[2];
					trueStatements1[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Width"), widthExpression);
					trueStatements1[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "IsModified"), new CodePrimitiveExpression(true));
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(widthExpression, CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Width")), trueStatements1));

					//					if ((PrototypeView.headerHeight != viewerTile0.RectangleF.Height))
					//					{
					//						viewerTile0.RectangleF.Height = PrototypeView.headerHeight;
					//						viewerTile0.IsModified = true;
					//					}
					CodeStatement[] trueStatements2 = new CodeStatement[2];
					trueStatements2[0] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Height"), heightExpression);
					trueStatements2[1] = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "IsModified"), new CodePrimitiveExpression(true));
					this.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(heightExpression, CodeBinaryOperatorType.IdentityInequality, new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "RectangleF"), "Height")), trueStatements2));

					//					viewerTile0.Cursor.Width = PrototypeView.workingStatusImageWidth;
					//					viewerTile0.Cursor.Height = 0;
					if (tileIndex < dataTransform.View.Count - 1)
					{
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Cursor"), "Width"), widthExpression));
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Cursor"), "Height"), new CodePrimitiveExpression(0.0f)));
					}
					else
					{
						CodeExpression columnWidthExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Width", CamelCase.Convert(dataTransform.View[tileIndex].Column.ColumnId)));
						CodeExpression rowWidthExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Width", CamelCase.Convert(rowNode.RowId)));
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Cursor"), "Width"), new CodeBinaryOperatorExpression(columnWidthExpression, CodeBinaryOperatorType.Subtract, rowWidthExpression)));
						this.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(tileVariableName), "Cursor"), "Height"), heightExpression));
					}

					//					viewerRow0.Tiles[PrototypeViewer.statusImageOrdinal] = viewerTile0;
					this.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Tiles"),
						new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.DataTransformId), string.Format("{0}Ordinal", CamelCase.Convert(columnReferenceNode.ColumnId)))),
						new CodeVariableReferenceExpression(tileVariableName)));

					// This indexer will keep the variable names distinct for each column.
					tileIndex++;
				}

				// Each row can have one or more relationship to child rows that are specified in the 'ApplyTemplate' node.  This 
				// specification works very similar to the XSL analog in that it traces through the child nodes seeing if there are
				// any templates that can be applied.
				int childIndex = 0;
				foreach (DataTransform.ApplyTemplateNode applyTemplateNode in rowNode.ApplyTemplates)
				{

					// It's possible for debugging to include a 'False' row filter, which would remove all the children from the
					// applied templates.  Unfortunately, an expression that always evaluates to 'False' will result in a warning
					// about unreachable code.  This will remove the entire reference to the children when the row filter 
					// expression is 'False'.
					if (applyTemplateNode.RowFilter.ToLower() != bool.FalseString.ToLower())
					{

						//						// This will add the child relations for this row to the document.
						//						MarkThree.Forms.ViewerTable workingOrderTable = new MarkThree.Forms.ViewerTable();
						//						for (int rowIndex = 0; (rowIndex < SimulatedDatabase.WorkingOrder.Count); rowIndex = (rowIndex + 1))
						//						{
						//							SimulatedDatabase.WorkingOrderRow workingOrderRow = SimulatedDatabase.WorkingOrder[rowIndex];
						//							if (workingOrderRow.StatusCode != 6)
						//							{
						//								this.WorkingOrderTemplate(workingOrderTable, workingOrderRow);
						//							}
						//						}
						this.Statements.Add(new CodeCommentStatement("This will add the child relations for this row to the document."));
						string tableVariableName = string.Format("{0}Table", CamelCase.Convert(applyTemplateNode.Select));
						this.Statements.Add(new CodeVariableDeclarationStatement(typeof(ViewerTable), tableVariableName, new CodeObjectCreateExpression(typeof(ViewerTable))));
						CodeExpression childRows = isRootTemplate ?
							(CodeExpression)new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(dataTransform.Source), applyTemplateNode.Select) :
							(CodeExpression)new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression(rowVariableName), string.Format("Get{0}Rows", applyTemplateNode.Select)));
						string countingMember = isRootTemplate ? "Count" : "Length";
						CodeStatement initializationStatement = new CodeVariableDeclarationStatement(typeof(int), "rowIndex", new CodePrimitiveExpression(0));
						CodeExpression testExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowIndex"), CodeBinaryOperatorType.LessThan, new CodeFieldReferenceExpression(childRows, countingMember));
						CodeStatement incrementStatement = new CodeAssignStatement(new CodeVariableReferenceExpression("rowIndex"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("rowIndex"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));
						CodeStatementCollection iterationBody = new CodeStatementCollection();
						string childRowName = CamelCase.Convert(string.Format("{0}Row", applyTemplateNode.Select));
						iterationBody.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(string.Format("{0}.{1}Row", dataTransform.Source, applyTemplateNode.Select)), childRowName, new CodeIndexerExpression(childRows, new CodeVariableReferenceExpression("rowIndex"))));
						if (applyTemplateNode.RowFilter == string.Empty)
							iterationBody.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), string.Format("{0}Template", applyTemplateNode.Select), new CodeVariableReferenceExpression(tableVariableName), new CodeVariableReferenceExpression(childRowName))));
						else
							iterationBody.Add(new CodeConditionStatement(new CodeSnippetExpression(applyTemplateNode.RowFilter), new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), string.Format("{0}Template", applyTemplateNode.Select), new CodeVariableReferenceExpression(tableVariableName), new CodeVariableReferenceExpression(childRowName)))));
						CodeStatement[] iterationArray = new CodeStatement[iterationBody.Count];
						iterationBody.CopyTo(iterationArray, 0);
						this.Statements.Add(new CodeIterationStatement(initializationStatement, testExpression, incrementStatement, iterationArray));
						if (applyTemplateNode.Sorts.Count > 0)
							this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(tableVariableName), "Sort", new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), string.Format("{0}RowComparer", CamelCase.Convert(applyTemplateNode.Select)))));

						//						// The child table is associated to the parent row here.
						//						viewerRow0.Children[0] = workingOrderTable;
						this.Statements.Add(new CodeCommentStatement("The child table is associated to the parent row here."));
						this.Statements.Add(new CodeAssignStatement(new CodeIndexerExpression(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(viewerRowVariableName), "Children"), new CodePrimitiveExpression(childIndex)), new CodeVariableReferenceExpression(tableVariableName)));
					}

				}

				//				// The child rows are added to the parent tables when they've been built.  In this way the hierarchical document is
				//				// built.
				//				viewerTable.Add(viewerRow0);
				this.Statements.Add(new CodeCommentStatement("The child rows are added to the parent tables when they've been built.  In this way the hierarchical document is"));
				this.Statements.Add(new CodeCommentStatement("built."));
				this.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("viewerTable"), "Add", new CodeVariableReferenceExpression(viewerRowVariableName)));

			}

		}

	}

}
