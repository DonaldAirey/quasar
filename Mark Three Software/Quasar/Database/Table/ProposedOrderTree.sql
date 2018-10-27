/*************************************************************************************************************************
*
*	File:			ProposedOrderTree.sql
*	Description:	Define the relationship between proposed orders.
*					Donald Roy Airey  Copyright (C) 2001 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"ProposedOrderTree"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "ProposedOrderTree"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('ProposedOrderTree', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "ProposedOrderTree"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "ProposedOrderTree" (
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
		ALTER TABLE "ProposedOrderTree" WITH NOCHECK ADD 
			CONSTRAINT "PKProposedOrderTree" PRIMARY KEY  CLUSTERED 
			(
				"ParentId",
				"ChildId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "ProposedOrderTree" ADD 
			CONSTRAINT "FKProposedOrderProposedOrderTreeChildId" FOREIGN KEY
			(
				"ChildId"
			) REFERENCES "ProposedOrder" (
				"ProposedOrderId"
			),
			CONSTRAINT "FKProposedOrderProposedOrderTreeParentId" FOREIGN KEY
			(
				"ParentId"
			) REFERENCES "ProposedOrder" (
				"ProposedOrderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end
	
	/* Defaults */

	execute ('
		ALTER TABLE "ProposedOrderTree" WITH NOCHECK ADD 
			CONSTRAINT "DefaultProposedOrderTreeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultProposedOrderTreeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end
	
	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''ProposedOrderTree''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "ProposedOrderTree"
go
