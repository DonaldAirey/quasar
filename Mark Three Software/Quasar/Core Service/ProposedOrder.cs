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
    public class ProposedOrder
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
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ProposedOrder));
        }
        
        /// <summary>Inserts a ProposedOrder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Insert(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object blotterId = parameters["blotterId"].Value;
            int accountId = parameters["accountId"];
            int securityId = parameters["securityId"];
            int settlementId = parameters["settlementId"];
            object brokerId = parameters["brokerId"].Value;
            int positionTypeCode = parameters["positionTypeCode"];
            int transactionTypeCode = parameters["transactionTypeCode"];
            int timeInForceCode = parameters["timeInForceCode"];
            int orderTypeCode = parameters["orderTypeCode"];
            object conditionCode = parameters["conditionCode"].Value;
            object isAgency = parameters["isAgency"].Value;
            decimal quantity = parameters["quantity"];
            object price1 = parameters["price1"].Value;
            object price2 = parameters["price2"].Value;
            object note = parameters["note"].Value;
            // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
            long rowVersion = long.MinValue;
            // Call the internal method to complete the operation.
            int proposedOrderId = ProposedOrder.Insert(adoTransaction, sqlTransaction, ref rowVersion, blotterId, accountId, securityId, settlementId, brokerId, positionTypeCode, transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, isAgency, quantity, price1, price2, note);
            // Return values.
            parameters["rowVersion"] = rowVersion;
            parameters.Return = proposedOrderId;
        }
        
        /// <summary>Inserts a ProposedOrder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="blotterId">The value for the BlotterId column.</param>
        /// <param name="accountId">The value for the AccountId column.</param>
        /// <param name="securityId">The value for the SecurityId column.</param>
        /// <param name="settlementId">The value for the SettlementId column.</param>
        /// <param name="brokerId">The value for the BrokerId column.</param>
        /// <param name="positionTypeCode">The value for the PositionTypeCode column.</param>
        /// <param name="transactionTypeCode">The value for the TransactionTypeCode column.</param>
        /// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
        /// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
        /// <param name="conditionCode">The value for the ConditionCode column.</param>
        /// <param name="isAgency">The value for the IsAgency column.</param>
        /// <param name="quantity">The value for the Quantity column.</param>
        /// <param name="price1">The value for the Price1 column.</param>
        /// <param name="price2">The value for the Price2 column.</param>
        /// <param name="note">The value for the Note column.</param>
        public static int Insert(
                    AdoTransaction adoTransaction, 
                    SqlTransaction sqlTransaction, 
                    ref long rowVersion, 
                    object blotterId, 
                    int accountId, 
                    int securityId, 
                    int settlementId, 
                    object brokerId, 
                    int positionTypeCode, 
                    int transactionTypeCode, 
                    int timeInForceCode, 
                    int orderTypeCode, 
                    object conditionCode, 
                    object isAgency, 
                    decimal quantity, 
                    object price1, 
                    object price2, 
                    object note)
        {
            // Accessor for the ProposedOrder Table.
            ServerDataModel.ProposedOrderDataTable proposedOrderTable = ServerDataModel.ProposedOrder;
            // Apply Defaults
            if ((blotterId == null))
            {
                blotterId = System.DBNull.Value;
            }
            if ((brokerId == null))
            {
                brokerId = System.DBNull.Value;
            }
            if ((conditionCode == null))
            {
                conditionCode = System.DBNull.Value;
            }
            if ((isAgency == null))
            {
                isAgency = false;
            }
            if ((price1 == null))
            {
                price1 = System.DBNull.Value;
            }
            if ((price2 == null))
            {
                price2 = System.DBNull.Value;
            }
            if ((note == null))
            {
                note = System.DBNull.Value;
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Insert the record into the ADO database.
            ServerDataModel.ProposedOrderRow proposedOrderRow = proposedOrderTable.NewProposedOrderRow();
            proposedOrderRow[proposedOrderTable.RowVersionColumn] = rowVersion;
            proposedOrderRow[proposedOrderTable.BlotterIdColumn] = blotterId;
            proposedOrderRow[proposedOrderTable.AccountIdColumn] = accountId;
            proposedOrderRow[proposedOrderTable.SecurityIdColumn] = securityId;
            proposedOrderRow[proposedOrderTable.SettlementIdColumn] = settlementId;
            proposedOrderRow[proposedOrderTable.BrokerIdColumn] = brokerId;
            proposedOrderRow[proposedOrderTable.PositionTypeCodeColumn] = positionTypeCode;
            proposedOrderRow[proposedOrderTable.TransactionTypeCodeColumn] = transactionTypeCode;
            proposedOrderRow[proposedOrderTable.TimeInForceCodeColumn] = timeInForceCode;
            proposedOrderRow[proposedOrderTable.OrderTypeCodeColumn] = orderTypeCode;
            proposedOrderRow[proposedOrderTable.ConditionCodeColumn] = conditionCode;
            proposedOrderRow[proposedOrderTable.IsAgencyColumn] = isAgency;
            proposedOrderRow[proposedOrderTable.QuantityColumn] = quantity;
            proposedOrderRow[proposedOrderTable.Price1Column] = price1;
            proposedOrderRow[proposedOrderTable.Price2Column] = price2;
            proposedOrderRow[proposedOrderTable.NoteColumn] = note;
            proposedOrderTable.AddProposedOrderRow(proposedOrderRow);
            adoTransaction.DataRows.Add(proposedOrderRow);
            // Insert the record into the SQL database.
            SqlCommand sqlCommand = new SqlCommand(@"insert ""ProposedOrder"" (""rowVersion"",""ProposedOrderId"",""BlotterId"",""AccountId"",""SecurityId"",""SettlementId"",""BrokerId"",""PositionTypeCode"",""TransactionTypeCode"",""TimeInForceCode"",""OrderTypeCode"",""ConditionCode"",""IsAgency"",""Quantity"",""Price1"",""Price2"",""Note"") values (@rowVersion,@proposedOrderId,@blotterId,@accountId,@securityId,@settlementId,@brokerId,@positionTypeCode,@transactionTypeCode,@timeInForceCode,@orderTypeCode,@conditionCode,@isAgency,@quantity,@price1,@price2,@note)");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@proposedOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, proposedOrderRow[proposedOrderTable.ProposedOrderIdColumn]));
            sqlCommand.Parameters.Add(new SqlParameter("@blotterId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, blotterId));
            sqlCommand.Parameters.Add(new SqlParameter("@accountId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountId));
            sqlCommand.Parameters.Add(new SqlParameter("@securityId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, securityId));
            sqlCommand.Parameters.Add(new SqlParameter("@settlementId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, settlementId));
            sqlCommand.Parameters.Add(new SqlParameter("@brokerId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, brokerId));
            sqlCommand.Parameters.Add(new SqlParameter("@positionTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, positionTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@transactionTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, transactionTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@timeInForceCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, timeInForceCode));
            sqlCommand.Parameters.Add(new SqlParameter("@orderTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, orderTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@conditionCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, conditionCode));
            sqlCommand.Parameters.Add(new SqlParameter("@isAgency", SqlDbType.Bit, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, isAgency));
            sqlCommand.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, quantity));
            sqlCommand.Parameters.Add(new SqlParameter("@price1", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, price1));
            sqlCommand.Parameters.Add(new SqlParameter("@price2", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, price2));
            sqlCommand.Parameters.Add(new SqlParameter("@note", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, note));
            sqlCommand.ExecuteNonQuery();
            // Return Statements
            return proposedOrderRow.ProposedOrderId;
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Update(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ProposedOrder));
            ProposedOrderTree.Update(adoTransaction);
        }
        
        /// <summary>Inserts a ProposedOrder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Update(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int proposedOrderId = parameters["proposedOrderId"];
            object blotterId = parameters["blotterId"].Value;
            object accountId = parameters["accountId"].Value;
            object securityId = parameters["securityId"].Value;
            object settlementId = parameters["settlementId"].Value;
            object brokerId = parameters["brokerId"].Value;
            object positionTypeCode = parameters["positionTypeCode"].Value;
            object transactionTypeCode = parameters["transactionTypeCode"].Value;
            object timeInForceCode = parameters["timeInForceCode"].Value;
            object orderTypeCode = parameters["orderTypeCode"].Value;
            object conditionCode = parameters["conditionCode"].Value;
            object isAgency = parameters["isAgency"].Value;
            object quantity = parameters["quantity"].Value;
            object price1 = parameters["price1"].Value;
            object price2 = parameters["price2"].Value;
            object note = parameters["note"].Value;
            // Call the internal method to complete the operation.
            ProposedOrder.Update(adoTransaction, sqlTransaction, ref rowVersion, proposedOrderId, blotterId, accountId, securityId, settlementId, brokerId, positionTypeCode, transactionTypeCode, timeInForceCode, orderTypeCode, conditionCode, isAgency, quantity, price1, price2, note);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Updates a ProposedOrder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="rowVersion">The version number of the row</param>
        /// <param name="proposedOrderId">The value for the ProposedOrderId column.</param>
        /// <param name="blotterId">The value for the BlotterId column.</param>
        /// <param name="accountId">The value for the AccountId column.</param>
        /// <param name="securityId">The value for the SecurityId column.</param>
        /// <param name="settlementId">The value for the SettlementId column.</param>
        /// <param name="brokerId">The value for the BrokerId column.</param>
        /// <param name="positionTypeCode">The value for the PositionTypeCode column.</param>
        /// <param name="transactionTypeCode">The value for the TransactionTypeCode column.</param>
        /// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
        /// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
        /// <param name="conditionCode">The value for the ConditionCode column.</param>
        /// <param name="isAgency">The value for the IsAgency column.</param>
        /// <param name="quantity">The value for the Quantity column.</param>
        /// <param name="price1">The value for the Price1 column.</param>
        /// <param name="price2">The value for the Price2 column.</param>
        /// <param name="note">The value for the Note column.</param>
        public static void Update(
                    AdoTransaction adoTransaction, 
                    SqlTransaction sqlTransaction, 
                    ref long rowVersion, 
                    int proposedOrderId, 
                    object blotterId, 
                    object accountId, 
                    object securityId, 
                    object settlementId, 
                    object brokerId, 
                    object positionTypeCode, 
                    object transactionTypeCode, 
                    object timeInForceCode, 
                    object orderTypeCode, 
                    object conditionCode, 
                    object isAgency, 
                    object quantity, 
                    object price1, 
                    object price2, 
                    object note)
        {
            // Accessor for the ProposedOrder Table.
            ServerDataModel.ProposedOrderDataTable proposedOrderTable = ServerDataModel.ProposedOrder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.ProposedOrderRow proposedOrderRow = proposedOrderTable.FindByProposedOrderId(proposedOrderId);
            if ((proposedOrderRow == null))
            {
                throw new Exception(string.Format("The ProposedOrder table does not have an element identified by {0}", proposedOrderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((proposedOrderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Apply Defaults
            if ((blotterId == null))
            {
                blotterId = proposedOrderRow[proposedOrderTable.BlotterIdColumn];
            }
            if ((accountId == null))
            {
                accountId = proposedOrderRow[proposedOrderTable.AccountIdColumn];
            }
            if ((securityId == null))
            {
                securityId = proposedOrderRow[proposedOrderTable.SecurityIdColumn];
            }
            if ((settlementId == null))
            {
                settlementId = proposedOrderRow[proposedOrderTable.SettlementIdColumn];
            }
            if ((brokerId == null))
            {
                brokerId = proposedOrderRow[proposedOrderTable.BrokerIdColumn];
            }
            if ((positionTypeCode == null))
            {
                positionTypeCode = proposedOrderRow[proposedOrderTable.PositionTypeCodeColumn];
            }
            if ((transactionTypeCode == null))
            {
                transactionTypeCode = proposedOrderRow[proposedOrderTable.TransactionTypeCodeColumn];
            }
            if ((timeInForceCode == null))
            {
                timeInForceCode = proposedOrderRow[proposedOrderTable.TimeInForceCodeColumn];
            }
            if ((orderTypeCode == null))
            {
                orderTypeCode = proposedOrderRow[proposedOrderTable.OrderTypeCodeColumn];
            }
            if ((conditionCode == null))
            {
                conditionCode = proposedOrderRow[proposedOrderTable.ConditionCodeColumn];
            }
            if ((isAgency == null))
            {
                isAgency = proposedOrderRow[proposedOrderTable.IsAgencyColumn];
            }
            if ((quantity == null))
            {
                quantity = proposedOrderRow[proposedOrderTable.QuantityColumn];
            }
            if ((price1 == null))
            {
                price1 = proposedOrderRow[proposedOrderTable.Price1Column];
            }
            if ((price2 == null))
            {
                price2 = proposedOrderRow[proposedOrderTable.Price2Column];
            }
            if ((note == null))
            {
                note = proposedOrderRow[proposedOrderTable.NoteColumn];
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Update the record in the ADO database.
            proposedOrderRow[proposedOrderTable.RowVersionColumn] = rowVersion;
            proposedOrderRow[proposedOrderTable.BlotterIdColumn] = blotterId;
            proposedOrderRow[proposedOrderTable.AccountIdColumn] = accountId;
            proposedOrderRow[proposedOrderTable.SecurityIdColumn] = securityId;
            proposedOrderRow[proposedOrderTable.SettlementIdColumn] = settlementId;
            proposedOrderRow[proposedOrderTable.BrokerIdColumn] = brokerId;
            proposedOrderRow[proposedOrderTable.PositionTypeCodeColumn] = positionTypeCode;
            proposedOrderRow[proposedOrderTable.TransactionTypeCodeColumn] = transactionTypeCode;
            proposedOrderRow[proposedOrderTable.TimeInForceCodeColumn] = timeInForceCode;
            proposedOrderRow[proposedOrderTable.OrderTypeCodeColumn] = orderTypeCode;
            proposedOrderRow[proposedOrderTable.ConditionCodeColumn] = conditionCode;
            proposedOrderRow[proposedOrderTable.IsAgencyColumn] = isAgency;
            proposedOrderRow[proposedOrderTable.QuantityColumn] = quantity;
            proposedOrderRow[proposedOrderTable.Price1Column] = price1;
            proposedOrderRow[proposedOrderTable.Price2Column] = price2;
            proposedOrderRow[proposedOrderTable.NoteColumn] = note;
            adoTransaction.DataRows.Add(proposedOrderRow);
            // Update the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand(@"update ""ProposedOrder"" set ""RowVersion""=@rowVersion,""BlotterId""=@blotterId,""AccountId""=@accountId,""SecurityId""=@securityId,""SettlementId""=@settlementId,""BrokerId""=@brokerId,""PositionTypeCode""=@positionTypeCode,""TransactionTypeCode""=@transactionTypeCode,""TimeInForceCode""=@timeInForceCode,""OrderTypeCode""=@orderTypeCode,""ConditionCode""=@conditionCode,""IsAgency""=@isAgency,""Quantity""=@quantity,""Price1""=@price1,""Price2""=@price2,""Note""=@note where ""ProposedOrderId""=@proposedOrderId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@rowVersion", SqlDbType.BigInt, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, rowVersion));
            sqlCommand.Parameters.Add(new SqlParameter("@proposedOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, proposedOrderId));
            sqlCommand.Parameters.Add(new SqlParameter("@blotterId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, blotterId));
            sqlCommand.Parameters.Add(new SqlParameter("@accountId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, accountId));
            sqlCommand.Parameters.Add(new SqlParameter("@securityId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, securityId));
            sqlCommand.Parameters.Add(new SqlParameter("@settlementId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, settlementId));
            sqlCommand.Parameters.Add(new SqlParameter("@brokerId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, brokerId));
            sqlCommand.Parameters.Add(new SqlParameter("@positionTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, positionTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@transactionTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, transactionTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@timeInForceCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, timeInForceCode));
            sqlCommand.Parameters.Add(new SqlParameter("@orderTypeCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, orderTypeCode));
            sqlCommand.Parameters.Add(new SqlParameter("@conditionCode", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, conditionCode));
            sqlCommand.Parameters.Add(new SqlParameter("@isAgency", SqlDbType.Bit, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, isAgency));
            sqlCommand.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, quantity));
            sqlCommand.Parameters.Add(new SqlParameter("@price1", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, price1));
            sqlCommand.Parameters.Add(new SqlParameter("@price2", SqlDbType.Decimal, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, price2));
            sqlCommand.Parameters.Add(new SqlParameter("@note", SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, note));
            // Update the record in the SQL database.
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Delete(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ProposedOrder));
            ProposedOrderTree.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a ProposedOrder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Delete(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int proposedOrderId = parameters["proposedOrderId"];
            // Call the internal method to complete the operation.
            ProposedOrder.Delete(adoTransaction, sqlTransaction, rowVersion, proposedOrderId);
        }
        
        /// <summary>Deletes a ProposedOrder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="proposedOrderId">The value for the ProposedOrderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Delete(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int proposedOrderId)
        {
            // Accessor for the ProposedOrder Table.
            ServerDataModel.ProposedOrderDataTable proposedOrderTable = ServerDataModel.ProposedOrder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.ProposedOrderRow proposedOrderRow = proposedOrderTable.FindByProposedOrderId(proposedOrderId);
            if ((proposedOrderRow == null))
            {
                throw new Exception(string.Format("The ProposedOrder table does not have an element identified by {0}", proposedOrderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((proposedOrderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Delete the child records.
            for (int index = 0; (index < proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeChildId().Length); index = (index + 1))
            {
                ServerDataModel.ProposedOrderTreeRow childProposedOrderTreeRow = proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeChildId()[index];
                ProposedOrderTree.Delete(adoTransaction, sqlTransaction, childProposedOrderTreeRow.RowVersion, childProposedOrderTreeRow.ParentId, childProposedOrderTreeRow.ChildId);
            }
            for (int index = 0; (index < proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId().Length); index = (index + 1))
            {
                ServerDataModel.ProposedOrderTreeRow childProposedOrderTreeRow = proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId()[index];
                ProposedOrderTree.Delete(adoTransaction, sqlTransaction, childProposedOrderTreeRow.RowVersion, childProposedOrderTreeRow.ParentId, childProposedOrderTreeRow.ChildId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            proposedOrderRow[proposedOrderTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(proposedOrderRow);
            proposedOrderRow.Delete();
            // Delete the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"ProposedOrder\" set \"IsDeleted\" = 1 where \"ProposedOrderId\"=@proposedOrder" +
                    "Id");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@proposedOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, proposedOrderId));
            sqlCommand.ExecuteNonQuery();
        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="adoTransaction">A list of locks required for this operation.</param>
        public static void Archive(AdoTransaction adoTransaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.Add(new TableWriterRequest(ServerDataModel.ProposedOrder));
            ProposedOrderTree.Archive(adoTransaction);
        }
        
        /// <summary>Inserts a ProposedOrder record using Metadata Parameters.</summary>
        /// <param name="parameters">Contains the metadata parameters.</param>
        public static void Archive(ParameterList parameters)
        {
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            long rowVersion = parameters["rowVersion"];
            int proposedOrderId = parameters["proposedOrderId"];
            // Call the internal method to complete the operation.
            ProposedOrder.Archive(adoTransaction, sqlTransaction, rowVersion, proposedOrderId);
        }
        
        /// <summary>Archives a ProposedOrder record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="RowVersion">The version number of this row.</param>
        /// <param name="proposedOrderId">The value for the ProposedOrderId column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(AdoTransaction adoTransaction, SqlTransaction sqlTransaction, long rowVersion, int proposedOrderId)
        {
            // Accessor for the ProposedOrder Table.
            ServerDataModel.ProposedOrderDataTable proposedOrderTable = ServerDataModel.ProposedOrder;
            // Rule #1: Make sure the record exists before updating it.
            ServerDataModel.ProposedOrderRow proposedOrderRow = proposedOrderTable.FindByProposedOrderId(proposedOrderId);
            if ((proposedOrderRow == null))
            {
                throw new Exception(string.Format("The ProposedOrder table does not have an element identified by {0}", proposedOrderId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((proposedOrderRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            for (int index = 0; (index < proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeChildId().Length); index = (index + 1))
            {
                ServerDataModel.ProposedOrderTreeRow childProposedOrderTreeRow = proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeChildId()[index];
                ProposedOrderTree.Archive(adoTransaction, sqlTransaction, childProposedOrderTreeRow.RowVersion, childProposedOrderTreeRow.ParentId, childProposedOrderTreeRow.ChildId);
            }
            for (int index = 0; (index < proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId().Length); index = (index + 1))
            {
                ServerDataModel.ProposedOrderTreeRow childProposedOrderTreeRow = proposedOrderRow.GetProposedOrderTreeRowsByFKProposedOrderProposedOrderTreeParentId()[index];
                ProposedOrderTree.Archive(adoTransaction, sqlTransaction, childProposedOrderTreeRow.RowVersion, childProposedOrderTreeRow.ParentId, childProposedOrderTreeRow.ChildId);
            }
            // Increment the row version
            rowVersion = ServerDataModel.RowVersion.Increment();
            // Delete the record in the ADO database.
            proposedOrderRow[proposedOrderTable.RowVersionColumn] = rowVersion;
            adoTransaction.DataRows.Add(proposedOrderRow);
            proposedOrderRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update \"ProposedOrder\" set \"IsArchived\" = 1 where \"ProposedOrderId\"=@proposedOrde" +
                    "rId");
            sqlCommand.Connection = sqlTransaction.Connection;
            sqlCommand.Transaction = sqlTransaction;
            sqlCommand.Parameters.Add(new SqlParameter("@proposedOrderId", SqlDbType.Int, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, proposedOrderId));
            sqlCommand.ExecuteNonQuery();
        }
    }
}
