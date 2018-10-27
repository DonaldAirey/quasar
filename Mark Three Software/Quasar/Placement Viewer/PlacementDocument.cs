/**********************************************************************************************************************************
*
*	File:			PlacementDocument.cs
*	Description:	Creates the DOM for the placement document.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
**********************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Placement
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

	/// <summary>
	/// Creates a Document Object Model representation of a group of placements against a block order.
	/// </summary>
	class PlacementDocument : XmlDocument
	{

		/// <summary>
		/// Constructs and empty, but well formed, placement document.
		/// </summary>
		public PlacementDocument()
		{

			// Create the root element for the document.
			this.AppendChild(this.CreateElement("Placements"));

		}

		/// <summary>
		/// Constructs a well formed Document Object Model for a group of placements on a block order.
		/// </summary>
		/// <param name="BlockOrderId">The block order for which we want a placement document.</param>
		public PlacementDocument(int blockOrderId, PlacementSet placementSet)
		{

			// Create the root element for the document.
			XmlElement rootElement = this.CreateElement("Placements");
			this.AppendChild(rootElement);

			// This list is used to sort the placements.
			ArrayList placementsList = new ArrayList();

			// Sort the placements in the order which they were created.
			DataView dataView = new DataView(placementSet.Placement, null, "placementId", DataViewRowState.CurrentRows);

			// Copy the placements that belong to the currently viewed block order (remember that incomplete rows belonging to 
			// other blocks can still hang around in the PlacementSet until committed) into the document.
			foreach (DataRowView dataRowView in dataView)
			{
				PlacementSet.PlacementRow placementRow = (PlacementSet.PlacementRow)dataRowView.Row;
				if (placementRow.BlockOrderId == blockOrderId && !placementRow.IsPlacementIdNull())
					rootElement.AppendChild(this.CreatePlacementElement(placementRow));
			}

			// The temporary records are placed at the end.  This appears to be the more logical placement for the records in the
			// situation where an incomplete record was left in the viewer.  That is, it will most likely be the most recent record
			// in the list.  But because there's no global identifier, it can't be sorted by the order it was inserted in the
			// global tables.  So an assumption is made that, since it's the most reccent, it should be placed at the end of the
			// list.
			foreach (PlacementSet.PlacementRow placementRow in placementSet.Placement)
				if (placementRow.BlockOrderId == blockOrderId && placementRow.IsPlacementIdNull())
					rootElement.AppendChild(this.CreatePlacementElement(placementRow));

			// The placeholder is a queue for the document manager to create a new placement.
			this.DocumentElement.AppendChild(this.CreatePlaceholderElement());

		}

		/// <summary>
		/// Creates a placement element for the PlacementDocument
		/// </summary>
		/// <param name="placementRow">A Placement record</param>
		/// <returns>An element that represents a placement.</returns>
		public PlacementElement CreatePlacementElement(PlacementSet.PlacementRow placementRow)
		{
			return new PlacementElement(this, placementRow);
		}

		/// <summary>
		/// Creates a placeholder element for new Placement.
		/// </summary>
		/// <returns>An element that can be used as a prompt for new placements.</returns>
		public PlaceholderElement CreatePlaceholderElement()
		{
			return new PlaceholderElement(this);
		}

	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class PlacementElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the placement document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="placementRow">A placement record.</param>
		public PlacementElement(XmlDocument xmlDocument, PlacementSet.PlacementRow placementRow) :
			base("Placement", xmlDocument)
		{

			// This is the primary identifier for the record.
			AddAttribute("PlacementId", placementRow.LocalPlacementId.ToString());

			// Broker field
			if (!placementRow.IsBrokerIdNull())
			{

				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(placementRow.BrokerId);
				if (brokerRow != null)
				{
					AddAttribute("BrokerId", brokerRow.BrokerId.ToString());
					AddAttribute("BrokerName", brokerRow.ObjectRow.Name);
					AddAttribute("BrokerSymbol", brokerRow.Symbol);
				}

			}

			// TimeInForce field
			if (!placementRow.IsTimeInForceCodeNull())
			{

				ClientMarketData.TimeInForceRow timeInForceRow =
					ClientMarketData.TimeInForce.FindByTimeInForceCode(placementRow.TimeInForceCode);
				if (timeInForceRow != null)
				{
					AddAttribute("TimeInForceCode", placementRow.TimeInForceCode.ToString());
					AddAttribute("TimeInForceMnemonic", timeInForceRow.Mnemonic);
				}

			}

			// OrderType field
			if (!placementRow.IsOrderTypeCodeNull())
			{

				ClientMarketData.OrderTypeRow orderTypeRow =
					ClientMarketData.OrderType.FindByOrderTypeCode(placementRow.OrderTypeCode);
				if (orderTypeRow != null)
				{
					AddAttribute("OrderTypeCode", placementRow.OrderTypeCode.ToString());
					AddAttribute("OrderTypeMnemonic", orderTypeRow.Mnemonic);
				}

			}

			// Quantity
			if (!placementRow.IsQuantityNull())
				AddAttribute("Quantity", placementRow.Quantity.ToString());

			// Price 1
			if (!placementRow.IsPrice1Null())
				AddAttribute("Price1", placementRow.Price1.ToString());

			// Price 2
			if (!placementRow.IsPrice2Null())
				AddAttribute("Price2", placementRow.Price2.ToString());

			// Created Time
			if (!placementRow.IsCreatedTimeNull())
				AddAttribute("CreatedTime", placementRow.CreatedTime.ToString("s"));

			// Created Login
			if (!placementRow.IsCreatedLoginIdNull())
			{

				ClientMarketData.LoginRow createdLoginRow = ClientMarketData.Login.FindByLoginId(placementRow.CreatedLoginId);
				if (createdLoginRow != null)
				{
					AddAttribute("CreatedLoginId", createdLoginRow.LoginId.ToString());
					AddAttribute("CreatedLoginName", createdLoginRow.ObjectRow.Name);
				}

			}

			// Modified Time
			if (!placementRow.IsModifiedTimeNull())
				AddAttribute("ModifiedTime", placementRow.ModifiedTime.ToString("s"));

			// Modified Login
			if (!placementRow.IsModifiedLoginIdNull())
			{

				ClientMarketData.LoginRow modifiedLoginRow = ClientMarketData.Login.FindByLoginId(placementRow.ModifiedLoginId);
				if (modifiedLoginRow != null)
				{
					AddAttribute("ModifiedLoginId", modifiedLoginRow.LoginId.ToString());
					AddAttribute("ModifiedLoginName", modifiedLoginRow.ObjectRow.Name);
				}

			}

		}
	
	}

	/// <summary>
	/// A XML Element representing a placment in the document.
	/// </summary>
	class PlaceholderElement : CommonElement
	{

		/// <summary>
		/// Creates an XML Element representing a placment in the placement document.
		/// </summary>
		/// <param name="xmlDocument">The destination XML document.</param>
		/// <param name="placementRow">A placement record.</param>
		public PlaceholderElement(XmlDocument xmlDocument) : base("Placeholder", xmlDocument)
		{

			// This provided an empty placeholder for the stylesheet to display the input prompt.

		}

	}

}
