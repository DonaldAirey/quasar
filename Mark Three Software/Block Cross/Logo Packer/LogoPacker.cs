namespace MarkThree.Guardian.Utilities
{

	using MarkThree;
	using MarkThree.Client;
	using System;
	using System.Configuration;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Xml;
	using System.Xml.XPath;

	/// <summary>
	/// Loads company logos into the Security table.
	/// </summary>
	class LogoPacker : System.ComponentModel.Component
	{

		// Private Members
		private MarkThree.UserPreferences userPreferences;
		private System.ComponentModel.IContainer components;
		private string Assembly;
		private string ConfigurationId;
		private string Method;
		private string Type;

		// Public Members
		public bool HasErrors;
		public bool ForceLogin;
		public int LogoCount;
		public string Path;
		public string OutputFile;

		/// <summary>
		/// Initializes the loader.
		/// </summary>
		public LogoPacker()
		{

			/// Required for Windows.Forms Class Composition Designer support
			InitializeComponent();

			// The user can be forced to enter the connection settings even if preferences have been saved from a previous session.
			this.ForceLogin = false;

			// Load the constants from the configuration file.
			this.Assembly = ConfigurationManager.AppSettings["assembly"];
			this.Type = ConfigurationManager.AppSettings["type"];
			this.Method = ConfigurationManager.AppSettings["method"];
			this.ConfigurationId = ConfigurationManager.AppSettings["configurationId"];

		}

		/// <summary> 
		/// Dispose of managed resources.
		/// </summary>
		protected override void Dispose( bool disposing )
		{

			// Dispose of any managed components.
			if (disposing)
				if (components != null)
					components.Dispose();

			// Allow the base class to complete the destruction of this object.
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
		/// Read and load the company logos into the Security table.
		/// </summary>
		public void Load()
		{

			// If this flag is set during the processing of the file, the program will exit with an error code.
			this.HasErrors = false;

			// This counts the number of logos processed.
			this.LogoCount = 0;
				
			// If the user wants to specify a new URL and certificate, then prompt for the connection info the next time a 
			// WebTransactionProtocol sends off a batch.
			if (this.ForceLogin)
			{
				WebTransactionProtocol.IsUrlPrompted = true;
				WebTransactionProtocol.IsCredentialPrompted = true;
			}

			// This will create the XML file and the document root node.
			XmlTextWriter xmlTextWriter = new XmlTextWriter(this.OutputFile, System.Text.Encoding.UTF8);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument(true);
			xmlTextWriter.WriteStartElement("batch");
			xmlTextWriter.WriteAttributeString("name", "Corporate Logos");

			DirectoryInfo directoryInfo = new DirectoryInfo(this.Path);
			FileInfo[] files = directoryInfo.GetFiles("*.png");
			foreach (FileInfo fileInfo in files)
			{

				xmlTextWriter.WriteStartElement("method");
				xmlTextWriter.WriteAttributeString("assembly", this.Assembly);
				xmlTextWriter.WriteAttributeString("type", this.Type);
				xmlTextWriter.WriteAttributeString("name", this.Method);

				Bitmap bitmap = new Bitmap(fileInfo.FullName);
				MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Png);
				memoryStream.Close();

				string securityId = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName);

				xmlTextWriter.WriteElementString("configurationId", this.ConfigurationId);
				xmlTextWriter.WriteElementString("equityId", securityId);
				xmlTextWriter.WriteElementString("logo", Convert.ToBase64String(memoryStream.GetBuffer()));

				// End of the 'method' element.
				xmlTextWriter.WriteEndElement();

				// Keep track of the number of logos processed.
				this.LogoCount++;

			}

			// Close out the opening elements.
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Close();

		}

	}

}
