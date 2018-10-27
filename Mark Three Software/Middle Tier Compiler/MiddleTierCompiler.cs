namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState {None, TargetNamespace, InputFileName, OutputFileName};

	/// <summary>
	/// Creates an executable wrapper around the IDE tool used to generate a middle tier.
	/// </summary>
	class MiddleTierCompiler
	{

		// Private Members
		private static MarkThree.MiddleTier.ArgumentState argumentState;
		private static System.String inputFileName;
		private static System.String outputFileName;
		private static System.String targetNamespace;
		private static System.CodeDom.Compiler.CodeDomProvider codeProvider;
		private static System.CodeDom.Compiler.CodeGeneratorOptions codeGeneratorOptions;

		/// <summary>
		/// Initialize the static members of the Middle Tier Compiler.
		/// </summary>
		static MiddleTierCompiler()
		{

			// Initialize the object
			MiddleTierCompiler.codeProvider = CodeDomProvider.CreateProvider("C#");
			MiddleTierCompiler.codeGeneratorOptions = new CodeGeneratorOptions();
			MiddleTierCompiler.codeGeneratorOptions.BlankLinesBetweenMembers = true;
			MiddleTierCompiler.codeGeneratorOptions.BracingStyle = "C";
			MiddleTierCompiler.codeGeneratorOptions.IndentString = "\t";

		}

		/// <summary>
		/// The Code Provider for the Middle Tier Generator.
		/// </summary>
		public static CodeDomProvider CodeProvider
		{

			get { return MiddleTierCompiler.codeProvider; }
			set { if (value != null) MiddleTierCompiler.codeProvider = value; }

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
				targetNamespace = "DefaultNamespace";

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.InputFileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					if (argument == "-i") {argumentState = ArgumentState.InputFileName; continue;}
					if (argument == "-ns") {argumentState = ArgumentState.TargetNamespace; continue;}
					if (argument == "-out") {argumentState = ArgumentState.OutputFileName; continue;}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{

						case ArgumentState.InputFileName: inputFileName = argument; break;
						case ArgumentState.OutputFileName: outputFileName = argument; break;
						case ArgumentState.TargetNamespace: targetNamespace = argument; break;

					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.InputFileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (inputFileName == null)
					throw new Exception("Usage: DataSetGenerator -i <InputFileName>");

				// If no output file name was specified, create one from the input file specification.
				if (outputFileName == null)
					outputFileName = string.Format("{0}.cs", Path.GetFileNameWithoutExtension(inputFileName));

				// Read the schema into a string.  This emulates the way that the IDE would normally call a code generator.  Create
				// the MiddleTierSchema (like a Schema, but with extra helping functions and relations for this type of code 
				// generation).
				StreamReader streamReader = new StreamReader(inputFileName);
				DataModelSchema schema = new DataModelSchema(streamReader.ReadToEnd());
				streamReader.Close();

				// Transfer the calling parameters into the schema.
				schema.TargetNamespace = targetNamespace;

				// This will generate a buffer of source code from the input schema.
				byte[] buffer = GenerateCode(schema);

				// Write the buffer to the specified UTF8 output file.
				StreamWriter streamWriter = new StreamWriter(outputFileName);
				streamWriter.Write(Encoding.UTF8.GetString(buffer));
				streamWriter.Close();

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
		protected static byte[] GenerateCode(DataModelSchema schema)
		{

			// Create a namespace MarkThree.MiddleTier add it to the module.  This is where all the work is done to generate the
			// CodeDOM specification for the output file.
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(new MiddleTierNamespace(schema));

			// The purpose of this method is to emulate the way the IDE tools would return the data.  This generates the code and 
			// then converts it to an UTF8 array of bytes, which is what the IDE expects to get from any code generation tools.
			StringWriter stringWriter = new StringWriter();
			MiddleTierCompiler.CodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter,
				MiddleTierCompiler.codeGeneratorOptions);
			return System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());

		}

	}

}
