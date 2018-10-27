/*************************************************************************************************************************
*
*	File:			SecurityType.sql
*	Description:	Categories for Securities.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"SecurityType"
*	Description:	Categories for Securities.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "SecurityType"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('SecurityType', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "SecurityType"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "SecurityType" (
			"SecurityTypeCode" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"Description" "nvarchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
			"ExternalId0" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId1" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId2" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"ExternalId3" "varchar"(32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "SecurityType" WITH NOCHECK ADD 
			CONSTRAINT "PKSecurityType" PRIMARY KEY  CLUSTERED 
			(
				"SecurityTypeCode"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "SecurityType" WITH NOCHECK ADD 
			CONSTRAINT "DefaultSecurityTypeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultSecurityTypeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''SecurityType''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "SecurityType"
go
