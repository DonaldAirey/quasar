/*************************************************************************************************************************
*
*	File:			Timer.sql
*	Description:	Major Asset category for Security
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Timer"
*	Description:	Major Asset category for Security
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Timer"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Timer', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Timer"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Timer" (
			"CurrentTime" "datetime" NOT NULL ,
			"ExternalId0" "varchar"(128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
			"IsActive" "bit" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"StopTime" "datetime" NULL ,
			"TimerId" "int" NOT NULL ,
			"UpdateTime" "int" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Timer" WITH NOCHECK ADD 
			CONSTRAINT "PKTimer" PRIMARY KEY  CLUSTERED 
			(
				"TimerId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Timer" WITH NOCHECK ADD 
			CONSTRAINT "DefaultTimerIsActive" DEFAULT (0) FOR "IsActive",
			CONSTRAINT "DefaultTimerIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultTimerIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Timer''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Timer"
go
