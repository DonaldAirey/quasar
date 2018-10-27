/*************************************************************************************************************************
*
*	File:			SectorTarget.sql
*	Description:	Table containing the percentages of a given sector in a model.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"SectorTarget"
*	Description:	Primary repository of sectorTarget data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "SectorTarget"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('SectorTarget', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "SectorTarget"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "SectorTarget"
		(
			"ModelId" "int" NOT NULL ,
			"SectorId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Percent" "decimal"(15, 4) NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "SectorTarget" WITH NOCHECK ADD 
			CONSTRAINT "PKSectorTarget" PRIMARY KEY  CLUSTERED 
			(
				"ModelId",
				"SectorId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "SectorTarget" ADD 
			CONSTRAINT "FKModelSectorTarget" FOREIGN KEY 
			(
				"ModelId"
			) REFERENCES "Model" (
				"ModelId"
			),
			CONSTRAINT "FKSectorSectorTarget" FOREIGN KEY 
			(
				"SectorId"
			) REFERENCES "Sector" (
				"SectorId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "SectorTarget" WITH NOCHECK ADD 
			CONSTRAINT "DefaultSectorTargetIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultSectorTargetIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''SectorTarget''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "SectorTarget"
go
