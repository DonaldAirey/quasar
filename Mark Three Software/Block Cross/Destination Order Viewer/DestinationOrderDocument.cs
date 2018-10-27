namespace MarkThree.Guardian.Forms
{

	using MarkThree.Guardian.Client;
	using MarkThree.Guardian;
	using MarkThree;
	using MarkThree.Forms;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;

	class DestinationOrderDocument : XmlDocument
	{

		/// <summary>
		/// Creates a well formed working order document object model from fragments of data.
		/// </summary>
		/// <param name="blotterId">A list of items to be included in the DOM.</param>
		public DestinationOrderDocument(FragmentList fragmentList)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// The root of the fragment document.
				XmlNode fragmentNode = this.AppendChild(this.CreateElement("Fragment"));

				// The insert, update and delete elements are only included only when there is data in those sections.
				XmlNode insertNode = null;
				XmlNode updateNode = null;
				XmlNode deleteNode = null;

				foreach (Fragment fragment in fragmentList)
				{

					// The generic record in the fragment is cast here to a DestinationOrderRow.  By design, this is the only type of 
					// record that will be placed into the FragmentList.
					ClientMarketData.DestinationOrderRow destinationOrderRow = (ClientMarketData.DestinationOrderRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{

					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new DestinationOrderElement(this, destinationOrderRow, FieldArray.Set));

						break;
				
				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new DestinationOrderElement(this, destinationOrderRow, fragment.Fields));

						break;

					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("DestinationOrder", this);
						commonElement.AddAttribute("DestinationOrderId", (int)fragment.Key[0]);
						deleteNode.AppendChild(commonElement);

						break;

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld) ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld) ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld) ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Creates a well formed working order document object model.
		/// </summary>
		/// <param name="blotterId">The blotter identifies what blocks are to be included in this document.</param>
		public DestinationOrderDocument(Blotter blotter, WorkingOrder[] workingOrders)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Create the root element for the document.
				XmlNode documentNode = this.AppendChild(this.CreateElement("Document"));

				if (workingOrders == null)
				{

					// Find the top level blotter record and recursively construct the report by merging all the children.
					ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(blotter.BlotterId);
					if (objectRow != null)
						RecurseBlotter(documentNode, objectRow);

				}
				else
				{

					foreach (WorkingOrder workingOrder in workingOrders)
					{
						ClientMarketData.WorkingOrderRow workingOrderRow = ClientMarketData.WorkingOrder.FindByWorkingOrderId(workingOrder.WorkingOrderId);
						if (workingOrderRow != null)
						{
							foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
								documentNode.AppendChild(new DestinationOrderElement(this, destinationOrderRow, FieldArray.Set));
						}

					}

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld) ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld) ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld) ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld) ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld) ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		private void RecurseBlotter(XmlNode parentNode, ClientMarketData.ObjectRow objectRow)
		{

			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in objectRow.GetObjectTreeRowsByObjectObjectTreeParentId())
				RecurseBlotter(parentNode, objectTreeRow.ObjectRowByObjectObjectTreeChildId);

			foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in blotterRow.GetWorkingOrderRows())
					foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
						parentNode.AppendChild(new DestinationOrderElement(this, destinationOrderRow, FieldArray.Set));

		}

	}

	/// <summary>
	/// The attributes and data of a working order.
	/// </summary>
	class DestinationOrderElement : CommonElement
	{

		decimal totalExecutedQuantity;
		decimal totalLeavesQuantity;
		decimal averagePriceExecuted;
		decimal totalCommission;
		decimal totalMarketValue;

		public DestinationOrderElement(DestinationOrderDocument destinationOrderDocument, ClientMarketData.DestinationOrderRow destinationOrderRow, FieldArray fields) :
			base("DestinationOrder", destinationOrderDocument)
		{

			// Aggregate all the data related to this staged order.  This will work out the aggregates and distinct column
			// operations.
			if (fields[Field.OrderedQuantity] || fields[Field.ExecutedQuantity])
				AggregateFields(destinationOrderRow);

			// DestinationOrderId - Primary Key for this report.
			AddAttribute("DestinationOrderId", destinationOrderRow.DestinationOrderId);

			// Status - Note that the status code is always provided to the DOM for color coding of the fields.
			AddAttribute("StatusCode", destinationOrderRow.StatusCode);
			if (fields[Field.Status])
				AddAttribute("StatusName", destinationOrderRow.StatusRow.Mnemonic);

			// Blotter
			if (fields[Field.Blotter])
			{
				AddAttribute("Blotter", destinationOrderRow.WorkingOrderRow.BlotterId);
				AddAttribute("BlotterName", destinationOrderRow.WorkingOrderRow.BlotterRow.ObjectRow.Name);
			}

			// OrderType
			if (fields[Field.OrderType])
			{
				AddAttribute("OrderTypeCode", destinationOrderRow.WorkingOrderRow.OrderTypeCode);
				AddAttribute("OrderTypeMnemonic", destinationOrderRow.WorkingOrderRow.OrderTypeRow.Mnemonic);
				AddAttribute("CashSign", destinationOrderRow.WorkingOrderRow.OrderTypeRow.CashSign);
				AddAttribute("QuantitySign", destinationOrderRow.WorkingOrderRow.OrderTypeRow.QuantitySign);
			}

			// TimeInForce
			if (fields[Field.TimeInForce])
			{
				AddAttribute("TimeInForceCode", destinationOrderRow.WorkingOrderRow.TimeInForceRow.TimeInForceCode);
				AddAttribute("TimeInForceName", destinationOrderRow.WorkingOrderRow.TimeInForceRow.Mnemonic);
			}

			// Security
			if (fields[Field.Security])
			{
				AddAttribute("SecurityId", destinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.SecurityId);
				AddAttribute("SecuritySymbol", destinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Symbol);
				AddAttribute("SecurityName", destinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.ObjectRow.Name);
			}

			//  Destination Order Total Quantity
			if (fields[Field.OrderedQuantity])
				AddAttribute("DestinationOrderQuantity", destinationOrderRow.OrderedQuantity);

			// Total Executed Quantity and Average Price
			if (fields[Field.ExecutedQuantity])
			{
				AddAttribute("ExecutedQuantity", this.totalExecutedQuantity);
				AddAttribute("AveragePrice", this.averagePriceExecuted);
			}

			//  Destination Order Leaves (Total Source Quantity - Total Destination Quantity)
			if (fields[Field.OrderedQuantity] || fields[Field.ExecutedQuantity])
				AddAttribute("LeavesQuantity", this.totalLeavesQuantity);
			
			// Destination
			if (fields[Field.Destination])
				if (!destinationOrderRow.WorkingOrderRow.IsDestinationIdNull())
				{
					AddAttribute("DestinationId", destinationOrderRow.WorkingOrderRow.DestinationRow.DestinationId);
					AddAttribute("DestinationName", destinationOrderRow.WorkingOrderRow.DestinationRow.Name);
					AddAttribute("DestinationShortName", destinationOrderRow.WorkingOrderRow.DestinationRow.ShortName);
				}

			// The Direction of this order (buy, sell, buy cover, etc.)
			if (fields[Field.PriceType])
			{
				AddAttribute("PriceTypeCode", destinationOrderRow.WorkingOrderRow.PriceTypeRow.PriceTypeCode);
				AddAttribute("PriceTypeMnemonic", destinationOrderRow.WorkingOrderRow.PriceTypeRow.Mnemonic);
			}

			// The remaining Destination Order Fields
			if (fields[Field.DestinationOrder])
			{

				// Created
				AddAttribute("CreatedName", destinationOrderRow.WorkingOrderRow.UserRowByUserWorkingOrderCreatedUserId.ObjectRow.Name);
				AddAttribute("CreatedTime", destinationOrderRow.WorkingOrderRow.CreatedTime.ToString("s"));

				// Commission
				AddAttribute("Commission", this.totalCommission);

				// FilledNet
				AddAttribute("NetMarketValue", this.totalMarketValue - destinationOrderRow.WorkingOrderRow.OrderTypeRow.CashSign * this.totalCommission);

				// Market
				//			AddAttribute("MarketId", destinationOrderRow.WorkingOrderRow.SecurityRowByFKSecurityDestinationOrderSecurityId.MarketRow.MarketId);
				//			AddAttribute("MarketName", destinationOrderRow.WorkingOrderRow.SecurityRowByFKSecurityDestinationOrderSecurityId.MarketRow.Name);
				//			AddAttribute("MarketShortName", destinationOrderRow.WorkingOrderRow.SecurityRowByFKSecurityDestinationOrderSecurityId.MarketRow.ShortName);

				// Limit Price
				if (!destinationOrderRow.WorkingOrderRow.IsLimitPriceNull())
					AddAttribute("LimitPrice", (decimal)destinationOrderRow.WorkingOrderRow.LimitPrice);

				// Stop Price
				if (!destinationOrderRow.WorkingOrderRow.IsStopPriceNull())
					AddAttribute("StopPrice", (decimal)destinationOrderRow.WorkingOrderRow.StopPrice);

				// TradeDate
				AddAttribute("TradeDate", destinationOrderRow.WorkingOrderRow.CreatedTime.ToString("s"));

				// CommissionType
				//				if (destinationOrderRow != null)
				//				{
				//				ClientMarketData.CommissionRateTypeRow commissionRateTypeRow =
				//					ClientMarketData.CommissionRateType.FindByCommissionRateTypeCode((int)this.commissionRateTypeCode);
				//				AddAttribute("CommissionRateTypeCode", commissionRateTypeRow.CommissionRateTypeCode);
				//				AddAttribute("CommissionRateTypeName", commissionRateTypeRow.Name);
				//					AddAttribute("CommissionRate", this.averageCommissionRate);
				//				}

				// Filled Gross
				AddAttribute("MarketValue", this.totalMarketValue);

			}

			// Find the pricing record.
			if (!destinationOrderRow.WorkingOrderRow.IsSettlementIdNull())
			{

				ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityId(destinationOrderRow.WorkingOrderRow.SecurityId);
				if (priceRow != null)
				{

					if (fields[Field.LastPrice])
						AddAttribute("LastPrice", priceRow.LastPrice);
					if (fields[Field.BidPrice])
						AddAttribute("BidPrice", priceRow.BidPrice);
					if (fields[Field.AskPrice])
						AddAttribute("AskPrice", priceRow.AskPrice);
					if (fields[Field.LastSize])
						AddAttribute("LastSize", priceRow.LastSize);
					if (fields[Field.BidSize])
						AddAttribute("BidSize", priceRow.BidSize);
					if (fields[Field.AskPrice])
						AddAttribute("AskSize", priceRow.AskSize);

				}

			}

		}

		public void AggregateFields(ClientMarketData.DestinationOrderRow destinationOrderRow)
		{

			this.totalExecutedQuantity = 0.0M;
			this.totalLeavesQuantity = 0.0M;
			this.averagePriceExecuted = 0.0M;
			this.totalCommission = 0.0M;
			this.totalMarketValue = 0.0M;

			foreach (ClientMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
			{
				this.totalExecutedQuantity += executionRow.ExecutionQuantity;
				this.totalMarketValue += executionRow.ExecutionQuantity * executionRow.ExecutionPrice;
			}

			this.totalLeavesQuantity = destinationOrderRow.OrderedQuantity - this.totalExecutedQuantity;

			this.averagePriceExecuted = this.totalExecutedQuantity == 0.0M ? 0.0M :
				this.totalMarketValue / this.totalExecutedQuantity;

		}

	}

}
