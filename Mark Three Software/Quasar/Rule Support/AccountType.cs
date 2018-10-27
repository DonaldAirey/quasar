/*************************************************************************************************************************
*
*	File:			AccountType.cs
*	Description:	Identifies financial object used by the system (Account, Security, Model, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Indicates the type of object (account, hierarchy, model, user, etc.) 
	/// </summary>
	public enum AccountType
	{

		/// <summary>A Group Account</summary>
		Group = 0,
		/// <summary>An Individual Account/Fund</summary>
		Individual = 1,
		/// <summary>A Sub Account/Fund</summary>
		SubAccount = 2

	};

}
