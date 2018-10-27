/*************************************************************************************************************************
*
*	File:			PositionType.sql
*	Description:	Position types tell us if a holding is long or short
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"PositionType"
*	Description:	Used to differentiate between long and short holdings of the same security.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "PositionType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('PositionType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "PositionType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "PositionType" (
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Sign" "decimal"(28, 7) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "PositionType" WITH NOCHECK ADD 
			CONSTRAINT "PKPositionType" PRIMARY KEY  CLUSTERED 
			(
				"PositionTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "PositionType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPositionTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPositionTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''PositionType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "PositionType"
go
