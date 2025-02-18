namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// These are the parsing states used to read the arguments on the command line.
	/// </summary>
	enum ArgumentState { InputFileName, None, OutputFileName };

	/// <summary>
	/// This object will load the property table from a formatted file.
	/// </summary>
	class DatabaseGenerator
	{

		// Constants
		private const int defaultStringLength = 256;

		// Private Fields
		private static string inputFileName;
		private static string outputFileName;
		private static ArgumentState argumentState;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			try
			{

				// The command line parser is driven by different states that are triggered by the flags read.  Unless a flag has
				// been read, the command line parser assumes that it's reading the file name from the command line.
				argumentState = ArgumentState.InputFileName;

				// Parse the command line for arguments.
				foreach (string argument in args)
				{

					// Decode the current argument into a state change (or some other action).
					switch (argument)
					{
					case "-i":
						argumentState = ArgumentState.InputFileName;
						continue;
					case "-out":
						argumentState = ArgumentState.OutputFileName;
						continue;
					}

					// The parsing state will determine which variable is read next.
					switch (argumentState)
					{
					case ArgumentState.InputFileName:
						DatabaseGenerator.inputFileName = argument;
						break;
					case ArgumentState.OutputFileName:
						DatabaseGenerator.outputFileName = argument;
						break;
					}

					// The default state is to look for the input file name on the command line.
					argumentState = ArgumentState.InputFileName;

				}

				// Throw a usage message back at the user if no file name was given.
				if (inputFileName == null)
					throw new Exception("Usage: \"Database Generator\" -i <input file name> -out <DDL file>");

				// If no output file name was specified, create one from the input file specification.
				if (outputFileName == null)
					outputFileName = string.Format("{0}.sql", Path.GetFileNameWithoutExtension(inputFileName));

				// Read the schema into a string.  This emulates the way that the IDE would normally call a code generator.  Create
				// the MiddleTierSchema (like a Schema, but with extra helping functions and relations for this type of code generation).
				StreamReader streamReader = new StreamReader(inputFileName);
				// This will generate a buffer of source code from the input schema.
				byte[] buffer = GenerateDdl(new DataModelSchema(streamReader.ReadToEnd()));

				// Write the buffer to the specified output file.
				FileStream outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);
				outputStream.Write(buffer, 0, buffer.Length);
				outputStream.SetLength(buffer.Length);
				outputStream.Close();

			}
			catch (Exception exception)
			{

				Console.WriteLine(exception.Message);

			}

			return 0;

		}

		/// <summary>
		/// Generate the code from the inputs.
		/// </summary>
		/// <param name=\"inputFileName\">The name of the input file.</param>
		/// <param name=\"inputFileContent\">The contents of the input file.</param>
		/// <returns>A buffer containing the generated code.</returns>
		private static byte[] GenerateDdl(DataModelSchema schema)
		{

			// The generated data is written to a memeory stream using a writer.
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);

			// Generate the file header.
			streamWriter.WriteLine("/*******************************************************************************");
			streamWriter.WriteLine("*	<auto-generated>");
			streamWriter.WriteLine("*	This code was generated by a tool.");
			streamWriter.WriteLine("*	Runtime Version:1.0.0.0");
			streamWriter.WriteLine("*");
			streamWriter.WriteLine("*	Changes to this file may cause incorrect behavior and will be lost if");
			streamWriter.WriteLine("*	the code is regenerated.");
			streamWriter.WriteLine("*	</auto-generated>");
			streamWriter.WriteLine("*******************************************************************************/");
			streamWriter.WriteLine();
			streamWriter.WriteLine("/* Set the environment */");
			streamWriter.WriteLine("set nocount on");
			streamWriter.WriteLine("set quoted_identifier on");
			streamWriter.WriteLine();

			// If this is the first version, then generate the version control schema.
			if (schema.Version == 0.0m)
				GenerateVersionControl(streamWriter);

			// The tables must be written out to the DDL file so that parent tables are written out before child tables.  As tables
			// are emitted into the DDL file, they are removed from the list.  This continues until the list of tables is empty.
			TableSchemaCollection tables = schema.Tables;
			while (tables.Count > 0)
				foreach (TableSchema tableSchema in tables)
				{

					// This will search the tables to see if the current table has any parent dependancies that haven't been
					// written yet.
					bool isParentDefined = true;
					foreach (KeyrefSchema parentKeyref in tableSchema.ParentKeyrefs)
						if (tables.Contains(parentKeyref.Refer.Selector.QualifiedName))
						{
							isParentDefined = false;
							break;
						}

					// If there are parent dependancies that have not yet been generated, then skip this table for now.
					if (!isParentDefined)
						continue;

					// The table schema is removed from the list after it is written to the stream.
					GenerateTable(streamWriter, tableSchema);
					tables.Remove(tableSchema);
					break;

				}

			// This will flush all the generated code into an array of bytes that can be written back to a file.  The choice of a
			// byte array as a means of returning the generated data is intended to emulate the interface of the Visual Studio
			// tools.
			streamWriter.Flush();
			return memoryStream.ToArray();

		}

		/// <summary>
		/// Converts the system type into an equivalent Sql data type.
		/// </summary>
		/// <param name="type">Represents a system type declaration.</param>
		/// <returns>An equivalent SQL datatype.</returns>
		private static string GetSqlDataType(Type type)
		{

			// This will convert the system datatype into the SQL equivalent.
			switch (type.ToString())
			{
			case "System.Boolean":
				return "\"bit\"";
			case "System.Int16":
				return "\"smallint\"";
			case "System.Int32":
				return "\"int\"";
			case "System.Int64":
				return "\"bigint\"";
			case "System.Decimal":
				return "\"decimal\"";
			case "System.Single":
				return "\"real\"";
			case "System.Double":
				return "\"float\"";
			case "System.DateTime":
				return "\"datetime\"";
			case "System.String":
				return string.Format("\"nvarchar\"({0})", DatabaseGenerator.defaultStringLength);
			}

			// Failure to convert a data type generates an exception.
			throw new Exception(string.Format("The type {0} can't be converted to an SQL data type", type));

		}

		private static void GenerateVersionControl(StreamWriter streamWriter)
		{

			// The VersionControl table keeps track of the current revision of a table.
			streamWriter.WriteLine("/* The VersionControl table keeps track of the current revision of a table. */");
			streamWriter.WriteLine("begin transaction");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Drop the previous table */");
			streamWriter.WriteLine("if exists (select * from sysobjects where type = 'F' and name = 'VersionControlVersionTag')");
			streamWriter.WriteLine("	alter table \"VersionTag\" drop constraint \"VersionControlVersionTag\"");
			streamWriter.WriteLine("if exists (select * from sysobjects where type = 'U' and name = 'VersionControl')");
			streamWriter.WriteLine("	drop table \"VersionControl\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Create the table */");
			streamWriter.WriteLine("CREATE TABLE \"VersionControl\" (");
			streamWriter.WriteLine("	\"Name\" \"varchar\"(64) NOT NULL ,");
			streamWriter.WriteLine("	\"Revision\" \"decimal\" NOT NULL ");
			streamWriter.WriteLine(") ON \"PRIMARY\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Primary Clustered Index. */");
			streamWriter.WriteLine("ALTER TABLE \"VersionControl\" WITH NOCHECK ADD ");
			streamWriter.WriteLine("	CONSTRAINT \"KeyVersionControl\" PRIMARY KEY  CLUSTERED ");
			streamWriter.WriteLine("	(");
			streamWriter.WriteLine("		\"Name\"");
			streamWriter.WriteLine("	)  ON \"PRIMARY\" ");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* The table and keys can be committed now. */");
			streamWriter.WriteLine("commit transaction");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Write the status of the operation to the log. */");
			streamWriter.WriteLine("if @@error = 0");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionControl\", Created'");
			streamWriter.WriteLine("else");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionControl\", Error creating'");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");

			// The VersionHistory is a record of all the revisions in this database.
			streamWriter.WriteLine("/* The VersionHistory is a record of all the revisions in this database. */");
			streamWriter.WriteLine("begin transaction");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Drop the previous table */");
			streamWriter.WriteLine("if exists (select * from sysobjects where type = 'F' and name = 'VersionHistoryVersionTag')");
			streamWriter.WriteLine("	alter table \"VersionTag\" drop constraint \"VersionHistoryVersionTag\"");
			streamWriter.WriteLine("if exists (select * from sysobjects where type = 'U' and name = 'VersionHistory')");
			streamWriter.WriteLine("	drop table \"VersionHistory\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Create the table */");
			streamWriter.WriteLine("CREATE TABLE \"VersionHistory\" (");
			streamWriter.WriteLine("	\"Label\" \"sysname\" NOT NULL ,");
			streamWriter.WriteLine("	\"Date\" \"Datetime\" NULL ,");
			streamWriter.WriteLine("	\"Active\" \"bit\" NULL ");
			streamWriter.WriteLine(") ON \"PRIMARY\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Primary Clustered Index. */");
			streamWriter.WriteLine("ALTER TABLE \"VersionHistory\" WITH NOCHECK ADD ");
			streamWriter.WriteLine("	CONSTRAINT \"KeyVersionHistory\" PRIMARY KEY  CLUSTERED ");
			streamWriter.WriteLine("	(");
			streamWriter.WriteLine("		\"Label\"");
			streamWriter.WriteLine("	)  ON \"PRIMARY\" ");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* The table and keys can be committed now. */");
			streamWriter.WriteLine("commit transaction");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Write the status of the operation to the log. */");
			streamWriter.WriteLine("if @@error = 0");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionHistory\", Created'");
			streamWriter.WriteLine("else");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionHistory\", Error creating'");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");

			// The VersionTag is used to collect various revisions of tables into a single release.
			streamWriter.WriteLine("/* The VersionTag is used to collect various revisions of tables into a single release. */");
			streamWriter.WriteLine("begin transaction");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Drop the previous table */");
			streamWriter.WriteLine("if exists (select * from sysobjects where type = 'U' and name = 'VersionTag')");
			streamWriter.WriteLine("	drop table \"VersionTag\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Create the table */");
			streamWriter.WriteLine("CREATE TABLE \"VersionTag\" (");
			streamWriter.WriteLine("	\"Label\" \"sysname\" NOT NULL ,");
			streamWriter.WriteLine("	\"Name\" \"varchar\"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,");
			streamWriter.WriteLine("	\"Revision\" \"int\" NOT NULL ");
			streamWriter.WriteLine(") ON \"PRIMARY\"");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Primary Clustered Index. */");
			streamWriter.WriteLine("ALTER TABLE \"VersionTag\" WITH NOCHECK ADD ");
			streamWriter.WriteLine("	CONSTRAINT \"PKVersionTag\" PRIMARY KEY  CLUSTERED ");
			streamWriter.WriteLine("	(");
			streamWriter.WriteLine("		\"Label\",");
			streamWriter.WriteLine("		\"Name\"");
			streamWriter.WriteLine("	)  ON \"PRIMARY\" ");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Foreign keys */");
			streamWriter.WriteLine("ALTER TABLE \"VersionTag\" ADD ");
			streamWriter.WriteLine("	CONSTRAINT \"VersionControlVersionTag\" FOREIGN KEY ");
			streamWriter.WriteLine("	(");
			streamWriter.WriteLine("		\"Name\"");
			streamWriter.WriteLine("	) REFERENCES \"VersionControl\" (");
			streamWriter.WriteLine("		\"Name\"");
			streamWriter.WriteLine("	),");
			streamWriter.WriteLine("	CONSTRAINT \"VersionHistoryVersionTag\" FOREIGN KEY ");
			streamWriter.WriteLine("	(");
			streamWriter.WriteLine("		\"Label\"");
			streamWriter.WriteLine("	) REFERENCES \"VersionHistory\" (");
			streamWriter.WriteLine("		\"Label\"");
			streamWriter.WriteLine("	)");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* The table, indices and constraints can be committed now. */");
			streamWriter.WriteLine("commit transaction");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("/* Write the status of the operation to the log. */");
			streamWriter.WriteLine("if @@error = 0");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionTag\", Created'");
			streamWriter.WriteLine("else");
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"VersionTag\", Error creating'");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");

		}

		/// <summary>
		/// Generates the DDL for a table.
		/// </summary>
		/// <param name="streamWriter">The file to which the DDL is written.</param>
		/// <param name="tableSchema">The schema description of the table.</param>
		private static void GenerateTable(StreamWriter streamWriter, TableSchema tableSchema)
		{

			// Generate the prolog for this table.
			streamWriter.WriteLine("/* The {0} Table */", tableSchema.Name);

			// Generate a prolog showing the time of creation and the current version of the table for the log file.
			streamWriter.WriteLine("if not exists (select * from \"VersionControl\" where \"Name\" = '{0}')", tableSchema.Name);
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z <Undefined>: \"{0}\" doesn''t exist in the catalogs.'", tableSchema.Name);
			streamWriter.WriteLine("else");
			streamWriter.WriteLine("begin");
			streamWriter.WriteLine("	declare @revision \"decimal\"");
			streamWriter.WriteLine("	select @revision = \"Revision\" from \"VersionControl\" where \"Name\" = '{0}'", tableSchema.Name);
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"{0}\", Initial revision: ' + convert(varchar, @revision)", tableSchema.Name);
			streamWriter.WriteLine("end");
			streamWriter.WriteLine();

			// The table schema is only run if the version control table indicates that the update is needed.
			streamWriter.WriteLine("/* This checks the version control table to determine if an update is needed. */");
			streamWriter.WriteLine("declare @currentRevision \"decimal\"");
			streamWriter.WriteLine("declare @requiredRevision \"decimal\"");
			streamWriter.WriteLine("select @currentRevision = isnull((select \"Revision\" from \"VersionControl\" where \"VersionControl\".\"Name\" = '{0}'), -1.0)", tableSchema.Name);
			streamWriter.WriteLine("select @requiredRevision = isnull((select \"Revision\" from \"VersionHistory\", \"VersionTag\"");
			streamWriter.WriteLine("	where \"VersionHistory\".\"Active\" = 1 and \"VersionHistory\".\"Label\" = \"VersionTag\".\"Label\" and	\"VersionTag\".\"Name\" = '{0}'), {1})", tableSchema.Name, tableSchema.DataModelSchema.Version);
			streamWriter.WriteLine("if @currentRevision < {0} and {0} <= @requiredRevision", tableSchema.DataModelSchema.Version);
			streamWriter.WriteLine("begin");
			streamWriter.WriteLine("");

			// This transaction will remove the previous version of the table and create the new version.
			streamWriter.WriteLine("	/* The revision must be completed as a unit. */");
			streamWriter.WriteLine("	begin transaction");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("	/* Remove the object and any dependancies. */");
			foreach (KeyrefSchema childkeyref in tableSchema.ChildKeyrefs)
			{
				streamWriter.WriteLine("	if exists (select * from sysobjects where type = 'F' and name = '{0}')", childkeyref.Name);
				streamWriter.WriteLine("		alter table \"{0}\" drop constraint \"{1}\"", childkeyref.Selector.Name, childkeyref.Name);
			}
			streamWriter.WriteLine("	if exists (select * from sysobjects where type = 'U' and name = '{0}')", tableSchema.Name);
			streamWriter.WriteLine("		drop table \"{0}\"", tableSchema.Name);
			streamWriter.WriteLine("");

			// The table is described here.
			streamWriter.WriteLine("	/* Create the table. */");
			streamWriter.WriteLine(string.Format("	create table \"{0}\" (", tableSchema.Name));

			// Generate each of the column descriptions.
			foreach (ColumnSchema columnSchema in tableSchema.Columns)
				if (columnSchema.DeclaringType == tableSchema.TypeSchema)
					streamWriter.WriteLine(string.Format("		\"{0}\" {1} {2},", columnSchema.Name, GetSqlDataType(columnSchema.DataType),
					columnSchema.MinOccurs == 0 ? "null" : "not null"));

			// These columns are always included for house keeping.
			streamWriter.WriteLine("		\"IsArchived\" \"bit\" not null,");
			streamWriter.WriteLine("		\"IsDeleted\" \"bit\" not null,");
			streamWriter.WriteLine("		\"RowVersion\" \"bigint\" not null");

			// The table is always generated on the primary (default) device.
			streamWriter.WriteLine("	) on \"PRIMARY\"");
			streamWriter.WriteLine("");

			// Generate the keys, foreign keys and defaults on this table.
			GenerateKeys(streamWriter, tableSchema);
			GenerateIndices(streamWriter, tableSchema);
			GenerateForeignKeys(streamWriter, tableSchema);
			GenerateDefaults(streamWriter, tableSchema);

			// This will update the version control table to indicate that the table was updated to the new version.
			streamWriter.WriteLine("	/* Update the versionControl table to reflect the change. */");
			streamWriter.WriteLine("	if exists (select * from \"VersionControl\" where \"Name\" = '{0}')", tableSchema.Name);
			streamWriter.WriteLine("		update \"VersionControl\" set \"Revision\" = {0} where \"Name\" = '{1}'", tableSchema.DataModelSchema.Version, tableSchema.Name);
			streamWriter.WriteLine("	else");
			streamWriter.WriteLine("		insert \"VersionControl\" (\"Name\", \"Revision\") select '{0}', {1}", tableSchema.Name, tableSchema.DataModelSchema.Version);
			streamWriter.WriteLine("");

			// Commit or reject the changes to the table.
			streamWriter.WriteLine("	/* Commit the changes to the table. */");
			streamWriter.WriteLine("	commit transaction");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("end");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");

			// Generate the epilog for a successful update.
			streamWriter.WriteLine("if @@error = 0");
			streamWriter.WriteLine("begin");
			streamWriter.WriteLine("	declare @newRevision \"decimal\"");
			streamWriter.WriteLine("	select @newRevision = \"Revision\" from \"VersionControl\" where \"Name\" = '{0}'", tableSchema.Name);
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"{0}\", Final revision: ' + convert(varchar, @newRevision)", tableSchema.Name);
			streamWriter.WriteLine("end");
			streamWriter.WriteLine("else");
			streamWriter.WriteLine("begin");
			streamWriter.WriteLine("	declare @oldRevision \"decimal\"");
			streamWriter.WriteLine("	select @oldRevision = isnull((select \"Revision\" from \"VersionControl\" where \"Name\" = '{0}'), 0.0)", tableSchema.Name);
			streamWriter.WriteLine("	print convert(varchar, getdate(), 120) + 'Z Table: \"{0}\", Error upgrading from revision: ' + convert(varchar, @oldRevision)", tableSchema.Name);
			streamWriter.WriteLine("end");
			streamWriter.WriteLine("go");
			streamWriter.WriteLine("");

		}

		/// <summary>
		/// Generate the keys on a table.
		/// </summary>
		/// <param name="streamWriter">The file to which the DDL is written.</param>
		/// <param name="tableSchema">The schema description of the table.</param>
		private static void GenerateKeys(StreamWriter streamWriter, TableSchema tableSchema)
		{

			// This architecture foregoes the 'query' mechanism for finding records in favor of a more direct record identifier.
			// Every table has an implicit key created on the row identifier used to quickly find any given record.
			streamWriter.WriteLine("	/* Unique Constraints */");
			streamWriter.WriteLine("	alter table \"{0}\" with nocheck add ", tableSchema.Name);
			// This will add any additional keys to the table.
			for (int keyIndex = 0; keyIndex < tableSchema.Keys.Count; keyIndex++)
			{

				ConstraintSchema constraintSchema = tableSchema.Keys[keyIndex];

				if (constraintSchema.IsNullable)
					continue;

				if (constraintSchema.IsPrimaryKey)
					streamWriter.WriteLine("		constraint \"{0}\" primary key clustered", constraintSchema.Name);
				else
						streamWriter.WriteLine("		constraint \"{0}\" unique", constraintSchema.Name);
				streamWriter.WriteLine("		(");
				ColumnSchema[] keyFields = constraintSchema.Fields;
				for (int columnIndex = 0; columnIndex < keyFields.Length; columnIndex++)
				{
					ColumnSchema columnSchema = keyFields[columnIndex];
					streamWriter.Write("			\"{0}\"", columnSchema.Name);
					streamWriter.WriteLine(columnIndex == keyFields.Length - 1 ? "" : ",");
				}
				streamWriter.Write("		)  on \"PRIMARY\" ");

				// End with a comment if there are more foreign constraints comming.
				streamWriter.WriteLine(keyIndex < tableSchema.Keys.Count - 1 ? "," : "");

			}
			streamWriter.WriteLine("");

			// Roll the transaction back if the indices can't be created.
			streamWriter.WriteLine("");

		}

		/// <summary>
		/// Generate the keys on a table.
		/// </summary>
		/// <param name="streamWriter">The file to which the DDL is written.</param>
		/// <param name="tableSchema">The schema description of the table.</param>
		private static void GenerateIndices(StreamWriter streamWriter, TableSchema tableSchema)
		{

			bool isCommentEmitted = false;

			// This will add any additional keys to the table.
			foreach (ConstraintSchema constraintSchema in tableSchema.Keys)
			{

				if (!constraintSchema.IsNullable)
					continue;

				if (!isCommentEmitted)
				{

					isCommentEmitted = true;

					// This architecture foregoes the 'query' mechanism for finding records in favor of a more direct record identifier.
					// Every table has an implicit key created on the row identifier used to quickly find any given record.
					streamWriter.WriteLine("	/* Non-Unique Indices */");

				}

				streamWriter.WriteLine("	create index \"{0}\"", constraintSchema.Name);
				streamWriter.WriteLine("		on \"{0}\"", tableSchema.Name);
				streamWriter.WriteLine("		(");
				ColumnSchema[] keyFields = constraintSchema.Fields;
				for (int columnIndex = 0; columnIndex < keyFields.Length; columnIndex++)
				{
					ColumnSchema columnSchema = keyFields[columnIndex];
					streamWriter.Write("			\"{0}\"", columnSchema.Name);
					streamWriter.WriteLine(columnIndex == keyFields.Length - 1 ? "" : ",");
				}
				streamWriter.WriteLine("		)  on \"PRIMARY\" ");
				streamWriter.WriteLine("");

			}

		}

		/// <summary>
		/// Generates the foreign keys on a table.
		/// </summary>
		/// <param name="streamWriter">The file to which the DDL is written.</param>
		/// <param name="tableSchema">The schema description of the table.</param>
		private static void GenerateForeignKeys(StreamWriter streamWriter, TableSchema tableSchema)
		{

			// This will generate the foreign keys for the table.
			if (tableSchema.ParentKeyrefs.Count != 0)
			{
				streamWriter.WriteLine("	/* Foreign Keys */");
				streamWriter.WriteLine("	alter table \"{0}\" add ", tableSchema.Name);
				for (int foreignKeyIndex = 0; foreignKeyIndex < tableSchema.ParentKeyrefs.Count; foreignKeyIndex++)
				{
					KeyrefSchema keyrefSchema = tableSchema.ParentKeyrefs[foreignKeyIndex] as KeyrefSchema;
					streamWriter.WriteLine("		constraint \"{0}\" foreign key ", keyrefSchema.Name);
					streamWriter.WriteLine("		(");

					// Generate the child field names.
					ColumnSchema[] childFields = keyrefSchema.Fields;
					for (int columnIndex = 0; columnIndex < childFields.Length; columnIndex++)
					{
						ColumnSchema columnSchema = childFields[columnIndex];
						streamWriter.Write("			\"{0}\"", columnSchema.Name);
						streamWriter.WriteLine(columnIndex == childFields.Length - 1 ? "" : ",");
					}

					// Generate the parent 'refer' selector and field names.
					streamWriter.WriteLine("		) references \"{0}\" (", keyrefSchema.Refer.Selector.Name);
					ColumnSchema[] parentFields = keyrefSchema.Refer.Fields;
					for (int columnIndex = 0; columnIndex < childFields.Length; columnIndex++)
					{
						ColumnSchema columnSchema = parentFields[columnIndex];
						streamWriter.Write("			\"{0}\"", columnSchema.Name);
						streamWriter.WriteLine(columnIndex == childFields.Length - 1 ? "" : ",");
					}

					// End with a comment if there are more foreign constraints comming.
					streamWriter.WriteLine(foreignKeyIndex < tableSchema.ParentKeyrefs.Count - 1 ? "		)," : "		)");

				}

				// Roll the transaction back on errors.
				streamWriter.WriteLine("");

			}

		}

		/// <summary>
		/// Generates the defaults for a table.
		/// </summary>
		/// <param name="streamWriter">The file to which the DDL is written.</param>
		/// <param name="tableSchema">The schema description of the table.</param>
		private static void GenerateDefaults(StreamWriter streamWriter, TableSchema tableSchema)
		{

			// Every table has a flag for archived and deleted records that are not managed by the volatile data model.  The
			// defaults are provided for every table for these flags so they don't need to be provided with every 'insert' 
			// statement generated from the middle tier.
			streamWriter.WriteLine("	/* Defaults */");
			streamWriter.WriteLine("	alter table \"{0}\" with nocheck add ", tableSchema.Name);
			streamWriter.WriteLine("		constraint \"Default{0}IsArchived\" default (0) for \"IsArchived\",", tableSchema.Name);
			streamWriter.WriteLine("		constraint \"Default{0}IsDeleted\" default (0) for \"IsDeleted\"", tableSchema.Name);
			streamWriter.WriteLine("");

		}

	}

}
