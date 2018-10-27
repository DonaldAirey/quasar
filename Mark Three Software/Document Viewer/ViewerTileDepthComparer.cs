namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class ViewerTileDepthComparer : IComparer<ViewerTile>
	{

		#region IComparer<ViewerTile> Members

		public int Compare(ViewerTile x, ViewerTile y)
		{
			return x.Depth.CompareTo(y.Depth);
		}

		#endregion

	}

}
