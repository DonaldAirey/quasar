namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A list of attributes for a viewer style.
	/// </summary>
	public class ViewerAttributeList : System.Collections.Generic.List<ViewerAttribute>
	{

		// Private Members
		private ViewerStyle viewerStyle;

		/// <summary>
		/// Construct a list of attributes associated with the ViewerStyle.
		/// </summary>
		/// <param name="viewerStyle">The style that owns the attributes.</param>
		public ViewerAttributeList(ViewerStyle viewerStyle)
		{

			// Initialize the object.
			this.viewerStyle = viewerStyle;

		}

		/// <summary>
		/// Adds a ViewerAttribute to the style and checks to see if the style has changed.
		/// </summary>
		/// <param name="viewerAttribute">The attribute to add.</param>
		public new void Add(ViewerAttribute viewerAttribute)
		{

			// The main idea here is to check to see if the same attribute has already been added to this ViewerStyle and, if it
			// has, whether this new specification for that attribute will change the style.  Said differently, as the ViewerStyles
			// are reconstructed, this will set the 'IsModified' flag when the style has changed from the original.  As an 
			// optimization during the compilation and reloading of a view, only modified styles are passed on to the viewer.
			for (int attributeIndex = 0; attributeIndex < this.viewerStyle.Attributes.Count; attributeIndex++)
				if (viewerAttribute.GetType() == this.viewerStyle.Attributes[attributeIndex].GetType())
				{

					// If the incoming attribute is different from the current one, then replace the old attribute and mark the
					// style so it will be sent to the viewer with the new settings.
					if (!viewerAttribute.Equals(this.viewerStyle.Attributes[attributeIndex]))
					{
						this.viewerStyle.Attributes[attributeIndex] = viewerAttribute;
						this.viewerStyle.IsModified = true;
					}

					// After a matching attribute is found, there is nothing more to be done.
					return;

				}

			// New attributes will automatically mark the style as modified and force it to be sent to the viewer.
			this.viewerStyle.IsModified = true;
			base.Add(viewerAttribute);

		}

	}

}
