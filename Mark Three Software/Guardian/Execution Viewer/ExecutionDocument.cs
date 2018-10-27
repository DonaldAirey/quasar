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

	class ExecutionDocument : XmlDocument
	{

		/// <summary>
		/// Creates a well formed working order document object model from fragments of data.
		/// </summary>
		/// <param name="blotterId">A list of items to be included in the DOM.</param>
		public ExecutionDocument(FragmentList fragmentList)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StateLock.AcquireReaderLock(CommonTimeout.LockWait);
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

					// The generic record in the fragment is cast here to a ExecutionRow.  By design, this is the only type of 
					// record that will be placed into the FragmentList.
					ClientMarketData.ExecutionRow executionRow = (ClientMarketData.ExecutionRow)fragment.Row;

					// Insert, Update or Delete the fragment.
					switch (fragment.DataAction)
					{

					case DataAction.Insert:

						// The 'insert' element is optional until there is something to insert.
						if (insertNode == null)
							insertNode = fragmentNode.AppendChild(this.CreateElement("Insert"));

						// Insert the record.
						insertNode.AppendChild(new ExecutionElement(this, executionRow, FieldArray.Set));

						break;
				
				
					case DataAction.Update:

						// The 'update' element is optional until there is something to update.
						if (updateNode == null)
							updateNode = fragmentNode.AppendChild(this.CreateElement("Update"));
					
						// Update individual properties of the record.
						updateNode.AppendChild(new ExecutionElement(this, executionRow, fragment.Fields));

						break;

					case DataAction.Delete:

						// The 'delete' element is optional until there is something to delete.
						if (deleteNode == null)
							deleteNode = fragmentNode.AppendChild(this.CreateElement("Delete"));

						// The original record can't be used (because it has been deleted, duh).  A key is constructed from the data
						// stored in the fragment list.
						CommonElement commonElement = new CommonElement("Execution", this);
						commonElement.AddAttribute("ExecutionId", (int)fragment.Key[0]);
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
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld) ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld) ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld) ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld) ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StateLock.IsReaderLockHeld) ClientMarketData.StateLock.ReleaseReaderLock();
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
		public ExecutionDocument(Blotter blotter, WorkingOrder[] workingOrders)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerAccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DestinationOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ExecutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StateLock.AcquireReaderLock(CommonTimeout.LockWait);
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
								foreach (ClientMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
									documentNode.AppendChild(new ExecutionElement(this, executionRow, FieldArray.Set));
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
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld)
					ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerAccountLock.IsReaderLockHeld)
					ClientMarketData.BrokerAccountLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationLock.IsReaderLockHeld)
					ClientMarketData.DestinationLock.ReleaseReaderLock();
				if (ClientMarketData.DestinationOrderLock.IsReaderLockHeld)
					ClientMarketData.DestinationOrderLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.PriceTypeLock.IsReaderLockHeld)
					ClientMarketData.PriceTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.ExecutionLock.IsReaderLockHeld)
					ClientMarketData.ExecutionLock.ReleaseReaderLock();
				if (ClientMarketData.StateLock.IsReaderLockHeld)
					ClientMarketData.StateLock.ReleaseReaderLock();
				if (ClientMarketData.SourceLock.IsReaderLockHeld)
					ClientMarketData.SourceLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld)
					ClientMarketData.StylesheetLock.ReleaseReaderLock();
				if (ClientMarketData.TimeInForceLock.IsReaderLockHeld)
					ClientMarketData.TimeInForceLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
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
						foreach (ClientMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
							parentNode.AppendChild(new ExecutionElement(this, executionRow, FieldArray.Set));

		}

	}

	/// <summary>
	/// The attributes and data of a working order.
	/// </summary>
	class ExecutionElement : CommonElement
	{

		public ExecutionElement(ExecutionDocument executionDocument, ClientMarketData.ExecutionRow executionRow, FieldArray fields) :
			base("Execution", executionDocument)
		{

			// ExecutionId - Primary Key for this report.
			AddAttribute("ExecutionId", executionRow.ExecutionId);

			// State - Note that the status code is always provided to the DOM for color coding of the fields.
			AddAttribute("SourceStateCode", executionRow.SourceStateCode);
			AddAttribute("DestinationStateCode", executionRow.DestinationStateCode);
			if (fields[Field.SourceState])
				AddAttribute("SourceStateMnemonic", executionRow.StateRowByStateExecutionSourceStateCode.Mnemonic);
			if (fields[Field.DestinationState])
				AddAttribute("DestinationStateMnemonic", executionRow.StateRowByStateExecutionDestinationStateCode.Mnemonic);

			// Blotter
			if (fields[Field.Blotter])
			{
				AddAttribute("Blotter", executionRow.DestinationOrderRow.WorkingOrderRow.BlotterId);
				AddAttribute("BlotterName", executionRow.DestinationOrderRow.WorkingOrderRow.BlotterRow.ObjectRow.Name);
			}

			// OrderType
			if (fields[Field.OrderType])
			{
				AddAttribute("OrderTypeCode", executionRow.DestinationOrderRow.WorkingOrderRow.OrderTypeCode);
				AddAttribute("OrderTypeMnemonic", executionRow.DestinationOrderRow.WorkingOrderRow.OrderTypeRow.Mnemonic);
				AddAttribute("CashSign", executionRow.DestinationOrderRow.WorkingOrderRow.OrderTypeRow.CashSign);
				AddAttribute("QuantitySign", executionRow.DestinationOrderRow.WorkingOrderRow.OrderTypeRow.QuantitySign);
			}

			// TimeInForce
			if (fields[Field.TimeInForce])
			{
				AddAttribute("TimeInForceCode", executionRow.DestinationOrderRow.WorkingOrderRow.TimeInForceRow.TimeInForceCode);
				AddAttribute("TimeInForceName", executionRow.DestinationOrderRow.WorkingOrderRow.TimeInForceRow.Mnemonic);
			}

			// Security
			if (fields[Field.Security])
			{
				AddAttribute("SecurityId", executionRow.DestinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.SecurityId);
				AddAttribute("SecuritySymbol", executionRow.DestinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.Symbol);
				AddAttribute("SecurityName", executionRow.DestinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.ObjectRow.Name);
			}

			//  Source Order Total Quantity
			if (fields[Field.ExecutionQuantity])
				AddAttribute("ExecutionQuantity", executionRow.ExecutionQuantity);

			// Execution Price
			if (fields[Field.ExecutionPrice])
				AddAttribute("ExecutionPrice", executionRow.ExecutionPrice);

			// Market Value
			if (fields[Field.ExecutionPrice] || fields[Field.ExecutionQuantity])
				AddAttribute("GrossValue", executionRow.ExecutionQuantity * executionRow.ExecutionPrice);

			// Commission
			if (fields[Field.Commission])
				AddAttribute("Commission", executionRow.Commission);

			// Execution Net
			if (fields[Field.ExecutionPrice] || fields[Field.ExecutionQuantity] || fields[Field.Commission])
				AddAttribute("NetValue", (executionRow.ExecutionQuantity * executionRow.DestinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.QuantityFactor) *
					(executionRow.ExecutionPrice * executionRow.DestinationOrderRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId.PriceFactor) +
					(executionRow.Commission * executionRow.DestinationOrderRow.WorkingOrderRow.OrderTypeRow.QuantitySign));

			// Destination
			if (fields[Field.Destination])
				if (!executionRow.DestinationOrderRow.WorkingOrderRow.IsDestinationIdNull())
				{
					AddAttribute("DestinationId", executionRow.DestinationOrderRow.WorkingOrderRow.DestinationRow.DestinationId);
					AddAttribute("DestinationName", executionRow.DestinationOrderRow.WorkingOrderRow.DestinationRow.Name);
					AddAttribute("DestinationShortName", executionRow.DestinationOrderRow.WorkingOrderRow.DestinationRow.ShortName);
				}

			// The Direction of this order (buy, sell, buy cover, etc.)
			if (fields[Field.PriceType])
			{
				AddAttribute("PriceTypeCode", executionRow.DestinationOrderRow.WorkingOrderRow.PriceTypeRow.PriceTypeCode);
				AddAttribute("PriceTypeMnemonic", executionRow.DestinationOrderRow.WorkingOrderRow.PriceTypeRow.Mnemonic);
			}

			// The clearing broker.
			if (fields[Field.Broker])
			{
				if (!executionRow.IsBrokerIdNull())
				{
					AddAttribute("BrokerId", executionRow.BrokerRow.BrokerId);
					AddAttribute("BrokerName", executionRow.BrokerRow.SourceRow.BlotterRow.ObjectRow.Name);
					AddAttribute("BrokerSymbol", executionRow.BrokerRow.Symbol);
				}
			}

			// The account for clearing this execution.
			if (fields[Field.BrokerAccount])
			{
				if (!executionRow.IsBrokerAccountIdNull())
				{
					AddAttribute("BrokerAccountId", executionRow.BrokerAccountRow.BrokerAccountId);
					AddAttribute("BrokerAccountMnemonic", executionRow.BrokerAccountRow.Mnemonic);
					AddAttribute("BrokerAccountDescription", executionRow.BrokerAccountRow.Description);
				}
			}

			// The remaining Source Order Fields
			if (fields[Field.Execution])
			{

				// Created
				AddAttribute("CreatedName", executionRow.DestinationOrderRow.WorkingOrderRow.UserRowByUserWorkingOrderCreatedUserId.ObjectRow.Name);
				AddAttribute("CreatedTime", executionRow.DestinationOrderRow.WorkingOrderRow.CreatedTime.ToString("s"));

				// Limit Price
				if (!executionRow.DestinationOrderRow.WorkingOrderRow.IsLimitPriceNull())
					AddAttribute("LimitPrice", (decimal)executionRow.DestinationOrderRow.WorkingOrderRow.LimitPrice);

				// Stop Price
				if (!executionRow.DestinationOrderRow.WorkingOrderRow.IsStopPriceNull())
					AddAttribute("StopPrice", (decimal)executionRow.DestinationOrderRow.WorkingOrderRow.StopPrice);

			}

		}

	}

}
