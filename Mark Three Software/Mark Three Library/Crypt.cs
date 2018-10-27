namespace MarkThree
{

	using System;
	using System.Runtime.InteropServices;
	using System.Security.Cryptography.X509Certificates;

	public class Crypt
	{

		// HCERTSTORE WINAPI CertOpenSystemStore(HCRYPTPROV hprov, LPTCSTR szSubsystemProtocol);
		[DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern IntPtr CertOpenSystemStore(IntPtr hCryptProv, string storename);

		// BOOL WINAPI CertCloseStore(HCERTSTORE hCertStore, DWORD dwFlags);
		[DllImport("crypt32.dll", SetLastError=true)]
		public static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);

		// BOOL WINAPI CertFreeCertificateContext(PCCERT_CONTEXT pCertContext);
		[DllImport("crypt32.dll", SetLastError=true)]
		public static extern bool CertFreeCertificateContext(IntPtr hCertStore);

		// PCCERT_CONTEXT WINAPI CertEnumCertificatesInStore(HCERTSTORE hCertStore, PCCERT_CONTEXT pPrevCertContext);
		[DllImport("crypt32.dll", SetLastError=true)]
		public static extern IntPtr CertEnumCertificatesInStore(IntPtr hCertStore, IntPtr pPrevCertContext);

		/// <summary>
		/// Gets a collection of certificates from the specified store.
		/// </summary>
		/// <param name="storeName">The name of the certificate store.</param>
		/// <returns>A collection of certificates from the specified store.</returns>
		public static X509CertificateCollection GetCertificateStore(string storeName)
		{

			// The collection of certificates will be returned in this object.
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();

			// If the specified certificate store can be opened, then collect all the certificates found.
			IntPtr hCertificateStore = Crypt.CertOpenSystemStore(IntPtr.Zero, storeName);
			if (hCertificateStore != IntPtr.Zero)
			{

				// Used to hold the current certificate in the enumeration.
				IntPtr hCertificateContext = IntPtr.Zero;

				while (true)
				{

					// The enumeration of certificates ends when a null (zero) is read.  Read all the certificates out
					// of the store until a null is passed back for a handle.  This means we've reached the end of the list.
					hCertificateContext = Crypt.CertEnumCertificatesInStore(hCertificateStore, hCertificateContext);
					if (hCertificateContext == IntPtr.Zero)
						break;

					// Add the most recently read certificate to the list.
					x509CertificateCollection.Add(new X509Certificate(hCertificateContext));

				}

				// Close the store when we're done.
				Crypt.CertCloseStore(hCertificateStore, 0);

			}

			// This is a collection of all the certificates in the specified store.
			return x509CertificateCollection;

		}

	}

}
