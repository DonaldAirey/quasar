namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// A specification for an image.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerImage : ViewerAttribute
	{

		// Public Members
		public Image Image;

		/// <summary>
		/// Create a specification for an image.
		/// </summary>
		/// <param name="image">The image to be displayed in a tile.</param>
		public ViewerImage(Image image)
		{

			// Initialize the object.
			this.Image = image;

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerImage(this.Image.Clone() as Image);

		}

		/// <summary>
		/// Determines if two ViewerImage objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// All bitmaps are stored in the 'Variables' table to insure there is only one copy of them.  This works in the same
			// way the 'NameTable' works in XML documents.  That is, all the work in comparing them is done when they are entered
			// into the hash table.  The 'Equals' operation involves just making sure the references are the same.
			if (obj is ViewerImage)
				return (object.ReferenceEquals(this.Image, (obj as ViewerImage).Image));

			// This object isn't equivalent to any other types of object.
			return false;

		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A value that can be used for hashing algorithms.</returns>
		public override int GetHashCode()
		{

			// Use the constituent parts for calculating a hash code.
			return this.Image.GetHashCode();

		}

	}

}
