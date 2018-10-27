using System;

namespace MarkThree.Guardian
{

	/// <summary>
	/// Identifies blocking styles used by blotter to group order into blocks.
	/// </summary>
	public class BlockingStyle
	{

		/// <summary>No blocking</summary>
		public const int None = 0;
		/// <summary>Group trades by the security.</summary>
		public const int Security = 1;
		/// <summary>Group trades by security and settlement currency.</summary>
		public const int SecurityCurrency = 2;
		/// <summary>Group trades by account, security and settlement currency.</summary>
		public const int AccountSecurityCurrency = 3;

	};
}
