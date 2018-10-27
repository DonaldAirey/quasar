/*************************************************************************************************************************
*
*	File:			Command.cs
*	Description:	Commands for the Blotter Viewer
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Blotter
{

	/// <summary>
	/// Commands handled by this viewer.
	/// </summary>
	internal class Command
	{

		/// <summary>Not a valid command</summary>
		public const int None = 0;
		/// <summary>Opens the Viewer</summary>
		public const int OpenViewer = 1;
		/// <summary>Close the Viewer</summary>
		public const int CloseViewer = 2;
		/// <summary>Opens the Document</summary>
		public const int OpenDocument = 3;
		/// <summary>Close the Document</summary>
		public const int CloseDocument = 4;

	};

}
