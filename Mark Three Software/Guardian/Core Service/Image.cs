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
    public class Image
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
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ImageLock);
        }
        
        /// <summary>Inserts a Image record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object externalId0 = parameters["externalId0"].Value;
            string image = parameters["image"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int imageId = Image.Insert(adoTransaction, sqlTransaction, ref rowVersion, externalId0, image);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = imageId;
        }
        
        /// <summary>Inserts a Image record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="image">The value for the Image column.</param>
        public static int Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object externalId0, string image)
        {
            // Accessor for the Image Table.
            ServerMarketData.ImageDataTable imageTable = ServerMarketData.Image;
            // Apply Defaults
            if ((externalId0 == null))
            {
                externalId0 = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerMarketData.ImageRow imageRow = imageTable.NewImageRow();
            imageRow[imageTable.RowVersionColumn] = rowVersion;
            imageRow[imageTable.ExternalId0Column] = externalId0;
            imageRow[imageTable.ImageColumn] = image;
            imageTable.AddImageRow(imageRow);
            adoTransaction.DataRows.Add(imageRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Image\" (\"rowVersion\",\"ExternalId0\",\"ImageId\",\"Image\") values (@rowVersion" +
                    ",@externalId0,@imageId,@image)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@imageId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, imageRow[imageTable.ImageIdColumn]));
            sqlCommand.Parameters.Add(new SqlParameter("@image", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, image));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return imageRow.ImageId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ImageLock);
        }
        
        /// <summary>Inserts a Image record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object externalId0 = parameters["externalId0"].Value;
            int imageId = parameters["imageId"];
            object image = parameters["image"].Value;
            // Call the internal method to complete the operation.
            Image.Update(adoTransaction, sqlTransaction, ref rowVersion, externalId0, imageId, image);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Image record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="imageId">The value for the ImageId column.</param>
        /// <param name="image">The value for the Image column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object externalId0, int imageId, object image)
        {
            // Accessor for the Image Table.
            ServerMarketData.ImageDataTable imageTable = ServerMarketData.Image;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.ImageRow imageRow = imageTable.FindByImageId(imageId);
            if ((imageRow == null))
            {
                throw new Exception(string.Format("The Image table does not have an element identified by {0}", imageId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((imageRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((externalId0 == null))
            {
                externalId0 = imageRow[imageTable.ExternalId0Column];
            }
            if ((image == null))
            {
                image = imageRow[imageTable.ImageColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            imageRow[imageTable.RowVersionColumn] = rowVersion;
            imageRow[imageTable.ExternalId0Column] = externalId0;
            imageRow[imageTable.ImageColumn] = image;
            adoTransaction.DataRows.Add(imageRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Image\" set \"RowVersion\"=@rowVersion,\"ExternalId0\"=@externalId0,\"Image\"=@i" +
                    "mage where \"ImageId\"=@imageId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@imageId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, imageId));
            sqlCommand.Parameters.Add(new SqlParameter("@image", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, image));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ImageLock);
        }
        
        /// <summary>Inserts a Image record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int imageId = parameters["imageId"];
            // Call the internal method to complete the operation.
            Image.Delete(adoTransaction, sqlTransaction, rowVersion, imageId);
        }
        
        /// <summary>Deletes a Image record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="imageId">The value for the ImageId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int imageId)
        {
            // Accessor for the Image Table.
            ServerMarketData.ImageDataTable imageTable = ServerMarketData.Image;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.ImageRow imageRow = imageTable.FindByImageId(imageId);
            if ((imageRow == null))
            {
                throw new Exception(string.Format("The Image table does not have an element identified by {0}", imageId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((imageRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            imageRow[imageTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(imageRow);
            imageRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Image\" set \"IsDeleted\" = 1 where \"ImageId\"=@imageId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@imageId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, imageId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.ImageLock);
        }
        
        /// <summary>Inserts a Image record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int imageId = parameters["imageId"];
            // Call the internal method to complete the operation.
            Image.Archive(adoTransaction, sqlTransaction, rowVersion, imageId);
        }
        
        /// <summary>Archives a Image record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="imageId">The value for the ImageId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int imageId)
        {
            // Accessor for the Image Table.
            ServerMarketData.ImageDataTable imageTable = ServerMarketData.Image;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.ImageRow imageRow = imageTable.FindByImageId(imageId);
            if ((imageRow == null))
            {
                throw new Exception(string.Format("The Image table does not have an element identified by {0}", imageId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((imageRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            imageRow[imageTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(imageRow);
            imageRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Image\" set \"IsArchived\" = 1 where \"ImageId\"=@imageId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@imageId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, imageId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
