namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Describes the border around a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerFlashAnimation : ViewerAnimation
	{

		// Public Members
		public System.Drawing.Color Foreground;
		public System.Drawing.Color Background;
		public System.Int32 On;
		public System.Int32 Off;
		public System.Int32 Repeat;

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		public ViewerFlashAnimation()
		{

			// Initialize the object
			this.Foreground = DefaultDocument.ForeColor;
			this.Background = DefaultDocument.BackColor;
			this.On = 0;
			this.Off = 0;
			this.Repeat = 1;

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		public ViewerFlashAnimation(Color foreground, Color background, int on, int off, int repeat)
		{

			// Initialize the object
			this.Foreground = foreground;
			this.Background = background;
			this.On = on;
			this.Off = off;
			this.Repeat = repeat;

		}

		/// <summary>
		/// Determines if two ViewerFlashAnimation objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerFlashAnimation objects are compared.
			if (obj is ViewerFlashAnimation)
			{
				ViewerFlashAnimation viewerFlashAnimation = obj as ViewerFlashAnimation;
				return this.Foreground == viewerFlashAnimation.Foreground && this.Background == viewerFlashAnimation.Background &&
					this.On == viewerFlashAnimation.On && this.Off == viewerFlashAnimation.Off &&
					this.Repeat == viewerFlashAnimation.Repeat;
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
			return this.Foreground.GetHashCode() + this.Background.GetHashCode() + this.On.GetHashCode() + this.Off.GetHashCode() +
				this.Repeat.GetHashCode();

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerFlashAnimation(this.Foreground, this.Background, this.On, this.Off, this.Repeat);

		}

	}

}
