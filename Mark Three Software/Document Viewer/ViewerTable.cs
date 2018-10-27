namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class ViewerTable
	{

		private List<ViewerRow> viewerRows;

		public ViewerTable()
		{

			// Initialize the object.
			this.viewerRows = new List<ViewerRow>();

		}

		public void Add(ViewerRow viewerRow) { viewerRows.Add(viewerRow); }

		public void Sort(IComparer<ViewerRow> iComparer) {this.viewerRows.Sort(iComparer);}

		public IEnumerator<ViewerRow> GetEnumerator() { return this.viewerRows.GetEnumerator(); }

	}

}
