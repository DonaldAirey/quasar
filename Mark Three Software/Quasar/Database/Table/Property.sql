/*************************************************************************************************************************
*
*	File:			Property.sql
*	description:	Properties of objects.
*					Donald Roy Airey  Copyright (C) 2001 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Property"
*	name:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Property"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Property', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Property"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Property" (
			"PropertyCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"ExternalId2" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"ExternalId3" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Property" WITH NOCHECK ADD 
			CONSTRAINT "PKProperty" PRIMARY KEY  CLUSTERED 
			(
				"PropertyCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Property" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPropertyIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPropertyIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Property''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Property"
go
