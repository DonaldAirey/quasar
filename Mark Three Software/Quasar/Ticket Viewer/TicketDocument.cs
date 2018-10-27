/*************************************************************************************************************************
*
*	File:			TicketDocument.cs
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
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Shadows.Quasar.Viewers.Ticket
{

	/// <summary>
	/// Creates a Document Object Model representation of a group of executions against a block order.
	/// </summary>
	class TicketDocument : XmlDocument
	{

		/// <summary>
		/// Constructs and empty, but well formed, execution document.
		/// </summary>
		public TicketDocument()
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Tickets"));

		}

		/// <summary>
		/// Constructs a well formed Document Object Model for a group of executions on a block order.
		/// </summary>
		/// <param name="blockOrderId">The block order for which we want a execution document.</param>
		public TicketDocument(int blotterId)
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Tickets");
			this.AppendChild(rootElement);

			// Start with the block order.  All the executions flow from this record.
			ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
			if (blotterRow != null)
			{

				// The block order id is occationally useful info at the root element.
				XmlAttribute blotterIdAttribute = this.CreateAttribute("BlotterId");
				rootElement.Attributes.Append(blotterIdAttribute);
				blotterIdAttribute.Value = blotterRow.BlotterId.ToString();

				this.WriteBlotter(blotterRow);

			}

		}

		private void WriteBlotter(ClientMarketData.BlotterRow blotterRow)
		{

			foreach (ClientMarketData.BlockOrderRow blockOrderRow in blotterRow.GetBlockOrderRows())
			{

				// Inhibit displaying blocks with no executions.
				if (blockOrderRow.GetExecutionRows().Length > 0)
				{

					XmlElement blockOrdersElement = this.CreateBlockOrderElement(blockOrderRow);
					this.DocumentElement.AppendChild(blockOrdersElement);

					// These structures are used to group executions by broker.
					ArrayList brokerList = new ArrayList();
					Hashtable brokerTable = new Hashtable();

					// We are going to run through all the executions posted against the block order and construct a 
					// hierarchical structure of brokers.  Since they aren't arranged that way in the data model, we need to
					// scan the whole table looking for distinct brokers.
					foreach (ClientMarketData.ExecutionRow executionRow in blockOrderRow.GetExecutionRows())
					{

						// See if we've run across this broker yet.  If we haven't, we'll create a list that can hold all the 
						// executions associated with a distinct broker.
						ArrayList executionsList = (ArrayList)brokerTable[executionRow.BrokerId];
						if (executionsList == null)
						{
							brokerList.Add(executionRow.BrokerRow);
							executionsList = new ArrayList();
							brokerTable[executionRow.BrokerId] = executionsList;
						}

						// Add this execution to the ones made to a particular broker, no matter what order it was entered
						// into the system.  This will allow us to display the placments grouped by broker if we decide.
						int index;
						for (index = 0; index < executionsList.Count; index++)
							if (((ClientMarketData.ExecutionRow)executionsList[index]).CreatedTime > executionRow.CreatedTime)
							{
								executionsList.Insert(index, executionRow);
								break;
							}

						// If the row wasn't sorted into the array above, it's added to the end of the list here.
						if (index == executionsList.Count)
							executionsList.Add(executionRow);

					}

					// At this point, we have a hierarchy of distinct brokers and the executions made to those brokers.  Go through the
					// structure and create the DOM.
					foreach (ClientMarketData.BrokerRow brokerRow in brokerList)
					{

						// Create a record for the broker.
						XmlElement brokersElement = this.CreateBrokersElement(brokerRow);
						blockOrdersElement.AppendChild(brokersElement);

						// Add all of the executions as children of the broker.
						foreach (ClientMarketData.ExecutionRow executionRow in (ArrayList)brokerTable[brokerRow.BrokerId])
							brokersElement.AppendChild(this.CreateExecutionElement(executionRow));

					}

				}

			}

			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in blotterRow.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (ClientMarketData.BlotterRow childBlotterRow in objectTreeRow.ObjectRowByFKObjectObjectTreeChildId.GetBlotterRows())
					this.WriteBlotter(childBlotterRow);

		}
		
		/// <summary>
		/// Creates a blockOrder element for the TicketDocument
		/// </summary>
		/// <param name="blockOrderRow">A BlockOrder record</param>
		/// <returns>An element that represents a blockOrder.</returns>
		public BlockOrderElement CreateBlockOrderElement(ClientMarketData.BlockOrderRow blockOrderRow)
		{
			return new BlockOrderElement(this, blockOrderRow);
		}

		/// <summary>
		/// Creates a broker element for the TicketDocument
		/// </summary>
		/// <param name="brokerRow">A Broker record</param>
		/// <returns>An element that represents a broker.</returns>
		public BrokersElement CreateBrokersElement(ClientMarketData.BrokerRow brokerRow)
		{
			return new BrokersElement(this, brokerRow);
		}

		/// <summary>
		/// Creates a execution element for the TicketDocument
		/// </summary>
		/// <param name="executionRow">A Ticket record</param>
		/// <returns>An element that represents a execution.</returns>
		public ExecutionElement CreateExecutionElement(ClientMarketData.ExecutionRow executionRow)
		{
			return new ExecutionElement(this, executionRow);
		}

	}

	/// <summary>
	/// A XML Element representing a block order in the document.
	/// </summary>
	class BlockOrderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a blockOrder in a execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="blockOrderRow">A blockOrder record.</param>
		public BlockOrderElement(XmlDocument xmlDocument, ClientMarketData.BlockOrderRow blockOrderRow) : base("BlockOrder", xmlDocument)
		{

			// Add the attributes of a blockOrder to this record.
			AddAttribute("BlockOrderId", blockOrderRow.BlockOrderId.ToString());

			// If the security exists, add the basic data to the block information.
			ClientMarketData.SecurityRow securityRow = blockOrderRow.SecurityRowByFKSecurityBlockOrderSecurityId;
			AddAttribute("SecurityId", securityRow.SecurityId.ToString());
			AddAttribute("SecuritySymbol", securityRow.Symbol);
			AddAttribute("SecurityName", securityRow.ObjectRow.Name);
			
			// It's useful to have the transaction type so we know whether to add fees and commissions or subtract 
			// them from the net calculations.
			AddAttribute("TransactionTypeCode", blockOrderRow.TransactionTypeCode.ToString());
			AddAttribute("TransactionTypeDescription", blockOrderRow.TransactionTypeRow.Description);

			decimal quantityOrdered = 0.0M;
			foreach (ClientMarketData.OrderRow orderRow in blockOrderRow.GetOrderRows())
				quantityOrdered += orderRow.Quantity;

			AddAttribute("QuantityOrdered", quantityOrdered.ToString());

		}

	}
	
	/// <summary>
	/// A XML Element representing a broker in the document.
	/// </summary>
	class BrokersElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a broker in a execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="brokerRow">A broker record.</param>
		public BrokersElement(XmlDocument xmlDocument, ClientMarketData.BrokerRow brokerRow) : base("Broker", xmlDocument)
		{

			// Add the attributes of a broker to this record.
			AddAttribute("BrokerId", brokerRow.BrokerId.ToString());
			AddAttribute("BrokerName", brokerRow.ObjectRow.Name);
			AddAttribute("BrokerSymbol", brokerRow.Symbol);
			if (!brokerRow.IsPhoneNull())
				AddAttribute("BrokerPhone", brokerRow.Phone);

		}

	}
	
	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class ExecutionElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="executionRow">A execution record.</param>
		public ExecutionElement(XmlDocument xmlDocument, ClientMarketData.ExecutionRow executionRow) :
			base("Execution", xmlDocument)
		{

			// Add the attributes of a execution to this record.
			AddAttribute("ExecutionId", executionRow.ExecutionId.ToString());
			AddAttribute("Quantity", executionRow.Quantity.ToString());
			AddAttribute("Price", executionRow.Price.ToString());
			AddAttribute("Commission", executionRow.Commission.ToString());
			AddAttribute("AccruedInterest", executionRow.AccruedInterest.ToString());
			AddAttribute("UserFee0", executionRow.UserFee0.ToString());
			AddAttribute("UserFee1", executionRow.UserFee1.ToString());
			AddAttribute("UserFee2", executionRow.UserFee2.ToString());
			AddAttribute("UserFee3", executionRow.UserFee3.ToString());
			AddAttribute("TradeDate", executionRow.TradeDate.ToString("s"));
			AddAttribute("SettlementDate", executionRow.SettlementDate.ToString("s"));
			AddAttribute("CreatedTime", executionRow.CreatedTime.ToString("s"));
			AddAttribute("CreatedLoginId", executionRow.CreatedLoginId.ToString());
			AddAttribute("CreatedLoginName", executionRow.LoginRowByFKLoginExecutionCreatedLoginId.ObjectRow.Name);
			AddAttribute("ModifiedTime", executionRow.ModifiedTime.ToString("s"));
			AddAttribute("ModifiedLoginId", executionRow.ModifiedLoginId.ToString());
			AddAttribute("ModifiedLoginName", executionRow.LoginRowByFKLoginExecutionModifiedLoginId.ObjectRow.Name);

		}

	}

}
