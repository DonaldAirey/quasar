
namespace MarkThree.Guardian.Forms
{
    using MarkThree.Forms;
    using MarkThree.Guardian;
    using MarkThree.Guardian.Forms;
    using MarkThree.Guardian.Client;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Media;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public partial class QuoteViewer : MarkThree.Forms.SpreadsheetViewer
    {
        // Private Members
        private int matchId;
        private int stylesheetId;
        
        private MarkThree.Forms.FragmentList fragmentList;
        
        //private static string STYLESHEET_ID = "Quote Viewer";
        private static int STYLESHEET_TYPECODE = 9;

        public QuoteViewer()
        {
            // IDs of the security for data and stylesheet for style
            this.Tag = Int32.MinValue;
            this.stylesheetId = Int32.MinValue;

            InitializeComponent();

            // This collects the changes to the data model and is used to generate incremental changes to the displayed document.
            this.fragmentList = new FragmentList();
        }

        /// <summary>
        /// Opens the viewer.
        /// </summary>
        /// <param name="tag">Integer securityId to be displayed in the quote viewer</param>
        public override void Open(object tag)
        {
            if (!(tag is Int32))
                throw new Exception(String.Format("{0} can't display objects of type {1}", this.GetType(), this.Tag.GetType()));

            this.Tag = tag;
           
            base.Open(tag);
        }  

        /// <summary>
        /// Opens the quote viewer to display the securityId selected in the match viewer
        /// </summary>
        /// <param name="matchId"></param>
        public void OpenQuote(int matchId)
        {

            this.Tag = Int32.MinValue;

            // Initialize the object
            this.matchId = matchId;

            ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeData));
            
        }

        /// <summary>
        /// Method called to retrieve the securityId for the security selected in the match viewere.
        /// </summary>
        /// <param name="parameter"></param>
        private void InitializeData(object parameter)
        {
            int id = Int32.MinValue;

            try
            {

                // Lock the tables.
                System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
                ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
                ClientMarketData.NegotiationLock.AcquireReaderLock(ClientTimeout.LockWait);
                ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
                ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
                ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

                // Find the Match record.
                ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(this.matchId);
                ClientMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;
                ClientMarketData.OrderTypeRow orderTypeRow = workingOrderRow.OrderTypeRow;
                ClientMarketData.SecurityRow securityRow = workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId;

                if (securityRow != null)
                    id = securityRow.SecurityId;
            }
            catch (Exception e)
            {
                // Write the error and stack trace out to the debug listener
                EventLog.Error("{0}, {1}", e.Message, e.StackTrace);
            }
            finally
            {

                // Release the locks.
                if (ClientMarketData.MatchLock.IsReaderLockHeld)
                    ClientMarketData.MatchLock.ReleaseReaderLock();
                if (ClientMarketData.NegotiationLock.IsReaderLockHeld)
                    ClientMarketData.NegotiationLock.ReleaseReaderLock();
                if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
                    ClientMarketData.OrderTypeLock.ReleaseReaderLock();
                if (ClientMarketData.SecurityLock.IsReaderLockHeld)
                    ClientMarketData.SecurityLock.ReleaseReaderLock();
                if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
                    ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
                System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

            }

            if (id != Int32.MinValue)
                this.Open(id);
        }

        /// <summary>
	    /// Opens the Quote Viewer
	    /// </summary>
	    protected override void OpenCommand()
	    {

			// This block will attach the Quote viewer to the data mdoel
			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.NegotiationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
                ClientMarketData.StylesheetTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
              
				// The stylesheets take a modest amount of time to parse from a string into an XSL structure.  As an optimization, 
				// this action is skipped if the new document in this viewer is using the same stylesheet as the last document 
				// loaded.
                
                // get the StylesheetTypeRow based on the hard-coded stylesheet type
                ClientMarketData.StylesheetTypeRow stylesheetTypeRow = 
                    ClientMarketData.StylesheetType.FindByStylesheetTypeCode(STYLESHEET_TYPECODE);

                // get the first stylesheet type row
                if (stylesheetTypeRow == null)
                    throw new Exception("Unable to retrieve the Quote Viewer stylesheet type.");

                // get the stylesheet row for this type
                ClientMarketData.StylesheetRow[] stylesheetRows = stylesheetTypeRow.GetStylesheetRows();
                if (stylesheetRows == null || stylesheetRows.Length < 1)
                    throw new Exception("Unable to retrieve the QuoteViewer stylesheet.");

                ClientMarketData.StylesheetRow stylesheetRow = stylesheetRows[0];
                if (this.stylesheetId != stylesheetRow.StylesheetId)
				{

					// This is the identity of the stylesheet used by this viewer.
					this.stylesheetId = stylesheetRow.StylesheetId;

					// This will load the TEXT object stored in the Stylesheet table into an internal structure that is used to
					// transform the document object model (DOM) into a viewable report.
					XmlDocument xslStylesheet = new XmlDocument();
					StringReader stringReader = new StringReader(stylesheetRow.Text);
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					xslStylesheet.Load(xmlTextReader);
					this.XslStylesheet = xslStylesheet;

				}

				// Draw the document.
				DrawDocument();

				
				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.Blotter.BlotterRowChanging += new ClientMarketData.BlotterRowChangeEventHandler(this.ChangeBlotterRow);
				ClientMarketData.Match.MatchRowChanging += new ClientMarketData.MatchRowChangeEventHandler(this.ChangeMatchRow);
				ClientMarketData.Negotiation.NegotiationRowChanging += new ClientMarketData.NegotiationRowChangeEventHandler(this.ChangeNegotiationRow);
				ClientMarketData.Object.ObjectRowChanging += new ClientMarketData.ObjectRowChangeEventHandler(this.ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging += new ClientMarketData.OrderTypeRowChangeEventHandler(this.ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging += new ClientMarketData.PriceRowChangeEventHandler(this.ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging += new ClientMarketData.PriceTypeRowChangeEventHandler(this.ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging += new ClientMarketData.SecurityRowChangeEventHandler(this.ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging += new ClientMarketData.StatusRowChangeEventHandler(this.ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging += new ClientMarketData.StylesheetRowChangeEventHandler(this.ChangeStylesheetRow);
				ClientMarketData.EndMerge += new EventHandler(this.EndMerge);
                
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
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.NegotiationLock.IsReaderLockHeld)
					ClientMarketData.NegotiationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
                if (ClientMarketData.StylesheetTypeLock.IsReaderLockHeld)
                    ClientMarketData.StylesheetTypeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Closes the Quote Viewer.
		/// </summary>
		protected override void CloseCommand()
		{

			// Make sure any exceptions are localized to this method.
			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.NegotiationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StatusLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);
                
				// Disengage the event handlers from the data model.
				ClientMarketData.Blotter.BlotterRowChanging -= new ClientMarketData.BlotterRowChangeEventHandler(this.ChangeBlotterRow);
				ClientMarketData.Match.MatchRowChanging -= new ClientMarketData.MatchRowChangeEventHandler(this.ChangeMatchRow);
				ClientMarketData.Negotiation.NegotiationRowChanging -= new ClientMarketData.NegotiationRowChangeEventHandler(this.ChangeNegotiationRow);
				ClientMarketData.Object.ObjectRowChanging -= new ClientMarketData.ObjectRowChangeEventHandler(this.ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanging -= new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ChangeObjectTreeRow);
				ClientMarketData.OrderType.OrderTypeRowChanging -= new ClientMarketData.OrderTypeRowChangeEventHandler(this.ChangeOrderTypeRow);
				ClientMarketData.Price.PriceRowChanging -= new ClientMarketData.PriceRowChangeEventHandler(this.ChangePriceRow);
				ClientMarketData.PriceType.PriceTypeRowChanging -= new ClientMarketData.PriceTypeRowChangeEventHandler(this.ChangePriceTypeRow);
				ClientMarketData.Security.SecurityRowChanging -= new ClientMarketData.SecurityRowChangeEventHandler(this.ChangeSecurityRow);
				ClientMarketData.Status.StatusRowChanging -= new ClientMarketData.StatusRowChangeEventHandler(this.ChangeStatusRow);
				ClientMarketData.Stylesheet.StylesheetRowChanging -= new ClientMarketData.StylesheetRowChangeEventHandler(this.ChangeStylesheetRow);
				ClientMarketData.EndMerge -= new EventHandler(this.EndMerge);
                
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
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.NegotiationLock.IsReaderLockHeld)
					ClientMarketData.NegotiationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.StatusLock.IsReaderLockHeld)
					ClientMarketData.StatusLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

        /// <summary>
        /// Handles a change to the blotter table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
        private void ChangeBlotterRow(object sender, ClientMarketData.BlotterRowChangeEvent blotterRowChangeEvent)
        {

        }

        /// <summary>
        /// Event driver for a change to the blotter table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
        private void ChangeMatchRow(object sender, ClientMarketData.MatchRowChangeEvent matchRowChangeEvent)
        {

        }

        /// <summary>
        /// Event driver for a change to the blotter table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="blotterRowChangeEvent">The parameters for the event handler.</param>
        private void ChangeNegotiationRow(object sender, ClientMarketData.NegotiationRowChangeEvent negotiationRowChangeEvent)
        {

        }

        /// <summary>
        /// Event driver for a change to the Object table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="ChangeObject">The parameters for the event handler.</param>
        private void ChangeObjectRow(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
        {

        }

        /// <summary>
        /// Event driver for a change to the ObjectTree table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="ChangeObjectTree">The parameters for the event handler.</param>
        private void ChangeObjectTreeRow(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectChangeTreeRow)
        {

        }

        /// <summary>
        /// Event driver for a change to the transactionType table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="transactionChangeType">The parameters for the event handler.</param>
        private void ChangeOrderTypeRow(object sender, ClientMarketData.OrderTypeRowChangeEvent transactionChangeTypeRow)
        {
        }

        /// <summary>
        /// This handler is called when prices have changed.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="priceRowChangingEvent">Event argument.</param>
        private void ChangePriceRow(object sender, ClientMarketData.PriceRowChangeEvent priceRowChangeEvent)
        {
            if (priceRowChangeEvent.Action == DataRowAction.Commit)
            {
                // Extract the price row from the event argumetns.
                ClientMarketData.PriceRow priceRow = priceRowChangeEvent.Row;

                // The code below will compare the current values to the original ones and set an indicator for which values have
                // changed.  This set of indicators will determine which values are included in the incremental XML fragment that
                // is created to update the viewer.
                FieldArray fields = FieldArray.Set;
                //FieldArray.Set;

                // There will only be 'Current' rows after the first batch of data is read from the server into the data model.
                // This check is needed to prevent exceptions when comparing the current row to the original one.
                if (!priceRow.HasVersion(DataRowVersion.Original))
                    return;

                // don't perform update if the securityId is not the security being displayed
                if (!priceRow.SecurityId.Equals(this.Tag))
                    return;

                // Set an indicator if the Last Price has changed.
                if (!priceRow[ClientMarketData.Price.LastPriceColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.LastPriceColumn, DataRowVersion.Current]))
                    fields[Field.LastPrice] = true;

                // Set an indicator if the Ask Price has changed.
                if (!priceRow[ClientMarketData.Price.AskPriceColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.AskPriceColumn, DataRowVersion.Current]))
                    fields[Field.AskPrice] = true;

                // Set an indicator if the Bid Price has changed.
                if (!priceRow[ClientMarketData.Price.BidPriceColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.BidPriceColumn, DataRowVersion.Current]))
                    fields[Field.BidPrice] = true;

                // Set an indicator if the Last Size has changed.
                if (!priceRow[ClientMarketData.Price.LastSizeColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.LastSizeColumn, DataRowVersion.Current]))
                    fields[Field.LastSize] = true;

                // Set an indicator if the Ask Size has changed.
                if (!priceRow[ClientMarketData.Price.AskSizeColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.AskSizeColumn, DataRowVersion.Current]))
                    fields[Field.AskSize] = true;

                // Set an indicator if the Bid Size has changed.
                if (!priceRow[ClientMarketData.Price.BidSizeColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.BidSizeColumn, DataRowVersion.Current]))
                    fields[Field.BidSize] = true;

                // Set an indiator if the Volume has changed.
                if (!priceRow[ClientMarketData.Price.VolumeColumn, DataRowVersion.Original].Equals(
                    priceRow[ClientMarketData.Price.VolumeColumn, DataRowVersion.Current]))
                    fields[Field.Volume] = true;

                if (fields)
                    this.fragmentList.Add(DataAction.Update, priceRow, fields);

            }

        }

        /// <summary>
        /// Event driver for a change to the PriceType table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="ChangePriceType">The parameters for the event handler.</param>
        private void ChangePriceTypeRow(object sender, ClientMarketData.PriceTypeRowChangeEvent orderChangeTypeRow)
        {
        }

        /// <summary>
        /// Event driver for a change to the Security table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="ChangeSecurityRow">The parameters for the event handler.</param>
        private void ChangeSecurityRow(object sender, ClientMarketData.SecurityRowChangeEvent securityRowChangeEvent)
        {

        }

        /// <summary>
        /// Event driver for a change to the status table.
        /// </summary>
        /// <param name="sender">The object that generated the event.</param>
        /// <param name="statusRowChangeEvent">The parameters for the event handler.</param>
        private void ChangeStatusRow(object sender, ClientMarketData.StatusRowChangeEvent statusRowChangeEvent)
        {
        }

        /// <summary>
        /// Handles a changed stylesheet in the data model.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="stylesheetRowChangeEvent">The event argument.</param>
        private void ChangeStylesheetRow(object sender, ClientMarketData.StylesheetRowChangeEvent stylesheetRowChangeEvent)
        {

            // The only changes that impact the viewer are committed records.
            if (stylesheetRowChangeEvent.Action == DataRowAction.Commit)
            {

                // This will make it easier to operate on the changed record.
                ClientMarketData.StylesheetRow stylesheetRow = stylesheetRowChangeEvent.Row;

                // Reload the stylesheet if the modified stylesheet is the one currently used by this viewer.
                if (stylesheetRow.StylesheetId == this.stylesheetId)
                {

                    // This will read the stylesheet out of the changed record and into the viewers internal data structures.
                    XmlDocument xslStylesheet = new XmlDocument();
                    StringReader stringReader = new StringReader(stylesheetRow.Text);
                    XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
                    xslStylesheet.Load(xmlTextReader);
                    this.XslStylesheet = xslStylesheet;

                    // This indicates that the list of fragments is invalid and the entire document should be redrawn.
                    this.fragmentList.IsValid = false;

                }

            }

        }

        /// <summary>
        /// Handles the event that is generated when all the data model changes have been incorporated.
        /// </summary>
        /// <param name="sender">Object that generated the event.</param>
        /// <param name="e">This aregument isn't used.</param>
        protected void EndMerge(object sender, EventArgs e)
        {

            // When all the fragments have been collected they are sent to be drawn in another thread.  Note that a clean, empty
            // fragment list is created each time the data model is changed.  This prevents the data model event handlers from
            // corrupting the list of fragments before they can be drawn.
            if (this.fragmentList.IsValid)
            {
                if (this.fragmentList.Count != 0)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DrawFragmentCommand), this.fragmentList);
                    this.fragmentList = new FragmentList();
                }
            }
            else
            {
                DrawDocument();
                this.fragmentList.Clear();
            }

        }

        /// <summary>
        /// Creates the DOM and Refreshes the spreadsheet control in the background.
        /// </summary>
        /// <remarks>
        /// This thread is responsible for recreating the Document Object Model for the blotter and translating
        /// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
        /// DOM is complete to either update the entire document, if the structure has changed, or only update
        /// the changed cells, when only the data has changed.
        /// </remarks>
        protected override void DrawDocumentCommand(object parameter)
        {

            // Extract the document from the thread parameters.
            Blotter blotter = null;
            if (parameter is Blotter)
                blotter = (Blotter)parameter;

            if (parameter is BlotterMatchDetail)
                blotter = ((BlotterMatchDetail)parameter).Blotter;

#if DEBUGXML
			// Create the blotter document.
			MatchHistoryDocument matchDocument = new MatchHistoryDocument(blotter.BlotterId);

			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				matchDocument.Save("matchHistoryDOM.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will display the
			this.XmlDocument = matchDocument;
#else
            // This will display the quote viewer
            this.XmlDocument = new QuoteDocument(Convert.ToInt32(this.Tag));
#endif

        }

        /// <summary>
        /// Creates the DOM and Refreshes the spreadsheet control in the background.
        /// </summary>
        /// <remarks>
        /// This thread is responsible for recreating the Document Object Model for the blotter and translating
        /// that DOM into a format compatible with the spreadsheet viewer.  It will call the foreground when the
        /// DOM is complete to either update the entire document, if the structure has changed, or only update
        /// the changed cells, when only the data has changed.
        /// </remarks>
        private void DrawFragmentCommand(object parameter)
        {

            // Extract the list of fragments from the thread parameters.
            FragmentList fragmentList = (FragmentList)parameter;

#if DEBUGXML
			// This will create an XML Document containing only the records and fields that have changed since the viewer was
			// last updated.
			XmlDocument stagedOrderFragment = new MatchHistoryDocument(fragmentList);
					
			// Make sure that writing the debug file doesn't interrupt the session.
			try
			{

				// During debugging, it's very useful to have a copy of the DOM on the disk.
				stagedOrderFragment.Save("stagedOrderFragment.xml");

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the event log.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This will write the changes to the viewer by passing it through the stylesheet.
			this.XmlDocument = stagedOrderFragment;
#else
            // This will write the changes to the viewer by passing it through the stylesheet.
            //this.XmlDocument = new MatchHistoryDocument(fragmentList);
            this.XmlDocument = new QuoteDocument(fragmentList);
#endif

        }

    }

}

