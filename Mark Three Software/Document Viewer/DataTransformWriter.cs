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
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class XmlDataTransformWriter : System.Xml.XmlWriter
	{

		private MarkThree.Forms.Lexicon lexicon;
		private MarkThree.Forms.DataTransform dataTransform;
		private System.Collections.Generic.Stack<Node> nodeStack;
		private System.Collections.Generic.Dictionary<Token, TokenHandler> tokenHandlerDictionary;

		/// <summary>
		/// Creates a writer to translate an incoming XSL stylesheet into a data structure.
		/// </summary>
		public XmlDataTransformWriter(DataTransform dataTransform)
		{

			// Initialize the object;
			this.dataTransform = dataTransform;

			// This determines how to handle each of the incoming tokens parsed out of the XML stream.
			this.tokenHandlerDictionary = new Dictionary<Token, TokenHandler>();
			
			// This stack holds the current state of parsing the XSL language elements.  It is needed to build a tree of these
			// structures that is roughly the same as the incoming XSL document.  The stack structure is required because the
			// parsing can recursively dig down into the data structures, the unwind back up the tree before reading more child
			// elements.  The stack structure keeps track of how far down into the tree the parsing has gone.
			this.nodeStack = new Stack<Node>();
			this.nodeStack.Push(new RootNode());

			// This will create a lexical analyzer for the XSL DataTransforms and installs handlers for each of the tokens read from
			// the XML stream as it parses the stylesheet.
			this.lexicon = new Lexicon();
			this.tokenHandlerDictionary.Add(Token.Animation, new TokenHandler(ParseAnimation));
			this.tokenHandlerDictionary.Add(Token.ApplyTemplate, new TokenHandler(ParseApplyTemplate));
			this.tokenHandlerDictionary.Add(Token.BottomBorder, new TokenHandler(ParseBottomBorder));
			this.tokenHandlerDictionary.Add(Token.LeftBorder, new TokenHandler(ParseLeftBorder));
			this.tokenHandlerDictionary.Add(Token.RightBorder, new TokenHandler(ParseRightBorder));
			this.tokenHandlerDictionary.Add(Token.TopBorder, new TokenHandler(ParseTopBorder));
			this.tokenHandlerDictionary.Add(Token.Tile, new TokenHandler(ParseTile));
			this.tokenHandlerDictionary.Add(Token.Column, new TokenHandler(ParseColumn));
			this.tokenHandlerDictionary.Add(Token.Columns, new TokenHandler(ParseColumns));
			this.tokenHandlerDictionary.Add(Token.ColumnReference, new TokenHandler(ParseColumnReference));
			this.tokenHandlerDictionary.Add(Token.Data, new TokenHandler(ParseData));
			this.tokenHandlerDictionary.Add(Token.DataTransform, new TokenHandler(ParseDataTransform));
			this.tokenHandlerDictionary.Add(Token.View, new TokenHandler(ParseView));
			this.tokenHandlerDictionary.Add(Token.Filter, new TokenHandler(ParseFilter));
			this.tokenHandlerDictionary.Add(Token.Font, new TokenHandler(ParseFont));
			this.tokenHandlerDictionary.Add(Token.FontBrush, new TokenHandler(ParseFontBrush));
			this.tokenHandlerDictionary.Add(Token.Image, new TokenHandler(ParseImage));
			this.tokenHandlerDictionary.Add(Token.InteriorBrush, new TokenHandler(ParseInteriorBrush));
			this.tokenHandlerDictionary.Add(Token.Locks, new TokenHandler(ParseLocks));
			this.tokenHandlerDictionary.Add(Token.Lock, new TokenHandler(ParseLock));
			this.tokenHandlerDictionary.Add(Token.NumberFormat, new TokenHandler(ParseNumberFormat));
			this.tokenHandlerDictionary.Add(Token.Protection, new TokenHandler(ParseProtection));
			this.tokenHandlerDictionary.Add(Token.Row, new TokenHandler(ParseRow));
			this.tokenHandlerDictionary.Add(Token.Style, new TokenHandler(ParseStyle));
			this.tokenHandlerDictionary.Add(Token.StyleId, new TokenHandler(ParseStyleId));
			this.tokenHandlerDictionary.Add(Token.Styles, new TokenHandler(ParseStyles));
			this.tokenHandlerDictionary.Add(Token.Template, new TokenHandler(ParseTemplate));
			this.tokenHandlerDictionary.Add(Token.Scale, new TokenHandler(ParseScale));
			this.tokenHandlerDictionary.Add(Token.Split, new TokenHandler(ParseSplit));
			this.tokenHandlerDictionary.Add(Token.Scratch, new TokenHandler(ParseScratch));
			this.tokenHandlerDictionary.Add(Token.StringFormat, new TokenHandler(ParseStringFormat));
			this.tokenHandlerDictionary.Add(Token.Sort, new TokenHandler(ParseSort));
			this.tokenHandlerDictionary.Add(Token.SortColumn, new TokenHandler(ParseSortColumn));
			this.tokenHandlerDictionary.Add(Token.Source, new TokenHandler(ParseSource));
			this.tokenHandlerDictionary.Add(Token.Variable, new TokenHandler(ParseVariable));

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
		/// Parses the XSL DataTransform for the DataTransform Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseDataTransform()
		{

			// This is the root element of the DataTransform data strucure.  Several global references are kept to provide access to
			// certain parts of the stylesheet datastructure.  For example, the list of Templates is used to add a section that
			// will match against incoming tokens to produce the XML output.  The list of Columns allows for a reordering of the
			// columns in the output document.  The list of Styles allows for modification of colors, borders, etc.
			return this.dataTransform;

		}

		/// <summary>
		/// Parses the XSL DataTransform for the Template Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseTemplate()
		{

			// Create a new Template Node and initialize it from the incoming XSL data.
			return new DataTransform.TemplateNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the List of Style Elements.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStyles()
		{

			// Create a new list of styles.
			return new DataTransform.StylesNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for a Style.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStyle()
		{

			// Create a new Style Node and initialize it from the incoming XSL data.
			return new DataTransform.StyleNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for a Style.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStyleId()
		{

			// Create a new Style Node and initialize it from the incoming XSL data.
			return new DataTransform.StyleIdNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the the Filter of objects inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseFilter()
		{

			// Create a new Filter.
			return new DataTransform.FilterNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the the Source of objects inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseSource()
		{

			// Create a new Source.
			return new DataTransform.SourceNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the the Source of objects inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseVariable()
		{

			// Create a new Source.
			return new DataTransform.VariableNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the the ApplyTemplate of objects inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseApplyTemplate()
		{

			// Create a new ApplyTemplate.
			return new DataTransform.ApplyTemplateNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the a border that outlines a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseBottomBorder()
		{

			// Create a borders and initialize it with the XSL data.
			return new DataTransform.BottomBorderNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the a border that outlines a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseLeftBorder()
		{

			// Create a borders and initialize it with the XSL data.
			return new DataTransform.LeftBorderNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the a border that outlines a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseRightBorder()
		{

			// Create a borders and initialize it with the XSL data.
			return new DataTransform.RightBorderNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the a border that outlines a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseTopBorder()
		{

			// Create a borders and initialize it with the XSL data.
			return new DataTransform.TopBorderNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the Font used to write text inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseFont()
		{

			// Create a new Font Node and initialize it from the incoming XSL data.
			return new DataTransform.FontNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the Font used to write text inside a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseFontBrush()
		{

			// Create a new Font Node and initialize it from the incoming XSL data.
			return new DataTransform.FontBrushNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the effects used to highlight a change of data in a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseAnimation()
		{

			// Create a new Animation Node and initialize it from the incoming XSL data.
			return new DataTransform.AnimationNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the background of a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseImage()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			return new DataTransform.ImageNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the background of a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseInteriorBrush()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			return new DataTransform.InteriorBrushNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the background of a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseLocks()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			return new DataTransform.LocksNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the background of a tile.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseLock()
		{

			// Create a new Interior Node and initialize it from the incoming XSL data.
			return new DataTransform.LockNode();

		}

		/// <summary>
		/// Parse the instruction to split the screen into quadrants.
		/// </summary>
		public Node ParseSplit()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new DataTransform.SplitNode();

		}

		/// <summary>
		/// Parse the instruction to split the screen into quadrants.
		/// </summary>
		public Node ParseScale()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new DataTransform.ScaleNode();

		}
	
		/// <summary>
		/// Parses the XSL DataTransform for the formatting instructions to turn public values into readable text.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseScratch()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new DataTransform.ScratchNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the formatting instructions to turn public values into readable text.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseStringFormat()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new DataTransform.StringFormatNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the formatting instructions to turn public values into readable text.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseNumberFormat()
		{

			// Create a new NumberFormat Node and initialize it from the incoming XSL data.
			return new DataTransform.NumberFormatNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the an instruction to prevent a tile from being edited.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseProtection()
		{

			// Create a new Protection Node and initialize it from the incoming XSL data.
			return new DataTransform.ProtectionNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the start of the screen specific style instructions.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseView()
		{

			// Create a new Display Node.
			return new DataTransform.ViewNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the start of a list of columns.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumns()
		{

			// Create a Columns Node.
			return new DataTransform.ColumnsNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the description of a column.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumn()
		{

			// Create a Column Node and initialize from the incoming XSL data.
			return new DataTransform.ColumnNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for a column that describes the unique index on the displayed data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseColumnReference()
		{

			// Create a ConstraintColumn Node and initialize from the incoming XSL data.
			return new DataTransform.ColumnReferenceNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the start of a description of how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseSort()
		{

			// Create a View Node and initialize from the incoming XSL data.
			return new DataTransform.SortNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the columns that describe how the displayed data is sorted.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseSortColumn()
		{

			// Create a ViewColumn Node and initialize from the incoming XSL data.
			return new DataTransform.SortNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the Data Element.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseData()
		{

			// Create a Data Node and initialize from the incoming XSL data.
			return new DataTransform.DataNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the description of a row of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseRow()
		{

			// Create a Row Node and initialize from the incoming XSL data.
			return new DataTransform.RowNode();

		}

		/// <summary>
		/// Parses the XSL DataTransform for the description of an individual tile of data.
		/// </summary>
		/// <param name="xmlTokenReader">Represents a reader that provides fast, non-cached, forward only access to XML Data.</param>
		public Node ParseTile()
		{

			// Create a Tile Node and initialize from the incoming XSL data.
			return new DataTransform.TileNode();

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
