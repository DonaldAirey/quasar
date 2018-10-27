namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used to represent a Comment in an XML document.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class CommentNode : Node
	{

		public string Text;

		public CommentNode(string text)
			: base(Token.Comment)
		{

			// Initialize the object.
			this.Text = text;

		}

		public override Node Clone()
		{

			CommentNode commentNode = new CommentNode(this.Text);
			foreach (Node childNode in this)
				commentNode.Add(childNode.Clone());
			return commentNode;

		}

	}

}
