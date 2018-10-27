namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// A list of Nodes that can be accessed with a unique identifier.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SortedNodeList
	{

		// Internal Members
		internal ArrayList sortedList;

		/// <summary>
		/// Creates a list of Nodes that can be accessed with a unique identifier.
		/// </summary>
		public SortedNodeList()
		{

			// Initialize the object.
			this.sortedList = new ArrayList();

		}

		/// <summary>
		/// Adds a Node to the end of the MarkThree.Forms.SortedNodeList.
		/// </summary>
		/// <param name="node">The Node to be added.</param>
		/// <returns>The Node that was added.</returns>
		public void Add(Node node)
		{

			// Insert the element into the sorted list.  This list is used to find a given element using a unique key.
			int index = this.sortedList.BinarySearch(node);
			if (index < 0)
				this.sortedList.Insert(~index, node);

		}

		/// <summary>
		/// Removes the first occurance of a specified Node.
		/// </summary>
		/// <param name="node">The node that is to be removed from the list.</param>
		/// <returns>The removed Node.</returns>
		public void Remove(Node node)
		{

			// The removed node no longer has a parent.
			int index = this.sortedList.BinarySearch(node);
			if (index >= 0)
				this.sortedList.RemoveAt(index);

		}

		/// <summary>
		/// Finds a Node in the SortedNodeList using a unique key.
		/// </summary>
		/// <param name="key">The unique key that is used to identify the Node.</param>
		/// <returns>The Node matching the unique key, null if there are no matching Nodes.</returns>
		public Node Find(object key)
		{

			// Return the node from the sorted list, or null if it doesn't exist in the list.
			int index = this.sortedList.BinarySearch(key);
			return index < 0 ? null : (Node)this.sortedList[index];

		}

	}

}
