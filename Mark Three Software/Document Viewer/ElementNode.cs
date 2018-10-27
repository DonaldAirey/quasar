namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used for parsing attributes from an XML document.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ElementNode : Node
	{

		public ElementNode() : base(Token.Element) { }

		protected ElementNode(Token token) : base(token) { }

	}

}
