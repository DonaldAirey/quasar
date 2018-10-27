/*************************************************************************************************************************
*
*	File:			OrderType.sql
*	Description:	Categories for Transactions (Buy, Sell, Sell Short, Buy Cover, Deposit, Widthdrawl, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"OrderType"
*	Description:	Categories for Transactions.
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
			"CashSign" "decimal"(28, 7) NOT NULL,
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16)COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"OrderTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"QuantitySign" "decimal"(28, 7) NOT NULL
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
