namespace MarkThree.Guardian
{

	using MarkThree;
	using MarkThree.Client;
	using MarkThree.Guardian;
	using System;

	/// <summary>
	/// Summary description for UserPrefereces.
	/// </summary>
	public class Preferences : System.ComponentModel.Component
	{

		private static object timeInForceCode = null;
		private static object blotterId = null;
		private static object brokerId = null;
		private static Pricing pricing = Pricing.Last;

		/// <summary>
		/// The identifier of this user.
		/// </summary>
		public static int UserId
		{
			get
			{

				try
				{

					// The starting point for the folder is the user ID.  The id will direct us to the user's personal folder and
					// that will drive the rest of the tree structure.
					Batch batch = new Batch();
					AssemblyPlan assembly = batch.Assemblies.Add("Server Market Data");
					TypePlan type = assembly.Types.Add("MarkThree.Guardian.Server.Environment");
					TransactionPlan transaction = batch.Transactions.Add();
					MethodPlan method = transaction.Methods.Add(type, "GetUserId");

					WebTransactionProtocol.Execute(batch);

					return (int)method.Parameters.Return;

				}
				catch (BatchException batchException)
				{

					// Write each of the exceptions taken on the server into the local event log.
					foreach (Exception exception in batchException.Exceptions)
						EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

					// This will provide a default user id for the static constructors when we get an error.  The value is
					// basically useless, but will allow the constructor to complete it's job.
					return -1;

				}

			}

		}

		public static int TimeInForceCode
		{
			get {return (int)Preferences.TimeInForceCode;}
			set {Preferences.TimeInForceCode = value;}
		}

		public static int BlotterId
		{
			get {return (int)Preferences.BlotterId;}
			set {Preferences.blotterId = value;}
		}

		public static int BrokerId
		{
			get {return (int)Preferences.brokerId;}
			set {Preferences.BrokerId = value;}
		}

		public static Pricing Pricing
		{
			get {return Preferences.pricing;}
			set {Preferences.pricing = value;}
		}

		public static bool IsTimeInForceCodeNull() {return Preferences.timeInForceCode == null;}
		public static bool IsBlotterIdNull() {return Preferences.blotterId == null;}

	}
}
