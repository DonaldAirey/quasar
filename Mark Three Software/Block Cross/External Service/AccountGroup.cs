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
    public class AccountGroup : AccountBase
    {
        
        /// <summary>Collects the table lock request(s) for an 'Load' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Load(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.AccountGroup.Insert(adoTransaction);
            MarkThree.Guardian.Core.AccountGroup.Update(adoTransaction);
            // These table lock(s) are required for the 'Load' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountGroupLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TypeLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.CountryLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.CurrencyLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ProvinceLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountBaseLock);
        }
        
        /// <summary>Loads a AccountGroup record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Load(ParameterList parameters)
        {
            // Accessor for the AccountGroup Table.
            ServerMarketData.AccountGroupDataTable accountGroupTable = ServerMarketData.AccountGroup;
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
            object externalCountryId = parameters["countryId"].Value;
            string externalCurrencyId = parameters["currencyId"];
            object mnemonic = parameters["mnemonic"].Value;
            object postalCode = parameters["postalCode"].Value;
            object externalProvinceId = parameters["provinceId"].Value;
            string externalUserId = parameters["userId"];
            object userData0 = parameters["userData0"].Value;
            object userData1 = parameters["userData1"].Value;
            object userData2 = parameters["userData2"].Value;
            object userData3 = parameters["userData3"].Value;
            object userData4 = parameters["userData4"].Value;
            object userData5 = parameters["userData5"].Value;
            object userData6 = parameters["userData6"].Value;
            object userData7 = parameters["userData7"].Value;
            string externalAccountGroupId = parameters["accountGroupId"];
            object externalTypeCode = parameters["typeCode"].Value;
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Resolve External Identifiers
            object countryId = Country.FindOptionalKey(configurationId, "countryId", externalCountryId);
            int currencyId = Security.FindRequiredKey(configurationId, "currencyId", externalCurrencyId);
            object provinceId = Province.FindOptionalKey(configurationId, "provinceId", externalProvinceId);
            int userId = User.FindRequiredKey(configurationId, "userId", externalUserId);
            int accountGroupId = AccountBase.FindKey(configurationId, "accountGroupId", externalAccountGroupId);
            object typeCode = Type.FindOptionalKey(configurationId, "typeCode", externalTypeCode);
            ServerMarketData.AccountGroupRow accountGroupRow = accountGroupTable.FindByAccountGroupId(accountGroupId);
            // The load operation will create a record if it doesn't exist, or update an existing record.  The external
            // identifier is used to determine if a record exists with the same key.
            if ((accountGroupRow == null))
            {
                // Populate the 'externalId' varaibles so that the external identifier can be used to find the row when an
                // external method is called with the same 'configurationId' parameter.
                int externalKeyIndex = AccountGroup.GetExternalKeyIndex(configurationId, "accountGroupId");
                object[] externalIdArray = new object[8];
                externalIdArray[externalKeyIndex] = externalAccountGroupId;
                object externalId0 = externalIdArray[0];
                object externalId1 = externalIdArray[1];
                object externalId2 = externalIdArray[2];
                object externalId3 = externalIdArray[3];
                object externalId4 = externalIdArray[4];
                object externalId5 = externalIdArray[5];
                object externalId6 = externalIdArray[6];
                object externalId7 = externalIdArray[7];
                // Call the internal method to complete the operation.
                MarkThree.Guardian.Core.AccountGroup.Insert(adoTransaction, sqlTransaction, ref rowVersion, description, externalId0, externalId1, externalId2, externalId3, externalId4, externalId5, externalId6, externalId7, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, currencyId, mnemonic, postalCode, provinceId, userId, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7, typeCode);
            }
            else
            {
                // While the optimistic concurrency checking is disabled for the external methods, the internal methods
                // still need to perform the check.  This ncurrency checking logic by finding the current row version to be
                // will bypass the coused when the internal method is called.
                rowVersion = ((long)(accountGroupRow[accountGroupTable.RowVersionColumn]));
                // Call the internal method to complete the operation.
                MarkThree.Guardian.Core.AccountGroup.Update(adoTransaction, sqlTransaction, ref rowVersion, description, null, null, null, null, null, null, null, null, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, currencyId, mnemonic, postalCode, provinceId, userId, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7, accountGroupId, typeCode);
            }
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Collects the table lock request(s) for an 'Update' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Update(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.AccountGroup.Insert(adoTransaction);
            MarkThree.Guardian.Core.AccountGroup.Update(adoTransaction);
            // These table lock(s) are required for the 'Update' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountGroupLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.TypeLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.CountryLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ObjectLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.SecurityLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.CurrencyLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ProvinceLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountBaseLock);
        }
        
        /// <summary>Updates a AccountGroup record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Update(ParameterList parameters)
        {
            // Accessor for the AccountGroup Table.
            ServerMarketData.AccountGroupDataTable accountGroupTable = ServerMarketData.AccountGroup;
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
            object externalCountryId = parameters["countryId"].Value;
            object externalCurrencyId = parameters["currencyId"].Value;
            object mnemonic = parameters["mnemonic"].Value;
            object postalCode = parameters["postalCode"].Value;
            object externalProvinceId = parameters["provinceId"].Value;
            object externalUserId = parameters["userId"].Value;
            object userData0 = parameters["userData0"].Value;
            object userData1 = parameters["userData1"].Value;
            object userData2 = parameters["userData2"].Value;
            object userData3 = parameters["userData3"].Value;
            object userData4 = parameters["userData4"].Value;
            object userData5 = parameters["userData5"].Value;
            object userData6 = parameters["userData6"].Value;
            object userData7 = parameters["userData7"].Value;
            string externalAccountGroupId = ((string)(parameters["accountGroupId"]));
            object externalTypeCode = parameters["typeCode"].Value;
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Resolve External Identifiers
            object countryId = Country.FindOptionalKey(configurationId, "countryId", externalCountryId);
            object currencyId = Security.FindOptionalKey(configurationId, "currencyId", externalCurrencyId);
            object provinceId = Province.FindOptionalKey(configurationId, "provinceId", externalProvinceId);
            object userId = User.FindOptionalKey(configurationId, "userId", externalUserId);
            int accountGroupId = AccountBase.FindRequiredKey(configurationId, "accountGroupId", externalAccountGroupId);
            object typeCode = Type.FindOptionalKey(configurationId, "typeCode", externalTypeCode);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.AccountGroupRow accountGroupRow = accountGroupTable.FindByAccountGroupId(accountGroupId);
            rowVersion = ((long)(accountGroupRow[accountGroupTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.AccountGroup.Update(adoTransaction, sqlTransaction, ref rowVersion, description, null, null, null, null, null, null, null, null, groupPermission, hidden, name, owner, ownerPermission, readOnly, worldPermission, address0, address1, address2, city, countryId, currencyId, mnemonic, postalCode, provinceId, userId, userData0, userData1, userData2, userData3, userData4, userData5, userData6, userData7, accountGroupId, typeCode);
            // Return values.
            parameters["rowVersion"] = rowVersion;
        }
        
        /// <summary>Collects the table lock request(s) for an 'Delete' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Delete(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.AccountGroup.Delete(adoTransaction);
            // These table lock(s) are required for the 'Delete' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountGroupLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
        }
        
        /// <summary>Deletes a AccountGroup record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Delete(ParameterList parameters)
        {
            // Accessor for the AccountGroup Table.
            ServerMarketData.AccountGroupDataTable accountGroupTable = ServerMarketData.AccountGroup;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            string externalAccountGroupId = parameters["accountGroupId"];
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Find the internal identifier using the primary key elements.
            // identifier is used to determine if a record exists with the same key.
            int accountGroupId = AccountGroup.FindRequiredKey(configurationId, "accountGroupId", externalAccountGroupId);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.AccountGroupRow accountGroupRow = accountGroupTable.FindByAccountGroupId(accountGroupId);
            rowVersion = ((long)(accountGroupRow[accountGroupTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.AccountGroup.Delete(adoTransaction, sqlTransaction, rowVersion, accountGroupId);
        }
        
        /// <summary>Collects the table lock request(s) for an 'Archive' operation</summary>
        /// <param name="adoTransaction">A collection of table locks required for this operation</param>
        public new static void Archive(AdoTransaction adoTransaction)
        {
            // Call the internal methods to lock the tables required for an insert or update operation.
            MarkThree.Guardian.Core.AccountGroup.Archive(adoTransaction);
            // These table lock(s) are required for the 'Archive' operation.
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.AccountGroupLock);
            adoTransaction.LockRequests.AddReaderLock(ServerMarketData.ConfigurationLock);
        }
        
        /// <summary>Archives a AccountGroup record using Metadata Parameters.</summary>
        /// <param name="transaction">Contains the parameters and exceptions for this command.</param>
        public new static void Archive(ParameterList parameters)
        {
            // Accessor for the AccountGroup Table.
            ServerMarketData.AccountGroupDataTable accountGroupTable = ServerMarketData.AccountGroup;
            // Extract the parameters from the command batch.
            AdoTransaction adoTransaction = parameters["adoTransaction"];
            SqlTransaction sqlTransaction = parameters["sqlTransaction"];
            object configurationId = parameters["configurationId"].Value;
            string externalAccountGroupId = parameters["accountGroupId"];
            // The row versioning is largely disabled for external operations.  The value is returned to the caller in the
            // event it's needed for operations within the batch.
            long rowVersion = long.MinValue;
            // Find the internal identifier using the primary key elements.
            // identifier is used to determine if a record exists with the same key.
            int accountGroupId = AccountGroup.FindRequiredKey(configurationId, "accountGroupId", externalAccountGroupId);
            // This disables the concurrency checking logic by finding the current row version and passing it to the
            // internal method.
            ServerMarketData.AccountGroupRow accountGroupRow = accountGroupTable.FindByAccountGroupId(accountGroupId);
            rowVersion = ((long)(accountGroupRow[accountGroupTable.RowVersionColumn]));
            // Call the internal method to complete the operation.
            MarkThree.Guardian.Core.AccountGroup.Archive(adoTransaction, sqlTransaction, rowVersion, accountGroupId);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
