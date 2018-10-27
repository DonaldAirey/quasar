/*************************************************************************************************************************
*
*	File:			Order.sql
*	Description:	Orders for Securities.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Order"
*	Description:	Orders for Securities
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Order"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Order', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Order"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Order" (
			"OrderId" "int" NOT NULL ,
			"BlockOrderId" "int" NOT NULL ,
			"AccountId" "int" NOT NULL ,
			"SecurityId" "int" NOT NULL ,
			"SettlementId" "int" NOT NULL ,
			"BrokerId" "int" NULL ,
			"PositionTypeCode" "int" NOT NULL ,
			"TransactionTypeCode" "int" NOT NULL ,
			"TimeInForceCode" "int" NOT NULL ,
			"OrderTypeCode" "int" NOT NULL ,
			"ConditionCode" "int" NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsAgency" "bit" NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Price1" "decimal"(15, 4) NULL ,
			"Price2" "decimal"(15, 4) NULL ,
			"Note" "nvarchar"(2048) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
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
		ALTER TABLE "Order" WITH NOCHECK ADD 
			CONSTRAINT "PKOrder" PRIMARY KEY  CLUSTERED 
			(
				"OrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Order" ADD 
			CONSTRAINT "FKBlockOrderOrder" FOREIGN KEY 
			(
				"BlockOrderId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			),
			CONSTRAINT "FKPositionTypeOrder" FOREIGN KEY 
			(
				"PositionTypeCode"
			) REFERENCES "PositionType" (
				"PositionTypeCode"
			),
			CONSTRAINT "FKAccountOrder" FOREIGN KEY 
			(
				"AccountId"
			) REFERENCES "Account" (
				"AccountId"
			),
			CONSTRAINT "FKSecurityOrder" FOREIGN KEY 
			(
				"SecurityId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKCurrencyOrder" FOREIGN KEY 
			(
				"SettlementId"
			) REFERENCES "Security" (
				"SecurityId"
			),
			CONSTRAINT "FKTransactionTypeOrder" FOREIGN KEY 
			(
				"TransactionTypeCode"
			) REFERENCES "TransactionType" (
				"TransactionTypeCode"
			),
			CONSTRAINT "FKOrderTypeOrder" FOREIGN KEY 
			(
				"OrderTypeCode"
			) REFERENCES "OrderType" (
				"OrderTypeCode"
			),
			CONSTRAINT "FKTimeInForceCodeOrder" FOREIGN KEY 
			(
				"TimeInForceCode"
			) REFERENCES "TimeInForce" (
				"TimeInForceCode"
			),
			CONSTRAINT "FKUserOrderCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserOrderModifiedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Order" WITH NOCHECK ADD
			CONSTRAINT "DefaultOrderIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultOrderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Order''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Order"
go
