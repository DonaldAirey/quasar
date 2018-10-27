namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Threading;
	using System.Xml;
	
	/// <summary>
	/// Translates an incoming XSL stylesheet into a data structure.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class StylesheetWriter : System.Xml.XmlWriter
	{

		private MarkThree.Forms.Lexicon lexicon;
		private MarkThree.Forms.Stylesheet stylesheet;
		private System.Collections.Generic.Stack<Driver> driverStack;
		private System.Collections.Generic.Stack<Node> nodeStack;
		private System.String attributePrefix;
		private System.String attributeNamespaceURI;
		private System.String attributeLocalName;

		/// <summary>
		/// Creates a writer to translate an incoming XSL stylesheet into a data structure.
		/// </summary>
		public StylesheetWriter(Stylesheet stylesheet)
		{

			// Initialize the object;
			this.stylesheet = stylesheet;

			// This holds a stack of the tokens as they're parsed from the incoming XSL stylesheet.
			this.driverStack = new Stack<Driver>();
			
			// This stack holds the current state of parsing the XSL language elements.  It is needed to build a tree of these
			// structures that is roughly the same as the incoming XSL document.  The stack structure is required because the
			// parsing can recursively dig down into the data structures, the unwind back up the tree before reading more child
			// elements.  The stack structure keeps track of how far down into the tree the parsing has gone.
			this.nodeStack = new Stack<Node>();

			// This will create a lexical analyzer for the XSL Stylesheets and installs handlers for each of the tokens read from
			// the XML stream as it parses the stylesheet.
			this.lexicon = new Lexicon();
			this.lexicon.InstallHandlers(Token.Alignment, new TokenHandler(this.ParseAlignmentStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Animation, new TokenHandler(this.ParseAnimationStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.ApplyTemplate, new TokenHandler(this.ParseApplyTemplateStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Attribute, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Border, new TokenHandler(this.ParseBorderStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Borders, new TokenHandler(this.ParseBordersStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Cell, new TokenHandler(this.ParseCellStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Choose, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Column, new TokenHandler(this.ParseColumnStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Columns, new TokenHandler(this.ParseColumnsStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.ComponentOptions, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Constraint, new TokenHandler(this.ParseConstraintStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.ConstraintColumn, new TokenHandler(this.ParseConstraintColumnStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Data, new TokenHandler(this.ParseDataStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Delete, new TokenHandler(this.ParseDeleteStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Display, new TokenHandler(this.ParseDisplayStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.DoNotSelectRows, new TokenHandler(this.ParseDoNotSelectRowsStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Font, new TokenHandler(this.ParseFontStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Fragment, new TokenHandler(this.ParseFragmentStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.HideOfficeLogo, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.If, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Insert, new TokenHandler(this.ParseInsertStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Interior, new TokenHandler(this.ParseInteriorStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.NumberFormat, new TokenHandler(this.ParseNumberFormatStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Otherwise, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Printer, new TokenHandler(this.ParsePrinterStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Protection, new TokenHandler(this.ParseProtectionStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Row, new TokenHandler(this.ParseRowStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Style, new TokenHandler(this.ParseStyleStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Styles, new TokenHandler(this.ParseStylesStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Stylesheet, new TokenHandler(this.ParseStylesheetStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Table, new TokenHandler(this.ParseTableStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Template, new TokenHandler(this.ParseTemplateStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Toolbar, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Update, new TokenHandler(this.ParseUpdateStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.ValueOf, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Variable, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.View, new TokenHandler(this.ParseViewStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.ViewColumn, new TokenHandler(this.ParseViewColumnStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.When, new TokenHandler(this.ParseXslElementStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Workbook, new TokenHandler(this.ParseWorkbookStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.Worksheet, new TokenHandler(this.ParseWorksheetStart), new TokenHandler(this.PopNodeStack));
			this.lexicon.InstallHandlers(Token.WorksheetOptions, new TokenHandler(this.ParseWorksheetOptionsStart), new TokenHandler(this.PopNodeStack));

		}

		/// <summary>
		/// Converts a text string into a System.Drawing.Color value.
		/// </summary>
		/// <param name="colorText">A formatted string containg RGB colors.</param>
		/// <returns>A Color made from the values found in the argument string.</returns>
		private static Color ConvertToColor(string colorText)
		{

			// Extract the RGB values from the string and reconstitute them into a System.Drawing.Color value.
			int rgb = Convert.ToInt32(colorText.Substring(1, colorText.Length - 1), 16);
			return Color.FromArgb(rgb >> 16 & 0xFF, rgb >> 8 & 0xFF, rgb & 0xFF);

		}

		/// <summary>
		/// This removes a stylesheet node from the stack when the End Element is read.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward-only access to XML data.</param>
		public void PopNodeStack()
		{

			// Remove the XSL element from the stack.  This method is called as a stub from the token parsing method.  Often
			// because a 'Parse<element>Start' method pushes something onto the stack.
			this.nodeStack.Pop();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Stylesheet Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseStylesheetStart()
		{

			// This is the root element of the Stylesheet data strucure.  Several global references are kept to provide access to
			// certain parts of the stylesheet datastructure.  For example, the list of Templates is used to add a section that
			// will match against incoming tokens to produce the XML output.  The list of Columns allows for a reordering of the
			// columns in the output document.  The list of Styles allows for modification of colors, borders, etc.
			this.stylesheet.stylesheetNode = new Stylesheet.StylesheetNode(this.stylesheet);

			// The Stylesheet datastructure is a hierarchical structure.  The element on the top of the Node stack is where the
			// child nodes are attached as they are parsed.  Since this is the top element, there is no parent to which this node
			// can be attached.
			this.nodeStack.Push(this.stylesheet.stylesheetNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Template Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseTemplateStart()
		{

			// Create a new Template Node and initialize it from the incoming XSL data.
			Stylesheet.TemplateNode templateNode = new Stylesheet.TemplateNode();

			// Add the Template Node to current parent node and then push it onto the stack.  These elements will form a tree
			// structure roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(templateNode);
			this.nodeStack.Push(templateNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Workbook Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseWorkbookStart()
		{

			// Create a new Workbook Node.
			Stylesheet.WorkbookNode workbook = new Stylesheet.WorkbookNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure 
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(workbook);
			this.nodeStack.Push(workbook);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the List of Style Elements.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseStylesStart()
		{

			// Create a new list of styles.
			Stylesheet.StylesNode styles = new Stylesheet.StylesNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(styles);
			this.nodeStack.Push(styles);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for a Style.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseStyleStart()
		{

			// Create a new Style Node and initialize it from the incoming XSL data.
			Stylesheet.StyleNode styleNode = new Stylesheet.StyleNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(styleNode);
			this.nodeStack.Push(styleNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the the Alignment of objects inside a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseAlignmentStart()
		{

			// Create a new Alignment Node and initialize with the XSL data.
			Stylesheet.AlignmentNode alignmentNode = new Stylesheet.AlignmentNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(alignmentNode);
			this.nodeStack.Push(alignmentNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Borders that outline a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseBordersStart()
		{

			// Create a set of borders to contain the description of the outline of a cell.
			Stylesheet.BordersNode bordersNode = new Stylesheet.BordersNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(bordersNode);
			this.nodeStack.Push(bordersNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the a border that outlines a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseBorderStart()
		{

			// Create a borders and initialize it with the XSL data.
			Stylesheet.BorderNode borderNode = new Stylesheet.BorderNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(borderNode);
			this.nodeStack.Push(borderNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Font used to write text inside a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseFontStart()
		{

			// Create a new Font Node and initialize it from the incoming XSL data.
			Stylesheet.FontNode fontNode = new Stylesheet.FontNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(fontNode);
			this.nodeStack.Push(fontNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the effects used to highlight a change of data in a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseAnimationStart()
		{

			// Create a new Animation Node and initialize it from the incoming XSL data.
			Stylesheet.AnimationNode animationNode = new Stylesheet.AnimationNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(animationNode);
			this.nodeStack.Push(animationNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the background of a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseInteriorStart()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			Stylesheet.InteriorNode interiorNode = new Stylesheet.InteriorNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(interiorNode);
			this.nodeStack.Push(interiorNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the formatting instructions to turn public values into readable text.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseNumberFormatStart()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			Stylesheet.NumberFormatNode numberFormatNode = new Stylesheet.NumberFormatNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(numberFormatNode);
			this.nodeStack.Push(numberFormatNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the an instruction to prevent a cell from being edited.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseProtectionStart()
		{

			// Create a new Protection Node and initialize it from the incoming XSL data.
			Stylesheet.ProtectionNode protectionNode = new Stylesheet.ProtectionNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(protectionNode);
			this.nodeStack.Push(protectionNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of the screen specific style instructions.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseDisplayStart()
		{

			// Create a new Display Node.
			Stylesheet.DisplayNode displayNode = new Stylesheet.DisplayNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(displayNode);
			this.nodeStack.Push(displayNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the hardcopy specific style instructions.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParsePrinterStart()
		{

			// Create a new Printer Node.
			Stylesheet.PrinterNode printerNode = new Stylesheet.PrinterNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(printerNode);
			this.nodeStack.Push(printerNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Table Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseTableStart()
		{

			// Create a Table Node and initialize from the incoming XSL data.
			Stylesheet.TableNode tableNode = new Stylesheet.TableNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure 
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(tableNode);
			this.nodeStack.Push(tableNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the a list of spreadsheets.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseWorksheetStart()
		{

			// Create a Worksheet Node and initialize from the incoming XSL data.
			Stylesheet.WorksheetNode worksheet = new Stylesheet.WorksheetNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(worksheet);
			this.nodeStack.Push(worksheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the a list of spreadsheets.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseWorksheetOptionsStart()
		{

			// Create a WorksheetOptions Node.
			Stylesheet.WorksheetOptions worksheetOptions = new Stylesheet.WorksheetOptions();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(worksheetOptions);
			this.nodeStack.Push(worksheetOptions);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the a list of spreadsheets.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseDoNotSelectRowsStart()
		{

			// Create a WorksheetOptions Node.
			Stylesheet.DoNotSelectRows doNotSelectRows = new Stylesheet.DoNotSelectRows();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(doNotSelectRows);
			this.nodeStack.Push(doNotSelectRows);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a list of columns.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseColumnsStart()
		{

			// Create a Columns Node.
			Stylesheet.ColumnsNode columnsNode = new Stylesheet.ColumnsNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(columnsNode);
			this.nodeStack.Push(columnsNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of a column.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseColumnStart()
		{

			// Create a Column Node and initialize from the incoming XSL data.
			Stylesheet.ColumnNode column = new Stylesheet.ColumnNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(column);
			this.nodeStack.Push(column);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the columns that describe a unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseConstraintStart()
		{

			// Create a Constraint Node and initialize from the incoming XSL data.
			Stylesheet.ConstraintNode constraintNode = new Stylesheet.ConstraintNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(constraintNode);
			this.nodeStack.Push(constraintNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for a column that describes the unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseConstraintColumnStart()
		{

			// Create a ConstraintColumn Node and initialize from the incoming XSL data.
			Stylesheet.ConstraintColumnNode constraintColumnNode = new Stylesheet.ConstraintColumnNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(constraintColumnNode);
			this.nodeStack.Push(constraintColumnNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a description of how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseViewStart()
		{

			// Create a View Node and initialize from the incoming XSL data.
			Stylesheet.ViewNode viewNode = new Stylesheet.ViewNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(viewNode);
			this.nodeStack.Push(viewNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the columns that describe how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseViewColumnStart()
		{

			// Create a ViewColumn Node and initialize from the incoming XSL data.
			Stylesheet.ViewColumnNode viewColumnNode = new Stylesheet.ViewColumnNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(viewColumnNode);
			this.nodeStack.Push(viewColumnNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the elements that instruct and direct the XSL transform.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseXslElementStart()
		{

			// Create a XslElement Node and initialize from the incoming XSL data.
			Stylesheet.XslElementNode xslElementNode = new Stylesheet.XslElementNode();
			xslElementNode.token = this.driverStack.Peek().Token;

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(xslElementNode);
			this.nodeStack.Push(xslElementNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the 'apply-template' Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseApplyTemplateStart()
		{

			// Create a ApplyTemplate Node.
			Stylesheet.ApplyTemplate applyTemplate = new Stylesheet.ApplyTemplate();

			// Create a new 'apply-template' record, add it to current parent node and then push it onto the stack.  These 
			// elements will form a tree structure roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(applyTemplate);
			this.nodeStack.Push(applyTemplate);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Fragment Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseFragmentStart()
		{

			// Create a Fragment Node and initialize from the incoming XSL data.
			Stylesheet.FragmentNode fragmentNode = new Stylesheet.FragmentNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(fragmentNode);
			this.nodeStack.Push(fragmentNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Insert Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseInsertStart()
		{

			// Create a Insert Node and initialize from the incoming XSL data.
			Stylesheet.InsertNode insertNode = new Stylesheet.InsertNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(insertNode);
			this.nodeStack.Push(insertNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Update Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseUpdateStart()
		{

			// Create a Update Node and initialize from the incoming XSL data.
			Stylesheet.UpdateNode updateNode = new Stylesheet.UpdateNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(updateNode);
			this.nodeStack.Push(updateNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Delete Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseDeleteStart()
		{

			// Create a Delete Node and initialize from the incoming XSL data.
			Stylesheet.DeleteNode deleteNode = new Stylesheet.DeleteNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(deleteNode);
			this.nodeStack.Push(deleteNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of a row of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseRowStart()
		{

			// Create a Row Node and initialize from the incoming XSL data.
			Stylesheet.RowNode rowNode = new Stylesheet.RowNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(rowNode);
			this.nodeStack.Push(rowNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of an individual cell of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseCellStart()
		{

			// Create a Cell Node and initialize from the incoming XSL data.
			Stylesheet.CellNode cellNode = new Stylesheet.CellNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(cellNode);
			this.nodeStack.Push(cellNode);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the data that goes into a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public void ParseDataStart()
		{

			// Create a Data Node and initialize from the incoming XSL data.
			Stylesheet.DataNode dataNode = new Stylesheet.DataNode();

			// Add this node to current parent node and then push it onto the stack.  These elements will form a tree structure
			// roughly equivalent to the same form they had in the XML file.
			this.nodeStack.Peek().Add(dataNode);
			this.nodeStack.Push(dataNode);

		}

		/// <summary>
		/// Write the document to an XML Writer.
		/// </summary>
		/// <param name="xmlWriter">An XML Writer.</param>
		public void WriteXml(XmlWriter xmlWriter)
		{

			// The XML Token Writer works much like an XmlWriter but it can accept Tokens as input and writes the fully qualified
			// name to the output device.
			XmlTokenWriter xmlTokenWriter = new XmlTokenWriter(xmlWriter);
			
			try
			{

				// Make sure that the Xsl Datastructure isn't modified while it is being written to the XML output device.
				this.stylesheet.readerWriterLock.AcquireReaderLock(Timeout.Infinite);

				// This will recursively scan through the entire data structure and create XML from the public data.
				if(this.stylesheet.stylesheetNode != null)
					this.stylesheet.stylesheetNode.Save(xmlTokenWriter);
			
				// Flush the writer when finished.
				xmlTokenWriter.Flush();

			}
			finally
			{

				// The locks are no longer needed.
				this.stylesheet.readerWriterLock.ReleaseReaderLock();

			}

		}

		public override void Close()
		{
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteCData(string text)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteCharEntity(char ch)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteComment(string text)
		{

			this.nodeStack.Peek().Add(new Stylesheet.CommentNode(text));

		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteEndAttribute()
		{

			if (this.attributeLocalName != "xmlns" && this.attributePrefix != "xmlns")
			{

				Driver driver = this.driverStack.Peek();
				if (driver.ElementEndHandler != null)
					driver.ElementEndHandler();
				this.driverStack.Pop();

			}

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteEndDocument()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteEndElement()
		{

			Driver driver = this.driverStack.Peek();

			if (driver.ElementEndHandler != null)
				driver.ElementEndHandler();

			this.driverStack.Pop();

		}

		public override void WriteEntityRef(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteFullEndElement()
		{

			Driver driver = this.driverStack.Peek();

			if (driver.ElementEndHandler != null)
				driver.ElementEndHandler();

			this.driverStack.Pop();

		}

		public override void WriteProcessingInstruction(string name, string text)
		{

			switch (name)
			{
			case "xml":

				Stylesheet.ProcessingInstructionNode processingInstructionNode = new Stylesheet.ProcessingInstructionNode();
				
				foreach (string arguments in text.Split(new char[] {' '}))
				{

					string[] keyPair = arguments.Trim().Split(new char[] { '=' });
					string key = keyPair[0];
					string value = keyPair[1].Trim(new char[] { '\"' });

					switch (key)
					{
					case "version":

						processingInstructionNode.Version = Convert.ToSingle(value);
						break;

					case "encoding":

						switch (value)
						{

						case "utf-8":

							processingInstructionNode.Encoding = System.Text.Encoding.UTF8;
							break;

						}

						break;

					case "standalone":

						processingInstructionNode.IsStandAlone = value.ToLower() == "yes";
						break;

					}

				}

				this.stylesheet.processingInstructionNode = processingInstructionNode;

				break;

			}

		}

		public override void WriteRaw(string data)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{

			if (ns == string.Empty)
				ns = "http://www.w3.org/1999/XSL/Transform";

			// Save the name of the attribute
			this.attributePrefix = prefix;
			this.attributeLocalName = localName;
			this.attributeNamespaceURI = ns;

			if (this.attributeLocalName != "xmlns" && this.attributePrefix != "xmlns")
			{

				Driver driver = this.lexicon.GetDriver(this.attributeNamespaceURI, this.attributeLocalName);
				this.driverStack.Push(driver);

				Stylesheet.AttributeNode attributeNode = new Stylesheet.AttributeNode(driver.Token);
				this.nodeStack.Push(attributeNode);

			}
			else
			{

				Stylesheet.AttributeNode attributeNode = new Stylesheet.AttributeNode(Token.Namespace);
				this.nodeStack.Push(attributeNode);

			}


		}

		public override void WriteStartDocument(bool standalone)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartDocument()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{

			Driver driver = this.lexicon.GetDriver(ns, localName);

			this.driverStack.Push(driver);

			if (driver.ElementStartHandler != null)
				driver.ElementStartHandler();

		}

		public override WriteState WriteState
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void WriteString(string text)
		{

			this.nodeStack.Peek().Add(new Stylesheet.TextNode(text));
		
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteWhitespace(string ws)
		{
			throw new Exception("The method or operation is not implemented.");
		}

	}

}
