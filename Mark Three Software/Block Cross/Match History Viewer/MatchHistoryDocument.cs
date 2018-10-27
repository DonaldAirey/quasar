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

	class MatchHistoryDocument : XmlDocument
	{

		/// <summary>
		/// Creates a well formed working order document object model from fragments of data.
		/// </summary>
		/// <param name="userId">A list of items to be included in the DOM.</param>
		public MatchHistoryDocument(FragmentList fragmentList)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
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

					// The generic record in the fragment is cast here to a WorkingOrderRow.  By design, this is the only type of 
					// record that will be placed into the FragmentList.
					ClientMarketData.MatchRow matchRow = (ClientMarketData.MatchRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{

					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new MatchElement(this, matchRow, FieldArray.Set));

						break;
				
				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new MatchElement(this, matchRow, fragment.Fields));

						break;

					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("Match", this);
						commonElement.AddAttribute("MatchId", (int)fragment.Key[0]);
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
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.MatchLock.IsReaderLockHeld) ClientMarketData.MatchLock.ReleaseReaderLock();
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
					foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
						parentNode.AppendChild(new MatchElement(this, matchRow, FieldArray.Set));

		}

		/// <summary>
		/// Creates a well formed working order document object model.
		/// </summary>
		/// <param name="userId">The blotter identifies what blocks are to be included in this document.</param>
		public MatchHistoryDocument(int userId)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
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

				// Find the top level blotter record and recursively construct the report by merging all the children.
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(userId);
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
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld) ClientMarketData.SourceOrderLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.MatchLock.IsReaderLockHeld) ClientMarketData.MatchLock.ReleaseReaderLock();
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

	}

	/// <summary>
	/// The attributes and data of a working order.
	/// </summary>
	class MatchElement : CommonElement
	{

		public MatchElement(MatchHistoryDocument matchDocument, ClientMarketData.MatchRow matchRow, FieldArray fields) :
			base("Match", matchDocument)
		{

			// WorkingOrderId
			AddAttribute("MatchId", matchRow.MatchId);
			AddAttribute("WorkingOrderId", matchRow.WorkingOrderId);
			AddAttribute("ContraOrderId", matchRow.ContraOrderId);

			// Status
			if (fields[Field.Status])
			{
				AddAttribute("StatusCode", matchRow.StatusCode);
				AddAttribute("StatusName", matchRow.StatusRow.Mnemonic);
			}

			// Blotter
			if (fields[Field.Blotter])
			{
				AddAttribute("Blotter", matchRow.WorkingOrderRow.BlotterRow.BlotterId);
				AddAttribute("BlotterName", matchRow.WorkingOrderRow.BlotterRow.ObjectRow.Name);
			}

			// Security
			if (fields[Field.Security])
			{
				AddAttribute("SecurityId", matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.SecurityId);
				AddAttribute("SecuritySymbol", matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Symbol);
				AddAttribute("SecurityName", matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.ObjectRow.Name);
				if (!matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.IsAverageDailyVolumeNull())
					AddAttribute("AverageDailyVolume", matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.AverageDailyVolume / 1000);
				if (!matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.IsMarketCapitalizationNull())
					AddAttribute("MarketCapitalization", matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.MarketCapitalization / 1000000);

				MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Logo));
				Bitmap bitmap = new Bitmap(memoryStream);
				Bitmap halfBitmap = new Bitmap(bitmap, new Size(bitmap.Width / 2, bitmap.Height / 2));
				MemoryStream halfStream = new MemoryStream();
				halfBitmap.Save(halfStream, System.Drawing.Imaging.ImageFormat.Png);
				AddAttribute("SecurityImage", Convert.ToBase64String(halfStream.GetBuffer()));
			}

			// Submitted Quantity
			if (fields[Field.SubmittedQuantity])
				AddAttribute("SubmittedQuantity", matchRow.WorkingOrderRow.SubmittedQuantity);

			// Quantity Executed
			if (fields[Field.Execution])
			{

				decimal executedQuantity = decimal.Zero;
				decimal marketValue = decimal.Zero;

				foreach (ClientMarketData.NegotiationRow negotiationRow in matchRow.GetNegotiationRows())
					if (negotiationRow.ExecutionRow != null)
					{
						executedQuantity = negotiationRow.ExecutionRow.ExecutionQuantity;
						marketValue = negotiationRow.ExecutionRow.ExecutionPrice * negotiationRow.ExecutionRow.ExecutionQuantity;
					}

				if (executedQuantity != decimal.Zero)
				{
					AddAttribute("ExecutedQuantity", executedQuantity);
					AddAttribute("AveragePrice", marketValue / executedQuantity);
				}

			}

			// Direction (OrderType)
			if (fields[Field.OrderTypeCode])
			{
				AddAttribute("OrderTypeCode", matchRow.WorkingOrderRow.OrderTypeCode);
				AddAttribute("OrderTypeMnemonic", matchRow.WorkingOrderRow.OrderTypeRow.Mnemonic);
				AddAttribute("OrderTypeDescription", matchRow.WorkingOrderRow.OrderTypeRow.Description);
			}

			// Created Time.
			if (fields[Field.CreatedTime])
			{
				AddAttribute("CreatedTime", matchRow.CreatedTime.ToString("s"));
			}

			// Find the pricing record.
			if (!matchRow.WorkingOrderRow.IsSettlementIdNull())
			{

				ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityId(matchRow.WorkingOrderRow.SecurityId);
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
                    if (fields[Field.VolumeWeightedAveragePrice])
                        AddAttribute("VolumeWeightedAveragePrice", priceRow.VolumeWeightedAveragePrice);

				}

			}


		}

	}

}
