/*************************************************************************************************************************
*
*	File:			Archive.sql
*	Description:	Archive data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Archive"
*	Description:	Primary repository of Archive data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Archive"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Archive', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Archive"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Archive" (
			"TableName" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RecordId" "int" NOT NULL ,
			"Revision" "int" NOT NULL ,
			"ColumnName" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"DataType" "int" NOT NULL ,
			"BitValue" "bit" NULL ,
			"IntValue" "int" NULL ,
			"BitIntValue" "bigint" NULL ,
			"DecimalValue" "decimal"(32, 7) NULL ,
			"FloatValue" "float" NULL ,
			"StringValue" "varchar"(2048) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Archive" WITH NOCHECK ADD 
			CONSTRAINT "PKArchive" PRIMARY KEY  NONCLUSTERED 
			(
				"TableName",
				"Revision",
				"ColumnName"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Archive''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Archive"
go
