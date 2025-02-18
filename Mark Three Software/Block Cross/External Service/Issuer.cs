﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarkThree.Guardian.External
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
    public class Issuer : Object
    {
        
        /// <summary>Collects the table lock request(s) for an 'Load' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Load(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.Issuer.Insert(adoTransaction);
            MarkThree.Guardian.Core.Issuer.Update(adoTransaction);
            // These table lock(s) are required for the 'Load' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.IssuerLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TypeLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
        }
        
        /// <summary>Loads a Issuer record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Load(ParameterList parameters)
        {
            // Accessor for the Issuer Table.
            ServerMarketData.IssuerDataTable issuerTable = ServerMarketData.Issuer;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            object description = parameters["description"].Value;
            object groupPermission = parameters["groupPermission"].Value;
            object hidden = parameters["hidden"].Value;
            string name = parameters["name"];
            object owner = parameters["owner"].Value;
            object ownerPermission = parameters["ownerPermission"].Value;
            object readOnly = parameters["readOnly"].Value;
            object worldPermission = parameters["worldPermission"].Value;
            object address0 = parameters["address0"].Value;
            object address1 = parameters["address1"].Value;
            object address2 = parameters["address2"].Value;
            object city = parameters["city"].Value;
            object countryId = parameters["countryId"].Value;
            string externalIssuerId = parameters["issuerId"];
            object postalCode = parameters["postalCode"].Value;
            object provinceId = parameters["provinceId"].Value;
            object rating0 = parameters["rating0"].Value;
            object rating1 = parameters["rating1"].Value;
            object rating2 = parameters["rating2"].Value;
            object rating3 = parameters["rating3"].Value;
            object externalTypeCode = parameters["typeCode"].Value;
            object userData0 = parameters["userData0"].Value;
            object userData1 = parameters["userData1"].Value;
            object userData2 = parameters["userData2"].Value;
            object userData3 = parameters["userData3"].Value;
            object userData4 = parameters["userData4"].Value;
            object userData5 = parameters["userData5"].Value;
            object userData6 = parameters["userData6"].Value;
            object userData7 = parameters["userData7"].Value;
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Resolve External Identifiers
            int issuerId = Object.FindKey(configurationId, "issuerId", externalIssuerId);
            object typeCode = Type.FindOptionalKey(configurationId, "typeCode", externalTypeCode);
            ServerMarketData.IssuerRow issuerRow = issuerTable.FindByIssuerId(issuerId);
            // The load operation will create a record if it doesn't exist, or update an existing record.  The external
            // identifier is used to determine if a record exists with the same key.
            if ((issuerRow == null))
            {
                // Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
                // external method is called with the same 'configurationId' parameter.
                int externalKeyIndex = Issuer.GetExternalKeyIndex(configurationId, "issuerId");
                object[] externalIdArray = new object[8];
                externalIdArray[externalKeyIndex] = externalIssuerId;
                object externalId0 = externalIdArray[0];
                object externalId1 = externalIdArray[1];
                object externalId2 = externalIdArray[2];
                object externalId3 = externalIdArray[3];
                object externalId4 = externalIdArray[4];
                object externalId5 = externalIdArray[5];
                object externalId6 = externalIdArray[6];
                object externalId7 = externalIdArray[7];
                // Call the internal method to complete the operation.
                MarkThree.Guardian.Core.Issuer.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, postalCode, provinceId, rating0, rating1, rating2, rating3, typeCode, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7);
            }
            else
            {
                // While the optimistic concurrency checking is disabled for the external methods, the internal methods
                // still need to perform the check.  This ncurrency checking logic by finding the current row version to be
                // will bypass the coused when the internal method is called.
                rowVersion = ((long)(issuerRow[issuerTable.RowVersionColumn]));
                // Call the internal method to complete the operation.
                MarkThree.Guardian.Core.Issuer.Update(adoTransaction, sqlTransaction, ref rowVersion, description, null, null, null, null, null, null, null, null, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, issuerId, postalCode, provinceId, rating0, rating1, rating2, rating3, typeCode, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7);
            }
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Collects the table lock request(s) for an 'Update' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Update(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.Issuer.Insert(adoTransaction);
            MarkThree.Guardian.Core.Issuer.Update(adoTransaction);
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.IssuerLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TypeLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
        }
        
        /// <summary>Updates a Issuer record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Update(ParameterList parameters)
        {
            // Accessor for the Issuer Table.
            ServerMarketData.IssuerDataTable issuerTable = ServerMarketData.Issuer;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            object description = parameters["description"].Value;
            object groupPermission = parameters["groupPermission"].Value;
            object hidden = parameters["hidden"].Value;
            object name = parameters["name"].Value;
            object owner = parameters["owner"].Value;
            object ownerPermission = parameters["ownerPermission"].Value;
            object readOnly = parameters["readOnly"].Value;
            object worldPermission = parameters["worldPermission"].Value;
            object address0 = parameters["address0"].Value;
            object address1 = parameters["address1"].Value;
            object address2 = parameters["address2"].Value;
            object city = parameters["city"].Value;
            object countryId = parameters["countryId"].Value;
            string externalIssuerId = ((string)(parameters["issuerId"]));
            object postalCode = parameters["postalCode"].Value;
            object provinceId = parameters["provinceId"].Value;
            object rating0 = parameters["rating0"].Value;
            object rating1 = parameters["rating1"].Value;
            object rating2 = parameters["rating2"].Value;
            object rating3 = parameters["rating3"].Value;
            object externalTypeCode = parameters["typeCode"].Value;
            object userData0 = parameters["userData0"].Value;
            object userData1 = parameters["userData1"].Value;
            object userData2 = parameters["userData2"].Value;
            object userData3 = parameters["userData3"].Value;
            object userData4 = parameters["userData4"].Value;
            object userData5 = parameters["userData5"].Value;
            object userData6 = parameters["userData6"].Value;
            object userData7 = parameters["userData7"].Value;
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Resolve External Identifiers
            int issuerId = Object.FindRequiredKey(configurationId, "issuerId", externalIssuerId);
            object typeCode = Type.FindOptionalKey(configurationId, "typeCode", externalTypeCode);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.IssuerRow issuerRow = issuerTable.FindByIssuerId(issuerId);
            rowVersion = ((long)(issuerRow[issuerTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.Issuer.Update(adoTransaction, sqlTransaction, ref rowVersion, description, null, null, null, null, null, null, null, null, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, issuerId, postalCode, provinceId, rating0, rating1, rating2, rating3, typeCode, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Collects the table lock request(s) for an 'Delete' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Delete(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.Issuer.Delete(adoTransaction);
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.IssuerLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
        }
        
        /// <summary>Deletes a Issuer record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Delete(ParameterList parameters)
        {
            // Accessor for the Issuer Table.
            ServerMarketData.IssuerDataTable issuerTable = ServerMarketData.Issuer;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            string externalIssuerId = parameters["issuerId"];
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Find the internal identifier using the primary key elements.
            // identifier is used to determine if a record exists with the same key.
            int issuerId = Issuer.FindRequiredKey(configurationId, "issuerId", externalIssuerId);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.IssuerRow issuerRow = issuerTable.FindByIssuerId(issuerId);
            rowVersion = ((long)(issuerRow[issuerTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.Issuer.Delete(adoTransaction, sqlTransaction, rowVersion, issuerId);
        }
        
        /// <summary>Collects the table lock request(s) for an 'Archive' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Archive(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.Issuer.Archive(adoTransaction);
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.IssuerLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
        }
        
        /// <summary>Archives a Issuer record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Archive(ParameterList parameters)
        {
            // Accessor for the Issuer Table.
            ServerMarketData.IssuerDataTable issuerTable = ServerMarketData.Issuer;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            string externalIssuerId = parameters["issuerId"];
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Find the internal identifier using the primary key elements.
            // identifier is used to determine if a record exists with the same key.
            int issuerId = Issuer.FindRequiredKey(configurationId, "issuerId", externalIssuerId);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.IssuerRow issuerRow = issuerTable.FindByIssuerId(issuerId);
            rowVersion = ((long)(issuerRow[issuerTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.Issuer.Archive(adoTransaction, sqlTransaction, rowVersion, issuerId);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
