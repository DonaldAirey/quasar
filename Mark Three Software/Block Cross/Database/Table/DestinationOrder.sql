/*************************************************************************************************************************
*
*	File:			DestinationOrder.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"DestinationOrder"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "DestinationOrder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('DestinationOrder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "DestinationOrder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "DestinationOrder" (
			"CanceledQuantity" "decimal"(32, 7) NOT NULL,
			"CanceledTime" "dateTime" NULL,
			"CreatedTime" "dateTime" NOT NULL,
			"CreatedUserId" "int" NOT NULL,
			"DestinationId" "int" NOT NULL,
			"DestinationOrderId" "int" NOT NULL,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsCanceledByUser" "bit" NOT NULL,
			"IsDeleted" "bit" NOT NULL ,
			"IsHidden" "bit" NOT NULL,
			"LimitPrice" "decimal"(32, 7) NULL,
			"ModifiedTime" "dateTime" NOT NULL,
			"ModifiedUserId" "int" NOT NULL,
			"OrderTypeCode" "int" NOT NULL ,
			"OrderedQuantity" "decimal"(32, 7) NOT NULL,
			"PriceTypeCode" "int" NOT NULL,
			"RowVersion" "bigint" NOT NULL ,
			"StateCode" "int" NOT NULL,
			"StatusCode" "int" NOT NULL,
			"StopPrice" "decimal"(32, 7) NULL,
			"TraderId" "int" NOT NULL,
			"TimeInForceCode" "int" NOT NULL,
			"WorkingOrderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "DestinationOrder" WITH NOCHECK ADD 
			CONSTRAINT "PKDestinationOrder" PRIMARY KEY  CLUSTERED 
			(
				"DestinationOrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "DestinationOrder" ADD 
			CONSTRAINT "FKSWorkingOrderDestinationOrder" FOREIGN KEY 
			(
				"WorkingOrderId"
			) REFERENCES "WorkingOrder" (
				"WorkingOrderId"
			),
			CONSTRAINT "FKStateDestinationOrder" FOREIGN KEY 
			(
				"StateCode"
			) REFERENCES "State" (
				"StateCode"
			),
			CONSTRAINT "FKStatusDestinationOrder" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			),
			CONSTRAINT "FKUserDestinationOrderCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserDestinationOrderModifiedUserId" FOREIGN KEY 
			(
				"ModifiedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKTraderDestinationOrder" FOREIGN KEY 
			(
				"TraderId"
			) REFERENCES "Trader" (
				"TraderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "DestinationOrder" WITH NOCHECK ADD
			CONSTRAINT "DefaultDestinationOrderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultDestinationOrderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''DestinationOrder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "DestinationOrder"
go
