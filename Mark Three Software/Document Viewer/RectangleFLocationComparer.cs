namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Text;

	public class RectangleFLocationComparer : IComparer<RectangleF>
	{

		#region IComparer<RectangleF> Members

		public int Compare(RectangleF x, RectangleF y)
		{
			int verticalCompare = x.Y.CompareTo(y.Y);
			return verticalCompare == 0 ? x.X.CompareTo(y.X) : verticalCompare;
		}

		#endregion

	}

}
