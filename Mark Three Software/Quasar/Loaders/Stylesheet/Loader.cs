/*************************************************************************************************************************
*
*	File:			Stylesheet Loader.cs
*	Description:	This application is used to add or update templates from an external source.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.Client;
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;

namespace Shadows.Stylesheet.Loader
{

	/// <summary>
	/// Loads templates from an external source.
	/// </summary>
	class StylesheetLoader
	{

		private static string externalConfigurationCode;
		private static string fileName;
		private static string assemblyName;
		private static string namespaceName;
		private static string StylesheetId;
		private static string stylesheetTypeCode;
		private static string name;
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
				assemblyName = "Service.External";
				namespaceName = "Shadows.WebService.External";
				externalConfigurationCode = "DEFAULT";
				argumentState = ArgumentState.FileName;
			
				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state.
					if (argument == "-c") {argumentState = ArgumentState.ConfigurationCode; continue;}
					if (argument == "-i") {argumentState = ArgumentState.FileName; continue;}
					if (argument == "-id") {argumentState = ArgumentState.StylesheetId; continue;}
					if (argument == "-t") {argumentState = ArgumentState.StylesheetTypeCode; continue;}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

							// Read the command line argument into the proper variable based on the parsing state.
						case ArgumentState.ConfigurationCode: externalConfigurationCode = argument; break;
						case ArgumentState.StylesheetTypeCode: stylesheetTypeCode = argument; break;
						case ArgumentState.FileName: fileName = argument; break;
						case ArgumentState.Name: name = argument; break;
						case ArgumentState.StylesheetId: StylesheetId = argument; break;

					}

					// The default state for the parser is to look for a file name.
					argumentState = ArgumentState.FileName;

				}

				// If no file name was specified, we return an error.
				if (fileName == null)
					throw new Exception("Usage: Loader.Stylesheet -i <FileName>");

				// Load up an XML document with the contents of the file specified on the command line.
				XmlDocument stylesheet = new XmlDocument();
				stylesheet.Load(fileName);

				// The XML document has several nodes that need to be read -- and then removed -- that contain attributes of the 
				// stylesheet.  These nodes can be found easily using the XSL Path functions which need a namespace manager to sort
				// out the tag prefixes.
				XmlNamespaceManager namespaceManager = new XmlNamespaceManager(stylesheet.NameTable);
				namespaceManager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
				namespaceManager.AddNamespace("sss", "urn:schemas-shadows-com:shadows:stylesheet");

				// The spreadhseet source has several nodes which contain information about how the data in the XML document should
				// be loaded into the server, such as the stylesheet identifier, the name and the stylesheet style.  They are found
				// at this node.  After these information nodes have been read, they are removed from the stylesheet source.
				XmlNode stylesheetNode = stylesheet.SelectSingleNode("xsl:stylesheet", namespaceManager);
				if (stylesheetNode == null)
					throw new Exception("Syntax Error: missing stylesheet declaration.");
			
				// Find the StylesheetId node.
				XmlNode StylesheetIdNode = stylesheetNode.SelectSingleNode("sss:stylesheetId", namespaceManager);
				if (StylesheetIdNode == null)
					throw new Exception("Syntax Error: missing StylesheetId declaration.");

				// Find the StylesheetStyle node.
				XmlNode stylesheetTypeCodeNode = stylesheetNode.SelectSingleNode("sss:stylesheetTypeCode", namespaceManager);
				if (stylesheetTypeCodeNode == null)
					throw new Exception("Syntax Error: missing StylesheetStyle declaration.");

				// Find the name node.
				XmlNode nameNode = stylesheetNode.SelectSingleNode("sss:name", namespaceManager);
				if (nameNode == null)
					throw new Exception("Syntax Error: missing name declaration.");

				// Extract the data from the XML nodes.
				StylesheetId = StylesheetIdNode.InnerText;
				stylesheetTypeCode = stylesheetTypeCodeNode.InnerText;
				name = nameNode.InnerText;

				// Remove the stylesheet nodes from the XSL spreadsheet before loading it into the server.
				stylesheetNode.RemoveChild(StylesheetIdNode);
				stylesheetNode.RemoveChild(stylesheetTypeCodeNode);
				stylesheetNode.RemoveChild(nameNode);

				// Create a command to load the stylesheet from the data loaded from the file.
				RemoteBatch remoteBatch = new RemoteBatch();
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add(assemblyName);
				RemoteType remoteType = remoteAssembly.Types.Add(string.Format("{0}.{1}", namespaceName, "Stylesheet"));
				RemoteMethod remoteMethod = remoteType.Methods.Add("Load");
				remoteMethod.Parameters.Add("StylesheetId", StylesheetId);
				remoteMethod.Parameters.Add("stylesheetTypeCode", stylesheetTypeCode);
				remoteMethod.Parameters.Add("name", name);
				remoteMethod.Parameters.Add("text", stylesheet.InnerXml);

				// Create a new web client that will serve as the connection point to the server and call the web services to 
				// execute the command batch.
				WebClient webClient = new WebClient();
				remoteBatch.Merge(webClient.Execute(remoteBatch));

				// Display the each of the exceptions and set a global flag that shows that there was an exception to the normal
				// execution.
				if (remoteBatch.HasExceptions)
				{
					foreach (RemoteException exception in remoteBatch.Exceptions)
						Console.WriteLine(String.Format("{0}: {1}", remoteMethod.Parameters["StylesheetId"].Value, exception.Message));
					hasErrors = true;
				}

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

			// Display the template that was loaded and exit with a successful code.
			Console.WriteLine(String.Format("{0} Stylesheet: {1}, Loaded", DateTime.Now.ToString("u"), name));
			return 0;

		}

	}

	// Argument Parsing States.
	enum ArgumentState {None, ConfigurationCode, StylesheetId, FileName, Name, StylesheetTypeCode};

}
