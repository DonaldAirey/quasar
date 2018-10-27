namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used for parsing attributes from an XML document.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class NamespaceNode : AttributeNode
	{

		// Internal Members
		internal string prefix;
		internal string localName;
		internal string ns;

		public NamespaceNode(string prefix, string localName, string ns) : base(Token.Namespace)
		{

			// Initialize the object.
			this.prefix = prefix;
			this.localName = localName;
			this.ns = ns;

		}

	}

}
