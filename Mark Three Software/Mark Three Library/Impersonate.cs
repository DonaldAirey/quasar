namespace MarkThree
{

	using System;
	using System.Runtime.InteropServices;
	using System.Security.Principal;
	using System.Security.Permissions;

	public class ImpersonateUser
	{

		// Constants
		private const int providerDefault = 0;
		private const int logonInteractive = 2;
		private const int securityImpersonation = 2;

		/// <summary>
		/// Login a user.
		/// </summary>
		/// <param name="lpszUsername">User name.</param>
		/// <param name="lpszDomain">User Domain.</param>
		/// <param name="lpszPassword">User's password.</param>
		/// <param name="dwLogonType"></param>
		/// <param name="dwLogonProvider"></param>
		/// <param name="phToken">A token used to identify the security context of this user.</param>
		/// <returns>true if the login is successful, false otherwise.</returns>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LogonUser(String userName, String domain, String password, int logonType, int logonProvider,
			ref IntPtr token);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private extern static bool CloseHandle(IntPtr handle);

		// If you incorporate this code into a DLL, be sure to demand FullTrust.
		[PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
		public static WindowsImpersonationContext CreateContext(string account, string password)
		{

			IntPtr tokenHandle = IntPtr.Zero;
			IntPtr dupeTokenHandle = IntPtr.Zero;
			WindowsImpersonationContext windowsImpersonationContext = null;

			try
			{

				// Break apart the account information into a domain and user name.
				string[] breakAccount = account.Split('\\');
				string domainName = breakAccount.Length == 1 ? Environment.MachineName : breakAccount[0].Trim();
				string userName = breakAccount[breakAccount.Length == 1 ? 0 : 1].Trim();

				// Call the unmanaged 'LoginUser' code to obtain a security token.
				if (!LogonUser(userName, domainName, password, logonInteractive, providerDefault, ref tokenHandle))
					throw new Exception("User could not be authenticated");

				// The token that is passed to the following constructor must 
				// be a primary token in order to use it for impersonation.
				WindowsIdentity windowsIdentity = new WindowsIdentity(tokenHandle);
				windowsImpersonationContext = windowsIdentity.Impersonate();

			}
			catch
			{

				// The calling process is not interested in detailed login error codes, just a valid impersonation context.  This
				// indicates that, for some reason, that couldn't be found.
				windowsImpersonationContext = null;

			}
			finally
			{

				// The tokens used to log in the user are no longer needed.
				if (tokenHandle != IntPtr.Zero)
					CloseHandle(tokenHandle);
				if (dupeTokenHandle != IntPtr.Zero)
					CloseHandle(dupeTokenHandle);

			}

			// This process is now running with the identity of the specified Windows User Account.  Calling the 'Undo' on this
			// context will revert to the original user identity.
			return windowsImpersonationContext;

		}

	}

}
