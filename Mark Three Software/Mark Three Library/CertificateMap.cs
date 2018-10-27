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
	/// Represents a mapping between a certificate and a Windows User Account.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	[Serializable]
	public class CertificateMap : ISerializable
	{

		// Public Members
		public bool IsEnabled;
		public string Account;
		public string MappingName;
		public string Password;
		public X509Certificate2 X509Certificate2;

		/// <summary>
		/// Creates a mapping between an X509 Certificate and a Windows User Account.
		/// </summary>
		/// <param name="name">The friendly name of the mapping.</param>
		/// <param name="x509Certificate">The X09 Certificate used to identify the user.</param>
		/// <param name="account">The Domain/Name of the Windows User.</param>
		/// <param name="password">The User's password.</param>
		public CertificateMap(string mappingName, X509Certificate2 x509Certificate, string account, string password)
		{

			// Initialize the object.
			this.IsEnabled = true;
			this.Account = account;
			this.MappingName = mappingName;
			this.Password = password;
			this.X509Certificate2 = x509Certificate;

		}

		/// <summary>
		/// Creates a mapping between an X509 Certificate and a Windows User Account.
		/// </summary>
		/// <param name="serializationInfo">Stores all the data needed to serialize or deserialize an object.</param>
		/// <param name="streamingContext">Describes the source and destination of a given serialized stream, and provides an
		/// additional caller-defined context.</param>
		protected CertificateMap(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{

			// Initialize the object from the serialization stream.
			this.IsEnabled = serializationInfo.GetBoolean("IsEnabled");
			this.Account = serializationInfo.GetString("Account");
			this.MappingName = serializationInfo.GetString("MappingName");
			this.Password = serializationInfo.GetString("Password");
			this.X509Certificate2 = new X509Certificate2(Convert.FromBase64String(serializationInfo.GetString("Certificate")));

		}

		#region ISerializable Members

		/// <summary>
		/// Provieds information needed to serialize this object.
		/// </summary>
		/// <param name="serializationInfo">Stores all the data needed to serialize or deserialize an object.</param>
		/// <param name="streamingContext">Describes the source and destination of a given serialized stream, and provides an
		/// additional caller-defined context.</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{

			// Serialize the members.
			info.AddValue("IsEnabled", this.IsEnabled);
			info.AddValue("Account", this.Account);
			info.AddValue("MappingName", this.MappingName);
			info.AddValue("Password", this.Password);
			info.AddValue("Certificate", Convert.ToBase64String(this.X509Certificate2.RawData));
			
		}

		#endregion

	}

}
