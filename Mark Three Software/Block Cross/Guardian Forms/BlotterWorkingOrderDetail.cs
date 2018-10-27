namespace MarkThree.Guardian.Forms
{

	using System;
	using MarkThree.Guardian;

	/// <summary>
	/// Summary description for BlotterDetail.
	/// </summary>
	public struct BlotterWorkingOrderDetail
	{

		public Blotter Blotter;
		public WorkingOrder[] WorkingOrders;

		public BlotterWorkingOrderDetail(Blotter blotter, WorkingOrder[] workingOrders)
		{

			// Initialize the object
			this.Blotter = blotter;
			this.WorkingOrders = workingOrders;

		}

	}

}
