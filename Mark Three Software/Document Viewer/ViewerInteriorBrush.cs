namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// A specification for the brush used to draw the interior of a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerInteriorBrush : ViewerAttribute
	{

		// Public Members
		public Brush Brush;

		/// <summary>
		/// Creates a specification for the brush used to draw the interior of a tile.
		/// </summary>
		/// <param name="color">The brush used to paint the interior of the tile.</param>
		public ViewerInteriorBrush(Brush brush)
		{

			// Initialize the object
			this.Brush = brush;

		}

		/// <summary>
		/// Creates a specification for the brush used to draw the interior of a tile.
		/// </summary>
		/// <param name="color">The color used for the interior of the tile.</param>
		public ViewerInteriorBrush(Color color)
		{

			// Initialize the object
			this.Brush = new SolidBrush(color);

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerInteriorBrush(this.Brush.Clone() as Brush);

		}

		/// <summary>
		/// Determines if two ViewerInteriorBrush objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerStringFormat objects are compared.
			if (obj is ViewerInteriorBrush)
			{

				// Cast the viewer so it can be examined in detail.
				ViewerInteriorBrush viewerInteriorBrush = obj as ViewerInteriorBrush;

				// The brushes can be compared if they are both SolidBrush types.
				if (this.Brush is SolidBrush && viewerInteriorBrush.Brush is SolidBrush)
					return (this.Brush as SolidBrush).Color == (viewerInteriorBrush.Brush as SolidBrush).Color;

				// The brushes can be compared if they are both TexturedBrush types.
				if (this.Brush is TextureBrush && viewerInteriorBrush.Brush is TextureBrush)
				{
					TextureBrush brush1 = this.Brush as TextureBrush;
					TextureBrush brush2 = viewerInteriorBrush.Brush as TextureBrush;
					return object.ReferenceEquals(brush1.Image, brush2.Image) &&
						brush1.Transform == brush2.Transform &&
						brush1.WrapMode == brush2.WrapMode;
				}

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
			return this.Brush.GetHashCode();

		}

	}

}
