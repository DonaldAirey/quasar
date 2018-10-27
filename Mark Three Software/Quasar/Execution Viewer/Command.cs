/*************************************************************************************************************************
*
*	File:			Command.cs
*	Description:	Commands for the Execution Viewer
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// Commands handled by this viewer.
	/// </summary>
	internal class Command
	{

		/// <summary>Not a valid command</summary>
		public const int None = 0;
		/// <summary>Open the Viewer</summary>
		public const int OpenViewer = 1;
		/// <summary>Close the Viewer</summary>
		public const int CloseViewer = 2;
		/// <summary>Open the Document</summary>
		public const int OpenDocument = 3;
		/// <summary>Close the Document</summary>
		public const int CloseDocument = 4;
		/// <summary>Draw the document in the viewer</summary>
		public const int Draw = 5;
		/// <summary>Opens a block order in the viewer</summary>
		public const int OpenBlockOrder = 6;
		/// <summary>Initialize a new execution record.</summary>
		public const int Initialize = 7;
		/// <summary>Update a local record.</summary>
		public const int UpdateLocal = 8;
		/// <summary>Delete a local record.</summary>
		public const int DeleteLocal = 9;
		/// <summary>Clear a local record.</summary>
		public const int ClearLocal = 10;
		/// <summary>Set a broker for a local record.</summary>
		public const int SetLocalBrokerSymbol = 11;
		/// <summary>Set the quantity of a local record.</summary>
		public const int SetLocalQuantity = 12;
		/// <summary>Set the price of a local record.</summary>
		public const int SetLocalPrice = 13;
		/// <summary>Set the commission of a local record.</summary>
		public const int SetLocalCommission = 14;
		/// <summary>Set the accrued interest of a local record.</summary>
		public const int SetLocalAccruedInterest = 15;
		/// <summary>Set the user fee 0 of a local record.</summary>
		public const int SetLocalUserFee0 = 16;
		/// <summary>Set the user fee 1 of a local record.</summary>
		public const int SetLocalUserFee1 = 17;
		/// <summary>Set the user fee 2 of a local record.</summary>
		public const int SetLocalUserFee2 = 18;
		/// <summary>Set the user fee 3 of a local record.</summary>
		public const int SetLocalUserFee3 = 19;
		/// <summary>Set the trade date of a local record.</summary>
		public const int SetLocalTradeDate = 20;
		/// <summary>Set the settlement date of a local record.</summary>
		public const int SetLocalSettlementDate = 21;
		/// <summary>Update a global record.</summary>
		public const int UpdateGlobal = 22;
		/// <summary>Delete a global record.</summary>
		public const int DeleteGlobal = 23;
		/// <summary>Set the broker on a global record.</summary>
		public const int SetGlobalBrokerSymbol = 24;
		/// <summary>Set the quantity of a global record.</summary>
		public const int SetGlobalQuantity = 25;
		/// <summary>Set the price of a global record.</summary>
		public const int SetGlobalPrice = 26;
		/// <summary>Set the commission of a global record.</summary>
		public const int SetGlobalCommission = 27;
		/// <summary>Set the accrued interest of a global record.</summary>
		public const int SetGlobalAccruedInterest = 28;
		/// <summary>Set the user fee 0 of a global record.</summary>
		public const int SetGlobalUserFee0 = 29;
		/// <summary>Set the user fee 1 of a global record.</summary>
		public const int SetGlobalUserFee1 = 30;
		/// <summary>Set the user fee 2 of a global record.</summary>
		public const int SetGlobalUserFee2 = 31;
		/// <summary>Set the user fee 3 of a global record.</summary>
		public const int SetGlobalUserFee3 = 32;
		/// <summary>Set the trade date of a global record.</summary>
		public const int SetGlobalTradeDate = 33;
		/// <summary>Set the settlement date of a global record.</summary>
		public const int SetGlobalSettlementDate = 34;

	};

}
