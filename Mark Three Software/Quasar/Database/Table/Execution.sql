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
			"ExecutionId" "int" NOT NULL ,
			"BlockOrderId" "int" NOT NULL ,
			"BrokerId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Price" "decimal"(15, 4) NOT NULL,
			"Commission" "decimal"(15, 4) NOT NULL ,
			"AccruedInterest" "decimal"(15, 4) NOT NULL ,
			"UserFee0" "decimal"(15, 4) NOT NULL ,
			"UserFee1" "decimal"(15, 4) NOT NULL ,
			"UserFee2" "decimal"(15, 4) NOT NULL ,
			"UserFee3" "decimal"(15, 4) NOT NULL ,
			"TradeDate" "datetime" NOT NULL ,
			"SettlementDate" "datetime" NOT NULL ,
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
			CONSTRAINT "DefaultExecutionIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultExecutionIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Execution" ADD 
			CONSTRAINT "FKBlockOrderExecution" FOREIGN KEY 
			(
				"BlockOrderId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			),
			CONSTRAINT "FKBrokerExecution" FOREIGN KEY 
			(
				"BrokerId"
			) REFERENCES "Broker" (
				"BrokerId"
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
