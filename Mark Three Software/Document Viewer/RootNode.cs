namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used for parsing text out of an XML document.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class RootNode : Node
	{

		public RootNode() : base(Token.Root) { }

		/// <summary>
		/// Handles the addition of child elements and attributes.
		/// </summary>
		/// <param name="node">The child element or attribute.</param>
		public override void Add(Node node)
		{

			// Nodes added to the root node are ignored.  This provides the parser with a top-level node to which all other nodes 
			// can be added.  This element is always guaranteed to be on the top, so there is no special logic that needs to be
			// checked when popping nodes of a stack for the start of the node tree.

		}

	}

}
