using MarkThree.Quasar;
using System;
using System.Data;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Summary description for Algorithm.
	/// </summary>
	public class Algorithm
	{

		/// <summary>Algorithm to rebalance security to percent of market value.</summary>
		public const int SecurityRebalancer = 0;
		/// <summary>Algorithm to rebalance sector to percent of individual account market value.</summary>
		public const int SectorWrapRebalancer = 1;
		/// <summary>Algorithm to rebalance sector to percent of aggregate market value.</summary>
		public const int SectorMergeRebalancer = 2;
		/// <summary>Algorithm to rebalance only the selected securities to security targets.</summary>
		public const int SelectedSecurityRebalancer = 3;
		/// <summary>Multicurrency blocking of orders.</summary>
		public const int MulticurrencyBlocking = 4;
		/// <summary>Security blocking of orders.</summary>
		public const int SecurityBlocking = 5;
		/// <summary>Account blocking of orders.</summary>
		public const int AccountBlocking = 6;
		/// <summary>Pro Rata Allocation.</summary>
		public const int ProRataAllocation = 7;

	}

}
