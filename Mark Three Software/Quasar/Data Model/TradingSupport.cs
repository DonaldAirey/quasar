namespace MarkThree.Quasar
{

	using System;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for TradingSupport.
	/// </summary>
	public class TradingSupport
	{

		public static int AutoRoute(DataModel.SecurityRow securityRow, decimal quantity)
		{

			// Use a blotter route for the security if it exists.
			foreach (DataModel.BlotterMapRow blotterMapRow in securityRow.GetBlotterMapRows())
				if (blotterMapRow.MinimumQuantity <= quantity && (blotterMapRow.IsMaximumQuantityNull() ||
					quantity <= blotterMapRow.MaximumQuantity))
					return blotterMapRow.BlotterId;
			
			// Return the default blotter.
			foreach (DataModel.BlotterRow blotterRow in DataModel.Blotter)
				if (blotterRow.DefaultBlotter)
					return blotterRow.BlotterId;

			throw new Exception("Default Blotter is not set.");
			
		}

	}

}
