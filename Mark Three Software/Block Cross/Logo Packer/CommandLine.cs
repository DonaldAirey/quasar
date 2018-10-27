namespace MarkThree.Guardian.Utilities
{

	using System;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, Path, OutputFile};

	/// <summary>
	/// Loads a Logo into the Security Table.
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

			// This is the object that will do the work of loading the stylesheets.
			LogoPacker logoLoader = new LogoPacker();

			try
			{

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.Path;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state.
					if (argument == "-f") {logoLoader.ForceLogin = true; continue;}
					if (argument == "-p") {argumentState = ArgumentState.Path; continue;}
					if (argument == "-o") {argumentState = ArgumentState.OutputFile; continue;}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

					// Read the command line argument into the proper variable based on the parsing state.
					case ArgumentState.Path:
						logoLoader.Path = argument;
						break;
					case ArgumentState.OutputFile:
						logoLoader.OutputFile = argument;
						break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.Path;

				}

				// Throw a usage message back at the user if no file name was given.
				if (logoLoader.Path == null)
					throw new Exception("Usage: \"Stylesheet Loader\" [-f] -i <Path>");

				// Now that the command line arguments have been parsed into the loader, send the data to the server.
				logoLoader.Load();

			}
			catch (Exception exception)
			{

				// Display the error.
				Console.WriteLine(exception.Message);

				// This will force an abnormal exit from the program.
				logoLoader.HasErrors = true;

			}

			// If an error happened anywhere, don't exit normally.
			if (logoLoader.HasErrors)
				return 1;
			
			// If we reached here, the file was imported without issue.
			if (!logoLoader.HasErrors)
				Console.WriteLine("{0} {1}: Company Logos Loaded", DateTime.Now.ToString("u"), logoLoader.LogoCount);

			// If we reached here, the file was imported without issue.
			return 0;

		}

	}

}
