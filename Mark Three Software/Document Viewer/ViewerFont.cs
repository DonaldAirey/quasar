namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Specification for the font used when drawing text in a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerFont : ViewerAttribute
	{

		// Public Members
		public System.Drawing.Font Font;

		/// <summary>
		/// Create a specification for a font.
		/// </summary>
		/// <param name="size">The size of the font in points.</param>
		public ViewerFont(Font font)
		{

			// Initialize the object.
			this.Font = font;

		}

		/// <summary>
		/// Create a specification for a font.
		/// </summary>
		/// <param name="size">The size of the font in points.</param>
		public ViewerFont(Single size)
		{

			// Initialize the object.
			this.Font = new Font(DefaultDocument.FontFamily, size);

		}

		/// <summary>
		/// Create a specification for a font.
		/// </summary>
		/// <param name="family">Describes the typeface used in this font.</param>
		/// <param name="size">The size of the font in points.</param>
		public ViewerFont(FontFamily family, Single size)
		{

			// Initialize the object.
			this.Font = new Font(family, size);

		}

		/// <summary>
		/// Create a specification for a font.
		/// </summary>
		/// <param name="family">Describes the typeface used in this font.</param>
		/// <param name="size">The size of the font in points.</param>
		/// <param name="fontStyle">Font attributes such as bold, italic, underline and strike-through</param>
		public ViewerFont(FontFamily family, Single size, FontStyle fontStyle)
		{

			// Initialize the object.
			this.Font = new Font(family, size, fontStyle);

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerFont(this.Font.Clone() as Font);

		}

		/// <summary>
		/// Determines if two ViewerFont objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerFont objects are compared.
			if (obj is ViewerFont)
			{
				ViewerFont viewerFont = obj as ViewerFont;
				return this.Font.FontFamily.Name == viewerFont.Font.FontFamily.Name &&
					this.Font.SizeInPoints == viewerFont.Font.SizeInPoints &&
					this.Font.Style == viewerFont.Font.Style;
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
			return this.Font.GetHashCode();

		}

	}

}
