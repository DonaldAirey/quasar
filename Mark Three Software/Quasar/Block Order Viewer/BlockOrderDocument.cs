/*************************************************************************************************************************
*
*	File:			BlockOrderDocument.cs
*	Description:	The classes to control the DOM for the blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.BlockOrder
{

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

	class BlockOrderDocument : XmlDocument
	{


		/// <summary>
		/// Creates an empty, but well formed, block order document object model.
		/// </summary>
		public BlockOrderDocument()
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Blotter"));

		}
		
		/// <summary>
		/// Creates a well formed block order document object model.
		/// </summary>
		/// <param name="blotterId">The blotter identifies what blocks are to be included in this document.</param>
		public BlockOrderDocument(int blotterId)
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Blotter"));

			// Find the top level blotter record and recursively construct the report by merging all the children.
			ClientMarketData.BlotterRow mainBlotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
			if (mainBlotterRow != null)
				RecurseBlotter(mainBlotterRow.ObjectRow);

			// Calculate the derrived fields: aggregates like total quantity ordered, and distinct fields like
			// Time-in-Force and limit prices.
			this.CalculateFields();

		}

		private void RecurseBlotter(ClientMarketData.ObjectRow objectRow)
		{

			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in objectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				RecurseBlotter(objectTreeRow.ObjectRowByFKObjectObjectTreeChildId);

			foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
				foreach (ClientMarketData.BlockOrderRow blockOrderRow in blotterRow.GetBlockOrderRows())
					if (blockOrderRow.GetBlockOrderTreeRowsByFKBlockOrderBlockOrderTreeChildId().Length == 0)
					{
						// At this point, we've found an element that has no parents.  Add it to the tree and recurse down in
						// to the structure to find all the children.
						BlockOrderElement blockOrdersElement = this.CreateBlockOrderElement(blockOrderRow);
						this.DocumentElement.AppendChild(blockOrdersElement);
						RecurseTree(blockOrdersElement);

						// This is a CPU intensive task, so sleep between each major branch that's uncovered.
						Thread.Sleep(0);

					}

		}

		public BlockOrderElement CreateBlockOrderElement(ClientMarketData.BlockOrderRow blockOrderRow)
		{
			return new BlockOrderElement(this, blockOrderRow);
		}

		public OrdersElement CreateOrdersElement(ClientMarketData.OrderRow orderRow)
		{
			return new OrdersElement(this, orderRow);
		}

		public PlacementsElement CreatePlacementsElement(ClientMarketData.PlacementRow orderRow)
		{
			return new PlacementsElement(this, orderRow);
		}

		public ExecutionElement CreateExecutionElement(ClientMarketData.ExecutionRow orderRow)
		{
			return new ExecutionElement(this, orderRow);
		}

		/// <summary>
		/// Construct a hierarchical tree structure by recursively scanning the parent-child relations.
		/// </summary>
		/// <param name="objectNode">The current node in the tree structure.</param>
		private void RecurseTree(BlockOrderElement parentElement)
		{

			// Recursion is sometimes difficult to follow.  At this point, we have a parent block order.  The 'BlockOrderTree' table can be used effectively for this.  Simply trace the parent
			// relation back to the 'BlockOrderTree' and you have a list of children block orders.  For every child we 
			// find, we're going to recurse down until there are no more descendants.
			foreach (ClientMarketData.BlockOrderTreeRow blockOrdersTreeRow in
				parentElement.BlockOrderRow.GetBlockOrderTreeRowsByFKBlockOrderBlockOrderTreeParentId())
			{

				// Trace the 'child id' column back to the 'objects' table and get the full record that belongs to
				// this relation.  We can create a new node from this information and add it to the tree.
				BlockOrderElement childElement =
					this.CreateBlockOrderElement(blockOrdersTreeRow.BlockOrderRowByFKBlockOrderBlockOrderTreeChildId);
				parentElement.AppendChild(childElement);

				// Go look for any children of this node.
				RecurseTree(childElement);

			}

			// Orders are also children of block orders: the relationship is easily defined.  There are no children of 
			// orders, so that's as far as we have to follow the relationships.
			foreach (ClientMarketData.OrderRow orderRow in parentElement.BlockOrderRow.GetOrderRows())
				parentElement.AppendChild(this.CreateOrdersElement(orderRow));

			foreach (ClientMarketData.PlacementRow placementRow in parentElement.BlockOrderRow.GetPlacementRows())
				parentElement.AppendChild(this.CreatePlacementsElement(placementRow));

			foreach (ClientMarketData.ExecutionRow executionRow in parentElement.BlockOrderRow.GetExecutionRows())
				parentElement.AppendChild(this.CreateExecutionElement(executionRow));

		}
		
		public void CalculateFields()
		{

			foreach (BlockOrderElement blockOrdersElement in this.SelectNodes(".//BlockOrder"))
			{
				blockOrdersElement.CalculateFields();
				Thread.Sleep(0);
			}

		}

	}

	/// <summary>
	/// The attributes and data of a block order.
	/// </summary>
	class BlockOrderElement : CommonElement
	{

		private ClientMarketData.BlockOrderRow blockOrderRow;

		public ClientMarketData.BlockOrderRow BlockOrderRow {get {return blockOrderRow;}}

		public BlockOrderElement(BlockOrderDocument blockOrderDocument, ClientMarketData.BlockOrderRow blockOrderRow) :
			base("BlockOrder", blockOrderDocument)
		{

			this.blockOrderRow = blockOrderRow;

			AddAttribute("BlockOrderId", this.blockOrderRow.BlockOrderId);
			AddAttribute("StatusCode", this.blockOrderRow.StatusCode);
			AddAttribute("StatusName", this.blockOrderRow.StatusRow.Mnemonic);

		}

		public void CalculateFields()
		{

			// The block order document has an operation that displays distinct values when the occur.  For instance, if 
			// all the orders in the block have the same time in force, then that time in force will appear in the
			// document at the block level.  However, if any one of the elements is different from the others, the same
			// column would be blanked out to show that there is more than one value associated with this attribute of the
			// block.  The first order is used as a seed for this operation.  If any element in the block doesn't share
			// the first order's attribute, then that attribute is left blank.
			decimal quantityOrdered = 0.0M;
			OrdersElement firstOrder = (OrdersElement)this.SelectSingleNode(".//Order");

			if (firstOrder != null)
			{

				// Aggregates by Orders.
				int orders = 0;
				int securityIdSum = 0;
				int accountIdSum = 0;
				int transactionTypeCodeSum = 0;
				int timeInForceCodeSum = 0;
				int orderTypeCodeSum = 0;
				decimal price1Sum = 0.0M;
				decimal price2Sum = 0.0M;
			
				foreach (OrdersElement ordersElement in this.SelectNodes("Order"))
				{
					orders++;
					securityIdSum += ordersElement.OrderRow.SecurityId;
					accountIdSum += ordersElement.OrderRow.AccountId;
					transactionTypeCodeSum += ordersElement.OrderRow.TransactionTypeCode;
					timeInForceCodeSum += ordersElement.OrderRow.TimeInForceCode;
					orderTypeCodeSum += ordersElement.OrderRow.OrderTypeCode;
					price1Sum += ordersElement.OrderRow.IsPrice1Null() ? 0.0M : ordersElement.OrderRow.Price1;
					price2Sum += ordersElement.OrderRow.IsPrice2Null() ? 0.0M : ordersElement.OrderRow.Price2;
					quantityOrdered += ordersElement.OrderRow.Quantity;
				}

				if (securityIdSum == firstOrder.OrderRow.SecurityId * orders)
				{

					AddAttribute("SecurityId", firstOrder.OrderRow.SecurityId);

					ClientMarketData.SecurityRow securityRow = firstOrder.OrderRow.SecurityRowByFKSecurityOrderSecurityId;
					AddAttribute("SecurityName", securityRow.ObjectRow.Name);
					AddAttribute("SecuritySymbol", securityRow.Symbol);

					ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityIdCurrencyId(
						firstOrder.OrderRow.SecurityId, firstOrder.OrderRow.SettlementId);
					if (priceRow != null)
					{

						AddAttribute("LastPrice", priceRow.LastPrice);
						AddAttribute("BidPrice", priceRow.BidPrice);
						AddAttribute("BidSize", priceRow.BidSize);
						AddAttribute("AskPrice", priceRow.AskPrice);
						AddAttribute("AskSize", priceRow.AskSize);
					}

					// Add Fixed Income Fundamentals where they exist.
					foreach (ClientMarketData.DebtRow debtRow in securityRow.GetDebtRowsByFKSecurityDebtDebtId())
					{

						if (!debtRow.IsIssuerIdNull())
							AddAttribute("IssuerId", debtRow.IssuerId);
						AddAttribute("DebtTypeCode", debtRow.DebtTypeCode);
						AddAttribute("Coupon", debtRow.Coupon);
						AddAttribute("MaturityDate", debtRow.MaturityDate.ToShortDateString());

					}
			
				}

				if (accountIdSum == firstOrder.OrderRow.AccountId * orders)
				{

					AddAttribute("AccountId", firstOrder.OrderRow.AccountId);

					ClientMarketData.AccountRow accountRow = firstOrder.OrderRow.AccountRow;
					AddAttribute("AccountName", accountRow.ObjectRow.Name);

				}
					
				if (transactionTypeCodeSum == firstOrder.OrderRow.TransactionTypeCode * orders)
				{
					AddAttribute("TransactionTypeCode", firstOrder.OrderRow.TransactionTypeCode);
					AddAttribute("TransactionTypeName", firstOrder.OrderRow.TransactionTypeRow.Mnemonic);
				}

				if (timeInForceCodeSum == firstOrder.OrderRow.TimeInForceCode * orders)
				{
					AddAttribute("TimeInForceCode", firstOrder.OrderRow.TimeInForceCode);
					AddAttribute("TimeInForceName", firstOrder.OrderRow.TimeInForceRow.Mnemonic);
				}

				if (orderTypeCodeSum == firstOrder.OrderRow.OrderTypeCode * orders)
				{
					AddAttribute("OrderTypeCode", firstOrder.OrderRow.OrderTypeCode);
					AddAttribute("OrderTypeName", firstOrder.OrderRow.OrderTypeRow.Mnemonic);
				}

				if (!firstOrder.OrderRow.IsPrice1Null() && price1Sum == firstOrder.OrderRow.Price1 * orders)
					AddAttribute("Price1", firstOrder.OrderRow.Price1);

				if (!firstOrder.OrderRow.IsPrice2Null() && price2Sum == firstOrder.OrderRow.Price2 * orders)
					AddAttribute("Price2", firstOrder.OrderRow.Price2);

				// HACK - Trade and Settlement are defaulted.  These should be distinct elements from the executions.
				AddAttribute("TradeDate", DateTime.Now.ToShortDateString());
				AddAttribute("SettlementDate", DateTime.Now.ToShortDateString());

			}

			AddAttribute("QuantityOrdered", quantityOrdered);

			// Aggregate Placements
			decimal quantityPlaced = 0.0M;
			PlacementsElement firstPlacement = (PlacementsElement)this.SelectSingleNode(".//Placement");

			if (firstPlacement != null)
			{

				// Aggregates by Placements.
				int placements = 0;
				int brokerIdSum = 0;

				foreach (PlacementsElement placementsElement in this.SelectNodes("Placement"))
				{
					placements++;
					brokerIdSum += placementsElement.PlacementRow.BrokerId;
					quantityPlaced += placementsElement.PlacementRow.Quantity;
				}

				if (brokerIdSum == firstPlacement.PlacementRow.BrokerId * placements)
				{

					AddAttribute("PlacementBrokerId", firstPlacement.PlacementRow.BrokerId);
					AddAttribute("PlacementBrokerSymbol", firstPlacement.PlacementRow.BrokerRow.Symbol);
			
				}

			}

			AddAttribute("QuantityPlaced", quantityPlaced);

			// Aggregate Execution
			decimal quantityExecuted = 0.0M;
			ExecutionElement firstExecution = (ExecutionElement)this.SelectSingleNode(".//Execution");

			if (firstExecution != null)
			{

				// Aggregates by Execution.
				int placements = 0;
				int brokerIdSum = 0;

				foreach (ExecutionElement placementsElement in this.SelectNodes("Execution"))
				{
					placements++;
					brokerIdSum += placementsElement.ExecutionRow.BrokerId;
					quantityExecuted += placementsElement.ExecutionRow.Quantity;
				}

				if (brokerIdSum == firstExecution.ExecutionRow.BrokerId * placements)
				{

					AddAttribute("PlacementBrokerId", firstExecution.ExecutionRow.BrokerId);
					AddAttribute("PlacementBrokerSymbol", firstExecution.ExecutionRow.BrokerRow.Symbol);
			
				}

			}

			AddAttribute("QuantityExecuted", quantityExecuted);

		}

	}

	/// <summary>
	/// The attributes and data of an order.
	/// </summary>
	class OrdersElement : CommonElement
	{

		private ClientMarketData.OrderRow orderRow;

		public ClientMarketData.OrderRow OrderRow {get {return orderRow;}}

		public OrdersElement(BlockOrderDocument blockOrderDocument, ClientMarketData.OrderRow orderRow) :
			base("Order", blockOrderDocument)
		{

			this.orderRow = orderRow;

			AddAttribute("OrderId", this.orderRow.OrderId);
			AddAttribute("AccountId", this.orderRow.AccountId);
			AddAttribute("SecurityId", this.orderRow.SecurityId);

			AddAttribute("SecurityName", this.orderRow.SecurityRowByFKSecurityOrderSecurityId.ObjectRow.Name);
			AddAttribute("SecuritySymbol", this.orderRow.SecurityRowByFKSecurityOrderSecurityId.Symbol);

			AddAttribute("SettlementName", this.orderRow.SecurityRowByFKSecurityOrderSettlementId.ObjectRow.Name);
			AddAttribute("SettlementSymbol", this.orderRow.SecurityRowByFKSecurityOrderSettlementId.Symbol);

			AddAttribute("PositionTypeCode", this.orderRow.PositionTypeCode);
			AddAttribute("TransactionTypeCode", this.orderRow.TransactionTypeCode);
			AddAttribute("TransactionTypeName", this.orderRow.TransactionTypeRow.Mnemonic);
			AddAttribute("Quantity", this.orderRow.Quantity);
			AddAttribute("TimeInForceCode", this.orderRow.TimeInForceCode);
			AddAttribute("TimeInForceName", this.orderRow.TimeInForceRow.Mnemonic);
			AddAttribute("OrderTypeCode", this.orderRow.OrderTypeCode);
			AddAttribute("OrderTypeName", this.orderRow.OrderTypeRow.Mnemonic);

			ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityIdCurrencyId(
				this.orderRow.SecurityId, this.orderRow.SettlementId);
			if (priceRow != null)
			{

				AddAttribute("LastPrice", priceRow.LastPrice);
				AddAttribute("BidPrice", priceRow.BidPrice);
				AddAttribute("BidSize", priceRow.BidSize);
				AddAttribute("AskPrice", priceRow.AskPrice);
				AddAttribute("AskSize", priceRow.AskSize);
			}

			if (!this.orderRow.IsPrice1Null())
				AddAttribute("Price1", this.orderRow.Price1);
			if (!this.orderRow.IsPrice2Null())
				AddAttribute("Price2", this.orderRow.Price2);
			if (!this.orderRow.IsConditionCodeNull())
				AddAttribute("ConditionCode", this.orderRow.ConditionCode);
			if (!this.orderRow.IsNoteNull())
				AddAttribute("Note", this.orderRow.Note);

		}

	}

	/// <summary>
	/// The attributes and data of a Placement
	/// </summary>
	class PlacementsElement : CommonElement
	{

		private ClientMarketData.PlacementRow placementRow;

		// Public access for the placement.
		public ClientMarketData.PlacementRow PlacementRow {get {return placementRow;}}

		/// <summary>
		/// Creates a Placement Element.
		/// </summary>
		/// <param name="blockOrderDocument">The parent document of this Xml Node.</param>
		/// <param name="placementRow">The placement record used to create the Xml Node.</param>
		public PlacementsElement(BlockOrderDocument blockOrderDocument, ClientMarketData.PlacementRow placementRow) :
			base("Placement", blockOrderDocument)
		{

			// Keep track of the record that created this element.
			this.placementRow = placementRow;

			// Apply the attributes to the element.
			AddAttribute("PlacementId", placementRow.PlacementId);
			AddAttribute("Quantity", placementRow.Quantity);
			AddAttribute("BrokerId", placementRow.BrokerId);
			AddAttribute("BrokerSymbol", placementRow.BrokerRow.Symbol);

		}

	}

	/// <summary>
	/// The attributes and data of a Execution
	/// </summary>
	class ExecutionElement : CommonElement
	{

		private ClientMarketData.ExecutionRow executionRow;

		// Public access for the execution.
		public ClientMarketData.ExecutionRow ExecutionRow {get {return executionRow;}}

		/// <summary>
		/// Creates a Execution Element.
		/// </summary>
		/// <param name="blockOrderDocument">The parent document of this Xml Node.</param>
		/// <param name="executionRow">The execution record used to create the Xml Node.</param>
		public ExecutionElement(BlockOrderDocument blockOrderDocument, ClientMarketData.ExecutionRow executionRow) :
			base("Execution", blockOrderDocument)
		{

			// Keep track of the record that created this element.
			this.executionRow = executionRow;

			// Apply the attributes to the element.
			AddAttribute("ExecutionId", executionRow.ExecutionId);
			AddAttribute("BrokerId", executionRow.BrokerId);
			AddAttribute("BrokerSymbol", executionRow.BrokerRow.Symbol);
			AddAttribute("Quantity", executionRow.Quantity);
			AddAttribute("Price", executionRow.Price);

		}

	}


}
