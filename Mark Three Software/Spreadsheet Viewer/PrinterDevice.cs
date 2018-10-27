namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	/// <summary>This class provides access to printer device settings.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class PrinterDevice
	{

		private const int physicalOffsetX = 112;
		private const int physicalOffsetY = 113;

		/// <summary>
		/// Get the settings from the device context.
		/// </summary>
		/// <param name="hdc">A handle to a device context.</param>
		/// <param name="index">An index to the setting.</param>
		/// <returns></returns>
		[DllImport("gdi32.dll")]
		private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 index);

		/// <summary>
		/// Calculates the hard margins of a sheet of paper.
		/// </summary>
		/// <param name="graphics">The Graphics of the device.</param>
		/// <param name="marginBounds">The unadjusted margin boundary.</param>
		/// <returns>A rectangle that defines the margin boundaries.</returns>
		public static Rectangle HardMargins(Graphics graphics, Rectangle marginBounds)
		{

			// Get the physical offsets for the X and Y directions from the printer device.
			IntPtr hDC = graphics.GetHdc();
			int hardMarginLeft = GetDeviceCaps(hDC , physicalOffsetX);
			int hardMarginTop  = GetDeviceCaps(hDC , physicalOffsetY);
			graphics.ReleaseHdc(hDC);

			// Adjust the margin rectangle by the offsets after converting them to device units.
			hardMarginLeft = (int)(hardMarginLeft * 100.0 / graphics.DpiX);
			hardMarginTop  = (int)(hardMarginTop  * 100.0 / graphics.DpiY);
			marginBounds.Offset(-hardMarginLeft , -hardMarginTop);

			// This rectangle properly reflects the requested margins.
			return marginBounds;

		}

	}

}
