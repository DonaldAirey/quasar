/*************************************************************************************************************************
*
*	File:			Order.cs
*	Description:	The Order Set and records are used as a factory to create order records that are independant
*					of the data model and don't have to be locked to be used.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.Client;
using System;
using System.Data;

namespace Shadows.Quasar.Viewers.Order
{

	/// <summary>
	/// A factory for denormalized order records.
	/// </summary>
	internal class Order
	{

		private static OrderSet orderSet;

		/// <summary>
		/// Initializes the static portion of the Order.
		/// </summary>
		static Order()
		{

			// This is a static data set that is used as a factory for creating order records.
			orderSet = new OrderSet();

		}
		
		/// <summary>
		/// Creates a denormalized order record from a local record.
		/// </summary>
		/// <param name="localOrder">A local order record.</param>
		/// <returns>A order record that is independant of the global data set for all the anscillary data.</returns>
		public static OrderSet.OrderRow Create(LocalOrderSet.OrderRow localOrder)
		{

			// Create a new, empty order record.
			OrderSet.OrderRow orderRow = orderSet.Order.NewOrderRow();

			// This new record is a copy of a local record and uses the local system of identifiers.
			orderRow.IsLocal = true;
			
			// Copy each field that has an analog in the local record set into the new record.
			foreach (DataColumn dataColumn in localOrder.Table.Columns)
				orderRow[dataColumn.ColumnName] = localOrder[dataColumn];

			// AccountId cross-referenced data is filled in here.
			if (!localOrder.IsAccountIdNull())
			{
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(orderRow.AccountId);
				if (accountRow != null)
				{
					orderRow.AccountMnemonic = accountRow.Mnemonic;
					orderRow.AccountName = accountRow.ObjectRow.Name;
				}
			}

			// SecurityId cross-referenced data is filled in here.
			if (!localOrder.IsSecurityIdNull())
			{
				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(orderRow.SecurityId);
				if (securityRow != null)
				{
					orderRow.SecuritySymbol = securityRow.Symbol;
					orderRow.SecurityName = securityRow.ObjectRow.Name;
				}
			}

			// CurrencyId cross-referenced data is filled in here.
			if (!localOrder.IsSettlementIdNull())
			{
				ClientMarketData.CurrencyRow currencyRow = ClientMarketData.Currency.FindByCurrencyId(orderRow.SettlementId);
				if (currencyRow != null)
				{
					orderRow.SettlementSymbol = currencyRow.SecurityRow.Symbol;
					orderRow.SettlementName = currencyRow.SecurityRow.ObjectRow.Name;
				}
			}

			// BrokerId cross-referenced data is filled in here.
			if (!localOrder.IsBrokerIdNull())
			{
				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(orderRow.BrokerId);
				if (brokerRow != null)
				{
					orderRow.BrokerSymbol = brokerRow.Symbol;
					orderRow.BrokerName = brokerRow.ObjectRow.Name;
				}
			}

			// TransactionType cross-referenced data is filled in here.
			if (!localOrder.IsTransactionTypeCodeNull())
			{
				ClientMarketData.TransactionTypeRow transactionTypeRow = ClientMarketData.TransactionType.FindByTransactionTypeCode(orderRow.TransactionTypeCode);
				if (transactionTypeRow != null)
					orderRow.TransactionTypeMnemonic = transactionTypeRow.Mnemonic;
			}

			// TimeInForce cross-referenced data is filled in here.
			if (!localOrder.IsTimeInForceCodeNull())
			{
				ClientMarketData.TimeInForceRow timeInForceRow = ClientMarketData.TimeInForce.FindByTimeInForceCode(orderRow.TimeInForceCode);
				if (timeInForceRow != null)
					orderRow.TimeInForceMnemonic = timeInForceRow.Mnemonic;
			}

			// TimeInForce cross-referenced data is filled in here.
			if (!localOrder.IsOrderTypeCodeNull())
			{
				ClientMarketData.OrderTypeRow orderTypeRow = ClientMarketData.OrderType.FindByOrderTypeCode(orderRow.OrderTypeCode);
				if (orderTypeRow != null)
					orderRow.OrderTypeMnemonic = orderTypeRow.Mnemonic;
			}

			// This is a complete record of the order, including the referenced data.
			return orderRow;
		
		}

		/// <summary>
		/// Creates a denormalized order record from a global record.
		/// </summary>
		/// <param name="globalOrder"></param>
		/// <returns></returns>
		public static OrderSet.OrderRow Create(ClientMarketData.OrderRow globalOrder)
		{

			// Create a new order record from the record factory.
			OrderSet.OrderRow orderRow = orderSet.Order.NewOrderRow();

			// These records are global and use the global system of identifiers.
			orderRow.IsLocal = false;
			
			// Copy each field that has an analog in the local record set into the new record.
			foreach (DataColumn dataColumn in globalOrder.Table.Columns)
				orderRow[dataColumn.ColumnName] = globalOrder[dataColumn];

			// AccountId cross-referenced data is filled in here.
			orderRow.AccountMnemonic = globalOrder.AccountRow.Mnemonic;
			orderRow.AccountName = globalOrder.AccountRow.ObjectRow.Name;
			
			// SecurityId cross-referenced data is filled in here.
			orderRow.SecuritySymbol = globalOrder.SecurityRowByFKSecurityOrderSecurityId.Symbol;
			orderRow.SecurityName = globalOrder.SecurityRowByFKSecurityOrderSecurityId.ObjectRow.Name;

			// CurrencyId cross-referenced data is filled in here.
			orderRow.SettlementSymbol = globalOrder.SecurityRowByFKSecurityOrderSettlementId.Symbol;
			orderRow.SettlementName = globalOrder.SecurityRowByFKSecurityOrderSettlementId.ObjectRow.Name;

			// BrokerId cross-referenced data is filled in here.
			if (!globalOrder.IsBrokerIdNull())
			{
				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(globalOrder.BrokerId);
				if (brokerRow != null)
				{
					orderRow.BrokerSymbol = brokerRow.Symbol;
					orderRow.BrokerName = brokerRow.ObjectRow.Name;
				}
			}

			// TimeInForce cross-referenced data is filled in here.
			orderRow.TimeInForceMnemonic = globalOrder.TimeInForceRow.Mnemonic;

			// TimeInForce cross-referenced data is filled in here.
			orderRow.OrderTypeMnemonic = globalOrder.OrderTypeRow.Mnemonic;

			// This is a complete record of the order, including the referenced data.
			return orderRow;
		
		}

	}

}