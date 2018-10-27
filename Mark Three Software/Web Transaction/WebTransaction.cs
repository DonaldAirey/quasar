namespace MarkThree.Server
{

	using MarkThree;
	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using System.Web;

	/// <summary>
	/// Handles Web Transactions over an HTTP connection.
	/// </summary>
	public class WebTransaction : IHttpHandler
	{

		// Private Members
		private const int bufferSize = 2048;
		private static BinaryFormatter binaryFormatter;
		private static List<ResourceDescription> resourceDescriptionList;

		/// <summary>
		/// Initializes the static members of this class.
		/// </summary>
		static WebTransaction()
		{

			// Log the start of the service.
			EventLog.Information("Web Transaction Service is starting.");

			try
			{

				// The PersistentStoreSection contains information about what databases and other 'persistent' storage is
				// available.  For example, an SQL Database would have a unique short name, used to identify the database, and a
				// connection string.  This data would be used to establish a connection to the store for queries, inserts, updates
				// and deletes.  Each library class that can participate in a transaction to this store will specify the short name
				// of the store in a 'PersistentStore' field.
				WebTransaction.resourceDescriptionList =
					(List<ResourceDescription>)ConfigurationManager.GetSection("resourceSection");

			}
			catch (Exception exception)
			{

				// Any problems initializing should be sent to the Event Log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}

			// The transactions handled by this server can access a combination of one or more volatile or durable data repositories.  This
			// will read the description of the recognized resources from the configuration file.
			foreach (ResourceDescription resourceDescription in WebTransaction.resourceDescriptionList)
			{

				// This will tell the SQL Resource Manager which database connection parameters are associated with a friendly 
				// name for that resource.
				if (resourceDescription is SqlResourceDescription)
				{
					SqlResourceDescription sqlResourceDescription = resourceDescription as SqlResourceDescription;
					SqlResourceManager.AddConnection(sqlResourceDescription.Name, sqlResourceDescription.ConnectionString);
				}
			}

		}

		/// <summary>This enables the reuse of the connections.</summary>
		/// <remarks>The transaction handler is state-less, so there's no requirement for the handler to keep any session level
		/// information around from one request to the next.</remarks>
		public bool IsReusable {get {return true;}}

		/// <summary>
		/// Process the HTTP Request
		/// </summary>
		/// <param name="httpContext">Information about the HTTP Request</param>
		public void ProcessRequest(HttpContext httpContext) 
		{

			// Insure that any uncaught exceptions are logged.
			try
			{

				// Switch to a method to handle each of the verbs.
				switch (httpContext.Request.HttpMethod)
				{
				case "POST" : ProcessPost(httpContext); break;
				}

			}
			catch (Exception exception)
			{

				// Post any high level errors to the event log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

				// Rethrow the exception so the client can handle it.
				throw exception;

			}

		}

		/// <summary>Handle the POST verb.</summary>
		/// <param name="httpContext">Information about the HTTP Request.</param>
		private void ProcessPost(HttpContext httpContext)
		{

			// The data from the client needs to be extracted from the incoming HTTP Stream, uncomrpessed and deserialized.  A 
			// Binary Formatter is needed to serialize the Batch structure when the results are sent back to the client.
			WebTransaction.binaryFormatter = new BinaryFormatter();
			binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			binaryFormatter.FilterLevel = TypeFilterLevel.Low;
			binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;

			// Read the data out of the buffer that belongs to the incoming request.
			HttpRequest httpRequest = httpContext.Request;
			byte[] requestBuffer = httpContext.Request.BinaryRead(httpContext.Request.TotalBytes);
			httpRequest.InputStream.Close();

			// This will uncompress the data from the client.
			DeflateStream deflateStreamRequest = new DeflateStream(new MemoryStream(requestBuffer), CompressionMode.Decompress);
			byte[] uncompressedBuffer = CompressedStreamReader.ReadBytes(deflateStreamRequest);
			deflateStreamRequest.Close();
			
			// Now that the data has been read and decompressed, the original data structure can be reconstituted from the 
			// serialized data.
			Batch batch = (Batch)binaryFormatter.Deserialize(new MemoryStream(uncompressedBuffer));

			// Execute the transactions contained in the batch.  Each transaction is committed or rejected as a unit.  The batch
			// contains the 'plan' of the transaction.  The task of this service is to execute the plan.
			foreach (TransactionPlan transactionPlan in batch.Transactions)
			{

				// The 'Transaction' is used to commit or reject the methods contained in the 'TransactionPlan' as a unit.  If an
				// exception is taken during the execution of any of the methods contained in the 'TransactionPlan', then the
				// transaction will be rejected and all changes will be rolled back to the beginning of this transaction.  If there
				// are no exceptions taken, then all the changes are committed to their respective data stores.
				using (Transaction transaction = new Transaction())
				{

					// Every transaction has one or more data repositories that can be read or modified.  This will create a Resource
					// Manager for every one of the data stores that can be referenced by this server during the processing of a
					// transaction.  A transaction can include one or more of these databases and the internal framework will escelate
					// from a single phase transaction to a two-phase transaction when there is more than one durable resource.
					foreach (ResourceDescription resourceDescription in WebTransaction.resourceDescriptionList)
					{

						// This will create a resource manager for an ADO data resource based on the configuration settings.
						if (resourceDescription is AdoResourceDescription)
							transaction.Add(new AdoResourceManager(resourceDescription.Name));

						// This will create a resource manager for SQL databases based on the configuration settings.
						if (resourceDescription is SqlResourceDescription)
							transaction.Add(new SqlResourceManager(resourceDescription.Name));

					}

					// This insures that any exceptions in the batch are reported back to the client as part of the return batch.
					try
					{

						// The batch specifies one or more methods that make up a transaction.  This part of the transaction handler
						// will attempt to collect the ADO locks for each method.  The locks are consolidated as they are collected and
						// alphabetized.  The consolidation prevents redudant locks from being called, and the alphabetization of the
						// table locks insures that the locks are always obtained in the same order.  Locking in the same order is
						// critical for preventing deadlocking.
						foreach (MethodPlan methodPlan in transactionPlan.Methods)
						{

							// Any exception that occurs while processing the method will be associted with the 'MethodPlan' and 
							// returned to the caller in the return batch.
							try
							{

								// Load the assembly from the specification in the 'MethodData' object.  Note that if the assembly is
								// already in memory, this operation returns quickly with a reference to the existing assembly.
								Assembly assembly = Assembly.Load(methodPlan.TypePlan.AssemblyPlan.Name);

								// Find and store the class (Type) of which this method is a member.  Obviously, if the type doesn't
								// exist, this transaction won't be able to complete successfully.
								Type type = assembly.GetType(methodPlan.TypePlan.Name);
								if (type == null)
									throw new Exception(String.Format("Unable to create an object of type {0}",
										methodPlan.TypePlan.Name));

								// The batch contains the plan for executing the method.  In order to pipeline the execution of all the
								// methods in the transaction, the resource locks required for the method are collected first.  Once
								// the collection is complete, the locks are acquired, then all the methods are executed.  The first
								// part of this transaction processing involves collecting all the locks.  The method plan contains the
								// name of the method to be executed.  Every method that wants to participate in an ADO transaction
								// needs to provide a method that can accept a 'LockRequestList'.  This list is populated with the
								// table locks required for the execution of the method.
								MethodInfo methodInfo = type.GetMethod(methodPlan.Name,
									new Type[] { typeof(ParameterList) });
								if (methodInfo == null)
									throw new Exception(String.Format("The method {0}.{1} can't be located in assembly {2}.",
										methodPlan.TypePlan.Name, methodPlan.Name, methodPlan.TypePlan.AssemblyPlan.Name));

								// ADO Transactions require tables to be locked before the method is executed.  If a method needs these
								// table locks, it will provide a 'prequel' method with the same name, but an 'AdoTransaction'
								// parameter.  If a method is declared with this calling convension, then execute it to get the table
								// locks.
								if (methodInfo != null)
								{

									// If any of the parameters is a reference type, that is, it references another parameter, then 
									// resolve the reference before calling the method.
									foreach (Parameter parameter in methodPlan.Parameters)
										if (parameter.Value is Parameter)
											parameter.Value = ((Parameter)parameter.Value).Value;

									// Call the method to retrieve the table locks.  These locks will be consolodated into a single
									// lock when dulicate table locks are requested.  They are also alphabetized to insure the locks
									// are always called in the same order.
									TransactionCallback transactionCallback = Delegate.CreateDelegate(typeof(TransactionCallback),
										methodInfo) as TransactionCallback;
									transaction.Add(new TransactionElement(transactionCallback, methodPlan.Parameters));

								}

							}
							catch (Exception exception)
							{

								// Any exceptions taken while trying to assembly the execution plan for the transaction will be logged
								// agains the method plan where the problem occurred.
								methodPlan.Exceptions.Add(exception is System.Reflection.TargetInvocationException ?
									exception.InnerException : exception);

								throw new Exception("Fatal error during batch processing.  Couldn't obtain locks for the resources.");

							}

						}

						transaction.AcquireLocks();

						// Each method that is executed in the batch has the potential to cause an exception.  However, a single
						// exception will not stop the batch.  The batch will run to completion attempting to find other methods that
						// may cause trouble as each method has an individual return code.  If any of the methods caused an exception,
						// the entire batch is rolled back.  Conversely, if everything executes without any trouble the entire
						// transaction is committed as a unit.  This basically keeps track of any error since the exceptions are caught
						// for each method executed.
						bool hasExceptions = false;

						int methodIndex = 0;

						// Once all the locks are in place, execute each of the methods prepared in the above code and register any 
						// exceptions with the MethodPlan that originated the operation.  When the batch is returned to the caller, the
						// exceptions will correlate with the MethodPlan that caused the trouble.
						foreach (TransactionElement transactionItem in transaction.TransactionElements)
						{

							try
							{

								// This will insure that any exceptions are passed back to the caller and associated with the 
								// 'MethodPlan' that originated the error.
								transactionItem.TransactionCallback(transactionItem.Parameters);

							}
							catch (Exception exception)
							{

								// This indicates that there was some problem with the transaction and the results should be rejected, 
								// but only after the remaining items in the transaction have been attempted.  This maximizes the
								// information that the caller has so the next time the batch is sent, all the errors can be fixed.  If
								// the transaction were to stop after the first exception was discovered, there could be a sequence of
								// many back-and-forth attempts to run the batch before all the errors were discovered and corrected.
								hasExceptions = true;

								// This mechanism will pass the exception back to the caller and correlate it with the MethodPlan that
								// caused the problem.
								transactionPlan.Methods[methodIndex].Exceptions.Add(
									exception is System.Reflection.TargetInvocationException ? exception.InnerException : exception);

							}

							// This will keep the MethodPlan items in the batch synchronized with the TransactionItems as the the
							// transaction items are invoked.  The index is used to pass exceptions back to the calling process in a
							// way where it matches up with the method that caused the exception.
							methodIndex++;

						}

						// If any exceptions were taken while processing the batch, reject the changes to the data model.  Otherwise,
						// the results can be committed as a unit.
						if (hasExceptions)
							transaction.Rollback();
						else
							transaction.Commit();

					}
					catch (Exception exception)
					{

						// This mechanism will pass back the exceptions to the client as part of the 'TransactionPlan' structure.
						transactionPlan.Exceptions.Add(exception);

					}

				}

			}

			// Pack up the output parameters and the exceptions into a special purpose 'BatchResult' structure and pass them back
			// to the client.  The client will integrate the return parameters and the exceptions with the original batch.  To the
			// user of the Web Transaction, it will look like the batch went to the server and returned as a single structure.
			// Returning just the changes is an optimization designed to reduce network traffic.
			MemoryStream memoryStreamResponse = new MemoryStream();
			binaryFormatter.Serialize(memoryStreamResponse, new BatchResult(batch));

			// This will compress the binary stream and write it to the response output stream.  Compressing the data is designed
			// to take up more CPU time, but provide better performance over the networks.
			Stream responseStream = httpContext.Response.OutputStream;
			DeflateStream responseDeflateStream = new DeflateStream(responseStream, CompressionMode.Compress, true);
			responseDeflateStream.Write(memoryStreamResponse.GetBuffer(), 0, (int)memoryStreamResponse.Length);
			responseDeflateStream.Close();

		}

	}

}
