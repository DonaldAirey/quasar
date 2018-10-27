namespace MarkThree.Forms
{

	using System;

	public class Series
	{

		public object[] values;
		public object[] format;

		/// <summary>
		/// Constructor for an empty series of a given size.
		/// </summary>
		/// <param name="size"></param>
		public Series(int size)
		{

			this.values = new object[size];
			this.format = new object[size];

		}

	}

}
