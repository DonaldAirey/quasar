/*************************************************************************************************************************
*
*	File:			BlockOrderTree.sql
*	Description:	Maintains the relationship between blocked orders.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"BlockOrderTree"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "BlockOrderTree"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('BlockOrderTree', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "BlockOrderTree"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "BlockOrderTree" (
			"ParentId" "int" NOT NULL ,
			"ChildId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "BlockOrderTree" WITH NOCHECK ADD 
			CONSTRAINT "PKBlockOrderTree" PRIMARY KEY  CLUSTERED 
			(
				"ParentId",
				"ChildId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "BlockOrderTree" ADD 
			CONSTRAINT "FKBlockOrderBlockOrderTreeParentId" FOREIGN KEY 
			(
				"ParentId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			),
			CONSTRAINT "FKBlockOrderBlockOrderTreeChildId" FOREIGN KEY 
			(
				"ChildId"
			) REFERENCES "BlockOrder" (
				"BlockOrderId"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''BlockOrderTree''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "BlockOrderTree"
go
