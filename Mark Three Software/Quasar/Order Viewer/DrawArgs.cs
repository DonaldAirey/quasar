/*************************************************************************************************************************
*
*	File:			DrawArgs.cs
*	Description:	Arguments used to draw the document.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using System;

namespace Shadows.Quasar.Viewers.Order
{

	/// <summary>
	/// Passes parameters to the OrderRefresh Thread.
	/// </summary>
	internal class DrawArgs
	{

		private LocalOrderSet localOrderSet;

		/// <summary>
		/// The local orders
		/// </summary>
		public LocalOrderSet LocalOrderSet {get {return this.localOrderSet;}}

		/// <summary>
		/// Constructor used to pass argument to the 'OrderRefresh' thread.
		/// </summary>
		public DrawArgs(LocalOrderSet localOrderSet)
		{

			// Initialize the class members.
			this.localOrderSet = localOrderSet;

		}

	}

}
