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
    public class Institution : MarkThree.Guardian.Core.Source
    {
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Insert(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Source.Insert(adoTransaction);
            // These table lock(s) are required for the 'Insert' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.InstitutionLock);
        }
        
        /// <summary>Inserts a Institution record using Metadata Parameters.</summary>
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
            object advertisementStylesheetId = parameters["advertisementStylesheetId"].Value;
            object destinationOrderDetailStylesheetId = parameters["destinationOrderDetailStylesheetId"].Value;
            object destinationOrderStylesheetId = parameters["destinationOrderStylesheetId"].Value;
            object executionDetailStylesheetId = parameters["executionDetailStylesheetId"].Value;
            object executionStylesheetId = parameters["executionStylesheetId"].Value;
            object matchStylesheetId = parameters["matchStylesheetId"].Value;
            object matchHistoryStylesheetId = parameters["matchHistoryStylesheetId"].Value;
            int partyTypeCode = parameters["partyTypeCode"];
            object sourceOrderDetailStylesheetId = parameters["sourceOrderDetailStylesheetId"].Value;
            object sourceOrderStylesheetId = parameters["sourceOrderStylesheetId"].Value;
            object workingOrderStylesheetId = parameters["workingOrderStylesheetId"].Value;
            object buyMarketValueThreshold = parameters["buyMarketValueThreshold"].Value;
            object buyQuantityThreshold = parameters["buyQuantityThreshold"].Value;
            object sellMarketValueThreshold = parameters["sellMarketValueThreshold"].Value;
            object sellQuantityThreshold = parameters["sellQuantityThreshold"].Value;
            string shortName = parameters["shortName"];
            object typeCode = parameters["typeCode"].Value;
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int institutionId = Institution.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, advertisementStylesheetId, destinationOrderDetailStylesheetId, destinationOrderStylesheetId, executionDetailStylesheetId, executionStylesheetId, matchStylesheetId, matchHistoryStylesheetId, partyTypeCode, sourceOrderDetailStylesheetId, sourceOrderStylesheetId, workingOrderStylesheetId, buyMarketValueThreshold, buyQuantityThreshold, sellMarketValueThreshold, sellQuantityThreshold, shortName, typeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = institutionId;
        }
        
        /// <summary>Inserts a Institution record.</summary>
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
        /// <param name="advertisementStylesheetId">The value for the AdvertisementStylesheetId column.</param>
        /// <param name="destinationOrderDetailStylesheetId">The value for the DestinationOrderDetailStylesheetId column.</param>
        /// <param name="destinationOrderStylesheetId">The value for the DestinationOrderStylesheetId column.</param>
        /// <param name="executionDetailStylesheetId">The value for the ExecutionDetailStylesheetId column.</param>
        /// <param name="executionStylesheetId">The value for the ExecutionStylesheetId column.</param>
        /// <param name="matchStylesheetId">The value for the MatchStylesheetId column.</param>
        /// <param name="matchHistoryStylesheetId">The value for the MatchHistoryStylesheetId column.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        /// <param name="sourceOrderDetailStylesheetId">The value for the SourceOrderDetailStylesheetId column.</param>
        /// <param name="sourceOrderStylesheetId">The value for the SourceOrderStylesheetId column.</param>
        /// <param name="workingOrderStylesheetId">The value for the WorkingOrderStylesheetId column.</param>
        /// <param name="buyMarketValueThreshold">The value for the BuyMarketValueThreshold column.</param>
        /// <param name="buyQuantityThreshold">The value for the BuyQuantityThreshold column.</param>
        /// <param name="sellMarketValueThreshold">The value for the SellMarketValueThreshold column.</param>
        /// <param name="sellQuantityThreshold">The value for the SellQuantityThreshold column.</param>
        /// <param name="shortName">The value for the ShortName column.</param>
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
                    object advertisementStylesheetId, 
                    object destinationOrderDetailStylesheetId, 
                    object destinationOrderStylesheetId, 
                    object executionDetailStylesheetId, 
                    object executionStylesheetId, 
                    object matchStylesheetId, 
                    object matchHistoryStylesheetId, 
                    int partyTypeCode, 
                    object sourceOrderDetailStylesheetId, 
                    object sourceOrderStylesheetId, 
                    object workingOrderStylesheetId, 
                    object buyMarketValueThreshold, 
                    object buyQuantityThreshold, 
                    object sellMarketValueThreshold, 
                    object sellQuantityThreshold, 
                    string shortName, 
                    object typeCode)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // Apply Defaults
            if ((typeCode == null))
            {
                typeCode = "Institution";
            }
            // Insert the base members using the base class.
            int institutionId = Source.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, advertisementStylesheetId, destinationOrderDetailStylesheetId, destinationOrderStylesheetId, executionDetailStylesheetId, executionStylesheetId, matchStylesheetId, matchHistoryStylesheetId, partyTypeCode, sourceOrderDetailStylesheetId, sourceOrderStylesheetId, workingOrderStylesheetId, buyMarketValueThreshold, buyQuantityThreshold, sellMarketValueThreshold, sellQuantityThreshold, shortName, typeCode);
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.NewInstitutionRow();
            institutionRow[institutionTable.RowVersionColumn] = rowVersion;
            institutionRow[institutionTable.InstitutionIdColumn] = institutionId;
            institutionTable.AddInstitutionRow(institutionRow);
            adoTransaction.DataRows.Add(institutionRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand("insert \"Institution\" (\"rowVersion\",InstitutionId) values (@rowVersion,@institutio" +
                    "nId)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@institutionId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, institutionId));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return institutionRow.InstitutionId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Update(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Source.Update(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void UpdateChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.InstitutionLock);
        }
        
        /// <summary>Inserts a Institution record using Metadata Parameters.</summary>
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
            object advertisementStylesheetId = parameters["advertisementStylesheetId"].Value;
            object destinationOrderDetailStylesheetId = parameters["destinationOrderDetailStylesheetId"].Value;
            object destinationOrderStylesheetId = parameters["destinationOrderStylesheetId"].Value;
            object executionDetailStylesheetId = parameters["executionDetailStylesheetId"].Value;
            object executionStylesheetId = parameters["executionStylesheetId"].Value;
            object matchStylesheetId = parameters["matchStylesheetId"].Value;
            object matchHistoryStylesheetId = parameters["matchHistoryStylesheetId"].Value;
            object partyTypeCode = parameters["partyTypeCode"].Value;
            object sourceOrderDetailStylesheetId = parameters["sourceOrderDetailStylesheetId"].Value;
            object sourceOrderStylesheetId = parameters["sourceOrderStylesheetId"].Value;
            object workingOrderStylesheetId = parameters["workingOrderStylesheetId"].Value;
            object buyMarketValueThreshold = parameters["buyMarketValueThreshold"].Value;
            object buyQuantityThreshold = parameters["buyQuantityThreshold"].Value;
            object sellMarketValueThreshold = parameters["sellMarketValueThreshold"].Value;
            object sellQuantityThreshold = parameters["sellQuantityThreshold"].Value;
            object shortName = parameters["shortName"].Value;
            int institutionId = parameters["institutionId"];
            object typeCode = parameters["typeCode"].Value;
            // Call the internal method to complete the operation.
            Institution.Update(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, advertisementStylesheetId, destinationOrderDetailStylesheetId, destinationOrderStylesheetId, executionDetailStylesheetId, executionStylesheetId, matchStylesheetId, matchHistoryStylesheetId, partyTypeCode, sourceOrderDetailStylesheetId, sourceOrderStylesheetId, workingOrderStylesheetId, buyMarketValueThreshold, buyQuantityThreshold, sellMarketValueThreshold, sellQuantityThreshold, shortName, institutionId, typeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a Institution record.</summary>
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
        /// <param name="advertisementStylesheetId">The value for the AdvertisementStylesheetId column.</param>
        /// <param name="destinationOrderDetailStylesheetId">The value for the DestinationOrderDetailStylesheetId column.</param>
        /// <param name="destinationOrderStylesheetId">The value for the DestinationOrderStylesheetId column.</param>
        /// <param name="executionDetailStylesheetId">The value for the ExecutionDetailStylesheetId column.</param>
        /// <param name="executionStylesheetId">The value for the ExecutionStylesheetId column.</param>
        /// <param name="matchStylesheetId">The value for the MatchStylesheetId column.</param>
        /// <param name="matchHistoryStylesheetId">The value for the MatchHistoryStylesheetId column.</param>
        /// <param name="partyTypeCode">The value for the PartyTypeCode column.</param>
        /// <param name="sourceOrderDetailStylesheetId">The value for the SourceOrderDetailStylesheetId column.</param>
        /// <param name="sourceOrderStylesheetId">The value for the SourceOrderStylesheetId column.</param>
        /// <param name="workingOrderStylesheetId">The value for the WorkingOrderStylesheetId column.</param>
        /// <param name="buyMarketValueThreshold">The value for the BuyMarketValueThreshold column.</param>
        /// <param name="buyQuantityThreshold">The value for the BuyQuantityThreshold column.</param>
        /// <param name="sellMarketValueThreshold">The value for the SellMarketValueThreshold column.</param>
        /// <param name="sellQuantityThreshold">The value for the SellQuantityThreshold column.</param>
        /// <param name="shortName">The value for the ShortName column.</param>
        /// <param name="institutionId">The value for the InstitutionId column.</param>
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
                    object advertisementStylesheetId, 
                    object destinationOrderDetailStylesheetId, 
                    object destinationOrderStylesheetId, 
                    object executionDetailStylesheetId, 
                    object executionStylesheetId, 
                    object matchStylesheetId, 
                    object matchHistoryStylesheetId, 
                    object partyTypeCode, 
                    object sourceOrderDetailStylesheetId, 
                    object sourceOrderStylesheetId, 
                    object workingOrderStylesheetId, 
                    object buyMarketValueThreshold, 
                    object buyQuantityThreshold, 
                    object sellMarketValueThreshold, 
                    object sellQuantityThreshold, 
                    object shortName, 
                    int institutionId, 
                    object typeCode)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.FindByInstitutionId(institutionId);
            if ((institutionRow == null))
            {
                throw new Exception(string.Format("The Institution table does not have an element identified by {0}", institutionId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((institutionRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            // Insert the base members using the base class.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = institutionRow.SourceRow.RowVersion;
            Source.Update(adoTransaction, sqlTransaction, ref baseRowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, advertisementStylesheetId, destinationOrderDetailStylesheetId, destinationOrderStylesheetId, executionDetailStylesheetId, executionStylesheetId, matchStylesheetId, matchHistoryStylesheetId, partyTypeCode, sourceOrderDetailStylesheetId, sourceOrderStylesheetId, workingOrderStylesheetId, buyMarketValueThreshold, buyQuantityThreshold, sellMarketValueThreshold, sellQuantityThreshold, shortName, institutionId, typeCode);
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Update the record in the ADO database.
            institutionRow[institutionTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(institutionRow);
            // Update the record in the SQL database.
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Delete(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Source.Delete(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void DeleteChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.InstitutionLock);
        }
        
        /// <summary>Inserts a Institution record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int institutionId = parameters["institutionId"];
            // Call the internal method to complete the operation.
            Institution.Delete(adoTransaction, sqlTransaction, rowVersion, institutionId);
        }
        
        /// <summary>Deletes a Institution record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="institutionId">The value for the InstitutionId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int institutionId)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.FindByInstitutionId(institutionId);
            if ((institutionRow == null))
            {
                throw new Exception(string.Format("The Institution table does not have an element identified by {0}", institutionId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((institutionRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = institutionRow.SourceRow.RowVersion;
            Source.Delete(adoTransaction, sqlTransaction, baseRowVersion, institutionId);
        }
        
        /// <summary>DeleteChildrens a Institution record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="institutionId">The value for the InstitutionId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal new static void DeleteChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int institutionId)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // This record can be used to iterate through all the children.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.FindByInstitutionId(institutionId);
            // Delete the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            institutionRow[institutionTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(institutionRow);
            institutionRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Institution\" set \"IsDeleted\" = 1 where \"InstitutionId\"=@institutionId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@institutionId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, institutionId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public new static void Archive(AdoTransaction adoTransaction)
        {
            // Lock the tables at the base level of the object hierarchy.
            Source.Archive(adoTransaction);
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        internal new static void ArchiveChildren(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddWriterLock(ServerMarketData.InstitutionLock);
        }
        
        /// <summary>Inserts a Institution record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public new static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int institutionId = parameters["institutionId"];
            // Call the internal method to complete the operation.
            Institution.Archive(adoTransaction, sqlTransaction, rowVersion, institutionId);
        }
        
        /// <summary>Archives a Institution record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of this row.</param>
        /// <param name="institutionId">The value for the InstitutionId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public new static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int institutionId)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.FindByInstitutionId(institutionId);
            if ((institutionRow == null))
            {
                throw new Exception(string.Format("The Institution table does not have an element identified by {0}", institutionId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((institutionRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the base class record.  Note that optimistic concurrency is only used
            // by the top level type in the hierarchy, it is bypassed after you pass the first test.
            long baseRowVersion = institutionRow.SourceRow.RowVersion;
            Source.Archive(adoTransaction, sqlTransaction, baseRowVersion, institutionId);
        }
        
        /// <summary>ArchiveChildrens a Institution record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">the version number of this row.</param>
        /// <param name="institutionId">The value for the InstitutionId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        internal new static void ArchiveChildren(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int institutionId)
        {
            // Accessor for the Institution Table.
            ServerMarketData.InstitutionDataTable institutionTable = ServerMarketData.Institution;
            // This record can be used to iterate through all the children.
            ServerMarketData.InstitutionRow institutionRow = institutionTable.FindByInstitutionId(institutionId);
            // Archive the child records.
            // Increment the row version
            rowVersion = ServerMarketData.RowVersion.Increment();
            // Delete the record in the ADO database.
            institutionRow[institutionTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(institutionRow);
            institutionRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"Institution\" set \"IsArchived\" = 1 where \"InstitutionId\"=@institutionId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@institutionId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, institutionId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
