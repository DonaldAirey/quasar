using System;

namespace MarkThree.Guardian
{

	/// <summary>
	/// This event is called when the user has accepted input from the spreadsheet edit control.
	/// </summary>
	public delegate void WorkingOrderEventHandler(object sender, WorkingOrderEventArgs e);

	/// <summary>
	/// Used to pass the identifier of a working order when the currently viewed order has changed.
	/// </summary>
	public class WorkingOrderEventArgs : EventArgs
	{

		private WorkingOrder[] workingOrders;

		/// <summary>
		/// Public access for the working order id.
		/// </summary>
		public WorkingOrder[] WorkingOrders {get {return this.workingOrders;}}

		/// <summary>
		/// Creates event argument broadcasting a change in the currently viewed working order.
		/// </summary>
		/// <param name="f"></param>
		public WorkingOrderEventArgs(WorkingOrder[] workingOrders) {this.workingOrders = workingOrders;}

	}

}
