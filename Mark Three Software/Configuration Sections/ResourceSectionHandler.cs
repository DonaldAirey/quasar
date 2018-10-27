namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	/// <summary>
	/// This object is used to parse a section of the configuration file containing information about the
	/// available persistent stores.
	/// </summary>
	public class ResourceSectionHandler : IConfigurationSectionHandler
	{

		/// <summary>
		/// Parses a section of the configuration file for information about the persistent stores.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system.
		/// Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A PersistentStoreSection object created from the data in the configuration file.</returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{

			List<ResourceDescription> resourceDescriptionList = new List<ResourceDescription>();

			foreach (XmlNode xmlNode in section)
				switch (xmlNode.Name)
				{

				case "adoResource":
					{
						string name = xmlNode.Attributes["name"].Value;
						string typeDescription = xmlNode.Attributes["type"].Value;
						string[] parts = typeDescription.Split(',');
						Assembly assembly = Assembly.Load(parts[1].Trim());
						Type type = assembly.GetType(parts[0].Trim());
						resourceDescriptionList.Add(new AdoResourceDescription(name, type));
					}
					break;

				case "sqlResource":
					{
						string name = xmlNode.Attributes["name"].Value;
						string connectionString = xmlNode.Attributes["connectionString"].Value;
						resourceDescriptionList.Add(new SqlResourceDescription(name, connectionString));
					}

					break;

				}

			// This object can be used to find a persistent store by nane and connect to it.
			return resourceDescriptionList;

		}

	}

}
