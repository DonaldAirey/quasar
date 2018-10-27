namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Collections;

	/// <summary>
	/// Type Converter for FIX OrdType Field
	/// </summary>
	public class OrdTypeConverter
	{

		// Private Members
		private static Hashtable fromTable;
		private static Hashtable toTable;
		private static object[,] pairs =
		{
			{OrdType.Market, "1"},
			{OrdType.Limit, "2"},
			{OrdType.Stop, "3"},
			{OrdType.StopLimit, "4"},
			{OrdType.MarketOnClose, "5"},
			{OrdType.WithOrWithout, "6"},
			{OrdType.LimitOrBetter, "7"},
			{OrdType.LimitWithOrWithout, "8"},
			{OrdType.OnBasis, "9"},
			{OrdType.OnClose, "A"},
			{OrdType.LimitOnClose, "B"},
			{OrdType.PreviouslyQuoted, "D"},
			{OrdType.PreviouslyIndicated, "E"},
			{OrdType.ForexLimit, "F"},
			{OrdType.ForexSwap, "G"},
			{OrdType.ForexPreviouslyIndicated, "H"},
			{OrdType.Funarii, "I"},
			{OrdType.MarketIfTouched, "J"},
			{OrdType.MarketWithLeftoverAsLimit, "K"},
			{OrdType.PreviousFundValuationPoint, "L"},
			{OrdType.NextFundValuationPoint, "M"},
			{OrdType.Pegged, "P"},
		};

		/// <summary>
		/// Initializes the shared members of a OrdTypeConverter.
		/// </summary>
		static OrdTypeConverter()
		{

			// Initialize the mapping of strings to OrdType.
			OrdTypeConverter.fromTable = new Hashtable();
			for (int element = 0; element < pairs.GetLength(0); element++)
				OrdTypeConverter.fromTable.Add(pairs[element, 1], pairs[element, 0]);

			// Initialize the mapping of OrdType to strings.
			OrdTypeConverter.toTable = new Hashtable();
			for (int element = 0; element < pairs.GetLength(0); element++)
				OrdTypeConverter.toTable.Add(pairs[element, 0], pairs[element, 1]);

		}

		/// <summary>
		/// Converts a string to a OrdType.
		/// </summary>
		/// <param name="value">The FIX string representation of a OrdType.</param>
		/// <returns>A OrdType value.</returns>
		public static OrdType ConvertFrom(string value) {return (OrdType)OrdTypeConverter.fromTable[value];}

		/// <summary>
		/// Converts a OrdType to a string.
		/// </summary>
		/// <returns>A OrdType value.</returns>
		/// <param name="value">The FIX string representation of a OrdType.</param>
		public static string ConvertTo(OrdType messageType) {return (string)OrdTypeConverter.toTable[messageType];}

	}

}
