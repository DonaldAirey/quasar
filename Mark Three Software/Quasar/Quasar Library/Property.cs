/*************************************************************************************************************************
*
*	File:			Property.cs
*	Description:	General purpose integers that are used to identify the property associated with object
*					For example, the security classification scheme is a property that is associated with an appraisal.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used to get or set the property on database object.
	/// </summary>
	public class Property
	{
		/// <summary>Scheme</summary>
		public const int Scheme = 0;
		/// <summary>Model</summary>
		public const int Model = 1;
		/// <summary>Time in Force</summary>
		public const int TimeInForce = 2;
		/// <summary>Blotter</summary>
		public const int Blotter = 3;
		/// <summary>Name</summary>
		public const int Name = 4;
		/// <summary>Description</summary>
		public const int Description = 5;
		/// <summary>Algorithm</summary>
		public const int Algorithm = 6;
		/// <summary>Read Only</summary>
		public const int ReadOnly = 7;
		/// <summary>Hidden</summary>
		public const int Hidden = 8;
		/// <summary>Deleted</summary>
		public const int Deleted = 9;
		/// <summary>Security Self</summary>
		public const int SecuritySelf = 10;
		/// <summary>Sector Self</summary>
		public const int SectorSelf = 11;
		/// <summary>Currency</summary>
		public const int Currency = 12;
		/// <summary>Lot Handling</summary>
		public const int LotHandling = 13;
		/// <summary>Name 0</summary>
		public const int Name0 = 14;
		/// <summary>Name 1</summary>
		public const int Name1 = 15;
		/// <summary>Name 2</summary>
		public const int Name2 = 16;
		/// <summary>Name 3</summary>
		public const int Name3 = 17;
		/// <summary>Address 0</summary>
		public const int Address0 = 18;
		/// <summary>Address 1</summary>
		public const int Address1 = 19;
		/// <summary>Address 2</summary>
		public const int Address2 = 20;
		/// <summary>City</summary>
		public const int City = 21;
		/// <summary>Province</summary>
		public const int Province = 22;
		/// <summary>Country</summary>
		public const int Country = 23;
		/// <summary>Postal Code</summary>
		public const int PostalCode = 24;
		/// <summary>Show Home Page</summary>
		public const int ShowHomePage = 25;
		/// <summary>Home Page URL</summary>
		public const int HomePageURL = 26;
		/// <summary>Default</summary>
		public const int Default = 27;
		/// <summary>Stylesheet</summary>
		public const int Stylesheet = 28;
		/// <summary>Blocking Code</summary>
		public const int BlockingCode = 29;
		/// <summary>Block Order Stylesheet</summary>
		public const int BlockOrderStylesheet = 30;
		/// <summary>Placement Stylesheet</summary>
		public const int PlacementStylesheet = 31;
		/// <summary>Execution Stylesheet</summary>
		public const int ExecutionStylesheet = 32;
		/// <summary>Folder</summary>
		public const int Folder = 33;
		/// <summary>Ticket Stylesheet</summary>
		public const int TicketStylesheet = 34;
		/// <summary>Symbol</summary>
		public const int Symbol = 35;
		/// <summary>Phone</summary>
		public const int Phone = 36;
		/// <summary>User Id 0</summary>
		public const int ExternalId0 = 37;
		/// <summary>User Id 1</summary>
		public const int ExternalId1 = 38;
		/// <summary>User Id 2</summary>
		public const int ExternalId2 = 39;
		/// <summary>User Id 3</summary>
		public const int ExternalId3 = 40;
		/// <summary>User Id 4</summary>
		public const int ExternalId4 = 41;
		/// <summary>User Id 5</summary>
		public const int ExternalId5 = 42;
		/// <summary>User Id 6</summary>
		public const int ExternalId6 = 43;
		/// <summary>User Id 7</summary>
		public const int ExternalId7 = 44;
		/// <summary>StylesheetType</summary>
		public const int StylesheetType = 45;
		/// <summary>Text</summary>
		public const int Text = 46;
		/// <summary>AccountIdColumn</summary>
		public const int AccountIdColumn = 47;
		/// <summary>AlgorithmIdColumn</summary>
		public const int AlgorithmIdColumn = 48;
		/// <summary>BlockingCodeColumn</summary>
		public const int BlockingCodeColumn = 50;
		/// <summary>BlotterIdColumn</summary>
		public const int BlotterIdColumn = 51;
		/// <summary>BrokerIdColumn</summary>
		public const int BrokerIdColumn = 52;
		/// <summary>childIdColumn</summary>
		public const int childIdColumn = 53;
		/// <summary>ConditionCodeColumn</summary>
		public const int ConditionCodeColumn = 54;
		/// <summary>CountryIdColumn</summary>
		public const int CountryIdColumn = 55;
		/// <summary>CurrencyIdColumn</summary>
		public const int CurrencyIdColumn = 56;
		/// <summary>StylesheetTypeCodeColumn</summary>
		public const int StylesheetTypeCodeColumn = 57;
		/// <summary>ExchangeIdColumn</summary>
		public const int ExchangeIdColumn = 58;
		/// <summary>FolderIdColumn</summary>
		public const int FolderIdColumn = 59;
		/// <summary>HolidayTypeCodeColumn</summary>
		public const int HolidayTypeCodeColumn = 60;
		/// <summary>IssuerIdColumn</summary>
		public const int IssuerIdColumn = 61;
		/// <summary>ObjectIdColumn</summary>
		public const int ObjectIdColumn = 62;
		/// <summary>LotHandlingIdColumn</summary>
		public const int LotHandlingIdColumn = 63;
		/// <summary>ModelIdColumn</summary>
		public const int ModelIdColumn = 64;
		/// <summary>ObjectIdColumn</summary>
		public const int LoginIdColumn = 65;
		/// <summary>TypeCodeColumn</summary>
		public const int TypeCodeColumn = 66;
		/// <summary>OrderTypeCodeColumn</summary>
		public const int OrderTypeCodeColumn = 67;
		/// <summary>parentIdColumn</summary>
		public const int parentIdColumn = 68;
		/// <summary>PositionTypeCodeColumn</summary>
		public const int PositionTypeCodeColumn = 69;
		/// <summary>PropertyCodeColumn</summary>
		public const int PropertyCodeColumn = 70;
		/// <summary>ProvinceIdColumn</summary>
		public const int ProvinceIdColumn = 71;
		/// <summary>SchemeIdColumn</summary>
		public const int SchemeIdColumn = 72;
		/// <summary>SectorIdColumn</summary>
		public const int SectorIdColumn = 73;
		/// <summary>SecurityIdColumn</summary>
		public const int SecurityIdColumn = 74;
		/// <summary>SecurityTypeCodeColumn</summary>
		public const int SecurityTypeCodeColumn = 75;
		/// <summary>TaxLotIdColumn</summary>
		public const int TaxLotIdColumn = 76;
		/// <summary>StylesheetIdColumn</summary>
		public const int StylesheetIdColumn = 77;
		/// <summary>TimeInForceCodeColumn</summary>
		public const int TimeInForceCodeColumn = 78;
		/// <summary>TransactionTypeCodeColumn</summary>
		public const int TransactionTypeCodeColumn = 79;
		/// <summary>Status</summary>
		public const int StatusCode = 80;
		/// <summary>Connected</summary>
		public const int Connected = 81;
		/// <summary>User Data 0</summary>
		public const int UserData0 = 82;
		/// <summary>User Data 1</summary>
		public const int UserData1 = 83;
		/// <summary>User Data 2</summary>
		public const int UserData2 = 84;
		/// <summary>User Data 3</summary>
		public const int UserData3 = 85;
		/// <summary>User Data 4</summary>
		public const int UserData4 = 86;
		/// <summary>User Data 5</summary>
		public const int UserData5 = 87;
		/// <summary>User Data 6</summary>
		public const int UserData6 = 88;
		/// <summary>User Data 7</summary>
		public const int UserData7 = 89;
		/// <summary>Mnemonic</summary>
		public const int Mnemonic = 90;
		/// <summary>Algorithm Type Code</summary>
		public const int AlgorithmTypeCode = 91;
		/// <summary>Module Name (for DLLs)</summary>
		public const int Assembly = 92;
		/// <summary>Type Name (for DLLs).</summary>
		public const int Type = 93;
		/// <summary>Severity Level (for errors).</summary>
		public const int Severity = 94;
		/// <summary>Approval needed for compliance overrides.</summary>
		public const int Approval = 95;
		/// <summary>Indicates a record with a brief lifetime.</summary>
		public const int Temporary = 96;
		/// <summary>Method name (for DLLs).</summary>
		public const int Method = 97;

	}

}
