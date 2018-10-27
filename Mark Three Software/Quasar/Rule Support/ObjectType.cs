using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Summary description for Shadows.Quasar.Rule.ObjectType
	/// </summary>
	public enum ObjectType
	{

		/// <summary>A System Folder.</summary>
		SystemFolder = 1,
		/// <summary>A General Purpose Folder.</summary>
		Folder = 2,
		/// <summary>An Account Group.</summary>
		AccountGroup = 3,
		/// <summary>An Individual Account.</summary>
		Account = 4,
		/// <summary>A Sub-Account.</summary>
		SubAccount = 5,
		/// <summary>A Trading Desk.</summary>
		Desk = 6,
		/// <summary>A Trader.</summary>
		Trader = 7,
		/// <summary>A Security Classification Scheme.</summary>
		Scheme = 8,
		/// <summary>A Security Sector.</summary>
		Sector = 9,
		/// <summary>A Security.</summary>
		Security = 10,
		/// <summary>A Sector Model.</summary>
		SectorModel = 11,
		/// <summary>A Security Model.</summary>
		SecurityModel = 12,
		/// <summary>A User in this Data Model.</summary>
		Login = 13,
		/// <summary>A Broker.</summary>
		Broker = 14

	};

}
