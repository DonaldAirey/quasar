namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Xml;
	
	/// <summary>
	/// A List of Stylesheet Nodes.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class NodeList
	{

		private Node owner;
		private ArrayList arrayList;

		/// <summary>
		/// Broadcasts an event when a node is added to the list.
		/// </summary>
		public event NodeEvent NodeAdded;

		/// <summary>
		/// Broadcasts an event when a node is removed from the list.
		/// </summary>
		public event NodeEvent NodeRemoved;

		/// <summary>
		/// Creates a list that will contain only Stylesheet elements.
		/// </summary>
		public NodeList(Node owner) {this.owner = owner; this.arrayList = new ArrayList();}

		/// <summary>
		/// Broadcasts when a Node is added to the list.
		/// </summary>
		/// <param name="node">The added Node.</param>
		public virtual void OnNodeAdded(Node node)
		{

			// Broadcast the event to anyone listening when a Node is added to the list.
			if (this.NodeAdded != null)
				this.NodeAdded(this, new NodeEventArgs(node));

		}

		/// <summary>
		/// Broadcasts when a Node is removed from the list.
		/// </summary>
		/// <param name="node">The added Node.</param>
		public virtual void OnNodeRemoved(Node node)
		{

			// Broadcast the event to anyone listening when a Node is removed from the list.
			if (this.NodeRemoved != null)
				this.NodeRemoved(this, new NodeEventArgs(node));

		}

		/// <summary>
		/// Gets the number of Nodes actually contained in the MarkThree.Forms.Stylesheet.NodeList.
		/// </summary>
		public int Count {get {return this.arrayList.Count;}}

		/// <summary>
		/// Returns the Node at the zero-based index in the array.
		/// </summary>
		public object this[int index] {get {return this.arrayList[index];}}

		/// <summary>
		/// Removes all elements from the MarkThree.Forms.Stylesheet.NodeList.
		/// </summary>
		public void Clear() {this.arrayList.Clear();}

		/// <summary>
		/// Returns an enumeration for a section of the MarkThree.Forms.NodeList.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the Nodes in the list.</returns>
		public IEnumerator GetEnumerator() {return this.arrayList.GetEnumerator();}

		/// <summary>
		/// Adds a Node to the end of the MarkThree.Forms.NodeList.
		/// </summary>
		/// <param name="node">The Node to be added.</param>
		/// <returns>The Node that was added.</returns>
		public virtual Node Add(Node node) {node.Parent = this.owner; this.arrayList.Add(node); OnNodeAdded(node); return node;}

		/// <summary>
		/// Removes the first occurance of a specified Node from the MarkThree.Forms.Stylesheet.NodeList.
		/// </summary>
		/// <param name="node">The node that is to be removed from the list.</param>
		public virtual void Remove(Node node) {node.Parent = null; this.arrayList.Remove(node); OnNodeRemoved(node);}

		/// <summary>
		/// Searches for the specified Node and returns the zero-based index of the first occurance within the NodeList.
		/// </summary>
		/// <param name="node">A Node.</param>
		/// <returns>The zero-based index of the Node in the NodeList.</returns>
		public int IndexOf(Node node) {return this.arrayList.IndexOf(node);}

		/// <summary>
		/// Write the NodeList to the Xml Document.
		/// </summary>
		/// <param name="xmlWriter">Represents a writer that provides a fast, non-cached, forward only means of generating
		/// streams or files containing XML data.</param>
		public virtual void Save(XmlTokenWriter xmlTokenWriter)
		{

			// Write each of the child elements out to the Xml Writer.
			foreach (Node node in this)
				node.Save(xmlTokenWriter);

		}

	}

}
