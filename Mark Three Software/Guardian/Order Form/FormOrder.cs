namespace MarkThree.Guardian.Forms
{

	using MarkThree.Forms;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// An order form for an Equity.
	/// </summary>
	public partial class FormOrder : Form
	{

		// Private Members
		private Blotter blotter;
		private InitializeDelegate initializeDelegate;
		private PostEndDelegate postEndDelegate;
		private delegate void InitializeDelegate(SourceOrder sourceOrder);
		private delegate void PostEndDelegate(bool exitDialog, BatchException batchException);
		private System.Threading.ManualResetEvent handleCreatedEvent;

		public FormOrder(Blotter blotter)
		{

			// This will initialize the IDE maintained components.
			InitializeComponent();

			this.blotter = blotter;

			// This event is used to hold up the background initialization of the form until a window handle has been created.  If 
			// the background thread doesn't wait for this signal, it can cause an exception when trying to pass information to the
			// foreground using the 'Invoke' commands.
			this.handleCreatedEvent = new ManualResetEvent(false);

			// Delegates for handling Windows thread actions from the background.
			this.initializeDelegate = new InitializeDelegate(InitializeDialog);
			this.postEndDelegate = new PostEndDelegate(PostEnd);

#if DEBUG
			// This will prevent the background initialization thread from running in the designer (background threads kill the designer.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// The remaining part of the initialization must be done from a background thread.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializationThread));

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Raised when the window has completed the initialization.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{

			// The initialization thread can complete when the handle for this window has been created.
			this.handleCreatedEvent.Set();

			// Allow the base class to complete the initialization of the window.
			base.OnHandleCreated(e);

		}

		/// <summary>
		/// Reads the contents of the form from the data model.
		/// </summary>
		private void InitializationThread(object parameter)
		{

			// This structure will be filled in with items from the data model and passed to the foreground so the foreground
			// thread doesn't need to worry about the locks.
			SourceOrder sourceOrder = new SourceOrder();

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TraderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Fill in the structure with the Trader values.
				ClientMarketData.TraderRow traderRow = ClientMarketData.Trader.FindByTraderId(Preferences.UserId);
				if (traderRow != null)
				{
					sourceOrder.MaximumVolatility = traderRow[ClientMarketData.Trader.MaximumVolatilityDefaultColumn];
					sourceOrder.StartTime = traderRow[ClientMarketData.Trader.StartTimeDefaultColumn];
					sourceOrder.StopTime = traderRow[ClientMarketData.Trader.StopTimeDefaultColumn];
					sourceOrder.NewsFreeTime = traderRow[ClientMarketData.Trader.NewsFreeTimeDefaultColumn];
					sourceOrder.IsBrokerMatch = traderRow.IsBrokerMatch;
					sourceOrder.IsHedgeMatch = traderRow.IsHedgeMatch;
					sourceOrder.IsInstitutionMatch = traderRow.IsInstitutionMatch;
				}

			}
			finally
			{

				// Release the locks
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TraderLock.IsReaderLockHeld)
					ClientMarketData.TraderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Pass the data to the foreground to be displayed in the form.
			this.handleCreatedEvent.WaitOne();
			BeginInvoke(this.initializeDelegate, new object[] { sourceOrder });

		}

		/// <summary>
		/// Initialize the dialog with the data read from the data model.
		/// </summary>
		/// <param name="sourceOrder">A structure containing the data for the form.</param>
		private void InitializeDialog(SourceOrder sourceOrder)
		{

			// Initialize the dialog box members with the data read from the data model.  The data must be collected in a 
			// background thread, but it can only be added to the dialog in the thread that created the dialog.
			this.orderTypePicker.Value = DBNull.Value;
			this.symbolBox.SelectedSecurityId = string.Empty;
			this.quantityBox.Value = sourceOrder.Quantity;
			this.percentPickerMaximumVolatility.Value = sourceOrder.MaximumVolatility;
			this.timePickerStartTime.Value = sourceOrder.StartTime;
			this.timePickerStopTime.Value = sourceOrder.StopTime;
			this.integerPickerNewsFreeTime.Value = sourceOrder.NewsFreeTime;
			this.checkBoxBroker.Checked = sourceOrder.IsBrokerMatch;
			this.checkBoxHedgeFund.Checked = sourceOrder.IsHedgeMatch;
			this.checkBoxInstitution.Checked = sourceOrder.IsInstitutionMatch;

			this.orderTypePicker.Select();

		}

		private void buttonPost_Click(object sender, EventArgs e)
		{

			// This structure is passed to a background thread to be applied to the server data model.
			SourceOrder sourceOrder = new SourceOrder();

			// Validate the data in the dialog.  First, validate the order type.
			if (this.orderTypePicker.Value is DBNull)
			{
				MessageBox.Show("You must pick an order type");
				return;
			}

			// Validate the quantity.
			if (this.quantityBox.Value == 0.0M)
			{
				MessageBox.Show("This is an invalid quantity for a trade");
				return;
			}

			// Fill in the options structure with the values found in this tab of the control.
			sourceOrder.OrderTypeCode = (int)this.orderTypePicker.Value;
			sourceOrder.Quantity = this.quantityBox.Value;
			sourceOrder.SecurityId = this.symbolBox.SelectedSecurityId;
			sourceOrder.SettlementId = this.symbolBox.SelectedSettlementId;
			sourceOrder.IsBrokerMatch = this.checkBoxBroker.Checked;
			sourceOrder.IsHedgeMatch = this.checkBoxHedgeFund.Checked;
			sourceOrder.IsInstitutionMatch = this.checkBoxInstitution.Checked;
			sourceOrder.MaximumVolatility = this.percentPickerMaximumVolatility.Value;
			sourceOrder.NewsFreeTime = this.integerPickerNewsFreeTime.Value;
			sourceOrder.StartTime = this.timePickerStartTime.Value;
			sourceOrder.StopTime = this.timePickerStopTime.Value;

			// Updating the server data model must be done in the background so as to not disturbe the flow of the message loop.
			ThreadPool.QueueUserWorkItem(new WaitCallback(PostThread), new Object[] { false, sourceOrder });

		}

		/// <summary>
		/// Begins the task of applying the changes in the form to the data model.
		/// </summary>
		private void PostThread(object parameter)
		{

			// Extract the parameters from the threads parameter.
			object[] parameters = (object[])parameter;
			bool exitDialog = (bool)parameters[0];
			SourceOrder sourceOrder = (SourceOrder)parameters[1];

			// This batch will collect the information needed to execute a complex transaction on the server.  The first part of 
			// the batch sets up the transaction: the assembly where the types and methods are found.  It also sets up a
			// transaction for a complex operation. In this case, the transaction is not that complex, just a single method to be
			// executed.
			Batch batch = new Batch();
			AssemblyPlan assembly = batch.Assemblies.Add("Trading Service");
			TypePlan type = assembly.Types.Add("MarkThree.Guardian.Trading.SourceOrder");
			TransactionPlan transaction = batch.Transactions.Add();
			MethodPlan methodPlan = transaction.Methods.Add(type, "Insert");

			// These are the parameters used to create a Source Order.  The Working Order will be created implicitly.
			methodPlan.Parameters.Add(new InputParameter("blotterId", this.blotter.BlotterId));
			methodPlan.Parameters.Add(new InputParameter("isBrokerMatch", sourceOrder.IsBrokerMatch));
			methodPlan.Parameters.Add(new InputParameter("isHedgeMatch", sourceOrder.IsHedgeMatch));
			methodPlan.Parameters.Add(new InputParameter("isInstitutionMatch", sourceOrder.IsInstitutionMatch));
			methodPlan.Parameters.Add(new InputParameter("submissionTypeCode", SubmissionType.UsePeferences));
			if (sourceOrder.MaximumVolatility != DBNull.Value)
				methodPlan.Parameters.Add(new InputParameter("maximumVolatility", sourceOrder.MaximumVolatility));
			if (sourceOrder.NewsFreeTime != DBNull.Value)
				methodPlan.Parameters.Add(new InputParameter("newsFreeTime", sourceOrder.NewsFreeTime));
			methodPlan.Parameters.Add(new InputParameter("orderedQuantity", sourceOrder.Quantity));
			methodPlan.Parameters.Add(new InputParameter("orderTypeCode", sourceOrder.OrderTypeCode));
			methodPlan.Parameters.Add(new InputParameter("priceTypeCode", PriceType.Market));
			methodPlan.Parameters.Add(new InputParameter("securityId", sourceOrder.SecurityId));
			methodPlan.Parameters.Add(new InputParameter("settlementId", sourceOrder.SettlementId));
			if (sourceOrder.StartTime != DBNull.Value)
				methodPlan.Parameters.Add(new InputParameter("startTime", sourceOrder.StartTime));
			if (sourceOrder.StopTime != DBNull.Value)
				methodPlan.Parameters.Add(new InputParameter("stopTime", sourceOrder.StopTime));
			methodPlan.Parameters.Add(new InputParameter("timeInForceCode", TimeInForce.Day));

			// This will execute the command on the server and return any exceptions.
			BatchException batchException = null;

			try
			{

				// Execute the batch.
				ClientMarketData.Execute(batch);

			}
			catch (BatchException exception)
			{

				// Any exceptions will be captured here and passed on to the foreground.
				batchException = exception;

			}

			// Call the foreground thread with the results of executing the batch on the server.  Also, in some cases the dialog is
			// going to be dismissed when the server data model has finished updating successfully.  Pass on the flag to the
			// foreground that will indicate whether the form is closed once the results are processed.
			BeginInvoke(this.postEndDelegate, new object[] { exitDialog, batchException });

		}

		/// <summary>
		/// Completes the task of applying the user's changes to the form.
		/// </summary>
		/// <param name="exitDialog">If true, the form is dismiss.</param>
		/// <param name="batchException">Contains any exceptions from processing the server update.</param>
		private void PostEnd(bool exitDialog, BatchException batchException)
		{

			// Display any errors from the background processing.
			if (batchException != null)
				foreach (Exception exception in batchException.Exceptions)
					MessageBox.Show(this, exception.Message, "Guardian Error");

			// If there were no exceptions and the user asked to exit the dialog, it is dismissed.
			if (batchException == null && exitDialog)
				Close();

			// This will re-initialize the order entry dialog with the user's preferences.
			ThreadPool.QueueUserWorkItem(new WaitCallback(InitializationThread));

		}

	}

}