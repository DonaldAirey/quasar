/*************************************************************************************************************************
*
*	File:			CodeGeneratorClass.cs
*	Description:	Creates the class that can be used to interface to tables in the ADO middle tier.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.MiddleTier
{
	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	public class ExternalInterfaceMethod : CodeMemberMethod
	{

		private ExternalInterfaceSchema externalInterfaceSchema;
		private MiddleTierTable middleTierTable;

		public ExternalInterfaceSchema ExternalInterfaceSchema { get { return this.externalInterfaceSchema; } }
		public MiddleTierTable MiddleTierTable { get { return this.middleTierTable; } }

		public ExternalInterfaceMethod(ExternalInterfaceSchema ExternalInterfaceSchema, MiddleTierTable middleTierTable)
		{

			// Initialize the object
			this.externalInterfaceSchema = ExternalInterfaceSchema;
			this.middleTierTable = middleTierTable;
		
		}

	}

}
