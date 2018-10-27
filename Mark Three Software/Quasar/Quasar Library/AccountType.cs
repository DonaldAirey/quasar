/*************************************************************************************************************************
*
*	File:			AccountType.cs
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
	public class AccountType
	{

		/// <summary>A Group Account</summary>
		public const int Group = 0;
		/// <summary>An Individual Account/Fund</summary>
		public const int Individual = 1;
		/// <summary>A Sub Account/Fund</summary>
		public const int Sub = 2;

	};

}
