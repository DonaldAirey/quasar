//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarkThree.Guardian.Core
{
    using MarkThree.Guardian;
    using MarkThree.Guardian.Server;
    using MarkThree;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    
    
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    public class Holiday
    {
        
        /// This value is used to map the object to a persistent storage device.  The parameters for the storage
        /// are found in the configuration file for this service.
        public static string PersistentStore = "Guardian";
        
        /// This member provides access to the in-memory database.
        private static ServerMarketData serverMarketData = new ServerMarketData();
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Insert(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for this operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.HolidayLock);
        }
        
        /// <summary>Inserts a Holiday record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            int countryId = parameters["countryId"];
            System.DateTime date = parameters["date"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            int holidayTypeCode = parameters["holidayTypeCode"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int holidayId = Holiday.Insert(adoTransaction, sqlTransaction, ref rowVersion, countryId, date, externalId0, externalId1, holidayTypeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = holidayId;
        }
        
        /// <summary>Inserts a Holiday record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="countryId">The value for the CountryId column.</param>
        /// <param name="date">The value for the Date column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="holidayTypeCode">The value for the HolidayTypeCode column.</param>
        public static int Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int countryId, System.DateTime date, object externalId0, object externalId1, int holidayTypeCode)
        {
            // Accessor for the Holiday Table.
            ServerMarketData.HolidayDataTable holidayTable = ServerMarketData.Holiday;
            // Apply Defaults
            if ((externalId0 == null))
            {
                externalId0 = System.DBNull.Value;
            }
            if ((externalId1 == null))
            {
                externalId1 = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerMarketData.HolidayRow holidayRow = holidayTable.NewHolidayRow();
            holidayRow[holidayTable.RowVersionColumn] = rowVersion;
            holidayRow[holidayTable.CountryIdColumn] = countryId;
            holidayRow[holidayTable.DateColumn] = date;
            holidayRow[holidayTable.ExternalId0Column] = externalId0;
            holidayRow[holidayTable.ExternalId1Column] = externalId1;
            holidayRow[holidayTable.HolidayTypeCodeColumn] = holidayTypeCode;
            holidayTable.AddHolidayRow(holidayRow);
            adoTransaction.DataRows.Add(holidayRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Holiday\" (\"rowVersion\",\"CountryId\",\"Date\",\"ExternalId0\",\"ExternalId1\",\"Ho" +
                    "lidayId\",\"HolidayTypeCode\") values (@rowVersion,@countryId,@date,@externalId0,@e" +
                    "xternalId1,@holidayId,@holidayTypeCode)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@countryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, countryId));
            sqlCommand.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, date));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@holidayId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayRow[holidayTable.HolidayIdColumn]));
            sqlCommand.Parameters.Add(new SqlParameter("@holidayTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayTypeCode));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return holidayRow.HolidayId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.HolidayLock);
        }
        
        /// <summary>Inserts a Holiday record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object countryId = parameters["countryId"].Value;
            object date = parameters["date"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            int holidayId = parameters["holidayId"];
            object holidayTypeCode = parameters["holidayTypeCode"].Value;
            // Call the internal method to complete the operation.
            Holiday.Update(adoTransaction, sqlTransaction, ref rowVersion, countryId, date, externalId0, externalId1, holidayId, holidayTypeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Holiday record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="countryId">The value for the CountryId column.</param>
        /// <param name="date">The value for the Date column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="holidayId">The value for the HolidayId column.</param>
        /// <param name="holidayTypeCode">The value for the HolidayTypeCode column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object countryId, object date, object externalId0, object externalId1, int holidayId, object holidayTypeCode)
        {
            // Accessor for the Holiday Table.
            ServerMarketData.HolidayDataTable holidayTable = ServerMarketData.Holiday;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.HolidayRow holidayRow = holidayTable.FindByHolidayId(holidayId);
            if ((holidayRow == null))
            {
                throw new Exception(string.Format("The Holiday table does not have an element identified by {0}", holidayId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((holidayRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((countryId == null))
            {
                countryId = holidayRow[holidayTable.CountryIdColumn];
            }
            if ((date == null))
            {
                date = holidayRow[holidayTable.DateColumn];
            }
            if ((externalId0 == null))
            {
                externalId0 = holidayRow[holidayTable.ExternalId0Column];
            }
            if ((externalId1 == null))
            {
                externalId1 = holidayRow[holidayTable.ExternalId1Column];
            }
            if ((holidayTypeCode == null))
            {
                holidayTypeCode = holidayRow[holidayTable.HolidayTypeCodeColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            holidayRow[holidayTable.RowVersionColumn] = rowVersion;
            holidayRow[holidayTable.CountryIdColumn] = countryId;
            holidayRow[holidayTable.DateColumn] = date;
            holidayRow[holidayTable.ExternalId0Column] = externalId0;
            holidayRow[holidayTable.ExternalId1Column] = externalId1;
            holidayRow[holidayTable.HolidayTypeCodeColumn] = holidayTypeCode;
            adoTransaction.DataRows.Add(holidayRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Holiday\" set \"RowVersion\"=@rowVersion,\"CountryId\"=@countryId,\"Date\"=@date" +
                    ",\"ExternalId0\"=@externalId0,\"ExternalId1\"=@externalId1,\"HolidayTypeCode\"=@holida" +
                    "yTypeCode where \"HolidayId\"=@holidayId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@countryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, countryId));
            sqlCommand.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, date));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@holidayId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayId));
            sqlCommand.Parameters.Add(new SqlParameter("@holidayTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayTypeCode));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.HolidayLock);
        }
        
        /// <summary>Inserts a Holiday record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int holidayId = parameters["holidayId"];
            // Call the internal method to complete the operation.
            Holiday.Delete(adoTransaction, sqlTransaction, rowVersion, holidayId);
        }
        
        /// <summary>Deletes a Holiday record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="holidayId">The value for the HolidayId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int holidayId)
        {
            // Accessor for the Holiday Table.
            ServerMarketData.HolidayDataTable holidayTable = ServerMarketData.Holiday;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.HolidayRow holidayRow = holidayTable.FindByHolidayId(holidayId);
            if ((holidayRow == null))
            {
                throw new Exception(string.Format("The Holiday table does not have an element identified by {0}", holidayId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((holidayRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            holidayRow[holidayTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(holidayRow);
            holidayRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Holiday\" set \"IsDeleted\" = 1 where \"HolidayId\"=@holidayId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@holidayId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.HolidayLock);
        }
        
        /// <summary>Inserts a Holiday record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int holidayId = parameters["holidayId"];
            // Call the internal method to complete the operation.
            Holiday.Archive(adoTransaction, sqlTransaction, rowVersion, holidayId);
        }
        
        /// <summary>Archives a Holiday record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="holidayId">The value for the HolidayId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int holidayId)
        {
            // Accessor for the Holiday Table.
            ServerMarketData.HolidayDataTable holidayTable = ServerMarketData.Holiday;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.HolidayRow holidayRow = holidayTable.FindByHolidayId(holidayId);
            if ((holidayRow == null))
            {
                throw new Exception(string.Format("The Holiday table does not have an element identified by {0}", holidayId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((holidayRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            holidayRow[holidayTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(holidayRow);
            holidayRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Holiday\" set \"IsArchived\" = 1 where \"HolidayId\"=@holidayId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@holidayId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, holidayId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
