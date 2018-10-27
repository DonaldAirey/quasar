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
	/// This object will load the property table from a formatted file.
	/// </summary>
	class ServerCompiler
	{

		/// <summary>
		/// These are the parsing states used to read the arguments on the command line.
		/// </summary>
		private enum ArgumentState
		{
			DurableStore,
			InternalNamespace,
			InputFileName,
			Name,
			None,
			OutputFileName,
			Reference,
			TargetNamespace,
			TargetTable,
			VolatileStore
		};

		// Private Fields
		private static MarkThree.MiddleTier.ServerCompiler.ArgumentState argumentState;
		private static System.CodeDom.Compiler.CodeDomProvider codeProvider;
		private static System.CodeDom.Compiler.CodeGeneratorOptions codeGeneratorOptions;
		private static System.Collections.Generic.List<string> references;
		private static System.String durableStoreName;
		private static System.String inputFileName;
		private static System.String name;
		private static System.String outputFileName;
		private static System.String targetNamespace;
		private static System.String targetTableName;
		private static System.String volatileStoreName;

		/// <summary>
		/// Initializes the static elements of the compiler.
		/// </summary>
		static ServerCompiler()
		{

			// Initialize the object
			ServerCompiler.codeProvider = CodeDomProvider.CreateProvider("C#");
			ServerCompiler.codeGeneratorOptions = new CodeGeneratorOptions();
			ServerCompiler.codeGeneratorOptions.BlankLinesBetweenMembers = true;
			ServerCompiler.codeGeneratorOptions.BracingStyle = "C";
			ServerCompiler.codeGeneratorOptions.IndentString = "\t";

		}

		/// <summary>
		/// The Code Provider for the Middle Tier Generator.
		/// </summary>
		public static CodeDomProvider CodeProvider
		{
			get { return ServerCompiler.codeProvider; }
			set { if (value != null) ServerCompiler.codeProvider = value; }
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
				ServerCompiler.name = string.Empty;
				ServerCompiler.volatileStoreName = string.Empty;
				ServerCompiler.durableStoreName = string.Empty;
				ServerCompiler.references = new System.Collections.Generic.List<string>();
				ServerCompiler.targetNamespace = "DefaultNamespace";

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.InputFileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					switch (argument)
					{
					case "-ds":
						argumentState = ArgumentState.DurableStore;
						continue;
					case "-i":
						argumentState = ArgumentState.InputFileName;
						continue;
					case "-n":
						argumentState = ArgumentState.Name;
						continue;
					case "-ns":
						argumentState = ArgumentState.TargetNamespace;
						continue;
					case "-out":
						argumentState = ArgumentState.OutputFileName;
						continue;
					case "-ref":
						argumentState = ArgumentState.Reference;
						continue;
					case "-t":
						argumentState = ArgumentState.TargetTable;
						continue;
					case "-vs":
						argumentState = ArgumentState.VolatileStore;
						continue;
					}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{
					case ArgumentState.DurableStore:
						ServerCompiler.durableStoreName = argument;
						break;
					case ArgumentState.InputFileName:
						ServerCompiler.inputFileName = argument;
						break;
					case ArgumentState.Name:
						ServerCompiler.name = argument;
						break;
					case ArgumentState.OutputFileName:
						ServerCompiler.outputFileName = argument;
						break;
					case ArgumentState.TargetNamespace:
						ServerCompiler.targetNamespace = argument;
						break;
					case ArgumentState.TargetTable:
						ServerCompiler.targetTableName = argument;
						break;
					case ArgumentState.Reference:
						ServerCompiler.references.Add(argument);
						break;
					case ArgumentState.VolatileStore:
						ServerCompiler.volatileStoreName = argument;
						break;
					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.InputFileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (inputFileName == null || outputFileName == null || name == string.Empty ||
					volatileStoreName == string.Empty || durableStoreName == string.Empty)
					throw new Exception("Usage: Generator -i <input file name> -t <target table name>");

				// Read the schema into a string.  This emulates the way that the IDE would normally call a code generator.  Create
				// the MiddleTierSchema (like a Schema, but with extra helping functions and relations for this type of code 
				// generation).
				StreamReader streamReader = new StreamReader(inputFileName);
				ServerSchema serverSchema = new ServerSchema(streamReader.ReadToEnd());
				TableSchema tableSchema = serverSchema.Tables.Find(ServerCompiler.targetTableName);

				// Install the parameters into the schema.
				serverSchema.Name = ServerCompiler.name;
				serverSchema.TargetNamespace = ServerCompiler.targetNamespace;
				serverSchema.References = ServerCompiler.references;
				serverSchema.VolatileStoreName = ServerCompiler.volatileStoreName;
				serverSchema.DurableStoreName = ServerCompiler.durableStoreName;

				// This will generate a buffer of source code from the intput table schema.
				byte[] buffer = GenerateCode(serverSchema, tableSchema);

				// Write the buffer to the specified UTF8 output file.
				StreamWriter streamWriter = new StreamWriter(outputFileName);
				streamWriter.Write(Encoding.UTF8.GetString(buffer));
				streamWriter.Close();

			}
			catch (Exception exception)
			{

				// Write the exceptions to the console.
				Console.WriteLine(exception.Message);

			}

			// This indicates the table was compiled successfully.
			return 0;

		}

		/// <summary>
		/// Generate the code from the inputs.
		/// </summary>
		/// <param name="inputFileName">The name of the input file.</param>
		/// <param name="inputFileContent">The contents of the input file.</param>
		/// <returns>A buffer containing the generated code.</returns>
		protected static byte[] GenerateCode(ServerSchema coreInterfaceSchema, TableSchema tableSchema)
		{

			// Create a namespace MarkThree.MiddleTier add it to the module.  This namespace MarkThree.MiddleTier all the relavent code in it.
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(new ServerNamespace(coreInterfaceSchema, tableSchema));

			// This will generate the source code and return it as an array of bytes.
			StringWriter stringWriter = new StringWriter();
			ServerCompiler.CodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter,
				ServerCompiler.codeGeneratorOptions);
			return System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());

		}

	}

}
