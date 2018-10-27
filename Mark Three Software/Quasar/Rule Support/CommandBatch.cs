namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Threading;

	public class CommandBatch
	{

		private static RemoteBatch remoteBatch;

		public static RemoteBatch RemoteBatch {get {lock (typeof(CommandBatch)) return CommandBatch.RemoteBatch;}}

		public static RemoteAssemblyCollection Assemblies
		{
			get {lock (typeof(CommandBatch)) return CommandBatch.remoteBatch.Assemblies;}
		}

		static CommandBatch()
		{

			CommandBatch.remoteBatch = new RemoteBatch();

		}

		public static void Flush()
		{

			if (CommandBatch.remoteBatch.RemoteMethod.Rows.Count > 0)
			{

				RemoteBatch currentBatch = null;

				lock (typeof(CommandBatch))
				{
					currentBatch = CommandBatch.remoteBatch;
					CommandBatch.remoteBatch = new RemoteBatch();
				}

				new WorkerThread(new ThreadHandler(FlushThread), "Command Batch Execution", currentBatch);
				
			}

		}

		private static void FlushThread(params object[] argument)
		{

			ClientMarketData.Send((RemoteBatch)argument[0]);

		}

	}

}
