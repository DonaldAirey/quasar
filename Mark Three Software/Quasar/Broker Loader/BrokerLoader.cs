namespace Shadows.Loader
{

	using MarkThree;
	using MarkThree.Client;
	using System;
	using System.Collections;
	using System.Web.Services.Protocols;
	using System.Data;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, Assembly, BatchSize, FileName, NameSpace};

	/// <summary>
	/// This object will load the property table from a formatted file.
	/// </summary>
	class TableLoader
	{

		private static int batchSize;
		private static string fileName;
		private static string assemblyName;
		private static string nameSpaceName;
		private static ArgumentState argumentState;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			// If this flag is set during the processing of the file, the program will exit with an error code.
			bool hasErrors = false;

			try
			{

				// Defaults
				batchSize = 100;
				assemblyName = "External Service";
				nameSpaceName = "MarkThree.Quasar.External";

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has been
				// read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.FileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					if (argument == "-a") { argumentState = ArgumentState.Assembly; continue; }
					if (argument == "-b") { argumentState = ArgumentState.BatchSize; continue; }
					if (argument == "-n") { argumentState = ArgumentState.NameSpace; continue; }
					if (argument == "-i") { argumentState = ArgumentState.FileName; continue; }

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

					case ArgumentState.Assembly:
						assemblyName = argument;
						break;
					case ArgumentState.BatchSize:
						batchSize = Convert.ToInt32(argument);
						break;
					case ArgumentState.FileName:
						fileName = argument;
						break;
					case ArgumentState.NameSpace:
						nameSpaceName = argument;
						break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.FileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (fileName == null)
					throw new Exception("Usage: Loader.Algorithm -i <FileName>");

				// Open up the file containing all the broker.
				BrokerReader brokerReader = new BrokerReader(fileName);

				// Loading the database involves creating a batch of commands and sending them off as a transaction.  This gives
				// the server a chance to pipeline a large chunk of processing, without completely locking up the server for the
				// entire set of data.  This will construct a header for the command batch which gives information about which
				// assembly contains the class that is used to load the data.
				Batch batch = new Batch();
				TransactionPlan transactionPlan = batch.Transactions.Add();
				AssemblyPlan assemblyPlan = batch.Assemblies.Add(assemblyName);
				TypePlan typePlan = assemblyPlan.Types.Add(string.Format("{0}.{1}", nameSpaceName, "Broker"));

				// Read the file until an EOF is reached.
				while (true)
				{

					// This counter keeps track of the number of records sent.  When the batch is full, it's sent to the server to be
					// executed as a single transaction.
					int batchCounter = 0;

					// Read the next broker from the input stream.  A 'null' is returned when we've read past the end of file.
					Broker broker = brokerReader.ReadBroker();
					if (broker != null)
					{

						// Create a new method from the type information and the name found in the XML file.
						MethodPlan methodPlan = new MethodPlan("Load");

						// Construct a call to the 'Load' method to populate the broker record.
						methodPlan.Parameters.Add(new InputParameter("brokerId", broker.Symbol));
						methodPlan.Parameters.Add(new InputParameter("name", broker.Name));
						methodPlan.Parameters.Add(new InputParameter("symbol", broker.Symbol));

						// Create a method from the XML data and add it to the transaction.
						transactionPlan.Methods.Add(typePlan, methodPlan);

					}

					// This will check to see if it's time to send the batch.  A batch is sent when the 'batchSize' has been
					// reached, or if the last record has just been converted into a command.
					if (++batchCounter % batchSize == 0 || broker == null)
					{

						WebTransactionProtocol.Execute(batch);

						batch = new Batch();
						transactionPlan = batch.Transactions.Add();
						assemblyPlan = batch.Assemblies.Add(assemblyName);
						typePlan = assemblyPlan.Types.Add(string.Format("{0}.{1}", nameSpaceName, "Broker"));

					}

					// If the end of file was reached, break out of the loop and exit the application.
					if (broker == null)
						break;

				}

			}
			catch (BatchException batchException)
			{

				foreach (Exception exception in batchException.Exceptions)
					Console.WriteLine(exception.Message);
				hasErrors = true;

			}
			catch (Exception exception)
			{

				// Show the system error and exit with an error.
				Console.WriteLine(exception.Message);
				hasErrors = true;

			}

			// Any errors will cause an abnormal exit.
			if (hasErrors)
				return 1;

			// Write a status message when a the file is loaded successfully.
			Console.WriteLine(String.Format("{0} Data: Brokers, Loaded", DateTime.Now.ToString("u")));

			// If we reached here, the file was imported without issue.
			return 0;

		}

	}

}
