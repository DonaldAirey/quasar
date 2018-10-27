/*************************************************************************************************************************
*
*	File:			ColumnType.cs
*	Description:	Defines the generic column functions for a block order report.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.BlockOrder
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
		/// <summary>BlockOrderId</summary>
		public const int BlockOrderId = 2;
		/// <summary>SecurityId</summary>
		public const int SecurityId = 3;
		/// <summary>StatusCode</summary>
		public const int StatusCode = 4;
		/// <summary>LastPrice</summary>
		public const int LastPrice = 5;
		/// <summary>StatusCode</summary>
		public const int AskPrice = 6;
		/// <summary>StatusCode</summary>
		public const int BidPrice = 7;
		/// <summary>Aggregate Quantity Ordered</summary>
		public const int QuantityOrdered = 8;
		/// <summary>Aggregate Quantity Placed</summary>
		public const int QuantityPlaced = 9;
		/// <summary>Aggregate Quantity Executed</summary>
		public const int QuantityExecuted = 10;
		/// <summary>Trade Date</summary>
		public const int TradeDate = 11;
		/// <summary>Settlement Date</summary>
		public const int SettlementDate = 12;
		/// <summary>Yield</summary>
		public const int Yield = 13;

		// This table is used to quickly associate a number with a column name.
		private static Hashtable hashtable;

		/// <summary>
		/// Creates a token lookup table for the enumeration.
		/// </summary>
		static ColumnType()
		{

			// Create a hash table to tokenize the column names in an execution document.
			ColumnType.hashtable = new Hashtable();
			ColumnType.hashtable["rowType"] = ColumnType.RowType;
			ColumnType.hashtable["blockOrderId"] = ColumnType.BlockOrderId;
			ColumnType.hashtable["securityId"] = ColumnType.SecurityId;
			ColumnType.hashtable["statusCode"] = ColumnType.StatusCode;
			ColumnType.hashtable["lastPrice"] = ColumnType.LastPrice;
			ColumnType.hashtable["bidPrice"] = ColumnType.BidPrice;
			ColumnType.hashtable["askPrice"] = ColumnType.AskPrice;
			ColumnType.hashtable["yield"] = ColumnType.Yield;
			ColumnType.hashtable["quantityOrdered"] = ColumnType.QuantityOrdered;
			ColumnType.hashtable["quantityPlaced"] = ColumnType.QuantityPlaced;
			ColumnType.hashtable["quantityExecuted"] = ColumnType.QuantityExecuted;
			ColumnType.hashtable["tradeDate"] = ColumnType.TradeDate;
			ColumnType.hashtable["settlementDate"] = ColumnType.SettlementDate;

		}

		/// <summary>
		/// Turns a text string for a column name into an integer token.
		/// </summary>
		/// <param name="text">The text of the column name.</param>
		/// <returns>An integer that represents the column.</returns>
		public static int Tokenize(string text)
		{

			// Return the tokenized value or 'None' if the token doesn't exist.
			return ColumnType.hashtable.ContainsKey(text) ? Convert.ToInt32(ColumnType.hashtable[text]) : ColumnType.None;

		}

	};

}
