namespace MarkThree.Forms
{
	using System;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Used during parsing of the XSL to push an element of the stylesheet onto the stack while the children are parsed.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class NodeStack
	{

		private Stack stack;

		/// <summary>
		/// Creates a stack of Stylesheet elements.
		/// </summary>
		public NodeStack() {this.stack = new Stack();}

		/// <summary>
		/// Examines the top Stylesheet element.
		/// </summary>
		/// <returns>The value of the top element on the stack without removing it.</returns>
		public Node Peek() {return (Node)this.stack.Peek();}

		/// <summary>
		/// Removes the top element from the stack.
		/// </summary>
		/// <returns>The value of the top element.</returns>
		public Node Pop() {return (Node)this.stack.Pop();}

		/// <summary>
		/// Pushes a stylesheet element on to the stack.
		/// </summary>
		/// <param name="node">A stylesheet element to be pushed onto the stack.</param>
		public void Push(Node node) {this.stack.Push(node);}

	}

}
