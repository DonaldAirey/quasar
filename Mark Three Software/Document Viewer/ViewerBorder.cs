namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Describes the border around a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	abstract public class ViewerBorder : ViewerAttribute
	{

		// Public Members
		public float Width;
		public System.Drawing.Brush Brush;

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		public ViewerBorder()
		{

			// Initialize the object
			this.Width = DefaultDocument.BorderWidth;
			this.Brush = new SolidBrush(DefaultDocument.BorderColor);

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		public ViewerBorder(Brush brush)
		{

			// Initialize the object
			this.Brush = brush;

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		public ViewerBorder(Color color)
		{

			// Initialize the object
			this.Width = DefaultDocument.BorderWidth;
			this.Brush = new SolidBrush(color);

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		/// <param name="width">The width of the border.</param>
		public ViewerBorder(Color color, Single width)
		{

			// Initialize the object
			this.Width = width;
			this.Brush = new SolidBrush(color);

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="brush">A brush used to paint the border.</param>
		/// <param name="width">The width of the border.</param>
		public ViewerBorder(Brush brush, Single width)
		{

			// Initialize the object
			this.Width = width;
			this.Brush = brush;

		}

		/// <summary>
		/// Determines if two ViewerBorder objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerBorder objects are compared.
			if (obj is ViewerBorder)
			{
				ViewerBorder viewerBorder = obj as ViewerBorder;
				if (this.Brush is SolidBrush && viewerBorder.Brush is SolidBrush)
				{
					SolidBrush solidBrush1 = this.Brush as SolidBrush;
					SolidBrush solidBrush2 = viewerBorder.Brush as SolidBrush;
					return solidBrush1.Color == solidBrush2.Color && this.Width == viewerBorder.Width;
				}
				return false;
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

			// Use the members for calculating a hash code.
			return this.Brush.GetHashCode();

		}

	}

}
