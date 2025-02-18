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
    public class Stylesheet
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
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.StylesheetLock);
        }
        
        /// <summary>Inserts a Stylesheet record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            string name = parameters["name"];
            int stylesheetTypeCode = parameters["stylesheetTypeCode"];
            string text = parameters["text"];
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int stylesheetId = Stylesheet.Insert(adoTransaction, sqlTransaction, ref rowVersion, externalId0, externalId1, name, stylesheetTypeCode, text);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = stylesheetId;
        }
        
        /// <summary>Inserts a Stylesheet record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="name">The value for the Name column.</param>
        /// <param name="stylesheetTypeCode">The value for the StylesheetTypeCode column.</param>
        /// <param name="text">The value for the Text column.</param>
        public static int Insert(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object externalId0, object externalId1, string name, int stylesheetTypeCode, string text)
        {
            // Accessor for the Stylesheet Table.
            ServerMarketData.StylesheetDataTable stylesheetTable = ServerMarketData.Stylesheet;
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
            ServerMarketData.StylesheetRow stylesheetRow = stylesheetTable.NewStylesheetRow();
            stylesheetRow[stylesheetTable.RowVersionColumn] = rowVersion;
            stylesheetRow[stylesheetTable.ExternalId0Column] = externalId0;
            stylesheetRow[stylesheetTable.ExternalId1Column] = externalId1;
            stylesheetRow[stylesheetTable.NameColumn] = name;
            stylesheetRow[stylesheetTable.StylesheetTypeCodeColumn] = stylesheetTypeCode;
            stylesheetRow[stylesheetTable.TextColumn] = text;
            stylesheetTable.AddStylesheetRow(stylesheetRow);
            adoTransaction.DataRows.Add(stylesheetRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Stylesheet\" (\"rowVersion\",\"ExternalId0\",\"ExternalId1\",\"Name\",\"StylesheetI" +
                    "d\",\"StylesheetTypeCode\",\"Text\") values (@rowVersion,@externalId0,@externalId1,@n" +
                    "ame,@stylesheetId,@stylesheetTypeCode,@text)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, name));
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetRow[stylesheetTable.StylesheetIdColumn]));
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@text", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, text));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return stylesheetRow.StylesheetId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.StylesheetLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a Stylesheet record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            object externalId0 = parameters["externalId0"].Value;
            object externalId1 = parameters["externalId1"].Value;
            object name = parameters["name"].Value;
            int stylesheetId = parameters["stylesheetId"];
            object stylesheetTypeCode = parameters["stylesheetTypeCode"].Value;
            object text = parameters["text"].Value;
            // Call the internal method to complete the operation.
            Stylesheet.Update(adoTransaction, sqlTransaction, ref rowVersion, externalId0, externalId1, name, stylesheetId, stylesheetTypeCode, text);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Stylesheet record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="externalId0">The value for the ExternalId0 column.</param>
        /// <param name="externalId1">The value for the ExternalId1 column.</param>
        /// <param name="name">The value for the Name column.</param>
        /// <param name="stylesheetId">The value for the StylesheetId column.</param>
        /// <param name="stylesheetTypeCode">The value for the StylesheetTypeCode column.</param>
        /// <param name="text">The value for the Text column.</param>
        public static void Update(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, ref long rowVersion, object externalId0, object externalId1, object name, int stylesheetId, object stylesheetTypeCode, object text)
        {
            // Accessor for the Stylesheet Table.
            ServerMarketData.StylesheetDataTable stylesheetTable = ServerMarketData.Stylesheet;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.StylesheetRow stylesheetRow = stylesheetTable.FindByStylesheetId(stylesheetId);
            if ((stylesheetRow == null))
            {
                throw new Exception(string.Format("The Stylesheet table does not have an element identified by {0}", stylesheetId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((stylesheetRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((externalId0 == null))
            {
                externalId0 = stylesheetRow[stylesheetTable.ExternalId0Column];
            }
            if ((externalId1 == null))
            {
                externalId1 = stylesheetRow[stylesheetTable.ExternalId1Column];
            }
            if ((name == null))
            {
                name = stylesheetRow[stylesheetTable.NameColumn];
            }
            if ((stylesheetTypeCode == null))
            {
                stylesheetTypeCode = stylesheetRow[stylesheetTable.StylesheetTypeCodeColumn];
            }
            if ((text == null))
            {
                text = stylesheetRow[stylesheetTable.TextColumn];
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            stylesheetRow[stylesheetTable.RowVersionColumn] = rowVersion;
            stylesheetRow[stylesheetTable.ExternalId0Column] = externalId0;
            stylesheetRow[stylesheetTable.ExternalId1Column] = externalId1;
            stylesheetRow[stylesheetTable.NameColumn] = name;
            stylesheetRow[stylesheetTable.StylesheetTypeCodeColumn] = stylesheetTypeCode;
            stylesheetRow[stylesheetTable.TextColumn] = text;
            adoTransaction.DataRows.Add(stylesheetRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Stylesheet\" set \"RowVersion\"=@rowVersion,\"ExternalId0\"=@externalId0,\"Exte" +
                    "rnalId1\"=@externalId1,\"Name\"=@name,\"StylesheetTypeCode\"=@stylesheetTypeCode,\"Tex" +
                    "t\"=@text where \"StylesheetId\"=@stylesheetId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId0", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId0));
            sqlCommand.Parameters.Add(new SqlParameter("@externalId1", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, externalId1));
            sqlCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, name));
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetId));
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@text", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, text));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.StylesheetLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a Stylesheet record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int stylesheetId = parameters["stylesheetId"];
            // Call the internal method to complete the operation.
            Stylesheet.Delete(adoTransaction, sqlTransaction, rowVersion, stylesheetId);
        }
        
        /// <summary>Deletes a Stylesheet record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="stylesheetId">The value for the StylesheetId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int stylesheetId)
        {
            // Accessor for the Stylesheet Table.
            ServerMarketData.StylesheetDataTable stylesheetTable = ServerMarketData.Stylesheet;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.StylesheetRow stylesheetRow = stylesheetTable.FindByStylesheetId(stylesheetId);
            if ((stylesheetRow == null))
            {
                throw new Exception(string.Format("The Stylesheet table does not have an element identified by {0}", stylesheetId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((stylesheetRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterAdvertisementStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterAdvertisementStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderDetailStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionDetailStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchHistoryStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchHistoryStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderDetailStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterWorkingOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterWorkingOrderStylesheetId()[index];
                Blotter.DeleteChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            stylesheetRow[stylesheetTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(stylesheetRow);
            stylesheetRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Stylesheet\" set \"IsDeleted\" = 1 where \"StylesheetId\"=@stylesheetId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.StylesheetLock);
            Blotter.ArchiveChildren(adoTransaction);
        }
        
        /// <summary>Inserts a Stylesheet record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int stylesheetId = parameters["stylesheetId"];
            // Call the internal method to complete the operation.
            Stylesheet.Archive(adoTransaction, sqlTransaction, rowVersion, stylesheetId);
        }
        
        /// <summary>Archives a Stylesheet record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="stylesheetId">The value for the StylesheetId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int stylesheetId)
        {
            // Accessor for the Stylesheet Table.
            ServerMarketData.StylesheetDataTable stylesheetTable = ServerMarketData.Stylesheet;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.StylesheetRow stylesheetRow = stylesheetTable.FindByStylesheetId(stylesheetId);
            if ((stylesheetRow == null))
            {
                throw new Exception(string.Format("The Stylesheet table does not have an element identified by {0}", stylesheetId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((stylesheetRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterAdvertisementStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterAdvertisementStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderDetailStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterDestinationOrderStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionDetailStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterExecutionStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchHistoryStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterMatchHistoryStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderDetailStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderDetailStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterSourceOrderStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            for (int index = 0; (index < stylesheetRow.GetBlotterRowsByStylesheetBlotterWorkingOrderStylesheetId().Length); index = (index + 1))
            {
                ServerMarketData.BlotterRow childBlotterRow = stylesheetRow.GetBlotterRowsByStylesheetBlotterWorkingOrderStylesheetId()[index];
                Blotter.ArchiveChildren(adoTransaction, sqlTransaction, childBlotterRow.RowVersion, childBlotterRow.BlotterId);
            }
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            stylesheetRow[stylesheetTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(stylesheetRow);
            stylesheetRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Stylesheet\" set \"IsArchived\" = 1 where \"StylesheetId\"=@stylesheetId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@stylesheetId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, stylesheetId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
