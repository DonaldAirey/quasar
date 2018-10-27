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
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
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

			// This table will tokenize the incoming stream of XML element names and attributes.
			Add(new XmlQualifiedName("Alignment"), Token.Alignment);
			Add(new XmlQualifiedName("Animation"), Token.Animation);
			Add(new XmlQualifiedName("ApplyTemplate"), Token.ApplyTemplate);
			Add(new XmlQualifiedName("Background"), Token.Background);
			Add(new XmlQualifiedName("Bold"), Token.Bold);
			Add(new XmlQualifiedName("BottomBorder"), Token.BottomBorder);
			Add(new XmlQualifiedName("Color"), Token.Color);
			Add(new XmlQualifiedName("Column"), Token.Column);
			Add(new XmlQualifiedName("ColumnId"), Token.ColumnId);
			Add(new XmlQualifiedName("ColumnReference"), Token.ColumnReference);
			Add(new XmlQualifiedName("Columns"), Token.Columns);
			Add(new XmlQualifiedName("Data"), Token.Data);
			Add(new XmlQualifiedName("DataTransform"), Token.DataTransform);
			Add(new XmlQualifiedName("DataTransformId"), Token.DataTransformId);
			Add(new XmlQualifiedName("Description"), Token.Description);
			Add(new XmlQualifiedName("Direction"), Token.Direction);
			Add(new XmlQualifiedName("Down"), Token.Down);
			Add(new XmlQualifiedName("Effect"), Token.Effect);
			Add(new XmlQualifiedName("Factor"), Token.Factor);
			Add(new XmlQualifiedName("Filter"), Token.Filter);
			Add(new XmlQualifiedName("Font"), Token.Font);
			Add(new XmlQualifiedName("FontBrush"), Token.FontBrush);
			Add(new XmlQualifiedName("FontName"), Token.FontName);
			Add(new XmlQualifiedName("Foreground"), Token.Foreground);
			Add(new XmlQualifiedName("Format"), Token.Format);
			Add(new XmlQualifiedName("Height"), Token.Height);
			Add(new XmlQualifiedName("Image"), Token.Image);
			Add(new XmlQualifiedName("InteriorBrush"), Token.InteriorBrush);
			Add(new XmlQualifiedName("Italic"), Token.Italic);
			Add(new XmlQualifiedName("LeftBorder"), Token.LeftBorder);
			Add(new XmlQualifiedName("LineAlignment"), Token.LineAlignment);
			Add(new XmlQualifiedName("LineStyle"), Token.LineStyle);
			Add(new XmlQualifiedName("Lock"), Token.Lock);
			Add(new XmlQualifiedName("Locks"), Token.Locks);
			Add(new XmlQualifiedName("Match"), Token.Match);
			Add(new XmlQualifiedName("Name"), Token.Name);
			Add(new XmlQualifiedName("NumberFormat"), Token.NumberFormat);
			Add(new XmlQualifiedName("On"), Token.On);
			Add(new XmlQualifiedName("Off"), Token.Off);
			Add(new XmlQualifiedName("Order"), Token.Order);
			Add(new XmlQualifiedName("Parent"), Token.Parent);
			Add(new XmlQualifiedName("Protected"), Token.Protected);
			Add(new XmlQualifiedName("Protection"), Token.Protection);
			Add(new XmlQualifiedName("ReadingOrder"), Token.ReadingOrder);
			Add(new XmlQualifiedName("Repeat"), Token.Repeat);
			Add(new XmlQualifiedName("RightBorder"), Token.RightBorder);
			Add(new XmlQualifiedName("Row"), Token.Row);
			Add(new XmlQualifiedName("RowId"), Token.RowId);
			Add(new XmlQualifiedName("Same"), Token.Same);
			Add(new XmlQualifiedName("Scale"), Token.Scale);
			Add(new XmlQualifiedName("Scratch"), Token.Scratch);
			Add(new XmlQualifiedName("Select"), Token.Select);
			Add(new XmlQualifiedName("Size"), Token.Size);
			Add(new XmlQualifiedName("Sort"), Token.Sort);
			Add(new XmlQualifiedName("SortApplyTemplate"), Token.SortApplyTemplate);
			Add(new XmlQualifiedName("SortColumn"), Token.SortColumn);
			Add(new XmlQualifiedName("SortRow"), Token.SortRow);
			Add(new XmlQualifiedName("SortTemplate"), Token.SortTemplate);
			Add(new XmlQualifiedName("Source"), Token.Source);
			Add(new XmlQualifiedName("Split"), Token.Split);
			Add(new XmlQualifiedName("StartColor"), Token.StartColor);
			Add(new XmlQualifiedName("Steps"), Token.Steps);
			Add(new XmlQualifiedName("Strikeout"), Token.Strikeout);
			Add(new XmlQualifiedName("StringFormat"), Token.StringFormat);
			Add(new XmlQualifiedName("Style"), Token.Style);
			Add(new XmlQualifiedName("StyleId"), Token.StyleId);
			Add(new XmlQualifiedName("Styles"), Token.Styles);
			Add(new XmlQualifiedName("TargetNamespace"), Token.TargetNamespace);
			Add(new XmlQualifiedName("Template"), Token.Template);
			Add(new XmlQualifiedName("Tile"), Token.Tile);
			Add(new XmlQualifiedName("TopBorder"), Token.TopBorder);
			Add(new XmlQualifiedName("Underline"), Token.Underline);
			Add(new XmlQualifiedName("Up"), Token.Up);
			Add(new XmlQualifiedName("Variable"), Token.Variable);
			Add(new XmlQualifiedName("VariableName"), Token.VariableName);
			Add(new XmlQualifiedName("View"), Token.View);
			Add(new XmlQualifiedName("Visible"), Token.Visible);
			Add(new XmlQualifiedName("Width"), Token.Width);

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
