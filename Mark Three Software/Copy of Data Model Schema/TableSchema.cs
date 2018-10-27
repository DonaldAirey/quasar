namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TableSchema : ObjectSchema, IComparable<TableSchema>
	{

		// Public Properties
		public readonly DataModelSchema DataModelSchema;

		// Private Members
		private XmlSchemaElement xmlSchemaElement;

		public TableSchema(DataModelSchema schema, XmlSchemaElement xmlSchemaElement) : base(xmlSchemaElement)
		{

			// Initialize the object.
			this.DataModelSchema = schema;
			this.xmlSchemaElement = xmlSchemaElement;

		}

		public override string ToString() {return this.Name;}
		
		public string Name { get { return this.xmlSchemaElement.Name; } }

		public XmlQualifiedName QualifiedName { get { return this.xmlSchemaElement.QualifiedName; } }


		public ColumnSchemaCollection Columns
		{

			get
			{

				ColumnSchemaCollection columnSchemaCollection = new ColumnSchemaCollection();
				GetColumns(columnSchemaCollection);
				return columnSchemaCollection;
			}

		}

		public TypeSchema GetTypeSchema()
		{

			return new TypeSchema(this.DataModelSchema, this.xmlSchemaElement.SchemaType as XmlSchemaComplexType);

		}

		private void GetColumns(ColumnSchemaCollection columnSchemaCollection)
		{

			TableSchema baseTable = this.BaseTable;
			if (baseTable != null)
				baseTable.GetColumns(columnSchemaCollection);

			if (this.xmlSchemaElement.SchemaType is XmlSchemaComplexType)
			{

				XmlSchemaComplexType xmlSchemaComplexType = this.xmlSchemaElement.SchemaType as XmlSchemaComplexType;

				// Columns can be specified as attributes.
				foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					columnSchemaCollection.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

				// The ComplexContent is mutually exclusive of the Particle.  That is, if there is no particle defined for this comlex
				// type then it must have a comlex content description.  Comlex content extends a base class.
				if (xmlSchemaComplexType.Particle == null)
				{

					// The Comlex Content describes an extension of a base class.  It is mutually exclusive of the Particle which 
					// describes a simple sequence of columns.
					if (xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
					{

						// Strongly type the XmlSchemaContent.
						XmlSchemaComplexContent xmlSchemaComplexContent = xmlSchemaComplexType.ContentModel as XmlSchemaComplexContent;

						// A complex content can be derived by extension (adding columns) or restriction (removing columns).  This
						// section will look for the extensions to the base class.
						if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
						{

							// The Complex Content Extension describes a base class and the additional columns that make up a derived
							// class.  This section will recursively collect the columns from the base class and then parse out the
							// extra columns in-line.
							XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension =
								xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

							// This will recursively read the columns from the base classes.
							TypeSchema baseType = this.DataModelSchema.GetTypeSchema(xmlSchemaComplexContentExtension.BaseTypeName);
							baseType.GetColumns(columnSchemaCollection);

							// The additional columns for this inherited table are found on the in-line in the <sequence> node that follows
							// the <extension> node.
							if (xmlSchemaComplexContentExtension.Particle is XmlSchemaSequence)
							{

								// Strongly type the XmlSchemaSequence
								XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentExtension.Particle as XmlSchemaSequence;

								// Read through the sequence and replace any column from an inherited class with the column in the
								// derived class.  Also note that the columns are added in alphabetical order to give some amount of
								// predictability to the way the parameter lists are constructed when there are several layers of
								// inheritance.
								foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
								{
									ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaObject);
									if (columnSchemaCollection.ContainsKey(columnSchema.Name))
										columnSchemaCollection.Remove(columnSchema.Name);
									columnSchemaCollection.Add(columnSchema);
								}

							}

							// The Complex Content can also contain attributes that describe columns.
							foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
							{
								ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
								if (columnSchemaCollection.ContainsKey(columnSchema.Name))
									columnSchemaCollection.Remove(columnSchema.Name);
								columnSchemaCollection.Add(columnSchema);
							}

						}

					}

				}
				else
				{

					// This section will parse the simple particle.  The particle has no inheritiance to evaluate.
					if (xmlSchemaComplexType.Particle is XmlSchemaSequence)
					{

						// Strongly type the XmlSchemaSequence member.
						XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexType.Particle as XmlSchemaSequence;

						// Each XmlSchemaElement on the Particle describes a column.
						foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
						{
							ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaObject);
							if (columnSchemaCollection.ContainsKey(columnSchema.Name))
								columnSchemaCollection.Remove(columnSchema.Name);
							columnSchemaCollection.Add(columnSchema);
						}

					}

					// The ComplexType can also have attributes that describe table columns.
					foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					{
						ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
						if (columnSchemaCollection.ContainsKey(columnSchema.Name))
							columnSchemaCollection.Remove(columnSchema.Name);
						columnSchemaCollection.Add(columnSchema);
					}

				}

			}

		}

		internal ColumnSchemaCollection MemberColumns
		{

			get
			{

				ColumnSchemaCollection columnSchemaCollection = new ColumnSchemaCollection();

				if (this.xmlSchemaElement.SchemaType is XmlSchemaComplexType)
				{

					XmlSchemaComplexType xmlSchemaComplexType = this.xmlSchemaElement.SchemaType as XmlSchemaComplexType;

					// Columns can be specified as attributes.
					foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
						columnSchemaCollection.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

					// The ComplexContent is mutually exclusive of the Particle.  That is, if there is no particle defined for this comlex
					// type then it must have a comlex content description.  Comlex content extends a base class.
					if (xmlSchemaComplexType.Particle == null)
					{

						// The Comlex Content describes an extension of a base class.
						if (xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
						{

							// Strongly type the XmlSchemaContent.
							XmlSchemaComplexContent xmlSchemaComplexContent = xmlSchemaComplexType.ContentModel as XmlSchemaComplexContent;

							// A complex content can be derived by extension (adding columns) or restriction (removing columns).  This
							// section will look for the extensions to the base class.
							if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
							{

								// The Complex Content Extension describes a base class and the additional columns that make up a derived
								// class.  This section will recursively collect the columns from the base class and then parse out the
								// extra columns in-line.
								XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension =
									xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

								// The additional columns for this inherited table are found on the in-line in the <sequence> node that follows
								// the <extension> node.
								if (xmlSchemaComplexContentExtension.Particle is XmlSchemaSequence)
								{

									// Strongly type the XmlSchemaSequence
									XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentExtension.Particle as XmlSchemaSequence;

									// Read through the sequence and replace any column from an inherited class with the column in the
									// derived class.  Also note that the columns are added in alphabetical order to give some amount of
									// predictability to the way the parameter lists are constructed when there are several layers of
									// inheritance.
									foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
									{
										ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaObject);
										if (columnSchemaCollection.ContainsKey(columnSchema.Name))
											columnSchemaCollection.Remove(columnSchema.Name);
										columnSchemaCollection.Add(columnSchema);
									}

								}

								// The Complex Content can also contain attributes that describe columns.
								foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
								{
									ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
									if (columnSchemaCollection.ContainsKey(columnSchema.Name))
										columnSchemaCollection.Remove(columnSchema.Name);
									columnSchemaCollection.Add(columnSchema);
								}

							}

						}

					}
					else
					{

						// This section will parse the simple particle.  The particle has no inheritiance to evaluate.
						if (xmlSchemaComplexType.Particle is XmlSchemaSequence)
						{

							// Strongly type the XmlSchemaSequence member.
							XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexType.Particle as XmlSchemaSequence;

							// Each XmlSchemaElement on the Particle describes a column.
							foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
							{
								ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaObject);
								if (columnSchemaCollection.ContainsKey(columnSchema.Name))
									columnSchemaCollection.Remove(columnSchema.Name);
								columnSchemaCollection.Add(columnSchema);
							}

						}

						// The ComplexType can also have attributes that describe table columns.
						foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
						{
							ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
							if (columnSchemaCollection.ContainsKey(columnSchema.Name))
								columnSchemaCollection.Remove(columnSchema.Name);
							columnSchemaCollection.Add(columnSchema);
						}

					}

				}

				return columnSchemaCollection;

			}

		}
		
		public ConstraintSchemaCollection Constraints
		{

			get
			{

				ConstraintSchemaCollection constraints = new ConstraintSchemaCollection();

				// If the schema is managed by the DataSet designer, then the constraints for a given table are specified as child
				// elements of the 'DataSet' element.  The selector on the constraint indicates which table is bound to that 
				// constraint.  If any of the constraints specified on a DataSet select this table, then it is considered to be
				// part of the constraints on this table.
				foreach (ObjectSchema objectSchema in this.DataModelSchema.Items)
					if (objectSchema is DataSetSchema)
					{
						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;
						foreach (ConstraintSchema constraintSchema in dataSetSchema.Constraints)
							if (constraintSchema.Selector == this)
								constraints.Add(constraintSchema);
					}

				foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in this.xmlSchemaElement.Constraints)
				{
					if (xmlSchemaIdentityConstraint is XmlSchemaKey)
					{
						XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
						constraints.Add(new KeySchema(this.DataModelSchema, xmlSchemaKey));
					}

					if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
					{
						XmlSchemaUnique xmlSchemaUnique = xmlSchemaIdentityConstraint as XmlSchemaUnique;
						constraints.Add(new UniqueSchema(this.DataModelSchema, xmlSchemaUnique));
					}

					if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
					{
						XmlSchemaKeyref xmlSchemaKeyref = xmlSchemaIdentityConstraint as XmlSchemaKeyref;
						constraints.Add(new KeyrefSchema(this.DataModelSchema, xmlSchemaKeyref));
					}

				}

				return constraints;

			}

		}

		public ConstraintSchema[] Keys
		{

			get
			{

				List<ConstraintSchema> keys = new List<ConstraintSchema>();

				// If the schema is managed by the DataSet designer, then the constraints for a given table are specified as child
				// elements of the 'DataSet' element.  The selector on the constraint indicates which table is bound to that 
				// constraint.  If any of the constraints specified on a DataSet select this table, then it is considered to be
				// part of the constraints on this table.
				foreach (ObjectSchema objectSchema in this.DataModelSchema.Items)
					if (objectSchema is DataSetSchema)
					{
						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;
						foreach (ConstraintSchema constraintSchema in dataSetSchema.Constraints)
							if (constraintSchema.Selector == this)
								if (constraintSchema is KeySchema || constraintSchema is UniqueSchema)
									keys.Add(constraintSchema);
					}

				foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in this.xmlSchemaElement.Constraints)
				{
					if (xmlSchemaIdentityConstraint is XmlSchemaKey)
					{
						XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
						keys.Add(new KeySchema(this.DataModelSchema, xmlSchemaKey));
					}

					if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
					{
						XmlSchemaUnique xmlSchemaUnique = xmlSchemaIdentityConstraint as XmlSchemaUnique;
						keys.Add(new UniqueSchema(this.DataModelSchema, xmlSchemaUnique));
					}
				}

				return keys.ToArray();

			}

		}

		public KeyrefSchema[] ParentKeyrefs
		{

			get
			{

				List<KeyrefSchema> keyrefs = new List<KeyrefSchema>();

				// If the schema is managed by the DataSet designer, then the constraints for a given table are specified as child
				// elements of the 'DataSet' element.  The selector on the constraint indicates which table is bound to that 
				// constraint.  If any of the constraints specified on a DataSet select this table, then it is considered to be
				// part of the constraints on this table.
				foreach (ObjectSchema objectSchema in this.DataModelSchema.Items)
					if (objectSchema is DataSetSchema)
					{
						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;
						foreach (ConstraintSchema constraintSchema in dataSetSchema.Constraints)
							if (constraintSchema.Selector == this)
								if (constraintSchema is KeyrefSchema)
									keyrefs.Add(constraintSchema as KeyrefSchema);
					}

				foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in this.xmlSchemaElement.Constraints)
				{

					if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
					{
						XmlSchemaKeyref xmlSchemaKeyref = xmlSchemaIdentityConstraint as XmlSchemaKeyref;
						keyrefs.Add(new KeyrefSchema(this.DataModelSchema, xmlSchemaKeyref));
					}

				}

				return keyrefs.ToArray();

			}

		}

		public TypeSchema TypeSchema
		{

			get
			{

				if (this.xmlSchemaElement.SchemaTypeName != XmlQualifiedName.Empty)
					return this.DataModelSchema.Classes.Find(this.xmlSchemaElement.SchemaTypeName.Name);
				else
				{
					TypeSchema typeSchema = new TypeSchema(this.DataModelSchema, this.xmlSchemaElement.SchemaType as XmlSchemaComplexType);
					string name = typeSchema.Name;
					return typeSchema;
				}

			}

		}

		public TypeSchema BaseClass
		{

			get
			{

				// Simple types aren't understood in this context, only complex types can describe a table.
				if (this.xmlSchemaElement.SchemaType is XmlSchemaComplexType)
				{

					XmlSchemaComplexType xmlSchemaComplexType = this.xmlSchemaElement.SchemaType as XmlSchemaComplexType;

					// Independent tables don't have a base class.
					if (xmlSchemaComplexType.Particle == null)
					{

						// Parse the Schema for Complex Content.
						if (xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
						{

							// Strongly type the XmlSchemaContent.
							XmlSchemaComplexContent xmlSchemaComplexContent = xmlSchemaComplexType.ContentModel as XmlSchemaComplexContent;

							// The additional columns for this table can be found on the 'ContentExtension' nodes.
							if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
							{

								// Stronly type the XmlSchemaComplexContentExtension
								XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

								// This will find a Complex Type Declaration that has the same name.
								return this.DataModelSchema.Classes.Find(xmlSchemaComplexContentExtension.BaseTypeName.Name);

							}

						}

					}

				}

				return null;

			}

		}

		public ConstraintSchema PrimaryKey
		{

			get
			{

				foreach (ConstraintSchema constraintSchema in this.Constraints)
					if (constraintSchema is KeySchema || constraintSchema is UniqueSchema && constraintSchema.IsPrimaryKey)
						return constraintSchema;

				return null;
			}

		}

		public TableSchema BaseTable
		{

			get
			{

				ConstraintSchema primaryKey = this.PrimaryKey;
				if (primaryKey == null)
					return null;

				foreach (ObjectSchema objectSchema in this.DataModelSchema.Items)
				{

					if (objectSchema is DataSetSchema)
					{

						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;

						foreach (ConstraintSchema constraintSchema in dataSetSchema.Constraints)
						{

							if (constraintSchema is KeyrefSchema)
							{

								KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;

								if (keyrefSchema.Selector == this)
								{

									bool isMatch = keyrefSchema.Fields.Length == primaryKey.Fields.Length;
									for (int index = 0; index < keyrefSchema.Fields.Length; index++)
										if (keyrefSchema.Fields[index] != primaryKey.Fields[index])
										{
											isMatch = false;
											break;
										}

									if (isMatch)
										return keyrefSchema.Refer.Selector;

								}

							}

						}

					}

				}

				return null;

			}

		}

		public override bool Equals(object obj)
		{
			if (obj is TableSchema)
			{
				TableSchema tableSchema = obj as TableSchema;
				return this.QualifiedName.Equals(tableSchema.QualifiedName);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.QualifiedName.GetHashCode();
		}

		public static bool operator==(TableSchema tableOne, TableSchema tableTwo)
		{
			return object.ReferenceEquals(tableOne, null) && object.ReferenceEquals(tableTwo, null) ? true :
				object.ReferenceEquals(tableOne, null) && !object.ReferenceEquals(tableTwo, null) ? false :
				!object.ReferenceEquals(tableOne, null) && object.ReferenceEquals(tableTwo, null) ? false :
				tableOne.QualifiedName == tableTwo.QualifiedName;
		}

		public static bool operator !=(TableSchema tableOne, TableSchema tableTwo)
		{
			return object.ReferenceEquals(tableOne, null) && object.ReferenceEquals(tableTwo, null) ? false :
				object.ReferenceEquals(tableOne, null) && !object.ReferenceEquals(tableTwo, null) ? true :
				!object.ReferenceEquals(tableOne, null) && object.ReferenceEquals(tableTwo, null) ? true :
				tableOne.QualifiedName != tableTwo.QualifiedName;
		}

		/// <summary>
		/// Tests to see if a given column is part of the primary key.
		/// </summary>
		/// <param name="elementTable">The table schema to test.</param>
		/// <param name="columnSchema">The column schema to test.</param>
		/// <returns>True if the column belongs to the primary key of the table.</returns>
		public bool IsPrimaryKeyColumn(ColumnSchema columnSchema)
		{

			// Find the primary key of the requested table.
			ConstraintSchema keySchema = PrimaryKey;

			// Test each field in the key to see if the selector matches the given column name.
			foreach (ColumnSchema fieldSchema in keySchema.Fields)
				if (fieldSchema == columnSchema)
					return true;

			// If there is a base class, then see if this column is part of the base class primary key.
			if (this.BaseTable != null)
				return this.BaseTable.IsPrimaryKeyColumn(columnSchema);

			// At this point, the column doesn not belong to any of the primary keys in the hiearchy.
			return false;

		}

		public bool IsAutogeneratedColumn(ColumnSchema columnSchema)
		{

			return columnSchema.IsIdentityColumn || (this.IsPrimaryKeyColumn(columnSchema) && this.BaseTable != null && this.BaseTable.HasIdentityColumn);

		}

		public List<TableSchema> GetParentTables
		{

			get
			{
				// Search the keys for a parent table.
				return null;
			}

		}

		/// <summary>
		/// Indicates whether a column is or is descended from an identity column.
		/// </summary>
		/// <param name="elementTable">The table description.</param>
		/// <param name="columnSchema">The column description.</param>
		/// <returns>true of the column is or is descended from an identity column.</returns>
		public bool IsExternalIdColumn(ColumnSchema columnSchema)
		{

			if (IsPrimaryKeyColumn(columnSchema) && this.HasExternalIdColumn)
				return true;

			foreach (TableSchema parentTable in this.GetParentTables)
				if (parentTable.IsExternalIdColumn(columnSchema))
					return true;

			// At this point, the object hierarchy has been searched and no identity column was found.
			return false;

		}

		/// <summary>
		/// Finds a foreign key that matches the table and collection of columns.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public int FindColumnIndex(string columnName)
		{

			return this.Columns.IndexOfKey(columnName);

		}

		/// <summary>
		/// Tests to see if a column is an identity column.
		/// </summary>
		/// <param name="elementTable">The table to be tested for an identity column.</param>
		/// <returns>True if the table contains an identity column.</returns>
		public bool HasExternalIdColumn
		{

			get
			{

				// Check each column in the table to see if it has an 'Identity' attribute.
				foreach (ColumnSchema columnSchema in this.Columns)
					if (columnSchema.Name.IndexOf("ExternalId") != -1)
						return true;

				// If a base table exists, search it also for 'ExternalId' columns.
				TableSchema baseTable = this.BaseTable;
				return baseTable == null ? false : baseTable.HasExternalIdColumn;

			}

		}

		/// <summary>
		/// Tests to see if a column is an identity column.
		/// </summary>
		/// <param name="elementTable">The table to be tested for an identity column.</param>
		/// <returns>True if the table contains an identity column.</returns>
		public bool HasIdentityColumn
		{

			get
			{

				// Check each column in the table to see if it has an 'Identity' attribute.
				foreach (ColumnSchema columnSchema in this.Columns)
					if (columnSchema.IsIdentityColumn)
						return true;

				// Test to see if any of the base tables contains an identity column.
				TableSchema baseTable = this.BaseTable;
				return baseTable == null ? false : baseTable.HasIdentityColumn;

			}

		}

		private void GetParentKeys(List<KeyrefSchema> parentKeys)
		{

			// Recurse down into the base classes looking for parent keys.
			TableSchema baseTable = this.BaseTable;
			if (baseTable != null)
				baseTable.GetParentKeys(parentKeys);

			// Search through all the foreign keys looking for a selector that matches the given table name.
			foreach (ConstraintSchema constraintSchema in this.Constraints)
				if (constraintSchema is KeyrefSchema)
				{
					KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;
					if (keyrefSchema.Selector == this)
						parentKeys.Add(keyrefSchema);
				}

		}

		/// <summary>
		/// Finds all foreign keys for a given table.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public List<KeyrefSchema> ParentKeys
		{

			get
			{

				// This will hold the Keyref objects temporarily while they're collected.
				List<KeyrefSchema> foreignKeyList = new List<KeyrefSchema>();

				// Call the recursive procedure to fill in the array.
				GetParentKeys(foreignKeyList);

				return foreignKeyList;

			}

		}

		/// <summary>
		/// Gets the parent keys of just the table requested.
		/// </summary>
		/// <param name="foreignKeyList"></param>
		/// <param name="elementTable"></param>
		private void GetMemberParentKeys(List<KeyrefSchema> foreignKeyList)
		{

			foreach (ConstraintSchema constraintSchema in this.Constraints)
				if (constraintSchema is KeyrefSchema)
				{
					KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;
					foreignKeyList.Add(keyrefSchema);
				}

		}

		/// <summary>
		/// Finds foreign keys for a given table.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public KeyrefSchema[] MemberParentKeys
		{

			get
			{

				// This will hold the Keyref objects temporarily while they're collected.
				List<KeyrefSchema> foreignKeyList = new List<KeyrefSchema>();

				// Call the recursive procedure to fill in the array.
				GetMemberParentKeys(foreignKeyList);

				// At this point, no matching keys were found.
				return foreignKeyList.ToArray();

			}

		}

		/// <summary>
		/// Finds all child foreign keys for a given table.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <returns>An array of foreign key that are children of the given table.</returns>
		public KeyrefSchema[] ChildKeyrefs
		{

			get
			{

				// This will hold the Keyref objects temporarily while they're collected.
				List<KeyrefSchema> childKeyList = new List<KeyrefSchema>();

				foreach (TableSchema tableSchema in this.DataModelSchema.Tables)
					foreach (KeyrefSchema keyrefSchema in tableSchema.ParentKeyrefs)
						if (keyrefSchema.Refer.Selector == this)
							childKeyList.Add(keyrefSchema);

				return childKeyList.ToArray();

			}


		}

		/// <summary>
		/// Finds a foreign key that matches the table and collection of columns.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public KeyrefSchema FindForeignKey(ColumnSchema[] columns)
		{

			foreach (ConstraintSchema constraintSchema in this.Constraints)
				if (constraintSchema is KeyrefSchema)
				{

					// Create a Strongly Typed Keyref record.
					KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;

					// A match must include all of the columns in the foreign key.  Try every permuatation of field against
					// field looking for a match.
					if (keyrefSchema.Selector == this)
					{

						int matches = 0;
						foreach (ColumnSchema fieldSchema in keyrefSchema.Fields)
							foreach (ColumnSchema columnSchema in columns)
								if (fieldSchema == columnSchema)
									matches++;

						// If the number of matches found is the same as the number of columns in the key, then this record is a match.
						if (matches == columns.Length)
							return keyrefSchema;

					}

				}

			// At this point, no matching keys were found.
			TableSchema baseTable = this.BaseTable;
			return baseTable == null ? null : baseTable.FindForeignKey(columns);

		}

		/// <summary>
		/// Indicates that a column is an identity column.
		/// </summary>
		/// <param name="columnSchema">The description of a column.</param>
		/// <returns>true if the column has an internally generated identifier.</returns>
		public bool IsIdentityColumn(ColumnSchema columnSchema)
		{

			if (columnSchema.IsIdentityColumn)
				return true;

			// See if the column may is part of an identity column in the base table.
			TableSchema baseTable = this.BaseTable;
			if (baseTable != null)
			{

				// See if the column is the identity column in the base table.
				if (baseTable.IsIdentityColumn(columnSchema))
					return true;

				// If the column is part of the primary key, then it's possible that the column is an identity column through a 
				// primary key relation.
				ConstraintSchema keySchema = this.PrimaryKey;
				if (keySchema != null && keySchema.Fields.Length == 1 && keySchema.Fields[0] == columnSchema)
				{

					// See if the column is related to an identity column through the primary index.
					ConstraintSchema baseKeySchema = baseTable.PrimaryKey;
					KeyrefSchema baseKeyRefSchema = this.FindForeignKey(new ColumnSchema[] { columnSchema });
					if (baseKeyRefSchema != null && baseKeyRefSchema.Refer == baseKeySchema)
						if (baseKeySchema.Fields.Length == 1)
						{
							ColumnSchema baseColumnSchema = baseTable.Columns[baseKeySchema.Fields[0].Name];
							return baseTable.IsExternalIdColumn(baseColumnSchema);
						}

				}

			}

			return false;

		}

		public bool IsPersistent
		{

			get
			{

				object isTablePersistentFlag = GetUnhandledAttribute("persistent");
				return isTablePersistentFlag == null ? true : Convert.ToBoolean(isTablePersistentFlag);

			}

		}

		public object GetUnhandledAttribute(string attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (this.xmlSchemaElement.UnhandledAttributes != null)
				foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
					if (xmlAttribute.LocalName == attributeName)
						return xmlAttribute.Value;

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

		public TableSchema[] TableHierarchy
		{

			get
			{

				List<TableSchema> tableHierarchy = new List<TableSchema>();
				TableSchema baseTable = this;
				while (baseTable != null)
				{
					tableHierarchy.Add(baseTable);
					baseTable = baseTable.BaseTable;
				}
				tableHierarchy.Reverse();
				return tableHierarchy.ToArray();

			}

		}

		public TableSchema[] Descendants
		{

			get
			{

				List<TableSchema> tableHierarchy = new List<TableSchema>();
				GetDescendants(tableHierarchy);
				return tableHierarchy.ToArray();

			}

		}

		private void GetDescendants(List<TableSchema> tableHierarchy)
		{

			foreach (KeyrefSchema childKeyref in this.ChildKeyrefs)
			{
				childKeyref.Selector.GetDescendants(tableHierarchy);
				if (!tableHierarchy.Contains(childKeyref.Selector))
					tableHierarchy.Add(childKeyref.Selector);
			}

		}

		public bool IsInheritedKey(ColumnSchema columnSchema)
		{

			return this.IsPrimaryKeyColumn(columnSchema) && this.BaseTable != null;

		}

		/// <summary>
		/// Indicates that the given foreign key is a reference to the base table.
		/// </summary>
		/// <param name="keyrefSchema">A foreign key description.</param>
		/// <returns>true if the given foreign key refers to the base table.</returns>
		public bool IsBaseKeyref(KeyrefSchema keyrefSchema)
		{

			// A true indicates that the given foreign key references the immediate base class of this table.
			return this.BaseTable != null && this.BaseTable == keyrefSchema.Refer.Selector;

		}

		public TableSchema RootTable
		{

			get
			{
				TableSchema rootTable = this;
				while (rootTable.BaseTable != null)
					rootTable = rootTable.BaseTable;
				return rootTable;
			}

		}

		/// <summary>
		/// Gets the number of times the specified table is referenced by parent keys.
		/// </summary>
		/// <param name="tableSchema"></param>
		/// <returns></returns>
		public int ParentKeyrefCount(TableSchema tableSchema)
		{

			int count = 0;
			foreach (KeyrefSchema keyrefSchema in this.ParentKeyrefs)
				if (keyrefSchema.Refer.Selector == tableSchema)
					count++;

			return count;

		}

		#region IComparable<TableSchema> Members

		public int CompareTo(TableSchema other)
		{
			int namespaceCompare = this.QualifiedName.Namespace.CompareTo(other.QualifiedName.Namespace);
			if (namespaceCompare != 0)
				return namespaceCompare;
			return this.QualifiedName.Name.CompareTo(other.QualifiedName.Name);
		}

		#endregion
	}

}
