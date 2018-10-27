namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Data;
	using System.Xml;

	/// <summary>
	/// Expands tokens to their XML Namespace equivalents and writes them to the XmlWriter.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class XmlTokenWriter
	{

		private Lexicon lexicon;
		private XmlWriter xmlWriter;

		public XmlTokenWriter(XmlWriter xmlWriter)
		{

			// Initialize the object.
			this.xmlWriter = xmlWriter;
			this.lexicon = new Lexicon();

		}

		public void Flush() {this.xmlWriter.Flush();}

		public void WriteStartDocument(bool standalone) {this.xmlWriter.WriteStartDocument(standalone);}

		public void WriteStartElement(string localName, string ns)
		{
			
			this.xmlWriter.WriteStartElement(localName, ns);

		}

		public void WriteStartElement(string prefix, string localName, string ns)
		{
			
			this.xmlWriter.WriteStartElement(prefix, localName, ns);

		}

		public void WriteAttributeString(string localName, string value)
		{

			this.xmlWriter.WriteAttributeString(localName, value);

		}

		public void WriteAttributeString(string localName, string ns, string value)
		{

			this.xmlWriter.WriteAttributeString(localName, ns, value);

		}

		public void WriteAttributeString(string prefix, string localName, string ns, string value)
		{

			this.xmlWriter.WriteAttributeString(prefix, localName, ns, value);

		}

		public void WriteComment(string text)
		{

			this.xmlWriter.WriteComment(text);

		}

		public void WriteStartElement(Token token)
		{

			QualifiedName qualifiedName = this.lexicon.GetQualifiedName(token);
			this.xmlWriter.WriteStartElement(qualifiedName.LocalName, qualifiedName.NamespaceURI);

		}

		public void WriteAttributeString(Token token, string value)
		{

			QualifiedName qualifiedName = this.lexicon.GetQualifiedName(token);
			string ns = qualifiedName.NamespaceURI;
			string localName = qualifiedName.LocalName;
			string prefix = this.lexicon.LookupPrefix(ns);
			if (prefix == string.Empty)
				this.xmlWriter.WriteAttributeString(localName, value);
			else
				this.xmlWriter.WriteAttributeString(localName, ns, value);

		}

		public void WriteNamespace()
		{

			foreach (DictionaryEntry dictionaryEntry in this.lexicon.prefixNamespaceURI)
				this.xmlWriter.WriteAttributeString("xmlns", (string)dictionaryEntry.Key, null, (string)dictionaryEntry.Value);

		}
		
		public void WriteString(string text) {this.xmlWriter.WriteString(text);}

		public void WriteEndDocument() {this.xmlWriter.WriteEndDocument();}

		public void WriteEndElement() {this.xmlWriter.WriteEndElement();}

	}

}
