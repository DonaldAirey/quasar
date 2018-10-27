/*************************************************************************************************************************
*
*	File:			ProposedOrder.sql
*	Description:	An order before it is given to a trader.  Allows a manager to see the impact of a trade before it
*					is sent to the trading desk.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ProposedOrder"
*	Description:	An order before it is given to a trader.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ProposedOrder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ProposedOrder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ProposedOrder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ProposedOrder" (
			"ProposedOrderId" "int" NOT NULL ,
			"AccountId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"SettlementId" "int" NOT NULL ,
			"BrokerId" "int" NULL ,
			"BlotterId" "int" NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"TransactionTypeCode" "int" NOT NULL ,
			"TimeInForceCode" "int" NOT NULL ,
			"OrderTypeCode" "int" NOT NULL ,
			"ConditionCode" "int" NULL ,
			"IsArchived" "bit" NOT NULL,
			"IsDeleted" "bit" NOT NULL ,
			"IsAgency" "bit" NOT NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Price1" "decimal"(15, 4) NULL ,
			"Price2" "decimal"(15, 4) NULL ,
			"Note" "nvarchar"(2048) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "ProposedOrder" WITH NOCHECK ADD 
			CONSTRAINT "PKProposedOrder" PRIMARY KEY  CLUSTERED 
			(
				"ProposedOrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "ProposedOrder" ADD 
			CONSTRAINT "FKPositionTypeProposedOrder" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			),
			CONSTRAINT "FKAccountProposedOrder" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityProposedOrderSecurityId" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKSecurityProposedOrderSettlementId" FOREIGN KEY 
			(
				"SettlementId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKTransactionTypeProposedOrder" FOREIGN KEY 
			(
				"TransactionTypeCode"
			) REFERENCES "TransactionType" (
				"TransactionTypeCode"
			),
			CONSTRAINT "FKOrderTypeProposedOrder" FOREIGN KEY 
			(
				"OrderTypeCode"
			) REFERENCES "OrderType" (
				"OrderTypeCode"
			),
			CONSTRAINT "FKTimeInForceProposedOrder" FOREIGN KEY 
			(
				"TimeInForceCode"
			) REFERENCES "TimeInForce" (
				"TimeInForceCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "ProposedOrder" WITH NOCHECK ADD
			CONSTRAINT "DefaultAgencyIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAgencyIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ProposedOrder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ProposedOrder"
go
