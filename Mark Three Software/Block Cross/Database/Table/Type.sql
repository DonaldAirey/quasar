/*************************************************************************************************************************
*
*	File:			Type.sql
*	Description:	Categories for Objects
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Type"
*	Description:	This table is used to qualify various the various object used in this database.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Type"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Type', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Type"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Type" (
			"Description" "nvarchar"(1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Specification" "varchar"(1024) NOT NULL ,
			"TypeCode" "varchar"(128) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Type" WITH NOCHECK ADD 
			CONSTRAINT "PKType" PRIMARY KEY  CLUSTERED 
			(
				"TypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Type" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Types''')
	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Type"
go
