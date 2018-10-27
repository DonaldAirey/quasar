namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Collections;

	public class AccountList : ArrayList
	{

        public new int Add(object value)
		{
			throw new Exception("Value is not compatible with AccountList.");
		}
		
		public Account Add(Account account)
		{

			if (!this.Contains(account))
				base.Add(account);

			return account;

		}

	}

}
