/*************************************************************************************************************************
*
*	File:			Stylesheet Loader.cs
*	Description:	This application is used to add or update templates from an external source.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Loader.Stylesheet
{

	using MarkThree;
	using MarkThree.Client;
	using System;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	using System.Xml.XPath;

	/// <summary>
	/// Loads templates from an external source.
	/// </summary>
	class StylesheetLoader : System.ComponentModel.Component
	{

		// Private Members
		private MarkThree.UserPreferences userPreferences;
		private System.ComponentModel.IContainer components;
		private string Assembly;
		private string ConfigurationCode;
		private string Method;
		private string Type;

		// Public Members
		public bool HasErrors;
		public bool ForceLogin;
		public string FileName;
		public string StylesheetId;
		public string StylesheetTypeCode;
		public string StylesheetName;

		/// <summary>
		/// Initializes the loader.
		/// </summary>
		/// <param name="container">The container for this object.</param>
		public StylesheetLoader(System.ComponentModel.IContainer container)
		{

			/// Required for Windows.Forms Class Composition Designer support
			container.Add(this);
			InitializeComponent();

			// The user can be forced to enter the connection settings even if preferences have been saved from a previous session.
			this.ForceLogin = false;

			// Load the constants from the configuration file.
			this.Assembly = ConfigurationManager.AppSettings["assembly"];
			this.Type = ConfigurationManager.AppSettings["type"];
			this.Method = ConfigurationManager.AppSettings["method"];
			this.ConfigurationCode = ConfigurationManager.AppSettings["configurationCode"];

		}

		/// <summary>
		/// Initializes the loader.
		/// </summary>
		public StylesheetLoader()
		{

			/// Required for Windows.Forms Class Composition Designer support
			InitializeComponent();

			// The user can be forced to enter the connection settings even if preferences have been saved from a previous session.
			this.ForceLogin = false;

			// Load the constants from the configuration file.
			this.Assembly = ConfigurationManager.AppSettings["assembly"];
			this.Type = ConfigurationManager.AppSettings["type"];
			this.Method = ConfigurationManager.AppSettings["method"];
			this.ConfigurationCode = ConfigurationManager.AppSettings["configurationCode"];

		}

		#region Dispose Method
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

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
		/// Read, parse and load the stylesheet into the middle tier.
		/// </summary>
		public void Load()
		{

			// If this flag is set during the processing of the file, the program will exit with an error code.
			this.HasErrors = false;
				
			// If the user wants to specify a new URL and certificate, then prompt for the connection info the next time a 
			// WebTransactionProtocol sends off a batch.
			if (this.ForceLogin)
			{
				WebTransactionProtocol.IsUrlPrompted = true;
				WebTransactionProtocol.IsCredentialPrompted = true;
			}

			// Load up an XML document with the contents of the file specified on the command line.
			XmlDocument stylesheet = new XmlDocument();
			stylesheet.Load(this.FileName);

			// The XML document has several nodes that need to be read -- and then removed -- that contain attributes of the
			// stylesheet.  These nodes can be found easily using the XSL Path functions which need a namespace manager to sort out
			// the tag prefixes.
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(stylesheet.NameTable);
			namespaceManager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
			namespaceManager.AddNamespace("mts", "urn:schemas-markthreesoftware-com:stylesheet");

			// The spreadhseet source has several nodes which contain information about how the data in the XML document should be
			// loaded into the server, such as the stylesheet identifier, the name and the stylesheet style.  They are found at
			// this node.  After these information nodes have been read, they are removed from the stylesheet source.
			XmlNode stylesheetNode = stylesheet.SelectSingleNode("xsl:stylesheet", namespaceManager);
			if (stylesheetNode == null)
				throw new Exception("Syntax Error: missing stylesheet declaration.");
			
			// Find the StylesheetId node.
			XmlNode StylesheetIdNode = stylesheetNode.SelectSingleNode("mts:stylesheetId", namespaceManager);
			if (StylesheetIdNode == null)
				throw new Exception("Syntax Error: missing StylesheetId declaration.");

			// Find the StylesheetStyle node.
			XmlNode stylesheetTypeCodeNode = stylesheetNode.SelectSingleNode("mts:stylesheetTypeCode", namespaceManager);
			if (stylesheetTypeCodeNode == null)
				throw new Exception("Syntax Error: missing StylesheetStyle declaration.");

			// Find the name node.
			XmlNode nameNode = stylesheetNode.SelectSingleNode("mts:name", namespaceManager);
			if (nameNode == null)
				throw new Exception("Syntax Error: missing name declaration.");

			// Extract the data from the XML nodes.
			this.StylesheetId = StylesheetIdNode.InnerText;
			this.StylesheetTypeCode = stylesheetTypeCodeNode.InnerText;
			this.StylesheetName = nameNode.InnerText;

			// Remove the stylesheet nodes from the XSL spreadsheet before loading it into the server.
			stylesheetNode.RemoveChild(StylesheetIdNode);
			stylesheetNode.RemoveChild(stylesheetTypeCodeNode);
			stylesheetNode.RemoveChild(nameNode);

			// Create a command to load the stylesheet from the data loaded from the file.
			Batch batch = new Batch();
			TransactionPlan transaction = batch.Transactions.Add();
			AssemblyPlan assembly = batch.Assemblies.Add(this.Assembly);
			TypePlan type = assembly.Types.Add(this.Type);
			MethodPlan method = transaction.Methods.Add(type, this.Method);
			method.Parameters.Add(new InputParameter("stylesheetId", this.StylesheetId));
			method.Parameters.Add(new InputParameter("stylesheetTypeCode", this.StylesheetTypeCode));
			method.Parameters.Add(new InputParameter("name", this.StylesheetName));
			method.Parameters.Add(new InputParameter("text", stylesheet.InnerXml));

			try
			{

				// Create a new web client that will serve as the connection point to the server and call the web services to execute
				// the command batch.
				WebTransactionProtocol.Execute(batch);

			}
			catch (BatchException)
			{

				foreach (MethodPlan batchMethod in transaction.Methods)
					foreach (Exception exception in batchMethod.Exceptions)
						Console.WriteLine(String.Format("{0}: {1}", method.Parameters["StylesheetId"].Value, exception.Message));

				this.HasErrors = true;

			}

		}

	}

}
