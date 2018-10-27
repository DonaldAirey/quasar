namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Configuration;
	using System.Data;
	using System.IO;
	using System.IO.Compression;
	using System.Net;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Security.Cryptography.X509Certificates;
	using System.Web;
	using System.Windows.Forms;
	using System.Text;

	/// <summary>
	/// Protocol for sending structured transactions to the server and having them processed.
	/// </summary>
	public class HttpBatchProcessor : MarkThree.IBatchHandler
	{

		// Constants
		private const int defaultTimeout = 10000;
		private const string myStoreName = "My";
		private const string webServiceMru0Key = "webServiceMru0";
		private const string certificateNameKey = "certificateName";
		private const string usernameKey = "userName";
		private const string domainKey = "domain";

		// Protected Members
		protected static X509CertificateCollection clientCertificates;
		protected static CredentialCache credentialCache;
		protected static NetworkCredential networkCredential;

		// Public Members
		public static string Url;
		public static int Timeout;
		public static bool IsUrlPrompted;
		public static bool IsCredentialPrompted;
		public static AuthenticationMode AuthenticationMode;

		/// <summary>This event is raised when the credentials change.</summary>
		public static EventHandler CredentialsChanged;

		/// <summary>
		/// Initializes the static elements of a 'WebTransactionProtocol' connection.
		/// </summary>
		static HttpBatchProcessor()
		{

			// We need to reference the user preferences in this static constructor.
			UserPreferences userPreferences = new UserPreferences();

			// Use a default timout for the Http Request if one was specified in the application configuration file.
			string timeoutText = ConfigurationManager.AppSettings["httpTimeout"];
			HttpBatchProcessor.Timeout = timeoutText == null ? defaultTimeout : Convert.ToInt32(timeoutText);

			// Initially, the decision about prompting for a URL is decided by whether there is a recently saved URL to try.  
			// After the first WebException, we'll prompt them until a good one is entered.
			HttpBatchProcessor.Url = (string)UserPreferences.LocalSettings[webServiceMru0Key];
			HttpBatchProcessor.IsUrlPrompted = HttpBatchProcessor.Url == string.Empty;

			// Initially the decision to prompt the user for credentials is made by whether a complete set of information for
			// establishing a session can be collected from the local settings.  If there isn't enough information, the user will
			// be prompted.  After the first time, a 401 error will indicate that the user should be prompted for their credentials
			// again.
			HttpBatchProcessor.IsCredentialPrompted = false;

			// Select the method for authenticating the user when they connect to the web transaction server.  The default is to
			// use certificates.  This can be overridden by configuration settings.
			HttpBatchProcessor.AuthenticationMode = AuthenticationMode.Certificate;
			string authenticationModeText = ConfigurationManager.AppSettings["authenticationMode"];
			if (authenticationModeText != null)
				HttpBatchProcessor.AuthenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode),
					authenticationModeText);

			// Set up the details of authentication based on the selected authentication mode.
			switch (HttpBatchProcessor.AuthenticationMode)
			{

				case AuthenticationMode.Certificate:

					// This is a collection of cached certificates used for SSL security.
					HttpBatchProcessor.clientCertificates = new X509CertificateCollection();

					// If a certificate name was stored in the user preferences, then attempt to match it up against the personal
					// certificate store.  If a previosly used certificate was found, then put it in the cache where it can be used the 
					// next time a connection is established.
					string certificateName = (string)UserPreferences.LocalSettings[certificateNameKey];
					if (certificateName != null)
						foreach (X509Certificate x509Certificate in Crypt.GetCertificateStore(myStoreName))
							if (x509Certificate.Subject == certificateName)
								HttpBatchProcessor.clientCertificates.Add(x509Certificate);

					// If the user preferences has a valid certificate, then the prompting can be bypassed.  Otherwise, the first time
					// a request is sent, the user will be asked for a URL and a certificate.
					HttpBatchProcessor.IsCredentialPrompted = HttpBatchProcessor.clientCertificates.Count == 0;

					break;

				case AuthenticationMode.WindowsIntegrated:

					// Integrated Mode uses the current user's security context, there is nothing for which to prompt the user.
					HttpBatchProcessor.IsCredentialPrompted = false;

					break;

				case AuthenticationMode.Basic:

					// Basic authorization over SSL can be used for the web request. See if the user has some saved credentials.
					HttpBatchProcessor.networkCredential = new NetworkCredential();
					HttpBatchProcessor.networkCredential.UserName = (string)UserPreferences.LocalSettings[usernameKey];
					HttpBatchProcessor.networkCredential.Domain = (string)UserPreferences.LocalSettings[domainKey];

					// The user must always be prompted for the password in Basic Mode due to an inability to secure the password 
					// on the local machine.
					HttpBatchProcessor.IsCredentialPrompted = true;

					break;

			}

			// Since the UserPreferences were used by a static non-component, the resources need to be cleaned up explicitly.  This
			// should balance the books on the UserPreferences component.
			userPreferences.Dispose();

		}

		/// <summary>
		/// Broadcasts an event when the credentials have changed.
		/// </summary>
		public static void OnCredentialsChanged()
		{

			// Brodcast the event to anyone listening that the credentials are different.
			if (HttpBatchProcessor.CredentialsChanged != null)
				HttpBatchProcessor.CredentialsChanged(typeof(HttpBatchProcessor), EventArgs.Empty);

		}

		/// <summary>
		/// Checks to see if a URL is needed from the user.
		/// </summary>
		private static void GetUrl()
		{

			// Check the global flag to see if we need to prompt the user for the URL to the server.
			if (HttpBatchProcessor.Url == null || HttpBatchProcessor.IsUrlPrompted)
			{

				// Prompte the user for a URL.  Throwing the 'UserAbortException' when the user hits the 'Cancel' button will
				// generally terminate the application unless it is caught.  In general, though, the application should be allowed
				// to exit when the user can't enter a valid server URL.
				FormUrl urlDialog = new FormUrl();
				if (urlDialog.ShowDialog() != DialogResult.OK)
					throw new UserAbortException("User cancelled the operation");

				// This becomes the URL for this session.
				HttpBatchProcessor.Url = urlDialog.Url;

				// There's no need to prompt again until some error forces the user back here.
				HttpBatchProcessor.IsUrlPrompted = false;

				// Dialogs need to be explicitly disposed or they'll hang around until the garbage is collected.
				urlDialog.Dispose();

			}

		}

		/// <summary>
		/// Creates a header for Basic Authentication in an HTTP message.
		/// </summary>
		/// <returns>A Basic Authentication header containing the user's credentials.</returns>
		private static string GetBasicAuthorizationHeader()
		{

			// The user, password and optional domain name are coded up in a 64 bit-encoded ascii string.
			string domain = string.Empty;
			if (HttpBatchProcessor.networkCredential.Domain != null && HttpBatchProcessor.networkCredential.Domain != string.Empty)
				domain = HttpBatchProcessor.networkCredential.Domain + @"\";
			string userAndPassword = domain + HttpBatchProcessor.networkCredential.UserName + ":" + HttpBatchProcessor.networkCredential.Password;
			return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userAndPassword));

		}

		/// <summary>
		/// Constructs an HttpWebRequest specifically destined for the transaction server.
		/// </summary>
		private static HttpWebRequest CreateCertificateRequest()
		{

			// This section of code is thread safe.  Since the user can modify the URL or the credentials of other threads that
			// want to use this class, the class itself is locked while the user is prompted (if prompting is necessary).
			lock (typeof(HttpBatchProcessor))
			{

				// Get the URL.
				GetUrl();

				// If the WebTransactionProtocol has successfully connected once, then try to use the URL and Certificate from the
				// previous connection for this request.
				if (HttpBatchProcessor.IsCredentialPrompted)
				{

					// A Certificate is also required for SSL communication.  A cache of certificates is kept by this class to speed up
					// the task of connecting to the server.  Since the URL has changed at this point, none of the previous
					// certificates that were selected by the user should be used.
					HttpBatchProcessor.clientCertificates.Clear();

					// The design goal of the certificate selection sequence is to limit the number of decisions that the user has to
					// make.  The personal certficate store is used to select a certificate for the session.  If the name on the
					// certificate matches the name stored in the user preferences, then that certificate is used.
					X509CertificateCollection x509CertificateCollection = Crypt.GetCertificateStore(myStoreName);

					// If there is only one certificate in the personal store, then use it without any prompting. Otherwise, ask the
					// user to select a certificate.
					if (x509CertificateCollection.Count == 1)
						HttpBatchProcessor.clientCertificates.Add(x509CertificateCollection[0]);
					else
					{

						// Only prompt for a certificate if there are more than one to choose from.
						if (x509CertificateCollection.Count != 0)
						{

							// Prompt the user for a certificate.  Note that if they don't select a certificate an exception is throw
							// that will terminate the application if not caught.  Generally, the application should be allowed to exit
							// if a secure connection with the server can't be established.
							FormSelectCertificate formSelectCertificate = new FormSelectCertificate();
							formSelectCertificate.ShowDialog();
							if (formSelectCertificate.DialogResult != DialogResult.OK)
								throw new UserAbortException("User cancelled the operation");

							// Add this certificate to the client's collection.
							HttpBatchProcessor.clientCertificates.Add(formSelectCertificate.SelectedCertificate);

							// There's no need to prompt again until some error forces the user back here.
							HttpBatchProcessor.IsCredentialPrompted = false;

							// The form must be destroyed explicitly or the components will hang around until the garbage is collected.
							formSelectCertificate.Dispose();

						}

					}

				}

			}

			// Create and initialize an HttpWebRequest to handle the conversation between the client and server.  All Batch
			// operations are handled using the 'POST' verb.  The timeout is specified above though the configuration file or a
			// programtic setting.  Also note that the Stream Buffering is disabled as it appears to have a negative impact on
			// throughput.  Finally, the chosen certificate is tacked onto the outgoing request for authentication.
			HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(HttpBatchProcessor.Url);
			httpWebRequest.Method = "POST";
			httpWebRequest.Timeout = HttpBatchProcessor.Timeout;
			httpWebRequest.AllowWriteStreamBuffering = true;
			httpWebRequest.ClientCertificates.AddRange(HttpBatchProcessor.clientCertificates);

			// This request can be used to send a 'Batch' object to the Web Transaction server.
			return httpWebRequest;

		}

		#region IBatchHandler Members

		/// <summary>
		/// Execute the batch on the Web Transaction server and return the results to the caller.
		/// </summary>
		/// <param name="batch">Contains one or more transactions that are executed on the server.  The results of executing the
		/// batch are returned to the caller in this object.</param>
		Result IBatchHandler.Execute(Batch batch)
		{

			Result batchResult = null;

			// These streams must be shut down, even when an exception is taken.  They are declared here so they can be accessed by
			// the 'finally' clause should something go wrong.
			Stream requestStream = null;
			WebResponse webResponse = null;
			Stream responseStream = null;

			try
			{

				// The client communicates the 'batch' data structure to the server using an HTTP channel.  The channel is
				// configured to use SSL and Certificates to insure the privacy of the conversation.
				WebRequest webRequest = CreateCertificateRequest();

				// The 'batch' data structure is serialized and compressed before being sent to the server.
				MemoryStream memoryStreamRequest = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStreamRequest, batch);

				// This section will compress the serial stream.  Note that it must be compressed into an intermediate buffer
				// before being sent via the HttpWebRequest class because the number of bytes must be known before the call is made
				// for the 'ContentLength' parameter of the HttpWebRequest object.
				memoryStreamRequest.Position = 0L;
				requestStream = webRequest.GetRequestStream();
				DeflateStream requestDeflateStream = new DeflateStream(requestStream, CompressionMode.Compress, true);
				requestDeflateStream.Write(memoryStreamRequest.GetBuffer(), 0, (int)memoryStreamRequest.Length);
				requestDeflateStream.Close();
				requestStream.Close();

				// This is where all the work is done.  Send the Request to the HTTP Server and get the response.  In this 
				// case, the response will be the results of executing the remote batch.  Note that we get back only the
				// framework of the RPB, not all the parameters, methods, assemblies and types.  This is an optimization so
				// that the same data isn't sent twice over the network.  This 'differential' data will be merged with the
				// original.  To the user, it appears that the Remote Batch.Method Batch was sent to the server, filled in with
				// result and exception data, then returned to the calling method.  Actually, the batch was sent, the results
				// and exceptions returned, and it was all stitched back together again before the user gets it.
				webResponse = webRequest.GetResponse();

				// Decompress the results from the HttpWebResponse stream.
				responseStream = webResponse.GetResponseStream();
				DeflateStream responseDeflateStream = new DeflateStream(responseStream, CompressionMode.Decompress);
				byte[] uncompressedBuffer = CompressedStreamReader.ReadBytes(responseDeflateStream);
				responseDeflateStream.Close();

				// Deserialize the batch results and merge it back with the original structure.  The 'BatchResult' 
				// structure has the same basic structure as the original batch, but only contains output parameters and
				// exceptions.
				MemoryStream memoryStreamResponse = new MemoryStream(uncompressedBuffer);
				BinaryFormatter binaryFormatterResponse = new BinaryFormatter();
				batchResult = (Result)binaryFormatterResponse.Deserialize(memoryStreamResponse);
				memoryStreamResponse.Close();

			}
			catch (WebException webException)
			{

				// The response in the exception usually indicates that we found the server, but the protocol failed for some
				// reason.  When the response is empty, that usually indicates that the URL is bad or the server is down for
				// some reason.
				if (webException.Response == null)
				{

					// Log the error.
					EventLog.Error("Web Exception {0}, {1}", webException.Status, webException.Message);

				}
				else
				{

					// Extract the information sent back from the server and log it.
					webResponse = (HttpWebResponse)webException.Response;
					EventLog.Error(webException.Message);

					// Present the user with the error code.  There may be a better way to do this, but this will work for now.
					MessageBox.Show(webException.Message, string.Format("{0} - {1}", Application.ProductName, Properties.Resources.ConnectionError));

				}

				// Prompt the user for the URL and credentials again.
				HttpBatchProcessor.IsUrlPrompted = true;
				HttpBatchProcessor.IsCredentialPrompted = true;

				// This gives the caller a unified way to handle all exceptions that may have occurred while processing a batch.  
				// The Web Exceptions are handled the same way as errors from the server.
				throw new BatchException(webException);

			}
			catch (UserAbortException)
			{

				// forward the exception so the caller can decide what to do.
				throw;

			}
			catch (Exception exception)
			{

				// This gives the caller a unified way to handle all exceptions that may have occurred while processing a batch.  
				// The general exceptions are handled the same way as exceptions from the server.
				throw new BatchException(exception);

			}
			finally
			{

				// Make sure that every stream involved with the request is closed when the WebRequest is finished.
				if (requestStream != null)
					requestStream.Close();
				if (responseStream != null)
					responseStream.Close();
				if (webResponse != null)
					webResponse.Close();

			}

			return batchResult;

		}

		#endregion

	}

}
