namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A node used for parsing attributes from an XML document.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class AttributeNode : Node
	{

		// Internal Members
		private TextNode textNode;

		public AttributeNode(Token token) : base(token) { }

		public AttributeNode(Token token, string value) : base(token)
		{

			// Initialize the object.
			Add(new TextNode(value));
		
		}

		/// <summary>
		/// Handles the addition of child elements and attributes.
		/// </summary>
		/// <param name="node">The child element or attribute.</param>
		public override void Add(Node node)
		{

			// The value of an attribute node comes from the text parsed out of the XML stream.
			if (node is TextNode)
				this.textNode = node as TextNode;

			// Allow the base node to complete the operation.
			base.Add(node);

		}

		public override void Remove(Node node)
		{

			// If the text node is removed from the list of children, the pointer to that node should be removed also.
			if (node is TextNode)
				this.textNode = null;

			// Allow the base node to complete the operation.
			base.Add(node);

		}

		public string Value
		{

			get { return this.textNode == null ? string.Empty : this.textNode.text; }

			set
			{

				if (value == string.Empty)
				{

					if (this.textNode != null)
						Remove(this.textNode);

				}
				else
				{

					if (this.textNode == null)
						Add(new TextNode(value));
					else
						this.textNode.text = value;

				}

			}

		}

	}

}
