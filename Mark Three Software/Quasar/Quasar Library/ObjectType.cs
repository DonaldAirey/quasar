/*************************************************************************************************************************
*
*	File:			Type.cs
*	Description:	Identifies financial object used by the system (Account, Security, Model, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Indicates the type of object (account, hierarchy, model, user, etc.) 
	/// </summary>
	public class Type
	{

		/// <summary>An Account</summary>
		public const int Account = 0;
		/// <summary>A View of Orders</summary>
		public const int Blotter = 1;
		/// <summary>A Broker</summary>
		public const int Broker = 2;
		/// <summary>A Folder</summary>
		public const int Folder = 3;
		/// <summary>An entity that issues debt or stock.</summary>
		public const int Issuer = 4;
		/// <summary>A login used to identify a user.</summary>
		public const int Login = 5;
		/// <summary>An abstract portfolio.</summary>
		public const int Model = 6;
		/// <summary>A method of categorizing security (SIC, GICS, etc.)</summary>
		public const int Scheme = 7;
		/// <summary>A classification for a sector of security (Technology, Consumer Durables, etc.).</summary>
		public const int Sector = 8;
		/// <summary>A Security.</summary>
		public const int Security = 9;
		/// <summary>A Ticket.</summary>
		public const int Ticket = 10;

	};

}
