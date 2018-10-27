namespace MarkThree
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Used to compile source modules using the CodeDOM.
	/// </summary>
	public struct MetaAssembly
	{

		public bool TreatWarningsAsErrors;
		public bool IncludeDebugInformation;
		public bool GenerateExecutable;
		public bool GenerateInMemory;
		public int WarningLevel;
		public string OutputAssembly;
		public string OutputType;
		public string CompilerOptions;
		public string[] Dependancies;
		public string[] SourceCode;

	}

}
