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
    public class Match
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
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
        }
        
        /// <summary>Inserts a Match record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            int contraOrderId = parameters["contraOrderId"];
            System.DateTime createdTime = parameters["createdTime"];
            object timerId = parameters["timerId"].Value;
            int statusCode = parameters["statusCode"];
            int workingOrderId = parameters["workingOrderId"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int matchId = Match.Insert(adoTransaction, sqlTransaction, ref rowVersion, contraOrderId, createdTime, timerId, statusCode, workingOrderId);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = matchId;
        }
        
        /// <summary>Inserts a Match record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="contraOrderId">The value for the ContraOrderId column.</param>
        /// <param name="createdTime">The value for the CreatedTime column.</param>
        /// <param name="timerId">The value for the TimerId column.</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="workingOrderId">The value for the WorkingOrderId column.</param>
        public static int Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int contraOrderId, System.DateTime createdTime, object timerId, int statusCode, int workingOrderId)
        {
            // Accessor for the Match Table.
            ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;
            // Apply Defaults
            if ((timerId == null))
            {
                timerId = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerMarketData.MatchRow matchRow = matchTable.NewMatchRow();
            matchRow[matchTable.RowVersionColumn] = rowVersion;
            matchRow[matchTable.ContraOrderIdColumn] = contraOrderId;
            matchRow[matchTable.CreatedTimeColumn] = createdTime;
            matchRow[matchTable.TimerIdColumn] = timerId;
            matchRow[matchTable.StatusCodeColumn] = statusCode;
            matchRow[matchTable.WorkingOrderIdColumn] = workingOrderId;
            matchTable.AddMatchRow(matchRow);
            adoTransaction.DataRows.Add(matchRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Match\" (\"rowVersion\",\"ContraOrderId\",\"CreatedTime\",\"MatchId\",\"TimerId\",\"S" +
                    "tatusCode\",\"WorkingOrderId\") values (@rowVersion,@contraOrderId,@createdTime,@ma" +
                    "tchId,@timerId,@statusCode,@workingOrderId)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@contraOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, contraOrderId));
            sqlCommand.Parameters.Add(new SqlParameter("@createdTime", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, createdTime));
            sqlCommand.Parameters.Add(new SqlParameter("@matchId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, matchRow[matchTable.MatchIdColumn]));
            sqlCommand.Parameters.Add(new SqlParameter("@timerId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, timerId));
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.Parameters.Add(new SqlParameter("@workingOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, workingOrderId));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return matchRow.MatchId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
            Negotiation.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a Match record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object contraOrderId = parameters["contraOrderId"].Value;
            object createdTime = parameters["createdTime"].Value;
            int matchId = parameters["matchId"];
            object timerId = parameters["timerId"].Value;
            object statusCode = parameters["statusCode"].Value;
            object workingOrderId = parameters["workingOrderId"].Value;
            // Call the internal method to complete the operation.
            Match.Update(adoTransaction, sqlTransaction, ref rowVersion, contraOrderId, createdTime, matchId, timerId, statusCode, workingOrderId);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Match record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="contraOrderId">The value for the ContraOrderId column.</param>
        /// <param name="createdTime">The value for the CreatedTime column.</param>
        /// <param name="matchId">The value for the MatchId column.</param>
        /// <param name="timerId">The value for the TimerId column.</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="workingOrderId">The value for the WorkingOrderId column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object contraOrderId, object createdTime, int matchId, object timerId, object statusCode, object workingOrderId)
        {
            // Accessor for the Match Table.
            ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
            if ((matchRow == null))
            {
                throw new Exception(string.Format("The Match table does not have an element identified by {0}", matchId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((matchRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((contraOrderId == null))
            {
                contraOrderId = matchRow[matchTable.ContraOrderIdColumn];
            }
            if ((createdTime == null))
            {
                createdTime = matchRow[matchTable.CreatedTimeColumn];
            }
            if ((timerId == null))
            {
                timerId = matchRow[matchTable.TimerIdColumn];
            }
            if ((statusCode == null))
            {
                statusCode = matchRow[matchTable.StatusCodeColumn];
            }
            if ((workingOrderId == null))
            {
                workingOrderId = matchRow[matchTable.WorkingOrderIdColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            matchRow[matchTable.RowVersionColumn] = rowVersion;
            matchRow[matchTable.ContraOrderIdColumn] = contraOrderId;
            matchRow[matchTable.CreatedTimeColumn] = createdTime;
            matchRow[matchTable.TimerIdColumn] = timerId;
            matchRow[matchTable.StatusCodeColumn] = statusCode;
            matchRow[matchTable.WorkingOrderIdColumn] = workingOrderId;
            adoTransaction.DataRows.Add(matchRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Match\" set \"RowVersion\"=@rowVersion,\"ContraOrderId\"=@contraOrderId,\"Creat" +
                    "edTime\"=@createdTime,\"TimerId\"=@timerId,\"StatusCode\"=@statusCode,\"WorkingOrderId" +
                    "\"=@workingOrderId where \"MatchId\"=@matchId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@contraOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, contraOrderId));
            sqlCommand.Parameters.Add(new SqlParameter("@createdTime", SqlDbType.DateTime, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, createdTime));
            sqlCommand.Parameters.Add(new SqlParameter("@matchId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, matchId));
            sqlCommand.Parameters.Add(new SqlParameter("@timerId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, timerId));
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.Parameters.Add(new SqlParameter("@workingOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, workingOrderId));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
            Negotiation.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a Match record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int matchId = parameters["matchId"];
            // Call the internal method to complete the operation.
            Match.Delete(adoTransaction, sqlTransaction, rowVersion, matchId);
        }
        
        /// <summary>Deletes a Match record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="matchId">The value for the MatchId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int matchId)
        {
            // Accessor for the Match Table.
            ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
            if ((matchRow == null))
            {
                throw new Exception(string.Format("The Match table does not have an element identified by {0}", matchId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((matchRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < matchRow.GetNegotiationRows().Length); index = (index + 1))
            {
                ServerMarketData.NegotiationRow childNegotiationRow = matchRow.GetNegotiationRows()[index];
                Negotiation.Delete(adoTransaction, sqlTransaction, childNegotiationRow.RowVersion, childNegotiationRow.NegotiationId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            matchRow[matchTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(matchRow);
            matchRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Match\" set \"IsDeleted\" = 1 where \"MatchId\"=@matchId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@matchId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, matchId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.MatchLock);
            Negotiation.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a Match record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int matchId = parameters["matchId"];
            // Call the internal method to complete the operation.
            Match.Archive(adoTransaction, sqlTransaction, rowVersion, matchId);
        }
        
        /// <summary>Archives a Match record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="matchId">The value for the MatchId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int matchId)
        {
            // Accessor for the Match Table.
            ServerMarketData.MatchDataTable matchTable = ServerMarketData.Match;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.MatchRow matchRow = matchTable.FindByMatchId(matchId);
            if ((matchRow == null))
            {
                throw new Exception(string.Format("The Match table does not have an element identified by {0}", matchId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((matchRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < matchRow.GetNegotiationRows().Length); index = (index + 1))
            {
                ServerMarketData.NegotiationRow childNegotiationRow = matchRow.GetNegotiationRows()[index];
                Negotiation.Archive(adoTransaction, sqlTransaction, childNegotiationRow.RowVersion, childNegotiationRow.NegotiationId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            matchRow[matchTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(matchRow);
            matchRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Match\" set \"IsArchived\" = 1 where \"MatchId\"=@matchId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@matchId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, matchId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
