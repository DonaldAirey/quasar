namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Describes the border around a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerFadeAnimation : ViewerAnimation
	{

		// Public Members
		public System.Drawing.Color Up;
		public System.Drawing.Color Down;
		public System.Drawing.Color Same;
		public System.Int32 Steps;

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		public ViewerFadeAnimation()
		{

			// Initialize the object
			this.Up = DefaultDocument.ForeColor;
			this.Down = DefaultDocument.ForeColor;
			this.Same = DefaultDocument.ForeColor;
			this.Steps = DefaultDocument.Steps;

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		public ViewerFadeAnimation(Color up, Color down)
		{

			// Initialize the object
			this.Up = up;
			this.Down = down;
			this.Same = DefaultDocument.ForeColor;
			this.Steps = DefaultDocument.Steps;

		}

		/// <summary>
		/// Consructs a description of a border around a tile.
		/// </summary>
		/// <param name="color">The color of the brush to use to draw the border.</param>
		public ViewerFadeAnimation(Color up, Color down, Color same, int steps)
		{

			// Initialize the object
			this.Up = up;
			this.Down = down;
			this.Same = same;
			this.Steps = steps;

		}

		/// <summary>
		/// Determines if two ViewerFadeAnimation objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerFadeAnimation objects are compared.
			if (obj is ViewerFadeAnimation)
			{
				ViewerFadeAnimation viewerFadeAnimation = obj as ViewerFadeAnimation;
				return this.Up == viewerFadeAnimation.Up && this.Down == viewerFadeAnimation.Down &&
					this.Same == viewerFadeAnimation.Same && this.Steps == viewerFadeAnimation.Steps;
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
			return this.Up.GetHashCode() + this.Down.GetHashCode() + this.Same.GetHashCode() + this.Steps.GetHashCode();

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerFadeAnimation(this.Up, this.Down, this.Same, this.Steps);

		}

	}

}
