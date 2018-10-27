namespace MarkThree.Forms
{

	using MarkThree.Forms;
	using System.Collections.Generic;
	using System;

	/// <summary>
	/// Delegate for handling the changing of the selected tile.
	/// </summary>
	public delegate void SelectionChangeHandler(object sender, SelectionChangeArgs selectionChangeArgs);

	// This class is used to pass tile coordinates around.
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SelectionChangeArgs : EventArgs
	{

		public List<Tile> Tiles;

		/// <summary>
		/// Constructor for a SelectionChangeArgs object.
		/// </summary>
		/// <param name="tileAddress">Contains the specifics of a tile address.</param>
		public SelectionChangeArgs(List<Tile> tiles)
		{

			// Initialize the object.
			this.Tiles = tiles;

		}

	}

}
