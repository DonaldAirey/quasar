/*************************************************************************************************************************
*
*	File:			Status.sql
*	Description:	Status codes for assorted objects.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Status"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Status"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Status', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Status"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Status" (
			"Description" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16)  COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"StatusCode" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Status" WITH NOCHECK ADD 
			CONSTRAINT "PKStatus" PRIMARY KEY  CLUSTERED 
			(
				"StatusCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Status" WITH NOCHECK ADD 
			CONSTRAINT "DefaultStatusIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultStatusIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Status''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Status"
go
