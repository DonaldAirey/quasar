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
	class ImagePacker : System.ComponentModel.Component
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
		public int ImageCount;
		public string Path;
		public string OutputFile;

		/// <summary>
		/// Initializes the loader.
		/// </summary>
		public ImagePacker()
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
		protected override void Dispose(bool disposing)
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
			xmlTextWriter.WriteAttributeString("name", "Images");

			// Read the images out of each of the directories.
			LoadImageType(xmlTextWriter, "Small");
			LoadImageType(xmlTextWriter, "Small Shadow");
			LoadImageType(xmlTextWriter, "Medium");
			LoadImageType(xmlTextWriter, "Medium Shadow");
			LoadImageType(xmlTextWriter, "Large");
			LoadImageType(xmlTextWriter, "Large Shadow");
			LoadImageType(xmlTextWriter, "Extra Large");
			LoadImageType(xmlTextWriter, "Extra Large Shadow");

			// Close out the opening elements.
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Close();

		}

		public void LoadImageType(XmlTextWriter xmlTextWriter, string subDirectoryName)
		{

			string subdirectory = System.IO.Path.Combine(this.Path, subDirectoryName);

			DirectoryInfo directoryInfo = new DirectoryInfo(subdirectory);
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

				string imageId = string.Format("{0} {1}", System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName), subDirectoryName);

				xmlTextWriter.WriteElementString("imageId", imageId);
				xmlTextWriter.WriteElementString("image", Convert.ToBase64String(memoryStream.GetBuffer()));

				// End of the 'method' element.
				xmlTextWriter.WriteEndElement();

				// This keeps track of the number of images packed.
				this.ImageCount++;

			}

		}

	}

}
