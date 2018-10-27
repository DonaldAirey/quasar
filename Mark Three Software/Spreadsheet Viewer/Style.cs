namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>A Style describes how a bit of data is presented to a user.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Style
	{

		// Private Members
		private System.String id;

		// Public Members
		private MarkThree.Forms.Style parent;
		public MarkThree.Forms.Style[] NilAnimation;
		public MarkThree.Forms.Style[] UpAnimation;
		public MarkThree.Forms.Style[] DownAnimation;
		public System.Boolean IsProtected;
		public System.Boolean IsAnimated;
		public System.Drawing.Brush InteriorBrush;
		public System.Drawing.Pen InteriorPen;
		public System.Drawing.Font Font;
		public System.Drawing.Pen BottomBorder;
		public System.Drawing.Pen LeftBorder;
		public System.Drawing.Pen RightBorder;
		public System.Drawing.Pen TopBorder;
		public System.Drawing.SolidBrush FontBrush;
		public System.Drawing.StringFormat StringFormat;
		public System.String NumberFormat;
		public System.Collections.Generic.List<Style> childStyles;

		public Style()
		{

			// Initialize the object
			this.id = null;
			this.parent = null;
			this.NilAnimation = null;
			this.UpAnimation = null;
			this.DownAnimation = null;
			this.IsAnimated = false;
			this.IsProtected = DefaultSpreadsheet.IsProtected;
			this.InteriorBrush = new SolidBrush(DefaultSpreadsheet.BackColor);
			this.InteriorPen = new Pen(this.InteriorBrush, 1.0f);
			this.Font = new System.Drawing.Font(DefaultSpreadsheet.FontName, DefaultSpreadsheet.FontSize);
			this.BottomBorder = null;
			this.LeftBorder = null;
			this.RightBorder = null;
			this.TopBorder = null;
			this.FontBrush = new SolidBrush(DefaultSpreadsheet.ForeColor);
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = DefaultSpreadsheet.Alignment;
			this.StringFormat.LineAlignment = DefaultSpreadsheet.LineAlignment;
			this.StringFormat.FormatFlags = DefaultSpreadsheet.FormatFlags;
			this.NumberFormat = DefaultSpreadsheet.Format;

			// This is a list of all the children styles of this style.
			this.childStyles = new List<Style>();

		}

		/// <summary>
		/// The unique identifier of this style.
		/// </summary>
		public string Id { get { return this.id; } set { this.id = value; } }

		/// <summary>
		/// The parent style, or null, if there is no parent.
		/// </summary>
		public Style Parent
		{

			get { return this.parent; }

			set
			{
				this.parent = value;

				// When assigning a parent, the child style gets a copy off all the attributes of the parent style.  The can be
				// overridden with any subsequent attributes in the XML stream.
				this.NilAnimation = null;
				this.UpAnimation = null;
				this.DownAnimation = null;
				this.IsAnimated = value.IsAnimated;
				this.IsProtected = value.IsProtected;
				this.InteriorBrush = value.InteriorBrush;
				this.InteriorPen = value.InteriorPen;
				this.Font = value.Font;
				this.BottomBorder = value.BottomBorder;
				this.LeftBorder = value.LeftBorder;
				this.RightBorder = value.RightBorder;
				this.TopBorder = value.TopBorder;
				this.FontBrush = value.FontBrush;
				this.StringFormat = value.StringFormat;
				this.NumberFormat = value.NumberFormat;

			}

		}

	}

}
