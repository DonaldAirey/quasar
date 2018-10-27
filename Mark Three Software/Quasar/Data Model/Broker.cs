/*************************************************************************************************************************
*
*	File:			Broker.cs
*	Description:	Classes and methods to handle a broker.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using MarkThree.Quasar;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Summary description for Broker.
	/// </summary>
	public class Broker
	{

		private int brokerId;
		private string typeCode;
		private string name;
		private string symbol;
		private string phone;

		/// <summary>
		/// Creates a broker record.
		/// </summary>
		/// <param name="brokerId"></param>
		/// <param name="objectType"></param>
		/// <param name="name"></param>
		/// <param name="symbol"></param>
		/// <param name="phone"></param>
		public Broker(int brokerId, string typeCode, string name, string symbol, string phone)
		{

			// Initialize the members
			this.brokerId = brokerId;
			this.typeCode = typeCode;
			this.name = name;
			this.symbol = symbol;
			this.phone = phone;

		}

		// Public access to members
		public int BrokerId {get {return this.brokerId;}}
		public string TypeCode {get {return this.typeCode;}}
		public string Name {get {return this.name;}}
		public string Symbol {get {return this.symbol;}}
		public string Phone {get {return this.phone;}}
		
		public static Broker FindBySymbol(string symbol)
		{

			Broker broker = null;

			Transaction transaction = new Transaction();
			transaction.AdoTransaction.LockRequests.Add(new TableReaderRequest(DataModel.Broker));
			transaction.AdoTransaction.LockRequests.Add(new TableReaderRequest(DataModel.Object));

			try
			{

				transaction.Begin();

				foreach (DataModel.BrokerRow brokerRow in DataModel.Broker)
					if (brokerRow.Symbol == symbol)
						broker = new Broker(brokerRow.BrokerId, brokerRow.ObjectRow.TypeCode,
							brokerRow.ObjectRow.Name, brokerRow.Symbol, brokerRow.IsPhoneNull() ? String.Empty : brokerRow.Phone);

				transaction.Commit();

			}
			catch
			{
				transaction.Rollback();
			}
			finally
			{
				transaction.EndTransaction();
			}

			return broker;

		}
	
	}

}
