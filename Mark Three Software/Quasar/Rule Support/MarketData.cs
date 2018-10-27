namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Client;
	using System;

	/// <summary>
	/// Summary description for MarketData.
	/// </summary>
	public class MarketData
	{

		public static System.EventHandler BeginMerge;
		public static System.EventHandler EndMerge;
	
		static MarketData()
		{

			ClientMarketData.BeginMerge += new EventHandler(BeginMergeHandler);
			ClientMarketData.EndMerge += new EventHandler(EndMergeHandler);

		}

		private static void BeginMergeHandler(object sender, EventArgs eventArgs)
		{

			if (BeginMerge != null)
				BeginMerge(typeof(MarketData), new EventArgs());

			ProposedOrder.OnBeginMerge();
			TaxLot.OnBeginMerge();

		}
	
		private static void EndMergeHandler(object sender, EventArgs eventArgs)
		{

			TaxLot.OnEndMerge();
			ProposedOrder.OnEndMerge();
			
			if (EndMerge != null)
				EndMerge(typeof(MarketData), new EventArgs());


			CommandBatch.Flush();

		}

	}

}
