namespace MarkThree.Forms
{

	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Creates a CodeDOM Snippet for other-than-basic types.
	/// </summary>
	public class CodeSnippet
	{

		/// <summary>
		/// Creates a code snippet for defining Colors as a CodeDOM primitive.
		/// </summary>
		/// <param name="color">A System.Drawing color.</param>
		/// <returns>A CodeDOM expression that will evaluate to a System.Drawing.Color when compiled.</returns>
		public static CodeExpression Convert(Color color)
		{

			// Convert the system and known colors to strings.  Convert every other color to a set of integers that will recreated
			// the color when compiled.
			return new CodeSnippetExpression(color.IsSystemColor ?
				string.Format("System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.{0})", Enum.GetName(typeof(System.Drawing.KnownColor), color.ToKnownColor())) :
				color.IsNamedColor ?
				string.Format("System.Drawing.Color.{0}", Enum.GetName(typeof(System.Drawing.KnownColor), color.ToKnownColor())) :
				string.Format("System.Drawing.Color.FromArgb({0}, {1}, {2}, {3})", color.A, color.R, color.G, color.B));

		}

		/// <summary>
		/// Creates a code snippet for defining a string alignmnment constant.
		/// </summary>
		/// <param name="color">A System.Drawing color.</param>
		/// <returns>A CodeDOM expression that will evaluate to a System.Drawing.StringAlignment when compiled.</returns>
		public static CodeExpression Convert(StringAlignment stringAlignment)
		{

			// Get the text of the enumerated value.  When compiled we'll arrive back at the same constant.
			return new CodeSnippetExpression(
				string.Format("System.Drawing.StringAlignment.{0}", Enum.GetName(typeof(System.Drawing.StringAlignment), stringAlignment)));

		}

		public static CodeExpression Convert(FontStyle fontStyle)
		{

			string flags = string.Empty;
			if ((System.Drawing.FontStyle.Bold & fontStyle) != 0)
				flags = "System.Drawing.FontStyle.Bold";
			if ((System.Drawing.FontStyle.Italic & fontStyle) != 0)
				flags += flags == string.Empty ? "System.Drawing.FontStyle.Italic" : " | System.Drawing.FontStyle.Italic";
			if ((System.Drawing.FontStyle.Strikeout & fontStyle) != 0)
				flags += flags == string.Empty ? "System.Drawing.FontStyle.Strikeout" : " | System.Drawing.FontStyle.Strikeout";
			if ((System.Drawing.FontStyle.Underline & fontStyle) != 0)
				flags += flags == string.Empty ? "System.Drawing.FontStyle.Underline" : " | System.Drawing.FontStyle.Underline";
			return new CodeSnippetExpression(flags);

		}

		public static CodeExpression Convert(FontFamily family)
		{

			// The only way to create code snippet describing a FontFamily is using the name of the target family.
			return new CodeObjectCreateExpression(typeof(FontFamily), new CodePrimitiveExpression(family.Name));

		}

	}

}
