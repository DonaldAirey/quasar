/*************************************************************************************************************************
*
*	File:			LocalFieldException.cs
*	Description:	Exception data for a local field in a viewer.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/
using System;

namespace MarkThree.Forms
{

	/// <summary>
	/// Exception raised for a specific field in a specific record.
	/// </summary>
	public class LocalFieldException : Exception
	{

		private int recordId;
		private int fieldId;
		private string stringId;

		/// <summary>
		/// The record identifier.
		/// </summary>
		public int RecordId {get {return this.recordId;}}

		/// <summary>
		/// The field identifier.
		/// </summary>
		public int FieldId {get {return this.fieldId;}}

		/// <summary>
		/// The string identifier for the resource file.
		/// </summary>
		public string StringId {get {return this.stringId;}}

		/// <summary>
		/// Constructor for an execption that happens in a local record's field.
		/// </summary>
		/// <param name="recordId">Identifier of the record.</param>
		/// <param name="fieldId">Identifier of the field.</param>
		/// <param name="stringId">The error message associated with the field in the record.</param>
		public LocalFieldException(int recordId, int fieldId, string stringId)
		{

			// Initialize the object
			this.recordId = recordId;
			this.fieldId = fieldId;
			this.stringId = stringId;

		}

	}

}
