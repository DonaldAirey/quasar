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
    public class VolumeCategory
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
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.VolumeCategoryLock);
        }
        
        /// <summary>Inserts a VolumeCategory record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object highVolumeRange = parameters["highVolumeRange"].Value;
            decimal lowVolumeRange = parameters["lowVolumeRange"];
            string mnemonic = parameters["mnemonic"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int volumeCategoryId = VolumeCategory.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, highVolumeRange, lowVolumeRange, mnemonic);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = volumeCategoryId;
        }
        
        /// <summary>Inserts a VolumeCategory record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="highVolumeRange">The value for the HighVolumeRange column.</param>
        /// <param name="lowVolumeRange">The value for the LowVolumeRange column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        public static int Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object description, object externalId0, object highVolumeRange, decimal lowVolumeRange, string mnemonic)
        {
            // Accessor for the VolumeCategory Table.
            ServerMarketData.VolumeCategoryDataTable volumeCategoryTable = ServerMarketData.VolumeCategory;
            // Apply Defaults
            if ((description == null))
            {
                description = System.DBNull.Value;
            }
            if ((externalId0 == null))
            {
                externalId0 = System.DBNull.Value;
            }
            if ((highVolumeRange == null))
            {
                highVolumeRange = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerMarketData.VolumeCategoryRow volumeCategoryRow = volumeCategoryTable.NewVolumeCategoryRow();
            volumeCategoryRow[volumeCategoryTable.RowVersionColumn] = rowVersion;
            volumeCategoryRow[volumeCategoryTable.DescriptionColumn] = description;
            volumeCategoryRow[volumeCategoryTable.ExternalId0Column] = externalId0;
            volumeCategoryRow[volumeCategoryTable.HighVolumeRangeColumn] = highVolumeRange;
            volumeCategoryRow[volumeCategoryTable.LowVolumeRangeColumn] = lowVolumeRange;
            volumeCategoryRow[volumeCategoryTable.MnemonicColumn] = mnemonic;
            volumeCategoryTable.AddVolumeCategoryRow(volumeCategoryRow);
            adoTransaction.DataRows.Add(volumeCategoryRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"VolumeCategory\" (\"rowVersion\",\"Description\",\"ExternalId0\",\"HighVolumeRang" +
                    "e\",\"LowVolumeRange\",\"Mnemonic\",\"VolumeCategoryId\") values (@rowVersion,@descript" +
                    "ion,@externalId0,@highVolumeRange,@lowVolumeRange,@mnemonic,@volumeCategoryId)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@highVolumeRange", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, highVolumeRange));
            sqlCommand.Parameters.Add(new SqlParameter("@lowVolumeRange", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, lowVolumeRange));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@volumeCategoryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, volumeCategoryRow[volumeCategoryTable.VolumeCategoryIdColumn]));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return volumeCategoryRow.VolumeCategoryId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.VolumeCategoryLock);
            Security.ArchiveChildren(adoTransaction);
            TraderVolumeSetting.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a VolumeCategory record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object highVolumeRange = parameters["highVolumeRange"].Value;
            object lowVolumeRange = parameters["lowVolumeRange"].Value;
            object mnemonic = parameters["mnemonic"].Value;
            int volumeCategoryId = parameters["volumeCategoryId"];
            // Call the internal method to complete the operation.
            VolumeCategory.Update(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, highVolumeRange, lowVolumeRange, mnemonic, volumeCategoryId);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a VolumeCategory record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="highVolumeRange">The value for the HighVolumeRange column.</param>
        /// <param name="lowVolumeRange">The value for the LowVolumeRange column.</param>
        /// <param name="mnemonic">The value for the Mnemonic column.</param>
        /// <param name="volumeCategoryId">The value for the VolumeCategoryId column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object description, object externalId0, object highVolumeRange, object lowVolumeRange, object mnemonic, int volumeCategoryId)
        {
            // Accessor for the VolumeCategory Table.
            ServerMarketData.VolumeCategoryDataTable volumeCategoryTable = ServerMarketData.VolumeCategory;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.VolumeCategoryRow volumeCategoryRow = volumeCategoryTable.FindByVolumeCategoryId(volumeCategoryId);
            if ((volumeCategoryRow == null))
            {
                throw new Exception(string.Format("The VolumeCategory table does not have an element identified by {0}", volumeCategoryId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((volumeCategoryRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((description == null))
            {
                description = volumeCategoryRow[volumeCategoryTable.DescriptionColumn];
            }
            if ((externalId0 == null))
            {
                externalId0 = volumeCategoryRow[volumeCategoryTable.ExternalId0Column];
            }
            if ((highVolumeRange == null))
            {
                highVolumeRange = volumeCategoryRow[volumeCategoryTable.HighVolumeRangeColumn];
            }
            if ((lowVolumeRange == null))
            {
                lowVolumeRange = volumeCategoryRow[volumeCategoryTable.LowVolumeRangeColumn];
            }
            if ((mnemonic == null))
            {
                mnemonic = volumeCategoryRow[volumeCategoryTable.MnemonicColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            volumeCategoryRow[volumeCategoryTable.RowVersionColumn] = rowVersion;
            volumeCategoryRow[volumeCategoryTable.DescriptionColumn] = description;
            volumeCategoryRow[volumeCategoryTable.ExternalId0Column] = externalId0;
            volumeCategoryRow[volumeCategoryTable.HighVolumeRangeColumn] = highVolumeRange;
            volumeCategoryRow[volumeCategoryTable.LowVolumeRangeColumn] = lowVolumeRange;
            volumeCategoryRow[volumeCategoryTable.MnemonicColumn] = mnemonic;
            adoTransaction.DataRows.Add(volumeCategoryRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"VolumeCategory\" set \"RowVersion\"=@rowVersion,\"Description\"=@description,\"" +
                    "ExternalId0\"=@externalId0,\"HighVolumeRange\"=@highVolumeRange,\"LowVolumeRange\"=@l" +
                    "owVolumeRange,\"Mnemonic\"=@mnemonic where \"VolumeCategoryId\"=@volumeCategoryId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, description));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@highVolumeRange", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, highVolumeRange));
            sqlCommand.Parameters.Add(new SqlParameter("@lowVolumeRange", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, lowVolumeRange));
            sqlCommand.Parameters.Add(new SqlParameter("@mnemonic", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mnemonic));
            sqlCommand.Parameters.Add(new SqlParameter("@volumeCategoryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, volumeCategoryId));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.VolumeCategoryLock);
            Security.ArchiveChildren(adoTransaction);
            TraderVolumeSetting.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a VolumeCategory record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int volumeCategoryId = parameters["volumeCategoryId"];
            // Call the internal method to complete the operation.
            VolumeCategory.Delete(adoTransaction, sqlTransaction, rowVersion, volumeCategoryId);
        }
        
        /// <summary>Deletes a VolumeCategory record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="volumeCategoryId">The value for the VolumeCategoryId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int volumeCategoryId)
        {
            // Accessor for the VolumeCategory Table.
            ServerMarketData.VolumeCategoryDataTable volumeCategoryTable = ServerMarketData.VolumeCategory;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.VolumeCategoryRow volumeCategoryRow = volumeCategoryTable.FindByVolumeCategoryId(volumeCategoryId);
            if ((volumeCategoryRow == null))
            {
                throw new Exception(string.Format("The VolumeCategory table does not have an element identified by {0}", volumeCategoryId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((volumeCategoryRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < volumeCategoryRow.GetSecurityRows().Length); index = (index + 1))
            {
                ServerMarketData.SecurityRow childSecurityRow = volumeCategoryRow.GetSecurityRows()[index];
                Security.DeleteChildren(adoTransaction, sqlTransaction, childSecurityRow.RowVersion, childSecurityRow.SecurityId);
            }
            for (int index = 0; (index < volumeCategoryRow.GetTraderVolumeSettingRows().Length); index = (index + 1))
            {
                ServerMarketData.TraderVolumeSettingRow childTraderVolumeSettingRow = volumeCategoryRow.GetTraderVolumeSettingRows()[index];
                TraderVolumeSetting.Delete(adoTransaction, sqlTransaction, childTraderVolumeSettingRow.RowVersion, childTraderVolumeSettingRow.TraderVolumeSettingId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            volumeCategoryRow[volumeCategoryTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(volumeCategoryRow);
            volumeCategoryRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"VolumeCategory\" set \"IsDeleted\" = 1 where \"VolumeCategoryId\"=@volumeCateg" +
                    "oryId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@volumeCategoryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, volumeCategoryId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.VolumeCategoryLock);
            Security.ArchiveChildren(adoTransaction);
            TraderVolumeSetting.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a VolumeCategory record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int volumeCategoryId = parameters["volumeCategoryId"];
            // Call the internal method to complete the operation.
            VolumeCategory.Archive(adoTransaction, sqlTransaction, rowVersion, volumeCategoryId);
        }
        
        /// <summary>Archives a VolumeCategory record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="volumeCategoryId">The value for the VolumeCategoryId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int volumeCategoryId)
        {
            // Accessor for the VolumeCategory Table.
            ServerMarketData.VolumeCategoryDataTable volumeCategoryTable = ServerMarketData.VolumeCategory;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.VolumeCategoryRow volumeCategoryRow = volumeCategoryTable.FindByVolumeCategoryId(volumeCategoryId);
            if ((volumeCategoryRow == null))
            {
                throw new Exception(string.Format("The VolumeCategory table does not have an element identified by {0}", volumeCategoryId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((volumeCategoryRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < volumeCategoryRow.GetSecurityRows().Length); index = (index + 1))
            {
                ServerMarketData.SecurityRow childSecurityRow = volumeCategoryRow.GetSecurityRows()[index];
                Security.ArchiveChildren(adoTransaction, sqlTransaction, childSecurityRow.RowVersion, childSecurityRow.SecurityId);
            }
            for (int index = 0; (index < volumeCategoryRow.GetTraderVolumeSettingRows().Length); index = (index + 1))
            {
                ServerMarketData.TraderVolumeSettingRow childTraderVolumeSettingRow = volumeCategoryRow.GetTraderVolumeSettingRows()[index];
                TraderVolumeSetting.Archive(adoTransaction, sqlTransaction, childTraderVolumeSettingRow.RowVersion, childTraderVolumeSettingRow.TraderVolumeSettingId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            volumeCategoryRow[volumeCategoryTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(volumeCategoryRow);
            volumeCategoryRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"VolumeCategory\" set \"IsArchived\" = 1 where \"VolumeCategoryId\"=@volumeCate" +
                    "goryId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@volumeCategoryId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, volumeCategoryId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
