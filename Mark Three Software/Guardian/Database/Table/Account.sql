/*************************************************************************************************************************
*
*	File:			Employee.sql
*	Description:	Employee data
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

/* Set the Environment */

set nocount on
set quoted_identifier on

/*************************************************************************************************************************
*	Table:			"Employee"
*	Description:	Primary repository of account data
*************************************************************************************************************************/

/* Print the initial state for the log */

execute "ObjectProlog" "Employee"
go

/* Revision 1 */

if ("dbo"."RevisionNeeded" ('Employee', 1) = 1)
begin

	/* The revision must be completed as a unit. */

	begin transaction

	/* Remove the object and any dependancies. */

	execute "RemoveObject" "Employee"

	if @@error <> 0 begin rollback transaction return end

	/* Create the table. */

	execute ('
		CREATE TABLE "Employee" (
			"EmployeeId" "int" NOT NULL ,
			"IsArchived" "bit" NOT NULL ,
			"IsDeleted" "bit" NOT NULL ,
			"LotHandlingCode" "int" NULL ,
			"RowVersion" "bigint" NOT NULL
		) ON "PRIMARY"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Primary Key */

	execute ('
		ALTER TABLE "Employee" WITH NOCHECK ADD 
			CONSTRAINT "PKEmployee" PRIMARY KEY  NONCLUSTERED 
			(
				"EmployeeId"
			)  ON "PRIMARY" 
	')

	if @@error <> 0 begin rollback transaction return end

	/* Foreign Keys */

	execute ('
		ALTER TABLE "Employee" ADD 
			CONSTRAINT "FKOEmployeeBaseEmployee" FOREIGN KEY 
			(
				"EmployeeId"
			) REFERENCES "EmployeeBase" (
				"EmployeeBaseId"
			),
			CONSTRAINT "FKLotHandlingEmployee" FOREIGN KEY 
			(
				"LotHandlingCode"
			) REFERENCES "LotHandling" (
				"LotHandlingCode"
			)
	')
	
	if @@error <> 0 begin rollback transaction return end

	/* Defaults */

	execute ('
		ALTER TABLE "Employee" WITH NOCHECK ADD 
			CONSTRAINT "DefaultEmployeeIsArchived" DEFAULT (0) FOR "IsArchived",
			CONSTRAINT "DefaultEmployeeIsDeleted" DEFAULT (0) FOR "IsDeleted"
	')

	if @@error <> 0 begin rollback transaction return end

	/* Update the versionControl table to reflect the change. */

	execute ('Update "VersionControl" set "Revision" = 1 where "Name" = ''Employee''')
	if @@error <> 0 begin rollback transaction return end
	
	/* At this point, the revision is complete. */

	commit transaction

end
go

/* Print the final state for the log. */

execute "ObjectEpilog" "Employee"
go
