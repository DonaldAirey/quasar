namespace MarkThree
{

	using System;

	/// <summary>
	/// Window style bits
	/// </summary>
	public enum WindowStyle : uint
	{

		Overlapped = 0x00000000,
		Popup = 0x80000000,
		Child = 0x40000000,
		Minimize = 0x20000000,
		Visible = 0x10000000,
		Disabled = 0x08000000,
		Clipsiblings = 0x04000000,
		Clipchildren = 0x02000000,
		Maximize = 0x01000000,
		Caption = 0x00c00000,
		Border = 0x00800000,
		Dlgframe = 0x00400000,
		Vscroll = 0x00200000,
		Hscroll = 0x00100000,
		Sysmenu = 0x00080000,
		Thickframe = 0x00040000,
		Group = 0x00020000,
		Tabstop = 0x00010000,
		Minimizebox = 0x00020000,
		Maximizebox = 0x00010000

	}

}
