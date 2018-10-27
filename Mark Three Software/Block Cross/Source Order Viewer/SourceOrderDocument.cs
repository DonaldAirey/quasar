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

	class SourceOrderDocument : XmlDocument
	{

		/// <summary>
		/// Creates a well formed working order document object model from fragments of data.
		/// </summary>
		/// <param name="blotterId">A list of items to be included in the DOM.</param>
		public SourceOrderDocument(FragmentList fragmentList)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
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

					// The generic record in the fragment is cast here to a SourceOrderRow.  By design, this is the only type of 
					// record that will be placed into the FragmentList.
					ClientMarketData.SourceOrderRow sourceOrderRow = (ClientMarketData.SourceOrderRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{

					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new SourceOrderElement(this, sourceOrderRow, FieldArray.Set));

						break;
				
				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new SourceOrderElement(this, sourceOrderRow, fragment.Fields));

						break;

					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("SourceOrder", this);
						commonElement.AddAttribute("SourceOrderId", (int)fragment.Key[0]);
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
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld) ClientMarketData.SourceOrderLock.ReleaseReaderLock();
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
		public SourceOrderDocument(Blotter blotter, WorkingOrder[] workingOrders)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
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
							foreach (ClientMarketData.SourceOrderRow sourceOrderRow in workingOrderRow.GetSourceOrderRows())
								documentNode.AppendChild(new SourceOrderElement(this, sourceOrderRow, FieldArray.Set));
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
				if (ClientMarketData.DestinationLock.IsReaderLockHeld) ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.SourceOrderLock.IsReaderLockHeld) ClientMarketData.SourceOrderLock.ReleaseReaderLock();
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
					foreach (ClientMarketData.SourceOrderRow sourceOrderRow in workingOrderRow.GetSourceOrderRows())
						parentNode.AppendChild(new SourceOrderElement(this, sourceOrderRow, FieldArray.Set));

		}

	}

	/// <summary>
	/// The attributes and data of a working order.
	/// </summary>
	class SourceOrderElement : CommonElement
	{

		public SourceOrderElement(SourceOrderDocument sourceOrderDocument, ClientMarketData.SourceOrderRow sourceOrderRow, FieldArray fields) :
			base("SourceOrder", sourceOrderDocument)
		{

			// SourceOrderId - Primary Key for this report.
			AddAttribute("SourceOrderId", sourceOrderRow.SourceOrderId);

			// Status - Note that the status code is always provided to the DOM for color coding of the fields.
			AddAttribute("StatusCode", sourceOrderRow.StatusCode);
			if (fields[Field.Status])
				AddAttribute("StatusName", sourceOrderRow.StatusRow.Mnemonic);

			// Blotter
			if (fields[Field.Blotter])
			{
				AddAttribute("Blotter", sourceOrderRow.WorkingOrderRow.BlotterId);
				AddAttribute("BlotterName", sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow.Name);
			}

			// OrderType
			if (fields[Field.OrderType])
			{
				AddAttribute("OrderTypeCode", sourceOrderRow.WorkingOrderRow.OrderTypeCode);
				AddAttribute("OrderTypeMnemonic", sourceOrderRow.WorkingOrderRow.OrderTypeRow.Mnemonic);
				AddAttribute("CashSign", sourceOrderRow.WorkingOrderRow.OrderTypeRow.CashSign);
				AddAttribute("QuantitySign", sourceOrderRow.WorkingOrderRow.OrderTypeRow.QuantitySign);
			}

			// TimeInForce
			if (fields[Field.TimeInForce])
			{
				AddAttribute("TimeInForceCode", sourceOrderRow.WorkingOrderRow.TimeInForceRow.TimeInForceCode);
				AddAttribute("TimeInForceName", sourceOrderRow.WorkingOrderRow.TimeInForceRow.Mnemonic);
			}

			// Security
			if (fields[Field.Security])
			{
				AddAttribute("SecurityId", sourceOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.SecurityId);
				AddAttribute("SecuritySymbol", sourceOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Symbol);
				AddAttribute("SecurityName", sourceOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.ObjectRow.Name);
			}

			//  Source Order Total Quantity
			if (fields[Field.OrderedQuantity])
				AddAttribute("SourceOrderQuantity", sourceOrderRow.OrderedQuantity);

			// Source
			if (fields[Field.Destination])
				if (!sourceOrderRow.WorkingOrderRow.IsDestinationIdNull())
				{
					AddAttribute("DestinationId", sourceOrderRow.WorkingOrderRow.DestinationRow.DestinationId);
					AddAttribute("DestinationName", sourceOrderRow.WorkingOrderRow.DestinationRow.Name);
					AddAttribute("DestinationShortName", sourceOrderRow.WorkingOrderRow.DestinationRow.ShortName);
				}

			// The Direction of this order (buy, sell, buy cover, etc.)
			if (fields[Field.PriceType])
			{
				AddAttribute("PriceTypeCode", sourceOrderRow.WorkingOrderRow.PriceTypeRow.PriceTypeCode);
				AddAttribute("PriceTypeMnemonic", sourceOrderRow.WorkingOrderRow.PriceTypeRow.Mnemonic);
			}

			// The remaining Source Order Fields
			if (fields[Field.SourceOrder])
			{

				// Created
				AddAttribute("CreatedName", sourceOrderRow.WorkingOrderRow.UserRowByUserWorkingOrderCreatedUserId.ObjectRow.Name);
				AddAttribute("CreatedTime", sourceOrderRow.WorkingOrderRow.CreatedTime.ToString("s"));

				// Limit Price
				if (!sourceOrderRow.WorkingOrderRow.IsLimitPriceNull())
					AddAttribute("LimitPrice", (decimal)sourceOrderRow.WorkingOrderRow.LimitPrice);

				// Stop Price
				if (!sourceOrderRow.WorkingOrderRow.IsStopPriceNull())
					AddAttribute("StopPrice", (decimal)sourceOrderRow.WorkingOrderRow.StopPrice);

			}

		}

	}

}
