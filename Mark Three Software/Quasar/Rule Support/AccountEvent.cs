namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;

	public delegate void AccountEvent(object sender, AccountEventArgs securityEventArgs);
	
	public class AccountEventArgs : EventArgs
	{

		private Account security;

		public Account Account {get {return this.security;}}
		
		public AccountEventArgs(Account security)
		{
			this.security = security;
		}

	}

}
