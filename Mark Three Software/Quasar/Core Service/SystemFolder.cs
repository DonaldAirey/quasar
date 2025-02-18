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
    public class SystemFolder : MarkThree.Quasar.Core.Folder
    {
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Insert(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Folder.Insert(adoTransaction);
            // These table lock(s) are required for the 'Insert' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.SystemFolder));
        }
        
        /// <summary>Inserts a SystemFolder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object externalId2 = parameters["externalId2"].Value;
            object externalId3 = parameters["externalId3"].Value;
            object externalId4 = parameters["externalId4"].Value;
            object externalId5 = parameters["externalId5"].Value;
            object externalId6 = parameters["externalId6"].Value;
            object externalId7 = parameters["externalId7"].Value;
            object groupPermission = parameters["groupPermission"].Value;
            object hidden = parameters["hidden"].Value;
            string name = parameters["name"];
            object owner = parameters["owner"].Value;
            object ownerPermission = parameters["ownerPermission"].Value;
            object readOnly = parameters["readOnly"].Value;
            object worldPermission = parameters["worldPermission"].Value;
            object typeCode = parameters["typeCode"].Value;
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int systemFolderId = SystemFolder.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, typeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = systemFolderId;
        }
        
        /// <summary>Inserts a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row.</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="externalId2">The value for the ExternalId2 column.</param>
        /// <param name="externalId3">The value for the ExternalId3 column.</param>
        /// <param name="externalId4">The value for the ExternalId4 column.</param>
        /// <param name="externalId5">The value for the ExternalId5 column.</param>
        /// <param name="externalId6">The value for the ExternalId6 column.</param>
        /// <param name="externalId7">The value for the ExternalId7 column.</param>
        /// <param name="groupPermission">The value for the GroupPermission column.</param>
        /// <param name="hidden">The value for the Hidden column.</param>
        /// <param name="name">The value for the Name column.</param>
        /// <param name="owner">The value for the Owner column.</param>
        /// <param name="ownerPermission">The value for the OwnerPermission column.</param>
        /// <param name="readOnly">The value for the ReadOnly column.</param>
        /// <param name="worldPermission">The value for the WorldPermission column.</param>
        /// <param name="typeCode">The value for the TypeCode column.</param>
        public new static int Insert(
                    AdoTransaction adoTransaction, 
                    SqlTransaction sqlTransaction, 
                    ref long rowVersion, 
                    object description, 
                    object externalId0, 
                    object externalId1, 
                    object externalId2, 
                    object externalId3, 
                    object externalId4, 
                    object externalId5, 
                    object externalId6, 
                    object externalId7, 
                    object groupPermission, 
                    object hidden, 
                    string name, 
                    object owner, 
                    object ownerPermission, 
                    object readOnly, 
                    object worldPermission, 
                    object typeCode)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // Apply Defaults
            if ((typeCode == null))
            {
                typeCode = "System Folder";
            }
            // Insert the base members using the base class.
            int systemFolderId = Folder.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, typeCode);
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.NewSystemFolderRow();
            systemFolderRow[systemFolderTable.RowVersionColumn] = rowVersion;
            systemFolderRow[systemFolderTable.SystemFolderIdColumn] = systemFolderId;
            systemFolderTable.AddSystemFolderRow(systemFolderRow);
            adoTransaction.DataRows.Add(systemFolderRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"SystemFolder\" (\"rowVersion\",SystemFolderId) values (@rowVersion,@systemFo" +
                    "lderId)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@systemFolderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, systemFolderId));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return systemFolderRow.SystemFolderId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Update(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Folder.Update(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void UpdateChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.SystemFolder));
            User.UpdateChildren(adoTransaction);
        }
        
        /// <summary>Inserts a SystemFolder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object description = parameters["description"].Value;
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object externalId2 = parameters["externalId2"].Value;
            object externalId3 = parameters["externalId3"].Value;
            object externalId4 = parameters["externalId4"].Value;
            object externalId5 = parameters["externalId5"].Value;
            object externalId6 = parameters["externalId6"].Value;
            object externalId7 = parameters["externalId7"].Value;
            object groupPermission = parameters["groupPermission"].Value;
            object hidden = parameters["hidden"].Value;
            object name = parameters["name"].Value;
            object owner = parameters["owner"].Value;
            object ownerPermission = parameters["ownerPermission"].Value;
            object readOnly = parameters["readOnly"].Value;
            object worldPermission = parameters["worldPermission"].Value;
            int systemFolderId = parameters["systemFolderId"];
            object typeCode = parameters["typeCode"].Value;
            // Call the internal method to complete the operation.
            SystemFolder.Update(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, systemFolderId, typeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="description">The value for the Description column.</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="externalId2">The value for the ExternalId2 column.</param>
        /// <param name="externalId3">The value for the ExternalId3 column.</param>
        /// <param name="externalId4">The value for the ExternalId4 column.</param>
        /// <param name="externalId5">The value for the ExternalId5 column.</param>
        /// <param name="externalId6">The value for the ExternalId6 column.</param>
        /// <param name="externalId7">The value for the ExternalId7 column.</param>
        /// <param name="groupPermission">The value for the GroupPermission column.</param>
        /// <param name="hidden">The value for the Hidden column.</param>
        /// <param name="name">The value for the Name column.</param>
        /// <param name="owner">The value for the Owner column.</param>
        /// <param name="ownerPermission">The value for the OwnerPermission column.</param>
        /// <param name="readOnly">The value for the ReadOnly column.</param>
        /// <param name="worldPermission">The value for the WorldPermission column.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="typeCode">The value for the TypeCode column.</param>
        public new static void Update(
                    AdoTransaction adoTransaction, 
                    SqlTransaction sqlTransaction, 
                    ref long rowVersion, 
                    object description, 
                    object externalId0, 
                    object externalId1, 
                    object externalId2, 
                    object externalId3, 
                    object externalId4, 
                    object externalId5, 
                    object externalId6, 
                    object externalId7, 
                    object groupPermission, 
                    object hidden, 
                    object name, 
                    object owner, 
                    object ownerPermission, 
                    object readOnly, 
                    object worldPermission, 
                    int systemFolderId, 
                    object typeCode)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.FindBySystemFolderId(systemFolderId);
            if ((systemFolderRow == null))
            {
                throw new Exception(string.Format("The SystemFolder table does not have an element identified by {0}", systemFolderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((systemFolderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            // Insert the base members using the base class.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = systemFolderRow.FolderRow.RowVersion;
            Folder.Update(adoTransaction, sqlTransaction, ref baseRowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, systemFolderId, typeCode);
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Update the record in the ADO database.
            systemFolderRow[systemFolderTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(systemFolderRow);
            // Update the record in the SQL database.
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Delete(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Folder.Delete(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void DeleteChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.SystemFolder));
            User.DeleteChildren(adoTransaction);
        }
        
        /// <summary>Inserts a SystemFolder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int systemFolderId = parameters["systemFolderId"];
            // Call the internal method to complete the operation.
            SystemFolder.Delete(adoTransaction, sqlTransaction, rowVersion, systemFolderId);
        }
        
        /// <summary>Deletes a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int systemFolderId)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.FindBySystemFolderId(systemFolderId);
            if ((systemFolderRow == null))
            {
                throw new Exception(string.Format("The SystemFolder table does not have an element identified by {0}", systemFolderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((systemFolderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = systemFolderRow.FolderRow.RowVersion;
            Folder.Delete(adoTransaction, sqlTransaction, baseRowVersion, systemFolderId);
        }
        
        /// <summary>DeleteChildrens a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal new static void DeleteChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int systemFolderId)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // This record can be used to iterate through all the children.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.FindBySystemFolderId(systemFolderId);
            // Delete the child records.
            for (int index = 0; (index < systemFolderRow.GetUserRows().Length); index = (index + 1))
            {
                ServerDataModel.UserRow childUserRow = systemFolderRow.GetUserRows()[index];
                User.DeleteChildren(adoTransaction, sqlTransaction, childUserRow.RowVersion, childUserRow.UserId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            systemFolderRow[systemFolderTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(systemFolderRow);
            systemFolderRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"SystemFolder\" set \"IsDeleted\" = 1 where \"SystemFolderId\"=@systemFolderId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@systemFolderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, systemFolderId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Archive(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Folder.Archive(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void ArchiveChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.SystemFolder));
            User.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a SystemFolder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int systemFolderId = parameters["systemFolderId"];
            // Call the internal method to complete the operation.
            SystemFolder.Archive(adoTransaction, sqlTransaction, rowVersion, systemFolderId);
        }
        
        /// <summary>Archives a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int systemFolderId)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.FindBySystemFolderId(systemFolderId);
            if ((systemFolderRow == null))
            {
                throw new Exception(string.Format("The SystemFolder table does not have an element identified by {0}", systemFolderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((systemFolderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = systemFolderRow.FolderRow.RowVersion;
            Folder.Archive(adoTransaction, sqlTransaction, baseRowVersion, systemFolderId);
        }
        
        /// <summary>ArchiveChildrens a SystemFolder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal new static void ArchiveChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int systemFolderId)
        {
            // Accessor for the SystemFolder Table.
            ServerDataModel.SystemFolderDataTable systemFolderTable = ServerDataModel.SystemFolder;
            // This record can be used to iterate through all the children.
            ServerDataModel.SystemFolderRow systemFolderRow = systemFolderTable.FindBySystemFolderId(systemFolderId);
            // Archive the child records.
            for (int index = 0; (index < systemFolderRow.GetUserRows().Length); index = (index + 1))
            {
                ServerDataModel.UserRow childUserRow = systemFolderRow.GetUserRows()[index];
                User.ArchiveChildren(adoTransaction, sqlTransaction, childUserRow.RowVersion, childUserRow.UserId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            systemFolderRow[systemFolderTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(systemFolderRow);
            systemFolderRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"SystemFolder\" set \"IsArchived\" = 1 where \"SystemFolderId\"=@systemFolderId" +
                    "");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@systemFolderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, systemFolderId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
