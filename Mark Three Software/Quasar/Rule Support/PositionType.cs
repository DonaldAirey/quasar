using System;
using System.Collections;

namespace Shadows.Quasar.Rule
{


	/// <summary>Long or Short position type.</summary>
	public class PositionTypes
	{

		private static ArrayList arrayList;

		public PositionTypes()
		{

			PositionTypes.arrayList = new ArrayList();
			PositionTypes.arrayList.Add(PositionType.Long);
			PositionTypes.arrayList.Add(PositionType.Short);

		}

		public IEnumerator GetEnumerator()
		{
			return PositionTypes.arrayList.GetEnumerator();
		}

	};
	
	/// <summary>Long or Short position type.</summary>
	public enum PositionType
	{

		/// <summary>A security that is owned.</summary>
		Long = 0,
		/// <summary>A security that is borrowed.</summary>
		Short = 1

	};

}
