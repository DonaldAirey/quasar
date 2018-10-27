/*************************************************************************************************************************
*
*	File:			Match.sql
*	Description:	Matched Working Orders
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Match"
*	Description:	Contains Matching Working Orders.
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Match"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Match', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Match"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Match" (
			"ContraOrderId" "int" NOT NULL ,
			"CreatedTime" "dateTime" NOT NULL,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"MatchId" "int" NOT NULL ,
			"RowVersion" "bigint" NOT NULL ,
			"StatusCode" "int" NOT NULL ,
			"TimerId" "int" NULL ,
			"WorkingOrderId" "int" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Match" WITH NOCHECK ADD 
			CONSTRAINT "KeyMatch" PRIMARY KEY  CLUSTERED 
			(
				"MatchId"
			)  ON "PRIMARY" 
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Foreign Key */

	execute ('
		ALTER TABLE "Match" ADD 
			CONSTRAINT "FKWorkingOrderMatchWorkingOrderId" FOREIGN KEY 
			(
				"WorkingOrderId"
			) REFERENCES "WorkingOrder" (
				"WorkingOrderId"
			),
			CONSTRAINT "FKWorkingOrderMatchContraOrderId" FOREIGN KEY 
			(
				"ContraOrderId"
			) REFERENCES "WorkingOrder" (
				"WorkingOrderId"
			),
			CONSTRAINT "FKStatusMatch" FOREIGN KEY 
			(
				"StatusCode"
			) REFERENCES "Status" (
				"StatusCode"
			)
	')

	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Match" WITH NOCHECK ADD 
			CONSTRAINT "DefaultMatchIsArchived" DEFAULT (0) FOR "IsArchived" ,
			CONSTRAINT "DefaultMatchIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Match''')

	if @@error <> 0 begin rollback transaction return end

	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Match"
go
 