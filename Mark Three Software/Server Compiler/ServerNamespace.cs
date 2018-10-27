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
	/// The root namespace of the server transaction classes.
	/// </summary>
	public class ServerNamespace : CodeNamespace
	{

		// Private Fields
		private ServerSchema serverchema;
		private TableSchema tableSchema;

		public ServerNamespace(ServerSchema serverSchema, TableSchema tableSchema)
		{

			// Initialize the object.
			this.serverchema = serverSchema;
			this.tableSchema = tableSchema;

			//namespace MarkThree.UnitTest.Server
			//{
			//	using MarkThree;
			//	using System;
			//	using System.Collections.Generic;
			//	using System.Data;
			//	using System.Data.SqlClient;
			//	using System.Threading;
			this.Name = this.serverchema.TargetNamespace;
			foreach (string reference in serverSchema.References)
				this.Imports.Add(new CodeNamespaceImport(reference));
			this.Imports.Add(new CodeNamespaceImport("MarkThree"));
			this.Imports.Add(new CodeNamespaceImport("System"));
			this.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			this.Imports.Add(new CodeNamespaceImport("System.Data"));
			this.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
			this.Imports.Add(new CodeNamespaceImport("System.Threading"));

			// This is the class that provides a transacted interface into the data model.
			this.Types.Add(new ServerClass(serverSchema, tableSchema));

		}

	}

}
