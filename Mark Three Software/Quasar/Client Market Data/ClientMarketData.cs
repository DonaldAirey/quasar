/*************************************************************************************************************************
*
*	File:			ClientMarketData.c
*	Description:	Component use to capture all the client-side data.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Client
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using Shadows.Quasar.Client.WebService;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Used to access shared data that is refreshed automatically from the server.
	/// </summary>
	public class ClientMarketData : Shadows.Quasar.Common.MarketData
	{

		private static long rowVersion;
		private static DateTime timeStamp;
		private static System.Threading.Thread threadBackground;
		private static System.Threading.AutoResetEvent refreshEvent;
		private static System.Threading.AutoResetEvent startDrawEvent;
		private static System.Threading.AutoResetEvent endDrawEvent;
		private static System.Threading.Mutex refreshMutex;
		private static ArrayList mergedRows;
		public static event EventHandler BeginMerge;
		public static event EventHandler EndMerge;
		public static long RowVersion {get {refreshMutex.WaitOne(); long r = rowVersion; refreshMutex.ReleaseMutex(); return r;}}

		/// <summary>
		/// Constructor for a ClientMarketData when used as a component.
		/// </summary>
		public ClientMarketData() : base(){}

		/// <summary>
		/// Constructor for a ClientMarketData when used as a component.
		/// </summary>
		/// <param name="container">The container object.</param>
		public ClientMarketData(System.ComponentModel.IContainer container) : base(container) {}

		/// <summary>
		/// Initializes the static members of the ClientMarketData class.
		/// </summary>
		static ClientMarketData()
		{

			// The 'rowVersion' keeps track of the 'age' of the data model.  It's the primary method to synchronize the client with
			// the server.  The 'timeStamp' value gives an absolute reference to the 'rowVersion' value. That is, each time the
			// server resets, the rowVersions begin counting again.  The 'timeStamp' from the server tells us if we can compare the
			// 'rowVersion's.  If the timstamp between the server and the client are different, then we have to re-synchronize the
			// client and the server.
			ClientMarketData.rowVersion = 0;
			ClientMarketData.timeStamp = DateTime.MinValue;

			// These events are used to refresh the data model in the background and to enable synchronous refreshes of the
			// data model on request.
			ClientMarketData.refreshEvent = new AutoResetEvent(false);
			ClientMarketData.startDrawEvent = new AutoResetEvent(false);
			ClientMarketData.endDrawEvent = new AutoResetEvent(false);

			// This mutex is to insure that only one thread access the data model refresh at a time.
			ClientMarketData.refreshMutex = new Mutex(false);

			ClientMarketData.mergedRows = new ArrayList();
			
			foreach (Table table in ClientMarketData.Tables)
			{
				table.RowChanged += new DataRowChangeEventHandler(RowChangedEvent);
				table.RowDeleted += new DataRowChangeEventHandler(RowChangedEvent);
			}

			// This thread is used to access the database for the objects that this user has access to.  The end product of this
			// thread is a tree structure that contains the hierarchy of objects (accounts, users, trading desks, securities, etc.)
			// that the user can navigate.  The results are passed back to this thread through the delegate procedure 'objectNode'
			// after the control window is created.
			threadBackground = new Thread(new ThreadStart(ThreadBackground));
			threadBackground.IsBackground = true;
			threadBackground.Priority = System.Threading.ThreadPriority.Lowest;
			threadBackground.Name = "ClientMarketData Background";
			threadBackground.Start();

		}

		#region Dispose Method
		/*****************************************************************************************************************
		*	Method:			Dispose
		*	Parameters:		bool disposing - True 
		*	Description:	Constructor for the FolderList control that is used to present Quasar objects (accounts, users
		*					blotters, securities, etc.)
		*****************************************************************************************************************/

		protected override void Dispose(bool disposing)
		{

			// Terminate the background thread when this component is destroyed.
			ClientMarketData.threadBackground.Abort();

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}
		#endregion

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
		/// Creates a collection of modified rows to accelerate merge time.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="dataRowChangeEventArgs">The event arguments.</param>
		private static void RowChangedEvent(object sender, DataRowChangeEventArgs dataRowChangeEventArgs)
		{

			// IMPORTANT CONCEPT: After a merge, the results must be accepted to purge out deleted records and maintain the state
			// of the row properly.  However, the 'AcceptChanges' operation for the DataSet and DataTable will attempt to commit
			// every row in the DataSet or DataTable, respectively.  This is an unaccetably inefficient operation.  This event
			// handler will collect every row that was modified during the merge.  When the merge is complete, only the modified
			// rows need to be committed: a much faster operation.  Note that 'Commit' actions are ignored.  If this check were not
			// included, an infinite cycle of 'Merge-Commit-Repeat' would ensue.
			if (dataRowChangeEventArgs.Action != DataRowAction.Commit)
				ClientMarketData.mergedRows.Add(dataRowChangeEventArgs.Row);

		}
		
		/// <summary>
		/// Event trigger when any record in the data model has changed.
		/// </summary>
		/// <param name="sender">Object originating the event.</param>
		public static void OnBeginMerge(object sender)
		{

			// If a handler has been associated with this event, invoke it.
			if (ClientMarketData.BeginMerge != null) ClientMarketData.BeginMerge(sender, EventArgs.Empty);

		}
		
		/// <summary>
		/// Event trigger when any record in the data model has changed.
		/// </summary>
		/// <param name="sender">Object originating the event.</param>
		public static void OnEndMerge(object sender)
		{

			// If a handler has been associated with this event, invoke it.
			if (ClientMarketData.EndMerge != null) ClientMarketData.EndMerge(sender, EventArgs.Empty);

		}
		
		/// <summary>
		/// ThreadBackground
		/// </summary>
		/// <remarks>
		/// This procedure runs as a thread.  It will asynchronously query the database for the raw data that makes up the
		/// hierarchy of objects found in the application.  It will then organize the raw, flat data into a hierarchial
		/// tree that's suitable to be viewed in a TreeView control.</remarks>
		private static void ThreadBackground()
		{

			// Create a web client for communicating with the server.
			WebClient webClient = new WebClient();
			
			// This thread body is executed until the application is terminated.
			while (true)
			{

				try
				{

					// Maure sure only one thread at a time tries to refresh the data model.
					ClientMarketData.refreshMutex.WaitOne();

					// IMPORTANT CONCEPT: The rowVersion keeps track of the state of the client in-memory database. The server
					// returns a value for us to use on the next cycle.  If, for any reason, the merge of the data on the client
					// should fail, we won't use the new value on the next cycle. That is, we're going to keep on asking for the
					// same incremental set of records until we've successfully merged them.  This guarantees that the client and
					// server databases stay consistent with each other.
					DateTime timeStamp = ClientMarketData.timeStamp;
					long rowVersion = ClientMarketData.rowVersion;

					// IMPORTANT CONCEPT:  Executing the 'GetClientMarketData' with a rowVersion returns to the client a DataSet
					// with only records that are the same age or younger than the rowVersion. This reduces the traffic on the
					// network to include only the essential records.  We are also merging it with the current ClientMarketData,
					// which adds new records and records that were deleted by the server.
					DataSet dataSet = webClient.Reconcile(ref timeStamp, ref rowVersion);

					// If the time stamps are out of sync, then the server has reset since our last refresh (or this is the first
					// time refreshing the market data).  This will reset the client side of the data model so that -- after the
					// current results are merged -- the client and the server will be in sync again.
					if (ClientMarketData.timeStamp != timeStamp)
					{
						ClientMarketData.rowVersion = 0;
						ClientMarketData.timeStamp = timeStamp;
						ClientMarketData.Clear();
					}

					// Optimization: Don't merge the results if there's nothing to merge.
					if (rowVersion > ClientMarketData.rowVersion)
					{

						// IMPORTANT CONCEPT: This broadcast can be used to set up conditions for the data event handlers. Often,
						// optimizing algorithms will be used to consolidate the results of a merge.  This will allow the event
						// driven logic to clear previous results and set up initial states for handling the bulk update of data.
						ClientMarketData.OnBeginMerge(typeof(ClientMarketData));

						try
						{

							// IMPORTANT CONCEPT: Since the results will still be on the server if the client misses a refresh
							// cycle, we take the attitude that this update process doesn't have to wait for locks. That is, if we
							// can't get all the tables locked quickly, we'll just wait until the next refresh period to get the
							// results. This effectively prevents deadlocking on the client. Make sure all the tables are locked
							// before populating them.
							foreach (TableLock tableLock in ClientMarketData.TableLocks)
								tableLock.AcquireWriterLock(ClientTimeout.LockWait);

							// IMPORANT CONCEPT:  Once all the write locks have been obtained, we can merge the results.  This will
							// trigger the events associated with the tables for updated and deleted rows.  Also, if 
							// "AcceptChanges" is invoked for a DataSet or a Table, every single record in the DataSet or DataTable
							// will be "Committed", even though they are unchanged.  This is a VERY inefficient operation.  To get
							// around this, an ArrayList of modified records is constructed during the Merge operation.  After the
							// merge, only the new records are committed.
							ClientMarketData.mergedRows.Clear();
							ClientMarketData.Merge(dataSet);
							for (int index = 0; index < ClientMarketData.mergedRows.Count; index++)
								((DataRow)ClientMarketData.mergedRows[index]).AcceptChanges();

							// If the merge operation was successful, then we can use the new rowVersion for the next cycle. Any
							// exception before this point will result in a request of the same set of data becaue the rowVersion
							// was never updated.
							ClientMarketData.rowVersion = rowVersion;

						}
						catch (ApplicationException applicationException)
						{

							// Tyipcally, a failure to gain a lock will invoke this exception.  If this information ends up being 
							// too much for the log device, we can alway inhibit exception catching here.
							Debug.WriteLine(applicationException.Message);

						}
						catch (ConstraintException)
						{

							// Write out the exact location of the error.
							foreach (DataTable dataTable in ClientMarketData.Tables)
								foreach (DataRow dataRow in dataTable.Rows)
									if (dataRow.HasErrors)
										Console.WriteLine("Error in '{0}': {1}", dataRow.Table.TableName, dataRow.RowError);
						
						}
						catch (Exception exception)
						{

							// Write the error and stack trace out to the debug listener
							Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

						}
						finally
						{

							// No matter what happens above, we need to release the locks acquired above.
							foreach (TableLock tableLock in ClientMarketData.TableLocks)
								if (tableLock.IsWriterLockHeld)
									tableLock.ReleaseWriterLock();

						}

						// IMPORTANT CONCEPT: When the merge is complete, this will broadcast an event which allows optimization code
						// to consolidate the results, examine the changed values and update reports based on the changed data.
						ClientMarketData.OnEndMerge(typeof(ClientMarketData));

					}

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Other threads can now request a refresh of the data model.
					ClientMarketData.refreshMutex.ReleaseMutex();

				}

				// Wait for a predetermined interval before refreshing the data model again.
				Thread.Sleep(ClientTimeout.RefreshInterval);

			}

		}

		/// <summary>
		/// Execute a command batch and synchronizes the client data model with the newer records from the server.
		/// </summary>
		public static void Execute(RemoteBatch remoteBatch)
		{

			// This 'try' block will insure that the mutual exclusion locks are released.
			try
			{

				// Maure sure only one thread at a time tries to refresh the data model.
				ClientMarketData.refreshMutex.WaitOne();

				// IMPORTANT CONCEPT: The rowVersion keeps track of the state of the client in-memory database. The server returns
				// a value for us to use on the next cycle.  If, for any reason, the merge of the data on the client should fail,
				// we won't use the new value on the next cycle. That is, we're going to keep on asking for the same incremental
				// set of records until we've successfully merged them.  This guarantees that the client and server databases stay
				// consistent with each other.
				DateTime timeStamp = ClientMarketData.timeStamp;
				long rowVersion = ClientMarketData.rowVersion;

				// IMPORTANT CONCEPT:  Executing the 'Reconcile' Web Service with a rowVersion returns to the client a DataSet with
				// only records that are the same age or younger than the rowVersion. This reduces the traffic on the network to
				// include only the essential records.  This set also contains the deleted records.  When the DataSet that is
				// returned from this Web Service is merged with the current data, the client will be reconciled with the server as
				// of the 'rowVersion' returned from the server.
				DataSet resultSet;
				WebClient webClient = new WebClient();
				DataSet dataSet = webClient.ExecuteAndReconcile(ref timeStamp, ref rowVersion, remoteBatch, out resultSet);
				remoteBatch.Merge(resultSet);

				// If the time stamps are out of sync, then the server has reset since our last refresh (or this is the first time
				// refreshing the data mdoel).  This will reset the client side of the data model so that, after this refresh, the
				// client and the server will be in sync again.
				if (ClientMarketData.timeStamp != timeStamp)
				{
					ClientMarketData.rowVersion = 0;
					ClientMarketData.timeStamp = timeStamp;
					ClientMarketData.Clear();
				}

				// Optimization: Don't merge the results if there's nothing to merge.
				if (rowVersion > ClientMarketData.rowVersion)
				{

					// IMPORTANT CONCEPT: This broadcast can be used to set up conditions for the data event handlers.  Often,
					// optimizing algorithms will be used to consolidate the results of a merge.  This will allow the event driven
					// logic to clear previous results and set up initial states for handling the bulk update of data.
					ClientMarketData.OnBeginMerge(typeof(ClientMarketData));

					// This 'try' block will insure that the locks are released if there's an error.
					try
					{

						// IMPORTANT CONCEPT: Make sure that there aren't any nested locks.  Table locks have to be aquired in the
						// same order for every thread that uses the shared data model.  It's possible that the current thread may
						// have locked table 'B' if the preamble, then try to lock table 'A' during the processing of a command
						// batch.  If another thread aquires the lock on table 'A' and blocks on table 'B', we will have a
						// deadlock.  This is designed to catch situations where the 'Execute' method is called while tables are
						// already locked.
						Debug.Assert(!ClientMarketData.AreLocksHeld);

						// IMPORTANT CONCEPT: Since the results will still be on the server if the client misses a refresh cycle,
						// we take the attitude that this update process doesn't have to wait for locks. That is, if we can't get
						// all the tables locked quickly, we'll just wait until the next refresh period to get the results. This
						// effectively prevents deadlocking on the client. Make sure all the tables are locked before populating
						// them.
						foreach (TableLock tableLock in ClientMarketData.TableLocks)
							tableLock.AcquireWriterLock(ClientTimeout.LockWait);

						// IMPORANT CONCEPT:  Once all the write locks have been obtained, we can merge the results.  This will
						// trigger the events associated with the tables for updated and deleted rows.  Also, if 
						// "AcceptChanges" is invoked for a DataSet or a Table, every single record in the DataSet or DataTable
						// will be "Committed", even though they are unchanged.  This is a VERY inefficient operation.  To get
						// around this, an ArrayList of modified records is constructed during the Merge operation.  After the
						// merge, only the new records are committed.
						ClientMarketData.mergedRows.Clear();
						ClientMarketData.Merge(dataSet);
						for (int index = 0; index < ClientMarketData.mergedRows.Count; index++)
							((DataRow)ClientMarketData.mergedRows[index]).AcceptChanges();

						// If the merge operation was successful, then we can use the new rowVersion for the next cycle. Any
						// exception before this point will result in a request of the same set of data becaue the rowVersion was
						// never updated.
						ClientMarketData.rowVersion = rowVersion;

					}
					catch (ConstraintException)
					{

						// Write out the exact location of the error.
						foreach (DataTable dataTable in ClientMarketData.Tables)
							foreach (DataRow dataRow in dataTable.Rows)
								if (dataRow.HasErrors)
									Console.WriteLine("Error in '{0}': {1}", dataRow.Table.TableName, dataRow.RowError);
						
					}
					catch (Exception exception)
					{

						// Write the error and stack trace out to the debug listener
						Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

					}
					finally
					{

						// No matter what happens above, we need to release the locks acquired above.
						foreach (TableLock tableLock in ClientMarketData.TableLocks)
							if (tableLock.IsWriterLockHeld)
								tableLock.ReleaseWriterLock();

					}

					// IMPORTANT CONCEPT: When the merge is complete and the tables are unlocked, this will broadcast an event
					// which allows optimization code to consolidate the results, examine the changed values and update reports
					// based on the changed data.
					ClientMarketData.OnEndMerge(typeof(ClientMarketData));

				}

			}
			finally
			{

				// Other threads can now request a refresh of the data model.
				ClientMarketData.refreshMutex.ReleaseMutex();

			}

			// Throw a specialized exception if the server returned any errors in the RemoteBatch structure.
			if (remoteBatch.HasExceptions)
				throw new BatchException(remoteBatch);

		}

		/// <summary>
		/// Execute the remote batch and catch any batch errors.
		/// </summary>
		/// <param name="remoteBatch"></param>
		public static void Send(RemoteBatch remoteBatch)
		{
		
			// The top level window of the application is needed as the owner of the error message dialog box that will pop up to
			// process the errors.
			System.Windows.Forms.Control activeForm = Form.ActiveForm;
			System.Windows.Forms.Control topLevelControl = activeForm == null ? null : activeForm.TopLevelControl;

			try
			{

				// Execute the remote batch.
				Execute(remoteBatch);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
				{
					DialogResult dialogResult = MessageBox.Show(topLevelControl, remoteException.Message, "Quasar Error",
						MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					if (dialogResult == DialogResult.Cancel)
						break;
				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

	}

}
