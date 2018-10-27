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

	class WorkingOrderDocument : XmlDocument
	{

		/// <summary>
		/// Creates a well formed working order document object model from fragments of data.
		/// </summary>
		/// <param name="fragmentList">A collection of fragments to be added/removed from the document in the viewer.</param>
		public WorkingOrderDocument(FragmentList fragmentList)
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
				ClientMarketData.ImageLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.VolumeCategoryLock.AcquireReaderLock(CommonTimeout.LockWait);

				// The root of the fragment document.
				XmlNode fragmentNode = this.AppendChild(this.CreateElement("Fragment"));

				// The insert, update and delete elements are only included only when there is data in those sections.
				XmlNode insertNode = null;
				XmlNode updateNode = null;
				XmlNode deleteNode = null;

				foreach (Fragment fragment in fragmentList)
				{

					// The generic record in the fragment is cast here to a WorkingOrderRow.  By design, this is the only type of 
					// record that will be placed into the FragmentList.
					ClientMarketData.WorkingOrderRow workingOrderRow = (ClientMarketData.WorkingOrderRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{

					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new WorkingOrderElement(this, workingOrderRow, FieldArray.Set));

						break;
				
				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new WorkingOrderElement(this, workingOrderRow, fragment.Fields));

						break;

					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("WorkingOrder", this);
						commonElement.AddAttribute("WorkingOrderId", (int)fragment.Key[0]);
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
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld)
					ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld)
					ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld)
					ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld)
					ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ImageLock.IsReaderLockHeld)
					ClientMarketData.ImageLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld)
					ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld)
					ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Creates a well formed working order document object model.
		/// </summary>
		/// <param name="userId">The blotter identifies what blocks are to be included in this document.</param>
		public WorkingOrderDocument(int blotterId)
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
				ClientMarketData.ImageLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimeInForceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TimerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.VolumeCategoryLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Create the root element for the document.
				XmlNode documentNode = this.AppendChild(this.CreateElement("Document"));

				// Find the top level blotter record and recursively construct the report by merging all the children.
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(blotterId);
				if (objectRow != null)
					RecurseBlotter(documentNode, objectRow);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld)
					ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld)
					ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld)
					ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld)
					ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.ImageLock.IsReaderLockHeld)
					ClientMarketData.ImageLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld)
					ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.TimerLock.IsReaderLockHeld)
					ClientMarketData.TimerLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				if (ClientMarketData.VolumeCategoryLock.IsReaderLockHeld)
					ClientMarketData.VolumeCategoryLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		private void RecurseBlotter(XmlNode parentNode, ClientMarketData.ObjectRow objectRow)
		{

			foreach (ClientMarketData.ObjectTreeRow objectTreeRow in objectRow.GetObjectTreeRowsByObjectObjectTreeParentId())
				RecurseBlotter(parentNode, objectTreeRow.ObjectRowByObjectObjectTreeChildId);

			foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
				foreach (ClientMarketData.WorkingOrderRow workingOrderRow in blotterRow.GetWorkingOrderRows())
					parentNode.AppendChild(new WorkingOrderElement(this, workingOrderRow, FieldArray.Set));

		}

	}

	/// <summary>
	/// The attributes and data of a working order.
	/// </summary>
	class WorkingOrderElement : CommonElement
	{

		decimal totalSourceOrderedQuantity;
		decimal totalDestinationOrderedQuantity;
		decimal totalWorkingQuantity;
		decimal totalExecutedQuantity;
		decimal totalAllocatedQuantity;
		decimal totalLeavesQuantity;
		decimal averagePriceExecuted;
		decimal totalCommission;
//		decimal averageCommissionRate;
		decimal totalMarketValue;

		public WorkingOrderElement(WorkingOrderDocument workingOrderDocument, ClientMarketData.WorkingOrderRow workingOrderRow, FieldArray fields) :
			base("WorkingOrder", workingOrderDocument)
		{

			// Aggregate all the data related to this staged order.  This will work out the aggregates and distinct column 
			// operations.
			if (fields[Field.SourceOrder] || fields[Field.DestinationOrder] || fields[Field.Execution] || fields[Field.Allocation])
				AggregateFields(workingOrderRow);

			// WorkingOrderId - Primary Key for this report.
			AddAttribute("WorkingOrderId", workingOrderRow.WorkingOrderId);

			// Status - Note that the status code is always provided to the DOM for color coding of the fields.
			AddAttribute("StatusCode", workingOrderRow.StatusCode);
			if (fields[Field.Status])
				AddAttribute("StatusName", workingOrderRow.StatusRow.Mnemonic);

			// Select a green flag for submitted records, a red flag for unsubmitted.
			int imageIndex = -1;
			switch (workingOrderRow.StatusCode)
			{
			case Status.Error:
				imageIndex = ClientMarketData.Image.KeyImageExternalId0.Find("Error Small");
				break;
			case Status.Submitted:
				imageIndex = ClientMarketData.Image.KeyImageExternalId0.Find("Flag Green Small");
				break;
			default:
				imageIndex = ClientMarketData.Image.KeyImageExternalId0.Find("Flag Red Small");
				break;
			}
			if (imageIndex != -1)
			{
				ClientMarketData.ImageRow imageRow =
					(ClientMarketData.ImageRow)ClientMarketData.Image.KeyImageExternalId0[imageIndex].Row;
				AddAttribute("StatusImage", imageRow.Image);
			}
			
			// Blotter
			if (fields[Field.Blotter])
			{
				AddAttribute("Blotter", workingOrderRow.BlotterId);
				AddAttribute("BlotterName", workingOrderRow.BlotterRow.ObjectRow.Name);
			}

			// SubmissionTypeCode
			if (fields[Field.SubmissionTypeCode])
				AddAttribute("SubmissionTypeCode", workingOrderRow.SubmissionTypeCode);

			// IsBrokerMatch
			if (fields[Field.IsBrokerMatch])
				AddAttribute("IsBrokerMatch", workingOrderRow.IsBrokerMatch);

			// IsInstitutionMatch
			if (fields[Field.IsInstitutionMatch])
				AddAttribute("IsInstitutionMatch", workingOrderRow.IsInstitutionMatch);

			// IsHedgeMatch
			if (fields[Field.IsHedgeMatch])
				AddAttribute("IsHedgeMatch", workingOrderRow.IsHedgeMatch);


            // Auto-Execute - we always send the isAutomatic flag and the quantity
            if (fields[Field.AutoExecute])
            {
                AddAttribute("IsAutomatic", workingOrderRow.IsAutomatic);

                if (!workingOrderRow.IsAutomaticQuantityNull())
                    AddAttribute("AutomaticQuantity", workingOrderRow.AutomaticQuantity);
                else
                    AddAttribute("AutomaticQuantity", 0);
            }

            // LimitPrice
            if (fields[Field.LimitPrice])
            {
                if (!workingOrderRow.IsLimitPriceNull())
                    AddAttribute("LimitPrice", workingOrderRow.LimitPrice);
                else
                    AddAttribute("LimitPrice", 0);
            }
            
			// OrderType
			if (fields[Field.OrderType])
			{
				AddAttribute("OrderTypeCode", workingOrderRow.OrderTypeCode);
				AddAttribute("OrderTypeDescription", workingOrderRow.OrderTypeRow.Description);
				AddAttribute("OrderTypeMnemonic", workingOrderRow.OrderTypeRow.Mnemonic);
				AddAttribute("CashSign", workingOrderRow.OrderTypeRow.CashSign);
				AddAttribute("QuantitySign", workingOrderRow.OrderTypeRow.QuantitySign);

				// Select a green flag for submitted records, a red flag for unsubmitted.
				int orderTypeImageIndex = -1;
				switch (workingOrderRow.OrderTypeCode)
				{
				case OrderType.Buy:
					orderTypeImageIndex = ClientMarketData.Image.KeyImageExternalId0.Find("Navigate Plain Green Small");
					break;
				case OrderType.Sell:
					orderTypeImageIndex = ClientMarketData.Image.KeyImageExternalId0.Find("Navigate Plain Red Small");
					break;
				}
				if (orderTypeImageIndex != -1)
				{
					ClientMarketData.ImageRow imageRow =
						(ClientMarketData.ImageRow)ClientMarketData.Image.KeyImageExternalId0[orderTypeImageIndex].Row;
					AddAttribute("OrderTypeImage", imageRow.Image);
				}

			}

			// TimeInForce
			if (fields[Field.TimeInForce])
			{
				AddAttribute("TimeInForceCode", workingOrderRow.TimeInForceRow.TimeInForceCode);
				AddAttribute("TimeInForceName", workingOrderRow.TimeInForceRow.Mnemonic);
			}

			// Security
			if (fields[Field.Security])
			{
				AddAttribute("SecurityId", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.SecurityId);
				AddAttribute("SecuritySymbol", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Symbol);
				AddAttribute("SecurityName", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.ObjectRow.Name);
				if (!workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.IsMarketCapitalizationNull())
					AddAttribute("MarketCapitalization", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.MarketCapitalization / 1000000.0m);
				if (!workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.IsAverageDailyVolumeNull())
					AddAttribute("AverageDailyVolume", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.AverageDailyVolume / 1000.0m);
				AddAttribute("VolumeCategoryMnemonic", workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.VolumeCategoryRow.Mnemonic);

			}

			// Source Order Total Quantity
			if (fields[Field.SourceOrder])
				AddAttribute("SourceOrderQuantity", this.totalSourceOrderedQuantity);

			//  Destination Order Total Quantity
			if (fields[Field.DestinationOrder])
				AddAttribute("DestinationOrderQuantity", this.totalDestinationOrderedQuantity);

			// Total Executed Quantity and Average Price
			if (fields[Field.SubmittedQuantity])
				AddAttribute("SubmittedQuantity", workingOrderRow.SubmittedQuantity);

			// Total Executed Quantity and Average Price
			if (fields[Field.Execution])
			{
				AddAttribute("ExecutedQuantity", this.totalExecutedQuantity);
				AddAttribute("AveragePrice", this.averagePriceExecuted);
			}

			// Total Quantity Allocated
			if (fields[Field.Allocation])
				AddAttribute("AllocatedQuantity", this.totalAllocatedQuantity);

			//  Working Order Leaves (Total Source Quantity - Total Destination Quantity)
			if (fields[Field.SourceOrder] || fields[Field.DestinationOrder])
				AddAttribute("LeavesQuantity", this.totalLeavesQuantity);
			
			//  Working Order Working Quantity (Total Source Quantity - Total Executed Quantity)
			if (fields[Field.SourceOrder] || fields[Field.Execution])
				AddAttribute("WorkingQuantity", this.totalWorkingQuantity);

			// Start Time
			if (fields[Field.StartTime] && !workingOrderRow.IsStartTimeNull())
				AddAttribute("StartTime", workingOrderRow.StartTime.ToString("s"));

			// Stop Time
			if (fields[Field.StopTime] && !workingOrderRow.IsStopTimeNull())
				AddAttribute("StopTime", workingOrderRow.StopTime.ToString("s"));

			// Maxmimum Volatility
			if (fields[Field.MaximumVolatility] && !workingOrderRow.IsMaximumVolatilityNull())
				AddAttribute("MaximumVolatility", workingOrderRow.MaximumVolatility);

			// News Free Time
			if (fields[Field.NewsFreeTime] && !workingOrderRow.IsNewsFreeTimeNull())
				AddAttribute("NewsFreeTime", workingOrderRow.NewsFreeTime);

			// Timer Field
			if (fields[Field.Timer])
			{
				TimeSpan timeLeft = workingOrderRow.TimerRow.StopTime.Subtract(workingOrderRow.TimerRow.CurrentTime);
				AddAttribute("TimeLeft", string.Format("{0:0}:{1:00}", timeLeft.Minutes, timeLeft.Seconds));
			}
			
			// The Working Order Fields
			if (fields[Field.WorkingOrder])
			{

				// Created
				AddAttribute("CreatedName", workingOrderRow.UserRowByUserWorkingOrderCreatedUserId.ObjectRow.Name);
				AddAttribute("CreatedTime", workingOrderRow.CreatedTime.ToString("s"));

				// Destination		
				if (!workingOrderRow.IsDestinationIdNull())
				{
					AddAttribute("DestinationId", workingOrderRow.DestinationRow.DestinationId);
					AddAttribute("DestinationName", workingOrderRow.DestinationRow.Name);
					AddAttribute("DestinationShortName", workingOrderRow.DestinationRow.ShortName);
				}

				// The Direction of this order (buy, sell, buy cover, etc.)
				AddAttribute("PriceTypeCode", workingOrderRow.PriceTypeRow.PriceTypeCode);
				AddAttribute("PriceTypeMnemonic", workingOrderRow.PriceTypeRow.Mnemonic);

				// Commission
				AddAttribute("Commission", this.totalCommission);

				// FilledNet
				AddAttribute("NetMarketValue", this.totalMarketValue - workingOrderRow.OrderTypeRow.CashSign * this.totalCommission);

				// Market
				//			AddAttribute("MarketId", workingOrderRow.SecurityRowByFKSecurityWorkingOrderSecurityId.MarketRow.MarketId);
				//			AddAttribute("MarketName", workingOrderRow.SecurityRowByFKSecurityWorkingOrderSecurityId.MarketRow.Name);
				//			AddAttribute("MarketShortName", workingOrderRow.SecurityRowByFKSecurityWorkingOrderSecurityId.MarketRow.ShortName);

				// LimitPrice
                // NOTE: LimitPrice is not sent when the WorkingOrder flag is set.
                // LimitPrice has a separate bit set in the field arary

				// Stop Price
				if (!workingOrderRow.IsStopPriceNull())
					AddAttribute("StopPrice", (decimal)workingOrderRow.StopPrice);

				// TradeDate
				AddAttribute("TradeDate", workingOrderRow.CreatedTime.ToString("s"));

				// UploadTime
				if (!workingOrderRow.IsUploadedTimeNull())
					AddAttribute("UploadTime", workingOrderRow.CreatedTime.ToString("s"));

				// CommissionType
				//				if (workingOrderRow != null)
				//				{
				//				ClientMarketData.CommissionRateTypeRow commissionRateTypeRow =
				//					ClientMarketData.CommissionRateType.FindByCommissionRateTypeCode((int)this.commissionRateTypeCode);
				//				AddAttribute("CommissionRateTypeCode", commissionRateTypeRow.CommissionRateTypeCode);
				//				AddAttribute("CommissionRateTypeName", commissionRateTypeRow.Name);
				//					AddAttribute("CommissionRate", this.averageCommissionRate);
				//				}

				// Filled Gross
				AddAttribute("MarketValue", this.totalMarketValue);

				// Unfilled
				AddAttribute("Unfilled", this.totalSourceOrderedQuantity - this.totalExecutedQuantity);

			}

			// Find the pricing record.
			if (!workingOrderRow.IsSettlementIdNull())
			{

				ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityId(workingOrderRow.SecurityId);
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
                    if (fields[Field.Volume])
                        AddAttribute("Volume", priceRow.Volume);
                    if (fields[Field.InterpolatedVolume])
                    {
                        AddAttribute("InterpolatedVolume", VolumeHelper.CurrentVolumePercentage(DateTime.Now,
                            workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.AverageDailyVolume,
                            priceRow.Volume));
                    }
                    if (fields[Field.VolumeWeightedAveragePrice])
                        AddAttribute("VolumeWeightedAveragePrice", priceRow.VolumeWeightedAveragePrice);

				}

			}

		}

		public void AggregateFields(ClientMarketData.WorkingOrderRow workingOrderRow)
		{

			this.totalSourceOrderedQuantity = 0.0M;
			this.totalDestinationOrderedQuantity = 0.0M;
			this.totalWorkingQuantity = 0.0M;
			this.totalExecutedQuantity = 0.0M;
			this.totalAllocatedQuantity = 0.0M;
			this.totalLeavesQuantity = 0.0M;
			this.averagePriceExecuted = 0.0M;
			this.totalCommission = 0.0M;
			this.totalMarketValue = 0.0M;
//			this.averageCommissionRate = 0.0M;

			foreach (ClientMarketData.SourceOrderRow customerOrderRow in workingOrderRow.GetSourceOrderRows())
				this.totalSourceOrderedQuantity += customerOrderRow.OrderedQuantity;

			foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
			{

				this.totalDestinationOrderedQuantity += destinationOrderRow.OrderedQuantity - destinationOrderRow.CanceledQuantity;

				foreach (ClientMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
				{
					this.totalExecutedQuantity += executionRow.ExecutionQuantity;
					this.totalMarketValue += executionRow.ExecutionQuantity * executionRow.ExecutionPrice;
				}

			}

			this.totalWorkingQuantity = this.totalSourceOrderedQuantity - this.totalExecutedQuantity;
			this.totalLeavesQuantity = this.totalSourceOrderedQuantity - this.totalDestinationOrderedQuantity;

			this.averagePriceExecuted = this.totalExecutedQuantity == 0.0M ? 0.0M :
				this.totalMarketValue / this.totalExecutedQuantity;

			foreach (ClientMarketData.AllocationRow allocationRow in workingOrderRow.GetAllocationRows())
			{

				this.totalAllocatedQuantity += allocationRow.Quantity;
				this.totalCommission += allocationRow.Commission;

			}

		}

	}

}
