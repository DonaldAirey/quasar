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
    public class AccountType
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
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.AccountType));
        }
        
        /// <summary>Inserts a AccountType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            int accountTypeCode = parameters["accountTypeCode"];
            string description = parameters["description"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            AccountType.Insert(adoTransaction, sqlTransaction, ref rowVersion, accountTypeCode, description, externalId0, externalId1);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Inserts a AccountType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="accountTypeCode">The value for the AccountTypeCode column.</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        public static void Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int accountTypeCode, string description, object externalId0, object externalId1)
        {
            // Accessor for the AccountType Table.
            ServerDataModel.AccountTypeDataTable accountTypeTable = ServerDataModel.AccountType;
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
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.AccountTypeRow accountTypeRow = accountTypeTable.NewAccountTypeRow();
            accountTypeRow[accountTypeTable.RowVersionColumn] = rowVersion;
            accountTypeRow[accountTypeTable.AccountTypeCodeColumn] = accountTypeCode;
            accountTypeRow[accountTypeTable.DescriptionColumn] = description;
            accountTypeRow[accountTypeTable.ExternalId0Column] = externalId0;
            accountTypeRow[accountTypeTable.ExternalId1Column] = externalId1;
            accountTypeTable.AddAccountTypeRow(accountTypeRow);
            adoTransaction.DataRows.Add(accountTypeRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"AccountType\" (\"rowVersion\",\"AccountTypeCode\",\"Description\",\"ExternalId0\"," +
                    "\"ExternalId1\") values (@rowVersion,@accountTypeCode,@description,@externalId0,@e" +
                    "xternalId1)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@accountTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.AccountType));
            Account.UpdateChildren(adoTransaction);
        }
        
        /// <summary>Inserts a AccountType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int accountTypeCode = parameters["accountTypeCode"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            // Call the internal method to complete the operation.
            AccountType.Update(adoTransaction, sqlTransaction, ref rowVersion, accountTypeCode, description, externalId0, externalId1);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a AccountType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="accountTypeCode">The value for the AccountTypeCode column.</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int accountTypeCode, object description, object externalId0, object externalId1)
        {
            // Accessor for the AccountType Table.
            ServerDataModel.AccountTypeDataTable accountTypeTable = ServerDataModel.AccountType;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.AccountTypeRow accountTypeRow = accountTypeTable.FindByAccountTypeCode(accountTypeCode);
            if ((accountTypeRow == null))
            {
                throw new Exception(string.Format("The AccountType table does not have an element identified by {0}", accountTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((accountTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((description == null))
            {
                description = accountTypeRow[accountTypeTable.DescriptionColumn];
            }
            if ((externalId0 == null))
            {
                externalId0 = accountTypeRow[accountTypeTable.ExternalId0Column];
            }
            if ((externalId1 == null))
            {
                externalId1 = accountTypeRow[accountTypeTable.ExternalId1Column];
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Update the record in the ADO database.
            accountTypeRow[accountTypeTable.RowVersionColumn] = rowVersion;
            accountTypeRow[accountTypeTable.DescriptionColumn] = description;
            accountTypeRow[accountTypeTable.ExternalId0Column] = externalId0;
            accountTypeRow[accountTypeTable.ExternalId1Column] = externalId1;
            adoTransaction.DataRows.Add(accountTypeRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"AccountType\" set \"RowVersion\"=@rowVersion,\"Description\"=@description,\"Ext" +
                    "ernalId0\"=@externalId0,\"ExternalId1\"=@externalId1 where \"AccountTypeCode\"=@accou" +
                    "ntTypeCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@accountTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.AccountType));
            Account.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a AccountType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int accountTypeCode = parameters["accountTypeCode"];
            // Call the internal method to complete the operation.
            AccountType.Delete(adoTransaction, sqlTransaction, rowVersion, accountTypeCode);
        }
        
        /// <summary>Deletes a AccountType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="accountTypeCode">The value for the AccountTypeCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int accountTypeCode)
        {
            // Accessor for the AccountType Table.
            ServerDataModel.AccountTypeDataTable accountTypeTable = ServerDataModel.AccountType;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.AccountTypeRow accountTypeRow = accountTypeTable.FindByAccountTypeCode(accountTypeCode);
            if ((accountTypeRow == null))
            {
                throw new Exception(string.Format("The AccountType table does not have an element identified by {0}", accountTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((accountTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < accountTypeRow.GetAccountRows().Length); index = (index + 1))
            {
                ServerDataModel.AccountRow childAccountRow = accountTypeRow.GetAccountRows()[index];
                Account.DeleteChildren(adoTransaction, sqlTransaction, childAccountRow.RowVersion, childAccountRow.AccountId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            accountTypeRow[accountTypeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(accountTypeRow);
            accountTypeRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"AccountType\" set \"IsDeleted\" = 1 where \"AccountTypeCode\"=@accountTypeCode" +
                    "");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@accountTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountTypeCode));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.AccountType));
            Account.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a AccountType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int accountTypeCode = parameters["accountTypeCode"];
            // Call the internal method to complete the operation.
            AccountType.Archive(adoTransaction, sqlTransaction, rowVersion, accountTypeCode);
        }
        
        /// <summary>Archives a AccountType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="accountTypeCode">The value for the AccountTypeCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int accountTypeCode)
        {
            // Accessor for the AccountType Table.
            ServerDataModel.AccountTypeDataTable accountTypeTable = ServerDataModel.AccountType;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.AccountTypeRow accountTypeRow = accountTypeTable.FindByAccountTypeCode(accountTypeCode);
            if ((accountTypeRow == null))
            {
                throw new Exception(string.Format("The AccountType table does not have an element identified by {0}", accountTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((accountTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < accountTypeRow.GetAccountRows().Length); index = (index + 1))
            {
                ServerDataModel.AccountRow childAccountRow = accountTypeRow.GetAccountRows()[index];
                Account.ArchiveChildren(adoTransaction, sqlTransaction, childAccountRow.RowVersion, childAccountRow.AccountId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            accountTypeRow[accountTypeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(accountTypeRow);
            accountTypeRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"AccountType\" set \"IsArchived\" = 1 where \"AccountTypeCode\"=@accountTypeCod" +
                    "e");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@accountTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountTypeCode));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
