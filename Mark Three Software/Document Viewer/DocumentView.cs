namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	abstract public class DocumentView
	{

		// Private Members
		protected MarkThree.Forms.DocumentViewer documentViewer;

		public DocumentView(DocumentViewer documentViewer)
		{

			// Initialize the object.
			this.documentViewer = documentViewer;

		}

		abstract public void InitializeView();
		
		abstract public ViewerTable BuildView();

	}

}
