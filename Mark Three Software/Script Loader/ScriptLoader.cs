namespace MarkThree.Utilities
{

	using MarkThree;
	using MarkThree.Client;
	using System;
	using System.Collections;
	using System.Web.Services.Protocols;
	using System.Data;
	using System.ComponentModel;
	using System.Threading;
	using System.Xml;

	/// <summary>
	/// Executes XML Scripts containing Financial Programming Language.
	/// </summary>
	public class ScriptLoader : System.ComponentModel.Component
	{

		// Private Members
		private const int defaultTransactionSize = 100;
		private MarkThree.UserPreferences userPreferences;
		private System.ComponentModel.IContainer components;
		private Batch batch;
		private TransactionPlan transaction;
		private int requestCount;
		private AutoResetEvent requestCompleted;
		
		// Public Members
		public bool ForceLogin;
		public bool HasErrors;
		public int TransactionSize;
		public int MethodCount;
		public string FileName;
		public string ScriptName;

		/// <summary>
		/// Create a general purpose loader for XML scripts.
		/// </summary>
		/// <param name="container">The container for this object.</param>
		public ScriptLoader(System.ComponentModel.IContainer container)
		{

			/// Required for Windows.Forms Class Composition Designer support
			container.Add(this);
			InitializeComponent();

			// The transaction size determines how many mathod should go into a single transaction.  This is for the script format 
			// that uses implicit transactions.  After a number of methods, specified by 'transactionSize', is packed into a batch,
			// the batch is sent to the server for processing.
			this.TransactionSize = ScriptLoader.defaultTransactionSize;

			// This is used to count the number of methods processed.
			this.MethodCount = 0;

			// The data is sent to the server using asynchronous write operations.  That is, the data is queued up and buffered as 
			// fast as it can be read from the disk.  To prevent the application from exiting before all the data has been
			// acknowledged by the server and the results read from the server, the 'Load' method will wait on this signal before
			// returning.
			this.requestCount = 0;
			this.requestCompleted = new AutoResetEvent(true);
			
			// Users can be forced to specify the connection information even when preferences have been set by a previous session.
			this.ForceLogin = false;

		}

		/// <summary>
		/// Create a general purpose loader for XML scripts.
		/// </summary>
		public ScriptLoader()
		{

			/// Required for Windows.Forms Class Composition Designer support
			InitializeComponent();

			// The transaction size determines how many mathod should go into a single transaction.  This is for the script format 
			// that uses implicit transactions.  After a number of methods, specified by 'transactionSize', is packed into a batch,
			// the batch is sent to the server for processing.
			this.TransactionSize = ScriptLoader.defaultTransactionSize;

			// This is used to count the number of methods processed.
			this.MethodCount = 0;

			// The data is sent to the server using asynchronous write operations.  That is, the data is queued up and buffered as 
			// fast as it can be read from the disk.  To prevent the application from exiting before all the data has been
			// acknowledged by the server and the results read from the server, the 'Load' method will wait on this signal before
			// returning.
			this.requestCount = 0;
			this.requestCompleted = new AutoResetEvent(true);

			// Users can be forced to specify the connection information even when preferences have been set by a previous session.
			this.ForceLogin = false;

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{

			// Release any of the managed components.
			if (disposing)
				if(components != null)
					components.Dispose();

			// Allow the base class to release its resources.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.userPreferences = new MarkThree.UserPreferences(this.components);

		}
		#endregion

		/// <summary>
		/// Load the XML script.
		/// </summary>
		public void Load()
		{

			// This flag is set when an error occurs anywhere in the processing of the XML file.
			this.HasErrors = false;

			// Read the XML data from the specified file.
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(this.FileName);

			// Sending records to the server is state driven.  This loop will collect a batch of commands until a new table is
			// found in the XML stream, or until the limit of a batch is reached, or until the end of the file is read.  The idea
			// is to allow several tables of data to exist in a single file.
			XmlNode rootNode = xmlDocument.DocumentElement;

			// The script name is stored in the root node.  The name is used in status and debugging messages.
			this.ScriptName = rootNode.Attributes["name"] == null ? "Unnamed Batch" : rootNode.Attributes["name"].Value;

			// If the user wants to specify a new URL and certificate, then prompt for the connection info the next time a 
			// WebTransactionProtocol sends off a batch.
			if (this.ForceLogin)
			{
				WebTransactionProtocol.IsUrlPrompted = true;
				WebTransactionProtocol.IsCredentialPrompted = true;
			}

			// Create a batch to hold the transactions.
			this.batch = new Batch();

			// As methods are parsed, they're added to this transaction.  The organization of the transaction is determined by the
			// input file.  Explicit transaction will create their own transactions when they appear in the file.  Implicit
			// transactions are created when an unaffiliated method element is parsed out of the XML file.
			this.transaction = null;

			// The batch is sent when this flag is set.
			bool sendBatch = false;

			// Cycle through all of the children of the root node.  The methods in this file will either be affiliated with an
			// explicit transaction, or be unaffiliated.  The 'free' methods will be packed together in an implicit transaction and
			// sent to the server when the transaction reaches a predetermined size.  This will search the file for both kinds of
			// records.
			foreach (XmlNode xmlNode in rootNode)
			{

				// A 'transaction' element is a command for an explicit transaction.  A 'method' element declares a method for an
				// implicit (internally created) transaction.
				switch (xmlNode.Name)
				{

				case "transaction":

					// Parse the methods out of the node and send the batch off to the server for execution.
					ParseTransaction(xmlNode);
					break;

				case "send":

					// Transactions can be grouped together in a file.  They are not sent to the Web Transaction handler until
					// either the end of the file, or an explicit 'send' node.
					sendBatch = true;;
					break;

				case "method":

					// For an implicit method execution, a transaction will be created automatically.  When the batch is sent, this
					// value will be cleared again.
					if (this.transaction == null)
						this.transaction = this.batch.Transactions.Add();

					// Parse the method out of the XML node.
					ParseMethod(xmlNode);

					// If the size of the transaction has reached the predetermined limit, then the batch can be sent.
					if (this.transaction.Methods.Count == this.TransactionSize)
						sendBatch = true;

					break;

				}

				// If the parser has determined that the batch is large enough, then it is sent to the server.
				if (sendBatch)
				{

					// Send the batch to the server.
					SendBatch(batch);

					// Prepare a brand new batch for the records remaining to be parsed.
					this.batch = new Batch();
					this.transaction = null;
					sendBatch = false;

				}

			}

			// If a transaction has been initialized but not sent, then send the batch.
			if (batch.Transactions.Count != 0)
				SendBatch(batch);

			// Wait here until all the IO requests have completed.
			this.requestCompleted.WaitOne();

		}

		/// <summary>
		/// Parse a transaction out of the XML file.
		/// </summary>
		/// <param name="xmlNode">The node where the transaction has been found.</param>
		private void ParseTransaction(XmlNode xmlNode)
		{

			// Create an explicit transaction for the methods found at this node.
			this.transaction = this.batch.Transactions.Add();

			// The <Method> tag specifies the name assembly, type and method name for this operation.  The parameters to the method
			// call will be nested under this heading.
			foreach (XmlNode methodNode in xmlNode.SelectNodes("method"))
				ParseMethod(methodNode);

		}

		/// <summary>
		/// Parse a method from the XML file.
		/// </summary>
		/// <param name="methodNode">The node where the method data is found.</param>
		private void ParseMethod(XmlNode methodNode)
		{

			// Keep track of the number of methods read and processed.
			this.MethodCount++;

			// Extract the assembly and type information from the method node.  These are used to locate an object containing the
			// method.  That is, the server will use 'System.Reflection' to load an assembly with the 'assembly' name, load and
			// instantiate an object of 'type', then call the named method with the parameters listed.
			AssemblyPlan assembly = this.batch.Assemblies.Add(methodNode.Attributes["assembly"].Value);
			TypePlan type = assembly.Types.Add(methodNode.Attributes["type"].Value);

			// Create a method from the XML data and add it to the transaction.
			this.transaction.Methods.Add(type, LoadMethod(methodNode));

		}
		
		/// <summary>
		/// Creates a method plan from the parameters listed.
		/// </summary>
		/// <param name="methodNode">An XML node where the method and parameters are found.</param>
		private MethodPlan LoadMethod(XmlNode methodNode)
		{

			// Create a new method from the type information and the name found in the XML file.
			MethodPlan method = new MethodPlan(methodNode.Attributes["name"].Value);

			// Load each of the parameters into the method structure.
			foreach (XmlNode parameterNode in methodNode.ChildNodes) 
			{

				// Ignore Comment Nodes.
				if (parameterNode.NodeType == XmlNodeType.Comment)
					continue;

				// Add the next parameter to the method.
				method.Parameters.Add(new InputParameter(parameterNode.Name, parameterNode.InnerText == string.Empty ? (object)DBNull.Value : (object)parameterNode.InnerText));

			}

			// Return the method plan created from the script.
			return method;

		}

		/// <summary>
		/// Sends a batch of commands to the server and processes the results.
		/// </summary>
		private void SendBatch(Batch batch)
		{

			WebTransactionProtocol.Execute(batch);

		}

		private void EndSendBatch(object sender, BatchEventArgs batchEventArgs)
		{

			try
			{

				if (Interlocked.Decrement(ref this.requestCount) == 0)
					this.requestCompleted.Set();

				// If the server returned an exception, throw a special purpose exception which packs up all the exceptions into a
				// list the caller to cycle through.  Processing the errors from a Batch execution is for generally more difficult
				// than ling a single error.
				if (batchEventArgs.Exception != null)
					throw batchEventArgs.Exception;

			}
			catch (BatchException batchException)
			{

				// Display each error on the console.								
				foreach (Exception exception in batchException.Exceptions)
					Console.WriteLine(exception.Message);

				// This will signal the error exit from this loader.
				this.HasErrors = true;

			}
			catch (Exception exception)
			{

				Console.WriteLine(exception.Message);

				// This will signal the error exit from this loader.
				this.HasErrors = true;

			}

		}

	}

}
