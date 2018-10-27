namespace MarkThree.Forms
{

	using System;
	using System.Data;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Summary description for XmlTokenReader.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class XmlTokenReader
	{

		private Stack stack;
		private XmlReader xmlReader;
		private Lexicon lexicon;
		public string Value;
		public Token token;
		public string LocalName;
		public string Prefix;
		public string NamespaceURI;
		public TokenHandler startElementHandler;
		public TokenHandler textHandler;
		public TokenHandler endElementHandler;

		public XmlTokenReader(XmlReader xmlReader, Lexicon lexicon)
		{

			// Initialize the object;
			this.stack = new Stack();
			this.xmlReader = xmlReader;
			this.lexicon = lexicon;

		}

		public Token Token {get {return this.token;}}

		public TokenHandler StartElementHandler {get {return this.StartElementHandler;}}

		public bool MoveToNextAttribute()
		{

			bool status = this.xmlReader.MoveToNextAttribute();
			if (status)
			{

				if (this.xmlReader.LocalName == "xmlns" || this.xmlReader.Prefix == "xmlns")
				{

					if (this.xmlReader.LocalName == "xmlns")
					{
						this.Prefix = this.xmlReader.LocalName;
						this.LocalName = string.Empty;
					}
					else
					{
						this.Prefix = this.xmlReader.Prefix;
						this.LocalName = this.xmlReader.LocalName;
					}

					this.Value = this.xmlReader.Value;
					this.NamespaceURI = null;
					this.token = Token.Namespace;

				}
				else
				{

					this.Prefix = this.xmlReader.Prefix;
					this.LocalName = this.xmlReader.LocalName;
					this.Value = this.xmlReader.Value;
					this.NamespaceURI = this.lexicon.LookupNamespaceURI(this.Prefix);
					if (this.NamespaceURI == null)
						this.NamespaceURI = string.Empty;

					Driver driver = this.lexicon.GetDriver(this.NamespaceURI, this.LocalName);
					this.token = driver.Token;

				}

			}

			return status;

		}

		public void AddNamespace(string prefix, string namespaceUri)
		{

			this.lexicon.AddNamespace(prefix, namespaceUri);

		}

		public void Analyze()
		{

			while (this.xmlReader.Read())
			{

				this.LocalName = this.xmlReader.LocalName;
				this.Prefix = this.xmlReader.Prefix;
				this.Value = this.xmlReader.Value;

				Driver driver;

				switch (this.xmlReader.NodeType)
				{

				case XmlNodeType.Element:

					driver = this.lexicon.GetDriver(xmlReader.NamespaceURI, xmlReader.LocalName);
					this.token = driver.Token;

					this.stack.Push(driver);

					bool emptyElement = xmlReader.IsEmptyElement;

					if (driver.ElementStartHandler != null)
						driver.ElementStartHandler(this);

					if (emptyElement)
					{
						driver = (Driver)this.stack.Pop();
				
						if (driver.ElementEndHandler != null)
							driver.ElementEndHandler(this);
					}

					break;

				case XmlNodeType.Text:

					driver = (Driver)this.stack.Peek();
					if (driver.TextHandler != null)
						driver.TextHandler(this);

					break;

				case XmlNodeType.EndElement:

					driver = (Driver)this.stack.Pop();
					if (driver.ElementEndHandler != null)
						driver.ElementEndHandler(this);

					break;

				}

			}

		}

	}

}
