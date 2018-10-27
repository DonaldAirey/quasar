namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	public partial class NegotiationConsole : MarkThree.Forms.Viewer
	{

		enum NegotiationState { None, Counting, Pending, Accepted, Rejected, Done};

		private decimal quantity;
		private decimal leavesQuantity;
		private int matchId;
		private int negotiationId;
		private NegotiationState negotiationState;
		private delegate void SetDialogAttributesDelegate(string title, string symbol, string name, Bitmap logo, decimal leavesQuantity, NegotiationState negotiationState);
		private delegate void MessageDelegate(string message, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon);
		private delegate void NegotiationStateDelegate(NegotiationState negotiationState);
		private delegate void TimeSpanDelegate(TimeSpan timeLeft);
		private delegate void VoidDelegate();
		private Dictionary<int, int> countdownTable;
		
		public NegotiationConsole()
		{
			InitializeComponent();

			this.matchId = int.MinValue;

			this.countdownTable = new Dictionary<int, int>();

#if DEBUG
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeEvents));

#if DEBUG
			}
#endif

		}

		public void OpenMatch(int matchId)
		{

			// Initialize the object
			this.matchId = matchId;
			this.negotiationId = int.MinValue;

			this.leavesQuantity = 0.0m;
			this.quantity = 25000.0m;

			SetNegotiationState(NegotiationState.None);
			this.timer.Enabled = true;

			ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeData));

		}

		/// <summary>
		/// Display an error message.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="errorEventArgs">The event arguments.</param>
		private void ShowMessage(string message, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
		{

			// Display the message.
			MessageBox.Show(this.TopLevelControl, message, Application.SafeTopLevelCaptionFormat, messageBoxButtons,
				messageBoxIcon);

		}

		private void SetNegotiationState(NegotiationState negotiationState)
		{

			this.negotiationState = negotiationState;

		}

		/// <summary>
		/// Initialize the data used in this application.
		/// </summary>
		private void InitializeEvents(object parameter)
		{

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.NegotiationLock.AcquireReaderLock(ClientTimeout.LockWait);

				ClientMarketData.Match.MatchRowChanging += new DataSetMarket.MatchRowChangeEventHandler(Match_MatchRowChanging);

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.NegotiationLock.IsReaderLockHeld)
					ClientMarketData.NegotiationLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Initialize the data used in this application.
		/// </summary>
		private void InitializeData(object parameter)
		{

			string title = string.Empty;
			string symbol = string.Empty;
            string name = string.Empty;
			Bitmap logo = null;
			decimal leavesQuantity = 0.0m;
			decimal minimumQuantity = 0.0m;
			NegotiationState negotiationState = NegotiationState.None;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.NegotiationLock.AcquireReaderLock(ClientTimeout.LockWait);
                ClientMarketData.ObjectLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// Find the Match record.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(this.matchId);
				ClientMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;
				ClientMarketData.OrderTypeRow orderTypeRow = workingOrderRow.OrderTypeRow;
				ClientMarketData.SecurityRow securityRow = workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId;

				symbol = securityRow.Symbol;
                name = securityRow.ObjectRow.Name;
				minimumQuantity = securityRow.MinimumQuantity;
				if (!securityRow.IsLogoNull())
				{
					MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(securityRow.Logo));
					logo = new Bitmap(memoryStream);
				}
				title = string.Format("{0} of {1}", orderTypeRow.Description, symbol);
				leavesQuantity = workingOrderRow.SubmittedQuantity;
				foreach (ClientMarketData.DestinationOrderRow destinationOrderRow in workingOrderRow.GetDestinationOrderRows())
					foreach (ClientMarketData.ExecutionRow executionRow in destinationOrderRow.GetExecutionRows())
						leavesQuantity -= executionRow.ExecutionQuantity;

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.NegotiationLock.IsReaderLockHeld)
					ClientMarketData.NegotiationLock.ReleaseReaderLock();
                if (ClientMarketData.ObjectLock.IsReaderLockHeld)
                    ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			Invoke(new SetDialogAttributesDelegate(SetDialogAttributes), new object[] { title, symbol, name, logo, leavesQuantity, negotiationState });

		}

		void Match_MatchRowChanging(object sender, DataSetMarket.MatchRowChangeEvent e)
		{

			if (e.Action == DataRowAction.Commit && e.Row.MatchId == this.matchId && this.IsHandleCreated)
                if (e.Row.StatusCode != Status.Active)
                    BeginInvoke(new VoidDelegate(DisableKeypad));
                
		}

		private void SetDialogAttributes(string title, string symbol, string name, Bitmap logo, decimal leavesQuantity, NegotiationState negotiationState)
		{

			// Submitted Quantity is saved here
			this.leavesQuantity = leavesQuantity;
			this.negotiationState = negotiationState;

			// Initialize the dialog elements with the data retrieved from the data model.
			this.Text = title;
			this.pictureBoxLogo.BackgroundImage = logo;
			this.decimalDomainQuantity.StartDecimal = 25000.0m;
			this.decimalDomainQuantity.StopDecimal = leavesQuantity;
			this.decimalDomainQuantity.Interval = 25000.0m;
			this.decimalDomainQuantity.Value = 25000.0m;
			this.labelLeavesQuantity.Text = leavesQuantity.ToString("#,##0");
			this.labelMinimumQuantity.Text = string.Format("{0:#,##0}", 25000.0m);
            this.labelTicker.Text = symbol;
            this.labelName.Text = name;

			// The keypad can now be enabled.
			EnableKeypad();

		}

		private void buttonTrade_Click(object sender, EventArgs e)
		{

			if (this.decimalDomainQuantity.Value != null)
			{

				this.quantity = (decimal)this.decimalDomainQuantity.Value;

				if (quantity < 25000.0m)
				{
					MessageBox.Show("Value must meet the minimum quantity.");
					return;
				}

				if (quantity > this.leavesQuantity)
				{
					MessageBox.Show("Value must be less than the quantity leaves.");
					return;
				}

				ThreadPool.QueueUserWorkItem(new WaitCallback(NegotiateTrade), this.quantity);

				DisableKeypad();

				this.negotiationId = int.MinValue;
				SetNegotiationState(NegotiationState.None);

			}

		}

		private void ClearKeypadEvent()
		{

			this.labelLeavesQuantity.Text = string.Format("{0:#,##0}", 0.0m);
			this.labelMinimumQuantity.Text = string.Format("{0:#,##0}", 0.0m);
			this.decimalDomainQuantity.StartDecimal = 0.0m;
			this.decimalDomainQuantity.Value = 0.0m;
			this.decimalDomainQuantity.StopDecimal = 0.0m;
			this.pictureBoxLogo.BackgroundImage = null;
            this.labelName.Text = string.Empty;
            this.labelTicker.Text = string.Empty;

		}

		public void DisableKeypad()
		{
            // Clear the keypad just in case
            this.ClearKeypadEvent();

			// Disable all the keypad buttons.
			this.labelLeaves.Enabled = false;
			this.labelLeavesQuantity.Enabled = false;
			this.labelQuantity.Enabled = false;
			this.decimalDomainQuantity.Enabled = false;
			this.labelMinimum.Enabled = false;
			this.labelMinimumQuantity.Enabled = false;
			this.buttonTrade.Enabled = false;
			this.buttonPass.Enabled = false;
		}

		public void EnableKeypad()
		{

			// Enable all the keypad buttons.
			this.labelLeaves.Enabled = true;
			this.labelLeavesQuantity.Enabled = true;
			this.labelQuantity.Enabled = true;
			this.decimalDomainQuantity.Enabled = true;
			this.labelMinimum.Enabled = true;
			this.labelMinimumQuantity.Enabled = true;
			this.buttonTrade.Enabled = true;
			this.buttonPass.Enabled = true;

		}

		private void NegotiateTrade(object parameter)
		{

			// Extract the thread parameters
			decimal quantity = (decimal)parameter;

			bool isBatchValid = true;
			Batch batch = new Batch();
			MethodPlan method = null;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// Find the Match record.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(this.matchId);

				// Construct a command to rename the object.
				AssemblyPlan assembly = batch.Assemblies.Add("Core Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Core.Negotiation");
				TransactionPlan transaction = batch.Transactions.Add();
				method = transaction.Methods.Add(type, "Insert");
				method.Parameters.Add(new InputParameter("matchId", matchId));
				method.Parameters.Add(new InputParameter("quantity", quantity));
				method.Parameters.Add(new InputParameter("statusCode", Status.Pending));

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				// This indicates that the batch shouldn't be executed.
				isBatchValid = false;

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// If the command batch was built successfully, then execute it.  If any part of it should fail, cancel the edit and
			// display the server errors.
			if (isBatchValid)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there 
					// are no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(batch);

					this.negotiationId = (int)method.Parameters.Return.Value;

				}
				catch (BatchException batchException)
				{

					// Display each error in the batch.
					foreach (Exception exception in batchException.Exceptions)
						Invoke(new MessageDelegate(ShowMessage), exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

		}

		private void buttonPass_Click(object sender, EventArgs e)
		{

			ThreadPool.QueueUserWorkItem(new WaitCallback(DeclineTrade));
			DisableKeypad();

		}

		private void DeclineTrade(object parameter)
		{

			bool isBatchValid = true;
			Batch batch = new Batch();
			MethodPlan method = null;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// Find the Match record.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(this.matchId);

				AssemblyPlan assembly = batch.Assemblies.Add("Core Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Core.Negotiation");
				TransactionPlan transaction = batch.Transactions.Add();
				method = transaction.Methods.Add(type, "Insert");
				method.Parameters.Add(new InputParameter("matchId", matchId));
				method.Parameters.Add(new InputParameter("quantity", 0.0m));
				method.Parameters.Add(new InputParameter("statusCode", Status.Declined));

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				// This indicates that the batch shouldn't be executed.
				isBatchValid = false;

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.OrderTypeLock.IsReaderLockHeld)
					ClientMarketData.OrderTypeLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// If the command batch was built successfully, then execute it.  If any part of it should fail, cancel the edit and
			// display the server errors.
			if (isBatchValid)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there 
					// are no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(batch);

					this.negotiationId = (int)method.Parameters.Return.Value;

				}
				catch (BatchException batchException)
				{

					// Display each error in the batch.
					foreach (Exception exception in batchException.Exceptions)
						Invoke(new MessageDelegate(ShowMessage), exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

		}

		private void timer_Tick(object sender, EventArgs e)
		{

			foreach (KeyValuePair<int, int> keyPair in this.countdownTable)
			{
				int counter = keyPair.Value - 1;
				if (counter == 0)
				{

					this.countdownTable.Remove(keyPair.Key);

					if (keyPair.Key == this.matchId)
						this.negotiationState = NegotiationState.Rejected;

                    // disable the keypad when the timer is up
                    DisableKeypad();
				}
				else
					this.countdownTable[keyPair.Key] = counter;

			}

		}

		private void labelMinimum_Click(object sender, EventArgs e)
		{
			this.decimalDomainQuantity.Value = 25000.0m;
		}

		private void labelLeaves_Click(object sender, EventArgs e)
		{
			this.decimalDomainQuantity.Value = this.leavesQuantity;
		}

	}

}

