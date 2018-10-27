namespace MarkThree
{

	using System;

	/// <summary>
	/// Extended Window Styles
	/// </summary>
	public enum ExtendedWindowStyle : uint
	{
		Dlgmodalframe = 0x00000001,
		Noparentnotify = 0x00000004,
		Topmost = 0x00000008,
		Acceptfiles = 0x00000010,
		Transparent = 0x00000020,
		Mdichild = 0x00000040,
		Toolwindow = 0x00000080,
		Windowedge = 0x00000100,
		Clientedge = 0x00000200,
		Contexthelp = 0x00000400,
		Right = 0x00001000,
		Left = 0x00000000,
		Rtlreading = 0x00002000,
		Ltrreading = 0x00000000,
		Leftscrollbar = 0x00004000,
		Rightscrollbar = 0x00000000,
		Controlparent = 0x00010000,
		Staticedge = 0x00020000,
		Appwindow = 0x00040000,
	}
}
