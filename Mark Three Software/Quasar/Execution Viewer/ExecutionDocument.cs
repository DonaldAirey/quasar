/*************************************************************************************************************************
*
*	File:			ExecutionDocument.cs
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

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// Creates a Document Object Model representation of a group of executions against a block order.
	/// </summary>
	class ExecutionDocument : XmlDocument
	{

		/// <summary>
		/// Constructs and empty, but well formed, execution document.
		/// </summary>
		public ExecutionDocument()
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Execution"));

		}

		/// <summary>
		/// Constructs a well formed Document Object Model for a group of executions on a block order.
		/// </summary>
		/// <param name="blockOrderId">The block order for which we want a execution document.</param>
		public ExecutionDocument(int blockOrderId, ExecutionSet executionSet)
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Execution");
			this.AppendChild(rootElement);

			// This list is used to sort the executions.
			ArrayList executionsList = new ArrayList();

			// Start with the block order.  All the executions flow from this record.
			ClientMarketData.BlockOrderRow blockOrderRow = ClientMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
			if (blockOrderRow != null)
			{

				// This element holds all the information related to the block order.
				XmlElement blockOrderElement = this.CreateBlockOrderElement(blockOrderRow);
				rootElement.AppendChild(blockOrderElement);

				// We are going to run through all the executions posted against the block order and construct a 
				// hierarchical structure of brokers.  Since they aren't arranged that way in the data model, we need to
				// scan the whole table looking for distinct brokers.
				foreach (ClientMarketData.ExecutionRow executionRow in blockOrderRow.GetExecutionRows())
				{

					// Sort the list of executions by the created time.
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

				// Add all of the executions as children of the broker.
				foreach (ClientMarketData.ExecutionRow executionRow in executionsList)
					blockOrderElement.AppendChild(this.CreateGlobalExecutionElement(executionRow));

				// Add the local executions.
				ExecutionSet.BlockOrderRow localBlockOrder = executionSet.BlockOrder.FindByBlockOrderId(blockOrderId);
				if (localBlockOrder != null)
					foreach (ExecutionSet.ExecutionRow executionRow in localBlockOrder.GetExecutionRows())
					{
						if (executionRow.RowState == DataRowState.Detached)
							Console.WriteLine("{0} is Detached", executionRow["ExecutionId", DataRowVersion.Original]);
						blockOrderElement.AppendChild(this.CreateLocalExecutionElement(executionRow));
					}

			}

			// The placeholder is a queue for the document manager to create a new execution.
			this.DocumentElement.AppendChild(this.CreatePlaceholderElement());

		}

		/// <summary>
		/// Creates a broker element for the ExecutionDocument
		/// </summary>
		/// <param name="blockOrderRow">A Broker record</param>
		/// <returns>An element that represents a broker.</returns>
		public BlockOrderElement CreateBlockOrderElement(ClientMarketData.BlockOrderRow blockOrderRow)
		{
			return new BlockOrderElement(this, blockOrderRow);
		}

		/// <summary>
		/// Creates a execution element for the ExecutionDocument
		/// </summary>
		/// <param name="executionRow">A Execution record</param>
		/// <returns>An element that represents a execution.</returns>
		public GlobalExecutionElement CreateGlobalExecutionElement(ClientMarketData.ExecutionRow executionRow)
		{
			return new GlobalExecutionElement(this, executionRow);
		}

		/// <summary>
		/// Creates a execution element for the ExecutionDocument
		/// </summary>
		/// <param name="executionRow">A Execution record</param>
		/// <returns>An element that represents a execution.</returns>
		public LocalExecutionElement CreateLocalExecutionElement(ExecutionSet.ExecutionRow executionRow)
		{
			return new LocalExecutionElement(this, executionRow);
		}

		/// <summary>
		/// Creates a placeholder element for new Execution.
		/// </summary>
		/// <returns>An element that can be used as a prompt for new executions.</returns>
		public PlaceholderElement CreatePlaceholderElement()
		{
			return new PlaceholderElement(this);
		}

	}

	/// <summary>
	/// A XML Element representing a broker in the document.
	/// </summary>
	class BlockOrderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a broker in a execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="blockOrderRow">A broker record.</param>
		public BlockOrderElement(XmlDocument xmlDocument, ClientMarketData.BlockOrderRow blockOrderRow) : base("BlockOrder", xmlDocument)
		{

			// The block order id is occationally useful info at the root element.
			AddAttribute("BlockOrderId", blockOrderRow.BlockOrderId);

			// It's useful to have the transaction type so we know whether to add fees and commissions or subtract 
			// them from the net calculations.
			AddAttribute("TransactionTypeCode", blockOrderRow.TransactionTypeCode);

			// Get a shortcut to the security.
			ClientMarketData.SecurityRow securityRow = blockOrderRow.SecurityRowByFKSecurityBlockOrderSecurityId;

			// Add Fixed Income Fundamentals where they exist.
			foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
			{

				// Add the fundamental data for the debt.
				if (!debtRow.IsIssuerIdNull())
					AddAttribute("IssuerId", debtRow.IssuerId.ToString());
				AddAttribute("DebtTypeCode", debtRow.DebtTypeCode.ToString());
				AddAttribute("Coupon", debtRow.Coupon.ToString());
				AddAttribute("MaturityDate", debtRow.MaturityDate.ToShortDateString());

			}

		}

	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class LocalExecutionElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="execution">A execution record.</param>
		public LocalExecutionElement(XmlDocument xmlDocument, ExecutionSet.ExecutionRow executionRow) :
			base("LocalExecution", xmlDocument)
		{

			// Broker field
			if (!executionRow.IsBrokerIdNull())
			{

				AddAttribute("BrokerId", executionRow.BrokerId.ToString());

				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(executionRow.BrokerId);
				if (brokerRow != null)
				{
					AddAttribute("BrokerId", brokerRow.BrokerId.ToString());
					AddAttribute("BrokerName", brokerRow.ObjectRow.Name);
					AddAttribute("BrokerSymbol", brokerRow.Symbol);
					if (!brokerRow.IsPhoneNull())
						AddAttribute("BrokerPhone", brokerRow.Phone);
				}

			}

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

		}

	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class GlobalExecutionElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="executionRow">A execution record.</param>
		public GlobalExecutionElement(XmlDocument xmlDocument, ClientMarketData.ExecutionRow executionRow) :
			base("GlobalExecution", xmlDocument)
		{

			// Add the attributes of a broker to this record.
			ClientMarketData.BrokerRow brokerRow = executionRow.BrokerRow;
			AddAttribute("BrokerId", brokerRow.BrokerId.ToString());
			AddAttribute("BrokerName", brokerRow.ObjectRow.Name);
			AddAttribute("BrokerSymbol", brokerRow.Symbol);
			
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
			if (!executionRow.IsCreatedTimeNull())
				AddAttribute("CreatedTime", executionRow.CreatedTime.ToString("s"));
			if (!executionRow.IsCreatedLoginIdNull())
			{
				AddAttribute("CreatedLoginId", executionRow.CreatedLoginId.ToString());
				AddAttribute("CreatedLoginName", executionRow.LoginRowByFKLoginExecutionCreatedLoginId.ObjectRow.Name);
			}
			if (!executionRow.IsModifiedTimeNull())
				AddAttribute("ModifiedTime", executionRow.ModifiedTime.ToString("s"));
			if (!executionRow.IsModifiedLoginIdNull())
			{
				AddAttribute("ModifiedLoginId", executionRow.ModifiedLoginId.ToString());
				AddAttribute("ModifiedLoginName", executionRow.LoginRowByFKLoginExecutionModifiedLoginId.ObjectRow.Name);
			}

		}

	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class PlaceholderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="executionRow">A execution record.</param>
		public PlaceholderElement(XmlDocument xmlDocument) : base("Placeholder", xmlDocument)
		{

			// This provided an empty placeholder for the stylesheet to display the input prompt.

		}

	}

}
