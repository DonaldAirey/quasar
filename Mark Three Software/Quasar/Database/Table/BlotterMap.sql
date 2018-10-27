/*************************************************************************************************************************
*
*	File:			BlotterMap.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright (C) 2001 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"BlotterMap"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "BlotterMap"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('BlotterMap', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "BlotterMap"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "BlotterMap" (
			"BlotterMapId" "int" NOT NULL ,
			"BlotterId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"MinimumQuantity" "decimal"(15, 4) NOT NULL ,
			"MaximumQuantity" "decimal"(15, 4) NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "BlotterMap" WITH NOCHECK ADD 
			CONSTRAINT "PKBlotterMap" PRIMARY KEY  CLUSTERED 
			(
				"BlotterMapId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "BlotterMap" ADD 
			CONSTRAINT "FKSecurityBlotterMap" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKBlotterBlotterMap" FOREIGN KEY 
			(
				"BlotterId"
			) REFERENCES "Blotter" (
				"BlotterId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "BlotterMap" WITH NOCHECK ADD 
			CONSTRAINT "DefaultBlotterMapIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBlotterMapIsIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''BlotterMap''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "BlotterMap"
go
