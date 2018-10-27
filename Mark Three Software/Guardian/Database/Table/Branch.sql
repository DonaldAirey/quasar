/*************************************************************************************************************************
*
*	File:			Branch.sql
*	Description:	Contains information about broker where securities are traded.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Branch"
*	Description:	Contains information about the place where securites are traded.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Branch"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Branch', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Branch"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Branch" (
			"BranchId" "int" NOT NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"ShortName" "nvarchar"(16) NOT NULL,
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Branch" WITH NOCHECK ADD 
			CONSTRAINT "PKBranch" PRIMARY KEY  CLUSTERED 
			(
				"BranchId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Branch" ADD 
			CONSTRAINT "FKObjectBranch" FOREIGN KEY 
			(
				"BranchId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Branch" WITH NOCHECK ADD 
			CONSTRAINT "DefaultBranchIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultBranchIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Branch''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Branch"
go
 