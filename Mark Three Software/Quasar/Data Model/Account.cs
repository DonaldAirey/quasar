/*************************************************************************************************************************
*
*	File:			Account.cs
*	Description:	Methods and data for supporting Account operations.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Quasar
{

	using System;
	using System.Collections;

	/// <summary>
	/// Methods and data for handling an account.
	/// </summary>
	public class Account
	{

		/// <summary>
		/// Get an array of accounts associated with the parent account
		/// </summary>
		/// <param name="accountId">The parent account id.</param>
		/// <returns>An array containing all the included identifiers in the given parent account's hierarchy.</returns>
		public static int[] GetAccountList(int accountId)
		{

			// Get the account record from the database.
			DataModel.AccountRow accountRow = DataModel.Account.FindByAccountId(accountId);
			if (accountRow == null)
				throw new Exception(String.Format("Account {0} has been deleted", accountId));

			// This array is used to collect all the child account identifiers.  The parent account is always part of the
			// hierarchy, so include it here as the seed for this recursive action.
			ArrayList accountList = new ArrayList();
			accountList.Add(accountId);

			// Recursively search the object hierarchy looking for children of this account.
			GetAccountList(accountList, accountRow);

			// Copy the list into an array.
			int[] accountArray = new int[accountList.Count];
			accountList.CopyTo(accountArray);
			return accountArray;

		}

		/// <summary>
		/// Recursively populate an array of accounts associated with the parent account
		/// </summary>
		/// <param name="accountList">The array used to collect account identifiers.</param>
		/// <param name="parentAccount">The current parent account in the hierachical search.</param>
		private static void GetAccountList(ArrayList accountList, DataModel.AccountRow parentAccount)
		{

			// Populate the array with each of the children account ids and then recurse into those children and collect the
			// grand-children.  This process continues until there are no more children to add to the array.
			foreach (DataModel.ObjectTreeRow parentObjectTree in parentAccount.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				foreach (DataModel.AccountRow childAccount in parentObjectTree.ObjectRowByFKObjectObjectTreeChildId.GetAccountRows())
				{
					accountList.Add(childAccount.AccountId);
					GetAccountList(accountList, childAccount);
				}

		}
	
	}

}
