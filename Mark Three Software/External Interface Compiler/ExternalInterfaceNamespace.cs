/*************************************************************************************************************************
*
*	File:			CodeGeneratorCompileUnit.cs
*	Description:	Creates the root namespace MarkThree.MiddleTier the Interface Source Code File.
*					suitable for a high volume of concurrent users and distribution.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Xml.Schema;

	/// <summary>
	/// The root namespace MarkThree.MiddleTier the Interface output file.
	/// </summary>
	public class ExternalInterfaceNamespace : CodeNamespace
	{

		private ExternalInterfaceSchema externalInterfaceSchema;
		private MiddleTierTable middleTierTable;

		public ExternalInterfaceNamespace(ExternalInterfaceSchema externalInterfaceSchema, MiddleTierTable middleTierTable)
		{

			// Initialize the object.
			this.externalInterfaceSchema = externalInterfaceSchema;
			this.middleTierTable = middleTierTable;

			// Name space and import declarations
			// namespace MarkThree.MiddleTier {
			//	  using System;
			//    using System.Data;
			//    using System.Xml;
			//    using System.Runtime.Serialization;
			this.Name = externalInterfaceSchema.TargetNamespace;
			foreach (string reference in externalInterfaceSchema.References)
				this.Imports.Add(new CodeNamespaceImport(reference));
			this.Imports.Add(new CodeNamespaceImport("MarkThree"));
			this.Imports.Add(new CodeNamespaceImport("System"));
			this.Imports.Add(new CodeNamespaceImport("System.Collections"));
			this.Imports.Add(new CodeNamespaceImport("System.Data"));
			this.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));

			this.Types.Add(new ExternalInterfaceClass(externalInterfaceSchema, middleTierTable));

		}

	}

}
