namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;

	/// <summary>
	/// Summary description for Account.
	/// </summary>
	public class Account : MarkThree.Guardian.Object
	{

		public Account(int objectId) : base(objectId)
		{

			// Initialize the object
			ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(objectId);

		}

	}

}
