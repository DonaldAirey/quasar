/*************************************************************************************************************************
*
*	File:			Allocation.sql
*	Description:	These records describe how a trade is allocated back to the original account or fund.  This record
*					is sent to the accounting system for reckonciliation.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Allocation"
*	Description:	allocation contains individual trades in a given security, account and long/short position.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Allocation"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Allocation', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Allocation"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Allocation" (
			"AllocationId" "int" NOT NULL ,
			"BlockOrderId" "int" NOT NULL ,
			"AccountId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"SettlementId" "int" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"TransactionTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Price" "decimal"(15, 4) NOT NULL ,
			"Commission" "decimal"(15, 4) NOT NULL ,
			"AccruedInterest" "decimal"(15, 4) NOT NULL ,
			"UserFee0" "decimal" NOT NULL ,
			"UserFee1" "decimal" NOT NULL ,
			"UserFee2" "decimal" NOT NULL ,
			"UserFee3" "decimal" NOT NULL ,
			"TradeDate" "Datetime" NOT NULL ,
			"SettlementDate" "Datetime" NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"CreatedTime" "datetime" NOT NULL ,
			"CreatedUserId" "int" NULL ,
			"ModifiedTime" "datetime" NOT NULL ,
			"ModifiedUserId" "int" NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Allocation" WITH NOCHECK ADD 
			CONSTRAINT "PKAllocation" PRIMARY KEY  NONCLUSTERED 
			(
				"AllocationId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Allocation" ADD 
			CONSTRAINT "FKBlockOrderAllocation" FOREIGN KEY 
			(
				"BlockOrderId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			),
			CONSTRAINT "FKAccountAllocation" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityAllocationSecurityId" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKSecurityAllocationSettlementId" FOREIGN KEY 
			(
				"SettlementId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKPositionTypeAllocation" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Allocation" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAllocationIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAllocationIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Allocation''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end

/* Print the final state for the log. */

execute "ObjectEpilog" "Allocation"
go
