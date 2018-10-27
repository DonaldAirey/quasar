namespace MarkThree.Guardian.Server
{

	using MarkThree;
	using System;
	
	/// <summary>
	/// Summary description for Environment.
	/// </summary>
	public class Environment
	{

		public static int UserId
		{

			get
			{

				// Use the unique index to find a match to the aliased user name from the environment.
				int index = ServerMarketData.User.KeyUserUserName.Find(new object[] {System.Environment.UserName});
				if (index == -1)
					throw new Exception(string.Format("The user '{0}' is not mapped to a User", System.Environment.UserName));
				return (int)ServerMarketData.User.KeyUserUserName[index].Row[ServerMarketData.User.UserIdColumn];

			}

		}

		public static string UserName
		{

			get
			{
				// Remove case sensitivity between Windows username and Guardian username.
				return System.Environment.UserName.ToLower();
			}

		}

		/// <summary>
		/// Gets the login identifier of the current user.
		/// </summary>
		public static void GetUserId(AdoTransaction adoTransaction)
		{

			// This table lock is required for this operation.
			adoTransaction.LockRequests.AddReaderLock(ServerMarketData.UserLock);

		}

		public static void GetUserId(ParameterList parameters)
		{

			// Return the user id.
			parameters.Return = UserId;

		}

	}

}
