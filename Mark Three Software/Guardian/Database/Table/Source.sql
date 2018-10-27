/*************************************************************************************************************************
*
*	File:			Source.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Source"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Source"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Source', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Source"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Source" (
			"BuyMarketValueThreshold" "decimal"(32, 7) NULL ,
			"BuyQuantityThreshold" "decimal"(32, 7) NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SellMarketValueThreshold" "decimal"(32, 7) NULL ,
			"SellQuantityThreshold" "decimal"(32, 7) NULL ,
			"ShortName" "nvarchar"(16) NOT NULL ,
			"SourceId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Source" WITH NOCHECK ADD 
			CONSTRAINT "PKSource" PRIMARY KEY  CLUSTERED 
			(
				"SourceId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Source" ADD 
			CONSTRAINT "FKObjectSource" FOREIGN KEY 
			(
				"SourceId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Source" WITH NOCHECK ADD 
			CONSTRAINT "DefaultSourceIsDeleted" DEFAULT (0) FOR "IsDeleted",
			CONSTRAINT "DefaultSourceIsArchived" DEFAULT (0) FOR "IsArchived"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Source''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Source"
go
  