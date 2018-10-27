namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Client;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Threading;

	/// <summary>
	/// Notifies the user of matching opportunities.
	/// </summary>
	public class NegotiationService : Component
	{

		// Private Delegates
		private delegate void NotificationInitialzationDelegate(int matchId, string title, string symbol, Bitmap logo);

		/// <summary>
		/// Will raise an event when the user has elected to negotiate a match.
		/// </summary>
		public event OpenObjectEventHandler OpenObject;

		/// <summary>
		/// Provides a notification message when a match opportunity is present.
		/// </summary>
		/// <param name="iContainer">The containing object.</param>
		public NegotiationService(IContainer iContainer)
		{

			// Add this object to the components managed by the container.
			iContainer.Add(this);

#if DEBUG
			// This will prevent the background initialization thread from running in the designer (background threads kill the designer.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// This thread will initialize the data model used by this service in the background.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeData));

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Initializes the data required for this component.
		/// </summary>
		/// <param name="parameter">Thread initialization data (not used).</param>
		private void InitializeData(object parameter)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Hook this service into the data model.  It will watch for any new matches and create a pop-up window when an
				// opportunity arises.
				ClientMarketData.Match.MatchRowChanging += new DataSetMarket.MatchRowChangeEventHandler(Match_MatchRowChanging);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Event handler for a match.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		void Match_MatchRowChanging(object sender, DataSetMarket.MatchRowChangeEvent e)
		{

			// When a new, pending match record has been added to the data mode, start a thread that will
			// display the notification window.
			if (e.Action == DataRowAction.Commit)
				if (!e.Row.HasVersion(DataRowVersion.Original) && e.Row.StatusCode == Status.Active)
					ThreadPool.QueueUserWorkItem(new WaitCallback(NotifyUser), e.Row.MatchId);

		}
		
		/// <summary>
		/// Notifies the user that a match opportunity exists.
		/// </summary>
		/// <param name="parameter">The thread initialization parameters.</param>
		private void NotifyUser(object parameter)
		{

			// Extract the thread parameters.
			int matchId = (int)parameter;

			// The symbol, title and the bitmap for the corporate logo will be retrieved from the data model in the code below.
			// They will be used to initialize the pop-up dialog after the locks on the data model have been released.
			string symbol = string.Empty;
			string title = string.Empty;
			Bitmap logo = null;

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.NegotiationLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// The match record, working order, order type and security records are used to construct the title, symbol and
				// logo used by the notification window.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(matchId);
				ClientMarketData.WorkingOrderRow workingOrderRow = matchRow.WorkingOrderRow;
				ClientMarketData.OrderTypeRow orderTypeRow = workingOrderRow.OrderTypeRow;
				ClientMarketData.SecurityRow securityRow = workingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId;

				// Get the security symbol.
				symbol = securityRow.Symbol;

				// Create a logo bitmap.
				if (!securityRow.IsLogoNull())
				{
					MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(securityRow.Logo));
					logo = new Bitmap(memoryStream);
				}

				// Construct the title for the notification window.
				title = string.Format("{0} of {1}", orderTypeRow.Description, symbol);

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

			// The notification window looks and acts like the Microsoft Instant Messaging window.  It will pop up in the lower
			// right hand corner of the screen with a title, the corporate logo and a chance to either accept or decline the
			// opportunity for a match.
			NotificationWindow notificationWindow = new NotificationWindow();
			notificationWindow.MatchId = matchId;
			notificationWindow.Symbol = symbol;
			notificationWindow.Message = title;
			notificationWindow.CompanyLogo = logo;
			notificationWindow.Accept += new MatchEventHandler(AcceptNegotiation);
			notificationWindow.Decline += new MatchEventHandler(DeclineNegotiation);
			notificationWindow.ChangeOptions += new EventHandler(ChangeOptions);
			notificationWindow.Show();

		}

		/// <summary>
		/// Accepts the opportunity to match an order.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="matchEventArgs">The event arguments.</param>
		private void AcceptNegotiation(object sender, MatchEventArgs matchEventArgs)
		{

			// The next step involves extracting data from the data model and must be done in a different thread than the window
			// thread.
			ThreadPool.QueueUserWorkItem(new WaitCallback(AcceptNegotiationThread), matchEventArgs.MatchId);

		}

		/// <summary>
		/// Marshals the data needed to navigate the client application to the accepted item in the Match blotter.
		/// </summary>
		/// <param name="parameter">The thread initialization data.</param>
		private void AcceptNegotiationThread(object parameter)
		{

			// Extract the parameters.
			int matchId = (int)parameter;

			// The main goal of this section of code is to construct an object that can be used to navigate the
			// client container to the selected item in the blotter.
			BlotterMatchDetail blotterMatchDetail = new BlotterMatchDetail();

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// The 'BlotterMatchDetail' can be used to open up a Blotter in a viewer and select the Match.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(matchId);
				if (matchRow != null)
				{
					Blotter blotter = new Blotter(matchRow.WorkingOrderRow.BlotterRow.BlotterId);
					Match match = new Match(matchRow.MatchId);
					blotterMatchDetail = new BlotterMatchDetail(blotter, new Match[] { match });
				}

			}
			finally
			{

				// Release the locks.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Notify the owner of this service that it should navigate to the item in the matching blotter that was selected by
			// the user.
			if (this.OpenObject != null)
				this.OpenObject(this, blotterMatchDetail);

		}

		/// <summary>
		/// Refuse the opportunity to match an order.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="matchEventArgs">The event arguments.</param>
		private void DeclineNegotiation(object sender, MatchEventArgs matchEventArgs)
		{

			// The next step involves extracting data from the data model and must be done in a different thread than the window
			// thread.
			ThreadPool.QueueUserWorkItem(new WaitCallback(DeclineNegotiationThread), matchEventArgs.MatchId);

		}

		/// <summary>
		/// Marshals the data needed refuse a chance to negotiate a trade.
		/// </summary>
		/// <param name="parameter">The thread initialization data.</param>
		private void DeclineNegotiationThread(object parameter)
		{

			// Extract the thread parameters
			int matchId = (int)parameter;

			// This batch, if succesfully constructed, will be sent to the server to deline the negotiation.
			Batch batch = new Batch();

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.OrderTypeLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(ClientTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(ClientTimeout.LockWait);

				// Find the Match record.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(matchId);

				// Construct a command decline the negotiation.
				AssemblyPlan assembly = batch.Assemblies.Add("Core Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Core.Negotiation");
				TransactionPlan transaction = batch.Transactions.Add();
				MethodPlan method = transaction.Methods.Add(type, "Insert");
				method.Parameters.Add(new InputParameter("matchId", matchId));
				method.Parameters.Add(new InputParameter("quantity", 0.0m));
				method.Parameters.Add(new InputParameter("statusCode", Status.Declined));

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				// This indicates that the batch shouldn't be executed.
				batch = null;

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

			// If the command batch was built successfully, then execute it.
			if (batch != null)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there
					// are no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(batch);

				}
				catch (BatchException batchException)
				{

					// Write any server generated error messages to the event log.
					foreach (Exception exception in batchException.Exceptions)
						EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				}

		}

		private void ChangeOptions(object sender, EventArgs eventArgs)
		{


		}

	}

}
