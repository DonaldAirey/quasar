/*************************************************************************************************************************
*
*	File:			ColumnType.cs
*	Description:	Defines the generic column functions in an appraisal.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.Appraisal
{
	/// <summary>
	/// Enumeration for the column types in the appraisal viewer.
	/// </summary>
	internal class ColumnType
	{

		/// <summary>None</summary>
		public const int None = 0;
		/// <summary>RowType</summary>
		public const int RowType = 1;
		/// <summary>SecurityId</summary>
		public const int SecurityId = 2;
		/// <summary>SectorId</summary>
		public const int SectorId = 3;
		/// <summary>PositionTypeCode</summary>
		public const int PositionTypeCode = 4;
		/// <summary>ModelPercent</summary>
		public const int ModelPercent = 5;
		/// <summary>ActualPercent</summary>
		public const int ActualPercent = 6;
		/// <summary>LastPrice</summary>
		public const int LastPrice = 7;
		/// <summary>CrossPrice</summary>
		public const int CrossPrice = 8;
		/// <summary>PriceFactor</summary>
		public const int PriceFactor = 9;
		/// <summary>PositionQuantity</summary>
		public const int PositionQuantity = 10;
		/// <summary>ProposedQuantity</summary>
		public const int ProposedQuantity = 11;
		/// <summary>OrderedQuantity</summary>
		public const int OrderedQuantity = 12;
		/// <summary>AllocatedQuantity</summary>
		public const int AllocatedQuantity = 13;
		/// <summary>QuantityFactor</summary>
		public const int QuantityFactor = 14;
		/// <summary>MarketValue</summary>
		public const int MarketValue = 15;
		/// <summary>UserData0</summary>
		public const int UserData0 = 16;
		/// <summary>UserData1</summary>
		public const int UserData1 = 17;
		/// <summary>UserData2</summary>
		public const int UserData2 = 18;
		/// <summary>UserData3</summary>
		public const int UserData3 = 19;
		/// <summary>UserData4</summary>
		public const int UserData4 = 20;
		/// <summary>UserData5</summary>
		public const int UserData5 = 21;
		/// <summary>UserData6</summary>
		public const int UserData6 = 22;
		/// <summary>UserData7</summary>
		public const int UserData7 = 23;
		/// <summary>SecurityName</summary>
		public const int SecurityName = 24;
		/// <summary>SecuritySymbol</summary>
		public const int SecuritySymbol = 25;

		// Hash table used to look up token integers from column names.
		private static Hashtable hashtable;

		/// <summary>
		/// Constructor for the ColumnType enumeration.
		/// </summary>
		static ColumnType()
		{

			// Create a hash table to tokenize the column names in an execution document.
			ColumnType.hashtable = new Hashtable();
			ColumnType.hashtable["allocatedQuantity"] = ColumnType.AllocatedQuantity;
			ColumnType.hashtable["actualPercent"] = ColumnType.ActualPercent;
			ColumnType.hashtable["crossPrice"] = ColumnType.CrossPrice;
			ColumnType.hashtable["lastPrice"] = ColumnType.LastPrice;
			ColumnType.hashtable["marketValue"] = ColumnType.MarketValue;
			ColumnType.hashtable["modelPercent"] = ColumnType.ModelPercent;
			ColumnType.hashtable["orderedQuantity"] = ColumnType.OrderedQuantity;
			ColumnType.hashtable["positionQuantity"] = ColumnType.PositionQuantity;
			ColumnType.hashtable["positionTypeCode"] = ColumnType.PositionTypeCode;
			ColumnType.hashtable["priceFactor"] = ColumnType.PriceFactor;
			ColumnType.hashtable["proposedQuantity"] = ColumnType.ProposedQuantity;
			ColumnType.hashtable["quantityFactor"] = ColumnType.QuantityFactor;
			ColumnType.hashtable["rowType"] = ColumnType.RowType;
			ColumnType.hashtable["securityId"] = ColumnType.SecurityId;
			ColumnType.hashtable["sectorId"] = ColumnType.SectorId;
			ColumnType.hashtable["securityName"] = ColumnType.SecurityName;
			ColumnType.hashtable["securitySymbol"] = ColumnType.SecuritySymbol;
			ColumnType.hashtable["userData0"] = ColumnType.UserData0;
			ColumnType.hashtable["userData1"] = ColumnType.UserData1;
			ColumnType.hashtable["userData2"] = ColumnType.UserData2;
			ColumnType.hashtable["userData3"] = ColumnType.UserData3;
			ColumnType.hashtable["userData4"] = ColumnType.UserData4;
			ColumnType.hashtable["userData5"] = ColumnType.UserData5;
			ColumnType.hashtable["userData6"] = ColumnType.UserData6;
			ColumnType.hashtable["userData7"] = ColumnType.UserData7;

		}

		/// <summary>
		/// Returns an integer corresponding to the enumeration text.
		/// </summary>
		/// <param name="text">The name of the enumeration.</param>
		/// <returns>An integer value representing the column type.</returns>
		public static int Tokenize(string text)
		{

			// Return the tokenized value or 'None' if the token doesn't exist.
			return ColumnType.hashtable.ContainsKey(text) ? Convert.ToInt32(ColumnType.hashtable[text]) : ColumnType.None;

		}

	};

}
