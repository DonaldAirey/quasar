namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, DataSetName, InputFileName, InternalNamespace, OutputFileName, PersistentStore, Reference,
		TargetNamespace, TargetTable};

	/// <summary>
	/// This object will load the property table from a formatted file.
	/// </summary>
	class ExternalInterfaceCompiler
	{

		private static string dataSetName;
		private static string inputFileName;
		private static string internalNamespace;
		private static string outputFileName;
		private static string persistentStore;
		private static string targetTableName;
		private static string targetNamespace;
		private static ArgumentState argumentState;
		private static ArrayList references;
		private static CodeDomProvider codeProvider;
		private static CodeGeneratorOptions codeGeneratorOptions;

		static ExternalInterfaceCompiler()
		{

			// Initialize the object
			ExternalInterfaceCompiler.codeProvider = CodeDomProvider.CreateProvider("C#");
			ExternalInterfaceCompiler.codeGeneratorOptions = new CodeGeneratorOptions();
			codeGeneratorOptions.BlankLinesBetweenMembers = true;
			codeGeneratorOptions.BracingStyle = "C";

		}

		/// <summary>
		/// The Code Provider for the Middle Tier Generator.
		/// </summary>
		public static CodeDomProvider CodeProvider
		{

			get { return ExternalInterfaceCompiler.codeProvider; }

			set { if (value != null) ExternalInterfaceCompiler.codeProvider = value; }

		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			try
			{

				// Defaults
				ExternalInterfaceCompiler.targetNamespace = string.Empty;
				ExternalInterfaceCompiler.internalNamespace = string.Empty;
				ExternalInterfaceCompiler.persistentStore = string.Empty;
				ExternalInterfaceCompiler.dataSetName = string.Empty;
				ExternalInterfaceCompiler.references = new ArrayList();

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.InputFileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					switch (argument)
					{

					case "-ds": argumentState = ArgumentState.DataSetName; continue;
					case "-i": argumentState = ArgumentState.InputFileName; continue;
					case "-is": argumentState = ArgumentState.InternalNamespace; continue;
					case "-ns": argumentState = ArgumentState.TargetNamespace; continue;
					case "-out": argumentState = ArgumentState.OutputFileName; continue;
					case "-ps": argumentState = ArgumentState.PersistentStore; continue;
					case "-ref": argumentState = ArgumentState.Reference; continue;
					case "-t": argumentState = ArgumentState.TargetTable; continue;

					}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

					case ArgumentState.DataSetName: ExternalInterfaceCompiler.dataSetName = argument; break;
					case ArgumentState.InputFileName: ExternalInterfaceCompiler.inputFileName = argument; break;
					case ArgumentState.InternalNamespace: ExternalInterfaceCompiler.internalNamespace = argument; break;
					case ArgumentState.OutputFileName: ExternalInterfaceCompiler.outputFileName = argument; break;
					case ArgumentState.PersistentStore: ExternalInterfaceCompiler.persistentStore = argument; break;
					case ArgumentState.Reference: ExternalInterfaceCompiler.references.Add(argument); break;
					case ArgumentState.TargetNamespace: ExternalInterfaceCompiler.targetNamespace = argument; break;
					case ArgumentState.TargetTable: ExternalInterfaceCompiler.targetTableName = argument; break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.InputFileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (inputFileName == null || targetNamespace == string.Empty)
					throw new Exception("Usage: Generator -i <input file name> -t <target table name>");

				// If the name of the internal namespace MarkThree.MiddleTier to find the commands that use only internal identifiers) was
				// not specified on the command line, then try to infer it from the target namespace.
				if (internalNamespace == string.Empty)
					internalNamespace = targetNamespace.Replace(".External", string.Empty);

				// If no output file name was specified, create one from the input file specification.
				if (outputFileName == null)
					outputFileName = string.Format("{0}.cs", Path.GetFileNameWithoutExtension(inputFileName));

				StreamReader streamReader = new StreamReader(inputFileName);
				string inputContents = streamReader.ReadToEnd();
				ExternalInterfaceSchema externalInterfaceSchema = new ExternalInterfaceSchema(inputContents);
				MiddleTierTable middleTierTable = new MiddleTierTable(externalInterfaceSchema, ExternalInterfaceCompiler.targetTableName);

				externalInterfaceSchema.InternalNamespace = ExternalInterfaceCompiler.internalNamespace;
				externalInterfaceSchema.TargetNamespace = ExternalInterfaceCompiler.targetNamespace;
				externalInterfaceSchema.PersistentStore = ExternalInterfaceCompiler.persistentStore;
				externalInterfaceSchema.DataSetName = ExternalInterfaceCompiler.dataSetName;
				externalInterfaceSchema.References = ExternalInterfaceCompiler.references;

				byte[] buffer = GenerateCode(externalInterfaceSchema, middleTierTable);

				FileStream outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);
				outputStream.Write(buffer, 0, buffer.Length);
				outputStream.Close();

			}
			catch (Exception exception)
			{

				Console.WriteLine(exception.Message);

			}

			return 0;

		}

		/// <summary>
		/// Generate the code from the inputs.
		/// </summary>
		/// <param name="inputFileName">The name of the input file.</param>
		/// <param name="inputFileContent">The contents of the input file.</param>
		/// <returns>A buffer containing the generated code.</returns>
		protected static byte[] GenerateCode(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
		{

			// Create a namespace MarkThree.MiddleTier add it to the module.  This namespace MarkThree.MiddleTier all the relavent code in it.
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(new ExternalInterfaceNamespace(ExternalInterfaceSchema, middleTierTable));

			// This will generate the code from the abstract compile unit and dump it in a memory stream.
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
			ExternalInterfaceCompiler.CodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter,
				ExternalInterfaceCompiler.codeGeneratorOptions);
			streamWriter.Close();

			// Return the source code converted to a stream of bytes.
			return memoryStream.GetBuffer();

		}

	}

}
