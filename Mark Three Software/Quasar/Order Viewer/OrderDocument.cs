/*************************************************************************************************************************
*
*	File:			OrderDocument.cs
*	Description:	The classes to control the DOM for the blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Client;
using Shadows.Quasar.Common;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Shadows.Quasar.Viewers.Order
{

	/// <summary>
	/// Creates a Document Object Model representation of a group of orders against a block order.
	/// </summary>
	class OrderDocument : XmlDocument
	{

		/// <summary>
		/// Constructs and empty, but well formed, order document.
		/// </summary>
		public OrderDocument()
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Order");
			this.AppendChild(rootElement);

			// The placeholder is a queue for the document manager to create a new order.
			rootElement.AppendChild(this.CreatePlaceholderElement());

		}

		/// <summary>
		/// Constructs a well formed Document Object Model for a group of orders on a block order.
		/// </summary>
		/// <param name="blockOrderId">The block order for which we want a order document.</param>
		public OrderDocument(int blockOrderId, LocalOrderSet localOrderSet)
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Order");
			this.AppendChild(rootElement);

			// Add each of the orders to the document.
			foreach (LocalOrderSet.OrderRow orderRow in localOrderSet.Order)
				rootElement.AppendChild(CreateLocalOrderElement(orderRow));

			// The placeholder is where new orders will be added.
			rootElement.AppendChild(CreatePlaceholderElement());

		}

		/// <summary>
		/// Creates a order element for the OrderDocument
		/// </summary>
		/// <param name="orderRow">A Order record</param>
		/// <returns>An element that represents a order.</returns>
		public LocalOrderElement CreateLocalOrderElement(LocalOrderSet.OrderRow orderRow)
		{
			return new LocalOrderElement(this, orderRow);
		}

		/// <summary>
		/// Creates a placeholder element for new Orders.
		/// </summary>
		/// <returns>An element that can be used as a prompt for new orders.</returns>
		public PlaceholderElement CreatePlaceholderElement()
		{
			return new PlaceholderElement(this);
		}

	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class LocalOrderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the order document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="orderRow">A order record.</param>
		public LocalOrderElement(XmlDocument xmlDocument, LocalOrderSet.OrderRow orderRow) :
			base("LocalOrder", xmlDocument)
		{

			// Add the attributes of a order to this record.
			AddAttribute("OrderId", orderRow.OrderId.ToString());

			// Account field
			if (!orderRow.IsAccountIdNull())
			{

				AddAttribute("AccountId", orderRow.AccountId.ToString());

				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(orderRow.AccountId);
				if (accountRow != null)
				{
					AddAttribute("AccountId", accountRow.AccountId.ToString());
					AddAttribute("AccountName", accountRow.ObjectRow.Name);
					AddAttribute("AccountMnemonic", accountRow.Mnemonic);
				}

			}

			// Security field
			if (!orderRow.IsSecurityIdNull())
			{

				AddAttribute("SecurityId", orderRow.SecurityId.ToString());

				ClientMarketData.SecurityRow securityRow = ClientMarketData.Security.FindBySecurityId(orderRow.SecurityId);
				if (securityRow != null)
				{
					AddAttribute("SecurityId", securityRow.SecurityId.ToString());
					AddAttribute("SecurityName", securityRow.ObjectRow.Name);
					AddAttribute("SecuritySymbol", securityRow.Symbol);
				}

			}

			// Broker field
			if (!orderRow.IsBrokerIdNull())
			{

				AddAttribute("BrokerId", orderRow.BrokerId.ToString());

				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(orderRow.BrokerId);
				if (brokerRow != null)
				{
					AddAttribute("BrokerId", brokerRow.BrokerId.ToString());
					AddAttribute("BrokerName", brokerRow.ObjectRow.Name);
					AddAttribute("BrokerSymbol", brokerRow.Symbol);
					if (!brokerRow.IsPhoneNull()) AddAttribute("BrokerPhone", brokerRow.Phone);
				}

			}

			// TransactionType field
			if (!orderRow.IsTransactionTypeCodeNull())
			{

				AddAttribute("TransactionTypeCode", orderRow.TransactionTypeCode.ToString());

				ClientMarketData.TransactionTypeRow transactionTypeRow = ClientMarketData.TransactionType.FindByTransactionTypeCode(orderRow.TransactionTypeCode);
				if (transactionTypeRow != null)
					AddAttribute("TransactionTypeMnemonic", transactionTypeRow.Mnemonic);

			}

			// TimeInForce field
			if (!orderRow.IsTimeInForceCodeNull())
			{

				AddAttribute("TimeInForceCode", orderRow.TimeInForceCode.ToString());

				ClientMarketData.TimeInForceRow timeInForceRow = ClientMarketData.TimeInForce.FindByTimeInForceCode(orderRow.TimeInForceCode);
				if (timeInForceRow != null)
					AddAttribute("TimeInForceMnemonic", timeInForceRow.Mnemonic);

			}

			// OrderType field
			if (!orderRow.IsOrderTypeCodeNull())
			{

				AddAttribute("OrderTypeCode", orderRow.OrderTypeCode.ToString());

				ClientMarketData.OrderTypeRow orderTypeRow = ClientMarketData.OrderType.FindByOrderTypeCode(orderRow.OrderTypeCode);
				if (orderTypeRow != null)
					AddAttribute("OrderTypeMnemonic", orderTypeRow.Mnemonic);

			}

			if (!orderRow.IsQuantityNull())
				AddAttribute("Quantity", orderRow.Quantity.ToString());
			if (!orderRow.IsPrice1Null())
				AddAttribute("Price1", orderRow.Price1.ToString());
			if (!orderRow.IsPrice2Null())
				AddAttribute("Price2", orderRow.Price2.ToString());

		}
	
	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class PlaceholderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the order document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="orderRow">A order record.</param>
		public PlaceholderElement(XmlDocument xmlDocument) : base("Placeholder", xmlDocument)
		{

			// This provided an empty placeholder for the stylesheet to display the input prompt.

		}

	}

}
