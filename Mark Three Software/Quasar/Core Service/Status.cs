//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarkThree.Quasar.Core
{
    using MarkThree.Quasar;
    using MarkThree.Quasar.Server;
    using MarkThree;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    
    
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    public class Status
    {
        
        /// This value is used to map the object to a persistent storage device.  The parameters for the storage
        /// are found in the configuration file for this service.
        public static string PersistentStore = "Quasar";
        
        /// This member provides access to the in-memory database.
        private static ServerDataModel serverDataModel = new ServerDataModel();
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Insert(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for this operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.Status));
        }
        
        /// <summary>Inserts a Status record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            int statusCode = parameters["statusCode"];
            string mnemonic = parameters["mnemonic"];
            string description = parameters["description"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object externalId2 = parameters["externalId2"].Value;
            object externalId3 = parameters["externalId3"].Value;
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            Status.Insert(adoTransaction, sqlTransaction, ref rowVersion, statusCode, mnemonic, description, externalId0, externalId1, externalId2, externalId3);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Inserts a Status record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="externalId2">The value for the ExternalId2 column.</param>
        /// <param name="externalId3">The value for the ExternalId3 column.</param>
        public static void Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int statusCode, string mnemonic, string description, object externalId0, object externalId1, object externalId2, object externalId3)
        {
            // Accessor for the Status Table.
            ServerDataModel.StatusDataTable statusTable = ServerDataModel.Status;
            // Apply Defaults
            if ((externalId0 == null))
            {
                externalId0 = System.DBNull.Value;
            }
            if ((externalId1 == null))
            {
                externalId1 = System.DBNull.Value;
            }
            if ((externalId2 == null))
            {
                externalId2 = System.DBNull.Value;
            }
            if ((externalId3 == null))
            {
                externalId3 = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.StatusRow statusRow = statusTable.NewStatusRow();
            statusRow[statusTable.RowVersionColumn] = rowVersion;
            statusRow[statusTable.StatusCodeColumn] = statusCode;
            statusRow[statusTable.MnemonicColumn] = mnemonic;
            statusRow[statusTable.DescriptionColumn] = description;
            statusRow[statusTable.ExternalId0Column] = externalId0;
            statusRow[statusTable.ExternalId1Column] = externalId1;
            statusRow[statusTable.ExternalId2Column] = externalId2;
            statusRow[statusTable.ExternalId3Column] = externalId3;
            statusTable.AddStatusRow(statusRow);
            adoTransaction.DataRows.Add(statusRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Status\" (\"rowVersion\",\"StatusCode\",\"Mnemonic\",\"Description\",\"ExternalId0\"" +
                    ",\"ExternalId1\",\"ExternalId2\",\"ExternalId3\") values (@rowVersion,@statusCode,@mne" +
                    "monic,@description,@externalId0,@externalId1,@externalId2,@externalId3)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId2", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId2));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId3", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId3));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.Status));
            BlockOrder.Update(adoTransaction);
        }
        
        /// <summary>Inserts a Status record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int statusCode = parameters["statusCode"];
            object mnemonic = parameters["mnemonic"].Value;
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object externalId2 = parameters["externalId2"].Value;
            object externalId3 = parameters["externalId3"].Value;
            // Call the internal method to complete the operation.
            Status.Update(adoTransaction, sqlTransaction, ref rowVersion, statusCode, mnemonic, description, externalId0, externalId1, externalId2, externalId3);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Status record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="externalId2">The value for the ExternalId2 column.</param>
        /// <param name="externalId3">The value for the ExternalId3 column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int statusCode, object mnemonic, object description, object externalId0, object externalId1, object externalId2, object externalId3)
        {
            // Accessor for the Status Table.
            ServerDataModel.StatusDataTable statusTable = ServerDataModel.Status;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.StatusRow statusRow = statusTable.FindByStatusCode(statusCode);
            if ((statusRow == null))
            {
                throw new Exception(string.Format("The Status table does not have an element identified by {0}", statusCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((statusRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((mnemonic == null))
            {
                mnemonic = statusRow[statusTable.MnemonicColumn];
            }
            if ((description == null))
            {
                description = statusRow[statusTable.DescriptionColumn];
            }
            if ((externalId0 == null))
            {
                externalId0 = statusRow[statusTable.ExternalId0Column];
            }
            if ((externalId1 == null))
            {
                externalId1 = statusRow[statusTable.ExternalId1Column];
            }
            if ((externalId2 == null))
            {
                externalId2 = statusRow[statusTable.ExternalId2Column];
            }
            if ((externalId3 == null))
            {
                externalId3 = statusRow[statusTable.ExternalId3Column];
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Update the record in the ADO database.
            statusRow[statusTable.RowVersionColumn] = rowVersion;
            statusRow[statusTable.MnemonicColumn] = mnemonic;
            statusRow[statusTable.DescriptionColumn] = description;
            statusRow[statusTable.ExternalId0Column] = externalId0;
            statusRow[statusTable.ExternalId1Column] = externalId1;
            statusRow[statusTable.ExternalId2Column] = externalId2;
            statusRow[statusTable.ExternalId3Column] = externalId3;
            adoTransaction.DataRows.Add(statusRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Status\" set \"RowVersion\"=@rowVersion,\"Mnemonic\"=@mnemonic,\"Description\"=@" +
                    "description,\"ExternalId0\"=@externalId0,\"ExternalId1\"=@externalId1,\"ExternalId2\"=" +
                    "@externalId2,\"ExternalId3\"=@externalId3 where \"StatusCode\"=@statusCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId2", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId2));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId3", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId3));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.Status));
            BlockOrder.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a Status record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int statusCode = parameters["statusCode"];
            // Call the internal method to complete the operation.
            Status.Delete(adoTransaction, sqlTransaction, rowVersion, statusCode);
        }
        
        /// <summary>Deletes a Status record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int statusCode)
        {
            // Accessor for the Status Table.
            ServerDataModel.StatusDataTable statusTable = ServerDataModel.Status;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.StatusRow statusRow = statusTable.FindByStatusCode(statusCode);
            if ((statusRow == null))
            {
                throw new Exception(string.Format("The Status table does not have an element identified by {0}", statusCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((statusRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < statusRow.GetBlockOrderRows().Length); index = (index + 1))
            {
                ServerDataModel.BlockOrderRow childBlockOrderRow = statusRow.GetBlockOrderRows()[index];
                BlockOrder.Delete(adoTransaction, sqlTransaction, childBlockOrderRow.RowVersion, childBlockOrderRow.BlockOrderId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            statusRow[statusTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(statusRow);
            statusRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Status\" set \"IsDeleted\" = 1 where \"StatusCode\"=@statusCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.Status));
            BlockOrder.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a Status record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int statusCode = parameters["statusCode"];
            // Call the internal method to complete the operation.
            Status.Archive(adoTransaction, sqlTransaction, rowVersion, statusCode);
        }
        
        /// <summary>Archives a Status record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="statusCode">The value for the StatusCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int statusCode)
        {
            // Accessor for the Status Table.
            ServerDataModel.StatusDataTable statusTable = ServerDataModel.Status;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.StatusRow statusRow = statusTable.FindByStatusCode(statusCode);
            if ((statusRow == null))
            {
                throw new Exception(string.Format("The Status table does not have an element identified by {0}", statusCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((statusRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < statusRow.GetBlockOrderRows().Length); index = (index + 1))
            {
                ServerDataModel.BlockOrderRow childBlockOrderRow = statusRow.GetBlockOrderRows()[index];
                BlockOrder.Archive(adoTransaction, sqlTransaction, childBlockOrderRow.RowVersion, childBlockOrderRow.BlockOrderId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            statusRow[statusTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(statusRow);
            statusRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Status\" set \"IsArchived\" = 1 where \"StatusCode\"=@statusCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@statusCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, statusCode));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
