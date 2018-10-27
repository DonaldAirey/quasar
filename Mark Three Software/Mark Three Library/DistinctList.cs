namespace MarkThree
{

	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for PositionTable.
	/// </summary>
	public class DistinctList : ArrayList
	{

		public new object Add(object distinctObject)
		{

			int index = BinarySearch(distinctObject);
			if (index < 0)
				Insert(~index, distinctObject);
			else
				distinctObject = this[index];

			return distinctObject;

		}

	}

}
