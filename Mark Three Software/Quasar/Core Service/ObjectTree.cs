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
    public class ObjectTree
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
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ObjectTree));
        }
        
        /// <summary>Inserts a ObjectTree record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            int parentId = parameters["parentId"];
            int childId = parameters["childId"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            ObjectTree.Insert(adoTransaction, sqlTransaction, ref rowVersion, parentId, childId);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Inserts a ObjectTree record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="parentId">The value for the ParentId column.</param>
        /// <param name="childId">The value for the ChildId column.</param>
        public static void Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, int parentId, int childId)
        {
            // Accessor for the ObjectTree Table.
            ServerDataModel.ObjectTreeDataTable objectTreeTable = ServerDataModel.ObjectTree;
            // Apply Defaults
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.ObjectTreeRow objectTreeRow = objectTreeTable.NewObjectTreeRow();
            objectTreeRow[objectTreeTable.RowVersionColumn] = rowVersion;
            objectTreeRow[objectTreeTable.ParentIdColumn] = parentId;
            objectTreeRow[objectTreeTable.ChildIdColumn] = childId;
            objectTreeTable.AddObjectTreeRow(objectTreeRow);
            adoTransaction.DataRows.Add(objectTreeRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"ObjectTree\" (\"rowVersion\",\"ParentId\",\"ChildId\") values (@rowVersion,@pare" +
                    "ntId,@childId)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@parentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, parentId));
            sqlCommand.Parameters.Add(new SqlParameter("@childId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, childId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ObjectTree));
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ObjectTree));
        }
        
        /// <summary>Inserts a ObjectTree record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int parentId = parameters["parentId"];
            int childId = parameters["childId"];
            // Call the internal method to complete the operation.
            ObjectTree.Delete(adoTransaction, sqlTransaction, rowVersion, parentId, childId);
        }
        
        /// <summary>Deletes a ObjectTree record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="parentId">The value for the ParentId column.</param>
        /// <param name="childId">The value for the ChildId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int parentId, int childId)
        {
            // Accessor for the ObjectTree Table.
            ServerDataModel.ObjectTreeDataTable objectTreeTable = ServerDataModel.ObjectTree;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.ObjectTreeRow objectTreeRow = objectTreeTable.FindByParentIdChildId(parentId, childId);
            if ((objectTreeRow == null))
            {
                throw new Exception(string.Format("The ObjectTree table does not have an element identified by {0}{0}", parentId, childId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((objectTreeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            objectTreeRow[objectTreeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(objectTreeRow);
            objectTreeRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"ObjectTree\" set \"IsDeleted\" = 1 where \"ParentId\"=@parentId and \"ChildId\"=" +
                    "@childId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@parentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, parentId));
            sqlCommand.Parameters.Add(new SqlParameter("@childId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, childId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ObjectTree));
        }
        
        /// <summary>Inserts a ObjectTree record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int parentId = parameters["parentId"];
            int childId = parameters["childId"];
            // Call the internal method to complete the operation.
            ObjectTree.Archive(adoTransaction, sqlTransaction, rowVersion, parentId, childId);
        }
        
        /// <summary>Archives a ObjectTree record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="parentId">The value for the ParentId column.</param>
        /// <param name="childId">The value for the ChildId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int parentId, int childId)
        {
            // Accessor for the ObjectTree Table.
            ServerDataModel.ObjectTreeDataTable objectTreeTable = ServerDataModel.ObjectTree;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.ObjectTreeRow objectTreeRow = objectTreeTable.FindByParentIdChildId(parentId, childId);
            if ((objectTreeRow == null))
            {
                throw new Exception(string.Format("The ObjectTree table does not have an element identified by {0}{0}", parentId, childId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((objectTreeRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            objectTreeRow[objectTreeTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(objectTreeRow);
            objectTreeRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"ObjectTree\" set \"IsArchived\" = 1 where \"ParentId\"=@parentId and \"ChildId\"" +
                    "=@childId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@parentId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, parentId));
            sqlCommand.Parameters.Add(new SqlParameter("@childId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, childId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
