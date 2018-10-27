namespace Register_Event_Source
{

	using System;
	using System.Configuration;
	using System.Diagnostics;

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			// Read the settings for the log from the configuration file.
			string logName = ConfigurationManager.AppSettings["eventLog"];
			string[] sourceList = ConfigurationManager.AppSettings["eventLogSource"].Split('|');
			bool deleteLog = Convert.ToBoolean(ConfigurationManager.AppSettings["deleteLog"]);
			bool createSources = Convert.ToBoolean(ConfigurationManager.AppSettings["createSources"]);

			// Delete the sources if they exist.  If the source is moved from one log to another, the user should be warned that
			// they need to reboot the computer.
			bool isSourceMoved = false;
			foreach(string source in sourceList)
			{
				string oldLogName = EventLog.LogNameFromSourceName(source, ".");
				if (EventLog.SourceExists(source))
					EventLog.DeleteEventSource(source);
				isSourceMoved = isSourceMoved ? true : oldLogName != logName;
			}

			// This will clean out the entire log if the feature is selected.
			if (deleteLog)
			{
				foreach (string source in sourceList)
				{
					string oldLogName = EventLog.LogNameFromSourceName(source, ".");
					if (EventLog.SourceExists(source))
						EventLog.DeleteEventSource(source);
				}

				if (EventLog.Exists(logName))
					EventLog.Delete(logName);

			}

			// Create each of the sources specified in the configuration.
			if (createSources)
				foreach(string source in sourceList)
					EventLog.CreateEventSource(source, logName);

			// Put up a helpful message about moving sources mapped to other logs.
			if (isSourceMoved)
			{
				Console.WriteLine("You have moved a source that was already mapped to another log.");
				Console.WriteLine("You must reboot the computer for the changes to take effect.");
				Console.WriteLine("Press 'Enter' key.");
				Console.ReadLine();
			}

		}

	}

}
