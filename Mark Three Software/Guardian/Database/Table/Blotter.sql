/*************************************************************************************************************************
*
*	File:			Blotter.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright (C) 2001 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Blotter"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Blotter"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Blotter', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Blotter"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Blotter" (
			"AdvertisementStylesheetId" "int" NULL ,
			"BlotterId" "int" NOT NULL ,
			"DestinationOrderDetailStylesheetId" "int" NULL ,
			"DestinationOrderStylesheetId" "int" NULL ,
			"ExecutionDetailStylesheetId" "int" NULL ,
			"ExecutionStylesheetId" "int" NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"MatchStylesheetId" "int" NULL ,
			"MatchHistoryStylesheetId" "int" NULL ,
			"PartyTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SourceOrderDetailStylesheetId" "int" NULL ,
			"SourceOrderStylesheetId" "int" NULL ,
			"WorkingOrderStylesheetId" "int" NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Blotter" WITH NOCHECK ADD 
			CONSTRAINT "PKBlotter" PRIMARY KEY  CLUSTERED 
			(
				"BlotterId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Blotter" ADD 
			CONSTRAINT "FKObjectBlotter" FOREIGN KEY 
			(
				"BlotterId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Blotter" WITH NOCHECK ADD 
			CONSTRAINT "DefaultBlotterIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBlotterIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Blotter''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Blotter"
go
