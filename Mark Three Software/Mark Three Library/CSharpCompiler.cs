namespace MarkThree
{

	using Microsoft.CSharp;
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.Configuration;
	using System.ComponentModel;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.Reflection;
	using System.Text;

	public class CSharpCompiler
	{

		static public object Compile(MetaAssembly metaAssembly, params object[] parameters)
		{

			object instance = null;

			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();

			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.OutputAssembly = metaAssembly.OutputAssembly;
			compilerParameters.TreatWarningsAsErrors = metaAssembly.TreatWarningsAsErrors;
			compilerParameters.WarningLevel = metaAssembly.WarningLevel;
			compilerParameters.GenerateExecutable = metaAssembly.GenerateExecutable;
			compilerParameters.CompilerOptions = metaAssembly.CompilerOptions;
			compilerParameters.GenerateInMemory = metaAssembly.GenerateInMemory;
			compilerParameters.IncludeDebugInformation = metaAssembly.IncludeDebugInformation;
			if (metaAssembly.Dependancies != null && metaAssembly.Dependancies.Length != 0)
				compilerParameters.ReferencedAssemblies.AddRange(metaAssembly.Dependancies);

			// Invoke compilation.
			CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, metaAssembly.SourceCode);
			if (compilerResults.Errors.Count > 0)
				foreach (CompilerError compilerError in compilerResults.Errors)
					EventLog.Error("{0}: {1}", compilerParameters.OutputAssembly, compilerError.ToString());
			else
				instance = compilerResults.CompiledAssembly.CreateInstance(metaAssembly.OutputType, false, BindingFlags.CreateInstance,
					null, parameters, CultureInfo.InvariantCulture, null);

			if (compilerResults.Errors.Count > 0)
				throw new Exception(string.Format("Module {0} had {1} compilation errors", compilerParameters.OutputAssembly,
					compilerResults.Errors.Count));

			return instance;

		}

	}

}
