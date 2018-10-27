namespace MarkThree.Guardian
{

	using MarkThree;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Client;
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

	/// <summary>
	/// Creates a Document Object Model representation of a group of executions against a block order.
	/// </summary>
	class AdvertisementDocument : XmlDocument
	{

		/// <summary>
		/// Constructs and empty, but well formed, execution document.
		/// </summary>
		public AdvertisementDocument()
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Advertisements"));

		}

		/// <summary>
		/// Constructs a well formed Document Object Model for a group of executions on a block order.
		/// </summary>
		/// <param name="workingOrderId">The block order for which we want a execution document.</param>
		public AdvertisementDocument(int blotterId)
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Advertisements");
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


		}
		
		/// <summary>
		/// Creates a workingOrder element for the AdvertisementDocument
		/// </summary>
		/// <param name="workingOrderRow">A WorkingOrder record</param>
		/// <returns>An element that represents a workingOrder.</returns>
		public WorkingOrderElement CreateWorkingOrderElement(ClientMarketData.WorkingOrderRow workingOrderRow)
		{
			return new WorkingOrderElement(this, workingOrderRow);
		}

		/// <summary>
		/// Creates a broker element for the AdvertisementDocument
		/// </summary>
		/// <param name="brokerRow">A Broker record</param>
		/// <returns>An element that represents a broker.</returns>
		public BrokersElement CreateBrokersElement(ClientMarketData.BrokerRow brokerRow)
		{
			return new BrokersElement(this, brokerRow);
		}

		/// <summary>
		/// Creates a execution element for the AdvertisementDocument
		/// </summary>
		/// <param name="executionRow">A Advertisement record</param>
		/// <returns>An element that represents a execution.</returns>
		public ExecutionElement CreateExecutionElement(ClientMarketData.ExecutionRow executionRow)
		{
			return new ExecutionElement(this, executionRow);
		}

	}

	/// <summary>
	/// A XML Element representing a block order in the document.
	/// </summary>
	class WorkingOrderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a workingOrder in a execution document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="workingOrderRow">A workingOrder record.</param>
		public WorkingOrderElement(XmlDocument xmlDocument, ClientMarketData.WorkingOrderRow workingOrderRow) : base("WorkingOrder", xmlDocument)
		{

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
			AddAttribute("BrokerName", brokerRow.SourceRow.BlotterRow.ObjectRow.Name);
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
			AddAttribute("Quantity", executionRow.ExecutionQuantity.ToString());
			AddAttribute("Price", executionRow.ExecutionPrice.ToString());
			AddAttribute("Commission", executionRow.Commission.ToString());
			AddAttribute("AccruedInterest", executionRow.AccruedInterest.ToString());
			AddAttribute("UserFee0", executionRow.UserFee0.ToString());
			AddAttribute("UserFee1", executionRow.UserFee1.ToString());
			AddAttribute("UserFee2", executionRow.UserFee2.ToString());
			AddAttribute("UserFee3", executionRow.UserFee3.ToString());
			AddAttribute("TradeDate", executionRow.TradeDate.ToString("s"));
			AddAttribute("SettlementDate", executionRow.SettlementDate.ToString("s"));
			AddAttribute("CreatedTime", executionRow.CreatedTime.ToString("s"));

		}

	}

}
