/*************************************************************************************************************************
*
*	File:			FixMessage.sql
*	Description:	A message sent to an electronic exchange or broker.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"FixMessage"
*	Description:	A message sent to an electronic exchange or broker.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "FixMessage"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('FixMessage', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "FixMessage"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "FixMessage" (
			"FixMessageId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Tag" "int" NOT NULL ,
			"Value" "sql_variant" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "FixMessage" WITH NOCHECK ADD 
			CONSTRAINT "FixFixMessage" PRIMARY KEY  NONCLUSTERED 
			(
				"FixMessageId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "FixMessage" WITH NOCHECK ADD 
			CONSTRAINT "DefaultFixMessageIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultFixMessageIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''FixMessage''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "FixMessage"
go
