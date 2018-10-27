/*************************************************************************************************************************
*
*	File:			Execution.sql
*	Description:	Information about execution of trades.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Execution"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Execution"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Execution', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Execution"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Execution" (
			"AccruedInterest" "decimal"(28, 7) NOT NULL ,
			"BrokerId" "int" NULL ,
			"BrokerAccountId" "int" NULL ,
			"Commission" "decimal"(28, 7) NOT NULL ,
			"CreatedTime" "datetime" NOT NULL ,
			"CreatedUserId" "int" NULL ,
			"DestinationOrderId" "int" NOT NULL ,
			"DestinationStateCode" "int" NOT NULL ,
			"ExecutionId" "int" NOT NULL ,
			"ExecutionPrice" "decimal"(28, 7) NOT NULL,
			"ExecutionQuantity" "decimal"(28, 7) NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
			"FixMessageId" "int" NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsHidden" "bit" NOT NULL ,
			"ModifiedTime" "datetime" NOT NULL ,
			"ModifiedUserId" "int" NULL ,
			"OriginalDestinationOrderId" "int" NULL ,
			"OriginalPrice" "decimal"(28, 7) NULL,
			"OriginalQuantity" "decimal"(28, 7) NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"SettlementDate" "datetime" NOT NULL ,
			"SourceExecutionId" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"SourceStateCode" "int" NOT NULL ,
			"TradeDate" "datetime" NOT NULL ,
			"UserFee0" "decimal"(28, 7) NOT NULL ,
			"UserFee1" "decimal"(28, 7) NOT NULL ,
			"UserFee2" "decimal"(28, 7) NOT NULL ,
			"UserFee3" "decimal"(28, 7) NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Execution" WITH NOCHECK ADD 
			CONSTRAINT "PKExecution" PRIMARY KEY  CLUSTERED 
			(
				"ExecutionId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Execution" WITH NOCHECK ADD 
			CONSTRAINT "DefaultExecutionIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultExecutionIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Execution" ADD 
			CONSTRAINT "FKDestinationOrderExecution" FOREIGN KEY 
			(
				"DestinationOrderId"
			) REFERENCES "DestinationOrder" (
				"DestinationOrderId"
			),
			CONSTRAINT "FKUserExecutionCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserExecutionModifiedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			)

	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Execution''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Execution"
go
