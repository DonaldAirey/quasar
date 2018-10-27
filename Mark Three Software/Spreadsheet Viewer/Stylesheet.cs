namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Threading;

	/// <summary>
	/// A Stylesheet describes how to transform an XML Document into viewable data.
	/// </summary>
	/// <remarks>
	/// The stylesheet is used to transform XML Data into some human readable output.  This class makes it possible to manipulate
	/// elements of the stylesheet in order to incorporate user preferences in the transformation.  For example, sort orders,
	/// filtering, color preferences and the like can be stored in a flexible format along with the rules for transformation.
	/// </remarks>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Stylesheet : System.ComponentModel.Component
	{

		// Private Members
		private MarkThree.Forms.Stylesheet.TableNode tableNode;
		private MarkThree.Forms.Stylesheet.StylesNode stylesNode;
		private MarkThree.Forms.Stylesheet.StylesheetNode stylesheetNode;
		private System.Boolean hasChangeEvent;
		private System.Delegate[] inhibitedChangedDelegates;
		private System.Threading.ReaderWriterLock readerWriterLock;

		// Public Events
		public event EventHandler Changed;

		/// <summary>
		/// The root element of a stylesheet.
		/// </summary>
		internal class StylesheetNode : ElementNode
		{

			// Private Members
			private MarkThree.DualList<string, TemplateNode> templateList;
			private MarkThree.Forms.AttributeNode versionNode;
			private MarkThree.Forms.Stylesheet stylesheet;

			/// <summary>
			/// This object is the root of the hierarchical data structure that forms the Stylesheet.
			/// </summary>
			public StylesheetNode(Stylesheet stylesheet) : base(Token.Stylesheet)
			{

				// Initialize the object.
				this.stylesheet = stylesheet;
				this.stylesheet.stylesheetNode = this;
				this.templateList = new DualList<string, TemplateNode>();

			}

			public float Version
			{

				get { return Convert.ToSingle(this.versionNode.Value); }
				set {this.versionNode.Value = Convert.ToString(value); }

			}

			public DualList<string, TemplateNode> Templates { get { return this.templateList; } }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{
					case Token.version:

						this.versionNode = node as AttributeNode;
						break;

					}

				}

				if (node is TemplateNode)
				{
					TemplateNode templateNode = node as TemplateNode;
					this.templateList.Add(templateNode.Match, templateNode);
				}

				base.Add(node);

			}

		}

		/// <summary>
		/// Matches data in the incoming XML file to produce output.
		/// </summary>
		/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		public class TemplateNode : ElementNode, IComparable
		{

			/// <summary>
			/// Used to match the names of incoming elements to produce output.
			/// </summary>
			private MarkThree.Forms.AttributeNode matchNode;
			private MarkThree.Forms.Stylesheet.RowNode rowNode;

			/// <summary>
			/// Creates a Template.
			/// </summary>
			public TemplateNode() : base(Token.Template) {}

			public RowNode Row { get { return this.rowNode; } }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.match:

						this.matchNode = attributeNode;
						break;

					}

				}

				if (node is RowNode)
					this.rowNode = node as RowNode;

				base.Add(node);

			}

			public string Match
			{

				get { return this.matchNode.Value; }
				set { this.matchNode.Value = value; }
			}

			#region IComparable Members

			/// <summary>
			/// Compars this instance with the specified System.Object object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>Less than zero, this instance is less than obj.  zero, this instance is equal to obj, greater than zero,
			/// this instance is greater than obj.</returns>
			public int CompareTo(object obj)
			{

				// Compare this object aginst a template.
				if (obj is TemplateNode)
					return this.Match.CompareTo(((TemplateNode)obj).Match);

				// Compare this object against a string.
				if (obj is string)
					return this.Match.CompareTo((string)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(),
					obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A Workbook is a collection of spreadsheets.
		/// </summary>
		/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		internal class DocumentNode : ElementNode
		{

			// Private Members
			private Stylesheet stylesheet;

			/// <summary>
			/// Create a Workbook node from the input XML.
			/// </summary>
			public DocumentNode(Stylesheet stylesheet)
				: base(Token.Document)
			{

				// Initialize the object.
				this.stylesheet = stylesheet;

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{
					case Token.Namespace:

						break;

					default:

						base.Add(attributeNode);
						break;

					}

					return;

				}

				// This is how the Spreadsheet provides a programatic interface for the list of styles.  The data structure is
				// maintained as a tree structure for organizing parent/child relations and specifying default and override-able
				// attributes, but these structures are too complicated for most access.  The lists are available to an outside 
				// program directly through these members.
				if (node is StylesNode)
					this.stylesheet.stylesNode = (StylesNode)node;

				// This is how the Spreadsheet provides a programatic interface for the list of styles.  The data structure is
				// maintained as a tree structure for organizing parent/child relations and specifying default and override-able
				// attributes, but these structures are too complicated for most access.  The lists are available to an outside 
				// program directly through these members.
				if (node is TableNode)
					this.stylesheet.tableNode = node as TableNode;

				base.Add(node);

			}

		}

		/// <summary>
		/// Describes how the contents of a cell should be aligned within the cell.
		/// </summary>
		/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		internal class AlignmentNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode horizontalNode;
			private MarkThree.Forms.AttributeNode verticalNode;
			private MarkThree.Forms.AttributeNode readingOrderNode;

			/// <summary>
			/// Initializes an Alignment with default values.
			/// </summary>
			public AlignmentNode() : base(Token.Alignment) { }

			public System.Drawing.StringFormat StringFormat
			{

				get
				{

					StringFormat stringFormat = new System.Drawing.StringFormat();
					stringFormat.Alignment = DefaultSpreadsheet.Alignment;
					stringFormat.LineAlignment = DefaultSpreadsheet.LineAlignment;
					stringFormat.FormatFlags = DefaultSpreadsheet.FormatFlags;

					if (this.horizontalNode != null)
						switch (this.horizontalNode.Value)
						{

						case "Left":

							stringFormat.Alignment = StringAlignment.Near;
							break;

						case "Center":

							stringFormat.Alignment = StringAlignment.Center;
							break;

						case "Right":

							stringFormat.Alignment = StringAlignment.Far;
							break;

						}

					if (this.verticalNode != null)
						switch (this.verticalNode.Value)
						{

						case "Top":

							stringFormat.LineAlignment = StringAlignment.Near;
							break;

						case "Center":

							stringFormat.LineAlignment = StringAlignment.Center;
							break;

						case "Bottom":

							stringFormat.LineAlignment = StringAlignment.Far;
							break;

						}

					if (this.readingOrderNode != null)
						switch (this.readingOrderNode.Value)
						{

						case "Context":

							stringFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
							break;

						case "RightToLeft":

							stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
							break;

						}

					return stringFormat;

				}

				set
				{

					if (value.Alignment == DefaultSpreadsheet.Alignment)
					{
						if (this.horizontalNode != null)
							Remove(this.horizontalNode);
					}
					else
					{

						if (this.horizontalNode == null)
							Add(new AttributeNode(Token.Horizontal));

						this.horizontalNode.Value = value.Alignment == StringAlignment.Center ? "Center" :
							value.Alignment == StringAlignment.Far ? "Right" : "Left";

					}

					if (value.LineAlignment == DefaultSpreadsheet.LineAlignment)
					{

						if (this.verticalNode != null)
							Remove(this.verticalNode);

					}
					else
					{

						if (this.verticalNode == null)
							Add(new AttributeNode(Token.Vertical));

						this.verticalNode.Value = value.LineAlignment == StringAlignment.Center ? "Center" :
							value.LineAlignment == StringAlignment.Far ? "Bottom" : "Top";

					}

					if ((value.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0)
					{
						if (this.readingOrderNode != null)
							Add(new AttributeNode(Token.ReadingOrder));

						this.readingOrderNode.Value = "RightToLeft";

					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;

					switch (attributeNode.token)
					{
					case Token.Horizontal:

						this.horizontalNode = attributeNode;
						break;

					case Token.Vertical:

						this.verticalNode = attributeNode;
						break;

					case Token.ReadingOrder:

						this.readingOrderNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;

					switch (attributeNode.token)
					{
					case Token.Horizontal:

						this.horizontalNode = null;
						break;

					case Token.Vertical:

						this.verticalNode = null;
						break;

					case Token.ReadingOrder:

						this.readingOrderNode = null;
						break;

					}

				}

				base.Remove(node);

			}

		}

		/// <summary>
		/// A Border describes how a cell is outlined.
		/// </summary>
		/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		public class BorderNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.AttributeNode borderPositionNode;
			private MarkThree.Forms.AttributeNode colorNode;
			private MarkThree.Forms.AttributeNode weightNode;

			/// <summary>
			/// Initialize a border.
			/// </summary>
			public BorderNode() : base(Token.Border) { }

			/// <summary>
			/// Initialize a border.
			/// </summary>
			/// <param name="color">The color of the border.</param>
			/// <param name="weight">The thickness of the border.</param>
			public BorderNode(Color color, float weight) : base(Token.Border)
			{

				// Initialize the object.
				this.BorderPosition = BorderPosition.Bottom;
				this.Weight = weight;
				this.Color = color;

			}

			public MarkThree.Forms.BorderPosition BorderPosition
			{

				get
				{
					return this.borderPositionNode == null ? BorderPosition.Bottom :
				   (BorderPosition)Enum.Parse(typeof(BorderPosition), this.borderPositionNode.Value);
				}

				set
				{

					if (this.borderPositionNode == null)
						Add(new AttributeNode(Token.Position));
					this.borderPositionNode.Value = Convert.ToString(value);

				}

			}

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultSpreadsheet.BorderColor : ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (value == DefaultSpreadsheet.BorderColor)
						Remove(this.colorNode);
					else
					{
						if (this.colorNode == null)
							Add(new AttributeNode(Token.Color));
						this.colorNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Single Weight
			{

				get
				{
					return this.weightNode == null ? DefaultSpreadsheet.BorderWeight : Convert.ToSingle(this.weightNode.Value);
				}

				set
				{

					if (value == DefaultSpreadsheet.BorderWeight)
					{
						if (this.weightNode != null)
							Remove(this.weightNode);
					}
					else
					{
						if (this.weightNode == null)
							Add(new AttributeNode(Token.Weight));
						this.weightNode.Value = Convert.ToString(value);
					}

				}

			}
			
			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.Position:

						this.borderPositionNode = node as AttributeNode;
						break;

					case Token.Weight:

						this.weightNode = node as AttributeNode;
						break;

					case Token.Color:

						this.colorNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

			#region IComparable Members

			/// <summary>
			/// Compars this instance with the specified System.Object object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>Less than zero, this instance is less than obj.  zero, this instance is equal to obj, greater than zero,
			/// this instance is greater than obj.</returns>
			public int CompareTo(object obj)
			{

				// Compare this object aginst a Border Node.
				if (obj is BorderNode)
					return this.BorderPosition.CompareTo(((BorderNode)obj).BorderPosition);

				// Compare this object against the Border Position (the primary key on the table).
				if (obj is BorderPosition)
					return this.BorderPosition.CompareTo((BorderPosition)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(),
					obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A List of Borders.
		/// </summary>
		/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		internal class BorderList : SortedNodeList
		{

			/// <summary>
			/// Finds a Style Node in the sorted list using a unique key.
			/// </summary>
			/// <param name="key">The unique key that is used to identify the Node.</param>
			/// <returns>The Node matching the unique key, null if there are no matching Nodes.</returns>
			public BorderNode Find(BorderPosition borderPosition) {return (BorderNode)base.Find(borderPosition);}

			/// <summary>
			/// Returns the Border at the specified index in the arrray.
			/// </summary>
			public BorderNode this[BorderPosition borderPosition] {get {return (BorderNode)base.Find(borderPosition);}}

		}

		/// <summary>
		/// Borders describe the outline around a cell.
		/// </summary>
		internal class BordersNode : ElementNode
		{

			/// <summary>
			/// A list of borders.
			/// </summary>
			public BorderList BorderList;

			/// <summary>
			/// Initializes a list of borders.
			/// </summary>
			public BordersNode() : base(Token.Borders)
			{

				// Initialize the object.
				this.BorderList = new BorderList();

			}

			public override void Add(Node node)
			{

				if (node is BorderNode)
					this.BorderList.Add(node);

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is BorderNode)
					this.BorderList.Remove(node);

				base.Remove(node);

			}

		}

		/// <summary>
		/// Describes how a cell changes colors when new values are entered.
		/// </summary>
		public class AnimationNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.AttributeNode movementNode;
			private MarkThree.Forms.AttributeNode startColorNode;
			private MarkThree.Forms.AttributeNode stepsNode;

			/// <summary>
			/// Initialize information used to animate a cell.
			/// </summary>
			public AnimationNode() : base(Token.Animation)
			{

				// Initialize the object.
				this.movementNode = null;
				this.startColorNode = null;
				this.stepsNode = null;

			}

			/// <summary>
			/// Initialize information used to animate a cell.
			/// </summary>
			public AnimationNode(Color startColor, int steps) : base(Token.Animation)
			{

				// Initialize the object.
				this.movementNode = null;
				this.startColorNode = null;
				this.stepsNode = null;
				this.StartColor = startColor;
				this.Steps = steps;

			}

			public MarkThree.Movement Movement
			{

				get
				{
					return this.movementNode == null ? Movement.Detached :
						(Movement)Enum.Parse(typeof(Movement), this.movementNode.Value);
				}

				set
				{

					if (this.movementNode != null)
						Remove(this.movementNode);

					if (value != Movement.Detached)
						Add(new AttributeNode(Token.Direction, value.ToString()));

				}

			}

			public System.Drawing.Color StartColor
			{

				get
				{
					return this.startColorNode == null ? DefaultSpreadsheet.BorderColor : ColorTranslator.FromHtml(this.startColorNode.Value);
				}

				set
				{

					if (this.startColorNode != null)
						Remove(this.startColorNode);

					if (value != DefaultSpreadsheet.StartColor)
						Add(new AttributeNode(Token.Color, ColorTranslator.ToHtml(value)));

				}

			}

			public System.Int32 Steps
			{

				get
				{
					return this.stepsNode == null ? DefaultSpreadsheet.Steps : Convert.ToInt32(this.stepsNode.Value);
				}

				set
				{

					if (this.stepsNode != null)
						Remove(this.stepsNode);

					if (value != DefaultSpreadsheet.Steps)
						Add(new AttributeNode(Token.Steps, Convert.ToString(value)));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Direction:

						this.movementNode = attributeNode;
						break;

					case Token.StartColor:

						this.startColorNode = attributeNode;
						break;

					case Token.Steps:

						this.stepsNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

			#region IComparable Members

			public int CompareTo(object obj)
			{

				if (obj is AnimationNode)
					return this.Movement.CompareTo(((AnimationNode)obj).Movement);

				if (obj is Movement)
					return this.Movement.CompareTo((Movement)obj);

				return 0;

			}

			#endregion

		}
	
		/// <summary>
		/// A List of instructions for how to animate a cell when the values change.
		/// </summary>
		internal class AnimationList : SortedNodeList
		{

			/// <summary>
			/// Finds a Style Node in the sorted list using a unique key.
			/// </summary>
			/// <param name="key">The unique key that is used to identify the Node.</param>
			/// <returns>The Node matching the unique key, null if there are no matching Nodes.</returns>
			public AnimationNode Find(Movement movement) {return (AnimationNode)base.Find(movement);}

			/// <summary>
			/// Returns the Animation at the specified index in the arrray.
			/// </summary>
			public AnimationNode this[Movement movement] {get {return (AnimationNode)base.Find(movement);}}

		}

		/// <summary>
		/// Describes the kind of font and the color use to display text.
		/// </summary>
		internal class FontNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode colorNode;
			private MarkThree.Forms.AttributeNode boldNode;
			private MarkThree.Forms.AttributeNode italicNode;
			private MarkThree.Forms.AttributeNode underlineNode;
			private MarkThree.Forms.AttributeNode strikeoutNode;
			private MarkThree.Forms.AttributeNode sizeNode;
			private MarkThree.Forms.AttributeNode nameNode;

			// Public Members
			internal AnimationList animationList;

			/// <summary>
			/// Initializes a Font object.
			/// </summary>
			public FontNode() : base(Token.Font)
			{

				// Initialize the object with the defaults.
				this.animationList = new AnimationList();

			}

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultSpreadsheet.ForeColor : ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (this.colorNode != null)
						Remove(this.colorNode);

					if (value != DefaultSpreadsheet.ForeColor)
						Add(new AttributeNode(Token.Color, ColorTranslator.ToHtml(value)));

				}

			}

			public System.Drawing.FontStyle FontStyle
			{

				get
				{

					FontStyle fontStyle = DefaultSpreadsheet.FontStyle;

					if (this.boldNode != null)
					{
						if (Convert.ToBoolean(this.boldNode.Value))
							fontStyle |= FontStyle.Bold;
						else
							fontStyle &= ~FontStyle.Bold;
					}

					if (this.italicNode != null)
					{
						if (Convert.ToBoolean(this.italicNode.Value))
							fontStyle |= FontStyle.Italic;
						else
							fontStyle &= ~FontStyle.Italic;
					}

					if (this.underlineNode != null)
					{
						if (Convert.ToBoolean(this.underlineNode.Value))
							fontStyle |= FontStyle.Underline;
						else
							fontStyle &= ~FontStyle.Underline;
					}

					if (this.strikeoutNode != null)
					{
						if (Convert.ToBoolean(this.strikeoutNode.Value))
							fontStyle |= FontStyle.Strikeout;
						else
							fontStyle &= ~FontStyle.Strikeout;
					}

					return fontStyle;

				}

				set
				{

					if ((value & FontStyle.Bold) == (DefaultSpreadsheet.FontStyle & FontStyle.Bold))
					{
						if (this.boldNode != null)
							Remove(this.boldNode);
					}
					else
					{
						if (this.boldNode == null)
							Add(new AttributeNode(Token.Bold));
						this.boldNode.Value = Convert.ToString((value & FontStyle.Bold) != 0);
					}

					if ((value & FontStyle.Italic) == (DefaultSpreadsheet.FontStyle & FontStyle.Italic))
					{
						if (this.italicNode != null)
							Remove(this.italicNode);
					}
					else
					{
						if (this.italicNode == null)
							Add(new AttributeNode(Token.Italic));
						this.italicNode.Value = Convert.ToString((value & FontStyle.Italic) != 0);
					}

					if ((value & FontStyle.Underline) == (DefaultSpreadsheet.FontStyle & FontStyle.Underline))
					{
						if (this.underlineNode != null)
							Remove(this.underlineNode);
					}
					else
					{
						if (this.underlineNode == null)
							Add(new AttributeNode(Token.Underline));
						this.underlineNode.Value = Convert.ToString((value & FontStyle.Underline) != 0);
					}

					if ((value & FontStyle.Strikeout) == (DefaultSpreadsheet.FontStyle & FontStyle.Strikeout))
					{
						if (this.strikeoutNode != null)
							Remove(this.strikeoutNode);
					}
					else
					{
						if (this.strikeoutNode == null)
							Add(new AttributeNode(Token.Strikeout));
						this.strikeoutNode.Value = Convert.ToString((value & FontStyle.Strikeout) != 0);
					}

				}

			}

			public System.String Name
			{

				get
				{

					return this.nameNode == null ? DefaultSpreadsheet.FontName : this.nameNode.Value;

				}

				set
				{

					if (this.nameNode != null)
						Remove(this.nameNode);

					if (value != DefaultSpreadsheet.FontName)
						Add(new AttributeNode(Token.FontName, value));

				}

			}

			public System.Single Size
			{

				get
				{
					return this.sizeNode == null ? DefaultSpreadsheet.FontSize : Convert.ToSingle(this.sizeNode.Value);
				}

				set
				{

					if (this.sizeNode != null)
						Remove(this.sizeNode);

					if (value != DefaultSpreadsheet.FontSize)
						Add(new AttributeNode(Token.Size, Convert.ToString(value)));

				}

			}
			
			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.FontName:

						this.nameNode = attributeNode;
						break;

					case Token.Size:

						this.sizeNode = attributeNode;
						break;

					case Token.Color:

						this.colorNode = attributeNode;
						break;

					case Token.Bold:

						this.boldNode = attributeNode;
						break;

					case Token.Italic:

						this.italicNode = attributeNode;
						break;

					case Token.Underline:

						this.underlineNode = attributeNode;
						break;

					case Token.Strikeout:

						this.strikeoutNode = attributeNode;
						break;

					}
				}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.FontName:

						this.nameNode = null;
						break;

					case Token.Size:

						this.sizeNode = null;
						break;

					case Token.Color:

						this.colorNode = null;
						break;

					case Token.Bold:

						this.boldNode = null;
						break;

					case Token.Italic:

						this.italicNode = null;
						break;

					case Token.Underline:

						this.underlineNode = null;
						break;

					case Token.Strikeout:

						this.strikeoutNode = null;
						break;

					}

				}

				base.Remove(node);

			}

		}
		
		/// <summary>
		/// Describes how to draw the interior area of a cell.
		/// </summary>
		internal class InteriorNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode colorNode;

			/// <summary>
			/// Initializes an Interior Node.
			/// </summary>
			public InteriorNode() : base(Token.Interior)
			{

				// Initialize the object with the defaults.
				this.Color = DefaultSpreadsheet.BackColor;

			}

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultSpreadsheet.BorderColor : ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (this.colorNode != null)
						Remove(this.colorNode);

					if (value != DefaultSpreadsheet.BorderColor)
						Add(new AttributeNode(Token.Color, ColorTranslator.ToHtml(value)));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Color:

						this.colorNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

		}

		/// <summary>
		/// Describes how the numbers (and text) are to be formatted.
		/// </summary>
		internal class NumberFormatNode : ElementNode
		{

			private MarkThree.Forms.AttributeNode formatNode;

			/// <summary>
			/// Create a NumberFormat object.
			/// </summary>
			public NumberFormatNode() : base(Token.NumberFormat)
			{

				// Initialize the object with defaults.
				this.formatNode = null;

			}

			public System.String Format
			{

				get
				{

					return this.formatNode == null ? DefaultSpreadsheet.Format :
						this.formatNode.Value == "General" ? "{0:}" : "{0:" + this.formatNode.Value + "}";

				}

				set
				{

					if (value == DefaultSpreadsheet.Format)
					{

						if (this.formatNode != null)
							Remove(this.formatNode);

					}
					else
					{
						if (this.formatNode == null)
							Add(new AttributeNode(Token.Format));

						string stringFormat = value.Trim(new char[] { '{', '}' });
						stringFormat = stringFormat.Remove(0, 2);
						if (stringFormat == string.Empty)
							stringFormat = "General";
						this.formatNode.Value = stringFormat;

					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Format:

						this.formatNode = attributeNode;
						break;

					}

				}

				base.Add(node);
				return;

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Format:

						this.formatNode = null;
						break;

					}

				}

				base.Add(node);
				return;

			}

		}

		/// <summary>
		/// Protection is used to prevent editing in a cell.
		/// </summary>
		internal class ProtectionNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode isProtectedNode;

			/// <summary>
			/// Initialize the Protection object.
			/// </summary>
			public ProtectionNode() : base(Token.Protection)
			{

				// Initialize the object from the defaults.
				this.IsProtected = DefaultSpreadsheet.IsProtected;

			}

			public System.Boolean IsProtected
			{

				get
				{
					return this.isProtectedNode == null ? DefaultSpreadsheet.IsProtected :
						Convert.ToBoolean(this.isProtectedNode.Value);
				}

				set
				{

					if (value == DefaultSpreadsheet.IsProtected)
					{
						if (this.isProtectedNode != null)
							this.Remove(this.isProtectedNode);
						this.isProtectedNode = null;
					}
					else
					{
						if (this.isProtectedNode == null)
							this.isProtectedNode = new AttributeNode(Token.Protected);
						this.isProtectedNode.Value = Convert.ToString(value);
					}

				}

			}
			
			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Protected:

						this.IsProtected = Convert.ToBoolean(attributeNode.Value);
						break;

					}

				}

				base.Add(node);
				return;

			}

		}

		/// <summary>
		/// A Worksheet is a collection of Spreadsheets.
		/// </summary>
		public class ViewNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode rowFilterNode;
			private DualList<string, ColumnReferenceNode> columnReferenceList;

			public ViewNode() : base(Token.View)
			{
				
				// Initialize the object
				this.columnReferenceList = new DualList<string, ColumnReferenceNode>();
			
			}

			public DualList<string, ColumnReferenceNode> ColumnReferences { get { return this.columnReferenceList; } }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.RowFilter:

						this.rowFilterNode = node as AttributeNode;
						break;

					}

				}

				if (node is ColumnReferenceNode)
				{
					ColumnReferenceNode columnReferenceNode = node as ColumnReferenceNode;
					this.columnReferenceList.Add(columnReferenceNode.ColumnId, columnReferenceNode);
				}

				base.Add(node);

			}

		}

		/// <summary>
		/// The attributes of a style that determine how the spreadsheet is drawn on the output device.
		/// </summary>
		public class StyleNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.Stylesheet.AlignmentNode alignmentNode;
			private MarkThree.Forms.Stylesheet.BordersNode bordersNode;
			private MarkThree.Forms.Stylesheet.FontNode fontNode;
			private MarkThree.Forms.Stylesheet.InteriorNode interiorNode;
			private MarkThree.Forms.Stylesheet.NumberFormatNode numberFormatNode;
			private MarkThree.Forms.Stylesheet.ProtectionNode protectionNode;
			private MarkThree.Forms.AttributeNode styleIdNode;
			private MarkThree.Forms.Stylesheet stylesheet;
			private MarkThree.DualList<string, StyleNode> childStyles;

			// Internal Members
			internal MarkThree.Forms.AttributeNode parentIdNode;
			internal MarkThree.Forms.Stylesheet.StyleNode parentStyleNode;

			/// <summary>
			/// Create a Style node from the input XML.
			/// </summary>
			/// <param name="xmlTokenReader">Represents a fast, non-cached, forward only access to XML Data.</param>
			/// <param name="tokenTable">A table of values that represent lexical elements of the XSL Stylesheet.</param>
			public StyleNode(Stylesheet stylesheet) : base(Token.Style)
			{

				// Initialize the object
				this.stylesheet = stylesheet;
				this.childStyles = new DualList<string, StyleNode>();

			}

			/// <summary>Gets the Identifier of the Style.</summary>
			public string Identifier
			{

				get { return this.styleIdNode.Value; }
				set
				{

					// Since the list of styles is ordered in order to quickly find a style using the identifier, it is important
					// to keep the list ordered properly when an item is given a new identity.  This will basically remove the node
					// and add it again with the new identity so it can find it's proper place in the list.
					this.styleIdNode.Value = value;
					if (this.parentIdNode != null)
					{
						StyleNode parentStyleNode = this.parentStyleNode;
						parent.Remove(this);
						this.styleIdNode.Value = value;
						parent.Add(this);
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.StyleId:

						this.styleIdNode = attributeNode;
						break;

					case Token.Parent:

						this.parentIdNode = attributeNode;
						break;

					}

				}

				if (node is AlignmentNode)
					this.alignmentNode = node as AlignmentNode;

				if (node is BordersNode)
					this.bordersNode = node as BordersNode;

				if (node is FontNode)
					this.fontNode = node as FontNode;

				if (node is InteriorNode)
					this.interiorNode = node as InteriorNode;

				if (node is NumberFormatNode)
					this.numberFormatNode = node as NumberFormatNode;

				if (node is ProtectionNode)
					this.protectionNode = node as ProtectionNode;

				base.Add(node);

			}

			public bool IsAlignmentNull { get { return this.alignmentNode == null; } }

			public bool IsBordersNull { get { return this.bordersNode == null; } }

			public bool IsFontNull { get { return this.fontNode == null; } }

			public bool IsInteriorNull { get { return this.interiorNode == null; } }

			public bool IsNumberFormatNull { get { return this.numberFormatNode == null; } }

			public bool IsProtectionNull { get { return this.protectionNode == null; } }

			/// <summary>
			/// Gets or sets whether editing is allowed in this cell.
			/// </summary>
			public bool IsProtected
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.protectionNode == null ? DefaultSpreadsheet.IsProtected : this.protectionNode.IsProtected; }

				// The only tricky part is making sure the attribute exists before setting the property.
				set
				{
					if (this.protectionNode == null)
						this.Add(new ProtectionNode());
					this.protectionNode.IsProtected = value;
				}

			}

			/// <summary>
			/// Gets a list of the descendant Style Nodes.
			/// </summary>
			/// <returns></returns>
			public DualList<string, StyleNode> GetStyles() {return this.childStyles;}

			/// <summary>
			/// Gets or sets the Parent in the Style hierarchy.
			/// </summary>
			public StyleNode ParentStyle
			{

				// This is the parent of this Style.
				get {return this.parentStyleNode;}

				set
				{

					// Remove the style from the parent hierarchy (if it had a parent) and add it to the children of the specified
					// style.
					if (this.parentStyleNode != null)
						this.parentStyleNode.GetStyles().Remove(this.Identifier);
					this.parentStyleNode = value;
					if (this.parentStyleNode != null)
						this.parentStyleNode.GetStyles().Add(this.Identifier, this);

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the string used to format numbers (and text) for a person to read.
			/// </summary>
			public string Format
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.numberFormatNode == null ? DefaultSpreadsheet.Format : this.numberFormatNode.Format; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.numberFormatNode == null)
						this.Add(new NumberFormatNode());
					this.numberFormatNode.Format = value;

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets how the contents of a cell are aligned and clipped within the cell.
			/// </summary>
			public StringFormat StringFormat
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.alignmentNode == null ? null : this.alignmentNode.StringFormat; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.alignmentNode == null)
						this.Add(new AlignmentNode());
					this.alignmentNode.StringFormat = value;

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets the border setting associated with the specified position.
			/// </summary>
			/// <param name="borderPosition">The position of the desired border (Left, Up, Right, Down).</param>
			/// <returns>The border associated with the specified position, or null if the border is not set.</returns>
			private BorderNode GetBorder(BorderPosition borderPosition)
			{

				// Return the Border setting associated with the given position (left, top, right, bottom), or null if there is no 
				// 'Font' setting defined for this style.
				return this.bordersNode == null ? null : this.bordersNode.BorderList[borderPosition];

			}

			/// <summary>
			/// Sets the specified border.
			/// </summary>
			/// <param name="borderPosition">The position (left, top, right, bottom) of the new border setting.</param>
			/// <param name="value">The new border setting for the specified position, null if the position is to be removed
			/// from the settings.</param>
			private void SetBorder(BorderPosition borderPosition, BorderNode value)
			{

				// This will insure that there is a 'Borders' element to which the new border setting is attached.
				if (this.bordersNode == null)
					this.Add(new BordersNode());

				// If there is a border already at this setting, remove it.
				BorderNode border = this.bordersNode.BorderList[borderPosition];
				if (border != null)
					this.bordersNode.Remove(border);

				// If there is a new border to be set, install it at the specified position.
				if (value != null)
				{
					value.BorderPosition = borderPosition;
					this.bordersNode.Add(value);
				}

				// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
				this.stylesheet.OnChanged(this);

			}

			/// <summary>Gets or sets the Left Border setting.</summary>
			public BorderNode LeftBorder { get { return GetBorder(BorderPosition.Left); } set { SetBorder(BorderPosition.Left, value); } }

			/// <summary>Gets or sets the Top Border setting.</summary>
			public BorderNode TopBorder { get { return GetBorder(BorderPosition.Top); } set { SetBorder(BorderPosition.Top, value); } }

			/// <summary>Gets or sets the Right Border setting.</summary>
			public BorderNode RightBorder { get { return GetBorder(BorderPosition.Right); } set { SetBorder(BorderPosition.Right, value); } }

			/// <summary>Gets or sets the Bottom Border setting.</summary>
			public BorderNode BottomBorder { get { return GetBorder(BorderPosition.Bottom); } set { SetBorder(BorderPosition.Bottom, value); } }

			/// <summary>
			/// Gets the Animation setting for the specified movement.
			/// </summary>
			/// <param name="movement">The movement (up, down, unchanged).</param>
			/// <returns>The Animation setting for the specified movement.</returns>
			private AnimationNode GetAnimation(Movement movement)
			{

				// Return the Animation setting associated with the given movement (up, down, same), or null if there is no 'Font' 
				// setting defined for this style.
				return this.fontNode == null ? null : this.fontNode.animationList[movement];

			}

			/// <summary>
			/// Sets the Animation setting for the specified movement.
			/// </summary>
			/// <param name="movement">The movement (up, down, unchanged).</param>
			/// <param name="value">The new value of the Anmiation for the specified movement, or null if the setting for the
			/// specified movement is to be removed.</param>
			private void SetAnimation(Movement movement, AnimationNode value)
			{

				// This insures that a 'Font' element exists for the Animation setting.
				if (this.fontNode == null)
					this.Add(new FontNode());

				// If an Animation setting already exists for the specified movement, remove it.
				AnimationNode animation = this.fontNode.animationList[movement];
				if (animation != null)
					this.fontNode.Remove(animation);

				// If the new setting is not provided (null) it will be removed.  Otherwise, this will add the new setting to the
				// list with the specified movement providing the unique key into the list.  This allows for indexing into the
				// array of Animation values.
				if (value != null)
				{
					value.Movement = movement;
					this.fontNode.Add(value);
				}

				// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
				this.stylesheet.OnChanged(this);

			}

			/// <summary>Gets or sets the animation setting for when a value is updated, but doesn't change.</summary>
			public AnimationNode NilAnimation { get { return GetAnimation(Movement.Nil); } set { SetAnimation(Movement.Nil, value); } }

			/// <summary>Gets or sets the animation setting for when a value is update and increases in value.</summary>
			public AnimationNode UpAnimation { get { return GetAnimation(Movement.Up); } set { SetAnimation(Movement.Up, value); } }

			/// <summary>Gets or sets the animation setting for when a value is updated, and decreases in value.</summary>
			public AnimationNode DownAnimation { get { return GetAnimation(Movement.Down); } set { SetAnimation(Movement.Down, value); } }

			/// <summary>
			/// Gets or sets the font used to draw text.
			/// </summary>
			public System.Drawing.Font Font
			{

				get
				{

					// Return a default font when there is no setting.  Otherwise, return a font with the attributes stored in the
					// 'Font' element.
					return this.fontNode == null ? new System.Drawing.Font(DefaultSpreadsheet.FontName, DefaultSpreadsheet.FontSize,
						 DefaultSpreadsheet.FontStyle) : new System.Drawing.Font(this.fontNode.Name, this.fontNode.Size, this.fontNode.FontStyle);

				}

				set
				{

					// This insures there is a 'Font' setting where the attributes can be stored.
					if (this.fontNode == null)
						this.Add(new FontNode());

					// Save the attributes of the font.
					this.fontNode.Name = value.Name;
					this.fontNode.Size = value.Size;
					this.fontNode.FontStyle = value.Style;

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the text.
			/// </summary>
			public Color ForeColor
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.fontNode == null ? DefaultSpreadsheet.ForeColor : this.fontNode.Color; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.fontNode == null)
						this.Add(new FontNode());
					this.fontNode.Color = value;

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a cell.
			/// </summary>
			public Color BackColor
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.interiorNode == null ? DefaultSpreadsheet.BackColor : this.interiorNode.Color; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.interiorNode == null)
						this.Add(new InteriorNode());
					this.interiorNode.Color = value;

					// This will fire off an event that will indicate to any listeners that the stylesheet has been modified.
					this.stylesheet.OnChanged(this);

				}

			}

			#region IComparable Members

			public int CompareTo(object obj)
			{

				if (obj is StyleNode)
					return this.Identifier.CompareTo(((StyleNode)obj).Identifier);

				if (obj is string)
					return this.Identifier.CompareTo((string)obj);

				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(), obj.GetType().ToString()));

			}

			#endregion

		}
		
		/// <summary>
		/// A collection of Style elements.
		/// </summary>
		public class StylesNode : SortedElementList
		{

			public StylesNode() : base(Token.Styles) { }

			/// <summary>
			/// Finds a Style Node in the sorted list using a unique key.
			/// </summary>
			/// <param name="key">The unique key that is used to identify the Node.</param>
			/// <returns>The Node matching the unique key, null if there are no matching Nodes.</returns>
			public StyleNode Find(string identifier) { return (StyleNode)base.Find(identifier); }

			public StyleNode this[string identifier] { get { return Find(identifier); } }

			public override void Add(Node node)
			{

				if (node is StyleNode)
				{

					StyleNode styleNode = node as StyleNode;
					if (styleNode.parentIdNode != null)
					{
						styleNode.parentStyleNode = Find(styleNode.parentIdNode.Value);
						if (styleNode.parentStyleNode != null)
							styleNode.parentStyleNode.GetStyles().Add(styleNode.Identifier, styleNode);
					}

				}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is StyleNode)
				{

					StyleNode styleNode = node as StyleNode;
					if (styleNode.parentStyleNode != null)
					{
						styleNode.parentStyleNode.GetStyles().Remove(styleNode.Identifier);
						styleNode.parentStyleNode = null;
					}

				}

				base.Add(node);

			}

		}
		
		/// <summary>
		/// A Table holds the definition of the spreadsheet layout including the columns, sort order, primary key and how to apply
		/// the row templates.
		/// </summary>
		public class TableNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.Stylesheet stylesheet;
			private MarkThree.Forms.AttributeNode headerHeightNode;
			private MarkThree.Forms.AttributeNode selectionModeNode;
			private MarkThree.Forms.Stylesheet.ColumnsNode columnsNode;
			private MarkThree.Forms.Stylesheet.ConstraintsNode constraintsNode;
			private MarkThree.Forms.Stylesheet.SortNode sortNode;
			private MarkThree.Forms.Stylesheet.ViewNode viewNode;

			/// <summary>
			/// Create a Table node from the input XML.
			/// </summary>
			public TableNode(Stylesheet stylesheet) : base(Token.Table)
			{

				// Initialize the object.
				this.stylesheet = stylesheet;

			}

			public System.Single HeaderHeight
			{

				get
				{
					return this.headerHeightNode == null ? DefaultSpreadsheet.HeaderHeight :
						Convert.ToSingle(this.headerHeightNode.Value);
				}

				set
				{

					if (this.headerHeightNode != null)
						Remove(this.headerHeightNode);

					if (value != DefaultSpreadsheet.HeaderHeight)
						Add(new AttributeNode(Token.HeaderHeight, Convert.ToString(value)));

				}

			}

			public MarkThree.Forms.SelectionMode SelectionMode
			{

				get
				{
					return this.selectionModeNode == null ? DefaultSpreadsheet.SelectionMode :
						(SelectionMode)Enum.Parse(typeof(SelectionMode), this.selectionModeNode.Value);
				}

				set
				{

					if (this.selectionModeNode != null)
						Remove(this.selectionModeNode);

					if (value != DefaultSpreadsheet.SelectionMode)
						Add(new AttributeNode(Token.Direction, value.ToString()));

				}

			}

			public ConstraintsNode Constraints { get { return this.constraintsNode; } }

			public SortNode Sort { get { return this.sortNode; } }

			public ViewNode View { get { return this.viewNode; } }

			/// <summary>
			/// Gets or sets the list of columns in the spreadsheet.
			/// </summary>
			[Browsable(false)]
			public ColumnsNode Columns
			{
				get { return this.columnsNode; }
				set { this.columnsNode = value; this.stylesheet.OnChanged(this); }
			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.HeaderHeight:

						this.headerHeightNode = attributeNode;
						break;

					case Token.SelectionMode:

						this.selectionModeNode = attributeNode;
						break;

					}

				}

				if (node is ColumnsNode)
					this.columnsNode = node as ColumnsNode;

				if (node is ConstraintsNode)
					this.constraintsNode = node as ConstraintsNode;

				if (node is SortNode)
					this.sortNode = node as SortNode;

				if (node is ViewNode)
					this.viewNode = node as ViewNode;

				base.Add(node);

			}

		}
		
		/// <summary>
		/// A Node that contains a collection of Columns.
		/// </summary>
		public class ColumnsNode : SortedElementList
		{

			public ColumnsNode() : base(Token.Columns) { }

			public new ColumnNode Find(object identifier) { return (ColumnNode)base.Find(identifier); }

			public new ColumnsNode Clone()
			{
				ColumnsNode columnsNode = new ColumnsNode();
				foreach (Node childNode in this)
					columnsNode.Add(childNode);
				return columnsNode;
			}

			public new ColumnNode this[int index] { get { return (ColumnNode)base[index]; } }

		}

		/// <summary>
		/// The attributes of a column of data in the spreadsheet.
		/// </summary>
		public class ColumnNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.AttributeNode identifierNode;
			private MarkThree.Forms.AttributeNode widthNode;
			private MarkThree.Forms.AttributeNode descriptionNode;
			private MarkThree.Forms.AttributeNode imageNode;
			private MarkThree.Forms.AttributeNode styleIdNode;
			private MarkThree.Forms.Stylesheet stylesheet;

			// Public Members
			public Stylesheet.StyleNode style;

			/// <summary>
			/// Initialize a Column.
			/// </summary>
			public ColumnNode(Stylesheet stylesheet)
				: base(Token.Column)
			{

				// Initialize the object
				this.stylesheet = stylesheet;

			}

			public string Identifier
			{

				get { return this.identifierNode == null ? string.Empty : this.identifierNode.Value; }

				set
				{

					if (this.identifierNode != null)
					{
						if (this.parent != null)
							this.parent.Remove(this);
						Remove(this.identifierNode);
					}

					if (value != string.Empty)
					{
						Add(new AttributeNode(Token.ColumnId, value));
						if (this.parent != null)
							this.parent.Add(this);
					}

				}

			}

			public System.Single Width
			{

				get
				{
					return this.widthNode == null ? DefaultSpreadsheet.ColumnWidth : Convert.ToSingle(this.widthNode.Value);
				}

				set
				{

					if (value == DefaultSpreadsheet.ColumnWidth)
					{
						if (this.widthNode != null)
							Remove(this.widthNode);
					}
					else
					{
						if (this.widthNode == null)
							Add(new AttributeNode(Token.Width));
						this.widthNode.Value = Convert.ToString(value);
					}

				}

			}

			public System.String Description
			{

				get
				{
					return this.descriptionNode == null ? string.Empty : this.descriptionNode.Value;
				}

				set
				{

					if (this.descriptionNode != null)
						Remove(this.descriptionNode);

					if (value != string.Empty)
						Add(new AttributeNode(Token.Description, value));

				}

			}

			public System.Drawing.Image Image
			{

				get
				{

					return this.imageNode == null ? null :
						Bitmap.FromStream(new MemoryStream(Convert.FromBase64String(this.imageNode.Value)));

				}

				set
				{

					if (this.imageNode != null)
						Remove(this.imageNode);

					if (value != null)
					{
						MemoryStream memoryStream = new MemoryStream();
						value.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
						Add(new AttributeNode(Token.Image, Convert.ToBase64String(memoryStream.GetBuffer())));
					}

				}

			}

			public System.String StyleIdentifier
			{

				get
				{
					return this.styleIdNode == null ? string.Empty : this.styleIdNode.Value;
				}

				set
				{

					if (this.styleIdNode != null)
						Remove(this.styleIdNode);

					if (value != string.Empty)
						Add(new AttributeNode(Token.StyleId, value));

				}

			}

			/// <summary>A descriptive text of the object.</summary>
			public override string ToString() {return this.Description.Replace("\n", " ");}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.ColumnId:

						this.identifierNode = attributeNode;
						break;

					case Token.Width:

						this.widthNode = attributeNode;
						break;

					case Token.Description:

						this.descriptionNode = attributeNode;
						break;

					case Token.Image:

						this.imageNode = attributeNode;
						break;

					case Token.StyleId:

						this.styleIdNode = attributeNode;
						
						// Validate that the identifier is correct.
						if (this.stylesheet.stylesNode.Find(attributeNode.Value) == null)
							throw new Exception(string.Format("An unknown style {0} was used in the declaration of column {1}",
								attributeNode.Value, this.Identifier));

						break;

					}

				}

				base.Add(node);

			}
			
			#region IComparable Members

			/// <summary>
			/// Compars this instance with the specified System.Object object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>Less than zero, this instance is less than obj.  zero, this instance is equal to obj, greater than zero,
			/// this instance is greater than obj.</returns>
			public int CompareTo(object obj)
			{

				// Compare this object aginst a Column Node.
				if (obj is ColumnNode)
					return this.Identifier.CompareTo(((ColumnNode)obj).Identifier);

				// Compare this object aginst a string.
				if (obj is string)
					return this.Identifier.CompareTo((string)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(),
					obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A constraint defines a primary index or some rule that is applied to the data in the viewer.
		/// </summary>
		public class ConstraintsNode : ElementNode
		{

			/// <summary>
			/// Initialize the Constraint object.
			/// </summary>
			public ConstraintsNode() : base(Token.Constraints) {}

			public override void Add(Node node)
			{

				if (node is ConstraintNode)
				{
					base.Add(node);
					return;
				}

				throw new Exception(string.Format("An {0} is not valid as a child or attribute of {1}.", node.GetType(), this.GetType()));

			}

		}

		/// <summary>
		/// A constraint defines a primary index or some rule that is applied to the data in the viewer.
		/// </summary>
		internal class ConstraintNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode primaryKeyNode;
			private MarkThree.Forms.AttributeNode uniqueNode;

			/// <summary>
			/// Initialize the Constraint object.
			/// </summary>
			public ConstraintNode() : base(Token.Constraint) { }

			public System.Boolean IsPrimaryKey
			{

				get { return this.primaryKeyNode == null ? false : Convert.ToBoolean(this.primaryKeyNode.Value); }

				set
				{

					if (this.primaryKeyNode != null)
						Remove(this.primaryKeyNode);

					if (value != false)
						Add(new AttributeNode(Token.PrimaryKey, true.ToString()));

				}

			}

			public System.Boolean IsUnique
			{

				get { return this.uniqueNode == null ? false : Convert.ToBoolean(this.uniqueNode.Value); }

				set
				{

					if (this.uniqueNode != null)
						Remove(this.uniqueNode);

					if (value != false)
						Add(new AttributeNode(Token.Unique, true.ToString()));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.PrimaryKey:

						this.primaryKeyNode = attributeNode;
						break;

					case Token.Unique:

						this.uniqueNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}
			
		}

		/// <summary>
		/// A Column used to declare a unique index or a rule.
		/// </summary>
		public class ColumnReferenceNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode columnIdNode;
			private MarkThree.Forms.Stylesheet stylesheet;

			public ColumnReferenceNode(Stylesheet stylesheet) : base(Token.ColumnReference)
			{
			
				// Initialize the object.
				this.stylesheet = stylesheet;

			}

			public string ColumnId
			{

				get {return this.columnIdNode == null ? string.Empty : this.columnIdNode.Value;}

				set
				{

					if (value == string.Empty)
					{
						if (this.columnIdNode != null)
							Remove(this.columnIdNode);
					}
					else
					{
						if (this.columnIdNode == null)
							Add(new AttributeNode(Token.ColumnId));
						this.columnIdNode.Value = value;
					}

				}

			}

			public MarkThree.Forms.Stylesheet.ColumnNode Column
			{

				get { return this.columnIdNode == null ? null : this.stylesheet.Table.Columns.Find(this.columnIdNode.Value); }

				set
				{

					if (this.columnIdNode == null)
						Remove(this.columnIdNode);

					if (value != null)
						Add(new AttributeNode(Token.ColumnId, value.Identifier));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.ColumnId:

						this.columnIdNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

		}

		/// <summary>
		/// Describes how the data is to be ordered in the viewer.
		/// </summary>
		public class SortNode : SortedElementList
		{

			public SortNode() : base(Token.Sort) { }

			public new SortColumnNode Find(object key) { return (SortColumnNode)base.Find(key); }

		}

		/// <summary>
		/// Used to define the sort order of the information displayed in the viewer.
		/// </summary>
		public class SortColumnNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.AttributeNode directionNode;
			private MarkThree.Forms.AttributeNode columnIdNode;
			private MarkThree.Forms.Stylesheet stylesheet;

			/// <summary>
			/// Initialize the description of the sort order.
			/// </summary>
			public SortColumnNode(Stylesheet stylesheet) : base(Token.SortColumn)
			{

				// Initialize the object
				this.stylesheet = stylesheet;

			}

			/// <summary>
			/// Initialize the description of the sort order.
			/// </summary>
			public SortColumnNode(Stylesheet stylesheet, ColumnNode column, SortDirection sortDirection) : base(Token.SortColumn)
			{

				// Initialize the object
				this.stylesheet = stylesheet;
				this.Column = column;
				this.Direction = sortDirection;

			}

			public MarkThree.Forms.Stylesheet.ColumnNode Column
			{

				get { return this.columnIdNode == null ? null : this.stylesheet.Table.Columns.Find(this.columnIdNode.Value); }

				set
				{

					if (this.columnIdNode == null)
						Remove(this.columnIdNode);

					if (value != null)
						Add(new AttributeNode(Token.ColumnId, value.Identifier));

				}

			}

			public MarkThree.Forms.SortDirection Direction
			{

				get
				{
					return this.directionNode == null ? SortDirection.Ascending :
						(SortDirection)Enum.Parse(typeof(SortDirection), this.directionNode.Value);
				}

				set
				{

					if (this.directionNode != null)
						Remove(this.directionNode);

					if (value != SortDirection.Ascending)
						Add(new AttributeNode(Token.Direction, value.ToString()));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{
					case Token.ColumnId:

						this.columnIdNode = attributeNode;
						break;

					case Token.Direction:

						this.directionNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

			#region IComparable Members

			/// <summary>
			/// Compars this instance with the specified System.Object object.
			/// </summary>
			/// <param name="obj">An object to compare with this instance.</param>
			/// <returns>Less than zero, this instance is less than obj.  zero, this instance is equal to obj, greater than zero,
			/// this instance is greater than obj.</returns>
			public int CompareTo(object obj)
			{

				// Compare this object aginst another ViewColumn.
				if (obj is SortColumnNode)
					return this.Column.Identifier.CompareTo(((SortColumnNode)obj).Column.Identifier);

				// Compare this object against a string.
				if (obj is string)
					return this.Column.Identifier.CompareTo((string)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(), obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// The 'apply-template' Nodes will generate output when in the incoming element matches a template in the XSL file.
		/// </summary>
		internal class ApplyTemplateNode : ElementNode
		{

			public ApplyTemplateNode() : base(Token.ApplyTemplate) { }

		}

		/// <summary>
		/// A Fragment is used to incrementally update the spreadsheet.
		/// </summary>
		internal class FragmentNode: ElementNode
		{

			public FragmentNode() : base(Token.Fragment) { }

		}

		/// <summary>
		/// A Insert is used to incrementally update the spreadsheet.
		/// </summary>
		internal class InsertNode: ElementNode
		{

			public InsertNode() : base(Token.Insert) { }

		}

		/// <summary>
		/// A Update is used to incrementally update the spreadsheet.
		/// </summary>
		internal class UpdateNode: ElementNode
		{

			public UpdateNode() : base(Token.Update) { }

		}

		/// <summary>
		/// A Delete is used to incrementally update the spreadsheet.
		/// </summary>
		internal class DeleteNode: ElementNode
		{

			public DeleteNode() : base(Token.Delete) { }

		}

		/// <summary>
		/// Contains the definition of a row of data in the viewer.
		/// </summary>
		public class RowNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode heightNode;

			public System.Single Height
			{

				get
				{
					return this.heightNode == null ? DefaultSpreadsheet.RowHeight : Convert.ToSingle(this.heightNode.Value);
				}

				set
				{

					if (value == DefaultSpreadsheet.RowHeight)
					{
						if (this.heightNode != null)
							Remove(this.heightNode);
					}
					else
					{
						if (this.heightNode == null)
							Add(new AttributeNode(Token.Height));
						this.heightNode.Value = Convert.ToString(value);
					}

				}

			}

			/// <summary>
			/// Initialize the Row.
			/// </summary>
			public RowNode() : base(Token.Row) {}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{
					case Token.Height:

						this.heightNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

		}

		/// <summary>
		/// A Cell is the smallest unit of displayable data in the viewer.
		/// </summary>
		internal class CellNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode columnIdNode;
			private MarkThree.Forms.AttributeNode dataTypeNode;
			private MarkThree.Forms.AttributeNode styleIdNode;
			private MarkThree.Forms.TextNode literalNode;
			private MarkThree.Forms.Stylesheet stylesheet;

			/// <summary>
			/// Initialize the smallest unit of data in the viewer.
			/// </summary>
			public CellNode(Stylesheet stylesheet) : base(Token.Cell)
			{

				// Initialize the object
				this.stylesheet = stylesheet;

			}

			public MarkThree.Forms.Stylesheet.ColumnNode Column
			{

				get { return this.columnIdNode == null ? null : this.stylesheet.Table.Columns.Find(this.columnIdNode.Value); }

				set
				{

					if (this.columnIdNode == null)
						Remove(this.columnIdNode);

					if (value != null)
						Add(new AttributeNode(Token.ColumnId, value.Identifier));

				}

			}

			public System.Type DataType
			{

				get
				{

					return this.dataTypeNode == null ? typeof(object) : Type.GetType(this.dataTypeNode.Value);

				}

				set
				{

					if (this.dataTypeNode != null)
						Remove(this.dataTypeNode);

					Add(new AttributeNode(Token.Type, value.ToString()));

				}

			}

			public System.String StyleIdentifier
			{

				get
				{
					return this.styleIdNode == null ? string.Empty : this.styleIdNode.Value;
				}

				set
				{

					if (this.styleIdNode != null)
						Remove(this.styleIdNode);

					if (value != string.Empty)
						Add(new AttributeNode(Token.StyleId, value));

				}

			}

			public System.String Literal
			{

				get { return this.literalNode == null ? string.Empty : this.literalNode.text; }

				set
				{

					if (this.literalNode != null)
						Remove(this.literalNode);

					if (value != string.Empty)
						Add(new TextNode(value));

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{
					case Token.ColumnId:

						this.columnIdNode = attributeNode;
						break;

					case Token.StyleId:

						this.styleIdNode = attributeNode;
						break;

					case Token.Type:

						this.dataTypeNode = attributeNode;
						break;

					}

				}

				if (node is TextNode)
					this.literalNode = node as TextNode;

				base.Add(node);

			}

		}

		/// <summary>
		/// Any one of the several XSL Instructions that can appear in a stylesheet.
		/// </summary>
		internal class XslElementNode : ElementNode
		{

			public XslElementNode(Token token) : base(token) { }
			
		}

		internal class XslAttributeNode : XslElementNode
		{

			public XslAttributeNode() : base(Token.Attribute) { }

		}

		internal class ChooseNode : XslElementNode
		{

			public ChooseNode() : base(Token.Choose) { }

		}

		internal class IfNode : XslElementNode
		{

			public IfNode() : base(Token.If) { }

		}

		internal class OtherwiseNode : XslElementNode
		{

			public OtherwiseNode() : base(Token.Otherwise) { }

		}

		internal class ValueOfNode : XslElementNode
		{

			public ValueOfNode() : base(Token.ValueOf) { }

		}

		internal class VariableNode : XslElementNode
		{

			public VariableNode() : base(Token.Variable) { }

		}

		internal class WhenNode : XslElementNode
		{

			public WhenNode() : base(Token.When) { }

		}

		/// <summary>
		/// A Stylesheet describes how the incoming XML data in transformed to a display device.
		/// </summary>
		public Stylesheet()
		{

			// Initialize the object.
			this.stylesheetNode = null;

			// This is used to coordinate access to this object.
			this.readerWriterLock = new ReaderWriterLock();

			// This is an array of prohibited delegates for the event that signals a change to the stylesheet parameters.  During a
			// block of updates to the stylesheet, for example when the stylesheet is loaded form an XML source, several updates
			// can occur.  Calling the 'BeginLoad' at the start and the 'EndLoad' at the end of a block of operations will insure
			// that only a single event is generated.
			this.inhibitedChangedDelegates = null;

		}

		public static implicit operator Node(Stylesheet stylesheet) { return stylesheet.stylesheetNode; }

		public DualList<string, TemplateNode> Templates { get { return this.stylesheetNode.Templates; } }

		/// <summary>
		/// This lock is used for multithreaded access to the Stylesheet.
		/// </summary>
		public ReaderWriterLock Lock { get { return this.readerWriterLock; } }

		/// <summary>
		/// Indexes into the list of styles in this stylesheet using a unique identifier.
		/// </summary>
		public StylesNode Styles {get {return this.stylesNode;}}

		/// <summary>
		/// The attributes of the table.
		/// </summary>
		public TableNode Table {get {return this.tableNode;}}

		/// <summary>
		/// Inhibits the 'ChangeEvent' until an 'EndLoad' method call.  This is used to prevent invokation of multiple events while doing
		/// multiple updates to the stylesheet.  When 'EndLoad' is called, a single event will be generated.
		/// </summary>
		public void BeginLoad()
		{

			// This will insure that the operation is thread-safe.
			lock (this)
			{

				// This effectively inhibits the events until the delegates are restored to the 'Change' event.
				if (this.Changed != null)
				{
					this.inhibitedChangedDelegates = this.Changed.GetInvocationList();
					foreach (Delegate delegateItem in this.inhibitedChangedDelegates)
						System.Delegate.Remove(this.Changed, delegateItem);
				}

				//  This is used to indicate that some 'Change' event has occurred during the time the events were inhibited.  If this
				// value is true when the 'EndLoad' is called, a single event will be invoked.
				this.hasChangeEvent = false;

			}

		}

		/// <summary>
		/// Enables the events associated with changing the stylesheet.
		/// </summary>
		public void EndLoad()
		{

			// This insures thread-safety.
			lock (this)
			{

				// Re-install any of the events for changing the stylesheet values.
				this.Changed = (EventHandler)System.Delegate.Combine(this.inhibitedChangedDelegates);

				// Clearing the array of delegates also acts as an indicator that there is no prohibition against invoking the
				// events.
				this.inhibitedChangedDelegates = null;

				// If any events triggered during the prohibition, they will be invoked here.
				if (this.hasChangeEvent)
					OnChanged(this);

			}

		}
		
		/// <summary>
		/// Indicates that a change has been made to the stylesheets.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		private void OnChanged(object sender)
		{

			// This insures thread safety.
			lock (this)
			{

				// If there are no prohibited event delegates, then the events trigger normally.  Otherwise, invoking the events 
				// is deferred until the 'EndLoad'.  If any events are triggered while the prohibition is in place, the 'EndLoad'
				// will fire off a single event when the handlers are re-installed.
				if (this.inhibitedChangedDelegates == null)
				{
					if (this.Changed != null)
						this.Changed(sender, EventArgs.Empty);
				}
				else
					this.hasChangeEvent = true;

			}

		}

		/// <summary>
		/// Adds a column and direction to the list that defines the sort order for the rows.
		/// </summary>
		/// <param name="columnId">A column on which to sort.</param>
		/// <param name="sortDirection">The direction (Ascending or Descending) to sort.</param>
		public void SetView(string columnId, SortDirection sortDirection)
		{

			// Clear out the existing sort order and add this column and direction to the new order.
			this.Table.Sort.Clear();
			ColumnNode column = this.tableNode.Columns.Find(columnId);
			if (column == null)
				throw new Exception(string.Format("Can't find column {0} for a View", columnId));
			this.Table.Sort.Add(new SortColumnNode(this, column, sortDirection));

		}

		/// <summary>
		/// Append a column and direction to the existing sort order.
		/// </summary>
		/// <param name="columnId">A column on which to sort.</param>
		/// <param name="sortDirection">The direction (Ascending or Descending) to sort.</param>
		public void AppendView(string columnId, SortDirection sortDirection)
		{

			// If the column already exists in the sort order, then change it's direction.  Otherwise, append the column and
			// direction to the existing specification for the sort order.
			SortColumnNode viewColumn = this.Table.Sort.Find(columnId);
			if (viewColumn != null)
				viewColumn.Direction = sortDirection;
			else
			{
				ColumnNode column = this.tableNode.Columns.Find(columnId);
				if (column == null)
					throw new Exception(string.Format("Can't find column {0} for a Sort Order", columnId));
				this.Table.Sort.Add(new SortColumnNode(this, column, sortDirection));
			}

		}
		
	}

}
