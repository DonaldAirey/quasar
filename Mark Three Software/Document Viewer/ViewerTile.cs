namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Text;

	public class ViewerTile : IComparable<ViewerTile>
	{

		// Public Members
		public System.Boolean IsModified;
		public System.Boolean IsObsolete;
		public System.Boolean IsVisible;
		public System.Boolean IsSelected;
		public System.Boolean IsActive;
		public System.Int32 ViewerTileId;
		public System.Int32 Depth;
		public System.Object Data;
		public System.String ViewerStyleId;
		public System.Drawing.SizeF Cursor;
		public System.Drawing.RectangleF RectangleF;

		#region IComparable<ViewerTile> Members

		public int CompareTo(ViewerTile other)
		{

			return ((IComparable)this.Data).CompareTo(other.Data);

		}

		#endregion

	}

}
