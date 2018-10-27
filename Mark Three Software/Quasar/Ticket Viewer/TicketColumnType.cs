/*************************************************************************************************************************
*
*	File:			TicketDocument.cs
*	Description:	The classes to control the DOM for the blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Collections;

namespace Shadows.Quasar.Viewers.Ticket
{
	/// <summary>
	/// Enumeration for the column types in the appraisal viewer.
	/// </summary>
	public class TicketColumnType
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

		static TicketColumnType()
		{

			// Create a hash table to tokenize the column names in an execution document.
			TicketColumnType.hashtable = new Hashtable();
			TicketColumnType.hashtable["rowType"] = TicketColumnType.RowType;
			TicketColumnType.hashtable["brokerId"] = TicketColumnType.BrokerId;
			TicketColumnType.hashtable["brokerName"] = TicketColumnType.BrokerName;
			TicketColumnType.hashtable["brokerSymbol"] = TicketColumnType.BrokerSymbol;
			TicketColumnType.hashtable["executionId"] = TicketColumnType.ExecutionId;
			TicketColumnType.hashtable["quantity"] = TicketColumnType.Quantity;
			TicketColumnType.hashtable["price"] = TicketColumnType.Price;
			TicketColumnType.hashtable["commission"] = TicketColumnType.Commission;
			TicketColumnType.hashtable["accruedInterest"] = TicketColumnType.AccruedInterest;
			TicketColumnType.hashtable["userFee0"] = TicketColumnType.UserFee0;
			TicketColumnType.hashtable["userFee1"] = TicketColumnType.UserFee1;
			TicketColumnType.hashtable["userFee2"] = TicketColumnType.UserFee2;
			TicketColumnType.hashtable["userFee3"] = TicketColumnType.UserFee3;
			TicketColumnType.hashtable["tradeDate"] = TicketColumnType.TradeDate;
			TicketColumnType.hashtable["settlementDate"] = TicketColumnType.SettlementDate;
			TicketColumnType.hashtable["createdTime"] = TicketColumnType.CreatedTime;
			TicketColumnType.hashtable["createdLoginId"] = TicketColumnType.CreatedLoginId;
			TicketColumnType.hashtable["createdLoginName"] = TicketColumnType.CreatedLoginName;
			TicketColumnType.hashtable["modifiedTime"] = TicketColumnType.ModifiedTime;
			TicketColumnType.hashtable["modifiedLoginId"] = TicketColumnType.ModifiedLoginId;
			TicketColumnType.hashtable["modifiedLoginName"] = TicketColumnType.ModifiedLoginName;

		}

		public static int Tokenize(string text)
		{

			// Return the tokenized value or 'None' if the token doesn't exist.
			return TicketColumnType.hashtable.ContainsKey(text) ? Convert.ToInt32(TicketColumnType.hashtable[text]) : TicketColumnType.None;

		}

	};

}
