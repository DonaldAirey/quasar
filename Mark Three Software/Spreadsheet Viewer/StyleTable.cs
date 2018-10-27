namespace MarkThree.Forms
{

	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for StyleTable.
	/// </summary>
	public class StyleTable : Hashtable
	{

		public void Add(Style style) {base.Add(style.Id, style);}

		public Style this[string id] {get {return (Style)base[id];} set {base[id] = value;}}
	}

}
