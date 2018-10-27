namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Delegate for events that include a constraint.
	/// </summary>
	/// <param name="sender">The object that originated the event.</param>
	/// <param name="constraintEventArgs">The event arguments.</param>
	public delegate void ConstraintEvent(object sender, ConstraintEventArgs constraintEventArgs);

	/// <summary>
	/// Event arguments that include a constraint.
	/// </summary>
	public class ConstraintEventArgs : EventArgs
	{

		// Public Properties
		public ConstraintSchema ConstraintSchema;

		/// <summary>
		/// Create event arguments using a constraint.
		/// </summary>
		/// <param name="constraintSchema">The constraint involved in the event.</param>
		public ConstraintEventArgs(ConstraintSchema constraintSchema)
		{

			// Initialize the object
			this.ConstraintSchema = constraintSchema;

		}
	}

}
