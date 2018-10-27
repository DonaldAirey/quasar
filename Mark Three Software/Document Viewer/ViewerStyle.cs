namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// A specification for a style that describes how data is presented in a tile on the viewer.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerStyle : ViewerObject
	{

		// Public Members
		public bool IsModified;
		public string StyleId;
		public string ParentId;

		// Public read only Members
		public readonly ViewerAttributeList Attributes;

		/// <summary>
		/// Construct the specification of a style.
		/// </summary>
		/// <param name="styleId">A unique identifier for the style.</param>
		public ViewerStyle(string styleId)
		{

			// Initialize the object.
			this.StyleId = styleId;
			this.ParentId = string.Empty;
			this.Attributes = new ViewerAttributeList(this);

		}

		public ViewerStyle Clone()
		{

			ViewerStyle viewerStyle = new ViewerStyle(this.StyleId);
			viewerStyle.ParentId = this.ParentId;
			foreach (ViewerAttribute viewerAttribute in this.Attributes)
				viewerStyle.Attributes.Add(viewerAttribute.Clone());
			return viewerStyle;

		}

	}

}
