using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Specifies the way an order is priced.
	/// </summary>
	public enum Approval
	{

		/// <summary>No Approval is required to trade.</summary>
		None = 0,
		/// <summary>One authorization needed to trade.</summary>
		One = 1,
		/// <summary>Two authorizations needed to trade.</summary>
		Two = 2,
		/// <summary>Compliance Officer authorization needed to trade.</summary>
		Officer = 3

	};

}
