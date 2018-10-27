namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Describes the right border of a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Bottoms Reserved.</copyright>
	public class ViewerBottomBorder : ViewerBorder
	{

		/// <summary>
		/// Consructs a description of the left border of a tile.
		/// </summary>
		/// <param name="pen">The pen to use to draw the border.</param>
		public ViewerBottomBorder(Brush brush) : base(brush) { }

		/// <summary>
		/// Consructs a description of the right border of a tile.
		/// </summary>
		/// <param name="color">The color of the pen to use to draw the border.</param>
		public ViewerBottomBorder(Color color) : base(color) { }

		/// <summary>
		/// Consructs a description of the right border of a tile.
		/// </summary>
		/// <param name="color">The color of the pen to use to draw the border.</param>
		/// <param name="width">The width of the border.</param>
		public ViewerBottomBorder(Color color, Single width) : base(color, width) { }

		/// <summary>
		/// Consructs a description of the right border of a tile.
		/// </summary>
		/// <param name="color">The color of the pen to use to draw the border.</param>
		/// <param name="width">The width of the border.</param>
		public ViewerBottomBorder(Brush brush, Single width) : base(brush, width) { }

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerBottomBorder(this.Brush.Clone() as Brush, this.Width);

		}

	}

}
