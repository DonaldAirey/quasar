using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Summary description for TimeInForce.
	/// </summary>
	public enum TIF
	{
		/// <summary>Day Order - Good from now until the market closes</summary>
		DAY = 0,
		/// <summary>Good 'til Canceled</summary>
		GTC = 1,
		/// <summary>Fill or Kill</summary>
		FOK = 2,
		/// <summary>Immediate or Cancel</summary>
		IOC = 3,
		/// <summary>Execute on Market Opening</summary>
		OPG = 4,
		/// <summary>Execute on Market Close</summary>
		CLO = 5
	};

}
