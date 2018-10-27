using System;

namespace Shadows.Quasar.Rule
{

	/// <summary>
	/// Specifies the way an order is priced.
	/// </summary>
	public enum Action
	{

		/// <summary>The record is unchanged.</summary>
		Nothing,
		/// <summary>The record has been added.</summary>
		Add,
		/// <summary>The record has been deleted.</summary>
		Delete,
		/// <summary>The record has been modified.</summary>
		Change

	};

}
