/*************************************************************************************************************************
*
*	File:			IntHashTable.cs
*	Description:	Strongly typed Integers from a Hashtable.
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
	public class IntHashtable : Hashtable
	{

		/// <summary>
		/// Allows only integers to be returned from the indexer of the class.
		/// </summary>
		public new int this[object key] {get {return Convert.ToInt32(base[key]);} set {base[key] = value;}}
	
	}

	/// <summary>
	/// Lookup table for integers.
	/// </summary>
	public class ColumnIndexTable : Hashtable
	{

		/// <summary>
		/// Allows only integers to be returned from the indexer of the class.
		/// </summary>
//		public new int this[object key] {get {return base.ContainsKey(key) ? Convert.ToInt32(base[key]) : SpreadsheetColumn.DoesNotExist;} set {base[key] = value;}}
	
	}

}
