/*************************************************************************************************************************
*
*	File:			Broker.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Broker"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Broker"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Broker', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Broker"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Broker" (
			"BrokerId" "int" NOT NULL ,
			"Connected" "bit" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Phone" "varchar"(64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"Symbol" "varchar"(8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Broker" WITH NOCHECK ADD 
			CONSTRAINT "PKBroker" PRIMARY KEY  CLUSTERED 
			(
				"BrokerId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Broker" ADD 
			CONSTRAINT "FKObjectBroker" FOREIGN KEY 
			(
				"BrokerId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Broker" WITH NOCHECK ADD 
			CONSTRAINT "DefaultBrokerIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBrokerIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Broker''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Broker"
go
