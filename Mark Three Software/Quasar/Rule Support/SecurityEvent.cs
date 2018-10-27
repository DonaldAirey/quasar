namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;

	public delegate void SecurityEvent(object sender, SecurityEventArgs securityEventArgs);
	
	public class SecurityEventArgs : EventArgs
	{

		private Security security;

		public Security Security {get {return this.security;}}
		
		public SecurityEventArgs(Security security)
		{
			this.security = security;
		}

	}

}
