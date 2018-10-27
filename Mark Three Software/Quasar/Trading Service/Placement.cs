/*************************************************************************************************************************
*
*	File:			Placement.cs
*	Description:	Methods to add, modify and delete placements.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.WebService.Trading
{
    using Shadows.Quasar.Common;
    using Shadows.Quasar.Server;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    
    
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    public class Placement
    {
        
        /// <summary>Collects the table lock request(s) for an Insert operation</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        public static void Insert(Transaction transaction)
        {
			// These tables are needed for the 'Update' Operation.
			transaction.Locks.AddWriterLock(ServerMarketData.BlockOrderLock);
			transaction.Locks.AddWriterLock(ServerMarketData.OrderLock);
			// These table lock(s) are required for the 'Insert' operation.
            Shadows.WebService.Core.Placement.Insert(transaction);
        }
        
        /// <summary>Inserts a Placement record using Metadata Parameters.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit.</param>
        /// <param name="remoteMethod">Contains the metadata parameters and exceptions for this command.</param>
        public static void Insert(Transaction transaction, RemoteMethod remoteMethod)
        {
            try
            {
                // Extract the parameters from the command batch.
                int blockOrderId = remoteMethod.Parameters.GetRequiredInt32("blockOrderId");
                int brokerId = remoteMethod.Parameters.GetRequiredInt32("brokerId");
                int timeInForceCode = remoteMethod.Parameters.GetRequiredInt32("timeInForceCode");
                int orderTypeCode = remoteMethod.Parameters.GetRequiredInt32("orderTypeCode");
                object isRouted = remoteMethod.Parameters.GetOptionalBoolean("isRouted");
                System.Decimal quantity = remoteMethod.Parameters.GetRequiredDecimal("quantity");
                object price1 = remoteMethod.Parameters.GetOptionalDecimal("price1");
                object price2 = remoteMethod.Parameters.GetOptionalDecimal("price2");
                // Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
                // from being called with bad data, but provides for error checking on all the parameters.
                if ((remoteMethod.HasExceptions == false))
                {
                    // The rowVersion is passed back to the caller in the event it's needed for additional commands in the batch.
                    long rowVersion = long.MinValue;
                    // Call the internal method to complete the operation.
                    int placementId = Placement.Insert(transaction, blockOrderId, brokerId, timeInForceCode, orderTypeCode, ref rowVersion, isRouted, quantity, price1, price2);
                    // Return values.
                    remoteMethod.Parameters.ReturnValue("rowVersion", rowVersion);
                    remoteMethod.Parameters.ReturnValue(placementId);
                }
            }
            catch (SqlException sqlException)
            {
                // Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
                for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
                {
                }
            }
            catch (Exception exception)
            {
                // This will pass the general exception back to the caller.
                remoteMethod.Exceptions.Add(exception);
            }
        }
        
        /// <summary>Inserts a Placement record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="blockOrderId">The value for the BlockOrderId column.</param>
        /// <param name="brokerId">The value for the BrokerId column.</param>
        /// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
        /// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
        /// <param name="rowVersion">The value for the RowVersion column.</param>
        /// <param name="isRouted">The value for the IsRouted column.</param>
        /// <param name="quantity">The value for the Quantity column.</param>
        /// <param name="price1">The value for the Price1 column.</param>
        /// <param name="price2">The value for the Price2 column.</param>
        /// <param name="createdTime">The value for the CreatedTime column.</param>
        /// <param name="createdLoginId">The value for the CreatedLoginId column.</param>
        /// <param name="modifiedTime">The value for the ModifiedTime column.</param>
        /// <param name="modifiedLoginId">The value for the ModifiedLoginId column.</param>
        public static int Insert(Transaction transaction, int blockOrderId, int brokerId, int timeInForceCode, int orderTypeCode, ref long rowVersion, object isRouted, System.Decimal quantity, object price1, object price2)
        {

			// Provide the created time and login identifier to the base class.
			int createdLoginId = ServerMarketData.LoginId;
			DateTime createdTime = DateTime.Now;

			// Rule #1: The block order exist.
			ServerMarketData.BlockOrderRow blockOrderRow = ServerMarketData.BlockOrder.FindByBlockOrderId(blockOrderId);
			if (blockOrderRow == null)
				throw new Exception("This block order has been deleted by someone else");

			// Rule #2: The Block Order is active
			if (blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Closed ||
				blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Confirmed ||
				blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Pending)
				throw new Exception("This order isn't active");

			// Rule #3: Quantity executed doesn't exceed the quantity ordered.
			decimal quantityOrdered = 0.0M;
			foreach (ServerMarketData.OrderRow orderSumRow in blockOrderRow.GetOrderRows())
				quantityOrdered += orderSumRow.Quantity;
			decimal quantityPlaced = 0.0M;
			foreach (ServerMarketData.PlacementRow placementSumRow in blockOrderRow.GetPlacementRows())
				quantityPlaced += placementSumRow.Quantity;
			if (quantityPlaced + quantity > quantityOrdered)
				throw new Exception("The quantity placed is more than the quantity ordered.");

			// Call the core class to complete the method.
			return Shadows.WebService.Core.Placement.Insert(transaction, blockOrderId, brokerId, timeInForceCode, orderTypeCode,
				ref rowVersion, null, isRouted, quantity, price1, price2, createdTime, createdLoginId, createdTime, createdLoginId);

        }
        
        /// <summary>Collects the table lock request(s) for an Update operation</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        public static void Update(Transaction transaction)
        {
			// These tables are needed for the 'Update' Operation.
			transaction.Locks.AddWriterLock(ServerMarketData.BlockOrderLock);
			transaction.Locks.AddWriterLock(ServerMarketData.OrderLock);
			// These table lock(s) are required for the 'Update' operation.
            Shadows.WebService.Core.Placement.Update(transaction);
        }
        
        /// <summary>Updates a Placement record using Metadata Parameters.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="remoteMethod">Contains the parameters and exceptions for this command.</param>
        public static void Update(Transaction transaction, RemoteMethod remoteMethod)
        {
            try
            {
                // Extract the parameters from the command batch.
                int placementId = remoteMethod.Parameters.GetRequiredInt32("placementId");
                object blockOrderId = remoteMethod.Parameters.GetOptionalInt32("blockOrderId");
                object brokerId = remoteMethod.Parameters.GetOptionalInt32("brokerId");
                object timeInForceCode = remoteMethod.Parameters.GetOptionalInt32("timeInForceCode");
                object orderTypeCode = remoteMethod.Parameters.GetOptionalInt32("orderTypeCode");
                long rowVersion = remoteMethod.Parameters.GetRequiredInt64("rowVersion");
                object isRouted = remoteMethod.Parameters.GetOptionalBoolean("isRouted");
                object quantity = remoteMethod.Parameters.GetOptionalDecimal("quantity");
                object price1 = remoteMethod.Parameters.GetOptionalDecimal("price1");
                object price2 = remoteMethod.Parameters.GetOptionalDecimal("price2");
                // Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
                // from being called with bad data, but provides for error checking on all the parameters.
                if ((remoteMethod.HasExceptions == false))
                {
                    // Call the internal method to complete the operation.
                    Placement.Update(transaction, placementId, blockOrderId, brokerId, timeInForceCode, orderTypeCode, ref rowVersion, isRouted, quantity, price1, price2);
                    // Return values.
                    remoteMethod.Parameters.ReturnValue("rowVersion", rowVersion);
                }
            }
            catch (SqlException sqlException)
            {
                // Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
                for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
                {
                }
            }
            catch (Exception exception)
            {
                // This will pass the general exception back to the caller.
                remoteMethod.Exceptions.Add(exception);
            }
        }
        
        /// <summary>Updates a Placement record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="placementId">The value for the PlacementId column.</param>
        /// <param name="blockOrderId">The value for the BlockOrderId column.</param>
        /// <param name="brokerId">The value for the BrokerId column.</param>
        /// <param name="timeInForceCode">The value for the TimeInForceCode column.</param>
        /// <param name="orderTypeCode">The value for the OrderTypeCode column.</param>
        /// <param name="rowVersion">The value for the RowVersion column.</param>
        /// <param name="isRouted">The value for the IsRouted column.</param>
        /// <param name="quantity">The value for the Quantity column.</param>
        /// <param name="price1">The value for the Price1 column.</param>
        /// <param name="price2">The value for the Price2 column.</param>
        public static void Update(Transaction transaction, int placementId, object blockOrderId, object brokerId, object timeInForceCode, object orderTypeCode, ref long rowVersion, object isRouted, object quantity, object price1, object price2)
        {

			// These values are provided by the server.
			DateTime modifiedTime = DateTime.Now;
			int modifiedLoginId = ServerMarketData.LoginId;

			// Rule #1: Make sure the record exists before updating it.
			ServerMarketData.PlacementRow placementRow = ServerMarketData.Placement.FindByPlacementId(placementId);
			if ((placementRow == null))
				throw new Exception("This Placement has been deleted by someone else");

			// Rule #2: The block order exist.
			ServerMarketData.BlockOrderRow blockOrderRow = ServerMarketData.BlockOrder.FindByBlockOrderId((int)blockOrderId);
			if (blockOrderRow == null)
				throw new Exception("This block order has been deleted by someone else");

			// Rule #3: The Block Order is active
			if (blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Closed ||
				blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Confirmed ||
				blockOrderRow.StatusCode == Shadows.Quasar.Common.Status.Pending)
				throw new Exception("This order isn't active");

			// Rule #4: Quantity executed doesn't exceed the quantity ordered.
			decimal quantityOrdered = 0.0M;
			foreach (ServerMarketData.OrderRow orderSumRow in blockOrderRow.GetOrderRows())
				quantityOrdered += orderSumRow.Quantity;
			decimal quantityPlaced = 0.0M;
			foreach (ServerMarketData.PlacementRow placementSumRow in blockOrderRow.GetPlacementRows())
				quantityPlaced += placementSumRow.Quantity;
			if (quantityPlaced - placementRow.Quantity + (decimal)quantity > quantityOrdered)
				throw new Exception("The quantity placed is more than the quantity ordered.");

			// Call the primitive library to handle the rest of the operation.
			Shadows.WebService.Core.Placement.Update(transaction, placementId, blockOrderId, brokerId, timeInForceCode, orderTypeCode,
				ref rowVersion, null, isRouted, quantity, price1, price2, null, null, modifiedTime, modifiedLoginId);

        }
        
        /// <summary>Collects the table lock request(s) for an Delete operation</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        public static void Delete(Transaction transaction)
        {

			// This method will call the primitive library to update the record, then delete it.
            Shadows.WebService.Core.Placement.Update(transaction);
			Shadows.WebService.Core.Placement.Delete(transaction);

        }
        
        /// <summary>Deletes a Placement record using Metadata Parameters.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="remoteMethod">Contains the parameters and exceptions for this command.</param>
        public static void Delete(Transaction transaction, RemoteMethod remoteMethod)
        {

			try
            {
                // Extract the parameters from the command batch.
                int placementId = remoteMethod.Parameters.GetRequiredInt32("placementId");
                long rowVersion = remoteMethod.Parameters.GetRequiredInt64("rowVersion");

				// Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
                // from being called with bad data, but provides for error checking on all the parameters.
                if ((remoteMethod.HasExceptions == false))
                {
                    // Call the internal method to complete the operation.
                    Placement.Delete(transaction, placementId, rowVersion);
                }
            }
            catch (SqlException sqlException)
            {
                // Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
                for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
                {
                }
            }
            catch (Exception exception)
            {
                // This will pass the general exception back to the caller.
                remoteMethod.Exceptions.Add(exception);
            }
        }
        
        /// <summary>Deletes a Placement record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="placementId">The value for the PlacementId column.</param>
        /// <param name="rowVersion">The value for the RowVersion column.</param>
        public static void Delete(Transaction transaction, int placementId, long rowVersion)
        {

			// These values are provided by the server.
			DateTime modifiedTime = DateTime.Now;
			int modifiedLoginId = ServerMarketData.LoginId;

			// Record the user id and the time of deletion in the record and delete it.
			Shadows.WebService.Core.Placement.Update(transaction, placementId, null, null, null, null, ref rowVersion, null, null, null, null,
				null, null, null, modifiedTime, modifiedLoginId);
			Shadows.WebService.Core.Placement.Delete(transaction, placementId, rowVersion);

		}
        
        /// <summary>Collects the table lock request(s) for an Archive operation</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        public static void Archive(Transaction transaction)
        {
            // These table lock(s) are required for the 'Archive' operation.
            transaction.Locks.AddWriterLock(ServerMarketData.PlacementLock);
        }
        
        /// <summary>Archives a Placement record using Metadata Parameters.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="remoteMethod">Contains the parameters and exceptions for this command.</param>
        public static void Archive(Transaction transaction, RemoteMethod remoteMethod)
        {
            try
            {
                // Extract the parameters from the command batch.
                int placementId = remoteMethod.Parameters.GetRequiredInt32("placementId");
                long rowVersion = remoteMethod.Parameters.GetRequiredInt64("rowVersion");
                // Make sure the parameters were parsed correctly before calling the internal method. This will prevent the method
                // from being called with bad data, but provides for error checking on all the parameters.
                if ((remoteMethod.HasExceptions == false))
                {
                    // Call the internal method to complete the operation.
                    Placement.Archive(transaction, placementId, rowVersion);
                }
            }
            catch (SqlException sqlException)
            {
                // Every exception from the SQL server call is packed into the 'RemoteMethod' structure and returned to the caller.
                for (IEnumerator iEnumerator = sqlException.Errors.GetEnumerator(); iEnumerator.MoveNext(); remoteMethod.Exceptions.Add(((SqlError)(iEnumerator.Current)).Message))
                {
                }
            }
            catch (Exception exception)
            {
                // This will pass the general exception back to the caller.
                remoteMethod.Exceptions.Add(exception);
            }
        }
        
        /// <summary>Archives a Placement record.</summary>
        /// <param name="transaction">Commits or rejects a set of commands as a unit</param>
        /// <param name="placementId">The value for the PlacementId column.</param>
        /// <param name="rowVersion">The value for the RowVersion column.</param>
        /// <param name="archive">true to archive the object, false to unarchive it.</param>
        public static void Archive(Transaction transaction, int placementId, long rowVersion)
        {
            // Accessor for the Placement Table.
            ServerMarketData.PlacementDataTable placementTable = ServerMarketData.Placement;
            // Rule #1: Make sure the record exists before updating it.
            ServerMarketData.PlacementRow placementRow = placementTable.FindByPlacementId(placementId);
            if ((placementRow == null))
            {
                throw new Exception(string.Format("The Placement table does not have an element identified by {0}", placementId));
            }
            // Rule #2: Optimistic Concurrency Check
            if ((placementRow.RowVersion != rowVersion))
            {
                throw new System.Exception("This record is busy.  Please try again later.");
            }
            // Archive the child records.
            // Delete the record in the ADO database.
            transaction.DataRows.Add(placementRow);
            placementRow.Delete();
            // Archive the record in the SQL database.
            SqlCommand sqlCommand = new SqlCommand("update Placement set Archived = 1 where \"PlacementId\"=@placementId");
            sqlCommand.Connection = transaction.SqlConnection;
            sqlCommand.Transaction = transaction.SqlTransaction;
            sqlCommand.Parameters.Add("@placementId", @placementId);
            sqlCommand.ExecuteNonQuery();
        }
    }
}
