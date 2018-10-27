/*************************************************************************************************************************
*
*	File:			ExecutionDocument.cs
*	Description:	The classes to control the DOM for the blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.Execution
{
	/// <summary>
	/// Enumeration for the column types in the appraisal viewer.
	/// </summary>
	public class ExecutionColumnType
	{

		/// <summary>Broker Symbol</summary>
		public const int None = 0;
		/// <summary>Row Type</summary>
		public const int RowType = 1;
		/// <summary>BrokerId</summary>
		public const int BrokerId = 2;
		/// <summary>BrokerName</summary>
		public const int BrokerName = 3;
		/// <summary>BrokerSymbol</summary>
		public const int BrokerSymbol = 4;
		/// <summary>ExecutionId</summary>
		public const int ExecutionId = 5;
		/// <summary>Quantity</summary>
		public const int Quantity = 6;
		/// <summary>Price</summary>
		public const int Price = 7;
		/// <summary>Commission</summary>
		public const int Commission = 8;
		/// <summary>Accrued Interest</summary>
		public const int AccruedInterest = 9;
		/// <summary>UserFee0</summary>
		public const int UserFee0 = 10;
		/// <summary>UserFee1</summary>
		public const int UserFee1 = 11;
		/// <summary>UserFee2</summary>
		public const int UserFee2 = 12;
		/// <summary>UserFee3</summary>
		public const int UserFee3 = 13;
		/// <summary>TradeDate</summary>
		public const int TradeDate = 14;
		/// <summary>SettlementDate</summary>
		public const int SettlementDate = 15;
		/// <summary>CreatedTime</summary>
		public const int CreatedTime = 16;
		/// <summary>CreatedLoginId</summary>
		public const int CreatedLoginId = 17;
		/// <summary>CreatedLoginName</summary>
		public const int CreatedLoginName = 18;
		/// <summary>ModifiedTime</summary>
		public const int ModifiedTime = 19;
		/// <summary>ModifiedLoginId</summary>
		public const int ModifiedLoginId = 20;
		/// <summary>ModifiedLoginName</summary>
		public const int ModifiedLoginName = 21;

		private static Hashtable hashtable;

		static ExecutionColumnType()
		{

			// Create a hash table to tokenize the column names in an execution document.
			ExecutionColumnType.hashtable = new Hashtable();
			ExecutionColumnType.hashtable["rowType"] = ExecutionColumnType.RowType;
			ExecutionColumnType.hashtable["brokerId"] = ExecutionColumnType.BrokerId;
			ExecutionColumnType.hashtable["brokerName"] = ExecutionColumnType.BrokerName;
			ExecutionColumnType.hashtable["brokerSymbol"] = ExecutionColumnType.BrokerSymbol;
			ExecutionColumnType.hashtable["executionId"] = ExecutionColumnType.ExecutionId;
			ExecutionColumnType.hashtable["quantity"] = ExecutionColumnType.Quantity;
			ExecutionColumnType.hashtable["price"] = ExecutionColumnType.Price;
			ExecutionColumnType.hashtable["commission"] = ExecutionColumnType.Commission;
			ExecutionColumnType.hashtable["accruedInterest"] = ExecutionColumnType.AccruedInterest;
			ExecutionColumnType.hashtable["userFee0"] = ExecutionColumnType.UserFee0;
			ExecutionColumnType.hashtable["userFee1"] = ExecutionColumnType.UserFee1;
			ExecutionColumnType.hashtable["userFee2"] = ExecutionColumnType.UserFee2;
			ExecutionColumnType.hashtable["userFee3"] = ExecutionColumnType.UserFee3;
			ExecutionColumnType.hashtable["tradeDate"] = ExecutionColumnType.TradeDate;
			ExecutionColumnType.hashtable["settlementDate"] = ExecutionColumnType.SettlementDate;
			ExecutionColumnType.hashtable["createdTime"] = ExecutionColumnType.CreatedTime;
			ExecutionColumnType.hashtable["createdLoginId"] = ExecutionColumnType.CreatedLoginId;
			ExecutionColumnType.hashtable["createdLoginName"] = ExecutionColumnType.CreatedLoginName;
			ExecutionColumnType.hashtable["modifiedTime"] = ExecutionColumnType.ModifiedTime;
			ExecutionColumnType.hashtable["modifiedLoginId"] = ExecutionColumnType.ModifiedLoginId;
			ExecutionColumnType.hashtable["modifiedLoginName"] = ExecutionColumnType.ModifiedLoginName;

		}

		public static int Tokenize(string text)
		{

			// Return the tokenized value or 'None' if the token doesn't exist.
			return ExecutionColumnType.hashtable.ContainsKey(text) ? Convert.ToInt32(ExecutionColumnType.hashtable[text]) : ExecutionColumnType.None;

		}

	};

}
