namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// A specification for the alignment and reading order.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerStringFormat : ViewerAttribute
	{

		// Public Members
		public StringFormat StringFormat;

		/// <summary>
		/// Construct a specification for the alignment and reading order of the style.
		/// </summary>
		public ViewerStringFormat()
		{

			// Initialize the object
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = DefaultDocument.Alignment;
			this.StringFormat.LineAlignment = DefaultDocument.LineAlignment;
			this.StringFormat.FormatFlags = DefaultDocument.StringFormatFlags;

		}

		/// <summary>
		/// Construct a specification for the alignment and reading order of the style.
		/// </summary>
		/// <param name="alignment">The horizontal alignment of the text in the cell.</param>
		public ViewerStringFormat(StringAlignment alignment)
		{

			// Initialize the object
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = alignment;
			this.StringFormat.LineAlignment = DefaultDocument.LineAlignment;
			this.StringFormat.FormatFlags = DefaultDocument.StringFormatFlags;

		}

		/// <summary>
		/// Construct a specification for the alignment and reading order of the style.
		/// </summary>
		/// <param name="alignment">The horizontal alignment of the text in the cell.</param>
		/// <param name="lineAlignment">The vertical alignment of the text in the cell.</param>
		public ViewerStringFormat(StringAlignment alignment, StringAlignment lineAlignment)
		{

			// Initialize the object
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = alignment;
			this.StringFormat.LineAlignment = lineAlignment;
			this.StringFormat.FormatFlags = DefaultDocument.StringFormatFlags;

		}

		/// <summary>
		/// Construct a specification for the alignment and reading order of the style.
		/// </summary>
		/// <param name="alignment">The horizontal alignment of the text in the cell.</param>
		/// <param name="lineAlignment">The vertical alignment of the text in the cell.</param>
		/// <param name="stringFormatFlags">The additional alignment flags such as Reading Order (Right to Left), etc.</param>
		public ViewerStringFormat(StringAlignment alignment, StringAlignment lineAlignment, StringFormatFlags stringFormatFlags)
		{

			// Initialize the object
			this.StringFormat = new StringFormat();
			this.StringFormat.Alignment = alignment;
			this.StringFormat.LineAlignment = lineAlignment;
			this.StringFormat.FormatFlags = stringFormatFlags;

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			ViewerStringFormat viewerStringFormat = new ViewerStringFormat();
			viewerStringFormat.StringFormat = this.StringFormat;
			return viewerStringFormat;

		}

		/// <summary>
		/// Determines if two ViewerStringFormat objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerStringFormat objects are compared.
			if (obj is ViewerStringFormat)
			{
				ViewerStringFormat viewerStringFormat = obj as ViewerStringFormat;
				return this.StringFormat.Alignment == viewerStringFormat.StringFormat.Alignment &&
					this.StringFormat.LineAlignment == viewerStringFormat.StringFormat.LineAlignment &&
					this.StringFormat.FormatFlags == viewerStringFormat.StringFormat.FormatFlags;
			}

			// This object isn't equivalent to any other types of object.
			return false;

		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A value that can be used for hashing algorithms.</returns>
		public override int GetHashCode()
		{

			// Use the constituent parts for calculating a hash code.
			return this.StringFormat.GetHashCode();

		}

	}

}
