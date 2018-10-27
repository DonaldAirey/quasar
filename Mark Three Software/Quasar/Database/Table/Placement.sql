/*************************************************************************************************************************
*
*	File:			Placement.sql
*	Description:	Information about a placement of an order with a broker.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Placement"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Placement"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Placement', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Placement"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Placement" (
			"PlacementId" "int" NOT NULL ,
			"BlockOrderId" "int" NOT NULL ,
			"BrokerId" "int" NOT NULL ,
			"TimeInForceCode" "int" NOT NULL ,
			"OrderTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"IsRouted" "bit" NOT NULL ,
			"Quantity" "decimal"(15, 4) NOT NULL ,
			"Price1" "decimal"(15, 4) NULL ,
			"Price2" "decimal"(15, 4) NULL ,
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
		ALTER TABLE "Placement" WITH NOCHECK ADD 
			CONSTRAINT "PKPlacement" PRIMARY KEY  CLUSTERED 
			(
				"PlacementId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Placement" WITH NOCHECK ADD 
			CONSTRAINT "DefaultPlacementIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultPlacementIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Placement" ADD 
			CONSTRAINT "FKBlockOrderPlacement" FOREIGN KEY 
			(
				"BlockOrderId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			),
			CONSTRAINT "FKBrokerPlacement" FOREIGN KEY 
			(
				"BrokerId"
			) REFERENCES "Broker" (
				"BrokerId"
			),
			CONSTRAINT "FKTimeInForcePlacement" FOREIGN KEY 
			(
				"TimeInForceCode"
			) REFERENCES "TimeInForce" (
				"TimeInForceCode"
			),
			CONSTRAINT "FKOrderTypePlacement" FOREIGN KEY 
			(
				"OrderTypeCode"
			) REFERENCES "OrderType" (
				"OrderTypeCode"
			),
			CONSTRAINT "FKUserPlacementCreatedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			),
			CONSTRAINT "FKUserPlacementModifiedUserId" FOREIGN KEY 
			(
				"CreatedUserId"
			) REFERENCES "User" (
				"UserId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Placement''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Placement"
go
