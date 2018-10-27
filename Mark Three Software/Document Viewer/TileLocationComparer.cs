namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Compares the depth of two tiles.
	/// </summary>
	public class TileLocationComparer : IComparer<Tile>
	{

		#region IComparer<List<List<Tile>>> Members

		/// <summary>
		/// Compares the depth of two tiles.
		/// </summary>
		/// <param name="x">The first tile to be compared.</param>
		/// <param name="y">The second tile to be compared.</param>
		/// <returns>zero if they are at the same location, -1 if X is less than Y, 1 otherwise.</returns>
		public int Compare(Tile x, Tile y)
		{
			int comparisonY = x.RectangleF.Y.CompareTo(y.RectangleF.Y);
			return comparisonY == 0 ? x.RectangleF.X.CompareTo(y.RectangleF.X) : comparisonY;
		}

		#endregion

	}

}