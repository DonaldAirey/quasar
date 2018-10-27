
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

    class QuoteDocument : XmlDocument
    {
        /// <summary>
		/// Creates a well formed quote document object model from fragments of data.
		/// </summary>
		/// <param name="fragmentList">A collection of fragments to be added/removed from the document in the viewer.</param>
		public QuoteDocument(FragmentList fragmentList)
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
					ClientMarketData.PriceRow priceRow = (ClientMarketData.PriceRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{
                    
					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new QuoteElement(this, priceRow, FieldArray.Set));

						break;
				    				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new QuoteElement(this, priceRow, fragment.Fields));

						break;
                
					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("QuoteOrder", this);
						commonElement.AddAttribute("SecurityId", (int)fragment.Key[0]);
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

        /*
        private void RecurseBlotter(XmlNode parentNode, ClientMarketData.ObjectRow objectRow)
        {
            foreach (ClientMarketData.ObjectTreeRow objectTreeRow in objectRow.GetObjectTreeRowsByObjectObjectTreeParentId())
                RecurseBlotter(parentNode, objectTreeRow.ObjectRowByObjectObjectTreeChildId);

            foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
                foreach (ClientMarketData.WorkingOrderRow workingOrderRow in blotterRow.GetWorkingOrderRows())
                    foreach (ClientMarketData.MatchRow matchRow in workingOrderRow.GetMatchRows())
                        parentNode.AppendChild(new MatchElement(this, matchRow, FieldArray.Set));

        }
        */


        /// <summary>
		/// Creates a well formed quote document object model.
		/// </summary>
		/// <param name="securityID">The user id that identifies the blotter being viewed.</param>
		public QuoteDocument(int securityId)
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

				// build a Quote order document based on security id
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(securityId);

                // navigate down to the price row 
                if (objectRow != null)
                {
                    // get the security rows array - make sure it is non-null and there is at least 1 security in there
                    ClientMarketData.SecurityRow[] securityRows = objectRow.GetSecurityRows();
                    if (securityRows != null && securityRows.Length > 0)
                    {
                        // get the price rows - make sure non-null and there is at least 1 price row
                        ClientMarketData.PriceRow[] priceRows = securityRows[0].GetPriceRows();
                        if (priceRows != null && priceRows.Length > 0)
                        {
                            documentNode.AppendChild(new QuoteElement(this, priceRows[0], FieldArray.Set));
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
	/// The attributes and data of a quote.
	/// </summary>
    class QuoteElement : CommonElement
    {

        public QuoteElement(QuoteDocument quoteDocument, ClientMarketData.PriceRow priceRow, FieldArray fields)
            : base("Quote", quoteDocument)
        {
            // try to add the data elements from the security row
            if (priceRow.SecurityRow != null)
            {

                // security name
                if (fields[Field.SecurityName])
                    AddAttribute("SecurityName", priceRow.SecurityRow.ObjectRow.Name);

                // symbol
                if (fields[Field.Symbol])
                    AddAttribute("Symbol", priceRow.SecurityRow.Symbol);

            }

            // try to add the data elements from the price row
            if (priceRow != null)
            {
                // SecurityId
                AddAttribute("SecurityId", priceRow.SecurityId);

                // last trade
                if (fields[Field.LastPrice])
                {
                    AddAttribute("LastPrice", priceRow.LastPrice);

                    decimal dayPriceDiff = priceRow.LastPrice - priceRow.OpenPrice;
                    AddAttribute("DayPriceDifference", priceRow.LastPrice - priceRow.OpenPrice);

					AddAttribute("DayPricePercentageDifference", priceRow.OpenPrice == 0.0M ? 0.0M :
						(priceRow.LastPrice - priceRow.OpenPrice) / priceRow.OpenPrice);

                }

                if (fields[Field.PreviousClosePrice])
                    AddAttribute("PreviousClosePrice", priceRow.ClosePrice);

                if (fields[Field.OpenPrice])
                    AddAttribute("OpenPrice", priceRow.OpenPrice);
    
                if (fields[Field.LastSize])
                    AddAttribute("LastSize", priceRow.LastSize);
                
                // price
                if (fields[Field.BidPrice])
                    AddAttribute("BidPrice", priceRow.BidPrice);
                if (fields[Field.AskPrice])
                    AddAttribute("AskPrice", priceRow.AskPrice);
                
                // size
                if (fields[Field.AskSize])
                    AddAttribute("AskSize", priceRow.AskSize);
                if (fields[Field.BidSize])
                    AddAttribute("BidSize", priceRow.BidSize);
                
                // volume
                if (fields[Field.Volume])
                    AddAttribute("Volume", priceRow.Volume);
                if (fields[Field.AverageDailyVolume])
                    AddAttribute("AverageDailyVolume", priceRow.SecurityRow.AverageDailyVolume);

                if (fields[Field.DayLowPrice])
                    AddAttribute("DayLowPrice", priceRow.LowPrice);
                if (fields[Field.DayHighPrice])
                    AddAttribute("DayHighPrice", priceRow.HighPrice);
             
                /* LOGO ???
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Logo));
                Bitmap bitmap = new Bitmap(memoryStream);
                Bitmap halfBitmap = new Bitmap(bitmap, new Size(bitmap.Width / 2, bitmap.Height / 2));
                MemoryStream halfStream = new MemoryStream();
                halfBitmap.Save(halfStream, System.Drawing.Imaging.ImageFormat.Png);
                AddAttribute("SecurityImage", Convert.ToBase64String(halfStream.GetBuffer()));
                 * */
            }


        }
    }
}
