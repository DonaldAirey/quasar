/*************************************************************************************************************************
*
*	File:			WorkingOrder.sql
*	Description:	Account data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"WorkingOrder"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "WorkingOrder"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('WorkingOrder', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "WorkingOrder"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "WorkingOrder" (
			"AutomaticQuantity" "decimal"(32, 7) NULL,
			"BlotterId" "int" NOT NULL,
			"CreatedTime" "dateTime" NOT NULL,
			"CreatedUserId" "int" NOT NULL,
			"DestinationId" "int" NULL,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"IsAgencyMatch" "bit" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsAutomatic" "bit" NOT NULL ,
			"IsAwake" "bit" NOT NULL ,
			"IsBrokerMatch" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsHedgeMatch" "bit" NOT NULL ,
			"IsInstitutionMatch" "bit" NOT NULL ,
			"LimitPrice" "decimal"(32, 7) NULL,
			"MaximumVolatility" "decimal"(32, 7) NULL,
			"ModifiedTime" "dateTime" NOT NULL,
			"ModifiedUserId" "int" NOT NULL,
			"NewsFreeTime" "int" NULL ,
			"OrderTypeCode" "int" NOT NULL,
			"PriceTypeCode" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SecurityId" "int" NOT NULL,
			"SettlementId" "int" NULL,
			"StartTime" "dateTime" NULL , 
			"StatusCode" "int" NOT NULL,
			"StopPrice" "decimal"(32, 7) NULL,
			"StopTime" "dateTime" NULL , 
			"SubmissionTypeCode" "int" NOT NULL ,
			"SubmittedQuantity" "decimal"(32, 7) NOT NULL,
			"SubmittedTime" "dateTime" NULL,
			"TimeInForceCode" "int" NOT NULL,
			"TimerId" "int" NULL ,
			"UploadedTime" "dateTime" NULL ,
			"WorkingOrderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "WorkingOrder" WITH NOCHECK ADD 
			CONSTRAINT "PKWorkingOrder" PRIMARY KEY  CLUSTERED 
			(
				"WorkingOrderId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "WorkingOrder" WITH NOCHECK ADD
			CONSTRAINT "DefaultWorkingOrderIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultWorkingOrderIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "WorkingOrder" ADD 
			CONSTRAINT "FKBlotterWorkingOrder" FOREIGN KEY 
			(
				"BlotterId"
			) REFERENCES "Blotter" (
				"BlotterId"
			),
			CONSTRAINT "FKStatusWorkingOrder" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			),
			CONSTRAINT "FKUserWorkingOrderCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserWorkingOrderModifiedUserId" FOREIGN KEY 
			(
				"ModifiedUserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''WorkingOrder''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "WorkingOrder"
go
