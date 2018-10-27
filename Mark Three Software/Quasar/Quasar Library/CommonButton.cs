/*************************************************************************************************************************
*
*	File:			CommonButton.cs
*	Description:	Common Buttons used by the container.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Common Buttons for the Container.
	/// </summary>
	public enum CommonButton
	{
		/// <summary>
		/// Selects the Web Browser and the Quasar home page.
		/// </summary>
		QuasarToday,
		/// <summary>
		/// Prints the currently viewed document.
		/// </summary>
		Print,
		/// <summary>
		/// Previewes the document before printing.
		/// </summary>
		PrintPreview,
		/// <summary>
		/// Move the object from one folder to another.
		/// </summary>
		MoveToFolder,
		/// <summary>
		/// Delete the current object.
		/// </summary>
		Delete,
		/// <summary>
		///  Cut to clipboard.
		/// </summary>
		Cut,
		/// <summary>
		/// Copy to clipboard.
		/// </summary>
		Copy,
		/// <summary>
		/// Paste from clipboard.
		/// </summary>
		Paste
	};

}
