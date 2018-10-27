/*************************************************************************************************************************
*
*	File:			BlockOrder.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"BlockOrder"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "BlockOrder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('BlockOrder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "BlockOrder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "BlockOrder" (
			"BlockOrderId" "int" NOT NULL ,
			"BlotterId" "int" NOT NULL ,
			"AccountId" "int" NULL ,
			"SecurityId" "int" NULL ,
			"SettlementId" "int" NULL ,
			"BrokerId" "int" NULL ,
			"StatusCode" "int" NOT NULL ,
			"TransactionTypeCode" "int" NULL ,
			"TimeInForceCode" "int" NULL ,
			"OrderTypeCode" "int" NULL ,
			"ConditionCode" "int" NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsAgency" "bit" NOT NULL ,
			"QuantityExecuted" "decimal"(15, 4) NULL ,
			"QuantityOrdered" "decimal"(15, 4) NULL ,
			"Price1" "decimal"(15, 4) NULL ,
			"Price2" "decimal"(15, 4) NULL ,
			"CreatedTime" "datetime" NOT NULL ,
			"CreatedUserId" "int" NOT NULL ,
			"ModifiedTime" "datetime" NOT NULL ,
			"ModifiedUserId" "int" NOT NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "BlockOrder" WITH NOCHECK ADD 
			CONSTRAINT "PKBlockOrder" PRIMARY KEY  CLUSTERED 
			(
				"BlockOrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "BlockOrder" WITH NOCHECK ADD
			CONSTRAINT "DefaultBlockOrderIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBlockOrderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "BlockOrder" ADD 
			CONSTRAINT "FKBlotterBlockOrder" FOREIGN KEY 
			(
				"BlotterId"
			) REFERENCES "Blotter" (
				"BlotterId"
			),
			CONSTRAINT "FKStatusBlockOrder" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			),
			CONSTRAINT "FKUserBlockOrderCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserBlockOrderModifiedUserId" FOREIGN KEY 
			(
				"ModifiedUserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''BlockOrder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "BlockOrder"
go
