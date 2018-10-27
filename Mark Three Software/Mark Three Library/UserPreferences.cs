namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.IO;
	using System.IO.IsolatedStorage;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Xml.XPath;

	/// <summary>
	/// Used to read and write user preferences from and to a persistent store.
	/// </summary>
	public class UserPreferences : System.ComponentModel.Component
	{

		private const string profileFileName = "UserPreferences";
		private static int instanceCount;
		private static Hashtable localSettings;
		private static Hashtable globalSettings;

		/// <summary>The user settings stored on the local machine.</summary>
		public static Hashtable LocalSettings {get {return UserPreferences.localSettings;}}
		
		/// <summary>The user settings stored centrally.</summary>
		public static Hashtable GlobalSettings {get {return UserPreferences.globalSettings;}}

		/// <summary>
		/// Create the static resources for the User Preferences.
		/// </summary>
		static UserPreferences()
		{

			// This is used to keep track of instances of the class.  When the instance count reaches zero, it's time to release
			// all the external resources associated with this component.
			UserPreferences.instanceCount = 0;

			// Load the settings from the local (local machine) persistent store.
			LocalRead();

			// Load the settings from the global (server) persistent store.
//			GlobalRead();
			UserPreferences.globalSettings = new Hashtable();
		
		}

		/// <summary>
		/// Create the instance resources for User Preferences.
		/// </summary>
		/// <param name="container">A container that holds this object.</param>
		public UserPreferences(System.ComponentModel.IContainer container)
		{

			// Add this component to the list of components managed by it's client.
			container.Add(this);

			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the 
			// external resources (namely the thread) are destroyed.
			UserPreferences.instanceCount++;

		}

		/// Create the instance resources for User Preferences.
		public UserPreferences()
		{

			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the 
			// external resources (namely the thread) are destroyed.
			UserPreferences.instanceCount++;

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			if (disposing)
			{

				// When there are no longer any owners of this component, the data is saved to the local (local machine) and 
				// global (server) persistent stores.
				if (--UserPreferences.instanceCount == 0)
				{
					LocalWrite();
					GlobalWrite();
				}

			}

			// Call the event dispatcher for the finalize method when the object is no longer referenced.
			base.Dispose(disposing);

		}

		/// <summary>
		/// Load the settings from the local persistent store.
		/// </summary>
		private static void LocalRead()
		{

			IsolatedStorageFileStream isolatedStorageFileStream = null;

			UserPreferences.localSettings = new Hashtable();

			try
			{

				// See if a file already exists with the user's preferences.  The scope of the isolated store is the 'Batch.Assembly' level, which means that
				// any code using this class will share the same settings.
				IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly();
				if (isolatedStorageFile.GetFileNames(profileFileName).Length != 0)
				{

					// Open the "User Preferences" store for this application/user combination.
					isolatedStorageFileStream = new IsolatedStorageFileStream(profileFileName,
						FileMode.Open, FileAccess.Read, FileShare.None, isolatedStorageFile);

					BinaryFormatter binaryFormatter = new BinaryFormatter();

					UserPreferences.localSettings = (Hashtable)binaryFormatter.Deserialize(isolatedStorageFileStream);

				}

			}
			catch (Exception exception)
			{

				UserPreferences.localSettings.Clear();

				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				if (isolatedStorageFileStream != null)
					isolatedStorageFileStream.Close();

			}

		}
		
		/// <summary>
		/// Save the user settings to a persistent data store.
		/// </summary>
		private static void LocalWrite()
		{

			IsolatedStorageFileStream isolatedStorageFileStream = null;

			try
			{

				// Create the document that will hold the application settings.
				IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly();

				// Create the document that will hold the application settings.
				isolatedStorageFileStream = new IsolatedStorageFileStream(profileFileName,
					FileMode.Create, FileAccess.Write, FileShare.None, isolatedStorageFile);

				BinaryFormatter binaryFormatter = new BinaryFormatter();

				binaryFormatter.Serialize(isolatedStorageFileStream, UserPreferences.localSettings);

			}
			catch (Exception exception)
			{

				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				if (isolatedStorageFileStream != null)
					isolatedStorageFileStream.Close();

			}

		}

		/// <summary>
		/// Load the settings from the local persistent store.
		/// </summary>
		private static void GlobalRead()
		{

			try
			{

//				FileStream fileStream = new FileStream("SettingsTest", FileMode.Open, FileAccess.Read);
//				BinaryReader binaryReader = new BinaryReader(fileStream);
//				int streamSize = binaryReader.ReadInt32();
//				Byte[] settingStream = binaryReader.ReadBytes(streamSize);
//				binaryReader.Close();
//
//				MemoryStream memoryStream = new MemoryStream(settingStream);
//				BinaryFormatter binaryFormatter = new BinaryFormatter();
//				UserPreferences.globalSettings = (Hashtable)binaryFormatter.Deserialize(memoryStream);
//				memoryStream.Close();

			}
			catch (Exception exception)
			{

				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

		}
		
		/// <summary>
		/// Save the user settings to a persistent data store.
		/// </summary>
		private static void GlobalWrite()
		{

			try
			{

//				MemoryStream memoryStream = new MemoryStream();
//				BinaryFormatter binaryFormatter = new BinaryFormatter();
//				binaryFormatter.Serialize(memoryStream, UserPreferences.globalSettings);
//
//				FileStream fileStream = new FileStream("SettingsTest", FileMode.OpenOrCreate, FileAccess.Write);
//				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
//				byte[] settingStream = memoryStream.GetBuffer();
//				binaryWriter.Write(settingStream.Length);
//				binaryWriter.Write(settingStream);
//				binaryWriter.Close();

			}
			catch (Exception exception)
			{

				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}

		}

	}

}
