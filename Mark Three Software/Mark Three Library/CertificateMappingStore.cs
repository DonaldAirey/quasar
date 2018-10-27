namespace MarkThree
{

	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using System.Security.Cryptography.X509Certificates;

	/// <summary>
	/// A collection of mappings of client certificates to Windows User Accounts.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class CertificateMappingStore : IEnumerable<CertificateMap>
	{

		// Public Readonly Members
		public readonly List<CertificateMap> CertificateMaps;
		public readonly Dictionary<X509Certificate2, CertificateMap> MapByCertificate;

		/// <summary>
		/// Create a store that associates client certificates with Windows User Accounts.
		/// </summary>
		public CertificateMappingStore()
		{

			// This table is used to find the mapping based on the certificate.
			this.MapByCertificate = new Dictionary<X509Certificate2, CertificateMap>();

			// Initialize the object.
			try
			{

				// This will read the metadatabase into memory using the application configuration to specify the location of the
				// metadata file.
				using (FileStream fileStream = new FileStream(ConfigurationManager.AppSettings["metaDatabase"], FileMode.Open))
				{

					// This will create a binary serializer that can read in from the metadatabase file.
					BinaryReader metaDatabase = new BinaryReader(fileStream);
					BinaryFormatter binaryFormatter = new BinaryFormatter();

					// Only the unordered list is stored in the file.  The hashtable is initialized from the unordered list once it
					// is read from the serialized stream.
					this.CertificateMaps = binaryFormatter.Deserialize(fileStream) as List<CertificateMap>;
					foreach (CertificateMap certificateMap in this.CertificateMaps)
						this.MapByCertificate.Add(certificateMap.X509Certificate2, certificateMap);
				}

			}
			catch
			{

				// In the event of an error, use a clean list of certificate mappings.
				this.CertificateMaps = new List<CertificateMap>();

			}

		}

		/// <summary>
		/// Save the changes to a file.
		/// </summary>
		public void Save()
		{

			// This will write the changes to the metadatabase file using the binary formatter to serialize the unorderd list of
			// mappings.  The hash table doesn't need to be saved because it contains the same information in the unordered list.
			using (FileStream fileStream = new FileStream(ConfigurationManager.AppSettings["metaDatabase"], FileMode.Create))
			{
				BinaryReader metaDatabase = new BinaryReader(fileStream);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream, this.CertificateMaps);
			}

		}

		/// <summary>
		/// Finds a certificate based on the Mapping Name.
		/// </summary>
		/// <param name="mappingName">The friendly name used to identify the mapping.</param>
		/// <returns>The certificate mapping matching the name or null if no mapping could be found.</returns>
		public CertificateMap Find(string mappingName)
		{

			// Use the brute force method to find the mapping.  This function is always driven from a user interface, so performance
			// is not an issue.  Return any certificate that matches the name.
			foreach (CertificateMap certificateMap in this.CertificateMaps)
				if (certificateMap.MappingName == mappingName)
					return certificateMap;

			// This indicates the mapping doesn't exist.
			return null;

		}

		/// <summary>
		/// Add a mapping to the store.
		/// </summary>
		/// <param name="certificateMap">An object that maps a client certificate to a Windows User Account.</param>
		public void Add(CertificateMap certificateMap)
		{

			// Reject the new item if the mapping name has already been used.
			if (Find(certificateMap.MappingName) != null)
				throw new Exception("This mapping already exists.");

			// The mappings between accounts and certicates can be accessed using an unordered list or the certificate.  Both the
			// unordered list and the hash table will get an instance of the mapping.
			this.MapByCertificate.Add(certificateMap.X509Certificate2, certificateMap);
			this.CertificateMaps.Add(certificateMap);

		}

		/// <summary>
		/// Deletes a mapping based on the Mapping Name.
		/// </summary>
		/// <param name="mappingName">The name of the certificate mapping to be removed from the store.</param>
		public void Delete(string mappingName)
		{

			// Find the certificate and remove it from the unordered list and the hash table.
			CertificateMap certificateMap = Find(mappingName);
			if (certificateMap != null)
			{
				this.CertificateMaps.Remove(certificateMap);
				this.MapByCertificate.Remove(certificateMap.X509Certificate2);
			}

		}

		/// <summary>
		/// Finds a certificate mapping based on a X509 Certificate.
		/// </summary>
		/// <param name="x509Certificate">The certificate used to identify the mapping.</param>
		/// <returns>A mapping between a certificate and a user account or a null if the certificate hasn't been mapped.</returns>
		public CertificateMap this[X509Certificate2 x509Certificate]
		{

			get
			{

				// Return the certificate map indexed by the given X509 certificate.
				CertificateMap certificateMap;
				return this.MapByCertificate.TryGetValue(x509Certificate, out certificateMap) ? certificateMap : null;

			}

		}

		#region IEnumerable<CertificateMap> Members

		public IEnumerator<CertificateMap> GetEnumerator()
		{
			return this.CertificateMaps.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.CertificateMaps.GetEnumerator();
		}

		#endregion

	}

}
