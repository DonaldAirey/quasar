/*************************************************************************************************************************
*
*	File:			Broker.cs
*	Description:	This module contains a class use to represent a broker read from a formatted file.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Loader
{

	/// <summary>
	/// An Execution Broker is someone who can complete a transaction.
	/// </summary>
	public class Broker
	{

		private bool connected;
		private string name;
		private string symbol;

		/// <summary>
		/// True if the broker is electronically connected.
		/// </summary>
		public bool Connected {get {return this.connected;}}
		
		/// <summary>
		/// Name of the broker
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Symbolic name of the broker
		/// </summary>
		public string Symbol {get {return this.symbol;}}

		/// <summary>
		/// Creates a broker object.
		/// </summary>
		/// <param name="symbol">Symbol used to identify broker.</param>
		/// <param name="name">Name of the broker.</param>
		/// <param name="location">Location of the broker.</param>
		/// <param name="phone">Phone number of the broker.</param>
		public Broker(bool connected, string symbol, string name)
		{

			// Initialize Class Members
			this.connected = connected;
			this.name = name;
			this.symbol = symbol;

		}

	}

}
