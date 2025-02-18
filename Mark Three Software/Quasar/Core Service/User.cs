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
    public class User : MarkThree.Quasar.Core.Object
    {
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Insert(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Object.Insert(adoTransaction);
            // These table lock(s) are required for the 'Insert' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.User));
        }
        
        /// <summary>Inserts a User record using Metadata Parameters.</summary>
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
            object preferences = parameters["preferences"].Value;
            object systemFolderId = parameters["systemFolderId"].Value;
            object typeCode = parameters["typeCode"].Value;
            string userName = parameters["userName"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int userId = User.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, preferences, systemFolderId, typeCode, userName);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = userId;
        }
        
        /// <summary>Inserts a User record.</summary>
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
        /// <param name="preferences">The value for the Preferences column.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="typeCode">The value for the TypeCode column.</param>
        /// <param name="userName">The value for the UserName column.</param>
        public static int Insert(
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
                    object preferences, 
                    object systemFolderId, 
                    object typeCode, 
                    string userName)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // Apply Defaults
            if ((preferences == null))
            {
                preferences = System.DBNull.Value;
            }
            if ((systemFolderId == null))
            {
                systemFolderId = System.DBNull.Value;
            }
            if ((typeCode == null))
            {
                typeCode = "User";
            }
            // Insert the base members using the base class.
            int userId = Object.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, typeCode, worldPermission);
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.UserRow userRow = userTable.NewUserRow();
            userRow[userTable.RowVersionColumn] = rowVersion;
            userRow[userTable.PreferencesColumn] = preferences;
            userRow[userTable.SystemFolderIdColumn] = systemFolderId;
            userRow[userTable.UserIdColumn] = userId;
            userRow[userTable.UserNameColumn] = userName;
            userTable.AddUserRow(userRow);
            adoTransaction.DataRows.Add(userRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"User\" (\"rowVersion\",Preferences,SystemFolderId,UserId,UserName) values (@" +
                    "rowVersion,@preferences,@systemFolderId,@userId,@userName)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@preferences", SqlDbType.Image, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, preferences));
            sqlCommand.Parameters.Add(new SqlParameter("@systemFolderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, systemFolderId));
            sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userId));
            sqlCommand.Parameters.Add(new SqlParameter("@userName", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userName));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return userRow.UserId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Update(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Object.Update(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal static void UpdateChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.User));
            Allocation.Update(adoTransaction);
            Execution.Update(adoTransaction);
            Placement.Update(adoTransaction);
        }
        
        /// <summary>Inserts a User record using Metadata Parameters.</summary>
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
            object preferences = parameters["preferences"].Value;
            object systemFolderId = parameters["systemFolderId"].Value;
            object typeCode = parameters["typeCode"].Value;
            int userId = parameters["userId"];
            object userName = parameters["userName"].Value;
            // Call the internal method to complete the operation.
            User.Update(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, preferences, systemFolderId, typeCode, userId, userName);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a User record.</summary>
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
        /// <param name="preferences">The value for the Preferences column.</param>
        /// <param name="systemFolderId">The value for the SystemFolderId column.</param>
        /// <param name="typeCode">The value for the TypeCode column.</param>
        /// <param name="userId">The value for the UserId column.</param>
        /// <param name="userName">The value for the UserName column.</param>
        public static void Update(
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
                    object preferences, 
                    object systemFolderId, 
                    object typeCode, 
                    int userId, 
                    object userName)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.UserRow userRow = userTable.FindByUserId(userId);
            if ((userRow == null))
            {
                throw new Exception(string.Format("The User table does not have an element identified by {0}", userId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((userRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((preferences == null))
            {
                preferences = userRow[userTable.PreferencesColumn];
            }
            if ((systemFolderId == null))
            {
                systemFolderId = userRow[userTable.SystemFolderIdColumn];
            }
            if ((userName == null))
            {
                userName = userRow[userTable.UserNameColumn];
            }
            // Insert the base members using the base class.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = userRow.ObjectRow.RowVersion;
            Object.Update(adoTransaction, sqlTransaction, ref baseRowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, userId, owner, ownerPermission, readOnly, typeCode, worldPermission);
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Update the record in the ADO database.
            userRow[userTable.RowVersionColumn] = rowVersion;
            userRow[userTable.PreferencesColumn] = preferences;
            userRow[userTable.SystemFolderIdColumn] = systemFolderId;
            userRow[userTable.UserNameColumn] = userName;
            adoTransaction.DataRows.Add(userRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"User\" set \"RowVersion\"=@rowVersion,\"Preferences\"=@preferences,\"SystemFold" +
                    "erId\"=@systemFolderId,\"UserName\"=@userName where \"UserId\"=@userId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@preferences", SqlDbType.Image, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, preferences));
            sqlCommand.Parameters.Add(new SqlParameter("@systemFolderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, systemFolderId));
            sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userId));
            sqlCommand.Parameters.Add(new SqlParameter("@userName", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userName));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Delete(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Object.Delete(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal static void DeleteChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.User));
            Allocation.Delete(adoTransaction);
            Execution.Delete(adoTransaction);
            Placement.Delete(adoTransaction);
        }
        
        /// <summary>Inserts a User record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int userId = parameters["userId"];
            // Call the internal method to complete the operation.
            User.Delete(adoTransaction, sqlTransaction, rowVersion, userId);
        }
        
        /// <summary>Deletes a User record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="userId">The value for the UserId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int userId)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.UserRow userRow = userTable.FindByUserId(userId);
            if ((userRow == null))
            {
                throw new Exception(string.Format("The User table does not have an element identified by {0}", userId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((userRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = userRow.ObjectRow.RowVersion;
            Object.Delete(adoTransaction, sqlTransaction, baseRowVersion, userId);
        }
        
        /// <summary>DeleteChildrens a User record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="userId">The value for the UserId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal static void DeleteChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int userId)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // This record can be used to iterate through all the children.
            ServerDataModel.UserRow userRow = userTable.FindByUserId(userId);
            // Delete the child records.
            for (int index = 0; (index < userRow.GetAllocationRowsByFKUserAllocationCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.AllocationRow childAllocationRow = userRow.GetAllocationRowsByFKUserAllocationCreatedUserId()[index];
                Allocation.Delete(adoTransaction, sqlTransaction, childAllocationRow.RowVersion, childAllocationRow.AllocationId);
            }
            for (int index = 0; (index < userRow.GetAllocationRowsByFKUserAllocationModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.AllocationRow childAllocationRow = userRow.GetAllocationRowsByFKUserAllocationModifiedUserId()[index];
                Allocation.Delete(adoTransaction, sqlTransaction, childAllocationRow.RowVersion, childAllocationRow.AllocationId);
            }
            for (int index = 0; (index < userRow.GetExecutionRowsByFKUserExecutionCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.ExecutionRow childExecutionRow = userRow.GetExecutionRowsByFKUserExecutionCreatedUserId()[index];
                Execution.Delete(adoTransaction, sqlTransaction, childExecutionRow.RowVersion, childExecutionRow.ExecutionId);
            }
            for (int index = 0; (index < userRow.GetExecutionRowsByFKUserExecutionModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.ExecutionRow childExecutionRow = userRow.GetExecutionRowsByFKUserExecutionModifiedUserId()[index];
                Execution.Delete(adoTransaction, sqlTransaction, childExecutionRow.RowVersion, childExecutionRow.ExecutionId);
            }
            for (int index = 0; (index < userRow.GetPlacementRowsByFKUserPlacementCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.PlacementRow childPlacementRow = userRow.GetPlacementRowsByFKUserPlacementCreatedUserId()[index];
                Placement.Delete(adoTransaction, sqlTransaction, childPlacementRow.RowVersion, childPlacementRow.PlacementId);
            }
            for (int index = 0; (index < userRow.GetPlacementRowsByFKUserPlacementModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.PlacementRow childPlacementRow = userRow.GetPlacementRowsByFKUserPlacementModifiedUserId()[index];
                Placement.Delete(adoTransaction, sqlTransaction, childPlacementRow.RowVersion, childPlacementRow.PlacementId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            userRow[userTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(userRow);
            userRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"User\" set \"IsDeleted\" = 1 where \"UserId\"=@userId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Archive(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Object.Archive(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal static void ArchiveChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.User));
            Allocation.Archive(adoTransaction);
            Execution.Archive(adoTransaction);
            Placement.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a User record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int userId = parameters["userId"];
            // Call the internal method to complete the operation.
            User.Archive(adoTransaction, sqlTransaction, rowVersion, userId);
        }
        
        /// <summary>Archives a User record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="userId">The value for the UserId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int userId)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.UserRow userRow = userTable.FindByUserId(userId);
            if ((userRow == null))
            {
                throw new Exception(string.Format("The User table does not have an element identified by {0}", userId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((userRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = userRow.ObjectRow.RowVersion;
            Object.Archive(adoTransaction, sqlTransaction, baseRowVersion, userId);
        }
        
        /// <summary>ArchiveChildrens a User record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="userId">The value for the UserId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal static void ArchiveChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int userId)
        {
            // Accessor for the User Table.
            ServerDataModel.UserDataTable userTable = ServerDataModel.User;
            // This record can be used to iterate through all the children.
            ServerDataModel.UserRow userRow = userTable.FindByUserId(userId);
            // Archive the child records.
            for (int index = 0; (index < userRow.GetAllocationRowsByFKUserAllocationCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.AllocationRow childAllocationRow = userRow.GetAllocationRowsByFKUserAllocationCreatedUserId()[index];
                Allocation.Archive(adoTransaction, sqlTransaction, childAllocationRow.RowVersion, childAllocationRow.AllocationId);
            }
            for (int index = 0; (index < userRow.GetAllocationRowsByFKUserAllocationModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.AllocationRow childAllocationRow = userRow.GetAllocationRowsByFKUserAllocationModifiedUserId()[index];
                Allocation.Archive(adoTransaction, sqlTransaction, childAllocationRow.RowVersion, childAllocationRow.AllocationId);
            }
            for (int index = 0; (index < userRow.GetExecutionRowsByFKUserExecutionCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.ExecutionRow childExecutionRow = userRow.GetExecutionRowsByFKUserExecutionCreatedUserId()[index];
                Execution.Archive(adoTransaction, sqlTransaction, childExecutionRow.RowVersion, childExecutionRow.ExecutionId);
            }
            for (int index = 0; (index < userRow.GetExecutionRowsByFKUserExecutionModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.ExecutionRow childExecutionRow = userRow.GetExecutionRowsByFKUserExecutionModifiedUserId()[index];
                Execution.Archive(adoTransaction, sqlTransaction, childExecutionRow.RowVersion, childExecutionRow.ExecutionId);
            }
            for (int index = 0; (index < userRow.GetPlacementRowsByFKUserPlacementCreatedUserId().Length); index = (index + 1))
            {
                ServerDataModel.PlacementRow childPlacementRow = userRow.GetPlacementRowsByFKUserPlacementCreatedUserId()[index];
                Placement.Archive(adoTransaction, sqlTransaction, childPlacementRow.RowVersion, childPlacementRow.PlacementId);
            }
            for (int index = 0; (index < userRow.GetPlacementRowsByFKUserPlacementModifiedUserId().Length); index = (index + 1))
            {
                ServerDataModel.PlacementRow childPlacementRow = userRow.GetPlacementRowsByFKUserPlacementModifiedUserId()[index];
                Placement.Archive(adoTransaction, sqlTransaction, childPlacementRow.RowVersion, childPlacementRow.PlacementId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            userRow[userTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(userRow);
            userRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"User\" set \"IsArchived\" = 1 where \"UserId\"=@userId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, userId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
