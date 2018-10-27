namespace MarkThree.Utilities
{

	using System;
	using System.ComponentModel;
	using System.Configuration;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState { None, FileName, TransactionSize };

	/// <summary>
	/// This object will load the property table from a formatted file.
	/// </summary>
	class CommandLine
	{

		private static ArgumentState argumentState;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			// This is the object that will do the work of loading the scripts.
			ScriptLoader scriptLoader = new ScriptLoader();

			try
			{

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has 
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.FileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					if (argument == "-f") { scriptLoader.ForceLogin = true; continue; }
					if (argument == "-i") { argumentState = ArgumentState.FileName; continue; }
					if (argument == "-b") { argumentState = ArgumentState.TransactionSize; continue; }

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

					case ArgumentState.FileName:
						scriptLoader.FileName = argument;
						break;
					case ArgumentState.TransactionSize:
						scriptLoader.TransactionSize = Convert.ToInt32(argument);
						break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.FileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (scriptLoader.FileName == null)
					throw new Exception("Usage: \"Script Loader\" [-b <Batch Size>] [-f] -i <FileName>");

				// Now that the command line arguments have been parsed into the loader, send the data to the server.
				scriptLoader.Load();

			}
			catch (BatchException batchException)
			{

				foreach (Exception exception in batchException.Exceptions)
					Console.WriteLine(exception.Message);

			}
			catch (Exception exception)
			{

				// Display the error.
				Console.WriteLine(exception.Message);

				// This will force an abnormal exit from the program.
				scriptLoader.HasErrors = true;

			}

			// If an error happened anywhere, don't exit normally.
			if (scriptLoader.HasErrors)
				return 1;

			// If we reached here, the file was imported without issue.
			if (!scriptLoader.HasErrors)
				Console.WriteLine(String.Format("{0} {1}: {2} Commands Executed", DateTime.Now.ToString("u"),
					scriptLoader.ScriptName, scriptLoader.MethodCount));

			// The Script Loader has components that must be disposed of explicitly.
			scriptLoader.Dispose();

			// If we reached here, the file was imported without issue.
			return 0;

		}

	}

}
