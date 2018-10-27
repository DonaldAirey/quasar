namespace MarkThree.Guardian
{

	using System;

	/// <summary>
	/// A collection of client orders that can be allocated as a single unit.
	/// </summary>
	public class WorkingOrder
	{

		public readonly int WorkingOrderId;
		public readonly int BlotterId;

		public WorkingOrder(int workingOrderId, int blotterId)
		{

			// Initialize the object.
			this.WorkingOrderId = workingOrderId;
			this.BlotterId = blotterId;

		}

	}

}
