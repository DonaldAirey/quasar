/*************************************************************************************************************************
*
*	File:			FolderType.cs
*	Description:	Identifies financial object used by the system (Folder, Security, Model, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Indicates the type of object (account, hierarchy, model, user, etc.) 
	/// </summary>
	public class FolderType
	{

		/// <summary>A System Folder</summary>
		public const int System = 0;
		/// <summary>Normal File Folder</summary>
		public const int File = 1;

	};

}
