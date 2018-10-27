namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Xml;

	/// <summary>
	/// A Lexical analyzer for an XML stream.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Lexicon
	{

		// Public Members
		public Dictionary<Token, XmlQualifiedName> QualifiedNameDictionary;
		public Dictionary<string, Dictionary<string, Token>> TokenDictionary;

		/// <summary>
		/// Create an object that will convert to and from Xml Qualified Names.
		/// </summary>
		public Lexicon()
		{

			// Initialize the object.
			this.QualifiedNameDictionary = new Dictionary<Token, XmlQualifiedName>();
			this.TokenDictionary = new Dictionary<string, Dictionary<string, Token>>();

			// These attributes don't live in a namespace.  It is expected that the context of their use is sufficient to identify
			// them without the namespace qualifier.  This section will add the qualified name and tokens to two sets of hash 
			// tables which will allow for easy conversions from the human readable version of the template language element to a
			// binary (token) value value on which the machine will operate.
			Add(new XmlQualifiedName("Bold"), Token.Bold);
			Add(new XmlQualifiedName("Color"), Token.Color);
			Add(new XmlQualifiedName("ColumnId"), Token.ColumnId);
			Add(new XmlQualifiedName("Description"), Token.Description);
			Add(new XmlQualifiedName("Direction"), Token.Direction);
			Add(new XmlQualifiedName("FontName"), Token.FontName);
			Add(new XmlQualifiedName("Format"), Token.Format);
			Add(new XmlQualifiedName("Formula"), Token.Formula);
			Add(new XmlQualifiedName("HeaderHeight"), Token.HeaderHeight);
			Add(new XmlQualifiedName("Height"), Token.Height);
			Add(new XmlQualifiedName("Horizontal"), Token.Horizontal);
			Add(new XmlQualifiedName("Image"), Token.Image);
			Add(new XmlQualifiedName("Italic"), Token.Italic);
			Add(new XmlQualifiedName("LineStyle"), Token.LineStyle);
			Add(new XmlQualifiedName("Parent"), Token.Parent);
			Add(new XmlQualifiedName("Pattern"), Token.Pattern);
			Add(new XmlQualifiedName("Position"), Token.Position);
			Add(new XmlQualifiedName("PrimaryKey"), Token.PrimaryKey);
			Add(new XmlQualifiedName("Protected"), Token.Protected);
			Add(new XmlQualifiedName("ReadingOrder"), Token.ReadingOrder);
			Add(new XmlQualifiedName("RowFilter"), Token.RowFilter);
			Add(new XmlQualifiedName("SelectionMode"), Token.SelectionMode);
			Add(new XmlQualifiedName("Size"), Token.Size);
			Add(new XmlQualifiedName("StartColor"), Token.StartColor);
			Add(new XmlQualifiedName("Steps"), Token.Steps);
			Add(new XmlQualifiedName("Strikeout"), Token.Strikeout);
			Add(new XmlQualifiedName("StyleId"), Token.StyleId);
			Add(new XmlQualifiedName("Type"), Token.Type);
			Add(new XmlQualifiedName("Underline"), Token.Underline);
			Add(new XmlQualifiedName("Unique"), Token.Unique);
			Add(new XmlQualifiedName("Vertical"), Token.Vertical);
			Add(new XmlQualifiedName("Weight"), Token.Weight);
			Add(new XmlQualifiedName("Width"), Token.Width);
			Add(new XmlQualifiedName("match"), Token.match);
			Add(new XmlQualifiedName("name"), Token.name);
			Add(new XmlQualifiedName("select"), Token.select);
			Add(new XmlQualifiedName("test"), Token.test);
			Add(new XmlQualifiedName("version"), Token.version);
			
			// Populate the conversion table with elements and their associated namespaces.
			Add(new XmlQualifiedName("Alignment", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Alignment);
			Add(new XmlQualifiedName("Animation", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Animation);
			Add(new XmlQualifiedName("Border", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Border);
			Add(new XmlQualifiedName("Borders", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Borders);
			Add(new XmlQualifiedName("Cell", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Cell);
			Add(new XmlQualifiedName("Column", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Column);
			Add(new XmlQualifiedName("ColumnReference", "urn:schemas-markthreesoftware-com:stylesheet"), Token.ColumnReference);
			Add(new XmlQualifiedName("Columns", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Columns);
			Add(new XmlQualifiedName("Constraints", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Constraints);
			Add(new XmlQualifiedName("Constraint", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Constraint);
			Add(new XmlQualifiedName("Delete", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Delete);
			Add(new XmlQualifiedName("Document", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Document);
			Add(new XmlQualifiedName("Font", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Font);
			Add(new XmlQualifiedName("Fragment", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Fragment);
			Add(new XmlQualifiedName("Insert", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Insert);
			Add(new XmlQualifiedName("Interior", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Interior);
			Add(new XmlQualifiedName("NumberFormat", "urn:schemas-markthreesoftware-com:stylesheet"), Token.NumberFormat);
			Add(new XmlQualifiedName("Protection", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Protection);
			Add(new XmlQualifiedName("Row", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Row);
			Add(new XmlQualifiedName("Sort", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Sort);
			Add(new XmlQualifiedName("SortColumn", "urn:schemas-markthreesoftware-com:stylesheet"), Token.SortColumn);
			Add(new XmlQualifiedName("Style", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Style);
			Add(new XmlQualifiedName("Styles", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Styles);
			Add(new XmlQualifiedName("Table", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Table);
			Add(new XmlQualifiedName("Update", "urn:schemas-markthreesoftware-com:stylesheet"), Token.Update);
			Add(new XmlQualifiedName("View", "urn:schemas-markthreesoftware-com:stylesheet"), Token.View);
			Add(new XmlQualifiedName("apply-templates", "http://www.w3.org/1999/XSL/Transform"), Token.ApplyTemplate);
			Add(new XmlQualifiedName("attribute", "http://www.w3.org/1999/XSL/Transform"), Token.Attribute);
			Add(new XmlQualifiedName("choose", "http://www.w3.org/1999/XSL/Transform"), Token.Choose);
			Add(new XmlQualifiedName("if", "http://www.w3.org/1999/XSL/Transform"), Token.If);
			Add(new XmlQualifiedName("otherwise", "http://www.w3.org/1999/XSL/Transform"), Token.Otherwise);
			Add(new XmlQualifiedName("output", "http://www.w3.org/1999/XSL/Transform"), Token.Output);
			Add(new XmlQualifiedName("stylesheet", "http://www.w3.org/1999/XSL/Transform"), Token.Stylesheet);
			Add(new XmlQualifiedName("template", "http://www.w3.org/1999/XSL/Transform"), Token.Template);
			Add(new XmlQualifiedName("value-of", "http://www.w3.org/1999/XSL/Transform"), Token.ValueOf);
			Add(new XmlQualifiedName("variable", "http://www.w3.org/1999/XSL/Transform"), Token.Variable);
			Add(new XmlQualifiedName("when", "http://www.w3.org/1999/XSL/Transform"), Token.When);

		}

		/// <summary>
		/// Add the Qualified Name and Token combination to the conversion tables.
		/// </summary>
		/// <param name="xmlQualifiedName">A Qualified Name</param>
		/// <param name="token">A Token</param>
		public void Add(XmlQualifiedName xmlQualifiedName, Token token)
		{

			// This simple hash table can be used to convert a token into a qualified name.
			this.QualifiedNameDictionary.Add(token, xmlQualifiedName);

			// The second table is a little more complex.  The primary key is composed of two elements: the namespace and the
			// name.  Two hash tables are used to find a unique token associated with the given namespace:localName combination.  
			// The first table contains the namespaces and links to another hash table that links the local name to the token.  In
			// this way, the combination of namespace and local name can be used as a two dimensional array to look up a unique
			// token.
			Dictionary<string, Token> localNameDictionary;
			if (!this.TokenDictionary.TryGetValue(xmlQualifiedName.Namespace, out localNameDictionary))
			{
				localNameDictionary = new Dictionary<string, Token>();
				this.TokenDictionary.Add(xmlQualifiedName.Namespace, localNameDictionary);
			}
			localNameDictionary.Add(xmlQualifiedName.Name, token);

		}

	}

}
