/*************************************************************************************************************************
*
*	File:			Sector.sql
*	Description:	Primary source of information about sub-sectors, sectors and sector groups.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Sector"
*	Description:	Primary repository of sector data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Sector"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Sector', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Sector"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Sector" (
			"SectorId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"SortOrder" "int" NOT NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Sector" WITH NOCHECK ADD 
			CONSTRAINT "PKSector" PRIMARY KEY  CLUSTERED 
			(
				"SectorId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Sector" ADD 
			CONSTRAINT "FKObjectSector" FOREIGN KEY 
			(
				"SectorId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Sector" WITH NOCHECK ADD 
			CONSTRAINT "DefaultSectorIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultSectorIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Sector''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Sector"
go
