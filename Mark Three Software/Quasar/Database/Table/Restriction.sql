/*************************************************************************************************************************
*
*	File:			Restriction.sql
*	Description:	Restriction are a way of identifying one set of compliance violation from another.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Restriction"
*	Description:	Restriction are a way of identifying one set of compliance violation from another.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Restriction"
go

/* Version 0.1 */

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Restriction', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Restriction"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Restriction" (
			"RestrictionId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL,
			"IsDeleted" "bit" NOT NULL ,
			"Severity" "int" NOT NULL ,
			"Approval" "int" NOT NULL ,
			"Description" "nvarchar"(2048) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Clustered Index. */

	execute ('
		ALTER TABLE "Restriction" WITH NOCHECK ADD 
			CONSTRAINT "PKRestriction" PRIMARY KEY  CLUSTERED 
			(
				"RestrictionId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Restriction" WITH NOCHECK ADD 
			CONSTRAINT "DefaultRestrictionIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultRestrictionIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Restriction''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "Restriction"
go
