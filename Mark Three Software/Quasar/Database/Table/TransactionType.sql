/*************************************************************************************************************************
*
*	File:			TransactionType.sql
*	Description:	Categories for Transactions (Buy, Sell, Sell Short, Buy Cover, Deposit, Widthdrawl, etc.)
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"TransactionType"
*	Description:	Categories for Transactions.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "TransactionType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('TransactionType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "TransactionType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "TransactionType" (
			"TransactionTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"QuantitySign" "decimal"(16, 4) NOT NULL ,
			"CashSign" "decimal"(16, 4) NOT NULL,
			"Mnemonic" "nvarchar"(6)COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
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
		ALTER TABLE "TransactionType" WITH NOCHECK ADD 
			CONSTRAINT "PKTransactionType" PRIMARY KEY  CLUSTERED 
			(
				"TransactionTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "TransactionType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTransactionTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTransactionTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''TransactionType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "TransactionType"
go
