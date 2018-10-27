namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used to represent a Comment in an XML document.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class CommentNode : Node
	{

		public string Text;

		public CommentNode(string text)
			: base(Token.Comment)
		{

			// Initialize the object.
			this.Text = text;

		}

	}


}
