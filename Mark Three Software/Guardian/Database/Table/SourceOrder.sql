/*************************************************************************************************************************
*
*	File:			SourceOrder.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"SourceOrder"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "SourceOrder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('SourceOrder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "SourceOrder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "SourceOrder" (
			"CreatedTime" "dateTime" NOT NULL,
			"CreatedUserId" "int" NOT NULL,
			"DestinationId" "int" NULL,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"IsAdvertised" "bit" NOT NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsAutoExecute" "bit" NOT NULL,
			"IsCanceled" "bit" NOT NULL,
			"IsDeleted" "bit" NOT NULL ,
			"IsHeld" "bit" NOT NULL,
			"IsSteppedIn" "bit" NOT NULL,
			"IsSubmitted" "bit" NOT NULL,
			"LimitPrice" "decimal"(32, 7) NULL,
			"MaximumVolatility" "decimal"(32, 7) NULL,
			"ModifiedTime" "dateTime" NOT NULL,
			"ModifiedUserId" "int" NOT NULL,
			"NewsFreeTime" "int" NULL,
			"OrderTypeCode" "int" NOT NULL,
			"OrderedQuantity" "decimal"(32, 7) NOT NULL,
			"PriceTypeCode" "int" NOT NULL,
			"ReceivedTime" "dateTime" NULL,
			"RowVersion" "bigint" NOT NULL ,
			"SecurityId" "int" NOT NULL,
			"SettlementId" "int" NULL,
			"SourceOrderId" "int" NOT NULL,
			"StartTime" "dateTime" NULL,
			"StatusCode" "int" NOT NULL,
			"StopPrice" "decimal"(32, 7) NULL,
			"StopTime" "dateTime" NULL,
			"SubmittedQuantity" "decimal"(32, 7) NOT NULL ,
			"SubmittedTime" "dateTime" NULL,
			"TargetPrice" "decimal"(32, 7) NULL,
			"TimeInForceCode" "int" NOT NULL,
			"WorkingOrderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "SourceOrder" WITH NOCHECK ADD 
			CONSTRAINT "PKSourceOrder" PRIMARY KEY  CLUSTERED 
			(
				"SourceOrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "SourceOrder" WITH NOCHECK ADD
			CONSTRAINT "DefaultSourceOrderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultSourceOrderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "SourceOrder" ADD 
			CONSTRAINT "FKWorkingOrderSourceOrder" FOREIGN KEY 
			(
				"WorkingOrderId"
			) REFERENCES "WorkingOrder" (
				"WorkingOrderId"
			),
			CONSTRAINT "FKStatusSourceOrder" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			),
			CONSTRAINT "FKUserSourceOrderCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserSourceOrderModifiedUserId" FOREIGN KEY 
			(
				"ModifiedUserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''SourceOrder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "SourceOrder"
go
