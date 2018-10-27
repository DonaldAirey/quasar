/*************************************************************************************************************************
*
*	File:			Command Line.cs
*	Destylesheetion:	Loads a Stylesheet into the Middle Tier.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Loader.Stylesheet
{

	using System;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, FileName};

	/// <summary>
	/// Loads a Stylesheet into the Middle Tier.
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
			StylesheetLoader stylesheetLoader = new StylesheetLoader();

			try
			{

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has 
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.FileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state.
					if (argument == "-f") {stylesheetLoader.ForceLogin = true; continue;}
					if (argument == "-i") {argumentState = ArgumentState.FileName; continue;}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

							// Read the command line argument into the proper variable based on the parsing state.
						case ArgumentState.FileName: stylesheetLoader.FileName = argument; break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.FileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (stylesheetLoader.FileName == null)
					throw new Exception("Usage: \"Stylesheet Loader\" [-f] -i <FileName>");

				// Now that the command line arguments have been parsed into the loader, send the data to the server.
				stylesheetLoader.Load();

			}
			catch (Exception exception)
			{

				// Display the error.
				Console.WriteLine(exception.Message);

				// This will force an abnormal exit from the program.
				stylesheetLoader.HasErrors = true;

			}

			// If an error happened anywhere, don't exit normally.
			if (stylesheetLoader.HasErrors)
				return 1;
			
			// If we reached here, the file was imported without issue.
			if (!stylesheetLoader.HasErrors)
				Console.WriteLine(String.Format("{0} {1}: Stylesheet Loaded", DateTime.Now.ToString("u"),
					stylesheetLoader.StylesheetName));

			// If we reached here, the file was imported without issue.
			return 0;

		}

	}

}
