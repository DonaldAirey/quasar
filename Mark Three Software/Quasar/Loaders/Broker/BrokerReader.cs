/*************************************************************************************************************************
*
*	File:			BrokerReader.cs
*	Description:	This module contains a class that reads broker records from a formatted, flat file.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Text.RegularExpressions;
using System.IO;

namespace Shadows.Loader
{

	/// <summary>
	/// A broker reader is an input stream that reads formatted broker records from a flat file.
	/// </summary>
	public class BrokerReader : StreamReader
	{

		// This expression is used to parse the fields out of the NASDAQ symbol file.
		private static readonly Regex brokerFormat = new Regex(@"(?<symbol>[^\t]*)\t((""(?<name>[^""]*)"")|(?<name>[^\t]*))\t?(?<connected>[^\t]*)");

		// Records that have these symbols will be removed from the input stream.
		private static readonly string[] constantSymbolList = new string[] {"NASD MARKET MAKER/ECN/EXCHANGE CODES"};

		// There are several different object contained in the input file.  Only the ones that have a type listed here 
		// are considered execution broker.
		private static readonly string[] constantBrokerTypeList = new string[] {"M"};
		
		/// <summary>
		/// Constructor for an input stream that reads broker records from a flat file.
		/// </summary>
		/// <param name="fileName"></param>
		public BrokerReader(string fileName) : base(fileName) {}

		/// <summary>
		/// Reads a broker from the input stream.
		/// </summary>
		/// <returns>A broker record parsed out of the input file.</returns>
		public Broker ReadBroker()
		{

			// This loop will continue to read the input stream until we find a valid broker record or the end of file is
			// encountered.
			while (true)
			{
			
				// Read a line of raw data from the input stream and parse it.  A 'null' indicates we've reached the end
				// of file.
				String rawBrokerData = this.ReadLine();
				if (rawBrokerData == null)
					return null;

				// Parse the raw data against the regular expressions that will extract the fields.
				Match match = brokerFormat.Match(rawBrokerData);

				// Create a new broker record from the fields parsed out of the raw input line.
				string symbolType = "M";
				bool connected = match.Groups["connected"].Value.ToUpper() == "YES" ? true : false;
				Broker broker = new Broker(connected, match.Groups["symbol"].Value, match.Groups["name"].Value);

				// The input file contains many entries that are not executing broker.  This will break out of the
				// loop and accept a record that isn't a constant symbol and is a broker record.
				if (IsBrokerType(symbolType) && IsBrokerSymbol(broker.Symbol))
					return broker;

			}

		}

		/// <summary>
		/// Determines if a broker symbol is valid or not.
		/// </summary>
		/// <param name="brokerSymbol">The executing broker symbol.</param>
		/// <returns>True if given a valid broker symbol.</returns>
		private static bool IsBrokerSymbol(string brokerSymbol)
		{

			// Check the broker symbol against each of the constant fields.
			foreach (string constantSymbol in constantSymbolList)
			{

				// This will limit the number of characters compared to the smallest string.  This was done primarily so 
				// we could match the timestamp at the end of the file to the constant label for the timestamp.
				int minLength = constantSymbol.Length < brokerSymbol.Length ? constantSymbol.Length :
					brokerSymbol.Length;

				// An empty symbol is not a valid one.
				if (brokerSymbol == String.Empty)
					return false;

				// If the strings match (using a minimum character count), then the symbol is considered part of the
				// constant data in the file.
				if (String.Compare(brokerSymbol, 0, constantSymbol, 0, minLength) == 0)
					return false;

			}

			// If we reached here, the symbol is valid.
			return true;

		}

		/// <summary>
		/// Determines if a given broker type is an execution broker.
		/// </summary>
		/// <param name="brokerType">Indicates what type of symbol is in the parsed file.</param>
		/// <returns>True if the given broker type is an execution broker.</returns>
		private static bool IsBrokerType(string brokerType)
		{

			// Check the list of broker types for the ones we accept.
			foreach (string constantBrokerType in constantBrokerTypeList)
				if (brokerType == constantBrokerType)
					return true;

			// If we reached here, the given type isn't an executing broker.
			return false;

		}

	}

}
