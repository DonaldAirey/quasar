/*************************************************************************************************************************
*
*	File:			ColumnType.cs
*	Description:	Used to define and manipulate the column types in a placement spreadsheet control.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.Placement
{

	/// <summary>
	/// Enumeration for the column types in the appraisal viewer.
	/// </summary>
	internal class ColumnType
	{

		private static Hashtable hashtable;

		/// <summary>None</summary>
		public const int None = 0;
		/// <summary>Row Type</summary>
		public const int RowType = 1;
		/// <summary>BrokerId</summary>
		public const int BrokerId = 2;
		/// <summary>BrokerName</summary>
		public const int BrokerName = 3;
		/// <summary>BrokerSymbol</summary>
		public const int BrokerSymbol = 4;
		/// <summary>PlacementId</summary>
		public const int PlacementId = 5;
		/// <summary>Quantity</summary>
		public const int Quantity = 6;
		/// <summary>TimeInForceCode</summary>
		public const int TimeInForceCode = 7;
		/// <summary>TimeInForceMnemonic</summary>
		public const int TimeInForceMnemonic = 8;
		/// <summary>OrderTypeCode</summary>
		public const int OrderTypeCode = 9;
		/// <summary>OrderTypeMnemonic</summary>
		public const int OrderTypeMnemonic = 10;
		/// <summary>RowVersion</summary>
		public const int RowVersion = 11;
		/// <summary>Price1</summary>
		public const int Price1 = 12;
		/// <summary>Price2</summary>
		public const int Price2 = 13;
		/// <summary>CreatedTime</summary>
		public const int CreatedTime = 14;
		/// <summary>CreatedLoginId</summary>
		public const int CreatedLoginId = 15;
		/// <summary>CreatedLoginName</summary>
		public const int CreatedLoginName = 16;
		/// <summary>ModifiedTime</summary>
		public const int ModifiedTime = 17;
		/// <summary>ModifiedLoginId</summary>
		public const int ModifiedLoginId = 18;
		/// <summary>ModifiedLoginName</summary>
		public const int ModifiedLoginName = 19;

		/// <summary>
		/// Constructor for Placement Column Types.
		/// </summary>
		static ColumnType()
		{

			// Create a hash table to tokenize the column names in an placement document.
			ColumnType.hashtable = new Hashtable();
			ColumnType.hashtable["rowType"] = ColumnType.RowType;
			ColumnType.hashtable["brokerId"] = ColumnType.BrokerId;
			ColumnType.hashtable["brokerName"] = ColumnType.BrokerName;
			ColumnType.hashtable["brokerSymbol"] = ColumnType.BrokerSymbol;
			ColumnType.hashtable["placementId"] = ColumnType.PlacementId;
			ColumnType.hashtable["quantity"] = ColumnType.Quantity;
			ColumnType.hashtable["timeInForceCode"] = ColumnType.TimeInForceCode;
			ColumnType.hashtable["timeInForceMnemonic"] = ColumnType.TimeInForceMnemonic;
			ColumnType.hashtable["orderTypeCode"] = ColumnType.OrderTypeCode;
			ColumnType.hashtable["orderTypeMnemonic"] = ColumnType.OrderTypeMnemonic;
			ColumnType.hashtable["rowVersion"] = ColumnType.RowVersion;
			ColumnType.hashtable["price1"] = ColumnType.Price1;
			ColumnType.hashtable["price2"] = ColumnType.Price2;
			ColumnType.hashtable["createdTime"] = ColumnType.CreatedTime;
			ColumnType.hashtable["createdLoginId"] = ColumnType.CreatedLoginId;
			ColumnType.hashtable["createdLoginName"] = ColumnType.CreatedLoginName;
			ColumnType.hashtable["modifiedTime"] = ColumnType.ModifiedTime;
			ColumnType.hashtable["modifiedLoginId"] = ColumnType.ModifiedLoginId;
			ColumnType.hashtable["modifiedLoginName"] = ColumnType.ModifiedLoginName;

		}

		/// <summary>
		/// Finds a token based on the text of that token.
		/// </summary>
		/// <param name="text">Text of the column name.</param>
		/// <returns>Integer token representing the text.</returns>
		public static int Tokenize(string text)
		{

			// Return the tokenized value or 'None' if the token doesn't exist.
			return ColumnType.hashtable.ContainsKey(text) ? Convert.ToInt32(ColumnType.hashtable[text]) : ColumnType.None;

		}

	};

}
