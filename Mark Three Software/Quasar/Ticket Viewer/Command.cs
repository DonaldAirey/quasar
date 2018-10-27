/*************************************************************************************************************************
*
*	File:			Command.cs
*	Description:	Commands for the Ticket Viewer.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Ticket
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
		/// <summary>Initialize a new execution record.</summary>
		public const int Initialize = 6;
		/// <summary>Update a global record.</summary>
		public const int UpdateGlobal = 7;
		/// <summary>Delete a global record.</summary>
		public const int DeleteGlobal = 8;
		/// <summary>Set the broker on a global record.</summary>
		public const int SetGlobalBrokerSymbol = 9;
		/// <summary>Set the quantity of a global record.</summary>
		public const int SetGlobalQuantity = 10;
		/// <summary>Set the price of a global record.</summary>
		public const int SetGlobalPrice = 11;
		/// <summary>Set the commission of a global record.</summary>
		public const int SetGlobalCommission = 12;
		/// <summary>Set the accrued interest of a global record.</summary>
		public const int SetGlobalAccruedInterest = 13;
		/// <summary>Set the user fee 0 of a global record.</summary>
		public const int SetGlobalUserFee0 = 14;
		/// <summary>Set the user fee 1 of a global record.</summary>
		public const int SetGlobalUserFee1 = 15;
		/// <summary>Set the user fee 2 of a global record.</summary>
		public const int SetGlobalUserFee2 = 16;
		/// <summary>Set the user fee 3 of a global record.</summary>
		public const int SetGlobalUserFee3 = 17;
		/// <summary>Set the trade date of a global record.</summary>
		public const int SetGlobalTradeDate = 18;
		/// <summary>Set the settlement date of a global record.</summary>
		public const int SetGlobalSettlementDate = 19;

	};

}
