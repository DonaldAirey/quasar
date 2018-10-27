namespace MarkThree.Forms
{

	using MarkThree;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Threading;

	public enum Effect { None, Fade, Flash };

	/// <summary>
	/// A DataTransform describes how to transform an XML Document into viewable data.
	/// </summary>
	/// <remarks>
	/// The dataTransform is used to transform XML Data into some human readable output.  This class makes it possible to manipulate
	/// elements of the dataTransform in order to incorporate user preferences in the transformation.  For example, sort orders,
	/// filtering, color preferences and the like can be stored in a flexible format along with the rules for transformation.
	/// </remarks>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class DataTransform : ElementNode
	{

		// Private Members
		private MarkThree.Forms.AttributeNode targetNamespaceNode;
		private MarkThree.Forms.AttributeNode dataTransformIdNode;
		private MarkThree.Forms.AttributeNode sourceNode;
		private MarkThree.Forms.DataTransform.LocksNode locksNode;
		private MarkThree.Forms.DataTransform.StylesNode stylesNode;
		private MarkThree.Forms.DataTransform.ColumnsNode columnsNode;
		private MarkThree.Forms.DataTransform.ViewNode viewNode;
		private MarkThree.Forms.DataTransform.SplitNode splitNode;
		private MarkThree.Forms.DataTransform.ScaleNode scaleNode;
		private System.Boolean hasChangeEvent;
		private System.Delegate[] inhibitedChangedDelegates;

		// Public Read Only Members
		public readonly MarkThree.DualList<string, TemplateNode> Templates;
		public readonly MarkThree.DualList<string, VariableNode> Variables;

		// Public Events
		public event EventHandler Changed;

		public MarkThree.Forms.DataTransform.LocksNode Locks
		{

			get
			{
				if (this.locksNode == null)
					this.Add(new LocksNode());
				return this.locksNode;
			}

		}

		public MarkThree.Forms.DataTransform.StylesNode Styles
		{

			get
			{
				if (this.stylesNode == null)
					this.Add(new StylesNode());
				return this.stylesNode;
			}

		}

		public MarkThree.Forms.DataTransform.ViewNode View
		{

			get
			{
				if (this.viewNode == null)
					this.Add(new ViewNode());
				return this.viewNode;
			}

		}

		public MarkThree.Forms.DataTransform.ColumnsNode Columns
		{

			get
			{
				if (this.columnsNode == null)
					this.Add(new ColumnsNode());
				return this.columnsNode;
			}

			set
			{

				this.columnsNode.Clear();
				foreach (DataTransform.ColumnsNode columnNode in value)
					this.columnsNode.Add(columnNode);

			}

		}

		public SizeF SplitSize
		{

			get
			{
				return this.splitNode == null ? SizeF.Empty : this.splitNode.SizeF;
			}

		}

		public float ScaleFactor
		{

			get
			{
				return this.scaleNode == null ? 1.0f : this.scaleNode.Factor;
			}

		}

		/// <summary>
		/// Matches data in the incoming XML file to produce output.
		/// </summary>
		/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		public class TemplateNode : ElementNode, IComparable
		{

			private MarkThree.Forms.AttributeNode matchNode;
			public readonly MarkThree.DualList<string, RowNode> Rows;

			/// <summary>
			/// Creates a Template.
			/// </summary>
			public TemplateNode()
				: base(Token.Template)
			{

				// Initialize the object.
				this.Rows = new DualList<string, RowNode>();

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.Match:

						this.matchNode = attributeNode;
						break;

					}

				}

				if (node is RowNode)
				{
					RowNode rowNode = node as RowNode;
					this.Rows.Add(rowNode.RowId, rowNode);
				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				TemplateNode templateNode = new TemplateNode();
				foreach (Node childNode in this)
					templateNode.Add(childNode.Clone());
				return templateNode;

			}

			public System.String Match
			{

				get
				{
					return this.matchNode == null ? string.Empty : this.matchNode.Value;
				}

				set
				{

					if (value == string.Empty)
						Remove(this.matchNode);
					else
					{
						if (this.matchNode == null)
							Add(new AttributeNode(Token.Match));
						this.matchNode.Value = value;
					}

				}

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
		/// Describes how the contents of a tile should be aligned within the tile.
		/// </summary>
		/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		internal class StringFormatNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode alignmentNode;
			private MarkThree.Forms.AttributeNode lineAlignmentNode;

			/// <summary>
			/// Initializes an Alignment with default values.
			/// </summary>
			public StringFormatNode() : base(Token.StringFormat) { }

			public System.Drawing.StringFormat StringFormat
			{

				get
				{

					StringFormat stringFormat = new System.Drawing.StringFormat();
					stringFormat.Alignment = DefaultDocument.Alignment;
					stringFormat.LineAlignment = DefaultDocument.LineAlignment;
					stringFormat.FormatFlags = DefaultDocument.StringFormatFlags;

					if (this.alignmentNode != null)
						stringFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), this.alignmentNode.Value);

					if (this.lineAlignmentNode != null)
						stringFormat.LineAlignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), this.lineAlignmentNode.Value);

					return stringFormat;

				}

				set
				{

					if (value.Alignment == DefaultDocument.Alignment)
					{
						if (this.alignmentNode != null)
							Remove(this.alignmentNode);
					}
					else
					{

						if (this.alignmentNode == null)
							Add(new AttributeNode(Token.Alignment));

						this.alignmentNode.Value = Enum.GetName(typeof(StringAlignment), value.Alignment);

					}

					if (value.LineAlignment == DefaultDocument.LineAlignment)
					{

						if (this.lineAlignmentNode != null)
							Remove(this.lineAlignmentNode);

					}
					else
					{

						if (this.lineAlignmentNode == null)
							Add(new AttributeNode(Token.LineAlignment));

						this.lineAlignmentNode.Value = Enum.GetName(typeof(StringAlignment), value.LineAlignment);

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
					case Token.Alignment:

						this.alignmentNode = attributeNode;
						break;

					case Token.LineAlignment:

						this.lineAlignmentNode = attributeNode;
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
					case Token.Alignment:

						this.alignmentNode = null;
						break;

					case Token.LineAlignment:

						this.lineAlignmentNode = null;
						break;

					}

				}

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				StringFormatNode stringFormatNode = new StringFormatNode();
				foreach (Node childNode in this)
					stringFormatNode.Add(childNode.Clone());
				return stringFormatNode;

			}

		}

		/// <summary>
		/// A Border describes how a tile is outlined.
		/// </summary>
		/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
		abstract public class BorderNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode colorNode;
			private MarkThree.Forms.AttributeNode widthNode;

			/// <summary>
			/// Initialize a border.
			/// </summary>
			public BorderNode(Token token) : base(token) { }

			/// <summary>
			/// Initialize a border.
			/// </summary>
			/// <param name="color">The color of the border.</param>
			/// <param name="weight">The thickness of the border.</param>
			public BorderNode(Token token, Color color, float weight)
				: base(token)
			{

				// Initialize the object.
				this.Width = weight;
				this.Color = color;

			}

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultDocument.BorderColor : ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (value == DefaultDocument.BorderColor)
						Remove(this.colorNode);
					else
					{
						if (this.colorNode == null)
							Add(new AttributeNode(Token.Color));
						this.colorNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Single Width
			{

				get
				{
					return this.widthNode == null ? DefaultDocument.BorderWidth : Convert.ToSingle(this.widthNode.Value);
				}

				set
				{

					if (value == DefaultDocument.BorderWidth)
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

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.Width:

						this.widthNode = node as AttributeNode;
						break;

					case Token.Color:

						this.colorNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

		}

		public class BottomBorderNode : BorderNode
		{

			public BottomBorderNode() : base(Token.BottomBorder) { }

			public BottomBorderNode(Color color, float width) : base(Token.BottomBorder, color, width) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				BottomBorderNode bottomBorderNode = new BottomBorderNode();
				foreach (Node childNode in this)
					bottomBorderNode.Add(childNode.Clone());
				return bottomBorderNode;

			}

		}

		public class LeftBorderNode : BorderNode
		{

			public LeftBorderNode() : base(Token.LeftBorder) { }

			public LeftBorderNode(Color color, float width) : base(Token.LeftBorder, color, width) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				LeftBorderNode leftBorderNode = new LeftBorderNode();
				foreach (Node childNode in this)
					leftBorderNode.Add(childNode.Clone());
				return leftBorderNode;

			}

		}

		public class RightBorderNode : BorderNode
		{

			public RightBorderNode() : base(Token.RightBorder) { }

			public RightBorderNode(Color color, float width) : base(Token.RightBorder, color, width) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				RightBorderNode rightBorderNode = new RightBorderNode();
				foreach (Node childNode in this)
					rightBorderNode.Add(childNode.Clone());
				return rightBorderNode;

			}

		}

		public class TopBorderNode : BorderNode
		{

			public TopBorderNode() : base(Token.TopBorder) { }

			public TopBorderNode(Color color, float width) : base(Token.TopBorder, color, width) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				TopBorderNode topBorderNode = new TopBorderNode();
				foreach (Node childNode in this)
					topBorderNode.Add(childNode.Clone());
				return topBorderNode;

			}

		}

		/// <summary>
		/// Describes how a tile changes colors when new values are entered.
		/// </summary>
		public class AnimationNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode downNode;
			private MarkThree.Forms.AttributeNode effectNode;
			private MarkThree.Forms.AttributeNode backgroundNode;
			private MarkThree.Forms.AttributeNode foregroundNode;
			private MarkThree.Forms.AttributeNode onNode;
			private MarkThree.Forms.AttributeNode offNode;
			private MarkThree.Forms.AttributeNode repeatNode;
			private MarkThree.Forms.AttributeNode sameNode;
			private MarkThree.Forms.AttributeNode stepsNode;
			private MarkThree.Forms.AttributeNode upNode;

			/// <summary>
			/// Initialize information used to animate a tile.
			/// </summary>
			public AnimationNode()
				: base(Token.Animation) { }

			public MarkThree.Forms.Effect Effect
			{

				get
				{
					return this.effectNode == null ? Effect.None : (Effect)Enum.Parse(typeof(Effect), this.effectNode.Value);
				}

				set
				{

					if (value == Effect.None)
					{
						if (this.effectNode != null)
							Remove(this.effectNode);
					}
					else
					{
						if (this.effectNode == null)
							Add(new AttributeNode(Token.Effect));
						this.effectNode.Value = value.ToString();
					}

				}

			}

			public System.Drawing.Color Background
			{

				get
				{
					return this.backgroundNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.backgroundNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.backgroundNode != null)
							Remove(this.backgroundNode);
					}
					else
					{
						if (this.backgroundNode == null)
							Add(new AttributeNode(Token.Background));
						this.backgroundNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Drawing.Color Foreground
			{

				get
				{
					return this.foregroundNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.foregroundNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.foregroundNode != null)
							Remove(this.foregroundNode);
					}
					else
					{
						if (this.foregroundNode == null)
							Add(new AttributeNode(Token.Foreground));
						this.foregroundNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Drawing.Color Up
			{

				get
				{
					return this.upNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.upNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.upNode != null)
							Remove(this.upNode);
					}
					else
					{
						if (this.upNode == null)
							Add(new AttributeNode(Token.Up));
						this.upNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Drawing.Color Down
			{

				get
				{
					return this.downNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.downNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.downNode != null)
							Remove(this.downNode);
					}
					else
					{
						if (this.downNode == null)
							Add(new AttributeNode(Token.Down));
						this.downNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Drawing.Color Same
			{

				get
				{
					return this.sameNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.sameNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.sameNode != null)
							Remove(this.sameNode);
					}
					else
					{
						if (this.sameNode == null)
							Add(new AttributeNode(Token.Same));
						this.sameNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public System.Int32 Steps
			{

				get
				{
					return this.stepsNode == null ? DefaultDocument.Steps : Convert.ToInt32(this.stepsNode.Value);
				}

				set
				{

					if (value == DefaultDocument.Steps)
					{

						if (this.stepsNode != null)
							Remove(this.stepsNode);
					}
					else
					{
						if (this.stepsNode == null)
							Add(this.stepsNode);
						this.stepsNode.Value = Convert.ToString(value);
					}

				}

			}

			public System.Int32 Repeat
			{

				get
				{
					return this.repeatNode == null ? 0 : Convert.ToInt32(this.repeatNode.Value);
				}

				set
				{

					if (value == 0)
					{

						if (this.repeatNode != null)
							Remove(this.repeatNode);
					}
					else
					{
						if (this.repeatNode == null)
							Add(this.repeatNode);
						this.repeatNode.Value = Convert.ToString(value);
					}

				}

			}

			public System.Int32 Off
			{

				get
				{
					return this.offNode == null ? 0 : Convert.ToInt32(this.offNode.Value);
				}

				set
				{

					if (value == 0)
					{

						if (this.offNode != null)
							Remove(this.offNode);
					}
					else
					{
						if (this.offNode == null)
							Add(this.offNode);
						this.offNode.Value = Convert.ToString(value);
					}

				}

			}

			public System.Int32 On
			{

				get
				{
					return this.onNode == null ? 0 : Convert.ToInt32(this.onNode.Value);
				}

				set
				{

					if (value == 0)
					{

						if (this.onNode != null)
							Remove(this.onNode);
					}
					else
					{
						if (this.onNode == null)
							Add(this.onNode);
						this.onNode.Value = Convert.ToString(value);
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.Effect:

						this.effectNode = node as AttributeNode;
						break;

					case Token.Background:

						this.backgroundNode = node as AttributeNode;
						break;

					case Token.Foreground:

						this.foregroundNode = node as AttributeNode;
						break;

					case Token.Repeat:

						this.repeatNode = node as AttributeNode;
						break;

					case Token.On:

						this.onNode = node as AttributeNode;
						break;

					case Token.Off:

						this.offNode = node as AttributeNode;
						break;

					case Token.Up:

						this.upNode = node as AttributeNode;
						break;

					case Token.Down:

						this.downNode = node as AttributeNode;
						break;

					case Token.Same:

						this.sameNode = node as AttributeNode;
						break;

					case Token.Steps:

						this.stepsNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.Effect:

						this.effectNode = null;
						break;

					case Token.Background:

						this.backgroundNode = null;
						break;

					case Token.Foreground:

						this.foregroundNode = null;
						break;

					case Token.Repeat:

						this.repeatNode = null;
						break;

					case Token.On:

						this.onNode = null;
						break;

					case Token.Off:

						this.offNode = null;
						break;

					case Token.Up:

						this.upNode = null;
						break;

					case Token.Down:

						this.downNode = null;
						break;

					case Token.Same:

						this.sameNode = null;
						break;

					case Token.Steps:

						this.stepsNode = null;
						break;

					}

				}

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				AnimationNode animationNode = new AnimationNode();
				foreach (Node childNode in this)
					animationNode.Add(childNode.Clone());
				return animationNode;

			}

		}

		/// <summary>
		/// A FontBrush describes how a tile is outlined.
		/// </summary>
		public class FontBrushNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode colorNode;

			/// <summary>
			/// Initialize a border.
			/// </summary>
			public FontBrushNode() : base(Token.FontBrush) { }

			/// <summary>
			/// Initialize a border.
			/// </summary>
			/// <param name="color">The color of the border.</param>
			/// <param name="weight">The thickness of the border.</param>
			public FontBrushNode(Color color)
				: base(Token.FontBrush)
			{

				// Initialize the object.
				this.Color = color;

			}

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultDocument.ForeColor : ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.colorNode != null)
							Remove(this.colorNode);
					}
					else
					{
						if (this.colorNode == null)
							Add(new AttributeNode(Token.Color));
						this.colorNode.Value = ColorTranslator.ToHtml(value);
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.Color:

						this.colorNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				FontBrushNode fontBrushNode = new FontBrushNode();
				foreach (Node childNode in this)
					fontBrushNode.Add(childNode.Clone());
				return fontBrushNode;

			}

		}

		/// <summary>
		/// Describes the kind of font and the color use to display text.
		/// </summary>
		internal class FontNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode boldNode;
			private MarkThree.Forms.AttributeNode italicNode;
			private MarkThree.Forms.AttributeNode underlineNode;
			private MarkThree.Forms.AttributeNode strikeoutNode;
			private MarkThree.Forms.AttributeNode sizeNode;
			private MarkThree.Forms.AttributeNode nameNode;

			/// <summary>
			/// Initializes a Font object.
			/// </summary>
			public FontNode() : base(Token.Font) { }

			public System.Drawing.FontFamily FontFamily
			{

				get
				{

					return this.nameNode == null ? DefaultDocument.FontFamily : new FontFamily(this.nameNode.Value);

				}

				set
				{

					if (value == DefaultDocument.FontFamily)
					{
						if (this.nameNode != null)
							Remove(this.nameNode);
					}
					else
					{
						if (this.nameNode == null)
							Add(new AttributeNode(Token.FontName));
						this.nameNode.Value = value.Name;
					}

				}

			}

			public System.Drawing.FontStyle FontStyle
			{

				get
				{

					FontStyle fontStyle = DefaultDocument.FontStyle;

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

					if ((value & FontStyle.Bold) == (DefaultDocument.FontStyle & FontStyle.Bold))
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

					if ((value & FontStyle.Italic) == (DefaultDocument.FontStyle & FontStyle.Italic))
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

					if ((value & FontStyle.Underline) == (DefaultDocument.FontStyle & FontStyle.Underline))
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

					if ((value & FontStyle.Strikeout) == (DefaultDocument.FontStyle & FontStyle.Strikeout))
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

			public System.Single Size
			{

				get
				{
					return this.sizeNode == null ? DefaultDocument.FontSize : Convert.ToSingle(this.sizeNode.Value);
				}

				set
				{

					if (value == DefaultDocument.FontSize)
					{
						if (this.sizeNode != null)
							Remove(this.sizeNode);
					}
					else
					{

						if (this.sizeNode == null)
							Add(new AttributeNode(Token.Size));
						this.sizeNode.Value = Convert.ToString(value);

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

					case Token.FontName:

						this.nameNode = attributeNode;
						break;

					case Token.Size:

						this.sizeNode = attributeNode;
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

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				FontNode fontNode = new FontNode();
				foreach (Node childNode in this)
					fontNode.Add(childNode.Clone());
				return fontNode;

			}

		}

		/// <summary>
		/// Describes how to draw the interior area of a tile.
		/// </summary>
		internal class ImageNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode variableNameNode;

			/// <summary>
			/// Initializes an Interior Node.
			/// </summary>
			public ImageNode() : base(Token.Image) { }

			public object Image
			{

				get
				{
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					return this.variableNameNode == null ? null : dataTransform.Variables[this.variableNameNode.Value];
				}

			}

			public string VariableName
			{

				get
				{
					return this.variableNameNode == null ? string.Empty : this.variableNameNode.Value;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.variableNameNode != null)
							Remove(this.variableNameNode);
					}
					else
					{

						if (this.variableNameNode == null)
							Add(new AttributeNode(Token.VariableName));
						this.variableNameNode.Value = value;
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{
					switch (node.token)
					{

					case Token.VariableName:

						this.variableNameNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ImageNode imageNode = new ImageNode();
				foreach (Node childNode in this)
					imageNode.Add(childNode.Clone());
				return imageNode;

			}

		}

		/// <summary>
		/// Describes how to draw the interior area of a tile.
		/// </summary>
		internal class LockNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.TextNode lockNode;

			/// <summary>
			/// Initializes an Interior Node.
			/// </summary>
			public LockNode() : base(Token.Lock) { }

			public override Node Clone()
			{

				LockNode lockNode = new LockNode();
				foreach (Node childNode in this)
					lockNode.Add(childNode.Clone());
				return lockNode;

			}

			public string Lock
			{

				get
				{
					return this.lockNode == null ? string.Empty : this.lockNode.text;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.lockNode != null)
							Remove(this.lockNode);
					}
					else
					{
						if (this.lockNode == null)
							Add(new TextNode(value));
						else
							this.lockNode.text = value;
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is TextNode)
					this.lockNode = node as TextNode;

				base.Add(node);

			}

		}

		/// <summary>
		/// Describes how to draw the interior area of a tile.
		/// </summary>
		internal class InteriorBrushNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode colorNode;

			/// <summary>
			/// Initializes an Interior Node.
			/// </summary>
			public InteriorBrushNode() : base(Token.InteriorBrush) { }

			public System.Drawing.Color Color
			{

				get
				{
					return this.colorNode == null ? DefaultDocument.BackColor :
						this.colorNode.Value.StartsWith("KnownColor.") ?
						Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), this.colorNode.Value.Replace("KnownColor.", string.Empty))) :
						ColorTranslator.FromHtml(this.colorNode.Value);
				}

				set
				{

					if (value == DefaultDocument.BackColor)
					{
						if (this.colorNode != null)
							Remove(this.colorNode);
					}
					else
					{
						if (this.colorNode == null)
							Add(new AttributeNode(Token.Color));
						this.colorNode.Value = ColorTranslator.ToHtml(value);
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

					case Token.Color:

						this.colorNode = attributeNode;
						break;

					}

				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				InteriorBrushNode interiorBrushNode = new InteriorBrushNode();
				foreach (Node childNode in this)
					interiorBrushNode.Add(childNode.Clone());
				return interiorBrushNode;

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
			public NumberFormatNode()
				: base(Token.NumberFormat)
			{

				// Initialize the object with defaults.
				this.formatNode = null;

			}

			public System.String Format
			{

				get
				{

					return this.formatNode == null ? DefaultDocument.Format :
						this.formatNode.Value == "General" ? "{0:}" : "{0:" + this.formatNode.Value + "}";

				}

				set
				{

					if (value == DefaultDocument.Format)
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

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				NumberFormatNode numberFormatNode = new NumberFormatNode();
				foreach (Node childNode in this)
					numberFormatNode.Add(childNode.Clone());
				return numberFormatNode;

			}

		}

		/// <summary>
		/// Protection is used to prevent editing in a tile.
		/// </summary>
		internal class ProtectionNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode isProtectedNode;

			/// <summary>
			/// Initialize the Protection object.
			/// </summary>
			public ProtectionNode()
				: base(Token.Protection)
			{

				// Initialize the object from the defaults.
				this.IsProtected = DefaultDocument.IsProtected;

			}

			public System.Boolean IsProtected
			{

				get
				{
					return this.isProtectedNode == null ? DefaultDocument.IsProtected :
						Convert.ToBoolean(this.isProtectedNode.Value);
				}

				set
				{

					if (value == DefaultDocument.IsProtected)
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

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ProtectionNode protectionNode = new ProtectionNode();
				foreach (Node childNode in this)
					protectionNode.Add(childNode.Clone());
				return protectionNode;

			}

		}

		/// <summary>
		/// A View describes the visible columns in the document.
		/// </summary>
		public class ViewNode : ElementNode
		{

			public ViewNode() : base(Token.View) { }

			public new DataTransform.ColumnReferenceNode this[int index]
			{

				get { return base[index] as DataTransform.ColumnReferenceNode; }

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ViewNode viewNode = new ViewNode();
				foreach (Node childNode in this)
					viewNode.Add(childNode.Clone());
				return viewNode;

			}

		}

		/// <summary>
		/// A View describes the visible columns in the document.
		/// </summary>
		public class StyleIdNode : ElementNode
		{

			private TextNode textNode;

			public StyleIdNode() : base(Token.StyleId) { }

			public override void Add(Node node)
			{

				if (node is TextNode)
					this.textNode = node as TextNode;

				base.Add(node);
			}

			public string Text
			{

				get { return this.textNode == null ? string.Empty : this.textNode.text; }
				set
				{

					if (value == string.Empty)
					{
						if (this.textNode != null)
							Remove(this.textNode);
					}
					else
					{
						this.textNode = new TextNode(value);
					}
				}

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				StyleIdNode styleIdNode = new StyleIdNode();
				foreach (Node childNode in this)
					styleIdNode.Add(childNode.Clone());
				return styleIdNode;

			}

		}

		/// <summary>
		/// The attributes of a style that determine how the spreadsheet is drawn on the output device.
		/// </summary>
		public class StyleNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.DataTransform.StringFormatNode stringFormatNode;
			private MarkThree.Forms.DataTransform.BottomBorderNode bottomBorderNode;
			private MarkThree.Forms.DataTransform.LeftBorderNode leftBorderNode;
			private MarkThree.Forms.DataTransform.RightBorderNode rightBorderNode;
			private MarkThree.Forms.DataTransform.TopBorderNode topBorderNode;
			private MarkThree.Forms.DataTransform.FontBrushNode fontBrushNode;
			private MarkThree.Forms.DataTransform.FontNode fontNode;
			private MarkThree.Forms.DataTransform.ImageNode imageNode;
			private MarkThree.Forms.DataTransform.InteriorBrushNode interiorNode;
			private MarkThree.Forms.DataTransform.NumberFormatNode numberFormatNode;
			private MarkThree.Forms.DataTransform.ProtectionNode protectionNode;
			private MarkThree.Forms.DataTransform.AnimationNode animationNode;
			private MarkThree.Forms.AttributeNode styleIdNode;
			private MarkThree.DualList<string, StyleNode> childStyles;

			// Internal Members
			internal MarkThree.Forms.AttributeNode parentIdNode;
			internal MarkThree.Forms.DataTransform.StyleNode parentStyleNode;

			/// <summary>
			/// Create a Style node from the input XML.
			/// </summary>
			/// <param name="xmlTokenReader">Represents a fast, non-cached, forward only access to XML Data.</param>
			/// <param name="tokenTable">A table of values that represent lexical elements of the XSL DataTransform.</param>
			public StyleNode()
				: base(Token.Style)
			{

				// Initialize the object
				this.childStyles = new DualList<string, StyleNode>();

			}

			/// <summary>Gets the Identifier of the Style.</summary>
			public string StyleId
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

				if (node is AnimationNode)
					this.animationNode = node as AnimationNode;

				if (node is StringFormatNode)
					this.stringFormatNode = node as StringFormatNode;

				if (node is BottomBorderNode)
					this.bottomBorderNode = node as BottomBorderNode;

				if (node is LeftBorderNode)
					this.leftBorderNode = node as LeftBorderNode;

				if (node is RightBorderNode)
					this.rightBorderNode = node as RightBorderNode;

				if (node is TopBorderNode)
					this.topBorderNode = node as TopBorderNode;

				if (node is FontNode)
					this.fontNode = node as FontNode;

				if (node is FontBrushNode)
					this.fontBrushNode = node as FontBrushNode;

				if (node is InteriorBrushNode)
					this.interiorNode = node as InteriorBrushNode;

				if (node is ImageNode)
					this.imageNode = node as ImageNode;

				if (node is NumberFormatNode)
					this.numberFormatNode = node as NumberFormatNode;

				if (node is ProtectionNode)
					this.protectionNode = node as ProtectionNode;

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
				{

					AttributeNode attributeNode = (AttributeNode)node;
					switch (attributeNode.token)
					{

					case Token.StyleId:

						this.styleIdNode = null;
						break;

					case Token.Parent:

						this.parentIdNode = null;
						break;

					}

				}

				if (node is AnimationNode)
					this.animationNode = null;

				if (node is StringFormatNode)
					this.stringFormatNode = null;

				if (node is BottomBorderNode)
					this.bottomBorderNode = null;

				if (node is LeftBorderNode)
					this.leftBorderNode = null;

				if (node is RightBorderNode)
					this.rightBorderNode = null;

				if (node is TopBorderNode)
					this.topBorderNode = null;

				if (node is FontNode)
					this.fontNode = null;

				if (node is FontBrushNode)
					this.fontBrushNode = null;

				if (node is InteriorBrushNode)
					this.interiorNode = null;

				if (node is ImageNode)
					this.imageNode = null;

				if (node is NumberFormatNode)
					this.numberFormatNode = null;

				if (node is ProtectionNode)
					this.protectionNode = null;

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				StyleNode styleNode = new StyleNode();
				foreach (Node childNode in this)
					styleNode.Add(childNode.Clone());
				return styleNode;

			}

			public bool HasAnimation { get { return this.animationNode != null; } }

			public bool HasStringFormat { get { return this.stringFormatNode != null; } }

			public bool HasBottomBorder { get { return this.bottomBorderNode != null; } }

			public bool HasLeftBorder { get { return this.leftBorderNode != null; } }

			public bool HasRightBorder { get { return this.rightBorderNode != null; } }

			public bool HasTopBorder { get { return this.topBorderNode != null; } }

			public bool IsFontNull { get { return this.fontNode != null; } }

			public bool HasImage { get { return this.imageNode != null; } }

			public bool HasFont { get { return this.fontNode != null; } }

			public bool HasFontBrush { get { return this.fontBrushNode != null; } }

			public bool HasInterior { get { return this.interiorNode != null; } }

			public bool HasNumberFormat { get { return this.numberFormatNode != null; } }

			public bool IsProtectionNull { get { return this.protectionNode != null; } }

			/// <summary>
			/// Gets or sets whether editing is allowed in this tile.
			/// </summary>
			public bool IsProtected
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.protectionNode == null ? DefaultDocument.IsProtected : this.protectionNode.IsProtected; }

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
			public DualList<string, StyleNode> GetStyles() { return this.childStyles; }

			/// <summary>
			/// Gets or sets the Parent in the Style hierarchy.
			/// </summary>
			public StyleNode ParentStyle
			{

				// This is the parent of this Style.
				get { return this.parentStyleNode; }

				set
				{

					// Remove the style from the parent hierarchy (if it had a parent) and add it to the children of the specified
					// style.
					if (this.parentStyleNode != null)
						this.parentStyleNode.GetStyles().Remove(this.StyleId);
					this.parentStyleNode = value;
					if (this.parentStyleNode != null)
						this.parentStyleNode.GetStyles().Add(this.StyleId, this);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the animation effects for a style.
			/// </summary>
			public AnimationNode Animation
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.animationNode == null ? new AnimationNode() : this.animationNode; }

				set
				{

					if (this.animationNode == null)
						Remove(this.animationNode);
					Add(value);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the string used to format numbers (and text) for a person to read.
			/// </summary>
			public string NumberFormat
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.numberFormatNode == null ? DefaultDocument.Format : this.numberFormatNode.Format; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.numberFormatNode == null)
						this.Add(new NumberFormatNode());
					this.numberFormatNode.Format = value;

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets how the contents of a tile are aligned and clipped within the tile.
			/// </summary>
			public StringFormat StringFormat
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.stringFormatNode == null ? null : this.stringFormatNode.StringFormat; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.stringFormatNode == null)
						this.Add(new StringFormatNode());
					this.stringFormatNode.StringFormat = value;

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the font used to draw text.
			/// </summary>
			public System.Drawing.Font Font
			{

				get
				{

					// Return a default font when there is no setting.  Otherwise, return a font with the attributes stored in the
					// 'Font' element.
					return this.fontNode == null ? new System.Drawing.Font(DefaultDocument.FontFamily, DefaultDocument.FontSize,
						 DefaultDocument.FontStyle) : new System.Drawing.Font(this.fontNode.FontFamily, this.fontNode.Size, this.fontNode.FontStyle);

				}

				set
				{

					if (value.FontFamily.Name == DefaultDocument.FontFamily.Name && value.Size == DefaultDocument.FontSize &&
						value.Style == DefaultDocument.FontStyle)
					{
						if (this.fontNode != null)
							Remove(this.fontNode);
					}
					else
					{
						if (this.fontNode == null)
							this.Add(new FontNode());
						this.fontNode.FontFamily = value.FontFamily;
						this.fontNode.Size = value.Size;
						this.fontNode.FontStyle = value.Style;
					}

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the text.
			/// </summary>
			public Color ForeColor
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.fontBrushNode == null ? DefaultDocument.ForeColor : this.fontBrushNode.Color; }

				set
				{

					if (value == DefaultDocument.ForeColor)
					{
						if (this.fontBrushNode != null)
							Remove(this.fontBrushNode);
					}
					else
					{
						if (this.fontBrushNode == null)
							this.Add(new FontBrushNode());
						this.fontBrushNode.Color = value;
					}

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the name of the variable that provides an image for the style.
			/// </summary>
			public string Image
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.imageNode == null ? string.Empty : this.imageNode.VariableName; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (value == string.Empty)
					{
						if (this.imageNode != null)
							Remove(this.imageNode);
					}
					else
					{
						if (this.imageNode == null)
							Add(new ImageNode());
						imageNode.VariableName = value;
					}

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a tile.
			/// </summary>
			public BottomBorderNode BottomBorder
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.bottomBorderNode == null ? new BottomBorderNode(DefaultDocument.BorderColor, DefaultDocument.BorderWidth) : this.bottomBorderNode; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.bottomBorderNode == null)
						Remove(this.bottomBorderNode);
					Add(value);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a tile.
			/// </summary>
			public LeftBorderNode LeftBorder
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.leftBorderNode == null ? new LeftBorderNode(DefaultDocument.BorderColor, DefaultDocument.BorderWidth) : this.leftBorderNode; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.leftBorderNode == null)
						Remove(this.leftBorderNode);
					Add(value);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a tile.
			/// </summary>
			public RightBorderNode RightBorder
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.rightBorderNode == null ? new RightBorderNode(DefaultDocument.BorderColor, DefaultDocument.BorderWidth) : this.rightBorderNode; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.rightBorderNode == null)
						Remove(this.rightBorderNode);
					Add(value);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a tile.
			/// </summary>
			public TopBorderNode TopBorder
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.topBorderNode == null ? new TopBorderNode(DefaultDocument.BorderColor, DefaultDocument.BorderWidth) : this.topBorderNode; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.topBorderNode == null)
						Remove(this.topBorderNode);
					Add(value);

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			/// <summary>
			/// Gets or sets the color used to paint the background of a tile.
			/// </summary>
			public Color BackColor
			{

				// The only tricky part is returning a default when there is no attribute defined.
				get { return this.interiorNode == null ? DefaultDocument.BackColor : this.interiorNode.Color; }

				set
				{

					// The only tricky part is making sure the attribute exists before setting the property.
					if (this.interiorNode == null)
						this.Add(new InteriorBrushNode());
					this.interiorNode.Color = value;

					// This will fire off an event that will indicate to any listeners that the dataTransform has been modified.
					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					dataTransform.OnChanged(this);

				}

			}

			#region IComparable Members

			public int CompareTo(object obj)
			{

				if (obj is StyleNode)
					return this.StyleId.CompareTo(((StyleNode)obj).StyleId);

				if (obj is string)
					return this.StyleId.CompareTo((string)obj);

				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(), obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A collection of Lock elements.
		/// </summary>
		public class LocksNode : ElementNode
		{

			public LocksNode() : base(Token.Locks) { }

			public override Node Clone()
			{

				LocksNode locksNode = new LocksNode();
				foreach (Node childNode in this)
					locksNode.Add(childNode.Clone());
				return locksNode;

			}

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
							styleNode.parentStyleNode.GetStyles().Add(styleNode.StyleId, styleNode);
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
						styleNode.parentStyleNode.GetStyles().Remove(styleNode.StyleId);
						styleNode.parentStyleNode = null;
					}

				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				StylesNode stylesNode = new StylesNode();
				foreach (Node childNode in this)
					stylesNode.Add(childNode.Clone());
				return stylesNode;

			}

		}

		/// <summary>
		/// A Node that contains a collection of Columns.
		/// </summary>
		public class ColumnsNode : SortedElementList
		{

			public ColumnsNode() : base(Token.Columns) { }

			public new ColumnNode Find(object identifier) { return (ColumnNode)base.Find(identifier); }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ColumnsNode columnsNode = new ColumnsNode();
				foreach (Node childNode in this)
					columnsNode.Add(childNode.Clone());
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
			private MarkThree.Forms.AttributeNode sortTemplateNode;
			private MarkThree.Forms.AttributeNode sortRowNode;
			private MarkThree.Forms.AttributeNode sortApplyTemplateNode;
			private MarkThree.Forms.AttributeNode visibleNode;

			/// <summary>
			/// Initialize a Column.
			/// </summary>
			public ColumnNode() : base(Token.Column) { }

			public bool IsVisible
			{

				get { return this.visibleNode == null ? true : Convert.ToBoolean(this.visibleNode.Value); }

				set
				{

					if (value)
					{
						if (this.visibleNode != null)
							this.Remove(visibleNode);
					}
					else
					{
						if (this.visibleNode == null)
							Add(new AttributeNode(Token.Visible));
						this.visibleNode.Value = Convert.ToString(value);
					}

				}

			}

			public string ColumnId
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
					return this.widthNode == null ? DefaultDocument.ColumnWidth : Convert.ToSingle(this.widthNode.Value);
				}

				set
				{

					if (value == DefaultDocument.ColumnWidth)
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

			public string Description
			{

				get { return this.descriptionNode == null ? string.Empty : this.descriptionNode.Value; }

				set
				{

					if (value == string.Empty)
					{
						if (this.descriptionNode != null)
							Remove(this.descriptionNode);
					}
					else
					{
						if (this.descriptionNode == null)
							Add(new AttributeNode(Token.Description));
						this.descriptionNode.Value = value;
					}

				}

			}

			public string SortApplyTemplate
			{

				get { return this.sortApplyTemplateNode == null ? string.Empty : this.sortApplyTemplateNode.Value; }

				set
				{

					if (value == string.Empty)
					{
						if (this.sortApplyTemplateNode != null)
							Remove(this.sortApplyTemplateNode);
					}
					else
					{
						if (this.sortApplyTemplateNode == null)
							Add(new AttributeNode(Token.SortApplyTemplate));
						this.sortApplyTemplateNode.Value = value;
					}

				}

			}

			public string SortRow
			{

				get { return this.sortRowNode == null ? string.Empty : this.sortRowNode.Value; }

				set
				{

					if (value == string.Empty)
					{
						if (this.sortRowNode != null)
							Remove(this.sortRowNode);
					}
					else
					{
						if (this.sortRowNode == null)
							Add(new AttributeNode(Token.SortRow));
						this.sortRowNode.Value = value;
					}

				}

			}

			public string SortTemplate
			{

				get { return this.sortTemplateNode == null ? string.Empty : this.sortTemplateNode.Value; }

				set
				{

					if (value == string.Empty)
					{
						if (this.sortTemplateNode != null)
							Remove(this.sortTemplateNode);
					}
					else
					{
						if (this.sortTemplateNode == null)
							Add(new AttributeNode(Token.SortTemplate));
						this.sortTemplateNode.Value = value;
					}

				}

			}

			/// <summary>A descriptive text of the object.</summary>
			public override string ToString() { return this.Description.Replace("\n", " "); }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{

					case Token.ColumnId:

						this.identifierNode = node as AttributeNode;
						break;

					case Token.Width:

						this.widthNode = node as AttributeNode;
						break;

					case Token.Description:

						this.descriptionNode = node as AttributeNode;
						break;

					case Token.SortTemplate:

						this.sortTemplateNode = node as AttributeNode;
						break;

					case Token.SortRow:

						this.sortRowNode = node as AttributeNode;
						break;

					case Token.SortApplyTemplate:

						this.sortApplyTemplateNode = node as AttributeNode;
						break;

					case Token.Visible:

						this.visibleNode = node as AttributeNode;
						break;

					}

				}

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ColumnNode columnNode = new ColumnNode();
				foreach (Node childNode in this)
					columnNode.Add(childNode.Clone());
				return columnNode;

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
					return this.ColumnId.CompareTo(((ColumnNode)obj).ColumnId);

				// Compare this object aginst a string.
				if (obj is string)
					return this.ColumnId.CompareTo((string)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(),
					obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A Column used to declare a unique index or a rule.
		/// </summary>
		public class ColumnReferenceNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode columnIdNode;

			public ColumnReferenceNode() : base(Token.ColumnReference) { }

			public string ColumnId
			{

				get { return this.columnIdNode == null ? string.Empty : this.columnIdNode.Value; }

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

			public MarkThree.Forms.DataTransform.ColumnNode Column
			{

				get
				{

					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					return dataTransform.Columns.Find(this.columnIdNode.Value);

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

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ColumnReferenceNode columnReferenceNode = new ColumnReferenceNode();
				foreach (Node childNode in this)
					columnReferenceNode.Add(childNode.Clone());
				return columnReferenceNode;

			}

		}

		/// <summary>
		/// Describes how the data is to be ordered in the viewer.
		/// </summary>
		public class SortsNode : SortedElementList
		{

			public SortsNode() : base(Token.Sorts) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				SortsNode sortsNode = new SortsNode();
				foreach (Node childNode in this)
					sortsNode.Add(childNode.Clone());
				return sortsNode;

			}

		}

		/// <summary>
		/// Used to define the sort order of the information displayed in the viewer.
		/// </summary>
		public class SortNode : ElementNode, IComparable
		{

			// Private Members
			private MarkThree.Forms.AttributeNode orderNode;
			private MarkThree.Forms.AttributeNode selectNode;

			/// <summary>
			/// Initialize the description of the sort order.
			/// </summary>
			public SortNode() : base(Token.Sort) { }

			/// <summary>
			/// Initialize the description of the sort order.
			/// </summary>
			public SortNode(ColumnNode column, SortOrder sortDirection)
				: base(Token.SortColumn)
			{

				// Initialize the object
				this.Column = column;
				this.Direction = sortDirection;

			}

			public MarkThree.Forms.DataTransform.ColumnNode Column
			{

				get
				{

					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					return dataTransform.Columns.Find(this.selectNode.Value);

				}

				set
				{

					if (value == null)
					{
						if (this.selectNode != null)
							Remove(this.selectNode);
					}
					else
					{
						if (this.selectNode == null)
							Add(new AttributeNode(Token.Select));
						this.selectNode.Value = value.ColumnId;
					}

				}

			}

			public MarkThree.Forms.SortOrder Direction
			{

				get
				{
					return this.orderNode == null ? SortOrder.Ascending :
						(SortOrder)Enum.Parse(typeof(SortOrder), this.orderNode.Value, true);
				}

				set
				{

					if (value == SortOrder.Ascending)
					{
						if (this.orderNode != null)
							Remove(this.orderNode);
					}
					else
					{
						if (this.orderNode == null)
							Add(new AttributeNode(Token.Order));
						this.orderNode.Value = value.ToString();
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
					case Token.Select:

						this.selectNode = attributeNode;
						break;

					case Token.Order:

						this.orderNode = attributeNode;
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
					case Token.Select:

						this.selectNode = null;
						break;

					case Token.Order:

						this.orderNode = null;
						break;

					}

				}

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				SortNode sortNode = new SortNode();
				foreach (Node childNode in this)
					sortNode.Add(childNode.Clone());
				return sortNode;

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
				if (obj is SortNode)
					return this.Column.ColumnId.CompareTo(((SortNode)obj).Column.ColumnId);

				// Compare this object against a string.
				if (obj is string)
					return this.Column.ColumnId.CompareTo((string)obj);

				// All other comparisons result in an exception.
				throw new Exception(string.Format("Can't compare a {0} to a {1}", this.GetType().ToString(), obj.GetType().ToString()));

			}

			#endregion

		}

		/// <summary>
		/// A Source is used to incrementally update the spreadsheet.
		/// </summary>
		internal class SourceNode : ElementNode
		{

			public SourceNode() : base(Token.Source) { }

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				SourceNode sourceNode = new SourceNode();
				foreach (Node childNode in this)
					sourceNode.Add(childNode.Clone());
				return sourceNode;

			}

		}

		/// <summary>
		/// A Source is used to incrementally update the spreadsheet.
		/// </summary>
		internal class ScaleNode : ElementNode
		{

			private AttributeNode factorNode;

			public ScaleNode() : base(Token.Scale) { }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Factor:

						this.factorNode = node as AttributeNode;
						break;

					}

				base.Add(node);

			}

			public float Factor
			{

				get { return this.factorNode == null ? DefaultDocument.ScaleFactor : Convert.ToSingle(this.factorNode.Value); }
				set
				{

					if (value == DefaultDocument.ScaleFactor)
					{
						if (this.factorNode != null)
							Remove(this.factorNode);
					}
					else
					{
						if (this.factorNode == null)
							Add(new AttributeNode(Token.Factor));
						this.factorNode.Value = Convert.ToString(value);
					}

				}

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ScaleNode scaleNode = new ScaleNode();
				foreach (Node childNode in this)
					scaleNode.Add(childNode.Clone());
				return scaleNode;

			}

		}

		/// <summary>
		/// A Source is used to incrementally update the spreadsheet.
		/// </summary>
		internal class SplitNode : ElementNode
		{

			// Private Members
			private AttributeNode widthNode;
			private AttributeNode heightNode;

			public SplitNode() : base(Token.Split) { }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Width:

						this.widthNode = node as AttributeNode;
						break;

					case Token.Height:

						this.heightNode = node as AttributeNode;
						break;

					}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Width:

						this.widthNode = null;
						break;

					case Token.Height:

						this.heightNode = null;
						break;

					}

				base.Remove(node);

			}

			public override Node Clone()
			{

				SplitNode splitNode = new SplitNode();
				foreach (Node childNode in this)
					splitNode.Add(childNode.Clone());
				return splitNode;

			}

			public SizeF SizeF
			{

				get
				{
					float width = this.widthNode == null ? 0.0f : Convert.ToSingle(this.widthNode.Value);
					float height = this.heightNode == null ? 0.0f : Convert.ToSingle(this.heightNode.Value);
					return new SizeF(width, height);
				}

				set
				{

					if (value.Width == 0.0f)
					{
						if (this.widthNode != null)
							Remove(this.widthNode);
					}
					else
					{
						if (this.widthNode == null)
							Add(new AttributeNode(Token.Width));
						this.widthNode.Value = Convert.ToString(value.Width);
					}

					if (value.Height == 0.0f)
					{
						if (this.heightNode != null)
							Remove(this.heightNode);
					}
					else
					{
						if (this.heightNode == null)
							Add(new AttributeNode(Token.Height));
						this.heightNode.Value = Convert.ToString(value.Height);
					}

				}

			}
		
		}

		/// <summary>
		/// A Source is used to incrementally update the spreadsheet.
		/// </summary>
		public class VariableNode : ElementNode
		{

			private AttributeNode nameNode;
			private AttributeNode selectNode;

			public VariableNode() : base(Token.Variable) { }

			public override void Add(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Name:

						this.nameNode = node as AttributeNode;
						break;

					case Token.Select:

						this.selectNode = node as AttributeNode;
						break;

					}

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Name:

						this.nameNode = null;
						break;

					case Token.Select:

						this.selectNode = null;
						break;

					}

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				VariableNode variableNode = new VariableNode();
				foreach (Node childNode in this)
					variableNode.Add(childNode.Clone());
				return variableNode;

			}

			public System.String Name
			{

				get
				{
					return this.nameNode == null ? string.Empty : this.nameNode.Value;
				}

				set
				{

					if (value == string.Empty)
						Remove(this.nameNode);
					else
					{
						if (this.nameNode == null)
							Add(new AttributeNode(Token.Name));
						this.nameNode.Value = value;
					}

				}

			}

			public System.String Select
			{

				get
				{
					return this.selectNode == null ? string.Empty : this.selectNode.Value;
				}

				set
				{

					if (value == string.Empty)
						Remove(this.selectNode);
					else
					{
						if (this.selectNode == null)
							Add(new AttributeNode(Token.Select));
						this.selectNode.Value = value;
					}

				}

			}

		}

		/// <summary>
		/// A View describes the visible columns in the document.
		/// </summary>
		public class FilterNode : ElementNode
		{

			private TextNode textNode;

			public FilterNode() : base(Token.Filter) { }

			public override void Add(Node node)
			{

				if (node is TextNode)
					this.textNode = node as TextNode;

				base.Add(node);
			}

			public string Text
			{

				get { return this.textNode == null ? string.Empty : this.textNode.text; }
				set
				{

					if (value == string.Empty)
					{
						if (this.textNode != null)
							Remove(this.textNode);
					}
					else
					{
						this.textNode = new TextNode(value);
					}
				}

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				FilterNode filterNode = new FilterNode();
				foreach (Node childNode in this)
					filterNode.Add(childNode.Clone());
				return filterNode;

			}

		}

		/// <summary>
		/// A ApplyTemplate is used to incrementally update the spreadsheet.
		/// </summary>
		public class ApplyTemplateNode : ElementNode, IComparable<ApplyTemplateNode>
		{

			// Private Members
			private AttributeNode selectNode;
			private FilterNode rowFilterNode;

			// Public Read Only Members
			public readonly List<SortNode> Sorts;

			public ApplyTemplateNode()
				: base(Token.ApplyTemplate)
			{

				// Initialize the object.
				this.Sorts = new List<SortNode>();

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Select:

						this.selectNode = node as AttributeNode;
						break;

					}

				if (node is FilterNode)
					this.rowFilterNode = node as FilterNode;

				if (node is SortNode)
					this.Sorts.Add(node as SortNode);

				base.Add(node);

			}

			public override void Remove(Node node)
			{

				if (node is AttributeNode)
					switch (node.token)
					{

					case Token.Select:

						this.selectNode = null;
						break;

					}

				if (node is FilterNode)
					this.rowFilterNode = null;

				if (node is SortNode)
					this.Sorts.Remove(node as SortNode);

				base.Remove(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ApplyTemplateNode applyTemplateNode = new ApplyTemplateNode();
				foreach (Node childNode in this)
					applyTemplateNode.Add(childNode.Clone());
				return applyTemplateNode;

			}

			public System.String Select
			{

				get
				{
					return this.selectNode == null ? string.Empty : this.selectNode.Value;
				}

				set
				{

					if (value == string.Empty)
						Remove(this.selectNode);
					else
					{
						if (this.selectNode == null)
							Add(new AttributeNode(Token.Select));
						this.selectNode.Value = value;
					}

				}

			}

			public System.String RowFilter
			{

				get
				{
					return this.rowFilterNode == null ? string.Empty : this.rowFilterNode.Text;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.rowFilterNode != null)
							Remove(this.rowFilterNode);
					}
					else
					{
						if (this.rowFilterNode == null)
							Add(new FilterNode());
						this.rowFilterNode.Text = value;
					}

				}

			}

			#region IComparable<ApplyTemplateNode> Members

			public int CompareTo(ApplyTemplateNode other)
			{
				return this.Select.CompareTo(other.Select);
			}

			#endregion
		}

		/// <summary>
		/// A View describes the visible columns in the document.
		/// </summary>
		public class DataNode : ElementNode
		{

			// Private Members
			private TextNode textNode;
			private MarkThree.Forms.AttributeNode variableNameNode;

			public DataNode() : base(Token.Data) { }

			public string VariableName
			{

				get
				{
					return this.variableNameNode == null ? string.Empty : this.variableNameNode.Value;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.variableNameNode != null)
							Remove(this.variableNameNode);
					}
					else
					{

						if (this.variableNameNode == null)
							Add(new AttributeNode(Token.VariableName));
						this.variableNameNode.Value = value;
					}

				}

			}

			public string Text
			{

				get { return this.textNode == null ? string.Empty : this.textNode.text; }
				set
				{

					if (value == string.Empty)
					{
						if (this.textNode != null)
							Remove(this.textNode);
					}
					else
					{
						if (this.textNode == null)
							this.textNode = new TextNode(value);
						else
							this.textNode.text = value;
					}
				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{
					switch (node.token)
					{

					case Token.VariableName:

						this.variableNameNode = node as AttributeNode;
						break;

					}

				}

				if (node is TextNode)
					this.textNode = node as TextNode;

				base.Add(node);
			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				DataNode dataNode = new DataNode();
				foreach (Node childNode in this)
					dataNode.Add(childNode.Clone());
				return dataNode;

			}

		}

		/// <summary>
		/// A View describes the visible columns in the document.
		/// </summary>
		public class ScratchNode : ElementNode
		{

			private TextNode textNode;

			public ScratchNode() : base(Token.Scratch) { }

			public override void Add(Node node)
			{

				if (node is TextNode)
					this.textNode = node as TextNode;

				base.Add(node);
			}

			public string Text
			{

				get { return this.textNode == null ? string.Empty : this.textNode.text; }
				set
				{

					if (value == string.Empty)
					{
						if (this.textNode != null)
							Remove(this.textNode);
					}
					else
					{
						if (this.textNode == null)
							Add(new TextNode(value));
						else
							this.textNode.text = value;
					}
				}

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				ScratchNode scratchNode = new ScratchNode();
				foreach (Node childNode in this)
					scratchNode.Add(childNode.Clone());
				return scratchNode;

			}

		}

		/// <summary>
		/// A Node that contains a collection of Rows.
		/// </summary>
		public class RowsNode : SortedElementList
		{

			public RowsNode() : base(Token.Rows) { }

			public new RowNode Find(object identifier) { return (RowNode)base.Find(identifier); }

			public new RowNode this[int index] { get { return (RowNode)base[index]; } }

		}

		/// <summary>
		/// Contains the definition of a row of data in the viewer.
		/// </summary>
		public class RowNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode rowIdNode;
			private MarkThree.Forms.AttributeNode heightNode;

			// Public Read Only Members
			public readonly MarkThree.DualList<string, TileNode> Tiles;
			public readonly MarkThree.DualList<string, ApplyTemplateNode> ApplyTemplates;
			public readonly System.Collections.Generic.List<ScratchNode> ScratchList;

			public RowNode()
				: base(Token.Row)
			{

				// Initialize the object
				this.ApplyTemplates = new DualList<string, ApplyTemplateNode>();
				this.Tiles = new DualList<string, TileNode>();
				this.ScratchList = new List<ScratchNode>();

			}

			public System.Single Height
			{

				get
				{
					return this.heightNode == null ? DefaultDocument.RowHeight : Convert.ToSingle(this.heightNode.Value);
				}

				set
				{

					if (value == DefaultDocument.RowHeight)
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

			public System.String RowId
			{

				get { return this.rowIdNode == null ? string.Empty : this.rowIdNode.Value; }

				set
				{

					if (value == string.Empty)
					{
						if (this.rowIdNode != null)
							Remove(this.rowIdNode);
					}
					else
					{
						if (this.rowIdNode == null)
							Add(new AttributeNode(Token.RowId));
						this.rowIdNode.Value = value;
					}

				}

			}

			public override void Add(Node node)
			{

				if (node is AttributeNode)
				{

					switch (node.token)
					{
					case Token.Height:

						this.heightNode = node as AttributeNode;
						break;

					case Token.RowId:

						this.rowIdNode = node as AttributeNode;
						break;

					}

				}

				if (node is ApplyTemplateNode)
				{
					ApplyTemplateNode applyTemplateNode = node as ApplyTemplateNode;
					this.ApplyTemplates.Add(applyTemplateNode.Select, applyTemplateNode);
				}

				if (node is TileNode)
				{
					TileNode tileNode = node as TileNode;
					this.Tiles.Add(tileNode.ColumnId, tileNode);
				}

				if (node is ScratchNode)
					this.ScratchList.Add(node as ScratchNode);

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				RowNode rowNode = new RowNode();
				foreach (Node childNode in this)
					rowNode.Add(childNode.Clone());
				return rowNode;

			}

		}

		/// <summary>
		/// A Tile is the smallest unit of displayable data in the viewer.
		/// </summary>
		public class TileNode : ElementNode
		{

			// Private Members
			private MarkThree.Forms.AttributeNode columnIdNode;
			private MarkThree.Forms.DataTransform.StyleIdNode styleIdNode;
			private MarkThree.Forms.DataTransform.DataNode dataNode;

			/// <summary>
			/// Initialize the smallest unit of data in the viewer.
			/// </summary>
			public TileNode() : base(Token.Tile) { }

			public string ColumnId
			{

				get
				{

					return this.columnIdNode == null ? string.Empty : this.columnIdNode.Value;

				}


				set
				{

					if (this.columnIdNode == null)
						Remove(this.columnIdNode);

					if (value != string.Empty)
						Add(new AttributeNode(Token.ColumnId, value));

				}

			}

			public MarkThree.Forms.DataTransform.ColumnNode Column
			{

				get
				{

					DataTransform dataTransform = this.TopLevelNode as DataTransform;
					return this.columnIdNode == null ? null : dataTransform.Columns.Find(this.columnIdNode.Value);

				}


				set
				{

					if (this.columnIdNode == null)
						Remove(this.columnIdNode);

					if (value != null)
						Add(new AttributeNode(Token.ColumnId, value.ColumnId));

				}

			}

			public System.String StyleId
			{

				get
				{
					return this.styleIdNode == null ? string.Empty : this.styleIdNode.Text;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.styleIdNode != null)
							Remove(this.styleIdNode);
					}
					else
					{
						if (this.styleIdNode == null)
							Add(new AttributeNode(Token.StyleId, value));
						else
							this.styleIdNode.Text = value;
					}

				}

			}

			public System.String Data
			{

				get
				{
					return this.dataNode == null ? string.Empty : this.dataNode.Text;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.dataNode != null)
							Remove(this.dataNode);
					}
					else
					{
						if (this.dataNode == null)
							Add(new AttributeNode(Token.Data, value));
						else
							this.dataNode.Text = value;
					}

				}

			}

			public System.String VariableName
			{

				get
				{
					return this.dataNode == null ? string.Empty : this.dataNode.VariableName;
				}

				set
				{

					if (value == string.Empty)
					{
						if (this.dataNode != null)
						{
							this.dataNode.VariableName = value;
							if (this.dataNode.Count == 0)
								Remove(this.dataNode);
						}
					}
					else
					{
						if (this.dataNode == null)
							Add(new AttributeNode(Token.Data));
						this.dataNode.VariableName = value;
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
					case Token.ColumnId:

						this.columnIdNode = attributeNode;
						break;

					}

				}

				if (node is StyleIdNode)
					this.styleIdNode = node as StyleIdNode;

				if (node is DataNode)
					this.dataNode = node as DataNode;

				base.Add(node);

			}

			/// <summary>
			/// Creates a copy of this node.
			/// </summary>
			/// <returns>A copy of this node.</returns>
			public override Node Clone()
			{

				// Add cloned copies of all the children to a copy of this node.
				TileNode tileNode = new TileNode();
				foreach (Node childNode in this)
					tileNode.Add(childNode.Clone());
				return tileNode;

			}

		}

		/// <summary>
		/// A DataTransform describes how the incoming XML data in transformed to a display device.
		/// </summary>
		public DataTransform() : base(Token.DataTransform)
		{

			this.Templates = new DualList<string, TemplateNode>();
			this.Variables = new DualList<string, VariableNode>();

			// This is an array of prohibited delegates for the event that signals a change to the dataTransform parameters.  During a
			// block of updates to the dataTransform, for example when the dataTransform is loaded form an XML source, several updates
			// can occur.  Calling the 'BeginLoad' at the start and the 'EndLoad' at the end of a block of operations will insure
			// that only a single event is generated.
			this.inhibitedChangedDelegates = null;

		}

		public override void Add(Node node)
		{

			if (node is ScaleNode)
				this.scaleNode = node as ScaleNode;

			if (node is SplitNode)
				this.splitNode = node as SplitNode;

			if (node is ColumnsNode)
				this.columnsNode = node as ColumnsNode;

			if (node is ViewNode)
				this.viewNode = node as ViewNode;

			if (node is StylesNode)
				this.stylesNode = node as StylesNode;

			if (node is LocksNode)
				this.locksNode = node as LocksNode;

			if (node is TemplateNode)
			{
				TemplateNode templateNode = node as TemplateNode;
				this.Templates.Add(templateNode.Match, templateNode);
			}

			if (node is VariableNode)
			{
				VariableNode variableNode = node as VariableNode;
				this.Variables.Add(variableNode.Name, variableNode);
			}

			if (node is AttributeNode)
			{

				switch (node.token)
				{
				case Token.TargetNamespace:

					this.targetNamespaceNode = node as AttributeNode;
					break;

				case Token.DataTransformId:

					this.dataTransformIdNode = node as AttributeNode;
					break;

				case Token.Source:

					this.sourceNode = node as AttributeNode;
					break;

				}

			}

			base.Add(node);

		}

		public System.String TargetNamespace
		{

			get
			{
				return this.targetNamespaceNode == null ? string.Empty : this.targetNamespaceNode.Value;
			}

			set
			{

				if (value == string.Empty)
					Remove(this.targetNamespaceNode);
				else
				{
					if (this.targetNamespaceNode == null)
						Add(new AttributeNode(Token.TargetNamespace));
					this.targetNamespaceNode.Value = value;
				}

			}

		}

		public System.String DataTransformId
		{

			get
			{
				return this.dataTransformIdNode == null ? string.Empty : this.dataTransformIdNode.Value;
			}

			set
			{

				if (value == string.Empty)
					Remove(this.dataTransformIdNode);
				else
				{
					if (this.dataTransformIdNode == null)
						Add(new AttributeNode(Token.DataTransformId));
					this.dataTransformIdNode.Value = value;
				}

			}

		}

		public System.String Source
		{

			get
			{
				return this.sourceNode == null ? string.Empty : this.sourceNode.Value;
			}

			set
			{

				if (value == string.Empty)
					Remove(this.sourceNode);
				else
				{
					if (this.sourceNode == null)
						Add(new AttributeNode(Token.Source));
					this.sourceNode.Value = value;
				}

			}

		}

		/// <summary>
		/// Finds the column based on its position in the view.
		/// </summary>
		/// <param name="left">The left edge of the column.</param>
		/// <returns>The column found at the given edge.</returns>
		public DataTransform.ColumnReferenceNode FindLeftEdge(float leftEdge)
		{

			// The left edge of the columns is not stored anywhere, so it is calculated here on the fly as each of the columns is
			// tested to see if it matches the given edge.
			float columnEdge = 0.0f;
			foreach (DataTransform.ColumnReferenceNode columnReferenceNode in this.viewNode)
			{
				if (leftEdge == columnEdge)
					return columnReferenceNode;
				columnEdge += columnReferenceNode.Column.Width;
			}

			// At this point, there is no column in the transform that matches the given edge.
			return null;

		}

		/// <summary>
		/// Finds the column based on its position in the view.
		/// </summary>
		/// <param name="left">The left edge of the column.</param>
		/// <returns>The column found at the given edge.</returns>
		public DataTransform.ColumnReferenceNode FindRightEdge(float rightEdge)
		{

			// The left edge of the columns is not stored anywhere, so it is calculated here on the fly as each of the columns is
			// tested to see if it matches the given edge.
			float columnEdge = 0.0f;
			foreach (DataTransform.ColumnReferenceNode columnReferenceNode in this.viewNode)
			{
				columnEdge += columnReferenceNode.Column.Width;
				if (rightEdge == columnEdge)
					return columnReferenceNode;
			}

			// At this point, there is no column in the transform that matches the given edge.
			return null;

		}

		/// <summary>
		/// Inhibits the 'ChangeEvent' until an 'EndLoad' method call.  This is used to prevent invokation of multiple events while doing
		/// multiple updates to the dataTransform.  When 'EndLoad' is called, a single event will be generated.
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
		/// Enables the events associated with changing the dataTransform.
		/// </summary>
		public void EndLoad()
		{

			// This insures thread-safety.
			lock (this)
			{

				// Re-install any of the events for changing the dataTransform values.
				this.Changed = (EventHandler)System.Delegate.Combine(this.inhibitedChangedDelegates);

				// Clearing the array of delegates also acts as an indicator that there is no prohibition against invoking the
				// events.
				this.inhibitedChangedDelegates = null;

				// If any events triggered during the prohibition, they will be invoked here.
				if (this.hasChangeEvent)
					OnChanged(this);

			}

		}

		public new DataTransform Clone()
		{

			DataTransform dataTransform = new DataTransform();
			foreach (Node node in this)
				dataTransform.Add(node.Clone());
			return dataTransform;

		}

		/// <summary>
		/// Indicates that a change has been made to the dataTransforms.
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

	}

}
