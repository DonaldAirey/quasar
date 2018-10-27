namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for ChRGB.
	/// </summary>
	public class ChRGB
	{

		public static object Get(Color color)
		{
			if (color == Color.Transparent)
				return 0xFFFFFFFE;

			return color.B << 16 | color.G << 8 | color.R;

		}

	}

}
