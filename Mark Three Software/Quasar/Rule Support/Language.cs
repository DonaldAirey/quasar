/*************************************************************************************************************************
*
*	File:			Environment.cs
*	Description:	Classes 
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;
	using Microsoft.CSharp;

	/// <summary>
	/// Summary description for Execution.
	/// </summary>
	public class Language
	{

		/// <summary>
		/// Creates a structure of standard parameters for a compilation of a rule.
		/// </summary>
		/// <returns>The standard options for the comiler.</returns>
		public static CompilerParameters StandardParameters()
		{
		
			// Create an initialize the compiler options (parameters)
			System.CodeDom.Compiler.CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.TreatWarningsAsErrors = true;
			compilerParameters.GenerateInMemory = false;
			compilerParameters.WarningLevel = 4;
			compilerParameters.IncludeDebugInformation = true;

			// These assemblies are found in the GUC.
			compilerParameters.ReferencedAssemblies.Add(@"System.Windows.Forms.dll");

			// Pick up the support libraries from the executable director.
			string assemblyFullName = Assembly.GetExecutingAssembly().Location;
			compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

			// These parameters can be passed back into the compiler.
			return compilerParameters;

		}
	
		/// <summary>
		/// Comiles the source code into rules.
		/// </summary>
		/// <param name="compilerParameters">Parameters for the compilation.</param>
		/// <param name="sourceCode">Source code of the rule.</param>
		/// <returns>Results of the compilation.</returns>
		public static CompilerResults Compile(CompilerParameters compilerParameters, string sourceCode)
		{

			// Get an instance of the C Sharp compiler and assemble the source code using the parameters
			// passed in.  The results are neatly packaged in a structure that is returned to the caller.
			ICodeCompiler codeCompiler = new CSharpCodeProvider().CreateCompiler();
			return codeCompiler.CompileAssemblyFromSource(compilerParameters, sourceCode);

		}

		/// <summary>
		/// Executes all the rules in an assembly until they've all inciated that they've triggered.
		/// </summary>
		/// <param name="argument">Arguments for the command.</param>
		public static void ExecuteOnce(CompilerResults compilerResults)
		{

			// The top level window of the application is needed as the owner of the error message dialog box that will pop up to
			// process the errors.
			System.Windows.Forms.Control activeForm = Form.ActiveForm;
			System.Windows.Forms.Control topLevelControl = activeForm == null ? null : activeForm.TopLevelControl;

			// The assembly is the binary version of the rule that we compiled.
			Assembly assembly = compilerResults.CompiledAssembly;

			// Make a list of every method in the asssembly which is not inherited.  This is the way we track
			// which methods have executed to completion.  When a method returns a 'true' value, it is
			// removed from the list and is not run again.  When all the methods have triggered, the list
			// will be emtpy and the thread will exit.
			ArrayList typeList = new ArrayList();
			foreach (Type type in assembly.GetTypes())
			{

				TypeMethod typeMethod = new TypeMethod(assembly.CreateInstance(type.FullName));
				typeList.Add(typeMethod);

				foreach (MethodInfo methodInfo in type.GetMethods())
					if (methodInfo.DeclaringType == type && methodInfo.IsPublic && !methodInfo.IsStatic)
						typeMethod.MethodList.Add(methodInfo);

			}

			// Run through the list of rules that still need to be checked.  As a rule
			// triggers, its removed from the list.  When the list is empty, the thread
			// will exit.
			foreach (TypeMethod typeMethod in typeList)
			{

				foreach (MethodInfo methodInfo in typeMethod.MethodList)
				{

					try
					{

						methodInfo.Invoke(typeMethod.Instance, null);

					}
					catch (Exception exception)
					{
						if (exception.InnerException.GetType() == typeof(BatchException))
						{
							BatchException batchException = (BatchException)exception.InnerException;
							// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
							foreach (RemoteException remoteException in batchException.Exceptions)
								if (MessageBox.Show(topLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
									MessageBoxIcon.Error) == DialogResult.Cancel)
									break;
						}
						else
						{

							MessageBox.Show(exception.InnerException.Message, "Exception",
								System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
						}

					}

				}

			}

		}

		/// <summary>
		/// Executes all the rules in an assembly until they've all inciated that they've triggered.
		/// </summary>
		/// <param name="argument">Arguments for the command.</param>
		public static void ExecutionCommand(params object[] argument)
		{

			// The top level window of the application is needed as the owner of the error message dialog box that will pop up to
			// process the errors.
			System.Windows.Forms.Control activeForm = Form.ActiveForm;
			System.Windows.Forms.Control topLevelControl = activeForm == null ? null : activeForm.TopLevelControl;

			// The first argument in the variable list is the compiler results.  The assembly is the binary
			// version of the rule that we compiled.
			CompilerResults compilerResults = (CompilerResults)argument[0];
			Assembly assembly = compilerResults.CompiledAssembly;

			// Make a list of every method in the asssembly which is not inherited.  This is the way we track
			// which methods have executed to completion.  When a method returns a 'true' value, it is
			// removed from the list and is not run again.  When all the methods have triggered, the list
			// will be emtpy and the thread will exit.
			ArrayList typeList = new ArrayList();
			foreach (Type type in assembly.GetTypes())
			{

				TypeMethod typeMethod = new TypeMethod(assembly.CreateInstance(type.FullName));
				typeList.Add(typeMethod);

				foreach (MethodInfo methodInfo in type.GetMethods())
					if (methodInfo.DeclaringType == type && methodInfo.IsPublic && !methodInfo.IsStatic)
						typeMethod.MethodList.Add(methodInfo);

			}

			// Keep on calling the methods that haven't triggered until the list is empty.
			while (typeList.Count > 0)
			{

				// This list is used to remove methods from the list of rules.  When the
				// trigger returns 'true', it's removed from the list.
				ArrayList typeDeletionList = new ArrayList();

				// Run through the list of rules that still need to be checked.  As a rule
				// triggers, its removed from the list.  When the list is empty, the thread
				// will exit.
				foreach (TypeMethod typeMethod in typeList)
				{

					ArrayList methodDeletionList = new ArrayList();
					
					foreach (MethodInfo methodInfo in typeMethod.MethodList)
					{

						bool success = true;

						try
						{

							success = (bool)methodInfo.Invoke(typeMethod.Instance, null);

						}
						catch (Exception exception)
						{
							if (exception.InnerException.GetType() == typeof(BatchException))
							{
								BatchException batchException = (BatchException)exception.InnerException;
								// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
								foreach (RemoteException remoteException in batchException.Exceptions)
									if (MessageBox.Show(topLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
										MessageBoxIcon.Error) == DialogResult.Cancel)
										break;
							}
							else
							{

								MessageBox.Show(exception.InnerException.Message, "Exception",
									System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
							}

							success = true;

						}

						if (success)
							methodDeletionList.Add(methodInfo);

					}

					// Delete the methods that have triggered.  The will not be run again.
					foreach (MethodInfo methodInfo in methodDeletionList)
						typeMethod.MethodList.Remove(methodInfo);

					if (typeMethod.MethodList.Count == 0)
						typeDeletionList.Add(typeMethod);

				}

				foreach (TypeMethod typeMethod in typeDeletionList)
					typeList.Remove(typeMethod);

				// Sleep for a respectable amount of time and run the rules again.
				Thread.Sleep(1000);

			}

		}

	}
	
}
