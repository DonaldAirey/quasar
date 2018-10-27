namespace MarkThree.Guardian.Client
{

	using MarkThree;
	using MarkThree.Client;
	using MarkThree.Guardian;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Used to access shared data that is refreshed automatically from the server.
	/// </summary>
	[System.ComponentModel.DesignerCategory("Component")]
	public class ClientMarketData : MarkThree.Guardian.MarketData
	{

		// Private Static Members
		private static System.Boolean isReconcilliationThreadRunning;
		private static System.Boolean isReset;
		private static System.Int64 rowVersion;
		private static System.Threading.Thread threadBackground;
		private static System.Threading.Mutex clientDataMutex;

		// Public Events
		public static event EventHandler BeginMerge;
		public static event EventHandler EndMerge;

		/// <summary>
		/// Initializes the static members of the ClientMarketData class.
		/// </summary>
		static ClientMarketData()
		{

			// The 'rowVersion' keeps track of the 'age' of the data model.  It's the primary method to synchronize the client with
			// the server.
			ClientMarketData.rowVersion = 0L;

			// This mutex is to insure that only one thread access the data model refresh at a time.
			ClientMarketData.clientDataMutex = new Mutex(false);

			// The data model is cleared and reloaded when the credentials change to reflect the subset of data available to the 
			// selected user.
			WebTransactionProtocol.CredentialsChanged += new EventHandler(ChangeCredentials);

		}

		/// <summary>
		/// Constructor for a ClientMarketData when used as a component.
		/// </summary>
		public ClientMarketData() : base()
		{

#if DEBUG
			// This will insure that the background thread to access the server isn't spawned when in the design mode.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				if (ClientMarketData.InstanceCount == 1)
				{

					// This thread is used to access the database for the objects that this user has access to.  The end product of this
					// thread is a tree structure that contains the hierarchy of objects (accounts, users, trading desks, securities, etc.)
					// that the user can navigate.  The results are passed back to this thread through the delegate procedure 'objectNode'
					// after the control window is created.
					threadBackground = new Thread(new ThreadStart(ReconcilliationThread));
					threadBackground.Name = "ClientMarketData Background";
					threadBackground.Start();

				}

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Constructor for a ClientMarketData when used as a component.
		/// </summary>
		/// <param name="container">The container object.</param>
		public ClientMarketData(System.ComponentModel.IContainer container) : base(container)
		{

#if DEBUG
			// This will insure that the background thread to access the server isn't spawned when in the design mode.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// Begin the background thread to keep the client data model reconciled with the server data model when the first
				// reference is made.
				if (ClientMarketData.InstanceCount == 1)
				{

					// This thread is used to access the database for the objects that this user has access to.  The end product of
					// this thread is a tree structure that contains the hierarchy of objects (accounts, users, trading desks,
					// securities, etc.) that the user can navigate.  The results are passed back to this thread through the
					// delegate procedure 'objectNode' after the control window is created.  Note that this is a background 
					// thread: it will self-immolate when the foreground threads are terminated.
					threadBackground = new Thread(new ThreadStart(ReconcilliationThread));
					threadBackground.Name = "ClientMarketData Background";
					threadBackground.Start();

				}

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Dispose of the managed resources allocated by this object.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{

			// If 'disposing' is set by the caller, the managed resources owned by this component should be released.
			if (disposing)
			{

				// Terminate the background thread when this component is destroyed.
				if (MarketData.InstanceCount == 1)
				{
					ClientMarketData.IsReconcilliationThreadRunning = false;
					if (ClientMarketData.threadBackground.Join(ClientTimeout.EndOfThreadWait))
						ClientMarketData.threadBackground.Abort();
				}

			}

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		/// <summary>
		/// Gets or sets an indication of whether the background thread that reconciles the client data model is running or not.
		/// </summary>
		private static bool IsReconcilliationThreadRunning
		{
			get {lock (typeof(ClientMarketData)) return ClientMarketData.isReconcilliationThreadRunning;}
			set {lock (typeof(ClientMarketData)) ClientMarketData.isReconcilliationThreadRunning = value;}
		}

		/// <summary>
		/// Resets the data model when the credentials have changed.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="eventArgs">The (empty) event arguments.</param>
		private static void ChangeCredentials(object sender, EventArgs eventArgs)
		{

			// This will instruct the logic that loads the data to clear out the data model first.
			ClientMarketData.isReset = true;

		}

		/// <summary>
		/// Event trigger when any record in the data model has changed.
		/// </summary>
		/// <param name="sender">Object originating the event.</param>
		public static void OnBeginMerge(object sender)
		{

			// If a handler has been associated with this event, invoke it.
			if (ClientMarketData.BeginMerge != null)
				ClientMarketData.BeginMerge(sender, EventArgs.Empty);

		}
		
		/// <summary>
		/// Event trigger when any record in the data model has changed.
		/// </summary>
		/// <param name="sender">Object originating the event.</param>
		public static void OnEndMerge(object sender)
		{

			// If a handler has been associated with this event, invoke it.
			if (ClientMarketData.EndMerge != null)
				ClientMarketData.EndMerge(sender, EventArgs.Empty);

		}

		/// <summary>
		/// ReconcilliationThread
		/// </summary>
		/// <remarks>
		/// This procedure runs as a thread.  It will asynchronously query the database for the raw data that makes up the
		/// hierarchy of objects found in the application.  It will then organize the raw, flat data into a hierarchial
		/// tree that's suitable to be viewed in a TreeView control.</remarks>
		private static void ReconcilliationThread()
		{

			// The tread can be terminated gracefully from another thread by clearing this value.
			ClientMarketData.IsReconcilliationThreadRunning = true;

			// This thread body is executed until the application is terminated.
			while (ClientMarketData.IsReconcilliationThreadRunning)
			{

				try
				{

					// Execute an empty batch.  This has the effect of reconciling the data without executing any commands.
					Execute(new Batch());

				}
				catch (BatchException batchException)
				{

					// Write each of the exceptions taken on the server into the local event log.
					foreach (Exception exception in batchException.Exceptions)
						EventLog.Error(exception.Message);

				}
				catch (UserAbortException)
				{

					// Exit the application when the user aborts out of the connection dialog.
					ClientMarketData.IsReconcilliationThreadRunning = false;
					Application.Exit();

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					EventLog.Error(exception.Message);

				}

				// Wait for a predetermined interval before refreshing the data model again.
				Thread.Sleep(ClientTimeout.RefreshInterval);

			}

		}

		/// <summary>
		/// Execute a command batch and synchronizes the client data model with the newer records from the server.
		/// </summary>
		public static void Execute(Batch batch)
		{

			// This 'try' block will insure that the mutual exclusion locks are released.
			try
			{

				// Make sure only one thread at a time tries to refresh the data model.
				ClientMarketData.clientDataMutex.WaitOne();

				// Switching credentials without exiting the application will force the data model to be cleared and the row
				// counting to begin at zero again.  That is, it effectively resets the client data model.
				if (ClientMarketData.isReset)
				{
					ClientMarketData.isReset = false;
					ClientMarketData.rowVersion = 0;
					ClientMarketData.Clear();
				}

				// IMPORTANT CONCEPT: The rowVersion keeps track of the state of the client in-memory database. The server
				// returns a value for us to use on the next cycle.
				long rowVersion = ClientMarketData.rowVersion;

				// IMPORTANT CONCEPT:  Executing the 'GetClientMarketData' with a rowVersion returns to the client a DataSet
				// with only records that are the same age or younger than the rowVersion. This reduces the traffic on the
				// network to include only the essential records.  We are also merging it with the current ClientMarketData,
				// which adds new records and records that were deleted by the server.
				AssemblyPlan assembly = batch.Assemblies.Add("Server Market Data");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Server.ServerMarketData");
				TransactionPlan transaction = batch.Transactions.Add();
				MethodPlan method = transaction.Methods.Add(type, "Reconcile");
				Parameter rowVersionParameter = method.Parameters.Add(new InputParameter("rowVersion", rowVersion));

				// Execute the the command to get the incremental changes from the server.
				WebTransactionProtocol.Execute(batch);

				// This is the updated data model information returned from the server.
				ArrayList reconciledData = (ArrayList)method.Parameters.Return.Value;

				// Optimization: Don't merge the results if there's nothing to merge.
				if (reconciledData != null)
				{

					// IMPORTANT CONCEPT: Since the results will still be on the server if the client misses a refresh
					// cycle, we take the attitude that thBzis update process doesn't have to wait for locks. That is, if we
					// can't get all the tables locked quickly, we'll just wait until the next refresh period to get the
					// results. This effectively prevents deadlocking on the client. Make sure all the tables are locked
					// before populating them.
					foreach (TableLock tableLock in ClientMarketData.TableLocks)
						tableLock.AcquireWriterLock(ClientTimeout.LockWait);

					// IMPORTANT CONCEPT: This broadcast can be used to set up conditions for the data event handlers. Often,
					// optimizing algorithms will be used to consolidate the results of a merge.  This will allow the event
					// driven logic to clear previous results and set up initial states for handling the bulk update of data.
					ClientMarketData.OnBeginMerge(typeof(ClientMarketData));

					// IMPORANT CONCEPT:  Once all the write locks have been obtained, we can merge the results.  This will
					// trigger the events associated with the tables for updated and deleted rows.  Note that this 'Merge' 
					// method has different characteristics than the original one provided by Microsoft (that is, this one
					// works).
					ClientMarketData.rowVersion = MarketData.Merge(reconciledData);

					// IMPORTANT CONCEPT: When the merge is complete, this will broadcast an event which allows optimization code
					// to consolidate the results, examine the changed values and update reports based on the changed data.
					ClientMarketData.OnEndMerge(typeof(ClientMarketData));

				}

			}
			finally
			{

				// No matter what happens above, we need to release the locks acquired above.
				foreach (TableLock tableLock in ClientMarketData.TableLocks)
					if (tableLock.IsWriterLockHeld)
						tableLock.ReleaseWriterLock();

				// Other threads can now request a refresh of the data model.
				ClientMarketData.clientDataMutex.ReleaseMutex();

			}

			// Throw a specialized exception if the server returned any errors in the Batch structure.
			if (batch.HasExceptions)
				throw new BatchException(batch);

		}

		/// <summary>
		/// Execute the batch and catch the errors.
		/// </summary>
		/// <param name="batch">A batch of commands to be executed.</param>
		/// <returns>Any exceptions from executing the batch, null if there are none.</returns>
		public static bool Execute(Batch batch, Control owner, BatchExceptionEventHandler batchExceptionEventHandler)
		{

			bool success = false;

			// Execute the batch and process the results.
			if (batch != null)
			{

				success = true;

				try
				{


					// Execute the batch on the server and process the results.
					ClientMarketData.Execute(batch);

				}
				catch (BatchException batchException)
				{

					success = false;

					owner.BeginInvoke(batchExceptionEventHandler, new object[] { typeof(ClientMarketData),
						new BatchExceptionEventArgs(batchException) });

				}

			}

			return success;

		}

	}

}
