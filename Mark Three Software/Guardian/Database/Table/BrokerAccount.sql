/*************************************************************************************************************************
*
*	File:			BrokerAccount.sql
*	Description:	Major Asset category for Security
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"BrokerAccount"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "BrokerAccount"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('BrokerAccount', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "BrokerAccount"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "BrokerAccount" (
			"BrokerAccountId" "int" NOT NULL ,
			"BrokerId" "int" NOT NULL ,
			"Description" "nvarchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Mnemonic" "nvarchar"(16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"TraderId" "int" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "BrokerAccount" WITH NOCHECK ADD 
			CONSTRAINT "PKBrokerAccount" PRIMARY KEY  CLUSTERED 
			(
				"BrokerAccountId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "BrokerAccount" WITH NOCHECK ADD 
			CONSTRAINT "DefaultBrokerAccountIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBrokerAccountIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''BrokerAccount''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "BrokerAccount"
go
