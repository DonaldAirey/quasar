/*************************************************************************************************************************
*
*	File:			OrderTree.sql
*	Description:	Relationships between Orders for Securities.
*					Donald Roy Airey  Copyright (C) 2001 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"OrderTree"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "OrderTree"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('OrderTree', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "OrderTree"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "OrderTree" (
			"ParentId" "int" NOT NULL ,
			"ChildId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "OrderTree" WITH NOCHECK ADD 
			CONSTRAINT "PKOrderTree" PRIMARY KEY  CLUSTERED 
			(
				"ParentId",
				"ChildId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "OrderTree" ADD 
			CONSTRAINT "FKOrderOrderTreeChildId" FOREIGN KEY
			(
				"ChildId"
			) REFERENCES "Order" (
				"OrderId"
			),
			CONSTRAINT "FKOrderOrderTreeParentId" FOREIGN KEY
			(
				"ParentId"
			) REFERENCES "Order" (
				"OrderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end
	
	/* Defaults */

	execute ('
		ALTER TABLE "OrderTree" WITH NOCHECK ADD 
			CONSTRAINT "DefaultOrderTreeIsIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultOrderTreeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end
	
	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''OrderTree''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "OrderTree"
go
