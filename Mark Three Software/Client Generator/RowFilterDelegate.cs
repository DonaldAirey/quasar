/*************************************************************************************************************************
*
*	File:			RowFilterDelegate.cs
*	Description:	Generates the Relation Class which adds functionality to the DataRelation class for multithreaded safety
*					persistence and other features for three-tier operation.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// A Class that implements a table.
	/// </summary>
	class RowFilterDelegate : CodeTypeDelegate
	{

		/// <summary>
		/// Implements a DataTable.
		/// </summary>
		public RowFilterDelegate()
		{

			this.Name = "RowFilterDelegate";
			this.ReturnType = new CodeTypeReference(typeof(bool));
			this.TypeAttributes = System.Reflection.TypeAttributes.Public;
			this.Parameters.AddRange(new CodeParameterDeclarationExpression[] {new CodeParameterDeclarationExpression(typeof(System.Data.DataRow), "userDataRow"), new CodeParameterDeclarationExpression(typeof(System.Data.DataRow), "dataRow")});

		}
	
	}

}
