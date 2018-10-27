/*************************************************************************************************************************
*
*	File:			Exchange.sql
*	Description:	Contains information about exchange where security are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Exchange"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Exchange"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Exchange', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Exchange"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Exchange" (
			"ExchangeId" "int" NOT NULL ,
			"ExternalId0" "varchar" (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar" (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar" (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar" (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Name" "varchar" (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Exchange" WITH NOCHECK ADD 
			CONSTRAINT "PKExchange" PRIMARY KEY  CLUSTERED 
			(
				"ExchangeId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Exchange" WITH NOCHECK ADD 
			CONSTRAINT "DefaultExchangeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultExchangeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Exchange''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Exchange"
go
