namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;


	/// <summary>
	/// Used to pass common information to handlers of viewer events.
	/// </summary>
	public class BackgroundContext
	{

		// Public Members
		public bool HasSizeChanged;
		public List<RectangleF> UpdateList;

		/// <summary>
		/// Construct an object that maintains common information during the background updates to the viewer.
		/// </summary>
		public BackgroundContext()
		{

			// Initialize the object.
			this.HasSizeChanged = false;
			this.UpdateList = new List<RectangleF>();

		}

	}
}
