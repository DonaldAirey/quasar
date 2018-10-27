namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Common;
	using Shadows.Quasar.Client;
	using System;
	using System.Diagnostics;
	using System.Data;

	/// <summary>
	/// The common class for all financial objects.
	/// </summary>
	public class Object
	{

		/// <summary>The type of object (account, security, trading desk, etc.)</summary>
		ObjectType objectType;
		/// <summary>Internal identifier of the object.</summary>
		int objectId;
		/// <summary>The object's name.</summary>
		string name;
		/// <summary>The description of the object.</summary>
		string description;
		/// <summary>User defined data.</summary>
		string userId0;
		/// <summary>User defined data.</summary>
		string userId1;
		/// <summary>User defined data.</summary>
		string userId2;
		/// <summary>User defined data.</summary>
		string userId3;
		/// <summary>User defined data.</summary>
		string userId4;
		/// <summary>User defined data.</summary>
		string userId5;
		/// <summary>User defined data.</summary>
		string userId6;
		/// <summary>User defined data.</summary>
		string userId7;

		/// <summary>
		/// Gets the object's identifier.
		/// </summary>
		public int ObjectId {get {return this.objectId;}}

		/// <summary>
		/// Gets the object's type
		/// </summary>
		public ObjectType ObjectType {get {return this.objectType;}}

		/// <summary>
		/// Gets the object's name.
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Gets a description of the object.
		/// </summary>
		public string Description {get {return this.description;}}

		/// <summary>
		/// User supplied field #1
		/// </summary>
		public string ExternalId0 {get {return this.userId0;}}

		/// <summary>
		/// User supplied field #1
		/// </summary>
		public string ExternalId1 {get {return this.userId1;}}

		/// <summary>
		/// User supplied field #2
		/// </summary>
		public string ExternalId2 {get {return this.userId2;}}

		/// <summary>
		/// User supplied field #3
		/// </summary>
		public string ExternalId3 {get {return this.userId3;}}

		/// <summary>
		/// User supplied field #4
		/// </summary>
		public string ExternalId4 {get {return this.userId4;}}

		/// <summary>
		/// User supplied field #5
		/// </summary>
		public string ExternalId5 {get {return this.userId5;}}

		/// <summary>
		/// User supplied field #6
		/// </summary>
		public string ExternalId6 {get {return this.userId6;}}

		/// <summary>
		/// User supplied field #7
		/// </summary>
		public string ExternalId7 {get {return this.userId7;}}

		public Object() {}
		
		/// <summary>
		/// Constructor for a common object.
		/// </summary>
		/// <param name="objectId">The objects' unique identifier.</param>
		protected virtual void Initialize(int objectId)
		{

			ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(objectId);
			if (objectRow == null)
				throw new Exception(String.Format("Object {0} doesn't exist", objectId));
			
			Initialize(objectRow);

		}

		protected void Initialize(ClientMarketData.ObjectRow objectRow)
		{

			this.objectId = objectRow.ObjectId;
			this.objectType = (ObjectType)objectRow.ObjectTypeCode;
			this.name = objectRow.Name;
			if (!objectRow.IsDescriptionNull())
				this.description = objectRow.Description;
			if (!objectRow.IsExternalId0Null())
				this.userId0 = objectRow.ExternalId0;
			if (!objectRow.IsExternalId1Null())
				this.userId1 = objectRow.ExternalId1;
			if (!objectRow.IsExternalId2Null())
				this.userId2 = objectRow.ExternalId2;
			if (!objectRow.IsExternalId3Null())
				this.userId3 = objectRow.ExternalId3;
			if (!objectRow.IsExternalId4Null())
				this.userId4 = objectRow.ExternalId4;
			if (!objectRow.IsExternalId5Null())
				this.userId5 = objectRow.ExternalId5;
			if (!objectRow.IsExternalId6Null())
				this.userId6 = objectRow.ExternalId6;
			if (!objectRow.IsExternalId7Null())
				this.userId7 = objectRow.ExternalId7;

		}

		public override bool Equals(System.Object target)
		{

			if (target == null || GetType() != target.GetType())
				return false;

			return this.objectId == ((Object)target).objectId;

		}

		public override int GetHashCode() 
		{
			return this.objectId.GetHashCode();
		}
	
		public static bool operator ==(Object object1, Object object2) 
		{

			if ((object)object1 == null && (object)object2 == null)
				return true;

			if ((object)object1 == null || (object)object2 == null)
				return false;

			return (object1.objectId == object2.objectId);

		}

		public static bool operator !=(Object object1, Object object2) 
		{

			if ((object)object1 == null && (object)object2 == null)
				return false;

			if ((object)object1 == null || (object)object2 == null)
				return true;

			return (object1.objectId != object2.objectId);

		}

		/// <summary>Finds a a Object record using a configuration and an external identifier.</summary>
		/// <param name="configurationId">Specified which mappings (user id columns) to use when looking up external identifiers.</param>
		/// <param name="externalId">The external (user supplied) identifier for the record.</param>
		public static int FindKey(string configurationId, string parameterId, string externalId)
		{
			// Accessor for the Object Table.
			ClientMarketData.ObjectDataTable objectTable = ClientMarketData.Object;
			// Translate the configurationId and the predefined parameter name into an index into the array of user ids.  The index
			// is where we expect to find the identifier.  That is, an index of 1 will guide the lookup logic to use the external
			// identifiers found in the 'ExternalId1' column.
			ClientMarketData.ConfigurationRow configurationRow = ClientMarketData.Configuration.FindByConfigurationIdParameterId(configurationId, parameterId);
			int userIdIndex = 0;
			if ((configurationRow != null))
			{
				userIdIndex = configurationRow.ColumnIndex;
			}
			// This does an indirect lookup operation using the views created for the ExternalId columns.  Take the index of the user
			// identifier column calcualted above and use it to find a record containing the external identifier.
			DataView[] userIdIndexArray = new DataView[] {
															 objectTable.UKObjectExternalId0,
															 objectTable.UKObjectExternalId1,
															 objectTable.UKObjectExternalId2,
															 objectTable.UKObjectExternalId3,
															 objectTable.UKObjectExternalId4,
															 objectTable.UKObjectExternalId5,
															 objectTable.UKObjectExternalId6,
															 objectTable.UKObjectExternalId7};
			DataRowView[] dataRowView = userIdIndexArray[userIdIndex].FindRows(new object[] {
																								externalId});
			// If a row was found using the indirect operation above, then return the unique identifier of that row.  Otherwise,
			// indicate that the row doesn't exist.
			if ((dataRowView.Length == 0))
			{
				return Identifier.NotFound;
			}
			return ((int)(dataRowView[0].Row[objectTable.ObjectIdColumn]));
		}
       
	}

}
