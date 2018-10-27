namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Data;
	using System.Reflection;
	using System.Threading;

	/// <summary>
	/// Executes a batch of commands as a series of transactions that are committed or rejected as a unit.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class BatchProcessor : MarshalByRefObject, IBatchHandler
	{

		// Private Static Members
		private static List<ResourceDescription> resourceDescriptionList;

		/// <summary>
		/// Executes a batch of commands as a series of transactions that are committed or rejected as a unit.
		/// </summary>
		static BatchProcessor()
		{

			try
			{

				// The 'resourceSection' section of the configuration file contains about the resource managers available for the
				// transaction processor. Volatile Resource Managers provide an API and housekeeping data for managing the memory
				// based data while the Durable Resource Managers govern the handling of the data that resides on a disk
				// somewhere.  This section of the configuration file describes the resource managers available and their
				// properties.
				BatchProcessor.resourceDescriptionList = (List<ResourceDescription>)ConfigurationManager.GetSection("resourceSection");

				// This will read through each of the descriptions of the resource managers found in the configuratino file and
				// initialize the static properties.
				foreach (ResourceDescription resourceDescription in BatchProcessor.resourceDescriptionList)
					if (resourceDescription is SqlResourceDescription)
					{
						SqlResourceDescription sqlResourceDescription = resourceDescription as SqlResourceDescription;
						SqlResourceManager.AddConnection(sqlResourceDescription.Name, sqlResourceDescription.ConnectionString);
					}

				// This will read through each of the descriptions of the resource managers found in the configuratino file and
				// initialize the static properties.
				foreach (ResourceDescription resourceDescription in BatchProcessor.resourceDescriptionList)
					if (resourceDescription is AdoResourceDescription)
					{
						AdoResourceDescription adoResourceDescription = resourceDescription as AdoResourceDescription;
						MethodInfo methodInfo = adoResourceDescription.Type.GetMethod("GetDataSet");
						AdoResourceManager.AddDataSet(adoResourceDescription.Name, methodInfo.Invoke(null, null) as DataSet);
					}

			}
			catch (Exception exception)
			{

				// Any problems initializing should be sent to the Event Log.
				EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

			}

		}

		#region IBatchHandler Members

		/// <summary>
		/// Execute the batch.
		/// </summary>
		/// <param name="batch"></param>
		Result IBatchHandler.Execute(Batch batch)
		{

			// Execute the transactions contained in the batch.  Each transaction is committed or rejected as a unit.  The commands
			// within the batch are specified as text strings and object arrays which are be resolved through the 'Reflection'
			// classes to assemblies, types, methods and parameters.  A wrapper around the .NET Framework Transaction class is used
			// to commit or reject these collections of commands as a unit.
			foreach (Batch.Transaction batchTransaction in batch.Transactions)
			{

				// The 'Transaction' is used to commit or reject the pseudo-methods contained in the 'Batch.Transaction. as a
				// unit.  The transaction will be rejected and all changes will be rejected if an exception is taken during the 
				// execution of any of the methods contained in the 'Batch.Transaction'. If there are no exceptions taken, then all
				// the changes are committed to their respective data stores.
				Transaction transaction = new Transaction();

				// Every transaction has one or more data repositories that can be read or modified.  This step will create a
				// Resource Manager for every one of the data stores that can be referenced by this server during the processing of
				// a transaction.  A transaction can include one or more of these databases and the internal framework will
				// escelate from a single phase transaction to a two-phase transaction when there is more than one durable 
				// resource.
				foreach (ResourceDescription resourceDescription in BatchProcessor.resourceDescriptionList)
				{

					// This will create a resource manager for an ADO data resource based on the configuration settings.
					if (resourceDescription is AdoResourceDescription)
						transaction.Add(new AdoResourceManager(resourceDescription.Name));

					// This will create a resource manager for SQL databases based on the configuration settings.
					if (resourceDescription is SqlResourceDescription)
						transaction.Add(new SqlResourceManager(resourceDescription.Name));

				}

				try
				{

					// This variable controls whether the transaction is committed or rejected.
					bool hasExceptions = false;

					// This step will use reflection to resolve the assemblies, types and method names into objects that are 
					// loaded in memory. Once the objects are resolved, the list of parameters is passed to the method for
					// execution.  The beautiful part about this architecture is that the target methods themselves are written and
					// supported as strongly typed interfaces.  The 'Reflection' class takes care of matching the variable array of
					// objects to the strongly typed, individual parameters.
					foreach (Batch.Method batchMethod in batchTransaction.Methods)
					{

						try
						{

							// The 'Reflection' classes are used to resolve the method specified in the batch to an object loaded
							// in memory that can be used to invoke that method.
							Assembly assembly = Assembly.Load(batchMethod.Type.Assembly.DisplayName);
							Type type = assembly.GetType(batchMethod.Type.Name);
							if (type == null)
								throw new Exception(String.Format("Unable to locate type \"{0}\" in assembly {1}",
									batchMethod.Type.Name, batchMethod.Type.Assembly.DisplayName));
							MethodInfo methodInfo = type.GetMethod(batchMethod.Name);
							if (methodInfo == null)
								throw new Exception(String.Format("The method \"{0}.{1}\" can't be located in assembly {2}.",
									batchMethod.Type.Name, batchMethod.Name, batchMethod.Type.Assembly.DisplayName));

							// This will invoke the method specified in the batch and match each of the variable, generic 
							// parameters to strongly typed parameters.
							object result = methodInfo.Invoke(null, batchMethod.Parameters);

							// Like WebServices, this will return the parameters to the client as a variable, generic array.  It is
							// up to the client to sort out the order of the returned parameters.
							List<Object> resultList = new List<object>();
							if (methodInfo.ReturnType != typeof(void))
								resultList.Add(result);
							foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
								if (parameterInfo.ParameterType.IsByRef)
									resultList.Add(batchMethod.Parameters[parameterInfo.Position]);
							batchMethod.Results = resultList.ToArray();

						}
						catch (Exception exception)
						{

							// This indicates that there was some problem with the transaction and the results should be rejected,
							// but only after the remaining items in the transaction have been attempted.  This maximizes the
							// information that the caller has so the next time the batch is sent, all the errors can be fixed.  If
							// the transaction were to stop after the first exception was discovered, there could be a sequence of
							// many back-and-forth attempts to run the batch before all the errors were discovered and corrected.
							hasExceptions = true;

							// This mechanism will pass the actual exception in the method back to the caller and correlate it with
							// the Batch.Method that caused the problem.  Passing the 'TargetInvocationException' back to the
							// client is of no practical value, so the inner exception is extracted instead and passsed back to the
							// client.  When the client processes the BatchException, it will appear as if the exception that
							// happend here on the server actually happeded local to the client machine.  This is the desired
							// action.
							batchMethod.Exceptions.Add(exception is System.Reflection.TargetInvocationException ?
								exception.InnerException : exception);

						}

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

					// This will catch any errors that occurred while using 'Reflection' to execute the batch.
					batchTransaction.Exceptions.Add(exception);

				}
				finally
				{

					// Release the locks and any database connection resources.
					transaction.Dispose();

				}

			}

			// Only the return parameters and exceptions are returned to the client.  There is no need to transmit the entire batch
			// across the process boundary.  The client will integrate the return parameters and exceptions back into the original
			// batch making it appear as if the bach made a round trip.
			return new Result(batch);

		}

		#endregion

	}

}
