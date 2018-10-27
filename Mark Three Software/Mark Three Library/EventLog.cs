namespace MarkThree
{

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Configuration;

	/// <summary>
	/// An Event Log for Web Services.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class EventLog
	{

		// Private Members
		private static bool isMultiuser;
		private static System.Diagnostics.EventLog eventLog;

		/// <summary>
		/// Initialize the Web Service Event Log.
		/// </summary>
		static EventLog()
		{

			// Read the settings for the log from the configuration file.
			string logName = ConfigurationManager.AppSettings["eventLog"];
			string sourceName = ConfigurationManager.AppSettings["eventLogSource"];

			EventLog.isMultiuser = Convert.ToBoolean(ConfigurationManager.AppSettings["isMultiuserLog"]);

			// If the configuration file doesn't contain specifications for the log, then use the application log with
			// an undefined source.
			if (logName == null)
				logName = "Application";
			if (sourceName == null)
				sourceName = Process.GetCurrentProcess().MainModule.ModuleName;

			// Initialize the Event Log.
			EventLog.eventLog = new System.Diagnostics.EventLog();
			EventLog.eventLog.Log = logName;
			EventLog.eventLog.Source = sourceName;

		}

		/// <summary>
		/// Write a formatted error entry into the event log.
		/// </summary>
		/// <param name="message">The format string for the message.</param>
		/// <param name="arguments">An array of optional arguments for the format string.</param>
		public static void Error(string format, params object[] arguments)
		{

			string message = CreateFormattedMessage(format, arguments);
			EventLog.eventLog.WriteEntry(message, EventLogEntryType.Error);

		}

		/// <summary>
		/// Write a formatted warning to the event log.
		/// </summary>
		/// <param name="message">The format string for the message.</param>
		/// <param name="arguments">An array of optional arguments for the format string.</param>
		public static void Warning(string format, params object[] arguments)
		{

			string message = CreateFormattedMessage(format, arguments);
			EventLog.eventLog.WriteEntry(message, EventLogEntryType.Warning);

		}

		/// <summary>
		/// Write an informational message to the event log.
		/// </summary>
		/// <param name="message">The format string for the message.</param>
		/// <param name="arguments">An array of optional arguments for the format string.</param>
		public static void Information(string format, params object[] arguments)
		{

			string message = CreateFormattedMessage(format, arguments);
			EventLog.eventLog.WriteEntry(message, EventLogEntryType.Information);

		}

		/// <summary>
		/// Combines the format string and the input parameters.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private static string CreateFormattedMessage(string format, params object[] arguments)
		{
			// If the parameter list is empty, just forward the message as is.
			// This avoids a format exception when you want to write a string that may 
			// have format characters in it, but is not a format string. e.g. forwarding an exception message.
			if ( arguments.Length.Equals(0) )
				return format;

			// The message is assembled with the user domain and name for a multiuser log. 
			string message = EventLog.isMultiuser ? String.Format(@"{0}\{1}: {2}", System.Environment.UserDomainName,
				System.Environment.UserName, string.Format(format, arguments)) :
				String.Format(format, arguments);

			return message;
		}

	}

}