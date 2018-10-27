namespace MarkThree.Forms
{

	using System;
	using System.Collections;

	/// <summary>
	/// A List of Nodes Nodes that can be indexed and reordered.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class OrderableNodeList : SortedNodeList
	{

		private System.Collections.ArrayList orderableList;

		/// <summary>
		/// Initialize the list of nodes.
		/// </summary>
		public OrderableNodeList()
		{

			// Initialize the object.
			this.orderableList = new ArrayList();

		}

		/// <summary>
		/// Add a node to the list.
		/// </summary>
		/// <param name="node">The node to be added.</param>
		/// <returns>The added node.</returns>
		public new Node Add(Node node) {this.orderableList.Add(node); return (Node)base.Add(node);}

		/// <summary>
		/// Removes a node from the list.
		/// </summary>
		/// <param name="node">The node to be removed.</param>
		public new void Remove(Node node) {this.orderableList.Remove(node); base.Remove(node);}

		/// <summary>
		/// Removes all elements from the MarkThree.Forms.Stylesheet.NodeList.
		/// </summary>
		public new void Clear() {this.orderableList.Clear(); base.Clear();}

		/// <summary>
		/// The node at the specified zero-based index in the list.
		/// </summary>
		public new Node this[int identifier] {get {return (Node)this.orderableList[identifier];}}

		/// <summary>
		/// Searches for the specified Node and returns the zero-based index of the first occurance within the NodeList.
		/// </summary>
		/// <param name="node">A Node.</param>
		/// <returns>The zero-based index of the Node in the NodeList.</returns>
		public new int IndexOf(Node node) {return this.orderableList.IndexOf(node);}

		/// <summary>
		/// Returns an enumeration for a section of the MarkThree.Forms.NodeList.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the Nodes in the list.</returns>
		public new IEnumerator GetEnumerator() {return this.orderableList.GetEnumerator();}

		/// <summary>
		/// Moves the Node from the specified source index in the NodeList to the specified destination index.
		/// </summary>
		/// <param name="sourceIndex">The zero-based index of the Node that is to be moved.</param>
		/// <param name="destinationIndex">The zero-based index in the NodeList where the Node is to be relocated.</param>
		public void Move(int sourceIndex, int destinationIndex)
		{

			// Use the index to find the object in the table, put that object out and then stuff it back in at the new 
			// specified index.  Note that if the destination for the Node is after the Node that is being moved, the
			// destination index needs to be adjusted to account for the fact that an element has been removed.
			object sourceObject = this.orderableList[sourceIndex];
			this.orderableList.RemoveAt(sourceIndex);
			this.orderableList.Insert(sourceIndex < destinationIndex ? destinationIndex - 1 : destinationIndex, sourceObject);

		}

		/// <summary>
		/// Moves the specified source Node to a position before the specified destination Node.
		/// </summary>
		/// <param name="sourceNode">The source Node.</param>
		/// <param name="destinationNode">Provides the destination for the Node, null specifies the end of the list.\
		/// </param>
		public void Move(object sourceNode, object destinationNode)
		{

			// Remove the object from the list and calculate the new destination index based on the destination Node.  If the
			// destination Node is null, the record is moved to the end of the list.
			this.orderableList.Remove(sourceNode);
			int index = destinationNode == null ? this.orderableList.Count : this.orderableList.IndexOf(destinationNode);
			this.orderableList.Insert(index, sourceNode);

		}

		/// <summary>
		/// Copies a range of Nodes from the NodeList to a compatible one dimensional array.
		/// </summary>
		/// <param name="array">The destination array for the Nodes in this list.</param>
		/// <param name="arrayIndex">The starting index for the copy operation.</param>
		public new void CopyTo(System.Array array, int arrayIndex) {this.orderableList.CopyTo(array, arrayIndex);}

		/// <summary>
		/// Make a shallow copy of the NodeList.
		/// </summary>
		/// <returns>A shallow copy (the list is copied, the contents are not) of the original NodeList.</returns>
		public NodeList Clone()
		{

			// Make a copy of the list and copy all the elements from the old list to the copy.
			NodeList orderableList = new NodeList(null);
			foreach (Node node in this.orderableList)
				orderableList.Add(node);
			return orderableList;

		}

		/// <summary>
		/// Write the SortedNodeList to the Xml Document.
		/// </summary>
		/// <param name="xmlTokenWriter">Represents a writer that provides a fast, non-cached, forward only means of generating
		/// streams or files containing XML data.</param>
		public override void Save(XmlTokenWriter xmlTokenWriter)
		{

			// Write each of the child elements out to the Xml Writer.
			foreach (Node node in this.orderableList)
				node.Save(xmlTokenWriter);

		}

	}

}
