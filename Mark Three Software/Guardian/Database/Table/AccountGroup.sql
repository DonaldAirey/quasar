/*************************************************************************************************************************
*
*	File:			AccountGroup.sql
*	Description:	AccountGroup data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"AccountGroup"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "AccountGroup"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('AccountGroup', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "AccountGroup"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "AccountGroup" (
			"AccountGroupId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "AccountGroup" WITH NOCHECK ADD 
			CONSTRAINT "PKAccountGroup" PRIMARY KEY  NONCLUSTERED 
			(
				"AccountGroupId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "AccountGroup" ADD 
			CONSTRAINT "FKAccountBaseAccountGroup" FOREIGN KEY 
			(
				"AccountGroupId"
			) REFERENCES "AccountBase" (
				"AccountBaseId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "AccountGroup" WITH NOCHECK ADD 
			CONSTRAINT "DefaultAccountGroupIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultAccountGroupIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''AccountGroup''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "AccountGroup"
go
