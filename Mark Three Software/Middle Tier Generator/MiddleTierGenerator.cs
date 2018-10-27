namespace MarkThree.MiddleTier
{

	using Microsoft.VisualStudio.Shell.Interop;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.OLE.Interop;
	using System;
	using System.CodeDom.Compiler;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Xml;

	[Guid("AAD59B48-FFEC-438d-902B-E5950F3EADBD")]
	public class MiddleTierGenerator : Microsoft.VisualStudio.Shell.Interop.IVsSingleFileGenerator,
		Microsoft.VisualStudio.OLE.Interop.IObjectWithSite
	{

		// Private Members
		private string codeFileNameSpace;
		private string codeFilePath;
		private object site;
		private CodeDomProvider codeProvider;
		private CodeGeneratorOptions codeGeneratorOptions;
		private IVsGeneratorProgress ivsGeneratorProgress;

		/// <summary>
		/// Used to convert the abstract DOM into source code.
		/// </summary>
		public CodeDomProvider CodeDomProvider
		{

			get
			{

				// Since the COM invocation of the generators doesn't provide for a constructor, the member values will be
				// constructed "Just In Time" to be used.
				if (this.codeProvider == null)
					this.codeProvider = CodeDomProvider.CreateProvider("C#");
				
				// This is the code provider for this class.
				return this.codeProvider;

			}

		}

		/// <summary>
		/// The options for generating source code.
		/// </summary>
		public CodeGeneratorOptions CodeGeneratorOptions
		{

			get
			{

				// Since the COM invocation of the generators doesn't provide for a constructor, the member variables will be
				// constructed "Just In Time" to be used.
				if (this.codeGeneratorOptions == null)
				{
					this.codeGeneratorOptions = new CodeGeneratorOptions();
					this.codeGeneratorOptions.BlankLinesBetweenMembers = false;
					this.codeGeneratorOptions.BracingStyle = "C";
					this.codeGeneratorOptions.IndentString = "\t";
					this.codeGeneratorOptions.VerbatimOrder = true;
				}

				// Here can be found the bones of Pirates (Arrrgggg)!
				return this.codeGeneratorOptions;

			}

		}

		#region IVsSingleFileGenerator Members

		/// <summary>
		/// Gets the default extension for the selected code generator.
		/// </summary>
		/// <param name="extension">The suffix to be appended to the generated file.</param>
		/// <returns>0</returns>
		public int DefaultExtension(out string extension)
		{

			// This insures that the string will not be empty.
			extension = string.Empty;

			// This insures that the period is part of the default extension.
			string defaultExtension = this.CodeDomProvider.FileExtension;
			if (defaultExtension != null && defaultExtension.Length > 0 && defaultExtension[0] != '.')
				defaultExtension = "." + defaultExtension;

			// If the extension isn't empty, add the '.Designer' to the suffix so the visual studio will handle the file correctly
			// once it has been generated.
			if (!string.IsNullOrEmpty(defaultExtension))
				extension = ".Designer" + defaultExtension;

			// This indicates (I believe) that the extension was handled.
			return 0;

		}

		/// <summary>
		/// Generate the code from the custom tool.
		/// </summary>
		/// <param name="wszInputFilePath">The name of the input file.</param>
		/// <param name="bstrInputFileContents">The contents of the input file.</param>
		/// <param name="wszDefaultNamespace">The namespace for the generated code.</param>
		/// <param name="pbstrOutputFileContents">The generated code.</param>
		/// <param name="pbstrOutputFileContentSize">The buffer size of the generated code.</param>
		/// <param name="pGenerateProgress">An indication of the tools progress.</param>
		/// <returns>0 indicates the tool handled the command.</returns>
		public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
			IntPtr[] pbstrOutputFileContents, out uint pbstrOutputFileContentSize, IVsGeneratorProgress pGenerateProgress)
		{

			// Throw an execption if there is nothing to process.
			if (bstrInputFileContents == null)
				throw new ArgumentNullException(bstrInputFileContents);

			// This will make the parameters used to invoke this custom tool available to the class that generates the code.
			this.codeFilePath = wszInputFilePath;
			this.codeFileNameSpace = wszDefaultNamespace;
			this.ivsGeneratorProgress = pGenerateProgress;

			DataModelSchema schema = new DataModelSchema(bstrInputFileContents);
			schema.TargetNamespace = wszDefaultNamespace;

			// Call the super class to generate the code.
			byte[] generatedBuffer = GenerateCode(schema);
			if (generatedBuffer == null)
			{

				// This will pack up an empty buffer in a format that can be moved across the managed code boundary.
				pbstrOutputFileContents[0] = IntPtr.Zero;
				pbstrOutputFileContentSize = 0;

			}
			else
			{

				// Pack the data back into a form that can be moved across the managed code boundary.
				pbstrOutputFileContents[0] = Marshal.AllocCoTaskMem(generatedBuffer.Length);
				Marshal.Copy(generatedBuffer, 0, pbstrOutputFileContents[0], generatedBuffer.Length);
				pbstrOutputFileContentSize = (uint)generatedBuffer.Length;

			}

			// This indicates that the 'Generate' method was handled.
			return 0;

		}
		#endregion

		#region IObjectWithSite Members

		public void GetSite(ref Guid riid, out IntPtr ppvSite)
		{

			if (this.site == null)
				throw new Win32Exception(-2147467259);

			IntPtr objectPointer = Marshal.GetIUnknownForObject(this.site);

			try
			{
				Marshal.QueryInterface(objectPointer, ref riid, out ppvSite);
				if (ppvSite == IntPtr.Zero)
				{
					throw new Win32Exception(-2147467262);
				}
			}
			finally
			{
				if (objectPointer != IntPtr.Zero)
				{
					Marshal.Release(objectPointer);
					objectPointer = IntPtr.Zero;
				}
			}
		}

		public void SetSite(object pUnkSite)
		{
			this.site = pUnkSite;
			this.codeProvider = null;
		}

		#endregion

		/// <summary>
		/// Generate the code from the inputs.
		/// </summary>
		/// <param name="inputFileName">The name of the input file.</param>
		/// <param name="inputFileContent">The contents of the input file.</param>
		/// <returns>A buffer containing the generated code.</returns>
		protected byte[] GenerateCode(DataModelSchema schema)
		{

			// Create a namespace MarkThree.MiddleTier add it to the module.  This namespace MarkThree.MiddleTier all the relavent code in it.
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			codeCompileUnit.Namespaces.Add(new MiddleTierNamespace(schema));

			// If a handler was provided for the generation of the code, then call it with an update.
			if (this.ivsGeneratorProgress != null)
				this.ivsGeneratorProgress.Progress(75, 100);

			// This will generate the source code and return it as an array of bytes.
			StringWriter stringWriter = new StringWriter();
			this.CodeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter,
				this.CodeGeneratorOptions);

			// If a handler was provided for the progress, then let it know that the task is complete.
			if (this.ivsGeneratorProgress != null)
				this.ThrowOnFailure(this.ivsGeneratorProgress.Progress(100, 100));

			// Return the source code converted to a stream of bytes.
			return System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());

		}

		/// <summary>
		/// Throws an exception across the managed code boundary.
		/// </summary>
		/// <param name="hr">The Windows System Error Code.</param>
		private void ThrowOnFailure(int hr)
		{

			// If the result handle shows an error, throw the exception across the managed code boundary.
			if ((hr < 0))
				Marshal.ThrowExceptionForHR(hr);

		}

	}

}
