/*************************************************************************************************************************
*
*	File:			ElementNameTable.cs
*	Description:	Used to tokenize Element names in an XML file.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Lookup table for integers.
	/// </summary>
	public class ElementNameTable : Hashtable
	{

		/// <summary>
		/// Creates a table that can turn spreadsheet element names into tokens.
		/// </summary>
		public ElementNameTable()
		{

			// Initialize with the elements found in the spreadsheet.
			this.Add("ss:NamedRange", ElementName.NamedRange);
			this.Add("ss:Row", ElementName.Row);
			this.Add("ss:Cell", ElementName.Cell);
			this.Add("ss:Data", ElementName.Data);

		}

		/// <summary>
		/// Allows only integers to be returned from the indexer of the class.
		/// </summary>
		public new ElementName this[object key]
		{
			
			get
			{

				// Return a 'ElementName.None' value if there is no mapping.  Otherwise, cast the found object back to an
				// ElementName type.
				object nodeType = base[key];
				return (nodeType == null) ? ElementName.None : (ElementName)base[key];

			}
			
			set {base[key] = value;}
		
		}
	
	}

	/// <summary>
	/// Element names for an XML Spreadsheet.
	/// </summary>
	public enum ElementName {None, NamedRange, Row, Cell, Data};

}
