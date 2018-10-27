namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Tile : ViewerObject
	{
		// Public Members
		public System.Boolean IsSelected;
		public System.Boolean IsActive;
		public System.Int32 TileId;
		public System.Int32 Depth;
		public System.Int32 StyleIndex;
		public System.Object Data;
		public System.String StyleId;
		public System.String[] StyleArray;
		public System.Drawing.RectangleF RectangleF;

		public Tile() { }

		/// <summary>
		/// Initializes a new tile from the information in a ViewerTile.
		/// </summary>
		/// <param name="viewerTile">Specification for the data and style used to present a tile.</param>
		public Tile(ViewerTile viewerTile)
		{

			// Initialize the object from the ViewerTile.
			this.IsSelected = viewerTile.IsSelected;
			this.IsActive = viewerTile.IsActive;
			this.TileId = viewerTile.ViewerTileId;
			this.Depth = viewerTile.Depth;
			this.Data = viewerTile.Data;
			this.StyleId = viewerTile.ViewerStyleId;
			this.RectangleF = viewerTile.RectangleF;

		}

		/// <summary>
		/// Creates a copy of this tile.
		/// </summary>
		/// <returns>A copy of this tile.</returns>
		public Tile Clone()
		{

			// Return a copy of this tile.
			Tile tile = new Tile();
			tile.IsSelected = this.IsSelected;
			tile.IsActive = this.IsActive;
			tile.TileId = this.TileId;
			tile.Depth = this.Depth;
			tile.Data = this.Data;
			tile.StyleId = this.StyleId;
			tile.RectangleF = this.RectangleF;
			return tile;

		}

	}

}
