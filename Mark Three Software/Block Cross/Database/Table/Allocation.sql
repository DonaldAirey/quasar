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
			"AccountId" "int" NOT NULL ,
			"AccruedInterest" "decimal"(28, 7) NOT NULL ,
			"AllocationId" "int" NOT NULL ,
			"Commission" "decimal"(28, 7) NOT NULL ,
			"CreatedTime" "datetime" NOT NULL ,
			"CreatedUserId" "int" NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"ModifiedTime" "datetime" NOT NULL ,
			"ModifiedUserId" "int" NULL ,
			"OrderTypeCode" "int" NOT NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"Price" "decimal"(28, 7) NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Quantity" "decimal"(28, 7) NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"SettlementDate" "Datetime" NOT NULL ,
			"SettlementId" "int" NOT NULL ,
			"TradeDate" "Datetime" NOT NULL ,
			"UserFee0" "decimal"(32, 7) NOT NULL ,
			"UserFee1" "decimal"(32, 7) NOT NULL ,
			"UserFee2" "decimal"(32, 7) NOT NULL ,
			"UserFee3" "decimal"(32, 7) NOT NULL ,
			"WorkingOrderId" "int" NOT NULL
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
			CONSTRAINT "FKWorkingOrderAllocation" FOREIGN KEY 
			(
				"WorkingOrderId"
			) REFERENCES "WorkingOrder" (
				"WorkingOrderId"
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
			),
			CONSTRAINT "FKOrderTypeAllocation" FOREIGN KEY 
			(
				"OrderTypeCode"
			) REFERENCES "OrderType" (
				"OrderTypeCode"
			)

	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Allocation" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAllocationIsArchived" DEFAULT (0) FOR "IsArchived",
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
