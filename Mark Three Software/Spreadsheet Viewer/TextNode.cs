namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used for parsing text out of an XML document.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class TextNode : Node
	{

		// Internal Members
		internal string text;

		/// <summary>
		/// Creates a node used for parsing text out of an XML document.
		/// </summary>
		/// <param name="text"></param>
		internal TextNode(string text) : base(Token.Text)
		{

			// Initialize the object.
			this.text = text;

		}

	}

}
