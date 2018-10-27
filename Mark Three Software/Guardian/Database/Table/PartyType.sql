/*************************************************************************************************************************
*
*	File:			PartyType.sql
*	Description:	Categories for Orders (Buy, Sell, Buy Cover, Sell Short, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"PartyType"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "PartyType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('PartyType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "PartyType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "PartyType" (
			"Description" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"PartyTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "PartyType" WITH NOCHECK ADD 
			CONSTRAINT "PKPartyType" PRIMARY KEY  CLUSTERED 
			(
				"PartyTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "PartyType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPartyTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPartyTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''PartyType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "PartyType"
go
