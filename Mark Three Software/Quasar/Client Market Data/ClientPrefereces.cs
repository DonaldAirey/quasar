/*************************************************************************************************************************
*
*	File:			ClientPreferences.cs
*	Description:	Manages the client's user preferences as well as the attributes of the user.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.Client;
using System;

namespace Shadows.Quasar.Client
{

	/// <summary>
	/// Summary description for UserPrefereces.
	/// </summary>
	public class ClientPreferences : Preferences
	{

		private static int loginId;

		/// <summary>
		/// The identifier of this user.
		/// </summary>
		public static int LoginId {get {return loginId;}}

		static ClientPreferences()
		{

			// The starting point for the folder is the login ID.  The id will direct us to the user's personal folder and
			// that will drive the rest of the tree structure.
			WebClient webClient = new WebClient();
			ClientPreferences.loginId = webClient.GetLoginId();

		}

	}

}
