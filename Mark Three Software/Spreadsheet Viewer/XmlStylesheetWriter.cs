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
	public class XmlStylesheetWriter : System.Xml.XmlWriter
	{

		private MarkThree.Forms.Lexicon lexicon;
		private MarkThree.Forms.Stylesheet stylesheet;
		private System.Collections.Generic.Stack<Node> nodeStack;
		private System.Collections.Generic.Dictionary<Token, TokenHandler> tokenHandlerDictionary;

		/// <summary>
		/// Creates a writer to translate an incoming XSL stylesheet into a data structure.
		/// </summary>
		public XmlStylesheetWriter(Stylesheet stylesheet)
		{

			// Initialize the object;
			this.stylesheet = stylesheet;

			// This determines how to handle each of the incoming tokens parsed out of the XML stream.
			this.tokenHandlerDictionary = new Dictionary<Token, TokenHandler>();
			
			// This stack holds the current state of parsing the XSL language elements.  It is needed to build a tree of these
			// structures that is roughly the same as the incoming XSL document.  The stack structure is required because the
			// parsing can recursively dig down into the data structures, the unwind back up the tree before reading more child
			// elements.  The stack structure keeps track of how far down into the tree the parsing has gone.
			this.nodeStack = new Stack<Node>();
			this.nodeStack.Push(new RootNode());

			// This will create a lexical analyzer for the XSL Stylesheets and installs handlers for each of the tokens read from
			// the XML stream as it parses the stylesheet.
			this.lexicon = new Lexicon();
			this.tokenHandlerDictionary.Add(Token.Alignment, new TokenHandler(ParseAlignment));
			this.tokenHandlerDictionary.Add(Token.Animation, new TokenHandler(ParseAnimation));
			this.tokenHandlerDictionary.Add(Token.ApplyTemplate, new TokenHandler(ParseApplyTemplate));
			this.tokenHandlerDictionary.Add(Token.Attribute, new TokenHandler(ParseAttribute));
			this.tokenHandlerDictionary.Add(Token.Border, new TokenHandler(ParseBorder));
			this.tokenHandlerDictionary.Add(Token.Borders, new TokenHandler(ParseBorders));
			this.tokenHandlerDictionary.Add(Token.Cell, new TokenHandler(ParseCell));
			this.tokenHandlerDictionary.Add(Token.Choose, new TokenHandler(ParseChoose));
			this.tokenHandlerDictionary.Add(Token.Column, new TokenHandler(ParseColumn));
			this.tokenHandlerDictionary.Add(Token.Columns, new TokenHandler(ParseColumns));
			this.tokenHandlerDictionary.Add(Token.Constraint, new TokenHandler(ParseConstraint));
			this.tokenHandlerDictionary.Add(Token.Constraints, new TokenHandler(ParseConstraints));
			this.tokenHandlerDictionary.Add(Token.ColumnReference, new TokenHandler(ParseColumnReference));
			this.tokenHandlerDictionary.Add(Token.Delete, new TokenHandler(ParseDelete));
			this.tokenHandlerDictionary.Add(Token.View, new TokenHandler(ParseView));
			this.tokenHandlerDictionary.Add(Token.Font, new TokenHandler(ParseFont));
			this.tokenHandlerDictionary.Add(Token.Fragment, new TokenHandler(ParseFragment));
			this.tokenHandlerDictionary.Add(Token.If, new TokenHandler(ParseIf));
			this.tokenHandlerDictionary.Add(Token.Insert, new TokenHandler(ParseInsert));
			this.tokenHandlerDictionary.Add(Token.Interior, new TokenHandler(ParseInterior));
			this.tokenHandlerDictionary.Add(Token.NumberFormat, new TokenHandler(ParseNumberFormat));
			this.tokenHandlerDictionary.Add(Token.Otherwise, new TokenHandler(ParseOtherwise));
			this.tokenHandlerDictionary.Add(Token.Protection, new TokenHandler(ParseProtection));
			this.tokenHandlerDictionary.Add(Token.Row, new TokenHandler(ParseRow));
			this.tokenHandlerDictionary.Add(Token.Style, new TokenHandler(ParseStyle));
			this.tokenHandlerDictionary.Add(Token.Styles, new TokenHandler(ParseStyles));
			this.tokenHandlerDictionary.Add(Token.Stylesheet, new TokenHandler(ParseStylesheet));
			this.tokenHandlerDictionary.Add(Token.Table, new TokenHandler(ParseTable));
			this.tokenHandlerDictionary.Add(Token.Template, new TokenHandler(ParseTemplate));
			this.tokenHandlerDictionary.Add(Token.Update, new TokenHandler(ParseUpdate));
			this.tokenHandlerDictionary.Add(Token.ValueOf, new TokenHandler(ParseValueOf));
			this.tokenHandlerDictionary.Add(Token.Variable, new TokenHandler(ParseVariable));
			this.tokenHandlerDictionary.Add(Token.Sort, new TokenHandler(ParseSort));
			this.tokenHandlerDictionary.Add(Token.SortColumn, new TokenHandler(ParseSortColumn));
			this.tokenHandlerDictionary.Add(Token.When, new TokenHandler(ParseWhen));
			this.tokenHandlerDictionary.Add(Token.Document, new TokenHandler(ParseDocument));

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
		public Node ParseStylesheet()
		{

			// This is the root element of the Stylesheet data strucure.  Several global references are kept to provide access to
			// certain parts of the stylesheet datastructure.  For example, the list of Templates is used to add a section that
			// will match against incoming tokens to produce the XML output.  The list of Columns allows for a reordering of the
			// columns in the output document.  The list of Styles allows for modification of colors, borders, etc.
			return new Stylesheet.StylesheetNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Template Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseTemplate()
		{

			// Create a new Template Node and initialize it from the incoming XSL data.
			return new Stylesheet.TemplateNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Workbook Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseWhen()
		{

			// Create a new Workbook Node.
			return new Stylesheet.WhenNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Workbook Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseDocument()
		{

			// Create a new Workbook Node.
			return new Stylesheet.DocumentNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the List of Style Elements.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStyles()
		{

			// Create a new list of styles.
			return new Stylesheet.StylesNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for a Style.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStyle()
		{

			// Create a new Style Node and initialize it from the incoming XSL data.
			return new Stylesheet.StyleNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the the Alignment of objects inside a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseAlignment()
		{

			// Create a new Alignment.
			return new Stylesheet.AlignmentNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Borders that outline a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseBorders()
		{

			// Create a set of borders to contain the description of the outline of a cell.
			return new Stylesheet.BordersNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the a border that outlines a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseBorder()
		{

			// Create a borders and initialize it with the XSL data.
			return new Stylesheet.BorderNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Font used to write text inside a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseFont()
		{

			// Create a new Font Node and initialize it from the incoming XSL data.
			return new Stylesheet.FontNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the effects used to highlight a change of data in a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseAnimation()
		{

			// Create a new Animation Node and initialize it from the incoming XSL data.
			return new Stylesheet.AnimationNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the background of a cell.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseInterior()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			return new Stylesheet.InteriorNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the formatting instructions to turn public values into readable text.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseNumberFormat()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new Stylesheet.NumberFormatNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the an instruction to prevent a cell from being edited.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseOtherwise()
		{

			// Create a new Otherwise Node and initialize it from the incoming XSL data.
			return new Stylesheet.OtherwiseNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the an instruction to prevent a cell from being edited.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseProtection()
		{

			// Create a new Protection Node and initialize it from the incoming XSL data.
			return new Stylesheet.ProtectionNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of the screen specific style instructions.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseView()
		{

			// Create a new Display Node.
			return new Stylesheet.ViewNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Table Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseTable()
		{

			// Create a Table Node and initialize from the incoming XSL data.
			return new Stylesheet.TableNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a list of columns.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumns()
		{

			// Create a Columns Node.
			return new Stylesheet.ColumnsNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of a column.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseChoose()
		{

			// Create a Choose Node and initialize from the incoming XSL data.
			return new Stylesheet.ChooseNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of a column.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumn()
		{

			// Create a Column Node and initialize from the incoming XSL data.
			return new Stylesheet.ColumnNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the columns that describe a unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseConstraint()
		{

			// Create a Constraint Node and initialize from the incoming XSL data.
			return new Stylesheet.ConstraintNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the columns that describe a unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseConstraints()
		{

			// Create a Constraint Node and initialize from the incoming XSL data.
			return new Stylesheet.ConstraintsNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for a column that describes the unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumnReference()
		{

			// Create a ConstraintColumn Node and initialize from the incoming XSL data.
			return new Stylesheet.ColumnReferenceNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a description of how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseValueOf()
		{

			// Create a View Node and initialize from the incoming XSL data.
			return new Stylesheet.ValueOfNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a description of how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseVariable()
		{

			// Create a View Node and initialize from the incoming XSL data.
			return new Stylesheet.VariableNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the start of a description of how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseSort()
		{

			// Create a View Node and initialize from the incoming XSL data.
			return new Stylesheet.SortNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the columns that describe how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseSortColumn()
		{

			// Create a ViewColumn Node and initialize from the incoming XSL data.
			return new Stylesheet.SortColumnNode(this.stylesheet);

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the 'apply-template' Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseApplyTemplate()
		{

			// Create a ApplyTemplate Node.
			return new Stylesheet.ApplyTemplateNode();

		}

		public Node ParseAttribute()
		{

			return new Stylesheet.XslAttributeNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Fragment Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseFragment()
		{

			// Create a Fragment Node and initialize from the incoming XSL data.
			return new Stylesheet.FragmentNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the If Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseIf()
		{

			// Create a If Node and initialize from the incoming XSL data.
			return new Stylesheet.IfNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Insert Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseInsert()
		{

			// Create a Insert Node and initialize from the incoming XSL data.
			return new Stylesheet.InsertNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Update Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseUpdate()
		{

			// Create a Update Node and initialize from the incoming XSL data.
			return new Stylesheet.UpdateNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the Delete Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseDelete()
		{

			// Create a Delete Node and initialize from the incoming XSL data.
			return new Stylesheet.DeleteNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of a row of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseRow()
		{

			// Create a Row Node and initialize from the incoming XSL data.
			return new Stylesheet.RowNode();

		}

		/// <summary>
		/// Parses the XSL Stylesheet for the description of an individual cell of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseCell()
		{

			// Create a Cell Node and initialize from the incoming XSL data.
			return new Stylesheet.CellNode(this.stylesheet);

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

			this.nodeStack.Peek().Add(new CommentNode(text));

		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteEndAttribute()
		{

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteEndDocument()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteEndElement()
		{

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteEntityRef(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void WriteFullEndElement()
		{

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteProcessingInstruction(string name, string text)
		{

			// There is no information here that is preserved.

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

			// Save the name of the attribute
			if (localName != "xmlns" && prefix != "xmlns")
			{

				AttributeNode attributeNode = new AttributeNode(this.lexicon.TokenDictionary[ns][localName]);
				this.nodeStack.Push(attributeNode);

			}
			else
			{

				NamespaceNode namespaceNode = new NamespaceNode(prefix, localName, ns);
				this.nodeStack.Push(namespaceNode);

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

			this.nodeStack.Push(this.tokenHandlerDictionary[this.lexicon.TokenDictionary[ns][localName]]());

		}

		public override WriteState WriteState
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void WriteString(string text)
		{

			this.nodeStack.Peek().Add(new TextNode(text));
		
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
