namespace MarkThree
{

	using System;
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	/// <summary>
	/// This object is used to parse a section of the configuration file containing information about the
	/// available persistent stores.
	/// </summary>
	public class ViewerSectionHandler : IConfigurationSectionHandler
	{

		private const int TypeIndex = 0;
		private const int AssemblyIndex = 1;

		/// <summary>
		/// Parses a section of the configuration file for information about the persistent stores.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system.
		/// Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A ViewerSection object created from the data in the configuration file.</returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{

			// Create an object to hold the data from the 'viewer' section of the application configuration file.
			ViewerSection viewerSection = new ViewerSection();

			try
			{

				// Read each of the nodes that contain information about the persistent stores and place them in the table.
				foreach (XmlNode xmlNode in section.SelectNodes("add"))
				{

					// The 'type' section of the configuration file is modeled after other places in the OS where the type and 
					// assembly information are combined in the same string.  A simpler method might have been to break aprart the
					// type string from the assembly string, but it's also a good idea to use standards where you find them.  In
					// any event, when the 'type' specification is done this way, the padded spaces need to be removed from the
					// values onced they're broken out from the original string.
					string typeSpecification = xmlNode.Attributes["type"].Value;
					if (typeSpecification == null)
						throw new Exception("Syntax error in configuration file section 'viewers'.");

					string[] typeParts = typeSpecification.Split(new char[] {','});
					if (typeParts.Length != 2)
						throw new Exception("Syntax error in configuration file section 'viewers'.");

					Assembly typeAssembly = Assembly.Load(typeParts[ViewerSectionHandler.AssemblyIndex].Trim());
					Type typeType = typeAssembly.GetType(typeParts[ViewerSectionHandler.TypeIndex]);

					string viewerSpecification = xmlNode.Attributes["viewer"].Value;
					if (viewerSpecification == null)
						throw new Exception("Syntax error in configuration file section 'viewers'.");

					string[] viewerParts = viewerSpecification.Split(new char[] {','});
					if (viewerParts.Length != 2)
						throw new Exception("Syntax error in configuration file section 'viewers'.");

					Assembly viewerAssembly = Assembly.Load(viewerParts[ViewerSectionHandler.AssemblyIndex].Trim());
					Type viewerType = viewerAssembly.GetType(viewerParts[ViewerSectionHandler.TypeIndex]);

					// Add the viewer information to the section.  Each of these ViewerInfo items describes what kind of object the 
					// viewer is associated with and where to find the viewer.
					viewerSection.Add(new ViewerInfo(typeType, viewerType));

				}

			}
			catch (Exception exception)
			{

				// Make sure that any errors caught while trying to load the viewer info is recorded in the log.  A system
				// administrator can look through these to figure out why the viewer information isn't formatted correctly.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

			// This object can be used to find a persistent store by nane and connect to it.
			return viewerSection;

		}

	}

}
