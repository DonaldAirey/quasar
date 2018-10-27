namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Data;
	using System.Xml;

	class NodeState
	{

		public Node Node;
		public IEnumerator IEnumerator;

		public NodeState(Node node, IEnumerator iEnumerator)
		{

			this.Node = node;
			this.IEnumerator = iEnumerator;

		}

	}

	public class DataTransformReader : XmlReader
	{

		// Private Members
		private Lexicon lexicon;
		private DataTransform dataTransform;
		private XmlNamespaceManager xmlNamespaceManager;
		private System.Collections.Generic.Stack<NodeState> stateStack;

		public DataTransformReader(DataTransform dataTransform)
		{

			// Initialize the object.
			this.dataTransform = dataTransform;
			this.lexicon = new Lexicon();
			this.xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			this.stateStack = new System.Collections.Generic.Stack<NodeState>();

		}

		public override int AttributeCount
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override string BaseURI
		{
			get { return this.xmlNamespaceManager.DefaultNamespace; }
		}

		public override void Close()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override int Depth
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override bool EOF
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override string GetAttribute(int i)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string GetAttribute(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool HasValue
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override bool IsEmptyElement
		{
			get
			{
				NodeState nodeState = this.stateStack.Peek();
				int nonAttributeNodeCount = nodeState.Node.Count;
				foreach (Node node in nodeState.Node)
					if (node is AttributeNode)
						nonAttributeNodeCount--;
				bool isEmpty = nonAttributeNodeCount == 0;
				return isEmpty;
			}
		}

		public override string LocalName
		{
			get
			{

				NodeState nodeState = this.stateStack.Peek();
				if (nodeState.Node is NamespaceNode)
				{

					NamespaceNode namespaceNode = nodeState.Node as NamespaceNode;
					return namespaceNode.localName;

				}

				XmlQualifiedName xmlQualifiedName = this.lexicon.QualifiedNameDictionary[nodeState.Node.token];
				return xmlQualifiedName.Name;
			
			}

		}

		public override string LookupNamespace(string prefix)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool MoveToAttribute(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool MoveToElement()
		{

			NodeState nodeState = this.stateStack.Peek();
			if (nodeState.Node is AttributeNode)
			{
				this.stateStack.Pop();
				return true;
			}

			return false;

		}

		public override bool MoveToFirstAttribute()
		{

			NodeState nodeState = this.stateStack.Peek();
			if (nodeState.Node is ElementNode)
			{

				nodeState.IEnumerator = nodeState.Node.GetEnumerator();

				while (nodeState.IEnumerator.MoveNext())
				{

					Node node = (Node)nodeState.IEnumerator.Current;
					if (node is AttributeNode)
					{
						this.stateStack.Push(new NodeState(node, node.GetEnumerator()));
						return true;
					}

				}

				// At this point, the entire node has been scanned unsuccesfully for child nodes.  Return it to the start of the
				// children in preparation of recursing into the node.
				nodeState.IEnumerator = nodeState.Node.GetEnumerator();

			}

			return false;

		}

		public override bool MoveToNextAttribute()
		{

			NodeState nodeState = this.stateStack.Peek();

			if (nodeState.Node is ElementNode)
				return MoveToFirstAttribute();

			if (nodeState.Node is TextNode)
				this.stateStack.Pop();

			nodeState = this.stateStack.Peek();

			if (nodeState.Node is AttributeNode)
			{

				this.stateStack.Pop();
				nodeState = this.stateStack.Peek();

				while (nodeState.IEnumerator.MoveNext())
				{

					Node node = (Node)nodeState.IEnumerator.Current;
					if (node is AttributeNode)
					{
						this.stateStack.Push(new NodeState(node, node.GetEnumerator()));
						return true;
					}

				}

				// At this point, the entire node has been scanned unsuccesfully for child nodes.  Return it to the start of the
				// children in preparation of recursing into the node.
				nodeState.IEnumerator = nodeState.Node.GetEnumerator();

			}

			return false;

		}

		public override XmlNameTable NameTable
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override string NamespaceURI
		{
			get
			{

				NodeState nodeState = this.stateStack.Peek();

				if (nodeState.Node is NamespaceNode)
				{

					NamespaceNode namespaceNode = nodeState.Node as NamespaceNode;
					return namespaceNode.ns;

				}

				XmlQualifiedName xmlQualifiedName = this.lexicon.QualifiedNameDictionary[nodeState.Node.token];
				string namespaceUri = xmlQualifiedName.Namespace;
				return namespaceUri;

			}
				
		}

		public override XmlNodeType NodeType
		{
			get
			{

				NodeState nodeState = this.stateStack.Peek();

				XmlNodeType xmlNodeType =
					nodeState.Node is TextNode ? XmlNodeType.Text :
					nodeState.Node is AttributeNode ? XmlNodeType.Attribute :
					nodeState.Node is CommentNode ? XmlNodeType.Comment :
					nodeState.Node is ElementNode ? XmlNodeType.Element :
					nodeState.Node is EndElementNode ? XmlNodeType.EndElement : XmlNodeType.None;
				if (xmlNodeType == XmlNodeType.None)
					Console.WriteLine("Warning: unrecognized node type");
				return xmlNodeType;

			}

		}

		public override string Prefix
		{
			get
			{

				NodeState nodeState = this.stateStack.Peek();

				if (nodeState.Node is NamespaceNode)
				{

					NamespaceNode namespaceNode = nodeState.Node as NamespaceNode;
					return namespaceNode.prefix;

				}

				XmlQualifiedName xmlQualifiedName = this.lexicon.QualifiedNameDictionary[nodeState.Node.token];
				string prefix = this.xmlNamespaceManager.LookupPrefix(xmlQualifiedName.Namespace);
				return prefix;
			
			}
		}

		private void LoadNamespace(ElementNode elementNode)
		{

			foreach (Node childNode in elementNode)
				if (childNode is NamespaceNode)
				{
					NamespaceNode namespaceNode = childNode as NamespaceNode;
					string namespacePrefix = namespaceNode.localName == "xmlns" ? string.Empty : namespaceNode.localName;
					this.xmlNamespaceManager.AddNamespace(namespacePrefix, namespaceNode.Value);
				}

			this.xmlNamespaceManager.PushScope();

		}

		/// <summary>
		/// Read the next node in the XML structure of the dataTransform.
		/// </summary>
		/// <returns>true if the next node was read successfully; false if there are no more nodes to read.</returns>
		public override bool Read()
		{

			// The dataTransform is organized as a tree structure of parent nodes and children.  The idea here is to transverse the
			// tree and emit element and attribute information when it is requested.  The 'stateStack' acts as a type of 'cursor'
			// for recursing into the tree since the architecture of the XML reader involves responding to a request for the next
			// node.  The stack is initialized on the first 'Read' and continues to return nodes until the entire tree structure
			// has been recursed into and read.
			if (this.stateStack.Count == 0)
			{

				// Initialize the stack with the root dataTransform node.
				Node node = this.dataTransform;
				this.stateStack.Push(new NodeState(node, node.GetEnumerator()));

				// The namespace associated with each node is pushed onto a Namespace manager while the
				// tree is read.
				LoadNamespace(node as ElementNode);

				// This indicates that a node was successfully read.
				return true;

			}
			else
			{

				// The state of reading the DataTransform as an XML stream is kept on the 'stateStack'.  Each time another 'Read' is
				// requested, the top of the stack is examined to restore the state of the reader to where it was when the last
				// node was read.
				NodeState nodeState = this.stateStack.Peek();

				// This loop will find the next element that should be emitted to the reader.
				while (true)
				{

					// Move on to the next element in the list of child nodes.
					while (nodeState.IEnumerator.MoveNext())
					{

						// Take the next child node and examine it.
						Node node = nodeState.IEnumerator.Current as Node;

						// When a child element is found, push it onto the stack.  The next time the 'Read' is called the process
						// will recurse into the children of this node.  Note that whenever an element is pushed onto the stack,
						// the node is examined for new namespace elements and pushed
						if (!(node is AttributeNode))
						{

							// When a child element is found, push it onto the stack.  The next time the 'Read' is called the 
							// process will recurse into the children of this node.
							this.stateStack.Push(new NodeState(node, node.GetEnumerator()));

							// Note that whenever an element is pushed onto the stack, the node is examined for new namespace
							// elements and pushed
							if (node is ElementNode)
								LoadNamespace(node as ElementNode);

							// This node is now on the top of the stack and will be emitted to the reader.
							return true;

						}

					}

					// Element End nodes are an implicit part of the tree.  They need to be added explicity to the structure that 
					// is read into an XML file.
					if (nodeState.Node is ElementNode && !IsEmptyElement)
					{
						Node endElementNode = new EndElementNode();
						this.stateStack.Push(new NodeState(endElementNode, endElementNode.GetEnumerator()));
						return true;
					}

					// When the end element has been processed, pop it off the stack.
					if (nodeState.Node is EndElementNode)
					{
						this.stateStack.Pop();
						nodeState = this.stateStack.Peek();
					}

					// At this point, the hierarchy of nodes contains no more children to read.  Time to pop back up to the next
					// level of the tree.  This also includes popping the namespace scope up a level.
					if (nodeState.Node is ElementNode)
						this.xmlNamespaceManager.PopScope();

					// This will pop the 'cursor' into the tree structure back one level after all the children have been examined.
					// When he stack is empty, signal the caller that there are no more nodes to be read.
					this.stateStack.Pop();
					if (this.stateStack.Count == 0)
						return false;

					// This will examine the top of the stack for the next time through the loop.  The loop will continue until an
					// element node is found or the stack is exhausted.
					nodeState = this.stateStack.Peek();

				}

			}

		}

		public override bool ReadAttributeValue()
		{

			// Examine the top of the stack.
			NodeState nodeState = this.stateStack.Peek();

			if (nodeState.Node is AttributeNode)
			{
				
				Node childNode = nodeState.Node.Count == 0 ? new TextNode(string.Empty) : nodeState.Node[0];
				this.stateStack.Push(new NodeState(childNode, childNode.GetEnumerator()));
				return true;

			}

			return false;

		}

		public override ReadState ReadState
		{
			get { return ReadState.Initial; }
		}

		public override void ResolveEntity()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string Value
		{
			get
			{

				// Examine the top of the stack.
				NodeState nodeState = this.stateStack.Peek();

				if (nodeState.Node is TextNode)
				{
					TextNode textNode = nodeState.Node as TextNode;
					return textNode.text;
				}

				if (nodeState.Node is CommentNode)
				{
					CommentNode commentNode = nodeState.Node as CommentNode;
					return commentNode.Text;
				}

				throw new Exception("The method or operation is not implemented.");
			
			}

		}

	}

}
