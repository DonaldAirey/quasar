namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// A specification for the brush used to draw text in a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerFontBrush : ViewerAttribute
	{

		// Public Members
		public SolidBrush SolidBrush;

		/// <summary>
		/// Create a specification for a brush used to draw text in a tile.
		/// </summary>
		/// <param name="color">The color used for a solid brush.</param>
		public ViewerFontBrush(SolidBrush solidBrush)
		{

			// Initialize the object
			this.SolidBrush = solidBrush;

		}

		/// <summary>
		/// Create a specification for a brush used to draw text in a tile.
		/// </summary>
		/// <param name="color">The color used for a solid brush.</param>
		public ViewerFontBrush(Color color)
		{

			// Initialize the object
			this.SolidBrush = new SolidBrush(color);

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerFontBrush(this.SolidBrush.Clone() as SolidBrush);

		}

		/// <summary>
		/// Determines if two ViewerFont objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerFontBrush objects are compared.
			if (obj is ViewerFontBrush)
			{
				ViewerFontBrush viewerFontBrush = obj as ViewerFontBrush;
				return this.SolidBrush.Color == viewerFontBrush.SolidBrush.Color;
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
			return this.SolidBrush.GetHashCode();

		}

	}

}
