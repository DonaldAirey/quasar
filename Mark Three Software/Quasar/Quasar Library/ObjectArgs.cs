/*************************************************************************************************************************
*
*	File:			ObjectArgs.cs
*	Description:	Arguments used to describe a Quasar Object.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Used by events that open an object (account, trading desk, security).
	/// </summary>
	public class ObjectArgs : System.EventArgs
	{

		private int objectId;
		private string typeCode;
		private string name;

		/// <summary>
		/// The unique identifier of an object.
		/// </summary>
		public int ObjectId {get {return this.objectId;}}

		/// <summary>
		/// The type of object.
		/// </summary>
		public String TypeCode {get {return this.typeCode;}}

		/// <summary>
		/// The display name of the object.
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Initializes a new instance of the MarkThree.Quasar.ObjectArgs class.
		/// </summary>
		public ObjectArgs() {}

		/// <summary>
		/// Initializes a new instance of the MarkThree.Quasar.ObjectArgs class.
		/// </summary>
		/// <param name="type">Type of object to open.</param>
		/// <param name="objectId">Unique identifier of the object.</param>
		public ObjectArgs(string typeCode, int objectId) {this.typeCode = typeCode; this.objectId = objectId;}

		/// <summary>
		/// Initializes a new instance of the MarkThree.Quasar.ObjectArgs class.
		/// </summary>
		/// <param name="type">Type of object to open.</param>
		/// <param name="objectId">Unique identifier of the object.</param>
		/// <param name="name">Display name of the obect.</param>
		/// <param name="argument">Variable length list of argument for opening the document.</param>
		public ObjectArgs(string typeCode, int objectId, string name) {this.typeCode = typeCode; this.objectId = objectId; this.name = name;}

		/// <summary>
		/// Converts the Object Arguments to string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{

			// Return a formated string
			return "{" + String.Format("Type={0}, Id={1}, Name={2}", this.typeCode, this.objectId, this.name) + "}";
														
		}

	}

	/// <summary>
	/// Used by controls to notify the container that an object needs to be opened in a viewer.
	/// </summary>
	public delegate void ObjectOpenEventHandler(object sender, ObjectArgs event_args);

}
