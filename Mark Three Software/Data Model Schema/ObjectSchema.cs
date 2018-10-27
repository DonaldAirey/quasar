namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Schema;
	using System.Xml;

	public class ObjectSchema
	{

		// Public Readonly Fields
		public DataModelSchema DataModelSchema;
		public static XmlQualifiedName primaryKeyAttributeName;

		private XmlSchemaObject xmlSchemaObject;

		static ObjectSchema()
		{

			// Initialize the static fields.
			ObjectSchema.primaryKeyAttributeName = new XmlQualifiedName("PrimaryKey", "urn:schemas-microsoft-com:xml-msdata");

		}

		public ObjectSchema(DataModelSchema dataModelSchema, XmlSchemaObject xmlSchemaObject)
		{

			// Initialize the object.
			this.DataModelSchema = dataModelSchema;
			this.xmlSchemaObject = xmlSchemaObject;

		}

		protected XmlSchema RootSchema
		{

			get
			{

				XmlSchemaObject rootObject = this.xmlSchemaObject;
				while (rootObject.Parent != null)
					rootObject = rootObject.Parent;

				return rootObject as XmlSchema;

			}

		}

		protected static bool IsDataSetElement(XmlSchemaObject xmlSchemaObject)
		{

			if (xmlSchemaObject is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
				if (xmlSchemaElement.UnhandledAttributes != null)
					foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
						if (xmlAttribute.LocalName == "IsDataSet")
							return Convert.ToBoolean(xmlAttribute.Value);
			}

			return false;

		}

		public bool ContainsColumn(XmlSchemaComplexType xmlSchemaComplexType, XmlSchemaObject columnObject)
		{

			XmlQualifiedName columnName = columnObject is XmlSchemaAttribute ?
				(columnObject as XmlSchemaAttribute).QualifiedName : columnObject is XmlSchemaElement ?
				(columnObject as XmlSchemaElement).QualifiedName : XmlQualifiedName.Empty;

			// Columns can be specified as attributes.
			foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
				if (xmlSchemaAttribute.QualifiedName == columnName)
					return true;

			// Comlex content extends a base class.  The ComplexContent is mutually exclusive of the Particle, so if the 
			// particle is empty, the ComplexContent is present and should be parsed for columns.
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

						// The Complex Content Extension describes a base class and the additional columns that make up a 
						// derived class.  This section will recursively collect the columns from the base class and then parse
						// out the extra columns in-line.
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension =
							xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

						// The additional columns for this inherited table are found on the <Sequence> node that follows the
						// <Extension> node.
						if (xmlSchemaComplexContentExtension.Particle is XmlSchemaSequence)
						{

							// Strongly type the XmlSchemaSequence
							XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentExtension.Particle as XmlSchemaSequence;

							// Read through the sequence and replace any column from an inherited class with the column in the
							// derived class.  Also note that the columns are added in alphabetical order to give some amount of
							// predictability to the way the parameter lists are constructed when there are several layers of
							// inheritance.
							foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
								if ((xmlSchemaObject as XmlSchemaElement).QualifiedName == columnName)
									return true;

						}

						// The Complex Content can also contain attributes that describe columns.
						foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
							if (xmlSchemaAttribute.QualifiedName == columnName)
								return true;

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
						if ((xmlSchemaObject as XmlSchemaElement).QualifiedName == columnName)
							return true;

				}

				// The ComplexType can also have attributes that describe table columns.
				foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					if (xmlSchemaAttribute.QualifiedName == columnName)
						return true;

			}

			return false;

		}

		public ColumnSchema[] GetColumns(XmlSchemaElement tableElement)
		{

			List<ColumnSchema> columnList = new List<ColumnSchema>();

			if (tableElement.SchemaType is XmlSchemaComplexType)
			{

				XmlSchemaComplexType xmlSchemaComplexType = tableElement.SchemaType as XmlSchemaComplexType;

				// Columns can be specified as attributes.
				foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					columnList.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

				// Comlex content extends a base class.  The ComplexContent is mutually exclusive of the Particle, so if the 
				// particle is empty, the ComplexContent is present and should be parsed for columns.
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

							// The Complex Content Extension describes a base class and the additional columns that make up a 
							// derived class.  This section will recursively collect the columns from the base class and then parse
							// out the extra columns in-line.
							XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension =
								xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

							// The additional columns for this inherited table are found on the <Sequence> node that follows the
							// <Extension> node.
							if (xmlSchemaComplexContentExtension.Particle is XmlSchemaSequence)
							{

								// Strongly type the XmlSchemaSequence
								XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentExtension.Particle as XmlSchemaSequence;

								// Read through the sequence and replace any column from an inherited class with the column in the
								// derived class.  Also note that the columns are added in alphabetical order to give some amount of
								// predictability to the way the parameter lists are constructed when there are several layers of
								// inheritance.
								foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
									columnList.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaObject));

							}

							// The Complex Content can also contain attributes that describe columns.
							foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
								columnList.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

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
							columnList.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaObject));

					}

					// The ComplexType can also have attributes that describe table columns.
					foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
						columnList.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

				}

			}

			return columnList.ToArray();

		}

		public XmlSchemaElement FindSelector(XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
		{

			string[] tokens = xmlSchemaIdentityConstraint.Selector.XPath.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
			XmlSchemaObject navigationObject = xmlSchemaIdentityConstraint.Parent;
			foreach (string token in tokens)
			{
				switch (token)
				{
				case ".":

					break;

				case "..":

					navigationObject = navigationObject.Parent;
					break;

				default:

					string[] qNameParts = token.Split(':');
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(qNameParts[1],
						this.DataModelSchema.XmlNamespaceManager.LookupNamespace(qNameParts[0]));
					navigationObject = NavigateSchema(navigationObject, xmlQualifiedName);
					break;

				}

			}

			if (navigationObject is XmlSchemaElement)
				return navigationObject as XmlSchemaElement;

			return null;

		}

		private static XmlSchemaObject NavigateSchema(XmlSchemaObject navigationObject, XmlQualifiedName xmlQualifiedName)
		{

			XmlSchemaElement parentElement = navigationObject as XmlSchemaElement;
			if (parentElement.SchemaTypeName == XmlQualifiedName.Empty)
			{
				if (parentElement.SchemaType is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = parentElement.SchemaType as XmlSchemaComplexType;
					if (xmlSchemaComplexType.Particle is XmlSchemaChoice)
					{
						XmlSchemaChoice xmlSchemaChoice = xmlSchemaComplexType.Particle as XmlSchemaChoice;
						foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaChoice.Items)
						{
							if (xmlSchemaObject is XmlSchemaElement)
							{
								XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
								if (xmlSchemaElement.QualifiedName == xmlQualifiedName)
									return xmlSchemaElement;
							}
						}
					}
				}
			}

			return null;

		}

		public bool GetPrimaryKeyStatus(XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
		{

			if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
			{
				XmlSchemaUnique xmlSchemaUnique = xmlSchemaIdentityConstraint as XmlSchemaUnique;
				if (xmlSchemaUnique.UnhandledAttributes != null)
					foreach (XmlAttribute xmlAttribute in xmlSchemaUnique.UnhandledAttributes)
						if (xmlAttribute.NamespaceURI == KeySchema.primaryKeyAttributeName.Namespace &&
							xmlAttribute.LocalName == KeySchema.primaryKeyAttributeName.Name)
							return Convert.ToBoolean(xmlAttribute.Value);
			}

			if (xmlSchemaIdentityConstraint is XmlSchemaKey)
			{
				XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
				if (xmlSchemaKey.UnhandledAttributes != null)
					foreach (XmlAttribute xmlAttribute in xmlSchemaKey.UnhandledAttributes)
						if (xmlAttribute.NamespaceURI == KeySchema.primaryKeyAttributeName.Namespace &&
							xmlAttribute.LocalName == KeySchema.primaryKeyAttributeName.Name)
							return Convert.ToBoolean(xmlAttribute.Value);

			}

			return false;

		}

		public XmlSchemaIdentityConstraint FindPrimaryKey(XmlSchemaElement xmlSchemaElement)
		{

			foreach (XmlSchemaObject xmlSchemaObject in GetXmlSchema(xmlSchemaElement).Items)
			{

				if (IsDataSetElement(xmlSchemaObject))
				{

					XmlSchemaElement dataSetElement = xmlSchemaObject as XmlSchemaElement;
					foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in dataSetElement.Constraints)
					{
						if (GetPrimaryKeyStatus(xmlSchemaIdentityConstraint) && FindSelector(xmlSchemaIdentityConstraint) == xmlSchemaElement)
							return xmlSchemaIdentityConstraint;
					}

				}

				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement tableElement = xmlSchemaObject as XmlSchemaElement;
					foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in tableElement.Constraints)
						if (GetPrimaryKeyStatus(xmlSchemaIdentityConstraint) && FindSelector(xmlSchemaIdentityConstraint) == xmlSchemaElement)
							return xmlSchemaIdentityConstraint;
				}

			}

			return null;

		}

		public XmlSchemaComplexType FindBaseType(XmlSchemaObject baseObject)
		{

			if (baseObject is XmlSchemaComplexContentExtension)
			{
				XmlSchemaComplexContentExtension baseComplexContentExtension = baseObject as XmlSchemaComplexContentExtension;
				return FindComplexType(baseComplexContentExtension);
			}

			if (baseObject is XmlSchemaComplexType)
			{

				XmlSchemaComplexType baseComplexType = baseObject as XmlSchemaComplexType;
				if (baseComplexType.Parent is XmlSchemaElement)
				{
					XmlSchemaElement baseElement = baseComplexType.Parent as XmlSchemaElement;

					XmlSchemaIdentityConstraint primaryKey = FindPrimaryKey(baseElement);
					if (primaryKey == null)
						return null;

					foreach (XmlSchemaObject xmlSchemaObject in RootSchema.Items)
					{

						if (xmlSchemaObject is XmlSchemaElement)
						{

							if (IsDataSetElement(xmlSchemaObject))
							{

								XmlSchemaElement dataSetElement = xmlSchemaObject as XmlSchemaElement;

								foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in dataSetElement.Constraints)
								{
									if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
									{

										XmlSchemaKeyref xmlSchemaKeyref = xmlSchemaIdentityConstraint as XmlSchemaKeyref;
										if (this.FindSelector(xmlSchemaKeyref) == baseElement)
										{

											bool isMatch = xmlSchemaKeyref.Fields.Count == primaryKey.Fields.Count;
											for (int index = 0; index < xmlSchemaKeyref.Fields.Count; index++)
											{
												XmlSchemaXPath xPath1 = (XmlSchemaXPath)xmlSchemaKeyref.Fields[index];
												XmlSchemaXPath xPath2 = (XmlSchemaXPath)primaryKey.Fields[index];
												if (xPath1.XPath != xPath2.XPath)
												{
													isMatch = false;
													break;
												}
											}

											if (isMatch)
											{
												XmlSchemaIdentityConstraint xmlSchemaKey = FindKey(xmlSchemaKeyref);
												XmlSchemaElement xmlSchemaElement = FindSelector(xmlSchemaKey);
												return xmlSchemaElement.SchemaType as XmlSchemaComplexType;
											}

										}

									}

								}

							}
							else
							{

								XmlSchemaElement tableElement = xmlSchemaObject as XmlSchemaElement;

								foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in tableElement.Constraints)
								{




								}

							}

						}

					}

					return null;

				}

			}

			return null;

		}

		public static XmlSchema GetXmlSchema(XmlSchemaObject xmlSchemaObject)
		{

			// Move up the tree to the root object.
			while (xmlSchemaObject.Parent != null)
				xmlSchemaObject = xmlSchemaObject.Parent;
			return xmlSchemaObject as XmlSchema;

		}

		public static XmlSchemaComplexType FindComplexType(XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension)
		{

			foreach (XmlSchemaObject xmlSchemaObject in GetXmlSchema(xmlSchemaComplexContentExtension).Items)
				if (xmlSchemaObject is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaObject as XmlSchemaComplexType;
					if (xmlSchemaComplexType.QualifiedName == xmlSchemaComplexContentExtension.BaseTypeName)
						return xmlSchemaComplexType;
				}

			return null;

		}

		public static XmlSchemaIdentityConstraint FindKey(XmlSchemaKeyref xmlSchemaKeyref)
		{

			foreach (XmlSchemaObject xmlSchemaObject in GetXmlSchema(xmlSchemaKeyref).Items)
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;

					if (IsDataSetElement(xmlSchemaElement))
					{
						XmlSchemaElement dataSetElement = xmlSchemaObject as XmlSchemaElement;
						foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in dataSetElement.Constraints)
						{
							if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
							{
								XmlSchemaUnique xmlSchemaUnique = xmlSchemaIdentityConstraint as XmlSchemaUnique;
								if (xmlSchemaUnique.QualifiedName.Namespace == xmlSchemaKeyref.Refer.Namespace &&
									xmlSchemaUnique.QualifiedName.Name == xmlSchemaKeyref.Refer.Name)
									return xmlSchemaUnique;
							}
							if (xmlSchemaIdentityConstraint is XmlSchemaKey)
							{
								XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
								if (xmlSchemaKey.QualifiedName.Namespace == xmlSchemaKeyref.Refer.Namespace &&
									xmlSchemaKey.QualifiedName.Name == xmlSchemaKeyref.Refer.Name)
									return xmlSchemaKey;
							}
						}
					}
					else
					{
						foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in xmlSchemaElement.Constraints)
							if (xmlSchemaIdentityConstraint is XmlSchemaKey)
							{
								XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
								if (xmlSchemaKey.QualifiedName.Namespace == xmlSchemaKeyref.Refer.Namespace &&
									xmlSchemaKey.QualifiedName.Name == xmlSchemaKeyref.Refer.Name)
									return xmlSchemaKey;
							}
					}

				}

			return null;

		}

	}

}
