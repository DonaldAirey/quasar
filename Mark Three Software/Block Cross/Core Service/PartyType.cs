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
    public class PartyType
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
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.PartyTypeLock);
        }
        
        /// <summary>Inserts a PartyType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            string description = parameters["description"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            string mnemonic = parameters["mnemonic"];
            int partyTypeCode = parameters["partyTypeCode"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            PartyType.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, mnemonic, partyTypeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Inserts a PartyType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        public static void Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, string description, object externalId0, object externalId1, string mnemonic, int partyTypeCode)
        {
            // Accessor for the PartyType Table.
            ServerMarketData.PartyTypeDataTable partyTypeTable = ServerMarketData.PartyType;
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
            ServerMarketData.PartyTypeRow partyTypeRow = partyTypeTable.NewPartyTypeRow();
            partyTypeRow[partyTypeTable.RowVersionColumn] = rowVersion;
            partyTypeRow[partyTypeTable.DescriptionColumn] = description;
            partyTypeRow[partyTypeTable.ExternalId0Column] = externalId0;
            partyTypeRow[partyTypeTable.ExternalId1Column] = externalId1;
            partyTypeRow[partyTypeTable.MnemonicColumn] = mnemonic;
            partyTypeRow[partyTypeTable.PartyTypeCodeColumn] = partyTypeCode;
            partyTypeTable.AddPartyTypeRow(partyTypeRow);
            adoTransaction.DataRows.Add(partyTypeRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"PartyType\" (\"rowVersion\",\"Description\",\"ExternalId0\",\"ExternalId1\",\"Mnemo" +
                    "nic\",\"PartyTypeCode\") values (@rowVersion,@description,@externalId0,@externalId1" +
                    ",@mnemonic,@partyTypeCode)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@partyTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, partyTypeCode));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.PartyTypeLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a PartyType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object mnemonic = parameters["mnemonic"].Value;
            int partyTypeCode = parameters["partyTypeCode"];
            // Call the internal method to complete the operation.
            PartyType.Update(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, mnemonic, partyTypeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a PartyType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object description, object externalId0, object externalId1, object mnemonic, int partyTypeCode)
        {
            // Accessor for the PartyType Table.
            ServerMarketData.PartyTypeDataTable partyTypeTable = ServerMarketData.PartyType;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.PartyTypeRow partyTypeRow = partyTypeTable.FindByPartyTypeCode(partyTypeCode);
            if ((partyTypeRow == null))
            {
                throw new Exception(string.Format("The PartyType table does not have an element identified by {0}", partyTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((partyTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((description == null))
            {
                description = partyTypeRow[partyTypeTable.DescriptionColumn];
            }
            if ((externalId0 == null))
            {
                externalId0 = partyTypeRow[partyTypeTable.ExternalId0Column];
            }
            if ((externalId1 == null))
            {
                externalId1 = partyTypeRow[partyTypeTable.ExternalId1Column];
            }
            if ((mnemonic == null))
            {
                mnemonic = partyTypeRow[partyTypeTable.MnemonicColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            partyTypeRow[partyTypeTable.RowVersionColumn] = rowVersion;
            partyTypeRow[partyTypeTable.DescriptionColumn] = description;
            partyTypeRow[partyTypeTable.ExternalId0Column] = externalId0;
            partyTypeRow[partyTypeTable.ExternalId1Column] = externalId1;
            partyTypeRow[partyTypeTable.MnemonicColumn] = mnemonic;
            adoTransaction.DataRows.Add(partyTypeRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"PartyType\" set \"RowVersion\"=@rowVersion,\"Description\"=@description,\"Exter" +
                    "nalId0\"=@externalId0,\"ExternalId1\"=@externalId1,\"Mnemonic\"=@mnemonic where \"Part" +
                    "yTypeCode\"=@partyTypeCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@partyTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, partyTypeCode));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.PartyTypeLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a PartyType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int partyTypeCode = parameters["partyTypeCode"];
            // Call the internal method to complete the operation.
            PartyType.Delete(adoTransaction, sqlTransaction, rowVersion, partyTypeCode);
        }
        
        /// <summary>Deletes a PartyType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int partyTypeCode)
        {
            // Accessor for the PartyType Table.
            ServerMarketData.PartyTypeDataTable partyTypeTable = ServerMarketData.PartyType;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.PartyTypeRow partyTypeRow = partyTypeTable.FindByPartyTypeCode(partyTypeCode);
            if ((partyTypeRow == null))
            {
                throw new Exception(string.Format("The PartyType table does not have an element identified by {0}", partyTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((partyTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < partyTypeRow.GetBlotterRows().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = partyTypeRow.GetBlotterRows()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            partyTypeRow[partyTypeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(partyTypeRow);
            partyTypeRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"PartyType\" set \"IsDeleted\" = 1 where \"PartyTypeCode\"=@partyTypeCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@partyTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, partyTypeCode));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.PartyTypeLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a PartyType record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int partyTypeCode = parameters["partyTypeCode"];
            // Call the internal method to complete the operation.
            PartyType.Archive(adoTransaction, sqlTransaction, rowVersion, partyTypeCode);
        }
        
        /// <summary>Archives a PartyType record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int partyTypeCode)
        {
            // Accessor for the PartyType Table.
            ServerMarketData.PartyTypeDataTable partyTypeTable = ServerMarketData.PartyType;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.PartyTypeRow partyTypeRow = partyTypeTable.FindByPartyTypeCode(partyTypeCode);
            if ((partyTypeRow == null))
            {
                throw new Exception(string.Format("The PartyType table does not have an element identified by {0}", partyTypeCode));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((partyTypeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < partyTypeRow.GetBlotterRows().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = partyTypeRow.GetBlotterRows()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            partyTypeRow[partyTypeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(partyTypeRow);
            partyTypeRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"PartyType\" set \"IsArchived\" = 1 where \"PartyTypeCode\"=@partyTypeCode");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@partyTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, partyTypeCode));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
