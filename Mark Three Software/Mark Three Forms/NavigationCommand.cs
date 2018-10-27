namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// Commands for navigating a spreadsheet control.
	/// </summary>
	public enum NavigationCommand
	{
		Up, Down, Left, Right,
		Enter, Nothing,
		StartOfRow, StartOfDocument,
		EndOfRow, EndOfDocument,
		PageUp, PageDown
	}

}
