namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Common class used to provide an animation effect for tiles.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public abstract class Animation
	{

		/// <summary>
		/// Sets the initial animation sequence for a tile.
		/// </summary>
		/// <param name="tile">A tile that appears in a viewer.</param>
		public abstract void SetSequence(Tile tile);

		/// <summary>
		/// Sets the animation sequence for a tile when the data has changed.
		/// </summary>
		/// <param name="beforeTile">The current tile in the viewer.</param>
		/// <param name="afterTile">The updated version of the tile.</param>
		public abstract void SetSequence(Tile beforeTile, Tile afterTile);

	}
}
