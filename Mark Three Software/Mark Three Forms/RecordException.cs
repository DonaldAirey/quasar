namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// Exception raised for a specific field in a specific record.
	/// </summary>
	public class RecordException : Exception
	{

		private int recordId;
		private string stringId;

		/// <summary>
		/// The record identifier.
		/// </summary>
		public int RecordId {get {return this.recordId;}}

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
		public RecordException(int recordId, string stringId)
		{

			// Initialize the object
			this.recordId = recordId;
			this.stringId = stringId;

		}

	}

}
