namespace MarkThree.Guardian.Server
{

	using MarkThree;
	using MarkThree.Guardian;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Configuration;
	using System.Data.SqlClient;
	using System.Data;
	using System.Threading;

	/// <summary>The server data model for the Guardian Crossing Network.</summary>
	public class ServerMarketData : MarketData
	{

		// Private Static Members
		private static string PersistentStore = "Guardian";
		private static RowVersion rowVersion;
		private static Hashtable masks;
		private static Hashtable maskLocks;
		private static TableLock maskLock;

		/// <summary>
		/// Initializes the Globa Data Model.
		/// </summary>
		static ServerMarketData()
		{

			// IMPORTANT CONCEPT: The RowVersion object keeps track of the 'age' of the server dataset.  When rows are added,
			// updated and deleted, the row version is incremented.  There is a single, monotonically incrementing value used for
			// the entire DataSet.  We initialize the database with a value of 1, that allows the client to use a value of zero
			// when initializing.  A requested row version of zero will guarantee that all the contents of the database are
			// returned to the client.
			ServerMarketData.rowVersion = new RowVersion();

			// The persistent storage device used by this server is specified in this custom configuration section.
			PersistentStoreSection persistentStoreSection =
				(PersistentStoreSection)ConfigurationManager.GetSection("persistentStoreSection");
			PersistentStoreInfo persistentStoreInfo = persistentStoreSection[PersistentStore];
			if (persistentStoreInfo == null)
				throw new Exception("There is no persistent storage device defined for this server.");
			SqlConnection sqlConnection = new SqlConnection(persistentStoreInfo.ConnectionString);

			try
			{

				// Make sure all the tables are locked before populating them.
				foreach (TableLock tableLock in ServerMarketData.TableLocks)
					tableLock.AcquireWriterLock(CommonTimeout.LockWait);

				// IMPORTANT CONCEPT: These filters are used for security and performance.  They remove elements that a user has 
				// no right to view on the local client.  They must also guarantee referential integrity if a record is removed.
				// That is, if you have a filter to remove an element, there must be another filter to guarantee the children of
				// that element are not returned to the client.
				ServerMarketData.Object.UserFilter = new RowFilterDelegate(FilterObject);
				ServerMarketData.Security.UserFilter = new RowFilterDelegate(FilterSecurity);
				ServerMarketData.Price.UserFilter = new RowFilterDelegate(FilterPrice);
				ServerMarketData.Currency.UserFilter = new RowFilterDelegate(FilterCurrency);
				ServerMarketData.Debt.UserFilter = new RowFilterDelegate(FilterDebt);
				ServerMarketData.Equity.UserFilter = new RowFilterDelegate(FilterEquity);
				ServerMarketData.WorkingOrder.UserFilter = new RowFilterDelegate(FilterWorkingOrder);
				ServerMarketData.SourceOrder.UserFilter = new RowFilterDelegate(FilterSourceOrder);
				ServerMarketData.DestinationOrder.UserFilter = new RowFilterDelegate(FilterDestinationOrder);
				ServerMarketData.Execution.UserFilter = new RowFilterDelegate(FilterExecution);
				ServerMarketData.Allocation.UserFilter = new RowFilterDelegate(FilterAllocation);
				ServerMarketData.Match.UserFilter = new RowFilterDelegate(FilterMatch);
				ServerMarketData.Negotiation.UserFilter = new RowFilterDelegate(FilterNegotiation);

				// A view is needed for all the tables so we can search the records according to 'age'.  The 'RowVersion' is an
				// indication of the relative age of an record.  When a client requests a refresh of it's data model, we will
				// return any records that are younger than the oldest record on the client.  To find these records efficiently, we
				// need this view on the table.
				foreach (Table table in ServerMarketData.Tables)
				{
					table.DefaultView.Sort = "RowVersion DESC";
					table.DefaultView.RowStateFilter = DataViewRowState.OriginalRows;
				}

				// Open a connection to the server and read all the tables into the data model.
				sqlConnection.Open();

				// Constraints are disabled so the data can be loaded in table-by-table from the persistent store.
				ServerMarketData.EnforceConstraints = false;

				// This will keep track of the largest row number in the persistent data store.  This maximum row version will
				// become the seed value for the system-wide row version that is assigned to every row that is added, updated or 
				// deleted.
				long maxRowVersion = 0;

				// Read each of the persistent tables from the relational database store.
				foreach (Table table in ServerMarketData.Tables)
					if (table.IsPersistent)
					{

						// Every record has an 'Archived' bit which indicates whether the record has been deleted from the active
						// database.  This bit is used here to discard records that shouldn't be included in the active database.
						// While it would be possible to simply select all the records where the 'Archived' bit is clear, they are
						// added to the table and then deleted in order to keep the identity columns of the ADO tables synchronized
						// with the indices of the persistent SQL tables.  This section of code will construct a statement that
						// will select all the persistent columns from the relational database.
						string columnList = "IsArchived, IsDeleted";
						foreach (Column column in table.Columns)
							if (column.IsPersistent)
								columnList += (columnList == string.Empty ? string.Empty : ", ") + "\"" + column.ColumnName + "\"";
						string selectCommand = String.Format("select {0} from \"{1}\"", columnList, table.TableName);

						// Execute the command to read the table.
						SqlCommand sqlCommand = new SqlCommand(selectCommand);
						sqlCommand.Connection = sqlConnection;
						SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

						// IMPORTANT CONCEPT:  The persistent database can store many deleted and archived rows.  To minimize the
						// amount of memory needed to read these tables, the rows are reused as they are read when the row has been
						// deleted or archived.  Active rows will be added to the data model and a new row will be created the next
						// time a row from that table is read.
						Row row = null;

						// Read each record from the table into the ADO database.
						while(sqlDataReader.Read()) 
						{

							// Create a new record (unless the row is being reused because it was deleted or archived).
							if (row == null)
								row = (Row)table.NewRow();

							// Read all records from the database, throw away the ones that are archived.  This will keep the
							// identity columns in the ADO database from repeating any of the key elements in the SQL database.
							bool isArchived = false;
							bool isDeleted = false;
							for (int column = 0; column < sqlDataReader.FieldCount; column++)
							{
								string columnName = sqlDataReader.GetName(column);

								if (columnName == "IsArchived")
									isArchived = (bool)sqlDataReader.GetValue(column);
								if (columnName == "IsDeleted")
									isDeleted = (bool)sqlDataReader.GetValue(column);

								DataColumn destinationColumn = table.Columns[columnName];
								if (destinationColumn != null)
									row[destinationColumn] = sqlDataReader.GetValue(column);

							}

							// IMPORTANT CONCEPT: The initial system-wide row version used by the in-memory data model to keep 
							// track of the age of a record will be the maximum row version read from the persistent store
							// (including deleted and archived rows).
							maxRowVersion = row.RowVersion > maxRowVersion ? row.RowVersion : maxRowVersion;

							// Add active rows to the ADO table, reuse deleted and archived rows for the next record read.
							if (!isArchived && !isDeleted)
							{
								table.Rows.Add(row);
								row = null;
							}

						}

						// This is the end of reading a table.  Close out the reader, accept the changes and move on to the next
						// table in the DataSet.
						sqlDataReader.Close();
						table.AcceptChanges();

					}

				// This is the row version that will be used for all inserted, modified and deleted records.
				rowVersion.Set(maxRowVersion);

				// Once all the tables have been read, the constraints can be enforced again.  This is where any Relational 
				// Integrity problems will kick out.
				ServerMarketData.EnforceConstraints = true;

				// These masks are used to dynamically filter the data that is returned to the client.
				ServerMarketData.masks = new Hashtable();
				ServerMarketData.maskLocks = new Hashtable();
				ServerMarketData.maskLock = new TableLock("Mask.Master");

				// Run through each of the users and create a mask for their pricing data.
				foreach (ServerMarketData.UserRow userRow in ServerMarketData.User.Rows)
				{

					// UserRow.UserName must match MarkThree.Guardian.Server.Environment.UserName which is always lower case.
					string maskName = string.Format("Mask.{0}", userRow.UserName.ToLower());
					DataSet maskSet = new DataSet(maskName);
					ServerMarketData.masks[userRow.UserName.ToLower()] = maskSet;
					ServerMarketData.maskLocks[userRow.UserName.ToLower()] = new TableLock(maskName);

					// The mask has a single table with the security identifier in it.  Any price that matches the security
					// identifier is returned to the client.
					DataTable priceTable = maskSet.Tables.Add("Price");
					DataColumn securityIdColumn = priceTable.Columns.Add("SecurityId", typeof(int));
					priceTable.PrimaryKey = new DataColumn[] {securityIdColumn};

				}

			}
			catch (ConstraintException constraintException)
			{

				// Write out the exact location of the error.
				foreach (DataTable dataTable in ServerMarketData.Tables)
					foreach (DataRow dataRow in dataTable.Rows)
						if (dataRow.HasErrors)
							EventLog.Error("Error in '{0}': {1}", dataRow.Table.TableName, dataRow.RowError);

				// Rethrow the exception.
				throw constraintException;
						
			}
			catch (SqlException sqlException)
			{

				// Write out the exact location of the error.
				foreach (SqlError sqlError in sqlException.Errors)
					EventLog.Error(sqlError.Message);

				// Rethrow the exception.
				throw sqlException;
						
			}
			finally
			{

				// Release all of the write locks.
				foreach (TableLock tableLock in ServerMarketData.TableLocks)
					if (tableLock.IsWriterLockHeld)
						tableLock.ReleaseWriterLock();

				// Make sure the sqlConnection is closed, even when an exception happens.
				sqlConnection.Close();

			}

		}

		/// <summary>
		/// Constructor for a ServerMarketData when used as a component.
		/// </summary>
		public ServerMarketData() : base() {}

		/// <summary>
		/// Constructor for a ServerMarketData when used as a component.
		/// </summary>
		/// <param name="container">The container object.</param>
		public ServerMarketData(System.ComponentModel.IContainer container) : base(container) {}
	
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			// This will destroy any of the managed resources.
			if (disposing)
				if (components != null)
					components.Dispose();

			// Allow the base class to dispose of the remaining resources.
			base.Dispose(disposing);

		}
		
		#region Component Designer generatedCode
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with theCode editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion

		// Public Static Members
		public static RowVersion RowVersion {get {return ServerMarketData.rowVersion;}}

		/// <summary>
		/// The identifier of the current user.
		/// </summary>
		public static int UserId 
		{

			get
			{

				try
				{

					// Prevent any write operations on the User table.
					ServerMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Use the unique index to find a match to the aliased user name from the environment.
					int index = ServerMarketData.User.KeyUserUserName.Find(new object[] {System.Environment.UserName});
					if (index == -1)
						throw new Exception(string.Format("The user '{0}' is not mapped to a User", System.Environment.UserName));
					return (int)ServerMarketData.User.KeyUserUserName[index].Row[ServerMarketData.User.UserIdColumn];
				
				}
				finally
				{

					// Release the User table.
					ServerMarketData.UserLock.ReleaseReaderLock();

				}

			}

		}

		/// <summary>
		/// Authorizes a Object to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="objectDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterObject(DataRow userDataRow, DataRow objectDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.ObjectRow objectRow = (ServerMarketData.ObjectRow)objectDataRow;

			foreach (ServerMarketData.SecurityRow securityRow in objectRow.GetSecurityRows())
			{

				foreach (ServerMarketData.SourceOrderRow sourceOrderRow in securityRow.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
					if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
						return true;

				foreach (ServerMarketData.SourceOrderRow sourceOrderRow in securityRow.GetSourceOrderRowsBySecuritySourceOrderSettlementId())
					if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
						return true;

				return false;

			}

			return true;

		}

		/// <summary>
		/// Authorizes a Security to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="securityDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterSecurity(DataRow userDataRow, DataRow securityDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.SecurityRow securityRow = (ServerMarketData.SecurityRow)securityDataRow;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in securityRow.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in securityRow.GetSourceOrderRowsBySecuritySourceOrderSettlementId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			return false;

		}

		/// <summary>
		/// Authorizes a Price to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="priceDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterPrice(DataRow userDataRow, DataRow priceDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.PriceRow priceRow = (ServerMarketData.PriceRow)priceDataRow;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in priceRow.SecurityRow.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in priceRow.SecurityRow.GetSourceOrderRowsBySecuritySourceOrderSettlementId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			return false;

		}

		/// <summary>
		/// Authorizes a Currency to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="currencyDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterCurrency(DataRow userDataRow, DataRow currencyDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.CurrencyRow currencyRow = (ServerMarketData.CurrencyRow)currencyDataRow;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in currencyRow.SecurityRow.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in currencyRow.SecurityRow.GetSourceOrderRowsBySecuritySourceOrderSettlementId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			return false;

		}

		/// <summary>
		/// Authorizes a Debt to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="debtDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterDebt(DataRow userDataRow, DataRow debtDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.DebtRow debtRow = (ServerMarketData.DebtRow)debtDataRow;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in debtRow.SecurityRowBySecurityDebtDebtId.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			return false;

		}

		/// <summary>
		/// Authorizes a Equity to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="equityDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterEquity(DataRow userDataRow, DataRow equityDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.EquityRow equityRow = (ServerMarketData.EquityRow)equityDataRow;

			foreach (ServerMarketData.SourceOrderRow sourceOrderRow in equityRow.SecurityRowBySecurityEquityEquityId.GetSourceOrderRowsBySecuritySourceOrderSecurityId())
				if (Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow, sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow))
					return true;

			return false;

		}
		
		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterWorkingOrder(DataRow userDataRow, DataRow workingOrderDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.WorkingOrderRow workingOrderRow = (ServerMarketData.WorkingOrderRow)workingOrderDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				workingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterSourceOrder(DataRow userDataRow, DataRow sourceOrderDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.SourceOrderRow sourceOrderRow = (ServerMarketData.SourceOrderRow)sourceOrderDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				sourceOrderRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterDestinationOrder(DataRow userDataRow, DataRow destinationOrderDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.DestinationOrderRow destinationOrderRow = (ServerMarketData.DestinationOrderRow)destinationOrderDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				destinationOrderRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterExecution(DataRow userDataRow, DataRow executionDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.ExecutionRow executionRow = (ServerMarketData.ExecutionRow)executionDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				executionRow.DestinationOrderRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterAllocation(DataRow userDataRow, DataRow allocationDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.AllocationRow allocationRow = (ServerMarketData.AllocationRow)allocationDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				allocationRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterMatch(DataRow userDataRow, DataRow matchDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.MatchRow matchRow = (ServerMarketData.MatchRow)matchDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				matchRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		/// <summary>
		/// Authorizes a WorkingOrder to be returned to the client.
		/// </summary>
		/// <param name="userDataRow">Identifies the current user.</param>
		/// <param name="workingOrderDataRow">The record to be tested for authorization.</param>
		/// <returns>true if the record belongs in the user's hierarchy.</returns>
		public static bool FilterNegotiation(DataRow userDataRow, DataRow negotiationDataRow)
		{

			// This will test the record and return true if it belongs to the hierarchies this user is authorized to view.  False
			// if the record should not be included in the user's data model.
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)userDataRow;
			ServerMarketData.NegotiationRow negotiationRow = (ServerMarketData.NegotiationRow)negotiationDataRow;
			return Hierarchy.IsDescendant(userRow.SystemFolderRow.FolderRow.ObjectRow,
				negotiationRow.MatchRow.WorkingOrderRow.BlotterRow.ObjectRow);

		}

		public static void Reconcile(AdoTransaction adoTransaction)
		{

			// In order to capture the referential intergrity of the ServerMarketData, we need to prevent any updates to
			// the DataSet until we're through scanning it.
			foreach (TableLock tableLock in ServerMarketData.TableLocks)
				adoTransaction.LockRequests.AddReaderLock(tableLock);

		}

		/// <summary>
		/// Retrieves the records in the ServerMarketData that are newer than the given 'RowVersion'.
		/// </summary>
		/// <param name="RowVersion">A value which reflects the most recent record on the client.  This value will be
		/// updated by the server based on the returned DataSet and can be used for the next cycle.</param>
		/// <returns>All the records in a ServerMarketData that are newer than the given 'RowVersion'.</returns>
		public static void Reconcile(ParameterList parameters)
		{

			// Extract the client's current row version from the batch.
			long clientRowVersion = parameters["rowVersion"];

			// IMPORTANT CONCEPT: The data returned to the client is the set of rows that have been modified since the last call to
			// the 'reconcile' method.  The new data destined for the client is arranged in generic lists and arrays. This should
			// be done with structures, but the transmission of the type details of a structure over the network takes a measurable
			// amount of time.  This generic data structure of base types has the advantage of requiring less bandwidth to
			// serialize.  The big picture is that the data is passed back as an ArrayList containing essentially the table
			// information and a list of inserted, updated and deleted rows associated with each table structure.
			ArrayList reconciledData = null;

			// The "User" record of the current user is used to filter rows.
			int recordIndex = ServerMarketData.User.KeyUserUserName.Find(new object[] {System.Environment.UserName});
			if (recordIndex == -1)
				throw new Exception(string.Format("The user '{0}' is not mapped to a User", System.Environment.UserName));
			ServerMarketData.UserRow userRow = (ServerMarketData.UserRow)ServerMarketData.User.KeyUserUserName[recordIndex].Row;

			// The big picture is to scan through the tables looking for any row that has been modified after the 'RowVersion'
			// value passed to this method.  The 'RowVersion' acts as a one-size-fits-all state of the client data.  A generic data
			// structure will be constructed here with only the more recent records.  When the client merges this 'incremental'
			// data structure with the existing set, the server and client will be synchronized.
			foreach (Table sourceTable in ServerMarketData.Tables)
			{

				// IMPORTANT CONCEPT:  Only after we've determined that a table has new data exists will we move the schema into
				// the result set.  If there's no data in this table, it won't be initialized and a 'null' is returned to the 
				// client.
				ArrayList targetRows = null;

				// IMPORTANT CONCEPT: Start scanning the table from the most recent to the eldest record.  To accomplish this, we
				// need to view the data according to 'RowVersion' (in descending order to make the loop easier to read).  While
				// the view adds a little overhead for inserting and updating, the time is more than recovered during this
				// operation.
				foreach (System.Data.DataRowView dataRowView in sourceTable.DefaultView)
				{

					// Extract the source row from the view.  It is used several times below.
					Row sourceRow = (Row)dataRowView.Row;

					// This record's RowVersion will determine whether it is returned to the client or not.  Note that deleted
					// records as well as current are included in the view and passed back to the client when appropriate.
					long sourceRowVersion = (long)sourceRow[sourceTable.RowVersionColumn, DataRowVersion.Original];

					// To prevent the entire table from being scanned, only the records that are newer than the client's timestamp
					// are considered for transmission back to the client.  The default DataView on every table is organized by
					// RowVersions, so it's safe to quit this loop when a record is older than the client's data model.
					if (sourceRowVersion <= clientRowVersion)
						break;

					// All current records are compared against an optional filter and prevented from returning to the client if
					// they are not part of what the client is authorized to view.  Some tables are public and can be shared
					// without filters, other tables contain confidential information which is only transmitted to an entitled
					// client.  The entitlements are generally specified by the object tree.
					if (sourceRow.RowState != DataRowState.Deleted && sourceTable.UserFilter != null &&
						!sourceTable.UserFilter(userRow, sourceRow))
						continue;
					
					// The bucket to hold the rows isn't created until at least one row has been discovered that needs to be
					// returned to the client.  This is to keep the size of the returned data structure to a minimum when
					// transmitting the incremental records.
					if (targetRows == null)
					{

						// Like the bucket to hold the rows, the bucket that holds the tables -- the 'reconciledData' variable -- 
						// isn't created until there is at least one table to be returned to the client.
						if (reconciledData == null)
							reconciledData = new ArrayList();

						// The table-level record holds the name of the table, which will be used to look up the table on the
						// client from the DataSet's "Table" member.  It also holds a list that contains all the records in that
						// table.  There is one of these records for every table that has updated data that needs to be 
						// transmitted back to the client.
						object[] targetTable = new object[2];
						targetTable[0] = sourceTable.TableName;
						targetTable[1] = targetRows = new ArrayList();
						reconciledData.Add(targetTable);

					}

					// Deleted records are added as just the key elements.  Inserted and updated rows are added as the entire
					// record.  The RowState is passed back to the client to tell it whether to delete, insert or update the client
					// data model with this record.
					targetRows.Add(new object[2] {sourceRow.RowState, sourceRow.RowState == DataRowState.Deleted ?
													 DeletedRow(sourceRow) : sourceRow.ItemArray});

				}

			}

			// When this data structure is merged with the client data model, the server and client databases will be in synch.  A 
			// 'null' in the return data indicates that there is no data that is new since the last reconcilliation.
			parameters.Return.Value = reconciledData;

		}

		/// <summary>
		/// Creates a deleted row containing the key information and row version.
		/// </summary>
		/// <param name="sourceRow">The deleted row</param>
		/// <returns>An array of objects that contains the primary key and the row version.</returns>
		private static object[] DeletedRow(Row sourceRow)
		{

			// IMPORTANT CONCEPT:  The main idea here is to copy just the information needed to delete the row on the client.  
			// This is the key data and the row version and order columns.
			Table sourceTable = sourceRow.Table;
			object[] data = new object[sourceTable.Columns.Count];
			foreach (DataColumn dataColumn in sourceTable.Columns)
				data[dataColumn.Ordinal] = sourceRow[dataColumn, DataRowVersion.Original];
			return data;

		}

		/// <summary>
		/// Sets a user's masking tables.
		/// </summary>
		/// <param name="adoTransaction">A transaction used to </param>
		public static void SetMask(AdoTransaction adoTransaction)
		{

			try
			{

				ServerMarketData.maskLock.AcquireReaderLock(CommonTimeout.LockWait);

				TableLock tableLock = (TableLock)ServerMarketData.maskLocks[MarkThree.Guardian.Server.Environment.UserName];
				adoTransaction.LockRequests.AddWriterLock(tableLock);

			}
			finally
			{

				if (ServerMarketData.maskLock.IsReaderLockHeld) ServerMarketData.maskLock.ReleaseReaderLock();

			}

			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.maskLock);

		}

		public static void SetMask(ParameterList parameters)
		{

			string userName = MarkThree.Guardian.Server.Environment.UserName;

			DataSet sourceMask = parameters["maskSet"];

			DataSet destinationMask = (DataSet)ServerMarketData.masks[userName];

			if (destinationMask == null)
			{
				destinationMask = new DataSet(string.Format("{0} Mask", userName));
				ServerMarketData.masks[userName] = destinationMask;
			}

			foreach (DataTable sourceTable in sourceMask.Tables)
			{

				DataTable destinationTable = destinationMask.Tables[sourceTable.TableName];
				if (destinationTable == null)
					destinationTable = new DataTable(sourceTable.TableName);
				else
					destinationTable.Clear();

			}

			destinationMask.Merge(sourceMask);
			destinationMask.AcceptChanges();
		
		}

		public static void MergeMask(AdoTransaction adoTransaction)
		{

			try
			{

				ServerMarketData.maskLock.AcquireReaderLock(CommonTimeout.LockWait);

				TableLock tableLock = (TableLock)ServerMarketData.maskLocks[MarkThree.Guardian.Server.Environment.UserName];
				adoTransaction.LockRequests.AddWriterLock(tableLock);

			}
			finally
			{

				if (ServerMarketData.maskLock.IsReaderLockHeld) ServerMarketData.maskLock.ReleaseReaderLock();

			}

			adoTransaction.LockRequests.AddWriterLock(ServerMarketData.maskLock);

		}

		public static void MergeMask(ParameterList parameters)
		{

			string userName = MarkThree.Guardian.Server.Environment.UserName;

			DataSet sourceMask = parameters["maskSet"];

			DataSet destinationMask = (DataSet)ServerMarketData.masks[userName];

			if (destinationMask == null)
			{
				destinationMask = new DataSet(string.Format("{0} Mask", userName));
				ServerMarketData.masks[userName] = destinationMask;
			}

			destinationMask.Merge(sourceMask);
			destinationMask.AcceptChanges();
		
		}

	}

}
