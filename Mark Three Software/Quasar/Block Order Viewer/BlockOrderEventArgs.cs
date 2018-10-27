/*************************************************************************************************************************
*
*	File:			OpenBlockOrderEventArgs.cs
*	Description:	Used to communicate that a new block order has been selected.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.BlockOrder
{

	/// <summary>
	/// This event is called when the user has accepted input from the spreadsheet edit control.
	/// </summary>
	public delegate void BlockOrderEventHandler(object sender, BlockOrderEventArgs e);

	/// <summary>
	/// Used to pass the identifier of a block order when the currently viewed order has changed.
	/// </summary>
	public class BlockOrderEventArgs : EventArgs
	{

		private int blockOrderId;

		/// <summary>
		/// Public access for the block order id.
		/// </summary>
		public int BlockOrderId {get {return this.blockOrderId;}}

		/// <summary>
		/// Creates event argument broadcasting a change in the currently viewed block order.
		/// </summary>
		/// <param name="f"></param>
		public BlockOrderEventArgs(int blockOrderId) {this.blockOrderId = blockOrderId;}

	}

}
