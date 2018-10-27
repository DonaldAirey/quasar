namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Provides an animation effect that does nothing.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class DefaultAnimation : Animation
	{

		// Private Members
		private string[] styles;

		/// <summary>
		/// Create a simple animation effect that does nothing.
		/// </summary>
		/// <param name="parentStyle">The parent style for the effect.</param>
		public DefaultAnimation(Style parentStyle)
		{

			// Initialize the object.  This animation effect is intended to provide a common architecture for all tiles to handle
			// animation updates.  While it is theoretically slower for tiles that don't need to change, the common framework
			// measurably improves the speed of animation effects.  This will initialize a simple array containing the parent style
			// which will always be referenced.
			this.styles = new string[] {parentStyle.StyleId};

		}

		/// <summary>
		/// Sets the initial animation sequence for a tile.
		/// </summary>
		/// <param name="tile">A tile that appears in a viewer.</param>
		public override void SetSequence(Tile tile)
		{

			// This will initialize the animation effect to always show the parent style.
			tile.StyleIndex = this.styles.Length - 1;
			tile.StyleArray = this.styles;

		}

		/// <summary>
		/// Sets the animation sequence for a tile when the data has changed.
		/// </summary>
		/// <param name="beforeTile">The current tile in the viewer.</param>
		/// <param name="afterTile">The updated version of the tile.</param>
		public override void SetSequence(Tile beforeTile, Tile afterTile) { }

	}

}
