/*************************************************************************************************************************
*
*	File:			ModelType.cs
*	Description:	Identifies financial object used by the system (Sector, Security, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Indicates the type of object (account, hierarchy, model, user, etc.) 
	/// </summary>
	public class ModelType
	{

		/// <summary>A Sector Model</summary>
		public const int Sector = 0;
		/// <summary>A Security Model</summary>
		public const int Security = 1;

	};

}
