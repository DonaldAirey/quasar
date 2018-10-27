namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// Used to broadcast an event involving a Node.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class NodeEventArgs : EventArgs
	{

		/// <summary>
		/// The subject Node of the event.
		/// </summary>
		public readonly Node Node;

		/// <summary>
		/// Create event arguments for Node events.
		/// </summary>
		/// <param name="node">The Node that is the subject of the event.</param>
		public NodeEventArgs(Node node) {this.Node = node;}

	}
		
}
