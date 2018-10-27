namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Xml;

	/// <summary>
	/// Writes an XML file to a Spreadsheet object.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	/// <remarks>The purpose of this class is to parse an XML document stream and populate a Spreadsheet with the data and
	/// formatting collected from that stream.  Parsing an XML stream into a data structure can be done several way, but XML is
	/// essentially a hierarchical collection of data.  After trying several methods, this one seemed to be the best.  This method
	/// is slightly slower than some of the others, but involves the use of a stack to assemble the objects.  Nodes that resemble
	/// the XML elements and attributes are assembled on a stack and converted into Spreadsheet data and formatting as they are
	/// popped off the stack.</remarks>
	internal class XmlSpreadsheetWriter : XmlWriter
	{

		// When this many rows have been processed, the SpreadsheetWriter will release the locks and broadcast an intermediate
		// document.  The interruption is used to allow other processes, most notably the foreground thread, to access the data
		// structures in the spreadsheet.  If this value is set too low, it will take a long time to draw a large document.  If the
		// value is too large, then other threads, like the foreground thread, will be suspended while the document is read.
		private const int updateTrigger = 100;

		// Private Members
		private MarkThree.Forms.Lexicon lexicon;
		private MarkThree.Forms.Spreadsheet spreadsheet;
		private MarkThree.Forms.Style defaultStyle;
		private System.Collections.Generic.Stack<Node> nodeStack;
		private System.Collections.Generic.Dictionary<Token, TokenHandler> tokenHandlerDictionary;
		private System.Int32 rowCounter;

		/// <summary>
		/// Manages the writing of the alignment instructions for a cell.
		/// </summary>
		internal class AlignmentNode : Node
		{

			// Internal Members
			internal StringFormat StringFormat;

			/// <summary>
			/// Creates an object that will manage the writing of alignment specifications for a style.
			/// </summary>
			internal AlignmentNode() : base(Token.Alignment)
			{

				// Initialize the object.
				this.StringFormat = new StringFormat();

				// Initialize the alignment attributes from defaults.
				this.StringFormat.Alignment = DefaultSpreadsheet.Alignment;
				this.StringFormat.LineAlignment = DefaultSpreadsheet.LineAlignment;
				this.StringFormat.FormatFlags = DefaultSpreadsheet.FormatFlags;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Horizontal:

						// This will specify how the contents of a cell are aligned horizontally.
						this.StringFormat.Alignment = attributeNode.Value == "Left" ? StringAlignment.Near :
							attributeNode.Value == "Center" ? StringAlignment.Center :
							attributeNode.Value == "Right" ? StringAlignment.Far :
							DefaultSpreadsheet.Alignment;
						break;

					case Token.Vertical:

						// This will specify how the contents of a cell are aligned vertically.
						this.StringFormat.LineAlignment = attributeNode.Value == "Top" ? StringAlignment.Near :
							attributeNode.Value == "Center" ? StringAlignment.Center :
							attributeNode.Value == "Bottom" ? StringAlignment.Far : DefaultSpreadsheet.LineAlignment;
						break;

					case Token.ReadingOrder:

						// This determines the reading order of the information in the cell.
						if (attributeNode.Value == "RightToLeft")
							this.StringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages writing the specification for an Animation Sequence.
		/// </summary>
		private class AnimationNode : Node
		{

			// Internal Members
			internal int steps;
			internal Color startColor;
			internal Movement movement;

			/// <summary>
			/// Creates an object to manage the writing of the Animation Sequence.
			/// </summary>
			internal AnimationNode() : base(Token.Animation)
			{

				// Initialize the animation with the defaults.
				this.steps = DefaultSpreadsheet.Steps;
				this.startColor = DefaultSpreadsheet.StartColor;
				this.movement = Movement.Detached;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.StartColor:

						// This is the starting color used when a change in a cell is discovered.
						this.startColor = ColorTranslator.FromHtml(attributeNode.Value);
						break;

					case Token.Steps:

						// This is the number of steps that are used to fade the starting color into the normal color.  The more 
						// steps, the longer the animation sequence will take and the smoother the transition will appear.
						this.steps = Convert.ToInt32(attributeNode.Value);
						break;

					case Token.Direction:

						// This determines whether this sequence is used when the value increases, decreases or stays the same.
						this.movement = attributeNode.Value == string.Empty ? Movement.Detached :
							(Movement)Enum.Parse(typeof(Movement), attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of the specification of a border.
		/// </summary>
		private class BorderNode : Node
		{

			// Internal Members
			internal MarkThree.Forms.BorderPosition BorderPosition;
			internal System.Single Weight;
			internal System.Drawing.Color Color;

			/// <summary>
			/// Creates an object to manage the writing of the border specification to the style.
			/// </summary>
			internal BorderNode() : base(Token.Border)
			{

				// Set the defaults.
				this.BorderPosition = BorderPosition.Bottom;
				this.Weight = DefaultSpreadsheet.BorderWeight;
				this.Color = DefaultSpreadsheet.BorderColor;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Color:

						// This defines the color of the border.
						this.Color = ColorTranslator.FromHtml(attributeNode.Value);
						break;

					case Token.Position:

						// This determines whether the border appears at the top, left, bottom or right side of the cell.
						this.BorderPosition = (BorderPosition)Enum.Parse(typeof(BorderPosition), attributeNode.Value);
						break;

					case Token.Weight:

						// This determines how thick the border is.
						this.Weight = Convert.ToSingle(attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages a collection of BorderNode objects.
		/// </summary>
		private class BordersNode : Node
		{

			public BordersNode() : base(Token.Borders) { }

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is BorderNode)
				{

					// This is the only legal child node for a BordersNode object.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}
	
		}

		/// <summary>
		/// Manages the writing of a cell.
		/// </summary>
		private class CellNode : Node, IComparable
		{

			// Private Members
			private MarkThree.Forms.XmlSpreadsheetWriter spreadsheetWriter;
			private MarkThree.Forms.Spreadsheet spreadsheet;

			// Internal Members
			internal MarkThree.Forms.SpreadsheetColumn spreadsheetColumn;
			internal MarkThree.Forms.Style style;
			internal System.Object value;
			internal System.String dataType;

			/// <summary>
			/// Create an object to manage the writing of a cell.
			/// </summary>
			/// <param name="spreadsheet">The spreadsheet to which this cell belongs.</param>
			internal CellNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Cell)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;
				this.spreadsheetColumn = null;

				// Initialize the defaults.
				this.value = DefaultSpreadsheet.Value;
				this.style = this.spreadsheetWriter.defaultStyle;
				this.dataType = DefaultSpreadsheet.DataType;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.StyleId:

						// This sets the style for the node.
						this.style = this.spreadsheet.styleTable[attributeNode.Value];
						break;

					case Token.ColumnId:

						// This assigns the cell to a column in the table.  If no style has been specified for the new cell, then 
						// it will use the style assigned to the column.
						this.spreadsheetColumn = (SpreadsheetColumn)this.spreadsheet.Columns[attributeNode.Value];
						if (this.style == null)
							this.style = this.spreadsheetColumn.Style;
						break;

					case Token.Type:

						// The datatype defines what kind of conversion is needed when reading the text of the value.
						this.dataType = attributeNode.Value;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A TextNode contains the free form data contained in this cell.  The 'dataType' member specifies how the text is
				// converted to an internal value.
				if (node is TextNode)
				{

					// Provide a strong type for the generic node.
					TextNode textNode = node as TextNode;

					// Convert the incoming data into a native datatype.
					switch (this.dataType)
					{

					case "string":

						// String data
						this.value = textNode.text;
						break;

					case "boolean":

						// Boolean data
						this.value = Convert.ToBoolean(textNode.text);
						break;

					case "short":

						// Short Integer data
						this.value = Convert.ToInt16(textNode.text);
						break;

					case "int":

						// Integer data
						this.value = Convert.ToInt32(textNode.text);
						break;

					case "long":

						// Long Integer data
						this.value = Convert.ToInt64(textNode.text);
						break;

					case "decimal":

						// Decimal data
						this.value = Convert.ToDecimal(textNode.text);
						break;

					case "float":

						// Short floating point data.
						this.value = Convert.ToSingle(textNode.text);
						break;

					case "double":

						// Floating point data.
						this.value = Convert.ToDouble(textNode.text);
						break;

					case "dateTime":

						// Data/Time data
						this.value = Convert.ToDateTime(textNode.text);
						break;

					case "image":

						// Bitmap data
						MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(textNode.text));
						this.value = new Bitmap(memoryStream);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

			#region IComparable Members

			/// <summary>
			/// Compare this cell to another.
			/// </summary>
			/// <param name="obj">The target object for a comparison</param>
			/// <returns>This will compare a cell to another object.</returns>
			/// <remarks>The purpose of comparing CellNodes is to order them correctly so they can be found with a binary search.
			/// The name of the column associated with this cell is used to order them and find them in the list.</remarks>
			public int CompareTo(object obj)
			{

				// Cells are ordered in this list by the column names.
				if (obj is CellNode)
					return this.spreadsheetColumn.ColumnName.CompareTo(((CellNode)obj).spreadsheetColumn.ColumnName);

				// Cells are ordered in this list by the column names.
				if (obj is SpreadsheetColumn)
					return this.spreadsheetColumn.ColumnName.CompareTo(((SpreadsheetColumn)obj).ColumnName);

				// No other form of comparison to a cell is permitted.
				throw new Exception(string.Format("Can't compare object of type {0} to {1}", this.GetType(), obj.GetType()));

			}

			#endregion

		}

		/// <summary>
		/// A specification for a column of data in the spreadsheet table.
		/// </summary>
		private class ColumnNode : Node
		{

			// Private Members
			private XmlSpreadsheetWriter spreadsheetWriter;
			private Spreadsheet spreadsheet;

			// Internal Members
			internal SpreadsheetColumn spreadsheetColumn;

			/// <summary>
			/// Create an object to describe the attributes of a column of data.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages writing data and updates into the spreadsheet viewer.</param>
			internal ColumnNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Column)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;

				// Create a new column and set the default values for the attributes.
				this.spreadsheetColumn = new SpreadsheetColumn();
				this.spreadsheetColumn.Style = this.spreadsheetWriter.defaultStyle;

				// Set the default width of the displayed column.
				Rectangle rectangle = spreadsheetColumn.Rectangle;
				rectangle.Width = Convert.ToInt32(DefaultSpreadsheet.ColumnWidth);
				spreadsheetColumn.Rectangle = rectangle;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.ColumnId:

						// This sets the identity of the column.
						this.spreadsheetColumn.ColumnName = attributeNode.Value;
						break;

					case Token.Image:

						// This provides an image for the column heading.
						MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(attributeNode.Value));
						this.spreadsheetColumn.Image = new Bitmap(memoryStream);
						break;

					case Token.Description:

						// This provides a descriptive text that appears in the header.
						this.spreadsheetColumn.Description = attributeNode.Value;
						break;

					case Token.Width:

						// This is the width of the heading.
						Rectangle rectangle = spreadsheetColumn.Rectangle;
						rectangle.Width = Convert.ToInt32(Convert.ToSingle(attributeNode.Value));
						spreadsheetColumn.Rectangle = rectangle;
						break;

					case Token.StyleId:

						// This style will be used by the heading text and any cell that doesn't have an explicit style.
						Style style;
						if (!this.spreadsheet.styleTable.TryGetValue(attributeNode.Value, out style))
							throw new Exception(string.Format("Column {0} has a Style {1} that is not part of the stylesheet",
								spreadsheetColumn.ColumnName, attributeNode.Value));
						spreadsheetColumn.Style = style;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of a reference to an existing column in the table.
		/// </summary>
		private class ColumnReferenceNode : Node
		{

			// Internal Members
			internal string columnId;

			/// <summary>
			/// Initialize an object that manages the writing of a reference to an existing column in the table.
			/// </summary>
			internal ColumnReferenceNode() : base(Token.Column)
			{

				// Initialize the object.
				this.columnId = string.Empty;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.ColumnId:

						// This identifies the source column.
						this.columnId = attributeNode.Value;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// A collection of columns that describe the layout of data in the spreadsheet table.
		/// </summary>
		private class ColumnsNode : Node
		{

			public ColumnsNode() : base(Token.Columns) { }

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A ColumnNode is a complete specification of a column of data.
				if (node is ColumnNode)
				{

					// Allow the base class to add the column.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of the elements and attributes that make up a unique index on the table.
		/// </summary>
		private class ConstraintNode : Node
		{

			// Internal Members
			internal bool isPrimaryKey;
			internal bool isUnique;

			/// <summary>
			/// Initialie the object that manages writing a unique index on the table.
			/// </summary>
			internal ConstraintNode() : base(Token.Constraint)
			{

				// Initialize the object.
				this.isPrimaryKey = false;
				this.isUnique = false;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Unique:

						// This defines a unique constraint on the data in the table.
						this.isUnique = Convert.ToBoolean(attributeNode.Value);
						break;

					case Token.PrimaryKey:

						// This defines the primary index on the table.
						this.isPrimaryKey = Convert.ToBoolean(attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A ColumnReferenceNode specifies an existing column in the table.  One or more instances here define the columns
				// that make up the index.
				if (node is ColumnReferenceNode)
				{

					// Allow the base class to add the node.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of the elements and attributes that make up a unique index on the table.
		/// </summary>
		private class ConstraintsNode : Node
		{

			/// <summary>
			/// Initialie the object that manages writing a unique index on the table.
			/// </summary>
			internal ConstraintsNode() : base(Token.Constraints) { }

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A ColumnReferenceNode specifies an existing column in the table.  One or more instances here define the columns
				// that make up the index.
				if (node is ConstraintNode)
				{

					// Allow the base class to add the node.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Contains a specification of the rows to be deleted from a document in the viewer.
		/// </summary>
		private class DeleteNode : Node
		{

			// Private Members
			private XmlSpreadsheetWriter spreadsheetWriter;
			private Spreadsheet spreadsheet;
			private object[] searchKey;

			/// <summary>
			/// Create a handler for the rows that are to be deleted from the document in the viewer.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages updates to the spreadsheet table.</param>
			internal DeleteNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Delete)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;
				this.searchKey = new object[this.spreadsheet.PrimaryKey.Length];

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A RowNode contains a row of data intended for the viewer.
				if (node is RowNode)
				{

					// Provide a strong type for the generic node.
					RowNode rowNode = node as RowNode;

					// Find the row in the existing data table based on the primary key and delete it.
					int keyIndex = 0;
					foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.PrimaryKey)
						this.searchKey[keyIndex++] = rowNode[spreadsheetColumn].value;
					SpreadsheetRow spreadsheetRow = this.spreadsheet.Rows.Find(this.searchKey) as SpreadsheetRow;
					if (spreadsheetRow != null)
						spreadsheetRow.Delete();

					// Loading a large document can be time consuming.  This will draw the new data periodically and allow other
					// threads to run in order keep a large block of data from hanging up the user interface.
					this.spreadsheetWriter.GiveUpControl();

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the changes to the font specification of a style.
		/// </summary>
		private class FontNode : Node
		{

			// Internal Members
			internal System.Drawing.Color Color;
			internal System.Drawing.FontStyle FontStyle;
			internal System.String FontName;
			internal System.Single Size;

			/// <summary>
			/// Creates an object that manages the writing of the font specifications for a style.
			/// </summary>
			internal FontNode() : base(Token.Font)
			{

				// Initialize the objects from the defaults.
				this.Color = DefaultSpreadsheet.ForeColor;
				this.FontStyle = FontStyle.Regular;
				this.FontName = DefaultSpreadsheet.FontName;
				this.Size = DefaultSpreadsheet.FontSize;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				if (node is AnimationNode)
				{

					// Allow the base class to handle the addition of the animation nodes.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Color:

						// This will define the foreground color of the font.
						this.Color = ColorTranslator.FromHtml(attributeNode.Value);
						break;

					case Token.FontName:

						// This will define the font.
						this.FontName = attributeNode.Value;
						break;

					case Token.Size:

						// This defines the size of the font in points.
						this.Size = Convert.ToSingle(attributeNode.Value);
						break;

					case Token.Bold:

						// This will make the font bold.
						if (Convert.ToBoolean(attributeNode.Value))
							this.FontStyle |= FontStyle.Bold;
						else
							this.FontStyle &= ~FontStyle.Bold;
						break;

					case Token.Italic:

						// This will display the font in the italic style.
						if (Convert.ToBoolean(attributeNode.Value))
							this.FontStyle |= FontStyle.Italic;
						else
							this.FontStyle &= ~FontStyle.Italic;
						break;

					case Token.Underline:

						// This will display the font with underlines.
						if (Convert.ToBoolean(attributeNode.Value))
							this.FontStyle |= FontStyle.Underline;
						else
							this.FontStyle &= ~FontStyle.Underline;
						break;

					case Token.Strikeout:

						// This will display the font with underlines.
						if (Convert.ToBoolean(attributeNode.Value))
							this.FontStyle |= FontStyle.Strikeout;
						else
							this.FontStyle &= ~FontStyle.Underline;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Top level node in a partial spreadsheet document.
		/// </summary>
		private class FragmentNode : Node
		{

			// Private Members
			private XmlSpreadsheetWriter spreadsheetWriter;

			/// <summary>
			/// Create a node that is used to managed the changes to an existing document in the viewer.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages writing data and updates into the spreadsheet viewer.</param>
			internal FragmentNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Fragment)
			{

				// Initialize the object
				this.spreadsheetWriter = spreadsheetWriter;

				// Initialize the document for writing.
				this.spreadsheetWriter.OpenDocument();

			}

		}

		/// <summary>
		/// Inserts data into an existing view of a document.
		/// </summary>
		private class InsertNode : Node
		{

			// Private Members
			private XmlSpreadsheetWriter spreadsheetWriter;
			private Spreadsheet spreadsheet;

			/// <summary>
			/// Creates an instruction to insert data into an exiting view of a document.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages writing data and updates into the spreadsheet viewer.</param>
			internal InsertNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Insert)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A RowNode contains a row of data intended for the viewer.
				if (node is RowNode)
				{

					// Provide a strong type for the generic node.
					RowNode rowNode = node as RowNode;

					// Create, populate and add a new row of data for the spreadsheet table using the information collected in the
					// RowNode.  Note that the rows are given an index as they are added.  This is used to give the cells an
					// absolute location (rowIndex, columnIndex) in the cartesean layout of the data.
					SpreadsheetRow spreadsheetRow = this.spreadsheet.NewSpreadsheetRow();
					spreadsheetRow.rectangle.Height = rowNode.displayHeight;
					spreadsheetRow.RowIndex = this.spreadsheet.rowIndex++;
					foreach (CellNode cellNode in rowNode)
					{
						SpreadsheetCell spreadsheetCell = spreadsheetRow[cellNode.spreadsheetColumn];
						spreadsheetCell.Value = cellNode.value;
						spreadsheetCell.Style = cellNode.style;
					}
					spreadsheet.Rows.Add(spreadsheetRow);

					// Loading a large document can be time consuming.  This will draw the new data periodically and allow other
					// threads to run in order keep a large block of data from hanging up the user interface.
					this.spreadsheetWriter.GiveUpControl();

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Provides the specification for the interior portion of a cell.
		/// </summary>
		private class InteriorNode : Node
		{

			// Internal Members
			internal System.Drawing.Color color;

			/// <summary>
			/// Creates an object to manage the formatting for the interior portion of a cell.
			/// </summary>
			internal InteriorNode() : base(Token.Interior)
			{

				// Set up the defaults.
				this.color = DefaultSpreadsheet.BackColor;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Color:

						// This will set the background color of a cell.
						this.color = ColorTranslator.FromHtml(attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of the number format to the style.
		/// </summary>
		private class NumberFormatNode : Node
		{

			// Internal Members
			internal string format;

			/// <summary>
			/// Construct an object that will parse the number format for data from the XML stream.
			/// </summary>
			internal NumberFormatNode() : base(Token.NumberFormat)
			{

				// Initialize the number format from the defaults.
				this.format = DefaultSpreadsheet.Format;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Format:

						// This will become the formatting string used to present data of all different datatypes.
						this.format = attributeNode.Value;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the parsing of the Protection element.
		/// </summary>
		private class ProtectionNode : Node
		{

			// Internal Members
			internal bool isProtected;

			/// <summary>
			/// Construct an object that will manage the parsing of the Protection element from the XML stream.
			/// </summary>
			internal ProtectionNode() : base(Token.Protection)
			{

				// Initialize the object from the defaults.
				this.isProtected = DefaultSpreadsheet.IsProtected;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Protected:

						// This determines whether or not editing is allowed in a cell.
						this.isProtected = Convert.ToBoolean(attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages updates to a row of data in the table.
		/// </summary>
		private class RowNode : SortedElementList
		{

			// Internal Members
			internal int displayHeight;

			/// <summary>
			/// Creates an object used describe the attributes and contents of a row of data in the viewer.
			/// </summary>
			internal RowNode() : base(Token.Row)
			{

				// Set up the default values.
				this.displayHeight = Convert.ToInt32(DefaultSpreadsheet.RowHeight);

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.Height:

						// Set the height of the row on the display device.
						this.displayHeight = Convert.ToInt32(attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A CellNode contains the data for a column in the row.
				if (node is CellNode)
				{

					// Have the base class add the specification for the data to a list of cells.  When the row has been completely
					// assembled, these cell nodes will be read into the table.
					base.Add(node);
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

			/// <summary>
			/// Finds a CellNode given the column to which it refers.
			/// </summary>
			/// <param name="spreadsheetColumn">A column specification.</param>
			/// <returns>The CellNode that corresponds to the given SpreadsheetColumn, or null if there is no
			/// equivalent CellNode for the column.</returns>
			internal CellNode this[SpreadsheetColumn spreadsheetColumn]
			{

				get
				{

					// Find and return the CellNode that corresponds to the given column.
					return base.Find(spreadsheetColumn) as CellNode;

				}

			}

		}

		/// <summary>
		/// Specifies the sort order for the displayed document.
		/// </summary>
		private class SortNode : Node
		{

			public SortNode() : base(Token.Sort) { }

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A ColumnNode is a complete specification of a column of data.
				if (node is SortColumnNode)
				{

					// Allow the base class to add the column.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}
		
		}

		/// <summary>
		/// Part of the specification of the sort order for the displayed document.
		/// </summary>
		private class SortColumnNode : Node
		{

			// Internal Methods.
			internal string columnId;
			internal SortDirection sortDirection;

			/// <summary>
			/// Create an element used to specify the sort order of a displayed document.
			/// </summary>
			internal SortColumnNode() : base(Token.SortColumn)
			{

				// Initialize the object.
				this.columnId = string.Empty;
				this.sortDirection = SortDirection.NotSorted;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.ColumnId:

						// This specifies which column will be used for sorting.
						this.columnId = attributeNode.Value;
						break;

					case Token.Direction:

						// This determines whether the values in the column will increase or decrease as you read from the top of
						// the report to the bottom.
						this.sortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), attributeNode.Value, true);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

			}

		}

		/// <summary>
		/// Manages the writing of a style to the spreadsheet.
		/// </summary>
		private class StyleNode : Node
		{

			// Private Members
			private System.Collections.Generic.Dictionary<string, Style> styleTable;

			// Internal Members
			internal Style style;

			/// <summary>
			/// Creates an object to manage the writing of a style.
			/// </summary>
			/// <param name="styleTable"></param>
			internal StyleNode(Dictionary<string, Style> styleTable) : base(Token.Style)
			{

				// Initialize the object.
				this.style = new Style();
				this.styleTable = styleTable;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AlignmentNode defines how the contents of the cell is positioned within the cell.
				if (node is AlignmentNode)
				{

					// Provide a strong type for the generic node.
					AlignmentNode alignmentNode = node as AlignmentNode;

					// This will set the alignment in the style.
					this.style.StringFormat = alignmentNode.StringFormat;

					// There's no need to check other types once a node is handled.
					return;

				}

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.StyleId:

						// This defines the identifier for the style.
						this.style.Id = attributeNode.Value;
						break;

					case Token.Parent:

						// If a parent was specified, find the parent and, if it exists, copy the attributes of the parent to the
						// child.
						Style parentStyle;
						if (this.styleTable.TryGetValue(attributeNode.Value, out parentStyle))
							this.style.Parent = parentStyle;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A BordersNode contains a set of BorderNodes that describe what type of borders should be placed around a cell.
				if (node is BordersNode)
				{

					// The Top, Bottom, Right and Left borders are all described in seperate nodes.  This will copy the attributes
					// from the child nodes into the Style structure.
					foreach (Node childNode in node)
					{

						// Provide a strong type for the generic node.
						BorderNode borderNode = childNode as BorderNode;

						// Create a pen based on the color and weight parsed out of the XML stream.
						Pen pen = new Pen(new SolidBrush(borderNode.Color), borderNode.Weight);

						// This will describe the borders of all cells that have this style.
						switch (borderNode.BorderPosition)
						{

						case BorderPosition.Left:

							// Left Border
							this.style.LeftBorder = pen;
							break;

						case BorderPosition.Top:

							// Top Border
							this.style.TopBorder = pen;
							break;

						case BorderPosition.Right:

							// Right Border
							this.style.RightBorder = pen;
							break;

						case BorderPosition.Bottom:

							// Botton Border
							this.style.BottomBorder = pen;
							break;

						}

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// An InteriorNode describes the background of a cell.
				if (node is InteriorNode)
				{

					// Provide a strong type for the generic node.
					InteriorNode interiorNode = node as InteriorNode;

					// Create a brush for the interior area of the cells.
					Brush brush = new SolidBrush(interiorNode.color);
					Pen pen = new Pen(brush, 1.0f);

					// The interior pen and brush are used to paint the background of a cell.
					this.style.InteriorBrush = brush;
					this.style.InteriorPen = pen;

					// There's no need to check other types once a node is handled.
					return;

				}

				// The FontNode specifies the font used for the style.
				if (node is FontNode)
				{

					// Provide a strong type for the generic node.
					FontNode fontNode = node as FontNode;

					// The style will use this font and this color for the foreground.
					this.style.Font = new System.Drawing.Font(fontNode.FontName, fontNode.Size, fontNode.FontStyle);
					this.style.FontBrush = new SolidBrush(fontNode.Color);

					// A font can have an animation sequence defined which will change the colors according to a formula as a 
					// function of time.
					foreach (Node childNode in fontNode)
					{

						// Provide a strong type for the generic node.
						AnimationNode animationNode = childNode as AnimationNode;

						// This indicates that the style contains animation which needs to be analyzed when the data in cells that have this 
						// style are modified.
						this.style.IsAnimated = true;

						// The next step is to create an array of styles the cycle through a calculated color change from the 
						// start color to the base color of the Font.  When the cell changes value, these styles will be used to
						// make the color burst into the StartColor and gradually fade over time to the font color.  The number of
						// steps indicates how gradual and how long the fade effect will be.  If the animation period is 1 second,
						// and there are ten steps, the fade will take place using 10 different styles over a total of 10 seconds.
						Style[] styleArray = new Style[animationNode.steps];

						// Extract the color components of the start and end colors of this fade effect.
						Color endColor = fontNode.Color;
						decimal totalSteps = Convert.ToDecimal(animationNode.steps - 1);
						decimal startRed = animationNode.startColor.R;
						decimal startGreen = animationNode.startColor.G;
						decimal startBlue = animationNode.startColor.B;
						decimal endRed = endColor.R;
						decimal endGreen = endColor.G;
						decimal endBlue = endColor.B;

						// This will create a new style for each of the colors in the fade effect.  Animation is accomplished by 
						// changing the style on the cell to each one of these styles in sequence.
						for (int newStyleIndex = 0; newStyleIndex < animationNode.steps; newStyleIndex++)
						{

							// Create a new style and name it using a combination of the parent style name, the direction of the 
							// movement (up, down, any, none) and the step involved.
							styleArray[newStyleIndex] = new Style();
							styleArray[newStyleIndex].Id = string.Format("{0}{1}{2}", this.style.Id, animationNode.movement, newStyleIndex);
							styleArray[newStyleIndex].Parent = this.style;

							// The color used for the font is an involved calculation.  In round terms, the starting color is the color
							// specified in the 'Animation' element.  The ending color for the animated sequence is the color of the font.  The
							// steps are worked out so that the starting color morphs into the ending color.
							decimal index = Convert.ToDecimal(newStyleIndex);
							decimal red = startRed - ((startRed - endRed) / totalSteps) * index;
							decimal green = startGreen - ((startGreen - endGreen) / totalSteps) * index;
							decimal blue = startBlue - ((startBlue - endBlue) / totalSteps) * index;
							Color brushColor = Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
							styleArray[newStyleIndex].FontBrush = new SolidBrush(brushColor);

						}

						// This will assign the animation effect to the proper movement of the data in the cell.  Note that 
						// animation is only valid for a display device.
						switch (animationNode.movement)
						{
						case Movement.Nil:

							// Assign the animation for no movemement.
							this.style.NilAnimation = styleArray;
							break;

						case Movement.Up:

							// Assign the animation for increase in the cell's value.
							this.style.UpAnimation = styleArray;
							break;

						case Movement.Down:

							// Assign the animation for a decrease in the cell's value.
							this.style.DownAnimation = styleArray;
							break;

						}

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// The NubmerFormatNode specifies how data should be formatted when it is shown in the cell.
				if (node is NumberFormatNode)
				{

					// Provide a strong type for the generic node.
					NumberFormatNode numberFormatNode = node as NumberFormatNode;

					// Translate the number format into an internal form that can be used with the 'String.Format' method.  Note 
					// that the text 'General' is translated into the default format for a datatype.
					this.style.NumberFormat = numberFormatNode.format == "General" ? "{0}" : "{0:" + numberFormatNode.format + "}";

					// There's no need to check other types once a node is handled.
					return;

				}

				// A ProtectionNode specifies whether a cell allows editing or not.
				if (node is ProtectionNode)
				{

					// Provide a strong type for the generic node.
					ProtectionNode protectionNode = node as ProtectionNode;

					// This will assign the protection settings to the Display.
					this.style.IsProtected = protectionNode.isProtected;

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Handles a list of styles that describe how data is presented in the viewer.
		/// </summary>
		private class StylesNode : Node
		{

			// Private Members
			private Spreadsheet spreadsheet;
			private XmlSpreadsheetWriter spreadsheetWriter;

			/// <summary>
			/// Create an object to handle the list of styles.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages writing data and updates into the spreadsheet viewer.</param>
			internal StylesNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Styles)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A StyleNode has attributes that describe how data is presented in the viewer.
				if (node is StyleNode)
				{

					// Provide a strong type for the generic node.
					StyleNode styleNode = node as StyleNode;

					// An implicit "Default" style is created when the Workbook is initialized.  If an explicit "Default" style is
					// defined in the XML specification, then remove the original one so there isn't a conflict.
					if (styleNode.style.Id == DefaultSpreadsheet.StyleId)
					{
						this.spreadsheet.styleTable.Remove(DefaultSpreadsheet.StyleId);
						this.spreadsheetWriter.defaultStyle = styleNode.style;
					}

					// Don't allow multiple styles with the same identifier.
					if (this.spreadsheet.styleTable.ContainsKey(styleNode.style.Id))
						throw new Exception(string.Format("Style {0} already exists in the style table", styleNode.style.Id));

					// Add the style to the list of styles managed by the spreadsheet.
					this.spreadsheet.styleTable.Add(styleNode.style.Id, styleNode.style);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages updates to the data table.
		/// </summary>
		private class TableNode : Node
		{

			// Private Members
			private Spreadsheet spreadsheet;
			private XmlSpreadsheetWriter spreadsheetWriter;

			/// <summary>
			/// Create a handler for the specifications of a table of data.
			/// </summary>
			/// <param name="spreadsheetWriter">The writer that manages updates to the viewer.</param>
			internal TableNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Table)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = spreadsheetWriter.spreadsheet;

				// Set up the default properties.
				this.spreadsheet.headerRectangle.Height = Convert.ToInt32(DefaultSpreadsheet.HeaderHeight);
				this.spreadsheet.SelectionMode = DefaultSpreadsheet.SelectionMode;

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use 
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.HeaderHeight:

						// This will set the height of the column heading area.
						this.spreadsheet.headerRectangle.Height =
							Convert.ToInt32(Convert.ToSingle(attributeNode.Value));
						break;

					case Token.SelectionMode:

						// This wil set the selection mode (the mouse will either select an entire row, just a cell, or the selection
						// is turned off).
						this.spreadsheet.SelectionMode = (SelectionMode)Enum.Parse(typeof(SelectionMode), attributeNode.Value);
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A ColumnsNode contains the specification of the columns found in the table.
				if (node is ColumnsNode)
				{

					// Provide a strong type for the generic node.
					ColumnsNode columnsNode = node as ColumnsNode;

					// The column can be added to the spreadsheet once it's been initialized.
					int columnIndex = 0;
					foreach (ColumnNode columnNode in columnsNode)
					{
						columnNode.spreadsheetColumn.ColumnIndex = columnIndex++;
						this.spreadsheet.Columns.Add(columnNode.spreadsheetColumn);
					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A ConstraintNode specifies the primary and unique indices enforced on a table of data.
				if (node is ConstraintsNode)
				{

					ConstraintsNode constraintsNode = node as ConstraintsNode;

					foreach (ConstraintNode constraintNode in constraintsNode)
					{

						// This will construct a primary key for the table.
						if (constraintNode.isPrimaryKey)
						{

							// Create an array of columns from the specification in the ConstraintNode.  These columns specify a unique
							// index used to unambigously locate a row of data.
							int fieldIndex = 0;
							DataColumn[] dataRowColumns = new DataColumn[constraintNode.Count];
							foreach (ColumnReferenceNode constraintColumnNode in constraintNode)
								dataRowColumns[fieldIndex++] = this.spreadsheet.Columns[constraintColumnNode.columnId];
							this.spreadsheet.PrimaryKey = dataRowColumns;

						}

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// The DisiplayViewNode describes the order of the visible columns and rows in the viewer.  The set of columns and
				// rows can be larger than the set of visible columns and rows.  There may be intermediate values and numeric codes
				// that are not used directly, but determine what is displayed in the visible cells.  Also, there may be user
				// preferences that determine why one cell is visible and another hidden.  The visible rows and columns are
				// specified in a filter the same way that rows can be specified with a row filter.
				if (node is ViewNode)
				{

					// Provide a strong type for the generic node.
					ViewNode viewNode = node as ViewNode;

					// This will determine which rows are visible.
					this.spreadsheet.ViewRows.RowFilter = viewNode.rowFilter;

					// Create a filter for the columns based on the specification found in the DisplayViewNode.  The list of nodes
					// in the DisplayViewNode contains the visible columns in the specified order for the display.
					string columnFilter = string.Empty;
					for (int columnIndex = 0; columnIndex < viewNode.Count; columnIndex++)
					{
						ColumnReferenceNode columnReferenceNode = viewNode[columnIndex] as ColumnReferenceNode;
						columnFilter += columnReferenceNode.columnId;
						if (columnIndex < viewNode.Count - 1)
							columnFilter += ",";
					}
					this.spreadsheet.ViewColumns.ColumnFilter = columnFilter;

					// Measure the size of the header and calcualte the invalid region to be updated.  The header can sometimes be 
					// part of a very big document.  By taking a quick break here and drawing the header area, the user is given
					// some valuable feedback that something is happening and they can expect a beautiful new document in their
					// viewer soon.
					Region region = this.spreadsheet.MeasureHeader();
					this.spreadsheet.readerWriterLock.ReleaseWriterLock();
					this.spreadsheet.OnHeaderSizeChanged(this.spreadsheet.headerRectangle.Size);
					this.spreadsheet.OnHeaderInvalidated(region);
					this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

					// There's no need to check other types once a node is handled.
					return;

				}

				// A SortNode describes the order of the rows displayed in the viewer.
				if (node is SortNode)
				{

					// Provide a strong type for the generic node.
					SortNode sortNode = node as SortNode;

					// This will construct the spreadsheetRowView sort string from the ViewColumns.
					int columnIndex = 0;
					string sort = string.Empty;
					foreach (SortColumnNode viewColumnNode in sortNode)
						sort += string.Format(columnIndex++ < sortNode.Count - 1 ? "{0} {1}," : "{0} {1}",
							viewColumnNode.columnId, viewColumnNode.sortDirection == SortDirection.Descending ? "DESC" : "ASC");
					this.spreadsheet.spreadsheetRowView.Sort = sort;

					// There's no need to check other types once a node is handled.
					return;

				}

				// A RowNode contains a row of data intended for the viewer.
				if (node is RowNode)
				{

					// Provide a strong type for the generic node.
					RowNode rowNode = node as RowNode;

					// Create, populate and add a new row of data for the spreadsheet table using the information collected in the
					// RowNode.  Note that the rows are given an index as they are added.  The columns also are indexed.  This 
					// gives every cells a unique, absolute address as a coordinate pair of a row and column index.
					SpreadsheetRow spreadsheetRow = this.spreadsheet.NewSpreadsheetRow();
					spreadsheetRow.rectangle.Height = rowNode.displayHeight;
					spreadsheetRow.RowIndex = this.spreadsheet.rowIndex++;
					foreach (CellNode cellNode in rowNode)
					{
						SpreadsheetCell spreadsheetCell = spreadsheetRow[cellNode.spreadsheetColumn];
						spreadsheetCell.Value = cellNode.value;
						spreadsheetCell.Style = cellNode.style;
					}
					spreadsheet.Rows.Add(spreadsheetRow);

					// Loading a large document can be time consuming.  This will draw the new data periodically and allow other
					// threads to run in order keep a large block of data from hanging up the user interface.
					this.spreadsheetWriter.GiveUpControl();

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Handles an update to an existing document in the viewer.
		/// </summary>
		private class UpdateNode : Node
		{

			// Private Members
			private Spreadsheet spreadsheet;
			private XmlSpreadsheetWriter spreadsheetWriter;
			private object[] searchKey;

			/// <summary>
			/// Creates a node that manages the updates to an existing document in the viewer.
			/// </summary>
			/// <param name="spreadsheetWriter">The writer that manages updates to the viewer.</param>
			internal UpdateNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Update)
			{

				// Initialize the object.
				this.spreadsheetWriter = spreadsheetWriter;
				this.spreadsheet = this.spreadsheetWriter.spreadsheet;

				// The search key is used to find the record in the table of existing data.  Since the size doesn't change while
				// processing the update node, it is allocated here.
				this.searchKey = new object[this.spreadsheet.PrimaryKey.Length];

			}

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// A RowNode contains a row of data intended for the viewer.
				if (node is RowNode)
				{

					// Provide a strong type for the generic node.
					RowNode rowNode = node as RowNode;

					// It is possible in a multithreaded application to get a fragment before there is a document structure.  This 
					// code insures that there is a valid DataTable into which the data in the incoming row can be placed.
					if (this.spreadsheet.PrimaryKey.Length == 0)
						return;

					// Search the viewer database for the incoming row.  If it doesn't exist, it will be added.  If a row is found 
					// that matches the primary key, it will be compared against the existing row to see what has changed.
					int keyIndex = 0;
					foreach (SpreadsheetColumn spreadsheetColumn in this.spreadsheet.PrimaryKey)
						this.searchKey[keyIndex++] = rowNode[spreadsheetColumn].value;
					SpreadsheetRow spreadsheetRow = (SpreadsheetRow)this.spreadsheet.Rows.Find(this.searchKey);
					if (spreadsheetRow != null)
					{

						// This will inhibit triggers until the entire row is updated.
						spreadsheetRow.BeginEdit();

						// If the incoming row already has been added to the data, then compare the new row, column by column, against 
						// the old row to see what has changed.
						foreach (CellNode cellNode in rowNode)
						{

							// This is the current data and style in the cell.  The incoming data will replace this if it is different.
							SpreadsheetCell spreadsheetCell = spreadsheetRow[cellNode.spreadsheetColumn];
							object currentValue = spreadsheetCell.Value;
							Style currentStyle = spreadsheetCell.Style;

							// Compare this cell and its style against the incoming data.  If it is different, then the 
							// spreadsheet will be updated with the new data.
							if (!cellNode.value.Equals(currentValue) || cellNode.style != currentStyle)
							{

								// This will cause the cell to be updated when measured again.
								spreadsheetCell.IsModified = true;
								spreadsheetCell.Value = cellNode.value;
								spreadsheetCell.Style = cellNode.style;

								// This will dispose of any bitmap resources that occupied the cell before the new data replaces 
								// it.  Without this statement, system memory will be consumed and not released, eventually choking
								// the memory pool.
								if (currentValue is Bitmap)
									((Bitmap)currentValue).Dispose();

								// If this cel is animated, the initialize the sequence of styles that will make the data in the 
								// cells appear to change color over time.
								if (spreadsheetCell.Style.IsAnimated)
								{

									// If the current value in the cell can be compared to the original value, then we'll determine
									// if the new data is greater, less than or the same as the current value.  The direction will
									// determine which animatino sequence is used.
									if (currentValue is IComparable && spreadsheetCell.Value is IComparable)
									{

										// An array of styles is created to gradually change the color of the text in the cell from
										// the start color to the normal color for the cell.  This test will determine in which
										// direction the value in the cell has changed.
										int comparision = ((IComparable)currentValue).CompareTo(spreadsheetCell.Value);
										Movement movement = comparision > 0 ? Movement.Up : comparision < 0 ? Movement.Down :
											Movement.Nil;
										Style[] styleArray = null;
										switch (movement)
										{
										case Movement.Nil:

											// Animate the cell when there is no change in the value in the cell.
											styleArray = cellNode.style.NilAnimation;
											break;

										case Movement.Up:

											// Animate the cell when the value has increased.
											styleArray = cellNode.style.UpAnimation;
											break;

										case Movement.Down:

											// Animate the cell when the value has decreased.
											styleArray = cellNode.style.DownAnimation;
											break;

										}

										// This will initialize the animation squence in the cell once an array of styles has been chosen.
										if (styleArray != null)
										{

											// The sequence is primed by grabbing the first color (the start color) out of the
											// array of styles.  This color will immediately be show as soon as the XML is finished
											// reading.  The rest of the colors will be displayed as the animation thread gets
											// around to them.
											spreadsheetCell.AnimationIndex = 0;
											spreadsheetCell.AnimationArray = styleArray;
											spreadsheetCell.Style = styleArray[0];

											// This list keeps track of the animated cells for the animation thread.  When the 
											// animation sequence is complete, they are removed from this list.
											this.spreadsheet.animatedList.Add(spreadsheetCell);

										}

									}

								}

							}

						}

						// This will allow any triggers to fire again.
						spreadsheetRow.EndEdit();

						// Loading a large document can be time consuming.  This will draw the new data periodically and allow 
						// other threads to run in order keep a large block of data from hanging up the user interface.
						this.spreadsheetWriter.GiveUpControl();

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of a View to the spreadsheet.
		/// </summary>
		private class ViewNode : Node
		{

			public ViewNode() : base(Token.View) { }

			// Internal Members
			internal string rowFilter;

			/// <summary>
			/// Handles the addition of child elements and attributes.
			/// </summary>
			/// <param name="node">The child element or attribute.</param>
			public override void Add(Node node)
			{

				// An AttributeNode contain data that modify the properties of this node.
				if (node is AttributeNode)
				{

					// Provide a strong type for the generic node.
					AttributeNode attributeNode = node as AttributeNode;

					// Each attribute node has a token and value that is read from the stream.  This will take that pair and use
					// them to set the properties of this node.
					switch (attributeNode.token)
					{

					case Token.RowFilter:

						// The 'rowFilter' is a formula that determines which rows are visible in the viewer.
						this.rowFilter = attributeNode.Value;
						break;

					}

					// There's no need to check other types once a node is handled.
					return;

				}

				// A ColumnReferenceNode is a reference to an existing column in the table.
				if (node is ColumnReferenceNode)
				{

					// Allow the base class to add this node.
					base.Add(node);

					// There's no need to check other types once a node is handled.
					return;

				}

				// All other node types are illegal here.
				throw new Exception(string.Format("A node of type {0} is illegal in this context", node.GetType()));

			}

		}

		/// <summary>
		/// Manages the writing of the top level node in a complete spreadsheet document.
		/// </summary>
		private class DocumentNode : Node
		{

			/// <summary>
			/// Create an object that will manage the writing the start of a complete spreadsheet document.
			/// </summary>
			/// <param name="spreadsheetWriter">Manages writing data and updates into the spreadsheet viewer.</param>
			internal DocumentNode(XmlSpreadsheetWriter spreadsheetWriter) : base(Token.Document)
			{

				// Initialize the document for writing.
				spreadsheetWriter.OpenDocument();

				// This element acts as an instruction to reset the the entire Spreadsheet data structure.  This includes styles,
				// sizes, filters, indexing and data.
				spreadsheetWriter.spreadsheet.Reset();

				// A default style is always available to expedite the inititalization of columns and cells.
				spreadsheetWriter.defaultStyle = spreadsheetWriter.spreadsheet.styleTable[DefaultSpreadsheet.StyleId];

			}

		}

		/// <summary>
		/// Creates a writer that will populate a Spreadsheet object with data from an XML document stream.
		/// </summary>
		/// <param name="spreadsheet">The target spreadsheet.</param>
		public XmlSpreadsheetWriter(Spreadsheet spreadsheet)
		{

			// Initialize the object.
			this.spreadsheet = spreadsheet;

			// XML is a hierarchical data structure.  Most of the data items in the incoming XML document are elements nested in
			// other elements.  This stack of 'Nodes' allows for smaller bits of the incoming spreadsheet to be assembled on a
			// stack and passed on to larger spreadsheet objects.
			this.nodeStack = new Stack<Node>();
			this.nodeStack.Push(new RootNode());

			// This determines how to handle each of the incoming tokens parsed out of the XML stream.
			this.tokenHandlerDictionary = new Dictionary<Token, TokenHandler>();

			// This lexicon will change the incoming XSL element and attribute names into tokens.  These installed handlers will
			// call up a handler to deal with the incoming tokens as they are recognized by the analyzer.
			this.lexicon = new Lexicon();

			this.tokenHandlerDictionary.Add(Token.Alignment, new TokenHandler(ParseAlignment));
			this.tokenHandlerDictionary.Add(Token.Animation, new TokenHandler(ParseAnimation));
			this.tokenHandlerDictionary.Add(Token.Borders, new TokenHandler(ParseBorders));
			this.tokenHandlerDictionary.Add(Token.Border, new TokenHandler(ParseBorder));
			this.tokenHandlerDictionary.Add(Token.Cell, new TokenHandler(ParseCell));
			this.tokenHandlerDictionary.Add(Token.Column, new TokenHandler(ParseColumn));
			this.tokenHandlerDictionary.Add(Token.Columns, new TokenHandler(ParseColumns));
			this.tokenHandlerDictionary.Add(Token.Constraint, new TokenHandler(ParseConstraintNode));
			this.tokenHandlerDictionary.Add(Token.Constraints, new TokenHandler(ParseConstraintsNode));
			this.tokenHandlerDictionary.Add(Token.ColumnReference, new TokenHandler(ParseColumnReferenceNode));
			this.tokenHandlerDictionary.Add(Token.Delete, new TokenHandler(ParseDelete));
			this.tokenHandlerDictionary.Add(Token.View, new TokenHandler(ParseView));
			this.tokenHandlerDictionary.Add(Token.Font, new TokenHandler(ParseFont));
			this.tokenHandlerDictionary.Add(Token.Fragment, new TokenHandler(ParseFragment));
			this.tokenHandlerDictionary.Add(Token.Insert, new TokenHandler(ParseInsert));
			this.tokenHandlerDictionary.Add(Token.Interior, new TokenHandler(ParseInterior));
			this.tokenHandlerDictionary.Add(Token.NumberFormat, new TokenHandler(ParseNumberFormat));
			this.tokenHandlerDictionary.Add(Token.Protection, new TokenHandler(ParseProtection));
			this.tokenHandlerDictionary.Add(Token.Row, new TokenHandler(ParseRow));
			this.tokenHandlerDictionary.Add(Token.Style, new TokenHandler(ParseStyle));
			this.tokenHandlerDictionary.Add(Token.Styles, new TokenHandler(ParseStyles));
			this.tokenHandlerDictionary.Add(Token.Table, new TokenHandler(ParseTable));
			this.tokenHandlerDictionary.Add(Token.Update, new TokenHandler(ParseUpdate));
			this.tokenHandlerDictionary.Add(Token.Sort, new TokenHandler(ParseSort));
			this.tokenHandlerDictionary.Add(Token.SortColumn, new TokenHandler(ParseSortColumn));
			this.tokenHandlerDictionary.Add(Token.Document, new TokenHandler(ParseDocument));

		}

		/// <summary>
		/// Closes the stream opened for XML writing.
		/// </summary>
		public override void Close()
		{

			// There is no action required to close the spredsheet.

		}

		/// <summary>
		/// Flushes the data stream.
		/// </summary>
		public override void Flush()
		{

			// Measure the document and scan it for changed regions after all the data has been read.
			Region region = this.spreadsheet.MeasureData();

			// The owner of this object may will be interested in the size when the document has been changed.  The size is
			// broadcast out on an event.  However, once the write lock is released, the size can change, so directly accessing the
			// member variable 'this.displayRectangle' can lead to random changes in the size.  In order to broadcast the change in
			// a thread-safe way, a copy of the size is read here, then broadcast once the lock has been released.
			Size size = this.spreadsheet.displayRectangle.Size;

			// Allow other threads to access the data in the spreadsheet viewer.
			if (this.spreadsheet.readerWriterLock.IsWriterLockHeld)
				this.spreadsheet.readerWriterLock.ReleaseWriterLock();

			// Generate an event when the size has changed.
			this.spreadsheet.OnDataSizeChanged(size);

			// Broadcast the changed regions when the data has changed.
			this.spreadsheet.OnDataInvalidated(region);

		}

		/// <summary>
		/// Finds a prefix for a given namespace.
		/// </summary>
		/// <param name="ns">The URI of a namespace.</param>
		/// <returns>The prefix associated with that namespace.</returns>
		public override string LookupPrefix(string ns)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// This method allows partial results to be viewed while writing a large document.
		/// </summary>
		private void GiveUpControl()
		{

			// In order to improve the response of the application using this database when a large document is being parsed,
			// control is returned to the other threads periodically.  A command is also broadcast to update the viewer with the
			// partial document so the viewer can present something to the user quickly while the rest of the document is
			// constructed in this background thread.
			if (++this.rowCounter % updateTrigger == 0)
			{

				// Measure the document and get the new size.  These will be broadcast out to the viewer when the locks have been
				// removed.
				Region region = this.spreadsheet.MeasureData();
				Size size = this.spreadsheet.displayRectangle.Size;

				// This will release the locks on the table so it can be read by the viewer, then it will broadcast the changes,
				// allow the other threads, including the foreground message thread, to run for a little while.  When control is
				// returned to this thread, the construction of the document will continue.
				this.spreadsheet.readerWriterLock.ReleaseWriterLock();
				this.spreadsheet.OnDataSizeChanged(size);
				this.spreadsheet.OnDataInvalidated(region);
				Thread.Sleep(0);
				this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

			}

		}

		/// <summary>
		/// Begins an operation to update the data in the spreadsheet.
		/// </summary>
		private void OpenDocument()
		{

			// The locking and unlocking of this data structure needs to be handled internal to the 'Write' operation.  This is
			// because the data is tied directly to a display object and to make sure that the display object is never 'frozen' for
			// very long, the writing of data to this object will periodically release the lock and allow the foreground to update
			// the display.  This is how it is possible to write to very large objects without making the user think that the
			// machine has locked up.
			this.spreadsheet.readerWriterLock.AcquireWriterLock(Timeout.Infinite);

			// In order to keep the SpreadsheetWriter from consuming too much of the CPU time, control is surrendered to other
			// threads after reading a certain number of rows.  This counter keeps track of how many rows have been read from the
			// XML Document.
			this.rowCounter = 0;

		}

		/// <summary>
		/// Write a Base64 encoded string.
		/// </summary>
		/// <param name="buffer">A buffer to be written.</param>
		/// <param name="index">Starting index into the buffer.</param>
		/// <param name="count">The number of bytes to encode.</param>
		public override void WriteBase64(byte[] buffer, int index, int count)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Write a CDATA section.
		/// </summary>
		/// <param name="text">The content text.</param>
		public override void WriteCData(string text)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Write an Entity References.
		/// </summary>
		/// <param name="ch">The Character data.</param>
		public override void WriteCharEntity(char ch)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Write a raw buffer of characters.
		/// </summary>
		/// <param name="buffer">The buffer to be written.</param>
		/// <param name="index">The starting point in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		public override void WriteChars(char[] buffer, int index, int count)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Write a comment.
		/// </summary>
		/// <param name="text">The comment.</param>
		public override void WriteComment(string text)
		{

			// There is no action to be taken when writing a comment.

		}

		/// <summary>
		/// Write the document information section.
		/// </summary>
		/// <param name="name">The name of the document.</param>
		/// <param name="pubid">public identifier.</param>
		/// <param name="sysid">System identifier.</param>
		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Writes the end of an attribute.
		/// </summary>
		public override void WriteEndAttribute()
		{

			// Pop the node off the stack and add it to the parent node.
			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		/// <summary>
		/// Writes the end of the document.
		/// </summary>
		public override void WriteEndDocument()
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		/// <summary>
		/// Writes the end of an element.
		/// </summary>
		public override void WriteEndElement()
		{

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteEntityRef(string name)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteFullEndElement()
		{

			Node node = this.nodeStack.Pop();
			this.nodeStack.Peek().Add(node);

		}

		public override void WriteProcessingInstruction(string name, string text)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteRaw(string data)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{

			if (localName == "xmlns" || prefix == "xmlns")
			{

				NamespaceNode namespaceNode = new NamespaceNode(prefix, localName, ns);
				this.nodeStack.Push(namespaceNode);

			}
			else
			{

				Token token = this.lexicon.TokenDictionary[ns][localName];
				AttributeNode attributeNode = new AttributeNode(token);
				this.nodeStack.Push(attributeNode);

			}
			
		}

		public override void WriteStartDocument(bool standalone)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteStartDocument()
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{

			this.nodeStack.Push(this.tokenHandlerDictionary[this.lexicon.TokenDictionary[ns][localName]]());

		}

		public override WriteState WriteState
		{

			// This method isn't implemented for this version of the XML writer.
			get { throw new Exception("The method or operation is not implemented."); }

		}

		public override void WriteString(string text)
		{

			this.nodeStack.Peek().Add(new TextNode(text));

		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		public override void WriteWhitespace(string ws)
		{

			// This method isn't implemented for this version of the XML writer.
			throw new Exception("The method or operation is not implemented.");

		}

		private Node ParseAlignment()
		{

			return new AlignmentNode();

		}

		/// <summary>
		/// Handler for the start of the Animation element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseAnimation()
		{

			return new AnimationNode();

		}

		private Node ParseBorders()
		{

			return new BordersNode();

		}

		/// <summary>
		/// Handler for the start of the Border tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseBorder()
		{

			return new BorderNode();

		}

		/// <summary>
		/// Handler for the start of the Cell element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseCell()
		{

			return new CellNode(this);

		}

		/// <summary>
		/// Handler for the start of the Column element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseColumn()
		{

			return new ColumnNode(this);

		}

		private Node ParseColumns()
		{

			return new ColumnsNode();

		}

		/// <summary>
		/// Handler for the start of the Constraint element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseConstraintNode()
		{

			return new ConstraintNode();

		}

		/// <summary>
		/// Handler for the start of the Constraint element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseConstraintsNode()
		{

			return new ConstraintsNode();

		}

		/// <summary>
		/// Handler for the start of the ColumnReference element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseColumnReferenceNode()
		{

			return new ColumnReferenceNode();

		}

		/// <summary>
		/// Begin examining instructions for deleting a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private Node ParseDelete()
		{

			return new DeleteNode(this);

		}

		/// <summary>
		/// Handler for the Display tag.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private Node ParseView()
		{

			return new ViewNode();

		}

		private Node ParseFont()
		{

			return new FontNode();

		}

		/// <summary>
		/// Handler for the start of the Interior element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseInterior()
		{

			return new InteriorNode();

		}

		/// <summary>
		/// Handler for the start of the NumberFormat element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseNumberFormat()
		{

			return new NumberFormatNode();

		}

		/// <summary>
		/// Handler for the start of the Protection element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseProtection()
		{

			return new ProtectionNode();

		}

		/// <summary>
		/// Handler for the start of the Row element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseRow()
		{

			return new RowNode();

		}

		/// <summary>
		/// Handler for the start of the Style tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseStyle()
		{

			return new StyleNode(this.spreadsheet.styleTable);

		}

		/// <summary>
		/// Handler for the Styles tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseStyles()
		{

			return new StylesNode(this);

		}

		/// <summary>
		/// Handler for the start of the Table element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseTable()
		{

			return new TableNode(this);

		}

		/// <summary>
		/// Handler for the start of the ViewColumn element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseSortColumn()
		{

			return new SortColumnNode();

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private Node ParseFragment()
		{

			return new FragmentNode(this);

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private Node ParseInsert()
		{

			return new InsertNode(this);

		}

		/// <summary>
		/// Begin parsing an update to a row.
		/// </summary>
		/// <param name="xmlTokenReader"></param>
		private Node ParseUpdate()
		{

			return new UpdateNode(this);

		}

		/// <summary>
		/// Handler for the start of the View element.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseSort()
		{

			return new SortNode();

		}

		/// <summary>
		/// Handler for the Workbook tag.
		/// </summary>
		/// <param name="xmlTokenReader">The XML Document reader.</param>
		private Node ParseDocument()
		{

			return new DocumentNode(this);

		}

	}

}
