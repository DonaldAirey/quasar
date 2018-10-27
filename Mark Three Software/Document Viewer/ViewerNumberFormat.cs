namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// Specification for the way numbers are presented when drawing text in a tile.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ViewerNumberFormat : ViewerAttribute
	{

		// Public Members
		public System.String Format;

		/// <summary>
		/// Create the specification for the way numbers are presented when drawing text in a tile.
		/// </summary>
		/// <param name="format">Formatting instructions for data.</param>
		public ViewerNumberFormat(string format)
		{

			// Initialize the object.
			this.Format = format;

		}

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		public override ViewerAttribute Clone()
		{

			// Return a copy of this attribute.
			return new ViewerNumberFormat(this.Format.Clone() as string);

		}

		/// <summary>
		/// Determines if two ViewerNumberFormat objects are equivalent.
		/// </summary>
		/// <param name="obj">An object to be compared.</param>
		/// <returns>true if the two objects value is the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// This will compare the values when two ViewerNumberFormat objects are compared.
			if (obj is ViewerNumberFormat)
				return this.Format.Equals((obj as ViewerNumberFormat).Format);

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
			return this.Format.GetHashCode();

		}

	}

}
