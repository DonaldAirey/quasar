/*************************************************************************************************************************
*
*	File:			ObjectTree.sql
*	Description:	Hierarchical organization for objects
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ObjectTree"
*	Description:	Maps parent objectTree to children for a hierarchy of objectTree.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ObjectTree"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ObjectTree', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ObjectTree"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ObjectTree" (
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"ParentId" "int" NOT NULL ,
			"ChildId" "int" NOT NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')
		
	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "ObjectTree" WITH NOCHECK ADD 
			CONSTRAINT "PKObjectTree" PRIMARY KEY  CLUSTERED 
			(
				"ParentId",
				"ChildId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "ObjectTree" ADD 
			CONSTRAINT "FKObjectObjectTreeChildId" FOREIGN KEY 
			(
				"ChildId"
			) REFERENCES "Object" (
				"ObjectId"
			),
			CONSTRAINT "FKObjectObjectTreeParentId" FOREIGN KEY 
			(
				"ParentId"
			) REFERENCES "Object" (
				"ObjectId"
			)
	')
	
	/* Defaults */

	execute ('
		ALTER TABLE "ObjectTree" WITH NOCHECK ADD 
			CONSTRAINT "DefaultObjectTreeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultObjectTreeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end
	
	/* Update the "VersionControl" to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ObjectTree''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ObjectTree"
go
