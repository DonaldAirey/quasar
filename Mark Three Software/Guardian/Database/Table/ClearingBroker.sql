/*************************************************************************************************************************
*
*	File:			ClearingBroker.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ClearingBroker"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ClearingBroker"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ClearingBroker', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ClearingBroker"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ClearingBroker" (
			"ClearingBrokerId" "int" NOT NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "ClearingBroker" WITH NOCHECK ADD 
			CONSTRAINT "PKClearingBroker" PRIMARY KEY  CLUSTERED 
			(
				"ClearingBrokerId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "ClearingBroker" WITH NOCHECK ADD 
			CONSTRAINT "DefaultClearingBrokerIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultClearingBrokerIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ClearingBroker''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ClearingBroker"
go
  