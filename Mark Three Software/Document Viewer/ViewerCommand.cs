namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public delegate void ViewerCommandDelegate(ViewerObject viewerObject, BackgroundContext backgroundContext);

	public class ViewerCommand
	{

		public ViewerObject ViewerObject;
		public ViewerCommandDelegate ViewerDelegate;

		public ViewerCommand(ViewerObject viewerObject, ViewerCommandDelegate viewerDelegate)
		{

			// Initialize the object.
			this.ViewerObject = viewerObject;
			this.ViewerDelegate = viewerDelegate;

		}

	}

}
