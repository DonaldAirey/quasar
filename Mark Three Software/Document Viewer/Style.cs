namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>A Style describes how a bit of data is presented to a user.</summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Style
	{

		// Public Members
		public MarkThree.Forms.Animation Animation;
		public MarkThree.Forms.ViewerBorder BottomBorder;
		public MarkThree.Forms.ViewerBorder LeftBorder;
		public MarkThree.Forms.ViewerBorder RightBorder;
		public MarkThree.Forms.ViewerBorder TopBorder;
		public System.Boolean IsProtected;
		public System.Collections.Generic.List<ViewerStyle> ChildStyles;
		public System.Drawing.Brush InteriorBrush;
		public System.Drawing.Font Font;
		public System.Drawing.Image Image;
		public System.Drawing.Pen InteriorPen;
		public System.Drawing.SolidBrush FontBrush;
		public System.Drawing.StringFormat StringFormat;
		public System.String StyleId;
		public System.String ParentId;
		public System.String NumberFormat;

		public Style()
		{

			// Initialize the object
			this.StyleId = string.Empty;
			this.ParentId = string.Empty;
			this.Animation = null;
			this.IsProtected = DefaultDocument.IsProtected;
			this.InteriorBrush = new SolidBrush(DefaultDocument.BackColor);
			this.InteriorPen = new Pen(this.InteriorBrush, 1.0f);
			this.Font = new System.Drawing.Font(DefaultDocument.FontFamily, DefaultDocument.FontSize);
			this.BottomBorder = null;
			this.LeftBorder = null;
			this.RightBorder = null;
			this.TopBorder = null;
			this.FontBrush = new SolidBrush(DefaultDocument.ForeColor);
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = DefaultDocument.Alignment;
			this.StringFormat.LineAlignment = DefaultDocument.LineAlignment;
			this.StringFormat.FormatFlags = DefaultDocument.StringFormatFlags;
			this.NumberFormat = DefaultDocument.Format;

			// This is a list of all the children styles of this style.
			this.ChildStyles = new List<ViewerStyle>();

		}

		public Style(string styleId)
		{

			// Initialize the object
			this.StyleId = styleId;
			this.ParentId = string.Empty;
			this.Animation = null;
			this.IsProtected = DefaultDocument.IsProtected;
			this.InteriorBrush = new SolidBrush(DefaultDocument.BackColor);
			this.InteriorPen = new Pen(this.InteriorBrush, 1.0f);
			this.Font = new System.Drawing.Font(DefaultDocument.FontFamily, DefaultDocument.FontSize);
			this.BottomBorder = null;
			this.LeftBorder = null;
			this.RightBorder = null;
			this.TopBorder = null;
			this.FontBrush = new SolidBrush(DefaultDocument.ForeColor);
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = DefaultDocument.Alignment;
			this.StringFormat.LineAlignment = DefaultDocument.LineAlignment;
			this.StringFormat.FormatFlags = DefaultDocument.StringFormatFlags;
			this.NumberFormat = DefaultDocument.Format;

			// This is a list of all the children styles of this style.
			this.ChildStyles = new List<ViewerStyle>();

		}

		/// <summary>
		/// The parent style, or null, if there is no parent.
		/// </summary>
		public Style Parent
		{

			set
			{

				// When assigning a parent, the child style gets a copy off all the attributes of the parent style.  The can be
				// overridden with any subsequent attributes in the XML stream.
				this.Animation = value.Animation;
				this.ParentId = value.StyleId;
				this.IsProtected = value.IsProtected;
				this.InteriorBrush = value.InteriorBrush.Clone() as Brush;
				this.InteriorPen = value.InteriorPen.Clone() as Pen;
				this.Font = value.Font.Clone() as Font;
				this.BottomBorder = value.BottomBorder == null ? null : value.BottomBorder.Clone() as ViewerBorder;
				this.LeftBorder = value.LeftBorder == null ? null : value.LeftBorder.Clone() as ViewerBorder;
				this.RightBorder = value.RightBorder == null ? null : value.RightBorder.Clone() as ViewerBorder;
				this.TopBorder = value.TopBorder == null ? null : value.TopBorder.Clone() as ViewerBorder;
				this.FontBrush = value.FontBrush.Clone() as SolidBrush;
				this.StringFormat = value.StringFormat.Clone() as StringFormat;
				this.NumberFormat = value.NumberFormat.Clone() as string;

			}

		}

		public Style Clone()
		{

			Style style = new Style();
			style.Animation = this.Animation;
			style.StyleId = this.StyleId;
			style.ParentId = this.ParentId;
			style.IsProtected = this.IsProtected;
			style.Image = this.Image;
			style.InteriorBrush = this.InteriorBrush;
			style.InteriorPen = this.InteriorPen;
			style.Font = this.Font;
			style.BottomBorder = this.BottomBorder;
			style.LeftBorder = this.LeftBorder;
			style.RightBorder = this.RightBorder;
			style.TopBorder = this.TopBorder;
			style.FontBrush = this.FontBrush;
			style.StringFormat = this.StringFormat;
			style.NumberFormat = this.NumberFormat;
			style.ChildStyles = this.ChildStyles;
			return style;

		}

	}

}
