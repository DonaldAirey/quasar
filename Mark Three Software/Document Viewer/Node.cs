namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Xml;

	/// <summary>
	/// An object in a hierarchy used as a datastructure to represent an XSL Stylesheet.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Node
	{

		// Internal Members
		internal Token token;
		internal List<Node> list;
		internal Node parent;

		/// <summary>
		/// Creates a Stylesheet node.
		/// </summary>
		public Node(Token token)
		{

			// Initialize the object.
			this.token = token;
			this.parent = null;
			this.list = new List<Node>();

		}

		/// <summary>
		/// Returns the Node at the zero-based index in the array.
		/// </summary>
		public Node this[int index] { get { return (Node)this.list[index]; } }

		/// <summary>
		/// Gets the top-level ancestor of this node.
		/// </summary>
		public Node TopLevelNode
		{

			get
			{
				// The ancestor that doesn't have a parent is the top level node.
				Node node = this;
				while (node.parent != null)
					node = node.parent;
				return node;
			}

		}

		/// <summary>
		/// Gets the number of child nodes.
		/// </summary>
		public int Count {get {return this.list.Count;}}

		/// <summary>
		/// Removes all the child nodes.
		/// </summary>
		public void Clear() {this.list.Clear();}

		/// <summary>
		/// Returns an enumeration of the child nodes.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the Nodes in the list.</returns>
		public IEnumerator<Node> GetEnumerator() {return this.list.GetEnumerator();}

		/// <summary>
		/// Adds a Node.
		/// </summary>
		/// <param name="node">The Node to be added.</param>
		/// <returns>The Node that was added.</returns>
		public virtual void Add(Node node)
		{
			
			// This node becomes the parent of the added node.
			node.parent = this;
			this.list.Add(node);

		}

		/// <summary>
		/// Removes the first occurance of a specified Node.
		/// </summary>
		/// <param name="node">The node that is to be removed from the list.</param>
		/// <returns>The removed Node.</returns>
		public virtual void Remove(Node node)
		{

			// The removed node no longer has a parent.
			node.parent = null;
			this.list.Remove(node);

		}

		/// <summary>
		/// Make a shallow copy of the NodeList.
		/// </summary>
		/// <returns>A shallow copy (the list is copied, the contents are not) of the original NodeList.</returns>
		public virtual Node Clone()
		{

			// Make a copy of the list and copy all the elements from the old list to the copy.
			Node node = new Node(this.token);
			foreach (Node childNode in this)
				node.Add(childNode.Clone());
			return node;

		}

		/// <summary>
		/// Searches for the specified Node and returns the zero-based index of the first occurance within the NodeList.
		/// </summary>
		/// <param name="node">A Node.</param>
		/// <returns>The zero-based index of the Node in the NodeList.</returns>
		public int IndexOf(Node node) {return this.list.IndexOf(node);}

		/// <summary>
		/// Copies a range of Nodes from the NodeList to a compatible one dimensional array.
		/// </summary>
		/// <param name="array">The destination array for the Nodes in this list.</param>
		/// <param name="arrayIndex">The starting index for the copy operation.</param>
		public void CopyTo(Node[] array, int arrayIndex)
		{

			// Copy the list of children into an array.
			this.list.CopyTo(array, arrayIndex);

		}

		/// <summary>
		/// Moves the Node from the specified source index in the NodeList to the specified destination index.
		/// </summary>
		/// <param name="sourceIndex">The zero-based index of the Node that is to be moved.</param>
		/// <param name="destinationIndex">The zero-based index in the NodeList where the Node is to be relocated.</param>
		public void Move(int sourceIndex, int destinationIndex)
		{

			// Use the index to find the object in the table, put that object out and then stuff it back in at the new specified
			// index.
			Node sourceObject = this.list[sourceIndex];
			this.list.RemoveAt(sourceIndex);
			this.list.Insert(destinationIndex, sourceObject);

		}

		/// <summary>
		/// Moves the specified source Node to a position before the specified destination Node.
		/// </summary>
		/// <param name="sourceNode">The source Node.</param>
		/// <param name="destinationNode">Provides the destination for the Node, null specifies the end of the list.\
		/// </param>
		public void Move(Node sourceNode, Node destinationNode)
		{

			// Remove the object from the list and calculate the new destination index based on the destination Node.  If the
			// destination Node is null, the record is moved to the end of the list.
			this.list.Remove(sourceNode);
			int index = destinationNode == null ? this.list.Count : this.list.IndexOf(destinationNode);
			this.list.Insert(index, sourceNode);

		}

	}

}
