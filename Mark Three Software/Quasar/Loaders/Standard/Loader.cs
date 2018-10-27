/*************************************************************************************************************************
*
*	File:			Loader.cs
*	Description:	Loads and updates the [algorithm] table.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.Client;
using System;
using System.Collections;
using System.Web.Services.Protocols;
using System.Data;
using System.Xml;
using System.Xml.XPath;

namespace Shadows.Loader
{

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, Assembly, BatchSize, FileName, NameSpace, Timeout};

	/// <summary>
	/// This object will load the property table from a formatted file.
	/// </summary>
	class TableLoader
	{

		private static int batchSize;
		private static string fileName;
		private static ArgumentState argumentState;
		private static RemoteBatch remoteBatch = null;
		private static RemoteTransaction remoteTransaction = null;
		private static RemoteAssembly remoteAssembly = null;
		private static RemoteType remoteType = null;
		private static string tableName = string.Empty;
		private static int timeout = 100000;
		private static int recordCounter = 0;
		private static int batchCounter = 0;
		private static bool hasErrors = false;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			try
			{

				// Defaults
				batchSize = 100;

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has 
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.FileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					if (argument == "-a") {argumentState = ArgumentState.Assembly; continue;}
					if (argument == "-b") {argumentState = ArgumentState.BatchSize; continue;}
					if (argument == "-n") {argumentState = ArgumentState.NameSpace; continue;}
					if (argument == "-i") {argumentState = ArgumentState.FileName; continue;}
					if (argument == "-t") {argumentState = ArgumentState.Timeout; continue;}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

						case ArgumentState.BatchSize: batchSize = Convert.ToInt32(argument); break;
						case ArgumentState.FileName: fileName = argument; break;
						case ArgumentState.Timeout: timeout = Convert.ToInt32(argument) * 1000; break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.FileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (fileName == null)
					throw new Exception("Usage: Loader.Table -i <FileName>");

				// Read the XML data from the specified file.
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fileName);

				// Sending records to the server is state driven.  This loop will collect a batch of commands until a new table is
				// found in the XML stream, or until the limit of a batch is reached, or until the end of the file is read.  The
				// idea is to allow several tables of data to exist in a single file.
				XmlNode rootNode = xmlDocument.DocumentElement;
				switch (rootNode.Name)
				{

					case "batch":

						LoadCommand(rootNode);
						break;

				}

				// If we reached here, the file was imported without issue.
				if (!hasErrors)
					Console.WriteLine(String.Format("{0} {1}: {2} Commands Executed", DateTime.Now.ToString("u"),
						rootNode.Attributes["name"] == null ? "Unnamed Batch" : rootNode.Attributes["name"].Value, recordCounter));

			}
			catch (Exception exception)
			{

				// Show the system error and exit with an error.
				Console.WriteLine(exception.Message);
				hasErrors = true;

			}

			// Any errors will cause an abnormal exit.
			return hasErrors ? 1 : 0;

		}

		/// <summary>
		/// Load and execute a command batch.
		/// </summary>
		/// <param name="rootNode">The root node of the Xml Data structure that contains the command.</param>
		private static void LoadCommand(XmlNode rootNode)
		{

			// Read each of the transactions and send them to the server.
			foreach (XmlNode transactionNode in rootNode.SelectNodes("transaction"))
			{

				// Ignore Comment Nodes.
				if (transactionNode.NodeType == XmlNodeType.Comment)
					continue;

				// Each transaction in the batch is handled as a unit.  That is, everything between the start and end <Transaction>
				// tags will be executed or rolled back as a unit.
				remoteBatch = new RemoteBatch();
				remoteTransaction = remoteBatch.Transactions.Add();
				remoteAssembly = null;
				remoteType = null;

				// The <Assembly> tag specifies the name of an assembly where the object and methods are found.
				foreach (XmlNode assemblyNode in transactionNode.SelectNodes("assembly"))
					LoadAssembly(assemblyNode, 0);

				// Once the entire transaction has been loaded, send it off to the server.
				SendBatch();

			}

			// If the batch file doesn't contain explicit transactions, then implicit ones are assumed and the processing of the file
			// continues with the 'assembly' nodes.
			XmlNodeList assemblyNodes = rootNode.SelectNodes("assembly");
			if (assemblyNodes.Count != 0)
			{
			
				// This will tell the 'LoadAssembly' to determine the size of the batch from the number of records processed.
				remoteBatch = null;
				remoteTransaction = null;
				remoteAssembly = null;
				remoteType = null;

				// If an assembly is included outside of a Transaction, then the transaction is implicit and the size of the
				// transaction is the 'batchSize' parameter.
				foreach (XmlNode assemblyNode in assemblyNodes)
				{

					// Load the assebly node ninto the batch.
					LoadAssembly(assemblyNode, batchSize);

					// There will be records in the batch when the above method returns (unless there are exactly as many records 
					// in the batch as the batch size).  This will send the remaining records to the server.
					if (remoteBatch != null) SendBatch();

				}

			}

		}
		
		/// <summary>
		/// Load up an assembly section of a batch.
		/// </summary>
		/// <param name="assemblyNode"></param>
		private static void LoadAssembly(XmlNode assemblyNode, int batchSize)
		{

			// Ignore Comment Nodes.
			if (assemblyNode.NodeType == XmlNodeType.Comment)
				return;

			// Add an Assembly specifier to the batch.  This essentially describes the DLL where the methods are found.
			string assemblyName = assemblyNode.Attributes["name"].Value;

			// Each assembly can have one or more Objects (or Types) which can be instantiated.  The batch command
			// processing can call static methods belonging to the instantiated object.
			foreach (XmlNode typeNode in assemblyNode.SelectNodes("type")) 
			{

				// Ignore Comment Nodes.
				if (typeNode.NodeType == XmlNodeType.Comment)
					continue;

				// Loading the database involves creating a batch of commands and sending them off as a transaction.  
				// This gives the server a chance to pipeline a large chunk of processing, without completely locking
				// up the server for the entire set of data.  This will construct a header for the command batch which
				// gives information about which assembly contains the class that is used to load the data.
				string typeName = typeNode.Attributes["name"].Value;

				// Attach each method and it's parameters to the command batch.
				foreach (XmlNode methodNode in typeNode.SelectNodes("method")) 
				{

					// Ignore Comment Nodes.
					if (methodNode.NodeType == XmlNodeType.Comment)
						continue;

					// The 'remoteBatch' will be cleared after it is sent to the server.  Make sure a batch exists before adding
					// methods and parameters to it.
					if (remoteBatch == null)
					{
						
						// Loading the database involves creating a batch of commands and sending them off as a transaction.  This
						// gives the server a chance to pipeline a large chunk of processing, without completely locking up the
						// server for the entire set of data.  This will create a batch for the next bunch of commands.
						remoteBatch = new RemoteBatch();
						remoteTransaction = remoteBatch.Transactions.Add();

						// This counts the number of records placed in the batch.
						batchCounter = 0;
						
					}

					if (remoteAssembly == null || remoteAssembly.Name != assemblyName)
						remoteAssembly = remoteBatch.Assemblies.Add(assemblyName);

					if (remoteType == null || remoteType.Name != typeName)
						remoteType = remoteAssembly.Types.Add(typeName);

					// Each method is part of the transaction defined by the tagged outline structure of the input 
					// file.
					RemoteMethod remoteMethod = remoteType.Methods.Add(methodNode.Attributes["name"].Value);
					remoteMethod.Transaction = remoteTransaction;

					// Load each of the parameters into the method structure.
					foreach (XmlNode parameterNode in methodNode.ChildNodes) 
					{

						// Ignore Comment Nodes.
						if (parameterNode.NodeType == XmlNodeType.Comment)
							continue;

						// Add the next parameter to the method.
						remoteMethod.Parameters.Add(parameterNode.Name, parameterNode.InnerText);

					}

					// One more record for the grand total, one more record for the number in the current batch.
					recordCounter++;
					batchCounter++;

					// This will check to see if it's time to send the batch.  A batch is sent when the 'batchSize' has been
					// reached, or if the last record has just been converted into a command.
					if (batchSize != 0 && recordCounter % batchSize == 0)
						SendBatch();

				}

			}

		}
		
		/// <summary>
		/// Sends a batch of commands to the server and processes the results.
		/// </summary>
		private static void SendBatch()
		{

			// Create a new web client that will serve as the connection point to the server.
			WebClient webClient = new WebClient();
			webClient.Timeout = TableLoader.timeout;

			// Call the web services to execute the command batch.
			remoteBatch.Merge(webClient.Execute(remoteBatch));

			// Any error during the batch will terminate the execution of the remaining records in the data file.
			if (remoteBatch.HasExceptions)
			{

				// Display each error on the console.								
				foreach (RemoteException remoteException in remoteBatch.Exceptions)
					Console.WriteLine(remoteException.Message);

				// This will signal the error exit from this loader.
				hasErrors = true;

			}

			// Clearing out the batch indicates that a new header should be created for the next set of commands.
			remoteBatch = null;
			remoteTransaction = null;
			remoteAssembly = null;
			remoteType = null;

		}

	}

}
