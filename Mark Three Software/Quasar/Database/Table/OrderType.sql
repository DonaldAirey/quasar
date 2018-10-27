/*************************************************************************************************************************
*
*	File:			OrderType.sql
*	Description:	Categories for Orders (Buy, Sell, Buy Cover, Sell Short, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"OrderType"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "OrderType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('OrderType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "OrderType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "OrderType" (
			"OrderTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "OrderType" WITH NOCHECK ADD 
			CONSTRAINT "PKOrderType" PRIMARY KEY  CLUSTERED 
			(
				"OrderTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "OrderType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultOrderTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultOrderTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''OrderType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "OrderType"
go
