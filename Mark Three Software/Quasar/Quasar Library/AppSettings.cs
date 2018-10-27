/*************************************************************************************************************************
*
*	File:			AppSettings.cs
*	Description:	This file contains the class for reading and writing application settings to the configuration file.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace MarkThree.Quasar
{
	/// <summary>
	/// Provides a means or reading and writing the state of the application to a file.
	/// </summary>
	/// <remarks>This object provides a means or reading and writing XML files that contain the state and user preferences
	/// selected in the application.  Since it inherits the XmlDocument class, all the XML methods of searching, reading
	/// writing user preferences are available.  This class is superior to the 'AppSettings' and 'AppSettingsReader'
	/// classes in that it provides a method of writing the file when new entries have been added.  Additionally, if the
	/// entries don't exist, they can be created at the proper place in the configuration file.  In all other respects,
	/// 'AppSettings' attempts to use the same XML file structure.  This class also provides regular expression parsing
	/// so that often used, complex data types, like 'Point' and 'Size', can be read into structures.</remarks>
	public class AppSettings
	{

		static private XmlDocument application_settings;
		static private XmlNode app_settings_element;
		static private FileInfo config_file_info;
		
		/// <summary>
		/// Initializes a new instance of the MarkThree.Quasar.AppSettings class.
		/// </summary>
		/// <remarks>This constructor will open the configuration file.  If the file doesn't exist, it has all the logic
		/// to create the outline of a configuration file.</remarks>
		static AppSettings()
		{

			// Create the document that will hold the application settings.
			application_settings = new XmlDocument();

			// The name of the config file is the name of the executable with a '.config' extension.
			config_file_info = new FileInfo(Application.ExecutablePath + ".config");

			// If the configuration file exists, read it into the XML document.
			if (config_file_info.Exists)
				application_settings.Load(config_file_info.FullName);

			// Find the root node.  If it doesn't exist, create it and add it to the root of the document.
			XmlNode configuration_element = application_settings.SelectSingleNode("configuration");
			if (configuration_element == null)
			{
				XmlDeclaration xml_declaration = application_settings.CreateXmlDeclaration("1.0", "Windows-1252", null);
				application_settings.AppendChild(xml_declaration);
				configuration_element = application_settings.CreateElement("configuration");
				application_settings.AppendChild(configuration_element);
			}
		
			// Find the 'appSettings' node.  This is where all of the key-value pairs are located.  If this section
			// doesn't exist, create it and add it to the 'configuration' section.
			app_settings_element = configuration_element.SelectSingleNode("appSettings");
			if (app_settings_element == null)
			{
				app_settings_element = application_settings.CreateElement("appSettings");
				configuration_element.AppendChild(app_settings_element);
			}
		
		}

		/// <summary>
		/// Returns an XmlNodeList containing all the elements that match the specified tag_name.
		/// </summary>
		/// <param name="tag_name">The nameIdentifying the elements to be selected.</param>
		/// <returns>A list of XmlNode items that match the specified tag_name.</returns>
		static public XmlNodeList GetElementsByTagName(string tag_name)
		{
			return application_settings.GetElementsByTagName(tag_name);
		}

		/// <summary>
		/// Returns an XmlNodeList containing all the elements that match the specified XPath specification.
		/// </summary>
		/// <param name="xpath">An XPath specification</param>
		/// <returns>A list of XmlNode items that match the specified XPath specification.</returns>
		static public XmlNodeList SelectNodes(string xpath)
		{
			return application_settings.SelectNodes(xpath);
		}
		
		/// <summary>
		/// Save the user settings to disk.
		/// </summary>
		static public void Save()
		{
			application_settings.Save(config_file_info.FullName);
		}

		/// <summary>
		/// Finds the node containing the key value in the 'appSettings' section.
		/// </summary>
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The node containing the key, null if the node doesn't exist.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		static public XmlNode FindKey(string keyName)
		{
			
			// A null value isn't allowed in the configuration file.
			if (keyName == null)
				throw new System.ArgumentNullException("Value cannot be null.");

			// Search all the 'add' elements for a key that matches 'keyName'.  If an element is found that matches the
			// criteria, return that node.
			foreach(XmlNode node in app_settings_element.SelectNodes("add"))
				if (node.Attributes["key"].Value == keyName)
					return node;

			// At this point we've searched the configuration file and not found an 'add' node with the given key.
			return null;

		}

		/// <summary>
		/// Finds the node containing the key value.
		/// </summary>
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The node containing the key, null if the node doesn't exist.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static protected XmlNode GetKey(string keyName)
		{
			
			// Find the key-value pair in the 'appSettings' section.  If it doesn't exist, throw an exception.
			XmlNode add_element = FindKey(keyName);
			if (add_element == null)
			{
				String message = "The key '{0}' does not exist in the appSettings configuration section.";
				throw new System.InvalidOperationException(String.Format(message, keyName));
			}

			// Return the node that matches the 'key' value in the 'appSettings' section.
			return add_element;

		}

		/// <summary>
		/// Creates an element in the 'appSettings' section with the specified key.
		/// </summary>
		/// <param name="keyName">The name of the key.</param>
		/// <returns>A node containing the newly constructed element</returns>
		/// <remarks>This method is used to create a new element if none exists.  It's used in the
		/// <code>SetValue</code> method to create entries where none existed before.</remarks>
		static protected XmlNode CreateKey(string keyName)
		{

			// Create a new key-value pair element.  These elements are named 'add' for reasons unknown.  They have two
			// attributes: 'key' and 'value'.
			XmlNode add_element = application_settings.CreateElement("add");
			XmlAttribute key_attribute = application_settings.CreateAttribute("key");
			XmlAttribute value_attribute = application_settings.CreateAttribute("value");

			// Arrange the XML node parts.  The attributes are children of the element.
			add_element.Attributes.Append(key_attribute);
			add_element.Attributes.Append(value_attribute);
			app_settings_element.AppendChild(add_element);

			// Set the 'key' attribute to the given key name.
			key_attribute.InnerText = keyName;

			// We now have a properly formed key-value pair that can be placed in the 'appSettings' section.
			return add_element;

		}

		/// <summary>
		/// Returns the boolean value of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The boolean value associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public bool GetBoolean(string keyName)
		{

			// Get the text associated with the key and convert it to a 'true' or 'false' value.
			return Convert.ToBoolean(GetKey(keyName).Attributes["value"].InnerText);

		}

		/// <summary>
		/// Returns the integer value of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The integer value associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public int GetInteger(string keyName)
		{

			// Get the text associated with the key and convert it to an integer value.
			return Convert.ToInt32(GetKey(keyName).Attributes["value"].InnerText);

		}

		/// <summary>
		/// Returns the string value of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The string value associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public string GetString(string keyName)
		{

			// Return the text associated with the key.
			return GetKey(keyName).Attributes["value"].InnerText;

		}

		/// <summary>
		/// Returns the string value of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <param name="default_value">A value that is returned if the key-value pair doesn't exist.</param>
		/// <returns>The string value associated with the key, 'default_value' if the key doesn't exist.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		static public string GetString(string keyName, string default_value)
		{

			// Find the string.  If it exists, then return the value associated with it.
			XmlNode xmlNode = FindKey(keyName);
			if (xmlNode != null)
				return xmlNode.Attributes["value"].InnerText;

			// If the key-value pair doesn't exist, return a default value.
			return default_value;

		}

		/// <summary>
		/// Returns a Point structure of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The Point structure associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public Point GetPoint(string keyName)
		{

			Point point = new Point();

			// Parse the X and Y values out of the text associated with the key.  This is the same text that is generated
			// with the ToString() method in 'Point'.
			Regex point_regex = new Regex("{X=(?<X>[^,]+), *Y=(?<Y>.+)}");
			Match match = point_regex.Match(GetKey(keyName).Attributes["value"].InnerText);
			if (match.Success)
			{
				point.X = Convert.ToInt32(match.Groups["X"].Value);
				point.Y = Convert.ToInt32(match.Groups["Y"].Value);
			}

			// Return the Point.
			return point;

		}

		/// <summary>
		/// Returns a Size structure of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The Size structure associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public Size GetSize(string keyName)
		{

			Size size = new Size();

			// Parse the X and Y values out of the text associated with the key.  This is the same text that is generated
			// with the ToString() method in 'Point'.
			Regex size_regex = new Regex("{Width=(?<Width>[^,]+), *Height=(?<Height>.+)}");
			Match match = size_regex.Match(GetKey(keyName).Attributes["value"].InnerText);
			if (match.Success)
			{
				size.Width = Convert.ToInt32(match.Groups["Width"].Value);
				size.Height = Convert.ToInt32(match.Groups["Height"].Value);
			}

			// Return the Size.
			return size;

		}

		/// <summary>
		/// Returns an ObjectArgs structure of a key-value entry in the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The Size structure associated with the key.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		static public ObjectArgs GetObjectArgs(string keyName)
		{

			// Find the key in the application settings.
			XmlNode keyNode = GetKey(keyName);
			
			// If the key exists, attempt to parse it.
			if (keyNode != null)
			{
			
				ObjectArgs objectArgs = new ObjectArgs();

				// Parse the X and Y values out of the text associated with the key.  This is the same text that is generated
				// with the ToString() method in 'Point'.
				Regex size_regex = new Regex("{Type=(?<Type>[^,]+), *Id=(?<Id>.+), *Name=(?<Name>.+)}");
				Match match = size_regex.Match(GetKey(keyName).Attributes["value"].InnerText);
				if (match.Success)
					objectArgs = new ObjectArgs(match.Groups["Type"].Value, Convert.ToInt32(match.Groups["Id"].Value), match.Groups["Name"].Value);

				// Return the object argument extracted from the file.
				return objectArgs;

			}

			// This indicates that the key wasn't found.
			return null;

		}

		/// <summary>
		/// Stores the value-key pair in the 'appSettings' section of the configuration file.
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <param name="value">The value associated with the key.</param>
		static public void SetValue(string keyName, object value)
		{

			// Find the node that contains the key name.  If it doesn't exist, create a suitable element.
			XmlNode add_element = FindKey(keyName);
			if (add_element == null)
				add_element = CreateKey(keyName);

			// Update (add) the value to the element.
			add_element.Attributes["value"].InnerText = value.ToString();

		}

	}
}
