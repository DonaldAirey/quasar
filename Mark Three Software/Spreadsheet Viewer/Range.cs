namespace MarkThree.Forms
{

	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for Range.
	/// </summary>
	public struct Range : IComparable
	{

		public int Row;
		public int Column;
		public int Rows;
		public int Columns;

		public Range(int row, int column)
		{

			// Initialize the object
			this.Row = row;
			this.Columns = column;
			this.Rows = 1;
			this.Column = 1;

		}

		public Range(int row, int column, int rows, int columns)
		{

			// Initialize the object
			this.Row = row;
			this.Column = column;
			this.Rows = rows;
			this.Columns = columns;

		}

		public bool IsEmpty {get {return this.Rows == 0 && this.Columns == 0;}}

		public static Range Empty {get {return new Range(0, 0, 0, 0);}}

		#region IComparable Members

		public int CompareTo(object obj)
		{

			if (obj is Range)
			{
				Range operand = (Range)obj;
				if (this.Row < operand.Row)
					return -1;
				if (this.Row > operand.Row)
					return 1;
				if (this.Column < operand.Column)
					return -1;
				if (this.Column > operand.Column)
					return 1;
			}

			return 0;

		}

		#endregion

	}

}
